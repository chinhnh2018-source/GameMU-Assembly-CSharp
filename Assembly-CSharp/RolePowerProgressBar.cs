using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class RolePowerProgressBar : UserControl
{
	public RolePowerProgressBar()
	{
		this.Container.Width = 68.0;
		this.Container.Height = 68.0;
		this.BackgroundIcon = U3DUtils.NEW<GIcon>();
		this.BackgroundIcon.Width = 68.0;
		this.BackgroundIcon.Height = 68.0;
		this.BackgroundIcon.TipType = 7;
		this.BackgroundIcon.Tip = string.Empty;
		this.BackgroundIcon.DisableHandCursor = true;
		this.Container.Children.Add(this.BackgroundIcon);
		Canvas.SetTop(this.TextPanel, 25);
		this.TextPanel.Children.Add(this.ProgressValue);
		this.ProgressValue.IsHitTestVisible = false;
		this.ProgressValue.Height = 23.0;
		this.TextPanel.Width = 68.0;
		this.TextPanel.Height = 68.0;
		this.TextPanel.IsHitTestVisible = false;
		this.ProgressValue.TextColor = new SolidColorBrush(uint.MaxValue);
		this.ProgressValue.TextSize = (double)HSTextField.defaultFontSize;
		this.ProgressValue.HorizontalAlignment = global::Layout.Center;
		this.ProgressValue.VerticalAlignment = global::Layout.Center;
		this.ProgressValue.IsHitTestVisible = false;
		this.ProgressValue.Visibility = false;
		this.Container.Children.Add(this.TextPanel);
	}

	public double Left
	{
		get
		{
			return Canvas.GetLeft(this.Container);
		}
		set
		{
			Canvas.SetLeft(this.Container, value);
		}
	}

	public double Top
	{
		get
		{
			return Canvas.GetTop(this.Container);
		}
		set
		{
			Canvas.SetTop(this.Container, value);
		}
	}

	public int ZIndex
	{
		get
		{
			return (int)Canvas.GetZIndex(this.Container);
		}
		set
		{
			Canvas.SetZIndex(this.Container, (double)value);
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
		}
	}

	public GSpriteTypes SpriteType
	{
		get
		{
			return this._SpriteType;
		}
		set
		{
			this._SpriteType = value;
		}
	}

	public double ProgressTotalWidth
	{
		get
		{
			return this.Container.Width;
		}
	}

	public double ProgressHeight
	{
		get
		{
			return this._ProgressHeight;
		}
		set
		{
			this._ProgressHeight = Math.Min(this.Container.Height, value);
			this.DrawProgress();
		}
	}

	public string ProgressText
	{
		set
		{
			this.ProgressValue.Text = value;
		}
	}

	public string ProgressTip
	{
		set
		{
			this.BackgroundIcon.Tip = value;
		}
	}

	public double BackWidth
	{
		get
		{
			return this.Container.Width;
		}
	}

	private void DrawProgress()
	{
	}

	private StackPanel TextPanel = new StackPanel();

	private GTextBlockOutLine ProgressValue = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private BitmapData bmp = new BitmapData(68.0, 68.0, true, uint.MaxValue);

	private BitmapData bmp_full = new BitmapData(68.0, 68.0, true, uint.MaxValue);

	private GIcon BackgroundIcon;

	private double _ProgressHeight;

	private int _RoleID;

	private GSpriteTypes _SpriteType;
}
