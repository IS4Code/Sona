using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Sona.Compiler;

internal sealed class Program(string[] inputs, (string path, BinaryTarget target)? output, bool check, bool ignoreLines, bool emitMarkers, bool emitHeaders, bool quiet, string newline, bool optimize, bool unprivileged, bool skipNamespaces, bool interactive)
{
    private static async Task<int> Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        if(args.Length == 0)
        {
            // Show help by default
            args = new[] { "--help" };
        }

        var inputsArgument = new Argument<string[]>("input")
        {
            Description = "Sona source code paths.",
            Arity = ArgumentArity.ZeroOrMore
        };

        var outputOption = new Option<string?>("--out", "--output", "-o")
        {
            Description = "Compiled output path."
        };

        var targetOption = new Option<BinaryTarget?>("--target", "-t")
        {
            Description = "The output format (exe|winexe|library|module|fs).",
            HelpName = "kind"
        };
        targetOption.CustomParser = ParseTarget;

        var checkOption = new Option<bool>("--check", "-c")
        {
            Description = "Run type-check over the output."
        };
        var noLinesOption = new Option<bool>("--no-lines", "-L")
        {
            Description = "Strip F# line-number directives."
        };
        var emitMarkersOption = new Option<bool>("--emit-markers", "-M")
        {
            Description = "Emit diagnostic block and statement markers."
        };
        var emitHeadersOption = new Option<bool>("--emit-headers", "-H")
        {
            Description = "Emit section headers and per-file comments."
        };
        var quietOption = new Option<bool>("--quiet", "-q")
        {
            Description = "Suppress progress messages and diagnostics."
        };
        var newlineOption = new Option<string>("--newline", "-n")
        {
            Description = "Newline sequence for emitted F#.",
            DefaultValueFactory = _ => Environment.NewLine
        };
        var optimizeOption = new Option<bool>("--optimize", "-O")
        {
            Description = "Enable F# optimizations."
        };
        var unprivilegedOption = new Option<bool>("--unprivileged", "-u")
        {
            Description = "Compile without support for unverifiable code and reflection calls."
        };
        var skipNamespacesOption = new Option<bool>("--skip-namespaces")
        {
            Description = "Don't open the default namespaces."
        };
        var interactiveOption = new Option<bool>("--interactive", "-i")
        {
            Description = "Wait for keypress to retry."
        };

        var root = new RootCommand("Compiles or runs Sona source.") {
            inputsArgument,
            outputOption,
            targetOption,
            checkOption,
            noLinesOption,
            emitMarkersOption,
            emitHeadersOption,
            quietOption,
            newlineOption,
            optimizeOption,
            unprivilegedOption,
            skipNamespacesOption,
            interactiveOption
        };

        root.SetAction(parseResult => new Program(
            parseResult.GetValue(inputsArgument) ?? Array.Empty<string>(),
            GetOutput(parseResult.GetValue(outputOption), parseResult.GetValue(targetOption)),
            parseResult.GetValue(checkOption),
            parseResult.GetValue(noLinesOption),
            parseResult.GetValue(emitMarkersOption),
            parseResult.GetValue(emitHeadersOption),
            parseResult.GetValue(quietOption),
            parseResult.GetValue(newlineOption) ?? Environment.NewLine,
            parseResult.GetValue(optimizeOption),
            parseResult.GetValue(unprivilegedOption),
            parseResult.GetValue(skipNamespacesOption),
            parseResult.GetValue(interactiveOption) && !Console.IsInputRedirected
        ).RunAsync());

        var parseResult = root.Parse(args);

        if(FindUnrecognizedOptions(parseResult, inputsArgument) is { Count: > 0 } options)
        {
            foreach(var option in options)
            {
                Console.Error.WriteLine($"Unrecognized option '{option}'.");
            }
            Console.Error.WriteLine("Try 'sonac --help' for usage.");
            return 2;
        }

        int exitCode = await parseResult.InvokeAsync();

