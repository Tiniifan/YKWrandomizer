using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Level5.Text
{
    public class T2bþ
    {
        List<Entry> Text = new List<Entry>();

        List<Entry> Noun = new List<Entry>();

        public T2bþ(byte[] data)
        {
            Read(data);
        }

        private void Read(byte[] data)
        {
            BinaryDataReader reader = new BinaryDataReader(data);

            var header = reader.ReadStruct<T2bþSupport.Header>();
            BinaryDataReader readerText = new BinaryDataReader(reader.GetSection((uint)header.StartText, header.LengthText));

            // Text Entry
            long textEntryOffset = reader.Find<uint>(0x0EFB9738, 0x10);
            if (textEntryOffset != -1)
            {
                reader.Seek((uint)textEntryOffset + 0x08);
                var entries = reader.ReadMultipleStruct<T2bþSupport.Text>(reader.ReadValue<int>());
                foreach (T2bþSupport.Text entry in entries)
                {
                    int offset = entry.TextOffset;
                    if (offset != -1)
                    {
                        readerText.Seek((uint)entry.TextOffset);
                        Text.Add(new Entry(entry.Crc32, entry.TextNumber, readerText.ReadString(Encoding.UTF8)));
                    }
                }
            }

            // Noun Entry
            long nounEntryOffset = reader.Find<uint>(0x5EA2CA19, 0x10);
            if (nounEntryOffset != -1)
            {
                reader.Seek((uint)nounEntryOffset + 0x08);
                var entries = reader.ReadMultipleStruct<T2bþSupport.Noun>(reader.ReadValue<int>());
                foreach (T2bþSupport.Noun entry in entries)
                {
                    int offset = entry.TextOffset;
                    if (offset != -1)
                    {
                        readerText.Seek((uint)offset);
                        Noun.Add(new Entry(entry.Crc32, entry.TextNumber, readerText.ReadString(Encoding.UTF8)));
                    }
                }
            }

            reader.Dispose();
            readerText.Dispose();
        }

        public string GetNounText(UInt32 key, int textNumber)
        {
            Entry noun = Noun.FirstOrDefault(x => x.Key == key && x.TextNumber == textNumber);

            if (noun != null)
            {
                return noun.Text;
            }
            else
            {
                return null;
            }
        }

        public string GetText(UInt32 key, int textNumber)
        {
            Entry noun = Text.FirstOrDefault(x => x.Key == key && x.TextNumber == textNumber);

            if (noun != null)
            {
                return noun.Text;
            }
            else
            {
                return null;
            }
        }
    }

    public class Entry
    {
        public uint Key;

        public int TextNumber;

        public string Text;

        public Entry(uint key, int textNumber, string text)
        {
            Key = key;
            TextNumber = textNumber;
            Text = text;
        }
    }
}
