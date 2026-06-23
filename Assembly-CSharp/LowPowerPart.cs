using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LowPowerPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (this.XScrollBar)
		{
			this.XScrollBar.onDragFinished = new UIScrollBar.OnDragFinished(this.OnDragFinishedHandler);
		}
		this.InitTextInPrefabs();
	}

	private void InitTextInPrefabs()
	{
		this.LabelUnlockTip.text = Global.GetLang("向右滑动解锁 》》");
	}

	private void OnDragFinishedHandler()
	{
		if ((double)this.XScrollBar.scrollValue > 0.9)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(this, new DPSelectedItemEventArgs());
			}
		}
		else if (this.XScrollBar.scrollValue > 0f && (double)this.XScrollBar.scrollValue <= 0.9)
		{
			this.IsTweeningScroll = true;
			this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = false;
		}
	}

	private void OnEnable()
	{
		int num = Random.Range(0, Super.AutoSystemChatItemsArray.Length);
		this.Tips.text = Super.AutoSystemChatItemsArray[num];
		this.XScrollBar.scrollValue = 0f;
		this.IsTweeningScroll = false;
		this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = true;
	}

	private new void Update()
	{
		if (this.IsTweeningScroll)
		{
			if (this.XScrollBar.scrollValue >= 0.005f)
			{
				this.XScrollBar.scrollValue = this.XScrollBar.scrollValue - this.XScrollBar.scrollValue / 3f;
			}
			else
			{
				this.XScrollBar.scrollValue = 0f;
				this.IsTweeningScroll = false;
				this.XScrollBar.foreground.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}

	public UILabel Tips;

	public UILabel LabelUnlockTip;

	public ShowNetImage Back;

	public UIScrollBar XScrollBar;

	public DPSelectedItemEventHandler CloseHandler;

	private bool IsTweeningScroll;
}
