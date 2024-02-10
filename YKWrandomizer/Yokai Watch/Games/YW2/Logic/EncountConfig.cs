using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW2.Logic
{
    public class EncountTable : IEncountTable
    {
        public new int EncountConfigHash { get => base.EncountConfigHash; set => base.EncountConfigHash = value; }
        public new int[] EncountOffsets { get => base.EncountOffsets; set => base.EncountOffsets = value; }
        public object BattleScript { get; set; }
        public object Unk1 { get; set; }
        public object Unk2 { get; set; }
        public object Unk3 { get; set; }
        public object Unk4 { get; set; }
        public object Unk5 { get; set; }
        public object Unk6 { get; set; }
        public object Unk7 { get; set; }
        public object Unk8 { get; set; }
        public object Unk9 { get; set; }
        public object Unk10 { get; set; }
        public object Unk11 { get; set; }
        public object Unk12 { get; set; }
        public object Unk13 { get; set; }
        public object Unk14 { get; set; }
        public object Unk15 { get; set; }
        public object Unk16 { get; set; }
        public object Unk17 { get; set; }
        public object Unk18 { get; set; }
        public object Unk19 { get; set; }
        public object Unk20 { get; set; }
        public object Unk21 { get; set; }
        public object Unk22 { get; set; }

        public EncountTable()
        {
            EncountOffsets = new int[6];
        }
    }

    public class EncountChara : IEncountChara
    {
        public new int ParamHash { get => base.ParamHash; set => base.ParamHash = value; }
        public new int Level { get => base.Level; set => base.Level = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int Unk3 { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
    }
}
