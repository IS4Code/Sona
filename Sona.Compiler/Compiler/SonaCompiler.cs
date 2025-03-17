using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.IO;
using FSharp.Compiler.Text;
using IS4.Sona.Compiler.States;
using IS4.Sona.Compiler.Tools;
using IS4.Sona.Grammar;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using static FSharp.Compiler.Interactive.Shell;

namespace IS4.Sona.Compiler
{
    public class SonaCompiler
    {
        static readonly bool debugging =
#if DEBUG
            Debugger.IsAttached
#else
            false
#endif
            ;

        public CompilerResult CompileToSource(ICharStream inputStream, TextWriter output, CompilerOptions options)
        {
            var result = new CompilerResult(options);

            // Will add diagnostics to result
            var errorListener = new ErrorHandler(result);

            var lexer = GetLexer(inputStream);
            errorListener.AddListener(lexer);

            // Store pragmas and other channel-specific entities
            var lexerContext = new LexerContext(lexer);

            var tokenStream = new UnbufferedListenerTokenStream(lexer, lexerContext.OnLexerToken);
            var parser = new SonaParser(tokenStream);
            errorListener.AddListener(parser);
            parser.ErrorHandler = errorListener;

            bool debugBeginEnd = (options.Flags & CompilerFlags.DebuggingComments) != 0;

            using var writer = new SourceWriter(output);
            writer.AdjustLines = (options.Flags & CompilerFlags.IgnoreLineNumbers) == 0;
            writer.SkipEmptyLines = !debugBeginEnd;

            if(!debugging)
            {
                // Parse tree shall be kept only in localized cases
                parser.BuildParseTree = false;
            }

            var context = new ScriptEnvironment(parser, writer, lexerContext, debugBeginEnd ? "(*begin*)" : "", debugBeginEnd ? "(*end*)" : "");

            // Main state to process the chunk
            parser.AddParseListener(new ChunkState(context));

            try
            {
                parser.chunk();
            }
            catch(Exception e)
            {
                result.AddDiagnostic(new(DiagnosticLevel.Error, "COMPILER", e.Message, lexer.Line, e));
            }

            return result;
        }

        [CLSCompliant(false)]
        public SonaLexer GetLexer(ICharStream inputStream)
        {
            var lexer = new SonaLexer(inputStream);

            // Add empty mode below, to handle close parenthesis
            int defaultMode = lexer.CurrentMode;
            lexer.Mode(SonaLexer.Empty);
            lexer.PushMode(defaultMode);

            return lexer;
        }

        sealed class ErrorHandler : DefaultErrorStrategy, IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
        {
            readonly CompilerResult result;

            public ErrorHandler(CompilerResult result)
            {
                this.result = result;
            }

            public void AddListener<TInterpreter>(Recognizer<IToken, TInterpreter> recognizer) where TInterpreter : Antlr4.Runtime.Atn.ATNSimulator
            {
                recognizer.RemoveErrorListeners();
                recognizer.AddErrorListener(this);
            }

            public void AddListener<TInterpreter>(Recognizer<int, TInterpreter> recognizer) where TInterpreter : Antlr4.Runtime.Atn.ATNSimulator
            {
                recognizer.RemoveErrorListeners();
                recognizer.AddErrorListener(this);
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                if(debugging)
                {
                    Debugger.Break();
                }
                result.AddDiagnostic(new(DiagnosticLevel.Error, "PARSER", msg, line, e));
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                if(debugging)
                {
                    Debugger.Break();
                }
                result.AddDiagnostic(new(DiagnosticLevel.Error, "LEXER", msg, line, e));
            }

            public override void ReportError(Parser recognizer, RecognitionException e)
            {
                if(e is SemanticException && !InErrorRecoveryMode(recognizer))
                {
                    NotifyErrorListeners(recognizer, e.Message, e);
                    return;
                }
                base.ReportError(recognizer, e);
            }
        }

        static readonly Dictionary<string, string> embeddedLibraries = new[]
        {
            "Sona.Runtime",
            "FSharp.Core",
            "System.Buffers",
            "System.Memory",
            "System.Numerics.Vectors",
            "System.Runtime.CompilerServices.Unsafe"
        }.ToDictionary(n => n, n => n + ".dll", StringComparer.OrdinalIgnoreCase);

        static readonly string[] referencedLibraries = new[]
        {
            "Sona.Runtime",
            "FSharp.Core"
        };

        static readonly Assembly currentAssembly = typeof(SonaCompiler).Assembly;

