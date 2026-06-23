using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

[ExecuteInEditMode]
public class TextBlock : MonoBehaviour, ISilverLight
{
	private void Awake()
	{
		this.mTrans = base.transform;
		if (null != this.Label)
		{
			this.Label = base.GetComponent<UILabel>();
			this.Label.Margin = this._CharMargin;
			UILabel label = this.Label;
			label.Margin.y = label.Margin.y + (float)this.LineHeight;
		}
	}

	public double Width
	{
		get
		{
			return this._Width;
		}
		set
		{
			this._Width = value;
			this.UpdateLayout();
		}
	}

	public double Height
	{
		get
		{
			return this._Height;
		}
		set
		{
			this._Height = value;
			this.UpdateLayout();
		}
	}

	public double ActualWidth
	{
		get
		{
			return (double)((int)(this.Label.relativeSize.x * this.Label.transform.localScale.x));
		}
	}

	public double ActualHeight
	{
		get
		{
			return (double)((int)(this.Label.relativeSize.y * this.Label.transform.localScale.y));
		}
	}

	public double MaxWidth
	{
		set
		{
			this.Label.lineWidth = (int)value;
		}
	}

	public string Name
	{
		get
		{
			return base.name;
		}
		set
		{
			base.name = value;
		}
	}

	public bool Visibility
	{
		get
		{
			return this.Label.gameObject.activeSelf;
		}
		set
		{
			this.Label.gameObject.SetActive(value);
		}
	}

	public object Tag
	{
		get
		{
			return this._Tag;
		}
		set
		{
			this._Tag = value;
		}
	}

	public GameObject Parent
	{
		get
		{
			if (null != base.transform.parent)
			{
				return base.transform.parent.gameObject;
			}
			return null;
		}
		set
		{
			base.transform.parent = value.transform;
		}
	}

	public double X
	{
		get
		{
			return (double)base.transform.localPosition.x;
		}
		set
		{
			this._X = value;
			base.transform.localPosition = new Vector3((float)this._X, (float)this.Y, (float)this.Z);
		}
	}

	public double Y
	{
		get
		{
			return (double)base.transform.localPosition.y;
		}
		set
		{
			this._Y = value;
			base.transform.localPosition = new Vector3((float)this.X, (float)this._Y, (float)this.Z);
		}
	}

	public double Z
	{
		get
		{
			return (double)base.transform.localPosition.z;
		}
		set
		{
			this._Z = value;
			base.transform.localPosition = new Vector3((float)this.X, (float)this.Y, (float)this._Z);
		}
	}

	public string Text
	{
		get
		{
			return this.Label.text;
		}
		set
		{
			this.Label.text = value;
			this.UpdateLayout();
		}
	}

	public SolidColorBrush Background { get; set; }

	public SolidColorBrush Foreground
	{
		get
		{
			return new SolidColorBrush(Colors.Color2Uint(this.Label.color));
		}
		set
		{
			this.Label.color = Colors.Uint2Color(value.Color);
		}
	}

	public bool IsHitTestVisible { get; set; }

	public void UpdateLayout()
	{
		if (null != this.Label)
		{
			this.Label.Margin = this._CharMargin;
			UILabel label = this.Label;
			label.Margin.y = label.Margin.y + (float)this.LineHeight;
		}
	}

	public Thickness Margin
	{
		get
		{
			return this._Margin;
		}
		set
		{
			this._Margin = value;
			this.UpdateLayout();
		}
	}

	public UIWidget.Pivot Pivot
	{
		set
		{
			this.Label.pivot = value;
		}
	}

	public uint fontBorder
	{
		get
		{
			return (uint)NGUIMath.ColorToIntEx(this.Label.effectColor);
		}
		set
		{
			this.Label.effectStyle = 1;
			this.Label.effectColor = NGUIMath.HexToColorEx(value);
		}
	}

	public uint textColor
	{
		get
		{
			return (uint)NGUIMath.ColorToIntEx(this.Label.color);
		}
		set
		{
			this.Label.color = NGUIMath.HexToColorEx(value);
		}
	}

	public int FontSize
	{
		get
		{
			return (int)base.transform.localScale.x;
		}
		set
		{
			base.transform.localScale = new Vector3((float)value, (float)value, 0f);
		}
	}

	public SolidColorBrush CaretBrush { get; set; }

	public bool mouseEnabled { get; set; }

	public bool selectable { get; set; }

	public bool TextWrapping { get; set; }

	public bool AcceptsReturn { get; set; }

	public bool TextFontWrapping { get; set; }

	public bool wordWrap { get; set; }

	public bool multiline { get; set; }

	public bool visible { get; set; }

	public string VerticalAlignment { get; set; }

	public string HorizontalAlignment { get; set; }

	public int HorizontalScrollBarVisibility { get; set; }

	public int VerticalScrollBarVisibility { get; set; }

	public string text
	{
		get
		{
			return this.Label.text;
		}
		set
		{
			this.Label.text = value;
			this.UpdateLayout();
		}
	}

	public string htmlText
	{
		get
		{
			return this.text;
		}
		set
		{
			this.text = value;
		}
	}

	public TextFormat defaultTextFormat { get; set; }

	public void setTextFormat(TextFormat textFormat)
	{
	}

	public void addEventListener(string eventName, MouseEventHandler handler)
	{
	}

	public void removeEventListener(string eventName, MouseEventHandler handler)
	{
	}

	public int LineHeight;

	public UILabel Label;

	private Transform mTrans;

	[SerializeField]
	public Vector2 _CharMargin;

	protected double _Width = double.NaN;

	protected double _Height = double.NaN;

	private object _Tag;

	private double _X;

	private double _Y;

	private double _Z;

	private Thickness _Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
}
