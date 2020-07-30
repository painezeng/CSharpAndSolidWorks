using Microsoft.VisualBasic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Windows.Forms;

namespace CSharpAndSolidWorks
{
    public partial class FrmCopy : Form
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="f">源文件路径</param>
        /// <param name="t">目标文件路径</param>
        public FrmCopy(string f, string t)
        {
            InitializeComponent();
            txtFrom.Text = f;
            txtTo.Text = t;
            btnPack.Focus();
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            ModelDoc2 swModel = (ModelDoc2)swApp.ActiveDoc;

            //CopySolidworksFile(txtFrom.Text, txtTo.Text, "", "");
            progressBarCopy.Value = 5;
            CopySolidworksFile(txtFrom.Text, txtTo.Text);
            progressBarCopy.Value = 100;
            MessageBox.Show("操作完成");
            progressBarCopy.Value = 0;
        }

        private void CopySolidworksFile(string file_from, string file_to, string replease_from, string replact_to)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();
            string source = System.IO.Path.GetDirectoryName(file_from);
            string target = System.IO.Path.GetDirectoryName(file_to);
            string sourcefile = file_from; ;
            string tempfile = System.IO.Path.GetFileName(file_to);

            bool traverse = false;
            bool search = false;
            bool addreadonlyinfo = false;
            object[] depends = null;
            string[] sourcefiles = null;
            string[] targetfiles = null;
            int idx = 0;
            int sourcecount = 0;
            int copyopt = 0;
            int errors = 0;
            int lind = 0;
            ModelDoc2 doc = default(ModelDoc2);

            // sourcefile = file_from;

            tempfile = file_to;// System.IO.Path.GetDirectoryName(file_to);
            lind = tempfile.LastIndexOf("\\");
            target = tempfile.Substring(0, lind);

            traverse = true;
            search = true;
            addreadonlyinfo = false;

            depends = (object[])swApp.GetDocumentDependencies2(sourcefile, traverse, search, addreadonlyinfo);

            if ((depends == null))
                return;

            idx = 1;

            while (idx <= depends.GetUpperBound(0))
            {
                Array.Resize(ref sourcefiles, sourcecount + 1);
                Array.Resize(ref targetfiles, sourcecount + 1);

                sourcefiles[sourcecount] = (string)depends[idx];
                lind = sourcefiles[sourcecount].LastIndexOf("\\");
                targetfiles[sourcecount] = target + sourcefiles[sourcecount].Substring(lind, sourcefiles[sourcecount].Length - lind);

                sourcecount = sourcecount + 1;
                idx = idx + 2;
            }

            swApp.CloseAllDocuments(true);

            copyopt = (int)swMoveCopyOptions_e.swMoveCopyOptionsOverwriteExistingDocs;

            errors = swApp.CopyDocument(sourcefile, target + @"\" + System.IO.Path.GetFileName(sourcefile), (sourcefiles), (targetfiles), (int)copyopt);
        }

