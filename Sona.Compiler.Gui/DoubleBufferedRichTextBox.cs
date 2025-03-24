using System.Windows.Forms;

namespace Sona.Compiler.Gui
{
    internal class DoubleBufferedRichTextBox : RichTextBox
    {
        public DoubleBufferedRichTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
