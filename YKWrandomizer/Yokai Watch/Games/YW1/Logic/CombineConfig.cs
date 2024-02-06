using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class CombineConfig : ICombineConfig
    {
        public new bool BaseIsItem { get => base.BaseIsItem; set => base.BaseIsItem = value; }
        public new int BaseHash { get => base.BaseHash; set => base.BaseHash = value; }
        public new bool MaterialIsItem { get => base.MaterialIsItem; set => base.MaterialIsItem = value; }
        public new int MaterialHash { get => base.MaterialHash; set => base.MaterialHash = value; }
        public new bool EvolveToIsItem { get => base.EvolveToIsItem; set => base.EvolveToIsItem = value; }
        public new int EvolveToHash { get => base.EvolveToHash; set => base.EvolveToHash = value; }
        public new int CombineConfigHash { get => base.CombineConfigHash; set => base.CombineConfigHash = value; }
    }
}
