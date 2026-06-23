using System;

namespace HSGameEngine.GameEngine.Common
{
	public class MathUtils
	{
		public static int GMin(int l, int r)
		{
			return (l > r) ? r : l;
		}

		public static int GMax(int l, int r)
		{
			return (l < r) ? r : l;
		}

		public static double GMin(double l, double r)
		{
			return (l > r) ? r : l;
		}

		public static double GMax(double l, double r)
		{
			return (l < r) ? r : l;
		}
	}
}
