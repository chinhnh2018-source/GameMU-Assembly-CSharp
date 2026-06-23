using System;

namespace Server.Tools.AStarEx
{
	public class ANode
	{
		public ANode(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static int GetGUID(int key1, int key2)
		{
			return key1 << 16 | key2;
		}

		public static int GetGUID_X(int val)
		{
			return (int)((long)(val >> 16) & 65535L);
		}

		public static int GetGUID_Y(int val)
		{
			return (int)((long)val & 65535L);
		}

		public int x;

		public int y;
	}
}
