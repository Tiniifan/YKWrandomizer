using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1.Logic
{
    public class CharaabilityConfig : ICharaabilityConfig
    {
        public new int CharaabilityConfigHash { get => base.CharaabilityConfigHash; set => base.CharaabilityConfigHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public new int UnkNameHash { get => base.UnkNameHash; set => base.UnkNameHash = value; }
        public new int CharaAbilityEffectHash { get => base.CharaAbilityEffectHash; set => base.CharaAbilityEffectHash = value; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
    }
}
