namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class ICharaparam
    {
        public int ParamHash { get; set; }
        public int BaseHash { get; set; }
        public int Tribe { get; set; }
        public int MinHP { get; set; }
        public int MinStrength { get; set; }
        public int MinSpirit { get; set; }
        public int MinDefense { get; set; }
        public int MinSpeed { get; set; }
        public int MaxHP { get; set; }
        public int MaxStrength { get; set; }
        public int MaxSpirit { get; set; }
        public int MaxDefense { get; set; }
        public int MaxSpeed { get; set; }
        public int AttackHash { get; set; }
        public int TechniqueHash { get; set; }
        public int InspiritHash { get; set; }
        public float AttributeDamageFire { get; set; }
        public float AttributeDamageIce { get; set; }
        public float AttributeDamageEarth { get; set; }
        public float AttributeDamageLigthning { get; set; }
        public float AttributeDamageWater { get; set; }
        public float AttributeDamageWind { get; set; }
        public int SoultimateHash { get; set; }
        public int AbilityHash { get; set; }
        public int Money { get; set; }
        public int Experience { get; set; }
        public int Drop1Hash { get; set; }
        public int Drop1Rate { get; set; }
        public int Drop2Hash { get; set; }
        public int Drop2Rate { get; set; }
        public int ExperienceCurve { get; set; }
        public int Quote1 { get; set; }
        public int Quote2 { get; set; }
        public int Quote3 { get; set; }
        public int BefriendQuote { get; set; }
        public int EvolveOffset { get; set; }
        public int EvolveParam { get; set; }
        public int EvolveLevel { get; set; }
        public int EvolveCost { get; set; }
        public int MedaliumOffset { get; set; }
        public bool ShowInMedalium { get; set; }
        public int ScoutableHash { get; set; }
        public int FavoriteDonut { get; set; }
        public int Speed { get; set; }
        public int Strongest { get; set; }
        public int Weakness { get; set; }
        public bool CanFuse { get; set; }
        public int WaitTime { get; set; }

        public int BlasterSkill { get; set; }
        public int BlasterAttack { get; set; }
        public int BlasterSoultimate { get; set; }
        public int BlasterMoveSlot1 { get; set; }
        public int BlasterEarnLevelMoveSlot1 { get; set; }
        public int BlasterMoveSlot2 { get; set; }
        public int BlasterEarnLevelMoveSlot2 { get; set; }
        public int BlasterMoveSlot3 { get; set; }
        public int BlasterEarnLevelMoveSlot3 { get; set; }
        public int BlasterMoveSlot4 { get; set; }
        public int BlasterEarnLevelMoveSlot4 { get; set; }
        public int DropOniOrbRate { get; set; }
        public int DropOniOrb { get; set; }

        public int EquipmentSlotsAmount { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
