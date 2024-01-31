using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Compression;
using YKWrandomizer.Level5.Compression.NoCompression;

namespace YKWrandomizer.Level5.Archive.ARC0
{
    public class ARC0 : IArchive
    {
        public string Name => "ARC0";

        public VirtualDirectory Directory { get; set; }

        public Stream BaseStream;

        public ARC0Support.Header Header;

        public ARC0()
        {
            Directory = new VirtualDirectory("/");
        }

        public ARC0(Stream stream)
        {
            BaseStream = stream;
            Directory = Open();
        }

        public VirtualDirectory Open()
        {
            VirtualDirectory folder = new VirtualDirectory();

            BinaryDataReader data = new BinaryDataReader(BaseStream);
            var header = data.ReadStruct<ARC0Support.Header>();
            Header = header;

            // directory entries
            data.Seek((uint)header.DirectoryEntriesOffset);
            byte[] directoryEntriesComp = data.GetSection(header.DirectoryHashOffset - header.DirectoryEntriesOffset);
            var directoryEntries = DecompressBlockTo<ARC0Support.DirectoryEntry>(directoryEntriesComp, header.DirectoryEntriesCount);

            // directory hashes
            data.Seek((uint)header.DirectoryHashOffset);
            byte[] directoryHashesComp = data.GetSection(header.FileEntriesOffset - header.DirectoryHashOffset);
            var directoryHashes = DecompressBlockTo<uint>(directoryHashesComp, header.DirectoryHashCount);

            // File Entry Table
            data.Seek((uint)header.FileEntriesOffset);
            var fileEntriesComp = data.GetSection(header.NameOffset - header.FileEntriesOffset);
            var fileEntries = DecompressBlockTo<ARC0Support.FileEntry>(fileEntriesComp, header.FileEntriesCount);

            // NameTable
            data.Seek((uint)header.NameOffset);
            byte[] fileNamesComp = Compressor.Decompress(data.GetSection(header.DataOffset - header.NameOffset));
            BinaryDataReader fileNames = new BinaryDataReader(fileNamesComp);

            foreach (var directory in directoryEntries)
            {
                fileNames.Seek((uint)directory.DirectoryNameStartOffset);
                string directoryName = fileNames.ReadString(Encoding.GetEncoding("Shift-JIS"));
                VirtualDirectory newFolder = new VirtualDirectory(directoryName);

                var filesInDirectory = fileEntries.Skip(directory.FirstFileIndex).Take(directory.FileCount);

                foreach (var file in filesInDirectory)
                {
                    fileNames.Seek((uint)(directory.FileNameStartOffset + file.NameOffsetInFolder));
                    var fileName = fileNames.ReadString(Encoding.GetEncoding("Shift-JIS"));

                    if (directoryName == "")
                    {
                        folder.AddFile(fileName, new SubMemoryStream(BaseStream, header.DataOffset + file.FileOffset, file.FileSize));
                    }
                    else
                    {
                        newFolder.AddFile(fileName, new SubMemoryStream(BaseStream, header.DataOffset + file.FileOffset, file.FileSize));
                    }
                }

                folder.AddFolder(newFolder);
            }

            folder.Reorganize();
            folder.SortAlphabetically();

            return folder;
        }

        public void Save(string fileName, ProgressBar progressBar = null)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                BinaryDataWriter writer = new BinaryDataWriter(stream);

                int tableNameOffset = 0;
                int firstFileIndex = 0;
                uint fileOffset = 0;

                List<byte[]> tableName = new List<byte[]>();
                var directoryEntries = new List<ARC0Support.DirectoryEntry>();
                var fileEntries = new List<ARC0Support.FileEntry>();
                Dictionary<ARC0Support.FileEntry, SubMemoryStream> files = new Dictionary<ARC0Support.FileEntry, SubMemoryStream>();
                Dictionary<string, VirtualDirectory> folders = Directory.GetAllFoldersAsDictionnary();

