using SolidWorks.Interop.sldworks;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CSharpAndSolidWorks
{
    public partial class FrmScreen : Form
    {
        public FrmScreen()
        {
            InitializeComponent();
        }

        private void FrmScreen_Load(object sender, EventArgs e)
        {
            GetPCInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ISldWorks sldWorks = Utility.ConnectToSolidWorks();
            ModelDoc2 modelDoc = (ModelDoc2)sldWorks.ActiveDoc;
            ModelView modelView = (ModelView)modelDoc.ActiveView;
            // modelDoc.ViewZoomtofit2();
            modelView.Scale2 = double.Parse(txtSC.Text);

            modelDoc.GraphicsRedraw2();
        }

        public void GetPCInfo()
        {
            //    Screen[] screens=Screen.AllScreens;
            // Dim arr As Screen() = Screen.AllScreens
            //Dim PicW = Screen.PrimaryScreen.Bounds.Width
            //Dim PicH = Screen.PrimaryScreen.Bounds.Height
            int index;
            int upperBound;

            // Gets an array of all the screens connected to the system.

            Screen[] screens = Screen.AllScreens;
            upperBound = screens.GetUpperBound(0);

            txtScreenX.Text = screens[0].WorkingArea.Width.ToString();
            txtScreenY.Text = screens[0].WorkingArea.Height.ToString();

            for (index = 0; index <= upperBound; index++)
            {
                // For each screen, add the screen properties to a list box.
                listBox1.Items.Add("Device Name: " + screens[index].DeviceName);
                listBox1.Items.Add("Bounds: " + screens[index].Bounds.ToString());
                listBox1.Items.Add("Type: " + screens[index].GetType().ToString());
                listBox1.Items.Add("Working Area: " + screens[index].WorkingArea.ToString());
                listBox1.Items.Add("Primary Screen: " + screens[index].Primary.ToString());
            }

            MyScreen sysInfo = new MyScreen();
            string id = sysInfo.GetMonitorPnpDeviceId()[0];
            SizeF size = sysInfo.GetMonitorPhysicalSize(id);

            float ss = MyScreen.MonitorScaler(size);

            textScreenSize.Text = ss.ToString();

            int width, height;
            if (MonitorInfo.GetPhysicalSize(@"\\.\DISPLAY1", out width, out height))
            {
                txtSCweight.Text = width.ToString();
                txtSCHeight.Text = height.ToString();
            }

            //MessageBox.Show(MyScreen.MonitorScaler(size).ToString() + @"寸");
        }

        public double GetScreenRealWeight(double screensize)
        {
            if (screensize == 24)
            {
                return 521;
            }

            return 0;
        }

        private void buttonAuto_Click(object sender, EventArgs e)
        {
            //新建一个零件  ,画一条150长度的直线,然后最大化.

            ISldWorks iswApp = Utility.ConnectToSolidWorks();

            string partDefaultPath = iswApp.GetDocumentTemplate(1, "", 0, 0, 0);

            var part = iswApp.NewDocument(partDefaultPath, 0, 0, 0);

            ModelDoc2 modelDoc = (ModelDoc2)iswApp.ActiveDoc;

            modelDoc.Extension.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);

            modelDoc.SketchManager.InsertSketch(true);

            modelDoc.SketchManager.CreateLine(0, 0.1, 0, 0, 0, 0);

            modelDoc.SketchManager.InsertSketch(true);

            modelDoc.ClearSelection2(true);

            modelDoc.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 1, null, 0);

            var b = modelDoc.InsertCompositeCurve();

            modelDoc.ClearSelection2(true);

            modelDoc.FeatureManager.ViewFeatures = false;

            modelDoc.ViewZoomtofit2();

            ModelView modelView = (ModelView)modelDoc.ActiveView;

            iswApp.FrameState = 1; //最大化solidworks
            modelDoc.ViewZoomtofit2();
            modelView.Scale2 = 0.5;

            //

            modelDoc.ClearSelection2(true);

            string ImagePath = @"D:\temp.JPG";

            modelDoc.SaveAs3(ImagePath, 0, 0);

            int LineInImage = img2color(ImagePath);

            try
            {
                System.IO.File.Delete(ImagePath);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            if (LineInImage > 0)
            {
                double ActionScreenHeight = double.Parse(txtSCHeight.Text); //GetScreenRealWeight(double.Parse( textScreenSize.Text));

                //当前视图的比例
                double ActionScale = modelView.Scale2;

                //solidworks中的直线占比
                double defaultSolidworksP = LineInImage / 1200.0;//double.Parse(txtScreenY.Text);

                //当前比例下 1像素  等于多少mm
                double thisSc = 100 / LineInImage;

                //实际屏幕 1 像素尺寸

                double oneP = (ActionScreenHeight) / 1200.0;

                //当比例设置为 1时 1像素能表示的长度:
                double onePshowLength = thisSc / ActionScale;

                //959  1370

                GetWindowsSize getWindowSize = new GetWindowsSize();

                GetWindowsSize.windsize solidworksize = getWindowSize.GetSize("SLDWORKS");

                double getSC = 50.0 / (LineInImage * oneP);

                txtSC.Text = getSC.ToString();// getSC.ToString();

                modelView.Scale2 = getSC;

                modelDoc.GraphicsRedraw2();

                Debug.Print(getSC.ToString());
                // modelView.Scale2 = double.Parse(txtSC.Text);
                // sldWorks.scen
            }

            iswApp.CloseDoc(modelDoc.GetPathName());
        }

        private void txtReal1_TextChanged(object sender, EventArgs e)
        {
            txtSC.Text = (double.Parse(txtsw.Text) / (double.Parse(txtReal1.Text) * 2)).ToString();
        }

        public static int img2color(String imgfile)
        {
            Bitmap img = new Bitmap(imgfile);

            Color[,] allcolor = new Color[img.Height, img.Width];

            int minH = img.Height;
            int maxH = 0;

            Debug.Print(img.Height + "," + img.Width);

            for (int h = 0; h < img.Height; h++)
                for (int w = img.Width / 2 - 200; w < img.Width / 2 + 200; w++)
                {
                    allcolor[h, w] = img.GetPixel(w, h);

                    if (allcolor[h, w].R != 196)
                    {
                        Debug.Print(allcolor[h, w].R.ToString() + "," + allcolor[h, w].G.ToString() + "," + allcolor[h, w].B.ToString());
                    }

                    if (allcolor[h, w].R == 0 && allcolor[h, w].G == 0 && allcolor[h, w].B == 254)
                    {
                        // Debug.Print(allcolor[h, w].ToString());
                        if (h > maxH)
                        {
                            maxH = h;
                        }
                        if (h < minH)
                        {
                            minH = h;
                        }
                    }
                }

            Debug.Print(minH + "," + maxH);

            GC.Collect();

            return maxH - minH;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ISldWorks sldWorks = Utility.ConnectToSolidWorks();

            ModelDoc2 modelDoc = (ModelDoc2)sldWorks.ActiveDoc;
            ModelView modelView = (ModelView)modelDoc.ActiveView;

            sldWorks.FrameState = 1;
            modelDoc.ViewZoomtofit2();
            modelView.Scale2 = 0.5;
        }
    }
}