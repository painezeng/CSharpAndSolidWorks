using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;

namespace CSharpAndSolidWorks
{
    public partial class MyPane : Form
    {
        private List<BomItem> bomItems = new List<BomItem>();
        public string ignore = "";
        public bool cancel = false;

        public ISldWorks iswApp = null;

        public MyPane(ISldWorks IswApp)
        {
            InitializeComponent();

            this.iswApp = IswApp;

            ResetGridTable();
            LoadFunction();
        }

        private void LoadFunction()
        {
            ResetGridTable();

            bomItems.Clear();

            GetBOMList();

            //InsertBOMTable();
            InsertBOMtoReoGrid();

            StatusLab.Text = "加载完成...";
        }

        private void InsertBOMTable()
        {
            ModelDoc2 modelDoc2 = iswApp.ActiveDoc;

            ModelDocExtension modelDocExtension = modelDoc2.Extension;

            TableAnnotation tableAnnotation = modelDocExtension.InsertGeneralTableAnnotation(true, 0, 0, (int)swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopLeft, @"E:\01_Work\14_Project\材料清单.sldtbt", bomItems.Count + 2, 7);

            for (int i = 0; i < bomItems.Count; i++)
            {
                tableAnnotation.Text[i + 2, 0] = (i + 1).ToString();
                tableAnnotation.Text[i + 2, 1] = bomItems[i].name;
                tableAnnotation.Text[i + 2, 2] = bomItems[i].material;
                tableAnnotation.Text[i + 2, 3] = bomItems[i].qty.ToString();
                tableAnnotation.Text[i + 2, 4] = bomItems[i].comment;
            }

            //tableAnnotation.SetColumnWidth(0, 10,0);

            //foreach (var b in bomItems)
            //{
            //	Debug.Print(b.code + "--->" + b.qty.ToString());

            //}
        }

        /// <summary>
        /// 把BOM清单插入表格式
        /// </summary>
        private void InsertBOMtoReoGrid()
        {
            ResetGridTable();
            var sheet = ReoGridReport.CurrentWorksheet;

            //bomItems.Sort();

            string xuhao = "";

            for (int i = 0; i < bomItems.Count; i++)
            {
                string Qianzhui = bomItems[i].level;

                string Qianzhui2 = "";

                try
                {
                    Qianzhui2 = bomItems[i - 1].level;
                }
                catch (Exception)
                {
                    Qianzhui2 = ".";
                }

                string tempA = "";
                if (i == 0)
                {
                    tempA = "1";
                    xuhao = "1";
                }
                else
                {
                    tempA = sheet[string.Join("", "A", i + 1)].ToString();
                    xuhao = GetNextLevel(tempA, Qianzhui2, Qianzhui);
                }

                int actionRow = i + 2;

                sheet[string.Join("", "A", actionRow)] = xuhao;

                sheet[string.Join("", "B", actionRow)] = bomItems[i].name2;

                sheet[string.Join("", "C", actionRow)] = bomItems[i].material;

                string tempsize = bomItems[i].size;

                sheet[string.Join("", "D", actionRow)] = bomItems[i].qty.ToString();

                //备料 尺寸 长宽厚

                string tempCommect = bomItems[i].comment;

                sheet[string.Join("", "E", actionRow)] = tempCommect;
            }

            sheet.RowCount = bomItems.Count + 1;

            sheet.AutoFitColumnWidth(0, true);
            sheet.AutoFitColumnWidth(1, true);
            sheet.AutoFitColumnWidth(2, true);
            sheet.AutoFitColumnWidth(3, false);
            sheet.AutoFitColumnWidth(4, false);

            sheet.SetColumnsWidth(3, 3, 45);
            sheet.SetColumnsWidth(4, 3, 45);

            sheet.ColumnHeaders["A"].Style.Horizo​​ntalAlign = ReoGridHorAlign.Left;
            sheet.ColumnHeaders["B"].Style.Horizo​​ntalAlign = ReoGridHorAlign.Left;
            sheet.ColumnHeaders["C"].Style.Horizo​​ntalAlign = ReoGridHorAlign.Left;
            sheet.ColumnHeaders["D"].Style.Horizo​​ntalAlign = ReoGridHorAlign.Left;
            sheet.ColumnHeaders["E"].Style.Horizo​​ntalAlign = ReoGridHorAlign.Center;

            ReoGridReport.DoAction(sheet, new SetRangeBorderAction(new RangePosition(0, 0, sheet.RowCount, sheet.ColumnCount),
                                BorderPositions.All,
                                new RangeBorderStyle
                                {
                                    Color = unvell.ReoGrid.Graphics.SolidColor.Black,
                                    Style = BorderLineStyle.Solid
                                })
);

            sheet.SetRangeStyles("A1:E1", new WorksheetRangeStyle
            {
                Flag = PlainStyleFlag.BackColor,
                BackColor = Color.LightGray
            });

            //var header = sheet.ColumnHeaders["A"];

            //header.Style.Horizo​​ntalAlign = ReoGridHorAlign.Center;
        }

