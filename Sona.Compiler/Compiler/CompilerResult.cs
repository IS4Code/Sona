using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            return Options.AssemblyLoadContext.LoadFromStream(Stream);
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
                entryPoint.CreateDelegate<Action>().Invoke();
            };
#pragma warning restore CS1998
        }
    }

    public record CompilerDiagnostic(DiagnosticLevel Level, string Code, string Message, int Line, object? Data)
    {
        public CompilerDiagnostic(FSharpDiagnostic diagnostic) : this(
            diagnostic.Severity.IsError ? DiagnosticLevel.Error :
            diagnostic.Severity.IsWarning ? DiagnosticLevel.Warning :
            DiagnosticLevel.Info,
            diagnostic.ErrorNumberText,
            diagnostic.Message,
            diagnostic.StartLine,
            diagnostic.ExtendedData?.Value)
        {

        }

        public CompilerDiagnostic(Exception exception, int? line = null) : this(
            DiagnosticLevel.Error,
            "COMPILER",
            exception.Message,
            (exception is RecognitionException re ? re.OffendingToken?.Line : line) ?? -1,
            exception)
        {

        }

        public override string ToString()
        {
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
