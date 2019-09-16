using SolidWorks.Interop.sldworks;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CSharpAndSolidWorks
{
    public class Utility
    {
        public static ISldWorks SwApp { get; private set; }

        public static ISldWorks ConnectToSolidWorks()
        {
            if (SwApp != null)
            {
                return SwApp;
            }
            else
            {
                Debug.Print("connect to solidworks on " + DateTime.Now);
                try
                {
                    SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application");
                }
                catch (COMException)
                {
                    try
                    {
                        SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application.23");//2015
                    }
                    catch (COMException)
                    {
                        try
                        {
                            SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application.26");//2018
                        }
                        catch (COMException)
                        {
                            MessageBox.Show("Could not connect to SolidWorks.", "SolidWorks", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            SwApp = null;
                        }
                    }
                }

                return SwApp;
            }
        }

        /// <summary>
        /// 遍历特征
        /// </summary>
        /// <param name="thisFeat"></param>
        /// <param name="isTopLevel"></param>
        public static void TraverseFeatures(Feature thisFeat, bool isTopLevel)
        {
            Feature curFeat = default(Feature);
            curFeat = thisFeat;

            while ((curFeat != null))
            {
                //输出特征名称
                Debug.Print(curFeat.Name);

                Feature subfeat = default(Feature);
                subfeat = (Feature)curFeat.GetFirstSubFeature();

                while ((subfeat != null))
                {
                    TraverseFeatures(subfeat, false);
                    Feature nextSubFeat = default(Feature);
                    nextSubFeat = (Feature)subfeat.GetNextSubFeature();
                    subfeat = nextSubFeat;
                    nextSubFeat = null;
                }

                subfeat = null;

                Feature nextFeat = default(Feature);

                if (isTopLevel)
                {
                    nextFeat = (Feature)curFeat.GetNextFeature();
                }
                else
                {
                    nextFeat = null;
                }

                curFeat = nextFeat;
                nextFeat = null;
            }
        }
    }
}