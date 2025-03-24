using System;
using System.IO;
using System.Text;

namespace Sona.Compiler.Tools
{
    internal sealed class StringStream : Stream
    {
        readonly Encoding encoding;
        readonly Encoder encoder;
        readonly string text;
        int textOffset;
        int bytePosition;
        bool completed;

        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanSeek => false;

        public override long Length => encoding.GetByteCount(text);

        public override long Position {
            get => bytePosition;
            set => throw new NotSupportedException();
        }

        public StringStream(string text, Encoding encoding)
        {
            this.encoding = encoding;
            this.text = text;
            encoder = encoding.GetEncoder();
        }

        public override int Read(Span<byte> buffer)
        {
            if(completed)
            {
                return 0;
            }
            var span = text.AsSpan(textOffset);
            encoder.Convert(span, buffer, false, out var used, out var count, out completed);
            textOffset += used;
            bytePosition += count;
            return count;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int ReadByte()
        {
            Span<byte> result = stackalloc byte[1];
            int count = Read(result);
            if(count == 0)
            {
                return -1;
            }
            return result[0];
        }

        public override void Flush()
        {

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
