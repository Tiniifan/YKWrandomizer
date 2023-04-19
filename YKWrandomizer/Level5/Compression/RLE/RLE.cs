using System.Collections.Generic;

namespace YKWrandomizer.Level5.Compression.RLE
{
    public class RLE : ICompression
    {
        public byte[] Compress(byte[] indata)
        {
            // Not Implemented

            return null;
        }

        public byte[] Decompress(byte[] instream)
        {
            long inLength = instream.Length;
            long ReadBytes = 0;
            int p = 0;

            p++;

            int decompressedSize = (instream[p++] & 0xFF)
                    | ((instream[p++] & 0xFF) << 8)
                    | ((instream[p++] & 0xFF) << 16);
            ReadBytes += 4;
            if (decompressedSize == 0)
            {
                decompressedSize = decompressedSize
                        | ((instream[p++] & 0xFF) << 24);
                ReadBytes += 4;
            }

            List<byte> outstream = new List<byte>();

            while (p < instream.Length)
            {

                int flag = (byte)instream[p++];
                ReadBytes++;

                bool compressed = (flag & 0x80) > 0;
                int length = flag & 0x7F;

                if (compressed)
                    length += 3;
                else
                    length += 1;

                if (compressed)
                {

                    int data = (byte)instream[p++];
                    ReadBytes++;

                    byte bdata = (byte)data;
                    for (int i = 0; i < length; i++)
                    {
                        outstream.Add(bdata);
                    }

                }
                else
                {

                    int tryReadLength = length;
                    if (ReadBytes + length > inLength)
                        tryReadLength = (int)(inLength - ReadBytes);

                    ReadBytes += tryReadLength;

                    for (int i = 0; i < tryReadLength; i++)
                    {
                        outstream.Add((byte)(instream[p++] & 0xFF));
                    }
                }
            }

            if (ReadBytes < inLength)
            {
            }

            return outstream.ToArray();
        }
    }
}
