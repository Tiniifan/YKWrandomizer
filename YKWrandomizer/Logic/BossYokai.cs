using System;

namespace YKWrandomizer.Logic
{
    public class BossYokai
    {
        public UInt32 ID;

        public int Count;

        public int Level;

        public uint PositionInEncounter;

        public string ScriptName;

        public BossYokai(UInt32 _ID, int _Count, int _Level, uint _PositionInEncounter, string _ScriptName)
        {
            ID = _ID;
            Count = _Count;
            Level = _Level;
            PositionInEncounter = _PositionInEncounter;
            ScriptName = _ScriptName;
        }
    }
}
