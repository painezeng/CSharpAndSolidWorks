namespace GetRayIntersectionWithBody
{
    partial class FrmMain
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnViewRay = new System.Windows.Forms.Button();
            this.txtVertor_Z = new System.Windows.Forms.TextBox();
            this.txtStart_Z = new System.Windows.Forms.TextBox();
            this.txtVertor_Y = new System.Windows.Forms.TextBox();
            this.txtStart_Y = new System.Windows.Forms.TextBox();
            this.txtVertor_X = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtStart_X = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGetIntersectionPoint = new System.Windows.Forms.Button();
            this.btnGetIntersectionPointInPart = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClear);
            this.groupBox1.Controls.Add(this.btnViewRay);
            this.groupBox1.Controls.Add(this.txtVertor_Z);
            this.groupBox1.Controls.Add(this.txtStart_Z);
            this.groupBox1.Controls.Add(this.txtVertor_Y);
            this.groupBox1.Controls.Add(this.txtStart_Y);
            this.groupBox1.Controls.Add(this.txtVertor_X);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtStart_X);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(367, 93);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "射线参数";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(286, 50);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 35);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnViewRay
            // 
            this.btnViewRay.Location = new System.Drawing.Point(286, 14);
            this.btnViewRay.Name = "btnViewRay";
            this.btnViewRay.Size = new System.Drawing.Size(75, 35);
            this.btnViewRay.TabIndex = 2;
            this.btnViewRay.Text = "预览";
            this.btnViewRay.UseVisualStyleBackColor = true;
            this.btnViewRay.Click += new System.EventHandler(this.btnViewRay_Click);
            // 
            // txtVertor_Z
            // 
            this.txtVertor_Z.Location = new System.Drawing.Point(203, 64);
            this.txtVertor_Z.Name = "txtVertor_Z";
            this.txtVertor_Z.Size = new System.Drawing.Size(58, 21);
            this.txtVertor_Z.TabIndex = 1;
            this.txtVertor_Z.Text = "-652";
            // 
            // txtStart_Z
            // 
            this.txtStart_Z.Location = new System.Drawing.Point(71, 64);
            this.txtStart_Z.Name = "txtStart_Z";
            this.txtStart_Z.Size = new System.Drawing.Size(58, 21);
            this.txtStart_Z.TabIndex = 1;
            this.txtStart_Z.Text = "252";
            // 
            // txtVertor_Y
            // 
            this.txtVertor_Y.Location = new System.Drawing.Point(203, 40);
            this.txtVertor_Y.Name = "txtVertor_Y";
            this.txtVertor_Y.Size = new System.Drawing.Size(58, 21);
            this.txtVertor_Y.TabIndex = 1;
            this.txtVertor_Y.Text = "0";
            // 
            // txtStart_Y
            // 
            this.txtStart_Y.Location = new System.Drawing.Point(71, 40);
            this.txtStart_Y.Name = "txtStart_Y";
            this.txtStart_Y.Size = new System.Drawing.Size(58, 21);
            this.txtStart_Y.TabIndex = 1;
            this.txtStart_Y.Text = "0";
            // 
            // txtVertor_X
            // 
            this.txtVertor_X.Location = new System.Drawing.Point(203, 14);
            this.txtVertor_X.Name = "txtVertor_X";
            this.txtVertor_X.Size = new System.Drawing.Size(58, 21);
            this.txtVertor_X.TabIndex = 1;
            this.txtVertor_X.Text = "125";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(155, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "向量Z:";
            // 
            // txtStart_X
            // 
            this.txtStart_X.Location = new System.Drawing.Point(71, 14);
            this.txtStart_X.Name = "txtStart_X";
            this.txtStart_X.Size = new System.Drawing.Size(58, 21);
            this.txtStart_X.TabIndex = 1;
            this.txtStart_X.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(155, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "向量Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "起点Z:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(155, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "向量X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "起点Y:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "起点X:";
            // 
            // btnGetIntersectionPoint
            // 
            this.btnGetIntersectionPoint.Location = new System.Drawing.Point(13, 112);
            this.btnGetIntersectionPoint.Name = "btnGetIntersectionPoint";
            this.btnGetIntersectionPoint.Size = new System.Drawing.Size(176, 43);
            this.btnGetIntersectionPoint.TabIndex = 2;
            this.btnGetIntersectionPoint.Text = "计算交点(装配体)-有问题";
            this.btnGetIntersectionPoint.UseVisualStyleBackColor = true;
            this.btnGetIntersectionPoint.Visible = false;
            this.btnGetIntersectionPoint.Click += new System.EventHandler(this.btnGetIntersectionPoint_Click);
            // 
            // btnGetIntersectionPointInPart
            // 
            this.btnGetIntersectionPointInPart.Location = new System.Drawing.Point(13, 112);
            this.btnGetIntersectionPointInPart.Name = "btnGetIntersectionPointInPart";
            this.btnGetIntersectionPointInPart.Size = new System.Drawing.Size(361, 43);
            this.btnGetIntersectionPointInPart.TabIndex = 2;
            this.btnGetIntersectionPointInPart.Text = "计算交点(零件中)";
            this.btnGetIntersectionPointInPart.UseVisualStyleBackColor = true;
            this.btnGetIntersectionPointInPart.Click += new System.EventHandler(this.btnGetIntersectionPointInPart_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 169);
            this.Controls.Add(this.btnGetIntersectionPointInPart);
            this.Controls.Add(this.btnGetIntersectionPoint);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmMain";
            this.Text = "射线交点计算";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtVertor_Z;
        private System.Windows.Forms.TextBox txtStart_Z;
        private System.Windows.Forms.TextBox txtVertor_Y;
        private System.Windows.Forms.TextBox txtStart_Y;
        private System.Windows.Forms.TextBox txtVertor_X;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtStart_X;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnViewRay;
        private System.Windows.Forms.Button btnGetIntersectionPoint;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnGetIntersectionPointInPart;
    }
}

