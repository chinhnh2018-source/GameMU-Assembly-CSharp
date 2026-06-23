using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class CAttribute : UserControl
{
	public new string Name
	{
		get
		{
			if (null != this._Name)
			{
				return this._Name.text;
			}
			return null;
		}
		set
		{
			if (null != this._Name)
			{
				this._Name.text = value;
			}
		}
	}

	public string Text
	{
		get
		{
			return this._Text.text;
		}
		set
		{
			this._Text.text = value;
		}
	}

	public long Value
	{
		set
		{
			this._Text.text = value.ToString();
		}
	}

	public long MaxValue
	{
		set
		{
			UILabel text = this._Text;
			text.text = text.text + "-" + value;
		}
	}

	public long DeltaValue
	{
		set
		{
			if (null != this._Icon)
			{
				if (value == 0L)
				{
					this._Icon.spriteName = "none";
					this._DeltaText.text = null;
				}
				else
				{
					if (value > 0L)
					{
						this._Icon.spriteName = "up";
					}
					else
					{
						this._Icon.spriteName = "down";
					}
					this._Icon.MakePixelPerfect();
					this._DeltaText.text = value.ToString();
				}
			}
		}
	}

	public int FontSize
	{
		set
		{
			Vector3 localScale;
			localScale..ctor((float)value, (float)value, 1f);
			if (null != this._Name)
			{
				this._Name.transform.localScale = localScale;
			}
			this._Text.transform.localScale = localScale;
			this._DeltaText.transform.localScale = localScale;
		}
	}

	public Color TextColor
	{
		set
		{
			this._Text.color = value;
		}
	}

	public uint textColor
	{
		set
		{
			this._Text.color = Colors.Uint2Color(value);
		}
	}

	public UILabel _Name;

	public UILabel _Text;

	public UISprite _Icon;

	public UILabel _DeltaText;
}
