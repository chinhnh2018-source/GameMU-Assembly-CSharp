using System;

namespace HSGameEngine.Drawing
{
	[Serializable]
	public struct Rectangle
	{
		public Rectangle(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public static Rectangle FromLTRB(int left, int top, int right, int bottom)
		{
			return new Rectangle(left, top, right - left, bottom - top);
		}

		public static Rectangle Intersect(Rectangle a, Rectangle b)
		{
			if (!a.IntersectsWithInclusive(b))
			{
				return Rectangle.Empty;
			}
			return Rectangle.FromLTRB(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
		}

		public void Intersect(Rectangle rect)
		{
			this = Rectangle.Intersect(this, rect);
		}

		public static Rectangle Union(Rectangle a, Rectangle b)
		{
			return Rectangle.FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
		}

		public int Bottom
		{
			get
			{
				return this.y + this.height;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.x == 0 && this.y == 0 && this.width == 0 && this.height == 0;
			}
		}

		public int Left
		{
			get
			{
				return this.X;
			}
		}

		public Point Location
		{
			get
			{
				return new Point(this.x, this.y);
			}
			set
			{
				this.x = value.X;
				this.y = value.Y;
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
				return this.y;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public bool Contains(int x, int y)
		{
			return x >= this.Left && x < this.Right && y >= this.Top && y < this.Bottom;
		}

		public bool Contains(Point pt)
		{
			return this.Contains(pt.X, pt.Y);
		}

		public override int GetHashCode()
		{
			return this.height + this.width ^ this.x + this.y;
		}

		public bool IntersectsWith(Rectangle rect)
		{
			return this.Left < rect.Right && this.Right > rect.Left && this.Top < rect.Bottom && this.Bottom > rect.Top;
		}

		private bool IntersectsWithInclusive(Rectangle r)
		{
			return this.Left <= r.Right && this.Right >= r.Left && this.Top <= r.Bottom && this.Bottom >= r.Top;
		}

		public void Offset(int x, int y)
		{
			this.x += x;
			this.y += y;
		}

		public void Offset(Point pos)
		{
			this.x += pos.X;
			this.y += pos.Y;
		}

		public override string ToString()
		{
			return string.Format("{{X={0},Y={1},Width={2},Height={3}}}", new object[]
			{
				this.x,
				this.y,
				this.width,
				this.height
			});
		}

		private int x;

		private int y;

		private int width;

		private int height;

		public static readonly Rectangle Empty;
	}
}
