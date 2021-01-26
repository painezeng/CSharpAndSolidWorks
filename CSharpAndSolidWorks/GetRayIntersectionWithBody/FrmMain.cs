using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace GetRayIntersectionWithBody
{
    public partial class FrmMain : Form
    {
        private readonly SldWorks swApp;

        private Manipulator manipulator;

        public static List<Body2> AllBodies = new List<Body2>();

        public FrmMain()
        {
            InitializeComponent();
            swApp = ConnectToSolidworks.ConnectToSolidworks.GetSolidWorks();

            if (swApp == null)
            {
                MessageBox.Show("连接Solidworks失败! 请打开solidworks后重试！");
                Close();
            }
        }

        private void btnViewRay_Click(object sender, EventArgs e)
        {
            try
            {
                manipulator?.Remove();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            double[] basePoint = new double[3];
            double[] vectorPoint = new double[3];
            try
            {
                basePoint[0] = double.Parse(txtStart_X.Text) / 1000;
                basePoint[1] = double.Parse(txtStart_Y.Text) / 1000;
                basePoint[2] = double.Parse(txtStart_Z.Text) / 1000;

                vectorPoint[0] = double.Parse(txtVertor_X.Text) / 1000;
                vectorPoint[1] = double.Parse(txtVertor_Y.Text) / 1000;
                vectorPoint[2] = double.Parse(txtVertor_Z.Text) / 1000;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("请检查坐标输入！");
                return;
            }

            var swMathUtil = (MathUtility)swApp.GetMathUtility();

            var swStartPoint = (MathPoint)swMathUtil.CreatePoint(basePoint);
            var swVector = (MathVector)swMathUtil.CreateVector(vectorPoint);

            manipulator = CreateNewDrag(swVector, swStartPoint, 0.9);
        }

        /// <summary>
        /// 创建箭头
        /// </summary>
        /// <param name="swN"></param>
        /// <param name="swPickPt"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Manipulator CreateNewDrag(MathVector swN, MathPoint swPickPt, double length)
        {
            var swModel = (ModelDoc2)swApp.ActiveDoc;
            var swModViewMgr = swModel.ModelViewManager;
            SwManHandler2 swDragHdlr = new SwManHandler2();

            Manipulator swManip = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swDragArrowManipulator, swDragHdlr);

            DragArrowManipulator swDrag = (DragArrowManipulator)swManip.GetSpecificManipulator();

            swDrag.AllowFlip = false;
            swDrag.ShowRuler = false;
            swDrag.ShowOppositeDirection = false;
            swDrag.Length = length;
            swDrag.Direction = swN;
            //swDrag.LengthOppositeDirection = 0.01;

            swDrag.Origin = swPickPt;

            swManip.Show(swModel);

            swDrag.Update();

            return swManip;
        }

        private void btnGetIntersectionPoint_Click(object sender, EventArgs e)
        {
            AllBodies.Clear();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            if (swModel == null)
            {
                MessageBox.Show("请先打开装配体");
                return;
            }

            if (swModel.GetType() != (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                MessageBox.Show("请先打开装配体");
                return;
            }

            swApp.CommandInProgress = true;
            if (swApp != null)
            {
                Configuration swConf = (Configuration)swModel.GetActiveConfiguration();

                Component2 swRootComp = (Component2)swConf.GetRootComponent();

                //遍历
                TraverseCompXform(swRootComp, 0);
            }

            //if (AllBodies.Count > 0)
            //{
            //    MessageBox.Show(AllBodies.Count.ToString());
            //}

            CreatePoints();

            swApp.CommandInProgress = false;
        }

        //通过射线创建点，并计算距离
        public double CreatePoints()
        {
            double sumDis = 0;
            IBody2[] body = AllBodies.ToArray();

            double[] basePoint = new double[3];
            double[] vectorPoint = new double[3];
            try
            {
                basePoint[0] = double.Parse(txtStart_X.Text) / 1000;
                basePoint[1] = double.Parse(txtStart_Y.Text) / 1000;
                basePoint[2] = double.Parse(txtStart_Z.Text) / 1000;

                vectorPoint[0] = double.Parse(txtVertor_X.Text) / 1000;
                vectorPoint[1] = double.Parse(txtVertor_Y.Text) / 1000;
                vectorPoint[2] = double.Parse(txtVertor_Z.Text) / 1000;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("请检查坐标输入！");
                return 0;
            }

            double[] rayVectorOrigins = basePoint; //起点

            double[] rayVectorDirections = vectorPoint; //方向

            ModelDoc2 swDoc = ((ModelDoc2)(swApp.ActiveDoc));

            int numIntersectionsFound = (int)swDoc.RayIntersections(body,

                (object)rayVectorOrigins,

                (object)rayVectorDirections,

                (int)(swRayPtsOpts_e.swRayPtsOptsTOPOLS | swRayPtsOpts_e.swRayPtsOptsNORMALS),

                (double).0000001,

                (double).0000001);

            double[] points = (double[])swDoc.GetRayIntersectionsPoints();
            swDoc.GetRayIntersectionsTopology();

            swDoc.SketchManager.Insert3DSketch(true);

            swDoc.SketchManager.AddToDB = true;

            int tempPointIndex = 1;
            double[] tempLineStartPoint = new double[3];

            for (int i = 0; i < points.Length; i += 9)

            {
                double[] pt = new double[] { points[i + 3], points[i + 4], points[i + 5] };

                swDoc.SketchManager.CreatePoint(pt[0], pt[1], pt[2]);

                if (points[2] == 1)
                {
                    Debug.Print("this point is on face");

                    if (tempPointIndex % 2 == 1)
                    {
                        tempLineStartPoint = new double[3];

                        tempLineStartPoint[0] = pt[0] * 1000.0;
                        tempLineStartPoint[1] = pt[1] * 1000.0;
                        tempLineStartPoint[2] = pt[2] * 1000.0;
                    }
                    if (tempPointIndex % 2 == 0)
                    {
                        var tempLineEndPoint = new double[3];

                        tempLineEndPoint[0] = pt[0] * 1000.0;
                        tempLineEndPoint[1] = pt[1] * 1000.0;
                        tempLineEndPoint[2] = pt[2] * 1000.0;

                        sumDis = sumDis + Vector3.Distance(new Vector3(tempLineStartPoint[0], tempLineStartPoint[1],
                                     tempLineStartPoint[2]), new Vector3(tempLineEndPoint[0], tempLineEndPoint[1],
                                     tempLineEndPoint[2]));
                    }

                    tempPointIndex++;
                }

                //if (points[2] == 4)
                //{
                //    Debug.Print("this point is on edge");
                //}

                //if (points[2] == 16)
                //{
                //    Debug.Print("this point is enter");
                //}

                //if (points[2] == 32)
                //{
                //    Debug.Print("this point is exit");
                //}

                //if (points[2] == 8)
                //{
                //    Debug.Print("this point is on vertex");
                //}

                //if (points[2] == 1)
                //{
                //    Debug.Print("this point is on face");
                //}
            }

            swDoc.SketchManager.AddToDB = false;

            swDoc.SketchManager.Insert3DSketch(true);

            return sumDis;
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

                if (swModel != null)
                {
                    Debug.Print("Loading:" + swComp.Name2);

                    if (swModel is PartDoc)
                    {
                        var partDoc = (PartDoc)swModel;
                        var vBodies = (Object[])partDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);

                        for (int index = 0; index < vBodies.Length; index++)
                        {
                            AllBodies.Add((Body2)vBodies[index]);
                        }
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            manipulator?.Remove();
        }

        private void btnGetIntersectionPointInPart_Click(object sender, EventArgs e)
        {
            //清空实体
            AllBodies.Clear();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            if (swModel == null)
            {
                MessageBox.Show("请先打开一个文件");
                return;
            }

            if (swModel.GetType() != (int)swDocumentTypes_e.swDocPART)
            {
                MessageBox.Show("请先打开零件");
                return;
            }

            swApp.CommandInProgress = true;

            var partDoc = (PartDoc)swModel;
            var vBodies = (Object[])partDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);

            for (int index = 0; index < vBodies.Length; index++)
            {
                AllBodies.Add((Body2)vBodies[index]);
            }

            //if (AllBodies.Count > 0)
            //{
            //    MessageBox.Show(AllBodies.Count.ToString());
            //}

            var sumDis = CreatePoints();

            swApp.CommandInProgress = false;

            MessageBox.Show($"{sumDis.ToString("F")}mm");
        }

        internal class Vector3
        {
            private double x;
            private double y;
            private double z;

            public Vector3(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            internal static double Distance(Vector3 startAirportPos, Vector3 endAirportPos)
            {
                double sqrt = System.Math.Sqrt(System.Math.Pow(startAirportPos.x - endAirportPos.x, 2) + System.Math.Pow(startAirportPos.y - endAirportPos.y, 2) + System.Math.Pow(startAirportPos.z - endAirportPos.z, 2));
                return System.Math.Abs(sqrt);
            }
        }
    }
}