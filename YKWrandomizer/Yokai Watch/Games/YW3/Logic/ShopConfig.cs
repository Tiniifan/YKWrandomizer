using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class ShopConfig : IShopConfig
    {
        public new int ShopConfigHash { get => base.ShopConfigHash; set => base.ShopConfigHash = value; }
        public new int ItemHash { get => base.ItemHash; set => base.ItemHash = value; }
        public new int Price { get => base.Price; set => base.Price = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public new int ShopValidConditionIndex { get => base.ShopValidConditionIndex; set => base.ShopValidConditionIndex = value; }
        public object Unk7 { get; set; }  
    }

    public class ShopValidCondition : IShopValidCondition
    {
        public new int Price { get => base.Price; set => base.Price = value; }
        public new object Condition { get => base.Condition; set => base.Condition = value; }
    }
}
