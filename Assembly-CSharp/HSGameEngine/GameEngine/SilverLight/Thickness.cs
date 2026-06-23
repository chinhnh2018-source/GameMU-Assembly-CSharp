using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class Thickness
	{
		public Thickness(double left, double top, double right, double bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		public bool IsEmpty()
		{
			return this.Left == 0.0 && this.Top == 0.0 && this.Right == 0.0 && this.Bottom == 0.0;
		}

		public double Bottom { get; set; }

		public double Left { get; set; }

		public double Right { get; set; }

		public double Top { get; set; }
	}
}
