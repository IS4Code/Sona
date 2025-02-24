using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IS4.Sona.Compiler.Gui
{
    static class ControlExtensions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(this Control control)
        {
            try
            {
                SendMessage(control.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
            }
            catch
            {

            }
        }

        public static void ResumeDrawing(this Control control)
        {
            try
            {
                SendMessage(control.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            }
            catch
            {
                return;
            }
            control.Invalidate(true);
        }
    }
}
