using System.Runtime.InteropServices;

namespace YKWrandomizer.Level5.Text
{
    public static class T2bþSupport
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public int FileCount;
            public int StartText;
            public int LengthText;
            public int LineNumber;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Text
        {
            public uint EntryCrc32;
            public uint TextInfo;
            public uint Crc32;
            public int TextNumber;
            public int TextOffset;
            public int Empty;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Noun
        {
            public uint EntryCrc32;
            public uint TextInfo;
            public uint Unk1;
            public uint Crc32;
            public int TextNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
            public byte[] EmptyBlock1;
            public int TextOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] EmptyBlock2;
        }
    }
}
