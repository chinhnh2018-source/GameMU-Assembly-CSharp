using System;

namespace HTMLEngine.Core
{
	internal abstract class DeviceChunk : PoolableObject
	{
		public int TotalWidth
		{
			get
			{
				return this.Rect.Width + this.ExtraSpace;
			}
		}

		public int TotalHeight
		{
			get
			{
				return this.Rect.Height;
			}
		}

		public abstract void Draw(float deltaTime, string linkText, object userData);

		internal override void OnAcquire()
		{
		}

		internal override void OnRelease()
		{
		}

		public abstract void MeasureSize();

		public bool Contains(int x, int y)
		{
			int left = this.Rect.Left;
			int num = this.Rect.Right + this.ExtraSpace;
			int top = this.Rect.Top;
			int bottom = this.Rect.Bottom;
			return x >= left && x < num && y >= top && y < bottom;
		}

		public HtRect Rect;

		public HtFont Font;

		public int ExtraSpace;
	}
}
