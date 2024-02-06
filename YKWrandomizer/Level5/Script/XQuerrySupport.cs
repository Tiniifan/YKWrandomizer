using System;
using System.Runtime.InteropServices;

namespace YKWrandomizer.Level5.Script
{
    public class XQuerrySupport
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public UInt32 Magic;
            public short FunctionCount;
            public short FunctionOffset;
            public short JumpOffset;
            public short JumpCount;
            public short InstructionOffset;
            public short InstructionCount;
            public short ArgumentOffset;
            public short ArgumentCount;
            public short GlobalVariableCount;
            public short StringOffset;

            public int FunctionOffsetShifted => FunctionOffset << 2;
            public int JumpOffsetShifted => JumpOffset << 2;
            public int InstructionOffsetShifted => InstructionOffset << 2;
            public int ArgumentOffsetShifted => ArgumentOffset << 2;
            public int StringOffsetShifted => StringOffset << 2;
        }
    }
}
