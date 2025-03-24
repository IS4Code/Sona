using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Sona.Compiler.Tools
{
    internal sealed class BlockBufferStream : Stream
    {
        readonly Storage storage;
        readonly bool copy;

        int blockIndex;
        long blockStart;
        int blockOffset;
        ArraySegment<byte> block;
        int blockSize => block.Count;

        public BlockBufferStream() : this(ArrayPool<byte>.Shared)
        {

        }

        public BlockBufferStream(ArrayPool<byte> arrayPool)
        {
            storage = new Storage(arrayPool);
            copy = false;
        }

        public BlockBufferStream(BlockBufferStream source)
        {
            storage = source.storage;
            copy = true;
        }

        public ReadOnlySequence<byte> GetBuffer()
        {
            return storage.GetBuffer();
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length { get => storage.Length; }

        public override long Position {
            get => GetBlockOffset(blockIndex) + blockOffset;
            set {
                var index = GetBlockIndex(value);
                var start = GetBlockOffset(index);
                var offset = value - start;
                if(offset > Int32.MaxValue)
                {
                    throw new ArgumentException("Position cannot be represented.", nameof(value));
                }
                blockOffset = (int)offset;
                if(index == blockIndex)
                {
                    // Same index
                    return;
                }
                blockIndex = index;
                blockStart = start;
                block = default;
            }
        }

        private void PrepareBlock()
        {
            if(block.Array == null)
            {
                block = storage.GetBlock(blockIndex);
            }
        }

        public override void Flush()
        {

        }

        public override int ReadByte()
        {
            return base.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long remaining = storage.Length - (blockStart + blockOffset);
            if(remaining <= 0)
            {
                // Nothing to read
                return 0;
            }
            if(count > remaining)
            {
                // Read only until the end
                count = (int)remaining;
            }
            else if(count == 0)
            {
                return 0;
            }
            int read = 0;
            while(count > 0)
            {
                // Get data for this block
                PrepareBlock();

                int blockRemaining = blockSize - blockOffset;
                if(count < blockRemaining)
                {
                    // Rest is within this block
                    read += count;
                    block.Slice(blockOffset, count).CopyTo(buffer, offset);
                    blockOffset += count;
                    break;
                }

                // Fully read this block
                block.Slice(blockOffset).CopyTo(buffer, offset);

                read += blockRemaining;
                count -= blockRemaining;
                offset += blockRemaining;

                // Entering a new block
                blockIndex++;
                blockOffset = 0;
                blockStart = GetBlockOffset(blockIndex);
                block = default;
            }
            return read;
        }

        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            while(count > 0)
            {
                // Get data for this block
                PrepareBlock();

                int blockRemaining = blockSize - blockOffset;
                if(count < blockRemaining)
                {
                    // Rest is within this block
                    Array.Copy(buffer, offset, block.Array!, block.Offset + blockOffset, count);
                    blockOffset += count;

                    storage.Expand(blockStart + blockOffset);
                    break;
                }

                // Write to the end of this block
                Array.Copy(buffer, offset, block.Array!, block.Offset + blockOffset, blockRemaining);

                count -= blockRemaining;
                offset += blockRemaining;

                // Entering a new block
                blockIndex++;
                blockOffset = 0;
                blockStart = GetBlockOffset(blockIndex);
                storage.Expand(blockStart);
                block = default;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    return Position = offset;
                case SeekOrigin.Current:
                    return Position += offset;
                case SeekOrigin.End:
                    return Position = Length + offset;
                default:
                    throw new InvalidEnumArgumentException(nameof(origin), (int)origin, typeof(SeekOrigin));
            }
        }

        public override void SetLength(long value)
        {
            storage.Truncate(value);
            Position = Math.Min(Position, value);
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing && !copy)
            {
                storage.Dispose();
            }
        }

        const int blockGranularity = 1024;

        private static long GetBlockOffset(int index)
        {
            if(index == 0)
            {
                return 0;
            }
            return (1L << (index - 1)) * blockGranularity;
        }

        private static long GetBlockSize(int index)
        {
            if(index == 0)
            {
                return blockGranularity;
            }
            return (1L << (index - 1)) * blockGranularity;
        }

        private static int GetBlockIndex(long offset)
        {
            offset /= blockGranularity;
            if(offset == 0)
            {
                return 0;
            }
            return 1 + (int)Math.Floor(Math.Log2(offset));
        }

        class Storage : IDisposable
        {
            readonly ArrayPool<byte> arrayPool;
            readonly ConcurrentDictionary<int, ArraySegment<byte>> blocks = new();
            long length;

            public long Length => Interlocked.Read(ref length);

            readonly Func<int, ArraySegment<byte>> addBlock;

            public Storage(ArrayPool<byte> arrayPool)
            {
                this.arrayPool = arrayPool;

                addBlock = index => {
                    var size = GetBlockSize(index);
                    if(size > Int32.MaxValue)
                    {
                        throw new IOException($"Files larger than {Int32.MaxValue} bytes cannot be stored in memory.");
                    }
                    var sizeInt = (int)size;
                    var array = arrayPool.Rent(sizeInt);
                    return new ArraySegment<byte>(array, 0, sizeInt);
                };
            }

            public ArraySegment<byte> GetBlock(int index)
            {
                return blocks.GetOrAdd(index, addBlock);
            }

            public void Dispose()
            {
                foreach(var pair in blocks)
                {
                    arrayPool.Return(pair.Value.Array!);
                }
                blocks.Clear();
            }

            public void Truncate(long newLength)
            {
                var maxIndex = GetBlockIndex(Interlocked.Exchange(ref length, newLength));
                if(newLength == 0)
                {
                    Dispose();
                    return;
                }
                var index = GetBlockIndex(newLength);
                var start = GetBlockOffset(index);
                if(newLength > start)
                {
                    // Truncate current block
                    blocks.AddOrUpdate(index, addBlock, (index, original) => {
                        int offset = (int)(start - newLength);
                        Array.Clear(original.Array!, original.Offset + offset, original.Count - offset);
                        return original;
                    });
                    // Drop from next
                    index++;
                }
                while(index <= maxIndex)
                {
                    // Drop the block
                    if(blocks.TryRemove(index, out var segment))
                    {
                        arrayPool.Return(segment.Array!);
                    }
                    index++;
                }
            }

            public void Expand(long newLength)
            {
                long current;
                do
                {
                    current = Length;
                    if(current >= newLength)
                    {
                        return;
                    }
                }
                while(Interlocked.CompareExchange(ref length, newLength, current) != current);
            }

            public ReadOnlySequence<byte> GetBuffer()
            {
                var length = Length;
                var maxIndex = GetBlockIndex(length);

                Segment? first = null, previous = null;

                long segmentIndex = 0;
                for(int index = 0; index < maxIndex; index++)
                {
                    if(blocks.TryGetValue(index, out var slice))
                    {
                        // Fully occupied block
                        AddSegment(slice);
                    }
                    else
                    {
                        // Empty block
                        var size = GetBlockSize(index);
                        while(size >= Segment.EmptySize)
                        {
                            size -= Segment.EmptySize;
                            AddSegment(Segment.Empty);
                        }
                        AddSegment(Segment.Empty.Slice(0, (int)size));
                    }
                }

                {
                    // Add last block
                    var offset = GetBlockOffset(maxIndex);
                    long lastSize = length - offset;
                    if(blocks.TryGetValue(maxIndex, out var slice))
                    {
                        // Slice to length
                        AddSegment(slice.Slice(0, (int)lastSize));
                    }
                    else
                    {
                        // Empty block
                        while(lastSize >= Segment.EmptySize)
                        {
                            lastSize -= Segment.EmptySize;
                            AddSegment(Segment.Empty);
                        }
                        AddSegment(Segment.Empty.Slice(0, (int)lastSize));
                    }
                }

                void AddSegment(ArraySegment<byte> slice)
                {
                    if(slice.Count == 0)
                    {
                        return;
                    }
                    var segment = new Segment(slice, segmentIndex);
                    segmentIndex += slice.Count;
                    if(previous != null)
                    {
                        previous.Link(segment);
                    }
                    else
                    {
                        first = segment;
                    }
                    previous = segment;
                }

                if(previous == null)
                {
                    // Empty
                    return new ReadOnlySequence<byte>();
                }
                return new ReadOnlySequence<byte>(first!, 0, previous, previous.Memory.Length);
            }

            class Segment : ReadOnlySequenceSegment<byte>
            {
                public const int EmptySize = blockGranularity * 4;
                public static readonly ArraySegment<byte> Empty = new(new byte[EmptySize]);

                public Segment(ArraySegment<byte> segment, long offset)
                {
                    Memory = segment.AsMemory();
                    RunningIndex = offset;
                }

                public void Link(Segment next)
                {
                    Next = next;
                }
            }
        }
    }
}
