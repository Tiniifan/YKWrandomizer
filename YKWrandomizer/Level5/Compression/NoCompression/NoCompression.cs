using System.IO;
using System.Linq;

namespace YKWrandomizer.Level5.Compression.NoCompression
{
    public class NoCompression : ICompression
    {
        public byte[] Compress(byte[] indata)
        {
            MemoryStream outstream = new MemoryStream();

            var compressionHeader = new[] {
                (byte)((byte)(indata.Length << 3) | 0),
                (byte)(indata.Length >> 5),
                (byte)(indata.Length >> 13),
                (byte)(indata.Length >> 21) };
            outstream.Write(compressionHeader, 0, 4);
            outstream.Write(indata, 0, indata.Length);

            return outstream.ToArray();
        }

        public byte[] Decompress(byte[] data)
        {
            return data.Skip(4).ToArray();
        }
    }
}