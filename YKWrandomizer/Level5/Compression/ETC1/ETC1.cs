using System;
using System.Linq;

namespace YKWrandomizer.Level5.Compression.ETC1
{
    public class ETC1 : ICompression
    {
        private int Width;

        private int Height;

        private bool HasAlphaCanal;

        public ETC1(bool hasAlphaCanal, int width, int height)
        {
            HasAlphaCanal = hasAlphaCanal;
            Width = width;
            Height = height;
        }

        public byte[] Compress(byte[] data)
        {
            // Not Implemented
            return null;
        }

        public byte[] Decompress(byte[] data)
        {
            switch (HasAlphaCanal)
            {
                case true:
                    return ETC1Decoder.DecompressETC1A4(data, Width, Height);
                default:
                    return null;
            }
        }
    }
}
