using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class Skillconfig : ISkillconfig
    {
        public new int SkillHash { get => base.SkillHash; set => base.SkillHash = value; }
        public int Unk1 { get; set; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public int Unk3 { get; set; }
        public int Power { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public int Unk6 { get; set; }
        public int Element { get; set; }
        public int Unk8 { get; set; }
        public int Unk9 { get; set; }
        public int Unk10 { get; set; }
    }
}
