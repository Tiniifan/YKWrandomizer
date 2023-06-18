using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace YKWrandomizer.Yokai_Watch.Randomizer
{
    public class RandomNumber
    {
        public readonly int Seed;

        private readonly RandomNumberGenerator RNG;

        public RandomNumber()
        {
            RNG = RandomNumberGenerator.Create();
            byte[] seedBytes = new byte[4];
            RNG.GetBytes(seedBytes);
            Seed = System.BitConverter.ToInt32(seedBytes, 0);
        }

        public RandomNumber(int seed)
        {
            RNG = RandomNumberGenerator.Create();
            Seed = seed;
        }

        public int Next(int max)
        {
            byte[] randomNumber = new byte[4];
            RNG.GetBytes(randomNumber);
            int result = System.BitConverter.ToInt32(randomNumber, 0);
            return new Random(Seed ^ result).Next(0, max);
        }

        public int Next(int min, int max)
        {
            byte[] randomNumber = new byte[4];
            RNG.GetBytes(randomNumber);
            int result = System.BitConverter.ToInt32(randomNumber, 0);
            return new Random(Seed ^ result).Next(min, max);
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
            List<int> randomList = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int randomNumber = Next(min, max);

                while (randomList.Contains(randomNumber))
                {
                    randomNumber = Next(min, max);
                }

                randomList.Add(randomNumber);
            }

            return randomList;
        }

        public List<int> GetNumbers(int min, int max, int count, List<int> excludeIndex)
        {
            List<int> randomList = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int randomNumber = Next(min, max);

                while (randomList.Contains(randomNumber) || excludeIndex.Contains(randomNumber))
                {
                    randomNumber = Next(min, max);
                }

                randomList.Add(randomNumber);
            }

            return randomList;
        }

        public int Probability(int[] itemsProbability)
        {
            List<double> pool = itemsProbability.Select(x => x / 100.0).ToList();

            double Next()
            {
                double u = pool.Sum(p => p);
                byte[] randomBytes = new byte[8];
                RNG.GetBytes(randomBytes);
                double r = System.BitConverter.ToUInt64(randomBytes, 0) / (ulong.MaxValue + 1.0);

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