using System;
using System.Linq;

namespace YKWrandomizer.Level5.Compression
{
    public static class Compressor
    {
        public static ICompression GetCompression(uint method)
        {
            switch (method)
            {
                case 0:
                    return new NoCompression.NoCompression();

                case 1:
                    return new LZ10.LZ10();

                case 2:
                    return new Huffman.Huffman(4);

                case 3:
                    return new Huffman.Huffman(8);

                case 4:
                    return new RLE.RLE();

                case 5:
                    return new Zlib.Zlib();

                default:
                    throw new NotSupportedException($"Unknown compression method {method}");
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            var sizeMethodBuffer = data.Take(4).ToArray();
            int size = (sizeMethodBuffer[0] >> 3) | (sizeMethodBuffer[1] << 5) |
                                   (sizeMethodBuffer[2] << 13) | (sizeMethodBuffer[3] << 21);
            ICompression method = GetCompression(BitConverter.ToUInt32(sizeMethodBuffer, 0) & 0x7);

            if (method != null)
            {
                return method.Decompress(data).Take(size).ToArray();
            }
            else
            {
                return data;
            }
        }
    }
}
