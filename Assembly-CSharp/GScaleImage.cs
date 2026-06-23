using System;
using HSGameEngine.GameEngine.SilverLight;

public class GScaleImage : Sprite
{
	public BitmapData Fill
	{
		set
		{
			while (base.numChildren > 0)
			{
				base.removeChildAt(base.numChildren - 1);
			}
		}
	}
}
