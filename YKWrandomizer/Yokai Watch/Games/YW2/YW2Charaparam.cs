using System;
using YKWrandomizer.Tool;
using YKWrandomizer.Yokai_Watch.Res;

namespace YKWrandomizer.Yokai_Watch.Games.YW2
{
    public class YW2Charaparam : ICharaparam
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
        public int FavoriteDonut;
        public float[] AttributeDamage;

        public YW2Charaparam()
        {

        }

        public void Read(DataReader reader)
        {
            Length = reader.Length;

            reader.Skip(0x14);

            ParamID = reader.ReadUInt32();
            BaseID = reader.ReadUInt32();

            MinStat = new int[5];
            for (int i = 0; i < 5; i++)
            {
                MinStat[i] = reader.ReadInt32();
            }

            MaxStat = new int[5];
            for (int i = 0; i < 5; i++)
            {
                MaxStat[i] = reader.ReadInt32();
            }

            FavoriteDonut = reader.ReadInt32();

            AttackID = reader.ReadUInt32();
            reader.Skip(0x04);

            TechniqueID = reader.ReadUInt32();
            reader.Skip(0x04);

            InspiritID = reader.ReadUInt32();
            reader.Skip(0x04);

            // Unknow byte
            reader.Skip(0x0C);

            AttributeDamage = new float[6];
            for (int i = 0; i < 6; i++)
            {
                AttributeDamage[i] = reader.ReadSingle();
            }

            SoultimateID = reader.ReadUInt32();
            reader.Skip(0x04);

            // Unknow byte
            reader.Skip(0x04);

            FoodID = reader.ReadUInt32();
            SkillID = reader.ReadUInt32();
            Money = reader.ReadInt32();
            Experience = reader.ReadInt32();
            reader.Skip(0x04);

            DropID = new UInt32[2];
            DropRate = new int[2];
            for (int i = 0; i < 2; i++)
            {
                DropID[i] = reader.ReadUInt32();
                DropRate[i] = reader.ReadInt32();
            }

            // Unknow byte
            reader.Skip(0x08);

            ExperienceCurve = reader.ReadInt32();

            QuoteID = new UInt32[4];
            for (int i = 0; i < 4; i++)
            {
                QuoteID[i] = reader.ReadUInt32();
            }

            // Unknow byte
            reader.Skip(0x04);

            EvolveOffset = reader.ReadInt32();
            MedaliumOffset = reader.ReadInt32();
        }

        public void Write(DataWriter writer)
        {
            writer.Seek((uint)Offset);
            writer.Skip(0x14);

            writer.WriteUInt32(ParamID);
            writer.WriteUInt32(BaseID);

            for (int i = 0; i < 5; i++)
            {
                writer.WriteInt32(MinStat[i]);
            }

            for (int i = 0; i < 5; i++)
            {
                writer.WriteInt32(MaxStat[i]);
            }

            writer.WriteInt32(FavoriteDonut);

            writer.WriteUInt32(AttackID);
            writer.Skip(0x04);

            writer.WriteUInt32(TechniqueID);
            writer.Skip(0x04);

            writer.WriteUInt32(InspiritID);
            writer.Skip(0x04);

            // Unknow byte
            writer.Skip(0x0C);

            for (int i = 0; i < 6; i++)
            {
                if (AttributeDamage[i] == 0x00)
                {
                    writer.WriteInt32(0x01);
                } else
                {
                    writer.Write(AttributeDamage[i]);
                }
                
            }

            writer.WriteUInt32(SoultimateID);
            writer.Skip(0x04);

            // Unknow byte
            writer.Skip(0x04);

            writer.WriteUInt32(FoodID);
            writer.WriteUInt32(SkillID);
            writer.WriteInt32(Money);
            writer.WriteInt32(Experience);
            writer.Skip(0x04);

            for (int i = 0; i < 2; i++)
            {
                writer.WriteUInt32(DropID[i]);
                writer.WriteInt32(DropRate[i]);
            }

            // Unknow byte
            writer.Skip(0x08);

            writer.WriteInt32(ExperienceCurve);

            for (int i = 0; i < 4; i++)
            {
                writer.WriteUInt32(QuoteID[i]);
            }

            // Unknow byte
            writer.Skip(0x04);

            writer.WriteInt32(EvolveOffset);
            writer.WriteInt32(MedaliumOffset);
            writer.WriteInt32(0x01);
        }
    }
}
