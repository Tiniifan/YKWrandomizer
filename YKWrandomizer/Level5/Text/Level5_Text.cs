using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Level5.Text
{
    class Level5_Text
    {
        private LongText LongText;

        private Noun Noun;

        public Level5_Text(byte[] data)
        {
            LongText = new LongText();
            Noun = new Noun();
            Read(data);
        }

        private void Read(byte[] data)
        {
            DataReader reader = new DataReader(data);

            // Load Data Table
            int FileCount = reader.ReadInt32();
            int startText = reader.ReadInt32();
            int lengthText = reader.ReadInt32();
            int numberLines = reader.ReadInt32();

            // Create Reader Text
            DataReader readerText = new DataReader(reader.GetSection((uint)startText, lengthText));

            for (int i = 0; i < FileCount;)
            {
                UInt32 formatText = reader.ReadUInt32();

                if (formatText == 0x0EFB9738)
                {
                    // Text Format

                    reader.Skip(0x04);
                    int count = reader.ReadInt32();

                    // Load All data
                    for (int j = 0; j < count; j++)
                    {
                        reader.Skip(0x08);
                        UInt32 key = reader.ReadUInt32();

                        // Check if key not exist
                        if (LongText.Entry.All(x => x.Key != key))
                        {
                            reader.Skip(0x04);
                            int offsetText = reader.ReadInt32();
                            readerText.Seek((uint)offsetText);
                            LongText.Entry.Add(new Entry(key, readerText.TakeWhile(x => x != 0x00).ToArray()));
                            reader.Skip(0x04);
                        }
                        else
                        {
                            reader.Skip(0x0C);
                        }

                        i++;
                    }

                    // End Data
                    reader.Skip(0x08);
                    i += 2;
                }
                else if (formatText == 0x5EA2CA19)
                {
                    // Noun Format

                    reader.Skip(0x04);
                    int count = reader.ReadInt32();

                    // Load All data
                    for (int j = 0; j < count; j++)
                    {
                        reader.Skip(0x0C);
                        UInt32 key = reader.ReadUInt32();
                        Int32 variantKey = reader.ReadInt32();

                        // Check if key not exist
                        if (Noun.Entry.All(x => x.Key != key) || Noun.Entry.All(x => x.Key == key && x.VariantKey != variantKey))
                        {
                            reader.Skip(0x0C);
                            int offsetText = reader.ReadInt32();
                            if (offsetText != -1)
                            {
                                readerText.Seek((uint)offsetText);
                                byte[] test = readerText.TakeWhile(x => x != 0x00).ToArray();
                                Noun.Entry.Add(new Entry(key, variantKey, test));
                                reader.Skip(0x20);
                            }
                            else
                            {
                                reader.Skip(0x20);
                            }
                        }
                        else
                        {
                            reader.Skip(0x30);
                        }

                        i++;
                    }

                    // End Data
                    reader.Skip(0x08);
                    i += 2;
                }
            }
        }

        private byte[] GenerateFooter()
        {
            long outputBlock = 16;
            outputBlock += 24 * Convert.ToInt32(LongText.Entry.Count > 0) + 24 * Convert.ToInt32(Noun.Entry.Count > 0);
            outputBlock += 40 * Convert.ToInt32(LongText.Entry.Count > 0) + 40 * Convert.ToInt32(Noun.Entry.Count > 0);
            outputBlock += 10;
            long emptyBlock = 16 - outputBlock % 16;

            byte[] output = new byte[outputBlock + emptyBlock];
            DataWriter writer = new DataWriter(output);
            writer.Write(new byte[] { 0x90, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x50, 0x00, 0x00, 0x00 });

            if (LongText.Entry.Count > 0)
            {
                writer.Write(new byte[] { 0x38, 0x97, 0xFB, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x89, 0x7E, 0xF4, 0x49, 0x10, 0x00, 0x00, 0x00, 0xA7, 0x09, 0xDE, 0x6A, 0x1A, 0x00, 0x00, 0x00 });
            }

            if (Noun.Entry.Count > 0)
            {
                writer.Write(new byte[] { 0x19, 0xCA, 0xA2, 0x5E, 0x28, 0x00, 0x00, 0x00, 0x93, 0xEA, 0x40, 0x22, 0x38, 0x00, 0x00, 0x00, 0x1C, 0x7D, 0xA9, 0xF5, 0x42, 0x00, 0x00, 0x00 });
            }

            if (LongText.Entry.Count > 0)
            {
                writer.Write(new byte[] { 0x54, 0x45, 0x58, 0x54, 0x5F, 0x49, 0x4E, 0x46, 0x4F, 0x5F, 0x42, 0x45, 0x47, 0x49, 0x4E, 0x00, 0x54, 0x45, 0x58, 0x54, 0x5F, 0x49, 0x4E, 0x46, 0x4F, 0x00, 0x54, 0x45, 0x58, 0x54, 0x5F, 0x49, 0x4E, 0x46, 0x4F, 0x5F, 0x45, 0x4E, 0x44, 0x00 });
            }

            if (Noun.Entry.Count > 0)
            {
                writer.Write(new byte[] { 0x4E, 0x4F, 0x55, 0x4E, 0x5F, 0x49, 0x4E, 0x46, 0x4F, 0x5F, 0x42, 0x45, 0x47, 0x49, 0x4E, 0x00, 0x4E, 0x4F, 0x55, 0x4E, 0x5F, 0x49, 0x4E, 0x46, 0x4F, 0x00, 0x4E, 0x4F, 0x55, 0x4E, 0x5F, 0x49, 0x4E, 0x46, 0x4F, 0x5F, 0x45, 0x4E, 0x44, 0x00 });
            }

            writer.Write(new byte[] { 0x01, 0x74, 0x32, 0x62, 0xFE, 0x01, 0x01, 0x00, 0x01, 0x00 });

            for (int i = 0; i < emptyBlock; i++)
            {
                writer.WriteByte(0xFF);
            }

            return output;
        }

        public void Save()
        {
            // Long Text Block
            byte[] longTextOffset = LongText.OffsetToByteArray(0);
            byte[] longTextMessage = LongText.TextToByteArray();

            // Long Noun Block
            byte[] nounTextOffset = Noun.OffsetToByteArray(longTextMessage.Length);
            byte[] nounMessage = Noun.TextToByteArray();

            // Fouter Block
            byte[] footer = GenerateFooter();

            // Get output Size
            long outputLength = longTextOffset.Length + longTextMessage.Length + nounTextOffset.Length + nounMessage.Length + 16;
            long emptyBlock = 16 - outputLength % 16;
            outputLength += footer.Length;
            byte[] output = new byte[outputLength + emptyBlock];

            DataWriter writer = new DataWriter(output);

            // Header
            writer.WriteInt32(LongText.Entry.Count + Convert.ToInt32(LongText.Entry.Count > 0) * 2 + Noun.Entry.Count + Convert.ToInt32(Noun.Entry.Count > 0) * 2);
            writer.WriteInt32((int)(longTextOffset.Length + nounTextOffset.Length + 16));
            writer.WriteInt32(longTextMessage.Length + nounMessage.Length);
            writer.WriteInt32(LongText.Entry.Count + Noun.Entry.Count);

            // Offset Block
            writer.Write(longTextOffset);
            writer.Write(nounTextOffset);

            // Text Block
            writer.Write(longTextMessage);
            writer.Write(nounMessage);

            // Empty block
            for (int i = 0; i < emptyBlock; i++)
            {
                writer.WriteByte(0xFF);
            }

            // Footer
            writer.Write(footer);
            File.WriteAllBytes("./tamer.dat", output);
        }

        private List<UInt32> GetAllKey()
        {
            List<UInt32> key = new List<UInt32>();
            key.AddRange(LongText.Entry.Select(x => x.Key));
            key.AddRange(Noun.Entry.Select(x => x.Key));
            return key;
        }

        public void AddToText(string text)
        {
            var exclude = new HashSet<UInt32>(GetAllKey());
            var range = Enumerable.Range(1, int.MaxValue).Select(i => (uint)i).Where(i => !exclude.Contains(i));
            UInt32 randomKey = range.ElementAt(0);

            LongText.Entry.Add(new Entry(randomKey, text));
        }

        private void AddToNoun(string text, int varianceKey)
        {

        }

        public string GetNounText(UInt32 key, int variantKey)
        {
            if (key != 0x00)
            {
                return Noun.Entry.FirstOrDefault(x => x.Key == key && x.VariantKey == variantKey).Text;
            }
            else
            {
                return "  ";
            }
        }
    }
}