        /// <summary>
        /// 读取当前装配的BOM 清单
        ///
        /// </summary>
        private void GetBOMList()
        {
            ModelDoc2 swModel = iswApp.ActiveDoc;
            if (swModel.GetType() == 2)
            {
                Configuration swConf = (Configuration)swModel.GetActiveConfiguration();
                Component2 swRootComp = (Component2)swConf.GetRootComponent3(false);

                //OutputCompXform(swRootComp, 0);

                OutputCompXformTemp(swRootComp, 0, "");
            }
            else
            {
                //MessageBox.Show("BOM功能只适用于装配体.");
            }
        }

        /// <summary>
        /// 利用当前序号 和 下一级层级得到下一级序号
        ///
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="NextB"></param>
        /// <returns></returns>
        private string GetNextLevel(string A, string B, string NextB)
        {
            string tempLevel = "";
            if (NextB.Length == B.Length)
            {
                if (A.Contains("."))
                {
                    List<string> tempA = new List<string>(A.Split('.'));

                    int tempI = int.Parse(tempA[tempA.Count - 1]) + 1;

                    tempA[tempA.Count - 1] = tempI.ToString();

                    return string.Join(".", tempA);
                }
                else
                {
                    int tempI = int.Parse(A) + 1;

                    return tempI.ToString();
                }
            }

            if (NextB.Length > B.Length)
            {
                //List<string> tempA = new List<string>(A.Split('.'));

                //int tempI = int.Parse(tempA[tempA.Count - 1]) + 1;

                //tempA[tempA.Count - 1] = tempI.ToString();

                return A + ".1";
            }
            if (NextB.Length < B.Length)
            {
                List<string> tempA = new List<string>(A.Split('.'));
                if (tempA.Count == 1)
                {
                }
                else
                {
                    int tempI = int.Parse(tempA[tempA.Count - 2]) + 1;

                    tempA[tempA.Count - 2] = tempI.ToString();

                    tempA.RemoveAt(tempA.Count - 1);

                    return string.Join(".", tempA);
                }
            }

            return tempLevel;
        }

        /// <summary>
        /// 遍历BOM清单
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="nLevel"></param>
        public void OutputCompXform(Component2 swComp, long nLevel)
        {
        }

