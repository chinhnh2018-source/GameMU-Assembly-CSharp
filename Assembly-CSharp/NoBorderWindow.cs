using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.SilverLight;

public class NoBorderWindow : UserControl
{
	public SpriteSL BodyPresenter
	{
		get
		{
			return this;
		}
	}

	public void SetContent(SpriteSL presenter, SpriteSL obj, double left, double top)
	{
		obj.X = 0.0;
		obj.Y = 0.0;
		presenter.Add(obj);
	}

	public Brush BodyBackBrush { get; set; }

	public double BodyBackOpacity { get; set; }

	public double Left
	{
		get
		{
			return this.X;
		}
		set
		{
			this.X = value;
		}
	}

	public double Top
	{
		get
		{
			return this.Y;
		}
		set
		{
			this.Y = value;
		}
	}

	public double ZIndex
	{
		get
		{
			return base.Z;
		}
		set
		{
			base.Z = value;
		}
	}

	public double BodyLeft { get; set; }

	public double BodyTop { get; set; }

	public double BodyWidth
	{
		get
		{
			return this._BodyWidth;
		}
		set
		{
			this._BodyWidth = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this._BodyHeight;
		}
		set
		{
			this._BodyHeight = value;
		}
	}

	public BitmapData BodyBackground { get; set; }

	public Rectangle GetBoundsRect()
	{
		return new Rectangle(0, 0, 0, 0);
	}

	private double _BodyWidth;

	private double _BodyHeight;
}
