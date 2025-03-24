using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using Antlr4.Runtime;
using FSharp.Compiler.Diagnostics;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
using FSharp.Compiler.Tokenization;
using Sona.Grammar;
using Microsoft.FSharp.Core;
using static FSharp.Compiler.Interactive.Shell;

namespace Sona.Compiler.Gui
{
    public partial class MainForm : Form
    {
        readonly Channel<string> codeChannel;
        readonly Channel<CompilerResult> sourceChannel;
        readonly SonaCompiler compiler;

        const float fontScale = 1.5f;

        public MainForm()
        {
            InitializeComponent();

            Text = $"{ProductName} Playground v{ProductVersion}";

            DoubleBuffered = true;

            var font = messageBox.Font;

            messageBox.Font = new Font(
                font.FontFamily,
                font.SizeInPoints * fontScale,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont
            );

            sonaRichText.Font = new Font(
                FontFamily.GenericMonospace,
                font.SizeInPoints * fontScale,
                font.Style,
                font.Unit,
                font.GdiCharSet,
                font.GdiVerticalFont
            );

            const int measureLength = 3000;
            const int tabSize = 2;
            var indentSize = (float)TextRenderer.MeasureText(new string('_', measureLength), sonaRichText.Font).Width / measureLength * tabSize;
            sonaRichText.SelectionTabs = Enumerable.Range(1, 32).Select(i => (int)Math.Round(i * indentSize)).ToArray();
            sonaRichText.SetPadding(8, 8, 8, 8);

            resultRichText.BackColor = sonaRichText.BackColor;
            resultRichText.ForeColor = sonaRichText.ForeColor;
            resultRichText.Font = sonaRichText.Font;
            resultRichText.SetPadding(8, 8, 8, 8);

            codeSplit.SplitterWidth = codeSplit.SplitterWidth * 3 / 2;

            orientationButton.Text = codeSplit.Orientation.ToString();
            orientationButton.Tag = codeSplit.Orientation;

            ZoomChanged(1);

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

            adjustLineNumbersButton.Checked = true;
#if !DEBUG
            orientationButton.PerformClick();
            orientationButton.PerformClick();
#endif
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

            reformatModifiedText = true;
            sonaRichText.Text = @"import System
import Console.*

let name = ""Sona""
echo $""Hello, {name}""
ReadKey(true)!";
            ResetUndo();
        }

        int lastSelectedStart, lastSelectedLength;
        bool reformatModifiedText;
        bool modifying;

        static readonly SearchValues<char> surroundReformattingCharacters = SearchValues.Create(new[] { '(', ')', '[', ']', '{', '}', '/', '*', '$', '"', '\'', '\\' });
        static readonly SearchValues<char> lineReformattingCharacters = SearchValues.Create(new[] { '#' });

        private void sonaText_SelectionChanged(object sender, EventArgs e)
        {
            if(modifying)
            {
                return;
            }

            var selectionStart = sonaRichText.SelectionStart;
            var selectionLength = sonaRichText.SelectionLength;

            lineLabel.Text = $"{sonaRichText.GetLineFromCharIndex(selectionStart) + 1} : {selectionStart - sonaRichText.GetFirstCharIndexOfCurrentLine() + 1}";

            if(!sonaRichText.Modified)
            {
                // Preserve selection when not due to modification
                lastSelectedStart = sonaRichText.SelectionStart;
                lastSelectedLength = selectionLength;
                UpdateUndo(selectionStart, selectionLength);
            }

            int start = Math.Max(selectionStart - 1, 0);
            int end = Math.Min(selectionStart + selectionLength + 1, sonaRichText.TextLength);
            var text = sonaRichText.Text;
            var selectedSpan = text.AsSpan(start, end - start);
            // Check if selection is near characters that might require reformatting
            bool reformat = selectedSpan.ContainsAny(surroundReformattingCharacters);
            if(!reformat)
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
                reformat = lineSpan.ContainsAny(lineReformattingCharacters);
            }

