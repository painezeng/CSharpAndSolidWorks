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
    }
}