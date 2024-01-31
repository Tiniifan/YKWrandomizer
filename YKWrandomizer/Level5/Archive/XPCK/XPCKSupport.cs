using System;
using System.Runtime.InteropServices;

namespace YKWrandomizer.Level5.Archive.XPCK
{
    public class XPCKSupport
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public UInt32 Magic;
            public byte fc1;
            public byte fc2;
            public ushort tmp1;
            public ushort tmp2;
            public ushort tmp3;
            public ushort tmp4;
            public ushort tmp5;
            public uint tmp6;

            public ushort FileCount => (ushort)((fc2 & 0xf) << 8 | fc1);
            public ushort FileInfoOffset => (ushort)(tmp1 << 2);
            public ushort FilenameTableOffset => (ushort)(tmp2 << 2);
            public ushort DataOffset => (ushort)(tmp3 << 2);
            public ushort FileInfoSize => (ushort)(tmp4 << 2);
            public ushort FilenameTableSize => (ushort)(tmp5 << 2);
            public uint DataSize => tmp6 << 2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileEntry
        {
            public uint Crc32;
            public ushort NameOffset;
            public ushort tmp;
            public ushort tmp2;
            public byte tmpZ;
            public byte tmp2Z;

            public uint FileOffset => (((uint)tmpZ << 16) | tmp) << 2;
            public uint FileSize => ((uint)tmp2Z << 16) | tmp2;
        }

        public static byte[] CalculateF1F2(int x)
        {
            int f1, f2;

            if (x < 256)
            {
                f1 = x;
                f2 = 1;

                while (f2 <= x)
                {
                    f2 *= 2;
                }

                int coefficient = (int)Math.Log(f2, 2) * 10;
                coefficient = Convert.ToInt32("0x" + coefficient, 16);

                return new byte[] { Convert.ToByte(f1), Convert.ToByte(coefficient) };
            }
            else
            {
                f1 = x & 0xFF;
                f2 = (x >> 8) & 0xFF;

                return new byte[] { Convert.ToByte(f1), Convert.ToByte(f2) };
            }
        }
    }
}