            if(sonaRichText.Modified)
            {
                reformatModifiedText |= reformat;
            }
            else
            {
                reformatModifiedText = reformat;
            }
        }

        record Checkpoint(int FirstLineIndex, int LastLineIndexFromEnd, string Content, int SelectionStart, int SelectionLength);

        readonly Stack<Checkpoint> undoCheckpoints = new();
        readonly Stack<Checkpoint> redoCheckpoints = new();

        void SaveUndo(int from, int to, int selectedStart, int selectedLength)
        {
            int firstLine = sonaRichText.GetLineFromCharIndex(from);
            int lastLine = sonaRichText.GetLineFromCharIndex(to);

            int lineStart = sonaRichText.GetFirstCharIndexFromLine(firstLine);
            int lineEnd = sonaRichText.GetFirstCharIndexFromLine(lastLine + 1);
            if(lineEnd == -1)
            {
                lineEnd = sonaRichText.TextLength;
            }

            // Drawing is suspended and selection will be restored
            sonaRichText.SelectionStart = lineStart;
            sonaRichText.SelectionLength = lineEnd - lineStart;
            var content = sonaRichText.SelectedText;

            // Offset from end
            lastLine -= sonaRichText.GetLineFromCharIndex(sonaRichText.TextLength);

            undoCheckpoints.Push(new(firstLine, lastLine, content, selectedStart, selectedLength));
        }

        void UpdateUndo(int selectedStart, int selectedLength)
        {
            if(!undoCheckpoints.TryPop(out var cp))
            {
                return;
            }
            undoCheckpoints.Push(cp with
            {
                SelectionStart = selectedStart,
                SelectionLength = selectedLength
            });
        }

        void PerformUndo()
        {
            if(undoCheckpoints.Count <= 1)
            {
                return;
            }
            redoCheckpoints.Push(undoCheckpoints.Pop());
            RestoreCheckpoint(undoCheckpoints.Peek());
        }

        void PerformRedo()
        {
            if(!redoCheckpoints.TryPop(out var cp))
            {
                return;
            }
            undoCheckpoints.Push(cp);
            RestoreCheckpoint(cp);
        }

        void ResetUndo()
        {
            sonaRichText.ClearUndo();
            undoCheckpoints.Clear();
            redoCheckpoints.Clear();
            undoCheckpoints.Push(new(0, 0, sonaRichText.Text, sonaRichText.SelectionStart, sonaRichText.SelectionLength));
        }

        void RestoreCheckpoint(Checkpoint cp)
        {
            var (firstLine, lastLine, content, selectedStart, selectedLength) = cp;

            int lineStart = sonaRichText.GetFirstCharIndexFromLine(firstLine);
            int lineEnd = sonaRichText.GetFirstCharIndexFromLine(lastLine + sonaRichText.GetLineFromCharIndex(sonaRichText.TextLength) + 1);
            if(lineEnd == -1)
            {
                lineEnd = sonaRichText.TextLength;
            }

            bool updated = false;
            sonaRichText.SuspendDrawing();
            modifying = true;
            try
            {
                // Select span to replace
                sonaRichText.SelectionStart = lineStart;
                sonaRichText.SelectionLength = lineEnd - lineStart;
                sonaRichText.SelectionFont = sonaRichText.Font;
                sonaRichText.SelectedText = content;

                // Recalculate line end after modification
                lineEnd = sonaRichText.GetFirstCharIndexFromLine(lastLine + sonaRichText.GetLineFromCharIndex(sonaRichText.TextLength) + 1);
                if(lineEnd == -1)
                {
                    lineEnd = sonaRichText.TextLength;
                }

                // Update formatting
                updated = FormatInputText(sonaRichText.Text, lineStart, lineEnd);
            }
            finally
            {
                sonaRichText.SelectionLength = 0;
                sonaRichText.SelectionFont = sonaRichText.Font;
                modifying = false;

                sonaRichText.ClearUndo();
                sonaRichText.Modified = false;

                sonaRichText.SelectionStart = selectedStart;
                sonaRichText.SelectionLength = selectedLength;
                sonaRichText.ResumeDrawing();

                if(updated)
                {
                    sonaRichText.Update();
                }
            }
        }

