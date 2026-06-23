using System;
using HSGameEngine.GameEngine.SilverLight;

public class ValidateCode : SpriteSL
{
	public ValidateCode(int type = 0, int length = 4)
	{
	}

	public string code
	{
		get
		{
			return this._code;
		}
	}

	public int type
	{
		get
		{
			return this._type;
		}
		set
		{
			this._type = value;
		}
	}

	public int length
	{
		get
		{
			return this._length;
		}
		set
		{
			this._length = value;
			this._bmd = new BitmapData((double)(this._oneCharWidth * this._length), (double)this._oneCharHeight, true, 0U);
			this.change(null);
		}
	}

	public string chars
	{
		get
		{
			return this._chars;
		}
		set
		{
			this._chars = value;
			this.change(null);
		}
	}

	public void change(MouseEvent e = null)
	{
		if (this.chars == string.Empty)
		{
			return;
		}
	}

	public string toString()
	{
		return string.Empty;
	}

	private string _code;

	private int _type;

	private string _chars = string.Empty;

	private int _length = 4;

	private BitmapData _bmd;

	private Bitmap _bmp;

	private int _oneCharWidth = 22;

	private int _oneCharHeight = 24;
}
