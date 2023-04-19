using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace YKWrandomizer.Level5.Compression.LZ10
{
    public class LZ10 : ICompression
    {
        public byte[] Compress(byte[] indata)
        {
            long inLength = indata.Length;
            MemoryStream outstream = new MemoryStream();

            var compressionHeader = new[] {
                (byte)((byte)(indata.Length << 3) | 1),
                (byte)(indata.Length >> 5),
                (byte)(indata.Length >> 13),
                (byte)(indata.Length >> 21) };
            outstream.Write(compressionHeader, 0, 4);

            if (inLength == 0)
                return new byte[0];

            int compressedLength = 0;

            byte[] outbuffer = new byte[8 * 2 + 1];
            outbuffer[0] = 0;
            int bufferlength = 1, bufferedBlocks = 0;
            int readBytes = 0;

            // Create a dictionary to store occurrences of previous patterns
            Dictionary<int, List<int>> occurrences = new Dictionary<int, List<int>>();

            while (readBytes < inLength)
            {
                if (bufferedBlocks == 8)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                    outbuffer[0] = 0;
                    bufferlength = 1;
                    bufferedBlocks = 0;
                }

                int disp;
                int oldLength = Math.Min(readBytes, 0x1000);
                int length = GetOccurrenceLength(indata, readBytes, (int)Math.Min(inLength - readBytes, 0x12),
                                                              indata, oldLength, out disp, occurrences);

                if (length < 3)
                {
                    outbuffer[bufferlength++] = indata[readBytes++];
                }
                else
                {
                    readBytes += length;

                    outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                    outbuffer[bufferlength] = (byte)(((length - 3) << 4) & 0xF0);
                    outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
                    bufferlength++;
                    outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
                    bufferlength++;
                }
                bufferedBlocks++;
            }

            if (bufferedBlocks > 0)
            {
                outstream.Write(outbuffer, 0, bufferlength);
                compressedLength += bufferlength;
            }

            return outstream.ToArray();
        }

        public byte[] Decompress(byte[] data)
        {
            var p = 4;
            var op = 0;
            var mask = 0;
            var flag = 0;

            List<byte> output = new List<byte>();

            while (p < data.Length)
            {
                if (mask == 0)
                {
                    flag = data[p];
                    p += 1;
                    mask = 0x80;
                }

                if ((flag & mask) == 0)
                {
                    if (p + 1 > data.Length)
                    {
                        break;
                    }
                    output.Add(data[p]);
                    p += 1;
                    op += 1;
                }
                else
                {
                    if (p + 2 > data.Length)
                    {
                        break;
                    }

                    var dat = data[p] << 8 | data[p + 1];
                    p += 2;
                    var pos = (dat & 0x0FFF) + 1;
                    var length = (dat >> 12) + 3;

                    foreach (var i in Enumerable.Range(0, length))
                    {
                        if (op - pos >= 0)
                        {
                            output.Add((byte)(op - pos < output.Count ? output[op - pos] : 0));
                            op += 1;
                        }
                    }
                }

                mask >>= 1;
            }

            return output.ToArray();
        }

        private int GetOccurrenceLength(byte[] newdata, int newStart, int newLength, byte[] olddata, int oldLength, out int disp, Dictionary<int, List<int>> occurrences, int minDisp = 1)
        {
            disp = 0;
            if (newLength == 0)
                return 0;

            int maxLength = 0;

            // Compute the hash of the current pattern
            int hash = GetHash(newdata, newStart, newLength);

            // Search for previous occurrences of the pattern in the dictionary
            if (occurrences.ContainsKey(hash))
            {
                List<int> offsets = occurrences[hash];

                for (int i = offsets.Count - 1; i >= 0; i--)
                {
                    int offset = offsets[i];

                    // Check if the offset is within the maximum distance
                    if (newStart - offset >= minDisp)
                    {
                        int length = FindMatchLength(newdata, newStart, newLength, olddata, offset, oldLength);

                        if (length > maxLength)
                        {
                            maxLength = length;
                            disp = newStart - offset;
                        }

                        if (length == newLength)
                            break;
                    }
                }
            }

            // Add the current pattern to the dictionary
            if (!occurrences.ContainsKey(hash))
                occurrences.Add(hash, new List<int>());
            occurrences[hash].Add(newStart);

            return maxLength;
        }

        private int FindMatchLength(byte[] newdata, int newStart, int newLength, byte[] olddata, int oldStart, int oldLength)
        {
            int length = 0;

            while (newStart + length < newLength && oldStart + length < oldLength)
            {
                if (newdata[newStart + length] != olddata[oldStart + length])
                    break;

                length++;
            }

            return length;
        }

        private int GetHash(byte[] data, int start, int length)
        {
            int hash = 5381;

            for (int i = start; i < start + length; i++)
                hash = ((hash << 5) + hash) ^ data[i];

            return hash;
        }
    }
}