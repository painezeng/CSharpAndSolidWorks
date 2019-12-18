using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

using WINDOWINFO = CSharpAndSolidWorks.tagWINDOWINFO;

namespace CSharpAndSolidWorks
{
    internal class GetWindowsSize
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        public struct windsize
        {
            public double _width;
            public double _height;
        }

        public windsize GetSize(string ProcessName)
        {
            windsize myWinSize = new windsize();
            Process[] ps = Process.GetProcessesByName(ProcessName);
            foreach (var p in ps)
            {
                IntPtr handle = p.MainWindowHandle;
                WINDOWINFO info = new WINDOWINFO();
                info.cbSize = (uint)Marshal.SizeOf(info);
                GetWindowInfo(handle, ref info);

                myWinSize._width = info.rcWindow.right - info.rcWindow.left;
                myWinSize._height = info.rcWindow.bottom - info.rcWindow.top;

                // return info;
                //  MessageBox.Show(info.rcWindow.left.ToString() + ":" + info.rcWindow.top.ToString());
            }
            return myWinSize;
        }
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct tagWINDOWINFO
    {
        /// DWORD->unsigned int
        public uint cbSize;

        /// RECT->tagRECT
        public tagRECT rcWindow;

        /// RECT->tagRECT
        public tagRECT rcClient;

        /// DWORD->unsigned int
        public uint dwStyle;

        /// DWORD->unsigned int
        public uint dwExStyle;

        /// DWORD->unsigned int
        public uint dwWindowStatus;

        /// UINT->unsigned int
        public uint cxWindowBorders;

        /// UINT->unsigned int
        public uint cyWindowBorders;

        /// ATOM->WORD->unsigned short
        public ushort atomWindowType;

        /// WORD->unsigned short
        public ushort wCreatorVersion;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct tagRECT
    {
        /// LONG->int
        public int left;

        /// LONG->int
        public int top;

        /// LONG->int
        public int right;

        /// LONG->int
        public int bottom;
    }
}