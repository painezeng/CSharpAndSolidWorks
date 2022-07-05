using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAndSolidWorks
{
    /// <summary>
    /// 给工程图标注长宽
    /// </summary>
    internal class AddSizeDimensionForDrawing
    {
        public SldWorks swApp { get; set; }
        public ModelDoc2 swModel  { get; set; }
        public SelectData swSelData { get; set; }
        public string  strError { get; set; }

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


        public AddSizeDimensionForDrawing(SldWorks App, ModelDoc2 model)
        {

            swApp = App;
            swModel = model;

        }

        /// <summary>
        /// 增加长宽尺寸
        /// </summary>
        /// <param name="Top">尺寸位置放左?</param>
        /// <param name="Left">尺寸位置放右?</param>
        public void AutoAddSize(bool Top = true, bool Left = true) {

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
        public void AutoOveralDimensions() {
                     

            var swDraw = swModel as DrawingDoc;

            if (swDraw != null) 
            {
                //当前图纸
                Sheet swSheet = (Sheet)swDraw.GetCurrentSheet();
                //所有视图
                var swViews = (object[])swSheet.GetViews();

                //循环
                for (int i = 0; i < swViews.Length; i++) {

                    View swView = (View)swViews[i];
                    //可以排除关联视图
                    if (swView.Type==(int)swDrawingViewTypes_e.swDrawingNamedView)
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

            var ParentPos =(double[]) swParentView.Position;

            for (int i = 0; i < intDependantViews; i++)
            {
                var depView = (View)objDependantViews[i];

                var DependantPos = (double[])depView.Position;

                if (Math.Abs(ParentPos[Math.Abs(intOrientation - 1)] - DependantPos[Math.Abs(intOrientation - 1)])<delta) {

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

            List<Vertex> swPcoll= new List<Vertex> ();

            swModel.ClearSelection2(true);

            var refModel= swView.ReferencedDocument;

            var vComps = (object[])swView.GetVisibleComponents();

            MathTransform swViewXform = ViewMathTransform(swDraw, swView);

            //获取所有直线边点
            for (int j = 0;j < swView.GetVisibleComponentCount(); j++)
            {
                var vEdges = (object[])swView.GetVisibleEntities((Component2)vComps[j], (int)swViewEntityType_e.swViewEntityType_Edge);
                  swSelData = (SelectData)swModel.ISelectionManager.CreateSelectData();
                    for (int itr = 0; itr < vEdges.Length; itr++)
                    {
                        var swCurve = (Curve)((Edge)vEdges[itr]).GetCurve();
                        if (!swCurve.IsCircle())
                        {
                            var startVertex= (Vertex)((Edge)vEdges[itr]).GetStartVertex();
                            var endVertex = (Vertex)((Edge)vEdges[itr]).GetEndVertex();

                            if (startVertex !=null)
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


            if (swPcoll.Count>0)
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
            if (DimOrientation == "Horizontal") {

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

                if(DimOnTop) //标注在顶部
                {
                    Ypos = (vBounds[3] + HorOffset);
                }
                else
                {
                    Ypos = (vBounds[1] - HorOffset);
                }

                var myDisplayDim = swModel.AddHorizontalDimension2(Xpos, Ypos, 0);


            } else if (DimOrientation == "Vertical")
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
                case 0  : return 0.001;
                case 1  : return 0.01;
                case 2  : return 1;
                case 3  : return 0.0254;
                case 4  : return 0.3048;
                case 5  : return -1;
                case 6  : return 0.0000000001;
                case 7  : return 0.000000001;
                case 8  : return 0.000001;
                case 9  : return 0.00254;
                case 10  : return 0.00000254;
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

                if (MinMax=="max" && vPt[axis]>Extr)
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

            if (swViewType==(int)swDocumentTypes_e.swDocASSEMBLY)
            {
                var swComp =(Component2) (swVertex as Entity).GetComponent();

                if (swComp!=null)
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
