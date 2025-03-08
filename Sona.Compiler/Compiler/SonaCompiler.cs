using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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

namespace IS4.Sona.Compiler
{
    public class SonaCompiler
    {
        public bool AdjustLines { get; set; } = true;
        public bool ShowBeginEnd { get; set; } = false;

        static readonly bool debugging =
#if DEBUG
            Debugger.IsAttached
#else
            false
#endif
            ;

        public void CompileToSource(ICharStream inputStream, TextWriter output, bool throwOnError = false)
        {
            var errorListener = throwOnError ? new ErrorListener() : null;

            var lexer = GetLexer(inputStream);
            errorListener?.AddTo(lexer);

            var channelContext = new LexerContext(lexer);

            var tokenStream = new UnbufferedListenerTokenStream(lexer, channelContext.OnLexerToken);
            var parser = new SonaParser(tokenStream);
            errorListener?.AddTo(parser);

            bool debugBeginEnd = ShowBeginEnd;

            using var writer = new SourceWriter(output);
            writer.AdjustLines = AdjustLines;
            writer.SkipEmptyLines = !debugBeginEnd;

            if(!debugging)
            {
                // Parse tree shall be kept only in localized cases
                parser.BuildParseTree = false;
            }

            var context = new ScriptEnvironment(parser, writer, channelContext, debugBeginEnd ? "(*begin*)" : "", debugBeginEnd ? "(*end*)" : "");

            // Main state to process the chunk
            parser.AddParseListener(new ChunkState(context));

            parser.chunk();

            if(errorListener?.HasErrors ?? false)
            {
                throw new ArgumentException("The input is syntactically invalid.", nameof(inputStream));
            }
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

        sealed class ErrorListener : IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
        {
            public bool HasErrors { get; private set; }

            public void AddTo<TInterpreter>(Recognizer<IToken, TInterpreter> recognizer) where TInterpreter : Antlr4.Runtime.Atn.ATNSimulator
            {
                recognizer.AddErrorListener(this);
            }

            public void AddTo<TInterpreter>(Recognizer<int, TInterpreter> recognizer) where TInterpreter : Antlr4.Runtime.Atn.ATNSimulator
            {
                recognizer.AddErrorListener(this);
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                HasErrors = true;
                if(debugging)
                {
                    Debugger.Break();
                }
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                HasErrors = true;
                if(debugging)
                {
                    Debugger.Break();
                }
            }
        }

        static readonly string[] embeddedFiles = { "FSharp.Core.dll", "Sona.Runtime.dll" };

        public async Task CompileToStream(AntlrInputStream inputStream, string fileName, Stream outputStream, bool isExecutable, AssemblyLoadContext assemblyLoadContext, CancellationToken cancellationToken = default)
        {
            string source;
            using(var sourceWriter = new StringWriter())
            {
                CompileToSource(inputStream, sourceWriter);
                sourceWriter.Flush();
                source = sourceWriter.ToString();
            }

            // A virtual file prefix (accessed through the local file system) to stand for in-memory dependencies
            var fsPrefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(fileName));
            
            var fs = new LocalFileSystem(fsPrefix, assemblyLoadContext);
            var fsxName = fsPrefix + ".fsx";
            var dllPath = fsPrefix + ".dll";
            var manifestPath = fsPrefix + ".win32manifest";

            fs.InputFiles[fsxName] = source;
            fs.InputFiles[manifestPath] = defaultWin32Manifest;

            var depsPath = fsPrefix + ".deps";
            var assembly = typeof(SonaCompiler).Assembly;
            foreach(var file in embeddedFiles)
            {
                var path = Path.Combine(depsPath, file);
                fs.InputFiles[path] = LocalInputFile.FromEmbeddedFile(assembly, file);
            }

            fs.OutputFiles[dllPath] = outputStream;

            using(var replacedFs = new FileSystemReplacement(fs))
            {
                var sourceText = SourceText.ofString(source);

                FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;
                
                var (checker, options) = await GetCheckerAndOptions(fsxName, sourceText, isExecutable, depsPath, cancelTokenOption);

                var args = new[]
                {
                    "fsc.exe", // ignored
                    "--win32manifest:" + manifestPath,
                    "--out:" + dllPath
                }.Concat(embeddedFiles.Select(f => "-r:" + Path.Combine(depsPath, f)))
                .Concat(options.OtherOptions)
                .Concat(options.ReferencedProjects.Select(p => p.OutputFile))
                .Concat(options.SourceFiles)
                .ToArray();

                var (diagnostics, exitCode) = await FSharpAsync.StartAsTask(
                    checker.Compile(args, userOpName: null),
                    taskCreationOptions: null, cancellationToken: cancelTokenOption
                );

                if(exitCode != 0)
                {
                    throw new SystemException($"The compilation failed with code {exitCode}: " + String.Join(Environment.NewLine, diagnostics.Select(d => d.ToString())));
                }
            }
        }

        public async Task<Stream> CompileToBinary(AntlrInputStream inputStream, string fileName, bool isExecutable, AssemblyLoadContext assemblyLoadContext, CancellationToken cancellationToken = default)
        {
            var stream = new BlockBufferStream();
            await CompileToStream(inputStream, fileName, stream, isExecutable, assemblyLoadContext, cancellationToken: cancellationToken);
            return stream;
        }

        public async Task<Assembly> CompileToAssembly(AntlrInputStream inputStream, string fileName, AssemblyLoadContext assemblyLoadContext, CancellationToken cancellationToken = default)
        {
            var outFile = await CompileToBinary(inputStream, fileName, false, assemblyLoadContext, cancellationToken: cancellationToken);
            return assemblyLoadContext.LoadFromStream(outFile);
        }

        static readonly string[] executableFlags = new[] {
            "--target:exe",
            "--subsystemversion:6.00",
            "--platform:x64",
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

            "--preferreduilang:en",
            //"--flaterrors",
            //"--parallelcompilation",

            "--reflectionfree",
            "--simpleresolution",
            "--optimize+",

            "--langversion:latest",
            "--warnon:21,52,1178,1182,3387,3388,3389,3397,3390,3517,3559",
            "--warnaserror+:20,25,3517",
            "--checknulls+",
        };

        static readonly FSharpOption<string[]> executableCheckerFlags = executableFlags.Concat(commonFlags).ToArray();
        static readonly FSharpOption<string[]> libraryCheckerFlags = libraryFlags.Concat(commonFlags).ToArray();

        static readonly SemaphoreSlim environmentSemaphore = new SemaphoreSlim(1, 1);

        private async Task<(FSharpChecker, FSharpProjectOptions)> GetCheckerAndOptions(string sourceName, ISourceText sourceText, bool isExecutable, string depsPath, FSharpOption<CancellationToken>? cancellationToken)
        {
            // Does not appear to be used at all
            var documentSource = GetDocumentSource(sourceName, sourceText);

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

                    var (options, _) = await FSharpAsync.StartAsTask(checker.GetProjectOptionsFromScript(
                        sourceName,
                        sourceText,
                        previewEnabled: null,
                        loadedTimeStamp: null,
                        otherFlags: isExecutable ? executableCheckerFlags : libraryCheckerFlags,
                        useFsiAuxLib: false,
                        useSdkRefs: false,
                        assumeDotNetFramework: isExecutable,
                        sdkDirOverride: null,
                        optionsStamp: null,
                        userOpName: null
                    ), taskCreationOptions: null, cancellationToken: cancellationToken);

                    return (checker, options);
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
