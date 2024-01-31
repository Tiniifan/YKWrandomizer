using System;
using System.IO;
using System.Drawing;

namespace YKWrandomizer.Tools
{
    public class SubMemoryStream
    {
        public long Offset;

        public long Size;

        public byte[] ByteContent;

        public Stream BaseStream;

        public Color Color = Color.Black;

        public SubMemoryStream(byte[] data)
        {
            Offset = 0;
            Size = data.Length;
            ByteContent = data;
        }

        public SubMemoryStream(Stream baseStream, long offset, long size)
        {
            Offset = offset;
            Size = size;
            BaseStream = baseStream;
        }

        public void Read()
        {
            ByteContent = new byte[Size];
            BaseStream.Seek(Offset, SeekOrigin.Begin);
            BaseStream.Read(ByteContent, 0, ByteContent.Length);
        }

        public void Seek()
        {
            BaseStream.Seek(Offset, SeekOrigin.Begin);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            // Adjust count to read within the available range
            long remainingBytes = (Offset + Size) - BaseStream.Position;
            if (remainingBytes <= 0)
                return 0;

            int bytesToRead = (int)Math.Min(count, remainingBytes);

            // Read bytes from the base stream
            int bytesRead = BaseStream.Read(buffer, offset, bytesToRead);

            return bytesRead;
        }

        public void CopyTo(Stream destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (BaseStream == null && ByteContent != null)
            {
                destination.Write(ByteContent, 0, ByteContent.Length);
                return;
            }

            if (!BaseStream.CanRead)
            {
                throw new InvalidOperationException("SubMemoryStream n'est pas lisible.");
            }

            if (ByteContent == null || ByteContent.Length == 0)
            {
                long offset = Offset;
                long length = Size;

                byte[] buffer = new byte[4096];

                BaseStream.Seek(offset, SeekOrigin.Begin);

                int bytesRead;
                while (length > 0 && (bytesRead = BaseStream.Read(buffer, 0, (int)Math.Min(length, buffer.Length))) > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                    length -= bytesRead;
                }
            }
            else
            {
                destination.Write(ByteContent, 0, ByteContent.Length);
            }
        }
    }
}

