using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class SolidColorBrush : Brush
	{
		public SolidColorBrush(uint color)
		{
			this._Color = color;
		}

		public uint Color
		{
			get
			{
				return this._Color;
			}
		}

		private uint _Color = ColorSL.FromArgb(255, 255, 255, 255);
	}
}
