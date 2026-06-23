using System;
using HSGameEngine.GameEngine.SilverLight;

public class GBasePart : UserControl
{
	public double BodyWidth
	{
		get
		{
			return base.Width;
		}
		set
		{
			base.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return base.Height;
		}
		set
		{
			base.Height = value;
		}
	}

	public Brush BodyBackgournd
	{
		set
		{
			base.Background = value;
		}
	}

	public double BodyBackOpacity
	{
		set
		{
		}
	}

	protected virtual void InitControls()
	{
	}

	public virtual void InitPartSize(int width, int height)
	{
	}

	public virtual void InitPartData()
	{
	}

	public virtual void CleanUpChildWindows()
	{
	}
}
