using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class localMap : UserControl
{
	private new void Start()
	{
	}

	public float ScreenScaleX
	{
		get
		{
			if (this._ScreenScaleX <= 0f)
			{
				this._ScreenScaleX = (float)Screen.width / 960f;
			}
			return this._ScreenScaleX;
		}
		set
		{
			this._ScreenScaleX = value;
		}
	}

	public float ScreenScaleY
	{
		get
		{
			if (this._ScreenScaleY <= 0f)
			{
				this._ScreenScaleY = (float)Screen.height / 540f;
			}
			return this._ScreenScaleY;
		}
		set
		{
			this._ScreenScaleY = value;
		}
	}

	public float ScalingX
	{
		get
		{
			if (this._ScalingX <= 0f)
			{
				this._ScalingX = this.m_localMap.transform.localScale.x / ((float)Global.Data.GameScene.CurrentMapData.MapWidth / 100f);
			}
			return this._ScalingX;
		}
		set
		{
			this._ScalingX = value;
		}
	}

	public float ScalingY
	{
		get
		{
			if (this._scalingY <= 0f)
			{
				this._scalingY = this.m_localMap.transform.localScale.y / ((float)Global.Data.GameScene.CurrentMapData.MapHeight / 100f);
			}
			return this._scalingY;
		}
		set
		{
			this._scalingY = value;
		}
	}

	public void OnClick()
	{
		if (Global.Data.DisableAutoRoad)
		{
			return;
		}
		if (this.m_localMap.transform.gameObject.activeInHierarchy)
		{
			float x = this.m_localMap.transform.parent.localPosition.x + 480f;
			float y = this.m_localMap.transform.parent.localPosition.y + 270f;
			Vector3 point;
			point..ctor(UICamera.lastTouchPosition.x / this.ScreenScaleX, UICamera.lastTouchPosition.y / this.ScreenScaleY, 0f);
			point = this.RotatePoint(x, y, -45f, point);
			float num = point.x + this.m_localMap.transform.localScale.x / 2f;
			float num2 = point.y + this.m_localMap.transform.localScale.y / 2f;
			num *= (float)Global.Data.GameScene.CurrentMapData.MapWidth / this.m_localMap.transform.localScale.x;
			num2 *= (float)Global.Data.GameScene.CurrentMapData.MapHeight / this.m_localMap.transform.localScale.y;
			Point point2 = new Point((int)num, (int)num2);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = point2.X,
					IDType = point2.Y
				});
			}
		}
	}

	private Vector3 RotatePoint(float x, float y, float angle, Vector3 point)
	{
		Vector3 result;
		result..ctor(point.x -= x, point.y -= y, point.z);
		float x2 = Mathf.Cos(angle * 0.0174532924f) * point.x - Mathf.Sin(angle * 0.0174532924f) * point.y;
		float y2 = Mathf.Sin(angle * 0.0174532924f) * point.x + Mathf.Cos(angle * 0.0174532924f) * point.y;
		result.x = x2;
		result.y = y2;
		return result;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage m_localMap;

	private float _ScreenScaleX;

	private float _ScreenScaleY;

	private float _ScalingX;

	private float _scalingY;
}
