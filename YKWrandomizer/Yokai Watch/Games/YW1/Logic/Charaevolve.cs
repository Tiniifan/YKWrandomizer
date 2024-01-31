using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class Charaevolve : ICharaevolve
    {
        public new int Level { get => base.Level; set => base.Level = value; }
        public new int ParamHash { get => base.ParamHash; set => base.ParamHash = value; }
    }
}
