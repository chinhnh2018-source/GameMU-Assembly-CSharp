using System;
using UnityEngine;

public class ContentPresenter : Sprite
{
	public object Content
	{
		set
		{
			this._Content = value;
			this._Canvas.Clear();
			this._Canvas.Children.Add(this._Content as GameObject);
		}
	}

	public SpriteSL getCanvas()
	{
		return this._Canvas;
	}

	public object _Content;

	public SpriteSL _Canvas = new SpriteSL();
}
