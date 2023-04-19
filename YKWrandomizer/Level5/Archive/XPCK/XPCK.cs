using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Level5.Compression;
using YKWrandomizer.Level5.Compression.NoCompression;

namespace YKWrandomizer.Level5.Archive.XPCK
{
    public class XPCK
    {
        public Stream BaseStream;

        public VirtualDirectory Directory;

        public XPCKSupport.Header Header;

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

            return folder;
        }

        public void Save()
        {
            // No add implemented

            using (BinaryDataWriter writer = new BinaryDataWriter(BaseStream))
            {
                foreach (var file in Directory.Files)
                {
                    if (file.Value.ByteContent != null)
                    {
                        writer.BaseStream.Position = file.Value.Offset;
                        file.Value.CopyTo(writer.BaseStream);
                    }
                }
            }
        }

        public void Close()
        {
            BaseStream?.Dispose();
            BaseStream = null;
            Directory = null;
        }
    }
}
