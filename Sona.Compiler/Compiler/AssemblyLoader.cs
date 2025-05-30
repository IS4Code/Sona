using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using FSharp.Compiler.IO;

namespace Sona.Compiler
{
    public interface ICompilerAssemblyLoader : IAssemblyLoader
    {
        Assembly AssemblyLoadStream(Stream assembly);
    }

    public sealed class AppDomainAssemblyLoader : ICompilerAssemblyLoader
    {
        public static readonly AppDomainAssemblyLoader Instance = new();

        private AppDomainAssemblyLoader()
        {

        }

        public Assembly AssemblyLoad(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        public Assembly AssemblyLoadFrom(string assemblyPath)
        {
            return Assembly.LoadFrom(assemblyPath);
        }

        public Assembly AssemblyLoadStream(Stream assembly)
        {
            if(assembly is MemoryStream memoryStream)
            {
                return Assembly.Load(memoryStream.ToArray());
            }
            var buffer = new MemoryStream();
            assembly.CopyTo(buffer);
            return Assembly.Load(buffer.ToArray());
        }
    }

#if NET6_0_OR_GREATER
    public record AssemblyContextLoader(AssemblyLoadContext LoadContext) : ICompilerAssemblyLoader
    {
        public static AssemblyContextLoader Default { get; } = new(AssemblyLoadContext.Default);

        public Assembly AssemblyLoad(AssemblyName assemblyName)
        {
            return LoadContext.LoadFromAssemblyName(assemblyName);
        }

        public Assembly AssemblyLoadFrom(string assemblyPath)
        {
            return LoadContext.LoadFromAssemblyPath(assemblyPath);
        }

        public Assembly AssemblyLoadStream(Stream assembly)
        {
            return LoadContext.LoadFromStream(assembly);
        }
    }
#endif
}
