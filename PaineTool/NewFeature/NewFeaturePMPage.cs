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

        private NewFeatureHandler handler = null;
        private ISldWorks iSwApp = null;
        private SwAddin userAddin = null;

        #region Property Manager Page Controls

        //Groups
        private IPropertyManagerPageGroup group1;

        private IPropertyManagerPageCheckbox checkbox1;
        private IPropertyManagerPageOption option1;
        private IPropertyManagerPageOption option2;
        private IPropertyManagerPageOption option3;
        private IPropertyManagerPageListbox list1;

        private IPropertyManagerPageSelectionbox selection1;
        public IPropertyManagerPageNumberbox numberSize;
        public IPropertyManagerPageCombobox combo1;

        //Control IDs
        // public const int group1ID = 0;

        public const int group2ID = 0;

        public const int selection1ID = 0;

        public const int num1ID = 9;
        public const int combo1ID = 10;

        #endregion Property Manager Page Controls

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

        protected void CreatePropertyManagerPage(bool isthisModify)
        {
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            handler = new NewFeatureHandler(userAddin, this, isthisModify);

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
        protected void AddControls()
        {
            short controlType = -1;
            short align = -1;
            int options = -1;

            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            group1 = (IPropertyManagerPageGroup)swPropertyPage.AddGroupBox(group2ID, "Please select a face", options);

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
                numberSize.Value = 50.0;
                numberSize.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 0.0, 100.0, 0.01, true);
            }

            //combo1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            combo1 = (IPropertyManagerPageCombobox)group1.AddControl(combo1ID, controlType, "Sample Combobox", align, options, "Combo list");
            if (combo1 != null)
            {
                string[] items = { "40x40", "60x60", "80x80" };
                combo1.AddItems(items);
                combo1.Height = 50;
            }
        }

        public void Show()
        {
            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }

        public void Show(MacroFeatureData featData, Object modelDoc)
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

        public static DispatchWrapper[] ObjectArrayToDispatchWrapper(IEnumerable<object> objects)

        {
            return objects.Select(o => new DispatchWrapper(o)).ToArray();
        }
    }
}