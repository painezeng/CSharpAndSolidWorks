using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using SolidWorks.Interop.swcommands;
using View = SolidWorks.Interop.sldworks.View;
using SolidWorks.Interop.swdocumentmgr;
using PSWStandalon;
using Microsoft.VisualBasic;
using System.Linq;
using Attribute = SolidWorks.Interop.sldworks.Attribute;
using GetRayIntersectionWithBody;
using SolidWorks.Interop.swdimxpert;
using GeometRi;
using SolidWorks.Interop.cosworks;
using System.Reflection;
using System.Xml.Linq;
using SolidWorks.Interop.swpublished;
using static System.Net.WebRequestMethods;

namespace CSharpAndSolidWorks
{
  
    public partial class Btn_Filter : Form
    {
        
        public Btn_Filter()
        {
            InitializeComponent();
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                string msg = "This message from C#. solidworks version is " + swApp.RevisionNumber();
                //发一个消息给solidworks用户
                swApp.SendMsgToUser(msg);
            }
        }

        private void BtnOpenAndNew_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

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
                    // swModel.SketchManager.InsertSketch(true);
                    swModel.SketchManager.Insert3DSketch(true);

                    //画一条线 长度100mm  (solidworks 中系统单位是米,所以这里写0.1)
                    swModel.SketchManager.CreateLine(0, 0, 0, 0.1, 0.1, 0.1);

                    //关闭草图
                    swModel.SketchManager.InsertSketch(true);

                    //设定保存文件的完整路径
                    string myNewPartPath = @"C:\myNewPart.SLDPRT";

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

            SldWorks swApp = Utility.ConnectToSolidWorks();

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
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                //1.增加配置
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
                string NewConfigName = "NewConfig";
                bool boolstatus = swModel.AddConfiguration2(NewConfigName, "", "", true, false, false, true, 256);

                swModel.ShowConfiguration2(NewConfigName);

                //2.增加特征(选择一条边，加圆角)
                boolstatus = swModel.Extension.SelectByID2("", "EDGE", 3.75842546947069E-03, 3.66350829162911E-02, 1.23295158888936E-03, false, 1, null, 0);

                Feature feature = (Feature)swModel.FeatureManager.FeatureFillet3(195, 0.000508, 0.01, 0, 0, 0, 0, null, null, null, null, null, null, null);

                //3.压缩特征

                feature.Select(false);

                swModel.EditSuppress();

                //4.修改尺寸
                Dimension dimension = (Dimension)swModel.Parameter("D1@Fillet8");
                dimension.SystemValue = 0.000254; //0.001英寸

                swModel.EditRebuild3();

                //5.删除特征

