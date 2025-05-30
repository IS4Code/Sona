﻿using System;
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
    }
}
