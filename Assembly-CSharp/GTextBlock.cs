using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.SilverLight;

public class GTextBlock : UserControl
{
	public void Init()
	{
		this.Container.Children.Add(this.ContentText);
		this.ContentText.background = false;
		this.ContentText.textColor = 16777215U;
	}

	protected override void InitializeComponent()
	{
		this.ContentText = base.GetComponent<TextBox>();
		this.Init();
	}

	public string font
	{
		get
		{
			return this.ContentText.FontFamily.FontName;
		}
	}

	public ImageBrush BodySource
	{
		get
		{
			return new ImageBrush(this._BodySource);
		}
		set
		{
			this.Container.Background = value;
			this._BodySource = value.ImageSource;
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
			this.ContentText.Width = value;
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
			this.ContentText.Height = value;
			this.Height = value;
		}
	}

	public bool Only
	{
		get
		{
			return this._Onlydouble;
		}
		set
		{
			this._Onlydouble = value;
			if (this._Onlydouble)
			{
				this.ContentText.restrict = "0-9";
			}
		}
	}

	public bool ReadOnly
	{
		get
		{
			return this._ReadOnly;
		}
		set
		{
			this._ReadOnly = value;
		}
	}

	public TextBox Text
	{
		get
		{
			return this.ContentText;
		}
		set
		{
			this.ContentText = value;
		}
	}

	public string EditText
	{
		get
		{
			return this.ContentText.Text;
		}
		set
		{
			this.setDefaultTextFormat();
			this.ContentText.Text = value;
			if (value == string.Empty || value == null)
			{
				this.tagArray.Clear();
			}
		}
	}

	public SolidColorBrush TextForeground
	{
		get
		{
			return this.ContentText.Foreground;
		}
		set
		{
			this.ContentText.Foreground = value;
		}
	}

	public int FontSize
	{
		get
		{
			return this.ContentText.FontSize;
		}
		set
		{
			this.ContentText.FontSize = value;
		}
	}

	private void ContentText_MouseDown(MouseEvent e)
	{
	}

	private void ContentText_TextChanged(object send, TextEvent e)
	{
		this.setDefaultTextFormat();
	}

	private void setDefaultTextFormat()
	{
	}

	private void ContentText_KeyDown(KeyboardEvent e)
	{
	}

	private string strDelete(string str, int startIndex, int endIndex)
	{
		string empty = string.Empty;
		string text = str.Substring(0, startIndex);
		string text2 = str.Substring(endIndex, str.Length);
		return text + text2;
	}

	public void addText(string str, object tag, uint color, bool underLine = true)
	{
		TextBox text = this.Text;
		text.text += str;
	}

	private void formatContentText()
	{
	}

	public void SetGoodsText(string goodsText, int offset)
	{
	}

	public string getTextContent()
	{
		return this.Text.text;
	}

	public string getAppendGoodsText(int addIndex = 0)
	{
		return this.Text.text;
	}

	public void Focus()
	{
		this.ContentText.Focus();
	}

	public EventHandler GotFocus
	{
		get
		{
			return this.ContentText.GotFocus;
		}
		set
		{
			this.ContentText.GotFocus = value;
		}
	}

	public EventHandler LostFocus
	{
		get
		{
			return this.ContentText.LostFocus;
		}
		set
		{
			this.ContentText.LostFocus = value;
		}
	}

	public SolidColorBrush TextFontColor { get; set; }

	public Thickness TextMargin { get; set; }

	public ImageBrush BodyBackground { get; set; }

	private TextBox ContentText;

	private BitmapData _BodySource;

	private bool _Onlydouble;

	private bool _ReadOnly;

	public List<TextBlockTag> tagArray = new List<TextBlockTag>();
}
