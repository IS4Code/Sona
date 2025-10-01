using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Sona.Compiler.States;
using Sona.Compiler.Tools;

namespace Sona.Compiler
{
    internal interface ISourceWriter
    {
        string CreateTemporaryIdentifier();

        void Write(string text);
        void Write(char value);
        void WriteIdentifier(string name);
        void WriteOperator(string op);
        void WriteOperator(char op);
        void WriteLeftOperator(string op);
        void WriteLeftOperator(char op);
        void WriteNamespacedName(string ns, string name);

        void WriteLine();
        void WriteLine(string text);
        void WriteLine(char value);
        void WriteLineEnd(string newline);

        void UpdateLine(IToken token);
        void EnterScope();
        void ExitScope();
        void EnterNestedScope(bool keepLevel = false);
        void ExitNestedScope();

        ISourceCapture StartCapture();
        void StopCapture(ISourceCapture capture);
    }

    internal interface ISourceCapture
    {
        void Play(ISourceWriter writer);
    }

    internal static class SourceWriterExtensions
    {
        public static void WriteNamespacedName(this ISourceWriter writer, string ns, string name, string member)
        {
            writer.WriteNamespacedName(ns, name);
            writer.Write('.');
            writer.WriteIdentifier(member);
        }

        public static void WriteCoreName(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Microsoft.FSharp.Core", name);
        }

        public static void WriteCoreOperatorName(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Microsoft.FSharp.Core.Operators", name);
        }

        public static void WriteCollectionName(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Microsoft.FSharp.Collections", name);
        }

        public static void WriteSystemName(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("System", name);
        }

#pragma warning disable CS8524
        public static void WriteOptionNone(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteCoreName(optionType switch {
                ImplementationType.Class => "None",
                ImplementationType.Struct => "ValueNone"
            });
        }

        public static void WriteOptionNoneIdentifier(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteIdentifier(optionType switch
            {
                ImplementationType.Class => "None",
                ImplementationType.Struct => "ValueNone"
            });
        }

        public static void WriteOptionSome(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteCoreName(optionType switch
            {
                ImplementationType.Class => "Some",
                ImplementationType.Struct => "ValueSome"
            });
        }

        public static void WriteOptionSomeIdentifier(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteIdentifier(optionType switch
            {
                ImplementationType.Class => "Some",
                ImplementationType.Struct => "ValueSome"
            });
        }

        public static void WriteOptionType(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteCoreName(optionType switch
            {
                ImplementationType.Class => "Option",
                ImplementationType.Struct => "ValueOption"
            });
        }

        public static void WriteOptionAbbreviation(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteCoreName(optionType switch
            {
                ImplementationType.Class => "option",
                ImplementationType.Struct => "voption"
            });
        }

        public static void WriteCustomOptionConversionOperator(this ISourceWriter writer, ImplementationType optionType)
        {
            writer.WriteCustomOperator(optionType switch
            {
                ImplementationType.Class => "TryConversion",
                ImplementationType.Struct => "TryConversionValue"
            });
        }

        public static void WriteTupleOpen(this ISourceWriter writer, ImplementationType tupleType)
        {
            writer.Write(tupleType switch
            {
                ImplementationType.Class => "(",
                ImplementationType.Struct => "(struct("
            });
        }

        public static void WriteTupleClose(this ISourceWriter writer, ImplementationType tupleType)
        {
            writer.Write(tupleType switch
            {
                ImplementationType.Class => ")",
                ImplementationType.Struct => "))"
            });
        }

        public static void WriteCustomTupleFromTreeOperator(this ISourceWriter writer, ImplementationType tupleType)
        {
            writer.WriteCustomTupleOperator(tupleType switch
            {
                ImplementationType.Class => "FromTree",
                ImplementationType.Struct => "FromTreeValue"
            });
        }

        public static void WriteAnonymousRecordOpen(this ISourceWriter writer, ImplementationType recordType)
        {
            writer.Write(recordType switch
            {
                ImplementationType.Class => "{| ",
                ImplementationType.Struct => "(struct{| "
            });
        }

