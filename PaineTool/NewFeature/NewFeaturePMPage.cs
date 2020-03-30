using Microsoft.VisualBasic.CompilerServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PaineTool.NewFeature
{
    public class NewFeaturePMPage
    {
        //Local Objects
        private IPropertyManagerPage2 swPropertyPage = null;

        public NewFeatureHandler handler = null;
        private ISldWorks iSwApp = null;
        private SwAddin userAddin = null;

        #region Property Manager Page Controls

        //Groups
        private IPropertyManagerPageGroup group1;

        private IPropertyManagerPageGroup group2;

        private IPropertyManagerPageCheckbox checkbox1;
        private IPropertyManagerPageOption option1;
        private IPropertyManagerPageOption option2;
        private IPropertyManagerPageOption option3;
        private IPropertyManagerPageListbox list1;

        private IPropertyManagerPageSelectionbox selection1;
        public IPropertyManagerPageNumberbox numberSize;
        public IPropertyManagerPageCombobox combo1;

        public PropertyManagerPageWindowFromHandle pm_MyPMPControl;

        private string sizeValue;

        public string SizeValue
        {
            get { return sizeValue; }
            set
            {
                sizeValue = value;
            }
        }

        //Control IDs
        // public const int group1ID = 0;

        public const int group1ID = 0;
        public const int group2ID = 1;

        public const int selection1ID = 0;

        public const int num1ID = 9;
        public const int combo1ID = 10;

        #endregion Property Manager Page Controls

        /// <summary>
        /// 此页面的构造函数
        /// </summary>
        /// <param name="addin"></param>
        /// <param name="isModify"></param>
        public NewFeaturePMPage(SwAddin addin, bool isModify)
        {
            userAddin = addin;

            if (userAddin != null)
            {
                iSwApp = (ISldWorks)userAddin.SwApp;
                CreatePropertyManagerPage(isModify);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("SwAddin not set.");
            }
        }

        /// <summary>
        /// 创建页面 主函数
        /// </summary>
        /// <param name="isthisModify"></param>
        protected void CreatePropertyManagerPage(bool isthisModify)
        {
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            handler = new NewFeatureHandler(userAddin, this);

            swPropertyPage = (IPropertyManagerPage2)iSwApp.CreatePropertyManagerPage("MyNewFeature", options, handler, ref errors);

            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                try
                {
                    AddControls();
                }
                catch (Exception e)
                {
                    iSwApp.SendMsgToUser2(e.Message, 0, 0);
                }
            }
        }

        //Controls are displayed on the page top to bottom in the order
        //in which they are added to the object.
        /// <summary>
        /// 增加所有控件到页面中
        /// </summary>
        protected void AddControls()
        {
            short controlType = -1;
            short align = -1;
            int options = -1;

            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            group1 = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(group1ID, "Please select a face", options);
            group2 = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(group2ID, "", options);

            //Add controls to group2
            //selection1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            selection1 = (IPropertyManagerPageSelectionbox)group1.AddControl(selection1ID, controlType, "Sample Selection", align, options, "Displays features selected in main view");
            if (selection1 != null)
            {
                int[] filter = { (int)swSelectType_e.swSelFACES };
                selection1.Height = 40;
                selection1.SetSelectionFilters(filter);
            }

            //num1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Numberbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            numberSize = (IPropertyManagerPageNumberbox)group1.AddControl(num1ID, controlType, "Sample Numberbox", align, options, "Allows for numerical input");
            if (numberSize != null)
            {
                numberSize.Value = 60;
                //输入框 定义 最小值 最大值 ,以及增量
                numberSize.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 30, 100, 10, true);
            }

            controlType = (int)swPropertyManagerPageControlType_e.swControlType_WindowFromHandle;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            pm_MyPMPControl = (PropertyManagerPageWindowFromHandle)group2.AddControl(90, controlType, "1122", align, options, "333444");
            pm_MyPMPControl.Height = 90;
            //pm_MyPMPControl.Height = 90;

            //if (b == false)
            //{
            //    MessageBox.Show("Failed to add User control!");
            //}

            ////combo1
            //controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
            //align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            //options = (int)swAddControlOptions_e.swControlOptions_Enabled |
            //          (int)swAddControlOptions_e.swControlOptions_Visible;

            //combo1 = (IPropertyManagerPageCombobox)group1.AddControl(combo1ID, controlType, "Sample Combobox", align, options, "Combo list");
            //if (combo1 != null)
            //{
            //    string[] items = { "40x40", "60x60", "80x80" };
            //    combo1.AddItems(items);
            //    combo1.Height = 50;
            //}
        }

        /// <summary>
        /// 显示页面
        /// </summary>
        public void Show()
        {
            MyPMPControl myPMPControl = new MyPMPControl(this);
            // myPMPControl.Visible = true;
            //myPMPControl.Show();

            var b = pm_MyPMPControl.SetWindowHandlex64(myPMPControl.Handle.ToInt64());

            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }

        /// <summary>
        /// 这里是修改时,带特征数据来填充页面,然后显示PMP
        /// </summary>
        /// <param name="featData"></param>
        /// <param name="modelDoc"></param>
        /// <param name="fea"></param>
        public void Show(MacroFeatureData featData, Object modelDoc, Feature fea)
        {
            object retParamNames = null;
            object retParamValues = null;
            object paramTypes = null;
            object retSelObj;
            object selObjType;
            object selMarks;
            object selDrViews;
            object compXforms;

            featData.AccessSelections(modelDoc, null);
            featData.GetParameters(out retParamNames, out paramTypes, out retParamValues);

            featData.GetSelections3(out retSelObj, out selObjType, out selMarks, out selDrViews, out compXforms);

            var objectsArray = (object[])retSelObj;

            var typesArray = (swSelectType_e[])selObjType;

            var marksArray = (int[])selMarks;

            var swmodel = (ModelDoc2)iSwApp.ActiveDoc;
            var swSelMgr = (SelectionMgr)swmodel.SelectionManager;

            var selections = objectsArray.Select((o, i) =>

                new
                {
                    o,

                    t = typesArray[i],

                    m = marksArray[i]
                }).ToList();

            var selectionsByMark = selections.GroupBy(s => s.m);

            foreach (var s in selectionsByMark)

            {
                var selectionData = swSelMgr.CreateSelectData();

                selectionData.Mark = s.Key;

                var array = s.Select(o => o.o).ToArray();

                var count = swmodel.Extension.MultiSelect2(ObjectArrayToDispatchWrapper(array), true, selectionData);

                if (array.Length != count)

                {
                    MessageBox.Show("Unable to select objects");
                }
            }

            string[] Pvs = (string[])retParamValues;

            numberSize.Value = double.Parse(Pvs[0]);

            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }

        /// <summary>
        /// 这个是转换对象的
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static DispatchWrapper[] ObjectArrayToDispatchWrapper(IEnumerable<object> objects)

        {
            return objects.Select(o => new DispatchWrapper(o)).ToArray();
        }

        public void showMyControl()
        {
            //NewLateBinding.LateSetComplex(this.pm_MyPMPControl, null, "visible", new object[]
            //    {
            //        true
            //    }, null, null, false, true);
        }
    }
}