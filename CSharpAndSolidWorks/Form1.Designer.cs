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
            this.BtnGetPartData = new System.Windows.Forms.Button();
            this.Btn_ChangeDim = new System.Windows.Forms.Button();
            this.Btn_Traverse_Feature = new System.Windows.Forms.Button();
            this.Btn_Traverse_Comp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(102, 31);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "1.连接SolidWorks";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // btnOpenAndNew
            // 
            this.btnOpenAndNew.Location = new System.Drawing.Point(12, 58);
            this.btnOpenAndNew.Name = "btnOpenAndNew";
            this.btnOpenAndNew.Size = new System.Drawing.Size(102, 30);
            this.btnOpenAndNew.TabIndex = 1;
            this.btnOpenAndNew.Text = "2.打开和创建";
            this.btnOpenAndNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenAndNew.UseVisualStyleBackColor = true;
            this.btnOpenAndNew.Click += new System.EventHandler(this.BtnOpenAndNew_Click);
            // 
            // BtnGetPartData
            // 
            this.BtnGetPartData.Location = new System.Drawing.Point(12, 103);
            this.BtnGetPartData.Name = "BtnGetPartData";
            this.BtnGetPartData.Size = new System.Drawing.Size(102, 31);
            this.BtnGetPartData.TabIndex = 2;
            this.BtnGetPartData.Text = "3.读取零件属性";
            this.BtnGetPartData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnGetPartData.UseVisualStyleBackColor = true;
            this.BtnGetPartData.Click += new System.EventHandler(this.BtnGetPartData_Click);
            // 
            // Btn_ChangeDim
            // 
            this.Btn_ChangeDim.BackColor = System.Drawing.SystemColors.Control;
            this.Btn_ChangeDim.Location = new System.Drawing.Point(12, 154);
            this.Btn_ChangeDim.Name = "Btn_ChangeDim";
            this.Btn_ChangeDim.Size = new System.Drawing.Size(102, 29);
            this.Btn_ChangeDim.TabIndex = 3;
            this.Btn_ChangeDim.Text = "4.修改零件";
            this.Btn_ChangeDim.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_ChangeDim.UseVisualStyleBackColor = true;
            this.Btn_ChangeDim.Click += new System.EventHandler(this.Btn_ChangeDim_Click);
            // 
            // Btn_Traverse_Feature
            // 
            this.Btn_Traverse_Feature.Location = new System.Drawing.Point(12, 199);
            this.Btn_Traverse_Feature.Name = "Btn_Traverse_Feature";
            this.Btn_Traverse_Feature.Size = new System.Drawing.Size(102, 29);
            this.Btn_Traverse_Feature.TabIndex = 4;
            this.Btn_Traverse_Feature.Text = "5.遍历零件特征";
            this.Btn_Traverse_Feature.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Traverse_Feature.UseVisualStyleBackColor = true;
            this.Btn_Traverse_Feature.Click += new System.EventHandler(this.Btn_Traverse_Feature_Click);
            // 
            // Btn_Traverse_Comp
            // 
            this.Btn_Traverse_Comp.Location = new System.Drawing.Point(135, 12);
            this.Btn_Traverse_Comp.Name = "Btn_Traverse_Comp";
            this.Btn_Traverse_Comp.Size = new System.Drawing.Size(108, 31);
            this.Btn_Traverse_Comp.TabIndex = 5;
            this.Btn_Traverse_Comp.Text = "6.遍历装配体";
            this.Btn_Traverse_Comp.UseVisualStyleBackColor = true;
            this.Btn_Traverse_Comp.Click += new System.EventHandler(this.Btn_Traverse_Comp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 238);
            this.Controls.Add(this.Btn_Traverse_Comp);
            this.Controls.Add(this.Btn_Traverse_Feature);
            this.Controls.Add(this.Btn_ChangeDim);
            this.Controls.Add(this.BtnGetPartData);
            this.Controls.Add(this.btnOpenAndNew);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "Solidworks二次开发 API";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnOpenAndNew;
        private System.Windows.Forms.Button BtnGetPartData;
        private System.Windows.Forms.Button Btn_ChangeDim;
        private System.Windows.Forms.Button Btn_Traverse_Feature;
        private System.Windows.Forms.Button Btn_Traverse_Comp;
    }
}

