using System;
using HSGameEngine.Drawing;

public class PointSL
{
	public PointSL(double x = 0.0, double y = 0.0)
	{
	}

	public double X
	{
		get
		{
			return 0.0;
		}
		set
		{
		}
	}

	public double Y
	{
		get
		{
			return 0.0;
		}
		set
		{
		}
	}

	public static implicit operator Point(PointSL pt)
	{
		return new Point((int)pt.X, (int)pt.Y);
	}
}
