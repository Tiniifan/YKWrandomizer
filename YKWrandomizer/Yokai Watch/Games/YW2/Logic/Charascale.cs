using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW2.Logic
{
    public class Charascale : ICharascale
    {
        public new int BaseHash { get => base.BaseHash; set => base.BaseHash = value; }
        public new float Scale1 { get => base.Scale1; set => base.Scale1 = value; } // WorldMap
        public new float Scale2 { get => base.Scale2; set => base.Scale2 = value; }
        public new float Scale3 { get => base.Scale3; set => base.Scale3 = value; } // Menu
        public new float Scale4 { get => base.Scale4; set => base.Scale4 = value; } // Battle
        public new float Scale5 { get => base.Scale5; set => base.Scale5 = value; }
        public new float Scale6 { get => base.Scale6; set => base.Scale6 = value; }
    }
}
