﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
            return options.AssemblyLoadContext.LoadFromStream(Stream);
        });

        Func<Task>? entryPoint;
        public Func<Task>? EntryPoint {
            get => entryPoint ??= CreateEntryPoint();
            set => entryPoint = value;
        } 

        public CompilerResult(CompilerOptions options)
        {
            this.options = options;
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
