namespace CSharpAndSolidWorks
{
    partial class Btn_Filter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Btn_Filter));
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
            this.btnFilter = new System.Windows.Forms.Button();
            this.btn_DeleteConstraints = new System.Windows.Forms.Button();
            this.btnSelectNamedFace = new System.Windows.Forms.Button();
            this.Btn_T_sketchsegment = new System.Windows.Forms.Button();
            this.btn_ThridData = new System.Windows.Forms.Button();
            this.btn_LoadThrid = new System.Windows.Forms.Button();
            this.btn_Tips = new System.Windows.Forms.Button();
            this.btn_Adv_Select = new System.Windows.Forms.Button();
            this.btnBounding = new System.Windows.Forms.Button();
            this.btn_Measure = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(118, 31);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "1.连接SolidWorks";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // btnOpenAndNew
            // 
            this.btnOpenAndNew.Location = new System.Drawing.Point(12, 59);
            this.btnOpenAndNew.Name = "btnOpenAndNew";
            this.btnOpenAndNew.Size = new System.Drawing.Size(118, 31);
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
            this.BtnGetPartData.Size = new System.Drawing.Size(118, 31);
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
            this.Btn_ChangeDim.Size = new System.Drawing.Size(118, 31);
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
            this.Btn_Traverse_Feature.Size = new System.Drawing.Size(118, 31);
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
            this.Btn_Traverse_Comp.Size = new System.Drawing.Size(118, 31);
            this.Btn_Traverse_Comp.TabIndex = 5;
            this.Btn_Traverse_Comp.Text = "6.遍历装配体";
            this.Btn_Traverse_Comp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_Traverse_Comp.UseVisualStyleBackColor = true;
            this.Btn_Traverse_Comp.Click += new System.EventHandler(this.Btn_Traverse_Comp_Click);
            // 
            // btn_Traverse_Drawing
            // 
            this.btn_Traverse_Drawing.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Traverse_Drawing.Location = new System.Drawing.Point(136, 59);
            this.btn_Traverse_Drawing.Name = "btn_Traverse_Drawing";
            this.btn_Traverse_Drawing.Size = new System.Drawing.Size(118, 31);
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
            this.btn_InsertPart.Size = new System.Drawing.Size(118, 31);
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
            this.btnExport.Size = new System.Drawing.Size(118, 31);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "9.导出x_t/Dwg";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnInsertLibF
            // 
            this.btnInsertLibF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertLibF.Location = new System.Drawing.Point(135, 198);
            this.btnInsertLibF.Name = "btnInsertLibF";
            this.btnInsertLibF.Size = new System.Drawing.Size(118, 31);
            this.btnInsertLibF.TabIndex = 6;
            this.btnInsertLibF.Text = "10.插入库特征";
            this.btnInsertLibF.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnInsertLibF.UseVisualStyleBackColor = true;
            this.btnInsertLibF.Click += new System.EventHandler(this.btnInsertLibF_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(259, 13);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(118, 30);
            this.btnFilter.TabIndex = 7;
            this.btnFilter.Text = "11. 选择过滤";
            this.btnFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btn_DeleteConstraints
            // 
            this.btn_DeleteConstraints.Location = new System.Drawing.Point(259, 60);
            this.btn_DeleteConstraints.Name = "btn_DeleteConstraints";
            this.btn_DeleteConstraints.Size = new System.Drawing.Size(118, 30);
            this.btn_DeleteConstraints.TabIndex = 8;
            this.btn_DeleteConstraints.Text = "12.删除草图的关系";
            this.btn_DeleteConstraints.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_DeleteConstraints.UseVisualStyleBackColor = true;
            this.btn_DeleteConstraints.Click += new System.EventHandler(this.btn_DeleteConstraints_Click);
            // 
            // btnSelectNamedFace
            // 
            this.btnSelectNamedFace.Location = new System.Drawing.Point(259, 104);
            this.btnSelectNamedFace.Name = "btnSelectNamedFace";
            this.btnSelectNamedFace.Size = new System.Drawing.Size(118, 30);
            this.btnSelectNamedFace.TabIndex = 9;
            this.btnSelectNamedFace.Text = "13.选择已命名的面";
            this.btnSelectNamedFace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectNamedFace.UseVisualStyleBackColor = true;
            this.btnSelectNamedFace.Click += new System.EventHandler(this.btnSelectNamedFace_Click);
            // 
            // Btn_T_sketchsegment
            // 
            this.Btn_T_sketchsegment.Location = new System.Drawing.Point(260, 154);
            this.Btn_T_sketchsegment.Name = "Btn_T_sketchsegment";
            this.Btn_T_sketchsegment.Size = new System.Drawing.Size(116, 30);
            this.Btn_T_sketchsegment.TabIndex = 10;
            this.Btn_T_sketchsegment.Text = "14.遍历草绘对象";
            this.Btn_T_sketchsegment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Btn_T_sketchsegment.UseVisualStyleBackColor = true;
            this.Btn_T_sketchsegment.Click += new System.EventHandler(this.Btn_T_sketchsegment_Click);
            // 
            // btn_ThridData
            // 
            this.btn_ThridData.Location = new System.Drawing.Point(259, 198);
            this.btn_ThridData.Name = "btn_ThridData";
            this.btn_ThridData.Size = new System.Drawing.Size(118, 30);
            this.btn_ThridData.TabIndex = 11;
            this.btn_ThridData.Text = "15.增加第三方数据";
            this.btn_ThridData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_ThridData.UseVisualStyleBackColor = true;
            this.btn_ThridData.Click += new System.EventHandler(this.btn_ThridData_Click);
            // 
            // btn_LoadThrid
            // 
            this.btn_LoadThrid.Location = new System.Drawing.Point(383, 12);
            this.btn_LoadThrid.Name = "btn_LoadThrid";
            this.btn_LoadThrid.Size = new System.Drawing.Size(132, 31);
            this.btn_LoadThrid.TabIndex = 12;
            this.btn_LoadThrid.Text = "15.2.读取第三方数据";
            this.btn_LoadThrid.UseVisualStyleBackColor = true;
            this.btn_LoadThrid.Click += new System.EventHandler(this.btn_LoadThrid_Click);
            // 
            // btn_Tips
            // 
            this.btn_Tips.Location = new System.Drawing.Point(383, 59);
            this.btn_Tips.Name = "btn_Tips";
            this.btn_Tips.Size = new System.Drawing.Size(132, 31);
            this.btn_Tips.TabIndex = 13;
            this.btn_Tips.Text = "16.提示信息与进度条";
            this.btn_Tips.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Tips.UseVisualStyleBackColor = true;
            this.btn_Tips.Click += new System.EventHandler(this.btn_Tips_Click);
            // 
            // btn_Adv_Select
            // 
            this.btn_Adv_Select.Location = new System.Drawing.Point(384, 103);
            this.btn_Adv_Select.Name = "btn_Adv_Select";
            this.btn_Adv_Select.Size = new System.Drawing.Size(130, 30);
            this.btn_Adv_Select.TabIndex = 14;
            this.btn_Adv_Select.Text = "17.高级选择";
            this.btn_Adv_Select.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Adv_Select.UseVisualStyleBackColor = true;
            this.btn_Adv_Select.Click += new System.EventHandler(this.btn_Adv_Select_Click);
            // 
            // btnBounding
            // 
            this.btnBounding.Location = new System.Drawing.Point(384, 154);
            this.btnBounding.Name = "btnBounding";
            this.btnBounding.Size = new System.Drawing.Size(131, 30);
            this.btnBounding.TabIndex = 15;
            this.btnBounding.Text = "18.包围盒";
            this.btnBounding.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBounding.UseVisualStyleBackColor = true;
            this.btnBounding.Click += new System.EventHandler(this.btnBounding_Click);
            // 
            // btn_Measure
            // 
            this.btn_Measure.Location = new System.Drawing.Point(383, 198);
            this.btn_Measure.Name = "btn_Measure";
            this.btn_Measure.Size = new System.Drawing.Size(132, 31);
            this.btn_Measure.TabIndex = 16;
            this.btn_Measure.Text = "19.通过测量获取数据";
            this.btn_Measure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Measure.UseVisualStyleBackColor = true;
            this.btn_Measure.Click += new System.EventHandler(this.btn_Measure_Click);
            // 
            // Btn_Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 243);
            this.Controls.Add(this.btn_Measure);
            this.Controls.Add(this.btnBounding);
            this.Controls.Add(this.btn_Adv_Select);
            this.Controls.Add(this.btn_Tips);
            this.Controls.Add(this.btn_LoadThrid);
            this.Controls.Add(this.btn_ThridData);
            this.Controls.Add(this.Btn_T_sketchsegment);
            this.Controls.Add(this.btnSelectNamedFace);
            this.Controls.Add(this.btn_DeleteConstraints);
            this.Controls.Add(this.btnFilter);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Btn_Filter";
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
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btn_DeleteConstraints;
        private System.Windows.Forms.Button btnSelectNamedFace;
        private System.Windows.Forms.Button Btn_T_sketchsegment;
        private System.Windows.Forms.Button btn_ThridData;
        private System.Windows.Forms.Button btn_LoadThrid;
        private System.Windows.Forms.Button btn_Tips;
        private System.Windows.Forms.Button btn_Adv_Select;
        private System.Windows.Forms.Button btnBounding;
        private System.Windows.Forms.Button btn_Measure;
    }
}

