using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.IO;
using FSharp.Compiler.Text;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Sona.Compiler.States;
using Sona.Compiler.Tools;
using Sona.Grammar;
using static FSharp.Compiler.Interactive.Shell;

namespace Sona.Compiler
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

        static SonaCompiler()
        {

        }

        public CompilerResult CompileToSource(string fileName, ICharStream inputStream, TextWriter output, CompilerOptions options)
        {
            var result = new CompilerResult(options);
            CompileToSource(fileName, inputStream, output, result);
            return result;
        }

        private CompilerResultFile CompileToSource(string fileName, ICharStream inputStream, TextWriter output, CompilerResult result)
        {
            var options = result.Options;

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

            bool debugBeginEnd = (options.Flags & CompilerFlags.DebuggingBlockComments) != 0;
            bool debugReturn = (options.Flags & CompilerFlags.DebuggingStatementComments) != 0;
            bool noNamespaces = (options.Flags & CompilerFlags.SkipDefaultNamespaces) != 0;

            using var writer = new SourceWriter(output);
            writer.NewLine = options.NewLine;
            writer.AdjustLines = (options.Flags & CompilerFlags.IgnoreLineNumbers) == 0;
            writer.SkipEmptyLines = !debugBeginEnd;

            var stringBuilder = new StringBuilder();
            var resultFile = new CompilerResultTextFile(fileName, output, stringBuilder);
            result.AddFile(resultFile);

            using var globalWriter = new SourceWriter(new StringWriter(stringBuilder));
            globalWriter.NewLine = options.NewLine;

            if(!debugging)
            {
                // Parse tree shall be kept only in localized cases
                parser.BuildParseTree = false;
            }

            var context = new ScriptEnvironment(parser, writer, globalWriter, lexerContext, debugBeginEnd ? "(*begin*)" : "", debugBeginEnd ? "(*end*)" : "", debugReturn ? "(*return*)" : "", noNamespaces ? Array.Empty<string>() : defaultNamespaces) {
                DefaultNewLine = options.NewLine
            };
            lexerContext.Environment = context;

            // Lexer context is fully set up
            tokenStream.StartReceiving();

            // Main state to process the chunk
            parser.AddParseListener(new ChunkState(context));

            try
            {
                parser.chunk();
            }
            catch(Exception e)
            {
                result.AddDiagnostic(new(e, lexer.Line));
            }

            foreach(var error in context.Errors)
            {
                result.AddDiagnostic(new(error, lexer.Line));
            }

            return resultFile;
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
                result.AddDiagnostic(new(DiagnosticLevel.Error, "PARSER", msg, line, charPositionInLine, e));
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                if(debugging)
                {
                    Debugger.Break();
                }
                result.AddDiagnostic(new(DiagnosticLevel.Error, "LEXER", msg, line, charPositionInLine, e));
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

        static readonly string[] referencedLibraries =
        {
            "Sona.Runtime"
        };

        static readonly string[] defaultNamespaces =
        {
            "System",
            "Sona.Runtime",
            "Sona.Runtime.Core",
            "Sona.Runtime.Computations",
            "Sona.Runtime.Traits"
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

        public CompilerResult CompileToString(string fileName, ICharStream inputStream, CompilerOptions options)
        {
            var result = new CompilerResult(options);
            CompileToString(fileName, inputStream, result);
            return result;
        }

        public CompilerResult CompileToString<TInputs>(TInputs inputs, CompilerOptions options) where TInputs : IReadOnlyCollection<KeyValuePair<string, ICharStream>>
        {
            if(inputs.Count == 0)
            {
                throw new ArgumentException("At least one input is required.", nameof(inputs));
            }

            var result = new CompilerResult(options);
            foreach(var pair in inputs)
            {
                CompileToString(pair.Key, pair.Value, result);
            }
            return result;
        }

        private CompilerResultStringFile CompileToString(string fileName, ICharStream inputStream, CompilerResult result)
        {
            string source;
            CompilerResultFile resultFile;
            using(var sourceWriter = new StringWriter())
            {
                resultFile = CompileToSource(fileName, inputStream, sourceWriter, result);
                sourceWriter.Flush();
                source = sourceWriter.ToString();
            }
            // Replace with string result
            var resultStringFile = new CompilerResultStringFile(resultFile.OriginalFileName, source, resultFile.GlobalCode);
            result.ReplaceFile(resultFile, resultStringFile);
            return resultStringFile;
        }

        private LocalFileSystem CreateFileSystem(CompilerOptions options, out string depsPath)
        {
            // A virtual file prefix (accessed through the local file system) to stand for in-memory dependencies
            var fsPrefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            var fs = new LocalFileSystem(fsPrefix, options.AssemblyLoader);
            depsPath = Path.Combine(fsPrefix, ".deps");
            foreach(var file in embeddedLibraries.Values)
            {
                var path = Path.Combine(depsPath, file);
                fs.InputFiles[path] = LocalInputFile.FromEmbeddedFile(currentAssembly, file);
            }
            return fs;
        }

        private string AddSourceDependency(LocalFileSystem fs, string fileName, string source, out string inputPath)
        {
            // Preserve file name for the top-level module
            var namePrefix = Path.Combine(fs.FileNamePrefix, Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(fileName));
            inputPath = namePrefix + ".fsx";

            fs.InputFiles[inputPath] = source;
            return namePrefix;
        }

        private string AddSourceMain(LocalFileSystem fs, string fileName, string source, out string inputPath, out string outputPath, out string manifestPath)
        {
            var namePrefix = AddSourceDependency(fs, fileName, source, out inputPath);
            outputPath = namePrefix + ".dll";
            manifestPath = namePrefix + ".win32manifest";

            fs.InputFiles[manifestPath] = defaultWin32Manifest;
            return namePrefix;
        }

        public Task<CompilerResult> CompileToMemory(string fileName, ICharStream inputStream, out Stream outputStream, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            return CompileToMemory(new SinglePairDictionary<string, ICharStream>(new(fileName, inputStream)), out outputStream, options, cancellationToken);
        }

        public Task<CompilerResult> CompileToMemory<TInputs>(TInputs inputs, out Stream outputStream, CompilerOptions options, CancellationToken cancellationToken = default) where TInputs : IReadOnlyCollection<KeyValuePair<string, ICharStream>>
        {
            var memoryStream = new BlockBufferStream();
            outputStream = memoryStream;
            try
            {
                return CompileToStream(inputs, new BlockBufferStream(memoryStream), options, cancellationToken);
            }
            catch(Exception e)
            {
                // Expose the stream even for synchronous exceptions
                return Task.FromException<CompilerResult>(e);
            }
        }

        public Task<CompilerResult> CompileToStream(string fileName, ICharStream inputStream, Stream outputStream, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            return CompileToStream(new SinglePairDictionary<string, ICharStream>(new(fileName, inputStream)), outputStream, options, cancellationToken);
        }

        public async Task<CompilerResult> CompileToStream<TInputs>(TInputs inputs, Stream outputStream, CompilerOptions options, CancellationToken cancellationToken = default) where TInputs : IReadOnlyCollection<KeyValuePair<string, ICharStream>>
        {
            if(inputs.Count == 0)
            {
                throw new ArgumentException("At least one input is required.", nameof(inputs));
            }

            var result = new CompilerResult(options);
            var fs = CreateFileSystem(options, out var depsPath);

            var inputPaths = new List<string>(inputs.Count);

            // Last input is the program code
            string mainInputPath = null!, mainSource = null!, outputPath = null!, manifestPath = null!;

            int index = 0;
            foreach(var pair in inputs)
            {
                var fileName = pair.Key;
                var inputStream = pair.Value;
                index++;

                // Finalize code and its prefix statements
                var resultFile = CompileToString(fileName, inputStream, result);
                var source = PrepareCode(resultFile.IntermediateCode, resultFile, options);

                string inputPath;
                if(index == inputs.Count)
                {
                    AddSourceMain(fs, fileName, source, out inputPath, out outputPath, out manifestPath);
                    mainInputPath = inputPath;
                    mainSource = source;
                }
                else
                {
                    AddSourceDependency(fs, fileName, source, out inputPath);
                }
                inputPaths.Add(inputPath);
            }

            fs.OutputFiles[outputPath] = outputStream;

            using var replacedEnv = await FSharpIsolatedEnvironment.CreateAsync(fs, depsPath, cancellationToken);

            var sourceText = SourceText.ofString(mainSource);

            FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;
                
            var (checker, flags) = await GetCheckerAndOptions(mainInputPath, sourceText, manifestPath, options, cancelTokenOption);

            var args = new[]
            {
                "fsc.exe", // ignored
                "--out:" + outputPath
            }.Concat(referencedLibraries.Select(f => $"-r:{Path.Combine(depsPath, f)}.dll"))
            .Concat(flags.OtherOptions)
            .Concat(flags.ReferencedProjects.Select(p => p.OutputFile))
            .Concat(inputPaths) // use actual inputs
            .ToArray();

            var (diagnostics, exception) = await FSharpAsync.StartAsTask(
                checker.Compile(args, userOpName: null),
                taskCreationOptions: null, cancellationToken: cancelTokenOption
            );

            result.Exception = exception?.Value;

            foreach(var diagnostic in diagnostics)
            {
                AddDiagnostic(result, diagnostic);
            }

            return result;
        }

        private static void AddDiagnostic(CompilerResult result, FSharp.Compiler.Diagnostics.FSharpDiagnostic diagnostic)
        {
            var message = CompilerMessages.ProcessDiagnostic(diagnostic.ErrorNumber, diagnostic.Message);
            if(message == null)
            {
                if(diagnostic.Severity.IsError)
                {
                    // Don't ignore non-errors
                    result.AddDiagnostic(new(diagnostic));
                }
                return;
            }
            result.AddDiagnostic(new(message, diagnostic));
        }

        public Task<CompilerResult> CompileToBinary(string fileName, ICharStream inputStream, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            return CompileToStream(fileName, inputStream, new BlockBufferStream(), options, cancellationToken: cancellationToken);
        }

        public Task<CompilerResult> CompileToBinary<TInputs>(TInputs inputs, CompilerOptions options, CancellationToken cancellationToken = default) where TInputs : IReadOnlyCollection<KeyValuePair<string, ICharStream>>
        {
            return CompileToStream(inputs, new BlockBufferStream(), options, cancellationToken: cancellationToken);
        }

        private string PrepareCode(string source, CompilerResultFile resultFile, CompilerOptions options)
        {
            if(resultFile.GlobalCode?.Length > 0)
            {
                var newResult = new StringBuilder();
                newResult.Append(resultFile.GlobalCode);
                newResult.Append(options.NewLine);
                newResult.Append(source);
                newResult.Append(options.NewLine);
                newResult.Append("()");
                return newResult.ToString();
            }
            return source + options.NewLine + "()";
        }

        readonly ConcurrentDictionary<CompilerOptions, FsiEvaluationSession> sessionCache = new();

        private FsiEvaluationSession CheckEvaluation(CompilerResult result, string manifestPath, string depsPath, CompilerOptions options, IReadOnlyList<string> preloadFiles, string mainSource, FSharpOption<CancellationToken>? cancelTokenOption = null)
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

            if(preloadFiles.Count > 0)
            {
                // Non-main files must be referenced via evaluated #load
                var sb = new StringBuilder();
                foreach(var preloadFile in preloadFiles)
                {
                    sb.AppendFormat("#load @\"{0}\"{1}", preloadFile, options.NewLine);
                }

                var (preloadResult, preloadDiagnostics) = session.EvalInteractionNonThrowing(sb.ToString(), cancelTokenOption);

                foreach(var diagnostic in preloadDiagnostics)
                {
                    AddDiagnostic(result, diagnostic);
                }

                if(preloadResult is FSharpChoice<FSharpOption<FsiValue>, Exception>.Choice2Of2 { Item: { } exception })
                {
                    if(GetExceptionDiagnostics(exception) is { Length: > 0 } errors)
                    {
                        foreach(var error in errors)
                        {
                            AddDiagnostic(result, error);
                        }
                    }
                    else
                    {
                        result.AddDiagnostic(new(exception));
                    }
                    result.Success = false;
                    return session;
                }
            }

            // Check main source without evaluating
            var (parseResults, fileResults, projectResults) = session.ParseAndCheckInteraction(mainSource);

            foreach(var diagnostic in parseResults.Diagnostics.Concat(fileResults.Diagnostics).Concat(projectResults.Diagnostics).Distinct())
            {
                AddDiagnostic(result, diagnostic);
            }

            if(projectResults.HasCriticalErrors)
            {
                result.Success = false;
            }
            return session;
        }

        private LocalFileSystem CreateEvaluationFileSystem(CompilerResult result, CompilerOptions options, out string depsPath, out string mainInputPath, out string manifestPath, out IReadOnlyList<string> preloadedFiles, out string mainSource)
        {
            var fs = CreateFileSystem(options, out depsPath);

            var codeFiles = result.CodeFiles;
            if(codeFiles.Count > 1)
            {
                var preload = new List<string>();
                for(int i = 0; i < codeFiles.Count - 1; i++)
                {
                    var stringFile = (CompilerResultStringFile)codeFiles[i];
                    var depSource = PrepareCode(stringFile.IntermediateCode, stringFile, options);
                    AddSourceDependency(fs, stringFile.OriginalFileName, depSource, out var inputPath);
                    preload.Add(inputPath);
                }
                preloadedFiles = preload;
            }
            else
            {
                preloadedFiles = Array.Empty<string>();
            }

            var mainFile = (CompilerResultStringFile)codeFiles[codeFiles.Count - 1];
            mainSource = PrepareCode(mainFile.IntermediateCode, mainFile, options);
            AddSourceMain(fs, mainFile.OriginalFileName, mainSource, out mainInputPath, out _, out manifestPath);

            return fs;
        }

        public Task<CompilerResult> CompileToDelegate(string fileName, ICharStream inputStream, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            return CompileToDelegate(new SinglePairDictionary<string, ICharStream>(new(fileName, inputStream)), options, cancellationToken);
        }

        public async Task<CompilerResult> CompileToDelegate<TInputs>(TInputs inputs, CompilerOptions options, CancellationToken cancellationToken = default) where TInputs : IReadOnlyCollection<KeyValuePair<string, ICharStream>>
        {
            if(inputs.Count == 0)
            {
                throw new ArgumentException("At least one input is required.", nameof(inputs));
            }

            var result = new CompilerResult(options);
            foreach(var pair in inputs)
            {
                CompileToString(pair.Key, pair.Value, result);
            }
            return await CompileToDelegate(result, cancellationToken);
        }

        public async Task<CompilerResult> CompileToDelegate(CompilerResult result, CancellationToken cancellationToken = default)
        {
            var options = result.Options;

            if(result.CodeFiles.Count == 0)
            {
                throw new ArgumentException("Result is missing compilable code.", nameof(result));
            }

            var fs = CreateEvaluationFileSystem(result, options, out var depsPath, out var mainInputPath, out var manifestPath, out var preloadedFiles, out var mainSource);

            using var replacedEnv = await FSharpIsolatedEnvironment.CreateAsync(fs, depsPath, cancellationToken);

            FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;

            var session = CheckEvaluation(result, manifestPath, depsPath, options, preloadedFiles, mainSource, cancelTokenOption);

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
                // Evaluate the main source with dependencies already loaded
                var (evalResult, evalDiagnostics) = session.EvalInteractionNonThrowing(mainSource, mainInputPath, cancelTokenOption);

                if(evalResult is FSharpChoice<FSharpOption<FsiValue>, Exception>.Choice2Of2 { Item: { } exception })
                {
                    // Unwrap exception
                    switch(GetExceptionDiagnostics(exception))
                    {
                        case { Length: > 0 } errors:
                            // Process errors normally
                            var phantomResult = new CompilerResult(result.Options);
                            foreach(var error in errors)
                            {
                                AddDiagnostic(phantomResult, error);
                            }
                            exception = new CompilationException(exception.Message, phantomResult.Diagnostics, exception);
                            break;
                        case { Length: 0 }:
                            exception = new CompilationException(exception.Message, Array.Empty<CompilerDiagnostic>(), exception);
                            break;
                    }
                    return Task.FromException(exception);
                }
                return Task.CompletedTask;
            }
        }

        private FSharp.Compiler.Diagnostics.FSharpDiagnostic[]? GetExceptionDiagnostics(Exception exception)
        {
            if(exception is not FsiCompilationException fsiException)
            {
                return null;
            }
            return fsiException.ErrorInfos?.Value;
        }

        public Task<CompilerResult> Compile(string fileName, ICharStream inputStream, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            if(options.Target == BinaryTarget.Script)
            {
                return CompileToDelegate(fileName, inputStream, options, cancellationToken);
            }
            else
            {
                return CompileToBinary(fileName, inputStream, options, cancellationToken);
            }
        }

        public void CheckResult(CompilerResult result, CompilerOptions options, CancellationToken cancellationToken = default)
        {
            if(result.CodeFiles.Count == 0)
            {
                throw new ArgumentException("Result is missing compilable code.", nameof(result));
            }

            var fs = CreateEvaluationFileSystem(result, options, out var depsPath, out _, out var manifestPath, out var preloadedFiles, out var mainSource);

            using var replacedEnv = FSharpIsolatedEnvironment.Create(fs, depsPath, cancellationToken);
            FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;
            CheckEvaluation(result, manifestPath, depsPath, options, preloadedFiles, mainSource, cancelTokenOption);
        }

        static readonly string[] executableFlags =
        {
            "--subsystemversion:6.00",
            "--standalone",
            "--nointerfacedata",
            "--platform:anycpu",
        };

        static readonly string[] libraryFlags =
        {
            "--nowin32manifest",
            "--targetprofile:netstandard",
            "--platform:anycpu",
        };

        static readonly string[] scriptFlags =
        {
            "--targetprofile:netstandard",
        };

        static readonly string[] commonFlags =
        {
            "--debug:embedded",
            "--deterministic+",

            "--preferreduilang:" + CompilerMessages.Culture.Name,
            //"--flaterrors",
            //"--parallelcompilation",

            "--simpleresolution",

            "--langversion:latest",
            "--nowarn:" + String.Join(",", CompilerMessages.IgnoredWarnings),
            "--warnon:" + String.Join(",", CompilerMessages.EnabledWarnings),
            "--warnaserror+:" + String.Join(",", CompilerMessages.ErrorWarnings),
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
                useTransparentCompiler: null,
                transparentCompilerCacheSizes: null
            );

            var (flags, _) = await FSharpAsync.StartAsTask(checker.GetProjectOptionsFromScript(
                sourceName,
                sourceText,
                caret: null,
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

        readonly record struct SinglePairDictionary<TKey, TValue>(KeyValuePair<TKey, TValue> Pair) : IReadOnlyDictionary<TKey, TValue>
        {
            public TValue this[TKey key] => ContainsKey(key) ? Pair.Value : throw new KeyNotFoundException();

            public IEnumerable<TKey> Keys => new[] { Pair.Key };
            public IEnumerable<TValue> Values => new[] { Pair.Value };
            public int Count => 1;
            public bool ContainsKey(TKey key) => EqualityComparer<TKey>.Default.Equals(key, Pair.Key);
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)new[] { Pair }).GetEnumerator();

#nullable disable
#nullable enable annotations
            public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            {
                if(!ContainsKey(key))
                {
                    value = default;
                    return false;
                }
                value = Pair.Value;
                return true;
            }
#nullable restore

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
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
