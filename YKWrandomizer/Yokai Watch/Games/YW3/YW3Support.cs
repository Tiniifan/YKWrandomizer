using System.Collections.Generic;
using System.Runtime.InteropServices;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public static class YW3Support
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Charabase
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] EmptyBlock1;
            public uint BaseID;
            public GameSupport.Model Model;
            public uint NameID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] EmptyBlock2;
            public uint Description;
            public GameSupport.Medal Medal;
            public uint Unk1;
            public int Rank;
            public bool IsRare;
            public bool IsLegendary;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
            public byte[] EmptyBlock3;
            public int Tribe;
            public bool IsClassic;
            public bool IsMerican;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock4;
            public bool IsDeva;
            public bool IsMystery;
            public bool IsTreasure;

            public void ReplaceWith(Yokai yokai)
            {
                Model.ModelFromText(yokai.ModelName);
                Rank = yokai.Rank;
                IsRare = yokai.Statut.IsRare;
                IsLegendary = yokai.Statut.IsLegendary;
                Tribe = yokai.Tribe;
                IsClassic = yokai.Statut.IsClassic;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Charaparam
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] EmptyBlock1;
            public uint ParamID;
            public uint BaseID;
            public bool ShowInMedallium;
            public int MedalliumOffset;
            public uint Unk1;
            public GameSupport.Stat MinStat;
            public GameSupport.Stat MaxStat;
            public int ExperienceCurve;
            public int Strongest;
            public int Weakness;
            public uint Unk2;
            public uint AttackID;
            public uint Unk3;
            public uint TechniqueID;
            public uint Unk4;
            public uint InspiritID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
            public byte[] EmptyBlock2;
            public uint SoultimateID;
            public uint SkillID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1C)]
            public byte[] EmptyBlock3;
            public uint ScoutableID;
            public uint Unk5;
            public int EvolveOffset;
            public uint Unk6;
            public int WaitTime;

            public void ReplaceWith(Yokai yokai)
            {
                MinStat = new GameSupport.Stat { HP = yokai.MinStat[0], Strength = yokai.MinStat[1], Spirit = yokai.MinStat[2], Defense = yokai.MinStat[3], Speed = yokai.MinStat[4] };
                MaxStat = new GameSupport.Stat { HP = yokai.MaxStat[0], Strength = yokai.MaxStat[1], Spirit = yokai.MaxStat[2], Defense = yokai.MaxStat[3], Speed = yokai.MaxStat[4] };
                AttackID = yokai.AttackID;
                TechniqueID = yokai.TechniqueID;
                InspiritID = yokai.InspiritID;
                //AttributesDamage = new GameSupport.Attributes { Fire = yokai.AttributeDamage[0], Ice = yokai.AttributeDamage[1], Earth = yokai.AttributeDamage[2], Ligthning = yokai.AttributeDamage[3], Water = yokai.AttributeDamage[4], Wind = yokai.AttributeDamage[4] };
                SoultimateID = yokai.SoultimateID;
                SkillID = yokai.SkillID;
                //Money = yokai.Money;
                //Experience = yokai.Experience;
                //Drop1 = new GameSupport.Drop { ID = yokai.DropID[0], Rate = yokai.DropRate[0] };
                //Drop2 = new GameSupport.Drop { ID = yokai.DropID[1], Rate = yokai.DropRate[1] };
                ExperienceCurve = yokai.ExperienceCurve;
                EvolveOffset = yokai.EvolveOffset;
                //MedaliumOffset = yokai.MedaliumOffset;
            }
        }

        public static Dictionary<string, string> AvailableLanguages = new Dictionary<string, string>()
        {
            { "English", "en"},
            { "Deutsch", "de"},
            { "Español", "es"},
            { "Français", "fr"},
            { "Italiano", "it"},
        };
    }
}

