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
        public new int ItemNumber { get => base.ItemNumber; set => base.ItemNumber = value; }
    }
}
