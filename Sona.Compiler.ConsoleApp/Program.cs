using System;
using System.Globalization;
using Antlr4.Runtime;
using System.IO;
using IS4.Sona.Compiler;
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

        while(true)
        {
            MethodInfo entryPoint;
            try
            {
                var compiler = new SonaCompiler();
                compiler.AdjustLines = true;
                compiler.ShowBeginEnd = false;

                var inputPath = args[0];
                using(var inputFile = File.OpenText(inputPath))
                {
                    var inputStream = new AntlrInputStream(inputFile);

                    if(args.Length == 2)
                    {
                        var outputPath = args[1];
                        using var outputFile = File.Create(outputPath);
                        await compiler.CompileToStream(inputStream, outputPath, outputFile, true, AssemblyLoadContext.Default);
                        Console.Error.WriteLine("Done! Press any key to retry.");
                        continue;
                    }
                    else
                    {
                        var assembly = await compiler.CompileToAssembly(inputStream, inputPath, AssemblyLoadContext.Default);
                        entryPoint = assembly.EntryPoint ?? throw new ApplicationException("The generated assembly is missing an entry point.");
                        Console.Error.WriteLine("Done! Press any key to execute.");
                    }
                }
            }
            catch(Exception e) when(!Debugger.IsAttached)
            {
                Console.Error.WriteLine(e.Message);
                continue;
            }
            finally
            {
                Console.ReadKey(true);
            }

            if(entryPoint.Invoke(null, null) is int exitCode)
            {
                Environment.Exit(exitCode);
            }
        }
    }
}