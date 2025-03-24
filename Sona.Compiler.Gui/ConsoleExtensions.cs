using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Sona.Compiler.Gui
{
    static class ConsoleExtensions
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        const uint SC_CLOSE = 0xF060;
        const uint MF_GRAYED = 0x00000001;

        static bool consoleAllocated;
        static ConsoleColor textColor;
        static ConsoleColor backColor;

        public static void ShowConsole()
        {
            if(!consoleAllocated)
            {
                if(AllocConsole() == 0)
                {
                    throw new Win32Exception();
                }

                var encoding = Encoding.GetEncoding(CultureInfo.InstalledUICulture.TextInfo.OEMCodePage);
                Console.SetOut(OpenConsoleWriter(encoding));
                Console.SetError(OpenConsoleWriter(encoding));
                Console.SetIn(OpenConsoleReader(encoding));

                textColor = Console.ForegroundColor;
                backColor = Console.BackgroundColor;
                consoleAllocated = true;
            }

            Console.Clear();
            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backColor;
            ToggleConsole(SW_SHOW);
        }

        public static void HideConsole()
        {
            ToggleConsole(SW_HIDE);
            Console.Clear();
        }

        static void ToggleConsole(int flag)
        {
            var window = GetConsoleWindow();
            if(window == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            ShowWindow(window, flag);
            if(flag == SW_SHOW)
            {
                var menu = GetSystemMenu(window, false);
                EnableMenuItem(menu, SC_CLOSE, MF_GRAYED);

                SetForegroundWindow(window);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            uint lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile);

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ_WRITE = 0x3;
        const uint OPEN_EXISTING = 0x3;

        static StreamWriter OpenConsoleWriter(Encoding encoding)
        {
            var handle = new SafeFileHandle(CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_READ_WRITE, 0, OPEN_EXISTING, 0, 0), true);
            var stream = new HandleStream(handle, FileAccess.Write);

            return new StreamWriter(stream, encoding)
            {
                AutoFlush = true
            };
        }

        static StreamReader OpenConsoleReader(Encoding encoding)
        {
            var handle = new SafeFileHandle(CreateFile("CONIN$", GENERIC_READ, FILE_SHARE_READ_WRITE, 0, OPEN_EXISTING, 0, 0), true);
            var stream = new HandleStream(handle, FileAccess.Read);

            return new StreamReader(stream, encoding);
        }

        sealed unsafe class HandleStream : Stream
        {
            SafeFileHandle? handle;

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool WriteFile(SafeFileHandle hFile, byte* lpBuffer, int nNumberOfBytesToWrite, IntPtr lpNumberOfBytesWritten, IntPtr lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool ReadFile(SafeFileHandle hFile, byte* lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);

            public HandleStream(SafeFileHandle handle, FileAccess access)
            {
                this.handle = handle;
                CanRead = access is FileAccess.Read or FileAccess.ReadWrite;
                CanWrite = access is FileAccess.Write or FileAccess.ReadWrite;
            }

            public override bool CanRead { get; }

            public override bool CanSeek => false;

            public override bool CanWrite { get; }

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override void Flush()
            {

            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                var span = buffer.AsSpan(offset, count);
                fixed(byte* ptr = span)
                {
                    if(!WriteFile(handle ?? throw new ObjectDisposedException(typeof(SafeFileHandle).ToString()), ptr, span.Length, IntPtr.Zero, IntPtr.Zero))
                    {
                        throw new IOException(null, new Win32Exception());
                    }
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var span = buffer.AsSpan(offset, count);
                fixed(byte* ptr = span)
                {
                    if(!ReadFile(handle ?? throw new ObjectDisposedException(typeof(SafeFileHandle).ToString()), ptr, span.Length, out var read, IntPtr.Zero))
                    {
                        throw new IOException(null, new Win32Exception());
                    }
                    return read;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if(disposing)
                {
                    if(handle != null)
                    {
                        handle.Dispose();
                        handle = null;
                    }
                }

                base.Dispose(disposing);
            }
        }
    }
}
