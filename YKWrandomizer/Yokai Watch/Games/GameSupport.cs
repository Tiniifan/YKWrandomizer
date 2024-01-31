using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Binary;

namespace YKWrandomizer.Yokai_Watch.Games
{
    public static class GameSupport
    {
        public static Dictionary<int, char> PrefixLetter = new Dictionary<int, char>() 
        {
            {0, 'c'},
            {2, '?'},
            {3, '?'},
            {4, '?'},
            {5, 'x'},
            {6, 'y'},
            {7, 'z'},
            {8, '?'},
            {12, '?'},
            {17, '?'},
        };

        static string FormatVariant(int x)
        {
            if (x > -1 && x < 10)
            {
                return $"0{x}0";
            }
            else if (x > 9 && x < 100)
            {
                return $"{x}0";
            }
            else if (x > 99 && x < 1000)
            {
                char[] charArray = x.ToString().ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
            else
            {
                throw new FormatException("Format non valide");
            }
        }

        static int ParseVariant(string str)
        {
            if (str.Length == 4 && str[0] == '0' && str[3] == '0')
            {
                return int.Parse(str[1].ToString());
            }
            else if (str.Length == 3 && str[2] == '0')
            {
                return int.Parse(str.Substring(0, 2));
            }
            else if (str.Length == 3)
            {
                char[] charArray = str.ToCharArray();
                Array.Reverse(charArray);
                return int.Parse(new string(charArray));
            }
            else
            {
                throw new FormatException("Format non valide");
            }
        }

        public static string GetFileModelText(int prefix, int number, int variant)
        {
            return PrefixLetter[prefix] + number.ToString("D3") + FormatVariant(variant);
        }

        public static (int,int,int) GetFileModelValue(string text)
        {
            int prefixIndex = PrefixLetter.FirstOrDefault(x => x.Value == text[0]).Key;
            int number = int.Parse(text.Substring(1, 3));
            int variant = ParseVariant(text.Substring(4, 3));

            number = BitConverter.ToInt32(BitConverter.GetBytes(number), 0);
            variant = BitConverter.ToInt32(BitConverter.GetBytes(variant), 0);

            return (prefixIndex, number, variant);
        }

        public static void SaveTextFile(GameFile fileName, T2bþ fileData)
        {
            VirtualDirectory directory = fileName.File.Directory.GetFolderFromFullPath(Path.GetDirectoryName(fileName.Path).Replace("\\", "/"));
            directory.Files[Path.GetFileName(fileName.Path)].ByteContent = fileData.Save(false);
        }

        public static T GetLogic<T>() where T : class, new()
        {
            return new T();
        }
    }
}

