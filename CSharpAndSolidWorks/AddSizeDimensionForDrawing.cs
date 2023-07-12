using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;

namespace CSharpAndSolidWorks
{
    /// <summary>
    /// 给工程图标注长宽
    /// </summary>
    internal class AddSizeDimensionForDrawing
    {
        public SldWorks swApp { get; set; }
        public ModelDoc2 swModel { get; set; }
        public SelectData swSelData { get; set; }
        public string strError { get; set; }

        //public string Options = "Two";

        /// <summary>
        /// 尺寸标注上方
        /// </summary>
        public bool DimOnTop = true;

        /// <summary>
        /// 尺寸标注左侧
        /// </summary>
        public bool DimOnLeft = true;

        /// <summary>
        /// 尺寸位置偏移量
        /// </summary>
        public double HorOffset = 0.005;

        /// <summary>
        /// 尺寸位置偏移量
        /// </summary>
        public double VerOffset = 0.005;

        #region Holes相关

        /// <summary>
        /// 孔边集合
        /// </summary>
        private List<Edge> swHoleList = new List<Edge>();

        /// <summary>
        /// 边集合
        /// </summary>
        private List<Edge> swEdgesList = new List<Edge>();

        /// <summary>
        /// 点集合
        /// </summary>
        private List<Vertex> swPointList = new List<Vertex>();

        #endregion Holes相关

        public AddSizeDimensionForDrawing(SldWorks App, ModelDoc2 model)
        {
            swApp = App;
            swModel = model;
        }

        private double AllowAutoArrange = 0;

        /// <summary>
        /// 自动增加孔尺寸
        /// </summary>
        /// <param name="s"></param>
        public void AutoAddHoleDimesnions(string s)
        {
            var swDraw = swModel as DrawingDoc;

            if (swDraw != null)
            {
                HorOffset = 0;
                VerOffset = 0;

                var swModelDocExt = (ModelDocExtension)swModel.Extension;
                var swSelMgr = (SelectionMgr)swModel.SelectionManager;

                swModel.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swViewDisplayHideAllTypes, true);

                //当前图纸
                Sheet swSheet = (Sheet)swDraw.GetCurrentSheet();
                //所有视图
                var swViews = (object[])swSheet.GetViews();

                swModel.ClearSelection2(true);
                //循环
                for (int i = 0; i < swViews.Length; i++)
                {
                    View swView = (View)swViews[i];

                    if (swView.GetOrientationName() != "*Isometric" && swView.GetOrientationName() != "*Trimetric" && swView.GetOrientationName() != "*Dimetric")
                    {
                        var vBounds = (double[])swView.GetOutline();

                        var swViewType = System.IO.Path.GetExtension(swView.GetReferencedModelName());

                        var vComps = (object[])swView.GetVisibleComponents();

                        if (swView.GetVisibleComponentCount() > 0)
                        {
                            var vEdges = (object[])swView.GetVisibleEntities((Component2)vComps[0], (int)swViewEntityType_e.swViewEntityType_Edge);
                            swSelData = (SelectData)swModel.ISelectionManager.CreateSelectData();
                            swSelData.View = swView;
                            MathTransform swViewXform = ViewMathTransform(swDraw, swView);
                            for (int itr = 0; itr < vEdges.Length; itr++)
                            {
                                var swCurve = (Curve)((Edge)vEdges[itr]).GetCurve();
                                if (swCurve.IsCircle())
                                {
                                    var swCircleParam = swCurve.GetEndParams(out double StartP, out double EndP, out bool IsClosedP, out bool IsPeriodicP);

                                    if (IsClosedP)
                                    {
                                        swHoleList.Add((Edge)vEdges[itr]);
                                    }
                                }
                                else
                                {
                                    swEdgesList.Add((Edge)vEdges[itr]);
                                    var startVertex = ((Edge)vEdges[itr]).GetStartVertex();

                                    swPointList.Add((Vertex)startVertex);
                                }
                            }

                            Entity swEntXmax = null, swEntXmin = null, swEntYmax = null, swEntYmin = null;

                            var refModel = swView.ReferencedDocument;

                            if (swViewType.ToString().ToLower() == ".sldprt")
                            {
                                if (swPointList.Count > 0)
                                {
                                    swEntXmax = FindExtremun(0, "max", swPointList, refModel.GetType(), swViewXform);
                                    swEntXmin = FindExtremun(0, "min", swPointList, refModel.GetType(), swViewXform);
                                    swEntYmax = FindExtremun(1, "max", swPointList, refModel.GetType(), swViewXform);
                                    swEntYmin = FindExtremun(1, "min", swPointList, refModel.GetType(), swViewXform);
                                }

                                if (swHoleList.Count > 0)
                                {
                                    AllowAutoArrange = 0;
                                    HorOffset = HorOffset + 0.01;
                                    VerOffset = VerOffset + 0.001;
                                    var swHcollSortX = SortHoles(0, swHoleList, swViewType, "CleanYes", swViewXform);//            ' 0 = X ,1 = Y
                                    PlaceHoleDimension((Vertex)swEntXmin, swHcollSortX, "Horizontal", vBounds, swViewType, swViewXform);
                                    var swHcollSortY = SortHoles(1, swHoleList, swViewType, "CleanYes", swViewXform);      //     ' 0 = X ,1 = Y
                                    PlaceHoleDimension((Vertex)swEntYmax, swHcollSortY, "Vertical", vBounds, swViewType, swViewXform);
                                    PlaceHoleLocationDimension(swHoleList, vBounds, swViewType, swViewXform);
                                }

                                if (AllowAutoArrange > 0 && AllowAutoArrange < 0.005)
                                {
                                    var swViewAnnot = (object[])swView.GetAnnotations();
                                    Annotation annotation;
                                    foreach (var item in swViewAnnot)
                                    {
                                        annotation = (Annotation)item;

                                        annotation.Select3(true, null);
                                    }

                                    swModelDocExt.AlignDimensions((int)swAlignDimensionType_e.swAlignDimensionType_AutoArrange, 0.001);
                                }
                            }
                        }
                    }
                }
            }
            swModel.ClearSelection2(true);
            swModel.GraphicsRedraw2();
        }

