using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class SwitchControl : UserControl
{
	public SwitchControl(bool changeDirection = false)
	{
		this.MyChangeDirection = changeDirection;
		this.thisCtrl = this;
	}

	public void Init()
	{
	}

	public bool ShowControl
	{
		get
		{
			return this._ShowControl;
		}
		set
		{
			this._ShowControl = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.InitControls();
	}

	public int SwitchWidth
	{
		set
		{
			this._width = value;
		}
	}

	public int SwitchHeight
	{
		set
		{
			this._height = value;
		}
	}

	public string ImgStrShowNormal
	{
		set
		{
			this._img1_normal = value;
		}
	}

	public string ImgStrShowHover
	{
		set
		{
			this._img1_hover = value;
		}
	}

	public string ImgStrHideNormal
	{
		set
		{
			this._img2_normal = value;
		}
	}

	public string ImgStrHideHover
	{
		set
		{
			this._img2_hover = value;
		}
	}

	protected void InitControls()
	{
		string uri = this._img1_normal;
		string uri2 = this._img1_hover;
		if (this.MyChangeDirection)
		{
			uri = this._img2_normal;
			uri2 = this._img2_hover;
		}
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = (double)this._width;
		gicon.Height = (double)this._height;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage(uri));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage(uri2));
		gicon.TipType = 0;
		gicon.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, 0);
		this.Container.Children.Add(gicon);
		this.ToLeftIcon = gicon;
		this.ToLeftIcon.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
		};
		this.ToLeftIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ToLeftIcon.Visibility = false;
			this.ToRightIcon.Visibility = true;
			if (this.toLeftDiection)
			{
				this._ShowControl = false;
			}
			else
			{
				this._ShowControl = true;
			}
			if (this.SwitchNotify != null)
			{
				this.SwitchNotify.Invoke(this.thisCtrl, EventArgs.Empty);
			}
		};
		uri = this._img2_normal;
		uri2 = this._img2_hover;
		if (this.MyChangeDirection)
		{
			uri = this._img1_normal;
			uri2 = this._img1_hover;
		}
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = (double)this._width;
		gicon.Height = (double)this._height;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage(uri));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage(uri2));
		gicon.TipType = 0;
		gicon.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(gicon, 0);
		Canvas.SetTop(gicon, 0);
		this.Container.Children.Add(gicon);
		this.ToRightIcon = gicon;
		this.ToRightIcon.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
		};
		this.ToRightIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ToLeftIcon.Visibility = true;
			this.ToRightIcon.Visibility = false;
			if (this.toLeftDiection)
			{
				this._ShowControl = true;
			}
			else
			{
				this._ShowControl = false;
			}
			if (this.SwitchNotify != null)
			{
				this.SwitchNotify.Invoke(this.thisCtrl, EventArgs.Empty);
			}
		};
		this.SwitchType(this.toLeftDiection);
	}

	public void SetSwitchTip(string left, string right)
	{
		this.ToLeftIcon.Tip = left;
		this.ToRightIcon.Tip = right;
	}

	public void SwitchType(bool toLeft)
	{
		this.toLeftDiection = toLeft;
		if (this.toLeftDiection)
		{
			if (this._ShowControl)
			{
				this.ToLeftIcon.Visibility = true;
				this.ToRightIcon.Visibility = false;
			}
			else
			{
				this.ToLeftIcon.Visibility = false;
				this.ToRightIcon.Visibility = true;
			}
		}
		else if (this._ShowControl)
		{
			this.ToLeftIcon.Visibility = false;
			this.ToRightIcon.Visibility = true;
		}
		else
		{
			this.ToLeftIcon.Visibility = true;
			this.ToRightIcon.Visibility = false;
		}
	}

	public EventHandler SwitchNotify;

	private GIcon ToLeftIcon;

	private GIcon ToRightIcon;

	private bool toLeftDiection = true;

	private bool _ShowControl = true;

	private SpriteSL thisCtrl;

	private Canvas Root;

	private bool MyChangeDirection;

	private int _width = 15;

	private int _height = 15;

	private string _img1_normal = "Images/Plate/show2_normal.png";

	private string _img1_hover = "Images/Plate/show2_hover.png";

	private string _img2_normal = "Images/Plate/collapse2_normal.png";

	private string _img2_hover = "Images/Plate/collapse2_hover.png";
}
