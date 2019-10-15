using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;

namespace PaineTool
{

    public class PMPHandler : IPropertyManagerPage2Handler9
    {
        ISldWorks iSwApp;
        SwAddin userAddin;
        UserPMPage ppage;

        public PMPHandler(SwAddin addin, UserPMPage page)
        {
            userAddin = addin;
            iSwApp = (ISldWorks)userAddin.SwApp;
            ppage = page;
        }

        //Implement these methods from the interface
        public void AfterClose()
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.
            int IndentSize;
            IndentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(IndentSize);
        }

        public void OnCheckboxCheck(int id, bool status)
        {

        }

        public void OnClose(int reason)
        {
            //This function must contain code, even if it does nothing, to prevent the
            //.NET runtime environment from doing garbage collection at the wrong time.
            int IndentSize;
            IndentSize = System.Diagnostics.Debug.IndentSize;
            System.Diagnostics.Debug.WriteLine(IndentSize);
        }

        public void OnComboboxEditChanged(int id, string text)
        {

        }

        public int OnActiveXControlCreated(int id, bool status)
        {
            return -1;
        }

        public void OnButtonPress(int id)
        {
            if (id == UserPMPage.buttonID1)        // Toggle the textbox control visibility state
            {
                if (((IPropertyManagerPageControl)ppage.textbox2).Visible == true)
                    ((IPropertyManagerPageControl)ppage.textbox2).Visible = false;
                else
                    ((IPropertyManagerPageControl)ppage.textbox2).Visible = true;
            }
            else if (id == UserPMPage.buttonID2)   // Toggle the textbox control enabled/disabled
            {
                if (((IPropertyManagerPageControl)ppage.textbox3).Enabled == true)
                    ((IPropertyManagerPageControl)ppage.textbox3).Enabled = false;
                else
                    ((IPropertyManagerPageControl)ppage.textbox3).Enabled = true;
            }
        }

        public void OnComboboxSelectionChanged(int id, int item)
        {

        }

        public void OnGroupCheck(int id, bool status)
        {

        }

        public void OnGroupExpand(int id, bool status)
        {

        }

        public bool OnHelp()
        {
            string helppath;
            System.Windows.Forms.Form helpForm = new System.Windows.Forms.Form();

            // Specify a url path or a path to a chm file
            helppath = "http://help.solidworks.com/2016/English/api/sldworksapiprogguide/Welcome.htm";
            //helppath = "C:\\Program Files\\SolidWorks Corp\\SOLIDWORKS (2)\\api\\apihelp.chm";

            System.Windows.Forms.Help.ShowHelp(helpForm, helppath);

            return true;
        }

        public void OnListboxSelectionChanged(int id, int item)
        {

        }

        public bool OnNextPage()
        {
            return true;
        }

        public void OnNumberboxChanged(int id, double val)
        {

        }

        public void OnNumberBoxTrackingCompleted(int id, double val)
        {

        }

        public void OnOptionCheck(int id)
        {

        }

        public bool OnPreviousPage()
        {
            return true;
        }

        public void OnSelectionboxCalloutCreated(int id)
        {

        }

        public void OnSelectionboxCalloutDestroyed(int id)
        {

        }

        public void OnSelectionboxFocusChanged(int id)
        {

        }

        public void OnSelectionboxListChanged(int id, int item)
        {
            // When a user selects entities to populate the selection box, display a popup cursor.
            ppage.swPropertyPage.SetCursor((int)swPropertyManagerPageCursors_e.swPropertyManagerPageCursors_Advance);
        }

        public void OnTextboxChanged(int id, string text)
        {

        }

        public void AfterActivation()
        {

        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return true;
        }

        public void OnPopupMenuItem(int Id)
        {

        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {

        }

        public bool OnPreview()
        {
            return true;
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {

        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {

        }

        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            return true;
        }

        public bool OnTabClicked(int Id)
        {
            return true;
        }

        public void OnUndo()
        {

        }

        public void OnWhatsNew()
        {

        }


        public void OnGainedFocus(int Id)
        {

        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {

        }

        public void OnLostFocus(int Id)
        {

        }

        public void OnRedo()
        {

        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return 0;
        }


    }
}
