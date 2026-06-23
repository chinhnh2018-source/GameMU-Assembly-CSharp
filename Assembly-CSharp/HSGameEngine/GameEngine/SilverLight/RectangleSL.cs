using System;
using HSGameEngine.Drawing;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class RectangleSL : SpriteSL, ISilverLight
	{
		public Brush Fill { get; set; }

		public Brush Stroke { get; set; }

		public Thickness StrokeThickness { get; set; }

		public double RadiusX { get; set; }

		public double RadiusY { get; set; }

		public bool doubleClickEnabled
		{
			get
			{
				return this._doubleClickEnabled;
			}
			set
			{
				this._doubleClickEnabled = value;
			}
		}

		public double alpha { get; set; }

		public float Opacity
		{
			get
			{
				return this._Opacity;
			}
			set
			{
				this._Opacity = value;
			}
		}

		public string HorizontalAlignment
		{
			get
			{
				return this._HorizontalAlignment;
			}
			set
			{
				this._HorizontalAlignment = value;
			}
		}

		public string VerticalAlignment
		{
			get
			{
				return this._VerticalAlignment;
			}
			set
			{
				this._VerticalAlignment = value;
			}
		}

		public static implicit operator Rectangle(RectangleSL rectangleSL)
		{
			if (null != rectangleSL)
			{
				return new Rectangle(0, 0, (int)rectangleSL.Width, (int)rectangleSL.Height);
			}
			return default(Rectangle);
		}

		private bool _doubleClickEnabled;

		private float _Opacity;

		private string _HorizontalAlignment = string.Empty;

		private string _VerticalAlignment = string.Empty;
	}
}
