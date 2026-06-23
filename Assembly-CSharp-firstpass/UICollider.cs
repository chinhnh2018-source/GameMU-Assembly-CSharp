using System;
using UnityEngine;

public class UICollider : MonoBehaviour
{
	private void LateUpdate()
	{
		if (this.updataNow)
		{
			this.updataNow = false;
			this.UpdataCollider();
		}
	}

	public Vector3 BoxSize
	{
		get
		{
			return this.box.size;
		}
	}

	public void UpdataCollider()
	{
		if (null == base.gameObject)
		{
			return;
		}
		if (null == this.box)
		{
			this.box = base.gameObject.AddComponent<BoxCollider>();
		}
		int num = NGUITools.CalculateNextDepth(base.gameObject);
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.gameObject.transform);
		this.box.isTrigger = true;
		if (this.originalPointPos == UICollider.OriginalPointPos.TopLeft)
		{
			if (this.direction == UICollider.Direction.Vertical)
			{
				this.box.center = new Vector3(bounds.size.x / 2f, -(bounds.size.y / 2f + this.padding.y), 0f);
			}
			else if (this.direction == UICollider.Direction.Horizontal)
			{
				this.box.center = new Vector3(bounds.size.x / 2f + this.padding.x, bounds.size.y, 0f);
			}
			this.box.center += Vector3.back * ((float)num * 0.25f);
		}
		else if (this.originalPointPos == UICollider.OriginalPointPos.Center)
		{
			this.box.center = bounds.center + Vector3.back * ((float)num * 0.25f);
		}
		this.box.size = new Vector3(bounds.size.x, bounds.size.y, 0f);
	}

	public bool updataNow;

	public UICollider.OriginalPointPos originalPointPos;

	public UICollider.Direction direction = UICollider.Direction.Vertical;

	public Vector2 padding = Vector2.zero;

	public BoxCollider box;

	public enum OriginalPointPos
	{
		TopLeft,
		Center
	}

	public enum Direction
	{
		Horizontal,
		Vertical,
		Auto
	}
}