        public void OutputCompXformTemp(Component2 swComp, long nLevel, string parents)
        {
            object[] vChild;
            Component2 swChildComp;
            string sPadStr = "";
            string tempSelectID = "";
            MathTransform swCompXform;
            //  object vXform;
            long i;

            for (i = 0; i < nLevel; i++)
            {
                sPadStr = sPadStr + ".";
            }

            swCompXform = swComp.Transform2;

            if (swCompXform != null)
            {
                ModelDoc2 swModel;
                swModel = (ModelDoc2)swComp.GetModelDoc2();
                Boolean tempSupp = swComp.IsSuppressed();
                Boolean tempHide = swComp.IsHidden(false);

                if (swModel != null && tempSupp == false)
                {
                    StatusLab.Text = "Loading:" + swComp.Name;

                    string tempCode = Path.GetFileNameWithoutExtension(swModel.GetPathName());
                    string swMateDB = "";
                    string tempMaterial = "";
                    if (swModel.GetType() == 1)
                    {
                        tempMaterial = ((PartDoc)swModel).GetMaterialPropertyName2("", out swMateDB);
                    }
                    else
                    {
                        tempMaterial = "";
                    }

                    string tempSize = swModel.GetCustomInfoValue("", "零件尺寸");

                    string tempComment = swModel.GetCustomInfoValue("", "其它说明");

                    string tempblanktype = swModel.GetCustomInfoValue("", "下料方式");

                    string tempName = CodetoName(tempCode);

                    string tempName2 = GetName(tempCode);

                    tempSelectID = swComp.GetSelectByIDString();

                    int tempblanqty = 1;

                    if (ignore != "" && tempSelectID.Contains(ignore) == true)
                    {
                    }
                    else
                    {
                        var q = bomItems.Where(x => x.name == tempName && x.parents == parents).ToList();

                        if (q.Count == 0 && tempCode.Contains("镜向") == false && tempCode.Contains("复制") == false)
                        {
                            bomItems.Add(new BomItem(sPadStr, tempCode, tempMaterial, tempSize, 1, "", tempComment, tempName, tempblanqty, tempName2, tempSelectID, parents, tempblanktype));
                        }
                        else if (tempCode.Contains("镜向"))
                        {
                            if (q.Count == 0)
                            {
                                if (sPadStr.Length == 1)
                                {
                                    bomItems.Add(new BomItem(sPadStr, tempCode, tempMaterial, tempSize, 0, "", tempComment, tempName, 1, tempName, tempSelectID, parents, tempblanktype));
                                }

                                bomItems.Add(new BomItem(sPadStr, tempCode, "", "", 1, "", tempComment, "", 0, tempName2, tempSelectID, parents, tempblanktype));
                            }
                            else
                            {
                                if (bomItems[bomItems.IndexOf(q[0])].parents == parents)
                                {
                                    bomItems[bomItems.IndexOf(q[0])].blankqty = bomItems[bomItems.IndexOf(q[0])].blankqty + 1;

                                    //	bomItems[bomItems.IndexOf(q[0])] = new BomItem(sPadStr, bomItems[bomItems.IndexOf(q[0])].code, bomItems[bomItems.IndexOf(q[0])].material, bomItems[bomItems.IndexOf(q[0])].size, bomItems[bomItems.IndexOf(q[0])].qty, bomItems[bomItems.IndexOf(q[0])].blanksize, tempComment, bomItems[bomItems.IndexOf(q[0])].name, bomItems[bomItems.IndexOf(q[0])].blankqty + 1, bomItems[bomItems.IndexOf(q[0])].name2);

                                    var q2 = bomItems.Where(x => x.name2 == tempName2).ToList();

                                    if (q2.Count == 0)
                                    {
                                        //bomItems.Add(new BomItem(sPadStr, tempCode, tempMaterial, tempSize, 0, tempBlankSize, tempComment, tempName, 0, tempName2));
                                        bomItems.Add(new BomItem(sPadStr, tempCode, "", "", 1, "", tempComment, "", 0, tempName2, tempSelectID, parents, tempblanktype));
                                    }
                                    else
                                    {
                                        bomItems[bomItems.IndexOf(q2[0])].qty = bomItems[bomItems.IndexOf(q2[0])].qty + 1;
                                    }
                                }
                                else
                                {
                                    bomItems.Add(new BomItem(sPadStr, tempCode, tempMaterial, tempSize, 1, "", tempComment, tempName, tempblanqty, tempName2, tempSelectID, parents, tempblanktype));
                                }
                            }
                        }
                        else if (tempCode.Contains("复制"))
                        {
                            if (q.Count == 0)
                            {
                                bomItems.Add(new BomItem(sPadStr, tempCode, tempMaterial, tempSize, 1, "", tempComment, tempName, 1, tempName, tempSelectID, parents, tempblanktype));
                            }
                            else
                            {
                                bomItems[bomItems.IndexOf(q[0])].qty = bomItems[bomItems.IndexOf(q[0])].qty + 1;
                                bomItems[bomItems.IndexOf(q[0])].blankqty = bomItems[bomItems.IndexOf(q[0])].blankqty + 1;

                                if (swModel.GetType() == 2)
                                {
                                    ignore = tempSelectID;
                                }
                            }
                        }
                        else if (q.Count > 0 && tempCode.Contains("镜向") == false && tempCode.Contains("复制") == false && bomItems[bomItems.IndexOf(q[0])].parents == parents)
                        {
                            bomItems[bomItems.IndexOf(q[0])].qty = bomItems[bomItems.IndexOf(q[0])].qty + 1;
                            bomItems[bomItems.IndexOf(q[0])].blankqty = bomItems[bomItems.IndexOf(q[0])].blankqty + 1;

                            if (swModel.GetType() == 2)
                            {
                                ignore = tempSelectID;
                            }
                        }
                        else
                        {
                            bomItems.Add(new BomItem(sPadStr, tempCode, tempMaterial, tempSize, 1, "", tempComment, tempName, tempblanqty, tempName2, tempSelectID, parents, tempblanktype));
                        }
                    }
                }
            };

            vChild = (object[])swComp.GetChildren();

            for (i = 0; i <= (vChild.Length - 1); i++)
            {
                swChildComp = (Component2)vChild[i];

                OutputCompXformTemp(swChildComp, nLevel + 1, tempSelectID);

                if (i == vChild.Length - 1)
                {
                    ignore = "";
                }
            }
        }

