using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace YKWrandomizer.Tools
{
	public class RandomNumber
	{
		public int min;

		public int max;

		public RandomNumber(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public int GetNumber()
		{
			object syncLock = new object();
			RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();
			lock (syncLock)
			{
				uint scale = uint.MaxValue;

				while (scale == uint.MaxValue)
				{
					byte[] four_bytes = new byte[4];
					Rand.GetBytes(four_bytes);
					scale = BitConverter.ToUInt32(four_bytes, 0);
				}

				return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
			}
		}

		public List<int> GetNumbers(int count)
        {
			List<int> randomList = new List<int>(new int [count]);

			for (int i = 0; i < randomList.Count; i++)
            {
				int randomNumber = GetNumber();

				while (randomList.IndexOf(randomNumber) == -1)
                {
					randomList[i] = randomNumber;
                }
            }

			return randomList;
        }

		public List<int> GetNumbers(int count, List<int> excludeIndex)
		{
			List<int> randomList = new List<int>(new int[count]);

			for (int i = 0; i < randomList.Count; i++)
			{
				int randomNumber = GetNumber();

				while (randomList.IndexOf(randomNumber) == -1 && excludeIndex.IndexOf(randomNumber) == -1)
				{
					randomList[i] = randomNumber;
				}
			}

			return randomList;
		}
	}
}
