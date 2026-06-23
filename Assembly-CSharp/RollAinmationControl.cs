using System;
using System.Collections.Generic;
using UnityEngine;

public class RollAinmationControl : MonoBehaviour
{
	public float Duration
	{
		set
		{
			this.mDuration = value;
		}
	}

	private void Start()
	{
	}

	public void AddObj(Component obj)
	{
		if (null != obj)
		{
			this.mTrans.Add(obj.transform);
			if (this.mTrans.Count == 0)
			{
				this.tempPos = new Vector2(0f + this.Interval.x, 0f + this.Interval.y);
			}
			else
			{
				this.tempPos = new Vector2(this.tempPos.x + this.Interval.x, this.tempPos.y + this.Interval.y);
			}
			obj.transform.localPosition = this.tempPos;
		}
	}

	private void Update()
	{
		if (this.Stop)
		{
			return;
		}
		float num = this.Speed * Time.deltaTime * 5f;
		for (int i = 0; i < this.mTrans.Count; i++)
		{
			if (null != this.mTrans[i])
			{
				Vector3 localPosition = this.mTrans[i].localPosition;
				if (0f < this.Interval.x)
				{
					localPosition.x -= num;
					if (this.EndPos.x > localPosition.x)
					{
						localPosition.x = this.tempPos.x - this.Interval.x;
					}
				}
				if (0f < this.Interval.y)
				{
					localPosition.y -= num;
					if (this.EndPos.y > localPosition.y)
					{
						localPosition.y = this.tempPos.y - this.Interval.y;
					}
				}
				this.mTrans[i].localPosition = localPosition;
			}
		}
	}

	private List<Transform> mTrans = new List<Transform>();

	public Vector2 EndPos = Vector2.zero;

	public Vector2 Interval = Vector2.zero;

	public float Speed;

	private Vector2 tempPos = Vector3.zero;

	private float mDuration;

	public bool Stop;
}
