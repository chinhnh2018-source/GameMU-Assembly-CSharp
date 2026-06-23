using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	internal class CoupleArenaMatchTimeLimiter
	{
		public int GetMatchTimes(int a1, int a2, int b1, int b2)
		{
			long key;
			long key2;
			this.GetUnionCouple2(a1, a2, b1, b2, out key, out key2);
			Dictionary<long, int> dictionary = null;
			int num = 0;
			int result;
			if (this.TimesDict.TryGetValue(key, out dictionary) && dictionary.TryGetValue(key2, out num))
			{
				result = num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public void AddMatchTimes(int a1, int a2, int b1, int b2, int times = 1)
		{
			long key;
			long num;
			this.GetUnionCouple2(a1, a2, b1, b2, out key, out num);
			Dictionary<long, int> dictionary = null;
			if (!this.TimesDict.TryGetValue(key, out dictionary))
			{
				dictionary = (this.TimesDict[key] = new Dictionary<long, int>());
			}
			if (dictionary.ContainsKey(num))
			{
				Dictionary<long, int> dictionary2;
				long key2;
				(dictionary2 = dictionary)[key2 = num] = dictionary2[key2] + times;
			}
			else
			{
				dictionary.Add(num, times);
			}
		}

		public void Reset()
		{
			this.TimesDict.Clear();
		}

		private void GetUnionCouple2(int a1, int a2, int b1, int b2, out long min, out long max)
		{
			long unionCouple = this.GetUnionCouple(a1, a2);
			long unionCouple2 = this.GetUnionCouple(b1, b2);
			min = Math.Min(unionCouple, unionCouple2);
			max = Math.Max(unionCouple, unionCouple2);
		}

		private long GetUnionCouple(int a1, int a2)
		{
			int num = Math.Min(a1, a2);
			int num2 = Math.Max(a1, a2);
			long num3 = (long)num;
			num3 <<= 32;
			return num3 | (long)((ulong)num2);
		}

		private Dictionary<long, Dictionary<long, int>> TimesDict = new Dictionary<long, Dictionary<long, int>>();
	}
}
