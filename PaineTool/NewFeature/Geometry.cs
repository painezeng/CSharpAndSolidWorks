using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaineTool.NewFeature
{
    /* 基本几何结构 */

    public class Point2D
    {
        public double X;
        public double Y;

        public Point2D(double a = 0, double b = 0)
        {
            X = a; Y = b;
        } //constructor
    }

    public class LINESEG
    {
        public Point2D s;
        public Point2D e;

        public LINESEG(Point2D a, Point2D b)
        {
            s = a; e = b;
        }

        public LINESEG()
        {
        }
    }

    // 直线的解析方程 a*x+b*y+c=0  为统一表示，约定 a >= 0
    public class LINE
    {
        public double a;
        public double b;
        public double c;

        public LINE(double d1 = 1, double d2 = -1, double d3 = 0)
        {
            a = d1; b = d2; c = d3;
        }
    }

    public class Geometry2D
    {
        /* 需要包含的头文件 */

        /* 常用的常量定义 */
        private const double INF = 1E200;
        private const double EP = 1E-10;
        private const int MAXV = 300;
        private const double PI = 3.14159265;

        /**********************
         *                    *
         *   点的基本运算     *
         *                    *
         **********************/

        /// <summary>
        /// 返回两点之间欧氏距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Dist(Point2D p1, Point2D p2)
        {
            return (Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }

        /// <summary>
        /// 判断两个点是否重合
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool Equal_Point(Point2D p1, Point2D p2)
        {
            return ((Math.Abs(p1.X - p2.X) < EP) && (Math.Abs(p1.Y - p2.Y) < EP));
        }

        /******************************************************************************
        r=Multiply(sp,ep,op),得到(sp-op)和(ep-op)的叉积
        r>0：ep在矢量opsp的逆时针方向；
        r=0：opspep三点共线；
        r<0：ep在矢量opsp的顺时针方向
        *******************************************************************************/

        /// <summary>
        /// r=Multiply(sp,ep,op),得到(sp-op)和(ep-op)的叉积
        /// r大于0：ep在矢量opsp的逆时针方向；
        /// r=0：opspep三点共线；
        /// r小于0：ep在矢量opsp的顺时针方向
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="ep"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static double Multiply(Point2D sp, Point2D ep, Point2D op)
        {
            return ((sp.X - op.X) * (ep.Y - op.Y) - (ep.X - op.X) * (sp.Y - op.Y));
        }

        /*
        r=DotMultiply(p1,p2,op),得到矢量(p1-op)和(p2-op)的点积，如果两个矢量都非零矢量
        r<0：两矢量夹角为钝角；
        r=0：两矢量夹角为直角；
        r>0：两矢量夹角为锐角
        *******************************************************************************/

        /// <summary>
        /// r=DotMultiply(p1,p2,op),得到矢量(p1-op)和(p2-op)的点积，如果两个矢量都非零矢量
        /// r小于0：两矢量夹角为钝角
        /// r=0：两矢量夹角为直角；
        /// r大于0：两矢量夹角为锐角
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p0"></param>
        /// <returns></returns>
        public static double DotMultiply(Point2D p1, Point2D p2, Point2D p0)
        {
            return ((p1.X - p0.X) * (p2.X - p0.X) + (p1.Y - p0.Y) * (p2.Y - p0.Y));
        }

        /******************************************************************************
        判断点p是否在线段l上
        条件：(p在线段l所在的直线上) && (点p在以线段l为对角线的矩形内)
        *******************************************************************************/

        /// <summary>
        /// 判断点p是否在线段l上
        /// 条件：(p在线段l所在的直线上) && (点p在以线段l为对角线的矩形内)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Online(LINESEG l, Point2D p)
        {
            return ((Multiply(l.e, p, l.s) == 0) && (((p.X - l.s.X) * (p.X - l.e.X) <= 0) && ((p.Y - l.s.Y) * (p.Y - l.e.Y) <= 0)));
        }

        /// <summary>
        /// 返回点p以点o为圆心逆时针旋转alpha(单位：弧度)后所在的位置
        /// </summary>
        /// <param name="o"></param>
        /// <param name="alpha"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point2D Rotate(Point2D o, double alpha, Point2D p)
        {
            Point2D tp = new Point2D();
            p.X -= o.X;
            p.Y -= o.Y;
            tp.X = p.X * Math.Cos(alpha) - p.Y * Math.Sin(alpha) + o.X;
            tp.Y = p.Y * Math.Cos(alpha) + p.X * Math.Sin(alpha) + o.Y;
            return tp;
        }

        /* 返回顶角在o点，起始边为os，终止边为oe的夹角(单位：弧度)
         角度小于pi，返回正值
         角度大于pi，返回负值
         可以用于求线段之间的夹角
        原理：
         r = DotMultiply(s,e,o) / (Dist(o,s)*Dist(o,e))
         r'= Multiply(s,e,o)
         r >= 1 angle = 0;
         r <= -1 angle = -PI
         -1<r<1 && r'>0 angle = arccos(r)
         -1<r<1 && r'<=0 angle = -arccos(r)
        */

        /// <summary>
        /// 返回顶角在o点，起始边为os，终止边为oe的夹角(单位：弧度)
        /// 角度小于pi，返回正值;
        /// 角度大于pi，返回负值;
        /// 可以用于求线段之间的夹角
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static double Angle(Point2D o, Point2D s, Point2D e)
        {
            double cosfi, fi, norm;
            double dsx = s.X - o.X;
            double dsy = s.Y - o.Y;
            double dex = e.X - o.X;
            double dey = e.Y - o.Y;

            cosfi = dsx * dex + dsy * dey;
            norm = (dsx * dsx + dsy * dsy) * (dex * dex + dey * dey);
            cosfi /= Math.Sqrt(norm);

            if (cosfi >= 1.0) return 0;
            if (cosfi <= -1.0) return -3.1415926;

            fi = Math.Acos(cosfi);
            if (dsx * dey - dsy * dex > 0) return fi;      // 说明矢量os 在矢量 oe的顺时针方向
            return -fi;
        }

        /*****************************\
        *                             *
        *      线段及直线的基本运算   *
        *                             *
        \*****************************/

        /* 判断点与线段的关系,用途很广泛
        本函数是根据下面的公式写的，P是点C到线段AB所在直线的垂足
                        AC dot AB
                r =     ---------
                         ||AB||^2
                     (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay)
                  = -------------------------------
                                  L^2
            r has the following meaning:
                r=0      P = A
                r=1      P = B
                r<0   P is on the backward extension of AB
          r>1      P is on the forward extension of AB
                0<r<1  P is interior to AB
        */

        public static double[] GetPerpendicular(double x, double y, double startx, double starty, double endx, double endy)
        {
            Point2D point2D = new Point2D(x, y);
            Point2D pointLineStart = new Point2D(startx, starty);
            Point2D pointLineEnd = new Point2D(endx, endy);

            LINESEG lineseg = new LINESEG(pointLineStart, pointLineEnd);

            var p = Geometry2D.Perpendicular(point2D, lineseg);

            double[] tempP = new double[2];

            tempP[0] = p.X;
            tempP[1] = p.Y;

            return tempP;
        }

        // 计算两点之间的距离
        public static double lineSpace(double x1, double y1, double x2, double y2)
        {
            double lineLength = 0;
            lineLength = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            return lineLength;
        }

        //点到线段距离
        public static double pointToLine(double x1, double y1, double x2, double y2, double px0, double py0)
        {
            double space = 0;
            double a, b, c;
            a = lineSpace(x1, y1, x2, y2);// 线段的长度
            b = lineSpace(x1, y1, px0, py0);// (x1,y1)到点的距离
            c = lineSpace(x2, y2, px0, py0);// (x2,y2)到点的距离
            if (c <= 0.000001 || b <= 0.000001)
            {
                space = 0;
                return space;
            }
            if (a <= 0.000001)
            {
                space = b;
                return space;
            }
            if (c * c >= a * a + b * b)
            {
                space = b;
                return space;
            }
            if (b * b >= a * a + c * c)
            {
                space = c;
                return space;
            }
            double p = (a + b + c) / 2;// 半周长
            double s = Math.Sqrt(p * (p - a) * (p - b) * (p - c));// 海伦公式求面积
            space = 2 * s / a;// 返回点到线的距离（利用三角形面积公式求高）
            return space;
        }

        public static double Relation(Point2D p, LINESEG l)
        {
            LINESEG tl = new LINESEG();
            tl.s = l.s;
            tl.e = p;
            return DotMultiply(tl.e, l.e, l.s) / (Dist(l.s, l.e) * Dist(l.s, l.e));
        }

        /// <summary>
        /// 求点C到线段AB所在直线的垂足 P
        /// </summary>
        /// <param name="p"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Point2D Perpendicular(Point2D p, LINESEG l)
        {
            double r = Relation(p, l);
            Point2D tp = new Point2D();
            tp.X = l.s.X + r * (l.e.X - l.s.X);
            tp.Y = l.s.Y + r * (l.e.Y - l.s.Y);
            return tp;
        }

        /// <summary>
        ///  求点p到线段l的最短距离,并返回线段上距该点最近的点np
        /// 注意：np是线段l上到点p最近的点，不一定是垂足
        /// </summary>
        /// <param name="p"></param>
        /// <param name="l"></param>
        /// <param name="np"></param>
        /// <returns></returns>
        public static double PtoLinesegDist(Point2D p, LINESEG l, ref Point2D np)
        {
            double r = Relation(p, l);
            if (r < 0)
            {
                np = l.s;
                return Dist(p, l.s);
            }
            if (r > 1)
            {
                np = l.e;
                return Dist(p, l.e);
            }
            np = Perpendicular(p, l);
            return Dist(p, np);
        }

        /// <summary>
        /// 求点p到线段l所在直线的距离,请注意本函数与上个函数的区别
        /// </summary>
        /// <param name="p"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double PtoLDist(Point2D p, LINESEG l)
        {
            return Math.Abs(Multiply(p, l.e, l.s)) / Dist(l.s, l.e);
        }

        /// <summary>
        ///  计算点到折线集的最近距离,并返回最近点.
        ///  注意：调用的是ptolineseg()函数
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="pointset"></param>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static double PtoPointSet(int vcount, Point2D[] pointset, Point2D p, ref Point2D q)
        {
            int i;
            double cd = INF, td;
            LINESEG l = new LINESEG();
            Point2D tq = new Point2D();
            Point2D cq = new Point2D();

            for (i = 0; i < vcount - 1; i++)
            {
                l.s = pointset[i];

                l.e = pointset[i + 1];
                td = PtoLinesegDist(p, l, ref tq);
                if (td < cd)
                {
                    cd = td;
                    cq = tq;
                }
            }
            q = cq;
            return cd;
        }

        /// <summary>
        /// 判断圆是否在多边形内.ptolineseg()函数的应用2
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool CircleInsidePolygon(int vcount, Point2D center, double radius, Point2D[] polygon)
        {
            Point2D q = new Point2D();
            double d;
            q.X = 0;
            q.Y = 0;
            d = PtoPointSet(vcount, polygon, center, ref q);
            if (d < radius || Math.Abs(d - radius) < EP)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 返回两个矢量l1和l2的夹角的余弦(-1 --- 1)注意：如果想从余弦求夹角的话，注意反余弦函数的定义域是从 0到pi
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static double Cosine(LINESEG l1, LINESEG l2)
        {
            return ((l1.e.X - l1.s.X) * (l2.e.X - l2.s.X) + (l1.e.Y - l1.s.Y) * (l2.e.Y - l2.s.Y)) / (Dist(l1.e, l1.s) * Dist(l2.e, l2.s));
        }

        /// <summary>
        /// 返回线段l1与l2之间的夹角 单位：弧度 范围(-pi，pi)
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static double LsAngle(LINESEG l1, LINESEG l2)
        {
            Point2D o = new Point2D();
            Point2D s = new Point2D();
            Point2D e = new Point2D();

            o.X = o.Y = 0;
            s.X = l1.e.X - l1.s.X;
            s.Y = l1.e.Y - l1.s.Y;
            e.X = l2.e.X - l2.s.X;
            e.Y = l2.e.Y - l2.s.Y;
            return Angle(o, s, e);
        }

        /// <summary>
        /// 如果线段u和v相交(包括相交在端点处)时，返回true .
        /// 判断P1P2跨立Q1Q2的依据是：( P1 - Q1 ) × ( Q2 - Q1 ) * ( Q2 - Q1 ) × ( P2 - Q1 ) >= 0。
        /// 判断Q1Q2跨立P1P2的依据是：( Q1 - P1 ) × ( P2 - P1 ) * ( P2 - P1 ) × ( Q2 - P1 ) >= 0。
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool Intersect(LINESEG u, LINESEG v)
        {
            return ((Math.Max(u.s.X, u.e.X) >= Math.Min(v.s.X, v.e.X)) &&                     //排斥实验
              (Math.Max(v.s.X, v.e.X) >= Math.Min(u.s.X, u.e.X)) &&
              (Math.Max(u.s.Y, u.e.Y) >= Math.Min(v.s.Y, v.e.Y)) &&
              (Math.Max(v.s.Y, v.e.Y) >= Math.Min(u.s.Y, u.e.Y)) &&
              (Multiply(v.s, u.e, u.s) * Multiply(u.e, v.e, u.s) >= 0) &&         //跨立实验
              (Multiply(u.s, v.e, v.s) * Multiply(v.e, u.e, v.s) >= 0));
        }

        /// <summary>
        /// (线段u和v相交)&&(交点不是双方的端点) 时返回true
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool Intersect_A(LINESEG u, LINESEG v)
        {
            return ((Intersect(u, v)) &&
              (!Online(u, v.s)) &&
              (!Online(u, v.e)) &&
              (!Online(v, u.e)) &&
              (!Online(v, u.s)));
        }

        /// <summary>
        /// 线段v所在直线与线段u相交时返回true；方法：判断线段u是否跨立线段v
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool Intersect_l(LINESEG u, LINESEG v)
        {
            return Multiply(u.s, v.e, v.s) * Multiply(v.e, u.e, v.s) >= 0;
        }

        /// <summary>
        /// 根据已知两点坐标，求过这两点的直线解析方程： a*x+b*y+c = 0  (a >= 0)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static LINE MakeLine(Point2D p1, Point2D p2)
        {
            LINE tl = new LINE();
            int sign = 1;
            tl.a = p2.Y - p1.Y;
            if (tl.a < 0)
            {
                sign = -1;
                tl.a = sign * tl.a;
            }
            tl.b = sign * (p1.X - p2.X);
            tl.c = sign * (p1.Y * p2.X - p1.X * p2.Y);
            return tl;
        }

        /// <summary>
        /// 根据直线解析方程返回直线的斜率k,水平线返回 0,竖直线返回 1e200
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Slope(LINE l)
        {
            if (Math.Abs(l.a) < 1e-20)
                return 0;
            if (Math.Abs(l.b) < 1e-20)
                return INF;
            return -(l.a / l.b);
        }

        /// <summary>
        /// 返回直线的倾斜角alpha ( 0 - pi)
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Alpha(LINE l)
        {
            if (Math.Abs(l.a) < EP)
                return 0;
            if (Math.Abs(l.b) < EP)
                return PI / 2;
            double k = Slope(l);
            if (k > 0)
                return Math.Atan(k);
            else
                return PI + Math.Atan(k);
        }

        /// <summary>
        /// 求点p关于直线l的对称点
        /// </summary>
        /// <param name="l"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point2D Symmetry(LINE l, Point2D p)
        {
            Point2D tp = new Point2D();
            tp.X = ((l.b * l.b - l.a * l.a) * p.X - 2 * l.a * l.b * p.Y - 2 * l.a * l.c) / (l.a * l.a + l.b * l.b);
            tp.Y = ((l.a * l.a - l.b * l.b) * p.Y - 2 * l.a * l.b * p.X - 2 * l.b * l.c) / (l.a * l.a + l.b * l.b);
            return tp;
        }

        /// <summary>
        /// 如果两条直线 l1(a1*x+b1*y+c1 = 0), l2(a2*x+b2*y+c2 = 0)相交，返回true，且返回交点p
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool LineIntersect(LINE l1, LINE l2, ref Point2D p) // 是 L1，L2
        {
            double d = l1.a * l2.b - l2.a * l1.b;
            if (Math.Abs(d) < EP) // 不相交
                return false;
            p.X = (l2.c * l1.b - l1.c * l2.b) / d;
            p.Y = (l2.a * l1.c - l1.a * l2.c) / d;
            return true;
        }

        /// <summary>
        /// 如果线段l1和l2相交，返回true且交点由(inter)返回，否则返回false
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="inter"></param>
        /// <returns></returns>
        public static bool Intersection(LINESEG l1, LINESEG l2, ref Point2D inter)
        {
            LINE ll1, ll2;
            ll1 = MakeLine(l1.s, l1.e);
            ll2 = MakeLine(l2.s, l2.e);
            if (LineIntersect(ll1, ll2, ref inter))
                return Online(l1, inter) && Online(l2, inter);
            else
                return false;
        }

        /******************************\
        *         *
        * 多边形常用算法模块    *
        *         *
        \******************************/

        // 如果无特别说明，输入多边形顶点要求按逆时针排列

        /*
        返回值：输入的多边形是简单多边形，返回true
        要 求：输入顶点序列按逆时针排序
        说 明：简单多边形定义：
        1：循环排序中相邻线段对的交是他们之间共有的单个点
        2：不相邻的线段不相交
        本程序默认第一个条件已经满足
        */

        /// <summary>
        /// 输入的多边形是简单多边形，返回true.
        /// 要 求：输入顶点序列按逆时针排序 .
        /// 说 明：简单多边形定义：
        /// 1：循环排序中相邻线段对的交是他们之间共有的单个点 .
        /// 2：不相邻的线段不相交 .
        /// 本程序默认第一个条件已经满足 .
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsSimple(int vcount, Point2D[] polygon)
        {
            int i, cn;
            LINESEG l1 = new LINESEG();
            LINESEG l2 = new LINESEG();
            for (i = 0; i < vcount; i++)
            {
                l1.s = polygon[i];
                l1.e = polygon[(i + 1) % vcount];
                cn = vcount - 3;
                while (cn != 0)
                {
                    l2.s = polygon[(i + 2) % vcount];
                    l2.e = polygon[(i + 3) % vcount];
                    if (Intersect(l1, l2))
                        break;
                    cn--;
                }
                if (cn != 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 凸凹性判断.
        /// 返回值：按输入顺序返回多边形顶点的凸凹性判断，bc[i]=1,iff:第i个顶点是凸顶点
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <param name="bc"></param>
        public static void CheckConvex(int vcount, Point2D[] polygon, bool[] bc)
        {
            int i, index = 0;
            Point2D tp = polygon[0];
            for (i = 1; i < vcount; i++) // 寻找第一个凸顶点
            {
                if (polygon[i].Y < tp.Y || (polygon[i].Y == tp.Y && polygon[i].X < tp.X))
                {
                    tp = polygon[i];
                    index = i;
                }
            }
            int count = vcount - 1;
            bc[index] = true;
            while (count != 0) // 判断凸凹性
            {
                if (Multiply(polygon[(index + 1) % vcount], polygon[(index + 2) % vcount], polygon[index]) >= 0)
                    bc[(index + 1) % vcount] = true;
                else
                    bc[(index + 1) % vcount] = false;
                index++;
                count--;
            }
        }

        /// <summary>
        /// 返回值：多边形polygon是凸多边形时，返回true
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsConvex(int vcount, Point2D[] polygon)
        {
            bool[] bc = new bool[MAXV];
            CheckConvex(vcount, polygon, bc);
            for (int i = 0; i < vcount; i++) // 逐一检查顶点，是否全部是凸顶点
                if (!bc[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 返回多边形面积(signed)；输入顶点按逆时针排列时，返回正值；否则返回负值
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static double Area_Of_Polygon(int vcount, Point2D[] polygon)
        {
            int i;
            double s;
            if (vcount < 3)
                return 0;
            s = polygon[0].Y * (polygon[vcount - 1].X - polygon[1].X);
            for (i = 1; i < vcount; i++)
                s += polygon[i].Y * (polygon[(i - 1)].X - polygon[(i + 1) % vcount].X);
            return s / 2;
        }

        /// <summary>
        /// 如果输入顶点按逆时针排列，返回true
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsConterClock(int vcount, Point2D[] polygon)
        {
            return Area_Of_Polygon(vcount, polygon) > 0;
        }

        /// <summary>
        /// 另一种判断多边形顶点排列方向的方法
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsCcwize(int vcount, Point2D[] polygon)
        {
            int i, index;
            Point2D a, b, v;
            v = polygon[0];
            index = 0;
            for (i = 1; i < vcount; i++) // 找到最低且最左顶点，肯定是凸顶点
            {
                if (polygon[i].Y < v.Y || polygon[i].Y == v.Y && polygon[i].X < v.X)
                {
                    index = i;
                }
            }
            a = polygon[(index - 1 + vcount) % vcount]; // 顶点v的前一顶点
            b = polygon[(index + 1) % vcount]; // 顶点v的后一顶点
            return Multiply(v, b, a) > 0;
        }

        /********************************************************************************************
        射线法判断点q与多边形polygon的位置关系，要求polygon为简单多边形，顶点逆时针排列
           如果点在多边形内：   返回0
           如果点在多边形边上： 返回1
           如果点在多边形外： 返回2
        *********************************************************************************************/

        /// <summary>
        /// 射线法判断点q与多边形polygon的位置关系，要求polygon为简单多边形，顶点逆时针排列
        /// 如果点在多边形内：返回0
        /// 如果点在多边形边上：返回1
        /// 如果点在多边形外：返回2
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="Polygon"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static int InsidePolygon(int vcount, Point2D[] Polygon, Point2D q)
        {
            int c = 0, i, n;
            LINESEG l1 = new LINESEG();
            LINESEG l2 = new LINESEG();
            bool bintersect_a, bonline1, bonline2, bonline3;
            double r1, r2;

            l1.s = q;
            l1.e = q;
            l1.e.X = INF;
            n = vcount;
            for (i = 0; i < vcount; i++)
            {
                l2.s = Polygon[i];
                l2.e = Polygon[(i + 1) % n];
                if (Online(l2, q))
                    return 1; // 如果点在边上，返回1

                bintersect_a = Intersect_A(l1, l2);// 相交且不在端点
                bonline1 = Online(l1, Polygon[(i + 1) % n]);// 第二个端点在射线上
                bonline2 = Online(l1, Polygon[(i + 2) % n]);
                bonline3 = Online(l1, Polygon[(i + 2) % n]);
                r1 = Multiply(Polygon[i], Polygon[(i + 1) % n], l1.s) * Multiply(Polygon[(i + 1) % n], Polygon[(i + 2) % n], l1.s);
                r2 = Multiply(Polygon[i], Polygon[(i + 2) % n], l1.s) * Multiply(Polygon[(i + 2) % n], Polygon[(i + 3) % n], l1.s);

                /* 前一个端点和后一个端点在射线两侧 */
                /* 下一条边是水平线，前一个端点和后一个端点在射线两侧  */
                if (bintersect_a || (bonline1 && (!bonline2 && (r1 > 0) || bonline3 && (r2 > 0))))
                {
                    c++;
                }
            }
            if (c % 2 == 1)
                return 0;
            else
                return 2;
        }

        /// <summary>
        /// 点q是凸多边形polygon内时，返回true；注意：多边形polygon一定要是凸多边形
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool InsideConvexPolygon(int vcount, Point2D[] polygon, Point2D q) // 可用于三角形！
        {
            Point2D p = new Point2D();
            LINESEG l = new LINESEG();
            int i;
            p.X = 0; p.Y = 0;
            for (i = 0; i < vcount; i++) // 寻找一个肯定在多边形polygon内的点p：多边形顶点平均值
            {
                p.X += polygon[i].X;
                p.Y += polygon[i].Y;
            }
            p.X /= vcount;
            p.Y /= vcount;

            for (i = 0; i < vcount; i++)
            {
                l.s = polygon[i]; l.e = polygon[(i + 1) % vcount];
                if (Multiply(p, l.e, l.s) * Multiply(q, l.e, l.s) < 0) /* 点p和点q在边l的两侧，说明点q肯定在多边形外 */
                    break;
            }
            return (i == vcount);
        }

        /// <summary>
        /// 寻找凸包的graham 扫描法
        /// </summary>
        /// <param name="PointSet">输入的点集</param>
        /// <param name="ch">输出的凸包上的点集，按照逆时针方向排列</param>
        /// <param name="n">为PointSet中的点的数目</param>
        /// <param name="len">为输出的凸包上的点的个数</param>
        public static void Graham_scan(Point2D[] PointSet, Point2D[] ch, int n, ref int len)
        {
            int i, j, k = 0, top = 2;
            Point2D tmp = new Point2D();
            // 选取PointSet中y坐标最小的点PointSet[k]，如果这样的点有多个，则取最左边的一个
            for (i = 1; i < n; i++)
                if (PointSet[i].Y < PointSet[k].Y || (PointSet[i].Y == PointSet[k].Y) && (PointSet[i].X < PointSet[k].X))
                    k = i;
            tmp = PointSet[0];
            PointSet[0] = PointSet[k];
            PointSet[k] = tmp; // 现在PointSet中y坐标最小的点在PointSet[0]
            for (i = 1; i < n - 1; i++) /* 对顶点按照相对PointSet[0]的极角从小到大进行排序，极角相同的按照距离PointSet[0]从近到远进行排序 */
            {
                k = i;
                for (j = i + 1; j < n; j++)
                    if (Multiply(PointSet[j], PointSet[k], PointSet[0]) > 0 ||  // 极角更小
                     (Multiply(PointSet[j], PointSet[k], PointSet[0]) == 0) && /* 极角相等，距离更短 */
                     Dist(PointSet[0], PointSet[j]) < Dist(PointSet[0], PointSet[k])
                       )
                        k = j;
                tmp = PointSet[i];
                PointSet[i] = PointSet[k];
                PointSet[k] = tmp;
            }
            ch[0] = PointSet[0];
            ch[1] = PointSet[1];
            ch[2] = PointSet[2];
            for (i = 3; i < n; i++)
            {
                while (Multiply(PointSet[i], ch[top], ch[top - 1]) >= 0)
                    top--;
                ch[++top] = PointSet[i];
            }
            len = top + 1;
        }

        /// <summary>
        /// 卷包裹法求点集凸壳，参数说明同graham算法
        /// </summary>
        /// <param name="PointSet"></param>
        /// <param name="ch"></param>
        /// <param name="n"></param>
        /// <param name="len"></param>
        public static void ConvexClosure(Point2D[] PointSet, Point2D[] ch, int n, ref int len)
        {
            int top = 0, i, index, first;
            double curmax, curcos, curdis;
            Point2D tmp;
            LINESEG l1 = new LINESEG();
            LINESEG l2 = new LINESEG();
            bool[] use = new bool[MAXV];
            tmp = PointSet[0];
            index = 0;
            // 选取y最小点，如果多于一个，则选取最左点
            for (i = 1; i < n; i++)
            {
                if (PointSet[i].Y < tmp.Y || PointSet[i].Y == tmp.Y && PointSet[i].X < tmp.X)
                {
                    index = i;
                }
                use[i] = false;
            }
            tmp = PointSet[index];
            first = index;
            use[index] = true;

            index = -1;
            ch[top++] = tmp;
            tmp.X -= 100;
            l1.s = tmp;
            l1.e = ch[0];
            l2.s = ch[0];

            while (index != first)
            {
                curmax = -100;
                curdis = 0;
                // 选取与最后一条确定边夹角最小的点，即余弦值最大者
                for (i = 0; i < n; i++)
                {
                    if (use[i]) continue;
                    l2.e = PointSet[i];
                    curcos = Cosine(l1, l2); // 根据Math.Cos值求夹角余弦，范围在 （-1 -- 1 ）
                    if (curcos > curmax || Math.Abs(curcos - curmax) < 1e-6 && Dist(l2.s, l2.e) > curdis)
                    {
                        curmax = curcos;
                        index = i;
                        curdis = Dist(l2.s, l2.e);
                    }
                }
                use[first] = false;            //清空第first个顶点标志，使最后能形成封闭的hull
                use[index] = true;
                ch[top++] = PointSet[index];
                l1.s = ch[top - 2];
                l1.e = ch[top - 1];
                l2.s = ch[top - 1];
            }
            len = top - 1;
        }

        /*********************************************************************************************
         判断线段是否在简单多边形内(注意：如果多边形是凸多边形，下面的算法可以化简)
            必要条件一：线段的两个端点都在多边形内；
         必要条件二：线段和多边形的所有边都不内交；
         用途： 1. 判断折线是否在简单多边形内
           2. 判断简单多边形是否在另一个简单多边形内
        **********************************************************************************************/

        /// <summary>
        /// 判断线段是否在简单多边形内
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static bool LinesegInsidePolygon(int vcount, Point2D[] polygon, LINESEG l)
        {
            // 判断线端l的端点是否不都在多边形内
            if (InsidePolygon(vcount, polygon, l.s) == 0 || InsidePolygon(vcount, polygon, l.e) == 0)
                return false;
            int top = 0, i, j;
            Point2D[] PointSet = new Point2D[MAXV];
            Point2D tmp = new Point2D();
            LINESEG s = new LINESEG();

            for (i = 0; i < vcount; i++)
            {
                s.s = polygon[i];
                s.e = polygon[(i + 1) % vcount];
                if (Online(s, l.s)) //线段l的起始端点在线段s上
                    PointSet[top++] = l.s;
                else if (Online(s, l.e)) //线段l的终止端点在线段s上
                    PointSet[top++] = l.e;
                else
                {
                    if (Online(l, s.s)) //线段s的起始端点在线段l上
                        PointSet[top++] = s.s;
                    else if (Online(l, s.e)) // 线段s的终止端点在线段l上
                        PointSet[top++] = s.e;
                    else
                    {
                        if (Intersect(l, s)) // 这个时候如果相交，肯定是内交，返回false
                            return false;
                    }
                }
            }

            for (i = 0; i < top - 1; i++) /* 冒泡排序，x坐标小的排在前面；x坐标相同者，y坐标小的排在前面 */
            {
                for (j = i + 1; j < top; j++)
                {
                    if (PointSet[i].X > PointSet[j].X || Math.Abs(PointSet[i].X - PointSet[j].X) < EP && PointSet[i].Y > PointSet[j].Y)
                    {
                        tmp = PointSet[i];
                        PointSet[i] = PointSet[j];
                        PointSet[j] = tmp;
                    }
                }
            }

            for (i = 0; i < top - 1; i++)
            {
                tmp.X = (PointSet[i].X + PointSet[i + 1].X) / 2; //得到两个相邻交点的中点
                tmp.Y = (PointSet[i].Y + PointSet[i + 1].Y) / 2;
                if (InsidePolygon(vcount, polygon, tmp) == 0)
                    return false;
            }
            return true;
        }

        /*********************************************************************************************
        求任意简单多边形polygon的重心
        需要调用下面几个函数：
         void AddPosPart(); 增加右边区域的面积
         void AddNegPart(); 增加左边区域的面积
         void AddRegion(); 增加区域面积
        在使用该程序时，如果把xtr,ytr,wtr,xtl,ytl,wtl设成全局变量就可以使这些函数的形式得到化简,
        但要注意函数的声明和调用要做相应变化
        **********************************************************************************************/

        /// <summary>
        /// 增加右边区域的面积
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="xtr"></param>
        /// <param name="ytr"></param>
        /// <param name="wtr"></param>
        public static void AddPosPart(double x, double y, double w, ref double xtr, ref double ytr, ref double wtr)
        {
            if (Math.Abs(wtr + w) < 1e-10) return; // detect zero regions
            xtr = (wtr * xtr + w * x) / (wtr + w);
            ytr = (wtr * ytr + w * y) / (wtr + w);
            wtr = w + wtr;
            return;
        }

        /// <summary>
        /// 增加左边区域的面积
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="xtl"></param>
        /// <param name="ytl"></param>
        /// <param name="wtl"></param>
        public static void AddNegPart(double x, double y, double w, ref double xtl, ref double ytl, ref double wtl)
        {
            if (Math.Abs(wtl + w) < 1e-10)
                return; // detect zero regions

            xtl = (wtl * xtl + w * x) / (wtl + w);
            ytl = (wtl * ytl + w * y) / (wtl + w);
            wtl = w + wtl;
            return;
        }

        /// <summary>
        /// 增加区域面积
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="xtr"></param>
        /// <param name="ytr"></param>
        /// <param name="wtr"></param>
        /// <param name="xtl"></param>
        /// <param name="ytl"></param>
        /// <param name="wtl"></param>
        public static void AddRegion(double x1, double y1, double x2, double y2, ref double xtr, ref double ytr,
          ref double wtr, ref double xtl, ref double ytl, ref double wtl)
        {
            if (Math.Abs(x1 - x2) < 1e-10)
                return;

            if (x2 > x1)
            {
                AddPosPart((x2 + x1) / 2, y1 / 2, (x2 - x1) * y1, ref xtr, ref ytr, ref wtr); /* rectangle 全局变量变化处 */
                AddPosPart((x1 + x2 + x2) / 3, (y1 + y1 + y2) / 3, (x2 - x1) * (y2 - y1) / 2, ref xtr, ref ytr, ref wtr);
                // triangle 全局变量变化处
            }
            else
            {
                AddNegPart((x2 + x1) / 2, y1 / 2, (x2 - x1) * y1, ref xtl, ref ytl, ref wtl);
                // rectangle 全局变量变化处
                AddNegPart((x1 + x2 + x2) / 3, (y1 + y1 + y2) / 3, (x2 - x1) * (y2 - y1) / 2, ref xtl, ref ytl, ref wtl);
                // triangle  全局变量变化处
            }
        }

        /// <summary>
        /// 求任意简单多边形polygon的重心
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static Point2D Cg_Simple(int vcount, Point2D[] polygon)
        {
            double xtr, ytr, wtr, xtl, ytl, wtl;
            //注意： 如果把xtr,ytr,wtr,xtl,ytl,wtl改成全局变量后这里要删去
            Point2D p1 = new Point2D();
            Point2D p2 = new Point2D();
            Point2D tp = new Point2D();
            xtr = ytr = wtr = 0.0;
            xtl = ytl = wtl = 0.0;
            for (int i = 0; i < vcount; i++)
            {
                p1 = polygon[i];
                p2 = polygon[(i + 1) % vcount];
                AddRegion(p1.X, p1.Y, p2.X, p2.Y, ref xtr, ref ytr, ref wtr, ref xtl, ref ytl, ref wtl); //全局变量变化处
            }
            tp.X = (wtr * xtr + wtl * xtl) / (wtr + wtl);
            tp.Y = (wtr * ytr + wtl * ytl) / (wtr + wtl);
            return tp;
        }

        /// <summary>
        /// 求凸多边形的重心,要求输入多边形按逆时针排序
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static Point2D GravityCenter(int vcount, Point2D[] polygon)
        {
            Point2D tp = new Point2D();
            double x, y, s, x0, y0, cs, k;
            x = 0; y = 0; s = 0;
            for (int i = 1; i < vcount - 1; i++)
            {
                x0 = (polygon[0].X + polygon[i].X + polygon[i + 1].X) / 3;
                y0 = (polygon[0].Y + polygon[i].Y + polygon[i + 1].Y) / 3; //求当前三角形的重心
                cs = Multiply(polygon[i], polygon[i + 1], polygon[0]) / 2;
                //三角形面积可以直接利用该公式求解
                if (Math.Abs(s) < 1e-20)
                {
                    x = x0; y = y0; s += cs; continue;
                }
                k = cs / s; //求面积比例
                x = (x + k * x0) / (1 + k);
                y = (y + k * y0) / (1 + k);
                s += cs;
            }
            tp.X = x;
            tp.Y = y;
            return tp;
        }

        /************************************************
        给定一简单多边形，找出一个肯定在该多边形内的点
        定理1 ：每个多边形至少有一个凸顶点
        定理2 ：顶点数>=4的简单多边形至少有一条对角线
        结论 ： x坐标最大，最小的点肯定是凸顶点
         y坐标最大，最小的点肯定是凸顶点
        ************************************************/

        /// <summary>
        /// 给定一简单多边形，找出一个肯定在该多边形内的点
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static Point2D A_Point_Insidepoly(int vcount, Point2D[] polygon)
        {
            Point2D v = new Point2D();
            Point2D a = new Point2D();
            Point2D b = new Point2D();
            Point2D r = new Point2D();
            int i, index;
            v = polygon[0];
            index = 0;
            for (i = 1; i < vcount; i++) //寻找一个凸顶点
            {
                if (polygon[i].Y < v.Y)
                {
                    v = polygon[i];
                    index = i;
                }
            }
            a = polygon[(index - 1 + vcount) % vcount]; //得到v的前一个顶点
            b = polygon[(index + 1) % vcount]; //得到v的后一个顶点
            Point2D[] tri = new Point2D[3];
            Point2D q = new Point2D();
            tri[0] = a; tri[1] = v; tri[2] = b;
            double md = INF;
            int in1 = index;
            bool bin = false;
            for (i = 0; i < vcount; i++) //寻找在三角形avb内且离顶点v最近的顶点q
            {
                if (i == index) continue;
                if (i == (index - 1 + vcount) % vcount) continue;
                if (i == (index + 1) % vcount) continue;
                if (!InsideConvexPolygon(3, tri, polygon[i])) continue;
                bin = true;
                if (Dist(v, polygon[i]) < md)
                {
                    q = polygon[i];
                    md = Dist(v, q);
                }
            }
            if (!bin) //没有顶点在三角形avb内，返回线段ab中点
            {
                r.X = (a.X + b.X) / 2;
                r.Y = (a.Y + b.Y) / 2;
                return r;
            }
            r.X = (v.X + q.X) / 2; //返回线段vq的中点
            r.Y = (v.Y + q.Y) / 2;
            return r;
        }

        /***********************************************************************************************
        求从多边形外一点p出发到一个简单多边形的切线,如果存在返回切点,其中rp点是右切点,lp是左切点
        注意：p点一定要在多边形外 ,输入顶点序列是逆时针排列
        原 理： 如果点在多边形内肯定无切线;凸多边形有唯一的两个切点,凹多边形就可能有多于两个的切点;
          如果polygon是凸多边形，切点只有两个只要找到就可以,可以化简此算法
          如果是凹多边形还有一种算法可以求解:先求凹多边形的凸包,然后求凸包的切线
        /***********************************************************************************************/

        /// <summary>
        /// 求从多边形外一点p出发到一个简单多边形的切线,
        /// 其中rp点是右切点,lp是左切点
        /// 注意：p点一定要在多边形外 ,输入顶点序列是逆时针排列
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <param name="p"></param>
        /// <param name="rp"></param>
        /// <param name="lp"></param>
        public static void PointTangentPoly(int vcount, Point2D[] polygon, Point2D p, ref Point2D rp, ref Point2D lp)
        {
            LINESEG ep = new LINESEG();
            LINESEG en = new LINESEG();
            bool blp, bln;
            rp = polygon[0];
            lp = polygon[0];
            for (int i = 1; i < vcount; i++)
            {
                ep.s = polygon[(i + vcount - 1) % vcount];
                ep.e = polygon[i];
                en.s = polygon[i];
                en.e = polygon[(i + 1) % vcount];
                blp = Multiply(ep.e, p, ep.s) >= 0;                // p is to the left of pre edge
                bln = Multiply(en.e, p, en.s) >= 0;                // p is to the left of next edge
                if (!blp && bln)
                {
                    if (Multiply(polygon[i], rp, p) > 0)           // polygon[i] is above rp
                        rp = polygon[i];
                }
                if (blp && !bln)
                {
                    if (Multiply(lp, polygon[i], p) > 0)           // polygon[i] is below lp
                        lp = polygon[i];
                }
            }
            return;
        }

        /// <summary>
        /// 如果多边形polygon的核存在，返回true，返回核上的一点p.顶点按逆时针方向输入
        /// </summary>
        /// <param name="vcount"></param>
        /// <param name="polygon"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool Core_Exist(int vcount, Point2D[] polygon, ref Point2D p)
        {
            int i, j, k;
            LINESEG l = new LINESEG();
            LINE[] lineset = new LINE[MAXV];
            for (i = 0; i < vcount; i++)
            {
                lineset[i] = MakeLine(polygon[i], polygon[(i + 1) % vcount]);
            }
            for (i = 0; i < vcount; i++)
            {
                for (j = 0; j < vcount; j++)
                {
                    if (i == j) continue;
                    if (LineIntersect(lineset[i], lineset[j], ref p))
                    {
                        for (k = 0; k < vcount; k++)
                        {
                            l.s = polygon[k];
                            l.e = polygon[(k + 1) % vcount];
                            if (Multiply(p, l.e, l.s) > 0)
                                //多边形顶点按逆时针方向排列，核肯定在每条边的左侧或边上
                                break;
                        }
                        if (k == vcount)             //找到了一个核上的点
                            break;
                    }
                }
                if (j < vcount) break;
            }
            if (i < vcount)
                return true;
            else
                return false;
        }

        /*************************\
        *       *
        * 圆的基本运算           *
        *          *
        \*************************/
        /******************************************************************************
        返回值 ： 点p在圆内(包括边界)时，返回true
        用途 ： 因为圆为凸集，所以判断点集，折线，多边形是否在圆内时，
         只需要逐一判断点是否在圆内即可。
        *******************************************************************************/

        public static bool Point_In_Circle(Point2D o, double r, Point2D p)
        {
            double d2 = (p.X - o.X) * (p.X - o.X) + (p.Y - o.Y) * (p.Y - o.Y);
            double r2 = r * r;
            return d2 < r2 || Math.Abs(d2 - r2) < EP;
        }

        /******************************************************************************
        用 途 ：求不共线的三点确定一个圆
        输 入 ：三个点p1,p2,p3
        返回值 ：如果三点共线，返回false；反之，返回true。圆心由q返回，半径由r返回
        *******************************************************************************/

        public static bool Cocircle(Point2D p1, Point2D p2, Point2D p3, ref Point2D q, ref double r)
        {
            double x12 = p2.X - p1.X;
            double y12 = p2.Y - p1.Y;
            double x13 = p3.X - p1.X;
            double y13 = p3.Y - p1.Y;
            double z2 = x12 * (p1.X + p2.X) + y12 * (p1.Y + p2.Y);
            double z3 = x13 * (p1.X + p3.X) + y13 * (p1.Y + p3.Y);
            double d = 2.0 * (x12 * (p3.Y - p2.Y) - y12 * (p3.X - p2.X));
            if (Math.Abs(d) < EP) //共线，圆不存在
                return false;
            q.X = (y13 * z2 - y12 * z3) / d;
            q.Y = (x12 * z3 - x13 * z2) / d;
            r = Dist(p1, q);
            return true;
        }

        public static int Line_Circle(LINE l, Point2D o, double r, ref Point2D p1, ref Point2D p2)
        {
            return 1;
        }

        /**************************\
        *        *
        * 矩形的基本运算          *
        *                         *
        \**************************/
        /*
        说明：因为矩形的特殊性，常用算法可以化简：
        1.判断矩形是否包含点
        只要判断该点的横坐标和纵坐标是否夹在矩形的左右边和上下边之间。
        2.判断线段、折线、多边形是否在矩形中
        因为矩形是个凸集，所以只要判断所有端点是否都在矩形中就可以了。
        3.判断圆是否在矩形中
        圆在矩形中的充要条件是：圆心在矩形中且圆的半径小于等于圆心到矩形四边的距离的最小值。
        */

        // 已知矩形的三个顶点(a,b,c)，计算第四个顶点d的坐标. 注意：已知的三个顶点可以是无序的
        public static Point2D Rect4th(Point2D a, Point2D b, Point2D c)
        {
            Point2D d = new Point2D();
            if (Math.Abs(DotMultiply(a, b, c)) < EP) // 说明c点是直角拐角处
            {
                d.X = a.X + b.X - c.X;
                d.Y = a.Y + b.Y - c.Y;
            }
            if (Math.Abs(DotMultiply(a, c, b)) < EP) // 说明b点是直角拐角处
            {
                d.X = a.X + c.X - b.X;
                d.Y = a.Y + c.Y - b.X;
            }
            if (Math.Abs(DotMultiply(c, b, a)) < EP) // 说明a点是直角拐角处
            {
                d.X = c.X + b.X - a.X;
                d.Y = c.Y + b.Y - a.Y;
            }
            return d;
        }

        /*************************\
        *      *
        * 常用算法的描述  *
        *      *
        \*************************/
        /*
        尚未实现的算法：
        1. 求包含点集的最小圆
        2. 求多边形的交
        3. 简单多边形的三角剖分
        4. 寻找包含点集的最小矩形
        5. 折线的化简
        6. 判断矩形是否在矩形中
        7. 判断矩形能否放在矩形中
        8. 矩形并的面积与周长
        9. 矩形并的轮廓
        10.矩形并的闭包
        11.矩形的交
        12.点集中的最近点对
        13.多边形的并
        14.圆的交与并
        15.直线与圆的关系
        16.线段与圆的关系
        17.求多边形的核监视摄象机
        18.求点集中不相交点对 railwai
        *//*
        寻找包含点集的最小矩形
        原理：该矩形至少一条边与点集的凸壳的某条边共线
        First take the convex hull of the points. Let the resulting convex
        polygon be P. It has been known for some time that the minimum
        area rectangle enclosing P must have one rectangle side flush with
        (i.e., collinear with and overlapping) one edge of P. This geometric
        fact was used by Godfried Toussaint to develop the "rotating calipers"
        algorithm in a hard-to-find 1983 paper, "Solving Geometric Problems
        with the Rotating Calipers" (Proc. IEEE MELECON). The algorithm
        rotates a surrounding rectangle from one flush edge to the next,
        keeping track of the minimum area for each edge. It achieves O(n)
        time (after hull computation). See the "Rotating Calipers Homepage"
        http://www.cs.mcgill.ca/~orm/rotcal.frame.html for a description
        and applet.
        *//*
        折线的化简 伪码如下：
        Input: tol = the approximation tolerance
        L = {V0,V1, ,Vn-1} is any n-vertex polyline
        Set start = 0;
        Set k = 0;
        Set W0 = V0;
        for each vertex Vi (i=1,n-1)
        {
        if Vi is within tol from Vstart
        then ignore it, and continue with the next vertex
        Vi is further than tol away from Vstart
        so add it as a new vertex of the reduced polyline
        Increment k++;
        Set Wk = Vi;
        Set start = i; as the new initial vertex
        }
        Output: W = {W0,W1, ,Wk-1} = the k-vertex simplified polyline
        */
          /********************\
          *        *
          * 补充    *
          *     *
          \********************/

        //两圆关系：
        /* 两圆：
        相离： return 1；
        外切： return 2；
        相交： return 3；
        内切： return 4；
        内含： return 5；
        */

        public static int CircleRelation(Point2D p1, double r1, Point2D p2, double r2)
        {
            double d = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));

            if (Math.Abs(d - r1 - r2) < EP) // 必须保证前两个if先被判定！
                return 2;
            if (Math.Abs(d - Math.Abs(r1 - r2)) < EP)
                return 4;
            if (d > r1 + r2)
                return 1;
            if (d < Math.Abs(r1 - r2))
                return 5;
            if (Math.Abs(r1 - r2) < d && d < r1 + r2)
                return 3;
            return 0; // indicate an error!
        }

        //判断圆是否在矩形内：
        // 判定圆是否在矩形内，是就返回true（设矩形水平，且其四个顶点由左上开始按顺时针排列）
        // 调用Ptoldist函数，在第4页
        public static bool CircleRecRelation(Point2D pc, double r, Point2D pr1, Point2D pr2, Point2D pr3, Point2D pr4)
        {
            if (pr1.X < pc.X && pc.X < pr2.X && pr3.Y < pc.Y && pc.Y < pr2.Y)
            {
                LINESEG line1 = new LINESEG(pr1, pr2);
                LINESEG line2 = new LINESEG(pr2, pr3);
                LINESEG line3 = new LINESEG(pr3, pr4);
                LINESEG line4 = new LINESEG(pr4, pr1);
                if (r < PtoLDist(pc, line1) && r < PtoLDist(pc, line2) && r < PtoLDist(pc, line3) && r < PtoLDist(pc, line4))
                    return true;
            }
            return false;
        }

        //点到平面的距离：
        //点到平面的距离,平面用一般式表示ax+by+cz+d=0
        public static double P2planeDist(double x, double y, double z, double a, double b, double c, double d)
        {
            return Math.Abs(a * x + b * y + c * z + d) / Math.Sqrt(a * a + b * b + c * c);
        }

        //点是否在直线同侧：
        //两个点是否在直线同侧，是则返回true
        public static bool SameSide(Point2D p1, Point2D p2, LINE line)
        {
            return (line.a * p1.X + line.b * p1.Y + line.c) *
            (line.a * p2.X + line.b * p2.Y + line.c) > 0;
        }

        //镜面反射线：
        // 已知入射线、镜面，求反射线。
        // a1,b1,c1为镜面直线方程(a1 x + b1 y + c1 = 0 ,下同)系数;
        //a2,b2,c2为入射光直线方程系数;
        //a,b,c为反射光直线方程系数.
        // 光是有方向的，使用时注意：入射光向量:<-b2,a2>；反射光向量:<b,-a>.
        // 不要忘记结果中可能会有"negative zeros"
        public static void Reflect(double a1, double b1, double c1, double a2, double b2, double c2, ref double a, ref double b, ref double c)
        {
            double n, m;
            double tpb, tpa;
            tpb = b1 * b2 + a1 * a2;
            tpa = a2 * b1 - a1 * b2;
            m = (tpb * b1 + tpa * a1) / (b1 * b1 + a1 * a1);
            n = (tpa * b1 - tpb * a1) / (b1 * b1 + a1 * a1);
            if (Math.Abs(a1 * b2 - a2 * b1) < 1e-20)
            {
                a = a2; b = b2; c = c2;
                return;
            }
            double xx, yy; //(xx,yy)是入射线与镜面的交点。
            xx = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);
            yy = (a2 * c1 - a1 * c2) / (a1 * b2 - a2 * b1);
            a = n;
            b = -m;
            c = m * yy - xx * n;
        }

        //矩形包含：
        // 矩形2（C，D）是否在1（A，B）内
        public static bool R2inr1(double A, double B, double C, double D)
        {
            double X, Y, L, K, DMax;
            if (A < B)
            {
                double tmp = A;
                A = B;
                B = tmp;
            }
            if (C < D)
            {
                double tmp = C;
                C = D;
                D = tmp;
            }
            if (A > C && B > D)                 // trivial case
                return true;
            else
             if (D >= B)
                return false;
            else
            {
                X = Math.Sqrt(A * A + B * B);         // outer rectangle's diagonal
                Y = Math.Sqrt(C * C + D * D);         // inner rectangle's diagonal
                if (Y < B) // check for marginal conditions
                    return true; // the inner rectangle can freely rotate inside
                else
                 if (Y > X)
                    return false;
                else
                {
                    L = (B - Math.Sqrt(Y * Y - A * A)) / 2;
                    K = (A - Math.Sqrt(Y * Y - B * B)) / 2;
                    DMax = Math.Sqrt(L * L + K * K);
                    if (D >= DMax)
                        return false;
                    else
                        return true;
                }
            }
        }

        //两圆交点：
        // 两圆已经相交（相切）
        public static void C2Point(Point2D p1, double r1, Point2D p2, double r2, ref Point2D rp1, ref Point2D rp2)
        {
            double a, b, r;
            a = p2.X - p1.X;
            b = p2.Y - p1.Y;
            r = (a * a + b * b + r1 * r1 - r2 * r2) / 2;
            if (a == 0 && b != 0)
            {
                rp1.Y = rp2.Y = r / b;
                rp1.X = Math.Sqrt(r1 * r1 - rp1.Y * rp1.Y);
                rp2.X = -rp1.X;
            }
            else if (a != 0 && b == 0)
            {
                rp1.X = rp2.X = r / a;
                rp1.Y = Math.Sqrt(r1 * r1 - rp1.X * rp2.X);
                rp2.Y = -rp1.Y;
            }
            else if (a != 0 && b != 0)
            {
                double delta;
                delta = b * b * r * r - (a * a + b * b) * (r * r - r1 * r1 * a * a);
                rp1.Y = (b * r + Math.Sqrt(delta)) / (a * a + b * b);
                rp2.Y = (b * r - Math.Sqrt(delta)) / (a * a + b * b);
                rp1.X = (r - b * rp1.Y) / a;
                rp2.X = (r - b * rp2.Y) / a;
            }

            rp1.X += p1.X;
            rp1.Y += p1.Y;
            rp2.X += p1.X;
            rp2.Y += p1.Y;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        //两圆公共面积：
        // 必须保证相交
        public static double C2Area(Point2D p1, double r1, Point2D p2, double r2)
        {
            Point2D rp1 = new Point2D();
            Point2D rp2 = new Point2D();
            C2Point(p1, r1, p2, r2, ref rp1, ref rp2);

            if (r1 > r2) //保证r2>r1
            {
                Swap(ref p1, ref p2);
                Swap(ref r1, ref r2);
            }
            double a, b, rr;
            a = p1.X - p2.X;
            b = p1.Y - p2.Y;
            rr = Math.Sqrt(a * a + b * b);

            double dx1, dy1, dx2, dy2;
            double sita1, sita2;
            dx1 = rp1.X - p1.X;
            dy1 = rp1.Y - p1.Y;
            dx2 = rp2.X - p1.X;
            dy2 = rp2.Y - p1.Y;
            sita1 = Math.Acos((dx1 * dx2 + dy1 * dy2) / r1 / r1);

            dx1 = rp1.X - p2.X;
            dy1 = rp1.Y - p2.Y;
            dx2 = rp2.X - p2.X;
            dy2 = rp2.Y - p2.Y;
            sita2 = Math.Acos((dx1 * dx2 + dy1 * dy2) / r2 / r2);
            double s = 0;
            if (rr < r2)//相交弧为优弧
                s = r1 * r1 * (PI - sita1 / 2 + Math.Sin(sita1) / 2) + r2 * r2 * (sita2 - Math.Sin(sita2)) / 2;
            else//相交弧为劣弧
                s = (r1 * r1 * (sita1 - Math.Sin(sita1)) + r2 * r2 * (sita2 - Math.Sin(sita2))) / 2;

            return s;
        }

        //圆和直线关系：
        //0----相离 1----相切 2----相交
        public static int Clpoint(Point2D p, double r, double a, double b, double c, ref Point2D rp1, ref Point2D rp2)
        {
            int res = 0;

            c = c + a * p.X + b * p.Y;
            double tmp;
            if (a == 0 && b != 0)
            {
                tmp = -c / b;
                if (r * r < tmp * tmp)
                    res = 0;
                else if (r * r == tmp * tmp)
                {
                    res = 1;
                    rp1.Y = tmp;
                    rp1.X = 0;
                }
                else
                {
                    res = 2;
                    rp1.Y = rp2.Y = tmp;
                    rp1.X = Math.Sqrt(r * r - tmp * tmp);
                    rp2.X = -rp1.X;
                }
            }
            else if (a != 0 && b == 0)
            {
                tmp = -c / a;
                if (r * r < tmp * tmp)
                    res = 0;
                else if (r * r == tmp * tmp)
                {
                    res = 1;
                    rp1.X = tmp;
                    rp1.Y = 0;
                }
                else
                {
                    res = 2;
                    rp1.X = rp2.X = tmp;
                    rp1.Y = Math.Sqrt(r * r - tmp * tmp);
                    rp2.Y = -rp1.Y;
                }
            }
            else if (a != 0 && b != 0)
            {
                double delta;
                delta = b * b * c * c - (a * a + b * b) * (c * c - a * a * r * r);
                if (delta < 0)
                    res = 0;
                else if (delta == 0)
                {
                    res = 1;
                    rp1.Y = -b * c / (a * a + b * b);
                    rp1.X = (-c - b * rp1.Y) / a;
                }
                else
                {
                    res = 2;
                    rp1.Y = (-b * c + Math.Sqrt(delta)) / (a * a + b * b);
                    rp2.Y = (-b * c - Math.Sqrt(delta)) / (a * a + b * b);
                    rp1.X = (-c - b * rp1.Y) / a;
                    rp2.X = (-c - b * rp2.Y) / a;
                }
            }
            rp1.X += p.X;
            rp1.Y += p.Y;
            rp2.X += p.X;
            rp2.Y += p.Y;
            return res;
        }

        //内切圆：
        public static void Incircle(Point2D p1, Point2D p2, Point2D p3, ref Point2D rp, ref double r)
        {
            double dx31, dy31, dx21, dy21, d31, d21, a1, b1, c1;
            dx31 = p3.X - p1.X;
            dy31 = p3.Y - p1.Y;
            dx21 = p2.X - p1.X;
            dy21 = p2.Y - p1.Y;

            d31 = Math.Sqrt(dx31 * dx31 + dy31 * dy31);
            d21 = Math.Sqrt(dx21 * dx21 + dy21 * dy21);
            a1 = dx31 * d21 - dx21 * d31;
            b1 = dy31 * d21 - dy21 * d31;
            c1 = a1 * p1.X + b1 * p1.Y;

            double dx32, dy32, dx12, dy12, d32, d12, a2, b2, c2;
            dx32 = p3.X - p2.X;
            dy32 = p3.Y - p2.Y;
            dx12 = -dx21;
            dy12 = -dy21;

            d32 = Math.Sqrt(dx32 * dx32 + dy32 * dy32);
            d12 = d21;
            a2 = dx12 * d32 - dx32 * d12;
            b2 = dy12 * d32 - dy32 * d12;
            c2 = a2 * p2.X + b2 * p2.Y;

            rp.X = (c1 * b2 - c2 * b1) / (a1 * b2 - a2 * b1);
            rp.Y = (c2 * a1 - c1 * a2) / (a1 * b2 - a2 * b1);
            r = Math.Abs(dy21 * rp.X - dx21 * rp.Y + dx21 * p1.Y - dy21 * p1.X) / d21;
        }

        //求切点：
        // p---圆心坐标， r---圆半径， sp---圆外一点， rp1,rp2---切点坐标
        public static void Cutpoint(Point2D p, double r, Point2D sp, ref Point2D rp1, ref Point2D rp2)
        {
            Point2D p2 = new Point2D();
            p2.X = (p.X + sp.X) / 2;
            p2.Y = (p.Y + sp.Y) / 2;

            double dx2, dy2, r2;
            dx2 = p2.X - p.X;
            dy2 = p2.Y - p.Y;
            r2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);
            C2Point(p, r, p2, r2, ref rp1, ref rp2);
        }

        //线段的左右旋：
        /* l2在l1的左/右方向（l1为基准线）
        返回 0 ： 重合；
        返回 1 ： 右旋；
        返回 –1 ： 左旋；
        */

        public static int Rotat(LINESEG l1, LINESEG l2)
        {
            double dx1, dx2, dy1, dy2;
            dx1 = l1.s.X - l1.e.X;
            dy1 = l1.s.Y - l1.e.Y;
            dx2 = l2.s.X - l2.e.X;
            dy2 = l2.s.Y - l2.e.Y;

            double d;
            d = dx1 * dy2 - dx2 * dy1;
            if (d == 0)
                return 0;
            else if (d > 0)
                return -1;
            else
                return 1;
        }

        /*
        公式：
        球坐标公式：
        直角坐标为 P(x, y, z) 时，对应的球坐标是(rsinφMath.Cosθ, rsinφMath.Sinθ, rcosφ),其中φ是向量OP与Z轴的夹角，范围[0，π]；是OP在XOY面上的投影到X轴的旋角，范围[0，2π]
        直线的一般方程转化成向量方程：
        ax+by+c=0
        x-x0     y-y0
           ------ = ------- // (x0,y0)为直线上一点，m,n为向量
        m        n
        转换关系：
        a=n；b=-m；c=m·y0-n·x0；
        m=-b; n=a;
        三点平面方程：
        三点为P1，P2，P3
        设向量  M1=P2-P1; M2=P3-P1;
        平面法向量：  M=M1 x M2 （）
        平面方程：    M.i(x-P1.x)+M.j(y-P1.y)+M.k(z-P1.z)=0
        */
    }
}