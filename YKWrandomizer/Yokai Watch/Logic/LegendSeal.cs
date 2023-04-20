using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class LegendSeal
    {
        public uint SealID;

        public uint LegendaryParamID;

        public uint[] RequirmentParamID;

        public LegendSeal()
        {

        }

        public LegendSeal(uint sealID, uint legendaryParamID, uint[] requirmentParamID)
        {
            SealID = sealID;
            LegendaryParamID = legendaryParamID;
            RequirmentParamID = requirmentParamID;
        }
    }
}
