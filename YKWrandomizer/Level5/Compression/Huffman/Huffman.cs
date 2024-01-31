using System.IO;

namespace YKWrandomizer.Level5.Compression.Huffman
{
    public class Huffman : ICompression
    {
        public int BitDepth;

        public Huffman(int bitDepth)
        {
            BitDepth = bitDepth;
        }

        public byte[] Compress(byte[] data)
        {
            // Not Implemented
            return null;
        }

        public byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            {
                var decoder = new HuffmanDecoder(BitDepth, NibbleOrder.LowNibbleFirst);
                decoder.Decode(input, output);

                return output.ToArray();
            }
        }
    }
}
