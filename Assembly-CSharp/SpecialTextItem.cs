using System;
using HSGameEngine.GameEngine.SilverLight;

public class SpecialTextItem
{
	public string Text
	{
		get
		{
			return this._Text;
		}
		set
		{
			this._Text = value;
		}
	}

	public SolidColorBrush FontBrush
	{
		get
		{
			return this._FontBrush;
		}
		set
		{
			this._FontBrush = value;
		}
	}

	public bool UnderLine
	{
		get
		{
			return this._UnderLine;
		}
		set
		{
			this._UnderLine = value;
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

	private string _Text;

	private SolidColorBrush _FontBrush = new SolidColorBrush(4278190080U);

	private bool _UnderLine;

	private object _Tag;
}
