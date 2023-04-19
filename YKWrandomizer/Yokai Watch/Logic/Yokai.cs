using System;
using System.Drawing;

namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class Yokai
    {
        public string Name;

        public string ModelName;

        public int Rank;

        public int Tribe;

        public int WaitTime;

        public int[] MinStat = new int[5];

        public int[] MaxStat = new int[5];

        public float[] AttributeDamage = new float[6];

        public byte Strongest;

        public byte Weakness;

        public UInt32 AttackID;

        public UInt32 TechniqueID;

        public UInt32 InspiritID;

        public UInt32 SoultimateID;

        public UInt32 FoodID;

        public UInt32 SkillID;

        public int Money;

        public int Experience;

        public UInt32[] DropID;

        public int[] DropRate;

        public int ExperienceCurve;

        public int EvolveOffset;

        public int MedaliumOffset;

        public Point Medal;

        public Statut Statut;

        public UInt32 ScoutableID;

        public UInt32 BaseID;

        public UInt32 ParamID;

        public Yokai()
        {

        }

        public Yokai(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Statut
    {
        public bool IsRare;

        public bool IsLegendary;

        public bool IsClassic;

        public bool IsMerican;

        public bool IsDeva;

        public bool IsMystery;

        public bool IsTreasure;

        public bool IsBoss;

        public bool IsStatic;

        public bool IsScoutable;

        public Statut()
        {

        }
    }
}