        public static Stream? ResolveEmbeddedAssembly(string? name)
        {
            if(name is null)
            {
                return null;
            }
            name = new AssemblyName(name).Name;
            if(name is null || !embeddedLibraries.TryGetValue(name, out var file))
            {
                return null;
            }
            return currentAssembly.GetManifestResourceStream(file);
        }

        public CompilerResult CompileToString(AntlrInputStream inputStream, CompilerOptions options)
        {
            CompilerResult result;
            string source;
            using(var sourceWriter = new StringWriter())
            {
                result = CompileToSource(inputStream, sourceWriter, options);
                sourceWriter.Flush();
                source = sourceWriter.ToString();
            }
            result.IntermediateCode = source;
            return result;
        }

        private LocalFileSystem GetFileSystem(string source, string fileName, CompilerOptions options, out string inputPath, out string outputPath, out string manifestPath, out string depsPath)
        {
            // A virtual file prefix (accessed through the local file system) to stand for in-memory dependencies
            var fsPrefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(fileName));

            var fs = new LocalFileSystem(fsPrefix, options.AssemblyLoadContext);
            inputPath = fsPrefix + ".fsx";
            outputPath = fsPrefix + ".dll";
            manifestPath = fsPrefix + ".win32manifest";

            fs.InputFiles[inputPath] = source;
            fs.InputFiles[manifestPath] = defaultWin32Manifest;

            depsPath = fsPrefix + ".deps";
            foreach(var file in embeddedLibraries.Values)
            {
                var path = Path.Combine(depsPath, file);
                fs.InputFiles[path] = LocalInputFile.FromEmbeddedFile(currentAssembly, file);
            }
            return fs;
        }

        public async Task<CompilerResult> CompileToStream(AntlrInputStream inputStream, string fileName, Stream outputStream, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            var result = CompileToString(inputStream, options);
            var source = result.IntermediateCode!;

            var fs = GetFileSystem(source, fileName, options, out var inputPath, out var outputPath, out var manifestPath, out var depsPath);

            fs.OutputFiles[outputPath] = outputStream;

            using var replacedEnv = await FSharpIsolatedEnvironment.CreateAsync(fs, depsPath, cancellationToken);

            var sourceText = SourceText.ofString(source);

            FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;
                
            var (checker, flags) = await GetCheckerAndOptions(inputPath, sourceText, manifestPath, options, cancelTokenOption);

            var args = new[]
            {
                "fsc.exe", // ignored
                "--out:" + outputPath
            }.Concat(referencedLibraries.Select(f => "-r:" + Path.Combine(depsPath, f)))
            .Concat(flags.OtherOptions)
            .Concat(flags.ReferencedProjects.Select(p => p.OutputFile))
            .Concat(flags.SourceFiles)
            .ToArray();

            var (diagnostics, exitCode) = await FSharpAsync.StartAsTask(
                checker.Compile(args, userOpName: null),
                taskCreationOptions: null, cancellationToken: cancelTokenOption
            );

            result.ExitCode = exitCode;
            foreach(var diagnostic in diagnostics)
            {
                var level =
                    diagnostic.Severity.IsError ? DiagnosticLevel.Error :
                    diagnostic.Severity.IsWarning ? DiagnosticLevel.Warning :
                    DiagnosticLevel.Info;

                result.AddDiagnostic(new(level, diagnostic.ErrorNumberText, diagnostic.Message, diagnostic.StartLine, diagnostic.ExtendedData?.Value));
            }

            return result;
        }

        public Task<CompilerResult> CompileToBinary(AntlrInputStream inputStream, string fileName, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            return CompileToStream(inputStream, fileName, new BlockBufferStream(), options, cancellationToken: cancellationToken);
        }

        readonly ConcurrentDictionary<CompilerOptions, FsiEvaluationSession> sessionCache = new();

        private FsiEvaluationSession CheckEvaluation(CompilerResult result, string manifestPath, string depsPath, CompilerOptions options)
        {
            var session = sessionCache.GetOrAdd(options, options => {
                var flags = GetOptions(manifestPath, options);

                var args = new[]
                {
                    "fsi.exe", // ignored
                    "--noninteractive",
                    "--consolecolors",
                    "--gui-",
                    "--quiet"
                }.Concat(referencedLibraries.Select(f => "-r:" + Path.Combine(depsPath, f)))
                .Concat(flags)
                .ToArray();

                var config = FsiEvaluationSession.GetDefaultConfiguration();
                return FsiEvaluationSession.Create(config, args, Console.In, Console.Out, Console.Error, collectible: true, legacyReferenceResolver: null);
            });

            // Check for errors separately 
            var (parseResults, fileResults, projectResults) = session.ParseAndCheckInteraction(result.IntermediateCode!);

            foreach(var diagnostic in parseResults.Diagnostics.Concat(fileResults.Diagnostics).Concat(projectResults.Diagnostics).Distinct())
            {
                var level =
                    diagnostic.Severity.IsError ? DiagnosticLevel.Error :
                    diagnostic.Severity.IsWarning ? DiagnosticLevel.Warning :
                    DiagnosticLevel.Info;

                result.AddDiagnostic(new(level, diagnostic.ErrorNumberText, diagnostic.Message, diagnostic.StartLine, diagnostic.ExtendedData?.Value));
            }

            if(projectResults.HasCriticalErrors)
            {
                result.Success = false;
            }
            return session;
        }

