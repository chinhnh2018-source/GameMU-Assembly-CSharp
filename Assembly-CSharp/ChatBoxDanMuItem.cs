using System;
using UnityEngine;

public class ChatBoxDanMuItem : UserControl
{
	public bool BeFree
	{
		get
		{
			return this.m_beFree;
		}
		set
		{
			this.m_beFree = value;
		}
	}

	public DanMuType DanMuType
	{
		get
		{
			return this.m_danMuType;
		}
		set
		{
			this.m_danMuType = value;
		}
	}

	public bool BeAllInScreen
	{
		get
		{
			return this.m_beAllInScreen;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	private void SetContent(string content)
	{
		this.lblcontent.text = content;
		this.lblcontent.depth = ChatBoxDanMuItem.m_index;
		ChatBoxDanMuItem.m_index++;
	}

	public void MoveText(string content, float width, Vector3 startPosition, float unitCost, float delay = 0.2f)
	{
		this.SetContent(content);
		this.m_unitCost = unitCost;
		this.m_startPosition = startPosition;
		float num = this.lblcontent.relativeSize.x * this.lblcontent.transform.localScale.x + 20f;
		this.m_textAllInPosition = new Vector3(this.m_startPosition.x - num, this.m_startPosition.y, this.m_startPosition.z);
		float num2 = num + width + 20f;
		this.m_endPosition = new Vector3(this.m_startPosition.x - num2, this.m_startPosition.y, this.m_startPosition.z);
		base.gameObject.transform.localPosition = this.m_startPosition;
		TweenPosition tweenPosition = base.gameObject.GetComponentInChildren<TweenPosition>();
		if (tweenPosition == null)
		{
			tweenPosition = base.gameObject.AddComponent<TweenPosition>();
		}
		tweenPosition.from = this.m_startPosition;
		tweenPosition.to = this.m_textAllInPosition;
		tweenPosition.Reset();
		tweenPosition.delay = delay;
		tweenPosition.duration = num / this.m_unitCost;
		tweenPosition.style = 0;
		tweenPosition.enabled = true;
		tweenPosition.onFinished = new UITweener.OnFinished(this.OnTweenStep1Finished);
	}

	private void OnTweenStep1Finished(UITweener tween)
	{
		if (this.OnMoveStep1Finish != null)
		{
			this.OnMoveStep1Finish.Invoke(this);
		}
		this.m_beAllInScreen = true;
		TweenPosition tweenPosition = base.gameObject.GetComponentInChildren<TweenPosition>();
		if (tweenPosition == null)
		{
			tweenPosition = base.gameObject.AddComponent<TweenPosition>();
		}
		tweenPosition.from = base.gameObject.transform.localPosition;
		tweenPosition.to = this.m_endPosition;
		tweenPosition.Reset();
		tweenPosition.delay = 0f;
		tweenPosition.duration = Mathf.Abs(this.m_endPosition.x - base.gameObject.transform.localPosition.x) / this.m_unitCost;
		tweenPosition.style = 0;
		tweenPosition.enabled = true;
		tweenPosition.onFinished = new UITweener.OnFinished(this.OnTweenStep2Finished);
	}

	private void OnTweenStep2Finished(UITweener tween)
	{
		if (this.OnMoveStep2Finish != null)
		{
			this.OnMoveStep2Finish.Invoke(this);
		}
	}

	public UILabel lblcontent;

	private static int m_index = 1;

	private Vector3 m_startPosition;

	private Vector3 m_endPosition;

	private Vector3 m_textAllInPosition;

	private float m_unitCost;

	private bool m_beFree;

	private DanMuType m_danMuType;

	private bool m_beAllInScreen;

	public Action<ChatBoxDanMuItem> OnMoveStep1Finish;

	public Action<ChatBoxDanMuItem> OnMoveStep2Finish;
}
