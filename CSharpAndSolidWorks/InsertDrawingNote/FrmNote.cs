using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CSharpAndSolidWorks
{
    public partial class FrmNote : Form
    {
        private SldWorks swApp;
        private ModelDoc2 swModel;
        private ModelDocExtension swModelDocExt;
        private ModelView swModelView;
        public static Mouse TheMouse;
        private mouseClass obj;

        public static double x = 50;
        public static double y = 50;
        public static double l = 50;

        public static bool haveNextNote = true; //是否还有下一个文字

        public int activeIndex = 0;

        public string activeNote { get; set; } //当前插入的文字

        public static List<string> noteList = new List<string>();

        public FrmNote()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            activeIndex = 0;
            activeNote = noteList[activeIndex];
            haveNextNote = true;
            // this.Visible = false;
            swApp = Comm.ConnectToSolidWorks();

            swModel = (ModelDoc2)swApp.ActiveDoc;

            swModelDocExt = swModel.Extension;

            swModelView = (ModelView)swModel.GetFirstModelView();

            TheMouse = swModelView.GetMouse();

            mouseClass mouseClass = new mouseClass(this);

            Frame swFrame = (Frame)swApp.Frame();

            swFrame.SetStatusBarText("Next Click  to insert " + activeNote);

            TheMouse.MouseSelectNotify += mouseClass.ms_MouseSelectNotify;

            Debug.Print("done");
        }

        public void NextNote()
        {
            activeIndex = activeIndex + 1;

            if (activeIndex >= noteList.Count)
            {
                haveNextNote = false;
            }
            else
            {
                activeNote = noteList[activeIndex];

                // labNextnote.Text = activeNote;
            }

            // activeNote = txtS1.Text + " " + DateTime.Now.ToString();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Refresh();

            foreach (var item in groupBox1.Controls)
            {
                if (item is RadioButton)
                {
                    var thisR = (RadioButton)item;

                    thisR.Text = "";
                }
            }

            var r = (RadioButton)sender;

            r.Text = "Text";

            Pen pen = new Pen(Color.Black, 1);

            Point point1 = new Point(r.Left, r.Top);

            Point point2 = new Point(groupBox1.Width / 2, groupBox1.Height / 2);

            Graphics g = groupBox1.CreateGraphics();

            g.DrawLine(pen, point1, point2);
            SetNoteLocation(r.Name.ToUpper());
        }

        private void SetNoteLocation(string rName)
        {
            switch (rName)
            {
                case "RADIOBUTTON1":
                    x = -double.Parse(cobLeaderLength.Text);
                    y = double.Parse(cobLeaderLength.Text);

                    break;

                case "RADIOBUTTON2":
                    x = -double.Parse(cobLeaderLength.Text);
                    y = 0;

                    break;

                case "RADIOBUTTON3":
                    x = -double.Parse(cobLeaderLength.Text);
                    y = -double.Parse(cobLeaderLength.Text);

                    break;

                case "RADIOBUTTON4":
                    x = 0;
                    y = -double.Parse(cobLeaderLength.Text);

                    break;

                case "RADIOBUTTON5":
                    x = double.Parse(cobLeaderLength.Text);
                    y = -double.Parse(cobLeaderLength.Text);

                    break;

                case "RADIOBUTTON6":
                    x = double.Parse(cobLeaderLength.Text);
                    y = 0;

                    break;

                case "RADIOBUTTON7":
                    x = double.Parse(cobLeaderLength.Text);
                    y = double.Parse(cobLeaderLength.Text);

                    break;

                case "RADIOBUTTON8":
                    x = 0;
                    y = double.Parse(cobLeaderLength.Text);

                    break;

                default:
                    x = double.Parse(cobLeaderLength.Text);
                    y = double.Parse(cobLeaderLength.Text);

                    break;
            }

            l = double.Parse(cobLeaderLength.Text);
        }

        private void FrmNote_Load(object sender, EventArgs e)
        {
            //this.Show();
            radioButton_CheckedChanged(radioButton7, null);
        }

        private void GetNodeList()
        {
            lstNoteList.Items.Clear();
            if (cobNoteOrderType.Text == "AAABBBC")
            {
                if (cobQty1.Text != "")
                {
                    for (int i = 0; i < int.Parse(cobQty1.Text); i++)
                    {
                        if (cobQty1.Text == "1" && i == 0)
                        {
                            lstNoteList.Items.Add(cobStart1.Text + cobEnd1.Text);
                        }
                        else
                        {
                            lstNoteList.Items.Add(cobStart1.Text + (i + 1) + cobEnd1.Text);
                        }
                    }
                }
                if (cobQty2.Text != "")
                {
                    for (int i = 0; i < int.Parse(cobQty2.Text); i++)
                    {
                        if (cobQty2.Text == "1" && i == 0)
                        {
                            lstNoteList.Items.Add(cobStart2.Text + cobEnd2.Text);
                        }
                        else
                        {
                            lstNoteList.Items.Add(cobStart2.Text + (i + 1) + cobEnd2.Text);
                        }
                    }
                }
                if (cobQty3.Text != "")
                {
                    for (int i = 0; i < int.Parse(cobQty3.Text); i++)
                    {
                        if (cobQty3.Text == "1" && i == 0)
                        {
                            lstNoteList.Items.Add(cobStart3.Text + cobEnd3.Text);
                        }
                        else
                        {
                            lstNoteList.Items.Add(cobStart3.Text + (i + 1) + cobEnd3.Text);
                        }
                    }
                }
            }

            if (cobNoteOrderType.Text == "ABABABC")
            {
                if (cobQty1.Text == "" || cobQty2.Text == "")
                {
                    MessageBox.Show("A和B有一项为空,没法选择ABABABC哦");

                    return;
                }

                var maxQty = int.Parse(cobQty1.Text) > int.Parse(cobQty2.Text) ? int.Parse(cobQty1.Text) : int.Parse(cobQty2.Text);

                for (int i = 0; i < maxQty; i++)
                {
                    if (maxQty == 1 && i == 0)
                    {
                        if (i < int.Parse(cobQty1.Text))
                        {
                            lstNoteList.Items.Add(cobStart1.Text + cobEnd1.Text);
                        }
                        if (i < int.Parse(cobQty2.Text))
                        {
                            lstNoteList.Items.Add(cobStart2.Text + cobEnd2.Text);
                        }
                    }
                    else
                    {
                        if (i < int.Parse(cobQty1.Text))
                        {
                            lstNoteList.Items.Add(cobStart1.Text + (i + 1) + cobEnd1.Text);
                        }
                        if (i < int.Parse(cobQty2.Text))
                        {
                            lstNoteList.Items.Add(cobStart2.Text + (i + 1) + cobEnd2.Text);
                        }
                    }
                }

                if (cobQty3.Text != "")
                {
                    for (int i = 0; i < int.Parse(cobQty3.Text); i++)
                    {
                        if (cobQty3.Text == "1" && i == 0)
                        {
                            lstNoteList.Items.Add(cobStart3.Text + cobEnd3.Text);
                        }
                        else
                        {
                            lstNoteList.Items.Add(cobStart3.Text + (i + 1) + cobEnd3.Text);
                        }
                    }
                }
            }

            noteList = lstNoteList.Items.Cast<string>().ToList();

            if (activeIndex < noteList.Count)
            {
                activeNote = noteList[activeIndex];
            }
        }

        private void butUpdateList_Click(object sender, EventArgs e)
        {
            GetNodeList();
        }

        private void cobStart1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cobQty1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}