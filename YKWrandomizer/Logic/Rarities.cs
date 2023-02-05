using System.Collections.Generic;

namespace YKWrandomizer.Logic
{
    public class Rarities
    {
        public static Dictionary<byte, string> Values = new Dictionary<byte, string>()
        {
            {0x00, "Common" },
            {0x01, "Rare" },
        };
    }
}
