using PaineTool.NewFeature;
using System.Drawing;
using System.Windows.Forms;

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