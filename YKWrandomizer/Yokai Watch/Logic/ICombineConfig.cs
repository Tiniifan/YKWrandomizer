namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class ICombineConfig
    {
        public bool BaseIsItem { get; set; }
        public int BaseHash { get; set; }
        public bool MaterialIsItem { get; set; }
        public int MaterialHash { get; set; }
        public bool EvolveToIsItem { get; set; }
        public int EvolveToHash { get; set; }
        public int CombineConfigHash { get; set; }
        public bool CombineIsItem { get; set; }
        public int OniOrbCost { get; set; }
    }
}
