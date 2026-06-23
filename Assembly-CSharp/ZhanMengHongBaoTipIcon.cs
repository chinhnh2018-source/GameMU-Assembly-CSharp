using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengHongBaoTipIcon : UserControl
{
	private void OnEnable()
	{
		this.InitValue();
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		UIEventListener.Get(this.mPanel).onClick = delegate(GameObject s)
		{
			this.SendShowHongBaoRequest();
		};
	}

	public void SendShowHongBaoRequest()
	{
		if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || Global.GetMapSceneUIClass() == SceneUIClasses.Comp || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle)
		{
			Super.HintMainText(Global.GetLang("跨服主线和跨服活动中不能进行此操作"), 10, 3);
			return;
		}
		if (Global.IsHavingBangHui())
		{
			if (this.GetHongBaoCount() > 0)
			{
				int hongBaoID = this.GetHongBaoID();
				if (hongBaoID > 0)
				{
					GameInstance.Game.SendShowHongBaoRequest(this.GetHongBaoType(), hongBaoID, 1);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("红包都被抢光了"), 10, 3);
			}
			return;
		}
		Super.HintMainText(Global.GetLang("加入战盟才可以收发战盟红包"), 10, 3);
		if (Global.Data == null)
		{
			return;
		}
		Global.Data.mZhanMengHongBaoTipsData.Clear();
		if (base.gameObject.activeSelf)
		{
			this.NotifyInitUIDataByServerData();
			base.gameObject.SetActive(false);
		}
	}

	private void InitValue()
	{
		this.RedPacketsAsuramRecordMax = ConfigSystemParam.GetSystemParamByName("RedPacketsAsuramRecordMax", true);
		if (!string.IsNullOrEmpty(this.RedPacketsAsuramRecordMax))
		{
			this.maxCount_cfg = ConvertExt.SafeConvertToInt32(this.RedPacketsAsuramRecordMax);
		}
	}

	public void NotifyInitUIDataByServerData()
	{
		if (this.GetHongBaoCount() >= this.maxCount_cfg)
		{
			this.mNum.Text = this.maxCount_cfg.ToString();
		}
		else
		{
			this.mNum.Text = this.GetHongBaoCount().ToString();
		}
	}

	private int GetHongBaoCount()
	{
		if (Global.Data != null && Global.Data.mZhanMengHongBaoTipsData != null && Global.Data.mZhanMengHongBaoTipsData.Count > 0)
		{
			return Global.Data.mZhanMengHongBaoTipsData.Count;
		}
		return 0;
	}

	private int GetHongBaoID()
	{
		if (Global.Data != null && Global.Data.mZhanMengHongBaoTipsData != null && Global.Data.mZhanMengHongBaoTipsData.Count > 0)
		{
			return Global.Data.mZhanMengHongBaoTipsData[0].hongBaoID;
		}
		return 0;
	}

	private int GetHongBaoType()
	{
		if (Global.Data != null && Global.Data.mZhanMengHongBaoTipsData != null && Global.Data.mZhanMengHongBaoTipsData.Count > 0)
		{
			return Global.Data.mZhanMengHongBaoTipsData[0].type;
		}
		return 0;
	}

	protected override void OnDestroy()
	{
	}

	public void SetHintPanelPosition(Vector3 v)
	{
		this.hintPanel.transform.localPosition = v;
	}

	public TextBlock mNum;

	public GameObject mPanel;

	public UIPanel hintPanel;

	private string RedPacketsAsuramRecordMax = string.Empty;

	private int maxCount_cfg;
}
