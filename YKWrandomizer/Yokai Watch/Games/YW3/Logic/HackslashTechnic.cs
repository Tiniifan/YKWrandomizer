using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class HackslashTechnic : IHackslashTechnic
    {
        public new int HackslashTechnicHash { get => base.HackslashTechnicHash; set => base.HackslashTechnicHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
    }
}
