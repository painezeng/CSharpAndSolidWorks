using SolidWorks.Interop.sldworks;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ComparePart
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void butCompare_Click(object sender, EventArgs e)
        {
            Compare compare = new Compare(ConnectToSolidWorks());

            if (compare.CheckTwoParts(txtSendtoCustomer.Text, txtLocal.Text) == true)
            {
                MessageBox.Show("这两个零件有区别");
            }
            else
            {
                MessageBox.Show("这两个零件一样");
            }
        }

        private SldWorks ConnectToSolidWorks()
        {
            SldWorks works2;
            try
            {
                works2 = (SldWorks)Marshal.GetActiveObject("SldWorks.Application.23");
            }
            catch (COMException)
            {
                try
                {
                    works2 = (SldWorks)Marshal.GetActiveObject("SldWorks.Application.26");
                }
                catch (COMException)
                {
                    // MessageBox.Show("Could not connect to SolidWorks.", "SolidWorks", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    works2 = null;
                }
            }
            return works2;
        }
    }
}