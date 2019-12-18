namespace CSharpAndSolidWorks
{
    partial class FrmScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmScreen));
            this.buttonAuto = new System.Windows.Forms.Button();
            this.txtSC = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtsw = new System.Windows.Forms.TextBox();
            this.txtReal1 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSCHeight = new System.Windows.Forms.TextBox();
            this.txtSCweight = new System.Windows.Forms.TextBox();
            this.cobScreenNumber = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtScreenY = new System.Windows.Forms.TextBox();
            this.txtScreenX = new System.Windows.Forms.TextBox();
            this.textScreenSize = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAuto
            // 
            this.buttonAuto.Location = new System.Drawing.Point(221, 14);
            this.buttonAuto.Name = "buttonAuto";
            this.buttonAuto.Size = new System.Drawing.Size(122, 52);
            this.buttonAuto.TabIndex = 15;
            this.buttonAuto.Text = "自动校准";
            this.buttonAuto.UseVisualStyleBackColor = true;
            this.buttonAuto.Click += new System.EventHandler(this.buttonAuto_Click);
            // 
            // txtSC
            // 
            this.txtSC.Location = new System.Drawing.Point(221, 92);
            this.txtSC.Name = "txtSC";
            this.txtSC.Size = new System.Drawing.Size(116, 20);
            this.txtSC.TabIndex = 14;
            this.txtSC.Text = "1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtsw);
            this.groupBox2.Controls.Add(this.txtReal1);
            this.groupBox2.Location = new System.Drawing.Point(6, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 95);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "手动";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(140, 18);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(66, 20);
            this.button3.TabIndex = 4;
            this.button3.Text = "先点我";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(212, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "mm";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(212, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "mm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Solidworks里的长度:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(62, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "实际尺寸:";
            // 
            // txtsw
            // 
            this.txtsw.Location = new System.Drawing.Point(140, 43);
            this.txtsw.Name = "txtsw";
            this.txtsw.Size = new System.Drawing.Size(66, 20);
            this.txtsw.TabIndex = 1;
            // 
            // txtReal1
            // 
            this.txtReal1.Location = new System.Drawing.Point(140, 66);
            this.txtReal1.Name = "txtReal1";
            this.txtReal1.Size = new System.Drawing.Size(66, 20);
            this.txtReal1.TabIndex = 1;
            this.txtReal1.TextChanged += new System.EventHandler(this.txtReal1_TextChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(474, 14);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(301, 212);
            this.listBox1.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSCHeight);
            this.groupBox1.Controls.Add(this.txtSCweight);
            this.groupBox1.Controls.Add(this.cobScreenNumber);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtScreenY);
            this.groupBox1.Controls.Add(this.txtScreenX);
            this.groupBox1.Controls.Add(this.textScreenSize);
            this.groupBox1.Location = new System.Drawing.Point(5, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 115);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前显示器信息";
            // 
            // txtSCHeight
            // 
            this.txtSCHeight.Location = new System.Drawing.Point(127, 89);
            this.txtSCHeight.Name = "txtSCHeight";
            this.txtSCHeight.Size = new System.Drawing.Size(43, 20);
            this.txtSCHeight.TabIndex = 5;
            // 
            // txtSCweight
            // 
            this.txtSCweight.Location = new System.Drawing.Point(63, 89);
            this.txtSCweight.Name = "txtSCweight";
            this.txtSCweight.Size = new System.Drawing.Size(43, 20);
            this.txtSCweight.TabIndex = 5;
            // 
            // cobScreenNumber
            // 
            this.cobScreenNumber.FormattingEnabled = true;
            this.cobScreenNumber.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cobScreenNumber.Location = new System.Drawing.Point(63, 20);
            this.cobScreenNumber.Name = "cobScreenNumber";
            this.cobScreenNumber.Size = new System.Drawing.Size(43, 21);
            this.cobScreenNumber.TabIndex = 4;
            this.cobScreenNumber.Text = "1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(111, 92);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "X";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "X";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 92);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "屏幕尺寸：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "分辨率：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "大小：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "序号：";
            // 
            // txtScreenY
            // 
            this.txtScreenY.Location = new System.Drawing.Point(127, 64);
            this.txtScreenY.Name = "txtScreenY";
            this.txtScreenY.Size = new System.Drawing.Size(43, 20);
            this.txtScreenY.TabIndex = 1;
            // 
            // txtScreenX
            // 
            this.txtScreenX.Location = new System.Drawing.Point(63, 64);
            this.txtScreenX.Name = "txtScreenX";
            this.txtScreenX.Size = new System.Drawing.Size(43, 20);
            this.txtScreenX.TabIndex = 1;
            // 
            // textScreenSize
            // 
            this.textScreenSize.Location = new System.Drawing.Point(63, 43);
            this.textScreenSize.Name = "textScreenSize";
            this.textScreenSize.Size = new System.Drawing.Size(43, 20);
            this.textScreenSize.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(349, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 52);
            this.button1.TabIndex = 10;
            this.button1.Text = "1:1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(221, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "正确比例：";
            // 
            // FrmScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 241);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.buttonAuto);
            this.Controls.Add(this.txtSC);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmScreen";
            this.Text = "屏幕显示实物大小";
            this.Load += new System.EventHandler(this.FrmScreen_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonAuto;
        private System.Windows.Forms.TextBox txtSC;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtsw;
        private System.Windows.Forms.TextBox txtReal1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSCHeight;
        private System.Windows.Forms.TextBox txtSCweight;
        private System.Windows.Forms.ComboBox cobScreenNumber;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtScreenY;
        private System.Windows.Forms.TextBox txtScreenX;
        private System.Windows.Forms.TextBox textScreenSize;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label11;
    }
}