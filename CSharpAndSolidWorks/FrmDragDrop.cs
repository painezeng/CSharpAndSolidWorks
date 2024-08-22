using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CSharpAndSolidWorks
{
    public partial class FrmDragDrop : Form
    {
        public FrmDragDrop()
        {
            InitializeComponent();
        }

        private void FrmDragDrop_Load(object sender, EventArgs e)
        {

            listBoxFiles.Items.Clear();
            var defaultPath = $@"{FormMain. RegDllPath("")}\TemplateModel";
            var allfile = System.IO.Directory.GetFiles(defaultPath, "*.*", SearchOption.AllDirectories);

            for (int i = 0; i < allfile.Length; i++)
            {
                listBoxFiles.Items.Add(allfile[i]);
            }

        }

        private string fName;
        private void listBoxFiles_MouseDown(object sender, MouseEventArgs e)
        {
            fName = listBoxFiles.Items[listBoxFiles.SelectedIndex].ToString();
            string[] fList = new string[1];
            fList[0] = fName;

            DataObject dataObj = new DataObject(DataFormats.FileDrop, fList);

            DragDropEffects eff = DoDragDrop(dataObj, DragDropEffects.Link | DragDropEffects.Copy);
        }
    }
}
