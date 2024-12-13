using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using IS4.Sona.Compiler.Tools;

namespace IS4.Sona.Compiler
{
    internal sealed class SourceWriter : IndentedTextWriter
    {
        private new LineCountingTextWriter InnerWriter => (LineCountingTextWriter)base.InnerWriter;

        public bool AdjustLines { get; set; } = true;

        int symbolIndex;

        public SourceWriter(TextWriter writer) : base(new LineCountingTextWriter(writer), " ")
        {

        }

        IToken? expectedNextLineToken;
        public void UpdateLine(IToken token)
        {
            expectedNextLineToken = null;

            if(!AdjustLines)
            {
                return;
            }

            int line = token.Line;
            int lineDiff = line - InnerWriter.LineNumber;
            if(lineDiff == 0)
            {
                // Line numbers agree, no need to correct
                return;
            }

            if(InnerWriter.LinePosition > 0)
            {
                // Middle of a line - try correcting after WriteLine
                expectedNextLineToken = token;
                return;
            }

            if(lineDiff == 1)
            {
                // Can be corrected just by inserting a simple blank line
                InnerWriter.WriteLine();
            }
            else
            {
                // Write line directive
                InnerWriter.Write("# ");
                InnerWriter.WriteLine(line);
                InnerWriter.ResetLine(line);
            }
        }

        private void OnNewLine()
        {
            if(expectedNextLineToken is { } token && InnerWriter.LinePosition == 0)
            {
                // Try again for this line
                expectedNextLineToken = null;
                UpdateLine(token);
            }
        }

        protected override void OutputTabs()
        {
            OnNewLine();
            base.OutputTabs();
        }

        protected async override Task OutputTabsAsync()
        {
            OnNewLine();
            await base.OutputTabsAsync();
        }

        public string GetTemporarySymbol()
        {
            return "_ " + Interlocked.Increment(ref symbolIndex);
        }

        public void WriteSymbol(string name)
        {
            if(Syntax.IsValidIdentifierName(name))
            {
                // Write as-is
                Write(name);
                return;
            }
            if(!Syntax.IsValidEnclosedIdentifierName(name))
            {
                // Cannot be enclosed
                throw new ArgumentException($"The identifier '{name}' cannot be syntactically represented.", nameof(name));
            }
            Write("``");
            Write(name);
            Write("``");
        }

        public void WriteOperator(string op)
        {
            Write(" ");
            Write(op);
            Write(" ");
        }

        public void WriteLeftOperator(string op)
        {
            Write(" ");
            Write(op);
        }

        public void WriteSpecialMember(string op)
        {
            Write(".``");
            Write(op);
            Write("``");
        }

        public void WriteNamespacedName(string ns, string op)
        {
            Write("global.");
            Write(ns);
            Write('.');
            Write(op);
        }

        public void WriteOperatorName(string op)
        {
            WriteNamespacedName("Microsoft.FSharp.Core.Operators", op);
        }

        readonly Stack<(int from, int to)> scopeRestore = new();
        public void EnterScope()
        {
            int pos = InnerWriter.LinePosition;
            if(pos > Indent)
            {
                scopeRestore.Push((pos, Indent));
                Indent = pos;
            }
            else
            {
                Indent++;
            }
        }

        public void ExitScope()
        {
            if(scopeRestore.TryPeek(out var restore) && restore.from == Indent)
            {
                Indent = scopeRestore.Pop().to;
            }
            else
            {
                Indent--;
            }
        }

        sealed class LineCountingTextWriter : TextWriter
        {
            readonly TextWriter writer;

            public int LineNumber { get; private set; } = 1;
            public int LinePosition { get; private set; }

            public LineCountingTextWriter(TextWriter writer)
            {
                this.writer = writer;
            }

            public void ResetLine(int lineNumber)
            {
                LineNumber = lineNumber;
                LinePosition = 0;
            }

            private void OnNewLine()
            {
                LineNumber++;
                LinePosition = 0;
            }

