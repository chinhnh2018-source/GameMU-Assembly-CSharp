using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SystemErrorText : UserControl
{
	public static void Clear()
	{
		SystemErrorText.WaitingErrorTextQueue.Clear();
		SystemErrorText.SystemErrorTextQueue.Clear();
		SystemErrorText.SystemErrorTextList.Clear();
	}

	protected override void InitializeComponent()
	{
		if (SystemErrorText._ParentCanvas == null)
		{
			SystemErrorText._ParentCanvas = this;
			this._Bak.gameObject.SetActive(false);
			this._Label.gameObject.SetActive(false);
			this.Panel.clipping = 0;
		}
		else
		{
			this._Bak.gameObject.SetActive(true);
			this._Label.gameObject.SetActive(true);
		}
		if (null == this.Hint)
		{
			this.Hint = base.GetComponentInChildren<TextBlock>();
		}
	}

	protected virtual void OnEnable()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			SystemErrorText.RendErrTexts();
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
	}

	public UIPanel Panel
	{
		get
		{
			if (null == this._Panel)
			{
				this._Panel = base.GetComponent<UIPanel>();
			}
			return this._Panel;
		}
	}

	public double BodyWidth
	{
		get
		{
			return (double)this.Panel.clipRange.z;
		}
	}

	public double BodyHeight
	{
		get
		{
			return (double)this.Panel.clipRange.z;
		}
	}

	public double StartTicks
	{
		get
		{
			return this._StartTicks;
		}
		set
		{
			this._StartTicks = value;
		}
	}

	public Canvas RootCanvas
	{
		get
		{
			return this.Container;
		}
	}

	public Canvas ParentCanvas
	{
		get
		{
			return SystemErrorText._ParentCanvas;
		}
	}

	public void SetErrorText(uint color, string text)
	{
		this.Hint.FontSize = ((text.Length <= 30) ? 20 : 16);
		this.Hint.Text = text;
		this.StartTicks = (double)Global.GetCorrectLocalTime();
	}

	protected void OnTweenFinish(object param)
	{
		bool flag = true;
		if (param is TweenPosition)
		{
			TweenPosition tweenPosition = param as TweenPosition;
			TweenAlpha component = tweenPosition.GetComponent<TweenAlpha>();
			if (null != component)
			{
				component.enabled = true;
			}
			flag = false;
		}
		if (flag)
		{
			if (this.type == ShowGameInfoTypes.OnlySysHint)
			{
				SystemErrorText.NoticeCount--;
			}
			SystemErrorText.SystemErrorTextList.Remove(this);
			Object.Destroy(base.gameObject);
		}
	}

	private static void Reposition()
	{
		if (SystemErrorText.SystemErrorTextList.Count > 0)
		{
			SystemErrorText systemErrorText = SystemErrorText.SystemErrorTextList[0];
			systemErrorText.transform.localPosition = new Vector3(0f, 0f);
		}
	}

	public static void RendErrTexts()
	{
		double num = (double)Global.GetCorrectLocalTime();
		if (num - SystemErrorText.LastShowSystemErrorTextTicks < 500.0)
		{
			return;
		}
		SystemErrorText.LastShowSystemErrorTextTicks = num;
		if (SystemErrorText.WaitingErrorTextQueue.Count <= 0 && SystemErrorText.WaitingNoticeTextList.Count <= 0)
		{
			return;
		}
		ErrTextItem errTextItem = null;
		if (SystemErrorText.WaitingErrorTextQueue.Count > 0)
		{
			errTextItem = SystemErrorText.WaitingErrorTextQueue.Find((ErrTextItem item) => item.type != ShowGameInfoTypes.OnlySysHint);
			if (errTextItem == null && SystemErrorText.SystemErrorTextList.Count == 0)
			{
				errTextItem = SystemErrorText.WaitingErrorTextQueue[0];
			}
		}
		if (errTextItem == null)
		{
			return;
		}
		SystemErrorText.WaitingErrorTextQueue.Remove(errTextItem);
		if (errTextItem.type != ShowGameInfoTypes.OnlySysHint)
		{
			Super.HintMainText(errTextItem.Text, 10, 3);
			return;
		}
		SystemErrorText systemErrorText = SystemErrorText.PopupSystemErrorText();
		systemErrorText.SetErrorText(errTextItem.TextColor, errTextItem.Text);
		systemErrorText.type = errTextItem.type;
		SystemErrorText.SystemErrorTextList.Add(systemErrorText);
		SystemErrorText.Reposition();
		Point start = new Point((int)systemErrorText.BodyWidth / 2, 0);
		Point end = new Point((int)(-systemErrorText.BodyWidth / 2.0 - systemErrorText.Hint.ActualWidth), 0);
		SystemErrorText.MoveText(systemErrorText, start, end, 1, 1, (double)((start.X - end.X) / 52), 0f, 0f);
	}

	public static void MoveText(SystemErrorText flyingText, Point start, Point end, int zoomX, int zoomY, double unitCost, float delay = 0.5f, float alplaDelay = 0f)
	{
		bool flag = false;
		flyingText.Hint.transform.localPosition = new Vector3((float)start.X, (float)start.Y, -1f);
		TweenPosition componentInChildren = flyingText.GetComponentInChildren<TweenPosition>();
		if (null != componentInChildren && start != end)
		{
			componentInChildren.Reset();
			componentInChildren.from = new Vector3((float)start.X, (float)start.Y, -1f);
			componentInChildren.to = new Vector3((float)end.X, (float)end.Y, -1f);
			componentInChildren.delay = delay;
			componentInChildren.duration = (float)(Global.GetAnimationTimeConsuming(start, end, zoomX, zoomY, unitCost) / 1000.0);
			componentInChildren.style = 0;
			componentInChildren.enabled = true;
			flag = true;
			delay = 0f;
		}
		TweenAlpha componentInChildren2 = flyingText.GetComponentInChildren<TweenAlpha>();
		if (null != componentInChildren2)
		{
			componentInChildren2.Reset();
			componentInChildren2.delay = alplaDelay;
			if (!flag)
			{
				componentInChildren2.enabled = true;
			}
		}
	}

	public static void ShowErrorText(Canvas rootCanvas, int x, int y, uint color, string text, ShowGameInfoTypes type = ShowGameInfoTypes.OnlyErr)
	{
		ErrTextItem errTextItem = new ErrTextItem();
		errTextItem.X = x;
		errTextItem.Y = y;
		errTextItem.TextColor = color;
		errTextItem.Text = text;
		errTextItem.type = type;
		SystemErrorText.WaitingErrorTextQueue.Add(errTextItem);
		SystemHintWindow.AddErrorHintText(color, text);
	}

	public static void InitSystemErrorPart(Canvas canvas)
	{
		SystemErrorText systemErrorText = U3DUtils.NEW<SystemErrorText>();
		canvas.Children.Add(systemErrorText);
		Vector3 localPosition;
		localPosition..ctor(0f, (float)Global.GlobalMainWindow.ActualHeight * 0.3f, -9900f);
		systemErrorText.transform.localPosition = localPosition;
		U3DUtils.ReplaceLayerInChildren(systemErrorText.gameObject, LayerMask.NameToLayer("MUUI"), null);
	}

	private static SystemErrorText PopupSystemErrorText()
	{
		SystemErrorText systemErrorText = U3DUtils.NEW<SystemErrorText>();
		systemErrorText.X = 0.0;
		systemErrorText.Y = 0.0;
		if (null != SystemErrorText._ParentCanvas)
		{
			SystemErrorText._ParentCanvas.Children.Add(systemErrorText);
		}
		systemErrorText.gameObject.SetActive(true);
		return systemErrorText;
	}

	private static void PushSystemErrorText(SystemErrorText systemErrorText)
	{
		TweenPosition component = systemErrorText.gameObject.GetComponent<TweenPosition>();
		if (null != component)
		{
			component.enabled = false;
		}
		TweenAlpha component2 = systemErrorText.gameObject.GetComponent<TweenAlpha>();
		if (null != component2)
		{
			component2.enabled = false;
		}
		systemErrorText.gameObject.SetActive(false);
		SystemErrorText.SystemErrorTextQueue.Add(systemErrorText);
	}

	private const int MoveRate = 52;

	public UISprite _Bak;

	public TextBlock _Label;

	public TextBlock Hint;

	public float TickInterval = 0.2f;

	public ShowGameInfoTypes type;

	private static double LastShowSystemErrorTextTicks = 0.0;

	private static List<ErrTextItem> WaitingErrorTextQueue = new List<ErrTextItem>();

	private static List<SystemErrorText> SystemErrorTextQueue = new List<SystemErrorText>();

	private static List<SystemErrorText> SystemErrorTextList = new List<SystemErrorText>();

	private static List<SystemErrorText> WaitingNoticeTextList = new List<SystemErrorText>();

	private static int NoticeCount = 0;

	private UIPanel _Panel;

	private double _StartTicks;

	private static Canvas _ParentCanvas = null;
}
