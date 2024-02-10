using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Compression;
using YKWrandomizer.Level5.Compression.NoCompression;
using YKWrandomizer.Level5.Compression.Zlib;
using YKWrandomizer.Level5.Compression.LZ10;

namespace YKWrandomizer.Level5.Archive.XPCK
{
    public class XPCK : IArchive
    {
        public string Name => "XPCK";

        public Stream BaseStream;

        public VirtualDirectory Directory { get; set; }

        public XPCKSupport.Header Header;

        public XPCK()
        {
            Directory = new VirtualDirectory("/");
        }

        public XPCK(Stream stream)
        {
            BaseStream = stream;
            Directory = Open();
        }

        public XPCK(byte[] fileByteArray)
        {
            BaseStream = new MemoryStream(fileByteArray);
            Directory = Open();
        }

        public VirtualDirectory Open()
        {
            VirtualDirectory folder = new VirtualDirectory();

            BinaryDataReader data = new BinaryDataReader(BaseStream);
            Header = data.ReadStruct<XPCKSupport.Header>();

            // File Entry
            data.Seek(Header.FileInfoOffset);
            XPCKSupport.FileEntry[] entries = data.ReadMultipleStruct<XPCKSupport.FileEntry>(Header.FileCount);

            // Name
            data.Seek(Header.FilenameTableOffset);
            byte[] fileNamesComp = Compressor.Decompress(data.GetSection(Header.DataOffset - Header.FilenameTableOffset));
            BinaryDataReader fileNames = new BinaryDataReader(fileNamesComp);

            foreach (var file in entries)
            {
                fileNames.Seek((uint)file.NameOffset);
                string fileName = fileNames.ReadString(Encoding.GetEncoding("Shift-JIS"));

                folder.AddFile(fileName, new SubMemoryStream(BaseStream, Header.DataOffset + file.FileOffset, file.FileSize));
            }

            folder.SortAlphabetically();

            return folder;
        }

        public void Save(string fileName, ProgressBar progressBar)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                BinaryDataWriter writer = new BinaryDataWriter(stream);

                long fileEntriesOffset = 0;
                long fileNameTableOffset = 0;
                long dataOffset = 0;

                int fileEntriesSize = 0;
                int fileNameTableSize = 0;
                int dataSize = 0;

                int nameOffset = 0;
                List<byte[]> tableName = new List<byte[]>();
                Dictionary<string, int> nameOffsets = new Dictionary<string, int>();
                var fileEntries = new List<XPCKSupport.FileEntry>();

                foreach (var file in Directory.Files)
                {
                    byte[] fileNameByte = Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key + '\0');
                    tableName.Add(fileNameByte);
                    nameOffsets.Add(file.Key, nameOffset);
                    nameOffset += fileNameByte.Length;
                }

                long totalBytes = Directory.Files.Sum(file => file.Value.Size);
                long bytesWritten = 0;

                Dictionary<string, (int, int)> fileOffset = new Dictionary<string, (int, int)>();
                MemoryStream dataStream = new MemoryStream();
                BinaryDataWriter dataWriter = new BinaryDataWriter(dataStream);

                foreach (var file in Directory.Files)
                {
                    long offset = dataWriter.BaseStream.Position;

                    file.Value.CopyTo(dataStream);

                    long newOffset = dataWriter.BaseStream.Position;
                    int shift = 0;

                    if (newOffset % 16 != 0)
                    {
                        shift = (int)(16 - (newOffset % 16));
                        dataWriter.WriteAlignment(16);
                    }

                    fileOffset.Add(file.Key, ((int)offset, shift));

                    bytesWritten += newOffset - offset;
                    progressBar.Value = (int)((double)bytesWritten / totalBytes * 100);
                }

