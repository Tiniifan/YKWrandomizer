using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BitConverter;

namespace YKWrandomizer.Tool
{
    public class BinaryDataReader : IDisposable
    {
        private EndianBitConverter _converter;
        private Stream _stream;

        public bool BigEndian { get; set; } = false;

        public long Length { get => _stream.Length; }

        public Stream BaseStream { get => _stream; }

        public long Position { get => _stream.Position; }

        public BinaryDataReader(byte[] data)
        {
            _stream = new MemoryStream(data);
            _converter = BigEndian ? EndianBitConverter.BigEndian : EndianBitConverter.LittleEndian;
        }

        public BinaryDataReader(Stream stream)
        {
            _stream = stream;
            _converter = BigEndian ? EndianBitConverter.BigEndian : EndianBitConverter.LittleEndian;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public T ReadValue<T>()
        {
            byte[] bytes = new byte[Marshal.SizeOf(typeof(T))];
            _stream.Read(bytes, 0, bytes.Length);

            if (typeof(T) == typeof(byte))
            {
                return (T)(object)bytes[0];
            }

            if (_converter.IsLittleEndian && BigEndian)
            {
                Array.Reverse(bytes);
            }

            return (T)typeof(EndianBitConverter).GetMethod("To" + typeof(T).Name).Invoke(_converter, new object[] { bytes, 0 });
        }

        public T[] ReadMultipleValue<T>(int count)
        {
            return Enumerable.Range(0, count).Select(x => ReadValue<T>()).ToArray();
        }

        public string ReadString(Encoding encoding)
        {
            List<byte> bytes = new List<byte>();
            int b;

            while ((b = _stream.ReadByte()) != 0x0 && _stream.Position < _stream.Length)
            {
                bytes.Add((byte)b);
            }

            return encoding.GetString(bytes.ToArray());
        }

        public void Skip(uint Size)
        {
            _stream.Seek(Size, SeekOrigin.Current);
        }

        public void Seek(uint Position)
        {
            _stream.Seek(Position, SeekOrigin.Begin);
        }

        public byte[] GetSection(int Size)
        {
            byte[] data = new byte[Size];
            _stream.Read(data, 0, data.Length);
            return data;
        }

        public byte[] GetSection(uint Offset, int Size)
        {
            long temp = _stream.Position;
            Seek(Offset);
            byte[] data = new byte[Size];
            _stream.Read(data, 0, data.Length);
            Seek((uint)temp);
            return data;
        }

        public long Find<T>(T search, uint start) where T : struct, IEquatable<T>
        {
            int count = (int)(_stream.Length - start) / Marshal.SizeOf(typeof(T));

            long temp = _stream.Position;
            Seek(start);

            T[] tableSearch = ReadMultipleStruct<T>(count);
            int foundIndex = Array.IndexOf(tableSearch, search);

            Seek((uint)temp);

            if (foundIndex != -1)
            {
                return start + foundIndex * Marshal.SizeOf(typeof(T));
            }
            else
            {
                return -1;
            }
        }

        public void SeekOf<T>(T search, uint start) where T : struct, IEquatable<T>
        {
            long pos = Find(search, start);

            if (pos != -1)
            {
                Seek((uint)pos);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void PrintPosition()
        {
            Console.WriteLine(_stream.Position.ToString("X"));
        }

        public T ReadStruct<T>()
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            _stream.Read(bytes, 0, size);

            if (_converter.IsLittleEndian && BigEndian)
                Array.Reverse(bytes);

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public T[] ReadMultipleStruct<T>(int count)
        {
            return Enumerable.Range(0, count).Select(x => ReadStruct<T>()).ToArray();
        }
    }
}
