using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Forms;
using Antlr4.Runtime;

namespace IS4.Sona.Compiler.Gui
{
    public partial class MainForm : Form
    {
        readonly Channel<string> codeChannel;
        readonly SonaCompiler compiler;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            var font = messageBox.Font;

            const float zoom = 1.5f;

            messageBox.Font = new Font(
                font.FontFamily,
                font.SizeInPoints * zoom,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont
            );

            sonaText.Font = new Font(
                FontFamily.GenericMonospace,
                font.SizeInPoints * zoom,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont
            );

            resultText.BackColor = sonaText.BackColor;
            resultText.ForeColor = sonaText.ForeColor;
            resultText.Font = sonaText.Font;

            codeSplit.SplitterWidth *= 2;

            codeChannel = Channel.CreateUnbounded<string>(new()
            {
                SingleReader = true,
                SingleWriter = true,
                // Reading will be done from a thread anyway
                AllowSynchronousContinuations = true
            });

            compiler = new SonaCompiler()
            {
                AdjustLines = false
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            new Thread(UpdateText)
            {
                // Run compiler as a background thread in case of freeze
                IsBackground = true
            }.Start();
        }

        int? moveAfterSpaces;

        private async void sonaText_TextChanged(object sender, EventArgs e)
        {
            if(moveAfterSpaces is int move)
            {
                // Text changed after newline was inserted
                moveAfterSpaces = null;
                sonaText.SelectionStart += move;
            }

            // Send the text to channel
            await codeChannel.Writer.WriteAsync(sonaText.Text);
        }

        private void sonaText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.Tab)
            {
                // Ctrl-Tab is non-input
                e.IsInputKey = !e.Control;
            }
        }

        static readonly Regex lastLineSpaces = new Regex(@"^( *).*(?-m)$", RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Compiled);

        private void sonaText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                var oldText = sonaText.Text;

                int position = sonaText.SelectionStart;
                // Find spaces at the last line
                var spaces = lastLineSpaces.Match(oldText, 0, position).Groups[1].Value;

                sonaText.Text = String.Concat(
                    oldText.AsSpan(0, position),
                    spaces.AsSpan(),
                    oldText.AsSpan(position + sonaText.SelectionLength)
                );
                sonaText.SelectionStart = position;
                // Move on TextChanged
                moveAfterSpaces = spaces.Length;
            }
        }

        private void UpdateText()
        {
            var reader = codeChannel.Reader;
            var stack = new Stack<string>();

            while(true)
            {
                bool latest = false;
                while(reader.TryRead(out var last))
                {
                    // Get latest snippets
                    stack.Push(last);
                    latest = true;
                }
                if(stack.Count == 0)
                {
                    // Nothing yet and no history to try
                    try
                    {
                        // Wait for code
                        var last = reader.ReadAsync().AsTask().Result;
                        stack.Push(last);
                    }
                    catch(AggregateException e) when(e.InnerExceptions.Count == 1)
                    {
                        // Unwrap exception
                        ExceptionDispatchInfo.Capture(e.InnerException!).Throw();
                    }
                    while(reader.TryRead(out var last))
                    {
                        // Anything afterwards?
                        stack.Push(last);
                    }
                    latest = true;
                }

                // Stack is non-empty

                if(CompileText(stack.Pop(), latest))
                {
                    // Success - clear history
                    stack.Clear();
                }
            }
        }

        private bool CompileText(string text, bool latest)
        {
            var inputStream = new AntlrInputStream(text);

            var writer = new StringWriter();
            try
            {
                compiler.CompileToSource(inputStream, writer, throwOnError: true);

                var result = writer.ToString();

                Invoke(() => {
                    if(latest)
                    {
                        // Show visible success only on latest code
                        resultText.Enabled = true;
                        messageBox.Text = "";
                    }
                    resultText.Text = result;
                });

                return true;
            }
            catch(Exception e)
            {
                if(latest)
                {
                    // Ignore exceptions from non-recent code
                    var message = e.ToString();
                    Invoke(() => {
                        resultText.Enabled = false;
                        messageBox.Text = e.Message;
                    });
                }

                return false;
            }
        }
    }
}
