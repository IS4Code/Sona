using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace IS4.Sona.Compiler.Tools
{
    internal abstract class LocalInputFile
    {
        public static LocalInputFile FromText(string text, Encoding encoding)
        {
            return new TextInput(text, encoding);
        }

        public static LocalInputFile FromBytes(ArraySegment<byte> bytes)
        {
            return new ByteInput(bytes);
        }

        public static LocalInputFile FromFile(string fileName)
        {
            return FromFile(new FileInfo(fileName));
        }

        public static LocalInputFile FromFile(FileInfo fileInfo)
        {
            return new FileInput(fileInfo);
        }

        public static LocalInputFile FromEmbeddedFile(Assembly assembly, string name)
        {
            return new EmbeddedInput(assembly, name);
        }

        public static implicit operator LocalInputFile(string text)
        {
            return FromText(text, Encoding.UTF8);
        }

        public static implicit operator LocalInputFile(ArraySegment<byte> bytes)
        {
            return FromBytes(bytes);
        }

        public static implicit operator LocalInputFile(byte[] bytes)
        {
            return FromBytes(bytes);
        }

        public static implicit operator LocalInputFile(FileInfo fileInfo)
        {
            return FromFile(fileInfo);
        }

        public abstract Stream OpenStream();

        sealed class TextInput : LocalInputFile
        {
            readonly string text;
            readonly Encoding encoding;

            public TextInput(string text, Encoding encoding)
            {
                this.text = text;
                this.encoding = encoding;
            }

            public override Stream OpenStream()
            {
                return new StringStream(text, encoding);
            }
        }

        sealed class ByteInput : LocalInputFile
        {
            readonly ArraySegment<byte> bytes;

            public ByteInput(ArraySegment<byte> bytes)
            {
                this.bytes = bytes;
            }

            public override Stream OpenStream()
            {
                return new MemoryStream(bytes.Array ?? Array.Empty<byte>(), bytes.Offset, bytes.Count, false);
            }
        }

        sealed class FileInput : LocalInputFile
        {
            readonly FileInfo fileInfo;

            public FileInput(FileInfo fileInfo)
            {
                this.fileInfo = fileInfo;
            }

            public override Stream OpenStream()
            {
                return fileInfo.OpenRead();
            }
        }

        sealed class EmbeddedInput : LocalInputFile
        {
            readonly Assembly assembly;
            readonly string name;

            public EmbeddedInput(Assembly assembly, string name)
            {
                this.assembly = assembly;
                this.name = name;
            }

            public override Stream OpenStream()
            {
                return assembly.GetManifestResourceStream(name) ?? throw new IOException("Embedded resource cannot be found.");
            }
        }
    }
}
