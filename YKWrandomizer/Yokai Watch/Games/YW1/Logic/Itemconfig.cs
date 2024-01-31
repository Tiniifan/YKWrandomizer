using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class Item : IItem
    {
        public new int ItemHash { get => base.ItemHash; set => base.ItemHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public new int ItemNumber { get => base.ItemNumber; set => base.ItemNumber = value; }
    }
    public class Consumable : IConsumable
    {
        public new int ItemHash { get => base.ItemHash; set => base.ItemHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public new int ItemNumber { get => base.ItemNumber; set => base.ItemNumber = value; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }
        public new int MaxQuantity { get => base.MaxQuantity; set => base.MaxQuantity = value; }
        public new bool CanBeBuy { get => base.CanBeBuy; set => base.CanBeBuy = value; }
        public new bool CanBeSell { get => base.CanBeSell; set => base.CanBeSell = value; }
        public int Unk9 { get; set; }
        public new int SellPrize { get => base.SellPrize; set => base.SellPrize = value; }
        public int Unk10 { get; set; }
        public new int ItemPosX { get => base.ItemPosX; set => base.ItemPosX = value; }
        public new int ItemPosY { get => base.ItemPosY; set => base.ItemPosY = value; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public new int Effect1Hash { get => base.Effect1Hash; set => base.Effect1Hash = value; }
        public new int Effect2Hash { get => base.Effect2Hash; set => base.Effect2Hash = value; }

        public int[] UnkBlock1 = new int[0x18];
    }
}
