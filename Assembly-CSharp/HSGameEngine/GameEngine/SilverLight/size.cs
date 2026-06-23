using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class size
	{
		public size(double width, double height)
		{
			this._Width = width;
			this._Height = height;
		}

		public double Height
		{
			get
			{
				return this._Height;
			}
			set
			{
				this._Height = value;
			}
		}

		public double Width
		{
			get
			{
				return this._Width;
			}
			set
			{
				this._Width = value;
			}
		}

		private double _Width;

		private double _Height;
	}
}
