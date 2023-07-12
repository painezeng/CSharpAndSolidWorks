using SolidWorks.Interop.sldworks;

namespace PaineTool.NewFeature
{
    public class FaceEdgeInfos
    {
        public Edge edge { get; set; }
        public double dim { get; set; }
        public double Angle { get; set; }
        public double Length { get; set; }
        public double chuizuPoint_X { get; set; }
        public double chuizuPoint_Y { get; set; }

        public FaceEdgeInfos()
        {
        }

        public FaceEdgeInfos(Edge edge, double dim, double angle, double length, double chuizu_x, double chuizu_y)
        {
            this.edge = edge;
            this.dim = dim;
            Angle = angle;
            Length = length;
            chuizuPoint_X = chuizu_x;
            chuizuPoint_Y = chuizu_y;
        }
    }
}