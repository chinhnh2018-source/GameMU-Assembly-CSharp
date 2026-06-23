using System;

namespace HSGameEngine.Drawing
{
	[Serializable]
	public struct Point
	{
		public Point(int dw)
		{
			this.y = dw >> 16;
			this.x = (dw & 65535);
		}

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static Point Ceiling(PointF value)
		{
			checked
			{
				int num = (int)Math.Ceiling((double)value.X);
				int num2 = (int)Math.Ceiling((double)value.Y);
				return new Point(num, num2);
			}
		}

		public static Point Round(PointF value)
		{
			checked
			{
				int num = (int)Math.Round((double)value.X);
				int num2 = (int)Math.Round((double)value.Y);
				return new Point(num, num2);
			}
		}

		public static Point Truncate(PointF value)
		{
			checked
			{
				int num = (int)value.X;
				int num2 = (int)value.Y;
				return new Point(num, num2);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.x == 0 && this.y == 0;
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

		public override bool Equals(object obj)
		{
			return obj is Point && this == (Point)obj;
		}

		public override int GetHashCode()
		{
			return this.x ^ this.y;
		}

		public void Offset(int dx, int dy)
		{
			this.x += dx;
			this.y += dy;
		}

		public static bool operator ==(Point left, Point right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(Point left, Point right)
		{
			return left.X != right.X || left.Y != right.Y;
		}

		public static implicit operator PointF(Point p)
		{
			return new PointF((float)p.X, (float)p.Y);
		}

		private int x;

		private int y;

		public static readonly Point Empty;
	}
}
