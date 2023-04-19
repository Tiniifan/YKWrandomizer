using System.IO;
using System.Linq;
using System.IO.Compression;

namespace YKWrandomizer.Level5.Compression.ZLib
{
    public class Zlib : ICompression
    {
        public byte[] Compress(byte[] data)
        {
            var compressionHeader = new[] {
                (byte)((byte)(data.Length << 3) | 1),
                (byte)(data.Length >> 5),
                (byte)(data.Length >> 13),
                (byte)(data.Length >> 21) 
            };

            using (var output = new MemoryStream())
            {
                output.Write(compressionHeader, 0, compressionHeader.Length);

                using (var compressor = new DeflateStream(output, CompressionMode.Compress))
                {
                    compressor.Write(data, 0, data.Length);
                }

                return output.ToArray();
            }
        }
        public byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data.Skip(4).ToArray()))
            using (var zlibStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zlibStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
