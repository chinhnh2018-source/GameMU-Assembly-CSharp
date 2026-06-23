using System;
using HSGameEngine.GameEngine.SilverLight;

public class HintBlockDialog : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.RectFrame);
		this.RectFrame.StrokeThickness = new Thickness(0.0, 0.0, 0.0, 0.0);
		this.RectFrame.Stroke = new SolidColorBrush(uint.MaxValue);
		this.RectFrame.RadiusX = 10.0;
		this.RectFrame.RadiusY = 10.0;
		this.Container.Children.Add(this.Panel);
		this.Panel.Children.Add(this.Hint);
		this.Hint.Text = string.Empty;
		this.Hint.FontSize = HSTextField.defaultFontSize;
		this.Hint.Foreground = new SolidColorBrush(uint.MaxValue);
		this.Hint.TextWrapping = TextWrapping.Wrap;
		this.Hint.VerticalAlignment = global::Layout.Center;
		this.Hint.HorizontalAlignment = global::Layout.Center;
		this.Hint.Width = 400.0;
	}

	public DispatcherTimer Heart
	{
		get
		{
			return this._Heart;
		}
		set
		{
			this._Heart = value;
		}
	}

	public int MaxTickCounter
	{
		get
		{
			return this._MaxTickCounter;
		}
		set
		{
			this._MaxTickCounter = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
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
			this.Panel.Width = value;
			this.RectFrame.Width = value;
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
			this.Panel.Height = value;
			this.RectFrame.Width = value;
		}
	}

	public string Text
	{
		get
		{
			return this.Hint.Text;
		}
		set
		{
			this.Hint.Text = value;
		}
	}

	public Brush TextColor
	{
		set
		{
			this.Hint.Foreground = (value as SolidColorBrush);
		}
	}

	public int TextSize
	{
		set
		{
			this.Hint.FontSize = value;
		}
	}

	public void MyStart(int seconds)
	{
		if (this._Heart != null)
		{
			return;
		}
		this._Heart = new DispatcherTimer("HintBlockDialog");
		this._Heart.Interval = TimeSpan.FromSeconds((double)seconds);
		this._Heart.Start();
	}

	private void Timer_Tick(object sender, object e)
	{
		this.TickCounter++;
		if (this.TickCounter >= this.MaxTickCounter)
		{
			Canvas canvas = this.Parent.SafeGetComponent<Canvas>();
			this._Heart.Tick = null;
			this._Heart.Stop();
			this._Heart = null;
			this.Visibility = false;
			canvas.Children.Remove(this, true);
		}
	}

	private DispatcherTimer _Heart;

	private int TickCounter;

	private Canvas Root;

	private RectangleSL RectFrame = new RectangleSL();

	private StackPanel Panel = new StackPanel();

	private TextBlock Hint = new TextBlock();

	private int _MaxTickCounter;
}
