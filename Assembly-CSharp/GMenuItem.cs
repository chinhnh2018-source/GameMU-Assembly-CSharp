using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class GMenuItem : UserControl
{
	public GMenuItem()
	{
		this.ItemText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		this.ItemText.TextColor = new SolidColorBrush(uint.MaxValue);
		this.ItemText.HorizontalAlignment = global::Layout.Center;
		this.ItemText.VerticalAlignment = global::Layout.Center;
		this.ItemText.IsHitTestVisible = false;
		this.ItemPanel.Children.Insert(1, this.ItemText);
		this.ItemPanel.Children.swapChildren(this.ItemText, this.GoodsImg);
		this.HoverRect.Visibility = false;
		this.HoverRect.Fill = GMenuItem.itemBak;
		this.thisCtrl = this;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.HoverRect);
		this.HoverRect.StrokeThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		this.HoverRect.Opacity = 0.8f;
		this.Container.Children.Add(this.BakPanel);
		this.BakPanel.Background = new SolidColorBrush(4286611584U);
		this.BakPanel.Opacity = 0.01;
		this.Container.Children.Add(this.ItemPanel);
		this.ItemPanel.Orientation = global::Layout.Horizontal;
		this.ItemPanel.Children.Add(this.GoodsImg);
		this.GoodsImg.Width = 40.0;
		this.GoodsImg.Height = 40.0;
		this.GoodsImg.Margin = new Thickness(0.0, 0.0, 5.0, 0.0);
		this.ItemPanel.Children.Add(this.ItemIcon);
		this.ItemIcon.Stretch = global::StretchSL.None;
		this.ItemIcon.HorizontalAlignment = global::Layout.Left;
		this.ItemIcon.VerticalAlignment = global::Layout.Center;
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.addEventListener("mouseUp", new MouseEventHandler(this.UserControl_MouseLeftButtonUp));
	}

	public int MenuItemID
	{
		get
		{
			return this._MenuItemID;
		}
		set
		{
			this._MenuItemID = value;
		}
	}

	public string MenuItemText
	{
		get
		{
			return this.ItemText.Text;
		}
		set
		{
			this.ItemText.Text = value;
		}
	}

	public BitmapData MenuItemIcon
	{
		get
		{
			return this.ItemIcon.Source.ImageSource;
		}
		set
		{
			this.ItemIcon.Source = new ImageBrush(value);
		}
	}

	public double Left
	{
		get
		{
			return Canvas.GetLeft(this.thisCtrl);
		}
		set
		{
			Canvas.SetLeft(this.thisCtrl, value);
		}
	}

	public double Top
	{
		get
		{
			return Canvas.GetTop(this.thisCtrl);
		}
		set
		{
			Canvas.SetTop(this.thisCtrl, value);
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.ItemPanel.Width;
		}
		set
		{
			ListBox itemPanel = this.ItemPanel;
			this.BakPanel.Width = value;
			itemPanel.Width = value;
			this.ItemText.TextWidth = this.ItemPanel.Width - 20.0;
			this.HoverRect.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.ItemPanel.Height;
		}
		set
		{
			ListBox itemPanel = this.ItemPanel;
			this.BakPanel.Height = value;
			itemPanel.Height = value;
			this.HoverRect.Height = value;
		}
	}

	public GIcon GoodImg
	{
		set
		{
			if (value != null)
			{
				this.GoodsImg.Children.Add(value);
				Canvas.SetLeft(value, 4);
				Canvas.SetTop(value, 4);
				Canvas.SetZIndex(value, 10.0);
			}
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.GoodsImg.Background = value;
		}
	}

	public double ImgWidth
	{
		get
		{
			return this.GoodsImg.Width;
		}
		set
		{
			this.GoodsImg.Width = value;
		}
	}

	public double ImgHeight
	{
		get
		{
			return this.GoodsImg.Height;
		}
		set
		{
			this.GoodsImg.Height = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			this.thisCtrl.Cursor = Cursors.Hand;
		}
		this.HoverRect.Visibility = true;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			this.thisCtrl.Cursor = Cursors.Auto;
		}
		this.HoverRect.Visibility = false;
	}

	private void UserControl_MouseLeftButtonUp(MouseEvent e)
	{
		if (this.MenuItemClick != null)
		{
			Super.GData.MenuItemMousePoint = new Point(e.stageX, e.stageY);
			this.MenuItemClick.Invoke(this.thisCtrl, e);
		}
	}

	private RectangleSL HoverRect = new RectangleSL();

	private Canvas BakPanel = new Canvas();

	private StackPanel ItemPanel = new StackPanel();

	private Canvas GoodsImg = new Canvas();

	private Image ItemIcon = new Image();

	private GTextBlockEx ItemText;

	private SpriteSL thisCtrl;

	private static ImageBrush itemBak = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 106.0, 21.0, 5.0, 5.0));

	private int _MenuItemID;

	public EventHandler MenuItemClick;
}
