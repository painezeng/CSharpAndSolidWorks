namespace CSharpAndSolidWorks
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnOpenAndNew = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(23, 28);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(89, 31);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "测试连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // btnOpenAndNew
            // 
            this.btnOpenAndNew.Location = new System.Drawing.Point(23, 74);
            this.btnOpenAndNew.Name = "btnOpenAndNew";
            this.btnOpenAndNew.Size = new System.Drawing.Size(89, 30);
            this.btnOpenAndNew.TabIndex = 1;
            this.btnOpenAndNew.Text = "打开和创建";
            this.btnOpenAndNew.UseVisualStyleBackColor = true;
            this.btnOpenAndNew.Click += new System.EventHandler(this.btnOpenAndNew_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 256);
            this.Controls.Add(this.btnOpenAndNew);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "Solidworks二次开发";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnOpenAndNew;
    }
}

