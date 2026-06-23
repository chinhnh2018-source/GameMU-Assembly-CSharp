using System;
using System.Globalization;

namespace HTMLEngine
{
	public struct HtSize
	{
		public HtSize(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "(Width:{0} Height:{1})", new object[]
			{
				this.Width,
				this.Height
			});
		}

		public int Width;

		public int Height;
	}
}