            private void OnCharsWritten(int count)
            {
                LinePosition += count;
            }

            #region Overrides
            public override Encoding Encoding => writer.Encoding;
            public override IFormatProvider FormatProvider => writer.FormatProvider;
            public override string NewLine {
#pragma warning disable CS8765
                get => writer.NewLine;
                set => writer.NewLine = value;
#pragma warning restore CS8765
            }
            
            public override void Close() => writer.Close();
            public override ValueTask DisposeAsync() => writer.DisposeAsync();
            public override void Flush() => writer.Flush();
            public override Task FlushAsync() => writer.FlushAsync();

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return writer.FlushAsync(cancellationToken);
            }

            public override void Write(string? s)
            {
                writer.Write(s);
                OnCharsWritten(s?.Length ?? 0);
            }

            public override void Write(char value)
            {
                writer.Write(value);
                OnCharsWritten(1);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                writer.Write(buffer, index, count);
                OnCharsWritten(count);
            }

            public async override Task WriteAsync(char value)
            {
                await writer.WriteAsync(value).ConfigureAwait(false);
                OnCharsWritten(1);
            }

            public async override Task WriteAsync(char[] buffer, int index, int count)
            {
                await writer.WriteAsync(buffer, index, count).ConfigureAwait(false);
                OnCharsWritten(count);
            }

            public async override Task WriteAsync(string? value)
            {
                await writer.WriteAsync(value).ConfigureAwait(false);
                OnCharsWritten(value?.Length ?? 0);
            }

            public async override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
            {
                await writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                OnCharsWritten(buffer.Length);
            }

            public async override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
            {
                await writer.WriteAsync(value).ConfigureAwait(false);
                OnCharsWritten(value?.Length ?? 0);
            }

            public override void WriteLine(string? s)
            {
                writer.WriteLine(s);
                OnNewLine();
            }

            public override void WriteLine()
            {
                writer.WriteLine();
                OnNewLine();
            }

            public override void WriteLine(bool value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(char value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(char[]? buffer)
            {
                writer.WriteLine(buffer);
                OnNewLine();
            }

            public override void WriteLine(char[] buffer, int index, int count)
            {
                writer.WriteLine(buffer, index, count);
                OnNewLine();
            }

            public override void WriteLine(double value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(float value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(int value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(long value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(object? value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public override void WriteLine(string format, object? arg0)
            {
                writer.WriteLine(format, arg0);
                OnNewLine();
            }

            public override void WriteLine(string format, object? arg0, object? arg1)
            {
                writer.WriteLine(format, arg0, arg1);
                OnNewLine();
            }

            public override void WriteLine(string format, params object?[] arg)
            {
                writer.WriteLine(format, arg);
                OnNewLine();
            }

#if NET9_0_OR_GREATER
            public override void WriteLine(string format, ReadOnlySpan<object?> arg)
            {
                writer.WriteLine(format, arg);
                OnNewLine();
            }
#endif

            public override void WriteLine(uint value)
            {
                writer.WriteLine(value);
                OnNewLine();
            }

            public async override Task WriteLineAsync()
            {
                await writer.WriteLineAsync().ConfigureAwait(false);
                OnNewLine();
            }

            public async override Task WriteLineAsync(char value)
            {
                await writer.WriteLineAsync(value).ConfigureAwait(false);
                OnNewLine();
            }

            public async override Task WriteLineAsync(char[] buffer, int index, int count)
            {
                await writer.WriteLineAsync(buffer, index, count).ConfigureAwait(false);
                OnNewLine();
            }

            public async override Task WriteLineAsync(string? value)
            {
                await writer.WriteLineAsync(value).ConfigureAwait(false);
                OnNewLine();
            }

            public async override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
            {
                await writer.WriteLineAsync(buffer, cancellationToken).ConfigureAwait(false);
                OnNewLine();
            }

            public async override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
            {
                await writer.WriteLineAsync(value, cancellationToken).ConfigureAwait(false);
                OnNewLine();
            }
#endregion
        }
    }
}
