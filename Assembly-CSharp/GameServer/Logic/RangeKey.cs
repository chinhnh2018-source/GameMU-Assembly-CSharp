using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	internal class RangeKey : IComparable<RangeKey>, IEqualityComparer<RangeKey>
	{
		public RangeKey(short left, short right)
		{
			this.Left = left;
			this.Right = right;
		}

		public int CompareTo(RangeKey obj)
		{
			if (this.Right < obj.Left)
			{
				return -1;
			}
			if (this.Left > obj.Right)
			{
				return 1;
			}
			return 0;
		}

		public bool Equals(RangeKey x, RangeKey y)
		{
			return 0 == x.CompareTo(y);
		}

		public int GetHashCode(RangeKey obj)
		{
			return obj.GetHashCode();
		}

		private short Left;

		private short Right;
	}
}
