using System;

namespace Sona.Compiler
{
    public record CompilerOptions(BinaryTarget Target, CompilerFlags Flags, ICompilerAssemblyLoader AssemblyLoader);

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
