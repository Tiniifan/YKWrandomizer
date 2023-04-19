using System.Collections.Generic;

namespace YKWrandomizer.Yokai_Watch.Common
{
    public static class GivenYokais
    {
        public static Dictionary<string, List<uint>> YW1 = new Dictionary<string, List<uint>>
        {
             {"/seq/phs/c01_trigger_0.01.xq", new List<uint>(){ 0x987 } },
            {"/seq/phs/c02_trigger_0.01.xq", new List<uint>(){ 0x7E4 } },
            {"/seq/phs/c03_trigger_0.01.xq", new List<uint>(){ 0x4E3 } },
            {"/seq/phs/c04_trigger_0.01.xq", new List<uint>(){ 0x58C } },
            {"/seq/phs/c06_trigger_0.01.xq", new List<uint>(){ 0x66D } },
            {"/seq/phs/c07_trigger_0.01.xq", new List<uint>(){ 0x2F1 } },
            {"/seq/phs/c09_trigger_0.01.xq", new List<uint>(){ 0x2E7 } },
            {"/seq/phs/c11_trigger_0.01.xq", new List<uint>(){ 0xD62 } },
        };

        public static Dictionary<string, List<uint>> YW3 = new Dictionary<string, List<uint>>
        {
            {"/seq/phs/c01_trigger_0.01.xq", new List<uint>() {0xE0D, 0xF09, 0xFA2 } },
        };
    }
}
