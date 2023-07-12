using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System.Diagnostics;

namespace GetRayIntersectionWithBody
{
    /// <summary>
    /// 箭头事件
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class SwManHandler2 : SwManipulatorHandler2
    {
        private int doneonce;

        private const int lenFact = 1;

        private bool SwManipulatorHandler2_OnHandleLmbSelected(object pManipulator)
        {
            Debug.Print("SwManipulatorHandler2_OnHandleLmbSelected");
            return true;
        }

        bool ISwManipulatorHandler2.OnHandleLmbSelected(object pManipulator)
        {
            return SwManipulatorHandler2_OnHandleLmbSelected(pManipulator);
        }

        private bool SwManipulatorHandler2_OnDelete(object pManipulator)
        {
            Debug.Print("SwManipulatorHandler2_OnDelete");
            return true;
        }

        bool ISwManipulatorHandler2.OnDelete(object pManipulator)
        {
            return SwManipulatorHandler2_OnDelete(pManipulator);
        }

        private void SwManipulatorHandler2_OnDirectionFlipped(object pManipulator)
        {
            Debug.Print("SwManipulatorHandler2_OnDirectionFlipped");
        }

        void ISwManipulatorHandler2.OnDirectionFlipped(object pManipulator)
        {
            SwManipulatorHandler2_OnDirectionFlipped(pManipulator);
        }

        private bool SwManipulatorHandler2_OnDoubleValueChanged(object pManipulator, int Id, ref double Value)
        {
            doneonce = doneonce + 1;
            Debug.Print("SwManipulatorHandler2_OnDoubleValueChanged");

            Debug.Print("  ID               = " + Id);

            Debug.Print("  Value            = " + Value);
            DragArrowManipulator swTmpManipulator = default(DragArrowManipulator);
            swTmpManipulator = (DragArrowManipulator)pManipulator;
            //Update origin
            MathPoint swMathPoint = default(MathPoint);
            swMathPoint = swTmpManipulator.Origin;
            double[] varMathPt = null;
            varMathPt = (double[])swMathPoint.ArrayData;
            varMathPt[1] = varMathPt[1] + lenFact / 1000;
            swMathPoint.ArrayData = varMathPt;
            if ((doneonce == 1))
            {
                swTmpManipulator.FixedLength = true;
            }
            swTmpManipulator.Origin = swMathPoint;

            swTmpManipulator.Update();
            return true;
        }

        bool ISwManipulatorHandler2.OnDoubleValueChanged(object pManipulator, int Id, ref double Value)
        {
            return SwManipulatorHandler2_OnDoubleValueChanged(pManipulator, Id, ref Value);
        }

        private void SwManipulatorHandler2_OnEndDrag(object pManipulator, int handleIndex)
        {
            DragArrowManipulator swTmpManipulator = default(DragArrowManipulator);
            swTmpManipulator = (DragArrowManipulator)pManipulator;
            swTmpManipulator.FixedLength = false;
            doneonce = doneonce + 1;
            Debug.Print("SwManipulatorHandler2_OnEndDrag");

            Debug.Print("  HandleIndex      = " + handleIndex);

            if ((handleIndex == (int)swDragArrowManipulatorOptions_e.swDragArrowManipulatorDirection2))
            {
                Debug.Print(" Direction1");
            }
            else
            {
                Debug.Print(" Direction2");
            }
        }

        void ISwManipulatorHandler2.OnEndDrag(object pManipulator, int handleIndex)
        {
            SwManipulatorHandler2_OnEndDrag(pManipulator, handleIndex);
        }

        private void SwManipulatorHandler2_OnEndNoDrag(object pManipulator, int handleIndex)
        {
            Debug.Print("SwManipulatorHandler2_OnEndNoDrag");
        }

        void ISwManipulatorHandler2.OnEndNoDrag(object pManipulator, int handleIndex)
        {
            SwManipulatorHandler2_OnEndNoDrag(pManipulator, handleIndex);
        }

        private void SwManipulatorHandler2_OnHandleRmbSelected(object pManipulator, int handleIndex)
        {
            Debug.Print("SwManipulatorHandler2_OnHandleRmbSelected");

            Debug.Print("  handleIndex      = " + handleIndex);
        }

        void ISwManipulatorHandler2.OnHandleRmbSelected(object pManipulator, int handleIndex)
        {
            SwManipulatorHandler2_OnHandleRmbSelected(pManipulator, handleIndex);
        }

        private void SwManipulatorHandler2_OnHandleSelected(object pManipulator, int handleIndex)
        {
            Debug.Print("SwManipulatorHandler2_OnHandleSelected");

            Debug.Print("  HandleIndex      = " + handleIndex);
        }

        void ISwManipulatorHandler2.OnHandleSelected(object pManipulator, int handleIndex)
        {
            SwManipulatorHandler2_OnHandleSelected(pManipulator, handleIndex);
        }

        private void SwManipulatorHandler2_OnItemSetFocus(object pManipulator, int Id)
        {
            Debug.Print("SwManipulatorHandler2_OnItemSetFocus");

            Debug.Print("  ID               = " + Id);
        }

        void ISwManipulatorHandler2.OnItemSetFocus(object pManipulator, int Id)
        {
            SwManipulatorHandler2_OnItemSetFocus(pManipulator, Id);
        }

        private bool SwManipulatorHandler2_OnStringValueChanged(object pManipulator, int Id, ref string Value)
        {
            Debug.Print("SwManipulatorHandler2_OnStringValueChanged");

            Debug.Print("  ID               = " + Id);

            Debug.Print("  Value            = " + Value);
            return true;
        }

        bool ISwManipulatorHandler2.OnStringValueChanged(object pManipulator, int Id, ref string Value)
        {
            return SwManipulatorHandler2_OnStringValueChanged(pManipulator, Id, ref Value);
        }

        private void SwManipulatorHandler2_OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
            Debug.Print("SwManipulatorHandler2_OnUpdateDrag");

            Debug.Print("  HandleIndex      = " + handleIndex);
        }

        void ISwManipulatorHandler2.OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
            SwManipulatorHandler2_OnUpdateDrag(pManipulator, handleIndex, newPosMathPt);
        }
    }
}