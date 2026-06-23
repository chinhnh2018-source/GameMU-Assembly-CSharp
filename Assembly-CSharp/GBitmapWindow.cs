using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GBitmapWindow : UserControl
{
	public ImageURL WinBackgroundURL { get; set; }

	public SpriteSL Body
	{
		get
		{
			return this;
		}
	}

	public string TitleText { get; set; }

	public double WindowTitleLeft { get; set; }

	public double WindowTitleTop { get; set; }

	public int MessageBoxReturn
	{
		get
		{
			return this._MessageBoxReturn;
		}
	}

	public void SetContent(SpriteSL presenter, SpriteSL obj)
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

	public bool CloseButtonVisible { get; set; }

	public BitmapData CloseButtonFill { get; set; }

	public BitmapData CloseButtonTransformFill { get; set; }

	public string CloseButtonTip { get; set; }

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

	public void NotifyClose(int boxReturn)
	{
		this._MessageBoxReturn = boxReturn;
		if (this.ChildWindowClose != null)
		{
			if (this.ChildWindowClose(this, EventArgs.Empty))
			{
				this.Visibility = false;
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

	public Point WinTouchPos
	{
		set
		{
			this._WinTouchPos = value;
		}
	}

	private int _MessageBoxReturn = -1;

	private double _BodyWidth;

	private double _BodyHeight;

	private Point _MapPoint = new Point(0, 0);

	private Point _WinTouchPos;
}
