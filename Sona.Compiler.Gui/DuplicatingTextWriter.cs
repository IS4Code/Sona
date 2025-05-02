using System.IO;
using System.Text;

namespace Sona.Compiler.Gui
{
    sealed class DuplicatingTextWriter : TextWriter
    {
        readonly TextWriter first, second;

        public override Encoding Encoding => first.Encoding;

        public DuplicatingTextWriter(TextWriter first, TextWriter second)
        {
            this.first = first;
            this.second = second;
        }

        public override void Write(char value)
        {
            first.Write(value);
            second.Write(value);
        }

        public override void Write(char[]? buffer)
        {
            first.Write(buffer);
            second.Write(buffer);
        }

        public override void Write(string? value)
        {
            first.Write(value);
            second.Write(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            first.Write(buffer, index, count);
            second.Write(buffer, index, count);
        }

        public override void WriteLine()
        {
            first.WriteLine();
            second.WriteLine();
        }
    }
}