        public async Task<CompilerResult> CompileToDelegate(AntlrInputStream inputStream, string fileName, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            var result = CompileToString(inputStream, options);
            var source = result.IntermediateCode!;

            var fs = GetFileSystem(source, fileName, options, out var inputPath, out _, out var manifestPath, out var depsPath);

            using var replacedEnv = await FSharpIsolatedEnvironment.CreateAsync(fs, depsPath, cancellationToken);

            FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;

            var session = CheckEvaluation(result, manifestPath, depsPath, options);

            if(!result.Success)
            {
                return result;
            }

            result.EntryPoint = async () => {
                using var replacedEnv = await FSharpIsolatedEnvironment.CreateAsync(fs, depsPath, cancellationToken);
                await EvalInteraction();
            };

            return result;

            Task EvalInteraction()
            {
                var (result, diagnostics) = session.EvalInteractionNonThrowing(source, inputPath, cancelTokenOption);

                if(result is FSharpChoice<FSharpOption<FsiValue>, Exception>.Choice2Of2 { Item: { } exception })
                {
                    // Unwrap exception
                    return Task.FromException(exception);
                }
                return Task.CompletedTask;
            }
        }

        public Task<CompilerResult> Compile(AntlrInputStream inputStream, string fileName, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            if(options.Target == BinaryTarget.Script)
            {
                return CompileToDelegate(inputStream, fileName, options, cancellationToken);
            }
            else
            {
                return CompileToBinary(inputStream, fileName, options, cancellationToken);
            }
        }

        public void CheckResult(CompilerResult result, string fileName, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            var fs = GetFileSystem(result.IntermediateCode ?? throw new ArgumentException("Result is missing compilable code.", nameof(result)), fileName, options, out _, out _, out var manifestPath, out var depsPath);

            using var replacedEnv = FSharpIsolatedEnvironment.Create(fs, depsPath, cancellationToken);
            CheckEvaluation(result, manifestPath, depsPath, options);
        }

        static readonly string[] executableFlags = new[] {
            "--subsystemversion:6.00",
            "--standalone",
            "--nointerfacedata",
            "--platform:anycpu",
        };

        static readonly string[] libraryFlags = new[] {
            "--nowin32manifest",
            "--targetprofile:netstandard",
            "--platform:anycpu",
        };

        static readonly string[] scriptFlags = new[] {
            "--targetprofile:netstandard",
        };

        static readonly string[] commonFlags = new[] {
            "--debug:embedded",
            "--deterministic+",

            "--preferreduilang:en",
            //"--flaterrors",
            //"--parallelcompilation",

            "--simpleresolution",

            "--langversion:latest",
            "--nowarn:3220,3559",
            // 3559 is triggered by unused generic returns
            "--warnon:21,52,1178,1182,3387,3388,3389,3397,3390,3517",
            "--warnaserror+:20,25,193,3517,3388",
            "--checknulls+",
        };

        private IEnumerable<string> GetOptions(string manifestPath, CompilerOptions options)
        {
            var allFlags = new List<string>();
            if(options.Target != BinaryTarget.Script)
            {
                allFlags.Add($"--target:{options.Target.ToString().ToLowerInvariant()}");
            }
            if((options.Flags & CompilerFlags.Optimize) != 0)
            {
                allFlags.Add("--optimize+");
            }
            else
            {
                allFlags.Add("--optimize-");
            }
            switch(options.Target)
            {
                case BinaryTarget.Exe:
                case BinaryTarget.WinExe:
                    allFlags.AddRange(executableFlags);
                    allFlags.Add($"--win32manifest:{manifestPath}");
                    break;
                case BinaryTarget.Script:
                    allFlags.AddRange(scriptFlags);
                    break;
                default:
                    allFlags.AddRange(libraryFlags);
                    break;
            }
            allFlags.AddRange(commonFlags);

            if((options.Flags & CompilerFlags.Privileged) == 0)
            {
                allFlags.Add("--reflectionfree");
            }
            return allFlags;
        }

