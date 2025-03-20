using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
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
        readonly Channel<CompilerResult> sourceChannel;
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

            // Reading will be done from a thread
            codeChannel = Channel.CreateUnbounded<string>(new()
            {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true
            });

            sourceChannel = Channel.CreateUnbounded<CompilerResult>(new()
            {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = true
            });

            compiler = new SonaCompiler();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Run compilers as a background thread in case of freeze
            new Thread(UpdateText)
            {
                IsBackground = true
            }.Start();

            new Thread(UpdateSource)
            {
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

        private void sonaText_TextChanged(object sender, EventArgs e)
        {
            // Compile current text
            Recompile();

            // Highlighting
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
        }

        private async void Recompile()
        {
            // Yield to get latest text in case of replacement
            await Task.Yield();

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

                if(start > lastPosition)
                {
                    // Ignored part after previous token
                    var ignoreLength = start - lastPosition;
                    FormatToken(lastPosition, ignoreLength, input.AsSpan(lastPosition, ignoreLength).IsWhiteSpace() ? FontStyle.Regular : FontStyle.Italic);
                }
                lastPosition = start + length;
                
                if(token.Type is SonaLexer.BEGIN_INLINE_SOURCE)
                {
                    FormatToken(start, length, FontStyle.Bold | FontStyle.Italic);
                    while((token = lexer.NextToken()) is { Type: not SonaLexer.END_INLINE_SOURCE } fsToken)
                    {
                        switch(token.Type)
                        {
                            case SonaLexer.Eof:
                                return true;
                            case SonaLexer.FS_WHITESPACE:
                            case SonaLexer.FS_EOL:
                                continue;
                            case SonaLexer.FS_COMMENT:
                            case SonaLexer.FS_BEGIN_BLOCK_COMMENT:
                            case SonaLexer.FS_END_BLOCK_COMMENT:
                                start = token.StartIndex;
                                length = token.StopIndex + 1 - start;
                                FormatToken(start, length, FontStyle.Italic);
                                continue;
                        }
                        var tokenText = token.Text;
                        if(String.IsNullOrWhiteSpace(tokenText))
                        {
                            continue;
                        }
                        start = token.StartIndex;
                        int offset = 0;
                        FSharpTokenize(tokenText, (part, style) => {
                            if(part.Length == 0)
                            {
                                return;
                            }
                            int found = tokenText.IndexOf(part, offset);
                            if(found == -1)
                            {
                                return;
                            }
                            offset = found + part.Length;
                            FormatToken(start + found, part.Length, style);
                        });
                    }
                    start = token.StartIndex;
                    length = token.StopIndex + 1 - start;
                    lastPosition = start + length;
                    FormatToken(start, length, FontStyle.Bold | FontStyle.Italic);
                    continue;
                }

                var text = token.Text;
                switch(token.Type)
                {
                    case SonaLexer.ERROR:
                    case SonaLexer.UNDERSCORE:
                    case SonaLexer.INT_SUFFIX:
                    case SonaLexer.FLOAT_SUFFIX:
                    case SonaLexer.EXP_SUFFIX:
                    case SonaLexer.HEX_SUFFIX:
                    case SonaLexer.END_STRING_SUFFIX:
                    case SonaLexer.END_CHAR_SUFFIX:
                        // Error
                        FormatToken(start, length, FontStyle.Italic | FontStyle.Strikeout);
                        break;
                    case SonaLexer.INT_LITERAL:
                    case SonaLexer.FLOAT_LITERAL:
                    case SonaLexer.EXP_LITERAL:
                    case SonaLexer.HEX_LITERAL:
                    case SonaLexer.STRING_LITERAL:
                    case SonaLexer.VERBATIM_STRING_LITERAL:
                    case SonaLexer.CHAR_LITERAL:
                    case SonaLexer.BEGIN_CHAR:
                    case SonaLexer.BEGIN_STRING:
                    case SonaLexer.BEGIN_INTERPOLATED_STRING:
                    case SonaLexer.BEGIN_VERBATIM_INTERPOLATED_STRING:
                    case SonaLexer.CHAR_PART:
                    case SonaLexer.STRING_PART:
                    case SonaLexer.END_CHAR:
                    case SonaLexer.END_STRING:
                    case SonaLexer.DOC_COMMENT:
                    case SonaLexer.INTERP_FORMAT_CUSTOM:
                    case SonaLexer.INTERP_FORMAT_STANDARD:
                    case SonaLexer.INTERP_ALIGNMENT:
                        // Literal
                        FormatToken(start, length, FontStyle.Italic);
                        break;
                    case SonaLexer.INTERP_FORMAT_GENERAL:
                    case SonaLexer.INTERP_FORMAT_NUMBER:
                    case SonaLexer.INTERP_FORMAT_BEGIN_COMPONENTS:
                    case SonaLexer.INTERP_COMPONENTS_PART_SHORT:
                    case SonaLexer.INTERP_COMPONENTS_PART_LONG:
                        // Checked interpolation format
                        if(text.Length > 0 && text[0] is '\\' or '\'')
                        {
                            goto case SonaLexer.INTERP_FORMAT_CUSTOM;
                        }
                        FormatToken(start, length, FontStyle.Bold | FontStyle.Italic);
                        break;
                    case not SonaLexer.NAME when text.Length > 0 && Char.IsAsciiLetter(text[0]):
                        // Keyword
                        FormatToken(start, length, FontStyle.Bold);
                        break;
                    case SonaLexer.BEGIN_GENERAL_LOCAL_ATTRIBUTE:
                    case SonaLexer.END_DIRECTIVE:
                        // Directive
                        FormatToken(start, length, FontStyle.Bold | FontStyle.Italic);
                        break;
                    default:
                        if(text.Length > 2 && text[0] == '#' && Char.IsAsciiLetter(text[1]))
                        {
                            goto case SonaLexer.END_DIRECTIVE;
                        }
                        // Normal
                        FormatToken(start, length, FontStyle.Regular);
                        break;
                }
            }

            if(input.Length > lastPosition)
            {
                // Ignored part after last token
                FormatToken(lastPosition, input.Length - lastPosition, input.AsSpan(lastPosition).IsWhiteSpace() ? FontStyle.Regular : FontStyle.Italic);
            }

            void FormatToken(int start, int length, FontStyle style)
            {
                if(!reformatModifiedText)
                {
                    if(start + length <= affectedStart)
                    {
                        return;
                    }
                    if(start >= affectedEnd)
                    {
                        return;
                    }
                }
                sonaRichText.SelectionStart = start;
                sonaRichText.SelectionLength = length;
                sonaRichText.SelectionFont = new(sonaRichText.Font, style);
                updated = true;
            }

            return updated;
        }

        int workCounter;

        private void BeginWork()
        {
            if(Interlocked.Increment(ref workCounter) == 1)
            {
                Invoke(() => {
                    progressBar.Style = ProgressBarStyle.Marquee;
                });
            }
        }

        private void EndWork()
        {
            if(Interlocked.Decrement(ref workCounter) == 0)
            {
                Invoke(() => {
                    progressBar.Style = ProgressBarStyle.Blocks;
                });
            }
        }

        private void UpdateText()
        {
            var reader = codeChannel.Reader;
            var stack = new Stack<string>();

            bool showBeginEnd = false;
            bool adjustLineNumbers = false;

            BeginWork();

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
                    EndWork();
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
                    BeginWork();
                    (showBeginEnd, adjustLineNumbers) = Invoke(() => {
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

                var options = new CompilerOptions
                (
                    Target: BinaryTarget.Script,
                    Flags: CompilerFlags.Privileged |
                        (showBeginEnd ? CompilerFlags.DebuggingComments : 0) |
                        (adjustLineNumbers ? 0 : CompilerFlags.IgnoreLineNumbers),
                    AssemblyLoadContext: AssemblyLoadContext.Default
                );

                if(CompileText(stack.Pop(), latest, options))
                {
                    // Success - clear history
                    stack.Clear();
                }
            }
        }

        (string text, bool latest, CompilerOptions options) latestCodeTuple;
        bool latestCodeResult;

        private bool CompileText(string text, bool latest, CompilerOptions options)
        {
            var tuple = (text, latest, options);
            if(latestCodeTuple == tuple)
            {
                return latestCodeResult;
            }

            var inputStream = new AntlrInputStream(text);

            try
            {
                var result = compiler.CompileToString(inputStream, options);

                var resultText = result.IntermediateCode ?? "";

                Invoke(() => {
                    if(latest)
                    {
                        // Show visible success only on latest code
                        resultRichText.Enabled = true;
                        messageBox.Text = String.Join(Environment.NewLine, result.Diagnostics.OrderBy(d => d.Line));
                    }

                    SetOutputText(resultText, (options.Flags & CompilerFlags.IgnoreLineNumbers) == 0);
                });

                if(latest)
                {
                    // Send to F# checker
                    if(!sourceChannel.Writer.TryWrite(result))
                    {
                        sourceChannel.Writer.WriteAsync(result).AsTask().Wait();
                    }
                }

                return latestCodeResult = true;
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

                return latestCodeResult = false;
            }
            finally
            {
                latestCodeTuple = tuple;
            }
        }
        

        private void UpdateSource()
        {
            var reader = sourceChannel.Reader;

            while(true)
            {
                CompilerResult? last = null;
                while(reader.TryRead(out var next))
                {
                    // Get latest results
                    last = next;
                }
                if(last == null)
                {
                    // Nothing yet
                    try
                    {
                        // Wait for results
                        last = reader.ReadAsync().AsTask().Result;
                    }
                    catch(AggregateException e) when(e.InnerExceptions.Count == 1)
                    {
                        // Unwrap exception
                        ExceptionDispatchInfo.Capture(e.InnerException!).Throw();
                    }
                    while(reader.TryRead(out var next))
                    {
                        // Anything afterwards?
                        last = next;
                    }
                }

                // Result exists
                BeginWork();
                var diagnostics = CheckSource(last!).Concat(last!.Diagnostics).Distinct().OrderBy(d => d.Line);
                EndWork();

                if(reader.TryPeek(out _))
                {
                    // Outdated
                    continue;
                }

                Invoke(() => {
                    messageBox.Text = String.Join(Environment.NewLine, diagnostics);
                });
            }
        }

        (string? text, CompilerOptions options) latestSourceTuple;
        IReadOnlyCollection<CompilerDiagnostic> latestSourceResult;

        private IReadOnlyCollection<CompilerDiagnostic> CheckSource(CompilerResult result)
        {
            var tuple = (result.IntermediateCode, result.Options);
            if(latestSourceTuple == tuple)
            {
                return latestSourceResult;
            }

            try
            {
                // Expose F# errors
                compiler.CheckResult(result, "input", result.Options);

                return latestSourceResult = result.Diagnostics;
            }
            catch(Exception e)
            {
                // Ignore exceptions from non-recent code
                var message = e.ToString();
                Invoke(() => {
                    resultRichText.Enabled = false;
                    messageBox.Text = e.Message;
                });

                return latestSourceResult = new[]
                {
                    new CompilerDiagnostic(DiagnosticLevel.Error, "EXCEPTION", e.ToString(), 0, e)
                };
            }
            finally
            {
                latestSourceTuple = tuple;
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
                FontStyle lastStyle = FontStyle.Regular;
                FSharpTokenize(str, (part, style) => {
                    if(part.Length == 0)
                    {
                        return;
                    }
                    if(style != lastStyle)
                    {
                        resultRichText.SelectionFont = new(resultRichText.Font, style);
                        lastStyle = style;
                    }
                    resultRichText.AppendText(part);
                });
                resultRichText.SelectionFont = resultRichText.Font;
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

        private void FSharpTokenize(string str, Action<string, FontStyle> receiver)
        {
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
                                receiver(pretext, FontStyle.Italic | FontStyle.Bold);
                            }
                            else
                            {
                                receiver(pretext, FontStyle.Italic);
                            }
                        }
                        else
                        {
                            receiver(pretext, FontStyle.Regular);
                        }
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
                        receiver(subtext, FontStyle.Bold);
                    }
                    else if(tok.IsCommentTrivia || tok.IsNumericLiteral || tok.IsStringLiteral || tok.Kind == FSharpTokenKind.KeywordString || tok.Kind == FSharpTokenKind.Char)
                    {
                        receiver(subtext, FontStyle.Italic);
                    }
                    else if(tok.Kind == FSharpTokenKind.Identifier && subtext.StartsWith('`'))
                    {
                        receiver(subtext, subtext.StartsWith("``_ ") ? FontStyle.Underline | FontStyle.Italic : FontStyle.Italic);
                    }
                    else
                    {
                        receiver(subtext, FontStyle.Regular);
                    }
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
            receiver(lasttext, FontStyle.Regular);
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
