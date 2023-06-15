using System;

namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class Evolution
    {
        public UInt32 BaseYokai { get; set; }

        public UInt32 EvolveTo { get; set; }

        public int Level { get; set; }

        public Evolution()
        {
        }

        public Evolution(UInt32 baseYokai, UInt32 evolveTo, int level)
        {
            BaseYokai = baseYokai;
            EvolveTo = evolveTo;
            Level = level;
        }
    }
}