        /// <summary>
        /// 不打开文件,复制文件
        /// </summary>
        /// <param name="sourcefile">源文件路径</param>
        /// <param name="target">目标文件</param>
        /// <param name="ReplaceFrom">需要替换的文件</param>
        /// <param name="ReplaceTo">替换成什么</param>
        /// <param name="likeFile">是否模糊匹配</param>
        /// <param name="ReplaceFrom2">替换字符2</param>
        /// <param name="ReplaceTo2">第二次替换成什么</param>
        /// <param name="likeFile2">模糊匹配?</param>
        public void CopySolidworksFile(string sourcefile, string target, string ReplaceFrom = "", string ReplaceTo = "", bool likeFile = true, string ReplaceFrom2 = "", string ReplaceTo2 = "", bool likeFile2 = false)
        {
            SldWorks swApp = Utility.ConnectToSolidWorks();

            target = System.IO.Path.GetDirectoryName(target) + @"\";

            int sourcecount = 0;

            string[] sourcefiles = null;

            string[] targetfiles = null;

            object[] depends;

            var sourcefileName = System.IO.Path.GetFileName(sourcefile);
            var oldSourcefiles = sourcefiles;
            sourcefiles = new string[sourcecount + 1];
            if (oldSourcefiles != null)
                Array.Copy(oldSourcefiles, sourcefiles, Math.Min(sourcecount + 1, oldSourcefiles.Length));
            var oldTargetfiles = targetfiles;
            targetfiles = new string[sourcecount + 1];
            if (oldTargetfiles != null)
                Array.Copy(oldTargetfiles, targetfiles, Math.Min(sourcecount + 1, oldTargetfiles.Length));
            sourcefiles[sourcecount] = sourcefile;
            progressBarCopy.Value = 10;
            if (ReplaceFrom != "")
            {
                if (likeFile == true)
                    targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom, ReplaceTo).Replace(ReplaceFrom2, ReplaceTo2);
                else if (sourcefileName.ToUpper() == ReplaceFrom.ToUpper())
                    targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom, ReplaceTo).Replace(ReplaceFrom2, ReplaceTo2);
                else
                    targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom2, ReplaceTo2);
            }
            else
                targetfiles[sourcecount] = target + sourcefileName;

            if (FileSystem.Dir(targetfiles[sourcecount]) != "")
            {
                if (Interaction.MsgBox(targetfiles[sourcecount] + "已经存在,是否替换?", Constants.vbYesNo, "文件打包") == Constants.vbNo)
                {
                    progressBarCopy.Value = 0;
                    return;
                }
            }

            try
            {
                FileSystem.FileCopy(sourcefiles[sourcecount], targetfiles[sourcecount]);
            }
            catch (Exception ex)
            {
            }
            progressBarCopy.Value = 35;
            sourcecount = sourcecount + 1;

            if (FileSystem.Dir(Strings.Replace(sourcefile, ".SLDASM", ".SLDDRW")) != "")
            {
                oldSourcefiles = sourcefiles;
                sourcefiles = new string[sourcecount + 1];
                if (oldSourcefiles != null)
                    Array.Copy(oldSourcefiles, sourcefiles, Math.Min(sourcecount + 1, oldSourcefiles.Length));
                oldTargetfiles = targetfiles;
                targetfiles = new string[sourcecount + 1];
                if (oldTargetfiles != null)
                    Array.Copy(oldTargetfiles, targetfiles, Math.Min(sourcecount + 1, oldTargetfiles.Length));

                sourcefiles[sourcecount] = Strings.Replace(sourcefile, ".SLDASM", ".SLDDRW");
                sourcefileName = System.IO.Path.GetFileName(sourcefiles[sourcecount]);
                if (ReplaceFrom != "")
                {
                    if (likeFile == true)
                        targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom, ReplaceTo).Replace(ReplaceFrom2, ReplaceTo2);
                    else if (sourcefileName.ToUpper() == ReplaceFrom.ToUpper())
                        targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom, ReplaceTo).Replace(ReplaceFrom2, ReplaceTo2);
                    else
                        targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom2, ReplaceTo2);
                }
                else
                    targetfiles[sourcecount] = target + sourcefileName;

                if (FileSystem.Dir(targetfiles[sourcecount]) != "")
                {
                    if (Interaction.MsgBox(targetfiles[sourcecount] + "已经存在,是否替换?", Constants.vbYesNo, "文件打包") == Constants.vbNo)
                        return;
                }

                try
                {
                    FileSystem.FileCopy(sourcefiles[sourcecount], targetfiles[sourcecount]);
                }
                catch (Exception ex)
                {
                }
                sourcecount = sourcecount + 1;
            }
            progressBarCopy.Value = 50;
            depends = (string[])swApp.GetDocumentDependencies2(sourcefile, true, true, false);

            if (depends == null)
                return;
            bool bRet;
            var idx = 1;

            while (idx <= Information.UBound(depends))
            {
                oldSourcefiles = sourcefiles;
                sourcefiles = new string[sourcecount + 1];
                if (oldSourcefiles != null)
                    Array.Copy(oldSourcefiles, sourcefiles, Math.Min(sourcecount + 1, oldSourcefiles.Length));
                oldTargetfiles = targetfiles;
                targetfiles = new string[sourcecount + 1];
                if (oldTargetfiles != null)
                    Array.Copy(oldTargetfiles, targetfiles, Math.Min(sourcecount + 1, oldTargetfiles.Length));

                sourcefiles[sourcecount] = depends[idx].ToString();
                sourcefileName = System.IO.Path.GetFileName(depends[idx].ToString());
                if (ReplaceFrom != "")
                {
                    if (likeFile == true)
                        targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom, ReplaceTo).Replace(ReplaceFrom2, ReplaceTo2);
                    else if (sourcefileName.ToUpper() == ReplaceFrom.ToUpper())
                        targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom, ReplaceTo).Replace(ReplaceFrom2, ReplaceTo2);
                    else
                        targetfiles[sourcecount] = target + sourcefileName.Replace(ReplaceFrom2, ReplaceTo2);
                }
                else
                    targetfiles[sourcecount] = target + sourcefileName;

                try
                {
                    FileSystem.FileCopy(sourcefiles[sourcecount], targetfiles[sourcecount]);
                }
                catch (Exception ex)
                {
                }

                idx = idx + 2;
                sourcecount = sourcecount + 1;
            }

            // swApp.SendMsgToUser("Done")
            progressBarCopy.Value = 80;
            for (int n = 0; n <= sourcecount - 1; n++)
            {
                var NewName = targetfiles[n];

                var RefQ = swApp.GetDocumentDependenciesCount(NewName, 1, 1) / (double)2;

                // Debug.Print(NewName & "--->参考文件有  " & RefQ)

                if (RefQ > 0)
                {
                    var q = 0;
                    for (q = 0; q <= sourcecount - 1; q++)
                    {
                        if (n == q)
                        {
                        }
                        else
                            bRet = swApp.ReplaceReferencedDocument(targetfiles[n], System.IO.Path.GetFileName(sourcefiles[q]), targetfiles[q]);
                    }
                }
            }

            progressBarCopy.Value = 95;
        }
    }
}