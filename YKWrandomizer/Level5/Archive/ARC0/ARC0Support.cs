using System;

namespace YKWrandomizer.Level5.Archive.ARC0
{
    public static class ARC0Support
    {
        public struct Header
        {
            public UInt32 Magic;
            public int DirectoryEntriesOffset;
            public int DirectoryHashOffset;
            public int FileEntriesOffset;
            public int NameOffset;
            public int DataOffset;
            public short DirectoryEntriesCount;
            public short DirectoryHashCount;
            public int FileEntriesCount;
            public int TableChunkSize;
            public int Zero1;

            //Hashes?
            public uint Unk2;
            public uint Unk3;
            public uint Unk4;
            public uint Unk5;

            public int DirectoryCount;
            public int FileCount;
            public uint Unk7;
            public int Zero2;
        }

        public struct DirectoryEntry
        {
            public uint Crc32;
            public ushort FirstDirectoryIndex;
            public short DirectoryCount;
            public ushort FirstFileIndex;
            public short FileCount;
            public int FileNameStartOffset;
            public int DirectoryNameStartOffset;
        }

        public struct FileEntry
        {
            public uint Crc32;
            public uint NameOffsetInFolder;
            public uint FileOffset;
            public uint FileSize;
        }
    }
}

