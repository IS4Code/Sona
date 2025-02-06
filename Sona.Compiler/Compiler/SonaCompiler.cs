using System;
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

        public void CompileToSource(ICharStream inputStream, TextWriter output, bool throwOnError = false)
        {
            var errorListener = throwOnError ? new ErrorListener() : null;

            var lexer = new SonaLexer(inputStream);
            errorListener?.AddTo(lexer);

            // Add empty mode below, to handle close parenthesis
            int emptyMode = Array.IndexOf(lexer.ModeNames, "Empty");
            int defaultMode = lexer.CurrentMode;
            lexer.Mode(emptyMode);
            lexer.PushMode(defaultMode);

            var channelContext = new LexerContext(lexer);

            var tokenStream = new UnbufferedListenerTokenStream(lexer, channelContext.OnLexerToken);
            var parser = new SonaParser(tokenStream);
            errorListener?.AddTo(parser);

            using var writer = new SourceWriter(output);
            writer.AdjustLines = AdjustLines;

            var context = new ScriptEnvironment(parser, writer, channelContext);

            // Main state to process the chunk
            parser.AddParseListener(new ChunkState(context));

            // Parse tree shall be kept only in localized cases
            parser.BuildParseTree = false;

            parser.chunk();

            if(errorListener?.HasErrors ?? false)
            {
                throw new ArgumentException("The input is syntactically invalid.", nameof(inputStream));
            }
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
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                HasErrors = true;
            }
        }

        public async Task<Assembly> CompileToAssembly(AntlrInputStream inputStream, string fileName, AssemblyLoadContext assemblyLoadContext, CancellationToken cancellationToken = default)
        {
            string source;
            using(var sourceWriter = new StringWriter())
            {
                CompileToSource(inputStream, sourceWriter);
                sourceWriter.Flush();
                source = sourceWriter.ToString();
            }

            // A virtual file (accessed through the local file system) to stand for the source
            var fsPrefix = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(fileName));
            
            var fs = new LocalFileSystem(fsPrefix);
            var fsxName = fsPrefix + ".fsx";
            var dllPath = fsPrefix + ".dll";

            fs.InputFiles[fsxName] = source;

            using(var replacedFs = new FileSystemReplacement(fs))
            {
                var sourceText = SourceText.ofString(source);

                FSharpOption<CancellationToken>? cancelTokenOption = cancellationToken.CanBeCanceled ? cancellationToken : null;
                var (checker, options) = await GetCheckerAndOptions(fsxName, sourceText, cancelTokenOption);
                
                var args = new[]
                {
                    "fsc.exe", // ignored
                    "--nowin32manifest",
                    "--reflectionfree",
                    "--warnon:21,52,1182,3517",
                    "--checknulls+",
                    "--preferreduilang:",
                    "--debug:embedded",
                    "--out:" + dllPath
                }.Concat(options.OtherOptions)
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

            // Retrieved the compiled file
            var outFile = fs.OutputFiles[dllPath];
            if(!outFile.TryGetBuffer(out var outBuffer))
            {
                outBuffer = outFile.ToArray();
            }
            outFile = new MemoryStream(outBuffer.Array!, outBuffer.Offset, outBuffer.Count, false);
            return assemblyLoadContext.LoadFromStream(outFile);
        }

        private async Task<(FSharpChecker, FSharpProjectOptions)> GetCheckerAndOptions(string sourceName, ISourceText sourceText, FSharpOption<CancellationToken>? cancellationToken)
        {
            // Does not appear to be used at all
            var documentSource = GetDocumentSource(sourceName, sourceText);

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
                otherFlags: null,
                useFsiAuxLib: false,
                useSdkRefs: false,
                assumeDotNetFramework: false,
                sdkDirOverride: null,
                optionsStamp: null,
                userOpName: null
            ), taskCreationOptions: null, cancellationToken: cancellationToken);

            return (checker, options);
        }

        static readonly FSharpAsync<FSharpOption<ISourceText>?> nullSourceTextAync = FSharpAsync.AwaitTask(Task.FromResult<FSharpOption<ISourceText>?>(
            null
        ));
        private DocumentSource GetDocumentSource(string name, ISourceText source)
        {
            var sourceAsync = FSharpAsync.AwaitTask(Task.FromResult<FSharpOption<ISourceText>?>(
                FSharpOption<ISourceText>.Some(source)
            ));
            var func = FSharpFunc<string, FSharpAsync<FSharpOption<ISourceText>?>>.FromConverter(
                path => path == name ? sourceAsync : nullSourceTextAync
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
    }
}
