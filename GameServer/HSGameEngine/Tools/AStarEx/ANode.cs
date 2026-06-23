using System;

namespace HSGameEngine.Tools.AStarEx
{
	public class ANode
	{
		public ANode(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static long GetGUID(int key1, int key2)
		{
			long num = (long)key1;
			long num2 = (long)key2;
			return num << 32 | num2;
		}

		public static int GetGUID_X(long val)
		{
			return (int)(val >> 32 & (long)((ulong)-1));
		}

		public static int GetGUID_Y(long val)
		{
			return (int)(val & (long)((ulong)-1));
		}

		public int x;

		public int y;
	}
}
