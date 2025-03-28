using System;
using System.Globalization;
using Antlr4.Runtime;
using System.IO;
using Sona.Compiler;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

internal class Program
{
    private static async Task Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        if(args.Length is not (1 or 2))
        {
            Console.Error.WriteLine($"Usage: {Environment.GetCommandLineArgs().FirstOrDefault()} input [output]");
            return;
        }

        var compiler = new SonaCompiler();

        var options = new CompilerOptions
        (
            Target: BinaryTarget.Exe,
            AssemblyLoadContext: AssemblyLoadContext.Default,
            Flags: CompilerFlags.Privileged
        );

        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
            if(SonaCompiler.ResolveEmbeddedAssembly(args.Name) is not { } stream)
            {
                return null;
            }
            return options.AssemblyLoadContext.LoadFromStream(stream);
        };

        var backColor = Console.BackgroundColor;
        var textColor = Console.ForegroundColor;

        while(true)
        {
            Func<Task>? entryPoint;
            try
            {
                CompilerResult result;
                bool compileToFile = args.Length == 2;

                var inputPath = args[0];
                using(var inputFile = File.OpenText(inputPath))
                {
                    var inputStream = new AntlrInputStream(inputFile);

                    WriteLine(ConsoleColor.Gray, "Compiling...");

                    if(compileToFile)
                    {
                        var outputPath = args[1];
                        using var outputFile = File.Create(outputPath);
                        result = await compiler.CompileToStream(inputStream, outputPath, outputFile, options);
                    }
                    else
                    {
                        result = await compiler.CompileToDelegate(inputStream, Guid.NewGuid().ToString(), options with { Target = BinaryTarget.Script });
                    }
                }

                foreach(var diagnostic in result.Diagnostics)
                {
#pragma warning disable CS8524
                    WriteLine(diagnostic.Level switch
                    {
                        DiagnosticLevel.Info => ConsoleColor.Cyan,
                        DiagnosticLevel.Warning => ConsoleColor.Yellow,
                        DiagnosticLevel.Error => ConsoleColor.Red
                    }, diagnostic.ToString());
#pragma warning restore CS8524
                }

                if(!result.Success)
                {
                    WriteLine(ConsoleColor.White, "Compilation unsuccessful! Press any key to retry.");
                    Console.ReadKey(true);
                    continue;
                }

                if(compileToFile)
                {
                    WriteLine(ConsoleColor.White, "Done! Press any key to retry.");
                    Console.ReadKey(true);
                    continue;
                }

                entryPoint = result.EntryPoint ?? throw new ApplicationException("The generated assembly is missing an entry point.");
                WriteLine(ConsoleColor.White, "Done!");
            }
            catch(Exception e) when(!Debugger.IsAttached)
            {
                WriteLine(ConsoleColor.Red, "Compiler error: " + e.Message);
                WriteLine(ConsoleColor.White, "Press any key to retry.");
                Console.ReadKey(true);
                continue;
            }

            try
            {
                await entryPoint();
            }
            catch(Exception e)
            {
                WriteLine(ConsoleColor.Red, e.ToString());
            }
            finally
            {
                Console.ForegroundColor = textColor;
                Console.BackgroundColor = backColor;
            }

            WriteLine(ConsoleColor.White, "Press any key to retry.");
            Console.ReadKey(true);

            void WriteLine(ConsoleColor color, string text)
            {
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
    }
}