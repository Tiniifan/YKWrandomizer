using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
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
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public int Unk6 { get; set; }
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
        public new bool IsPionner { get => base.IsPionner; set => base.IsPionner = value; }
        public new bool IsCommandant { get => base.IsPionner; set => base.IsPionner = value; }
        public new int FavoriteFoodHash { get => base.FavoriteFoodHash; set => base.FavoriteFoodHash = value; }
        public new int HatedFoodHash { get => base.HatedFoodHash; set => base.HatedFoodHash = value; }
        public int Unk11 { get; set; }
        public int Unk12 { get; set; }
        public new int Tribe { get => base.Tribe; set => base.Tribe = value; }
        public new bool IsClassic { get => base.IsClassic; set => base.IsClassic = value; }
        public new bool IsMerican { get => base.IsMerican; set => base.IsMerican = value; }
        public new int Role { get => base.Role; set => base.Role = value; }
        public int Unk13 { get; set; }
        public new bool IsDeva { get => base.IsDeva; set => base.IsDeva = value; }
        public new bool IsLegendaryMystery { get => base.IsLegendaryMystery; set => base.IsLegendaryMystery = value; }
        public new bool IsTreasure { get => base.IsTreasure; set => base.IsTreasure = value; }

        public YokaiCharabase()
        {
            IsYokai = true;
        }
    }
}
