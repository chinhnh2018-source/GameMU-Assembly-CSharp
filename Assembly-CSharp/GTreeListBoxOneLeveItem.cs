using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GTreeListBoxOneLeveItem : GBasePart
{
	public GTreeListBoxOneLeveItem()
	{
		this.Init();
		this.Root = this.Container;
		this.ItemName.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 113, 167, 180));
		this.ItemName.Text = Global.GetLang("名称");
		base.addEventListener("mouseUp", new MouseEventHandler(this.mouseEventLeftButtonUp));
		base.addEventListener("ROLL_OVER", new MouseEventHandler(this.mouseEventLeftButtonOver));
		base.addEventListener("ROLL_OUT", new MouseEventHandler(this.mouseEventLeftButtonOut));
	}

	public void Init()
	{
		this.Container.Children.Add(this.SelectedRect);
		this.SelectedRect.Fill = new SolidColorBrush(865070U);
		this.SelectedRect.alpha = 0.2;
		Canvas.SetLeft(this.SelectedRect, -1);
		Canvas.SetTop(this.SelectedRect, -1);
		Canvas.SetZIndex(this.SelectedRect, -1.0);
		this.SelectedRect.Visibility = false;
		this.ItemName.Text = Global.GetLang("名称");
		Canvas.SetLeft(this.ItemName, 18);
		Canvas.SetTop(this.ItemName, 1);
		this.Container.Children.Add(this.ItemName);
	}

	public bool SelectedState
	{
		get
		{
			return this._SelectedState;
		}
		set
		{
			this._SelectedState = value;
			if (this._SelectedState)
			{
				this.SelectedRect.Visibility = true;
			}
			else
			{
				this.SelectedRect.Visibility = false;
			}
		}
	}

	public new double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
			this.SelectedRect.Width = value + 2.0;
		}
	}

	public new double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
			this.SelectedRect.Height = value;
		}
	}

	private void mouseEventLeftButtonUp(MouseEvent evt)
	{
		if (this.MouseLeftButtonUp != null)
		{
			this.MouseLeftButtonUp.Invoke(this, evt);
		}
	}

	private void mouseEventLeftButtonOver(MouseEvent evt)
	{
		if (this.MouseLeave != null)
		{
			this.MouseEnter.Invoke(this, evt);
		}
	}

	private void mouseEventLeftButtonOut(MouseEvent evt)
	{
		if (this.MouseEnter != null)
		{
			this.MouseLeave.Invoke(this, evt);
		}
	}

	public Canvas Root;

	public GTextBlockOutLine ItemName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private RectangleSL SelectedRect = new RectangleSL();

	public EventHandler MouseLeftButtonUp;

	public EventHandler MouseEnter;

	public EventHandler MouseLeave;

	private bool _SelectedState;
}
