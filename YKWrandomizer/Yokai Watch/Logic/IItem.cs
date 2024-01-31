namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class IItem
    {
        public int ItemHash { get; set; }
        public int NameHash { get; set; }
        public int ItemNumber { get; set; }
        public int MaxQuantity { get; set; }
        public bool CanBeBuy { get; set; }
        public bool CanBeSell { get; set; }
        public int SellPrize { get; set; }
        public int ItemPosX { get; set; }
        public int ItemPosY { get; set; }
        public int DescriptionHash { get; set; }
    }

    public class IConsumable : IItem
    {
        public int Effect1Hash { get; set; }
        public int Effect2Hash { get; set; }
    }
}
