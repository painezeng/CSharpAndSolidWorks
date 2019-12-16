namespace CSharpAndSolidWorks
{
    partial class ExportForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.LabTip = new System.Windows.Forms.Label();
            this.butAutoRename = new System.Windows.Forms.Button();
            this.butExport = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBodies_MirrorCopy = new System.Windows.Forms.ListBox();
            this.listBodies_Normally = new System.Windows.Forms.ListBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labStatus,
            this.ProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 396);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(705, 23);
            this.statusStrip1.TabIndex = 24;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labStatus
            // 
            this.labStatus.AutoSize = false;
            this.labStatus.Name = "labStatus";
            this.labStatus.Size = new System.Drawing.Size(300, 18);
            this.labStatus.Text = "toolStripStatusLabel1";
            this.labStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProgressBar
            // 
            this.ProgressBar.AutoSize = false;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(300, 17);
            // 
            // LabTip
            // 
            this.LabTip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabTip.AutoSize = true;
            this.LabTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.LabTip.Location = new System.Drawing.Point(17, 444);
            this.LabTip.Name = "LabTip";
            this.LabTip.Size = new System.Drawing.Size(38, 13);
            this.LabTip.TabIndex = 23;
            this.LabTip.Text = "Ready";
            this.LabTip.Visible = false;
            // 
            // butAutoRename
            // 
            this.butAutoRename.Location = new System.Drawing.Point(538, 62);
            this.butAutoRename.Name = "butAutoRename";
            this.butAutoRename.Size = new System.Drawing.Size(146, 38);
            this.butAutoRename.TabIndex = 21;
            this.butAutoRename.Text = "智能重命名";
            this.butAutoRename.UseVisualStyleBackColor = true;
            this.butAutoRename.Click += new System.EventHandler(this.butAutoRename_Click_1);
            // 
            // butExport
            // 
            this.butExport.Location = new System.Drawing.Point(535, 3);
            this.butExport.Name = "butExport";
            this.butExport.Size = new System.Drawing.Size(149, 40);
            this.butExport.TabIndex = 19;
            this.butExport.Text = "批量导出并装配";
            this.butExport.UseVisualStyleBackColor = true;
            this.butExport.Click += new System.EventHandler(this.butExport_Click);
            // 
            // txtPath
            // 
            this.txtPath.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtPath.Enabled = false;
            this.txtPath.Location = new System.Drawing.Point(104, 3);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(408, 20);
            this.txtPath.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(223, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "镜像、复制实体:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "当前零件路径:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "一般实体特征:";
            // 
            // listBodies_MirrorCopy
            // 
            this.listBodies_MirrorCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBodies_MirrorCopy.FormattingEnabled = true;
            this.listBodies_MirrorCopy.Location = new System.Drawing.Point(220, 62);
            this.listBodies_MirrorCopy.Name = "listBodies_MirrorCopy";
            this.listBodies_MirrorCopy.Size = new System.Drawing.Size(292, 303);
            this.listBodies_MirrorCopy.TabIndex = 14;
            // 
            // listBodies_Normally
            // 
            this.listBodies_Normally.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBodies_Normally.FormattingEnabled = true;
            this.listBodies_Normally.Location = new System.Drawing.Point(17, 62);
            this.listBodies_Normally.Name = "listBodies_Normally";
            this.listBodies_Normally.Size = new System.Drawing.Size(177, 303);
            this.listBodies_Normally.TabIndex = 13;
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 419);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.LabTip);
            this.Controls.Add(this.butAutoRename);
            this.Controls.Add(this.butExport);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBodies_MirrorCopy);
            this.Controls.Add(this.listBodies_Normally);
            this.Name = "ExportForm";
            this.Text = "多实体导出并装配";
            this.Load += new System.EventHandler(this.AddText_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel labStatus;
		private System.Windows.Forms.ToolStripProgressBar ProgressBar;
		private System.Windows.Forms.Label LabTip;
		private System.Windows.Forms.Button butAutoRename;
		private System.Windows.Forms.Button butExport;
		private System.Windows.Forms.TextBox txtPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listBodies_MirrorCopy;
		private System.Windows.Forms.ListBox listBodies_Normally;
	}
}