using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class BattleCommand : IBattleCommand
    {
        public new int BattleCommandHash { get => base.BattleCommandHash; set => base.BattleCommandHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int[] Unk = new int[31];
    }
}
