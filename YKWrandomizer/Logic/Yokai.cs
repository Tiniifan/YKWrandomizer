using System;
using YKWrandomizer.Tools;
using System.Collections.Generic;

namespace YKWrandomizer.Logic
{
    public class Yokai
    {
        public Tribe Tribe;

        public Rank Rank;

        public Rarity Rarity;

        public bool IsLegendary;

        public List<Item> Drops = new List<Item>(new Item[2]);

        public Evolution Evolution;

        public Move Skill;

        public List<Move> Moveset = new List<Move>(new Move[4]);

        public List<int> BaseStat = new List<int>(new int[5]);

        public int Money;

        public int Experience;

        public bool BeFriend;

        public UInt32 CharabaseID;

        public string Name;

        public Status Status; 

        public Yokai(string _Name, UInt32 _CharaBaseID, Status _Status)
        {
            Name = _Name;
            CharabaseID = _CharaBaseID;
            Status = _Status;
        }

        public void NewStat(int min, int max)
        {
            for (int stat = 0; stat < BaseStat.Count; stat++)
            {
                if (Evolution != null)
                {
                    min = Evolution.EvolutionTo.BaseStat[stat];
                }

                if (stat > 0 & Evolution == null)
                {
                    min = 13;
                }

                BaseStat[stat] = new RandomNumber(min, max).GetNumber() + (Rank.ID + 1) * 2;
            }
        }
    }
}
