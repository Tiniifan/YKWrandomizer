using System.Collections.Generic;
using System.Runtime.InteropServices;
using YKWrandomizer.Level5.Image.Color_Formats;

namespace YKWrandomizer.Level5.Image
{
    public static class IMGCSupport
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public uint Magic;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x06)]
            public byte[] UnkBlock1;
            public byte ImageFormat;
            public byte Unk2;
            public byte CombineFormat;
            public byte BitDepth;
            public short BytesPerTile;
            public short Width;
            public short Height;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] UnkBlock3;
            public int TileOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] UnkBlock4;
            public int TileSize1;
            public int TileSize2;
            public int ImageSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] UnkBlock5;
        }

        public static Dictionary<byte, IColorFormat> ImageFormats = new Dictionary<byte, IColorFormat>
        {
            {0, new RGBA8() },
            {1, new RGBA4() },
            {28, new ETC1A4() },
        };
    }
}