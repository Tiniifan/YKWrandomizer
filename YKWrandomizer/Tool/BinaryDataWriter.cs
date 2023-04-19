using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BitConverter;

namespace YKWrandomizer.Tool
{
    public class BinaryDataWriter : IDisposable
    {
        private EndianBitConverter _converter;
        private Stream _stream;

        public bool BigEndian { get; set; } = false;

        public long Length { get => _stream.Length; }

        public Stream BaseStream { get => _stream; }

        public long Position { get => _stream.Position; }

        public BinaryDataWriter(byte[] data)
        {
            _stream = new MemoryStream(data);
            _converter = BigEndian ? EndianBitConverter.BigEndian : EndianBitConverter.LittleEndian;
        }

        public BinaryDataWriter(Stream stream)
        {
            _stream = stream;
            _converter = BigEndian ? EndianBitConverter.BigEndian : EndianBitConverter.LittleEndian;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void Skip(uint Size)
        {
            _stream.Seek(Size, SeekOrigin.Current);
        }

        public void Seek(uint Position)
        {
            _stream.Seek(Position, SeekOrigin.Begin);
        }

        public void PrintPosition()
        {
            Console.WriteLine(_stream.Position.ToString("X"));
        }

        public void Write(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(short value)
        {
            Write(System.BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            Write(System.BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            Write(System.BitConverter.GetBytes(value));
        }

        public void Write(ushort value)
        {
            Write(System.BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            Write(System.BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            Write(System.BitConverter.GetBytes(value));
        }

        public void WriteAlignment(int alignment = 16, byte alignmentByte = 0x0)
        {
            var remainder = BaseStream.Position % alignment;
            if (remainder <= 0) return;
            for (var i = 0; i < alignment - remainder; i++)
                Write(alignmentByte);
        }

        public void WriteAlignment()
        {
            Write((byte)0x00);
            WriteAlignment(16, 0xFF);
        }

        public void WriteStruct<T>(T structure)
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
            handle.Free();

            Write(bytes);
        }

        public void WriteMultipleStruct<T>(IEnumerable<T> structures)
        {
            foreach (T structure in structures)
            {
                WriteStruct(structure);
            }
        }
    }
}
