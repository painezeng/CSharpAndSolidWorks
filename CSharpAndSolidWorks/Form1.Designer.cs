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
            this.btn_Traverse_Drawing = new System.Windows.Forms.Button();
            this.btn_InsertPart = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnInsertLibF = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(112, 31);
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
            this.btnOpenAndNew.Size = new System.Drawing.Size(112, 30);
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
            this.BtnGetPartData.Size = new System.Drawing.Size(112, 31);
            this.BtnGetPartData.TabIndex = 2;
            this.BtnGetPartData.Text = "3.读取零件属性";
            this.BtnGetPartData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BtnGetPartData.UseVisualStyleBackColor = true;
            this.BtnGetPartData.Click += new System.EventHandler(this.BtnGetPartData_Click);
            // 
            // Btn_ChangeDim
            // 
            this.Btn_ChangeDim.BackColor = System.Drawing.SystemColors.Control;
            this.Btn_ChangeDim.Location = new System.Drawing.Point(12, 153);
            this.Btn_ChangeDim.Name = "Btn_ChangeDim";
            this.Btn_ChangeDim.Size = new System.Drawing.Size(112, 29);
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
            this.Btn_Traverse_Feature.Size = new System.Drawing.Size(112, 29);
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
            this.Btn_Traverse_Comp.Size = new System.Drawing.Size(122, 31);
            this.Btn_Traverse_Comp.TabIndex = 5;
            this.Btn_Traverse_Comp.Text = "6.遍历装配体";
            this.Btn_Traverse_Comp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Traverse_Comp.UseVisualStyleBackColor = true;
            this.Btn_Traverse_Comp.Click += new System.EventHandler(this.Btn_Traverse_Comp_Click);
            // 
            // btn_Traverse_Drawing
            // 
            this.btn_Traverse_Drawing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Traverse_Drawing.Location = new System.Drawing.Point(136, 58);
            this.btn_Traverse_Drawing.Name = "btn_Traverse_Drawing";
            this.btn_Traverse_Drawing.Size = new System.Drawing.Size(121, 30);
            this.btn_Traverse_Drawing.TabIndex = 6;
            this.btn_Traverse_Drawing.Text = "7.遍历视图与球标";
            this.btn_Traverse_Drawing.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Traverse_Drawing.UseVisualStyleBackColor = true;
            this.btn_Traverse_Drawing.Click += new System.EventHandler(this.btn_Traverse_Drawing_Click);
            // 
            // btn_InsertPart
            // 
            this.btn_InsertPart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_InsertPart.Location = new System.Drawing.Point(135, 103);
            this.btn_InsertPart.Name = "btn_InsertPart";
            this.btn_InsertPart.Size = new System.Drawing.Size(121, 30);
            this.btn_InsertPart.TabIndex = 6;
            this.btn_InsertPart.Text = "8.装配零件";
            this.btn_InsertPart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_InsertPart.UseVisualStyleBackColor = true;
            this.btn_InsertPart.Click += new System.EventHandler(this.btn_InsertPart_Click);
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Location = new System.Drawing.Point(136, 153);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(121, 30);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "9.导出x_t/Dwg";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnInsertLibF
            // 
            this.btnInsertLibF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertLibF.Location = new System.Drawing.Point(135, 198);
            this.btnInsertLibF.Name = "btnInsertLibF";
            this.btnInsertLibF.Size = new System.Drawing.Size(121, 30);
            this.btnInsertLibF.TabIndex = 6;
            this.btnInsertLibF.Text = "10.插入库特征";
            this.btnInsertLibF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInsertLibF.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 238);
            this.Controls.Add(this.btnInsertLibF);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btn_InsertPart);
            this.Controls.Add(this.btn_Traverse_Drawing);
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
        private System.Windows.Forms.Button btn_Traverse_Drawing;
        private System.Windows.Forms.Button btn_InsertPart;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnInsertLibF;
    }
}

