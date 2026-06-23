using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class CText : UserControl
{
	public new string Name
	{
		get
		{
			if (null != this._Name)
			{
				return this._Name.text;
			}
			if (null != this._Icon)
			{
				return this._Icon.spriteName;
			}
			return null;
		}
		set
		{
			if (null != this._Name)
			{
				this._Name.text = value;
			}
			else if (null != this._Icon)
			{
				this._Icon.spriteName = value;
				this._Icon.MakePixelPerfect();
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

	public int FontSize
	{
		set
		{
			if (null != this._Name)
			{
				this._Name.transform.localScale = new Vector3((float)value, (float)value, 1f);
			}
			this._Text.transform.localScale = new Vector3((float)value, (float)value, 1f);
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

	[NonSerialized]
	public int type = 1;
}
