using System;

public class Sprite : SpriteSL
{
	public bool removeChildAt(int index)
	{
		return true;
	}

	public void swapChildrenAt(int src, int dec)
	{
	}

	public XGraphics graphics
	{
		get
		{
			return this._graphics;
		}
	}

	public double alpha
	{
		get
		{
			return this._alpha;
		}
		set
		{
			this._alpha = value;
		}
	}

	public bool visible
	{
		get
		{
			return this._visible;
		}
		set
		{
			this._visible = value;
		}
	}

	private XGraphics _graphics;

	private double _alpha;

	private bool _visible;
}
