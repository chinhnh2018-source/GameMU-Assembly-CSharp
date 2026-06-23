using System;
using System.Globalization;

namespace HTMLEngine
{
	public struct HtRect
	{
		public HtRect(int x, int y, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public int Left
		{
			get
			{
				return this.X;
			}
		}

		public int Right
		{
			get
			{
				return this.X + this.Width;
			}
		}

		public int Top
		{
			get
			{
				return this.Y;
			}
		}

		public int Bottom
		{
			get
			{
				return this.Y + this.Height;
			}
		}

		public HtRect Offset(int dx, int dy)
		{
			return new HtRect(this.X + dx, this.Y + dy, this.Width, this.Height);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "(X:{0} Y:{1} Width:{2} Height:{3})", new object[]
			{
				this.X,
				this.Y,
				this.Width,
				this.Height
			});
		}

		public int X;

		public int Y;

		public int Width;

		public int Height;
	}
}
