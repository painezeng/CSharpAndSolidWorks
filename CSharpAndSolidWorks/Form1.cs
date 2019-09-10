using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpAndSolidWorks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                string msg = "This message from C#. solidworks version is " + swApp.RevisionNumber();

                swApp.SendMsgToUser(msg);
            }
        }
    }
}