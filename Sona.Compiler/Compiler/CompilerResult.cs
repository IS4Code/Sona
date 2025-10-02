using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using FSharp.Compiler.Diagnostics;

namespace Sona.Compiler
{
    public class CompilerResult
    {
        readonly List<CompilerDiagnostic> diagnostics = new();

        public CompilerOptions Options { get; }
        public Exception? Exception { get; internal set; }
        public string? IntermediateCode { get; internal set; }
        public StringBuilder? GlobalCode { get; internal set; }
        public Stream? Stream { get; internal set; }

        public IReadOnlyCollection<CompilerDiagnostic> Diagnostics => diagnostics;

        bool? success;
        public bool Success {
            get => success ?? !diagnostics.Any(d => d.Level == DiagnosticLevel.Error);
            internal set => success = value;
        }

        object syncLock = new();

        bool assemblyInitialized;
        Assembly? assembly;

        public Assembly? Assembly => LazyInitializer.EnsureInitialized(ref assembly, ref assemblyInitialized, ref syncLock, () => {
            if(Stream == null) return null;
            return Options.AssemblyLoader.AssemblyLoadStream(Stream);
        });

        Func<Task>? entryPoint;
        public Func<Task>? EntryPoint {
            get => entryPoint ??= CreateEntryPoint();
            set => entryPoint = value;
        } 

        public CompilerResult(CompilerOptions options)
        {
            Options = options;
        }

        internal void AddDiagnostic(CompilerDiagnostic diagnostic)
        {
            diagnostics.Add(diagnostic);
        }

        private Func<Task>? CreateEntryPoint()
        {
            if(Assembly is not { EntryPoint: { } entryPoint })
            {
                return null;
            }
#pragma warning disable CS1998
            return async () => {
                ((Action)entryPoint.CreateDelegate(typeof(Action))).Invoke();
            };
#pragma warning restore CS1998
        }
    }

    public record CompilerDiagnostic(DiagnosticLevel Level, string Code, string Message, int Line, int? Column, object? Data) : IComparable<CompilerDiagnostic>
    {
        public CompilerDiagnostic(string message, FSharpDiagnostic diagnostic) : this(
            diagnostic.Severity.IsError ? DiagnosticLevel.Error :
            diagnostic.Severity.IsWarning ? DiagnosticLevel.Warning :
            DiagnosticLevel.Info,
            diagnostic.ErrorNumberText,
            message,
            diagnostic.StartLine,
            Int32.MinValue + diagnostic.StartColumn,
            diagnostic.ExtendedData?.Value)
        {

        }

        public CompilerDiagnostic(FSharpDiagnostic diagnostic) : this(diagnostic.Message, diagnostic)
        {

        }

        public CompilerDiagnostic(Exception exception, int? line = null) : this(
            DiagnosticLevel.Error,
            "COMPILER",
            exception.Message,
            GetExceptionLine(exception, out var column) ?? line ?? -1,
            column,
            exception)
        {

        }

        static int? GetExceptionLine(Exception e, out int? column)
        {
            if(e is RecognitionException re)
            {
                column = re.OffendingToken?.Column;
                return re.OffendingToken?.Line;
            }
            column = null;
            return null;
        }

        public int CompareTo(CompilerDiagnostic? other)
        {
            if(other == null)
            {
                return 1;
            }
            if(Line.CompareTo(other.Line) is not 0 and var lineCmp)
            {
                return lineCmp;
            }
            if(Column == other.Column)
            {
                // Definite ordering
                return Message.CompareTo(other.Message);
            }
            // Null column comes first
            if(Column is not { } col)
            {
                return -1;
            }
            if(other.Column is not { } otherCol)
            {
                return 1;
            }
            // F# errors go last
            if(col < 0 && otherCol >= 0)
            {
                return 1;
            }
            if(col >= 0 && otherCol < 0)
            {
                return -1;
            }
            return col.CompareTo(otherCol);
        }

        public override string ToString()
        {
            if(Column is { } col and >= 0)
            {
                return $"{Code}:{Line}:{col}: {Message}";
            }
            return $"{Code}:{Line}: {Message}";
        }
    }

    public enum DiagnosticLevel
    {
        Info,
        Warning,
        Error
    }
}
