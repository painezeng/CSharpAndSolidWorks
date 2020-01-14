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

        /// <summary>
        /// 遍历装配体零件
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="nLevel"></param>
        public static void TraverseCompXform(Component2 swComp, long nLevel, bool setcolor = false)
        {
            object[] vChild;
            Component2 swChildComp;
            string sPadStr = "";
            MathTransform swCompXform;
            //  object vXform;
            long i;

            for (i = 0; i < nLevel; i++)
            {
                sPadStr = sPadStr + "  ";
            }
            swCompXform = swComp.Transform2;
            if (swCompXform != null)
            {
                ModelDoc2 swModel;
                swModel = (ModelDoc2)swComp.GetModelDoc2();

                try
                {
                    //子零件文件名
                    //Debug.Print(sPadStr + swComp.Name2);

                    if (swComp.GetSelectByIDString() == "")
                    {
                        //选择id
                        //Debug.Print(swComp.GetSelectByIDString());
                    }
                    else
                    {
                    }
                }
                catch
                {
                }
                if (swModel != null)
                {
                    Debug.Print("Loading:" + swComp.Name2);
                    //获取零件的一些信息，如属性，名字路径。
                    string tempPartNum = swModel.get_CustomInfo2(swComp.ReferencedConfiguration, "PartNum");
                    string tempName2 = swComp.Name2;
                    string tempName = swModel.GetPathName();
                    string tempConfigName = swComp.ReferencedConfiguration;
                    string tempComponentRef = swComp.ComponentReference;

                    //如果要设定颜色
                    if (setcolor == true)
                    {
                        double[] matPropVals = swModel.MaterialPropertyValues;
                        var tempC = GetRadomColor(System.IO.Path.GetFileNameWithoutExtension(swModel.GetPathName()));
                        matPropVals[0] = Convert.ToDouble(tempC.R) / 255;
                        matPropVals[1] = Convert.ToDouble(tempC.G) / 255;
                        matPropVals[2] = Convert.ToDouble(tempC.B) / 255;
                        swModel.MaterialPropertyValues = matPropVals;

                        swModel.WindowRedraw();
                    }
                }
            }
            else
            {
                ModelDoc2 swModel;
                swModel = (ModelDoc2)swComp.GetModelDoc2();
            }

            vChild = (object[])swComp.GetChildren();
            for (i = 0; i <= (vChild.Length - 1); i++)
            {
                swChildComp = (Component2)vChild[i];
                TraverseCompXform(swChildComp, nLevel + 1, setcolor);
            }
        }

        public static System.Drawing.Color GetRadomColor(string name)
        {
            Random rnd = new Random();

            //这里可以根据需要指定颜色。
            if (name.Contains("m1"))
            {
                return System.Drawing.Color.Red;
            }

            return System.Drawing.Color.FromArgb(
                 rnd.Next(0, 255), /*红色*/
                 rnd.Next(0, 255), /*绿色*/
                 rnd.Next(0, 255)  /*蓝色*/ );
        }
    }

    public class BodyModel
    {
        //static int totalQty;
        public string name;

        public string material;
        public string refBodyname;
        public int qty;
        public FeatureType featureT;
        public string comment = default(string);
        public string boxSize = "";

        public BodyModel(string n, string m, string refBody, FeatureType f, string b)
        {
            this.name = n;
            this.material = m;
            this.refBodyname = refBody;
            this.featureT = f;
            this.comment = "";
            this.boxSize = b;
        }

        //override void Remove(){
        //}
    }

    public struct BoxSize
    {
        public double Length;
        public double Weigth;
        public double Height;
    }
}