using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PaineTool.NewFeature;

namespace PaineTool
{
    public partial class MyPMPControl : UserControl
    {
        public NewFeaturePMPage pmPage;

        public MyPMPControl(NewFeaturePMPage pMPage)
        {
            InitializeComponent();
            pmPage = pMPage;
            BackColor = Color.FromArgb(247, 247, 247);
            // Size = new Size(270, 379);
        }
    }
}