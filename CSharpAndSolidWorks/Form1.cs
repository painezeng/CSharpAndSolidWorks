using CSharpAndSolidWorks.Properties;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using View = SolidWorks.Interop.sldworks.View;

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

        private void Btn_Traverse_Comp_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                Configuration swConf = swModel.GetActiveConfiguration();

                Component2 swRootComp = swConf.GetRootComponent();

                //遍历
                Utility.TraverseCompXform(swRootComp, 0);
            }
        }

        private void btn_Traverse_Drawing_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                DrawingDoc drawingDoc = (DrawingDoc)swModel;

                //获取当前工程图中的所有图纸名称
                var sheetNames = drawingDoc.GetSheetNames();

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();
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

                    AssemblyDoc assemblyDoc = swApp.ActivateDoc3("TempAssembly.sldasm", true, 0, errors);
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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            if (swApp != null)
            {
                ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

                if (swModel.GetType() == (int)swDocumentTypes_e.swDocPART || swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
                {
                    ModelDocExtension swModExt = (ModelDocExtension)swModel.Extension;

                    int error = 0;

                    int warnings = 0;

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 modelDoc2 = swApp.ActiveDoc;
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

            ISldWorks swApp = Utility.ConnectToSolidWorks();
            ModelDoc2 swModel = swApp.ActiveDoc;
            SelectionMgr swSelMgr = swModel.SelectionManager;

            //选择草图
            swModel.Extension.SelectByID2("Sketch2", "SKETCH", 0, 0, 0, false, 4, null, 0);

            //进入编辑草图
            swModel.EditSketch();

            //获取当前草图对象
            Sketch swSketch = swModel.GetActiveSketch2();

            //获取该草图中的所有线
            object[] vSketchSeg = swSketch.GetSketchSegments();

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();
            ModelDoc2 swModel = swApp.ActiveDoc;
            SelectionMgr swSelMgr = swModel.SelectionManager;

            #region 零件中选择

            PartDoc part1 = (PartDoc)swModel;
            //在零件中选择
            Face2 face1 = part1.GetEntityByName("SFace1", (int)swSelectType_e.swSelFACES);
            Entity entity1 = (Entity)face1;
            entity1.Select(false);

            #endregion 零件中选择

            #region 装配中选择

            //这里我们默认该零件已经是选中装配,否则我们需要遍历一次零件,仅做示例

            Component2 component = swSelMgr.GetSelectedObjectsComponent4(1, -1);

            swModel.ClearSelection();

            ModelDoc2 modelDoc = component.GetModelDoc2();

            //转换为PartDoc
            PartDoc part = (PartDoc)modelDoc;

            Face2 face = part.GetEntityByName("SFace1", (int)swSelectType_e.swSelFACES);

            Entity entity = (Entity)face;

            //在装配中再转换成装配中的实体
            Entity entityInComp = (Entity)component.GetCorrespondingEntity(entity);

            entityInComp.Select(false);

            #endregion 装配中选择
        }

        private void Btn_T_sketchsegment_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

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
            object[] vSketchSeg = (object[])swFeature.GetSpecificFeature2().GetSketchSegments();

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);

            swModel = swApp.ActiveDoc;
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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = default(ModelDoc2);
            ModelDocExtension swModelDocExt = default(ModelDocExtension);

            Frame swFrame = swApp.Frame();

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

            ISldWorks swApp = Utility.ConnectToSolidWorks();
            ModelDoc2 swModel = default(ModelDoc2);

            swModel = swApp.ActiveDoc;

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
                SelectionMgr selectionMgr = swModel.SelectionManager;
                Component2 swComp;
                //遍历已经选择的零件
                for (int j = 0; j < selectionMgr.GetSelectedObjectCount(); j++)
                {
                    swComp = selectionMgr.GetSelectedObject6(j + 1, 0);

                    swModel = swComp.GetModelDoc2();

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

            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = swApp.ActiveDoc;

            FeatureManager featureManager = swModel.FeatureManager;

            PartDoc partDoc = (PartDoc)swModel;
            //通过特征名字获取特征
            Feature feature = partDoc.FeatureByName("Bounding Box");
            int longstatus;
            if (feature == null)//特征为null时将创建Bounding Box

            {
                feature = featureManager.InsertGlobalBoundingBox((int)swGlobalBoundingBoxFitOptions_e.swBoundingBoxType_BestFit, true, false, out longstatus);
            }

            // 显示 Bounding Box sketch
            var b = swModel.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swViewDispGlobalBBox, true);

            //获取自动生成的属性值
            string str;
            string str2;
            string str3;
            string str4;
            IConfiguration configuration = swModel.GetActiveConfiguration();
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

            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = swApp.ActiveDoc;

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

            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = swApp.ActiveDoc;

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

            ISldWorks swApp = Utility.ConnectToSolidWorks();

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
                taskpaneView.DeleteView();
                Marshal.FinalReleaseComObject(taskpaneView);
                taskpaneView = null;
            }
            catch (Exception exception)
            {
            }
        }

        private void btn_SetMaterial_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = swApp.ActiveDoc;
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

            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = swApp.ActiveDoc;

            SelectionMgr selectionMgr = swModel.SelectionManager;
            try
            {
                for (int i = 1; i <= selectionMgr.GetSelectedObjectCount(); i++)
                {
                    Face2 face2 = (Face2)selectionMgr.GetSelectedObject6(i, -1);
                    var vFaceProp = swModel.MaterialPropertyValues;

                    var vProps = face2.GetMaterialPropertyValues2(1, null);
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

            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = swApp.ActiveDoc;

            ModelDocExtension swModelDocExt = (ModelDocExtension)swModel.Extension;

            SelectionMgr selectionMgr = swModel.SelectionManager;

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
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            Face2 swSelFace = default(Face2);
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //获取选择数据
            SelectData swSelData = default(SelectData);

            swSelData = swSelMgr.CreateSelectData();

            swSelFace = (Face2)swSelMgr.GetSelectedObject6(1, 0);

            var t = (double[])swSelFace.Normal;

            //获取屏幕鼠标选择的那个点
            var mousePoint = (double[])swSelMgr.GetSelectionPoint2(1, 0);

            swModel.ClearSelection2(true);

            //创建Ray选择

            var boolstatus = swModel.Extension.SelectByRay(mousePoint[0], mousePoint[1], mousePoint[2], t[0], t[1], t[2], 0.1, 2, false, 0, 0);

            if (boolstatus == true)
            {
                MessageBox.Show("选择完成!");
            }
        }

        private void GetDrawingModel_Click(object sender, EventArgs e)
        {
            //连接到Solidworks
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            // DrawingDoc dc = (DrawingDoc)swModel;

            SelectionMgr selectionMgr = (SelectionMgr)swModel.SelectionManager;

            //获取选择的视图对象
            View view = (View)selectionMgr.GetSelectedObject5(1);

            //获取视图中的引用模型
            var viewModel = view.ReferencedDocument;

            //其它读取属性请参考博文 读取零件属性 ->BtnGetPartData_Click

            MessageBox.Show(viewModel.GetPathName());
        }

        private void btn_Part_Export_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Utility.ConnectToSolidWorks();

            ExportForm exportForm = new ExportForm(swApp);

            exportForm.Show();
        }
    }
}