        public string GetName(string Rname)
        {
            if (Rname.Contains("^"))
            {
                List<string> temps = Rname.Split('^').ToList();

                return temps[0];
            }
            return Rname;
            //string Reuslt = "";

            //string pattern = @".*\-\D*?\d{1,2}?\]|.*\-\D*";

            //Regex reg = new Regex(pattern);

            //Reuslt = reg.Match(Rname).ToString();

            //if (Reuslt == "")
            //{
            //    Reuslt = Rname;
            //}

            //return Reuslt;
        }

        public string CodetoName(string _code)
        {
            if (_code.Contains("^"))
            {
                List<string> temps = _code.Split('^').ToList();

                _code = temps[0];
            }

            if (_code.Contains("复制-"))
            {
                return GetName(_code.Replace("复制-", ""));
            }
            else if (_code.Contains("镜向-"))
            {
                return GetName(_code.Replace("镜向-", ""));
                //return _code;
            }
            else if (_code.Contains("镜向"))
            {
                return GetName(_code.Replace("镜向", ""));
                //return _code;
            }
            else
            {
                return _code;
            }
        }

        private void MyPane_Load(object sender, EventArgs e)
        {
            //ResetGridTable();
            //LoadFunction();
        }

        /// <summary>
        /// 重设 表格格式
        /// </summary>
        private void ResetGridTable()
        {
            ModelDoc2 swModel = null;

            swModel = (ModelDoc2)iswApp.ActiveDoc;

            if (swModel.GetType() == 2)
            {
                labActionModelPath.Text = swModel.GetPathName();

                var sheet = ReoGridReport.CurrentWorksheet;
                sheet.Reset();

                sheet["A1"] = "序号";
                sheet["B1"] = "名称";
                sheet["C1"] = "材料";
                sheet["D1"] = "数量";
                sheet["E1"] = "备注";
                sheet.ColumnCount = 5;
                sheet.RowCount = 1000;

                sheet.AutoFitColumnWidth(0, true);
                sheet.AutoFitColumnWidth(1, true);
                sheet.AutoFitColumnWidth(2, true);
                sheet.AutoFitColumnWidth(3, true);
                sheet.AutoFitColumnWidth(4, true);

                sheet.SetColumnsWidth(3, 3, 45);
            }
            else
            {
                //MessageBox.Show("只有装配体才有BOM");
                cancel = true;
                this.Close();

                //return;
            }
        }

