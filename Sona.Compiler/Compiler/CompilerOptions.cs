using System;
using System.Runtime.Loader;

namespace IS4.Sona.Compiler
{
    public record CompilerOptions(BinaryTarget Target, CompilerFlags Flags, AssemblyLoadContext AssemblyLoadContext);

    public enum BinaryTarget
    {
        Exe,
        WinExe,
        Library,
        Module,
        Script
    }

    [Flags]
    public enum CompilerFlags
    {
        Optimize = 1,
        Privileged = 2,
        IgnoreLineNumbers = 4,
        DebuggingComments = 8
    }
}
