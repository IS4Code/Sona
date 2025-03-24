using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace Sona.Compiler.Tools
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class RawValueHash : HashAlgorithm
    {
        internal MemoryStream Stream { get; private set; } = new MemoryStream();

        static RawValueHash()
        {
            CryptoConfig.AddAlgorithm(typeof(RawValueHash), typeof(RawValueHash).FullName!);
        }

        public override void Initialize()
        {
            if(Stream.CanRead)
            {
                Stream.SetLength(0);
            }
            else
            {
                Stream = new MemoryStream();
            }
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            Stream.Write(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            return Array.Empty<byte>();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                Stream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