        /// <summary>
        /// 这里提供获取当前装配体的 bounding box
        /// </summary>
        /// <returns>返回 LxWxH</returns>
        private string GetActionAssemlbyBOX()
        {
            BoxSize newboxSize = new BoxSize();

            ModelDoc2 swModel = default(ModelDoc2);

            double L = 0;
            double W = 0;
            double H = 0;

            double[] BoxFeatureDblArray = new double[7];

            double[] BoxFaceDblArray = new double[7];

            SketchPoint[] swSketchPt = new SketchPoint[9];
            SketchSegment[] swSketchSeg = new SketchSegment[13];

            swModel = (ModelDoc2)iswApp.IActiveDoc2;

            AssemblyDoc assemblyDoc = (AssemblyDoc)swModel;

            BoxFeatureDblArray = (double[])assemblyDoc.GetBox((int)swBoundingBoxOptions_e.swBoundingBoxIncludeRefPlanes);

            L = (BoxFeatureDblArray[3] - BoxFeatureDblArray[0]) * 1000;
            W = (BoxFeatureDblArray[4] - BoxFeatureDblArray[1]) * 1000;
            H = (BoxFeatureDblArray[5] - BoxFeatureDblArray[2]) * 1000;

            List<double> myList = new List<double> { L, W, H };

            myList.Sort();

            newboxSize.Length = myList[2];
            newboxSize.Weigth = myList[1];
            newboxSize.Height = myList[0];

            bool b = swModel.Extension.SelectByID2("Size", "SKETCH", 0, 0, 0, false, 0, null, 0);
            if (b == false)
            {
                swModel.Insert3DSketch2(true);
                swModel.SetAddToDB(true);
                swModel.SetDisplayWhenAdded(false);

                SketchManager swSketchMgr = default(SketchManager);

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
            }

            BoxSize boxSize = new BoxSize();
            boxSize.Length = Math.Round(newboxSize.Length, 0);
            boxSize.Weigth = Math.Round(newboxSize.Weigth, 0);
            boxSize.Height = Math.Round(newboxSize.Height, 0);

            string proText = string.Join("x", boxSize.Length, boxSize.Weigth, boxSize.Height);

            return proText;
        }

        public struct BoxSize
        {
            public double Length;
            public double Weigth;
            public double Height;
        }

        private void ReloadBOMMenuItem_Click(object sender, EventArgs e)
        {
            LoadFunction();
        }

        private void ToolStripMenuItemExport_Click(object sender, EventArgs e)
        {
            string excelPath = System.IO.Path.GetDirectoryName(labActionModelPath.Text) + "\\" + System.IO.Path.GetFileNameWithoutExtension(labActionModelPath.Text) + ".xlsx";

            ReoGridReport.Save(excelPath, unvell.ReoGrid.IO.FileFormat.Excel2007, System.Text.Encoding.UTF8);

            MessageBox.Show("导出成功!");

            if (checkOpenFile.Checked == true)
            {
                Process.Start(excelPath);
            }
        }
    }

    //bom item class
    public class BomItem : IComparable

    {
        public string level { get; set; }
        public string code { get; set; }
        public string material { get; set; }
        public string size { get; set; }
        public int qty { get; set; }
        public string blanksize { get; set; }
        public string comment { get; set; }
        public string name { get; set; }
        public string name2 { get; set; }
        public int blankqty { get; set; }

        //public List<BomItem> childs { get; set; }
        public string selectID { get; set; }

        public string parents { get; set; }
        public string blankType { get; set; }

        public BomItem(string level, string code, string material, string size, int qty, string blanksize, string comment, string name, int blankqty, string name2, string selectID, string parents, string blankType)
        {
            this.level = level;
            this.code = code;
            this.material = material;
            this.size = size;
            this.qty = qty;
            this.blanksize = blanksize;
            this.comment = comment;
            this.name = name;
            this.blankqty = blankqty;
            this.name2 = name2;
            this.selectID = selectID;
            this.parents = parents;
            this.blankType = blankType;
        }

        public int CompareTo(object obj)
        {
            BomItem temp = (BomItem)obj;
            return this.selectID.CompareTo(temp.selectID);
        }
    }
}