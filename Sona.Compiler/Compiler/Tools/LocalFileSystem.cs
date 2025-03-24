using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using FSharp.Compiler.IO;
using Microsoft.FSharp.Core;

namespace Sona.Compiler.Tools
{
    internal sealed class LocalFileSystem : DefaultFileSystem, IAssemblyLoader
    {
        readonly ConcurrentDictionary<string, Stream> outputFiles = new();
        public Dictionary<string, LocalInputFile> InputFiles { get; } = new();
        public IDictionary<string, Stream> OutputFiles => outputFiles;

        public string FileNamePrefix { get; }

        readonly AssemblyLoadContext assemblyLoadContext;

        public LocalFileSystem(string fileNamePrefix, AssemblyLoadContext assemblyLoadContext)
        {
            FileNamePrefix = fileNamePrefix;
            this.assemblyLoadContext = assemblyLoadContext;
        }

        public override IAssemblyLoader AssemblyLoader => this;

        public override bool FileExistsShim(string fileName)
        {
            if(fileName.StartsWith(FileNamePrefix, StringComparison.Ordinal))
            {
                return true;
            }
            return base.FileExistsShim(fileName);
        }

        public override string GetFullPathShim(string fileName)
        {
            if(fileName.StartsWith(FileNamePrefix, StringComparison.Ordinal))
            {
                return fileName;
            }
            return base.GetFullPathShim(fileName);
        }

        public override Stream OpenFileForReadShim(string filePath, FSharpOption<bool>? useMemoryMappedFile, FSharpOption<bool>? shouldShadowCopy)
        {
            if(filePath.StartsWith(FileNamePrefix, StringComparison.Ordinal) && InputFiles.TryGetValue(filePath, out var file))
            {
                return file.OpenStream();
            }
            return base.OpenFileForReadShim(filePath, useMemoryMappedFile, shouldShadowCopy);
        }

        Assembly IAssemblyLoader.AssemblyLoadFrom(string fileName)
        {
            return assemblyLoadContext.LoadFromAssemblyPath(fileName) ?? base.AssemblyLoader.AssemblyLoadFrom(fileName);
        }

        Assembly IAssemblyLoader.AssemblyLoad(AssemblyName assemblyName)
        {
            return assemblyLoadContext.LoadFromAssemblyName(assemblyName) ?? base.AssemblyLoader.AssemblyLoad(assemblyName);
        }

        static readonly Func<string, Stream> createStream = static _ => new BlockBufferStream();
        static readonly Func<string, Stream> requireStream = static name =>
        {
            throw new IOException($"The file '{name}' is not present.");
        };
        static readonly Func<string, Stream, Stream> openStream = static (name, existing) =>
        {
            // Clone the stream to get an independent cursor
            switch(existing)
            {
                case MemoryStream memoryStream:
                    if(!memoryStream.TryGetBuffer(out var buffer))
                    {
                        buffer = memoryStream.ToArray();
                    }
                    return new MemoryStream(buffer.Array!, buffer.Offset, buffer.Count, true);
                case BlockBufferStream bufferStream:
                    return new BlockBufferStream(bufferStream);
                case FileStream fileStream:
                    return new NonClosingStream(new FileStream(fileStream.SafeFileHandle, fileStream.CanRead ? fileStream.CanWrite ? FileAccess.ReadWrite : FileAccess.Read : FileAccess.Write));
                case NonClosingStream wrappedMemoryStream:
                    return openStream!(name, wrappedMemoryStream.InnerStream);
                default:
                    throw new IOException($"The file '{name}' cannot be opened.");
            }
        };
        static readonly Func<string, Stream, Stream> appendStream = static (path, existing) =>
        {
            var stream = openStream(path, existing);
            stream.Position = stream.Length;
            return stream;
        };
        static readonly Func<string, Stream, Stream> truncateStream = static (path, existing) =>
        {
            var stream = openStream(path, existing);
            stream.SetLength(0);
            return stream;
        };

        public override Stream OpenFileForWriteShim(string filePath, FSharpOption<FileMode>? fileMode, FSharpOption<FileAccess>? fileAccess, FSharpOption<FileShare>? fileShare)
        {
            if(filePath.StartsWith(FileNamePrefix, StringComparison.Ordinal))
            {
                switch(fileMode?.Value)
                {
                    case FileMode.Open:
                        return outputFiles.AddOrUpdate(
                            filePath,
                            requireStream,
                            openStream
                        );
                    case FileMode.OpenOrCreate:
                        return outputFiles.AddOrUpdate(
                            filePath,
                            createStream,
                            openStream
                        );
                    case FileMode.Append:
                        return outputFiles.AddOrUpdate(
                            filePath,
                            createStream,
                            appendStream
                        );
                    case FileMode.Create:
                    case FileMode.CreateNew:
                        return outputFiles.AddOrUpdate(
                            filePath,
                            createStream,
                            truncateStream
                        );
                }
            }
            return Stream.Null;
        }
    }
}
