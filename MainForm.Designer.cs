namespace CloudPatchTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlDrop;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label lblFormat;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlDrop = new System.Windows.Forms.Panel();
            this.btnSelect = new System.Windows.Forms.Button();
            this.lblFormat = new System.Windows.Forms.Label();
            this.pnlDrop.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(167, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "云管家补丁工具";
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Font = new System.Drawing.Font("微软雅黑", 9.5F);
            this.lblHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblHint.Location = new System.Drawing.Point(33, 58);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(201, 19);
            this.lblHint.TabIndex = 1;
            this.lblHint.Text = "PATCH ⇄ ZIP 互相转换（免安装）";
            // 
            // pnlDrop
            // 
            this.pnlDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDrop.Controls.Add(this.btnSelect);
            this.pnlDrop.Location = new System.Drawing.Point(30, 90);
            this.pnlDrop.Name = "pnlDrop";
            this.pnlDrop.Size = new System.Drawing.Size(460, 160);
            this.pnlDrop.TabIndex = 2;
            // 
            // btnSelect
            // 
            this.btnSelect.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.btnSelect.Location = new System.Drawing.Point(120, 55);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(220, 50);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "选择或拖入 .patch / .zip";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblFormat.ForeColor = System.Drawing.Color.Gray;
            this.lblFormat.Location = new System.Drawing.Point(30, 260);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(284, 17);
            this.lblFormat.TabIndex = 3;
            this.lblFormat.Text = "支持功能：.patch → .zip   |   .zip → .patch";
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblStatus.Location = new System.Drawing.Point(30, 290);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(8);
            this.lblStatus.Size = new System.Drawing.Size(460, 95);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "等待文件……";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 410);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.pnlDrop);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "云管家补丁工具 (.NET 4.7.2)";
            this.pnlDrop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
