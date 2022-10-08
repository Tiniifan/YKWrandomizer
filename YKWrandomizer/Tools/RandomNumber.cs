using System;
using System.Collections.Generic;

namespace YKWrandomizer.Tools
{
    public class RandomNumber : Random
    {
        public readonly int Seed;

        public RandomNumber(int seed) : base(seed)
        {
            Seed = seed;
        }

        public int GetNumber(int min, int max)
        {
            return Next(min, max);
        }

        public List<int> GetNumbers(int min, int max, int count)
        {
            List<int> randomList = new List<int>(new int[count]);

            for (int i = 0; i < randomList.Count; i++)
            {
                int randomNumber = GetNumber(min, max);

                while (randomList.IndexOf(randomNumber) == -1)
                {
                    randomList[i] = randomNumber;
                }
            }

            return randomList;
        }

        public List<int> GetNumbers(int min, int max, int count, List<int> excludeIndex)
        {
            List<int> randomList = new List<int>(new int[count]);

            for (int i = 0; i < randomList.Count; i++)
            {
                int randomNumber = GetNumber(min, max);

                while (randomList.IndexOf(randomNumber) == -1 && excludeIndex.IndexOf(randomNumber) == -1)
                {
                    randomList[i] = randomNumber;
                }
            }

            return randomList;
        }
    }
}
