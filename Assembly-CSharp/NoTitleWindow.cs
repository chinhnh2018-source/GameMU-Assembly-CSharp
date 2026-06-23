using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class NoTitleWindow : UserControl
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

	public double CloseButtonLeft { get; set; }

	public double CloseButtonTop { get; set; }

	public double CloseButtonWidth { get; set; }

	public double CloseButtonHeight { get; set; }

	public BitmapData CloseButtonFill { get; set; }

	public BitmapData CloseButtonTransformFill { get; set; }

	public string CloseButtonTip { get; set; }

	public double LeftBorderWidth { get; set; }

	public double LeftBorderHeight { get; set; }

	public BitmapData LeftBorderFill { get; set; }

	public double RightBorderWidth { get; set; }

	public double RightBorderHeight { get; set; }

	public double RightBorderLeft { get; set; }

	public double BottomBorderWidth { get; set; }

	public double BottomBorderHeight { get; set; }

	public double BottomBorderLeft { get; set; }

	public double BottomBorderTop { get; set; }

	public BitmapData BottomBorderFill { get; set; }

	public double LeftCornerWidth { get; set; }

	public double LeftCornerHeight { get; set; }

	public double LeftCornerLeft { get; set; }

	public double LeftCornerTop { get; set; }

	public BitmapData LeftCornerFill { get; set; }

	public double RightCornerWidth { get; set; }

	public double RightCornerHeight { get; set; }

	public double RightCornerLeft { get; set; }

	public double RightCornerTop { get; set; }

	public BitmapData RightCornerFill { get; set; }

	public Rectangle GetBoundsRect()
	{
		return new Rectangle(0, 0, 0, 0);
	}

	public Point MapPoint
	{
		get
		{
			return this._MapPoint;
		}
		set
		{
			this._MapPoint = value;
		}
	}

	public bool IsOutViewRange(Point p)
	{
		return this.MapPoint.X != 0 && this.MapPoint.Y != 0 && !Global.InCircle(p, this.MapPoint, (double)Global.Data.MaxUnWatchDistance);
	}

	public int DialogBoxReturn
	{
		get
		{
			return this._DialogBoxReturn;
		}
	}

	public void NotifyClose(int boxReturn)
	{
		this._DialogBoxReturn = boxReturn;
		if (this.ChildWindowClose != null)
		{
			if (this.ChildWindowClose(this, EventArgs.Empty))
			{
				this.Visibility = false;
				GChildWindow.ZZ--;
			}
		}
		else
		{
			this.Visibility = false;
		}
	}

	public void ClearChildWindowCloseEvents()
	{
		this.ChildWindowClose = null;
	}

	private double _BodyWidth;

	private double _BodyHeight;

	private Point _MapPoint = new Point(0, 0);

	public new ChildWindowCloseEventHandler ChildWindowClose;

	private int _DialogBoxReturn = -1;
}
