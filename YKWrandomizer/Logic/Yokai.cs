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

        public UInt32 CharabaseID;

        public string Name;

        public Status Status; 

        public Yokai(string _Name, UInt32 _CharaBaseID, Status _Status)
        {
            Name = _Name;
            CharabaseID = _CharaBaseID;
            Status = _Status;
        }

        public void NewStat(RandomNumber seed, int min, int max)
        {
            for (int stat = 0; stat < BaseStat.Count; stat++)
            {
                if (stat > 0)
                {
                    min = 13;
                }

                BaseStat[stat] = seed.GetNumber(min, max) + (Rank.ID + 1) * 2;

                if (Status.Name == "Boss Friendly")
                {
                    BaseStat[stat] += 10;
                }
            }
        }

        public void NewStat(RandomNumber seed)
        {
            for (int stat = 0; stat < BaseStat.Count; stat++)
            {
                int min = Evolution.EvolutionTo.BaseStat[stat] - (Evolution.EvolutionTo.Rank.ID + 1) * 2;
                int max = 38;

                if (Evolution.EvolutionTo.Status.Name == "Boss Friendly")
                {
                    max += 10;
                }

                BaseStat[stat] = seed.GetNumber(min, max) + (Rank.ID + 1) * 2;

                if (Status.Name == "Boss Friendly")
                {
                    BaseStat[stat] += 10;
                }
            }
        }
    }
}
