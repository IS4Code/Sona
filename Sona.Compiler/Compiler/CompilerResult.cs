using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace IS4.Sona.Compiler
{
    public class CompilerResult
    {
        readonly CompilerOptions options;
        readonly List<CompilerDiagnostic> diagnostics = new();

        public int? ExitCode { get; internal set; }
        public string? IntermediateCode { get; internal set; }
        public Stream? Stream { get; internal set; }

        public IReadOnlyCollection<CompilerDiagnostic> Diagnostics => diagnostics;

        public bool Success => !diagnostics.Any(d => d.Level == DiagnosticLevel.Error);

        object syncLock = new();

        bool assemblyInitialized;
        Assembly? assembly;

        public Assembly? Assembly => LazyInitializer.EnsureInitialized(ref assembly, ref assemblyInitialized, ref syncLock, () => {
            if(Stream == null) return null;
            return options.AssemblyLoadContext.LoadFromStream(Stream);
        });

        public MethodInfo? EntryPoint => Assembly?.EntryPoint;

        public CompilerResult(CompilerOptions options)
        {
            this.options = options;
        }

        internal void AddDiagnostic(CompilerDiagnostic diagnostic)
        {
            diagnostics.Add(diagnostic);
        }
    }

    public record CompilerDiagnostic(DiagnosticLevel Level, string Code, string Message, int Line, object? Data)
    {
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
