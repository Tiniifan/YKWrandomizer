using System;

namespace YKWrandomizer.Logic
{
    public class Evolution
    {
        public UInt32 BaseYokai;

        public UInt32 EvolveTo;

        public int Level;

        public Evolution(UInt32 baseYokai, UInt32 evolveTo, int level)
        {
            BaseYokai = baseYokai;
            EvolveTo = evolveTo;
            Level = level;
        }
    }
}
