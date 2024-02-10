using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW2.Logic
{
    public class CapsuleConfig : ICapsuleConfig
    {
        public new int CapsuleHash { get => base.CapsuleHash; set => base.CapsuleHash = value; }
        public int Unk1 { get; set; }
        public new int ItemOrYokaiHash { get => base.ItemOrYokaiHash; set => base.ItemOrYokaiHash = value; }
        public new int CapsuleTextHash { get => base.CapsuleTextHash; set => base.CapsuleTextHash = value; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }
    }
}
