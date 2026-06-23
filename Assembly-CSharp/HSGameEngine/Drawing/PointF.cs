using System;

namespace HSGameEngine.Drawing
{
	[Serializable]
	public struct PointF
	{
		public PointF(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public bool IsEmpty
		{
			get
			{
				return (double)this.x == 0.0 && (double)this.y == 0.0;
			}
		}

		public float X
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

		public float Y
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
			return obj is PointF && this == (PointF)obj;
		}

		public override int GetHashCode()
		{
			return (int)this.x ^ (int)this.y;
		}

		public static bool operator ==(PointF left, PointF right)
		{
			return left.X == right.X && left.Y == right.Y;
		}

		public static bool operator !=(PointF left, PointF right)
		{
			return left.X != right.X || left.Y != right.Y;
		}

		private float x;

		private float y;

		public static readonly PointF Empty;
	}
}
