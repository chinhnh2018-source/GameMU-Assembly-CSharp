using System;

public class RectBound
{
	public RectBound(float minX, float minY, float maxX, float maxY)
	{
		this.MinX = minX;
		this.MinY = minY;
		this.MaxX = maxX;
		this.MaxY = maxY;
	}

	public float MaxX;

	public float MaxY;

	public float MinX;

	public float MinY;
}
