using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sona.Compiler.Gui
{
    static class ControlExtensions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        const int WM_SETREDRAW = 11;

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

        public static void SetPadding(this TextBoxBase textBox, int left, int top, int right, int bottom)
        {
            var rect = GetRect(textBox);
            var newRect = new Rectangle(left, top, rect.Width - left - right, rect.Height - top - bottom);
            SetRect(textBox, newRect);
        }

        struct RECT
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;

            private RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
            {

            }
        }

        [DllImport(@"user32.dll")]
        static extern int SendMessage(IntPtr hwnd, int Msg, IntPtr wParam, ref RECT lParam);

        const int EM_GETRECT = 0xB2;
        const int EM_SETRECT = 0xB3;

        static void SetRect(TextBoxBase textbox, Rectangle rect)
        {
            var r = new RECT(rect);
            SendMessage(textbox.Handle, EM_SETRECT, IntPtr.Zero, ref r);
        }

        static Rectangle GetRect(TextBoxBase textbox)
        {
            var r = new RECT();
            SendMessage(textbox.Handle, EM_GETRECT, IntPtr.Zero, ref r);
            return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
        }

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        const int SB_HORZ = 0;
        const int SB_VERT = 1;

        const int WM_HSCROLL = 0x114;
        const int WM_VSCROLL = 0x115;
        const int SB_THUMBPOSITION = 4;

        public static Point GetScrollOffset(this TextBoxBase textbox)
        {
            var handle = textbox.Handle;
            return new(GetScrollPos(handle, SB_HORZ), GetScrollPos(handle, SB_VERT));
        }

        public static void SetScrollOffset(this TextBoxBase textbox, Point offset)
        {
            var handle = textbox.Handle;
            SetScrollPos(handle, SB_HORZ, offset.X, false);
            SendMessage(handle, WM_HSCROLL, SB_THUMBPOSITION | (offset.X << 16), IntPtr.Zero);
            SetScrollPos(handle, SB_VERT, offset.Y, false);
            SendMessage(handle, WM_VSCROLL, SB_THUMBPOSITION | (offset.Y << 16), IntPtr.Zero);
        }
    }
}
