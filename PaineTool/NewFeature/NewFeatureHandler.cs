using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace PaineTool.NewFeature
{
    public class NewFeatureHandler : IPropertyManagerPage2Handler8, ISwComFeature
    {
        public static ISldWorks iSwApp;
        public static SwAddin userAddin;

        public static NewFeaturePMPage newFeaturePmPage;

        public bool isModify = false;

        private static MacroFeatureData featureData = null;

        public NewFeatureHandler(SwAddin addin)
        {
            userAddin = addin;
            iSwApp = (ISldWorks)userAddin.SwApp;
        }

        public NewFeatureHandler()
        {
        }

        public NewFeatureHandler(SwAddin addin, NewFeaturePMPage npage)
        {
            userAddin = addin;
            iSwApp = (ISldWorks)userAddin.SwApp;
            newFeaturePmPage = npage;
        }

        public void AfterActivation()
        {
            //throw new NotImplementedException();
        }

        public void OnClose(int Reason)
        {
            //throw new NotImplementedException();

            if (isModify == true && featureData != null)
            {
                featureData.ReleaseSelectionAccess();
            }
        }

        public void AfterClose()
        {
            //throw new NotImplementedException();
            AddMacroFeature();
        }

        public bool OnHelp()
        {
            return true;
        }

        public bool OnPreviousPage()
        {
            return true;
        }

        public bool OnNextPage()
        {
            return true;
        }

        public bool OnPreview()
        {
            return true;
        }

        public void OnWhatsNew()
        {
            //throw new NotImplementedException();
        }

        public void OnUndo()
        {
            //throw new NotImplementedException();
        }

        public void OnRedo()
        {
            //throw new NotImplementedException();
        }

        public bool OnTabClicked(int Id)
        {
            return true;
        }

        public void OnGroupExpand(int Id, bool Expanded)
        {
            //throw new NotImplementedException();
        }

        public void OnGroupCheck(int Id, bool Checked)
        {
            //throw new NotImplementedException();
        }

        public void OnCheckboxCheck(int Id, bool Checked)
        {
            //throw new NotImplementedException();
        }

        public void OnOptionCheck(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnButtonPress(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnTextboxChanged(int Id, string Text)
        {
            //throw new NotImplementedException();
        }

        public void OnNumberboxChanged(int Id, double Value)
        {
            //throw new NotImplementedException();

            if (Id == 9 && newFeaturePmPage != null)
            {
                newFeaturePmPage.numberSize.Value = Value;
            }
        }

        public void OnComboboxEditChanged(int Id, string Text)
        {
            //throw new NotImplementedException();
        }

        public void OnComboboxSelectionChanged(int Id, int Item)
        {
            //throw new NotImplementedException();
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
            //throw new NotImplementedException();
        }

        public void OnSelectionboxFocusChanged(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnSelectionboxListChanged(int Id, int Count)
        {
            //throw new NotImplementedException();
        }

        public void OnSelectionboxCalloutCreated(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnSelectionboxCalloutDestroyed(int Id)
        {
            //throw new NotImplementedException();
        }

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            return true;
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            return -1;
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {
            //throw new NotImplementedException();
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
            //throw new NotImplementedException();
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return true;
        }

        public void OnPopupMenuItem(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
            //throw new NotImplementedException();
        }

        public void OnGainedFocus(int Id)
        {
            //throw new NotImplementedException();
        }

        public void OnLostFocus(int Id)
        {
            //throw new NotImplementedException();
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return 0;
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
            //throw new NotImplementedException();
        }

        public void OnNumberBoxTrackingCompleted(int Id, double Value)
        {
            //throw new NotImplementedException();
        }

        #region AddNewFeature

        private Boolean AddMacroFeature()
        {
            IModelDoc2 moddoc;
            IFeatureManager FeatMgr;
            IFeature MacroFeature;
            Object paramNames;
            Object paramTypes;
            Object paramValues;
            string[] TparamNames = new string[3];
            int[] TparamTypes = new int[3]; //Use int for 64 bit compatibility
            string[] TparamValues = new string[3];
            IBody2 editBody;
            int opts;

            moddoc = (IModelDoc2)iSwApp.ActiveDoc;
            FeatMgr = (IFeatureManager)moddoc.FeatureManager;
            ISelectionMgr swSelMgr = (ISelectionMgr)moddoc.SelectionManager;

            if (swSelMgr.GetSelectedObjectCount() != 1)
            {
                iSwApp.SendMsgToUser("Please select one face or plane");

                return false;
            }

            var selFace = swSelMgr.GetSelectedObject6(1, 1);

            Feature[] swFeature = AddTMPBasePlaneFromSelectFace();

            //Include only data that won't be available from geometry
            TparamNames[0] = "Size";

            TparamTypes[0] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;

            TparamValues[0] = newFeaturePmPage.numberSize.Value.ToString();
            //TparamNames[0] = "Width";
            //TparamNames[1] = "Offset";
            //TparamNames[2] = "Depth";

            //TparamTypes[0] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
            //TparamTypes[1] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
            //TparamTypes[2] = (int)swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;

            ////Hard code the parameters for test,
            ////but in practice get this from Property Manager Page
            //TparamValues[0] = "0.01"; //Width
            //TparamValues[1] = "0.005"; //Offset
            //TparamValues[2] = "0.006"; //Depth

            paramNames = TparamNames;
            paramTypes = TparamTypes;
            paramValues = TparamValues;

            string[] icos = new string[9];
            icos[0] = @"FeatureIcon.bmp";
            icos[1] = @"FeatureIcon.bmp";
            icos[2] = @"FeatureIcon.bmp";
            icos[3] = @"FeatureIcon.bmp";
            icos[4] = @"FeatureIcon.bmp";
            icos[5] = @"FeatureIcon.bmp";
            icos[6] = @"FeatureIcon.bmp";
            icos[7] = @"FeatureIcon.bmp";
            icos[8] = @"FeatureIcon.bmp";

            editBody = null;

            opts = 0;

            MacroFeature = FeatMgr.InsertMacroFeature3("New-Feature", "PaineTool.NewFeature.NewFeatureHandler", null, (paramNames), (paramTypes), (paramValues), null, null, editBody, icos, opts);

            //var featureDif = (MacroFeatureData)MacroFeature.GetDefinition();

            // featureDif.GetSelectedObjects(Filter);

            //var selObj = new DispatchWrapper[] { selFace }; ;
            //var selObjMark = new int[] { 0 };
            //var views = new IView[] { null };

            //featureDif.SetSelections2(selObj, selObjMark, views);

            //featureDif.SetSelections(selFace, 1);

            foreach (var item in swFeature)
            {
                // MessageBox.Show(item.Name);

                MacroFeature.MakeSubFeature(item);
            }

            // featureDif.ReleaseSelectionAccess();

            TparamNames = null;
            TparamTypes = null;
            TparamValues = null;

            return true;
        }

        public Feature[] AddTMPBasePlaneFromSelectFace()
        {
            Feature[] tempFeatures = new Feature[2];
            //连接到Solidworks

            ModelDoc2 swModel = (ModelDoc2)iSwApp.ActiveDoc;

            //设定标注尺寸时 不输入值
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);

            // Get the pre-selected planar face
            Face2 swSelFace = default(Face2);
            SelectionMgr swSelMgr = (SelectionMgr)swModel.SelectionManager;

            //获取选择数据
            SelectData swSelData = default(SelectData);
            swSelData = swSelMgr.CreateSelectData();

            swSelFace = (Face2)swSelMgr.GetSelectedObject6(1, 1);

            //var ps = swSelFace.GetTessTriStrips(false);

            //获取屏幕鼠标选择的那个点
            var mousePoint = (double[])swSelMgr.GetSelectionPoint2(1, 1);

            List<Edge> thisFaceEdgeList = GetSelectFaceEdgeList(swSelFace);

            swModel.ViewZoomtofit2();

            Entity swSelFaceEntity = (Entity)swSelFace;

            string skFirstName = "";

            tempFeatures[0] = DrawnSketchOnFace(swModel, mousePoint, thisFaceEdgeList, swSelFaceEntity, out skFirstName);

            var t = (double[])swSelFace.Normal;

            swModel.ClearSelection2(true);

            swSelFaceEntity.Select4(false, swSelData);

            return tempFeatures;
        }

        private static List<Edge> GetSelectFaceEdgeList(Face2 swSelFace)
        {
            Object[] eages = (Object[])swSelFace.GetEdges();

            List<Edge> thisFaceEdgeList = new List<Edge>();

            foreach (var item in eages)
            {
                thisFaceEdgeList.Add((Edge)item);

                var tempE = (Edge)item;
                var tStart = (Vertex)tempE.GetStartVertex();
                var tEnd = (Vertex)tempE.GetEndVertex();
                var pstart = (double[])tStart.GetPoint();
                var pend = (double[])tEnd.GetPoint();
                var s = pstart[0] + "," + pstart[1] + "--------" + pend[0] + "," + pend[1];
            }

            return thisFaceEdgeList;
        }

        private Feature DrawnSketchOnFace(ModelDoc2 swModel, double[] mousePoint, List<Edge> thisFaceEdgeList, Entity swSelFaceEntity, out string sketchName)
        {
            swModel.SketchManager.InsertSketch(true);

            //通过两条相邻边来标注 创建点的位置

            //1。 计算鼠标选中点到面的各边的距离。

            List<FaceEdgeInfos> faceEdgeInfor = new List<FaceEdgeInfos>();

            //Dictionary<Edge, double> pointToEdgeDim = new Dictionary<Edge, double>();

            var selMousePt = (double[])TransformPoint(swModel.IGetActiveSketch2(), mousePoint[0], mousePoint[1], mousePoint[2]);

            foreach (var item in thisFaceEdgeList)
            {
                var tStart = (Vertex)item.GetStartVertex();
                var pstart = (double[])tStart.GetPoint();

                var tEnd = (Vertex)item.GetEndVertex();
                var pend = (double[])tEnd.GetPoint();

                pstart = (double[])TransformPoint(swModel.IGetActiveSketch2(), pstart[0], pstart[1], pstart[2]);
                pend = (double[])TransformPoint(swModel.IGetActiveSketch2(), pend[0], pend[1], pend[2]);

                var angleOfLine = Math.Atan2((pend[1] - pstart[1]), (pend[0] - pstart[0])) * 180 / Math.PI;

                double tempChuizuX = 0;
                double tempChuizuY = 0;

                //垂足的坐标位置
                var tempchuizu = Geometry2D.GetPerpendicular(selMousePt[0], selMousePt[1], pend[0], pend[1], pstart[0], pstart[1]);

                tempChuizuX = tempchuizu[0];
                tempChuizuY = tempchuizu[1];

                faceEdgeInfor.Add(new FaceEdgeInfos(item, Geometry2D.pointToLine(pstart[0], pstart[1], pend[0], pend[1], selMousePt[0], selMousePt[1]), angleOfLine, Geometry2D.lineSpace(pstart[0], pstart[1], pend[0], pend[1]), tempChuizuX, tempChuizuY));

                //pointToEdgeDim.Add(item, pointToLine(pstart[0], pstart[1], pend[0], pend[1], mousePoint[0], mousePoint[1]));
            }

            //把所有的边按距离排序
            var EdgeInfosOrderByDim = faceEdgeInfor.OrderBy(x => x.dim).ToList();

            //最近的一条边
            var minEdgeObject = EdgeInfosOrderByDim[0];

            //下一条相临的边
            FaceEdgeInfos nextMinEdga = null;

            for (int i = 1; i < EdgeInfosOrderByDim.Count; i++)
            {
                if (Math.Abs(EdgeInfosOrderByDim[i].Angle) != 180 - Math.Abs(minEdgeObject.Angle) && EdgeInfosOrderByDim[i].Angle != minEdgeObject.Angle)
                {
                    nextMinEdga = EdgeInfosOrderByDim[i];
                    break;
                }
            }

            // var eNextEdge = faceEdgeInfoses.Find(x => ((Vertex)x.edge.GetStartVertex()).GetPoint() == ((Vertex)minEdgeObject.GetEndVertex()).GetPoint());

            var minEdge = (Entity)minEdgeObject.edge;

            var nextminEdge = (Entity)nextMinEdga.edge;
            swModel.ClearSelection();

            //MessageBox.Show("done");

            swSelFaceEntity.Select(false);

            // var actSketch = swModel.SketchManager.ActiveSketch;

            ////把鼠标点坐标转换到草图中

            //var selMousePt = (double[])TransformPoint(swModel.IGetActiveSketch2(), mousePoint[0], mousePoint[1], mousePoint[2]);

            swModel.SketchManager.CreateCenterRectangle(selMousePt[0], selMousePt[1], selMousePt[2], selMousePt[0] + 0.03, selMousePt[1] + 0.03, selMousePt[2]);

            swModel.ClearSelection2(true);

            swModel.Extension.SelectByID2("Point1", "SKETCHPOINT", 0, 0, 0, false, 0, null, 0);
            minEdge.Select(true);

            // var thisDim = (DisplayDimension)swModel.AddDimension2(selMousePt[0], selMousePt[1], selMousePt[2]);
            var thisDim = (DisplayDimension)swModel.AddDimension2(minEdgeObject.chuizuPoint_X, minEdgeObject.chuizuPoint_Y, 0);

            thisDim.GetDimension2(0).SystemValue = Math.Round(minEdgeObject.dim * 1000, 0) / 1000;

            //第二个方向尺寸
            swModel.ClearSelection2(true);

            swModel.Extension.SelectByID2("Point1", "SKETCHPOINT", 0, 0, 0, false, 0, null, 0);
            nextminEdge.Select(true);

            //var nextthisDim = (DisplayDimension)swModel.AddDimension2(selMousePt[0], selMousePt[1], selMousePt[2]);
            var nextthisDim = (DisplayDimension)swModel.AddDimension2(nextMinEdga.chuizuPoint_X, nextMinEdga.chuizuPoint_Y, 0);

            nextthisDim.GetDimension2(0).SystemValue = Math.Round(nextMinEdga.dim * 1000, 0) / 1000;

            string featName = null;
            string featType = null;

            var skFeature = (Feature)swModel.IGetActiveSketch2();
            featName = skFeature.GetNameForSelection(out featType);
            sketchName = featName;

            swModel.SketchManager.InsertSketch(true);

            swModel.Extension.SelectByID2(sketchName, "SKETCH", 0, 0, 0, false, 1, null, 0);

            var boolstatus = swModel.InsertPlanarRefSurface();

            ISelectionMgr swSelMgr = (ISelectionMgr)swModel.SelectionManager;

            skFeature = (Feature)swSelMgr.GetSelectedObject6(1, 0);// swModel.FeatureByPositionReverse(0);

            return skFeature;
        }

        ////画第二个草图,中点重合即可.

        //private Feature DrawnSketchOnFace(ModelDoc2 swModel, double[] mousePoint, string SketchName, Entity swSelFaceEntity)
        //{
        //    swModel.SketchManager.InsertSketch(true);

        //    //通过两条相邻边来标注 创建点的位置

        //    //1。 计算鼠标选中点到面的各边的距离。

        //    //Dictionary<Edge, double> pointToEdgeDim = new Dictionary<Edge, double>();

        //    var selMousePt = (double[])TransformPoint(swModel.IGetActiveSketch2(), mousePoint[0], mousePoint[1], mousePoint[2]);

        //    swModel.SketchManager.CreateCenterRectangle(selMousePt[0], selMousePt[1], selMousePt[2], selMousePt[0] + 0.03, selMousePt[1] + 0.03, selMousePt[2]);

        //    swModel.ClearSelection2(true);

        //    swModel.Extension.SelectByID2("Point1", "SKETCHPOINT", 0, 0, 0, false, 0, null, 0);

        //    swModel.Extension.SelectByID2("Point1@" + SketchName, "EXTSKETCHPOINT", 0, 0, 0, true, 0, null, 0);

        //    swModel.SketchAddConstraints("sgCOINCIDENT");//增加重合

        //    var skFeature = (Feature)swModel.IGetActiveSketch2();

        //    swModel.SketchManager.InsertSketch(true);

        //    var boolstatus = swModel.InsertPlanarRefSurface();

        //    ISelectionMgr swSelMgr = (ISelectionMgr)swModel.SelectionManager;

        //    skFeature = (Feature)swSelMgr.GetSelectedObject6(1, 0);// swModel.FeatureByPositionReverse(0);

        //    return skFeature;
        //    //skFeature = swModel.FeatureByPositionReverse(0);

        //    //return skFeature;
        //}

        //把模型中的点转换到草图中的坐标。
        public object TransformPoint(Sketch Sketch1, double X, double Y, double Z)
        {
            ISldWorks swApp = iSwApp;

            MathUtility swMathUtil;
            object @params;
            MathTransform swMathTrans;

            MathPoint swMathPt;

            double[] ptArr = new double[3];

            ptArr[0] = X;
            ptArr[1] = Y;
            ptArr[2] = Z;

            swMathUtil = (MathUtility)swApp.GetMathUtility();
            swMathPt = (MathPoint)swMathUtil.CreatePoint((ptArr));
            object NewPt;

            @params = swMathPt.ArrayData;

            swMathTrans = Sketch1.ModelToSketchTransform;
            swMathPt = (MathPoint)swMathPt.MultiplyTransform(swMathTrans);

            NewPt = swMathPt.ArrayData;

            return NewPt;
        }

        public object Edit(object app, object modelDoc, object feature)
        {
            var f = (Feature)feature;
            //MacroFeatureData featData = (MacroFeatureData)f.GetDefinition();
            featureData = (MacroFeatureData)f.GetDefinition();
            newFeaturePmPage.Show(featureData, modelDoc);

            return true;
        }

        public object Regenerate(object app, object modelDoc, object feature)
        {
            return true;
        }

        public object Security(object app, object modelDoc, object feature)
        {
            return true;
        }

        #endregion AddNewFeature
    }
}