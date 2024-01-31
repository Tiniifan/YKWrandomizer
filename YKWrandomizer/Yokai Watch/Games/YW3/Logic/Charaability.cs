using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3.Logic
{
    public class CharaabilityConfig : ICharaabilityConfig
    {
        public new int CharaabilityConfigHash { get => base.CharaabilityConfigHash; set => base.CharaabilityConfigHash = value; }
        public new int NameHash { get => base.NameHash; set => base.NameHash = value; }
        public new int CharaAbilityEffectHash { get => base.CharaAbilityEffectHash; set => base.CharaAbilityEffectHash = value; }
        public new int DescriptionHash { get => base.DescriptionHash; set => base.DescriptionHash = value; }
    }
}