        public static void WriteAnonymousRecordClose(this ISourceWriter writer, ImplementationType recordType)
        {
            writer.Write(recordType switch
            {
                ImplementationType.Class => " |}",
                ImplementationType.Struct => " |})"
            });
        }

        public static void WriteCollectionOpen(this ISourceWriter writer, CollectionImplementationType collectionType)
        {
            writer.Write(collectionType switch
            {
                CollectionImplementationType.Array => "[|",
                CollectionImplementationType.List => "["
            });
        }

        public static void WriteCollectionClose(this ISourceWriter writer, CollectionImplementationType collectionType)
        {
            writer.Write(collectionType switch
            {
                CollectionImplementationType.Array => "|]",
                CollectionImplementationType.List => "]"
            });
        }

        public static void WriteCollectionTypeSuffix(this ISourceWriter writer, CollectionImplementationType collectionType)
        {
            switch(collectionType)
            {
                case CollectionImplementationType.Array:
                    writer.Write("[]");
                    break;
                case CollectionImplementationType.List:
                    writer.Write(' ');
                    writer.WriteCollectionName("list");
                    break;
            }
        }
#pragma warning restore CS8524

        public static void WriteCustomOperator(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "Operators", name);
        }

        public static void WriteCustomUnaryOperator(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "UnaryOperators", name);
        }

        public static void WriteCustomBinaryOperator(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "BinaryOperators", name);
        }

        public static void WriteCustomSequenceOperator(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "SequenceHelpers", name);
        }

        public static void WriteCustomTupleOperator(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "Tuples", name);
        }

        public static void WriteCustomPattern(this ISourceWriter writer, string name)
        {
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "Patterns", name);
        }

        public static void WriteSpecialMember(this ISourceWriter writer, string name)
        {
            writer.Write('.');
            writer.WriteIdentifier(name);
        }

        public static void WriteSpecialOperator(this ISourceWriter writer, string name)
        {
            writer.WriteOperator("|>");
            writer.WriteCustomOperator(name);
        }

        public static void WriteSpecialUnaryOperator(this ISourceWriter writer, string name)
        {
            writer.WriteOperator("|>");
            writer.WriteCustomUnaryOperator(name);
        }

        public static void WriteSpecialBinaryOperator(this ISourceWriter writer, string name)
        {
            writer.WriteOperator("|>");
            writer.WriteCustomBinaryOperator(name);
        }

        public static void WriteTraitAssertion(this ISourceWriter writer, string name)
        {
            writer.WriteOperator(':');
            writer.WriteNamespacedName("Sona.Runtime.Traits", "trait " + name);
            writer.Write("<_>");
        }

        public static void WriteTraitAssertion(this ISourceWriter writer, string name1, string name2)
        {
            writer.WriteOperator("|>");
            writer.WriteNamespacedName("Sona.Runtime.CompilerServices", "Inference", name1 + "|" + name2);
        }

        static readonly string space32 = new string(' ', 32);
        static readonly string space16 = new string(' ', 16);
        static readonly string space8 = new string(' ', 8);
        static readonly string space4 = new string(' ', 4);
        static readonly string space2 = new string(' ', 2);
        public static void WriteSpace(this ISourceWriter writer, int length)
        {
            if(length == 0)
            {
                return;
            }
            while(length > 32)
            {
                writer.Write(space32);
                length -= 32;
            }
            while(length > 16)
            {
                writer.Write(space16);
                length -= 16;
            }
            while(length > 8)
            {
                writer.Write(space8);
                length -= 8;
            }
            while(length > 4)
            {
                writer.Write(space4);
                length -= 4;
            }
            while(length > 2)
            {
                writer.Write(space2);
                length -= 2;
            }
            switch(length)
            {
                case 0:
                    return;
                case 1:
                    writer.Write(' ');
                    return;
                default:
                    writer.Write(new string(' ', length));
                    return;
            }
        }

        public static void WriteStringPart(this ISourceWriter writer, string part)
        {
            foreach(var str in Syntax.EscapeString(part))
            {
                writer.Write(str);
            }
        }
    }

    internal sealed class SourceWriter : IndentedTextWriter, ISourceWriter
    {
        private new LineCountingTextWriter InnerWriter => (LineCountingTextWriter)base.InnerWriter;

        public bool AdjustLines { get; set; } = true;

        public bool SkipEmptyLines { get; set; } = false;

        readonly Stack<Capture> captures = new();

        bool Recording([MaybeNullWhen(false)] out Capture capture) => captures.TryPeek(out capture);

        int tmpIdIndex;

        public SourceWriter(TextWriter writer) : base(new LineCountingTextWriter(writer), " ")
        {

        }

        void Record(Capture capture, Action<SourceWriter> action, CaptureActionType type = default)
        {
            if(type != default && capture.Count is > 0 and var count && capture[count - 1].type == type)
            {
                capture[count - 1] = (action, type);
            }
            else
            {
                capture.Add((action, type));
            }
        }

        IToken? expectedNextLineToken;
        public void UpdateLine(IToken token)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.UpdateLine(token), CaptureActionType.LineUpdate);
                return;
            }

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

