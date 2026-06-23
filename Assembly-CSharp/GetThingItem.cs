using System;
using HSGameEngine.GameEngine.SilverLight;

public class GetThingItem : UserControl
{
	protected override void InitializeComponent()
	{
		Canvas.SetLeft(this.LeftPanel, 4);
		Canvas.SetTop(this.LeftPanel, 4);
		this.LeftPanel.Orientation = global::Layout.Horizontal;
		this.Container.Children.Add(this.LeftPanel);
		this.Container.Children.Add(this.ItemNum);
		this.ItemNum.Text = string.Empty;
		this.ItemNum.FontSize = HSTextField.defaultFontSize;
		this.ItemNum.Foreground = new SolidColorBrush(4278382594U);
		Canvas.SetTop(this.ItemNum, 26);
		Canvas.SetLeft(this.ItemNum, 28);
		this.Container.Children.Add(this.RightPanel);
		Canvas.SetLeft(this.RightPanel, 8);
		Canvas.SetTop(this.RightPanel, 5);
		this.RightPanel.Children.Add(this.ItemName);
		this.ItemName.TextSize = (double)HSTextField.defaultFontSize;
		Canvas.SetTop(this.ItemName, 9);
		Canvas.SetLeft(this.ItemName, 22);
	}

	public double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
			this.LeftPanel.Height = value;
			this.RightPanel.Height = value;
		}
	}

	public double LeftPanelWidth
	{
		get
		{
			return this.LeftPanel.Width;
		}
		set
		{
			this.LeftPanel.Width = value;
			Canvas.SetLeft(this.RightPanel, value + 8.0);
		}
	}

	public double RightPanelWidth
	{
		get
		{
			return this.RightPanel.Width;
		}
		set
		{
			this.RightPanel.Width = value;
		}
	}

	public GGoodIcon GoodsIcon
	{
		set
		{
			this.LeftPanel.Children.Clear();
			this.LeftPanel.Children.Add(value);
		}
	}

	public string Text
	{
		set
		{
			this.ItemName.Text = value;
		}
	}

	public uint TextColor
	{
		set
		{
			this.ItemName.TextColor = new SolidColorBrush(value);
		}
	}

	public int DbID
	{
		get
		{
			return this._DbID;
		}
		set
		{
			this._DbID = value;
		}
	}

	private void UserControl_MouseEnter(object sender, MouseEvent e)
	{
	}

	private void UserControl_MouseLeave(object sender, MouseEvent e)
	{
	}

	private void UserControl_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected.Invoke(this, EventArgs.Empty);
		}
	}

	private StackPanel LeftPanel = new StackPanel();

	private TextBlock ItemNum = new TextBlock();

	private Canvas RightPanel = new Canvas();

	private GTextBlockOutLine ItemName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public EventHandler ItemSelected;

	public GIcon _GoodsIcon;

	private int _DbID;
}