                feature.Select(false);
                swModel.EditDelete();
            }
        }

        private void Btn_Traverse_Feature_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();
            //加速读取
            swApp.CommandInProgress = true;

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                //第一个特征
                Feature swFeat = (Feature)swModel.FirstFeature();

                //遍历
                Utility.TraverseFeatures(swFeat, true);
            }
            swApp.CommandInProgress = false;
        }

        private void Btn_Traverse_Comp_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                Configuration swConf = (Configuration)swModel.GetActiveConfiguration();

                Component2 swRootComp = (Component2)swConf.GetRootComponent();

                //遍历
                Utility.TraverseCompXform(swRootComp, 0);
            }
        }

        private void btn_Traverse_Drawing_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                DrawingDoc drawingDoc = (DrawingDoc)swModel;

                //获取当前工程图中的所有图纸名称
                var sheetNames = (object[])drawingDoc.GetSheetNames();

                //遍历并找出包含k3 的工程图名称
                string k3Name = "";
                foreach (var kName in sheetNames)
                {
                    if (((String)kName).Contains("k3"))
                    {
                        k3Name = (String)kName;
                    }
                }
                //切换图纸
                bool bActSheet = drawingDoc.ActivateSheet(k3Name);

                // 获取当前工程图对象
                Sheet drwSheet = default(Sheet);
                drwSheet = (Sheet)drawingDoc.GetCurrentSheet();

                //获取所有的视图
                object[] views = null;
                views = (object[])drwSheet.GetViews();

                foreach (object vView in views)
                {
                    var ss = (View)vView;
                    Debug.Print(ss.GetName2());
                }

                //选中新的视图，移动位置。
                bool boolstatus = swModel.Extension.SelectByID2("主视图1", "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
                //切换视图方向
                swModel.ShowNamedView2("*Front", (int)swStandardViews_e.swFrontView);
                //修改视图的名称
                swModel.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "主视图-1");

                SelectionMgr modelSel = swModel.ISelectionManager;

                //该视图对象
                View actionView = (View)modelSel.GetSelectedObject5(1);

                //位置 actionView.Position

                //获取注释
                var noteCount = actionView.GetNoteCount();

                List<Note> AllNotes = new List<Note>();
                if (noteCount > 0)
                {
                    Note note = (Note)actionView.GetFirstNote();

                    Debug.Print(noteCount.ToString());

                    //这里要判断类型之后 才能转成组件，才能获取 名字
                    //var anno = (Annotation)note.GetAnnotation();

                    //var types= (int[])anno.GetAttachedEntityTypes();

                    //var attOjbect = (object[])anno.GetAttachedEntities3();

                    //var attEntity = (Entity)attOjbect[0];

                    //var attComp = (Component2)(attEntity.GetComponent());

                    //Debug.Print(attComp.Name2);

                    // note.GetBalloonStyle
                    Debug.Print(note.GetText());

                    AllNotes.Add(note);

                    var leaderInfo = note.GetLeaderInfo();

                    for (int k = 0; k < noteCount - 1; k++)
                    {
                        note = (Note)note.GetNext();
                        Debug.Print(note.GetText());

                        AllNotes.Add(note);
                    }

                    swModel.EditRebuild3();

                    swModel.EditDelete();
                }
            }
        }

        private void btn_InsertPart_Click(object sender, EventArgs e)
        {
            //step1:生成一个新装配并保存.
            SldWorks swApp = Utility.ConnectToSolidWorks();
            int errors = 0;
            int warinings = 0;
            if (swApp != null)
            {
                //通过GetDocumentTemplate 获取默认模板的路径 ,第一个参数可以指定类型
                string partDefaultTemplate = swApp.GetDocumentTemplate((int)swDocumentTypes_e.swDocASSEMBLY, "", 0, 0, 0);
                //也可以直接指定slddot asmdot drwdot
                //partDefaultTemplate = @"xxx\..prtdot";

                var newDoc = swApp.NewDocument(partDefaultTemplate, 0, 0, 0);

                if (newDoc != null)
                {
                    //下面获取当前文件
                    ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                    swModel.Extension.SaveAs(@"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\TempAssembly.sldasm", 0, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, "", ref errors, ref warinings);

                    //step2:打开已有零件
                    string myNewPartPath = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\clamp1.sldprt";
                    swApp.OpenDoc(myNewPartPath, (int)swDocumentTypes_e.swDocPART);

                    //step3:切换到装配体中,利用面配合来装配零件.

                    AssemblyDoc assemblyDoc = (AssemblyDoc)swApp.ActivateDoc3("TempAssembly.sldasm", true, 0, errors);
                    swApp.ActivateDoc("TempAssembly.sldasm");

                    Component2 InsertedComponent = assemblyDoc.AddComponent5(myNewPartPath, 0, "", false, "", 0, 0, 0);

                    InsertedComponent.Select(false);

                    assemblyDoc.UnfixComponent();

                    //step4: 配合:

                    bool boolstatus = swModel.Extension.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);

                    boolstatus = swModel.Extension.SelectByID2("Front Plane@clamp1-1@TempAssembly", "PLANE", 0, 0, 0, true, 0, null, 0);
                    int longstatus = 0;
                    //重合
                    assemblyDoc.AddMate5(0, 0, false, 0, 0.001, 0.001, 0.001, 0.001, 0, 0, 0, false, false, 0, out longstatus);

                    swModel.EditRebuild3();
                    swModel.ClearSelection();

                    //距离配合 :
                    boolstatus = swModel.Extension.SelectByID2("Plane2", "PLANE", 0, 0, 0, false, 0, null, 0);
                    boolstatus = swModel.Extension.SelectByID2("Top Plane@clamp1-1@TempAssembly", "PLANE", 0, 0, 0, true, 0, null, 0);

                    assemblyDoc.AddMate5((int)swMateType_e.swMateDISTANCE, (int)swMateAlign_e.swMateAlignALIGNED, true, 0.01, 0.01, 0.01, 0.01, 0.01, 0, 0, 0, false, false, 0, out longstatus);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                if (swModel.GetType() == (int)swDocumentTypes_e.swDocPART || swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
                {
                    ModelDocExtension swModExt = (ModelDocExtension)swModel.Extension;

                    int error = 0;

                    int warnings = 0;

                    //设定导出坐标系

                    var setRes = swModel.Extension.SetUserPreferenceString(16, 0, "CustomerCS");

                    //设置导出版本
                    swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swParasolidOutputVersion, (int)swParasolidOutputVersion_e.swParasolidOutputVersion_161);

                    swModExt.SaveAs(@"C:\export.x_t", (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref error, ref warnings);
                }
                else if (swModel.GetType() == (int)swDocumentTypes_e.swDocDRAWING)
                {
                    ModelDocExtension swModExt = (ModelDocExtension)swModel.Extension;

                    int error = 0;

                    int warnings = 0;

                    //设置dxf 导出版本 R14
                    swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfVersion, 2);

                    //是否显示 草图
                    swModel.SetUserPreferenceToggle(196, false);

                    swModExt.SaveAs(@"C:\export.dxf", (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref error, ref warnings);
                }
            }
        }

        public DispatchWrapper[] ObjectArrayToDispatchWrapperArray(object[] Objects)
        {
            int ArraySize = 0;
            ArraySize = Objects.GetUpperBound(0);
            DispatchWrapper[] d = new DispatchWrapper[ArraySize + 1];
            int ArrayIndex = 0;
            for (ArrayIndex = 0; ArrayIndex <= ArraySize; ArrayIndex++)
            {
                d[ArrayIndex] = new DispatchWrapper(Objects[ArrayIndex]);
            }
            return d;
        }

        public DispatchWrapper[] LibRefs;

        private void btnInsertLibF_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            //可以参考API帮助中的:Create Library Feature Data Object and Library Feature With References Example (C#)

            //Step1:新建一个零件.
            Feature swFeature = default(Feature);
            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            SketchManager swSketchManager = default(SketchManager);
            SelectionMgr swSelectionManager = default(SelectionMgr);
            FeatureManager swFeatureManager = default(FeatureManager);
            LibraryFeatureData swLibFeat = default(LibraryFeatureData);
            bool status = false;
            object[] sketchLines = null;
            object Refs = null;
            object RefTypes = null;
            int RefCount = 0;
            int k = 0;
            int i = 0;
            DispatchWrapper[] LibRefs = null;

            string libPath = "C:\\ProgramData\\SOLIDWORKS\\SOLIDWORKS 2018\\design library\\features\\metric\\slots\\straight slot.sldlfp";

            // Create part
            swModel = (ModelDoc2)swApp.NewDocument("C:\\ProgramData\\SolidWorks\\SOLIDWORKS 2018\\templates\\Part.prtdot", 0, 0, 0);
            swModelDocExt = (ModelDocExtension)swModel.Extension;
            status = swModelDocExt.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            swModel.ClearSelection2(true);
            status = swModelDocExt.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchAddConstToRectEntity, (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified, false);
            status = swModelDocExt.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchAddConstLineDiagonalType, (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified, true);
            swSketchManager = (SketchManager)swModel.SketchManager;
            sketchLines = (object[])swSketchManager.CreateCornerRectangle(0, 0, 0, 1, 0.5, 0);
            swModel.ShowNamedView2("*Trimetric", 8);
            swModel.ClearSelection2(true);
            status = swModelDocExt.SelectByID2("Line2", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            status = swModelDocExt.SelectByID2("Line1", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            status = swModelDocExt.SelectByID2("Line4", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            status = swModelDocExt.SelectByID2("Line3", "SKETCHSEGMENT", 0, 0, 0, true, 0, null, 0);
            swFeatureManager = (FeatureManager)swModel.FeatureManager;
            swFeature = (Feature)swFeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.01, 0.01, false, false, false,
            false, 0.0174532925199433, 0.0174532925199433, false, false, false, false, true, true, true,
            0, 0, false);
            swSelectionManager = (SelectionMgr)swModel.SelectionManager;
            swSelectionManager.EnableContourSelection = false;

            swModel = (ModelDoc2)swApp.ActiveDoc;

            string actName = swModel.GetPathName();

            #region 第一种方法

            //Step2:初始化库特征
            swLibFeat = (LibraryFeatureData)swFeatureManager.CreateDefinition((int)swFeatureNameID_e.swFmLibraryFeature);
            status = swLibFeat.Initialize(libPath);

            //step3:获取库特征需要的参考对象
            RefCount = swLibFeat.GetReferencesCount();
            Refs = (object[])swLibFeat.GetReferences2((int)swLibFeatureData_e.swLibFeatureData_FeatureRespect, out RefTypes);

            if ((RefTypes != null))
            {
                Debug.Print("Types of references required (edge = 1): ");
                int[] RefType = (int[])RefTypes;
                for (k = RefType.GetLowerBound(0); k <= RefType.GetUpperBound(0); k++)
                {
                    Debug.Print("    " + RefType[k].ToString());
                }
            }
            //setp4:设定库特征默认的配置名称
            swLibFeat.ConfigurationName = "Default";
            //setp5:选择一个面,并插入库特征
            status = swModelDocExt.SelectByID2("", "FACE", 0.522458766456054, 0.288038964184011, 0.00999999999987722, false, 0, null, 0);
            swFeature = (Feature)swFeatureManager.CreateFeature(swLibFeat);
            //step6:
            swLibFeat = null;
            swLibFeat = (LibraryFeatureData)swFeature.GetDefinition();
            status = swLibFeat.AccessSelections(swModel, null);

            //step7:选择真实的参考
            status = swModelDocExt.SelectByID2("", "EDGE", 0.960865149149924, 0.497807163546383, 0.0131011390528215, true, 0, null, 0);
            status = swModelDocExt.SelectByID2("", "EDGE", 0.99866860703213, 0.481385806014544, 0.0113313929676906, true, 0, null, 0);
            int selCount = 0;
            selCount = swSelectionManager.GetSelectedObjectCount2(-1);

            object[] selectedObjects = new object[selCount];

            for (i = 0; i < selCount; i++)
            {
                object selectedObject = null;
                selectedObject = (object)swSelectionManager.GetSelectedObject6(i + 1, -1);
                selectedObjects[i] = selectedObject;
            }

            // 转换对象
            LibRefs = (DispatchWrapper[])ObjectArrayToDispatchWrapperArray((selectedObjects));

            // 设定引用关系到刚生成的库特征
            swLibFeat.SetReferences(LibRefs);

            // 更新库功能
            status = swFeature.ModifyDefinition(swLibFeat, swModel, null);

            // 取消抑制库功能
            status = swModelDocExt.SelectByID2("straight slot<1>", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            swModel.EditUnsuppress2();

            swModel.ClearSelection2(true);

            #endregion 第一种方法

            #region 第二种方法(已过时)

            ////先选中线,再插入库特征.

            ////要先打开库特征,然后切换到当前零件,选中参考特征,最后插入特征库

            //int errors = 0;
            //int warnings = 0;

            //swApp.OpenDoc6(libPath, 1, 0, "", errors, warnings);

            //swModel = swApp.ActivateDoc2(actName, true, errors);

            //status = swModelDocExt.SelectByID2("", "FACE", 0.522458766456054, 0.288038964184011, 9.99999999987722E-03, false, 0, null, 0);
            //status = swModelDocExt.SelectByID2("", "EDGE", 0.960865149149924, 0.497807163546383, 0.0131011390528215, true, 1, null, 0);
            //status = swModelDocExt.SelectByID2("", "EDGE", 0.99866860703213, 0.481385806014544, 0.0113313929676906, true, 2, null, 0);

            //swModel.InsertLibraryFeature(libPath);

            #endregion 第二种方法(已过时)
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 modelDoc2 = (ModelDoc2)swApp.ActiveDoc;
            //SelectionMgr selectionMgr = modelDoc2.SelectionManager;

            //设置可选择类型的数组
            swSelectType_e[] filters = new swSelectType_e[1];

            //让用户只能选择实体

            filters[0] = swSelectType_e.swSelSOLIDBODIES;

            swApp.SetSelectionFilters(filters, true);
        }

        private void btn_DeleteConstraints_Click(object sender, EventArgs e)
        {
            //请先打开clamp1这个零件

            SldWorks swApp = Utility.ConnectToSolidWorks();
            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //选择草图
            swModel.Extension.SelectByID2("草图2", "SKETCH", 0, 0, 0, false, 4, null, 0);

            //进入编辑草图
            swModel.EditSketch();

            //获取当前草图对象
            Sketch swSketch = (Sketch)swModel.GetActiveSketch2();

            //获取该草图中的所有线
            object[] vSketchSeg = (object[])swSketch.GetSketchSegments();

            //定义选择
            SelectData swSelData = swSelMgr.CreateSelectData();

            SketchSegment swSketchSeg;
            //遍历线
            for (int i = 0; i < vSketchSeg.Length; i++)
            {
                swSketchSeg = (SketchSegment)vSketchSeg[i];

                swSketchSeg.Select4(false, swSelData);
                //删除关系
                swModel.SketchConstraintsDelAll();
            }

            object[] vSketchPt = (SketchPoint[])swSketch.GetSketchPoints2();
            SketchPoint swSketchPt;
            //遍历点
            for (int i = 0; i < vSketchPt.Length; i++)
            {
                swSketchPt = (SketchPoint)vSketchPt[i];
                swSketchPt.Select4(false, swSelData);
                swModel.SketchConstraintsDelAll();
            }
            //退出编辑草图
            swModel.InsertSketch2(true);

            swModel.ClearSelection2(true);
        }

        private void btnSelectNamedFace_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();
            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            #region 零件中选择

            PartDoc part1 = (PartDoc)swModel;
            //在零件中选择
            Face2 face1 = (Face2)part1.GetEntityByName("SFace1", (int)swSelectType_e.swSelFACES);
            Entity entity1 = (Entity)face1;
            entity1.Select(false);

            #endregion 零件中选择

            #region 装配中选择

            //这里我们默认该零件已经是选中装配,否则我们需要遍历一次零件,仅做示例

            Component2 component = (Component2)swSelMgr.GetSelectedObjectsComponent4(1, -1);

            swModel.ClearSelection();

            ModelDoc2 modelDoc = (ModelDoc2)component.GetModelDoc2();

            //转换为PartDoc
            PartDoc part = (PartDoc)modelDoc;

            Face2 face = (Face2)part.GetEntityByName("SFace1", (int)swSelectType_e.swSelFACES);

            Entity entity = (Entity)face;

            //在装配中再转换成装配中的实体
            Entity entityInComp = (Entity)component.GetCorrespondingEntity(entity);

            entityInComp.Select(false);

            #endregion 装配中选择
        }

        private void Btn_T_sketchsegment_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            SelectionMgr swSelMgr = default(SelectionMgr);
            Feature swFeature = default(Feature);
            string fileName = null;
            bool status = false;
            int errors = 0;
            int warnings = 0;

            //打开文件
            fileName = "C:\\Users\\Public\\Documents\\SOLIDWORKS\\SOLIDWORKS 2018\\samples\\tutorial\\tolanalyst\\offset\\top_plate.sldprt";
            swModel = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            swModelDocExt = (ModelDocExtension)swModel.Extension;

            //选中草图
            status = swModelDocExt.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, 0);

            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            //转换
            swFeature = (Feature)swSelMgr.GetSelectedObject6(1, -1);
            //进入编辑草图
            swModel.EditSketch();
            //获取草图中的所有线
            var sk = (Sketch)swFeature.GetSpecificFeature2();
            object[] vSketchSeg = (object[])sk.GetSketchSegments();

            SketchSegment swSketchSeg;
            double totalLenth = 0;
            foreach (var tempSeg in vSketchSeg)
            {
                swSketchSeg = (SketchSegment)tempSeg;
                //这里判断 不是文本,并且不是中心线 则加入长度
                if (swSketchSeg.GetType() != (int)swSketchSegments_e.swSketchTEXT && swSketchSeg.ConstructionGeometry == false)
                {
                    totalLenth = totalLenth + swSketchSeg.GetLength();
                }
            }

            swModel.EditSketch();
            //显示总长
            swApp.SendMsgToUser("Total Length:" + totalLenth * 1000);
        }

        private ModelDoc2 m_RefDoc; //增加第三方数据流 共用模型.

        private void btn_ThridData_Click(object sender, EventArgs e)
        {
            //https://www.codestack.net/solidworks-api/data-storage/third-party/embed-file/
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);

            swModel = (ModelDoc2)swApp.ActiveDoc;
            m_RefDoc = swModel;

            switch (swModel.GetType())
            {
                case (int)swDocumentTypes_e.swDocPART:
                    (swModel as PartDoc).SaveToStorageNotify += new DPartDocEvents_SaveToStorageNotifyEventHandler(OnSaveToStorage);
                    break;

                case (int)swDocumentTypes_e.swDocASSEMBLY:
                    (swModel as AssemblyDoc).SaveToStorageNotify += new DAssemblyDocEvents_SaveToStorageNotifyEventHandler(OnSaveToStorage);
                    break;
            }

            swModel.SetSaveFlag();
            swApp.SendMsgToUser("请手动保存文件!这样会把数据流写入文件中.");
        }

        private int OnSaveToStorage()
        {
            IStream iStr = (IStream)m_RefDoc.IGet3rdPartyStorage("Tool.Name", true);

            using (ComStream comStr = new ComStream(iStr))
            {
                byte[] data = Encoding.Unicode.GetBytes("Paine's Tool");
                comStr.Write(data, 0, data.Length);
            }

            m_RefDoc.IRelease3rdPartyStorage("Tool.Name");

            return 0;
        }

        private void btn_LoadThrid_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            // ModelDoc2 swModel = default(ModelDoc2);

            IModelDoc2 doc = swApp.IActiveDoc2;
            ISelectionMgr selMgr = doc.ISelectionManager;
            //  IComponent2 comp = selMgr.GetSelectedObjectsComponent3(1, -1);

            IStream iStr = (IStream)doc.IGet3rdPartyStorage("Tool.Name", false);

            if (iStr != null)
            {
                using (ComStream comStr = new ComStream(iStr))
                {
                    byte[] data = new byte[comStr.Length];
                    comStr.Read(data, 0, (int)comStr.Length);

                    string strData = Encoding.Unicode.GetString(data);
                    MessageBox.Show(strData);
                }

                doc.IRelease3rdPartyStorage("Tool.Name");
            }
        }

        private void btn_Tips_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);

            Frame swFrame = (Frame)swApp.Frame();

            swFrame.SetStatusBarText("这里是提示信息-->");

            swApp.SendMsgToUser("下面提示显示进度条:");

            UserProgressBar userProgressBar;

            swApp.GetUserProgressBar(out userProgressBar);

            userProgressBar.Start(0, 100, "Status");

            int Position = 0;

            for (int i = 0; i <= 100; i++)
            {
                Position = i * 10;

                if (Position == 100)
                {
                    Position = 0;
                    break;
                }

                var lRet = userProgressBar.UpdateProgress(Position);
                userProgressBar.UpdateTitle("当前进度--->" + Position);

                swApp.SendMsgToUser("当前进度--->" + Position);
            }

            userProgressBar.End();
        }

        #region 高级选择

        private void btn_Adv_Select_Click(object sender, EventArgs e)
        {
            //请先打开C:\Users\Public\Documents\SOLIDWORKS\SOLIDWORKS 2018\samples\tutorial\api\landing_gear.sldasm

            //参考资料为API 帮助中的 Use Advanced Component Selection Example (C#)

            SldWorks swApp = Utility.ConnectToSolidWorks();
            ModelDoc2 swModel = default(ModelDoc2);

            swModel = (ModelDoc2)swApp.ActiveDoc;

            int DocType = 0;
            DocType = swModel.GetType();

            if (DocType != (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                swApp.SendMsgToUser("当前不是装配体!");
                return;
            }

            AdvancedSelectionCriteria advancedSelectionCriteria = default(AdvancedSelectionCriteria);

            AssemblyDoc assemblyDoc = (AssemblyDoc)swModel;

            advancedSelectionCriteria = assemblyDoc.GetAdvancedSelection();

            int count = advancedSelectionCriteria.GetItemCount();

            //清空选择条件
            for (int i = 0; i < advancedSelectionCriteria.GetItemCount(); i++)
            {
                advancedSelectionCriteria.DeleteItem(i);
            }

            //增加选择条件 : 文件名包含lnk   具体的其它组合条件请看API帮助
            advancedSelectionCriteria.AddItem("Document name -- SW Special", 16, "lnk", false);
            //增加选择条件(或者) : 文件名包含hub
            advancedSelectionCriteria.AddItem("Document name -- SW Special", 16, "hub", false);

            //解释当前的选择条件
            ReportAllValues(advancedSelectionCriteria);

            //选择
            var SelectSuccess = advancedSelectionCriteria.Select();

            if (SelectSuccess == true)//选择成功
            {
                SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;
                Component2 swComp;
                //遍历已经选择的零件
                for (int j = 0; j < selectionMgr.GetSelectedObjectCount(); j++)
                {
                    swComp = (Component2)selectionMgr.GetSelectedObject6(j + 1, 0);

                    swModel = (ModelDoc2)swComp.GetModelDoc2();

                    //显示文件名
                    Debug.Print(swModel.GetPathName());
                }
            }
        }

        public string GetStringFromEnum(int EnumVal)
        {
            string functionReturnValue = null;
            //From enum swAdvSelecType_e
            if (EnumVal == 1)
            {
                functionReturnValue = "And";
            }
            else if (EnumVal == 2)
            {
                functionReturnValue = "Or";
            }
            else if (EnumVal == 16384)
            {
                functionReturnValue = "is yes";
            }
            else if (EnumVal == 32768)
            {
                functionReturnValue = "is no";
            }
            else if (EnumVal == 8)
            {
                functionReturnValue = "is not";
            }
            else if (EnumVal == 16)
            {
                functionReturnValue = "contains";
            }
            else if (EnumVal == 32)
            {
                functionReturnValue = "Is_Contained_By";
            }
            else if (EnumVal == 64)
            {
                functionReturnValue = "Interferes_With";
            }
            else if (EnumVal == 128)
            {
                functionReturnValue = "Does_Not_Interferes_With";
            }
            else if (EnumVal == 4)
            {
                functionReturnValue = "is (exactly)";
            }
            else if (EnumVal == 8192)
            {
                functionReturnValue = "not =";
            }
            else if (EnumVal == 512)
            {
                functionReturnValue = "<";
            }
            else if (EnumVal == 2048)
            {
                functionReturnValue = "<=";
            }
            else if (EnumVal == 4096)
            {
                functionReturnValue = "=";
            }
            else if (EnumVal == 1024)
            {
                functionReturnValue = ">=";
            }
            else if (EnumVal == 256)
            {
                functionReturnValue = ">";
            }
            else
            {
                functionReturnValue = "Condition NOT found";
            }
            return functionReturnValue;
        }

        public void ReportAllValues(AdvancedSelectionCriteria AdvancedSelectionCriteria)
        {
            Debug.Print("");

            int Count = 0;
            Count = AdvancedSelectionCriteria.GetItemCount();
            Debug.Print("GetItemCount returned " + Count);

            int i = 0;
            string aProperty = "";
            int Condition = 0;
            string Value = "";
            bool IsAnd = false;
            int Rindex = 0;
            string ConditionString = null;
            string PrintString = null;

            string IndexFmt = null;
            string RindexFmt = null;
            string AndOrFmt = null;
            string PropertyFmt = null;
            string ConditionFmt = null;
            string ValueFmt = null;
            IndexFmt = "!@@@@@@@@";
            RindexFmt = "!@@@@@@@@@";
            AndOrFmt = "!@@@@@@@@@";
            PropertyFmt = "!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@";
            ConditionFmt = "!@@@@@@@@@@@@@@@";
            ValueFmt = "#.00";

            //Debug.Print
            PrintString = string.Format("Index", IndexFmt) + "     " + string.Format("Rindex", RindexFmt) + "  " + string.Format("And/Or", AndOrFmt) + "  " + string.Format("Property", PropertyFmt) + "                     " + string.Format("Condition", ConditionFmt) + "     " + string.Format("Value", ValueFmt);
            Debug.Print(PrintString);
            for (i = 0; i <= Count - 1; i++)
            {
                Rindex = AdvancedSelectionCriteria.GetItem(i, out aProperty, out Condition, out Value, out IsAnd);
                ConditionString = GetStringFromEnum(Condition);
                PrintString = string.Format(i.ToString(), IndexFmt) + "         " + string.Format(Rindex.ToString(), RindexFmt) + "       " + string.Format((IsAnd == false ? "OR" : "AND"), AndOrFmt) + "      " + string.Format(aProperty, PropertyFmt) + "  " + string.Format(ConditionString, ConditionFmt) + "  " + string.Format(Value, ValueFmt);
                Debug.Print(PrintString);
            }
            Debug.Print("");
        }

        #endregion 高级选择

        private void btnBounding_Click(object sender, EventArgs e)
        {
            //首先请打开一个零件.

            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            FeatureManager featureManager = swModel.FeatureManager;

            PartDoc partDoc = (PartDoc)swModel;
            //通过特征名字获取特征
            Feature feature = (Feature)partDoc.FeatureByName("Bounding Box");
            int longstatus;
            if (feature == null)//特征为null时将创建Bounding Box

            {
                feature = (Feature)featureManager.InsertGlobalBoundingBox((int)swGlobalBoundingBoxFitOptions_e.swBoundingBoxType_BestFit, true, false, out longstatus);
            }

            // 显示 Bounding Box sketch
            var b = swModel.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swViewDispGlobalBBox, true);

            //获取自动生成的属性值
            string str;
            string str2;
            string str3;
            string str4;
            IConfiguration configuration = (Configuration)swModel.GetActiveConfiguration();
            CustomPropertyManager manager2 = swModel.Extension.get_CustomPropertyManager(configuration.Name);

            manager2.Get3("Total Bounding Box Length", true, out str, out str2);
            manager2.Get3("Total Bounding Box Width", true, out str, out str3);
            manager2.Get3("Total Bounding Box Thickness", true, out str, out str4);

            swApp.SendMsgToUser($"size={str2}x{str3}x{str4}");
        }

        private void btn_Measure_Click(object sender, EventArgs e)
        {
            //请先打开../TemplateModel/Measure.SLDPRT  并选中保存的选择--SelMeasure
            //
            //返回指定草图中所有线的总长 请参考之前的遍历草图对象

            //下面的代码是获取零件的体积.
            //可以参考API帮助 的实例 Measure Selected Entities Example (C#)

            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;

            Measure swMeasure = (Measure)swModelDocExt.CreateMeasure();

            swMeasure.ArcOption = 0;

            bool status = swMeasure.Calculate(null);

            if (status)
            {
                swApp.SendMsgToUser((swMeasure.Distance * 1000).ToString());
            }
        }

        private void btn_GetMass_Click(object sender, EventArgs e)
        {
            // 获取质量属性可参考 Get Mass Properties of Visible and Hidden Components Example (C#)

            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;

            swModelDocExt.IncludeMassPropertiesOfHiddenBodies = false;
            int massStatus = 0;

            double[] massProperties = (double[])swModelDocExt.GetMassProperties(1, ref massStatus);
            if ((massProperties != null))
            {
                Debug.Print(" CenterOfMassX = " + massProperties[0]);
                Debug.Print(" CenterOfMassY = " + massProperties[1]);
                Debug.Print(" CenterOfMassZ = " + massProperties[2]);
                Debug.Print(" Volume = " + massProperties[3]);
                Debug.Print(" Area = " + massProperties[4]);
                Debug.Print(" Mass = " + massProperties[5]);
                Debug.Print(" MomXX = " + massProperties[6]);
                Debug.Print(" MomYY = " + massProperties[7]);
                Debug.Print(" MomZZ = " + massProperties[8]);
                Debug.Print(" MomXY = " + massProperties[9]);
                Debug.Print(" MomZX = " + massProperties[10]);
                Debug.Print(" MomYZ = " + massProperties[11]);
            }
            Debug.Print("-------------------------------");
        }

        private TaskpaneView taskpaneView;

        private void btn_Pane_Click(object sender, EventArgs e)
        {
            //注意: 这里只是显示自己的窗体到solidworks中,目前还是走的exe的方式 .
            //真正开发的时候应该在DLL中加入,这样速度会快很多.  exe读bom需要40s dll 只需要3秒左右.
            //获取当前程序所在路径
            string Dllpath = Path.GetDirectoryName(typeof(MyPane).Assembly.CodeBase).Replace(@"file:\", string.Empty);

            var imagePath = Path.Combine(Dllpath, "bomlist.bmp");

            SldWorks swApp = Utility.ConnectToSolidWorks();

            string toolTip;

            toolTip = "BOM List";

            //创建页面
            if (taskpaneView != null)
            {
                taskpaneView.DeleteView();
                Marshal.FinalReleaseComObject(taskpaneView);
                taskpaneView = null;
            }

            taskpaneView = swApp.CreateTaskpaneView2(imagePath, toolTip);

            MyPane myPane = new MyPane(swApp);

            myPane.Dock = DockStyle.Fill;
            // myPane.Show();

            //在页面中显示窗体(嵌入)

            taskpaneView.DisplayWindowFromHandlex64(myPane.Handle.ToInt64());
        }

        private void Btn_Filter_Load(object sender, EventArgs e)
        {
        }

        private void Btn_Filter_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //删除新加的控件
                // taskpaneView = null;
                taskpaneView?.DeleteView();
                Marshal.FinalReleaseComObject(taskpaneView);
                taskpaneView = null;
            }
            catch (Exception exception)
            {
            }
        }

        /// <summary>
        /// 返回运行时Dll的路径
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RegDllPath(string s)
        {
            try
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
            catch (Exception)
            {
                return "";
            }
        }



        private void btn_SetMaterial_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;
            string swMateDB = "";
            string tempMaterial = "";
            //获取现有材质
            tempMaterial = ((PartDoc)swModel).GetMaterialPropertyName2("", out swMateDB);

            MessageBox.Show($"当前零件材质为 {swMateDB} 中的 {tempMaterial} ");

            string configName = null;
            string databaseName = null;
            string newPropName = null;
            configName = "默认";
            databaseName = "SOLIDWORKS Materials";
            newPropName = "Beech";
            ((PartDoc)swModel).SetMaterialPropertyName2(configName, databaseName, newPropName);

            tempMaterial = ((PartDoc)swModel).GetMaterialPropertyName2("", out swMateDB);

            MessageBox.Show($"修改之后  当前零件材质为 {swMateDB} 中的 {tempMaterial} ");
        }

        private void btnSetColor_Click(object sender, EventArgs e)
        {
            //首先选择一个面.  点击按钮,将修改为红色.

            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;
            try
            {
                for (int i = 1; i <= selectionMgr.GetSelectedObjectCount(); i++)
                {
                    Face2 face2 = (Face2)selectionMgr.GetSelectedObject6(i, -1);
                    var vFaceProp = (object[])swModel.MaterialPropertyValues;

                    var vProps = (object[])face2.GetMaterialPropertyValues2(1, null);
                    vProps[0] = 1;
                    vProps[1] = 0;
                    vProps[2] = 0;
                    vProps[3] = vFaceProp[3];
                    vProps[4] = vFaceProp[4];
                    vProps[5] = vFaceProp[5];
                    vProps[6] = vFaceProp[6];
                    vProps[7] = vFaceProp[7];
                    vProps[8] = vFaceProp[8];

                    face2.SetMaterialPropertyValues2(vProps, 1, null);
                    vProps = null;

                    vFaceProp = null;
                }

                swModel.ClearSelection2(true);
            }
            catch (Exception)
            {
                MessageBox.Show("请选择面,其它类型无效!");
            }
        }

        private void Btn_ReplacePart_Click(object sender, EventArgs e)
        {
            //首先打开 TempAssembly.sldasm
            //运行后,程序会把装配体中的Clamp1零件替换成Clamp2

            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;

            AssemblyDoc assemblyDoc = (AssemblyDoc)swModel;

            //替换为同目录下的clamp2
            string ReplacePartPath = Path.GetDirectoryName(swModel.GetPathName()) + @"\clamp2.sldprt";

            bool boolstatus;

            //选择当前的clamp1
            boolstatus = swModelDocExt.SelectByID2("clamp1-1@TempAssembly", "COMPONENT", 0, 0, 0, false, 0, null, 0);

            boolstatus = assemblyDoc.ReplaceComponents2(ReplacePartPath, "", false, 0, true);

            if (boolstatus == true)
            {
                MessageBox.Show("替换完成!");
            }
        }

        private void btn_Add_CenterPoint_Click(object sender, EventArgs e)
        {
            //Open CenterPoint.SLDPRT

            AddCenterPointForSketch addCenterPointForSketch = new AddCenterPointForSketch();

            addCenterPointForSketch.CreateHeaterCenter("CenterLine");

            MessageBox.Show("中心点创建完成!");
        }

        private void btnInsertNote_Click(object sender, EventArgs e)
        {
            FrmNote frmNote = new FrmNote();

            frmNote.Show();
        }

        private void btnPackFile_Click(object sender, EventArgs e)
        {
            FrmCopy frmCopy = new FrmCopy(@"C:\TempAssembly.sldasm", @"D:\CopyTest.sldasm");

            frmCopy.Show();
        }

        private void btn_SelectByRay_Click(object sender, EventArgs e)
        {
            //连接到Solidworks
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            Face2 swSelFace = default(Face2);
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            Component2 selectPartComponent2 = swSelMgr.GetSelectedObjectsComponent3(1, -1);

            //获取选择数据
            SelectData swSelData = default(SelectData);

            swSelData = swSelMgr.CreateSelectData();

            swSelFace = (Face2)swSelMgr.GetSelectedObject6(1, 0);

            var t = (double[])swSelFace.Normal;  //这里是零件中面的向量

            object vVectort = null;

            vVectort = t;

            var swMathUtilT = (MathUtility)swApp.GetMathUtility();

            MathVector tVector1 = (MathVector)swMathUtilT.CreateVector((vVectort));
            if (selectPartComponent2 != null)
            {
                var swNormalVector = (MathVector)tVector1.MultiplyTransform(selectPartComponent2.Transform2);

                var t2 = (double[])swNormalVector.ArrayData;
                //获取屏幕鼠标选择的那个点
                var mousePoint = (double[])swSelMgr.GetSelectionPoint2(1, 0);  //装配中

                swModel.ClearSelection2(true);

                //创建Ray选择

                mousePoint[0] = mousePoint[0] - t2[0] * 0.01;
                mousePoint[1] = mousePoint[1] - t2[1] * 0.01;
                mousePoint[2] = mousePoint[2] - t2[2] * 0.01;

                var boolstatus = swModel.Extension.SelectByRay(mousePoint[0], mousePoint[1], mousePoint[2], t2[0], t2[1], t2[2], 0.1, 2, false, 0, 0);

                if (boolstatus == true)
                {
                    MessageBox.Show("选择完成!");
                }
            }
            else
            {
                var t2 = t;
                //获取屏幕鼠标选择的那个点
                var mousePoint = (double[])swSelMgr.GetSelectionPoint2(1, 0);  //装配中

                swModel.ClearSelection2(true);

                //创建Ray选择

                mousePoint[0] = mousePoint[0] - t2[0] * 0.01;
                mousePoint[1] = mousePoint[1] - t2[1] * 0.01;
                mousePoint[2] = mousePoint[2] - t2[2] * 0.01;

                var boolstatus = swModel.Extension.SelectByRay(mousePoint[0], mousePoint[1], mousePoint[2], t2[0], t2[1], t2[2], 0.1, 2, false, 0, 0);

                if (boolstatus == true)
                {
                    MessageBox.Show("选择完成!");
                }
            }
        }

        private void GetDrawingModel_Click(object sender, EventArgs e)
        {
            //连接到Solidworks
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            // DrawingDoc dc = (DrawingDoc)swModel;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;

            //获取选择的视图对象
            View view = (View)selectionMgr.GetSelectedObject5(1);

            //获取视图中的引用模型
            var viewModel = view.ReferencedDocument;

            //其它读取属性请参考博文 读取零件属性 ->BtnGetPartData_Click

            MessageBox.Show(viewModel.GetPathName());

            //下面是导出，如果不需要请注释掉以下代码。
            int error = 0;
            int warnings = 0;

            var stepName = System.IO.Path.GetFileNameWithoutExtension(viewModel.GetPathName());

            var bRes = viewModel.Extension.SaveAs($@"D:\{stepName}.step", (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref error, ref warnings);

            if (bRes == true)
            {
                MessageBox.Show("Export step Done!");
            }
            else
            {
                MessageBox.Show("Export Error!");
            }
        }

        private void btn_Part_Export_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ExportForm exportForm = new ExportForm(swApp);

            exportForm.Show();
        }

        private void btn_Scale_Click(object sender, EventArgs e)
        {
            FrmScreen frmScreen = new FrmScreen();

            frmScreen.Show();
        }

        private void btn_Transform_PartToAsm_Click(object sender, EventArgs e)
        {
            //连接到Solidworks

            //这个例子是把零件中的一个基准轴 的两个点的坐标转换到装配体中

            //请打开装配体，并在某个零件下选择一下基准轴

            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr swSelMgr = swModel.ISelectionManager;

            Feature swFeat = (Feature)swSelMgr.GetSelectedObject6(1, 0);

            String sAxisName = swFeat.Name;

            RefAxis RefAxis = (RefAxis)swFeat.GetSpecificFeature2();

            var vParam = (object[])RefAxis.GetRefAxisParams();

            Component2 inletPart = (Component2)swSelMgr.GetSelectedObjectsComponent4(1, 0);

            double[] nPt = new double[3];
            double[] nPt2 = new double[3];

            double[] vPt = new double[3];
            double[] vPt2 = new double[3];

            nPt[0] = (double)vParam[0]; nPt[1] = (double)vParam[1]; nPt[2] = (double)vParam[2];
            nPt2[0] = (double)vParam[3]; nPt2[1] = (double)vParam[4]; nPt2[2] = (double)vParam[5];

            vPt = nPt;
            vPt2 = nPt2;

            MathUtility swMathUtil = (MathUtility)swApp.GetMathUtility();

            MathTransform mathTransform = inletPart.Transform2;

            MathTransform swXform = (MathTransform)mathTransform;

            MathPoint swMathPt = (MathPoint)swMathUtil.CreatePoint((vPt));

            MathPoint swMathPt2 = (MathPoint)swMathUtil.CreatePoint((vPt2));

            //swXform.Inverse(); 反转的话就是把装配体中的点坐标转到零件对应的坐标系统中

            swMathPt = (MathPoint)swMathPt.MultiplyTransform(swXform);

            swMathPt2 = (MathPoint)swMathPt2.MultiplyTransform(swXform);

            var swStartPtArrayData = (double[])swMathPt.ArrayData;
            var swEndPtArrayData = (double[])swMathPt2.ArrayData;

            var x = swStartPtArrayData[0];
            var y = swStartPtArrayData[1];
            var z = swStartPtArrayData[2];
            var x2 = swEndPtArrayData[0];
            var y2 = swEndPtArrayData[1];
            var z2 = swEndPtArrayData[2];

            var v1 = x2 - x;
            var v2 = y2 - y;
            var v3 = z2 - z;

            if (Math.Round(v3, 4) != 0 && Math.Round(v1, 4) == 0 && Math.Round(v2, 4) == 0)
            {
                MessageBox.Show("此轴在Z方向上");
            }

            //  MathVector mathVector = new MathVector();
        }

        private void btn_Insert_Block_Click(object sender, EventArgs e)
        {
            //连接到Solidworks
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            DrawingDoc dc = (DrawingDoc)swModel;

            // SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;

            double[] nPt = new double[3];

            nPt[0] = 0;
            nPt[1] = 0;
            nPt[2] = 0;

            MathUtility swMathUtil = (MathUtility)swApp.GetMathUtility();

            MathPoint swMathPoint = (MathPoint)swMathUtil.CreatePoint(nPt);

            double blockScale = 1;

            string blockPath = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\TestBlock.SLDBLK";

            //插入图块
            var swBlockInst = InsertBlockReturnInst(swApp, swModel, swMathPoint, blockPath, blockScale);

            //  swModel.SketchManager.MakeSketchBlockFromFile(mathPoint, blockPath, false, blockScale, 0);

            //修改块的属性(如果只是普通的图块，则无需要这一步。直接使用上面的一行插入图块即可)
            swBlockInst.SetAttributeValue("Title1", "Paine");
        }

        /// <summary>
        /// 插入并返回最后一个图块实例
        /// </summary>
        /// <param name="sldWorks"></param>
        /// <param name="modelDoc2"></param>
        /// <param name="mathPoint"></param>
        /// <param name="blockPath"></param>
        /// <param name="blockScale"></param>
        /// <returns></returns>
        private SketchBlockInstance InsertBlockReturnInst(ISldWorks sldWorks, ModelDoc2 modelDoc2, MathPoint mathPoint, String blockPath, double blockScale)
        {
            SketchBlockInstance swBlockInst;
            List<String> NowBlockName = new List<String>();
            var swModel = modelDoc2;
            Boolean boolstatus = swModel.Extension.SelectByID2(System.IO.Path.GetFileNameWithoutExtension(blockPath), "SUBSKETCHDEF", 0, 0, 0, false, 0, null, 0);

            if (boolstatus == true)
            {
                var selMgr = (SelectionMgr)swModel.SelectionManager;
                Feature swFeat = (Feature)selMgr.GetSelectedObject6(1, 0);
                var swSketchBlockDef = (SketchBlockDefinition)swFeat.GetSpecificFeature2();

                var nbrBlockInst = swSketchBlockDef.GetInstanceCount();

                if (nbrBlockInst > 0)
                {
                    var vBlockInst = (object[])swSketchBlockDef.GetInstances();

                    for (int i = 0; i < nbrBlockInst; i++)
                    {
                        swBlockInst = (SketchBlockInstance)vBlockInst[i];

                        NowBlockName.Add(swBlockInst.Name.ToString());
                    }

                    swModel.SketchManager.MakeSketchBlockFromFile(mathPoint, blockPath, false, blockScale, 0);

                    swModel.ClearSelection2(true);

                    boolstatus = swModel.Extension.SelectByID2(System.IO.Path.GetFileNameWithoutExtension(blockPath), "SUBSKETCHDEF", 0, 0, 0, false, 0, null, 0);

                    swFeat = (Feature)selMgr.GetSelectedObject6(1, 0);

                    swSketchBlockDef = (SketchBlockDefinition)swFeat.GetSpecificFeature2();

                    nbrBlockInst = swSketchBlockDef.GetInstanceCount();

                    if (nbrBlockInst > 0)
                    {
                        vBlockInst = (object[])swSketchBlockDef.GetInstances();

                        for (int j = 0; j < nbrBlockInst; j++)
                        {
                            swBlockInst = (SketchBlockInstance)vBlockInst[j];
                            if (!NowBlockName.Contains(swBlockInst.Name.ToString()))
                            {
                                swModel.Extension.SelectByID2(swBlockInst.Name, "SUBSKETCHINST", 0, 0, 0, false, 0, null, 0);
                                swBlockInst = GetSketchBlockInstanceFromSelection();
                                return swBlockInst;
                            }
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var swSketchBlockDef = swModel.SketchManager.MakeSketchBlockFromFile(mathPoint, blockPath, false, blockScale, 0);

                var vBlockInst = (object[])swSketchBlockDef.GetInstances();

                swBlockInst = (SketchBlockInstance)vBlockInst[0];

                return swBlockInst;
            }
        }

        private SketchBlockInstance GetSketchBlockInstanceFromSelection()
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel;
            ModelDocExtension swModelDocExt;
            SketchBlockInstance SketchBlockInstance;

            DateTime time = DateTime.Now;

            try
            {
                swModel = (ModelDoc2)swApp.ActiveDoc;
                swModelDocExt = swModel.Extension;

                SelectionMgr swSelectionMgr;
                swSelectionMgr = (SelectionMgr)swModel.SelectionManager;

                string SelectByString = "";
                string ObjectType = "";
                int type;
                double x;
                double y;
                double z;

                if (swSelectionMgr.GetSelectedObjectCount2(-1) > 1)
                {
                    // Return only a SketchblockInstance when only one is selected...
                    // modify if you want return more than one (or only the first) selected Sketchblockinstance
                    return null;
                }

                swSelectionMgr.GetSelectionSpecification(1, out SelectByString, out ObjectType, out type, out x, out y, out z);
                Debug.WriteLine(SelectByString + " " + ObjectType + " " + type);

                if (type == (int)swSelectType_e.swSelSUBSKETCHINST)
                {
                    SketchBlockInstance = (SketchBlockInstance)swSelectionMgr.GetSelectedObject6(1, -1);
                    Debug.WriteLine("Found:" + SketchBlockInstance.Name);
                    return SketchBlockInstance;
                }
                else if (type == (int)swSelectType_e.swSelSKETCHSEGS | type == (int)swSelectType_e.swSelSKETCHPOINTS)
                {
                    // Show if a sketchblockinstance has the same name
                    SketchManager SwSketchMgr;
                    SwSketchMgr = swModel.SketchManager;

                    object[] blockDefinitions = (object[])SwSketchMgr.GetSketchBlockDefinitions();
                    foreach (SketchBlockDefinition blockDef in blockDefinitions)
                    {
                        foreach (SketchBlockInstance blockInstance in (SketchBlockInstance[])blockDef.GetInstances())
                        {
                            if (SelectByString.EndsWith(blockInstance.Name))
                            {
                                Debug.WriteLine("Found:" + blockInstance.Name);
                                return blockInstance;
                            }
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Debug.WriteLine(DateTime.Now.Subtract(time).Milliseconds);
            }

            return null;
        }

        private void btn_setcolor_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                Configuration swConf = (Configuration)swModel.GetActiveConfiguration();

                Component2 swRootComp = (Component2)swConf.GetRootComponent();

                //遍历

                Utility.TraverseCompXform(swRootComp, 0, true);

                swModel.WindowRedraw();

                swModel.EditRebuild3();
            }
        }

        private void butGlobalVariables_Click(object sender, EventArgs e)
        {
            //连接solidworks

            int errors = 0;
            int warnings = 0;

            SldWorks swApp = Utility.ConnectToSolidWorks();
            //swApp.OpenDoc6(@"E:\01_Work\22_Gitee\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\globalvariable.SLDPRT", 1, 0, "", errors, warnings);
            if (swApp != null)
            {
                //获取当前模型
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
                //定义方程式管理器
                EquationMgr swEqnMgr = default(EquationMgr);

                int i = 0;
                int nCount = 0;

                if (swModel != null)
                {
                    swEqnMgr = (EquationMgr)swModel.GetEquationMgr();
                    // nCount = swEqnMgr.GetCount();
                    //for (i = 0; i < nCount; i++)
                    //{
                    //    Debug.Print("  Equation(" + i + ")  = " + swEqnMgr.get_Equation(i));
                    //    Debug.Print("    Value = " + swEqnMgr.get_Value(i));
                    //    Debug.Print("    Index = " + swEqnMgr.Status);
                    //    Debug.Print("    Global variable? " + swEqnMgr.get_GlobalVariable(i));
                    //}

                    //修改高度为60

                    //var eq = @"""D1@Boass-Extrude1"" = ""h""";
                    //Todo: 无法增加方程式。
                    var eq = @"""aaaa""=18";

                    var addb = swEqnMgr.Add2(-1, eq, true);

                    if (SetEquationValue(swEqnMgr, "h", 60))
                    {
                        swModel.ForceRebuild3(true);
                    }
                    else
                    {
                        MessageBox.Show("没有找到这个值!");
                    }
                }
            }
        }

        #region 修改全局变量所用到的方法

        public bool SetEquationValue(EquationMgr eqMgr, string name, double newValue)
        {
            int index = GetEquationIndexByName(eqMgr, name);

            if (index != -1)
            {
                eqMgr.Equation[index] = "\"" + name + "\"=" + newValue;

                return true;
            }
            else
            {
                return false;
            }
        }

        //通过名字找方程式的位置
        private int GetEquationIndexByName(EquationMgr eqMgr, string name)
        {
            int i;
            for (i = 0; i <= eqMgr.GetCount() - 1; i++)
            {
                var eqName = eqMgr.Equation[i].Split('=')[0].Replace("=", "");

                eqName = eqName.Substring(1, eqName.Length - 2); // removing the "" symbols from the name

                if (eqName.ToUpper() == name.ToUpper())
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion 修改全局变量所用到的方法

        private void btnCreateSketch_Click(object sender, EventArgs e)
        {
            //如果没有打开文件，请执行打开和创建的操作：
            //BtnOpenAndNew_Click(null, null);

            //连接到Solidworks
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            //定义草图管理器
            SketchManager sketchManager = swModel.SketchManager;

            //按名字选择草图
            bool boolstatus = swModel.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, 0);

            if (boolstatus == true)
            {
                //编辑草图
                sketchManager.InsertSketch(false);
                //获取当前草图，当获取草图中的Segment对象
                Sketch sketch = (Sketch)swModel.GetActiveSketch2();
                object[] sketchSegments = (object[])sketch.GetSketchSegments();

                if (sketchSegments != null)
                {
                    //遍历
                    foreach (var skSeg in sketchSegments)
                    {
                        SketchSegment sketchSegment = (SketchSegment)skSeg;

                        //判断是直线时执行
                        if (sketchSegment.GetType() == (int)swSketchSegments_e.swSketchLINE)
                        {
                            SketchLine sketchLine = (SketchLine)sketchSegment;
                            SketchPoint sketchPointStart = (SketchPoint)sketchLine.GetStartPoint2();
                            SketchPoint sketchPointEnd = (SketchPoint)sketchLine.GetEndPoint2();

                            //这里显示弹出坐标，单位默认是米
                            MessageBox.Show(sketchPointStart.X.ToString() + "," + sketchPointStart.Y.ToString());
                            MessageBox.Show(sketchPointEnd.X.ToString() + "," + sketchPointEnd.Y.ToString());

                            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

                            //定义选择数据
                            SelectData swSelData = swSelMgr.CreateSelectData();

                            //选择此直线

                            sketchSegment.Select4(false, swSelData);

                            //删除当前的约束关系
                            swModel.SketchConstraintsDelAll();

                            //下在我们来修改坐标
                            sketchPointStart.X = 0.05;
                            sketchPointStart.Y = 0.04;

                            sketchPointEnd.X = 0.2;
                            sketchPointEnd.Y = 0.2;
                        }
                    }
                }

                //退出草图
                sketchManager.InsertSketch(true);
            }
        }

        private void btnSheetmetal_Click(object sender, EventArgs e)
        {
            //连接到Solidworks
            SldWorks swApp = Utility.ConnectToSolidWorks();
            swApp.CommandInProgress = true;
            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            //钣金 变成平板模式的特征
            List<Feature> flatPatternFeatures = new List<Feature>();

            //Bounding Box草图
            List<string> boundingSketchesName = new List<string>();

            //获取当前钣金状态--这个已经过时

            //swSMBendStateFlattened  2 = 弯曲变平；该模型回滚到FlattenBends功能之后，但恰好在相应的ProcessBends功能之前
            //swSMBendStateFolded 3 = 折弯处已折叠；模型回滚到FlattenBends ProcessBends功能对之后
            //swSMBendStateNone   0 = 不是钣金零件；没有SheetMetal功能
            //swSMBendStateSharps 1 = 弯曲处处于锐利状态；零件回滚到第一个FlattenBends功能之前

            var bendState = swModel.GetBendState();

            if (bendState == 0)
            {
                swApp.SendMsgToUser("不是钣金零件！");
                return;
            }
            //  swApp.SendMsgToUser("当前状态" + bendState);
            if (bendState != 2)
            {
                //swApp.Command((int)swCommands_e.swCommands_Flatten, "");
                //设定当前钣金状态 平板 ，下面这行代码不适用现在的零件 ，只适用于很早之前的零件
                //var setStatus = swModel.SetBendState((int)swSMBendState_e.swSMBendStateFlattened);

                //新钣金均是通过获取零件
                var swFeatureManager = swModel.FeatureManager;
                var flatPatternFolder = (FlatPatternFolder)swFeatureManager.GetFlatPatternFolder();

                var featArray = (object[])flatPatternFolder.GetFlatPatterns();

                for (int i = featArray.GetLowerBound(0); i <= featArray.GetUpperBound(0); i++)
                {
                    var feat = (Feature)featArray[i];
                    Debug.Print("    " + feat.Name);

                    flatPatternFeatures.Add(feat);
                    feat.SetSuppression2((int)swFeatureSuppressionAction_e.swUnSuppressFeature, (int)swInConfigurationOpts_e.swThisConfiguration, null);

                    //解压子特征
                    var swSubFeat = (Feature)feat.GetFirstSubFeature();

                    while ((swSubFeat != null))
                    {
                        Debug.Print(swSubFeat.Name.ToString());
                        switch (swSubFeat.GetTypeName())
                        {
                            //如果是草图
                            case "ProfileFeature":

                                var sketchSpc = (Sketch)swSubFeat.GetSpecificFeature2();
                                object[] vSketchSeg = (object[])sketchSpc.GetSketchSegments();

                                for (int j = 0; j < vSketchSeg.Length; j++)
                                {
                                    SketchSegment swSketchSeg = (SketchSegment)vSketchSeg[j];

                                    //如果直线不是折弯线，说明是边界框
                                    if (swSketchSeg.IsBendLine() == false)
                                    {
                                        boundingSketchesName.Add(swSubFeat.Name);
                                    }
                                    else if (swSketchSeg.IsBendLine() == true)
                                    {
                                        Debug.Print("钣金宽度为:" + swSketchSeg.GetLength() * 1000);
                                    }
                                }

                                break;

                            default:
                                break;
                        }

                        swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
                    }
                }

                swModel.EditRebuild3();
            }

            //遍历所有特征

            var swSelMgr = (SelectionMgr)swModel.SelectionManager;
            var swFeat = (Feature)swModel.FirstFeature();

            while ((swFeat != null))
            {
                //Debug.Print(swFeat.Name.ToString());
                // Process top-level sheet metal features
                switch (swFeat.GetTypeName())
                {
                    case "SMBaseFlange":
                        //var swBaseFlange = (BaseFlangeFeatureData)swFeat.GetDefinition();

                        //Debug.Print("钣金宽度为:" + swBaseFlange.D1OffsetDistance * 1000);

                        break;

                    case "SheetMetal":
                        //这里可以获取默认的厚度                        Debug.Print(swFeat.Name.ToString());
                        SheetMetalFeatureData sheetMetalFeatureData = (SheetMetalFeatureData)swFeat.GetDefinition();
                        Debug.Print("钣金默认厚度为:" + sheetMetalFeatureData.Thickness * 1000);

                        break;

                    case "SM3dBend":

                        break;

                    case "SMMiteredFlange":

                        break;
                }
                // process sheet metal sub-features
                var swSubFeat = (Feature)swFeat.GetFirstSubFeature();

                while ((swSubFeat != null))
                {
                    // Debug.Print(swSubFeat.Name.ToString());
                    switch (swSubFeat.GetTypeName())
                    {
                        case "SketchBend":

                            GetHisBendInformation(swApp, swModel, swSubFeat);
                            break;

                        case "OneBend":

                            GetHisBendInformation(swApp, swModel, swSubFeat);

                            break;

                        default:
                            break;
                            // Probably not a sheet metal feature
                    }

                    swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
                }

                swFeat = (Feature)swFeat.GetNextFeature();
            }

            return;
        }

        private void GetHisBendInformation(ISldWorks swApp, ModelDoc2 swModel, Feature swFeat)
        {
            MathUtility swMathUtil = default(MathUtility);
            SelectionMgr swSelMgr = default(SelectionMgr);
            OneBendFeatureData swOneBend = default(OneBendFeatureData);
            Object[] vSketchSegs = null;
            SketchSegment swSketchSeg = default(SketchSegment);
            Sketch swSketch = default(Sketch);
            Feature swSketchFeat = default(Feature);
            SketchLine swSketchLine = default(SketchLine);
            SketchPoint swSkStartPt = default(SketchPoint);
            SketchPoint swSkEndPt = default(SketchPoint);
            SelectData swSelData = default(SelectData);
            double[] nPt = new double[3];
            MathPoint swStartPt = default(MathPoint);
            MathPoint swEndPt = default(MathPoint);
            MathTransform swSkXform = default(MathTransform);
            int[] vID = null;
            int i = 0;

            swMathUtil = (MathUtility)swApp.GetMathUtility();

            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            //swFeat = (Feature)swSelMgr.GetSelectedObject6(1, -1);
            //swSelData = swSelMgr.CreateSelectData();
            swOneBend = (OneBendFeatureData)swFeat.GetDefinition();

            /*swBaseBend 4
            swEdgeFlangeBend 8
            swFlat3dBend 6
            swFlatBend 2
            swFreeFormBend 10 = Obsolete
            swHemBend 9
            swLoftedBend 12
            swMirrorBend 7
            swMiterBend 5
            swNoneBend 3
            swRoundBend 1
            swRuledBend 11 = Obsolete
            swSharpBend 0
            */

            Debug.Print("Type of bend (swBendType_e): " + swOneBend.GetType());
            Debug.Print("折弯次数: " + swOneBend.GetFlatPatternSketchSegmentCount2());
            Debug.Print("折弯序号: " + swOneBend.BendOrder);
            Debug.Print("折弯角度: " + swOneBend.BendAngle * 57.3 + " deg");
            Debug.Print("折弯圆角: " + swOneBend.BendRadius);

            if (swOneBend.BendDown == true)
            {
                Debug.Print("向下折弯: " + "Yes");
            }
            else
            {
                Debug.Print("向下折弯: " + " No");
            }

            vSketchSegs = (Object[])swOneBend.FlatPatternSketchSegments2;

            for (i = 0; i <= vSketchSegs.GetUpperBound(0); i++)
            {
                swSketchSeg = (SketchSegment)vSketchSegs[i];
                swSketch = swSketchSeg.GetSketch();
                swSketchLine = (SketchLine)swSketchSeg;
                swSkStartPt = (SketchPoint)swSketchLine.GetStartPoint2();
                swSkEndPt = (SketchPoint)swSketchLine.GetEndPoint2();
                vID = (int[])swSketchSeg.GetID();

                // Get sketch feature
                swSketchFeat = (Feature)swSketch;
                swSkXform = swSketch.ModelToSketchTransform;
                swSkXform = (MathTransform)swSkXform.Inverse();

                nPt[0] = swSkStartPt.X;
                nPt[1] = swSkStartPt.Y;
                nPt[2] = swSkStartPt.Z;
                swStartPt = (MathPoint)swMathUtil.CreatePoint(nPt);
                swStartPt = (MathPoint)swStartPt.MultiplyTransform(swSkXform);
                double[] swStartPtArrayData;
                swStartPtArrayData = (double[])swStartPt.ArrayData;

                nPt[0] = swSkEndPt.X;
                nPt[1] = swSkEndPt.Y;
                nPt[2] = swSkEndPt.Z;
                swEndPt = (MathPoint)swMathUtil.CreatePoint(nPt);
                swEndPt = (MathPoint)swEndPt.MultiplyTransform(swSkXform);
                double[] swEndPtArrayData;
                swEndPtArrayData = (double[])swEndPt.ArrayData;

                // Debug.Print("File = " + swModel.GetPathName());
                Debug.Print("  Feature = " + swFeat.Name + " [" + swFeat.GetTypeName2() + "]");
                Debug.Print("    Sketch             = " + swSketchFeat.Name);
                Debug.Print("    SegID              = [" + vID[0] + ", " + vID[1] + "]");
                Debug.Print("    Start with respect to sketch   = (" + swSkStartPt.X * 1000.0 + ", " + swSkStartPt.Y * 1000.0 + ", " + swSkStartPt.Z * 1000.0 + ") mm");
                Debug.Print("    End with respect to sketch   = (" + swSkEndPt.X * 1000.0 + ", " + swSkEndPt.Y * 1000.0 + ", " + swSkEndPt.Z * 1000.0 + ") mm");
                Debug.Print("    Start with respect to model    = (" + swStartPtArrayData[0] * 1000.0 + ", " + swStartPtArrayData[1] * 1000.0 + ", " + swStartPtArrayData[2] * 1000.0 + ") mm");
                Debug.Print("    End with respect to model    = (" + swEndPtArrayData[0] * 1000.0 + ", " + swEndPtArrayData[1] * 1000.0 + ", " + swEndPtArrayData[2] * 1000.0 + ") mm");
            }
        }

        private void btnGetDimensionInfo_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();
            swApp.CommandInProgress = true;

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;

            //转换成尺寸显示对象
            var swDisplayDimension = (DisplayDimension)selectionMgr.GetSelectedObject6(1, 0);

            DisplayData displayData = (DisplayData)swDisplayDimension.GetDisplayData();

            //获取尺寸上的文字
            var anno = (Annotation)swDisplayDimension.GetAnnotation();

            //获取所在视图  ---如果是图纸，这里会报错。需要用OwnerType来判断
            var thisView = (View)anno.Owner;//

            var textwidth = displayData.GetTextInBoxWidthAtIndex(0);

            var textHeight = displayData.GetTextHeightAtIndex(0);

            // dat.GetLineCount 几条线
            var lineCount = displayData.GetLineCount();
            var lineAngle = displayData.GetTextAngleAtIndex(0);
            var linePoints = (double[])displayData.GetLineAtIndex(0);
            var linePoints2 = (double[])displayData.GetLineAtIndex(1);
            var textPoint = (double[])displayData.GetTextPositionAtIndex(0);

            var thisDimAng = lineAngle * 180 / Math.PI;

            //尺寸对象
            var swDimension = (Dimension)swDisplayDimension.GetDimension();

            //获取尺寸的公差
            var cruToleranceType = swDimension.GetToleranceType();
            var cruTolerance = swDimension.Tolerance;

            if (cruToleranceType == (int)swTolType_e.swTolBILAT)
            {
                cruTolerance.GetMaxValue2(out double ToleranceValueMax); //上公差

                cruTolerance.GetMinValue2(out double ToleranceValueMin);//下公差
            }

            var TextAll = swDisplayDimension.GetText((int)swDimensionTextParts_e.swDimensionTextAll);
            var TextPrefix = swDisplayDimension.GetText((int)swDimensionTextParts_e.swDimensionTextPrefix);
            var TextSuffix = swDisplayDimension.GetText((int)swDimensionTextParts_e.swDimensionTextSuffix);
            var CalloutAbove = swDisplayDimension.GetText((int)swDimensionTextParts_e.swDimensionTextCalloutAbove);
            var CalloutBelow = swDisplayDimension.GetText((int)swDimensionTextParts_e.swDimensionTextCalloutBelow);

            var relValue = Math.Round(swDimension.GetSystemValue2("") * 1000, 3).ToString();

            MessageBox.Show(relValue);
        }

        private void btnLayoutMgr_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var swLayerMgr = (LayerMgr)swModel.GetLayerManager();

            //获取当前图层数量
            var layCount = swLayerMgr.GetCount();

            var layerList = (String[])swLayerMgr.GetLayerList();

            foreach (var lay in layerList)
            {
                var currentLayer = (Layer)swLayerMgr.GetLayer(lay);
                if (currentLayer != null)
                {
                    var currentName = currentLayer.Name;
                    //颜色的Ref值
                    var currentColor = currentLayer.Color;
                    var currentDesc = currentLayer.Description;

                    //swLineStyles_e 对应的值
                    var currentStype = Enum.GetName(typeof(swLineStyles_e), currentLayer.Style);

                    var currentWidth = currentLayer.Width;

                    int refcolor = currentColor;
                    int blue = refcolor >> 16 & 255;
                    int green = refcolor >> 8 & 255;
                    int red = refcolor & 255;
                    int colorARGB = 255 << 24 | (int)red << 16 | (int)green << 8 | (int)blue;

                    //得到对应的RGB值
                    Color ARGB = Color.FromArgb(colorARGB);  //得到结果

                    Debug.Print($"图层名称：{currentName}");
                    Debug.Print($"图层颜色：R {ARGB.R},G {ARGB.G} ,B {ARGB.B}");
                    Debug.Print($"图层描述：{currentDesc}");
                    Debug.Print($"图层线型：{currentStype}");
                    Debug.Print($"-------------------------------------");
                }
            }

            //下面来建图层。

            var swDrawing = (DrawingDoc)swModel;

            // var colorString = "Purple";
            Color color = Color.Purple; //System.Drawing.ColorTranslator.FromHtml(colorString); 如果是字符串可以通过这转
            //给定的
            int colorInt = color.ToArgb();
            int red2 = colorInt >> 16 & 255;
            int green2 = colorInt >> 8 & 255;
            int blue2 = colorInt & 255;
            int refcolor2 = (int)blue2 << 16 | (int)green2 << 8 | (int)red2;

            var bRes = swDrawing.CreateLayer2("NewPurple", "New Purple Layout ", (int)refcolor2, (int)swLineStyles_e.swLineCONTINUOUS, (int)swLineWeights_e.swLW_NORMAL, true, true);

            if (bRes == true)
            {
                Debug.Print($"图层已经创建");
            }
        }

        private void btnGetPreview_Click(object sender, EventArgs e)
        {
            // SldWorks swApp = Utility.ConnectToSolidWorks();

            string fileName = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\bodies.sldasm";
            string bitmapPathName = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\bodies.bmp";

            #region 第一种 solidworks的GetPreviewBitmapFile

            //此处路径请自己确保存在。

            //string configName = "Default";

            //

            //var status = swApp.GetPreviewBitmapFile(fileName, configName, bitmapPathName);

            //if (System.IO.File.Exists(bitmapPathName))
            //{
            //    swApp.SendMsgToUser("预览图获取完成。");
            //}

            #endregion 第一种 solidworks的GetPreviewBitmapFile

            #region 第二种 系统获取

            //string path = ThumbnailHelper.GetInstance().GetJPGThumbnail(fileName);

            ////这里自己再调用窗口预览就可以了
            //Debug.Print(path);

            #endregion 第二种 系统获取

            #region 第三种 DocumentMgr

            //2018版能用的key 仅供测试， 来源于其它软件。

            const string sLicenseKey = "Axemble:swdocmgr_general-11785-02051-00064-50177-08535-34307-00007-37408-17094-12655-31529-39909-49477-26312-14336-58516-10910-42487-02022-02562-54862-24526-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-7,swdocmgr_previews-11785-02051-00064-50177-08535-34307-00007-48008-04931-27155-53105-52081-64048-22699-38918-23742-63202-30008-58372-23951-37726-23245-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-1,swdocmgr_dimxpert-11785-02051-00064-50177-08535-34307-00007-16848-46744-46507-43004-11310-13037-46891-59394-52990-24983-00932-12744-51214-03249-23667-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-8,swdocmgr_geometry-11785-02051-00064-50177-08535-34307-00007-39720-42733-27008-07782-55416-16059-24823-59395-22410-04359-65370-60348-06678-16765-23356-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-3,swdocmgr_xml-11785-02051-00064-50177-08535-34307-00007-51816-63406-17453-09481-48159-24258-10263-28674-28856-61649-06436-41925-13932-52097-22614-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-7,swdocmgr_tessellation-11785-02051-00064-50177-08535-34307-00007-13440-59803-19007-55358-48373-41599-14912-02050-07716-07769-29894-19369-42867-36378-24376-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-0";//如果正版用户，请联系代理商申请。

            string sDocFileName = fileName;

            SwDMClassFactory swClassFact = default(SwDMClassFactory);
            SwDMApplication swDocMgr = default(SwDMApplication);
            SwDMDocument swDoc = default(SwDMDocument);
            SwDMDocument10 swDoc10 = default(SwDMDocument10);
            SwDmDocumentType nDocType = 0;
            SwDmDocumentOpenError nRetVal = 0;
            SwDmPreviewError nError = 0;

            // Determine type of SOLIDWORKS file based on file extension
            if (sDocFileName.EndsWith("sldprt"))
            {
                nDocType = SwDmDocumentType.swDmDocumentPart;
            }
            else if (sDocFileName.EndsWith("sldasm"))
            {
                nDocType = SwDmDocumentType.swDmDocumentAssembly;
            }
            else if (sDocFileName.EndsWith("slddrw"))
            {
                nDocType = SwDmDocumentType.swDmDocumentDrawing;
            }
            else
            {
                // Probably not a SOLIDWORKS file,
                // so cannot open
                nDocType = SwDmDocumentType.swDmDocumentUnknown;
                return;
            }

            swClassFact = new SwDMClassFactory();
            swDocMgr = (SwDMApplication)swClassFact.GetApplication(sLicenseKey);
            swDoc = (SwDMDocument)swDocMgr.GetDocument(sDocFileName, nDocType, true, out nRetVal);
            Debug.Print("File = " + swDoc.FullName);
            Debug.Print("  Version          = " + swDoc.GetVersion());
            Debug.Print("  Author           = " + swDoc.Author);
            Debug.Print("  Comments         = " + swDoc.Comments);
            Debug.Print("  CreationDate     = " + swDoc.CreationDate);
            Debug.Print("  Keywords         = " + swDoc.Keywords);
            Debug.Print("  LastSavedBy      = " + swDoc.LastSavedBy);
            Debug.Print("  LastSavedDate    = " + swDoc.LastSavedDate);
            Debug.Print("  Subject          = " + swDoc.Subject);
            Debug.Print("  Title            = " + swDoc.Title);

            swDoc10 = (SwDMDocument10)swDoc;
            // SwDMDocument10::GetPreviewBitmap throws an unmanaged COM exception
            // for out-of-process C# console applications
            // Use the following code in SOLIDWORKS C# macros and add-ins
            object objBitMap = swDoc10.GetPreviewBitmap(out nError);
            System.Drawing.Image imgPreview = PictureDispConverter.Convert(objBitMap);
            imgPreview.Save(bitmapPathName, System.Drawing.Imaging.ImageFormat.Bmp);
            imgPreview.Dispose();

            Debug.Print("    Preview stream   = " + swDoc10.PreviewStreamName);

            #endregion 第三种 DocumentMgr
        }

        /// <summary>
        /// 删除零件特征,但保留实体,类似于导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteFeature_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            if (swModel != null)
            {
                PartDoc part = (PartDoc)swModel;
                var vBodies = GetBodyCopies(part);

                DeleteAllUserFeature(swModel);

                CreateFeatureForBodies(part, vBodies);
            }
        }

        /// <summary>
        /// 获取零件实体的备份
        /// </summary>
        /// <param name="partDoc"></param>
        /// <returns></returns>
        private Body2[] GetBodyCopies(PartDoc partDoc)
        {
            var vBodies = (Object[])partDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);

            Body2[] newBodies = new Body2[vBodies.Length];

            for (int i = 0; i < vBodies.Length; i++)
            {
                var swBody2 = (Body2)vBodies[i];

                newBodies[i] = (Body2)swBody2.Copy();
            }

            return newBodies;
        }

        /// <summary>
        /// 把备份的实体 生成特征
        /// </summary>
        /// <param name="partDoc"></param>
        /// <param name="bodies"></param>
        private void CreateFeatureForBodies(PartDoc partDoc, Body2[] bodies)
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                partDoc.CreateFeatureFromBody3(bodies[i], false, (int)swCreateFeatureBodyOpts_e.swCreateFeatureBodySimplify);
            }
        }

        /// <summary>
        /// 删除当前所有的特征
        /// </summary>
        /// <param name="modelDoc2"></param>
        private void DeleteAllUserFeature(ModelDoc2 modelDoc2)
        {
            SelectAllUserFeature(modelDoc2);
            modelDoc2.Extension.DeleteSelection2((int)swDeleteSelectionOptions_e.swDelete_Children + (int)swDeleteSelectionOptions_e.swDelete_Absorbed);
        }

        /// <summary>
        /// 选择所有的特征
        /// </summary>
        /// <param name="modelDoc2"></param>
        private void SelectAllUserFeature(ModelDoc2 modelDoc2)
        {
            modelDoc2.ClearSelection2(true);

            var swFeature = (Feature)modelDoc2.FirstFeature();

            // var selectFeat = false;

            while (swFeature != null)
            {
                if (swFeature != null)
                {
                    swFeature.Select2(true, 1);
                }
                else
                {
                    if (swFeature.GetTypeName2() == "OriginProfileFeature")
                    {
                        //  selectFeat = true;
                    }
                }

                swFeature = (Feature)swFeature.GetNextFeature();
            }
        }

        private void btnSetPartTitle_Click(object sender, EventArgs e)
        {
            //此功能只针对未保存的过的,只在当前内存中存在的零件.
            SldWorks swApp = Utility.ConnectToSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var resSetTitle = swModel.SetTitle2("TitleNewPart");

            if (resSetTitle == false)
            {
                swApp.SendMsgToUser("失败了!");
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();
            var swModel = (ModelDoc2)swApp.ActiveDoc;

            swModel = (ModelDoc2)swApp.ActiveDoc;

            //这个就是重新打开,最后一个参数是放不要放弃修改(我们不修改,所以为true)
            swModel.ReloadOrReplace(false, swModel.GetPathName(), true);
        }

        private void btnGetMateInfor_Click(object sender, EventArgs e)
        {
            //请先打开TemplateModel文件夹下的装配TempAssembly.sldasm  D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var swFeat = (Feature)swModel.FirstFeature();

            Feature swMateFeat = null;
            Feature swSubFeat = default(Feature);
            Mate2 swMate = default(Mate2);
            Component2 swComp = default(Component2);
            MateEntity2[] swMateEnt = new MateEntity2[3];
            //string fileName = null;
            //int errors = 0;
            //int warnings = 0;
            int i = 0;
            double[] entityParameters = new double[8];

            //从特征树中查找配合文件夹 Iterate over features in FeatureManager design tree

            while ((swFeat != null))
            {
                if ("MateGroup" == swFeat.GetTypeName())
                {
                    swMateFeat = (Feature)swFeat;
                    break;
                }
                swFeat = (Feature)swFeat.GetNextFeature();
            }
            Debug.Print("  " + swMateFeat.Name);
            Debug.Print("");

            //获取第一个子配合特征 Get first mate, which is a subfeature
            swSubFeat = (Feature)swMateFeat.GetFirstSubFeature();
            while ((swSubFeat != null))
            {
                swMate = (Mate2)swSubFeat.GetSpecificFeature2();
                if ((swMate != null))
                {
                    for (i = 0; i <= 1; i++)
                    {
                        swMateEnt[i] = swMate.MateEntity(i);
                        Debug.Print("    " + swSubFeat.Name);
                        Debug.Print("      Type              = " + swMate.Type);
                        Debug.Print("      Alignment         = " + swMate.Alignment);
                        Debug.Print("      Can be flipped    = " + swMate.CanBeFlipped);
                        Debug.Print("");
                        swComp = (Component2)swMateEnt[i].ReferenceComponent;
                        Debug.Print("      Component         = " + swComp.Name2);
                        Debug.Print("      Mate enity type   = " + swMateEnt[i].ReferenceType);
                        entityParameters = (double[])swMateEnt[i].EntityParams;
                        Debug.Print("      (x,y,z)           = (" + (double)entityParameters[0] + ", " + (double)entityParameters[1] + ", " + (double)entityParameters[2] + ")");
                        Debug.Print("      (i,j,k)           = (" + (double)entityParameters[3] + ", " + (double)entityParameters[4] + ", " + (double)entityParameters[5] + ")");
                        Debug.Print("      Radius 1          = " + (double)entityParameters[6]);
                        Debug.Print("      Radius 2          = " + (double)entityParameters[7]);
                        Debug.Print("");
                    }
                    Debug.Print(" ");
                }
                // 从配合组中遍历 下一个配合 Get the next mate in MateGroup
                swSubFeat = (Feature)swSubFeat.GetNextSubFeature();
            }
        }

        private void btnGetAllDim_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();
            //加速读取
            swApp.CommandInProgress = true;

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                //第一个特征
                Feature swFeat = (Feature)swModel.FirstFeature();

                //遍历
                Utility.TraverseFeatures(swFeat, true, true);
            }
            swApp.CommandInProgress = false;
        }

        //用于事件对象共享。
        private PartDoc partDoc = null;

        private void btnUserSelectFirst_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                partDoc = swModel as PartDoc;
                //为当前文件增加选择对象之后 通知事件
                partDoc.UserSelectionPostNotify += PartDoc_UserSelectionPostNotify;
            }
        }

        /// <summary>
        /// 选择之后 处理事件内容
        /// </summary>
        /// <returns></returns>
        private int PartDoc_UserSelectionPostNotify()
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            var swSelMgr = (SelectionMgr)swModel.SelectionManager;

            var selectType = swSelMgr.GetSelectedObjectType3(1, -1);

            SendMessageToUser("You Select :" + Enum.GetName(typeof(swSelectType_e), selectType));

            return 1;
        }

        /// <summary>
        /// 做完通知之后 ，去掉事件绑定
        /// </summary>
        /// <param name="s"></param>
        private void SendMessageToUser(string s)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            swApp.SendMsgToUser(s);

            partDoc.UserSelectionPostNotify -= PartDoc_UserSelectionPostNotify;
        }

        private void btnRoundPointLoc_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            SelectionMgr swSelMgr = default(SelectionMgr);
            Feature swFeature = default(Feature);

            //连接文件

            swModel = (ModelDoc2)swApp.ActiveDoc;

            swModelDocExt = (ModelDocExtension)swModel.Extension;

            //选中草图
            var status = swModelDocExt.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, 0);

            swSelMgr = (SelectionMgr)swModel.SelectionManager;
            //转换
            swFeature = (Feature)swSelMgr.GetSelectedObject6(1, -1);
            //进入编辑草图
            swModel.EditSketch();

            //获取草图中的所有草图点来修改坐标

            var swSketch = (Sketch)swFeature.GetSpecificFeature2();

            var points = (object[])swSketch.GetSketchPoints2();

            for (int i = 0; i < points.Length; i++)
            {
                var p = (SketchPoint)points[i];

                var x = p.X * 1000;
                var y = p.Y * 1000;

                p.X = Math.Round(x, 0) / 1000;
                p.Y = Math.Round(y, 0) / 1000;

                //Debug.Print(p.X.ToString() + "     " + p.Y.ToString());
            }

            swModel.EditRebuild3();

            swModel.EditSketch();

            MessageBox.Show("完成了！");
        }

        private void btnRunCommand_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            //执行命令监控
            swApp.CommandOpenPreNotify += SwApp_CommandOpenPreNotify;

            //请参考SolidWorks.Interop.swcommands

            //swCommands_e 命令操作

            //swMouse_e  鼠标操作

            //打开选项对话框
            //swApp.RunCommand((int)swCommands_e.swCommands_Options, "");

            //开始3d草图
            swApp.RunCommand((int)swCommands_e.swCommands_3DSketch, "");

            //单击右键
            //swApp.RunCommand((int)swMouse_e.swMouse_Click, "");
        }

        /// <summary>
        /// 在执行命令前通知。
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="UserCommand"></param>
        /// <returns></returns>
        private int SwApp_CommandOpenPreNotify(int Command, int UserCommand)
        {
            Debug.Print($@"command is :{Enum.GetName(typeof(swCommands_e), Command)}");

            Debug.Print($@"user command Id is :{UserCommand}");

            if (Command == (int)swCommands_e.swCommands_FilterFaces)
            {
                MessageBox.Show("Fillet Faces Command is disable!");
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 插入异形孔特征
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertHole_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            AddHoleForThisPoint("holePoints", 10, "异型孔测试");
        }

        /// <summary>
        /// 插入简单孔特征
        /// </summary>
        /// <param name="sketchName">草图名称</param>
        /// <param name="DiaSize">孔径</param>
        /// <param name="holeName">名称</param>
        public void AddHoleForThisPoint(string sketchName, double DiaSize, string holeName)
        {
            SldWorks SwApp;

            Feature swFeature;

            string fileName;
            long errors;
            long warnings;
            bool status;
            int SlotType;
            int HoleType;
            int StandardIndex;
            int FastenerTypeIndex;
            string SSize;
            short EndType;
            double ConvFactorLength;
            double ConvFactorAngle;
            double Diameter;
            double Depth;
            double Length;
            double ScrewFit;
            double DrillAngle;
            double NearCsinkDiameter;
            double NearCsinkAngle;
            double FarCsinkDiameter;
            double FarCsinkAngle;
            double Offset;
            string ThreadClass;
            double CounterBoreDiameter;
            double CounterBoreDepth;
            double HeadClearance;
            double BotCsinkDiameter;
            double BotCsinkAngle;
            WizardHoleFeatureData2 swWizardHoleFeatData;

            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            var swFeatureMgr = swModel.FeatureManager;

            var swModelDocExt = swModel.Extension;

            status = swModel.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);

            HoleType = (int)swWzdGeneralHoleTypes_e.swWzdLegacy;
            StandardIndex = -1;
            FastenerTypeIndex = -1;
            SSize = "";
            EndType = (int)swEndConditions_e.swEndCondThroughAll;
            ConvFactorAngle = -1;

            Diameter = DiaSize / 1000;

            Depth = -1;
            Length = -1;

            CounterBoreDiameter = 0;    // Value1
            CounterBoreDepth = 0;   // Value2
            HeadClearance = -1;                              // Value3
            ScrewFit = -1;                                   // Value4
            DrillAngle = -1;                                 // Value5
            NearCsinkDiameter = -1;                          // Value6
            NearCsinkAngle = -1;                             // Value7
            BotCsinkDiameter = -1;                           // Value8
            BotCsinkAngle = -1;                              // Value9
            FarCsinkDiameter = -1;                           // Value10
            FarCsinkAngle = -1;                              // Value11
            Offset = -1;                                     // Value12
            ThreadClass = "";

            swFeature = swFeatureMgr.HoleWizard5(HoleType, StandardIndex, FastenerTypeIndex, SSize, EndType, Diameter, Depth, Length, CounterBoreDiameter, CounterBoreDepth, HeadClearance, ScrewFit, DrillAngle, NearCsinkDiameter, NearCsinkAngle, BotCsinkDiameter, BotCsinkAngle, FarCsinkDiameter, FarCsinkAngle, Offset, ThreadClass, false, false, false, false, false, false);

            Feature holeFeature = (Feature)swFeature.GetFirstSubFeature();

            Feature sizeFeature = (Feature)holeFeature.GetNextSubFeature();

            holeFeature.Select2(false, 0);
            swModel.EditSketch();

            swModel.ClearSelection2(true);
            status = swModel.Extension.SelectByID2("Point1", "SKETCHPOINT", 0, 0, 0, false, 0, null, 0);

            status = swModel.Extension.SelectByID2("Point1@" + sketchName, "EXTSKETCHPOINT", 0, 0, 0, true, 0, null, 0);

            swModel.SketchAddConstraints("sgCOINCIDENT");
            swModel.ClearSelection2(true);

            swModel.ClearSelection2(true);
            swModel.SketchManager.InsertSketch(true);

            holeFeature.Name = holeName + "-点位";
            sizeFeature.Name = holeName + "-尺寸";

            swFeature.Name = holeName;

            status = swModel.Extension.SelectByID2(holeName, "BODYFEATURE", 0, 0, 0, false, 4, null, 0);
            status = swModel.Extension.SelectByID2(sketchName, "SKETCH", 0, 0, 0, true, 64, null, 0);

            swFeature = swModel.FeatureManager.FeatureSketchDrivenPattern(true, false)
            ;

            swFeature.Name = "阵列-" + holeName;
        }

        /// <summary>
        /// 读取草图中的文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butGetTextInSketch_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //请先打开44_GetSketchText.SLDPRT

            //选择草图
            swModel.Extension.SelectByID2("SketchText", "SKETCH", 0, 0, 0, false, 4, null, 0);

            var swFeat = (Feature)swSelMgr.GetSelectedObject6(1, -1);

            var swSketch = (Sketch)swFeat.GetSpecificFeature2();

            swModel.EditSketch();

            var TextParams = (Object[])swSketch.GetSketchTextSegments();

            //第一个文本
            var SketchText = (SketchText)TextParams[0];

            MessageBox.Show($"Old Text is :{SketchText.Text}");

            SketchText.Text = "New text...";

            MessageBox.Show($"New Text is :{SketchText.Text}");

            swModel.InsertSketch2(true);

            swModel.EditRebuild3();
        }

        /// <summary>
        /// 组合零件后保留零件颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJoinKeepBodyColor_Click(object sender, EventArgs e)
        {
            //请先打开45_JoinTest.sldasm

            JoinPart(@"clamp1-1@45_JoinTest", @"JoinPart2-1@45_JoinTest");
        }

        /// <summary>
        /// 组合零件
        /// </summary>
        /// <param name="BasePartSelectID">基础零件</param>
        /// <param name="JoinPartSelectId">要组合进来的零件</param>
        /// <returns></returns>
        private bool JoinPart(string BasePartSelectID, string JoinPartSelectId)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();
            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            AssemblyDoc assemblyDoc = (AssemblyDoc)swModel;

            var boolstatus = swModel.Extension.SelectByID2(BasePartSelectID, "COMPONENT", 0, 0, 0, false, 0, null, 0);

            assemblyDoc.EditPart();

            var resSel = swModel.Extension.SelectByID2(JoinPartSelectId, "COMPONENT", 0, 0, 0, false, 0, null, 0);

            if (resSel == true)
            {
                var resJoin = assemblyDoc.InsertJoin2(true, false);

                if (resJoin == true)
                {
                    swModel.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "JoinColor");

                    assemblyDoc.EditAssembly();

                    swModel.ClearSelection();

                    boolstatus = swModel.Extension.SelectByID2(BasePartSelectID, "COMPONENT", 0, 0, 0, false, 0, null, 0);

                    assemblyDoc.OpenCompFile();

                    var swPart = (PartDoc)swApp.ActiveDoc;

                    var thisFeatureClip = (Feature)swPart.FeatureByName("JoinColor");

                    if (thisFeatureClip != null)
                    {
                        var vFaceProp = (double[])swPart.MaterialPropertyValues;

                        var vProps = (double[])thisFeatureClip.GetMaterialPropertyValues2(1, null);
                        //这里指定为红色，正常是要从被组合的零件中获取的。
                        vProps[0] = 1;
                        vProps[1] = 0;
                        vProps[2] = 0;
                        vProps[3] = vFaceProp[3];
                        vProps[4] = vFaceProp[4];
                        vProps[5] = vFaceProp[5];
                        vProps[6] = vFaceProp[6];
                        vProps[7] = vFaceProp[7];
                        vProps[8] = vFaceProp[8];

                        thisFeatureClip.SetMaterialPropertyValues2(vProps, 1, null);

                        vProps = null;

                        vFaceProp = null;
                        swPart.EditRebuild();
                    }
                }
                else
                {
                    MessageBox.Show("Error to Insert Join!");
                }
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowTemplateBody_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 导出选中的实体到文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportBodyToFile_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            // SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;
            //注意，选中实体才能导出。(可以使用过滤工具选择)

            //指定导出记录
            BodyHelper.ExportBodyToFile(@"D:\smallball.dat");

            MessageBox.Show("导出小球成功！");
        }

        /// <summary>
        /// 从文件导出实体并显示 (在鼠标位置显示小球)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowTemplateBody_Click_1(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //var selectFace = (Face2)swSelMgr.GetSelectedObject6(1, -1);

            var mousePoint = (double[])swSelMgr.GetSelectionPoint2(1, -1);

            //var faceNormal = selectFace.Normal;

            //var faceNormalDou = (double[])faceNormal;

            PartDoc partDoc = (PartDoc)swModel;

            var vBodies = (Object[])partDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);

            Debug.Print(vBodies.Length.ToString());

            vBodies = null;

            if (swModel != null)
            {
                IBody2 body = BodyHelper.LoadBodyFromFile(swApp, @"D:\smallball.dat");

                if (body != null)
                {
                    Color color = Color.Yellow; //System.Drawing.ColorTranslator.FromHtml(colorString); 如果是字符串可以通过这转

                    int colorInt = color.ToArgb();
                    int red2 = colorInt >> 16 & 255;
                    int green2 = colorInt >> 8 & 255;
                    int blue2 = colorInt & 255;
                    int refcolor2 = (int)blue2 << 16 | (int)green2 << 8 | (int)red2;
                    object vXform = null;
                    double[] Xform = new double[16];
                    Xform[0] = 1; //旋转
                    Xform[1] = 0.0;
                    Xform[2] = 0.0;

                    Xform[3] = 0;  //旋转
                    Xform[4] = 1;
                    Xform[5] = 0;

                    Xform[6] = 0; //旋转
                    Xform[7] = 0;
                    Xform[8] = 1;

                    Xform[9] = mousePoint[0]; //平移 x
                    Xform[10] = mousePoint[1]; //平移 y
                    Xform[11] = mousePoint[2]; //平移 z

                    Xform[12] = 1.0; //比例因子
                    Xform[13] = 0.0; //未使用
                    Xform[14] = 0.0;//未使用
                    Xform[15] = 0.0;//未使用

                    vXform = Xform;

                    var MathUtility = (MathUtility)swApp.GetMathUtility();

                    var MathXform = (MathTransform)MathUtility.CreateTransform(vXform);

                    //var swOrigPt = MathUtility.CreatePoint(new double[] { 0.094, 0.142, 0.012 });

                    //这里可以通过轴来创建变换，相当于坐标系的配合
                    //var swAxisVerX = (MathVector)MathUtility.CreateVector(new double[] { 0, 0, -1 });
                    //var swAxisVerY = (MathVector)MathUtility.CreateVector(new double[] { 0, 1, 0 });
                    //var swAxisVerZ = (MathVector)MathUtility.CreateVector(faceNormalDou);
                    //var swAxisVerM = (MathVector)MathUtility.CreateVector(new double[] { 0.094, 0.142, 0.012 });
                    // var MathXform = (MathTransform)MathUtility.ComposeTransform(swAxisVerX, swAxisVerY, swAxisVerZ, swAxisVerM, 1);

                    //这是通过轴 与角度 创建变换
                    //var MathXform = (MathTransform)MathUtility.CreateTransformRotateAxis(swOrigPt, swAxisVerX, Math.PI*0.5);

                    body.ApplyTransform(MathXform);

                    //显示实体，并设置为可选中。
                    body.Display3(swModel, refcolor2, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);

                    vBodies = (Object[])partDoc.GetBodies2((int)swBodyType_e.swAllBodies, false);

                    Debug.Print(vBodies.Length.ToString());

                    vBodies = null;

                    // body.CreateBaseFeature(body); //把实体变成导入特征。

                    body.DisableDisplay = true;
                    swModel.WindowRedraw();

                    body.DisableDisplay = false;
                    swModel.WindowRedraw();

                    body = null;
                    swModel.WindowRedraw();
                    swModel.GraphicsRedraw2();
                    //vBodies = (Object[])partDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);

                    //Debug.Print(vBodies.Length.ToString());

                    //vBodies = null;

                    //vb.net 写法  body.Display3(swModel, Information.RGB(255, 255, 0), swTempBodySelectOptions_e.swTempBodySelectOptionNone);
                }
                else
                    throw new Exception("失败了！");
            }
            else
                throw new Exception("请打开一个文件");
        }

        /// <summary>
        /// 遍历 草图中的闭环轮廓
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butGetSketchContour_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //选择草图
            swModel.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 4, null, 0);

            //把选择转换为特征
            var swFeat = (Feature)swSelMgr.GetSelectedObject6(1, -1);

            swModel.ClearSelection();

            //把特征转换为草图
            var swSketch = (Sketch)swFeat.GetSpecificFeature2();

            //获取轮廓数量
            var sketchContoursCount = swSketch.GetSketchContourCount();
            var sketchContours = (object[])swSketch.GetSketchContours();

            //选择所有轮廓
            for (int i = 0; i < sketchContoursCount; i++)
            {
                var skContous = (SketchContour)sketchContours[i];
                skContous.Select(true, 0);
            }

            var swFeatureManager = (FeatureManager)swModel.FeatureManager;
            //做一个简单的拉伸
            var swFeature = (Feature)swFeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.01, 0.01, false, false, false,
            false, 0, 0, false, false, false, false, true, true, true, 0, 0, false);
        }

        /// <summary>
        /// 增加属性特征，完整过程可参考博客内容 ,这里只是完全复制api中的例子。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAttribute_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            SelectionMgr swSelectionMgr = default(SelectionMgr);
            Feature swFeature = default(Feature);
            SolidWorks.Interop.sldworks.Attribute swAttribute = default(SolidWorks.Interop.sldworks.Attribute);
            AttributeDef swAttributeDef = default(AttributeDef);
            Face2 swFace = default(Face2);
            Parameter swParameter = default(Parameter);
            Object[] Faces = null;
            bool @bool = false;

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swModelDocExt = swModel.Extension;
            swSelectionMgr = (SelectionMgr)swModel.SelectionManager;

            // Create attribute 创建一个属性的定义
            swAttributeDef = (AttributeDef)swApp.DefineAttribute("TestPropagationOfAttribute");
            @bool = swAttributeDef.AddParameter("TestAttribute", (int)swParamType_e.swParamTypeDouble, 2.0, 0);// 增加一个属性值
            @bool = swAttributeDef.Register();//注册

            // Select the feature to which to add the attribute 择要附加属性的特征， 这里选择中的是Cut-Extrude1
            @bool = swModelDocExt.SelectByID2("Cut-Extrude1", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            swFeature = (Feature)swSelectionMgr.GetSelectedObject6(1, -1);//把选中的对象转特征
            Debug.Print("Name of feature to which to add attribute: " + swFeature.Name);

            // Add the attribute to one of the feature's faces   把属性附加到特征的一个面上。
            Faces = (Object[])swFeature.GetFaces(); //获取特征的所有面
            swFace = (Face2)Faces[0];//得到第一个面
            swAttribute = swAttributeDef.CreateInstance5(swModel, swFace, "TestAttribute", 0, (int)swInConfigurationOpts_e.swAllConfiguration);//创建属性
            //设置属性包括在库特征中
            swAttribute.IncludeInLibraryFeature = true;
            Debug.Print("Include attribute in library feature? " + swAttribute.IncludeInLibraryFeature);
            Debug.Print("Name of attribute: " + swAttribute.GetName());
            // Get name of parameter 读取之前设定的属性值
            swParameter = (Parameter)swAttribute.GetParameter("TestAttribute");
            Debug.Print("Parameter name: " + swParameter.GetName());

            swModel.ForceRebuild3(false);

            //直接通过特征读取到属性，然后获取值

            PartDoc partDoc = (PartDoc)swModel;

            var attFea = partDoc.IFeatureByName("TestAttribute");

            var attDefSpc = (Attribute)attFea.GetSpecificFeature2();

            Debug.Print("Parameter name: " + attDefSpc.GetName());

            var attValue = (Parameter)attDefSpc.GetParameter("TestAttribute");

            attValue.SetDoubleValue(5);
        }

        /// <summary>
        /// 导入dxf到 sketch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImpotDxfToSketch_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            //确保文件存在
            string filename = @"C:\Users\Public\Documents\SOLIDWORKS\SOLIDWORKS 2018\samples\tutorial\importexport\rainbow.DXF";

            ImportDxfDwgData importData = (ImportDxfDwgData)swApp.GetImportFileData(filename);

            importData.ImportMethod[""] = (int)swImportDxfDwg_ImportMethod_e.swImportDxfDwg_ImportToPartSketch;

            int longerrors = 0;

            var newDoc = swApp.LoadFile4(filename, "", importData, ref longerrors);

            //Gets
            Debug.Print("Part Sketch Gets:");
            Debug.Print("  Add constraints:   " + importData.AddSketchConstraints[""]);
            Debug.Print("  Merge points:      " + importData.GetMergePoints(""));
            Debug.Print("  Merge distance:    " + (importData.GetMergeDistance("") * 1000));
            Debug.Print("  Import dimensions: " + importData.ImportDimensions[""]);
            Debug.Print("  Import hatch:      " + importData.ImportHatch[""]);
            //Sets
            Debug.Print("Part Sketch Sets:");
            importData.AddSketchConstraints[""] = true;
            Debug.Print("  Add constraints:   " + importData.AddSketchConstraints[""]);
            var retVal = importData.SetMergePoints("", true, 0.000002);
            Debug.Print("  Merge points:      " + retVal);
            Debug.Print("  Merge distance:    " + (importData.GetMergeDistance("") * 1000));
            importData.ImportDimensions[""] = true;
            Debug.Print("  Import dimensions: " + importData.ImportDimensions[""]);
            importData.ImportHatch[""] = false;
            Debug.Print("  Import hatch:      " + importData.ImportHatch[""]);
        }

        /// <summary>
        /// 引用实体到草图中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConvertEntities_Click(object sender, EventArgs e)
        {
            //先打开54_ConvertEntitiesToSketch.SLDPRT
            SldWorks swApp = PStandAlone.GetSolidWorks();

            var swModel = swApp.IActiveDoc2;

            var partDoc = (PartDoc)swModel;

            //通过名字得到特征
            var baseSketchFeature = partDoc.IFeatureByName("BaseSketch");

            Sketch baseSketch = (Sketch)baseSketchFeature.GetSpecificFeature2();

            baseSketchFeature.Select(false);

            swModel.EditSketch();

            //选择已经命名的面

            Face2 face1 = (Face2)partDoc.GetEntityByName("MyNamedFace", (int)swSelectType_e.swSelFACES);

            Entity entity1 = (Entity)face1;

            entity1.Select(false);

            //引用实体
            swModel.SketchManager.SketchUseEdge3(false, false);

            //保存退出草图
            swModel.SketchManager.InsertSketch(true);
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            //先打开一个零件，打开哪个请随意。

            SldWorks swApp = PStandAlone.GetSolidWorks();

            int fileerror = 0;

            int filewarning = 0;

            bool boolstatus = false;

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var swModelDocExt = (ModelDocExtension)swModel.Extension;

            // Insert a camera  插入相机

            var swCamera = (Camera)swModelDocExt.InsertCamera();

            // Set camera type to floating  设置为浮动

            swCamera.Type = (int)swCameraType_e.swCameraType_Floating;

            // Show camera 显示相机

            boolstatus = swModelDocExt.SelectByID2("Camera1", "CAMERAS", 0, 0, 0, false, 0, null, 0);  //注意如果是中文系统，可能名称为相机1

            boolstatus = swModel.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swDisplayCameras, true);

            swModel.GraphicsRedraw2();

            // Get camera's pitch and yaw settings  获取相机的角度位置 设置

            // 1 radian = 180º/p = 57.295779513º or approximately 57.3º

            Debug.Print("Original pitch (up or down angle) = " + swCamera.Pitch * 57.3 + " deg");

            Debug.Print("Original yaw (side-to-side angle) = " + swCamera.Yaw * 57.3 + " deg");

            Debug.Print(" ");

            // Rotate camera   旋转相机

            swCamera.Pitch = -25;

            swCamera.Yaw = 150;

            // New pitch and yaw settings   新的位置

            Debug.Print("New pitch (up or down angle) = " + swCamera.Pitch * 57.3 + " deg");

            Debug.Print("New yaw (side-to-side angle) = " + swCamera.Yaw * 57.3 + " deg");

            swModel.GraphicsRedraw2();
        }

        private void btnReplaceReference_Click(object sender, EventArgs e)
        {
            const string sLicenseKey = "Axemble:swdocmgr_general-11785-02051-00064-50177-08535-34307-00007-37408-17094-12655-31529-39909-49477-26312-14336-58516-10910-42487-02022-02562-54862-24526-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-7,swdocmgr_previews-11785-02051-00064-50177-08535-34307-00007-48008-04931-27155-53105-52081-64048-22699-38918-23742-63202-30008-58372-23951-37726-23245-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-1,swdocmgr_dimxpert-11785-02051-00064-50177-08535-34307-00007-16848-46744-46507-43004-11310-13037-46891-59394-52990-24983-00932-12744-51214-03249-23667-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-8,swdocmgr_geometry-11785-02051-00064-50177-08535-34307-00007-39720-42733-27008-07782-55416-16059-24823-59395-22410-04359-65370-60348-06678-16765-23356-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-3,swdocmgr_xml-11785-02051-00064-50177-08535-34307-00007-51816-63406-17453-09481-48159-24258-10263-28674-28856-61649-06436-41925-13932-52097-22614-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-7,swdocmgr_tessellation-11785-02051-00064-50177-08535-34307-00007-13440-59803-19007-55358-48373-41599-14912-02050-07716-07769-29894-19369-42867-36378-24376-57604-46485-45449-00405-25144-23144-51942-23264-24676-28258-0";//如果正版用户，请联系代理商申请。

            string sDocFileName = @"D:\14_Model Test\dimension test\nt22v01v70-lf-02 ref.SLDDRW";

            SwDMClassFactory swClassFact = default(SwDMClassFactory);
            SwDMApplication swDocMgr = default(SwDMApplication);
            SwDMDocument swDoc = default(SwDMDocument);
            SwDMDocument10 swDoc10 = default(SwDMDocument10);
            SwDMDocument22 swDoc22 = default(SwDMDocument22);

            SwDmDocumentType nDocType = 0;
            SwDmDocumentOpenError nRetVal = 0;
            SwDmPreviewError nError = 0;

            // Determine type of SOLIDWORKS file based on file extension
            if (sDocFileName.ToLower().EndsWith("sldprt"))
            {
                nDocType = SwDmDocumentType.swDmDocumentPart;
            }
            else if (sDocFileName.ToLower().EndsWith("sldasm"))
            {
                nDocType = SwDmDocumentType.swDmDocumentAssembly;
            }
            else if (sDocFileName.ToLower().EndsWith("slddrw"))
            {
                nDocType = SwDmDocumentType.swDmDocumentDrawing;
            }
            else
            {
                // Probably not a SOLIDWORKS file,
                // so cannot open
                nDocType = SwDmDocumentType.swDmDocumentUnknown;
                return;
            }

            swClassFact = new SwDMClassFactory();
            swDocMgr = (SwDMApplication)swClassFact.GetApplication(sLicenseKey);
            swDoc = (SwDMDocument)swDocMgr.GetDocument(sDocFileName, nDocType, false, out nRetVal);

            swDoc10 = (SwDMDocument10)swDoc;
            swDoc22 = (SwDMDocument22)swDoc;

            object vBrokenRefs = null;
            object vIsVirtuals = null;
            object vTimeStamps = null;
            object vIsImported = null;

            string[] vDependArr = null;

            SwDMSearchOption swSearchOpt = default(SwDMSearchOption);

            swSearchOpt = swDocMgr.GetSearchOptionObject();

            vDependArr = (string[])swDoc22.GetAllExternalReferences5(swSearchOpt, out vBrokenRefs, out vIsVirtuals, out vTimeStamps, out vIsImported);

            if ((vDependArr == null)) return;

            var doc16 = (SwDMDocument16)swDoc;

            doc16.ReplaceReference(vDependArr[0], @"D:\14_Model Test\dimension test\nt22v01v70-lf-02 ref.SLDPRT");

            swDoc.Save();

            swDoc.CloseDoc();
        }

        private void btnGetRayPoints_Click(object sender, EventArgs e)
        {
            FrmMain frmMain = new FrmMain();

            frmMain.Show();
        }

        /// <summary>
        /// 删除草图中的错误关系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteRelation_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            var swModel = swApp.IActiveDoc2;

            //请先打开 D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\57_DeleteRelation.sldprt

            swModel.Extension.SelectByID2("Sketch11", "SKETCH", 0, 0, 0, false, 1, null, 0);
            swModel.EditSketch();

            swModel.ClearSelection2(true);

            var swSketch = swModel.IGetActiveSketch2();

            var skRelMgr = swSketch.RelationManager;

            //这里可以过滤掉你想找到哪些类型的参考
            var vRel = (object[])skRelMgr.GetRelations((int)swSketchRelationFilterType_e.swDangling + (int)swSketchRelationFilterType_e.swOverDefining);

            if (vRel != null)
            {
                for (int j = vRel.Length - 1; j >= 0; j--)
                {
                    var swSkRel = (SketchRelation)vRel[j];

                    //这一句是删除
                    skRelMgr.DeleteRelation(swSkRel);

                    //这一句是压缩掉 swSkRel.Suppressed = true;
                }
            }

            swModel.ForceRebuild3(true);
        }

        /// <summary>
        /// 这里有两种代码，一个是直接在工程图中选中了零件，另一个在工程图中选中了注释 或者球标(有引线指向零件)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetCompFromBalloon_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            var swModel = swApp.IActiveDoc2;

            //var drawingDoc = (DrawingDoc)swModel;

            //选择管理器
            var swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //检查是否有选择对象
            if (swSelMgr.GetSelectedObjectCount2(-1) == 0)
            {
                MessageBox.Show("还有没选择对象");
                return;
            }

            //遍历选中对象 (经验觉得第一个选中对象一直是视图，第二个开始才是要选中的对象)
            //所以下在的循环应该可以从2开始
            for (int i = 1; i <= swSelMgr.GetSelectedObjectCount(); i++)
            {
                //获取选中的对象，转为DrawingComponent
                var swDrawingComponent = (DrawingComponent)swSelMgr.GetSelectedObjectsComponent4(i, -1);

                if (swDrawingComponent != null)
                {
                    var swComponent = (Component2)swDrawingComponent.Component;

                    MessageBox.Show(swComponent.Name2);
                }

                //如果选择的是球标 或者 注释
                if (swSelMgr.GetSelectedObjectType3(i, -1) == (int)swSelectType_e.swSelNOTES)
                {
                    //获取Note
                    var thisNote = (Note)swSelMgr.GetSelectedObject6(i, -1);
                    //获取Annotation
                    var ann = (Annotation)thisNote.GetAnnotation();
                    //获取关联实体
                    var attEntity = (Entity)ann.IGetAttachedEntities();
                    //获取实体所对应的Component
                    var comp = attEntity.IGetComponent2();

                    MessageBox.Show(comp.Name2);
                }
            }
        }

        private void btnMovePart_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();

            var swModel = swApp.IActiveDoc2;

            //选择管理器
            var swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //检查是否有选择对象
            if (swSelMgr.GetSelectedObjectCount2(-1) == 4)
            {
                var tempPart = (Component2)swSelMgr.IGetSelectedObjectsComponent2(1);
                var tempLine1 = (SketchLine)swSelMgr.GetSelectedObject(2);
                var tempLine2 = (SketchLine)swSelMgr.GetSelectedObject(3);
                var tempLine3 = (SketchLine)swSelMgr.GetSelectedObject(4);

                var MathUtility = (MathUtility)swApp.GetMathUtility();

                var swAxisVerX = (MathVector)MathUtility.CreateVector(new double[] { tempLine1.IGetEndPoint2().X - tempLine1.IGetStartPoint2().X, tempLine1.IGetEndPoint2().Y - tempLine1.IGetStartPoint2().Y, tempLine1.IGetEndPoint2().Z - tempLine1.IGetStartPoint2().Z });
                var swAxisVerY = (MathVector)MathUtility.CreateVector(new double[] { tempLine2.IGetEndPoint2().X - tempLine2.IGetStartPoint2().X, tempLine2.IGetEndPoint2().Y - tempLine2.IGetStartPoint2().Y, tempLine2.IGetEndPoint2().Z - tempLine2.IGetStartPoint2().Z });
                var swAxisVerZ = (MathVector)MathUtility.CreateVector(new double[] { tempLine3.IGetEndPoint2().X - tempLine3.IGetStartPoint2().X, tempLine3.IGetEndPoint2().Y - tempLine3.IGetStartPoint2().Y, tempLine3.IGetEndPoint2().Z - tempLine3.IGetStartPoint2().Z });

                var swAxisVerM = (MathVector)MathUtility.CreateVector(new double[] { tempLine1.IGetStartPoint2().X, tempLine1.IGetStartPoint2().Y, tempLine1.IGetStartPoint2().Z });

                var MathXform = (MathTransform)MathUtility.ComposeTransform(swAxisVerX, swAxisVerY, swAxisVerZ, swAxisVerM, 1);

                //tempPart.Transform2 = MathXform;

                tempPart.Transform = MathXform;

                //(swModel as AssemblyDoc).UpdateBox();
            }

            //这里可以通过轴来创建变换，相当于坐标系的配合
            //var MathUtility = (MathUtility)swApp.GetMathUtility();
            //var swAxisVerX = (MathVector)MathUtility.CreateVector(new double[] { 0, 0, -1 });
            //var swAxisVerY = (MathVector)MathUtility.CreateVector(new double[] { 0, 1, 0 });
            //var swAxisVerZ = (MathVector)MathUtility.CreateVector(faceNormalDou);
            //var swAxisVerM = (MathVector)MathUtility.CreateVector(new double[] { 0.094, 0.142, 0.012 });
            // var MathXform = (MathTransform)MathUtility.ComposeTransform(swAxisVerX, swAxisVerY, swAxisVerZ, swAxisVerM, 1);
            // tempPart.Transform2 = MathXform;
        }

        private void btnGetDisMateInfo_Click(object sender, EventArgs e)
        {
            //打开一个装配，并选中一个距离配合

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var featureObject = (Feature)swModel.ISelectionManager.GetSelectedObject6(1, -1);

            var featureData = (DistanceMateFeatureData)featureObject.GetDefinition();

            MessageBox.Show((featureData.Distance * 1000).ToString());
        }

        /// <summary>
        /// 遍历的层级
        /// </summary>
        private int traverseLevel;

        /// <summary>
        /// 展开此项
        /// </summary>
        private bool expandThis;

        /// <summary>
        /// 遍历节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetFeatureNodes_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            FeatureManager featureMgr = default(FeatureManager);
            TreeControlItem rootNode = default(TreeControlItem);

            featureMgr = swModel.FeatureManager;

            //通过特征树获取根节点
            rootNode = featureMgr.GetFeatureTreeRootItem2((int)swFeatMgrPane_e.swFeatMgrPaneBottom);

            int i = 0;

            for (i = 0; i <= 1; i++)
            {
                if ((rootNode != null))
                {
                    Debug.Print("");
                    traverseLevel = 0;
                    traverse_node(rootNode);
                }

                expandThis = false;

                if (i == 0)
                {
                    MessageBox.Show(" OK to collapse all nodes?");
                }
            }
        }

        /// <summary>
        /// 遍历节点
        /// </summary>
        /// <param name="node"></param>
        private void traverse_node(TreeControlItem node)
        {
            TreeControlItem childNode = default(TreeControlItem);
            Feature featureNode = default(Feature);
            Component2 componentNode = default(Component2);

            int nodeObjectType = 0;
            object nodeObject = null;
            string restOfString = "";
            string indent = "";
            int i = 0;
            bool displayNodeInfo = false;
            string compName = null;
            int suppr = 0;
            string supprString = "";
            int vis = 0;
            string visString = "";
            string notfloatingString = "";
            bool notfloating = false;
            object componentDoc = null;
            string docString = "";
            string refConfigName = "";

            displayNodeInfo = false;

            nodeObjectType = node.ObjectType; //节点对象类型
            nodeObject = node.Object; //节点对象   目前只支持 Feature 和 Component对象转换

            switch (nodeObjectType)
            {
                case (int)swTreeControlItemType_e.swFeatureManagerItem_Feature:

                    displayNodeInfo = true;
                    if ((nodeObject != null))
                    {
                        featureNode = (Feature)nodeObject;
                        restOfString = "[FEATURE: " + featureNode.Name + "]";
                    }
                    else
                    {
                        restOfString = "[FEATURE: object Null?!]";
                    }

                    break;

                case (int)swTreeControlItemType_e.swFeatureManagerItem_Component:

                    displayNodeInfo = true;

                    if ((nodeObject != null))
                    {
                        componentNode = (Component2)nodeObject;
                        compName = componentNode.Name2;

                        if ((string.IsNullOrEmpty(compName)))
                        {
                            compName = "???";
                        }
                        //组件节点状态
                        suppr = componentNode.GetSuppression();

                        switch ((suppr))
                        {
                            case (int)swComponentSuppressionState_e.swComponentFullyResolved:
                                supprString = "Resolved";

                                break;

                            case (int)swComponentSuppressionState_e.swComponentLightweight:
                                supprString = "Lightweight";

                                break;

                            case (int)swComponentSuppressionState_e.swComponentSuppressed:
                                supprString = "Suppressed";

                                break;
                        }
                        //组件节点可见
                        vis = componentNode.Visible;

                        switch ((vis))
                        {
                            case (int)swComponentVisibilityState_e.swComponentHidden:
                                visString = "Hidden";

                                break;

                            case (int)swComponentVisibilityState_e.swComponentVisible:
                                visString = "Visible";

                                break;
                        }
                        //组件节点完全定义？
                        notfloating = componentNode.IsFixed();

                        if (notfloating == false)
                        {
                            notfloatingString = "Floating";
                        }
                        else
                        {
                            notfloatingString = "Fixed";
                        }

                        componentDoc = componentNode.GetModelDoc2();

                        if (componentDoc == null)
                        {
                            docString = "NotLoaded";
                        }
                        else
                        {
                            docString = "Loaded";
                        }
                        //组件节点配置
                        refConfigName = componentNode.ReferencedConfiguration;

                        if ((string.IsNullOrEmpty(refConfigName)))
                        {
                            refConfigName = "???";
                        }

                        restOfString = "[COMPONENT: " + compName + " " + docString + " " + supprString + " " + visString + " " + refConfigName + "]";
                    }
                    else
                    {
                        restOfString = "[COMPONENT: object Null?!]";
                    }

                    break;

                default:

                    displayNodeInfo = true;

                    if ((nodeObject != null))
                    {
                        restOfString = "[object type not handled]";
                    }
                    else
                    {
                        restOfString = "[object Null?!]";
                    }

                    break;
            }

            for (i = 1; i <= traverseLevel; i++)
            {
                indent = indent + "  ";
            }

            if ((displayNodeInfo))
            {
                Debug.Print(indent + node.Text + " : " + restOfString);
            }

            // Expand the node
            node.Expanded = expandThis;
            traverseLevel = traverseLevel + 1;
            childNode = node.GetFirstChild();

            while ((childNode != null))
            {
                Debug.Print(indent + "Node is expanded: " + childNode.Expanded);
                traverse_node(childNode);
                childNode = childNode.GetNext();
            }

            traverseLevel = traverseLevel - 1;
        }

        private void btnProfileCenter_Click(object sender, EventArgs e)
        {
            //请先分别在两个零件中选中用于轮廓配合的面。
            //连接到Solidworks
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            Face2 swSelFace = default(Face2);
            Face2 swSelFace2 = default(Face2);

            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //获取选择数据
            SelectData swSelData = default(SelectData);

            swSelData = swSelMgr.CreateSelectData();

            swSelFace = (Face2)swSelMgr.GetSelectedObject6(1, 0);
            swSelFace2 = (Face2)swSelMgr.GetSelectedObject6(2, 0);

            var swAsmembly = swModel as AssemblyDoc;

            ProfileCenterMateFeatureData profileCenterMateFeatureData =
                (ProfileCenterMateFeatureData)swAsmembly.CreateMateData((int)swMateType_e.swMatePROFILECENTER);

            object[] mateFaces = new object[2];

            mateFaces[0] = swSelFace;
            mateFaces[1] = swSelFace2;

            var lisFaces = (DispatchWrapper[])ObjectArrayToDispatchWrapperArray((mateFaces));

            profileCenterMateFeatureData.EntitiesToMate = lisFaces;

            profileCenterMateFeatureData.MateAlignment = 0;

            profileCenterMateFeatureData.LockRotation = false;

            profileCenterMateFeatureData.FlipDimension = false;

            profileCenterMateFeatureData.OffsetDistance = 0;

            Feature newMateFeature = (Feature)swAsmembly.CreateMate(profileCenterMateFeatureData);

            swModel.EditRebuild3();

            if (newMateFeature != null)
            {
                MessageBox.Show("配合完成。");
            }
        }

        private void btnOutWithCoordSystem_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            //设置用来导出文件的坐标系名称(需要装配体中有这个名称的坐标系)
            var setRes = swModel.Extension.SetUserPreferenceString(16, 0, "Coordinate System1");

            //设置导出版本

            int error = 0;

            int warnings = 0;
            //x_t的版本设置
            //swApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swParasolidOutputVersion, (int)swParasolidOutputVersion_e.swParasolidOutputVersion_161);

            swModel.Extension.SaveAs(@"D:\export.igs", (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref error, ref warnings);
            swModel.Extension.SaveAs(@"D:\export.x_t", (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref error, ref warnings);
            //Mark:SolidWorks 2018版本导出 step 用坐标系导出有bug！！！ x_t igs没有问题
            //swModel.Extension.SaveAs(@"D:\export.step", (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref error, ref warnings);

            MessageBox.Show("输出成功");
        }

        private void btnCreateDrawing_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            string dotPath = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\DrawingTemplate.DRWDOT";

            //用我们的模板新建一个工程图
            var Part = swApp.NewDocument(dotPath, 12, 0, 0);

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;
            //选中视图，并获取
            var boolstatus = swModel.Extension.SelectByID2("MyView1", "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);

            var thisView = (View)swModel.ISelectionManager.GetSelectedObject5(1);

            DrawingDoc drawingDoc = (DrawingDoc)swModel;

            string partPath = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\globalvariable.SLDPRT";
            //给预定义视图插入模型
            drawingDoc.InsertModelInPredefinedView(partPath);

            //设置视图的比例
            thisView.ScaleDecimal = 0.2;

            swModel.EditRebuild3();
        }

        private void btnTraverseFace_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            PartDoc partDoc2 = (PartDoc)swModel;

            var feat1 = (Feature)partDoc2.FeatureByName("EBox");
            var featPlaneSeg = (Feature)partDoc2.FeatureByName("Plane_Seg");

            object PlaneBox = null;

            featPlaneSeg.GetBox(ref PlaneBox);

            var PlaneBoxXYZ = (double[])PlaneBox;

            var faceCount = feat1.GetFaceCount();

            if (faceCount > 0)
            {
                var faces = (object[])feat1.GetFaces();

                for (int i = 0; i < faces.Length; i++)
                {
                    var thisFace = (Face2)faces[i];
                    object thisFaceBox = null;

                    thisFaceBox = thisFace.GetBox();

                    var thisFaceBoxXYZ = (double[])thisFaceBox;

                    if (Math.Abs(thisFaceBoxXYZ[5] - thisFaceBoxXYZ[2]) < 0.0001 && Math.Abs(thisFaceBoxXYZ[2] - PlaneBoxXYZ[2]) < 0.0001)
                    {
                        ((Entity)thisFace).Select(false);
                    }
                }
            }
        }

        private void btnGetView_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;

            //获取选择的视图对象
            View view = (View)selectionMgr.GetSelectedObject5(1);

            //获取视图的草图
            var viewSketch = view.IGetSketch();

            //var pointsCount = viewSketch.GetSketchPointsCount2();

            var points = (object[])viewSketch.GetSketchPoints2();

            var skSegs = (object[])viewSketch.GetSketchSegments();

            List<SketchPoint> segmentPoints = new List<SketchPoint>();
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var thisPoint = (SketchPoint)points[i];

                    Debug.Print(thisPoint.Type.ToString());

                    if (thisPoint.Type == 1)
                    {
                        // thisPoint.Select(true);

                        segmentPoints.Add(thisPoint);
                    }
                }
            }
            List<SketchSegment> segmentCenterLines = new List<SketchSegment>();

            if (skSegs != null)
            {
                for (int i = 0; i < skSegs.Length; i++)
                {
                    var thisSeg = (SketchSegment)skSegs[i];
                    // thisSeg.Select(false);
                    if (thisSeg.Style == (int)swLineStyles_e.swLineCENTER)
                    {
                        //thisSeg.Select(true);
                        segmentCenterLines.Add(thisSeg);
                    }
                }
            }
        }

        private void btnOpenWithHide_Click(object sender, EventArgs e)
        {
            SldWorks swApp = PStandAlone.GetSolidWorks();//连接solidworks

            //设置零件不显示
            swApp.DocumentVisible(false, (int)swDocumentTypes_e.swDocDRAWING);

            //你的操作....

            swApp.OpenDoc(@"D:\14_Model Test\206093-610\Models\57972-610b1-00(without filter)-2316.SLDDRW", (int)swDocumentTypes_e.swDocDRAWING);


            //设置零件为正常的显示
            swApp.DocumentVisible(true, (int)swDocumentTypes_e.swDocDRAWING);
        }

        private void btnAutoFillet_Click(object sender, EventArgs e)
        {
            //遍历Z轴方向的直线
            SldWorks swApp = PStandAlone.GetSolidWorks();//连接solidworks
            swApp.CommandInProgress = true; //因为是exe测试，所以启动该选项，加快速度
            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            SelectionMgr swSelMgr = default(SelectionMgr);
            Feature swFeature = null;

            swModel = (ModelDoc2)swApp.ActiveDoc; //获取当前零件

            swModelDocExt = (ModelDocExtension)swModel.Extension;

            //选择所有，零件中是选中所有的边。
            swModelDocExt.SelectAll();

            swSelMgr = (SelectionMgr)swModel.SelectionManager;

            var edgeCount = swSelMgr.GetSelectedObjectCount();  //获取 已经选中的边数

            Debug.Print("总边数：" + edgeCount);

            int faceidStart = 10000;  //设定一个面的起始id,用于识别该面是否已经被获取到。

            //List<Face2> face2sList001 = new List<Face2>();
            //List<Face2> face2sList002 = new List<Face2>();

            List<Edge> ZEdges = new List<Edge>();

            for (int i = 1; i <= edgeCount; i++)
            {
                var thisEdge = (Edge)swSelMgr.GetSelectedObject(i);

                var swCurve = (Curve)thisEdge.GetCurve();

                var thisCurveP = thisEdge.GetCurveParams3();

                if (swCurve.IsLine() == true)
                {
                    var lineV = (double[])swCurve.LineParams;

                    Debug.Print($"Root Point-> X {lineV[0].ToString()} ,Y {lineV[1].ToString()} ,Z {lineV[2].ToString()}");
                    Debug.Print($"Direction Point-> X {lineV[3].ToString()} ,Y {lineV[4].ToString()} ,Z {lineV[5].ToString()}");

                    if (lineV[3] == 0 && lineV[4] == 0)
                    {
                        ZEdges.Add(thisEdge);
                    }
                }
            }

            swModel.ClearSelection();//清除掉所有选择的边

            // 重新选中 需要处理掉0.001的面
            for (int i = 0; i < ZEdges.Count; i++)
            {
                var faceEntity = (Entity)ZEdges[i];

                // faceEntity.Select4(true, selectData);

                faceEntity.SelectByMark(true, 1);
            }
            double AsyRadius1;
            double AsyRadius2;
            double AsyRadius3;
            double AsyRadius4;
            bool boolstatus;
            double[] radiis = new double[2];
            object radiiArray0;
            object conicRhosArray0;
            object setBackArray0;
            object pointArray0;
            object pointRhoArray0;
            object dist2Array0;
            object pointDist2Array0;

            conicRhosArray0 = 0;
            setBackArray0 = 0;
            pointArray0 = 0;
            pointRhoArray0 = 0;
            pointDist2Array0 = 0;

            var FilletFea = (Feature)swModel.FeatureManager.FeatureFillet3(195, 0.002, 0.010, 0, (int)swFeatureFilletType_e.swFeatureFilletType_Simple, (int)swFilletOverFlowType_e.swFilletOverFlowType_Default, (int)swFeatureFilletProfileType_e.swFeatureFilletCircular, 0, 0, 0,
            (setBackArray0), (pointArray0), (pointDist2Array0), (pointRhoArray0));
            FilletFea.Name = "AutoFillet";
        }

        private void btnCreate3thSTDView_Click(object sender, EventArgs e)
        {


            SldWorks swApp = Utility.ConnectToSolidWorks();

            string dotPath = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\Drawing.drwdot";

            //用我们的模板新建一个工程图
            var Part = swApp.NewDocument(dotPath, 12, 0, 0);

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;        
        
            DrawingDoc drawingDoc = (DrawingDoc)swModel;

            string partPath = @"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\bodies.sldasm";

            #region 方案1


            //给视图插入模型  参数指定视图方向  与坐标
            var view1 = drawingDoc.CreateDrawViewFromModelView3(partPath, "*Front", 0.1, 0.1, 0);

            var boolstatus = swModel.Extension.SelectByID2(view1.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);

            //创建第一个关联视图
            var view2 = drawingDoc.CreateUnfoldedViewAt3(0.25, 0.1, 0, false);

            boolstatus = swModel.Extension.SelectByID2(view1.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);

            //创建第二个关联视图
            var view3 = drawingDoc.CreateUnfoldedViewAt3(0.1, 0.18, 0, false);

            //设定比例
            view1.ScaleDecimal = 0.25;

            #endregion


            #region 方案2
            //虽然 能出来，但位置要再去重新设置。
            //drawingDoc.Create3rdAngleViews2(partPath);

            #endregion



            swModel.EditRebuild3();





        }

        private void btnAddRel_Click(object sender, EventArgs e)
        {
            //Step1: 取消所有零件固定
            //Step2: 遍历所有零件
            //Step3: 循环查找每个零件与其它零件的最小距离 。
            //Step4: 最小距离为0时，再通过遍历面来查找哪些面距离 为0 并且法向平行 。
            //Step5: 选中两个面增加重合关系。

            SldWorks swApp = PStandAlone.GetSolidWorks();//连接solidworks
            swApp.CommandInProgress = true; //因为是exe测试，所以启动该选项，加快速度

            ModelDocExtension swModelDocExt = default(ModelDocExtension);
            SelectionMgr swSelMgr = default(SelectionMgr);
            Feature swFeature = null;

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            Configuration swConf = (Configuration)swModel.GetActiveConfiguration();

            Component2 swRootComp = (Component2)swConf.GetRootComponent();

            swSelMgr = (SelectionMgr)swModel.SelectionManager;

            var vChild = (object[])swRootComp.GetChildren();

            List<Component2> listcComponent2s = new List<Component2>();

            for (int i = 0; i <= (vChild.Length - 1); i++)
            {
                listcComponent2s.Add((Component2)vChild[i]);
            }

            var compCount = swSelMgr.GetSelectedObjectCount();  //获取 已经选中的边数

            AssemblyDoc assemblyDoc = (AssemblyDoc)swModel;

            swModel.ClearSelection();

            for (int i = 1; i < listcComponent2s.Count; i++)
            {
                listcComponent2s[i].Select(true);
            }

            assemblyDoc.UnfixComponent();

            swModel.ClearSelection();
            for (int i = 0; i < listcComponent2s.Count; i++)
            {
                var AComp = listcComponent2s[i];

                for (int j = i + 1; j < listcComponent2s.Count; j++)
                {
                    var BComp = listcComponent2s[j];

                    var d = swModel.ClosestDistance(AComp, BComp, out object p1, out object p2);

                    if (d * 1000 < 0.001 && -0.001 < d * 1000)
                    {
                        Debug.Print($@"{AComp.Name} 与 {BComp.Name} -->距离 为{d}");
                        CheckFaceStatus(AComp, BComp, swModel, swApp);
                    }
                }
            }

            swModel.EditRebuild3();
            swApp.CommandInProgress = false;
            MessageBox.Show("Done");
        }

        private void CheckFaceStatus(Component2 aComp, Component2 bComp, ModelDoc2 swModel, SldWorks swApp)
        {
            List<Face2> listFace21 = new List<Face2>();
            List<Face2> listFace22 = new List<Face2>();

            var bodys1 = (object[])aComp.GetBodies((int)swBodyType_e.swSolidBody);

            var bodys2 = (object[])bComp.GetBodies((int)swBodyType_e.swSolidBody);

            for (int i = 0; i < bodys1.Length; i++)
            {
                var thisBody = (Body2)bodys1[i];
                var thisBodyFaces = (object[])thisBody.GetFaces();

                for (int j = 0; j < thisBodyFaces.Length; j++)
                {
                    listFace21.Add((Face2)thisBodyFaces[j]);
                }
            }

            for (int i = 0; i < bodys2.Length; i++)
            {
                var thisBody = (Body2)bodys2[i];
                var thisBodyFaces = (object[])thisBody.GetFaces();

                for (int j = 0; j < thisBodyFaces.Length; j++)
                {
                    listFace22.Add((Face2)thisBodyFaces[j]);
                }
            }

            for (int i = 0; i < listFace21.Count; i++)
            {
                var face1 = listFace21[i];

                for (int j = 0; j < listFace22.Count; j++)
                {
                    var face2 = listFace22[j];

                    var d = swModel.ClosestDistance(face1, face2, out object p1, out object p2);

                    if (d * 1000 < 0.001 && -0.001 < d * 1000)
                    {
                        var part1Staus = aComp.GetConstrainedStatus();
                        var part2Staus = bComp.GetConstrainedStatus();

                        if (part1Staus == (int)swConstrainedStatus_e.swFullyConstrained && part2Staus == (int)swConstrainedStatus_e.swFullyConstrained)
                        {
                            return;
                        }

                        //(face1 as Entity).Select(false);
                        //(face2 as Entity).Select(true);

                        var norm1 = (double[])face1.Normal;
                        var norm2 = (double[])face2.Normal;

                        //要转换到装配体中

                        var swMathUtil = (MathUtility)swApp.GetMathUtility();
                        var trans1 = aComp.Transform;
                        var trans2 = bComp.Transform;

                        MathVector tVector1 = (MathVector)swMathUtil.CreateVector((norm1));
                        MathVector tVector2 = (MathVector)swMathUtil.CreateVector((norm2));

                        var swNormalVector1 = (MathVector)tVector1.MultiplyTransform(aComp.Transform2);
                        var swNormalVector2 = (MathVector)tVector2.MultiplyTransform(bComp.Transform2);

                        GeometRi.Vector3d v1 = new GeometRi.Vector3d((double[])swNormalVector1.ArrayData);

                        GeometRi.Vector3d v2 = new GeometRi.Vector3d((double[])swNormalVector2.ArrayData);

                        if (v1.Normalized.IsParallelTo(v2.Normalized))
                            if (v1.Normalized.X == v2.Normalized.X && v1.Normalized.Y == v2.Normalized.Y && v1.Normalized.Z == v2.Normalized.Z)
                            {
                                //(face1 as Entity).Select(false);
                                //(face2 as Entity).Select(true);

                                ////var swMathUtil = (MathUtility)swApp.GetMathUtility();
                                ////var n1= swMathUtil.ICreateVector().Normalise();
                                ////n1.ArrayData
                                ////  var swVector = (MathVector)swMathUtil.CreateVector(vectorPoint);

                                int longstatus = 0;
                                //重合

                                //swApp.RunCommand((int)swCommands_e.swCommands_Addedit_Mate, "");
                                //swApp.RunCommand((int)swCommands_e.swCommands_PmOK, "");

                                (swModel as AssemblyDoc).AddMate5(0, 0, false, 0, 0.001, 0.001, 0.001, 0.001, 0, 0, 0, false, true, 0, out longstatus);
                                swModel.EditRebuild3();

                                // MessageBox.Show("重合关系");
                            }
                            else
                            {
                                (face1 as Entity).Select(false);
                                (face2 as Entity).Select(true);

                                //var swMathUtil = (MathUtility)swApp.GetMathUtility();
                                //var n1= swMathUtil.ICreateVector().Normalise();
                                //n1.ArrayData
                                //  var swVector = (MathVector)swMathUtil.CreateVector(vectorPoint);

                                int longstatus = 0;
                                //重合

                                //swApp.RunCommand((int)swCommands_e.swCommands_Addedit_Mate, "");
                                //swApp.RunCommand((int)swCommands_e.swCommands_PmOK, "");

                                (swModel as AssemblyDoc).AddMate5(0, 2, false, 0, 0.001, 0.001, 0.001, 0.001, 0, 0, 0, false, true, 0, out longstatus);
                                swModel.EditRebuild3();
                            }
                    }
                }
            }
        }

        private void btnSettingAutoCutList_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                Configuration swConf = (Configuration)swModel.GetActiveConfiguration();

                Component2 swRootComp = (Component2)swConf.GetRootComponent();

                //遍历
                Utility.TraverseCompXform(swRootComp, 0, false, true);
            }
        }

        private void btnModifyTemplate_Click(object sender, EventArgs e)
        {
            //先打开Measure.slddrw
            SldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc; //当前文档

                var swDrawing = (DrawingDoc)swModel; //转换成工程图对象

                //当前图纸的信息。
                //var pageSetup = swDrawing.Sheet["Sheet1"].IPageSetup;

                //var res=  swDrawing.SetupSheet6("Sheet1", (int)swDwgPaperSizes_e.swDwgPaperA3size,
                //    (int)swDwgTemplates_e.swDwgTemplateA3size, 1, 5, true, "a3 - din.slddrt", 0, 0, "默认", false, 0, 0, 0, 0, 0, 0);
                //if (res)
                //{
                //    MessageBox.Show("更换成功。");
                //}

                var res = swDrawing.SetupSheet6("Sheet1", (int)swDwgPaperSizes_e.swDwgPaperA4size,
                    (int)swDwgTemplates_e.swDwgTemplateA4size, 1, 5, true, @"E:\01_Work\22_Gitee\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\sheetformat\a4 - gb.slddrt", 0, 0, "默认", true, 0, 0, 0, 0, 0, 0);
                if (res)
                {
                    MessageBox.Show("更换成功。");
                }
            }
        }

        private void btnModifyWeldment_Click(object sender, EventArgs e)
        {
            //请先打开WeldmentTest.sldprt
            //下面的功能是把原有的特征数据从020x020换成030x030.SLDLFP

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var swWeldFeat = (Feature)(swModel as PartDoc).FeatureByName("ABC");

            var swWeldFeatData = (StructuralMemberFeatureData)swWeldFeat.GetDefinition();

            swWeldFeatData.AccessSelections(swModel, null);

            var profilePath = swWeldFeatData.WeldmentProfilePath;

            var newPath = profilePath.Replace("020x020", "030x030");

            swWeldFeatData.WeldmentProfilePath = newPath;

            var boolstatus = swWeldFeat.ModifyDefinition(swWeldFeatData, swModel, null);

            swWeldFeatData.ReleaseSelectionAccess();

            swModel.EditRebuild3();
        }

        private void btnGetCutList_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var cutListPro = GetCutListPropertyManager(swModel);

            for (int i = 0; i < cutListPro.Count; i++)
            {
                var cutListName = (string[])cutListPro[i].GetNames();

                for (int j = 0; j < cutListName.Length; j++)
                {
                    var tempProName = cutListName[j];

                    bool resolved;
                    cutListPro[i].Get5(tempProName, false, out string prpVal, out string prpResVal, out resolved);
                    if (tempProName.Contains("LENGTH"))
                    {
                        Debug.Print($"{tempProName}->{prpVal}---->{prpResVal}");
                    }
                }
            }

            MessageBox.Show("获取完成");
        }

        private List<CustomPropertyManager> GetCutListPropertyManager(ModelDoc2 swModel)
        {
            List<CustomPropertyManager> customPropertyManagers = new List<CustomPropertyManager>();

            var swFeat = (Feature)swModel.FirstFeature();

            while (swFeat != null)
            {
                if (swFeat.GetTypeName2() == "CutListFolder")
                {
                    customPropertyManagers.Add(swFeat.CustomPropertyManager);

                    var bodyFolder = (BodyFolder)swFeat.GetSpecificFeature2();

                    var bodies = (object[])bodyFolder.GetBodies();

                    var tempBody = (Body2)bodies[0];

                    var fea = (object[])tempBody.GetFeatures();

                    var tempBodyFea = (Feature)fea[0];

                    Debug.Print(tempBodyFea.Name);
                }
                swFeat = (Feature)swFeat.GetNextFeature();
            }

            return customPropertyManagers;
        }

        private void btnCreateDimXpert_Click(object sender, EventArgs e)
        {

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            #region 读取一个选中的3d尺寸

            //var selectedObject = (object)swModel.ISelectionManager.GetSelectedObject6(1, -1);

            //var dimXpertFeature = selectedObject as DimXpertFeature;

            //Debug.Print(dimXpertFeature.Name);

            //var dimType = dimXpertFeature.Type;

            //Debug.Print("XpertFeature Type" + dimType);

            //var dimAnnotations = (object[])dimXpertFeature.GetAppliedAnnotations();

            //for (int i = 0; i < dimAnnotations.Length; i++)
            //{
            //    var tempAnno = (DimXpertAnnotation)dimAnnotations[i];

            //    Debug.Print(tempAnno.Type.ToString());


            //    Debug.Print(tempAnno.Name);

            //    var dimTol = tempAnno as DimXpertDimensionTolerance;

            //    //获取公差请参考https://help.solidworks.com/2018/english/api/swdocmgrapi/get_dimxpert_tolerance5_example_csharp.htm

            //    MessageBox.Show($"{dimTol.ToString()}");

            //}
            #endregion

            //通过先选中基准面，来执行自动尺寸方案。

            //这里选中的标记的动作后面需要用程序去选择，利用SelectByRay选择太局限了。

            #region VBA可运行版本
            //boolstatus = Part.Extension.SelectByRay(-0.199999999999932, 7.99159865590582E-03, 2.08147507191825E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, False, 0, 0)
            //boolstatus = Part.Extension.SelectByRay(-0.193540180170828, 0, 7.74886573458389E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, True, 0, 0)
            //boolstatus = Part.Extension.SelectByRay(-8.93087496499163E-02, 7.75246027125149E-02, 6.27227943436992E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, True, 0, 0)
            //boolstatus = Part.SelectionManager.SetSelectedObjectMark(3, 51, swSelectionMarkSet)
            //boolstatus = Part.Extension.SelectByRay(-0.117784661174085, 3.60124267366473E-02, 4.68486280385605E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, True, 0, 0)
            //boolstatus = Part.SelectionManager.SetSelectedObjectMark(4, 52, swSelectionMarkSet)
            //Dim swConfig As Object
            //Dim swDimXPertMgr As Object
            //Dim swDimXPertPart As Object
            //Dim swADSOpt As Object
            //Set swConfig = Part.ConfigurationManager.ActiveConfiguration
            //Set swDimXPertMgr = Part.Extension.DimXpertManager(swConfig.Name, True)
            //Set swDimXPertPart = swDimXPertMgr.DimXpertPart
            //Set swADSOpt = swDimXPertPart.GetAutoDimSchemeOption()
            //swADSOpt.PartType = swDimXpertAutoDimSchemePartType_Prismatic
            //swADSOpt.ToleranceType = swDimXpertAutoDimSchemeToleranceType_PlusMinus
            //swADSOpt.PatternType = swDimXpertAutoDimSchemePatternType_Linear
            //swADSOpt.PolarPatternHoleCount = 5
            //swADSOpt.ScopeAllFeature = False
            //boolstatus = swDimXPertPart.AutoDimensionScheme(swADSOpt)
            //Part.ClearSelection2 True
            #endregion

            //先打开 MBDTest_Clear.SLDPRT
            #region C#转换版
            var boolstatus = swModel.Extension.SelectByRay(-0.199999999999932, 7.99159865590582E-03, 2.08147507191825E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, false, 0, 0);
            boolstatus =swModel.Extension.SelectByRay(-0.193540180170828, 0, 7.74886573458389E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, true, 0, 0);
            boolstatus =swModel.Extension.SelectByRay(-8.93087496499163E-02, 7.75246027125149E-02, 6.27227943436992E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, true, 0, 0);
            boolstatus =swModel.ISelectionManager.SetSelectedObjectMark(3, 51,(int)swSelectionMarkAction_e.swSelectionMarkSet);
            boolstatus =swModel.Extension.SelectByRay(-0.117784661174085, 3.60124267366473E-02, 4.68486280385605E-03, 0.388737791327055, 0.42257073135628, -0.81872883581512, 1.1566601033182E-03, 2, true, 0, 0);
            boolstatus =swModel.ISelectionManager.SetSelectedObjectMark(4, 52, (int)swSelectionMarkAction_e.swSelectionMarkSet);
            var swConfig = swModel.ConfigurationManager.ActiveConfiguration;

            var swDimXPertMgr = swModel.Extension.DimXpertManager[swConfig.Name, true];
            var swDimXPertPart = (DimXpertPart)swDimXPertMgr.DimXpertPart;
            var swADSOpt = swDimXPertPart.GetAutoDimSchemeOption();

            swADSOpt.PartType = (int)swDimXpertAutoDimSchemePartType_e.swDimXpertAutoDimSchemePartType_Prismatic;
            swADSOpt.ToleranceType = (int)swDimXpertAutoDimSchemeToleranceType_e.swDimXpertAutoDimSchemeToleranceType_PlusMinus;
            swADSOpt.PatternType = (int)swDimXpertAutoDimSchemePatternType_e.swDimXpertAutoDimSchemePatternType_Linear;
            swADSOpt.PolarPatternHoleCount = 5;
            swADSOpt.ScopeAllFeature = false;
            boolstatus = swDimXPertPart.AutoDimensionScheme(swADSOpt);

            swModel.ClearSelection2(true);

            #endregion

        }

        private void butAddSize_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            AddSizeDimensionForDrawing addSizeDimensionForDrawing = new AddSizeDimensionForDrawing(swApp, swModel);
            addSizeDimensionForDrawing.AutoAddSize(false,false);

        }

        private void btnAddDimForHoles_Click(object sender, EventArgs e)
        {

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;
            AddSizeDimensionForDrawing addSizeDimensionForDrawing = new AddSizeDimensionForDrawing(swApp, swModel);
            addSizeDimensionForDrawing.AutoAddHoleDimesnions("");
        }

        private void btnKeepView_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;                     

            var oldView = swApp.IActiveDoc2.IActiveView;

            var oldAsmOrientation = oldView.Orientation3; //矩阵向量
            var oldAsmsc = oldView.Scale2; //矩阵向量

            var swModelAsm = (AssemblyDoc)swModel; //获取当前零件
                                 
            var baseComp = swModel.ISelectionManager.GetSelectedObjectsComponent3(1, -1);

            var PartTransform2 = baseComp.Transform2;//零件与装配体坐标完全重合 变换到安装位置的变换
                           
            baseComp.Select(false);
            
            swModelAsm.OpenCompFile();

            swModel = (ModelDoc2)swApp.ActiveDoc;

            var newView = swApp.IActiveDoc2.IActiveView;

            newView.Orientation3 = PartTransform2.IMultiply(oldAsmOrientation);
           
            //newView.Scale2 = oldAsmsc;  

            swModel.ViewZoomtofit();

            newView.GraphicsRedraw(null);

        }


        private static MathTransform MatrixToTransform(SldWorks swApp, Matrix3d changeMatrix)
        {
            double[] mathArray1 = new double[16];
            mathArray1[0] = changeMatrix.Row1.X;
            mathArray1[1] = changeMatrix.Row1.Y;
            mathArray1[2] = changeMatrix.Row1.Z;
            mathArray1[3] = changeMatrix.Row2.X;
            mathArray1[4] = changeMatrix.Row2.Y;
            mathArray1[5] = changeMatrix.Row2.Z;
            mathArray1[6] = changeMatrix.Row3.X;
            mathArray1[7] = changeMatrix.Row3.Y;
            mathArray1[8] = changeMatrix.Row3.Z;
            mathArray1[9] = 0;
            mathArray1[10] = 0;
            mathArray1[11] = 0;
            mathArray1[12] = 0;
            mathArray1[13] = 0;
            mathArray1[14] = 0;
            mathArray1[15] = 0;

            //double[] mathArray = new double[16] { 1, 0, 0, 0,1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 };

            MathTransform mathTransform = (MathTransform)swApp.IGetMathUtility().CreateTransform(mathArray1);
            return mathTransform;
        }

        private void btnAddCenterMark_Click(object sender, EventArgs e)
        {

            //请先打开HolePlate.slddrw文件

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var swDrawing = (DrawingDoc)swModel;

            //这里改成工程图中的视图名称

            swDrawing.ActivateView("Drawing View1");

            #region 遍历并删除
            //得到视图对象
            var swView = (View)swDrawing.ActiveDrawingView;

            //获取所有注释
            var annos = (object[])swView.GetAnnotations();

            //清空选择
            swModel.ClearSelection();

            //遍历中心标记
            foreach (var anno in annos)
            {
                var tempAnno = (Annotation)anno;
                if (tempAnno.GetType() == (int)swAnnotationType_e.swCenterMarkSym)
                {
                    //选中
                    tempAnno.Select(true);

                }

            }

            //删除选中
            swModel.EditDelete();
            #endregion


            #region 自动增加
            //增加中心标记

            swView.AutoInsertCenterMarks2((int)swAutoInsertCenterMarkTypes_e.swAutoInsertCenterMarkType_Slots| (int)swAutoInsertCenterMarkTypes_e.swAutoInsertCenterMarkType_Hole , (int)swCenterMarkConnectionLine_e.swCenterMark_ShowLinearConnectLines, true, true, true, 0.0025, 0.0025, true, true, 0);

            #endregion



            MessageBox.Show("操作完成");

        }

        private void btnGetSketchDim_Click(object sender, EventArgs e)
        {

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var selMgr = swModel.ISelectionManager;

            var displayDim = (Dimension)selMgr.GetSelectedObject(1);

            

            //获取尺寸的参考点对象
            var pObjs = (object[])displayDim.ReferencePoints;

            var p1 =(MathPoint) pObjs[0];

            var p2 =(MathPoint) pObjs[1];


            MessageBox.Show("");



        }
        /// <summary>
        /// 获取工程图中的图片对象，删除后插入一个已经存在的二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOLEObject_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            //old对象数量
            var oleCountOnThisSheet= swModel.Extension.GetOLEObjectCount((int)swOleObjectOptions_e.swOleObjectOptions_GetOnCurrentSheet);
            //获取 old对象
            var oleObjects = (object[])swModel.Extension.GetOLEObjects((int)swOleObjectOptions_e.swOleObjectOptions_GetOnCurrentSheet);

            //var buf1 = new object();

            //循环 按顺序 选中后删除
            for (int i = 0; i < oleObjects.Count(); i++)
            {
                var oldObj=(SwOLEObject)oleObjects[i];

                Debug.Print(oldObj.Clsid);
                //oldObj.IGetBuffer(oldObj.BufferSize, out byte bData);
                Debug.Print(oldObj.Aspect.ToString());
         
                //buf1 = oldObj.Buffer;

                //选中
                oldObj.Select(false);
               
                //删除
                swModel.EditDelete();
               
            }

            //重新插入一张图片
            //var picPath = @"D:\09_Study\CSharpAndSolidWorks\8888.png";
            ////swModel.Extension.InsertObjectFromFile(picPath, false, 1, 0.1, 0.1, 0);

            //var pos = new double[] { 0, 0.2, 0.03,0.23};

            //Todo: 如何把文件变成Buffer 

            //var impBuf = imageToByte(image);

            //var newOleObj=  swModel.Extension.CreateOLEObject(1, pos, impBuf, out int Err);

            //newOleObj.Refresh();


            #region 利用草图里的插入图片功能

            var picPath = @"D:\09_Study\CSharpAndSolidWorks\v.png";

            var skPic = swModel.SketchManager.InsertSketchPicture(picPath);

            skPic.SetOrigin(0.2, 0.2); //坐标

            skPic.SetSize(0.05, 0.05, true); //大小

            //取消属性显示页
            swApp.RunCommand(-2, "");



            #endregion


            MessageBox.Show("操作完成");

        }

        [DllImport("ole32.dll",
     EntryPoint = "GetHGlobalFromILockBytes",
     ExactSpelling = true, PreserveSig = true, CharSet = CharSet.Ansi,
     CallingConvention = CallingConvention.StdCall)]
        static extern int GetHGlobalFromILockBytes([MarshalAs(UnmanagedType.Interface)] object /* ILockBytes* */pLkbyt, out IntPtr /* HGLOBAL* */ phglobal);
        //将image转换成byte[]数据
        private byte[] imageToByte(System.Drawing.Image _image)
        {
            MemoryStream ms = new MemoryStream();
            _image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        //将byte[]数据转换成image
        private Image byteToImage(byte[] myByte)
        {
            MemoryStream ms = new MemoryStream(myByte);
            Image _Image = Image.FromStream(ms);
            return _Image;
        }


        /// <summary>
        /// 工程图替换文件引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReplaceModelForView_Click(object sender, EventArgs e)
        {
            var actPath = RegDllPath("");

            var start = actPath.Substring(0, actPath.IndexOf("CSharpAndSolidWorks", 0));

            var oldPartName = $@"{start}CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\replaceDrawingRef\AA(BB).SLDPRT";

            var newPartName = $@"{start}CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\replaceDrawingRef\AA(BB) - 副本.SLDPRT";

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var docModel = swModel as DrawingDoc;

            var views = (object[])docModel.GetViews();

            List<View> views1 = new List<View>();
            List<Component2> comps1 = new List<Component2>();


            for (int i = 0; i < views.Length; i++)
            {
                var tempV = (object[])views[i];

                if (tempV.Length>1)
                {
                    for (int j =1; j < tempV.Length; j++)
                    {
                        var tempView = (View)tempV[j];
                        if (tempView.RootDrawingComponent.Component!=null)
                        {
                            if (tempView.RootDrawingComponent.Component.IGetModelDoc().GetPathName().ToUpper() == oldPartName.ToUpper())
                            {
                                views1.Add(tempView);
                                comps1.Add(tempView.RootDrawingComponent.Component);
                            }

                        }
                        else
                        {
                            var visComps = (object[])tempView.GetVisibleComponents();

                            if (tempView.GetVisibleComponentCount()==1 && ((visComps[0] as Component2).IGetModelDoc().GetPathName().ToUpper() == oldPartName.ToUpper()))
                            {
                                views1.Add(tempView);
                                comps1.Add(visComps[0] as Component2);
                            }

                        }

                    }

                }


            }

    

            //DispatchWrapper[] viewsIn = new DispatchWrapper[views1.Count];
            //DispatchWrapper[] instancesIn = new DispatchWrapper[views1.Count];




            var res= docModel.ReplaceViewModel(newPartName, views1.ToArray(), comps1.ToArray());

        }

        private void btnSimStudy_Click(object sender, EventArgs e)
        {
            var actPath = RegDllPath("");

            var start = actPath.Substring(0,actPath.IndexOf("CSharpAndSolidWorks",0));
                    

            var partPath = $@"{start}CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\Simulation API Demo.SLDPRT";

            SldWorks swApp = PStandAlone.GetSolidWorks();

        
            CWModelDoc swsActDoc = default(CWModelDoc);
            CWStudyManager swsStudyMngr = default(CWStudyManager);
            CWStudy swsStudy = default(CWStudy);
            CWLoadsAndRestraintsManager swsLBCMgr = default(CWLoadsAndRestraintsManager);
            CWForce swsCWForce = default(CWForce); 

            int errors = 0;
            int warnings = 0;
       

            string fileName = partPath;

            // 打开模型
            var swModel = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocPART, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);

            // Get the SOLIDWORKS Simulation object
            dynamic COSMOSWORKS = default(dynamic);
            dynamic COSMOSObject = default(dynamic);

            // Determine host SOLIDWORKS major version
            int swVersion = Convert.ToInt32(swApp.RevisionNumber().Substring(0, 2));

            // Calculate the version-specific ProgID of the Simulation add-in that is compatible with this version of SOLIDWORKS
            int cwVersion = swVersion - 15;
            String cwProgID = String.Format("SldWorks.Simulation.{0}", cwVersion);
            Debug.Print(cwProgID);

            // Get the SOLIDWORKS Simulation object
            COSMOSObject = swApp.GetAddInObject(cwProgID);

            COSMOSWORKS = COSMOSObject.CosmosWorks;

            // Open and get active document
            swsActDoc = (CWModelDoc)COSMOSWORKS.ActiveDoc;

            if (swsActDoc == null) ErrorMsg(swApp, "No active document");

            // Create new static study
            swsStudyMngr = (CWStudyManager)swsActDoc.StudyManager;
            if (swsStudyMngr == null) ErrorMsg(swApp, "No CWStudyManager object");

            //得到第一个算例对象
            swsStudy = (CWStudy)swsStudyMngr.GetStudy(0);

            //算例名称，可以区分多个算例   swsStudy.Name



            if (swsStudy == null) ErrorMsg(swApp, "No CWStudy object");

            swsLBCMgr = (CWLoadsAndRestraintsManager)swsStudy.LoadsAndRestraintsManager;

            //get the Force Feature
            //这是是第一个条件，所以参数写0
            var sCwForce = swsLBCMgr.GetLoadsAndRestraints(0, out int errcode);

            if (sCwForce != null)
            {
                var swForce = (CWForce)sCwForce;
            
                swForce.ForceBeginEdit();

                

                //修改值为150
                swForce.NormalForceOrTorqueValue = 150;  
                var res = swForce.ForceEndEdit();

            }

            //Create mesh
            var CwMesh = (CWMesh)swsStudy.Mesh;

            if (CwMesh == null) ErrorMsg(swApp, "No mesh object");

            CwMesh.Quality = 1;
            CwMesh.GetDefaultElementSizeAndTolerance(0, out double el, out double tl);
            var errCode = swsStudy.CreateMesh(0, el, tl);
            if (errCode != 0) ErrorMsg(swApp, "Mesh failed");

            //Run
            swsStudy.RunAnalysis();
        }

        public void ErrorMsg(object swApp, string Message)
        {
            MessageBox.Show(Message);
            MessageBox.Show("'*** WARNING - General");
            MessageBox.Show("'*** " + Message);
            MessageBox.Show("");
        }

        private void btnImportDwg_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            string partDefaultTemplate = swApp.GetDocumentTemplate((int)swDocumentTypes_e.swDocPART, "", 0, 0, 0);
            
            var newDoc = swApp.NewDocument(partDefaultTemplate, 0, 0, 0);
                        
            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var actPath = RegDllPath("");

            var start = actPath.Substring(0, actPath.IndexOf("CSharpAndSolidWorks", 0));        

            var dwgPath = $@"{start}CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\ImportDWG.dwg";

            bool boolstatus = swModel.Extension.SelectByID2("前视基准面", "PLANE", 0, 0, 0, false, 0, null, 0);
            if (!boolstatus)
            {
                 boolstatus = swModel.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);

                if (!boolstatus)
                {
                    boolstatus = swModel.Extension.SelectByID2("Front", "PLANE", 0, 0, 0, false, 0, null, 0);

                    if (!boolstatus)
                    {
                        boolstatus = swModel.Extension.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);

                        if (!boolstatus)
                        {
                            MessageBox.Show("请选中一个基准面，再点确定。");

                        }           
                        
                    }
                }
            }
         
                       

            ImportDxfDwgData importData = default(ImportDxfDwgData);

            importData = (ImportDxfDwgData)swApp.GetImportFileData(dwgPath);

            importData.set_LengthUnit("", (int)swLengthUnit_e.swMM);
                       
            var bRet = importData.SetPosition("", (int)swDwgImportEntitiesPositioning_e.swDwgEntitiesCentered, 0, 0);
               
            importData.set_ImportMethod("", (int)swImportDxfDwg_ImportMethod_e.swImportDxfDwg_ImportToExistingPart);

            importData.SetMergePoints("", true, 0.001);

            importData.SetImportLayerVisibility(null, (int)swImportDxfDwg_LayerVisibility_e.swImportDxfDwg_LayerHidden);

            var listLayerNames = new List<string>() { "2" };
            var tempArray = listLayerNames.ToArray();
            object layers = tempArray;
            importData.SetImportLayerVisibility(layers, (int)swImportDxfDwg_LayerVisibility_e.swImportDxfDwg_LayerVisible);
               
            var LayFea01 = swModel.FeatureManager.InsertDwgOrDxfFile2(dwgPath, importData);

            if (LayFea01!=null)
            {
                LayFea01.Name = "导入的图层2";
            }
   

        }

    
        private void btnAddCallout_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();

            var colortable = (ColorTable)swApp.GetColorTable();
           

            ModelDoc2 swModel;
            ModelDocExtension swExt;
            SelectionMgr swSelMgr;
            MathUtility mathUtil;

            swModel = (ModelDoc2)swApp.ActiveDoc;
            swExt = swModel.Extension;
            swSelMgr = (SelectionMgr)swModel.SelectionManager;

            var mousePoint = (double[])swSelMgr.GetSelectionPoint2(1, -1);

            mathUtil = (MathUtility)swApp.GetMathUtility();
            calloutHandler handle = new calloutHandler();
            MathPoint mp;
            double[] vPnt = new double[3];
            vPnt[0] = 0.0;
            vPnt[1] = 0.0;
            vPnt[2] = 0.0;
            mp = (MathPoint)mathUtil.CreatePoint(mousePoint);
            Callout myCallout;
            myCallout = swExt.CreateCallout(2, handle);
            myCallout.set_Value(1, "-");
            myCallout.set_IgnoreValue(1, true);
            myCallout.set_Label2(1, "SldWorks API");
            //myCallout.SkipColon = true;
            //myCallout.TextColor[0] = t.GetColorRefAtIndex(0);
            //myCallout.OpaqueColor = t.GetColorRefAtIndex(1);

            myCallout.SetLeader(true, false);
            myCallout.SetTargetPoint(0, mousePoint[0], mousePoint[1], mousePoint[2]);
            //myCallout.SetTargetPoint(2, -0.001, 0.001, 0);
            myCallout.Position = mp;
            myCallout.set_ValueInactive(0, true);
            myCallout.TextBox = false;
            myCallout.Display(true);

            TextFormat swTextFormat = myCallout.TextFormat;
            ProcessTextFormat(swApp, swModel, swTextFormat);



        }
        public void ProcessTextFormat(SldWorks swApp, ModelDoc2 swModel, TextFormat swTextFormat)
        {

            Debug.Print("    BackWards                    = " + swTextFormat.BackWards);
            Debug.Print("    Bold                         = " + swTextFormat.Bold);
            Debug.Print("    CharHeight                   = " + swTextFormat.CharHeight);
            Debug.Print("    CharHeightInPts              = " + swTextFormat.CharHeightInPts);
            Debug.Print("    CharSpacingFactor            = " + swTextFormat.CharSpacingFactor);
            Debug.Print("    Escapement                   = " + swTextFormat.Escapement);
            Debug.Print("    IsHeightSpecifiedInPts       = " + swTextFormat.IsHeightSpecifiedInPts());
            Debug.Print("    Italic                       = " + swTextFormat.Italic);
            Debug.Print("    LineLength                   = " + swTextFormat.LineLength);
            Debug.Print("    LineSpacing                  = " + swTextFormat.LineSpacing);
            Debug.Print("    ObliqueAngle                 = " + swTextFormat.ObliqueAngle);
            Debug.Print("    Strikeout                    = " + swTextFormat.Strikeout);
            Debug.Print("    TypeFaceName                 = " + swTextFormat.TypeFaceName);
            Debug.Print("    Underline                    = " + swTextFormat.Underline);
            Debug.Print("    UpsideDown                   = " + swTextFormat.UpsideDown);
            Debug.Print("    Vertical                     = " + swTextFormat.Vertical);
            Debug.Print("    WidthFactor                  = " + swTextFormat.WidthFactor);

            Debug.Print("");

        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            //这个要先打开 bodies.sldasm
            var swApp = PStandAlone.GetSolidWorks();

            var currentLang= swApp.GetCurrentLanguage();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var boolstatus = swModel.Extension.SelectByID2("bodies-零件2-1@bodies", "COMPONENT", 0, 0, 0, false, 0, null, 0);

            var longstatus = swModel.Extension.RenameDocument("bodies-零件2-newName");

            if (longstatus==0)
            {
                swApp.SendMsgToUser("已经重命名！保存后生效。");
             }
            else
            {
                swApp.SendMsgToUser("失败了！");
            }



        }

  
        private void btnOrdinateDimension_Click(object sender, EventArgs e)
        {

            //请先打开HolePlate-2.slddrw文件

            var swApp = PStandAlone.GetSolidWorks();

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var swDrawing = (DrawingDoc)swModel;

            //这里改成工程图中的视图名称

            swDrawing.ActivateView("Drawing View1");



            #region 遍历并删除
            //得到视图对象
            var swView = (View)swDrawing.ActiveDrawingView;


            //找最小点
            var baseOrigin = FindOriginVertex(swApp, swView);

            var circleEdges = AddCenterPointForCircle(swApp, swView);

            //视图的外围位置 

            var viewLoc = (double[])swView.GetOutline();


            (baseOrigin as Entity).Select(false);

            // 这里还有个坐标相同的处理可以优化。

            foreach (var item in circleEdges)
            {
                (item as Entity).Select(true);

            }


            swDrawing.AddOrdinateDimension2((int)swAddOrdinateDims_e.swHorizontalOrdinate, 0, viewLoc[1] - 0.01, 0);


            (baseOrigin as Entity).Select(false);

            foreach (var item in circleEdges)
            {
                (item as Entity).Select(true);

            }

            swDrawing.AddOrdinateDimension2((int)swAddOrdinateDims_e.swVerticalOrdinate, viewLoc[0] - 0.01, 0, 0);

            #endregion




            MessageBox.Show("操作完成");



        }

        public Vertex FindOriginVertex(SldWorks swApp, View view)
        {
            Vertex resVertex = null;

            var vComps = (object[])view.GetVisibleComponents();

            foreach (var Comp in vComps)
            {
                var swComp = (Component2)Comp;
                //所以可见点
                var visbaleEnts = (object[])view.GetVisibleEntities2(swComp, (int)swViewEntityType_e.swViewEntityType_Vertex);

                for (int i = 0; i < visbaleEnts.Count(); i++)
                {
                    var swVertex = (Vertex)visbaleEnts[i];
                    (swVertex as Entity).Select(false);
                    if (resVertex == null)
                    {
                        resVertex = swVertex;

                    }
                    else
                    {
                        var vCurOrigCoord = GetVertexCoordinate(swApp, resVertex, view.ModelToViewTransform);
                        var vCoord = GetVertexCoordinate(swApp, swVertex, view.ModelToViewTransform);

                        if (vCoord[0] < vCurOrigCoord[0] && vCoord[1] < vCurOrigCoord[1])
                        {

                            resVertex = swVertex;
                        }



                    }


                }



            }

            return resVertex;

        }

        public List<Edge> AddCenterPointForCircle(SldWorks swApp, View view)
        {
            List<Edge> res = new List<Edge>();



            var vComps = (object[])view.GetVisibleComponents();

            foreach (var Comp in vComps)
            {
                var swComp = (Component2)Comp;
                //所有可见面
                var visbaleEnts = (object[])view.GetVisibleEntities2(swComp, (int)swViewEntityType_e.swViewEntityType_Face);

                for (int i = 0; i < visbaleEnts.Count(); i++)
                {
                    var swFace = (Face2)visbaleEnts[i];

                    var swThisLoop = (Loop2)swFace.GetFirstLoop();
                    while ((swThisLoop != null))
                    {
                        // Hole is inner loop
                        // Circular or elliptical hole has only one edge
                        bool bCount;
                        var bRet = swThisLoop.IsOuter();  //外环
                        var count = swThisLoop.GetEdgeCount(); //边数
                        if (count != 1 && count != 4)
                        {
                            bCount = false;
                        }
                        else
                        {
                            var edges = (object[])swThisLoop.GetEdges();

                            foreach (var e in edges)
                            {
                                var tempEdge = e as Edge;

                                if (tempEdge.IGetCurve().IsCircle())
                                {
                                    res.Add(tempEdge);
                                }
                            }

                            bCount = true;
                        }


                        // Move on to the next
                        swThisLoop = (Loop2)swThisLoop.GetNext();
                    }




                }



            }


            return res;

        }



        private double[] GetVertexCoordinate(SldWorks swApp, Vertex resVertex, MathTransform swViewTransform)
        {
            var vCoorPt = resVertex.GetPoint();
            var swMathPt = (MathPoint)swApp.IGetMathUtility().CreatePoint(vCoorPt);
            swMathPt = (MathPoint)swMathPt.MultiplyTransform(swViewTransform);
            return (double[])swMathPt.ArrayData;
        }

        private void btnSketchContour_Click(object sender, EventArgs e)
        {

        }

        private void btnCommandManagerTab_Click(object sender, EventArgs e)
        {
            var swApp = PStandAlone.GetSolidWorks();
            var swModelDoc = (ModelDoc2)swApp.ActiveDoc;
            var swModelDocExt = swModelDoc.Extension;

            //获取所有标签页
            var  cmdTabs = swModelDocExt.GetCommandTabs();      
            var activeTab = swModelDocExt.ActiveCommandTab;
            Debug.Print("当前标签是" + activeTab);
            var activeTabIndex = swModelDocExt.ActiveCommandTabIndex;
            Debug.Print("当前标签页序号是 " + activeTabIndex);
         
            var retval = swModelDocExt.get_CommandTabVisible(5);
            Debug.Print("序号为5的标签页是否显示  " + retval);

            //设置Sketch（草图）为当前标签页
            swModelDocExt.ActiveCommandTab = "Sketch";
            activeTabIndex = swModelDocExt.ActiveCommandTabIndex;
            //让标签序号为5的激活可见
            swModelDocExt.set_CommandTabVisible(5, true);




        }

        private void btnUndoOrRedo_Click(object sender, EventArgs e)
        {

            var swApp = PStandAlone.GetSolidWorks();
            var swModelDoc = (ModelDoc2)swApp.ActiveDoc;
            var swModelDocExt = swModelDoc.Extension;


            //开始记录可以退回的步骤
            swModelDocExt.StartRecordingUndoObject();      

            Dimension dimension = (Dimension)swModelDoc.Parameter("D1@Extrude2");
            dimension.SetValue3(0.21, 1,null) ; 
          
            dimension = (Dimension)swModelDoc.Parameter("D4@Sketch1");
            dimension.SetValue3(0.56, 1, null);   
            
            swModelDoc.EditRebuild3();

            //记录结束 ，是否显示在退回列表中。
            swModelDocExt.FinishRecordingUndoObject2("修改长和宽", false);

            //撤销
            swModelDoc.EditUndo2(1);

            //重新执行
            swModelDoc.EditRedo2(1);


        }

        /// <summary>
        /// 修改表格的图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeLayer_Click(object sender, EventArgs e)
        {
            //这里需要自己打开一个工程图。 并存在名称为Red的图层 (代码新建在之前的章节里有写)

            var swApp = PStandAlone.GetSolidWorks();
            var swModelDoc = (ModelDoc2)swApp.ActiveDoc;

            DrawingDoc drawingDoc = (DrawingDoc)swModelDoc;
            Sheet drwSheet = (Sheet)drawingDoc.GetCurrentSheet();

            var myRevisionTable = drwSheet.InsertRevisionTable2(true, 0, 0, (int)swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopLeft, @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\lang\English\standard revision block.sldrevtbt", (int)swRevisionTableSymbolShape_e.swRevisionTable_CircleSymbol, true);

            var tableAnn = (TableAnnotation)myRevisionTable;

            var Ann = tableAnn.GetAnnotation();

            Ann.Layer = "Red";



        }

        private void btnOpenedFiles_Click(object sender, EventArgs e)
        {
            //OpenDoc6 Method (ISldWorks)
            var swApp = PStandAlone.GetSolidWorks();

           // var swModelDoc = (ModelDoc2)swApp.ActiveDoc;

            var partOpened = swApp.GetOpenDocumentByName(@"D:\09_Study\CSharpAndSolidWorks\CSharpAndSolidWorks\TemplateModel\clamp3.sldprt");


            if (partOpened!=null)
            {
                MessageBox.Show("该文件已经被打开");
            }
            else
            {
                MessageBox.Show("该文件已经没有被打开。");
            }



        }

        private void btnGetDistance_Click(object sender, EventArgs e)
        {
            //This example shows how to get the minimum and maximum distances between face and edge entities.
            //打开一个零件，选中2个面。
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;
            bool bMin;
            long retval;
            double dist;
            double[] varParam=null;
            object varPos1;
            object varPos2;
            double[] Pos1 = new double[3];
            double[] Pos2 = new double[3];
            if (selectionMgr.GetSelectedObjectCount()==2)
            {
                var swTop1 = (Entity)selectionMgr.GetSelectedObject6(1, -1);
                var swTop2 = (Entity)selectionMgr.GetSelectedObject6(2, -1);
                 bMin = true;
                 retval = swTop1.GetDistance(swTop2, bMin, varParam, out varPos1, out varPos2, out dist);
                 Pos1 = (double[])varPos1;
                 Pos2 = (double[])varPos2;
                Debug.Print("IEquity::GetDistance return value (0 = success; -1 = failure) : " + retval);
                Debug.Print("Face1 coordinate: " + Pos1[0] + "," + Pos1[1] + "," + Pos1[2]);
                Debug.Print("Face2 coordinate: " + Pos2[0] + "," + Pos2[1] + "," + Pos2[2]);
                Debug.Print("Minimum distance between two faces = " + dist * 1000 + " mm");
                Debug.Print("");
                 Pos1 = new double[3];
                 Pos2 = new double[3];
                bMin = false;
                retval = swTop1.GetDistance(swTop2, bMin, varParam, out varPos1, out varPos2, out dist);
                Pos1 = (double[])varPos1;
                Pos2 = (double[])varPos2;
                Debug.Print("IEquity::GetDistance return value (0 = success; -1 = failure) : " + retval);
                Debug.Print("Face coordinate: " + Pos1[0] + "," + Pos1[1] + "," + Pos1[2]);
                Debug.Print("Edge coordinate: " + Pos2[0] + "," + Pos2[1] + "," + Pos2[2]);
                Debug.Print("Maximum distance between face and edge = " + dist * 1000 + " mm");
                Debug.Print("");


            }
            else
            {
                MessageBox.Show("请只选择面或者边，且总数量2个。");
            }



        }
    }


    [System.Runtime.InteropServices.ComVisible(true)]
    public class calloutHandler : SwCalloutHandler
    {
        #region ISwCalloutHandler Members
        bool ISwCalloutHandler.OnStringValueChanged(object pManipulator, int RowID, string Text)
        {
            Debug.Print("Text: " + Text);
            Debug.Print("Row: " + RowID.ToString());
            return true;
        }
        #endregion
    }

    public class PictureDispConverter : System.Windows.Forms.AxHost
    {
        public PictureDispConverter()
            : base("56174C86-1546-4778-8EE6-B6AC606875E7")
        {
        }

        public static System.Drawing.Image Convert(object objIDispImage)
        {
            System.Drawing.Image objPicture = default(System.Drawing.Image);
            objPicture = (System.Drawing.Image)System.Windows.Forms.AxHost.GetPictureFromIPicture(objIDispImage);
            return objPicture;
        }
    }
}