using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Compression;
using YKWrandomizer.Level5.Compression.NoCompression;
using YKWrandomizer.Level5.Compression.Zlib;
using YKWrandomizer.Level5.Compression.LZ10;

namespace YKWrandomizer.Level5.Script
{
    public class XQuerry
    {
        public Stream BaseStream;

        public XQuerrySupport.Header Header;

        public Dictionary<string, byte[]> Content;

        public XQuerry(Stream stream)
        {
            BaseStream = stream;
            Content = Open();
        }

        public XQuerry(byte[] fileByteArray)
        {
            BaseStream = new MemoryStream(fileByteArray);
            Content = Open();
        }

        public Dictionary<string, byte[]> Open()
        {
            BinaryDataReader data = new BinaryDataReader(BaseStream);
            Header = data.ReadStruct<XQuerrySupport.Header>();;

            byte[] functionData = Compressor.Decompress(data.GetSection((uint)Header.FunctionOffsetShifted, Header.JumpOffsetShifted - Header.FunctionOffsetShifted));
            byte[] jumpData = Compressor.Decompress(data.GetSection((uint)Header.JumpOffsetShifted, Header.InstructionOffsetShifted - Header.JumpOffsetShifted));
            byte[] instructionData = Compressor.Decompress(data.GetSection((uint)Header.InstructionOffsetShifted, Header.ArgumentOffsetShifted - Header.InstructionOffsetShifted));
            byte[] argumentData = Compressor.Decompress(data.GetSection((uint)Header.ArgumentOffsetShifted, Header.StringOffsetShifted - Header.ArgumentOffsetShifted));
            byte[] stringData = Compressor.Decompress(data.GetSection((uint)Header.StringOffsetShifted, (int)data.Length - Header.StringOffsetShifted));

            return new Dictionary<string, byte[]>()
            {
                { "Function", functionData },
                { "Jump", jumpData },
                { "Instruction", instructionData },
                { "Argument", argumentData },
                { "String", stringData },
            };
        }

        public byte[] Save()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryDataWriter writer = new BinaryDataWriter(stream);

                writer.Seek(0x20);
                Header.FunctionOffset = (short)(0x20 >> 2);
                writer.Write(CompressBlockTo<byte>(Content["Function"], new NoCompression()));
                Header.JumpOffset = (short)(writer.Position >> 2);
                writer.Write(CompressBlockTo<byte>(Content["Jump"], new NoCompression()));
                Header.InstructionOffset = (short)(writer.Position >> 2);
                writer.Write(CompressBlockTo<byte>(Content["Instruction"], new NoCompression()));
                Header.ArgumentOffset = (short)(writer.Position >> 2);
                writer.Write(CompressBlockTo<byte>(Content["Argument"], new NoCompression()));
                Header.StringOffset = (short)(writer.Position >> 2);
                writer.Write(CompressBlockTo<byte>(Content["String"], new NoCompression()));
                writer.Seek(0x00);
                writer.WriteStruct(Header);

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
            Content.Clear();
        }
    }
}
