using System;
using System.Globalization;

namespace HTMLEngine
{
	public struct HtPoint
	{
		public HtPoint(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "(X:{0} Y:{1})", new object[]
			{
				this.X,
				this.Y
			});
		}

		public int X;

		public int Y;
	}
}
