using System;
using System.Collections.Generic;
using UnityEngine;

public class SmartRect
{
	public bool CharInLink
	{
		get
		{
			return this._CharInLink;
		}
	}

	public string MessageStr
	{
		set
		{
			this._MessageStr = value;
		}
	}

	public void AddPoint(Vector3 point)
	{
		this.TempVertsList.Add(new Vector3(point.x, point.y, 0f));
	}

	public void Start()
	{
		this.TempVertsList.Clear();
		this._CharInLink = true;
		this._MessageStr = string.Empty;
	}

	private int GetChangeLineCount(List<Vector3> rectList)
	{
		if (0 >= rectList.Count)
		{
			return -1;
		}
		for (int i = 0; i < rectList.Count; i++)
		{
			if (i + 1 == rectList.Count)
			{
				return -1;
			}
			if (rectList[i + 1].x < rectList[i].x)
			{
				return i;
			}
		}
		return -1;
	}

	public bool End(ref List<RectContainer> rectList)
	{
		if (this.TempVertsList.Count == 0)
		{
			return false;
		}
		float x = this.TempVertsList[0].x;
		float y = this.TempVertsList[0].y;
		float x2 = this.TempVertsList[this.TempVertsList.Count - 1].x;
		float y2 = this.TempVertsList[this.TempVertsList.Count - 1].y;
		RectContainer rectContainer = new RectContainer();
		if (x2 < x)
		{
			int changeLineCount = this.GetChangeLineCount(this.TempVertsList);
			if (0 < changeLineCount)
			{
				rectContainer.rect.xMin = this.TempVertsList[0].x;
				rectContainer.rect.yMin = this.TempVertsList[0].y;
				rectContainer.rect.xMax = this.TempVertsList[changeLineCount].x;
				rectContainer.rect.yMax = this.TempVertsList[changeLineCount].y;
				rectContainer.message = this._MessageStr;
				rectList.Add(rectContainer);
				RectContainer rectContainer2 = new RectContainer();
				rectContainer2.rect.xMin = this.TempVertsList[changeLineCount + 1].x;
				rectContainer2.rect.yMin = this.TempVertsList[changeLineCount + 1].y;
				rectContainer2.rect.xMax = this.TempVertsList[this.TempVertsList.Count - 1].x;
				rectContainer2.rect.yMax = this.TempVertsList[this.TempVertsList.Count - 1].y;
				rectContainer2.message = this._MessageStr;
				rectList.Add(rectContainer2);
			}
		}
		else
		{
			rectContainer.rect.xMin = x;
			rectContainer.rect.xMax = x2;
			rectContainer.rect.yMin = y;
			rectContainer.rect.yMax = y2;
			rectContainer.message = this._MessageStr;
			rectList.Add(rectContainer);
		}
		this._CharInLink = false;
		return true;
	}

	private bool _CharInLink;

	private string _MessageStr;

	private List<Vector3> TempVertsList = new List<Vector3>();
}
