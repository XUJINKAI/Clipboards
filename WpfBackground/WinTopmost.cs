using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfBackground
{
    public static class WinTopmost
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOPMOST = 0x0008;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        public static bool GetTopmostStatus(IntPtr? hWnd = null)
        {
            IntPtr handle = hWnd ?? GetForegroundWindow();
            int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
            return (exStyle & WS_EX_TOPMOST) == WS_EX_TOPMOST;
        }

        public static void ToggleTopmost(IntPtr? hWnd = null, bool? SetOrToggle = null)
        {
            IntPtr handle = hWnd ?? GetForegroundWindow();
            bool topmost = SetOrToggle ?? !GetTopmostStatus();
            IntPtr hWndInsertAfter = topmost ? HWND_TOPMOST : HWND_NOTOPMOST;
            SetWindowPos(handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

    }
}
