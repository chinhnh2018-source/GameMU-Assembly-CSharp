using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	internal class CoupleArenaDivorceRecord
	{
		public void Add(int roleId1, int roleId2)
		{
			long unionCouple = this.GetUnionCouple(roleId1, roleId2);
			if (!this.keySet.Contains(unionCouple))
			{
				this.keySet.Add(unionCouple);
			}
		}

		public bool IsDivorce(int roleId1, int roleId2)
		{
			long unionCouple = this.GetUnionCouple(roleId1, roleId2);
			return this.keySet.Contains(unionCouple);
		}

		public void Reset()
		{
			this.keySet.Clear();
		}

		private long GetUnionCouple(int a1, int a2)
		{
			int num = Math.Min(a1, a2);
			int num2 = Math.Max(a1, a2);
			long num3 = (long)num;
			num3 <<= 32;
			return num3 | (long)((ulong)num2);
		}

		private HashSet<long> keySet = new HashSet<long>();
	}
}
