using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YWB.Logic
{
    public class CombineConfig : ICombineConfig
    {
        public new int BaseHash { get => base.BaseHash; set => base.BaseHash = value; }
        public new bool BaseIsItem { get => base.BaseIsItem; set => base.BaseIsItem = value; }
        public new int MaterialHash { get => base.MaterialHash; set => base.MaterialHash = value; }
        public new bool MaterialIsItem { get => base.MaterialIsItem; set => base.MaterialIsItem = value; }
        public new int EvolveToHash { get => base.EvolveToHash; set => base.EvolveToHash = value; }
        public new bool EvolveToIsItem { get => base.EvolveToIsItem; set => base.EvolveToIsItem = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Unk7 { get; set; }
        public int Unk8 { get; set; }
        public int Unk9 { get; set; }
        public int Unk10 { get; set; }
        public new int OniOrbCost { get => base.OniOrbCost; set => base.OniOrbCost = value; }
        public new int CombineConfigHash { get => base.CombineConfigHash; set => base.CombineConfigHash = value; }
        public int Unk11 { get; set; }
        public int Unk12 { get; set; }
        public int Unk13 { get; set; }
    }
}
