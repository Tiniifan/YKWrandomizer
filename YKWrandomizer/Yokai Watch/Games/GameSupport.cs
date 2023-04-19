using System;
using System.Runtime.InteropServices;

namespace YKWrandomizer.Yokai_Watch.Games
{
    public static class GameSupport
    {
        private static char[] PrefixLetter = new char[] { 'x', 'y' };

        public struct Medal
        {
            public int X;
            public int Y;
        }

        public struct Model
        {
            public int Prefix;
            public int Number;
            public int Variant;

            public void ModelFromText(string text)
            {
                int prefixIndex = Array.IndexOf(PrefixLetter, text[0]);
                int number = int.Parse(text.Substring(1, 3));
                int variant = int.Parse(text.Substring(4, 3));

                Prefix = prefixIndex + 5;
                Number = System.BitConverter.ToInt32(System.BitConverter.GetBytes(number), 0);
                Variant = System.BitConverter.ToInt32(System.BitConverter.GetBytes(variant), 0);
            }

            public string GetText()
            {
                return PrefixLetter[Prefix - 5] + Number.ToString("D3") + Variant.ToString("D3");
            }
        }

        public struct Stat
        {
            public int HP;
            public int Strength;
            public int Spirit;
            public int Defense;
            public int Speed;
        }

        public struct EquipmentStat
        {
            public int Strength;
            public int Spirit;
            public int Defense;
            public int Speed;
        }

        public struct Attributes
        {
            public float Fire;
            public float Ice;
            public float Earth;
            public float Ligthning;
            public float Water;
            public float Wind;
        }

        public struct Drop
        {
            public uint ID;
            public int Rate;
        }

        public struct Quotes
        {
            public uint ID1;
            public uint ID2;
            public uint ID3;
            public uint ID4;
        }

        public struct YokaiSeal
        {
            public uint ParamID;
            public bool Spoil;
        }
    }
}
