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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Attributes
        {
            private uint FireUInt;
            private uint IceUInt;
            private uint EarthUint;
            private uint LigthningUInt;
            private uint WaterUInt;
            private uint WindUInt;

            private float ConvertToFloat(uint number)
            {
                if (number == 0x01)
                {
                    return 1.0f;
                } else
                {
                    return System.BitConverter.ToSingle(System.BitConverter.GetBytes(number), 0);
                }
            }


            private uint ConvertToUInt(float number)
            {
                if (number == 1.0)
                {
                    return 0x01;
                }
                else
                {
                    return System.BitConverter.ToUInt32(System.BitConverter.GetBytes(number), 0);
                }
            }

            public float[] GetAttributes()
            {
                float[] attributes = new float[6];

                attributes[0] = ConvertToFloat(FireUInt);
                attributes[1] = ConvertToFloat(IceUInt);
                attributes[2] = ConvertToFloat(EarthUint);
                attributes[3] = ConvertToFloat(LigthningUInt);
                attributes[4] = ConvertToFloat(WaterUInt);
                attributes[5] = ConvertToFloat(WindUInt);

                return attributes;
            }

            public void SetAttributes(float[] attributes)
            {
                FireUInt = ConvertToUInt(attributes[0]);
                IceUInt = ConvertToUInt(attributes[1]);
                EarthUint = ConvertToUInt(attributes[2]);
                LigthningUInt = ConvertToUInt(attributes[3]);
                WaterUInt = ConvertToUInt(attributes[4]);
                WindUInt = ConvertToUInt(attributes[5]);
            }
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
