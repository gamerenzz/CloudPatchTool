using System;
using System.IO;
using System.Text;

namespace CloudPatchTool
{
    /// <summary>
    /// 云管家 patch 文件加解密核心逻辑
    /// </summary>
    public static class PatchCrypto
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("dongyl");
        private static readonly byte[] Header = Encoding.UTF8.GetBytes("zip99991");
        private const int BufferSize = 64 * 1024; // 64 KB 缓冲区

        /// <summary>
        /// 校验文件是否为 PATCH 格式
        /// </summary>
        public static bool IsPatchFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length < Header.Length) return false;

                byte[] headerBuffer = new byte[Header.Length];
                int read = fs.Read(headerBuffer, 0, headerBuffer.Length);
                if (read != Header.Length) return false;

                for (int i = 0; i < Header.Length; i++)
                {
                    if (headerBuffer[i] != Header[i]) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 校验文件是否为标准 ZIP 格式
        /// </summary>
        public static bool IsZipFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length < 4) return false;

                byte[] magic = new byte[4];
                if (fs.Read(magic, 0, 4) != 4) return false;

                // PK\x03\x04 (标准) | PK\x05\x06 (空包) | PK\x07\x08 (分卷)
                return magic[0] == 0x50 && magic[1] == 0x4B &&
                       ((magic[2] == 0x03 && magic[3] == 0x04) ||
                        (magic[2] == 0x05 && magic[3] == 0x06) ||
                        (magic[2] == 0x07 && magic[3] == 0x08));
            }
        }

        /// <summary>
        /// ZIP -> PATCH（流式加密）
        /// </summary>
        public static void EncryptZipFile(string zipPath, string patchPath)
        {
            if (!File.Exists(zipPath))
                throw new FileNotFoundException("找不到 ZIP 文件。", zipPath);

            if (!IsZipFile(zipPath))
                throw new InvalidDataException("输入的文件不是有效的 ZIP 格式。");

            using (FileStream inStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream outStream = new FileStream(patchPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                // 1. 写入 Header: zip99991
                outStream.Write(Header, 0, Header.Length);

                // 2. 流式异或加密写入
                byte[] buffer = new byte[BufferSize];
                long totalProcessed = 0;
                int bytesRead;

                while ((bytesRead = inStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ Key[(totalProcessed + i) % Key.Length]);
                    }
                    outStream.Write(buffer, 0, bytesRead);
                    totalProcessed += bytesRead;
                }
            }
        }

        /// <summary>
        /// PATCH -> ZIP（流式解密）
        /// </summary>
        public static void DecryptPatchFile(string patchPath, string zipPath)
        {
            if (!File.Exists(patchPath))
                throw new FileNotFoundException("找不到 PATCH 文件。", patchPath);

            if (!IsPatchFile(patchPath))
                throw new InvalidDataException("不是有效的云管家 .patch 补丁文件（Header 不匹配）。");

            using (FileStream inStream = new FileStream(patchPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream outStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                // 跳过前 8 字节 Header
                inStream.Seek(Header.Length, SeekOrigin.Begin);

                byte[] buffer = new byte[BufferSize];
                long totalProcessed = 0;
                int bytesRead;

                while ((bytesRead = inStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ Key[(totalProcessed + i) % Key.Length]);
                    }
                    outStream.Write(buffer, 0, bytesRead);
                    totalProcessed += bytesRead;
                }
            }

            // 解密完成后校验是否有效
            if (!IsZipFile(zipPath))
            {
                // 校验失败删除半成品
                try { File.Delete(zipPath); } catch { }
                throw new InvalidDataException("解密完成但数据无法识别为 ZIP，可能文件已损坏或密钥已被修改。");
            }
        }
    }
}
