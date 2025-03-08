using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using Antlr4.Runtime;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
using FSharp.Compiler.Tokenization;
using IS4.Sona.Grammar;
using Microsoft.FSharp.Core;

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

            sonaRichText.Font = new Font(
                FontFamily.GenericMonospace,
                font.SizeInPoints * zoom,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont
            );

            const int measureLength = 3000;
            const int tabSize = 2;
            var indentSize = (float)TextRenderer.MeasureText(new string('_', measureLength), sonaRichText.Font).Width / measureLength * tabSize;
            sonaRichText.SelectionTabs = Enumerable.Range(1, 32).Select(i => (int)Math.Round(i * indentSize)).ToArray();

            resultRichText.BackColor = sonaRichText.BackColor;
            resultRichText.ForeColor = sonaRichText.ForeColor;
            resultRichText.Font = sonaRichText.Font;

            codeSplit.SplitterWidth = codeSplit.SplitterWidth * 3 / 2;

            orientationButton.Text = codeSplit.Orientation.ToString();
            orientationButton.Tag = codeSplit.Orientation;

            zoomButton.Text = $"{sonaRichText.ZoomFactor:P0}";

            progressBar.Size = new(progressBar.Height, progressBar.Height);

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

        int lastSelectedStart, lastSelectedLength;
        bool reformatModifiedText;

        static readonly SearchValues<char> surroundReformattingCharacters = SearchValues.Create(new[] { '(', ')', '[', ']', '{', '}', '/', '*', '$', '"', '\'', '\\' });
        static readonly SearchValues<char> lineReformattingCharacters = SearchValues.Create(new[] { '#' });

        private void sonaText_SelectionChanged(object sender, EventArgs e)
        {
            if(sonaRichText.Modified)
            {
                // Ignore selection change due to modification
                return;
            }

            // Preserve selection
            lastSelectedStart = sonaRichText.SelectionStart;
            lastSelectedLength = sonaRichText.SelectionLength;

            int start = Math.Max(lastSelectedStart - 1, 0);
            int end = Math.Min(lastSelectedStart + lastSelectedLength + 1, sonaRichText.TextLength);
            var text = sonaRichText.Text;
            var selectedSpan = text.AsSpan(start, end - start);
            // Check if selection is near characters that might require reformatting
            reformatModifiedText = selectedSpan.ContainsAny(surroundReformattingCharacters);
            if(!reformatModifiedText)
            {
                // Get span of the whole line
                var startLine = sonaRichText.GetLineFromCharIndex(start);
                var endLine = sonaRichText.GetLineFromCharIndex(end);
                var lineStart = sonaRichText.GetFirstCharIndexFromLine(startLine);
                int lineEnd;
                if(sonaRichText.GetFirstCharIndexFromLine(endLine) == end)
                {
                    // Selected up to the next line
                    lineEnd = end;
                }
                else
                {
                    // Include the whole next line
                    lineEnd = sonaRichText.GetFirstCharIndexFromLine(endLine + 1);
                    if(lineEnd == -1)
                    {
                        lineEnd = sonaRichText.TextLength;
                    }
                }
                var lineSpan = text.AsSpan(lineStart, lineEnd - lineStart);
                // Check if lines contain characters that might require reformatting
                reformatModifiedText = lineSpan.ContainsAny(lineReformattingCharacters);
            }
        }

        private async void sonaText_TextChanged(object sender, EventArgs e)
        {
            var selectedStart = sonaRichText.SelectionStart;
            var selectedLength = sonaRichText.SelectionLength;

            bool updated = false;
            sonaRichText.SuspendDrawing();
            try
            {
                // Select span of what was replaced and extend by 1 character
                int start = Math.Max(Math.Min(lastSelectedStart - 1, selectedStart - 1), 0);
                int end = Math.Min(Math.Max(lastSelectedStart + lastSelectedLength + 1, selectedStart + 1), sonaRichText.TextLength);

                sonaRichText.SelectionStart = start;
                sonaRichText.SelectionLength = end - start;

                // Remove formatting of inserted text
                sonaRichText.SelectionFont = sonaRichText.Font;
                sonaRichText.SelectionBackColor = sonaRichText.BackColor;
                sonaRichText.SelectionColor = sonaRichText.ForeColor;
                sonaRichText.SelectedText = sonaRichText.SelectedText;

                // Update formatting of input
                updated = FormatInputText(sonaRichText.Text, start, end);
            }
            finally
            {
                sonaRichText.SelectionStart = selectedStart;
                sonaRichText.SelectionLength = selectedLength;
                sonaRichText.SelectionFont = sonaRichText.Font;
                sonaRichText.ResumeDrawing();
                if(updated)
                {
                    sonaRichText.Update();
                }
            }

            // Ignore modifications after selection changed
            sonaRichText.Modified = false;
            sonaText_SelectionChanged(sender, e);

            // Yield to get latest text in case of replacement
            await Task.Yield();

            // Compile current text
            Recompile();
        }

        private async void Recompile()
        {
            // Send the text to channel
            await codeChannel.Writer.WriteAsync(sonaRichText.Text);
        }

        private void blockDelimitersButton_CheckedChanged(object sender, EventArgs e)
        {
            Recompile();
        }

        private void adjustLineNumbersButton_CheckedChanged(object sender, EventArgs e)
        {
            Recompile();
        }

        private void sonaRichText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.Tab)
            {
                // Ctrl-Tab is non-input
                e.IsInputKey = !e.Control;
            }
        }

        static readonly Regex lastLineWhitespace = new Regex(@"^([ \t]*).*(?-m)$", RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Compiled);

        private void sonaRichText_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case '\r':
                {
                    var oldText = sonaRichText.Text;

                    int position = sonaRichText.SelectionStart;

                    // Find whitespace at the last line
                    var spaces = lastLineWhitespace.Match(oldText, 0, position).Groups[1].Value;

                    // Append whitespace
                    sonaRichText.SelectedText += spaces;
                    break;
                }
                case '\t':
                {
                    e.Handled = true;
                    int position = sonaRichText.SelectionStart;
                    int length = sonaRichText.SelectionLength;
                    switch(ModifierKeys)
                    {
                        case 0:
                        {
                            // Add tab in front
                            sonaRichText.SuspendDrawing();
                            try
                            {
                                sonaRichText.SelectionLength = 0;
                                sonaRichText.SelectedText = "\t";
                            }
                            finally
                            {
                                sonaRichText.SelectionStart = position + 1;
                                sonaRichText.SelectionLength = length;
                                sonaRichText.ResumeDrawing();
                                sonaRichText.Update();
                            }
                            break;
                        }
                        case Keys.Shift:
                        {
                            if(position == 0)
                            {
                                break;
                            }
                            if(sonaRichText.Text[position - 1] == '\t')
                            {
                                // Remove existing tab
                                sonaRichText.SuspendDrawing();
                                try
                                {
                                    sonaRichText.SelectionStart = position - 1;
                                    sonaRichText.SelectionLength = 1;
                                    sonaRichText.SelectedText = "";
                                }
                                finally
                                {
                                    sonaRichText.SelectionLength = length;
                                    sonaRichText.ResumeDrawing();
                                    sonaRichText.Update();
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private bool FormatInputText(string input, int affectedStart, int affectedEnd)
        {
            if(compiler == null)
            {
                return false;
            }

            bool updated = false;

            var inputStream = new AntlrInputStream(input);
            var lexer = compiler.GetLexer(inputStream);

            var lastPosition = 0;
            while(lexer.NextToken() is { Type: not SonaLexer.Eof } token)
            {
                // Read token from stream
                var start = token.StartIndex;
                var length = token.StopIndex + 1 - start;

                if(start > lastPosition && (reformatModifiedText || (affectedEnd >= lastPosition && affectedStart <= start)))
                {
                    // Ignored part after previous token
                    var ignoreLength = start - lastPosition;
                    sonaRichText.SelectionStart = lastPosition;
                    sonaRichText.SelectionLength = ignoreLength;
                    sonaRichText.SelectionFont = new(sonaRichText.Font, input.AsSpan(lastPosition, ignoreLength).IsWhiteSpace() ? FontStyle.Regular : FontStyle.Italic);
                    updated = true;
                }

                lastPosition = start + length;

                if(!reformatModifiedText)
                {
                    if(start + length <= affectedStart)
                    {
                        continue;
                    }
                    if(start >= affectedEnd)
                    {
                        continue;
                    }
                }

                // Needs reformatting
                sonaRichText.SelectionStart = start;
                sonaRichText.SelectionLength = length;

                var text = token.Text;
                if(token.Type is SonaLexer.FS_BEGIN_BLOCK_COMMENT or SonaLexer.FS_COMMENT or SonaLexer.FS_DIRECTIVE or SonaLexer.FS_END_BLOCK_COMMENT or SonaLexer.FS_EOL or SonaLexer.FS_PART or SonaLexer.FS_WHITESPACE)
                {
                    // Inline F#
                    sonaRichText.SelectionFont = new(sonaRichText.Font, FontStyle.Italic);
                }
                else if(token.Type is SonaLexer.ERROR or SonaLexer.UNDERSCORE or SonaLexer.INT_SUFFIX or SonaLexer.FLOAT_SUFFIX or SonaLexer.EXP_SUFFIX or SonaLexer.HEX_SUFFIX or SonaLexer.END_STRING_SUFFIX or SonaLexer.END_CHAR_SUFFIX)
                {
                    // Error
                    sonaRichText.SelectionFont = new(sonaRichText.Font, FontStyle.Italic | FontStyle.Strikeout);
                }
                else if(token.Type is SonaLexer.INT_LITERAL or SonaLexer.FLOAT_LITERAL or SonaLexer.EXP_LITERAL or SonaLexer.HEX_LITERAL or SonaLexer.STRING_LITERAL or SonaLexer.VERBATIM_STRING_LITERAL or SonaLexer.CHAR_LITERAL or SonaLexer.BEGIN_CHAR or SonaLexer.BEGIN_STRING or SonaLexer.BEGIN_INTERPOLATED_STRING or SonaLexer.BEGIN_VERBATIM_INTERPOLATED_STRING or SonaLexer.CHAR_PART or SonaLexer.STRING_PART or SonaLexer.END_CHAR or SonaLexer.END_STRING or SonaLexer.DOC_COMMENT)
                {
                    // Literal
                    sonaRichText.SelectionFont = new(sonaRichText.Font, FontStyle.Italic);
                }
                else if(text.Length > 0 && Char.IsAsciiLetter(text[0]) && token.Type is not SonaLexer.NAME)
                {
                    // Keyword
                    sonaRichText.SelectionFont = new(sonaRichText.Font, FontStyle.Bold);
                }
                else if((text.Length > 2 && text[0] == '#' && Char.IsAsciiLetter(text[1])) || token.Type is SonaLexer.END_INLINE_SOURCE or SonaLexer.BEGIN_GENERAL_LOCAL_ATTRIBUTE or SonaLexer.END_DIRECTIVE)
                {
                    // Directive
                    sonaRichText.SelectionFont = new(sonaRichText.Font, FontStyle.Bold | FontStyle.Italic);
                }
                else
                {
                    // Normal
                    sonaRichText.SelectionFont = sonaRichText.Font;
                }
                updated = true;
            }

            if(input.Length > lastPosition && (reformatModifiedText || affectedEnd >= lastPosition))
            {
                // Ignored part after last token
                sonaRichText.SelectionStart = lastPosition;
                sonaRichText.SelectionLength = input.Length - lastPosition;
                sonaRichText.SelectionFont = new(sonaRichText.Font, input.AsSpan(lastPosition).IsWhiteSpace() ? FontStyle.Regular : FontStyle.Italic);
                updated = true;
            }

            return updated;
        }

        private void UpdateText()
        {
            var reader = codeChannel.Reader;
            var stack = new Stack<string>();

            bool showBeginEnd = false;
            bool adjustLineNumbers = false;

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
                    Invoke(() => {
                        progressBar.Style = ProgressBarStyle.Blocks;
                    });
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
                    (showBeginEnd, adjustLineNumbers) = Invoke(() => {
                        progressBar.Style = ProgressBarStyle.Marquee;
                        return (blockDelimitersButton.Checked, adjustLineNumbersButton.Checked);
                    });

                    while(reader.TryRead(out var last))
                    {
                        // Anything afterwards?
                        stack.Push(last);
                    }
                    latest = true;
                }

                // Stack is non-empty

                if(CompileText(stack.Pop(), latest, showBeginEnd, adjustLineNumbers))
                {
                    // Success - clear history
                    stack.Clear();
                }
            }
        }

        (string text, bool latest, bool showBeginEnd, bool adjustLineNumbers) latestTuple;
        bool latestResult;

        private bool CompileText(string text, bool latest, bool showBeginEnd, bool adjustLineNumbers)
        {
            var tuple = (text, latest, showBeginEnd, adjustLineNumbers);
            if(latestTuple == tuple)
            {
                return latestResult;
            }

            var inputStream = new AntlrInputStream(text);

            var writer = new StringWriter();
            try
            {
                compiler.ShowBeginEnd = showBeginEnd;
                compiler.AdjustLines = adjustLineNumbers;
                compiler.CompileToSource(inputStream, writer, throwOnError: true);

                var result = writer.ToString();

                Invoke(() => {
                    if(latest)
                    {
                        // Show visible success only on latest code
                        resultRichText.Enabled = true;
                        messageBox.Text = "";
                    }

                    SetOutputText(result, adjustLineNumbers);
                });

                return latestResult = true;
            }
            catch(Exception e)
            {
                if(latest)
                {
                    // Ignore exceptions from non-recent code
                    var message = e.ToString();
                    Invoke(() => {
                        resultRichText.Enabled = false;
                        messageBox.Text = e.Message;
                    });
                }

                return latestResult = false;
            }
            finally
            {
                latestTuple = tuple;
            }
        }

        private void SetOutputText(string str, bool noHighlighting)
        {
            var position = resultRichText.SelectionStart;
            var zoom = resultRichText.ZoomFactor;
            resultRichText.SuspendDrawing();
            try
            {
                resultRichText.Clear();
                resultRichText.ZoomFactor = 1;
                resultRichText.ZoomFactor = zoom;
                if(noHighlighting)
                {
                    resultRichText.Text = str;
                    return;
                }
                var text = SourceText.ofString(str);
                var lastPosition = PositionModule.pos0;
                FSharpLexer.Tokenize(
                    text,
                    FSharpFunc<FSharpToken, Unit>.FromConverter(tok => {
                        var range = tok.Range;
                        if(range.IsSynthetic || PositionModule.posGeq(lastPosition, range.End))
                        {
                            return null!;
                        }
                        if(PositionModule.posGt(range.Start, lastPosition))
                        {
                            string pretext;
                            try
                            {
                                pretext = text.GetSubTextFromRange(RangeModule.mkRange("", lastPosition, range.Start));
                            }
                            catch(ArgumentException)
                            {
                                return null!;
                            }
                            if(pretext != ")" && !String.IsNullOrWhiteSpace(pretext))
                            {
                                if(pretext.Contains("(*begin*)", StringComparison.Ordinal) || pretext.Contains("(*end*)"))
                                {
                                    resultRichText.SelectionFont = new(resultRichText.Font, FontStyle.Italic | FontStyle.Bold);
                                }
                                else
                                {
                                    resultRichText.SelectionFont = new(resultRichText.Font, FontStyle.Italic);
                                }
                            }
                            resultRichText.AppendText(pretext);
                            resultRichText.SelectionFont = resultRichText.Font;
                            lastPosition = range.Start;
                        }
                        string subtext;
                        try
                        {
                            subtext = text.GetSubTextFromRange(range);
                        }
                        catch(ArgumentException)
                        {
                            return null!;
                        }
                        if(tok.Kind == FSharpTokenKind.OffsideBlockBegin && subtext != "begin")
                        {
                            return null!;
                        }
                        if(tok.Kind == FSharpTokenKind.OffsideBlockEnd && subtext != "end")
                        {
                            return null!;
                        }
                        lastPosition = range.End;
                        if(tok.IsKeyword || (subtext.Length > 0 && Char.IsAsciiLetter(subtext[0]) && !PrettyNaming.IsIdentifierName(subtext)))
                        {
                            resultRichText.SelectionFont = new(resultRichText.Font, FontStyle.Bold);
                        }
                        else if(tok.IsCommentTrivia || tok.IsNumericLiteral || tok.IsStringLiteral || tok.Kind == FSharpTokenKind.KeywordString || tok.Kind == FSharpTokenKind.Char)
                        {
                            resultRichText.SelectionFont = new(resultRichText.Font, FontStyle.Italic);
                        }
                        else if(tok.Kind == FSharpTokenKind.Identifier && subtext.StartsWith('`'))
                        {
                            resultRichText.SelectionFont = new(resultRichText.Font, subtext.StartsWith("``_ ") ? FontStyle.Underline | FontStyle.Italic : FontStyle.Italic);
                        }
                        resultRichText.AppendText(subtext);
                        resultRichText.SelectionFont = resultRichText.Font;
                        return null!;
                    }),
                    langVersion: null,
                    strictIndentation: null,
                    filePath: null,
                    conditionalDefines: null,
                    flags: null,
                    pathMap: null,
                    ct: null
                );
                var (lastline, lastcol) = text.GetLastCharacterPosition();
                string lasttext;
                try
                {
                    lasttext = text.GetSubTextFromRange(RangeModule.mkRange("", lastPosition, PositionModule.mkPos(lastline, lastcol)));
                }
                catch(ArgumentException)
                {
                    return;
                }
                resultRichText.AppendText(lasttext);
            }
            finally
            {
                if(position > resultRichText.TextLength)
                {
                    position = resultRichText.TextLength;
                }
                resultRichText.SelectionStart = position;
                resultRichText.ScrollToCaret();
                resultRichText.ResumeDrawing();
                resultRichText.Update();
            }
        }

        private void sonaRichText_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            resultRichText.ZoomFactor = sonaRichText.ZoomFactor;
            zoomButton.Text = $"{sonaRichText.ZoomFactor:P0}";
        }

        private void resultRichText_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            sonaRichText.ZoomFactor = resultRichText.ZoomFactor;
            zoomButton.Text = $"{resultRichText.ZoomFactor:P0}";
        }

        private void orientationButton_Click(object sender, EventArgs e)
        {
            var distance = codeSplit.SplitterDistance;
            codeSplit.SuspendDrawing();
            try
            {
                switch((Orientation?)orientationButton.Tag)
                {
                    case Orientation.Horizontal:
                        codeSplit.Orientation = Orientation.Vertical;
                        codeSplit.SplitterDistance = distance * codeSplit.Width / codeSplit.Height;
                        break;
                    case Orientation.Vertical:
                        codeSplit.Orientation = Orientation.Horizontal;
                        codeSplit.SplitterDistance = distance * codeSplit.Height / codeSplit.Width;
                        break;
                }
            }
            finally
            {
                codeSplit.ResumeDrawing();
                codeSplit.Update();
            }
            orientationButton.Text = codeSplit.Orientation.ToString();
            orientationButton.Tag = codeSplit.Orientation;
        }

        private void zoomButton_Click(object sender, EventArgs e)
        {
            sonaRichText.ZoomFactor = 1;
            resultRichText.ZoomFactor = 1;
            zoomButton.Text = $"{1:P0}";
        }
    }
}
