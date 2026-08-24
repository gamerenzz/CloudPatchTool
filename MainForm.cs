using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CloudPatchTool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeDragDrop();
            SetStatus("等待文件……\r\n支持直接拖入文件，或点击按钮选择。");
        }

        /// <summary>
        /// 初始化所有控件的拖放支持
        /// </summary>
        private void InitializeDragDrop()
        {
            Control[] dropControls = new Control[] { this, pnlDrop, btnSelect, lblTitle, lblHint, lblStatus, lblFormat };

            foreach (var ctrl in dropControls)
            {
                ctrl.AllowDrop = true;
                ctrl.DragEnter += Control_DragEnter;
                ctrl.DragDrop += Control_DragDrop;
            }
        }

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length == 1)
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files == null || files.Length != 1)
            {
                SetStatus("提示：一次只能处理一个文件。");
                return;
            }

            _ = ProcessFileAsync(files[0]);
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "选择 .patch 或 .zip 文件";
                dialog.Filter = "PATCH / ZIP 文件 (*.patch;*.zip)|*.patch;*.zip|所有文件 (*.*)|*.*";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _ = ProcessFileAsync(dialog.FileName);
                }
            }
        }

        /// <summary>
        /// 异步处理文件转换
        /// </summary>
        private async Task ProcessFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                SetStatus("错误：文件不存在！");
                return;
            }

            SetBusyState(true);

            try
            {
                string ext = Path.GetExtension(filePath).ToLowerInvariant();
                string dir = Path.GetDirectoryName(filePath);
                string nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);

                if (ext == ".patch" || PatchCrypto.IsPatchFile(filePath))
                {
                    // 执行解密
                    string outputPath = GetUniquePath(dir, nameWithoutExt, ".zip");
                    SetStatus($"正在解密 PATCH……\r\n文件：{Path.GetFileName(filePath)}");

                    await Task.Run(() => PatchCrypto.DecryptPatchFile(filePath, outputPath));

                    FileInfo info = new FileInfo(outputPath);
                    SetStatus($"解密成功！\r\n输出：{Path.GetFileName(outputPath)}\r\n大小：{FormatFileSize(info.Length)}");
                    MessageBox.Show(this, $"PATCH 解密成功！\r\n\r\n输出文件：\r\n{outputPath}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ext == ".zip" || PatchCrypto.IsZipFile(filePath))
                {
                    // 执行加密
                    string outputPath = GetUniquePath(dir, nameWithoutExt, ".patch");
                    SetStatus($"正在加密 ZIP……\r\n文件：{Path.GetFileName(filePath)}");

                    await Task.Run(() => PatchCrypto.EncryptZipFile(filePath, outputPath));

                    FileInfo info = new FileInfo(outputPath);
                    SetStatus($"加密成功！\r\n输出：{Path.GetFileName(outputPath)}\r\n大小：{FormatFileSize(info.Length)}");
                    MessageBox.Show(this, $"ZIP 加密成功！\r\n\r\n输出文件：\r\n{outputPath}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    throw new InvalidDataException("无法识别文件格式。请确保是有效的 .patch 或 .zip 文件。");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"处理失败：\r\n{ex.Message}");
                MessageBox.Show(this, ex.Message, "处理失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusyState(false);
            }
        }

        private void SetBusyState(bool isBusy)
        {
            btnSelect.Enabled = !isBusy;
            Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
        }

        private static string GetUniquePath(string directory, string fileNameWithoutExtension, string extension)
        {
            string path = Path.Combine(directory, fileNameWithoutExtension + extension);
            if (!File.Exists(path)) return path;

            int index = 1;
            while (true)
            {
                path = Path.Combine(directory, $"{fileNameWithoutExtension}_{index}{extension}");
                if (!File.Exists(path)) return path;
                index++;
            }
        }

        private void SetStatus(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatus), text);
                return;
            }
            lblStatus.Text = text;
        }

        private static string FormatFileSize(long size)
        {
            if (size < 1024) return size + " B";
            if (size < 1024 * 1024) return (size / 1024.0).ToString("0.00") + " KB";
            if (size < 1024L * 1024L * 1024L) return (size / (1024.0 * 1024.0)).ToString("0.00") + " MB";
            return (size / (1024.0 * 1024.0 * 1024.0)).ToString("0.00") + " GB";
        }
    }
}
