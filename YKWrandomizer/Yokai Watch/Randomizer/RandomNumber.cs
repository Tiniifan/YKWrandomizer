using System;
using System.Linq;
using System.Collections.Generic;

namespace YKWrandomizer.Yokai_Watch.Randomizer
{
    public class RandomNumber : Random
    {
        public readonly int Seed;

        public RandomNumber()
        {
            Seed = new Random().Next();
        }

        public RandomNumber(int seed) : base(seed)
        {
            Seed = seed;
        }

        public int Next(int min, int max, int excludeIndex)
        {
            int randomNumber = Next(min, max);

            while (randomNumber == excludeIndex)
            {
                randomNumber = Next(min, max);
            }

            return randomNumber;
        }

        public List<int> GetNumbers(int min, int max, int count)
        {
            List<int> randomList = new List<int>(new int[count]);

            for (int i = 0; i < randomList.Count; i++)
            {
                int randomNumber = Next(min, max);

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
                int randomNumber = Next(min, max);

                while (randomList.IndexOf(randomNumber) == -1 && excludeIndex.IndexOf(randomNumber) == -1)
                {
                    randomList[i] = randomNumber;
                }
            }

            return randomList;
        }

        public int Probability(int[] itemsProbability)
        {
            List<double> pool = itemsProbability.Select(x => x / 100.0).ToList();

            double Next()
            {
                double u = pool.Sum(p => p);
                double r = NextDouble() * u;

                double sum = 0;
                foreach (double n in pool)
                {
                    if (r <= (sum = sum + n))
                    {
                        return n;
                    }
                }

                return 0.0;
            }

            return pool.IndexOf(Next());
        }
    }
}
