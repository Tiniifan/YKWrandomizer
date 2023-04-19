using System.Collections.Generic;
using System.Runtime.InteropServices;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1
{
    public static class YW1Support
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Charabase
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
            public byte[] EmptyBlock1;
            public uint BaseID;
            public GameSupport.Model Model;
            public uint NameID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] EmptyBlock2;
            public uint Description;
            public int Rank;
            public bool IsRare;
            public bool IsLegendary;
            public GameSupport.Medal Medal;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] EmptyBlock3;

            public void ReplaceWith(Yokai yokai)
            {
                Model.ModelFromText(yokai.ModelName);
                Rank = yokai.Rank;
                IsRare = yokai.Statut.IsRare;
                IsLegendary = yokai.Statut.IsLegendary;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Charaparam
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] EmptyBlock1;
            public uint ParamID;
            public uint BaseID;
            public int Tribe;
            public GameSupport.Stat BaseStat;
            public uint Unk1;
            public uint AttackID;
            public uint Unk2;
            public uint TechniqueID;
            public uint Unk3;
            public uint InspiritID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] EmptyBlock3;
            public GameSupport.Attributes AttributesDamage;
            public uint SoultimateID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock4;
            public uint ScoutableID;
            public uint SkillID;
            public int Money;
            public int Experience;
            public GameSupport.Drop Drop1;
            public GameSupport.Drop Drop2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1C)]
            public byte[] EmptyBlock5;
            public int ExperienceCurve;
            public GameSupport.Quotes Quotes;
            public uint Unk5;
            public int EvolveOffset;
            public int MedaliumOffset;
            public uint Unk6;

            public void ReplaceWith(Yokai yokai)
            {
                Tribe = yokai.Tribe;
                BaseStat = new GameSupport.Stat { HP = yokai.MinStat[0], Strength = yokai.MinStat[1], Spirit = yokai.MinStat[2], Defense = yokai.MinStat[3], Speed = yokai.MinStat[4] };
                AttackID = yokai.AttackID;
                TechniqueID = yokai.TechniqueID;
                InspiritID = yokai.InspiritID;
                AttributesDamage = new GameSupport.Attributes { Fire = yokai.AttributeDamage[0], Ice = yokai.AttributeDamage[1], Earth = yokai.AttributeDamage[2], Ligthning = yokai.AttributeDamage[3], Water = yokai.AttributeDamage[4], Wind = yokai.AttributeDamage[4] };
                SoultimateID = yokai.SoultimateID;
                SkillID = yokai.SkillID;
                Money = yokai.Money;
                Experience = yokai.Experience;
                Drop1 = new GameSupport.Drop { ID = yokai.DropID[0], Rate = yokai.DropRate[0] };
                Drop2 = new GameSupport.Drop { ID = yokai.DropID[1], Rate = yokai.DropRate[1] };
                ExperienceCurve = yokai.ExperienceCurve;
                EvolveOffset = yokai.EvolveOffset;
                MedaliumOffset = yokai.MedaliumOffset;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Evolution
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock1;
            public int Level;
            public uint EvolveToID;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Fusion
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock1;
            public bool BaseIsYokai;
            public uint BaseID;
            public bool MaterialIsYokai;
            public uint MaterialID;
            public bool EvolveToIsYokai;
            public uint EvolveToID;
            public uint FusionID;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LegendSeal
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
            public byte[] EmptyBlock1;
            public uint LegendaryParamID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public GameSupport.YokaiSeal[] Seals;
            public int SealCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Encounter
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock1;
            public uint ParamID;
            public int Level;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] EmptyBlock2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WorldEncounter
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock1;
            public uint ParamID;
            public int Level;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] EmptyBlock2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CapsuleItem
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] EmptyBlock1;
            public uint ID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] EmptyBlock2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShopConfig
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
            public byte[] EmptyBlock1;
            public uint ItemID;
            public int Price;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] EmptyBlock2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TreasureBoxConfig
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock1;
            public uint ItemID;
            public int Quantity;
            public int MaximumQuantity;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock2;
        }

        public static Dictionary<string, string> AvailableLanguages = new Dictionary<string, string>()
        {
            { "English (GB)", "engb"},
            { "English (US)", "en"},
            { "Deutsch", "de"},
            { "Español", "es"},
            { "Français", "fr"},
            { "Italiano", "it"},
        };
    }
}