                // Retrieve the files and sort them based on CRC32
                var sortedFiles = Directory.Files.OrderBy(file =>
                {
                    return BitConverter.ToUInt32(new Crc32().ComputeHash(Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key)).Reverse().ToArray(), 0);
                });

                foreach (var file in sortedFiles)
                {
                    uint size;

                    if (file.Value.ByteContent == null)
                    {
                        size = (uint)file.Value.Size;
                    }
                    else
                    {
                        size = (uint)file.Value.ByteContent.Length;
                    }

                    uint shifted_offset = (uint)fileOffset[file.Key].Item1 >> 2;
                    ushort offset_higher = (ushort)(shifted_offset & 0xFFFF);
                    byte offset_lower = (byte)(shifted_offset >> 16);

                    int file_size = (int)size + fileOffset[file.Key].Item2;
                    ushort size_higher = (ushort)(file_size & 0xFFFF);
                    byte size_lower = (byte)(file_size >> 16);

                    var entryFile = new XPCKSupport.FileEntry();
                    entryFile.Crc32 = BitConverter.ToUInt32(new Crc32().ComputeHash(Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key)).Reverse().ToArray(), 0);
                    entryFile.NameOffset = (ushort)nameOffsets[file.Key];
                    entryFile.tmp = offset_higher;
                    entryFile.tmp2 = size_higher;
                    entryFile.tmpZ = offset_lower;
                    entryFile.tmp2Z = size_lower;

                    fileEntries.Add(entryFile);
                }

                // Write file entries
                writer.Seek(0x14);
                fileEntriesOffset = stream.Position;
                byte[] fileInfo = SerializeData(fileEntries.ToArray());
                fileEntriesSize = fileInfo.Length;
                writer.Write(fileInfo);
                writer.WriteAlignment(4);

                // Write name table
                fileNameTableOffset = stream.Position;
                byte[] tableNameData = CompressBlockTo<byte>(tableName.SelectMany(bytes => bytes).ToArray(), new LZ10());
                fileNameTableSize = tableNameData.Length;
                writer.Write(tableNameData);
                writer.WriteAlignment(16);

                dataOffset = stream.Position;
                dataSize = (int)dataStream.Length;
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.CopyTo(stream);

                // Write header
                var header = Header;
                byte[] f1f2 = XPCKSupport.CalculateF1F2(Directory.Files.Count);
                header.Magic = 0x4B435058;
                header.fc1 = f1f2[0];
                header.fc2 = f1f2[1];
                header.tmp1 = (ushort)(fileEntriesOffset >> 2);
                header.tmp2 = (ushort)(fileNameTableOffset >> 2);
                header.tmp3 = (ushort)(dataOffset >> 2);
                header.tmp4 = (ushort)(fileEntriesSize >> 2);
                header.tmp5 = (ushort)(fileNameTableSize >> 2);
                header.tmp6 = (uint)(dataSize >> 2);
                writer.Seek(0);
                writer.WriteStruct(header);
            }
        }

        public byte[] Save()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryDataWriter writer = new BinaryDataWriter(stream);

                long fileEntriesOffset = 0;
                long fileNameTableOffset = 0;
                long dataOffset = 0;

                int fileEntriesSize = 0;
                int fileNameTableSize = 0;
                int dataSize = 0;

                int nameOffset = 0;
                List<byte[]> tableName = new List<byte[]>();
                Dictionary<string, int> nameOffsets = new Dictionary<string, int>();
                var fileEntries = new List<XPCKSupport.FileEntry>();

                foreach (var file in Directory.Files)
                {
                    byte[] fileNameByte = Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key + '\0');
                    tableName.Add(fileNameByte);
                    nameOffsets.Add(file.Key, nameOffset);
                    nameOffset += fileNameByte.Length;
                }

                long totalBytes = Directory.Files.Sum(file => file.Value.Size);
                long bytesWritten = 0;

                Dictionary<string, (int, int)> fileOffset = new Dictionary<string, (int, int)>();
                MemoryStream dataStream = new MemoryStream();
                BinaryDataWriter dataWriter = new BinaryDataWriter(dataStream);

                foreach (var file in Directory.Files)
                {
                    long offset = dataWriter.BaseStream.Position;

                    file.Value.CopyTo(dataStream);

                    long newOffset = dataWriter.BaseStream.Position;
                    int shift = 0;

                    if (newOffset % 16 != 0)
                    {
                        shift = (int)(16 - (newOffset % 16));
                        dataWriter.WriteAlignment(16);
                    }

                    fileOffset.Add(file.Key, ((int)offset, shift));

                    bytesWritten += newOffset - offset;
                }

                // Retrieve the files and sort them based on CRC32
                var sortedFiles = Directory.Files.OrderBy(file =>
                {
                    return BitConverter.ToUInt32(new Crc32().ComputeHash(Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key)).Reverse().ToArray(), 0);
                });

                foreach (var file in sortedFiles)
                {
                    uint size;

                    if (file.Value.ByteContent == null)
                    {
                        size = (uint)file.Value.Size;
                    }
                    else
                    {
                        size = (uint)file.Value.ByteContent.Length;
                    }

                    uint shifted_offset = (uint)fileOffset[file.Key].Item1 >> 2;
                    ushort offset_higher = (ushort)(shifted_offset & 0xFFFF);
                    byte offset_lower = (byte)(shifted_offset >> 16);

                    int file_size = (int)size + fileOffset[file.Key].Item2;
                    ushort size_higher = (ushort)(file_size & 0xFFFF);
                    byte size_lower = (byte)(file_size >> 16);

                    var entryFile = new XPCKSupport.FileEntry();
                    entryFile.Crc32 = BitConverter.ToUInt32(new Crc32().ComputeHash(Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key)).Reverse().ToArray(), 0);
                    entryFile.NameOffset = (ushort)nameOffsets[file.Key];
                    entryFile.tmp = offset_higher;
                    entryFile.tmp2 = size_higher;
                    entryFile.tmpZ = offset_lower;
                    entryFile.tmp2Z = size_lower;

                    fileEntries.Add(entryFile);
                }

                // Write file entries
                writer.Seek(0x14);
                fileEntriesOffset = stream.Position;
                byte[] fileInfo = SerializeData(fileEntries.ToArray());
                fileEntriesSize = fileInfo.Length;
                writer.Write(fileInfo);
                writer.WriteAlignment(4);

                // Write name table
                fileNameTableOffset = stream.Position;
                byte[] tableNameData = CompressBlockTo<byte>(tableName.SelectMany(bytes => bytes).ToArray(), new LZ10());
                fileNameTableSize = tableNameData.Length;
                writer.Write(tableNameData);
                writer.WriteAlignment(16);

                dataOffset = stream.Position;
                dataSize = (int)dataStream.Length;
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.CopyTo(stream);

                // Write header
                var header = Header;
                byte[] f1f2 = XPCKSupport.CalculateF1F2(Directory.Files.Count);
                header.Magic = 0x4B435058;
                header.fc1 = f1f2[0];
                header.fc2 = f1f2[1];
                header.tmp1 = (ushort)(fileEntriesOffset >> 2);
                header.tmp2 = (ushort)(fileNameTableOffset >> 2);
                header.tmp3 = (ushort)(dataOffset >> 2);
                header.tmp4 = (ushort)(fileEntriesSize >> 2);
                header.tmp5 = (ushort)(fileNameTableSize >> 2);
                header.tmp6 = (uint)(dataSize >> 2);
                writer.Seek(0);
                writer.WriteStruct(header);

                return stream.ToArray();
            }
        }

        private byte[] CompressBlockTo<T>(T[] data, ICompression compression)
        {
            byte[] serializedData = SerializeData<T>(data);
            return compression.Compress(serializedData);
        }

        private byte[] SerializeData<T>(T[] data)
        {
            MemoryStream stream = new MemoryStream();

            BinaryDataWriter writer = new BinaryDataWriter(stream);
            writer.WriteMultipleStruct<T>(data);
            writer.Dispose();

            return stream.ToArray();
        }

        public void Close()
        {
            BaseStream?.Dispose();
            BaseStream = null;
            Directory = null;
        }
    }
}
