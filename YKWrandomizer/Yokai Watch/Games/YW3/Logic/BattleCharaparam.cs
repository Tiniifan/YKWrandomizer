using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class BattleCharaparam : IBattleCharaparam
    {
        public new int ParamHash { get => base.ParamHash; set => base.ParamHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public new int Money { get => base.Money; set => base.Money = value; }
        public new int Experience { get => base.Experience; set => base.Experience = value; }
        public new int Drop1Hash { get => base.Drop1Hash; set => base.Drop1Hash = value; }
        public new int Drop1Rate { get => base.Drop1Rate; set => base.Drop1Rate = value; }
        public new int Drop2Hash { get => base.Drop2Hash; set => base.Drop2Hash = value; }
        public new int Drop2Rate { get => base.Drop2Rate; set => base.Drop2Rate = value; }
        public int Unk7 { get; set; }
    }
}