        /// <summary>
        /// 标注孔位置尺寸
        /// </summary>
        /// <param name="swHcoll"></param>
        /// <param name="vBounds"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        private void PlaceHoleLocationDimension(List<Edge> swHcoll, double[] vBounds, string swViewType, MathTransform swViewXform)
        {
            try
            {
                var swColl = new List<Edge>();
                var swTempColl = new List<Edge>();
                swColl.AddRange(swHcoll);

                for (int n = swColl.Count - 1; n >= 0; n--)
                {
                    swTempColl.Clear();

                    (swColl[n] as Entity).Select4(false, swSelData);
                    var swCircleCurve = (Curve)swColl[n].GetCurve();
                    var swCircleParams = (double[])swCircleCurve.CircleParams;
                    var swTempRadius = swCircleParams[6];
                    swTempColl.Add(swColl[n]);
                    swColl.Remove(swColl[n]);

                    for (int j = swColl.Count - 1; j >= 0; j--)
                    {
                        swCircleCurve = (Curve)swColl[j].GetCurve();
                        (swHcoll[j] as Entity).Select4(false, swSelData);

                        swCircleParams = (double[])swCircleCurve.CircleParams;

                        if (Math.Abs(swTempRadius - swCircleParams[6]) < 0.001)
                        {
                            swTempColl.Add(swColl[j]);
                            swColl.Remove(swColl[j]);
                        }
                    }

                    swTempColl = SortHoles(1, swTempColl, swViewType, "CleanNo", swViewXform);
                    swTempColl = SortHoles(0, swTempColl, swViewType, "CleanNo", swViewXform);

                    swCircleParams = CircleCoordinates(swTempColl[0], swViewType, swViewXform);

                    var Xpos = swCircleParams[0] - 0.025;
                    var Ypos = vBounds[3];
                    (swTempColl[0] as Entity).Select4(false, swSelData);

                    var myDisplayDim = (DisplayDimension)swModel.AddDimension2(Xpos, Ypos, 0);
                    if (swTempColl.Count > 2)
                    {
                        myDisplayDim.SetBrokenLeader2(false, (int)swDisplayDimensionLeaderText_e.swBrokenLeaderHorizontalText);
                        var oldText = myDisplayDim.GetText((int)swDimensionTextParts_e.swDimensionTextPrefix);
                        myDisplayDim.SetText((int)swDimensionTextParts_e.swDimensionTextPrefix, $"{swTempColl.Count}x{oldText}");
                    }
                    n = swColl.Count;
                }
            }
            catch (Exception ex)
            {
                swApp.SendMsgToUser(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 标注孔尺寸
        /// </summary>
        /// <param name="swVertex"></param>
        /// <param name="swHcoll"></param>
        /// <param name="DimOrientation"></param>
        /// <param name="vBounds"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        private void PlaceHoleDimension(Vertex swVertex, List<Edge> swHcoll, string DimOrientation, double[] vBounds, string swViewType, MathTransform swViewXform)
        {
            try
            {
                swModel.ClearSelection2(true);

                var vPt = PointCoordinates(swApp, swVertex, swViewType, swViewXform);
                var vPt2 = CircleCoordinates(swHcoll[0], swViewType, swViewXform);
                double Xpos, Ypos;

                (swVertex as Entity).Select4(true, swSelData);

                //double dblConvFactor = GetUnitConvFactor("");

                if (DimOrientation == "Horizontal")
                {
                    (swHcoll[0] as Entity).Select4(true, swSelData);
                    vPt2 = CircleCoordinates(swHcoll[0], swViewType, swViewXform);
                    Xpos = (vPt[0] + vPt2[0]) / 2;

                    Ypos = (vBounds[3] + HorOffset);

                    if (AllowAutoArrange == 0 || AllowAutoArrange > (Math.Abs(vPt[0] - vPt2[0])))
                    {
                        AllowAutoArrange = Math.Abs(vPt[0] - vPt2[0]);
                    }

                    var myDisplayDim = swModel.AddHorizontalDimension2(Xpos, Ypos, 0);
                }
                if (DimOrientation == "Vertical")
                {
                    (swHcoll[0] as Entity).Select4(true, swSelData);

                    vPt2 = CircleCoordinates(swHcoll[0], swViewType, swViewXform);
                    Xpos = (vBounds[0] - VerOffset);

                    Ypos = (vPt[1] + vPt2[1]) / 2; ;

                    if (AllowAutoArrange == 0 || AllowAutoArrange > (Math.Abs(vPt[1] - vPt2[1])))
                    {
                        AllowAutoArrange = Math.Abs(vPt[1] - vPt2[1]);
                    }

                    var myDisplayDim = swModel.AddVerticalDimension2(Xpos, Ypos, 0);
                }
                swModel.ClearSelection2(true);

                for (int i = 0; i < swHcoll.Count - 1; i++)
                {
                    swModel.ClearSelection2(true);
                    (swHcoll[i + 1] as Entity).Select4(true, swSelData);
                    var vPt1 = CircleCoordinates(swHcoll[i], swViewType, swViewXform);
                    vPt2 = CircleCoordinates(swHcoll[i + 1], swViewType, swViewXform);

                    if (DimOrientation == "Horizontal")
                    {
                        if (Math.Round(vPt1[1], 10) != Math.Round(vPt2[1], 10))
                        {
                            (swVertex as Entity).Select4(true, swSelData);
                            HorOffset = HorOffset + 0.005;
                            Xpos = (vPt[0] + vPt2[0]) / 2;
                            Ypos = (vBounds[3] + HorOffset);
                        }
                        else
                        {
                            (swHcoll[i] as Entity).Select4(true, swSelData);

                            Xpos = (vPt1[0] + vPt2[0]) / 2;
                            Ypos = (vBounds[3] + HorOffset);

                            if (AllowAutoArrange == 0 || AllowAutoArrange > (Math.Abs(vPt[0] - vPt2[0])))
                            {
                                AllowAutoArrange = Math.Abs(vPt[0] - vPt2[0]);
                            }
                        }
                        var myDisplayDim = swModel.AddHorizontalDimension2(Xpos, Ypos, 0);
                    }
                    else if (DimOrientation == "Vertical")
                    {
                        (swHcoll[i] as Entity).Select4(true, swSelData);

                        //vPt2 = CircleCoordinates(swHcoll[0], swViewType, swViewXform);

                        Xpos = (vBounds[0] - VerOffset);

                        Ypos = (vPt[1] + vPt2[1]) / 2; ;

                        if (AllowAutoArrange == 0 || AllowAutoArrange > (Math.Abs(vPt[1] - vPt2[1])))
                        {
                            AllowAutoArrange = Math.Abs(vPt[1] - vPt2[1]);
                        }

                        var myDisplayDim = swModel.AddVerticalDimension2(Xpos, Ypos, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                swApp.SendMsgToUser(ex.StackTrace.ToString());
                throw;
            }
        }

        /// <summary>
        /// 排序孔
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="swHcoll"></param>
        /// <param name="swViewType"></param>
        /// <param name="swNeedToClean"></param>
        /// <param name="swViewXform"></param>
        /// <returns></returns>
        private List<Edge> SortHoles(int axis, List<Edge> swHcoll, string swViewType, string swNeedToClean, MathTransform swViewXform)
        {
            try
            {
                var swColl = new List<Edge>();
                swColl.AddRange(swHcoll);

                for (int i = 0; i < swColl.Count; i++)
                {
                    for (int j = 0; j < swColl.Count - i - 1; j++)
                    {
                        double[] swCircleParams1 = CircleCoordinates(swColl[j], swViewType, swViewXform);
                        double[] swCircleParams2 = CircleCoordinates(swColl[j + 1], swViewType, swViewXform);
                        if (axis == 0)
                        {
                            if (swCircleParams1[axis] > swCircleParams2[axis])
                            {
                                var vTemp = swColl[j + 1];
                                swColl.Remove(swColl[j + 1]);
                                swColl.Insert(j, vTemp);
                            }
                        }
                        else if (axis == 1)
                        {
                            if (swCircleParams1[axis] < swCircleParams2[axis])
                            {
                                var vTemp = swColl[j + 1];
                                swColl.Remove(swColl[j + 1]);
                                swColl.Insert(j, vTemp);
                            }
                        }
                    }
                }

                if (swNeedToClean == "CleanYes")
                {
                    return CleanSortedHoles(axis, swColl, swViewType, swViewXform);
                }
                else if (swNeedToClean == "CleanNo")
                {
                    return swColl;
                }

                return null;
            }
            catch (Exception ex)
            {
                swApp.SendMsgToUser(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 清理排序过的孔，比如X值 一样， Y值 一样的
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="swColl"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        /// <returns></returns>
        private List<Edge> CleanSortedHoles(int axis, List<Edge> swColl, string swViewType, MathTransform swViewXform)
        {
            try
            {
                for (int k = 0; k < swColl.Count - 2; k++)
                {
                    for (int n = swColl.Count - 1; n >= 1; n--)
                    {
                        double[] swCircleParams1 = CircleCoordinates(swColl[n], swViewType, swViewXform);
                        double[] swCircleParams2 = CircleCoordinates(swColl[n - 1], swViewType, swViewXform);

                        if (Math.Abs(swCircleParams1[axis] - swCircleParams2[axis]) < 0.000001)
                        {
                            if (axis == 0)
                            {
                                if (swCircleParams1[Math.Abs(axis - 1)] > swCircleParams2[Math.Abs(axis - 1)])
                                {
                                    swColl.Remove(swColl[n - 1]);
                                }
                                else
                                {
                                    swColl.Remove(swColl[n]);
                                }
                            }
                            else if (axis == 1)
                            {
                                if (swCircleParams1[Math.Abs(axis - 1)] < swCircleParams2[Math.Abs(axis - 1)])
                                {
                                    swColl.Remove(swColl[n - 1]);
                                }
                                else
                                {
                                    swColl.Remove(swColl[n]);
                                }
                            }
                        }
                    }
                }

                return swColl;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 圆点坐标
        /// </summary>
        /// <param name="swCircle"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        /// <returns></returns>
        private double[] CircleCoordinates(Edge swCircle, string swViewType, MathTransform swViewXform)
        {
            Curve swCircleCurve = (Curve)swCircle.GetCurve();
            var swCircleParams = (double[])swCircleCurve.CircleParams;
            var vPt = new double[3];
            vPt[0] = swCircleParams[0];
            vPt[1] = swCircleParams[1];
            vPt[2] = swCircleParams[2];

            var swMathUtils = (MathUtility)swApp.GetMathUtility();
            var swMathPt = (MathPoint)swMathUtils.CreatePoint(vPt);
            var swComp = (Component2)(swCircle as Entity).GetComponent();

            if (swViewType.ToUpper() == ".SLDASM")
            {
                var swCompXform = swComp.Transform2;
                var swTotalXForm = swCompXform.Multiply(swViewXform);
                swMathPt = (MathPoint)swMathPt.MultiplyTransform(swTotalXForm);
            }
            else
            {
                swMathPt = (MathPoint)swMathPt.MultiplyTransform(swViewXform);
            }
            return (double[])swMathPt.ArrayData;
        }

        /// <summary>
        /// 增加长宽尺寸
        /// </summary>
        /// <param name="Top">尺寸位置放左?</param>
        /// <param name="Left">尺寸位置放右?</param>
        public void AutoAddSize(bool Top = true, bool Left = true)
        {
            try
            {
                DimOnLeft = Left;
                DimOnTop = Top;
                AutoOveralDimensions();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 标注
        /// </summary>
        public void AutoOveralDimensions()
        {
            var swDraw = swModel as DrawingDoc;

            if (swDraw != null)
            {
                //当前图纸
                Sheet swSheet = (Sheet)swDraw.GetCurrentSheet();
                //所有视图
                var swViews = (object[])swSheet.GetViews();

                //循环
                for (int i = 0; i < swViews.Length; i++)
                {
                    View swView = (View)swViews[i];
                    //可以排除关联视图
                    if (swView.Type == (int)swDrawingViewTypes_e.swDrawingNamedView)
                    {
                        if (swView.GetOrientationName() != "*Isometric" && swView.GetOrientationName() != "*Trimetric" && swView.GetOrientationName() != "*Dimetric")
                        {
                            ProcessView(swDraw, swView, true, true);

                            //找关联视图
                            var depHorView = GetAlignedDependantView(swView, 0);

                            if (depHorView != null)
                            {
                                ProcessView(swDraw, depHorView, true, false);
                            }
                            var depVerView = GetAlignedDependantView(swView, 1);

                            if (depVerView != null)
                            {
                                ProcessView(swDraw, depVerView, false, true);
                            }
                        }
                    }
                }
            }
            swModel.ClearSelection2(true);
            swModel.GraphicsRedraw2();
        }

        /// <summary>
        /// 找关联视图
        /// </summary>
        /// <param name="swParentView"></param>
        /// <param name="intOrientation">方向，横向/纵向</param>
        /// <returns></returns>
        private View GetAlignedDependantView(View swParentView, int intOrientation)
        {
            var delta = 0.00001;

            var intDependantViews = swParentView.GetDependentViewCount(false, (int)swDrawingViewTypes_e.swDrawingProjectedView);
            var objDependantViews = (object[])swParentView.GetDependentViews(false, (int)swDrawingViewTypes_e.swDrawingProjectedView);

            var ParentPos = (double[])swParentView.Position;

            for (int i = 0; i < intDependantViews; i++)
            {
                var depView = (View)objDependantViews[i];

                var DependantPos = (double[])depView.Position;

                if (Math.Abs(ParentPos[Math.Abs(intOrientation - 1)] - DependantPos[Math.Abs(intOrientation - 1)]) < delta)
                {
                    return depView;
                }
            }

            return null;
        }

        /// <summary>
        /// 遍历 点 ，计算再标注
        /// </summary>
        /// <param name="swDraw"></param>
        /// <param name="swView"></param>
        /// <param name="PlaceHorDim"></param>
        /// <param name="PlaceVerDim"></param>
        private void ProcessView(DrawingDoc swDraw, View swView, bool PlaceHorDim, bool PlaceVerDim)
        {
            List<Vertex> swPcoll = new List<Vertex>();

            swModel.ClearSelection2(true);

            var refModel = swView.ReferencedDocument;

            var vComps = (object[])swView.GetVisibleComponents();

            MathTransform swViewXform = ViewMathTransform(swDraw, swView);

            //获取所有直线边点
            for (int j = 0; j < swView.GetVisibleComponentCount(); j++)
            {
                var vEdges = (object[])swView.GetVisibleEntities((Component2)vComps[j], (int)swViewEntityType_e.swViewEntityType_Edge);
                swSelData = (SelectData)swModel.ISelectionManager.CreateSelectData();
                for (int itr = 0; itr < vEdges.Length; itr++)
                {
                    var swCurve = (Curve)((Edge)vEdges[itr]).GetCurve();
                    if (!swCurve.IsCircle())
                    {
                        var startVertex = (Vertex)((Edge)vEdges[itr]).GetStartVertex();
                        var endVertex = (Vertex)((Edge)vEdges[itr]).GetEndVertex();

                        if (startVertex != null)
                        {
                            swPcoll.Add(startVertex);
                        }
                        if (endVertex != null)
                        {
                            swPcoll.Add(endVertex);
                        }
                    }
                }
            }

            Entity swEntXmax, swEntXmin, swEntYmax, swEntYmin;

            if (swPcoll.Count > 0)
            {
                swEntXmax = FindExtremun(0, "max", swPcoll, refModel.GetType(), swViewXform);
                swEntXmin = FindExtremun(0, "min", swPcoll, refModel.GetType(), swViewXform);
                swEntYmax = FindExtremun(1, "max", swPcoll, refModel.GetType(), swViewXform);
                swEntYmin = FindExtremun(1, "min", swPcoll, refModel.GetType(), swViewXform);

                var vBounds = (double[])swView.GetOutline();

                if (PlaceHorDim)
                {
                    PlaceOverallDimension(swModel, swEntXmax, swEntXmin, "Horizontal", vBounds);
                }

                if (PlaceVerDim)
                {
                    PlaceOverallDimension(swModel, swEntYmax, swEntYmin, "Vertical", vBounds);
                }
            }
        }

        /// <summary>
        /// 放置尺寸
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="swVertex1"></param>
        /// <param name="swVertex2"></param>
        /// <param name="DimOrientation"></param>
        /// <param name="vBounds"></param>
        private void PlaceOverallDimension(ModelDoc2 swModel, Entity swVertex1, Entity swVertex2, string DimOrientation, double[] vBounds)
        {
            double dblConvFactor = GetUnitConvFactor("");
            if (DimOrientation == "Horizontal")
            {
                HorOffset = HorOffset * dblConvFactor;
            }
            if (DimOrientation == "Vertical")
            {
                VerOffset = VerOffset * dblConvFactor;
            }
            swModel.ClearSelection2(true);

            swVertex1.Select4(false, swSelData);
            swVertex2.Select4(true, swSelData);
            double Xpos, Ypos;

            if (DimOrientation == "Horizontal")
            {
                Xpos = (vBounds[0] + vBounds[2]) / 2;

                if (DimOnTop) //标注在顶部
                {
                    Ypos = (vBounds[3] + HorOffset);
                }
                else
                {
                    Ypos = (vBounds[1] - HorOffset);
                }

                var myDisplayDim = swModel.AddHorizontalDimension2(Xpos, Ypos, 0);
            }
            else if (DimOrientation == "Vertical")
            {
                Ypos = (vBounds[1] + vBounds[3]) / 2;

                if (DimOnLeft)
                {
                    Xpos = (vBounds[0] - HorOffset);
                }
                else
                {
                    Xpos = (vBounds[2] + HorOffset);
                }

                var myDisplayDim = swModel.AddVerticalDimension2(Xpos, Ypos, 0);
            }
        }

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private double GetUnitConvFactor(string v)
        {
            var LenUnit = swModel.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swUnitsLinear);
            switch (LenUnit)
            {
                case 0: return 0.001;
                case 1: return 0.01;
                case 2: return 1;
                case 3: return 0.0254;
                case 4: return 0.3048;
                case 5: return -1;
                case 6: return 0.0000000001;
                case 7: return 0.000000001;
                case 8: return 0.000001;
                case 9: return 0.00254;
                case 10: return 0.00000254;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 找极点
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="MinMax"></param>
        /// <param name="swPcoll"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        /// <returns></returns>
        private Entity FindExtremun(int axis, string MinMax, List<Vertex> swPcoll, int swViewType, MathTransform swViewXform)
        {
            var swVertex = swPcoll[0];
            var vPt = PointCoordinates(swVertex, swViewType, swViewXform);
            var Extr = vPt[axis];

            for (int i = 0; i < swPcoll.Count; i++)
            {
                vPt = PointCoordinates(swPcoll[i], swViewType, swViewXform);

                if (MinMax == "max" && vPt[axis] > Extr)
                {
                    Extr = vPt[axis];
                    swVertex = swPcoll[i];
                }
                else if (MinMax == "min" && vPt[axis] < Extr)
                {
                    Extr = vPt[axis];
                    swVertex = swPcoll[i];
                }
            }
            return swVertex as Entity;
        }

        /// <summary>
        /// 坐标系转换
        /// </summary>
        /// <param name="swVertex"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        /// <returns></returns>
        private double[] PointCoordinates(Vertex swVertex, int swViewType, MathTransform swViewXform)
        {
            var swMathUtils = swApp.IGetMathUtility();
            var vPt = (double[])swVertex.GetPoint();
            var swMathPt = (MathPoint)swMathUtils.CreatePoint(vPt);

            if (swViewType == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                var swComp = (Component2)(swVertex as Entity).GetComponent();

                if (swComp != null)
                {
                    MathTransform swCompXform = swComp.Transform2;

                    MathTransform swTotalXForm = (MathTransform)swCompXform.Multiply(swViewXform);

                    swMathPt = (MathPoint)swMathPt.MultiplyTransform(swTotalXForm);
                }
            }
            else
            {
                swMathPt = (MathPoint)swMathPt.MultiplyTransform(swViewXform);
            }

            return (double[])swMathPt.ArrayData;
        }

        /// <summary>
        /// 坐标系转换
        /// </summary>
        /// <param name="swVertex"></param>
        /// <param name="swViewType"></param>
        /// <param name="swViewXform"></param>
        /// <returns></returns>
        private double[] PointCoordinates(SldWorks swApp, Vertex swVertex, string swViewType, MathTransform swViewXform)
        {
            var swMathUtils = swApp.IGetMathUtility();
            var vPt = (double[])swVertex.GetPoint();
            var swMathPt = (MathPoint)swMathUtils.CreatePoint(vPt);

            if (swViewType == "SLDASM")
            {
                var swComp = (Component2)(swVertex as Entity).GetComponent();

                if (swComp != null)
                {
                    MathTransform swCompXform = swComp.Transform2;

                    MathTransform swTotalXForm = (MathTransform)swCompXform.Multiply(swViewXform);

                    swMathPt = (MathPoint)swMathPt.MultiplyTransform(swTotalXForm);
                }
            }
            else
            {
                swMathPt = (MathPoint)swMathPt.MultiplyTransform(swViewXform);
            }

            return (double[])swMathPt.ArrayData;
        }

        /// <summary>
        /// 视图转换
        /// </summary>
        /// <param name="swDraw"></param>
        /// <param name="swView"></param>
        /// <returns></returns>
        private MathTransform ViewMathTransform(DrawingDoc swDraw, View swView)
        {
            try
            {
                var swViewXform = (MathTransform)swView.ModelToViewTransform;
                var swSheet = (Sheet)swDraw.GetCurrentSheet();
                var swSheetView = (View)swDraw.GetFirstView();
                var swSheetXform = (MathTransform)swSheetView.ModelToViewTransform;

                var vSheetPrps = (double[])swSheet.GetProperties();
                var scaleNom = vSheetPrps[2];
                var scaleDenom = vSheetPrps[3];
                var ViewMatrix = (Double[])swSheetXform.ArrayData;
                ViewMatrix[12] = 1; //'scaleNom / scaleDenom
                var swMathUtil = (MathUtility)swApp.GetMathUtility();
                swSheetXform = (MathTransform)swMathUtil.CreateTransform(ViewMatrix);
                return (MathTransform)swViewXform.Multiply(swSheetXform.Inverse());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}