        private async Task<(FSharpChecker, FSharpProjectOptions)> GetCheckerAndOptions(string sourceName, ISourceText sourceText, string manifestPath, CompilerOptions options, FSharpOption<CancellationToken>? cancellationToken)
        {
            // Does not appear to be used at all
            var documentSource = GetDocumentSource(sourceName, sourceText);

            var allFlags = GetOptions(manifestPath, options);

            var checker = FSharpChecker.Create(
                projectCacheSize: null,
                keepAssemblyContents: null,
                keepAllBackgroundResolutions: false,
                legacyReferenceResolver: null,
                tryGetMetadataSnapshot: null,
                suggestNamesForErrors: null,
                keepAllBackgroundSymbolUses: false,
                enableBackgroundItemKeyStoreAndSemanticClassification: null,
                enablePartialTypeChecking: null,
                parallelReferenceResolution: true,
                captureIdentifiersWhenParsing: null,
                documentSource: documentSource,
                useSyntaxTreeCache: false,
                useTransparentCompiler: null
            );

            var (flags, _) = await FSharpAsync.StartAsTask(checker.GetProjectOptionsFromScript(
                sourceName,
                sourceText,
                previewEnabled: null,
                loadedTimeStamp: null,
                otherFlags: allFlags.ToArray(),
                useFsiAuxLib: false,
                useSdkRefs: false,
                assumeDotNetFramework: options.Target is BinaryTarget.Exe or BinaryTarget.WinExe,
                sdkDirOverride: null,
                optionsStamp: null,
                userOpName: null
            ), taskCreationOptions: null, cancellationToken: cancellationToken);

            return (checker, flags);
        }

        static readonly FSharpAsync<FSharpOption<ISourceText>?> nullSourceTextAsync = FSharpAsync.AwaitTask(Task.FromResult<FSharpOption<ISourceText>?>(
            null
        ));
        private DocumentSource GetDocumentSource(string name, ISourceText source)
        {
            var sourceAsync = FSharpAsync.AwaitTask(Task.FromResult<FSharpOption<ISourceText>?>(
                FSharpOption<ISourceText>.Some(source)
            ));
            var func = FSharpFunc<string, FSharpAsync<FSharpOption<ISourceText>?>>.FromConverter(
                path => path == name ? sourceAsync : nullSourceTextAsync
            );
            return DocumentSource.NewCustom(func);
        }

        readonly struct FSharpIsolatedEnvironment : IDisposable
        {
            static readonly Type fileSystemLock = typeof(FileSystemAutoOpens);

            static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

            readonly IFileSystem previousFileSystem;
            readonly string? previousBin;

            private FSharpIsolatedEnvironment(IFileSystem newFileSystem, string compilerBinPath)
            {
                lock(fileSystemLock)
                {
                    previousFileSystem = FileSystemAutoOpens.FileSystem;
                    FileSystemAutoOpens.FileSystem = newFileSystem;

                    previousBin = Environment.GetEnvironmentVariable("FSHARP_COMPILER_BIN");
                    Environment.SetEnvironmentVariable("FSHARP_COMPILER_BIN", compilerBinPath);
                }
            }

            public static async ValueTask<FSharpIsolatedEnvironment> CreateAsync(IFileSystem newFileSystem, string compilerBinPath, CancellationToken cancellationToken)
            {
                await semaphore.WaitAsync(cancellationToken);
                return new FSharpIsolatedEnvironment(newFileSystem, compilerBinPath);
            }

            public static FSharpIsolatedEnvironment Create(IFileSystem newFileSystem, string compilerBinPath, CancellationToken cancellationToken)
            {
                semaphore.Wait(cancellationToken);
                return new FSharpIsolatedEnvironment(newFileSystem, compilerBinPath);
            }

            public void Dispose()
            {
                lock(fileSystemLock)
                {
                    FileSystemAutoOpens.FileSystem = previousFileSystem;
                    Environment.SetEnvironmentVariable("FSHARP_COMPILER_BIN", previousBin);
                }
                semaphore.Release();
            }
        }

        const string defaultWin32Manifest = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<assembly xmlns=""urn:schemas-microsoft-com:asm.v1"" manifestVersion=""1.0"">
  <assemblyIdentity version=""1.0.0.0"" name=""MyApplication.app""/>
  <trustInfo xmlns=""urn:schemas-microsoft-com:asm.v2"">
    <security>
      <requestedPrivileges xmlns=""urn:schemas-microsoft-com:asm.v3"">
        <requestedExecutionLevel level=""asInvoker"" uiAccess=""false""/>
      </requestedPrivileges>
    </security>
  </trustInfo>
</assembly>
";
    }
}
