using System.Text;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Level5.Text
{
    public class LongText
    {
        public List<Entry> Entry;

        public LongText()
        {
            Entry = new List<Entry>();
        }

        public byte[] OffsetToByteArray(int textOffset)
        {
            if (Entry.Count > 0)
            {
                long outputBlock = 12 + Entry.Count() * 24 + 4;
                long emptyBlock = 16 - outputBlock % 16;
                byte[] output = new byte[outputBlock + emptyBlock];

                DataWriter writer = new DataWriter(output);
                writer.Write(new byte[] { 0x38, 0x97, 0xFB, 0x0E, 0x01, 0x01, 0xFF, 0xFF });
                writer.WriteInt32(Entry.Count());

                for (int i = 0; i < Entry.Count; i++)
                {
                    writer.Write(new byte[] { 0x89, 0x7E, 0xF4, 0x49, 0x04, 0x45, 0xFF, 0xFF });
                    writer.WriteUInt32(Entry[i].Key);
                    writer.WriteInt32(0x00);
                    writer.WriteInt32(textOffset);
                    writer.WriteInt32(0x00);

                    textOffset += Encoding.UTF8.GetByteCount(Entry[i].Text) + 1;
                }

                writer.Write(new byte[] { 0xA7, 0x09, 0xDE, 0x6A });

                // Write Empty Block
                for (int i = 0; i < emptyBlock; i++)
                {
                    writer.WriteByte(0xFF);
                }

                return output;
            }
            else
            {
                return new byte[] { };
            }
        }

        public byte[] TextToByteArray()
        {
            if (Entry.Count > 0)
            {
                byte[] output = new byte[Encoding.UTF8.GetByteCount(string.Join("", Entry.Select(x => x.Text).ToList())) + Entry.Count];

                DataWriter writer = new DataWriter(output);

                for (int i = 0; i < Entry.Count; i++)
                {
                    writer.Write(Encoding.UTF8.GetBytes(Entry[i].Text));
                    writer.WriteByte(0x00);
                }

                return output;
            }
            else
            {
                return new byte[] { };
            }
        }
    }
}
