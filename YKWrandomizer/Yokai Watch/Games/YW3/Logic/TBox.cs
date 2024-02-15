using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class ItableDataMore : IItableDataMore
    {
        public int Unk1 { get; set; }
        public new int ItemHash { get => base.ItemHash; set => base.ItemHash = value; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public new int Percentage { get => base.Percentage; set => base.Percentage = value; }
        public object Unk4 { get; set; }
    }
}
