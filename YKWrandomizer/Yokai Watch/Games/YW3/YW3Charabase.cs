using System;
using System.Drawing;
using YKWrandomizer.Tool;
using YKWrandomizer.Yokai_Watch.Res;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public class YW3Charabase : ICharabase
    {
        // Inheritance
        public long Length { get; set; }
        public long Offset { get; set; }
        public UInt32 BaseID { get; set; }
        public string ModelName { get; set; }
        public UInt32 NameID { get; set; }
        public UInt32 DescriptionID { get; set; }
        public Point Medal { get; set; }
        public int Rank { get; set; }
        public bool IsRare { get; set; }
        public bool IsLegendary { get; set; }
        public int Tribe { get; set; }

        // Extend
        private char[] Prefix = new char[] { 'x', 'y' };
        public bool IsClassic { get; set; }
        public bool IsMerican { get; set; }
        public bool IsDeva { get; set; }
        public bool IsMystery { get; set; }
        public bool IsTreasure { get; set; }

        public void Read(DataReader reader)
        {
            Length = reader.Length;

            reader.Skip(0x010);

            BaseID = reader.ReadUInt32();
            ModelName += Prefix[reader.ReadInt32() - 5];
            for (int i = 0; i < 2; i++)
            {
                int modelName = reader.ReadInt32();

                if (modelName < 10)
                {
                    ModelName += "0" + modelName + "0";
                }
                else
                {
                    ModelName += modelName.ToString().PadRight(3, '0');
                }
            }

            reader.Skip(0x04);
            NameID = reader.ReadUInt32();

            // Unknow byte
            reader.Skip(0x14);

            DescriptionID = reader.ReadUInt32();
            Medal = new Point(reader.ReadInt32(), reader.ReadInt32());

            // Unknow byte
            reader.Skip(0x04);

            Rank = reader.ReadInt32();
            IsRare = Convert.ToBoolean(reader.ReadInt32());
            IsLegendary = Convert.ToBoolean(reader.ReadInt32());

            // Unknow byte
            reader.Skip(0x18);
            Tribe = reader.ReadInt32();
            IsClassic = Convert.ToBoolean(reader.ReadInt32());
            IsMerican = Convert.ToBoolean(reader.ReadInt32());

            // Unknow byte
            reader.Skip(0x08);
            IsDeva = Convert.ToBoolean(reader.ReadInt32());
            IsMystery = Convert.ToBoolean(reader.ReadInt32());
            IsTreasure = Convert.ToBoolean(reader.ReadInt32());
        }

        public void Write(DataWriter writer)
        {
            writer.Seek((uint)Offset);
        }
    }
}
