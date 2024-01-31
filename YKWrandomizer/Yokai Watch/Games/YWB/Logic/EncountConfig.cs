using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YWB.Logic
{
    public class EncountTable : IEncountTable
    {
        public new int EncountConfigHash { get => base.EncountConfigHash; set => base.EncountConfigHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public new int CharaCount { get => base.CharaCount; set { base.CharaCount = value; UpdateCharasSize(); } }
        public new int[] Charas { get => base.Charas; set => base.Charas = value; }

        private void UpdateCharasSize()
        {
            Charas = new int[CharaCount * 3];
        }
    }
}