        private void sonaText_TextChanged(object sender, EventArgs e)
        {
            // Compile current text
            Recompile();

            if(modifying)
            {
                return;
            }

            redoCheckpoints.Clear();
            sonaRichText.ClearUndo();

            // Highlighting
            var selectedStart = sonaRichText.SelectionStart;
            var selectedLength = sonaRichText.SelectionLength;

            bool updated = false;
            bool reformat = reformatModifiedText;
            sonaRichText.SuspendDrawing();
            try
            {
                // Select span of what was replaced and extend by 1 character
                int start = Math.Max(Math.Min(lastSelectedStart - 1, selectedStart - 1), 0);
                int end = Math.Min(Math.Max(lastSelectedStart + lastSelectedLength + 1, selectedStart + 1), sonaRichText.TextLength);

                // Save current modifications
                SaveUndo(start, end, selectedStart, selectedLength);

                sonaRichText.SelectionStart = start;
                sonaRichText.SelectionLength = end - start;

                // Remove formatting of inserted text
                sonaRichText.SelectionFont = sonaRichText.Font;
                sonaRichText.SelectionBackColor = sonaRichText.BackColor;
                sonaRichText.SelectionColor = sonaRichText.ForeColor;
                sonaRichText.SelectedText = sonaRichText.SelectedText;

                // Update formatting of input
                updated = FormatInputText(sonaRichText.Text, reformat ? 0 : start, reformat ? sonaRichText.TextLength : end);
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

            sonaRichText.ClearUndo();

            // Ignore modifications after selection changed
            sonaRichText.Modified = false;
            sonaText_SelectionChanged(sender, e);
        }

        private async void Recompile()
        {
            runMenuButton.Enabled = false;

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
            switch(e.KeyCode)
            {
                case Keys.Tab:
                    // Ctrl-Tab is non-input
                    e.IsInputKey = !e.Control;
                    break;
                case Keys.Z when e.Control && e.Shift:
                    PerformRedo();
                    break;
                case Keys.Z when e.Control:
                    PerformUndo();
                    break;
            }
        }

        private void sonaRichText_KeyDown(object sender, KeyEventArgs e)
        {
            if(!e.Control)
            {
                return;
            }
            switch(e.KeyCode)
            {
                case Keys.A:
                case Keys.C:
                case Keys.V:
                case Keys.X:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                case Keys.Delete:
                    return;
                default:
                    e.SuppressKeyPress = true;
                    return;
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
                if(start + length <= affectedStart)
                {
                    return;
                }
                if(start >= affectedEnd)
                {
                    return;
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

            var (showBeginEnd, adjustLineNumbers) = Invoke(() => {
                return (blockDelimitersButton.Checked, adjustLineNumbersButton.Checked);
            });

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
                if(latest && lastEntryPoint != null)
                {
                    Invoke(() => {
                        runMenuButton.Enabled = true;
                    });
                }
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

        Func<Task>? lastEntryPoint;

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
                var diagnostics = CheckSource(last!, out var entryPoint).Concat(last!.Diagnostics).Distinct().OrderBy(d => d.Line);
                EndWork();

                if(reader.TryPeek(out _))
                {
                    // Outdated
                    continue;
                }

                Invoke(() => {
                    messageBox.Text = String.Join(Environment.NewLine, diagnostics);
                    lastEntryPoint = entryPoint;
                    runMenuButton.Enabled = entryPoint != null;
                });
            }
        }

        (string? text, CompilerOptions options) latestSourceTuple;
        (IReadOnlyCollection<CompilerDiagnostic> diagnostics, Func<Task>? entryPoint) latestSourceResult;

        private IReadOnlyCollection<CompilerDiagnostic> CheckSource(CompilerResult result, out Func<Task>? entryPoint)
        {
            var tuple = (result.IntermediateCode, result.Options);
            if(latestSourceTuple == tuple)
            {
                var lastResult = latestSourceResult;
                entryPoint = lastResult.entryPoint;
                return lastResult.diagnostics;
            }

            try
            {
                // Expose F# errors
                result = compiler.CompileToDelegate(result, "input").Result;
                entryPoint = result.Success ? result.EntryPoint : null;
                latestSourceResult = (result.Diagnostics, entryPoint);
                return result.Diagnostics;
            }
            catch(Exception e)
            {
                // Ignore exceptions from non-recent code
                var message = e.ToString();
                Invoke(() => {
                    resultRichText.Enabled = false;
                    messageBox.Text = e.Message;
                });

                var diagnostics = new[]
                {
                    new CompilerDiagnostic(DiagnosticLevel.Error, "EXCEPTION", e.ToString(), 0, e)
                };
                entryPoint = null;
                latestSourceResult = (diagnostics, null);
                return diagnostics;
            }
            finally
            {
                latestSourceTuple = tuple;
            }
        }

        bool redrawing = false;

        private void SetOutputText(string str, bool noHighlighting)
        {
            var position = resultRichText.SelectionStart;
            var zoom = resultRichText.ZoomFactor;
            resultRichText.SuspendDrawing();
            redrawing = true;
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
                redrawing = false;
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
            ZoomChanged(sonaRichText.ZoomFactor);
        }

        private void resultRichText_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            if(redrawing)
            {
                return;
            }
            sonaRichText.ZoomFactor = resultRichText.ZoomFactor;
            ZoomChanged(resultRichText.ZoomFactor);
        }

        private void ZoomChanged(float zoom)
        {
            zoomButton.Text = $"{zoom:P0}";

            var statusFont = statusStrip.Font;
            lineLabel.Font = new Font(
                statusFont.FontFamily,
                statusFont.SizeInPoints * zoom * fontScale,
                statusFont.Style,
                statusFont.Unit,
                statusFont.GdiCharSet,
                statusFont.GdiVerticalFont
            );
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

                        sourceToolStrip.Visible = false;
                        editorToolStrip.Items.Remove(progressBar);
                        editorToolStrip.Items.Add(progressBar);
                        foreach(var item in sourceToolStrip.Items.Cast<ToolStripItem>().ToArray())
                        {
                            editorToolStrip.Items.Add(item);
                            if(item != progressBar)
                            {
                                item.Alignment = ToolStripItemAlignment.Right;
                            }
                            item.Tag = sourceToolStrip;
                        }

                        orientationButton.Text = $"Source: {codeSplit.Orientation}";
                        orientationButton.Tag = codeSplit.Orientation;
                        break;
                    case Orientation.Vertical:
                        codeSplit.Panel2Collapsed = true;
                        orientationButton.Text = "Source: Hidden";
                        orientationButton.Tag = null;

                        foreach(var item in editorToolStrip.Items.Cast<ToolStripItem>())
                        {
                            if(item.Tag == sourceToolStrip)
                            {
                                if(item != progressBar)
                                {
                                    item.Visible = false;
                                }
                            }
                        }
                        break;
                    case null:
                        codeSplit.Panel2Collapsed = false;

                        codeSplit.Orientation = Orientation.Horizontal;
                        codeSplit.SplitterDistance = distance * codeSplit.Height / codeSplit.Width;

                        foreach(var item in editorToolStrip.Items.Cast<ToolStripItem>().ToArray())
                        {
                            if(item.Tag == sourceToolStrip)
                            {
                                sourceToolStrip.Items.Add(item);
                                if(item != progressBar)
                                {
                                    item.Alignment = ToolStripItemAlignment.Left;
                                    item.Visible = true;
                                }
                            }
                        }
                        sourceToolStrip.Items.Add(progressBar);
                        sourceToolStrip.Visible = true;

                        orientationButton.Text = $"Source: {codeSplit.Orientation}";
                        orientationButton.Tag = codeSplit.Orientation;
                        break;
                }
            }
            finally
            {
                codeSplit.ResumeDrawing();
                codeSplit.Update();
            }
        }

        private void zoomButton_Click(object sender, EventArgs e)
        {
            sonaRichText.ZoomFactor = 1;
            resultRichText.ZoomFactor = 1;
            zoomButton.Text = $"{1:P0}";
        }

        CancellationTokenSource? executionCts;
        private async void runMenuButton_Click(object sender, EventArgs e)
        {
            var cts = executionCts;
            if(cts != null)
            {
                await cts.CancelAsync();
            }
            cts = executionCts = new();
            var entryPoint = lastEntryPoint;
            if(entryPoint == null)
            {
                return;
            }

            var buffer = new StringBuilder();

            var tcs = new TaskCompletionSource();
            cts.Token.Register(() => tcs.TrySetResult());

            new Thread(() => {
                try
                {
                    ConsoleExtensions.ShowConsole();

                    Console.SetOut(new DuplicatingTextWriter(Console.Out, new StringWriter(buffer)));
                    Console.SetError(new DuplicatingTextWriter(Console.Error, new StringWriter(buffer)));

                    Task? task = null;
#pragma warning disable SYSLIB0046
                    ControlledExecution.Run(() => {
                        task = entryPoint();
                    }, cts.Token);
#pragma warning restore SYSLIB0046

                    if(task != null)
                    {
                        try
                        {
                            task.Wait(cts.Token);
                        }
                        catch(AggregateException e) when(e.InnerExceptions.Count == 1)
                        {
                            ExceptionDispatchInfo.Capture(e.InnerException!).Throw();
                        }
                    }

                    tcs.TrySetResult();
                    try
                    {
                        Task.Delay(2000).Wait(cts.Token);
                    }
                    catch(OperationCanceledException)
                    {

                    }
                }
                catch(OperationCanceledException) when(cts.IsCancellationRequested)
                {

                }
                catch(FsiCompilationException fsiE)
                {
                    var errors = fsiE.ErrorInfos?.Value ?? Array.Empty<FSharpDiagnostic>();
                    if(errors.Length == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(fsiE);
                        Console.ReadKey(true);
                    }
                    else
                    {
                        foreach(var diagnostic in errors)
                        {
                            Console.WriteLine(new CompilerDiagnostic(diagnostic));
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ReadKey(true);
                }
                finally
                {
                    tcs.TrySetResult();
                    if(!cts.IsCancellationRequested)
                    {
                        ConsoleExtensions.HideConsole();
                    }
                }
            })
            {
                IsBackground = true
            }.Start();

            await tcs.Task;

            if(!cts.IsCancellationRequested)
            {
                messageBox.Text = buffer.ToString();
                Activate();
            }
        }

        string? currentFileName;

        private void saveMenuButton_Click(object sender, EventArgs e)
        {
            if(currentFileName == null)
            {
                saveAsMenuButton_Click(sender, e);
                return;
            }
            Save();
        }

        private void saveAsMenuButton_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFileName = saveFileDialog.FileName;
                Save();
            }
        }

        private void loadMenuButton_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFileName = openFileDialog.FileName;
                sonaRichText.Clear();
                sonaRichText.ClearUndo();
                reformatModifiedText = true;
                sonaRichText.Text = File.ReadAllText(currentFileName);
                ResetUndo();
            }
        }

        private void Save()
        {
            if(currentFileName == null)
            {
                return;
            }
            using var writer = File.CreateText(currentFileName);
            var first = true;
            foreach(var line in sonaRichText.Lines)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    writer.WriteLine();
                }
                writer.Write(line);
            }
        }
    }
}
