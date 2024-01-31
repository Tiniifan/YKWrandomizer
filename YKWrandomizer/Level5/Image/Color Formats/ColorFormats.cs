using System;
using System.Drawing;

namespace YKWrandomizer.Level5.Image
{
    public class RGBA4 : IColorFormat
    {
        public string Name => "RGBA4";

        public int Size => 2;

        public byte[] Encode(Color color)
        {
            int r = color.R >> 4;
            int g = color.G >> 4;
            int b = color.B >> 4;
            int a = color.A >> 4;

            ushort rgba4 = (ushort)((r << 12) | (g << 8) | (b << 4) | a);

            byte[] data = new byte[2];
            data[0] = (byte)(rgba4 & 0xFF);
            data[1] = (byte)(rgba4 >> 8);

            return data;
        }

        public Color Decode(byte[] data)
        {
            if (data.Length < 2)
            {
                return Color.FromArgb(0);
            }

            ushort rgba4 = (ushort)((data[1] << 8) | data[0]);

            int r = (rgba4 >> 12) & 0xF;
            int g = (rgba4 >> 8) & 0xF;
            int b = (rgba4 >> 4) & 0xF;
            int a = rgba4 & 0xF;

            r *= 16;
            g *= 16;
            b *= 16;
            a *= 16;

            return Color.FromArgb(a, r, g, b);
        }
    }

    public class RGBA8 : IColorFormat
    {
        public string Name => "RGBA8";

        public int Size => 4;

        public byte[] Encode(Color color)
        {
            int argb = color.ToArgb();
            return new byte[] { (byte)((argb >> 24) & 0xFF), (byte)(argb & 0xFF), (byte)((argb >> 8) & 0xFF), (byte)((argb >> 16) & 0xFF) };
        }

        public Color Decode(byte[] data)
        {
            if (data.Length < 4)
            {
                return Color.FromArgb(0);
            }
            int argb = (data[0] << 24) | (data[3] << 16) | (data[2] << 8) | data[1];
            return Color.FromArgb(argb);
        }
    }

    public class ETC1A4 : IColorFormat
    {
        public string Name => "ETC1A4";

        public int Size => 4;

        public byte[] Encode(Color color)
        {
            // Not implemented
            return null;
        }

        public Color Decode(byte[] data)
        {
            int r = data[0];
            int g = data[1];
            int b = data[2];
            int a = data[3];
            return Color.FromArgb(a, r, g, b);
        }
    }
}
