using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class HuiGuiLeiJiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("累充奖励");
		this.lblTime.text = Global.GetLang("活动时间：");
		this.lblContent.text = Global.GetLang("活动描述：活动期间，累积充值达到要求可以领取丰厚奖励");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(this, null);
			}
		};
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		this.lblTime.text = Global.GetLang("活动时间：") + selfHuoDongLevel.BeginTime + " - " + selfHuoDongLevel.FinishTime;
		this.LoadRewardItems();
		this.SendHuiGuiZhiChongInfo();
	}

	private void LoadRewardItems()
	{
		this.m_lstItems.Clear();
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		List<MUHuiGuiChongZhiGift> chongZhiGiftByLevel = IConfigbase<ConfigHuiGuiHuoDong>.Instance.GetChongZhiGiftByLevel(selfHuoDongLevel.HuoDongLevel);
		for (int i = 0; i < chongZhiGiftByLevel.Count; i++)
		{
			HuiGuiLeiJiPartItem huiGuiLeiJiPartItem = U3DUtils.NEW<HuiGuiLeiJiPartItem>();
			huiGuiLeiJiPartItem.transform.SetParent(this.itemContainer);
			huiGuiLeiJiPartItem.transform.localScale = Vector3.one;
			huiGuiLeiJiPartItem.transform.localPosition = new Vector3(0f, 0f - 73f * (float)i, 0f);
			huiGuiLeiJiPartItem.ChongZhiGiftInfo = chongZhiGiftByLevel[i];
			huiGuiLeiJiPartItem.OnSelectItem = new Action<HuiGuiLeiJiPartItem>(this.OnClickItem);
			this.m_lstItems.Add(huiGuiLeiJiPartItem);
		}
	}

	public void OnClickItem(HuiGuiLeiJiPartItem item)
	{
		if (item.State == HuiGuiLeiJiRewardState.CanGet)
		{
			this.GetReward(item);
		}
		else if (item.State == HuiGuiLeiJiRewardState.CanNotGet)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(this, null);
			}
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		}
	}

	private HuiGuiLeiJiRewardState GetState(HuiGuiLeiJiPartItem item)
	{
		if (item.ChongZhiGiftInfo.MinYuanBao > this.m_leiJiChongZhi)
		{
			return HuiGuiLeiJiRewardState.CanNotGet;
		}
		if (this.m_lstHasGetIds.IndexOf(item.ChongZhiGiftInfo.ID) > -1)
		{
			return HuiGuiLeiJiRewardState.HasGet;
		}
		return HuiGuiLeiJiRewardState.CanGet;
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<int, List<int>>("CMD_SPR_REGRESSACTIVE_INPUTINFO", new Action<int, List<int>>(this.ServeGetHuiGuiZhiChongInfo));
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_INPUT", new Action(this.ServeLingQuZhiChong));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<int, List<int>>("CMD_SPR_REGRESSACTIVE_INPUTINFO", new Action<int, List<int>>(this.ServeGetHuiGuiZhiChongInfo));
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_INPUT", new Action(this.ServeLingQuZhiChong));
	}

	private void SendHuiGuiZhiChongInfo()
	{
		GameInstance.Game.SendGetHuiGuiChongZhiInfo();
	}

	private void ServeGetHuiGuiZhiChongInfo(int totalChongZhiNum, List<int> getIds)
	{
		this.m_leiJiChongZhi = totalChongZhiNum;
		this.m_lstHasGetIds = getIds;
		if (this.m_lstHasGetIds == null)
		{
			this.m_lstHasGetIds = new List<int>();
		}
		for (int i = 0; i < this.m_lstItems.Count; i++)
		{
			this.m_lstItems[i].SetRewardState(this.GetState(this.m_lstItems[i]));
		}
	}

	private void ServeLingQuZhiChong()
	{
		if (this.m_sendLeiJiPartItem != null)
		{
			this.m_sendLeiJiPartItem.SetRewardState(HuiGuiLeiJiRewardState.HasGet);
		}
	}

	private void GetReward(HuiGuiLeiJiPartItem chongZhiGift)
	{
		this.m_sendLeiJiPartItem = chongZhiGift;
		GameInstance.Game.SendHuiGuiLingQuChongZhi(chongZhiGift.ChongZhiGiftInfo.ID, HuiGuiData.GetSelfHuoDongLevel().HuoDongLevel);
	}

	private const float CellHeight = 73f;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTime;

	public UILabel lblContent;

	public GButton btnClose;

	public Transform itemContainer;

	private List<HuiGuiLeiJiPartItem> m_lstItems = new List<HuiGuiLeiJiPartItem>();

	private List<int> m_lstHasGetIds = new List<int>();

	private int m_leiJiChongZhi;

	private HuiGuiLeiJiPartItem m_sendLeiJiPartItem;
}