        // Remap parse error to 2
        if(exitCode == 1 && parseResult.Errors.Count > 0)
        {
            exitCode = 2;
        }
        return exitCode;
    }

    private static List<string>? FindUnrecognizedOptions(ParseResult parseResult, Argument<string[]> inputsArgument)
    {
        var inputTokens = new HashSet<Token>(parseResult.GetResult(inputsArgument)?.Tokens ?? Array.Empty<Token>());
        List<string>? list = null;
        foreach(var token in parseResult.Tokens)
        {
            if(token.Type == TokenType.DoubleDash)
            {
                // Inputs separator
                break;
            }
            if(inputTokens.Contains(token) && token.Value.Length > 1 && token.Value[0] == '-')
            {
                // Option-like token
                (list ??= new()).Add(token.Value);
            }
        }
        return list;
    }

    private static BinaryTarget? ParseTarget(ArgumentResult result)
    {
        if(result.Tokens.Count == 0)
        {
            return null;
        }
        var token = result.Tokens[0].Value;
        switch(token.ToLowerInvariant())
        {
            case "exe": return BinaryTarget.Exe;
            case "winexe": return BinaryTarget.WinExe;
            case "library" or "dll": return BinaryTarget.Library;
            case "module" or "netmodule": return BinaryTarget.Module;
            case "fs" or "fsharp": return BinaryTarget.Script;
            default:
                result.AddError($"Unrecognized target '{token}'. Expected exe, winexe, library, module or fs.");
                return null;
        }
    }

    private static (string path, BinaryTarget target)? GetOutput(string? path, BinaryTarget? target)
    {
        return (path, target) switch {
            // None given
            (null, null) => null,
            // Prevent running
            _ => (path ?? "-", target ?? BinaryTarget.Exe)
        };
    }

    private readonly SonaCompiler compiler = new SonaCompiler();

    private async Task<int> RunAsync()
    {
        if(output is ("-", not BinaryTarget.Script) && !Console.IsOutputRedirected)
        {
            // Prevent writing raw data to the terminal
            WriteLine(ConsoleColor.Red, "Refusing to write binary output to a terminal; redirect standard output or use --out <file>.");
            return 2;
        }

        var flags = default(CompilerFlags)
            | (optimize ? CompilerFlags.Optimize : 0)
            | (unprivileged ? 0 : CompilerFlags.Privileged)
            | (ignoreLines ? CompilerFlags.IgnoreLineNumbers : 0)
            | (emitMarkers ? CompilerFlags.DebuggingComments : 0)
            | (skipNamespaces ? CompilerFlags.SkipDefaultNamespaces : 0);

        var options = new CompilerOptions(
            Target: output?.target ?? BinaryTarget.Script,
            AssemblyLoader:
#if NETCOREAPP
                AssemblyContextLoader.Default,
#else
                AppDomainAssemblyLoader.Instance,
#endif
            Flags: flags
        ) {
            NewLine = newline
        };

        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
            if(SonaCompiler.ResolveEmbeddedAssembly(args.Name) is not { } stream)
            {
                return null;
            }
            return options.AssemblyLoader.AssemblyLoadStream(stream);
        };

        bool success = output switch {
            // Without output, run the input
            null => await MainLoop(RunStep, options, run: !check),
            // With F# target, just emit
            (var path, BinaryTarget.Script) => await MainLoop(o => EmitStep(o, path), options),
            // Normal compile
            (var path, _) => await MainLoop(o => CompileStep(o, path), options)
        };

        return success ? 0 : 1;
    }

    private async Task<bool> MainLoop(Func<CompilerOptions, Task<CompilerResult>> action, CompilerOptions options, bool run = false)
    {
        while(true)
        {
            CompilerResult result;
            try
            {
                WriteLine(ConsoleColor.Gray, "Compiling...");

                result = await action(options);

                foreach(var diagnostic in result.Diagnostics)
                {
                    // Ignore SDK version diagnostics
                    if(diagnostic.Code == "FS3384") continue;
#pragma warning disable CS8524
                    WriteLine(diagnostic.Level switch {
                        DiagnosticLevel.Info => ConsoleColor.Cyan,
                        DiagnosticLevel.Warning => ConsoleColor.Yellow,
                        DiagnosticLevel.Error => ConsoleColor.Red
                    }, diagnostic.ToString());
#pragma warning restore CS8524
                }

                if(!result.Success)
                {
                    WriteLine(ConsoleColor.White, "Compilation unsuccessful!");
                }
                else
                {
                    if(run)
                    {
                        var backColor = Console.BackgroundColor;
                        var textColor = Console.ForegroundColor;
                        try
                        {
                            var entryPoint = result.EntryPoint ?? throw new ApplicationException("The generated assembly is missing an entry point.");
                            await entryPoint();
                        }
                        catch(Exception e)
                        {
                            WriteLine(ConsoleColor.Red, e.ToString());
                            if(!interactive)
                            {
                                return false;
                            }
                        }
                        finally
                        {
                            Console.ForegroundColor = textColor;
                            Console.BackgroundColor = backColor;
                        }
                    }
                    WriteLine(ConsoleColor.White, "Done!");
                }
            }
            catch(Exception e) when(!Debugger.IsAttached)
            {
                WriteLine(ConsoleColor.Red, "Compiler error: " + e.Message);
                return false;
            }

            if(!interactive)
            {
                return result.Success;
            }

            WriteLine(ConsoleColor.White, "Press any key to retry.");
            Console.ReadKey(true);
        }
    }

    private string? cachedStdin;
    private List<KeyValuePair<string, ICharStream>> GetInputStreams()
    {
        var paths = inputs.Length == 0 ? new[] { "-" } : inputs;
        var result = new List<KeyValuePair<string, ICharStream>>(paths.Length);
        foreach(var path in paths)
        {
            if(path == "-")
            {
                result.Add(new("-", CharStreams.fromString(cachedStdin ??= Console.In.ReadToEnd())));
            }
            else
            {
                result.Add(new(path, CharStreams.fromPath(path)));
            }
        }
        return result;
    }

    private async Task<CompilerResult> EmitStep(CompilerOptions options, string path)
    {
        var output = path == "-" ? Console.Out : new StreamWriter(path) { NewLine = options.NewLine };
        try
        {
            var result = compiler.CompileToString(GetInputStreams(), options);

            var codeFiles = result.CodeFiles;
            bool multiple = codeFiles.Count > 1;

            foreach(var file in codeFiles)
            {
                var stringFile = (CompilerResultStringFile)file;
                if(multiple && emitHeaders)
                {
                    output.WriteLine($"(* {Path.GetFileName(stringFile.OriginalFileName)} *)");
                }
                if(stringFile.GlobalCode is { Length: > 0 } global)
                {
                    if(emitHeaders)
                    {
                        output.WriteLine("// global");
                    }
                    output.Write(global.ToString());
                    if(emitHeaders)
                    {
                        output.WriteLine();
                        output.WriteLine("// body");
                    }
                }
                output.WriteLine(stringFile.IntermediateCode);
            }

            if(check)
            {
                result = await compiler.CompileToDelegate(result);
            }

            return result;
        }
        finally
        {
            if(!Object.ReferenceEquals(output, Console.Out))
            {
                output.Dispose();
            }
        }
    }

    private async Task<CompilerResult> CompileStep(CompilerOptions options, string outputPath)
    {
        if(outputPath == "-")
        {
            var task = compiler.CompileToMemory(GetInputStreams(), out var memoryStream, options);
            using(memoryStream)
            {
                var result = await task;

                if(result.Success)
                {
                    // Redirect to stdout when finished
                    memoryStream.Position = 0;
                    using var stdout = Console.OpenStandardOutput();
                    await memoryStream.CopyToAsync(stdout);
                }

                return result;
            }
        }
        else
        {
            using var outputFile = File.Create(outputPath);
            return await compiler.CompileToStream(GetInputStreams(), outputFile, options);
        }
    }

    private Task<CompilerResult> RunStep(CompilerOptions options)
    {
        return compiler.CompileToDelegate(GetInputStreams(), options);
    }

    private void WriteLine(ConsoleColor color, string text)
    {
        if(quiet)
        {
            return;
        }
        var textColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        try
        {
            Console.Error.WriteLine(text);
        }
        finally
        {
            Console.ForegroundColor = textColor;
        }
    }
}