#if NET6_0_OR_GREATER
        protected async override Task OutputTabsAsync()
        {
            OnNewLine();
            await base.OutputTabsAsync();
        }
#endif

        public string CreateTemporaryIdentifier()
        {
            return "_ " + Interlocked.Increment(ref tmpIdIndex);
        }

        void ISourceWriter.Write(string text)
        {
            if(text.Length == 0)
            {
                // Nothing to write anyway
                return;
            }

            if(Recording(out var capture))
            {
                Record(capture, x => ((ISourceWriter)x).Write(text));
                return;
            }

            Write(text);
        }

        void ISourceWriter.Write(char value)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => ((ISourceWriter)x).Write(value));
                return;
            }

            Write(value);
        }

        void ISourceWriter.WriteLine()
        {
            if(Recording(out var capture))
            {
                Record(capture, x => ((ISourceWriter)x).WriteLine());
                return;
            }

            if(SkipEmptyLines && InnerWriter.LinePosition == 0)
            {
                // Empty line (or only tabs) - ignore
                return;
            }

            WriteLine();
        }

        void ISourceWriter.WriteLine(string text)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => ((ISourceWriter)x).WriteLine(text));
                return;
            }

            if(SkipEmptyLines && text.Length == 0 && InnerWriter.LinePosition == 0)
            {
                // Empty line (or only tabs) - ignore
                return;
            }

            WriteLine(text);
        }

        void ISourceWriter.WriteLine(char value)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => ((ISourceWriter)x).WriteLine(value));
                return;
            }

            WriteLine(value);
        }

        public void WriteLineEnd(string newline)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteLineEnd(newline));
                return;
            }

            // Never skip empty lines

            var original = NewLine;
            NewLine = newline;
            try
            {
                WriteLine();
            }
            finally
            {
                NewLine = original;
            }
        }

        public void WriteIdentifier(string name)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteIdentifier(name));
                return;
            }

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
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteOperator(op));
                return;
            }

            Write(" ");
            Write(op);
            Write(" ");
        }

        public void WriteOperator(char op)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteOperator(op));
                return;
            }

            Write(" ");
            Write(op);
            Write(" ");
        }

        public void WriteLeftOperator(string op)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteLeftOperator(op));
                return;
            }

            Write(" ");
            Write(op);
        }

        public void WriteLeftOperator(char op)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteLeftOperator(op));
                return;
            }

            Write(" ");
            Write(op);
        }

        public void WriteNamespacedName(string ns, string name)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.WriteNamespacedName(ns, name));
                return;
            }

            Write("global.");
            Write(ns);
            Write('.');
            WriteIdentifier(name);
        }

        readonly Stack<(int from, int to)> scopeRestore = new();
        public void EnterScope()
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.EnterScope());
                return;
            }

            int pos = InnerWriter.LinePosition;
            if(pos > Indent)
            {
                throw new InvalidOperationException("The writer is not located at the beginning of a line.");
                //scopeRestore.Push((pos, Indent));
                //Indent = pos;
            }
            else
            {
                Indent++;
            }
        }

        public void ExitScope()
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.ExitScope());
                return;
            }

            if(scopeRestore.TryPeek(out var restore) && restore.from == Indent)
            {
                throw NestedScopeError();
                //Indent = scopeRestore.Pop().to;
            }
            else
            {
                Indent--;
            }
        }

        public void EnterNestedScope(bool keepLevel = false)
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.EnterNestedScope(keepLevel));
                return;
            }

            OutputTabs();
            int pos = InnerWriter.LinePosition;
            scopeRestore.Push((pos, Indent));
            if(keepLevel)
            {
                Indent = pos + 1;
            }
            else
            {
                Indent++;
            }
            scopeRestore.Push((Indent, pos));
        }

        public void ExitNestedScope()
        {
            if(Recording(out var capture))
            {
                Record(capture, x => x.ExitNestedScope());
                return;
            }

            if(!(scopeRestore.TryPeek(out var restore) && restore.from == Indent))
            {
                throw NestedScopeError();
            }
            var newIndent = scopeRestore.Pop().to;
            if(!(scopeRestore.TryPeek(out restore) && restore.from == newIndent))
            {
                scopeRestore.Push((Indent, newIndent));
                throw NestedScopeError();
            }
            Indent = newIndent;
            OutputTabs();
            Indent = scopeRestore.Pop().to;
        }

        Exception NestedScopeError()
        {
            return new InvalidOperationException($"{nameof(ExitNestedScope)} must be used to exit a scope entered with {nameof(EnterNestedScope)}.");
        }

        public ISourceCapture StartCapture()
        {
            var capture = new Capture();
            captures.Push(capture);
            return capture;
        }

        public void StopCapture(ISourceCapture capture)
        {
            if(capture is not Capture)
            {
                throw new ArgumentException("Argument must be a capture created from this instance.", nameof(capture));
            }
            if(!captures.TryPop(out var topCapture))
            {
                throw new InvalidOperationException("The writer is not currently recording.");
            }
            if(topCapture != capture)
            {
                // Push back
                captures.Push(topCapture!);
                throw new InvalidOperationException("Only the capture that is currently being recorded can be stopped.");
            }
        }

        private void PlayCapture(Capture capture)
        {
            if(!captures.TryPeek(out var topCapture))
            {
                // Direct output
                foreach(var (action, _) in capture)
                {
                    action(this);
                }
                return;
            }
            if(topCapture == capture)
            {
                throw new InvalidOperationException("It is not possible to play back the capture that is currently being recorded.");
            }
            // Recording - copy the contents
            if(capture.Count == 0)
            {
                return;
            }
            var type = capture[0].type;
            if(type != default && topCapture.Count is > 0 and var count && topCapture[count - 1].type == type)
            {
                // Merge actions of same type
                topCapture.RemoveAt(count - 1);
            }
            topCapture.AddRange(capture);
        }

        sealed class Capture : List<(Action<SourceWriter> action, CaptureActionType type)>, ISourceCapture
        {
            public void Play(ISourceWriter writer)
            {
                if(writer is not SourceWriter parentWriter)
                {
                    throw new ArgumentException("Argument must be the same object that created this instance.", nameof(writer));
                }
                parentWriter.PlayCapture(this);
            }
        }

        enum CaptureActionType
        {
            NonReplaceable,
            LineUpdate
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
            public override void Flush() => writer.Flush();
            public override Task FlushAsync() => writer.FlushAsync();

#if NET8_0_OR_GREATER
            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return writer.FlushAsync(cancellationToken);
            }
#endif

#if NETCOREAPP3_0_OR_GREATER
            public override ValueTask DisposeAsync() => writer.DisposeAsync();
#endif

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

#if NETCOREAPP2_1_OR_GREATER
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
#endif

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

#if NETCOREAPP2_1_OR_GREATER
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
#endif
            #endregion
        }
    }
}
