using System;
using System.Linq;

namespace YKWrandomizer.Level5.Compression.ETC1
{
    public static class ETC1Decoder
    {
        private static int[][] modifiers =
        {
            new[] { 2, 8, -2, -8 },
            new[] { 5, 17, -5, -17 },
            new[] { 9, 29, -9, -29 },
            new[] { 13, 42, -13, -42 },
            new[] { 18, 60, -18, -60 },
            new[] { 24, 80, -24, -80 },
            new[] { 33, 106, -33, -106 },
            new[] { 47, 183, -47, -183 }
        };

        private static int[] pixelOrder = new int[] { 0, 4, 1, 5, 8, 12, 9, 13, 2, 6, 3, 7, 10, 14, 11, 15 };

        public static byte[] DecompressETC1A4(byte[] data, int width, int height)
        {
            byte[] result = new byte[width * height * 4];
            int offset = 0;
            int writeOffset = 0;

            for (int blockY = 0; blockY < height; blockY += 4)
            {
                for (int blockX = 0; blockX < width; blockX += 4)
                {
                    byte[] alphaBlockData = data.Skip(offset).Take(8).ToArray();
                    offset += 8;

                    byte[] blockData = data.Skip(offset).Take(8).ToArray();
                    offset += 8;

                    byte[] alphas = DecodeBlockAlphas(alphaBlockData);
                    byte[] colors = DecodeBlockColors(blockData);

                    for (int i = 0; i < 16; i++)
                    {
                        byte red = colors[i * 3];
                        byte green = colors[i * 3 + 1];
                        byte blue = colors[i * 3 + 2];

                        result[writeOffset] = red;
                        result[writeOffset + 1] = green;
                        result[writeOffset + 2] = blue;
                        result[writeOffset + 3] = alphas[i];
                        writeOffset += 4;
                    }
                }
            }

            return result;
        }

        public static byte[] DecodeBlockColors(byte[] data)
        {
            byte[] result = new byte[48];

            ushort LSB = (ushort)(data[0] | (data[1] << 8));
            ushort MSB = (ushort)(data[2] | (data[3] << 8));
            byte flags = data[4];
            byte B = data[5];
            byte G = data[6];
            byte R = data[7];

            bool flipBit = (flags & 1) == 1;
            bool diffBit = (flags & 2) == 2;
            int colorDepth = diffBit ? 32 : 16;
            int table0 = (flags >> 5) & 7;
            int table1 = (flags >> 2) & 7;

            RGB color0 = new RGB(R * colorDepth / 256, G * colorDepth / 256, B * colorDepth / 256);

            RGB colors1 = new RGB();
            if (!diffBit)
            {
                colors1 = new RGB(R % 16, G % 16, B % 16);
            } else
            {
                RGB c0 = color0;
                int rd = RGB.Sign3(R % 8), gd = RGB.Sign3(G % 8), bd = RGB.Sign3(B % 8);
                colors1 = new RGB(c0.R + rd, c0.G + gd, c0.B + bd);
            }

            color0 = color0.Scale(colorDepth);
            colors1 = colors1.Scale(colorDepth);

            int flipbitmask = flipBit ? 2 : 8;
            int t = 0;
            foreach (int i in pixelOrder)
            {
                RGB basec = (i & flipbitmask) == 0 ? color0 : colors1;
                int[] mod = modifiers[(i & flipbitmask) == 0 ? table0 : table1];
                RGB c = basec + mod[(MSB >> i) % 2 * 2 + (LSB >> i) % 2];
                result[t] = c.R;
                result[t + 1] = c.G;
                result[t + 2] = c.B;
                t += 3;
            }

            return result;
        }

        private static byte[] DecodeBlockAlphas(byte[] blockData)
        {
            var canalAlpha = true ? System.BitConverter.ToUInt64(blockData, 0) : ulong.MaxValue;

            byte[] alphas = new byte[16];

            int t = 0;
            foreach (int i in pixelOrder)
            {
                alphas[t] = (byte)((canalAlpha >> (4 * i)) % 16 * 17);
                t++;
            }

            return alphas;
        }
    }
}