                foreach (var folder in folders)
                {
                    // Remove first slash from directory name
                    string directoryName = folder.Key.Substring(1, folder.Key.Length - 1);
                    byte[] directoryNameByte = Encoding.GetEncoding("Shift-JIS").GetBytes(directoryName + '\0');
                    tableName.Add(directoryNameByte);

                    var directoryEntry = new ARC0Support.DirectoryEntry();

                    directoryEntry.Crc32 = System.BitConverter.ToUInt32(new Crc32().ComputeHash(Encoding.UTF8.GetBytes(directoryName)).Reverse().ToArray(), 0);
                    if (directoryEntry.Crc32 == 0)
                    {
                        directoryEntry.Crc32 = 0xFFFFFFFF;
                    }

                    directoryEntry.DirectoryCount = (short)folder.Value.Folders.Count;
                    directoryEntry.FirstFileIndex = (ushort)firstFileIndex;
                    directoryEntry.FileCount = (short)folder.Value.Files.Count();
                    directoryEntry.DirectoryNameStartOffset = tableNameOffset;
                    directoryEntry.FileNameStartOffset = tableNameOffset + directoryNameByte.Length;
                    directoryEntries.Add(directoryEntry);

                    tableNameOffset += directoryNameByte.Length;
                    firstFileIndex += folder.Value.Files.Count();
                    int nameOffsetInFolder = 0;

                    // File Information
                    var fileEntryFromFolder = new List<ARC0Support.FileEntry>();
                    Dictionary<string, SubMemoryStream> filesInFolder = folder.Value.Files.OrderBy(file => file.Key).ToDictionary(file => file.Key, file => file.Value);
                    foreach (var file in filesInFolder)
                    {
                        byte[] fileNameByte = Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key + '\0');
                        tableName.Add(fileNameByte);

                        var entryFile = new ARC0Support.FileEntry();
                        entryFile.Crc32 = System.BitConverter.ToUInt32(new Crc32().ComputeHash(Encoding.GetEncoding("Shift-JIS").GetBytes(file.Key)).Reverse().ToArray(), 0);
                        entryFile.NameOffsetInFolder = (uint)nameOffsetInFolder;
                        entryFile.FileOffset = fileOffset;

                        if (file.Value.ByteContent == null)
                        {
                            entryFile.FileSize = (uint)file.Value.Size;
                        }
                        else
                        {
                            entryFile.FileSize = (uint)file.Value.ByteContent.Length;
                        }

                        fileEntryFromFolder.Add(entryFile);
                        files.Add(entryFile, file.Value);

                        tableNameOffset += fileNameByte.Length;
                        nameOffsetInFolder += fileNameByte.Length;
                        fileOffset = (uint)((fileOffset + entryFile.FileSize + 3) & ~3);
                    }

                    fileEntries.AddRange(fileEntryFromFolder.OrderBy(x => x.Crc32));
                }

                // Order directory entries by hash and set directoryIndex accordingly
                var directoryIndex = 0;
                directoryEntries = directoryEntries.OrderBy(x => x.Crc32).Select(x =>
                {
                    x.FirstDirectoryIndex = (ushort)directoryIndex;
                    directoryIndex += x.DirectoryCount;
                    return x;
                }).ToList();

                // Calculate directory hashes
                var directoryHashes = directoryEntries.Select(x => x.Crc32).ToList();

                long totalBytes = files.Sum(file => file.Value.Size);
                long bytesWritten = 0;
                //totalBytes += directoryEntries.Count * 20;
                //totalBytes += directoryHashes.Count * 4;
                //totalBytes += fileEntries.Count * 16;

                // Write the directory entries
                writer.Seek(0x48);
                long directoryEntriesOffset = 0x48;
                writer.Write(CompressBlockTo<ARC0Support.DirectoryEntry>(directoryEntries.ToArray(), new NoCompression()));
                writer.WriteAlignment(4);

                // Write the directory hashes
                long directoryHashOffset = stream.Position;
                writer.Write(CompressBlockTo<uint>(directoryHashes.ToArray(), new NoCompression()));
                writer.WriteAlignment(4);

                // Write the file entries
                long fileEntriesOffset = stream.Position;
                writer.Write(CompressBlockTo<ARC0Support.FileEntry>(fileEntries.ToArray(), new NoCompression()));
                writer.WriteAlignment(4);

                // Write name table
                long fileNameTableOffset = stream.Position;
                byte[] tableNameArray = tableName.SelectMany(bytes => bytes).ToArray();
                writer.Write(CompressBlockTo<byte>(tableNameArray, new NoCompression()));
                writer.WriteAlignment(4);

                // Write the file data
                long dataOffset = stream.Position;
                foreach (ARC0Support.FileEntry file in fileEntries)
                {
                    writer.BaseStream.Position = dataOffset + file.FileOffset;
                    files[file].CopyTo(stream);
                    bytesWritten += file.FileSize;

                    if (progressBar != null)
                    {
                        progressBar.Value = (int)((double)bytesWritten / totalBytes * 100);
                    }
                }

                // Update the header
                var header = Header;
                header.Magic = 0x30435241;
                header.DirectoryEntriesOffset = (int)directoryEntriesOffset;
                header.DirectoryHashOffset = (int)directoryHashOffset;
                header.FileEntriesOffset = (int)fileEntriesOffset;
                header.NameOffset = (int)fileNameTableOffset;
                header.DataOffset = (int)dataOffset;
                header.DirectoryEntriesCount = (short)directoryEntries.Count;
                header.DirectoryHashCount = (short)directoryHashes.Count;
                header.FileEntriesCount = fileEntries.Count;
                header.DirectoryCount = directoryEntries.Count;
                header.FileCount = fileEntries.Count;
                header.TableChunkSize = (int)(directoryEntries.Count * 20 +
                                    directoryHashes.Count * 4 +
                                    fileEntries.Count * 16 +
                                   tableNameArray.Length + 0x20 + 3) & ~3;
                writer.Seek(0);
                writer.WriteStruct(header);
            }
        }

        private T[] DecompressBlockTo<T>(byte[] data, int count)
        {
            BinaryDataReader tableDecomp = new BinaryDataReader(Compressor.Decompress(data));
            return tableDecomp.ReadMultipleStruct<T>(count);
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

