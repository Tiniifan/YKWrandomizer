using System;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Yokai_Watch.Res
{
    public interface ICharaparam
    {
        long Length { get; set; }
        long Offset { get; set; }
        UInt32 ParamID { get; set; }
        UInt32 BaseID { get; set; }
        int[] MinStat { get; set; }
        UInt32 AttackID { get; set; }
        UInt32 TechniqueID { get; set; }
        UInt32 InspiritID { get; set; }
        UInt32 SoultimateID { get; set; }
        UInt32 SkillID { get; set; }
        UInt32 FoodID { get; set; }
        int Money { get; set; }
        int Experience { get; set; }
        UInt32[] DropID { get; set; }
        int[] DropRate { get; set; }
        int ExperienceCurve { get; set; }
        UInt32[] QuoteID { get; set; }
        int EvolveOffset { get; set; }
        int MedaliumOffset { get; set; }

        void Read(DataReader reader);

        void Write(DataWriter writer);
    }
}
