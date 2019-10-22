namespace CSharpAndSolidWorks
{
    partial class MyPane
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
            this.StatusLab = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.ReoGridReport = new unvell.ReoGrid.ReoGridControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ReloadBOMMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.使用说明ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkOpenFile = new System.Windows.Forms.CheckBox();
            this.labActionModelPath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLab,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 634);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(352, 23);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLab
            // 
            this.StatusLab.AutoSize = false;
            this.StatusLab.Name = "StatusLab";
            this.StatusLab.Size = new System.Drawing.Size(300, 18);
            this.StatusLab.Text = "Ready";
            this.StatusLab.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(300, 17);
            this.toolStripProgressBar1.Visible = false;
            // 
            // ReoGridReport
            // 
            this.ReoGridReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReoGridReport.BackColor = System.Drawing.Color.White;
            this.ReoGridReport.ColumnHeaderContextMenuStrip = null;
            this.ReoGridReport.LeadHeaderContextMenuStrip = null;
            this.ReoGridReport.Location = new System.Drawing.Point(12, 74);
            this.ReoGridReport.Name = "ReoGridReport";
            this.ReoGridReport.RowHeaderContextMenuStrip = null;
            this.ReoGridReport.Script = null;
            this.ReoGridReport.SheetTabContextMenuStrip = null;
            this.ReoGridReport.SheetTabNewButtonVisible = true;
            this.ReoGridReport.SheetTabVisible = false;
            this.ReoGridReport.SheetTabWidth = 145;
            this.ReoGridReport.ShowScrollEndSpacing = true;
            this.ReoGridReport.Size = new System.Drawing.Size(328, 557);
            this.ReoGridReport.TabIndex = 6;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReloadBOMMenuItem,
            this.ToolStripMenuItemExport,
            this.使用说明ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Margin = new System.Windows.Forms.Padding(5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(352, 33);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ReloadBOMMenuItem
            // 
            this.ReloadBOMMenuItem.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ReloadBOMMenuItem.Margin = new System.Windows.Forms.Padding(5);
            this.ReloadBOMMenuItem.Name = "ReloadBOMMenuItem";
            this.ReloadBOMMenuItem.Size = new System.Drawing.Size(67, 19);
            this.ReloadBOMMenuItem.Text = "重新加载";
            this.ReloadBOMMenuItem.Click += new System.EventHandler(this.ReloadBOMMenuItem_Click);
            // 
            // ToolStripMenuItemExport
            // 
            this.ToolStripMenuItemExport.BackColor = System.Drawing.Color.LightGray;
            this.ToolStripMenuItemExport.Margin = new System.Windows.Forms.Padding(5);
            this.ToolStripMenuItemExport.Name = "ToolStripMenuItemExport";
            this.ToolStripMenuItemExport.Size = new System.Drawing.Size(81, 19);
            this.ToolStripMenuItemExport.Text = "导出到Excel";
            this.ToolStripMenuItemExport.Click += new System.EventHandler(this.ToolStripMenuItemExport_Click);
            // 
            // 使用说明ToolStripMenuItem
            // 
            this.使用说明ToolStripMenuItem.Name = "使用说明ToolStripMenuItem";
            this.使用说明ToolStripMenuItem.Size = new System.Drawing.Size(67, 29);
            this.使用说明ToolStripMenuItem.Text = "使用说明";
            this.使用说明ToolStripMenuItem.Visible = false;
            // 
            // checkOpenFile
            // 
            this.checkOpenFile.AutoSize = true;
            this.checkOpenFile.Checked = true;
            this.checkOpenFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOpenFile.Location = new System.Drawing.Point(13, 37);
            this.checkOpenFile.Name = "checkOpenFile";
            this.checkOpenFile.Size = new System.Drawing.Size(86, 17);
            this.checkOpenFile.TabIndex = 11;
            this.checkOpenFile.Text = "导出后打开";
            this.checkOpenFile.UseVisualStyleBackColor = true;
            // 
            // labActionModelPath
            // 
            this.labActionModelPath.AutoSize = true;
            this.labActionModelPath.ForeColor = System.Drawing.Color.Blue;
            this.labActionModelPath.Location = new System.Drawing.Point(76, 57);
            this.labActionModelPath.Name = "labActionModelPath";
            this.labActionModelPath.Size = new System.Drawing.Size(16, 13);
            this.labActionModelPath.TabIndex = 9;
            this.labActionModelPath.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "当前文件：";
            // 
            // MyPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 657);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ReoGridReport);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.checkOpenFile);
            this.Controls.Add(this.labActionModelPath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MyPane";
            this.ShowInTaskbar = false;
            this.Text = "MyPane";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MyPane_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLab;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        internal unvell.ReoGrid.ReoGridControl ReoGridReport;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ReloadBOMMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExport;
        private System.Windows.Forms.ToolStripMenuItem 使用说明ToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkOpenFile;
        private System.Windows.Forms.Label labActionModelPath;
        private System.Windows.Forms.Label label1;
    }
}