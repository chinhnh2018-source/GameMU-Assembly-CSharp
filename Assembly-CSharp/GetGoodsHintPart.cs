using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GetGoodsHintPart : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void AddTextItem(int key, string value)
	{
		if (null == this.m_GameObj)
		{
			return;
		}
		float num;
		if (!Super.HasGChildWindowShown())
		{
			num = 179f;
		}
		else
		{
			num = 0f;
		}
		Vector3 localPosition;
		localPosition..ctor(0f, num, -9900f);
		base.transform.localPosition = localPosition;
		TextAnim textAnim = U3DUtils.NEW<TextAnim>();
		if (null != textAnim)
		{
			if (null != textAnim.m_LblShowInfo)
			{
				textAnim.m_LblShowInfo.text = value;
			}
			textAnim.key = key;
			int count = this.TextAnimList.Count;
			List<TextAnim> list = new List<TextAnim>();
			TweenPosition tweenPosition;
			foreach (TextAnim textAnim2 in this.TextAnimList)
			{
				Vector3 localPosition2 = textAnim2.gameObject.transform.localPosition;
				Vector3 vector;
				vector..ctor(localPosition2.x, localPosition2.y + (float)GetGoodsHintPart.HEIGHT, localPosition2.z);
				tweenPosition = TweenPosition.Begin(textAnim2.gameObject, 0.3f, localPosition2, vector);
				tweenPosition.delay = 0f;
				tweenPosition.onFinished = new UITweener.OnFinished(this.OnTempPositionTweenComplete);
			}
			this.TextAnimList.Add(textAnim);
			U3DUtils.AddChild(this.m_GameObj.gameObject, textAnim.gameObject, true);
			tweenPosition = TweenPosition.Begin(textAnim.gameObject, 9f, new Vector3(0f, 400f, 0f));
			textAnim.TweenPositionDelay = 0f;
			textAnim.TweenPositionTime = 0f;
			tweenPosition.delay = 0.05f;
			tweenPosition.eventReceiver = base.gameObject;
			tweenPosition.onFinished = new UITweener.OnFinished(this.OnTweenComplete);
			textAnim.StartTime = Time.time;
		}
	}

	public void OnTempPositionTweenComplete(object _param)
	{
		TweenPosition tweenPosition = _param as TweenPosition;
		TextAnim component = tweenPosition.GetComponent<TextAnim>();
		if (component == null)
		{
			return;
		}
		float num = Time.time - component.StartTime;
		if (num < component.TweenPositionTime + component.TweenPositionDelay)
		{
			if (num < component.TweenPositionDelay)
			{
				tweenPosition = TweenPosition.Begin(component.gameObject, component.TweenPositionTime, new Vector3(0f, 400f, 0f));
				tweenPosition.delay = component.TweenPositionDelay - num;
				tweenPosition.onFinished = new UITweener.OnFinished(this.OnTweenComplete);
			}
			else
			{
				tweenPosition = TweenPosition.Begin(component.gameObject, component.TweenPositionTime + component.TweenPositionDelay - num, new Vector3(0f, 400f, 0f));
				tweenPosition.onFinished = new UITweener.OnFinished(this.OnTweenComplete);
			}
		}
		else
		{
			this.OnTweenComplete(_param);
		}
	}

	public void OnTweenComplete(object _param)
	{
		TweenPosition tweenPosition = _param as TweenPosition;
		TextAnim component = tweenPosition.GetComponent<TextAnim>();
		if (this.TextAnimList.Contains(component))
		{
			this.TextAnimList.Remove(component);
			component.Visibility = false;
			Object.Destroy(component.gameObject);
		}
	}

	public GameObject m_GameObj;

	public List<TextAnim> TextAnimList = new List<TextAnim>();

	private static int HEIGHT = 32;
}
