using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using FSharp.Compiler.IO;
using Microsoft.FSharp.Core;

namespace IS4.Sona.Compiler.Tools
{
    internal sealed class LocalFileSystem : DefaultFileSystem
    {
        readonly ConcurrentDictionary<string, MemoryStream> outputFiles = new();
        public Dictionary<string, LocalInputFile> InputFiles { get; } = new();
        public IReadOnlyDictionary<string, MemoryStream> OutputFiles => outputFiles;

        public string FileNamePrefix { get; }

        public LocalFileSystem(string fileNamePrefix)
        {
            FileNamePrefix = fileNamePrefix;
        }

        public override bool FileExistsShim(string fileName)
        {
            if (fileName.StartsWith(FileNamePrefix, StringComparison.Ordinal))
            {
                return true;
            }
            return base.FileExistsShim(fileName);
        }

        public override string GetFullPathShim(string fileName)
        {
            if (fileName.StartsWith(FileNamePrefix, StringComparison.Ordinal))
            {
                return fileName;
            }
            return base.GetFullPathShim(fileName);
        }

        public override Stream OpenFileForReadShim(string filePath, FSharpOption<bool>? useMemoryMappedFile, FSharpOption<bool>? shouldShadowCopy)
        {
            if (filePath.StartsWith(FileNamePrefix, StringComparison.Ordinal) && InputFiles.TryGetValue(filePath, out var file))
            {
                return file.OpenStream();
            }
            return base.OpenFileForReadShim(filePath, useMemoryMappedFile, shouldShadowCopy);
        }

        static readonly Func<string, MemoryStream> createStream = static _ => new MemoryStream();
        static readonly Func<string, MemoryStream> requireStream = static name =>
        {
            throw new InvalidOperationException($"The file '{name}' is not present.");
        };
        static readonly Func<string, MemoryStream, MemoryStream> openStream = static (_, existing) =>
        {
            if (!existing.TryGetBuffer(out var buffer))
            {
                buffer = existing.ToArray();
            }
            return new MemoryStream(buffer.Array!, buffer.Offset, buffer.Count, true);
        };
        static readonly Func<string, MemoryStream, MemoryStream> appendStream = static (path, existing) =>
        {
            var stream = openStream(path, existing);
            stream.Position = stream.Length;
            return stream;
        };

        public override Stream OpenFileForWriteShim(string filePath, FSharpOption<FileMode>? fileMode, FSharpOption<FileAccess>? fileAccess, FSharpOption<FileShare>? fileShare)
        {
            if (filePath.StartsWith(FileNamePrefix, StringComparison.Ordinal))
            {
                switch (fileMode?.Value)
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
                }
                var stream = new MemoryStream();
                outputFiles[filePath] = stream;
                return stream;
            }
            return Stream.Null;
        }
    }
}
