﻿using System;
using System.IO;

namespace YKWrandomizer.Tools
{
    public class DataWriter : BinaryWriter
    {
        public bool BigEndian { get; set; } = false;

        public long Length { get => BaseStream.Length; }

        public DataWriter(byte[] data) : base(new MemoryStream(data))
        {

        }

        public DataWriter(string path) : base(new FileStream(path, FileMode.Open))
        {

        }

        public override void Write(byte[] data)
        {
            BaseStream.Write(data, 0, data.Length);
        }

        public void WriteByte(int number)
        {
            BaseStream.WriteByte(Convert.ToByte(number));
        }

        public void WriteInt16(int number)
        {
            byte[] data = BitConverter.GetBytes(number);

            for (int i = 0; i < 2; i++)
            {
                WriteByte(data[i]);
            }
        }

        public void WriteInt32(int number)
        {
            byte[] data = BitConverter.GetBytes(number);

            for (int i = 0; i < 4; i++)
            {
                WriteByte(data[i]);
            }
        }

        public void WriteUInt16(int number)
        {
            byte[] data = BitConverter.GetBytes(number);
            Array.Reverse(data);

            for (int i = 0; i < 2; i++)
            {
                WriteByte(data[i]);
            }
        }

        public void WriteUInt32(uint number)
        {
            byte[] data = BitConverter.GetBytes(number);
            Array.Reverse(data);
            Write(data, 0, data.Length);
        }

        public void Skip(uint Size)
        {
            BaseStream.Seek(Size, SeekOrigin.Current);
        }

        public void Seek(uint Position)
        {
            BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public void PrintPosition()
        {
            Console.WriteLine(BaseStream.Position.ToString("X"));
        }
    }
}
