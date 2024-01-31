using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YWB.Logic
{
    public class OrgetimeTechnic : IOrgetimeTechnic
    {
        public new int OrgeTimeTechnicHash { get => base.OrgeTimeTechnicHash; set => base.OrgeTimeTechnicHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
    }
}
