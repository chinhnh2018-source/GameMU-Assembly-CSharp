using System;
using UnityEngine;

[RequireComponent(typeof(GUITexture))]
public class MPJoystick : MonoBehaviour
{
	private void Start()
	{
		if (this.m_fScreenWidth == 0f || this.m_fScreenHeight == 0f)
		{
			return;
		}
		float num = (float)Screen.width / this.m_fScreenWidth;
		float num2 = (float)Screen.height / this.m_fScreenHeight;
		this.gui = (GUITexture)base.GetComponent(typeof(GUITexture));
		this.gui.pixelInset = new Rect(0f, 0f, (float)((int)(num * (float)this.gui.texture.width)), (float)((int)(num2 * (float)this.gui.texture.height)));
		this.defaultRect = this.gui.pixelInset;
		this.defaultRect.x = this.defaultRect.x + base.transform.position.x * (float)Screen.width;
		this.defaultRect.y = this.defaultRect.y + base.transform.position.y * (float)Screen.height;
		base.transform.position = Vector3.zero;
		if (this.touchPad)
		{
			this.touchZone = this.defaultRect;
		}
		else
		{
			this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
			this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
			this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
			this.guiBoundary.min.x = this.defaultRect.x - this.guiTouchOffset.x;
			this.guiBoundary.max.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiBoundary.min.y = this.defaultRect.y - this.guiTouchOffset.y;
			this.guiBoundary.max.y = this.defaultRect.y + this.guiTouchOffset.y;
		}
	}

	public Vector2 getGUICenter()
	{
		return this.guiCenter;
	}

	private void Disable()
	{
		base.gameObject.SetActive(false);
	}

	private void ResetJoystick()
	{
		this.gui.pixelInset = this.defaultRect;
		this.lastFingerId = -1;
		this.position = Vector2.zero;
		this.fingerDownPos = Vector2.zero;
	}

	private bool IsFingerDown()
	{
		return this.lastFingerId != -1;
	}

	public void LatchedFinger(int fingerId)
	{
		if (this.lastFingerId == fingerId)
		{
			this.ResetJoystick();
		}
	}

	private void Update()
	{
		if (!MPJoystick.enumeratedJoysticks)
		{
			MPJoystick.joysticks = (MPJoystick[])Object.FindObjectsOfType(typeof(MPJoystick));
			MPJoystick.enumeratedJoysticks = true;
		}
		int touchCount = Input.touchCount;
		if (this.tapTimeWindow > 0f)
		{
			this.tapTimeWindow -= Time.deltaTime;
		}
		else
		{
			this.tapCount = 0;
		}
		if (touchCount == 0)
		{
			this.ResetJoystick();
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 vector = touch.position - this.guiTouchOffset;
				bool flag = false;
				if (this.touchPad)
				{
					if (this.touchZone.Contains(touch.position))
					{
						flag = true;
					}
				}
				else if (this.gui.HitTest(touch.position))
				{
					flag = true;
				}
				if (flag && (this.lastFingerId == -1 || this.lastFingerId != touch.fingerId))
				{
					if (this.touchPad)
					{
						this.lastFingerId = touch.fingerId;
					}
					this.lastFingerId = touch.fingerId;
					if (this.tapTimeWindow > 0f)
					{
						this.tapCount++;
					}
					else
					{
						this.tapCount = 1;
						this.tapTimeWindow = MPJoystick.tapTimeDelta;
					}
					for (int j = 0; j < MPJoystick.joysticks.Length; j++)
					{
						MPJoystick mpjoystick = MPJoystick.joysticks[j];
						if (mpjoystick != this)
						{
							mpjoystick.LatchedFinger(touch.fingerId);
						}
					}
				}
				if (this.lastFingerId == touch.fingerId)
				{
					if (touch.tapCount > this.tapCount)
					{
						this.tapCount = touch.tapCount;
					}
					if (this.touchPad)
					{
						this.position.x = Mathf.Clamp((touch.position.x - this.fingerDownPos.x) / (this.touchZone.width / 2f), -1f, 1f);
						this.position.y = Mathf.Clamp((touch.position.y - this.fingerDownPos.y) / (this.touchZone.height / 2f), -1f, 1f);
					}
					else
					{
						Rect pixelInset = this.gui.pixelInset;
						pixelInset.x = Mathf.Clamp(vector.x, this.guiBoundary.min.x, this.guiBoundary.max.x);
						pixelInset.y = Mathf.Clamp(vector.y, this.guiBoundary.min.y, this.guiBoundary.max.y);
						this.gui.pixelInset = pixelInset;
					}
					if (touch.phase == 3 || touch.phase == 4)
					{
						this.ResetJoystick();
					}
				}
			}
		}
		if (!this.touchPad && this.guiTouchOffset.x != 0f && this.guiTouchOffset.y != 0f)
		{
			this.position.x = (this.gui.pixelInset.x + this.guiTouchOffset.x - this.guiCenter.x) / this.guiTouchOffset.x;
			this.position.y = (this.gui.pixelInset.y + this.guiTouchOffset.y - this.guiCenter.y) / this.guiTouchOffset.y;
		}
		float num = Mathf.Abs(this.position.x);
		float num2 = Mathf.Abs(this.position.y);
		if (num < this.deadZone.x)
		{
			this.position.x = 0f;
		}
		else if (this.normalize)
		{
			this.position.x = Mathf.Sign(this.position.x) * (num - this.deadZone.x) / (1f - this.deadZone.x);
		}
		if (num2 < this.deadZone.y)
		{
			this.position.y = 0f;
		}
		else if (this.normalize)
		{
			this.position.y = Mathf.Sign(this.position.y) * (num2 - this.deadZone.y) / (1f - this.deadZone.y);
		}
	}

	private static MPJoystick[] joysticks;

	private static bool enumeratedJoysticks;

	private static float tapTimeDelta = 0.3f;

	public bool touchPad;

	public Vector2 position = Vector2.zero;

	public Rect touchZone;

	public Vector2 deadZone = Vector2.zero;

	public bool normalize;

	public int tapCount;

	private int lastFingerId = -1;

	private float tapTimeWindow;

	private Vector2 fingerDownPos;

	private GUITexture gui;

	private Rect defaultRect;

	private MPJoystick.Boundary guiBoundary = new MPJoystick.Boundary();

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;

	public float m_fScreenWidth = 800f;

	public float m_fScreenHeight = 450f;

	private class Boundary
	{
		public Vector2 min = Vector2.zero;

		public Vector2 max = Vector2.zero;
	}
}
