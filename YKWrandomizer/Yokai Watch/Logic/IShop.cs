namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class IShopConfig
    {
        public int ShopConfigHash { get; set; }
        public int ItemHash { get; set; }
        public int Price { get; set; }
        public int ShopValidConditionIndex { get; set; }
    }

    public class IShopValidCondition
    {
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
