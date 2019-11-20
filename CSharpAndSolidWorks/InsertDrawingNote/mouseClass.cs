using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpAndSolidWorks
{
    public class mouseClass
    {
        private FrmNote _frmNote;

        public mouseClass(FrmNote frmNote)
        {
            _frmNote = frmNote;
        }

        public int ms_MouseSelectNotify(int ix, int iy, double X, double Y, double Z)
        {
            try
            {
                Debug.Print("Mouse loc ix = " + ix + " iy = " + iy + " x= " + X);

                SldWorks swApp;
                swApp = Comm.ConnectToSolidWorks();

                bool boolstatus;
                long longstatus;

                ModelDoc2 swModel;
                swModel = swApp.ActiveDoc;

                Note myNote;
                Annotation myAnnotation;

                myNote = swModel.InsertNote(_frmNote.activeNote);
                if (myNote != null)
                {
                    myNote.Angle = 0;
                    boolstatus = myNote.SetBalloon(0, 0);
                    myAnnotation = myNote.GetAnnotation();
                    if (myAnnotation != null)
                    {
                        longstatus = myAnnotation.SetLeader3((int)swLeaderStyle_e.swSTRAIGHT, 0, true, false, false, false);

                        boolstatus = myAnnotation.SetPosition(X + FrmNote.x / 1000, Y + FrmNote.y / 1000, Z);

                        TextFormat txtFormat = default(TextFormat);

                        txtFormat = myAnnotation.GetTextFormat(0);

                        txtFormat.Bold = true;
                        txtFormat.CharHeight = 0.01;

                        boolstatus = myAnnotation.SetTextFormat(0, false, txtFormat);
                    }
                }

                swModel.ClearSelection2(true);
                swModel.WindowRedraw();

                _frmNote.NextNote();

                if (FrmNote.haveNextNote == false)
                {
                    FrmNote.TheMouse.MouseSelectNotify -= ms_MouseSelectNotify;
                }
                else
                {
                    Frame swFrame = swApp.Frame();

                    swFrame.SetStatusBarText("Next Click  to insert " + _frmNote.activeNote);
                }

                return 1;
            }
            catch (Exception)
            {
                MessageBox.Show("Error!");

                return 0;
            }
        }
    }
}