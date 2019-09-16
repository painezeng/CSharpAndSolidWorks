using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
                //发一个消息给solidworks用户
                swApp.SendMsgToUser(msg);
            }
        }

        private void BtnOpenAndNew_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                //通过GetDocumentTemplate 获取默认模板的路径 ,第一个参数可以指定类型
                string partDefaultTemplate = swApp.GetDocumentTemplate((int)swDocumentTypes_e.swDocPART, "", 0, 0, 0);
                //也可以直接指定slddot asmdot drwdot
                //partDefaultTemplate = @"xxx\..prtdot";

                var newDoc = swApp.NewDocument(partDefaultTemplate, 0, 0, 0);

                if (newDoc != null)
                {
                    //创建完成
                    swApp.SendMsgToUser("Create done.");

                    //下面获取当前文件
                    ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                    //选择对应的草图基准面
                    bool boolstatus = swModel.Extension.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);

                    //创建一个2d草图
                    swModel.SketchManager.InsertSketch(true);

                    //画一条线 长度100mm  (solidworks 中系统单位是米,所以这里写0.1)
                    swModel.SketchManager.CreateLine(0, 0, 0, 0, 0.1, 0);

                    //关闭草图
                    swModel.SketchManager.InsertSketch(true);

                    string myNewPartPath = @"C:\study\myNewPart.SLDPRT";

                    //保存零件.
                    int longstatus = swModel.SaveAs3(myNewPartPath, 0, 1);

                    //关闭零件
                    swApp.CloseDoc(myNewPartPath);
                    swApp.SendMsgToUser("Closed");
                    //重新打开零件.
                    swApp.OpenDoc(myNewPartPath, (int)swDocumentTypes_e.swDocPART);

                    swApp.SendMsgToUser("Open completed.");
                }
            }
        }

        private void BtnGetPartData_Click(object sender, EventArgs e)
        {
            //请先打开零件: ..\TemplateModel\clamp1.sldprt

            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc; //当前零件

                //获取通用属性值
                string project = swModel.GetCustomInfoValue("", "Project");

                swModel.DeleteCustomInfo2("", "Qty"); //删除指定项
                swModel.AddCustomInfo3("", "Qty", 30, "1"); //增加通用属性值

                var ConfigNames = (string[])swModel.GetConfigurationNames(); //所有配置名称

                Configuration swConfig = null;

                foreach (var configName in ConfigNames)//遍历所有配置
                {
                    swConfig = (Configuration)swModel.GetConfigurationByName(configName);

                    var manger = swModel.Extension.CustomPropertyManager[configName];
                    //删除当前配置中的属性
                    manger.Delete2("Code");
                    //增加一个属性到些配置
                    manger.Add3("Code", (int)swCustomInfoType_e.swCustomInfoText, "A-" + configName, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
                    //获取此配置中的Code属性
                    string tempCode = manger.Get("Code");
                    //获取此配置中的Description属性

                    var tempDesc = manger.Get("Description");
                    Debug.Print("  Name of configuration  ---> " + configName + " Desc.=" + tempCode);
                }
            }
            else
            {
                MessageBox.Show("Please open a part first.");
            }
        }

        private void Btn_ChangeDim_Click(object sender, EventArgs e)
        {
            //请先打开零件: ..\TemplateModel\clamp1.sldprt
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                //1.增加配置
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
                string NewConfigName = "NewConfig";
                bool boolstatus = swModel.AddConfiguration2(NewConfigName, "", "", true, false, false, true, 256);

                swModel.ShowConfiguration2(NewConfigName);

                //2.增加特征(选择一条边，加圆角)
                boolstatus = swModel.Extension.SelectByID2("", "EDGE", 3.75842546947069E-03, 3.66350829162911E-02, 1.23295158888936E-03, false, 1, null, 0);

                Feature feature = swModel.FeatureManager.FeatureFillet3(195, 0.000508, 0.01, 0, 0, 0, 0, null, null, null, null, null, null, null);

                //3.压缩特征

                feature.Select(false);

                swModel.EditSuppress();

                //4.修改尺寸
                swModel.Parameter("D1@Fillet8").SystemValue = 0.000254; //0.001英寸

                swModel.EditRebuild3();

                //5.删除特征

                feature.Select(false);
                swModel.EditDelete();
            }
        }

        private void Btn_Traverse_Feature_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                //第一个特征
                Feature swFeat = (Feature)swModel.FirstFeature();

                //遍历
                Utility.TraverseFeatures(swFeat, true);
            }
        }
    }
}