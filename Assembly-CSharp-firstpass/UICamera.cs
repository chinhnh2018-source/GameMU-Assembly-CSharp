using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Camera"), RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	private bool handlesEvents
	{
		get
		{
			return UICamera.eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return UICamera.mSel;
		}
		set
		{
			if (UICamera.mSel != value)
			{
				if (UICamera.mSel != null)
				{
					UICamera uicamera = UICamera.FindCameraForLayer(UICamera.mSel.layer);
					if (uicamera != null)
					{
						UICamera.current = uicamera;
						UICamera.currentCamera = uicamera.mCam;
						UICamera.Notify(UICamera.mSel, "OnSelect", false);
						if (uicamera.useController || uicamera.useKeyboard)
						{
							UICamera.Highlight(UICamera.mSel, false);
						}
						UICamera.current = null;
					}
				}
				UICamera.mSel = value;
				if (UICamera.mSel != null)
				{
					UICamera uicamera2 = UICamera.FindCameraForLayer(UICamera.mSel.layer);
					if (uicamera2 != null)
					{
						UICamera.current = uicamera2;
						UICamera.currentCamera = uicamera2.mCam;
						if (uicamera2.useController || uicamera2.useKeyboard)
						{
							UICamera.Highlight(UICamera.mSel, true);
						}
						UICamera.Notify(UICamera.mSel, "OnSelect", true);
						UICamera.current = null;
					}
				}
			}
		}
	}

	public static int touchCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < UICamera.mTouches.Count; i++)
			{
				if (UICamera.mTouches[i].pressed != null)
				{
					num++;
				}
			}
			for (int j = 0; j < UICamera.mMouse.Length; j++)
			{
				if (UICamera.mMouse[j].pressed != null)
				{
					num++;
				}
			}
			if (UICamera.mController.pressed != null)
			{
				num++;
			}
			return num;
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < UICamera.mTouches.Count; i++)
			{
				if (UICamera.mTouches[i].dragged != null)
				{
					num++;
				}
			}
			for (int j = 0; j < UICamera.mMouse.Length; j++)
			{
				if (UICamera.mMouse[j].dragged != null)
				{
					num++;
				}
			}
			if (UICamera.mController.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	private void OnApplicationQuit()
	{
		UICamera.mHighlighted.Clear();
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != null)) ? null : eventHandler.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < UICamera.mList.Count; i++)
			{
				UICamera uicamera = UICamera.mList[i];
				if (!(uicamera == null) && uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
				{
					return uicamera;
				}
			}
			return null;
		}
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos, ref RaycastHit hit)
	{
		for (int i = 0; i < UICamera.mList.Count; i++)
		{
			UICamera uicamera = UICamera.mList[i];
			if (uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
			{
				UICamera.currentCamera = uicamera.cachedCamera;
				Vector3 vector = UICamera.currentCamera.ScreenToViewportPoint(inPos);
				if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
				{
					Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
					int num = UICamera.currentCamera.cullingMask & uicamera.eventReceiverMask;
					float num2 = (uicamera.rangeDistance <= 0f) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uicamera.rangeDistance;
					if (uicamera.clipRaycasts)
					{
						RaycastHit[] array = Physics.RaycastAll(ray, num2, num);
						if (array.Length > 1)
						{
							Array.Sort<RaycastHit>(array, (RaycastHit r1, RaycastHit r2) => r1.distance.CompareTo(r2.distance));
							int j = 0;
							int num3 = array.Length;
							while (j < num3)
							{
								if (UICamera.IsVisible(ref array[j]))
								{
									hit = array[j];
									return true;
								}
								j++;
							}
						}
						else if (array.Length == 1 && UICamera.IsVisible(ref array[0]))
						{
							hit = array[0];
							return true;
						}
					}
					else if (Physics.Raycast(ray, ref hit, num2, num))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool IsVisible(ref RaycastHit hit)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(hit.collider.gameObject);
		return uipanel == null || uipanel.IsVisible(hit.point);
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < UICamera.mList.Count; i++)
		{
			UICamera uicamera = UICamera.mList[i];
			Camera cachedCamera = uicamera.cachedCamera;
			if (cachedCamera != null && (cachedCamera.cullingMask & num) != 0)
			{
				return uicamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (UICamera.mNextEvent < realtimeSinceStartup)
		{
			float axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				UICamera.mNextEvent = realtimeSinceStartup + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				UICamera.mNextEvent = realtimeSinceStartup + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static bool IsHighlighted(GameObject go)
	{
		int i = UICamera.mHighlighted.Count;
		while (i > 0)
		{
			UICamera.Highlighted highlighted = UICamera.mHighlighted[--i];
			if (highlighted.go == go)
			{
				return true;
			}
		}
		return false;
	}

	private static void Highlight(GameObject go, bool highlighted)
	{
		if (go != null)
		{
			int i = UICamera.mHighlighted.Count;
			while (i > 0)
			{
				UICamera.Highlighted highlighted2 = UICamera.mHighlighted[--i];
				if (highlighted2 == null || highlighted2.go == null)
				{
					UICamera.mHighlighted.RemoveAt(i);
				}
				else if (highlighted2.go == go)
				{
					if (highlighted)
					{
						highlighted2.counter++;
					}
					else if (--highlighted2.counter < 1)
					{
						UICamera.mHighlighted.Remove(highlighted2);
						UICamera.Notify(go, "OnHover", false);
					}
					return;
				}
			}
			if (highlighted)
			{
				UICamera.Highlighted highlighted3 = new UICamera.Highlighted();
				highlighted3.go = go;
				highlighted3.counter = 1;
				UICamera.mHighlighted.Add(highlighted3);
				UICamera.Notify(go, "OnHover", true);
			}
		}
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (go != null)
		{
			go.SendMessage(funcName, obj, 1);
			if (UICamera.genericEventHandler != null && UICamera.genericEventHandler != go)
			{
				UICamera.genericEventHandler.SendMessage(funcName, obj, 1);
			}
		}
	}

	public static UICamera.MouseOrTouch GetTouch(int id)
	{
		UICamera.MouseOrTouch mouseOrTouch = null;
		if (!UICamera.mTouches.TryGetValue(id, ref mouseOrTouch))
		{
			mouseOrTouch = new UICamera.MouseOrTouch();
			mouseOrTouch.touchBegan = true;
			UICamera.mTouches.Add(id, mouseOrTouch);
		}
		return mouseOrTouch;
	}

	public static void RemoveTouch(int id)
	{
		UICamera.mTouches.Remove(id);
	}

	private void Awake()
	{
		this.cachedCamera.eventMask = 0;
		if (Application.platform == 11 || Application.platform == 8)
		{
			this.useMouse = false;
			this.useTouch = true;
			if (Application.platform == 8)
			{
				this.useKeyboard = false;
				this.useController = false;
			}
		}
		else if (Application.platform == 9 || Application.platform == 10)
		{
			this.useMouse = false;
			this.useTouch = false;
			this.useKeyboard = false;
			this.useController = true;
		}
		else if (Application.platform == 7 || Application.platform == null)
		{
			this.mIsEditor = true;
		}
		UICamera.mMouse[0].pos.x = Input.mousePosition.x;
		UICamera.mMouse[0].pos.y = Input.mousePosition.y;
		UICamera.lastTouchPosition = UICamera.mMouse[0].pos;
		if (this.eventReceiverMask == -1)
		{
			this.eventReceiverMask = this.cachedCamera.cullingMask;
		}
	}

	private void Start()
	{
		UICamera.mList.Add(this);
		UICamera.mList.Sort(new Comparison<UICamera>(UICamera.CompareFunc));
	}

	private void OnDestroy()
	{
		UICamera.mList.Remove(this);
	}

	private void FixedUpdate()
	{
		if (this.userMouseMock && Application.isPlaying && this.handlesEvents)
		{
			UICamera.hoveredObject = ((!UICamera.Raycast(InputMockManager.mousePosition, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			for (int i = 0; i < 3; i++)
			{
				UICamera.mMouse[i].current = UICamera.hoveredObject;
			}
		}
		else if (this.useMouse && Application.isPlaying && this.handlesEvents)
		{
			UICamera.hoveredObject = ((!UICamera.Raycast(Input.mousePosition, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			for (int j = 0; j < 3; j++)
			{
				UICamera.mMouse[j].current = UICamera.hoveredObject;
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying || !this.handlesEvents)
		{
			return;
		}
		UICamera.current = this;
		if (this.userMouseMock)
		{
			this.ProcessMockMouse();
		}
		else
		{
			if (this.useMouse || (this.useTouch && this.mIsEditor))
			{
				this.ProcessMouse();
			}
			if (this.useTouch)
			{
				this.ProcessTouches();
			}
		}
		if (UICamera.onCustomInput != null)
		{
			UICamera.onCustomInput();
		}
		if (this.useMouse && UICamera.mSel != null && ((this.cancelKey0 != null && Input.GetKeyDown(this.cancelKey0)) || (this.cancelKey1 != null && Input.GetKeyDown(this.cancelKey1))))
		{
			UICamera.selectedObject = null;
		}
		if (UICamera.mSel != null)
		{
			string text = Input.inputString;
			if (this.useKeyboard && Input.GetKeyDown(127))
			{
				text += "\b";
			}
			if (text.Length > 0)
			{
				if (!this.stickyTooltip && this.mTooltip != null)
				{
					this.ShowTooltip(false);
				}
				UICamera.Notify(UICamera.mSel, "OnInput", text);
			}
		}
		else
		{
			UICamera.inputHasFocus = false;
		}
		if (UICamera.mSel != null)
		{
			this.ProcessOthers();
		}
		if (this.useMouse && UICamera.mHover != null)
		{
			float axis = Input.GetAxis(this.scrollAxisName);
			if (axis != 0f)
			{
				UICamera.Notify(UICamera.mHover, "OnScroll", axis);
			}
			if (UICamera.showTooltips && this.mTooltipTime != 0f && (this.mTooltipTime < Time.realtimeSinceStartup || Input.GetKey(304) || Input.GetKey(303)))
			{
				this.mTooltip = UICamera.mHover;
				this.ShowTooltip(true);
			}
		}
		UICamera.current = null;
	}

	public void ProcessMouse()
	{
		bool flag = this.useMouse && Time.timeScale < 0.9f;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					flag = true;
					break;
				}
			}
		}
		UICamera.mMouse[0].pos = Input.mousePosition;
		UICamera.mMouse[0].delta = UICamera.mMouse[0].pos - UICamera.lastTouchPosition;
		bool flag2 = UICamera.mMouse[0].pos != UICamera.lastTouchPosition;
		UICamera.lastTouchPosition = UICamera.mMouse[0].pos;
		if (flag)
		{
			UICamera.hoveredObject = ((!UICamera.Raycast(Input.mousePosition, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			UICamera.mMouse[0].current = UICamera.hoveredObject;
		}
		for (int j = 1; j < 3; j++)
		{
			UICamera.mMouse[j].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[j].delta = UICamera.mMouse[0].delta;
			UICamera.mMouse[j].current = UICamera.mMouse[0].current;
		}
		bool flag3 = false;
		for (int k = 0; k < 3; k++)
		{
			if (Input.GetMouseButton(k))
			{
				flag3 = true;
				break;
			}
		}
		if (flag3)
		{
			this.mTooltipTime = 0f;
		}
		else if (this.useMouse && flag2 && (!this.stickyTooltip || UICamera.mHover != UICamera.mMouse[0].current))
		{
			if (this.mTooltipTime != 0f)
			{
				this.mTooltipTime = Time.realtimeSinceStartup + this.tooltipDelay;
			}
			else if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
		}
		if (this.useMouse && !flag3 && UICamera.mHover != null && UICamera.mHover != UICamera.mMouse[0].current)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.Highlight(UICamera.mHover, false);
			UICamera.mHover = null;
		}
		if (this.useMouse)
		{
			for (int l = 0; l < 3; l++)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(l);
				bool mouseButtonUp = Input.GetMouseButtonUp(l);
				UICamera.currentTouch = UICamera.mMouse[l];
				UICamera.currentTouchID = -1 - l;
				if (mouseButtonDown)
				{
					UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				}
				else if (UICamera.currentTouch.pressed != null)
				{
					UICamera.currentCamera = UICamera.currentTouch.pressedCam;
				}
				this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			}
			UICamera.currentTouch = null;
		}
		if (this.useMouse && !flag3 && UICamera.mHover != UICamera.mMouse[0].current)
		{
			this.mTooltipTime = Time.realtimeSinceStartup + this.tooltipDelay;
			UICamera.mHover = UICamera.mMouse[0].current;
			UICamera.Highlight(UICamera.mHover, true);
		}
	}

	public void ProcessMockMouse()
	{
		this.useMouse = true;
		bool flag = this.userMouseMock && Time.timeScale < 0.9f;
		if (!flag && (InputMockManager.GetMouseButton() || InputMockManager.GetMouseButtonUp()))
		{
			flag = true;
		}
		UICamera.mMouse[0].pos = InputMockManager.mousePosition;
		UICamera.mMouse[0].delta = UICamera.mMouse[0].pos - UICamera.lastTouchPosition;
		bool flag2 = UICamera.mMouse[0].pos != UICamera.lastTouchPosition;
		UICamera.lastTouchPosition = UICamera.mMouse[0].pos;
		if (flag)
		{
			UICamera.hoveredObject = ((!UICamera.Raycast(InputMockManager.mousePosition, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			UICamera.mMouse[0].current = UICamera.hoveredObject;
		}
		for (int i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[i].delta = UICamera.mMouse[0].delta;
			UICamera.mMouse[i].current = UICamera.mMouse[0].current;
		}
		bool flag3 = false;
		if (InputMockManager.GetMouseButton())
		{
			flag3 = true;
		}
		if (flag3)
		{
			this.mTooltipTime = 0f;
		}
		else if (this.userMouseMock && flag2 && (!this.stickyTooltip || UICamera.mHover != UICamera.mMouse[0].current))
		{
			if (this.mTooltipTime != 0f)
			{
				this.mTooltipTime = Time.realtimeSinceStartup + this.tooltipDelay;
			}
			else if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
		}
		if (this.userMouseMock && !flag3 && UICamera.mHover != null && UICamera.mHover != UICamera.mMouse[0].current)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.Highlight(UICamera.mHover, false);
			UICamera.mHover = null;
		}
		if (this.userMouseMock)
		{
			for (int j = 0; j < 1; j++)
			{
				bool mouseButtonDown = InputMockManager.GetMouseButtonDown();
				bool mouseButtonUp = InputMockManager.GetMouseButtonUp();
				UICamera.currentTouch = UICamera.mMouse[j];
				UICamera.currentTouchID = -1 - j;
				if (mouseButtonDown)
				{
					UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				}
				else if (UICamera.currentTouch.pressed != null)
				{
					UICamera.currentCamera = UICamera.currentTouch.pressedCam;
				}
				this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			}
			UICamera.currentTouch = null;
		}
		if (this.userMouseMock && !flag3 && UICamera.mHover != UICamera.mMouse[0].current)
		{
			this.mTooltipTime = Time.realtimeSinceStartup + this.tooltipDelay;
			UICamera.mHover = UICamera.mMouse[0].current;
			UICamera.Highlight(UICamera.mHover, true);
		}
	}

	public void ProcessTouches()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			UICamera.currentTouchID = ((!this.allowMultiTouch) ? 1 : touch.fingerId);
			UICamera.currentTouch = UICamera.GetTouch(UICamera.currentTouchID);
			bool flag = touch.phase == null || UICamera.currentTouch.touchBegan;
			bool flag2 = touch.phase == 4 || touch.phase == 3;
			UICamera.currentTouch.touchBegan = false;
			if (flag)
			{
				UICamera.currentTouch.delta = Vector2.zero;
			}
			else
			{
				UICamera.currentTouch.delta = touch.position - UICamera.currentTouch.pos;
			}
			UICamera.currentTouch.pos = touch.position;
			UICamera.hoveredObject = ((!UICamera.Raycast(UICamera.currentTouch.pos, ref UICamera.lastHit)) ? UICamera.fallThrough : UICamera.lastHit.collider.gameObject);
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			UICamera.currentTouch.current = UICamera.hoveredObject;
			UICamera.lastTouchPosition = UICamera.currentTouch.pos;
			if (flag)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			if (touch.tapCount > 1)
			{
				UICamera.currentTouch.clickTime = Time.realtimeSinceStartup;
			}
			this.ProcessTouch(flag, flag2);
			if (flag2)
			{
				UICamera.RemoveTouch(UICamera.currentTouchID);
			}
			UICamera.currentTouch = null;
			if (!this.allowMultiTouch)
			{
				break;
			}
		}
	}

	public void ProcessOthers()
	{
		UICamera.currentTouchID = -100;
		UICamera.currentTouch = UICamera.mController;
		UICamera.inputHasFocus = (UICamera.mSel != null && UICamera.mSel.GetComponent<UIInput>() != null);
		bool flag = (this.submitKey0 != null && Input.GetKeyDown(this.submitKey0)) || (this.submitKey1 != null && Input.GetKeyDown(this.submitKey1));
		bool flag2 = (this.submitKey0 != null && Input.GetKeyUp(this.submitKey0)) || (this.submitKey1 != null && Input.GetKeyUp(this.submitKey1));
		if (flag || flag2)
		{
			UICamera.currentTouch.current = UICamera.mSel;
			this.ProcessTouch(flag, flag2);
			UICamera.currentTouch.current = null;
		}
		int num = 0;
		int num2 = 0;
		if (this.useKeyboard)
		{
			if (UICamera.inputHasFocus)
			{
				num += UICamera.GetDirection(273, 274);
				num2 += UICamera.GetDirection(275, 276);
			}
			else
			{
				num += UICamera.GetDirection(119, 273, 115, 274);
				num2 += UICamera.GetDirection(100, 275, 97, 276);
			}
		}
		if (this.useController)
		{
			if (!string.IsNullOrEmpty(this.verticalAxisName))
			{
				num += UICamera.GetDirection(this.verticalAxisName);
			}
			if (!string.IsNullOrEmpty(this.horizontalAxisName))
			{
				num2 += UICamera.GetDirection(this.horizontalAxisName);
			}
		}
		if (num != 0)
		{
			UICamera.Notify(UICamera.mSel, "OnKey", (num <= 0) ? 274 : 273);
		}
		if (num2 != 0)
		{
			UICamera.Notify(UICamera.mSel, "OnKey", (num2 <= 0) ? 276 : 275);
		}
		if (this.useKeyboard && Input.GetKeyDown(9))
		{
			UICamera.Notify(UICamera.mSel, "OnKey", 9);
		}
		if (this.cancelKey0 != null && Input.GetKeyDown(this.cancelKey0))
		{
			UICamera.Notify(UICamera.mSel, "OnKey", 27);
		}
		if (this.cancelKey1 != null && Input.GetKeyDown(this.cancelKey1))
		{
			UICamera.Notify(UICamera.mSel, "OnKey", 27);
		}
		UICamera.currentTouch = null;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		bool flag = UICamera.currentTouch == UICamera.mMouse[0] || UICamera.currentTouch == UICamera.mMouse[1] || UICamera.currentTouch == UICamera.mMouse[2];
		float num = (!flag) ? this.touchDragThreshold : this.mouseDragThreshold;
		float num2 = (!flag) ? this.touchClickThreshold : this.mouseClickThreshold;
		if (pressed)
		{
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			UICamera.currentTouch.pressStarted = true;
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.dragged = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = ((!flag) ? UICamera.ClickNotification.Always : UICamera.ClickNotification.BasedOnDelta);
			UICamera.currentTouch.totalDelta = Vector2.zero;
			UICamera.currentTouch.dragStarted = false;
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
			if (UICamera.currentTouch.pressed != UICamera.mSel)
			{
				if (this.mTooltip != null)
				{
					this.ShowTooltip(false);
				}
				UICamera.selectedObject = null;
			}
		}
		else
		{
			if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && !this.stickyPress && !unpressed && UICamera.currentTouch.pressStarted && UICamera.currentTouch.pressed != UICamera.hoveredObject)
			{
				UICamera.isDragging = true;
				UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
				UICamera.currentTouch.pressed = UICamera.hoveredObject;
				UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
				UICamera.isDragging = false;
			}
			if (UICamera.currentTouch.pressed != null)
			{
				float magnitude = UICamera.currentTouch.delta.magnitude;
				if (magnitude != 0f)
				{
					UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
					magnitude = UICamera.currentTouch.totalDelta.magnitude;
					if (!UICamera.currentTouch.dragStarted && num < magnitude)
					{
						UICamera.currentTouch.dragStarted = true;
						UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
					}
					if (UICamera.currentTouch.dragStarted)
					{
						if (this.mTooltip != null)
						{
							this.ShowTooltip(false);
						}
						UICamera.isDragging = true;
						bool flag2 = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
						UICamera.Notify(UICamera.currentTouch.dragged, "OnDrag", UICamera.currentTouch.delta);
						UICamera.isDragging = false;
						if (flag2)
						{
							UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
						}
						else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta && num2 < magnitude)
						{
							UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
						}
					}
				}
			}
		}
		if (unpressed)
		{
			UICamera.currentTouch.pressStarted = false;
			if (this.mTooltip != null)
			{
				this.ShowTooltip(false);
			}
			if (UICamera.currentTouch.pressed != null)
			{
				UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
				if (this.useMouse && UICamera.currentTouch.pressed == UICamera.mHover)
				{
					UICamera.Notify(UICamera.currentTouch.pressed, "OnHover", true);
				}
				if (UICamera.currentTouch.dragged == UICamera.currentTouch.current || (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.totalDelta.magnitude < num))
				{
					if (UICamera.currentTouch.pressed != UICamera.mSel)
					{
						UICamera.mSel = UICamera.currentTouch.pressed;
						UICamera.Notify(UICamera.currentTouch.pressed, "OnSelect", true);
					}
					else
					{
						UICamera.mSel = UICamera.currentTouch.pressed;
					}
					if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None)
					{
						float realtimeSinceStartup = Time.realtimeSinceStartup;
						UICamera.Notify(UICamera.currentTouch.pressed, "OnClick", null);
						if (UICamera.currentTouch.clickTime + 0.35f > realtimeSinceStartup)
						{
							UICamera.Notify(UICamera.currentTouch.pressed, "OnDoubleClick", null);
						}
						UICamera.currentTouch.clickTime = realtimeSinceStartup;
					}
				}
				else
				{
					UICamera.Notify(UICamera.currentTouch.current, "OnDrop", UICamera.currentTouch.dragged);
				}
			}
			UICamera.currentTouch.dragStarted = false;
			UICamera.currentTouch.pressed = null;
			UICamera.currentTouch.dragged = null;
		}
	}

	public void ShowTooltip(bool val)
	{
		this.mTooltipTime = 0f;
		UICamera.Notify(this.mTooltip, "OnTooltip", val);
		if (!val)
		{
			this.mTooltip = null;
		}
	}

	public bool debug;

	public bool userMouseMock;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyPress = true;

	public LayerMask eventReceiverMask = -1;

	public bool clipRaycasts = true;

	public float tooltipDelay = 1f;

	public bool stickyTooltip = true;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = 13;

	public KeyCode submitKey1 = 330;

	public KeyCode cancelKey0 = 27;

	public KeyCode cancelKey1 = 331;

	public static UICamera.OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static int currentTouchID = -1;

	public static UICamera.MouseOrTouch currentTouch = null;

	public static bool inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static List<UICamera> mList = new List<UICamera>();

	private static List<UICamera.Highlighted> mHighlighted = new List<UICamera.Highlighted>();

	private static GameObject mSel = null;

	private static UICamera.MouseOrTouch[] mMouse = new UICamera.MouseOrTouch[]
	{
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch()
	};

	private static GameObject mHover;

	private static UICamera.MouseOrTouch mController = new UICamera.MouseOrTouch();

	private static float mNextEvent = 0f;

	private static Dictionary<int, UICamera.MouseOrTouch> mTouches = new Dictionary<int, UICamera.MouseOrTouch>();

	private GameObject mTooltip;

	private Camera mCam;

	private LayerMask mLayerMask;

	private float mTooltipTime;

	private bool mIsEditor;

	public static bool isDragging = false;

	public static GameObject hoveredObject;

	public enum ClickNotification
	{
		None,
		Always,
		BasedOnDelta
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float clickTime;

		public UICamera.ClickNotification clickNotification = UICamera.ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;
	}

	private class Highlighted
	{
		public GameObject go;

		public int counter;
	}

	public delegate void OnCustomInput();
}
