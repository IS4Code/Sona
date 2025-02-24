using System.Windows.Forms;

namespace IS4.Sona.Compiler.Gui
{
    internal class DoubleBufferedRichTextBox : RichTextBox
    {
        public DoubleBufferedRichTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
