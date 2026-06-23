using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIJoystick : TTMonoBehaviour
{
	private void Awake()
	{
		this.userInitTouchPos = Vector3.zero;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		base.StartCoroutine<bool>(this.fadeOutJoystick());
	}

	private IEnumerator fadeOutJoystick()
	{
		yield return new WaitForSeconds(this.fadeOutDelay);
		for (int i = 0; i < this.widgetsToFade.Length; i++)
		{
			UIWidget widget = this.widgetsToFade[i];
			Color lastColor = widget.color;
			Color newColor = lastColor;
			newColor.a = this.fadeOutAlpha;
			TweenColor.Begin(widget.gameObject, 0.5f, newColor).method = 2;
		}
		yield break;
	}

	public void OnPress(bool pressed)
	{
		if (this.target != null)
		{
			this.mPressed = pressed;
			if (pressed)
			{
				base.StopAllCoroutines();
				if (Time.time < this.lastTapTime + this.doubleTapTimeWindow)
				{
					if (this.doubleTapMessageTarget != null && this.doubleTabMethodeName != string.Empty)
					{
						this.doubleTapMessageTarget.SendMessage(this.doubleTabMethodeName, 1);
						this.tapCount++;
					}
					else
					{
						MUDebug.LogWarning<string>(new string[]
						{
							"Double Tab on Joystick but no Reciever or Methodename available"
						});
					}
				}
				else
				{
					this.tapCount = 1;
				}
				this.lastTapTime = Time.time;
				Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
				float num = 0f;
				Vector3 point = ray.GetPoint(num);
				point.z = 0f;
				if (this.centerOnPress)
				{
					this.userInitTouchPos = point;
					for (int i = 0; i < this.widgetsToFade.Length; i++)
					{
						TweenColor.Begin(this.widgetsToFade[i].gameObject, 0.1f, Color.white).method = 1;
					}
					for (int j = 0; j < this.widgetsToCenter.Length; j++)
					{
						this.widgetsToCenter[j].position = this.userInitTouchPos;
					}
				}
				else
				{
					this.userInitTouchPos = this.target.position;
					for (int k = 0; k < this.widgetsToFade.Length; k++)
					{
						TweenColor.Begin(this.widgetsToFade[k].gameObject, 0.1f, Color.white).method = 1;
					}
					this.OnDrag(Vector2.zero);
				}
			}
			else
			{
				this.ResetJoystick();
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
		float num = 0f;
		Vector3 point = ray.GetPoint(num);
		Vector3 vector = point - this.userInitTouchPos;
		if (vector.x != 0f || vector.y != 0f)
		{
			vector = this.target.InverseTransformDirection(vector);
			vector.Scale(this.scale);
			vector = this.target.TransformDirection(vector);
			vector.z = 0f;
		}
		this.target.position = this.userInitTouchPos + vector;
		Vector3 vector2 = this.target.position;
		vector2.z = 0f;
		this.target.position = vector2;
		float magnitude = this.target.localPosition.magnitude;
		this.target.localPosition = Vector3.ClampMagnitude(this.target.localPosition, this.radius);
		this.position = this.target.localPosition;
		if (this.normalize && this.radius != 0f)
		{
			this.position = this.position / this.radius * Mathf.InverseLerp(this.radius, this.deadZone, 1f);
		}
	}

	private void Update()
	{
		if (!UIJoystick.enumeratedJoysticks)
		{
			UIJoystick.joysticks = (Object.FindObjectsOfType(typeof(UIJoystick)) as UIJoystick[]);
			UIJoystick.enumeratedJoysticks = true;
		}
	}

	private void ResetJoystick()
	{
		this.tapCount = 0;
		this.position = Vector2.zero;
		this.target.position = this.userInitTouchPos;
		base.StartCoroutine<bool>(this.fadeOutJoystick());
	}

	public static void SetVisualMode(bool allwaysShow = false)
	{
		for (int i = 0; i < UIJoystick.joysticks.Length; i++)
		{
			UIJoystick uijoystick = UIJoystick.joysticks[i];
			uijoystick.fadeOutAlpha = ((!allwaysShow) ? 0.2f : 1f);
			uijoystick.joystickRing.gameObject.SetActive(allwaysShow);
			uijoystick.target.localPosition = Vector3.zero;
			uijoystick.StartCoroutine_Auto(uijoystick.fadeOutJoystick());
		}
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
		UIJoystick.enumeratedJoysticks = false;
	}

	private static UIJoystick[] joysticks;

	private static bool enumeratedJoysticks;

	public Transform joystickRing;

	public Transform target;

	public Vector3 scale = Vector3.one;

	public float radius = 40f;

	private bool mPressed;

	public bool centerOnPress = true;

	public Vector3 userInitTouchPos = Vector2.zero;

	public int tapCount;

	public bool normalize;

	public Vector2 position;

	public float deadZone = 2f;

	public float fadeOutAlpha = 0.2f;

	public float fadeOutDelay = 1f;

	public UIWidget[] widgetsToFade;

	public Transform[] widgetsToCenter;

	private float lastTapTime;

	public float doubleTapTimeWindow = 0.5f;

	public GameObject doubleTapMessageTarget;

	public string doubleTabMethodeName;
}
