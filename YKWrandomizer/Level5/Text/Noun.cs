using System.Text;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Level5.Text
{
    public class Noun
    {
        public List<Entry> Entry;

        public Noun()
        {
            Entry = new List<Entry>();
        }

        public byte[] OffsetToByteArray(int textOffset)
        {
            if (Entry.Count > 0)
            {
                long outputBlock = 12 + Entry.Count() * 68 + 4;
                long emptyBlock = 16 - outputBlock % 16;
                byte[] output = new byte[outputBlock + emptyBlock];

                DataWriter writer = new DataWriter(output);
                writer.Write(new byte[] { 0x19, 0xCA, 0xA2, 0x5E, 0x01, 0x01, 0xFF, 0xFF });
                writer.WriteInt32(Entry.Count());

                for (int i = 0; i < Entry.Count; i++)
                {
                    writer.Write(new byte[] { 0x93, 0xEA, 0x40, 0x22, 0x0E, 0x05, 0x00, 0x50, 0x05, 0xFF, 0xFF, 0xFF });
                    writer.WriteUInt32(Entry[i].Key);
                    writer.WriteInt32(Entry[i].VariantKey);

                    // Empty byte
                    for (int j = 0; j < 3; j++)
                    {
                        writer.WriteUInt32(0xFFFFFFFF);
                    }

                    writer.WriteInt32(textOffset);

                    // Empty byte
                    for (int j = 0; j < 4; j++)
                    {
                        writer.WriteUInt32(0xFFFFFFFF);
                    }
                    // Empty byte
                    for (int j = 0; j < 4; j++)
                    {
                        writer.WriteUInt32(0x00);
                    }

                    textOffset += Encoding.UTF8.GetByteCount(Entry[i].Text) + 1;
                }

                writer.Write(new byte[] { 0x1C, 0x07D, 0xA9, 0xF5 });

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
