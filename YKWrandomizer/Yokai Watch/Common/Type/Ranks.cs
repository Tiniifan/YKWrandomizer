using System.Collections.Generic;

namespace YKWrandomizer.Yokai_Watch.Common
{
    public class Ranks
    {
        public static Dictionary<byte, string> Value = new Dictionary<byte, string>()
        {
            {0x00, "E" },
            {0x01, "D" },
            {0x02, "C" },
            {0x03, "B" },
            {0x04, "A" },
            {0x05, "S" },
        };
    }
}
