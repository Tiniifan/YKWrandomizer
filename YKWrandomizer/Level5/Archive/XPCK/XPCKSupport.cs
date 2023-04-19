using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace YKWrandomizer.Level5.Archive.XPCK
{
    public class XPCKSupport
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public UInt32 Magic;
            private byte fc1;
            private byte fc2;
            private ushort tmp1;
            private ushort tmp2;
            private ushort tmp3;
            private ushort tmp4;
            private ushort tmp5;
            private uint tmp6;

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
            private ushort tmp;
            private ushort tmp2;
            private byte tmpZ;
            private byte tmp2Z;

            public uint FileOffset => (((uint)tmpZ << 16) | tmp) << 2;
            public uint FileSize => ((uint)tmp2Z << 16) | tmp2;
        }
    }
}
