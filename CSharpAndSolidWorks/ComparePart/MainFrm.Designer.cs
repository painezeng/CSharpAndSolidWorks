namespace ComparePart
{
    partial class MainFrm
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
            this.txtLocal = new System.Windows.Forms.TextBox();
            this.txtSendtoCustomer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.butCompare = new System.Windows.Forms.Button();
            this.butsetcolor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtLocal
            // 
            this.txtLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocal.Location = new System.Drawing.Point(118, 12);
            this.txtLocal.Name = "txtLocal";
            this.txtLocal.Size = new System.Drawing.Size(518, 20);
            this.txtLocal.TabIndex = 0;
            this.txtLocal.Text = "D:\\09_Study\\CSharpAndSolidWorks\\CSharpAndSolidWorks\\TemplateModel\\Measure.SLDPRT";
            // 
            // txtSendtoCustomer
            // 
            this.txtSendtoCustomer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSendtoCustomer.Location = new System.Drawing.Point(118, 39);
            this.txtSendtoCustomer.Name = "txtSendtoCustomer";
            this.txtSendtoCustomer.Size = new System.Drawing.Size(518, 20);
            this.txtSendtoCustomer.TabIndex = 1;
            this.txtSendtoCustomer.Text = "D:\\09_Study\\CSharpAndSolidWorks\\CSharpAndSolidWorks\\TemplateModel\\Measure_defeatu" +
    "re.SLDPRT";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "本地文件:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "发给客户的文件:";
            // 
            // butCompare
            // 
            this.butCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCompare.Location = new System.Drawing.Point(561, 81);
            this.butCompare.Name = "butCompare";
            this.butCompare.Size = new System.Drawing.Size(75, 23);
            this.butCompare.TabIndex = 3;
            this.butCompare.Text = "比较";
            this.butCompare.UseVisualStyleBackColor = true;
            this.butCompare.Click += new System.EventHandler(this.butCompare_Click);
            // 
            // butsetcolor
            // 
            this.butsetcolor.Location = new System.Drawing.Point(466, 81);
            this.butsetcolor.Name = "butsetcolor";
            this.butsetcolor.Size = new System.Drawing.Size(75, 23);
            this.butsetcolor.TabIndex = 6;
            this.butsetcolor.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(223, 39);
            this.label3.TabIndex = 5;
            this.label3.Text = "结果文件中：透明的部分是一样的地方。\r\n红色： 发给客户的零件中缺少此实体。\r\n蓝色： 发给客户的零件中多了此实体。\r\n";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 118);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.butsetcolor);
            this.Controls.Add(this.butCompare);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSendtoCustomer);
            this.Controls.Add(this.txtLocal);
            this.Name = "MainFrm";
            this.Text = "比较文件";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLocal;
        private System.Windows.Forms.TextBox txtSendtoCustomer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button butCompare;
        private System.Windows.Forms.Button butsetcolor;
        private System.Windows.Forms.Label label3;
    }
}

