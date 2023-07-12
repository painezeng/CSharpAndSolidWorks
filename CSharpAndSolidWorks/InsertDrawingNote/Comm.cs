using SolidWorks.Interop.sldworks;
using System.Runtime.InteropServices;

namespace CSharpAndSolidWorks
{
    public class Comm
    {
        public static SldWorks ConnectToSolidWorks()
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