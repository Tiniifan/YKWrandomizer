using System;
using System.Drawing;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Yokai_Watch.Games.YW2
{
    public class YW2Charabase
    {
        public long Length;

        public long Offset;

        public UInt32 BaseID;

        public string ModelName;

        public UInt32 NameID;

        public Point Medal;

        public int Rank;

        public bool IsRare;

        public bool IsLegendary;

        public int Tribe;

        private char[] Prefix = new char[] { 'x', 'y' };

        public void Read(DataReader reader)
        {
            Length = reader.Length;

            reader.Skip(0x0C);

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
            NameID = reader.ReadUInt32();

            // Unknow byte
            reader.Skip(0x18);

            Medal = new Point(reader.ReadInt32(), reader.ReadInt32());

            // Unknow byte
            reader.Skip(0x04);

            Rank = reader.ReadInt32();
            IsRare = Convert.ToBoolean(reader.ReadInt32());
            IsLegendary = Convert.ToBoolean(reader.ReadInt32());

            // Unknow byte
            reader.Skip(0x1C);
            Tribe = reader.ReadInt32();
        }

        public void Write(DataWriter writer)
        {
            writer.Seek((uint)Offset);
            writer.Skip(0x0C);

            writer.WriteUInt32(BaseID);
            writer.WriteInt32(Convert.ToInt32(ModelName[0] == 'y') + 5);
            writer.WriteInt32(Convert.ToInt32(ModelName.Substring(1, 3)));
            writer.WriteInt32(Convert.ToInt32(ModelName.Substring(4, 3)) / 10);
            writer.WriteUInt32(NameID);

            // Unknow byte
            writer.Skip(0x18);

            writer.WriteInt32(Medal.X);
            writer.WriteInt32(Medal.Y);

            // Unknow byte
            writer.Skip(0x04);

            writer.WriteInt32(Rank);
            writer.WriteInt32(Convert.ToInt32(IsRare));
            writer.WriteInt32(Convert.ToInt32(IsLegendary));

            // Unknow byte
            writer.Skip(0x1C);
            writer.WriteInt32(Tribe);
        }
    }
}
