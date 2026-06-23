using System;
using HSGameEngine.GameEngine.SilverLight;

public class GProgressBar : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.Rect_bak);
		this.Rect_bak.Visibility = false;
		this.Container.Children.Add(this.Rect1);
		this.Container.Children.Add(this.Rect2);
		this.Container.Children.Add(this.TextPanel);
		this.TextPanel.IsHitTestVisible = false;
		Canvas.SetZIndex(this.TextPanel, 100.0);
		this.TextPanel.Children.Add(this.ProgText);
		this.ProgText.HorizontalAlignment = global::Layout.Center;
		this.ProgText.VerticalAlignment = global::Layout.Center;
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
			this.Rect1.Width = value;
			this.Rect2.Width = 0.0;
			this.TextPanel.Width = value;
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
			this.Rect1.Height = value;
			this.Rect2.Height = value;
			this.TextPanel.Height = Math.Max(value, 18.0);
			Canvas.SetTop(this.TextPanel, (this.Height - this.TextPanel.Height) / 2.0 + 2.0);
		}
	}

	public uint BackColor
	{
		get
		{
			return this._BackColor;
		}
		set
		{
			this._BackColor = value;
			this.Rect1.Fill = new SolidColorBrush(this._BackColor);
		}
	}

	public Brush ForeBrush
	{
		set
		{
			this.Rect2.Fill = value;
		}
	}

	public Thickness StrokeThickness
	{
		get
		{
			return this.Rect1.StrokeThickness;
		}
		set
		{
			this.Rect1.StrokeThickness = value;
			this.Rect2.StrokeThickness = value;
		}
	}

	public uint Stroke
	{
		get
		{
			return this._StrokeColor;
		}
		set
		{
			this._StrokeColor = value;
			this.Rect1.Stroke = new SolidColorBrush(this._StrokeColor);
			this.Rect2.Stroke = new SolidColorBrush(this._StrokeColor);
		}
	}

	public double RadiusX
	{
		get
		{
			return this.Rect1.RadiusX;
		}
		set
		{
			this.Rect1.RadiusX = value;
			this.Rect2.RadiusX = value;
		}
	}

	public double RadiusY
	{
		get
		{
			return this.Rect1.RadiusY;
		}
		set
		{
			this.Rect1.RadiusY = value;
			this.Rect2.RadiusY = value;
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
			this.Rect2.Width = this.Rect1.Width * this._Percent;
		}
	}

	public string ProgessText
	{
		get
		{
			return this.ProgText.Text;
		}
		set
		{
			this.ProgText.Text = value;
		}
	}

	public SolidColorBrush ProgessTextColor
	{
		get
		{
			return this.ProgText.TextColor;
		}
		set
		{
			this.ProgText.TextColor = value;
		}
	}

	public double BakWidth
	{
		get
		{
			return this.Rect_bak.Width;
		}
		set
		{
			this.Rect_bak.Width = value;
		}
	}

	public double BakHeight
	{
		get
		{
			return this.Rect_bak.Height;
		}
		set
		{
			this.Rect_bak.Height = value;
		}
	}

	public double BakWith
	{
		get
		{
			return this.Rect_bak.Width;
		}
		set
		{
			this.Rect_bak.Width = value;
		}
	}

	public global::PointSL BakPosition
	{
		set
		{
		}
	}

	public Brush BakBrush
	{
		set
		{
			this.Rect_bak.Fill = value;
		}
	}

	public bool BakVisibility
	{
		set
		{
			this.Rect_bak.Visibility = value;
		}
	}

	private RectangleSL Rect_bak = new RectangleSL();

	private RectangleSL Rect1 = new RectangleSL();

	private RectangleSL Rect2 = new RectangleSL();

	private StackPanel TextPanel = new StackPanel();

	private GTextBlockOutLine ProgText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private uint _BackColor;

	private uint _StrokeColor;

	private double _Percent;
}
