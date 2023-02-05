using System;
using System.Drawing;
using YKWrandomizer.Tool;

namespace YKWrandomizer.Yokai_Watch.Res
{
    public interface ICharabase
    {
        long Length { get; set; }
        long Offset { get; set; }
        UInt32 BaseID { get; set; }
        string ModelName { get; set; }
        UInt32 NameID { get; set; }
        Point Medal { get; set; }
        int Rank { get; set; }
        bool IsRare { get; set; }
        bool IsLegendary { get; set; }
        int Tribe { get; set; }

        void Read(DataReader reader);

        void Write(DataWriter writer);
    }
}
