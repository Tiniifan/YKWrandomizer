using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YWB.Logic
{
    public class NPCCharabase : ICharabase
    {
        public new int BaseHash { get => base.BaseHash; set => base.BaseHash = value; }
        public new int FileNamePrefix { get => base.FileNamePrefix; set => base.FileNamePrefix = value; }
        public new int FileNameNumber { get => base.FileNameNumber; set => base.FileNameNumber = value; }
        public new int FileNameVariant { get => base.FileNameVariant; set => base.FileNameVariant = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public NPCCharabase()
        {
            IsYokai = false;
        }
    }
    public class YokaiCharabase : ICharabase
    {
        public new int BaseHash { get => base.BaseHash; set => base.BaseHash = value; }
        public new int FileNamePrefix { get => base.FileNamePrefix; set => base.FileNamePrefix = value; }
        public new int FileNameNumber { get => base.FileNameNumber; set => base.FileNameNumber = value; }
        public new int FileNameVariant { get => base.FileNameVariant; set => base.FileNameVariant = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public new int MedalPosX { get => base.MedalPosX; set => base.MedalPosX = value; }
        public new int MedalPosY { get => base.MedalPosY; set => base.MedalPosY = value; }
        public int Unk6 { get; set; }
        public new int Rank { get => base.Rank; set => base.Rank = value; }
        public new bool IsRare { get => base.IsRare; set => base.IsRare = value; }
        public new bool IsLegend { get => base.IsLegend; set => base.IsLegend = value; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }
        public new int Tribe { get => base.Tribe; set => base.Tribe = value; }
        public int Unk9 { get => base.Role; set => base.Role = value; }
        public new int Role { get => base.Role; set => base.Role = value; }
        public int Unk10 { get; set; }

        public YokaiCharabase()
        {
            IsYokai = true;
        }
    }
}
