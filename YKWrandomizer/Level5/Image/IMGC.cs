using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using YKWrandomizer.Tool;
using YKWrandomizer.Level5.Compression;
using YKWrandomizer.Level5.Compression.ETC1;
using YKWrandomizer.Level5.Image.Color_Formats;

namespace YKWrandomizer.Level5.Image
{
    public static class IMGC
    {
        public static Bitmap ToBitmap(byte[] fileContent)
        {
            BinaryDataReader data = new BinaryDataReader(fileContent);

            var header = data.ReadStruct<IMGCSupport.Header>();

            byte[] _tileData = Compressor.Decompress(data.GetSection((uint)header.TileOffset, header.TileSize1));
            byte[] imageData = Compressor.Decompress(data.GetSection((uint)(header.TileOffset + header.TileSize2), header.ImageSize));
            return DecodeImage(_tileData, imageData, IMGCSupport.ImageFormats[header.ImageFormat], header.Width, header.Height, header.BitDepth);
        }

        private static Bitmap DecodeImage(byte[] tile, byte[] imageData, IColorFormat imgFormat, int width, int height, int bitDepth)
        {
            using (var table = new BinaryDataReader(tile))
            using (var tex = new BinaryDataReader(imageData))
            {
                int tableLength = (int)table.BaseStream.Length;

                var tmp = table.ReadValue<ushort>();
                table.BaseStream.Position = 0;
                var entryLength = 2;
                if (tmp == 0x453)
                {
                    byte[] entryStart = table.GetSection(8);
                    entryLength = 4;
                }

                var ms = new MemoryStream();
                for (int i = (int)table.BaseStream.Position; i < tableLength; i += entryLength)
                {
                    uint entry = (entryLength == 2) ? table.ReadValue<ushort>() : table.ReadValue<uint>();
                    if (entry == 0xFFFF || entry == 0xFFFFFFFF)
                    {
                        for (int j = 0; j < 64 * bitDepth / 8; j++)
                        {
                            ms.WriteByte(0);
                        }
                    }
                    else
                    {
                        if (entry * (64 * bitDepth / 8) < tex.BaseStream.Length)
                        {
                            tex.BaseStream.Position = entry * (64 * bitDepth / 8);
                            for (int j = 0; j < 64 * bitDepth / 8; j++)
                            {
                                ms.WriteByte(tex.ReadValue<byte>());
                            }
                        }
                    }
                }

                IMGCSwizzle imgcSwizzle = new IMGCSwizzle(width, height);
                var points = imgcSwizzle.GetPointSequence();

                var bmp = new Bitmap(width, height);
                var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                var stride = data.Stride / 4;

                byte[] imageDataAfterSwizzle;
                switch (imgFormat.Name)
                {
                    case "ETC1A4":
                        imageDataAfterSwizzle = new ETC1(true, width, height).Decompress(ms.ToArray());
                        break;
                    default:
                        imageDataAfterSwizzle = ms.ToArray();
                        break;
                }

                Color[] resultArray = Enumerable.Range(0, imageDataAfterSwizzle.Length / imgFormat.Size)
                    .Select(i => imageDataAfterSwizzle.Skip(i * imgFormat.Size).Take(imgFormat.Size))
                    .Select(group => imgFormat.Decode(group.ToArray()))
                    .ToArray();

                foreach (var pair in points.Zip(resultArray, Tuple.Create))
                {
                    int x = pair.Item1.X, y = pair.Item1.Y;
                    if (0 <= x && x < width && 0 <= y && y < height)
                    {
                        var color = pair.Item2;
                        int pixelOffset = y * stride + x;
                        int pixelValue = color.ToArgb();
                        Marshal.WriteInt32(data.Scan0 + pixelOffset * 4, pixelValue);
                    }
                }

                bmp.UnlockBits(data);

                return bmp;
            }
        }
    }
}