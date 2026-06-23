using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GongnengIconPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.MingxiangIcon.gameObject.SetActive(GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MingXiang));
		if (Context.IsHaiwai)
		{
			this.SecondPasswordIcon.gameObject.SetActive(true);
			this.ShareIcon.gameObject.SetActive(false);
			this.GMIcon.gameObject.SetActive(false);
			this.GongGaoIcon.gameObject.SetActive(false);
		}
		UIEventListener.Get(this.MingxiangIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.EmailIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		ActivityTipManager.RegActivityTipItem(5002, delegate(int s, ActivityTipItem e)
		{
			this._EmailTipIcon.gameObject.SetActive(e.IsActive);
		});
		UIEventListener.Get(this.GMIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		};
		UIEventListener.Get(this.SettingIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3
			});
		};
		UIEventListener.Get(this.GuajiIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 4
			});
		};
		UIEventListener.Get(this.GongGaoIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 5
			});
		};
		UIEventListener.Get(this.SecondPasswordIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 6
			});
		};
		ActivityTipManager.RegActivityTipItem(5001, delegate(int s, ActivityTipItem e)
		{
			this._MingXiangTipIcon.gameObject.SetActive(e.IsActive);
		});
		this.GMIcon.gameObject.SetActive(true);
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(5001, null);
		ActivityTipManager.RegActivityTipItem(5002, null);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton MingxiangIcon;

	public UIButton EmailIcon;

	public UIButton GMIcon;

	public UIButton GuajiIcon;

	public UIButton SettingIcon;

	public UIButton SecondPasswordIcon;

	public UIButton GongGaoIcon;

	public UIButton ShareIcon;

	public Transform _MingXiangTipIcon;

	public Transform _EmailTipIcon;

	public UISprite Bak;
}
