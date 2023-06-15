using System.Collections.Generic;
using System.Runtime.InteropServices;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW2
{
    public static class YW2Support
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
            public GameSupport.Medal Medal;
            public uint Unk1;
            public int Rank;
            public bool IsRare;
            public bool IsLegendary;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1C)]
            public byte[] EmptyBlock3;
            public int Tribe;
            public bool IsClassic;
            public uint Unk2;

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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
            public byte[] EmptyBlock1;
            public uint ParamID;
            public uint BaseID;
            public GameSupport.Stat MinStat;
            public GameSupport.Stat MaxStat;
            public int FavoriteDonut;
            public uint AttackID;
            public uint Unk2;
            public uint TechniqueID;
            public uint Unk3;
            public uint InspiritID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
            public byte[] EmptyBlock2;
            public GameSupport.Attributes AttributesDamage;
            public uint SoultimateID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock3;
            public uint ScoutableID;
            public uint SkillID;
            public int Money;
            public int Experience;
            public uint Unk4;
            public GameSupport.Drop Drop1;
            public GameSupport.Drop Drop2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock4;
            public int ExperienceCurve;
            public GameSupport.Quotes Quotes;
            public uint Unk5;
            public int EvolveOffset;
            public int MedaliumOffset;
            public uint ShowInMedallium;
            public uint Unk7;
            public uint Unk8;

            public void ReplaceWith(Yokai yokai)
            {
                MinStat = new GameSupport.Stat { HP = yokai.MinStat[0], Strength = yokai.MinStat[1], Spirit = yokai.MinStat[2], Defense = yokai.MinStat[3], Speed = yokai.MinStat[4] };
                MaxStat = new GameSupport.Stat { HP = yokai.MaxStat[0], Strength = yokai.MaxStat[1], Spirit = yokai.MaxStat[2], Defense = yokai.MaxStat[3], Speed = yokai.MaxStat[4] };
                AttackID = yokai.AttackID;
                TechniqueID = yokai.TechniqueID;
                InspiritID = yokai.InspiritID;

                GameSupport.Attributes attributes = new GameSupport.Attributes();
                attributes.SetAttributes(yokai.AttributeDamage);
                AttributesDamage = attributes;

                SoultimateID = yokai.SoultimateID;
                ScoutableID = yokai.ScoutableID;
                SkillID = yokai.SkillID;
                Money = yokai.Money;
                Experience = yokai.Experience;
                Drop1 = new GameSupport.Drop { ID = yokai.DropID[0], Rate = yokai.DropRate[0] };
                Drop2 = new GameSupport.Drop { ID = yokai.DropID[1], Rate = yokai.DropRate[1] };
                ExperienceCurve = yokai.ExperienceCurve;
                EvolveOffset = yokai.EvolveOffset;
                MedaliumOffset = yokai.MedaliumOffset;

                if (yokai.Statut.IsScoutable)
                {
                    ShowInMedallium = 1;
                }
            }
        }

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
            public bool BaseIsItem;
            public uint BaseID;
            public bool MaterialIsItem;
            public uint MaterialID;
            public bool EvolveToIsItem;
            public uint EvolveToID;
            public uint FusionID;
            public bool fusionIsItem;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LegendSeal
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0C)]
            public byte[] EmptyBlock1;
            public uint SealdID;
            public uint LegendaryParamID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public GameSupport.YokaiSeal[] Seals;
            public int SealCount;
            public int EmptyBlock2;
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SoulConfig
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24)]
            public byte[] EmptyBlock1;
            public uint YokaiID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
            public byte[] EmptyBlock2;
        }
    }
}

