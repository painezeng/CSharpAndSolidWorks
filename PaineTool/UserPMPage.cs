using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace PaineTool
{
    public class UserPMPage
    {
        //Local Objects
        public IPropertyManagerPage2 swPropertyPage = null;

        private PMPHandler handler = null;
        private ISldWorks iSwApp = null;
        private SwAddin userAddin = null;
        private IPropertyManagerPageTab ppagetab1 = null;
        private IPropertyManagerPageTab ppagetab2 = null;

        #region Property Manager Page Controls

        //Groups
        private IPropertyManagerPageGroup group1;

        private IPropertyManagerPageGroup group2;

        //Controls
        private IPropertyManagerPageTextbox textbox1;

        private IPropertyManagerPageCheckbox checkbox1;
        private IPropertyManagerPageOption option1;
        private IPropertyManagerPageOption option2;
        private IPropertyManagerPageOption option3;
        private IPropertyManagerPageListbox list1;

        private IPropertyManagerPageSelectionbox selection1;
        private IPropertyManagerPageNumberbox num1;
        private IPropertyManagerPageCombobox combo1;

        private IPropertyManagerPageButton button1;
        private IPropertyManagerPageButton button2;
        public IPropertyManagerPageTextbox textbox2;
        public IPropertyManagerPageTextbox textbox3;

        //Control IDs
        public const int group1ID = 0;

        public const int group2ID = 1;

        public const int textbox1ID = 2;
        public const int checkbox1ID = 3;
        public const int option1ID = 4;
        public const int option2ID = 5;
        public const int option3ID = 6;
        public const int list1ID = 7;

        public const int selection1ID = 8;
        public const int num1ID = 9;
        public const int combo1ID = 10;
        public const int tabID1 = 11;
        public const int tabID2 = 12;
        public const int buttonID1 = 13;
        public const int buttonID2 = 14;
        public const int textbox2ID = 15;
        public const int textbox3ID = 16;

        #endregion Property Manager Page Controls

        public UserPMPage(SwAddin addin)
        {
            userAddin = addin;
            if (userAddin != null)
            {
                iSwApp = (ISldWorks)userAddin.SwApp;
                CreateUserPMP();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("SwAddin not set.");
            }
        }

        private void CreateUserPMP()
        {
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            handler = new PMPHandler(userAddin, this);
            swPropertyPage = (IPropertyManagerPage2)iSwApp.CreatePropertyManagerPage("Sample PMP", options, handler, ref errors);
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
            bool retval;

            //Add Message
            retval = swPropertyPage.SetMessage3("This is a sample message, marked yellow to signify importance.",
                                            (int)swPropertyManagerPageMessageVisibility.swImportantMessageBox,
                                            (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                            "Sample Important Caption");

            // Add PropertyManager Page Tabs
            ppagetab1 = swPropertyPage.AddTab(tabID1, "Page Tab 1", "", 0);
            ppagetab2 = swPropertyPage.AddTab(tabID2, "Page Tab 2", "", 0);

            //Add the groups
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            group1 = (IPropertyManagerPageGroup)ppagetab1.AddGroupBox(group1ID, "Sample Group 1", options);

            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            group2 = (IPropertyManagerPageGroup)ppagetab1.AddGroupBox(group2ID, "Sample Group 2", options);

            //Add the controls to group1

            //textbox1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            textbox1 = (IPropertyManagerPageTextbox)group1.AddControl(textbox1ID, controlType, "Type Here", align, options, "This is an example textbox");

            //checkbox1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            checkbox1 = (IPropertyManagerPageCheckbox)group1.AddControl(checkbox1ID, controlType, "Sample Checkbox", align, options, "This is a sample checkbox");

            //option1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Option;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            option1 = (IPropertyManagerPageOption)group1.AddControl(option1ID, controlType, "Option1", align, options, "Radio Buttons");

            //option2
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Option;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            option2 = (IPropertyManagerPageOption)group1.AddControl(option2ID, controlType, "Option2", align, options, "Radio Buttons");

            //option3
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Option;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            option3 = (IPropertyManagerPageOption)group1.AddControl(option3ID, controlType, "Option3", align, options, "Radio Buttons");

            //list1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Listbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            list1 = (IPropertyManagerPageListbox)group1.AddControl(list1ID, controlType, "Sample Listbox", align, options, "List of selectable items");
            if (list1 != null)
            {
                string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                list1.Height = 50;
                list1.AddItems(items);
            }

            //Add controls to group2
            //selection1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            selection1 = (IPropertyManagerPageSelectionbox)group2.AddControl(selection1ID, controlType, "Sample Selection", align, options, "Displays features selected in main view");
            if (selection1 != null)
            {
                int[] filter = { (int)swSelectType_e.swSelEDGES, (int)swSelectType_e.swSelVERTICES };
                selection1.Height = 40;
                selection1.SetSelectionFilters(filter);
            }

            //num1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Numberbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            num1 = (IPropertyManagerPageNumberbox)group2.AddControl(num1ID, controlType, "Sample Numberbox", align, options, "Allows for numerical input");
            if (num1 != null)
            {
                num1.Value = 50.0;
                num1.SetRange((int)swNumberboxUnitType_e.swNumberBox_UnitlessDouble, 0.0, 100.0, 0.01, true);
            }

            //combo1
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            combo1 = (IPropertyManagerPageCombobox)group2.AddControl(combo1ID, controlType, "Sample Combobox", align, options, "Combo list");
            if (combo1 != null)
            {
                string[] items = { "One Fish", "Two Fish", "Red Fish", "Blue Fish" };
                combo1.AddItems(items);
                combo1.Height = 50;
            }

            // Button
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Button;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;

            button1 = (IPropertyManagerPageButton)group2.AddControl2(buttonID1, controlType, "Hide", align, options, "Change the visibility of the control");

            // Textbox2
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;

            textbox2 = (IPropertyManagerPageTextbox)group2.AddControl2(textbox2ID, controlType, "Sample Textbox", align, options, "Sample Textbox text");

            // Button
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Button;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;

            button2 = (IPropertyManagerPageButton)group2.AddControl2(buttonID2, controlType, "Disable", align, options, "Disable the control");

            // Textbox3
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                      (int)swAddControlOptions_e.swControlOptions_Visible;

            textbox3 = (IPropertyManagerPageTextbox)group2.AddControl2(textbox3ID, controlType, "Another sample Textbox", align, options, "Second Sample Textbox text");
        }

        public void Show()
        {
            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }
    }
}