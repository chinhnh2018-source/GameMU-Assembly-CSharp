using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LivePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitIcon();
		this.InitEvent();
		int num = -1;
		if (this.trans.Length > 0)
		{
			for (int i = 0; i < this.trans.Length; i++)
			{
				if (this.trans[i].gameObject != null && this.trans[i].gameObject.activeSelf)
				{
					num++;
					if (num < this.pos.Length)
					{
						this.trans[i].transform.localPosition = this.pos[num];
					}
				}
			}
		}
	}

	private void InitIcon()
	{
		this.InitValue();
	}

	private void InitValue()
	{
		this.ChangeRecordVedioSprite = Global.Data.IsRecordVedio;
	}

	private void InitEvent()
	{
		if (this.mBtnVedio != null)
		{
			if (VideoSystem.GetInstance().IsActive())
			{
				this.mBtnVedio.gameObject.SetActive(true);
			}
			else
			{
				this.mBtnVedio.gameObject.SetActive(false);
			}
			UIEventListener.Get(this.mBtnVedio.gameObject).onClick = delegate(GameObject s)
			{
				GameInstance.Game.SendVideoOpenInfo();
			};
			this.WatchVideoIconShow(Global.IsWatchingVedio);
		}
		if (this.mBtnRecord != null)
		{
			if (RecordVideoSystem.GetInstance().IsActive())
			{
				this.mBtnRecord.gameObject.SetActive(true);
			}
			else
			{
				this.mBtnRecord.gameObject.SetActive(false);
			}
			UIEventListener.Get(this.mBtnRecord.gameObject).onClick = delegate(GameObject s)
			{
				if (!RecordVideoSystem.IsSupportReplay())
				{
					Super.HintMainText(Global.GetLang("当前设备暂不支持录屏功能"), 10, 3);
					return;
				}
				TimeSpan timeSpan = DateTime.Now - RecordVideoSystem.LastClickDateTime;
				if (timeSpan < TimeSpan.FromSeconds(1.0))
				{
				}
				RecordVideoSystem.LastClickDateTime = DateTime.Now;
				if (Global.Data.IsRecordVedio)
				{
					RecordVideoSystem.StopRecording();
					this.ChangeRecordVedioSprite = false;
					Super.ShowNetWaiting(null);
				}
				else
				{
					RecordVideoSystem.StartRecording();
					this.ChangeRecordVedioSprite = true;
					Super.ShowNetWaiting(null);
				}
			};
		}
	}

	public void SetRecordVedioActive(bool result)
	{
		if (result)
		{
			Global.Data.IsRecordVedio = true;
			this.ChangeRecordVedioSprite = Global.Data.IsRecordVedio;
			Super.HideNetWaiting();
		}
		else
		{
			Global.Data.IsRecordVedio = false;
			this.ChangeRecordVedioSprite = Global.Data.IsRecordVedio;
			Super.HideNetWaiting();
		}
	}

	private bool ChangeRecordVedioSprite
	{
		set
		{
			UISprite componentInChildren = this.mBtnRecord.GetComponentInChildren<UISprite>();
			if (componentInChildren != null)
			{
				componentInChildren.spriteName = ((!value) ? "ShiPinLuZhi" : "ShiPinLuZhi_DianLiang");
			}
		}
	}

	private void ChangeLiveSprite(bool liveOn)
	{
		UISprite componentInChildren = this.mBtnLive.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.spriteName = ((!liveOn) ? "liveOff" : "liveOn");
		}
	}

	public void WatchVideoIconShow(bool p)
	{
		if (this.mBtnVedio == null)
		{
			return;
		}
		if (p)
		{
			UISprite componentInChildren = this.mBtnVedio.GetComponentInChildren<UISprite>();
			if (componentInChildren != null)
			{
				componentInChildren.spriteName = "GuanKanShiPin_DianLiang";
			}
		}
		else
		{
			UISprite componentInChildren2 = this.mBtnVedio.GetComponentInChildren<UISprite>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.spriteName = "GuanKanShiPin";
			}
		}
	}

	public void SetLiveBtn(bool liveOn)
	{
		string spriteName = (!liveOn) ? "liveOff" : "liveOn";
		UISprite componentInChildren = this.mBtnLive.GetComponentInChildren<UISprite>();
		if (componentInChildren != null)
		{
			componentInChildren.spriteName = spriteName;
		}
	}

	public UIButton mBtnVedio;

	public UIButton mBtnLive;

	public UIButton mBtnRecord;

	private Vector3[] pos = new Vector3[]
	{
		new Vector3(-90f, 35f, 0f),
		new Vector3(-20f, 35f, 0f),
		new Vector3(50f, 35f, 0f)
	};

	public Transform[] trans;
}
