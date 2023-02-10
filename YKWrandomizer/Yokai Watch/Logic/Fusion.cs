using System;

namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class Fusion
    {
        public UInt32 BaseYokai;

        public UInt32 Material;

        public UInt32 EvolveTo;

        public Fusion(UInt32 baseYokai, UInt32 mateiral, UInt32 evolveTo)
        {
            BaseYokai = baseYokai;
            Material = mateiral;
            EvolveTo = evolveTo;
        }
    }
}
