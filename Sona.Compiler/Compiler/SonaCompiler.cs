using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.IO;
using FSharp.Compiler.Text;
using IS4.Sona.Compiler.States;
using IS4.Sona.Compiler.Tools;
using IS4.Sona.Grammar;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;

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

            parser.chunk();

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

        static readonly Dictionary<string, string> embeddedFiles = new(StringComparer.OrdinalIgnoreCase)
        {
            { "FSharp.Core", "FSharp.Core.dll" },
            { "Sona.Runtime", "Sona.Runtime.dll" }
        };

        static readonly Assembly currentAssembly = typeof(SonaCompiler).Assembly;

        public static Stream? ResolveEmbeddedAssembly(string? name)
        {
            if(name is null)
            {
                return null;
            }
            name = new AssemblyName(name).Name;
            if(name is null || !embeddedFiles.TryGetValue(name, out var file))
            {
                return null;
            }
            return currentAssembly.GetManifestResourceStream(file);
        }

        public async Task<CompilerResult> CompileToStream(AntlrInputStream inputStream, string fileName, Stream outputStream, CompilerOptions options, CancellationToken cancellationToken = default)
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
            result.Stream = outputStream;

            // A virtual file prefix (accessed through the local file system) to stand for in-memory dependencies
            var fsPrefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(fileName));
            
            var fs = new LocalFileSystem(fsPrefix, options.AssemblyLoadContext);
            var fsxName = fsPrefix + ".fsx";
            var dllPath = fsPrefix + ".dll";
            var manifestPath = fsPrefix + ".win32manifest";

            fs.InputFiles[fsxName] = source;
            fs.InputFiles[manifestPath] = defaultWin32Manifest;

            var depsPath = fsPrefix + ".deps";
            foreach(var file in embeddedFiles.Values)
            {
                var path = Path.Combine(depsPath, file);
                fs.InputFiles[path] = LocalInputFile.FromEmbeddedFile(currentAssembly, file);
            }

            fs.OutputFiles[dllPath] = outputStream;

            using var replacedFs = new FileSystemReplacement(fs);

            var sourceText = SourceText.ofString(source);

            FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;
                
            var (checker, flags) = await GetCheckerAndOptions(fsxName, sourceText, depsPath, manifestPath, options, cancelTokenOption);

            var args = new[]
            {
                "fsc.exe", // ignored
                "--out:" + dllPath
            }.Concat(embeddedFiles.Values.Select(f => "-r:" + Path.Combine(depsPath, f)))
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

        static readonly string[] executableFlags = new[] {
            "--subsystemversion:6.00",
            "--standalone",
            "--nointerfacedata",
        };

        static readonly string[] libraryFlags = new[] {
            "--nowin32manifest",
            "--targetprofile:netstandard",
        };

        static readonly string[] commonFlags = new[] {
            "--debug:embedded",
            "--deterministic+",
            "--platform:anycpu",

            "--preferreduilang:en",
            //"--flaterrors",
            //"--parallelcompilation",

            "--simpleresolution",

            "--langversion:latest",
            "--nowarn:3220",
            "--warnon:21,52,1178,1182,3387,3388,3389,3397,3390,3517,3559",
            "--warnaserror+:20,25,193,3517",
            "--checknulls+",
        };

        static readonly SemaphoreSlim environmentSemaphore = new SemaphoreSlim(1, 1);

        private async Task<(FSharpChecker, FSharpProjectOptions)> GetCheckerAndOptions(string sourceName, ISourceText sourceText, string depsPath, string manifestPath, CompilerOptions options, FSharpOption<CancellationToken>? cancellationToken)
        {
            // Does not appear to be used at all
            var documentSource = GetDocumentSource(sourceName, sourceText);

            var allFlags = new List<string>();
            if(options.Target != BinaryTarget.Method)
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
            if(options.Target is BinaryTarget.Exe or BinaryTarget.WinExe)
            {
                allFlags.AddRange(executableFlags);
                allFlags.Add($"--win32manifest:{manifestPath}");
            }
            else
            {
                allFlags.AddRange(libraryFlags);
            }
            allFlags.AddRange(commonFlags);

            if((options.Flags & CompilerFlags.Privileged) == 0)
            {
                allFlags.Add("--reflectionfree");
            }

            await environmentSemaphore.WaitAsync();
            try
            {
                var previousBin = Environment.GetEnvironmentVariable("FSHARP_COMPILER_BIN");
                Environment.SetEnvironmentVariable("FSHARP_COMPILER_BIN", depsPath);
                try
                {
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
                finally
                {
                    Environment.SetEnvironmentVariable("FSHARP_COMPILER_BIN", previousBin);
                }
            }
            finally
            {
                environmentSemaphore.Release();
            }
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

        readonly struct FileSystemReplacement : IDisposable
        {
            static readonly Type fileSystemLock = typeof(FileSystemAutoOpens);

            readonly IFileSystem previous;

            public FileSystemReplacement(IFileSystem newFileSystem)
            {
                lock(fileSystemLock)
                {
                    previous = FileSystemAutoOpens.FileSystem;
                    FileSystemAutoOpens.FileSystem = newFileSystem;
                }
            }

            public void Dispose()
            {
                lock(fileSystemLock)
                {
                    FileSystemAutoOpens.FileSystem = previous;
                }
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
