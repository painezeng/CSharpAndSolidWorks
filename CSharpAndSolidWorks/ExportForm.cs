using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace CSharpAndSolidWorks
{
    public partial class ExportForm : Form
    {
        public ISldWorks iswApp = null;

        public List<BodyModel> bodyModels = new List<BodyModel>();

        private List<BomItem> bomItems = new List<BomItem>();

        public ExportForm(ISldWorks IswApp)
        {
            InitializeComponent();

            this.iswApp = IswApp;
        }

        private void ListBodies()
        {
            ModelDoc2 swModel = null;
            PartDoc swPart = null;
            object vBody;
            bool bRet;

            swModel = (ModelDoc2)iswApp.ActiveDoc;
            swModel.ClearSelection2(true);
            Debug.Print("File = " + swModel.GetPathName());

            txtPath.Text = swModel.GetPathName();

            this.Text = System.IO.Path.GetFileName(txtPath.Text);

            switch (swModel.GetType())
            {
                case (int)swDocumentTypes_e.swDocPART:
                    swPart = (PartDoc)swModel;
                    // Solid bodies
                    object[] vBodyArr = null;
                    Body2 swBody = default(Body2);

                    MathTransform swMathTrans = null;
                    vBodyArr = (object[])swPart.GetBodies2((int)swBodyType_e.swSolidBody, true);

                    if ((vBodyArr != null))
                    {
                        // Debug.Print("  Number of solid bodies: " + vBodyArr.Length);

                        foreach (object vBody_loopVariable in vBodyArr)
                        {
                            vBody = vBody_loopVariable;
                            swBody = (Body2)vBody;

                            string[] vConfigName = null;
                            vConfigName = (string[])swModel.GetConfigurationNames();
                            string sMatDB = "";
                            string sMatName = swBody.GetMaterialPropertyName("", out sMatDB);

                            //bRet = swBody.RemoveMaterialProperty((int)swInConfigurationOpts_e.swAllConfiguration, (vConfigName));

                            // Debug.Print("Body--> " + swBody.Name + " " + "");

                            FeatureType Ftype = 0;

                            var childFeature = swBody.GetFeatures();

                            foreach (var item in childFeature)
                            {
                                Feature f = (Feature)item;

                                Debug.Print(swBody.Name + "-->" + f.GetTypeName());

                                if (f.GetTypeName() == "MoveCopyBody")
                                {
                                    Ftype = FeatureType.Copy;
                                }
                                if (f.GetTypeName() == "MirrorSolid")
                                {
                                    Ftype = FeatureType.Mirror;
                                }
                            }

                            Body2 swOriBody = null;

                            string swOriBodyName = "";
                            string swOriBodyBox = "";

                            if (Ftype != 0)
                            {
                                try
                                {
                                    swOriBody = swBody.GetOriginalPatternedBody(out swMathTrans);

                                    swOriBodyName = swOriBody.Name;

                                    swOriBodyBox = GetBodyBox(swOriBody);
                                }
                                catch (Exception)
                                {
                                }
                            }

                            string bbox = GetBodyBox(swBody);

                            BodyModel tempBodyM = new BodyModel(swBody.Name, sMatName, swOriBodyName, Ftype, bbox);

                            if (bbox == swOriBodyBox && swBody.Name.ToString().Contains(swOriBodyName.ToString()) == false)
                            {
                                if ((int)tempBodyM.featureT == 0)
                                {
                                    listBodies_Normally.Items.Add(tempBodyM.name);
                                }
                                else if ((int)tempBodyM.featureT == 1 && tempBodyM.name.Contains("镜向") == true) //mirror
                                {
                                    listBodies_MirrorCopy.Items.Add(tempBodyM.name + "<--M--" + tempBodyM.refBodyname);
                                    tempBodyM.comment = "镜向-" + tempBodyM.refBodyname;
                                    //tempBodyM.name = "镜像-" + tempBodyM.refBodyname + "-" ;
                                }
                                else if ((int)tempBodyM.featureT == 2 && (tempBodyM.name.Contains("复制") == true || tempBodyM.name.Contains("阵列") == true))  //copy
                                {
                                    listBodies_MirrorCopy.Items.Add(tempBodyM.name + "<--C--" + tempBodyM.refBodyname);

                                    tempBodyM.comment = "复制-" + tempBodyM.refBodyname;
                                }
                            }
                            else
                            {
                                listBodies_Normally.Items.Add(tempBodyM.name);
                            }

                            bodyModels.Add(tempBodyM);
                        }
                    }
                    break;

                case (int)swDocumentTypes_e.swDocASSEMBLY:
                    //ProcessAssembly(swApp, swModel);
                    break;

                default:
                    return;
                    break;
            }

            Debug.Print(bodyModels.Count.ToString());
        }

        private string GetBodyBox(Body2 body2)
        {
            double[] BoxFeatureDblArray = new double[7];
            BoxFeatureDblArray = (double[])body2.GetBodyBox();

            double L = 0;
            double W = 0;
            double H = 0;

            L = (BoxFeatureDblArray[3] - BoxFeatureDblArray[0]) * 1000;
            W = (BoxFeatureDblArray[4] - BoxFeatureDblArray[1]) * 1000;
            H = (BoxFeatureDblArray[5] - BoxFeatureDblArray[2]) * 1000;

            string tempbox = string.Join("x", Math.Round(L, 2), Math.Round(W, 2), Math.Round(H, 2));

            return tempbox;
        }

        private void buttonGetBodies_Click(object sender, EventArgs e)
        {
            GetBodies();
        }

        private void GetBodies()
        {
            bodyModels.Clear();
            listBodies_Normally.Items.Clear();
            listBodies_MirrorCopy.Items.Clear();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ModelDoc2 swModel = null;

            swModel = (ModelDoc2)iswApp.ActiveDoc;

            ListBodies();

            stopwatch.Stop();

            labStatus.Text = "预加载实体完成";
        }

        private void butExport_Click(object sender, EventArgs e)
        {
            ExportBodies();

            iswApp.SendMsgToUser("操作完成");

            labStatus.Text = "完成";
        }

        private void ExportBodies()
        {
            ModelDoc2 swModel = null;
            swModel = (ModelDoc2)iswApp.ActiveDoc;
            int longstatus;
            int longWarnings;

            PartDoc swPart = (PartDoc)swModel;

            string assembName = System.IO.Path.GetFileNameWithoutExtension(txtPath.Text);

            string bodyFolder = string.Join("", System.IO.Path.GetDirectoryName(txtPath.Text), "\\", assembName);

            if (!System.IO.Directory.Exists(bodyFolder))
            {
                System.IO.Directory.CreateDirectory(bodyFolder);
            }

            string partDefaultPath = iswApp.GetDocumentTemplate(1, "", 0, 0, 0);

            List<string> exportPartsName = new List<string>();

            ProgressBar.Maximum = 2 * listBodies_Normally.Items.Count;

            for (int i = 0; i < listBodies_Normally.Items.Count; i++)
            {
                swModel.ClearSelection2(true);

                string ActionBodyName = listBodies_Normally.Items[i].ToString();

                Boolean boolstatus = swModel.Extension.SelectByID2(ActionBodyName, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);

                if (boolstatus == true)
                {
                    string bodyPath = "";

                    if (ActionBodyName.Contains("复制-"))
                    {
                        bodyPath = string.Join("", bodyFolder, "\\", "复制-", assembName + "-" + ActionBodyName.Replace("复制-", "").Replace("/", ""), ".sldprt");
                    }
                    else if (ActionBodyName.Contains("镜向-"))
                    {
                        bodyPath = string.Join("", bodyFolder, "\\", "镜向-", assembName + "-" + ActionBodyName.Replace("镜向-", "").Replace("/", ""), ".sldprt");
                    }
                    else
                    {
                        bodyPath = string.Join("", bodyFolder, "\\", assembName + "-" + ActionBodyName.Replace("/", ""), ".sldprt");
                    }

                    labStatus.Text = "正在导出-->" + ActionBodyName;

                    ProgressBar.Value = i;

                    Boolean b = swPart.SaveToFile3(bodyPath, 1, 1, false, partDefaultPath, out longstatus, out longWarnings);

                    if (b)
                    {
                        //BoxSize thisBoxSize = new BoxSize();
                        //GetBoundingBox(out thisBoxSize);
                        GetBoundingBox();
                        exportPartsName.Add(bodyPath);
                    }
                }
            }

            //把所有输出的零件 ，按默认原点装配成一个装配体。

            //string assemblyDefaultPath = iswApp.GetUserPreferenceStringValue(9); //swDefaultTemplatePart 8 swDefaultTemplateAssembly9 swDefaultTemplateDrawing

            //Debug.Print "Draw template = " & swApp.GetDocumentTemplate(swDocDRAWING, "", swDwgPaperAsize, 0#, 0#)
            //Debug.Print "Part template = " & swApp.GetDocumentTemplate(swDocPART, "", 0, 0#, 0#)
            //Debug.Print "Assy template = " & swApp.GetDocumentTemplate(swDocASSEMBLY, "", 0, 0#, 0#)

            string assemblyFullpath = System.IO.Path.GetDirectoryName(txtPath.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(txtPath.Text) + ".sldasm";

            CreateNewAssembly(assemblyFullpath, exportPartsName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="partsname"></param>
        private void CreateNewAssembly(string assemblyName, List<string> partsname)
        {
            string assemblyDefaultPath = iswApp.GetDocumentTemplate(2, "", 0, 0, 0);

            var part = iswApp.NewDocument(assemblyDefaultPath, 0, 0, 0);

            if (part != null)
            {
                AssemblyDoc assemblyDoc = part as AssemblyDoc;

                ModelDoc2 modelDoc2 = assemblyDoc as ModelDoc2;

                ModelDocExtension swModExt = default(ModelDocExtension);

                int errors = 0;
                int warnings = 0;

                swModExt = (ModelDocExtension)modelDoc2.Extension;

                swModExt.SaveAs(assemblyName,
                    (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, errors, warnings);

                modelDoc2 = iswApp.ActiveDoc;

                int i = 0;
                int tempV = ProgressBar.Value;
                foreach (var partN in partsname)
                {
                    labStatus.Text = "正在装配-->" + Path.GetFileNameWithoutExtension(partN);

                    ProgressBar.Value = tempV + i;

                    iswApp.OpenDoc6(partN.ToString(), 1, 32, "", ref errors, ref warnings);

                    assemblyDoc = (AssemblyDoc)iswApp.ActivateDoc3(System.IO.Path.GetFileNameWithoutExtension(assemblyName) + ".sldasm", true, 0, errors);

                    Component2 swInsertedComponent = default(Component2);

                    swInsertedComponent = assemblyDoc.AddComponent5(partN, 0, "", false, "", 0, 0, 0);

                    modelDoc2 = iswApp.ActiveDoc;
                    modelDoc2.ClearSelection2(true);

                    modelDoc2.Extension.SelectByID2(swInsertedComponent.GetSelectByIDString(), "COMPONENT", 0, 0, 0, false, 0, null, 0);

                    assemblyDoc.UnfixComponent();

                    modelDoc2.ClearSelection2(true);

                    modelDoc2.Extension.SelectByID2("Point1@Origin@" + swInsertedComponent.GetSelectByIDString(), "EXTSKETCHPOINT", 0, 0, 0, false, 0, null, 0);

                    modelDoc2.Extension.SelectByID2("Point1@Origin", "EXTSKETCHPOINT", 0, 0, 0, true, 0, null, 0);

                    Mate2 mate2 = default(Mate2);

                    mate2 = assemblyDoc.AddMate5(20, -1, false, 0, 0, 0, 0, 0.001, 0, 0, 0, false, false, 0, out int warings);
                    modelDoc2.ClearSelection2(true);
                    modelDoc2.EditRebuild3();

                    iswApp.CloseDoc(partN);

                    i = i + 1;
                }

                iswApp.ActivateDoc3(System.IO.Path.GetFileNameWithoutExtension(assemblyName) + ".sldasm", true, 0, errors);
                modelDoc2 = iswApp.ActiveDoc;
                modelDoc2.ShowNamedView2("*等轴测", 7);
                modelDoc2.ViewZoomtofit2();
                modelDoc2.Save();
            }

            ProgressBar.Value = ProgressBar.Maximum;
        }

        public void GetBoundingBox()
        {
            BoxSize newboxSize = new BoxSize();
            BoxSize boxSize = new BoxSize();
            ModelDoc2 swModel = default(ModelDoc2);

            double L = 0;
            double W = 0;
            double H = 0;

            double[] BoxFeatureDblArray = new double[7];

            double[] BoxFaceDblArray = new double[7];
            SketchManager swSketchMgr = default(SketchManager);
            SketchPoint[] swSketchPt = new SketchPoint[9];
            SketchSegment[] swSketchSeg = new SketchSegment[13];

            swModel = (ModelDoc2)iswApp.IActiveDoc2;

            PartDoc swPart = (PartDoc)swModel;

            BoxFeatureDblArray = (double[])swPart.GetPartBox(true);

            Debug.Print("  Pt1 = " + "(" + BoxFeatureDblArray[0] * 1000.0 + ", " + BoxFeatureDblArray[1] * 1000.0 + ", " + BoxFeatureDblArray[2] * 1000.0 + ") mm");
            Debug.Print("  Pt2 = " + "(" + BoxFeatureDblArray[3] * 1000.0 + ", " + BoxFeatureDblArray[4] * 1000.0 + ", " + BoxFeatureDblArray[5] * 1000.0 + ") mm");
            L = (BoxFeatureDblArray[3] - BoxFeatureDblArray[0]) * 1000;
            W = (BoxFeatureDblArray[4] - BoxFeatureDblArray[1]) * 1000;
            H = (BoxFeatureDblArray[5] - BoxFeatureDblArray[2]) * 1000;

            List<double> myList = new List<double> { L, W, H };

            myList.Sort();

            newboxSize.Length = myList[2];
            newboxSize.Weigth = myList[1];
            newboxSize.Height = myList[0];

            swModel.Insert3DSketch2(true);
            swModel.SetAddToDB(true);
            swModel.SetDisplayWhenAdded(false);

            swSketchMgr = (SketchManager)swModel.SketchManager;
            // Draw points at each corner of bounding box
            swSketchPt[0] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[1], BoxFeatureDblArray[5]);
            swSketchPt[1] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[1], BoxFeatureDblArray[5]);
            swSketchPt[2] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[1], BoxFeatureDblArray[2]);
            swSketchPt[3] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[1], BoxFeatureDblArray[2]);
            swSketchPt[4] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[4], BoxFeatureDblArray[5]);
            swSketchPt[5] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[4], BoxFeatureDblArray[5]);
            swSketchPt[6] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[4], BoxFeatureDblArray[2]);
            swSketchPt[7] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[4], BoxFeatureDblArray[2]);
            // Now draw bounding box
            swSketchSeg[0] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[0].X, swSketchPt[0].Y, swSketchPt[0].Z, swSketchPt[1].X, swSketchPt[1].Y, swSketchPt[1].Z);
            swSketchSeg[1] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[1].X, swSketchPt[1].Y, swSketchPt[1].Z, swSketchPt[2].X, swSketchPt[2].Y, swSketchPt[2].Z);
            swSketchSeg[2] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[2].X, swSketchPt[2].Y, swSketchPt[2].Z, swSketchPt[3].X, swSketchPt[3].Y, swSketchPt[3].Z);
            swSketchSeg[3] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[3].X, swSketchPt[3].Y, swSketchPt[3].Z, swSketchPt[0].X, swSketchPt[0].Y, swSketchPt[0].Z);
            swSketchSeg[4] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[0].X, swSketchPt[0].Y, swSketchPt[0].Z, swSketchPt[4].X, swSketchPt[4].Y, swSketchPt[4].Z);
            swSketchSeg[5] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[1].X, swSketchPt[1].Y, swSketchPt[1].Z, swSketchPt[5].X, swSketchPt[5].Y, swSketchPt[5].Z);
            swSketchSeg[6] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[2].X, swSketchPt[2].Y, swSketchPt[2].Z, swSketchPt[6].X, swSketchPt[6].Y, swSketchPt[6].Z);
            swSketchSeg[7] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[3].X, swSketchPt[3].Y, swSketchPt[3].Z, swSketchPt[7].X, swSketchPt[7].Y, swSketchPt[7].Z);
            swSketchSeg[8] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[4].X, swSketchPt[4].Y, swSketchPt[4].Z, swSketchPt[5].X, swSketchPt[5].Y, swSketchPt[5].Z);
            swSketchSeg[9] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[5].X, swSketchPt[5].Y, swSketchPt[5].Z, swSketchPt[6].X, swSketchPt[6].Y, swSketchPt[6].Z);
            swSketchSeg[10] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[6].X, swSketchPt[6].Y, swSketchPt[6].Z, swSketchPt[7].X, swSketchPt[7].Y, swSketchPt[7].Z);
            swSketchSeg[11] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[7].X, swSketchPt[7].Y, swSketchPt[7].Z, swSketchPt[4].X, swSketchPt[4].Y, swSketchPt[4].Z);

            swModel.SetDisplayWhenAdded(true);
            swModel.SetAddToDB(false);

            //string actionSketchname = swModel.SketchManager.ActiveSketch.Name;

            swModel.Insert3DSketch2(true);

            swModel.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Size");
            swModel.ClearSelection2(true);

            swModel.Extension.SelectByID2("Size", "SKETCH", 0, 0, 0, false, 0, null, 0);

            swModel.BlankSketch();

            swModel.ClearSelection2(true);

            boxSize.Length = Math.Round(newboxSize.Length, 1);
            boxSize.Weigth = Math.Round(newboxSize.Weigth, 1);
            boxSize.Height = Math.Round(newboxSize.Height, 1);
            //  swModel.Extension.CustomPropertyManager[""].Delete("尺寸");

            string proText = string.Join("x", boxSize.Length, boxSize.Weigth, boxSize.Height);

            string proTextStorck = string.Join("x", boxSize.Length + 10, boxSize.Weigth + 2, boxSize.Height + 1);

            swModel.Extension.CustomPropertyManager[""].Add3("零件尺寸", 30, proText, 1);
            swModel.Extension.CustomPropertyManager[""].Add3("下料尺寸", 30, proTextStorck, 1);
            swModel.Extension.CustomPropertyManager[""].Add3("下料方式", 30, "", 1);

            // swModel.AddCustomInfo("尺寸","Text", boxSize.Length.ToString());

            swModel.Save();
            iswApp.CloseDoc(swModel.GetPathName());
        }

        public void GetBoundingBox(out BoxSize boxSize)
        {
            BoxSize newboxSize = new BoxSize();

            ModelDoc2 swModel = default(ModelDoc2);

            double L = 0;
            double W = 0;
            double H = 0;

            double[] BoxFeatureDblArray = new double[7];

            SketchManager swSketchMgr = default(SketchManager);
            SketchPoint[] swSketchPt = new SketchPoint[9];
            SketchSegment[] swSketchSeg = new SketchSegment[13];

            swModel = (ModelDoc2)iswApp.IActiveDoc2;

            PartDoc swPart = (PartDoc)swModel;

            double[] BoxFaceDblArray = new double[7];
            BoxFeatureDblArray = (double[])swPart.GetPartBox(true);

            Debug.Print("  Pt1 = " + "(" + BoxFeatureDblArray[0] * 1000.0 + ", " + BoxFeatureDblArray[1] * 1000.0 + ", " + BoxFeatureDblArray[2] * 1000.0 + ") mm");
            Debug.Print("  Pt2 = " + "(" + BoxFeatureDblArray[3] * 1000.0 + ", " + BoxFeatureDblArray[4] * 1000.0 + ", " + BoxFeatureDblArray[5] * 1000.0 + ") mm");
            L = (BoxFeatureDblArray[3] - BoxFeatureDblArray[0]) * 1000;
            W = (BoxFeatureDblArray[4] - BoxFeatureDblArray[1]) * 1000;
            H = (BoxFeatureDblArray[5] - BoxFeatureDblArray[2]) * 1000;

            List<double> myList = new List<double> { L, W, H };

            myList.Sort();

            newboxSize.Length = myList[2];
            newboxSize.Weigth = myList[1];
            newboxSize.Height = myList[0];

            swModel.Insert3DSketch2(true);
            swModel.SetAddToDB(true);
            swModel.SetDisplayWhenAdded(false);

            swSketchMgr = (SketchManager)swModel.SketchManager;
            // Draw points at each corner of bounding box
            swSketchPt[0] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[1], BoxFeatureDblArray[5]);
            swSketchPt[1] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[1], BoxFeatureDblArray[5]);
            swSketchPt[2] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[1], BoxFeatureDblArray[2]);
            swSketchPt[3] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[1], BoxFeatureDblArray[2]);
            swSketchPt[4] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[4], BoxFeatureDblArray[5]);
            swSketchPt[5] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[4], BoxFeatureDblArray[5]);
            swSketchPt[6] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[0], BoxFeatureDblArray[4], BoxFeatureDblArray[2]);
            swSketchPt[7] = (SketchPoint)swSketchMgr.CreatePoint(BoxFeatureDblArray[3], BoxFeatureDblArray[4], BoxFeatureDblArray[2]);
            // Now draw bounding box
            swSketchSeg[0] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[0].X, swSketchPt[0].Y, swSketchPt[0].Z, swSketchPt[1].X, swSketchPt[1].Y, swSketchPt[1].Z);
            swSketchSeg[1] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[1].X, swSketchPt[1].Y, swSketchPt[1].Z, swSketchPt[2].X, swSketchPt[2].Y, swSketchPt[2].Z);
            swSketchSeg[2] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[2].X, swSketchPt[2].Y, swSketchPt[2].Z, swSketchPt[3].X, swSketchPt[3].Y, swSketchPt[3].Z);
            swSketchSeg[3] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[3].X, swSketchPt[3].Y, swSketchPt[3].Z, swSketchPt[0].X, swSketchPt[0].Y, swSketchPt[0].Z);
            swSketchSeg[4] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[0].X, swSketchPt[0].Y, swSketchPt[0].Z, swSketchPt[4].X, swSketchPt[4].Y, swSketchPt[4].Z);
            swSketchSeg[5] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[1].X, swSketchPt[1].Y, swSketchPt[1].Z, swSketchPt[5].X, swSketchPt[5].Y, swSketchPt[5].Z);
            swSketchSeg[6] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[2].X, swSketchPt[2].Y, swSketchPt[2].Z, swSketchPt[6].X, swSketchPt[6].Y, swSketchPt[6].Z);
            swSketchSeg[7] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[3].X, swSketchPt[3].Y, swSketchPt[3].Z, swSketchPt[7].X, swSketchPt[7].Y, swSketchPt[7].Z);
            swSketchSeg[8] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[4].X, swSketchPt[4].Y, swSketchPt[4].Z, swSketchPt[5].X, swSketchPt[5].Y, swSketchPt[5].Z);
            swSketchSeg[9] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[5].X, swSketchPt[5].Y, swSketchPt[5].Z, swSketchPt[6].X, swSketchPt[6].Y, swSketchPt[6].Z);
            swSketchSeg[10] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[6].X, swSketchPt[6].Y, swSketchPt[6].Z, swSketchPt[7].X, swSketchPt[7].Y, swSketchPt[7].Z);
            swSketchSeg[11] = (SketchSegment)swSketchMgr.CreateLine(swSketchPt[7].X, swSketchPt[7].Y, swSketchPt[7].Z, swSketchPt[4].X, swSketchPt[4].Y, swSketchPt[4].Z);

            swModel.SetDisplayWhenAdded(true);
            swModel.SetAddToDB(false);

            //string actionSketchname = swModel.SketchManager.ActiveSketch.Name;

            swModel.Insert3DSketch2(true);

            swModel.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Size");

            swModel.ClearSelection2(true);

            boxSize.Length = newboxSize.Length;
            boxSize.Weigth = newboxSize.Weigth;
            boxSize.Height = newboxSize.Height;
            //  swModel.Extension.CustomPropertyManager[""].Delete("尺寸");

            swModel.Extension.CustomPropertyManager[""].Add3("尺寸", 30, "aaaa", 1);

            // swModel.AddCustomInfo("尺寸","Text", boxSize.Length.ToString());

            swModel.Save();
        }

        private void butAutoRename_Click(object sender, EventArgs e)
        {
            AutoRenameBodies();
        }

        public void AutoRenameBodies()
        {
            List<BodyModel> needRenameBodies = bodyModels.FindAll(x => x.comment != "");

            ModelDoc2 swModel = null;

            swModel = (ModelDoc2)iswApp.ActiveDoc;

            PartDoc swPart = (PartDoc)swModel;

            for (int i = 0; i < needRenameBodies.Count; i++)
            {
                swModel.ClearSelection2(true);

                string actionbodyName = needRenameBodies[i].name.ToString();

                Boolean boolstatus = swModel.Extension.SelectByID2(actionbodyName, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);

                if (boolstatus == true)
                {
                    var b = swModel.SelectionManager.GetSelectedObject6(1, -1);

                    int maxQ = bodyModels.FindAll(x => x.name.Contains(needRenameBodies[i].refBodyname) == true).Count;

                    string tempbodyName = "";// needRenameBodies[i].comment + (maxQ + 1).ToString();

                    Boolean haveThisName = true;// swModel.Extension.SelectByID2(tempbodyName, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);

                    if (haveThisName)
                    {
                        while (haveThisName == true)
                        {
                            maxQ = maxQ + 1;

                            tempbodyName = needRenameBodies[i].comment + maxQ.ToString();

                            swModel.ClearSelection2(true);

                            haveThisName = swModel.Extension.SelectByID2(tempbodyName, "SOLIDBODY", 0, 0, 0, false, 0, null, 0);
                        }
                    }

                    b.Name = tempbodyName;
                    swModel.EditRebuild3();
                }
            }
        }

        private void AddText_Load(object sender, EventArgs e)
        {
            GetBodies();
        }

        private void butAutoRename_Click_1(object sender, EventArgs e)
        {
        }
    }

    public enum FeatureType
    {
        Normally = 0,
        Mirror = 1,
        Copy = 2
    }
}