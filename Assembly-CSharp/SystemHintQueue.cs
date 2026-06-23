using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SystemHintQueue : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitHintTypeLst();
		this.m_lstHitnData = new List<SystemHintData>();
		for (int i = 0; i < this.m_registeredHintType.Count; i++)
		{
			SystemHintData systemHintData = new SystemHintData();
			systemHintData.HintType = this.m_registeredHintType[i];
			systemHintData.BeActived = false;
			this.m_lstHitnData.Add(systemHintData);
			ActivityCustomTipManager.RegActivityTipListener((int)systemHintData.HintType, new CustomeActivityTipEventHandler(this.OnActivityStateChanged));
		}
		UIEventListener.Get(this.imgIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnCilkcIcon);
		base.gameObject.SetActive(false);
	}

	private void InitHintTypeLst()
	{
		this.m_registeredHintType = new List<ActivityTipTypes>();
		switch (this.hintIndex)
		{
		case SystemHintIndex.SystemHong:
			this.m_registeredHintType.Add(ActivityTipTypes.TeamCompeteRequestJoin);
			this.m_registeredHintType.Add(ActivityTipTypes.TeamCompeteInvite);
			this.m_registeredHintType.Add(ActivityTipTypes.ShenYou);
			this.m_registeredHintType.Add(ActivityTipTypes.MainEmailIcon);
			break;
		case SystemHintIndex.ChongZhi:
			this.m_registeredHintType.Add(ActivityTipTypes.ShouCiChongZhi);
			this.m_registeredHintType.Add(ActivityTipTypes.MeiRiChongZhi);
			this.m_registeredHintType.Add(ActivityTipTypes.LeiJiChongZhi);
			this.m_registeredHintType.Add(ActivityTipTypes.LeiJiXiaoFei);
			this.m_registeredHintType.Add(ActivityTipTypes.FuLiYueKaFanLi_Award);
			break;
		case SystemHintIndex.Login:
			this.m_registeredHintType.Add(ActivityTipTypes.FuLiLianXuDengLu);
			this.m_registeredHintType.Add(ActivityTipTypes.FuLiLeiJiDengLu);
			this.m_registeredHintType.Add(ActivityTipTypes.FuLiUpLevelGift);
			this.m_registeredHintType.Add(ActivityTipTypes.FuLiComBatGift);
			this.m_registeredHintType.Add(ActivityTipTypes.FuLiMeiRiHuoYue);
			break;
		}
	}

	private void OnActivityStateChanged(int type, bool beActive)
	{
		for (int i = this.m_lstHitnData.Count - 1; i >= 0; i--)
		{
			if (this.m_lstHitnData[i].HintType == (ActivityTipTypes)type)
			{
				this.m_lstHitnData[i].BeActived = beActive;
				break;
			}
		}
	}

	private new void OnDestroy()
	{
		for (int i = 0; i < this.m_lstHitnData.Count; i++)
		{
			ActivityCustomTipManager.UnRegActivityTipListener((int)this.m_lstHitnData[i].HintType, new CustomeActivityTipEventHandler(this.OnActivityStateChanged));
		}
		this.m_lstHitnData.Clear();
		this.m_lstHitnData = null;
	}

	public bool UpdateState()
	{
		if (this.m_lstHitnData == null)
		{
			return false;
		}
		bool result = false;
		if (!this.beCanShow())
		{
			if (base.gameObject.activeSelf)
			{
				result = true;
				base.gameObject.SetActive(false);
				this.m_currentHintData = null;
			}
			else if (!this.beCanShow())
			{
				this.m_currentHintData = null;
				result = false;
			}
			return result;
		}
		SystemHintData systemHintData = null;
		for (int i = 0; i < this.m_lstHitnData.Count; i++)
		{
			if (this.m_lstHitnData[i].BeActived)
			{
				systemHintData = this.m_lstHitnData[i];
				break;
			}
		}
		if (this.m_currentHintData != systemHintData)
		{
			if ((this.m_currentHintData != null && systemHintData == null) || (this.m_currentHintData == null && systemHintData != null))
			{
				result = true;
			}
			this.UpdateShowHint(systemHintData);
		}
		return result;
	}

	public bool BeShow()
	{
		return base.gameObject.activeSelf;
	}

	private void UpdateShowHint(SystemHintData showHint)
	{
		this.m_currentHintData = showHint;
		if (showHint == null)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
			this.imgIcon.spriteName = this.GetSpriteNameByHintType(showHint.HintType);
		}
	}

	private string GetSpriteNameByHintType(ActivityTipTypes type)
	{
		string result = string.Empty;
		switch (type)
		{
		case ActivityTipTypes.ShouCiChongZhi:
			result = "mainFirstChongzhi";
			break;
		case ActivityTipTypes.MeiRiChongZhi:
			result = "mainMeiriChongzhi";
			break;
		case ActivityTipTypes.LeiJiChongZhi:
			result = "mainLeiJiChongZhi";
			break;
		case ActivityTipTypes.LeiJiXiaoFei:
			result = "mainLeiJiXiaoFei";
			break;
		case ActivityTipTypes.FuLiMeiRiHuoYue:
			result = "mainMeiRiHuoYue";
			break;
		case ActivityTipTypes.FuLiLianXuDengLu:
			result = "mainLianXuDengLu";
			break;
		case ActivityTipTypes.FuLiLeiJiDengLu:
			result = "mainLeiJiDengLu";
			break;
		default:
			switch (type)
			{
			case ActivityTipTypes.TeamCompeteRequestJoin:
				result = "mainZhanDuiYaoQing";
				break;
			case ActivityTipTypes.TeamCompeteInvite:
				result = "mainZhanDuiYaoQing";
				break;
			case ActivityTipTypes.ShenYou:
				result = "mainMingXiang";
				break;
			default:
				if (type != ActivityTipTypes.MainEmailIcon)
				{
					if (type == ActivityTipTypes.ZiYuanZhaoHui)
					{
						result = "mainZiYaunZhaoHui";
					}
				}
				else
				{
					result = "mainEmail";
				}
				break;
			}
			break;
		case ActivityTipTypes.FuLiUpLevelGift:
			result = "mainDengJiJiangLi";
			break;
		case ActivityTipTypes.FuLiComBatGift:
			result = "mainZhanLiJiangLi";
			break;
		case ActivityTipTypes.FuLiYueKaFanLi_Award:
			result = "mainYueKaJiangLi";
			break;
		}
		return result;
	}

	private bool IsShowHintQueueIcon()
	{
		bool result = true;
		if (Global.IsCompetitionGuanKan || Global.IsInZhanMengLianSaiCompetetionMap() || Global.IsInKuaFuPlunderBattleMap() || Global.IsInKuaFuPlunderMainMap() || Global.IsInKuaFuTeamCompete() || Global.IsInDaTaoSha())
		{
			result = false;
		}
		return result;
	}

	private bool beCanShow()
	{
		return this.IsShowHintQueueIcon() && !Global.IsKuaFuMap(Global.Data.roleData.MapCode, false) && !Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) && Global.GetMapSceneUIClass() != SceneUIClasses.LangHunLingYu && !Global.IsInShiLiZhengBaMap() && !SceneUIClasses.RebornMap.IsTheScene();
	}

	private void OnCilkcIcon(GameObject go)
	{
		if (this.m_currentHintData == null)
		{
			return;
		}
		if (!this.beCanShow())
		{
			Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
			return;
		}
		ActivityTipTypes hintType = this.m_currentHintData.HintType;
		switch (hintType)
		{
		case ActivityTipTypes.ShouCiChongZhi:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(0, 0);
			break;
		case ActivityTipTypes.MeiRiChongZhi:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(0, 0);
			break;
		case ActivityTipTypes.LeiJiChongZhi:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(0, 0);
			break;
		case ActivityTipTypes.LeiJiXiaoFei:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(0, 0);
			break;
		case ActivityTipTypes.FuLiMeiRiHuoYue:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(2, 0);
			break;
		case ActivityTipTypes.FuLiLianXuDengLu:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(5, 0);
			break;
		case ActivityTipTypes.FuLiLeiJiDengLu:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(6, 0);
			break;
		default:
			switch (hintType)
			{
			case ActivityTipTypes.TeamCompeteRequestJoin:
				TeamCompeteDataManager.PopupRequestJoinTeamWindow();
				break;
			case ActivityTipTypes.TeamCompeteInvite:
				TeamCompeteDataManager.PopupConfirmInviteJoinTeamWindow();
				break;
			case ActivityTipTypes.ShenYou:
				PlayZone.GlobalPlayZone.OpenMingXiangShouYiWindow();
				break;
			default:
				if (hintType != ActivityTipTypes.MainEmailIcon)
				{
					if (hintType == ActivityTipTypes.ZiYuanZhaoHui)
					{
						PlayZone.GlobalPlayZone.ShowMeiRiBiZuoWindow(2);
					}
				}
				else
				{
					PlayZone.GlobalPlayZone.ShowEmailWindow(false, string.Empty);
				}
				break;
			}
			break;
		case ActivityTipTypes.FuLiUpLevelGift:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(3, 0);
			break;
		case ActivityTipTypes.FuLiComBatGift:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(4, 0);
			break;
		case ActivityTipTypes.FuLiYueKaFanLi_Award:
			PlayZone.GlobalPlayZone.OPenHuoDongWindow(1, 0);
			break;
		}
	}

	public UISprite imgIcon;

	public SystemHintIndex hintIndex;

	private List<ActivityTipTypes> m_registeredHintType;

	private SystemHintData m_currentHintData;

	private List<SystemHintData> m_lstHitnData;
}
