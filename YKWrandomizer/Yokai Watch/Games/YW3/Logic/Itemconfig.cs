using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class Item : IItem
    {
        public new int ItemHash { get => base.ItemHash; set => base.ItemHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public new int MaxQuantity { get => base.MaxQuantity; set => base.MaxQuantity = value; }
        public new bool CanBeBuy { get => base.CanBeBuy; set => base.CanBeBuy = value; }
        public new bool CanBeSell { get => base.CanBeSell; set => base.CanBeSell = value; }
        public int Unk5 { get; set; }
        public int SellPrice { get; set; }
        public new int PurchasePrice { get => base.PurchasePrice; set => base.PurchasePrice = value; }
        public new int ItemPosX { get => base.ItemPosX; set => base.ItemPosX = value; }
        public new int ItemPosY { get => base.ItemPosY; set => base.ItemPosY = value; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
    }
}
