using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Compression;
using YKWrandomizer.Level5.Compression.LZ10;
using YKWrandomizer.Level5.Compression.NoCompression;
using YKWrandomizer.Level5.Compression.ETC1;

namespace YKWrandomizer.Level5.Image
{
    public static class IMGC
    {
        public static Bitmap ToBitmap(byte[] fileContent)
        {
            BinaryDataReader data = new BinaryDataReader(fileContent);

            var header = data.ReadStruct<IMGCSupport.Header>();

            byte[] tileData = Compressor.Decompress(data.GetSection((uint)header.TileOffset, header.TileSize1));
            byte[] imageData = Compressor.Decompress(data.GetSection((uint) (header.TileOffset + header.TileSize2), header.ImageSize));

            return DecodeImage(tileData, imageData, IMGCSupport.ImageFormats[header.ImageFormat], header.Width, header.Height, header.BitDepth);
        }

        public static byte[] ToIMGC(Bitmap img, IColorFormat imgFormat)
        {
            using (MemoryStream outStream = new MemoryStream())
            using (BinaryDataWriter writer = new BinaryDataWriter(outStream))
            {
                // Get image properties
                int height = img.Height;
                int width = img.Width;

                // Get pixels
                Color[] px = GetColorArray(img);
                img.RotateFlip(RotateFlipType.RotateNoneFlipY);

                byte[] tileCompress = new NoCompression().Compress(ImageToTile(px, height, width));
                byte[] imageDataCompress = new NoCompression().Compress(EncodeImage(px, height, width, imgFormat));

                writer.Write(new byte[] { 0x49, 0x4D, 0x47, 0x43, 0x30, 0x30, 0x00, 0x00, 0x30, 0x00});
                writer.Write(IMGCSupport.ImageFormatTypes[imgFormat.GetType()]);
                writer.Write(new byte[] { 0x01, 0x01, 0x10, 0x80, 0x00 });
                writer.Write((short)width);
                writer.Write((short)height);
                writer.Write(new byte[] { 0x30, 0x00, 0x00, 0x00, 0x30, 0x00, 0x01, 0x00, 0x48, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                writer.Write((int)tileCompress.Length);
                writer.Write((int)tileCompress.Length);
                writer.Write((int)imageDataCompress.Length);
                writer.Write(BitConverter.GetBytes(0L));
                writer.Write(tileCompress);
                writer.Write(imageDataCompress);
                writer.WriteAlignment(16, 0x00);

                //Console.WriteLine(BitConverter.ToString(outStream.ToArray()).Replace("-", ""));
                return outStream.ToArray();
            }
        }

        private static Bitmap DecodeImage(byte[] tile, byte[] imageData, IColorFormat imgFormat, int width, int height, int bitDepth)
        {
            byte[] entryStart = null;

            using (var table = new BinaryDataReader(tile))
            using (var tex = new BinaryDataReader(imageData))
            {
                int tableLength = (int)table.BaseStream.Length;

                var tmp = table.ReadValue<ushort>();
                table.BaseStream.Position = 0;
                var entryLength = 2;
                if (tmp == 0x453)
                {
                    entryStart = table.ReadMultipleValue<byte>(8);
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

                byte[] pic;
                switch (imgFormat.Name)
                {
                    case "ETC1A4":
                        pic = new ETC1(true, width, height).Decompress(ms.ToArray());
                        break;
                    default:
                        pic = ms.ToArray();
                        break;
                }

                IMGCSwizzle imgcSwizzle = new IMGCSwizzle(width, height);
                var points = imgcSwizzle.GetPointSequence();

                int pixelCount = width * height;
                Color[] resultArray = new Color[pixelCount];

                for (int i = 0; i < pixelCount; i++)
                {
                    int dataIndex = i * imgFormat.Size;
                    byte[] group = new byte[imgFormat.Size];
                    Array.Copy(pic, dataIndex, group, 0, imgFormat.Size);
                    resultArray[i] = imgFormat.Decode(group);
                }

                var bmp = new Bitmap(width, height);
                var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                foreach (var pair in points.Zip(resultArray, Tuple.Create))
                {
                    int x = pair.Item1.X, y = pair.Item1.Y;
                    if (0 <= x && x < width && 0 <= y && y < height)
                    {
                        var color = pair.Item2;
                        int pixelOffset = data.Stride * y / 4 + x;
                        int pixelValue = color.ToArgb();
                        Marshal.WriteInt32(data.Scan0 + pixelOffset * 4, pixelValue);
                    }
                }

                bmp.UnlockBits(data);

                return bmp;
            }
        }

        private static Color[] GetColorArray(Bitmap img)
        {
            byte[] px = new byte[img.Width * img.Height * 4];
            BitmapData bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bmpData.Scan0, px, 0, px.Length);
            img.UnlockBits(bmpData);

            Color[] colors = new Color[px.Length / 4];

            for (int i = 0; i < colors.Length; i++)
            {
                int startIndex = i * 4;
                colors[i] = Color.FromArgb(px[startIndex + 3], px[startIndex + 2], px[startIndex + 1], px[startIndex]);
            }

            return colors;
        }

        private static byte[] ImageToTile(Color[] px, int height, int width)
        {
            List<Color[]> tiles = new List<Color[]>();
            List<byte> result = new List<byte>();

            for (int h = 0; h < height; h += 8)
            {
                for (int w = 0; w < width; w += 8)
                {
                    Color[] tile = new Color[64];

                    int index = 0;
                    for (int bh = 0; bh < 8; bh++)
                    {
                        for (int bw = 0; bw < 8; bw++)
                        {
                            tile[index] = px[(w + bw) + (h + bh) * width];
                            index++;
                        }
                    }

                    int indexInTiles = tiles.IndexOf(tile);

                    if (indexInTiles == -1)
                    {
                        tiles.Add(tile);
                        result.AddRange(BitConverter.GetBytes((short)(tiles.Count - 1)));
                    } else
                    {
                        result.AddRange(BitConverter.GetBytes((short)index));
                    }
                }
            }

            return result.ToArray();
        }

        private static List<int> Zorder = new List<int>
        {
            0, 2, 8, 10, 32, 34, 40, 42,
            1, 3, 9, 11, 33, 35, 41, 43,
            4, 6, 12, 14, 36, 38, 44, 46,
            5, 7, 13, 15, 37, 39, 45, 47,
            16, 18, 24, 26, 48, 50, 56, 58,
            17, 19, 25, 27, 49, 51, 57, 59,
            20, 22, 28, 30, 52, 54, 60, 62,
            21, 23, 29, 31, 53, 55, 61, 63
        };

        public static byte[] EncodeImage(Color[] px, int height, int width, IColorFormat imgFormat)
        {
            List<byte> outBytes = new List<byte>();
            List<Color[]> tiles = new List<Color[]>();

            for (int h = 0; h < height; h += 8)
            {
                for (int w = 0; w < width; w += 8)
                {
                    Color[] tile = new Color[64];

                    int index = 0;
                    for (int bh = 0; bh < 8; bh++)
                    {
                        for (int bw = 0; bw < 8; bw++)
                        {
                            tile[index] = px[(w + bw) + (h + bh) * width];
                            index++;
                        }
                    }

                    if (!tiles.Contains(tile))
                    {
                        tiles.Add(tile);

                        for (int bh = 0; bh < 8; bh++)
                        {
                            for (int bw = 0; bw < 8; bw++)
                            {
                                int pos = bw + bh * 8;
                                for (int i = 0; i < Zorder.Count; i++)
                                {
                                    if (Zorder[i] == pos)
                                    {
                                        outBytes.AddRange(imgFormat.Encode(tile[i]));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return outBytes.ToArray();
        }
    }
}
