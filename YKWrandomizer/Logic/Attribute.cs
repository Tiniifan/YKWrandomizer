using System.Collections.Generic;

namespace YKWrandomizer.Logic
{
    public class Attribute
    {
        public static Dictionary<byte, string> Value = new Dictionary<byte, string>()
        {
            {0x00, "Untype" },
            {0x01, "Fire" },
            {0x02, "Water" },
            {0x03, "Lightning" },
            {0x04, "Earth" },
            {0x05, "Wind" },
            {0x06, "Ice" },
            {0x07, "Drain" },
            {0x08, "Strong Attack" },
            {0x09, "Restoration" },
        };
    }
}
