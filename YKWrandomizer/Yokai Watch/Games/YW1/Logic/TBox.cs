using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class ItableDataMore : IItableDataMore
    {
        public new int ItemHash { get => base.ItemHash; set => base.ItemHash = value; }
        public new int Quantity { get => base.Quantity; set => base.Quantity = value; }
        public new int Percentage { get => base.Percentage; set => base.Percentage = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
    }
}
