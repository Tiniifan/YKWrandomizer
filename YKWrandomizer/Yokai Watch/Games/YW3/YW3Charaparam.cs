using System;
using YKWrandomizer.Tool;
using YKWrandomizer.Yokai_Watch.Res;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public class YW3Charaparam : ICharaparam
    {
        // Inheritance
        public long Length { get; set; }
        public long Offset { get; set; }
        public UInt32 ParamID { get; set; }
        public UInt32 BaseID { get; set; }
        public int[] MinStat { get; set; }
        public UInt32 AttackID { get; set; }
        public UInt32 TechniqueID { get; set; }
        public UInt32 InspiritID { get; set; }
        public UInt32 SoultimateID { get; set; }
        public UInt32 FoodID { get; set; }
        public UInt32 SkillID { get; set; }
        public int Money { get; set; }
        public int Experience { get; set; }
        public UInt32[] DropID { get; set; }
        public int[] DropRate { get; set; }
        public int ExperienceCurve { get; set; }
        public UInt32[] QuoteID { get; set; }
        public int EvolveOffset { get; set; }
        public int MedaliumOffset { get; set; }

        // Extend
        public int[] MaxStat;
        public byte Strongest;
        public byte Weakness;
        public int WaitTime;

        public YW3Charaparam()
        {

        }

        public void Read(DataReader reader)
        {
            Offset = reader.BaseStream.Position;
            Length = reader.Length;

            reader.Skip(0x10);
            ParamID = reader.ReadUInt32();
            BaseID = reader.ReadUInt32();
            reader.Skip(0x0C);

            MinStat = new Int32[5];
            for (int i = 0; i < 5; i++)
            {
                MinStat[i] = reader.ReadInt32();
            }

            MaxStat = new Int32[5];
            for (int i = 0; i < 5; i++)
            {
                MaxStat[i] = reader.ReadInt32();
            }

            ExperienceCurve = (byte)reader.ReadInt32();
            Strongest = (byte)reader.ReadInt32();
            Weakness = (byte)reader.ReadInt32();
            reader.Skip(0x04);
            AttackID = reader.ReadUInt32();
            reader.Skip(0x04);
            TechniqueID = reader.ReadUInt32();
            reader.Skip(0x04);
            InspiritID = reader.ReadUInt32();
            reader.Skip(0x04);
            reader.Skip(0x08);
            SoultimateID = reader.ReadUInt32();
            SkillID = reader.ReadUInt32();

            reader.Skip(0x24);
            EvolveOffset = reader.ReadInt32();
            reader.Skip(0x04);
            WaitTime = reader.ReadInt32();
        }

        public void Write(DataWriter writer)
        {
            writer.Seek((uint)Offset);
        }
    }
}
