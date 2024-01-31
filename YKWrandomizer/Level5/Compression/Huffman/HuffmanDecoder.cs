using System;
using System.IO;
using System.Linq;
using System.Text;

// HuffmanDecoder From https://github.com/FanTranslatorsInternational/Kuriimu2

namespace YKWrandomizer.Level5.Compression.Huffman
{
    public class HuffmanDecoder
    {
        private readonly int _bitDepth;
        private readonly HuffmanHeaderlessDecoder _decoder;

        public HuffmanDecoder(int bitDepth, NibbleOrder nibbleOrder)
        {
            _bitDepth = bitDepth;

            _decoder = new HuffmanHeaderlessDecoder(bitDepth, nibbleOrder);
        }

        public void Decode(Stream input, Stream output)
        {
            var compressionHeader = new byte[4];
            input.Read(compressionHeader, 0, 4);

            var huffmanMode = _bitDepth == 4 ? 2 : 3;
            if ((compressionHeader[0] & 0x7) != huffmanMode)
                throw new InvalidDataException($"Level5 Huffman{_bitDepth}");

            var decompressedSize = (compressionHeader[0] >> 3) | (compressionHeader[1] << 5) |
                                   (compressionHeader[2] << 13) | (compressionHeader[3] << 21);

            _decoder.Decode(input, output, decompressedSize);
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }

    public class HuffmanHeaderlessDecoder
    {
        private readonly int _bitDepth;
        private readonly NibbleOrder _nibbleOrder;

        public HuffmanHeaderlessDecoder(int bitDepth, NibbleOrder nibbleOrder)
        {
            _bitDepth = bitDepth;
            _nibbleOrder = nibbleOrder;
        }

        public void Decode(Stream input, Stream output, int decompressedSize)
        {
            var result = new byte[decompressedSize * 8 / _bitDepth];

            using (var br = new BinaryReader(input, Encoding.ASCII, true))
            {
                var treeSize = br.ReadByte();
                var treeRoot = br.ReadByte();
                var treeBuffer = br.ReadBytes(treeSize * 2);

                for (int i = 0, code = 0, next = 0, pos = treeRoot, resultPos = 0; resultPos < result.Length; i++)
                {
                    if (i % 32 == 0)
                        code = br.ReadInt32();

                    next += ((pos & 0x3F) << 1) + 2;
                    var direction = (code >> (31 - i)) % 2 == 0 ? 2 : 1;
                    var leaf = (pos >> 5 >> direction) % 2 != 0;

                    pos = treeBuffer[next - direction];
                    if (leaf)
                    {
                        result[resultPos++] = (byte)pos;
                        pos = treeRoot;
                        next = 0;
                    }
                }
            }

            if (_bitDepth == 8)
                output.Write(result, 0, result.Length);
            else
            {
                var combinedData = _nibbleOrder == NibbleOrder.LowNibbleFirst ?
                    Enumerable.Range(0, decompressedSize).Select(j => (byte)(result[2 * j] | (result[2 * j + 1] << 4))).ToArray() :
                    Enumerable.Range(0, decompressedSize).Select(j => (byte)((result[2 * j] << 4) | result[2 * j + 1])).ToArray();

                output.Write(combinedData, 0, combinedData.Length);
            }
        }
    }

    public enum NibbleOrder
    {
        LowNibbleFirst,
        HighNibbleFirst
    }
}
