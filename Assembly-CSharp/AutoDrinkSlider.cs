using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;

public class AutoDrinkSlider : UserControl
{
	public AutoDrinkSlider()
	{
		this.Container.Width = 120.0;
		this.Container.Height = 10.0;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 4.0;
		gicon.Height = 12.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Plate/ProgressBarDrag_Normal.png", new object[0])));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/Plate/ProgressBarDrag_Hover.png", new object[0])));
		gicon.Cursor = Cursors.Hand;
		Canvas.SetLeft(gicon, this.Container.Width - gicon.Width);
		Canvas.SetTop(gicon, 0.0);
		this.Container.Children.Add(gicon);
		this.SliderIcon = gicon;
		gicon.addEventListener("mouseDown", new MouseEventHandler(this.SliderIcon_MouseDown));
		base.addEventListener("ADDED_TO_STAGE", new MouseEventHandler(this.onAddedToStage));
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
	}

	public int SliderTipType
	{
		set
		{
			if (null != this.SliderIcon)
			{
				this.SliderIcon.TipType = value;
			}
		}
	}

	public double Percent
	{
		get
		{
			return this._Percent;
		}
		set
		{
			this._Percent = value;
			if (this._Percent < 0.0)
			{
				this._Percent = 0.0;
			}
			if (this._Percent > 100.0)
			{
				this._Percent = 100.0;
			}
			if (null != this.SliderIcon)
			{
				this.SliderIcon.Tip = StringUtil.substitute("{0}%", new object[]
				{
					this._Percent
				});
				double value2 = (this.Container.Width - this.SliderIcon.Width) * (this._Percent / 100.0);
				Canvas.SetLeft(this.SliderIcon, value2);
			}
		}
	}

	private void onAddedToStage(MouseEvent e)
	{
		base.stage.addEventListener("mouseMove", new MouseEventHandler(this.SliderIcon_MouseMove));
		base.stage.addEventListener("mouseUp", new MouseEventHandler(this.SliderIcon_MouseUp));
	}

	private void SliderIcon_MouseUp(MouseEvent e)
	{
		if (this.isMouseCaptured)
		{
			this._Percent = Math.Floor(Canvas.GetLeft(this.SliderIcon) / (this.Root.Width - this.SliderIcon.Width) * 100.0);
			this.SliderIcon.Tip = StringUtil.substitute("{0}%", new object[]
			{
				this._Percent
			});
			if (this.PercentChanged != null)
			{
				this.PercentChanged.Invoke(this, EventArgs.Empty);
			}
			this.isMouseCaptured = false;
		}
	}

	private void SliderIcon_MouseDown(MouseEvent e)
	{
		this.clickPoint = new global::MousePosition(e).GetPosition(this);
		this.isMouseCaptured = true;
	}

	private void SliderIcon_MouseMove(MouseEvent e)
	{
		if (this.isMouseCaptured)
		{
			double num = Canvas.GetLeft(this.SliderIcon);
			Point position = new global::MousePosition(e).GetPosition(this);
			double num2 = (double)(position.X - this.clickPoint.X);
			num += num2;
			if (num < 0.0)
			{
				num = 0.0;
			}
			if (num > this.Container.Width - this.SliderIcon.Width)
			{
				num = this.Container.Width - this.SliderIcon.Width;
			}
			Canvas.SetLeft(this.SliderIcon, num);
			this.clickPoint = position;
		}
	}

	private bool isMouseCaptured;

	private GIcon SliderIcon;

	private double _Percent;

	private Canvas Root;

	public EventHandler PercentChanged;

	public Point clickPoint;
}
