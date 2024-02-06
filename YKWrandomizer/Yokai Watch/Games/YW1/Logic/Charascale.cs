using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class Charascale : ICharascale
    {
        public new int BaseHash { get => base.BaseHash; set => base.BaseHash = value; }
        public new float MapSize { get => base.Scale1; set => base.Scale1 = value; }
        public new float Scale2 { get => base.Scale2; set => base.Scale2 = value; }
        public new float BattleOwnSize { get => base.Scale3; set => base.Scale3 = value; }
        public new float BattleRivalSize { get => base.Scale4; set => base.Scale4 = value; }
        public new float MenuSize { get => base.Scale5; set => base.Scale5 = value; }
    }
}
