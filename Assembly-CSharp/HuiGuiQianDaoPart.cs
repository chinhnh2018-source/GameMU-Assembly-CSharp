using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class HuiGuiQianDaoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("每日签到");
		this.lblTime.text = Global.GetLang("活动时间：");
		this.lblContent.text = Global.GetLang("活动描述：活动期间，每日上线签到可以领取丰厚奖励");
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
		this.LoadQianDaoItems();
		this.SendGetQianDaoInfo();
	}

	private void LoadQianDaoItems()
	{
		this.lstItems.Clear();
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		int totalLoginDayNum = IConfigbase<ConfigHuiGuiHuoDong>.Instance.GetTotalLoginDayNum();
		int num = 0;
		for (int i = 1; i <= totalLoginDayNum; i++)
		{
			MUHuiGuiLoginNumGift loginNumGiftByLevelAndDay = IConfigbase<ConfigHuiGuiHuoDong>.Instance.GetLoginNumGiftByLevelAndDay(selfHuoDongLevel.HuoDongLevel, i);
			if (loginNumGiftByLevelAndDay == null)
			{
				MUDebug.Log<string>(new string[]
				{
					"MUHuiGuiLoginNumGift DayNum " + i + "数据不存在"
				});
			}
			else
			{
				HuiGuiQianDaoPartItem huiGuiQianDaoPartItem = U3DUtils.NEW<HuiGuiQianDaoPartItem>();
				huiGuiQianDaoPartItem.transform.SetParent(this.itemContainer);
				huiGuiQianDaoPartItem.transform.localScale = Vector3.one;
				huiGuiQianDaoPartItem.transform.localPosition = new Vector3(0f, 0f - 73f * (float)num, 0f);
				num++;
				huiGuiQianDaoPartItem.LoginGiftInfo = loginNumGiftByLevelAndDay;
				HuiGuiQianDaoState itemState = this.GetItemState(i, HuiGuiData.cacheLoginInfo);
				huiGuiQianDaoPartItem.SetQianDaoState(itemState);
				huiGuiQianDaoPartItem.OnSelectItem = new Action<HuiGuiQianDaoPartItem>(this.OnClickItem);
				this.lstItems.Add(huiGuiQianDaoPartItem);
			}
		}
	}

	public void OnClickItem(HuiGuiQianDaoPartItem item)
	{
		if (item.State == HuiGuiQianDaoState.CanQianDao)
		{
			this.QianDao(item);
		}
	}

	private HuiGuiQianDaoState GetItemState(int day, Dictionary<int, int> infos)
	{
		if (infos == null)
		{
			return HuiGuiQianDaoState.CanNotQianDao;
		}
		if (!infos.ContainsKey(day))
		{
			return HuiGuiQianDaoState.CanNotQianDao;
		}
		if (infos[day] == 0)
		{
			return HuiGuiQianDaoState.CanQianDao;
		}
		return HuiGuiQianDaoState.HasQianDao;
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
		MUEventManager.AddEventListener<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_GETSIGNINFO", new Action<Dictionary<int, int>>(this.ServeGetHuiGuiQianDaoInfo));
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_SING", new Action(this.ServeHuiGuiQianDao));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_GETSIGNINFO", new Action<Dictionary<int, int>>(this.ServeGetHuiGuiQianDaoInfo));
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_SING", new Action(this.ServeHuiGuiQianDao));
	}

	private void SendGetQianDaoInfo()
	{
		GameInstance.Game.SendGetHuiGuiQianDaoInfo();
	}

	private void ServeGetHuiGuiQianDaoInfo(Dictionary<int, int> data)
	{
		for (int i = 0; i < this.lstItems.Count; i++)
		{
			int timeOl = this.lstItems[i].LoginGiftInfo.TimeOl;
			HuiGuiQianDaoState itemState = this.GetItemState(timeOl, data);
			this.lstItems[i].SetQianDaoState(itemState);
		}
	}

	private void QianDao(HuiGuiQianDaoPartItem item)
	{
		this.m_qianDaoItem = item;
		int timeOl = item.LoginGiftInfo.TimeOl;
		GameInstance.Game.SendHuiGuiQianDao(HuiGuiData.GetSelfHuoDongLevel().HuoDongLevel, timeOl);
	}

	private void ServeHuiGuiQianDao()
	{
		if (this.m_qianDaoItem != null)
		{
			this.m_qianDaoItem.SetQianDaoState(HuiGuiQianDaoState.HasQianDao);
		}
	}

	private const float CellHeight = 73f;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTime;

	public UILabel lblContent;

	public GButton btnClose;

	public Transform itemContainer;

	private List<HuiGuiQianDaoPartItem> lstItems = new List<HuiGuiQianDaoPartItem>();

	private HuiGuiQianDaoPartItem m_qianDaoItem;
}
