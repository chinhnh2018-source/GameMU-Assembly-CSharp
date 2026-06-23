using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;
using XMLCreater;

public class HuiGuiQianDaoPartItem : UserControl
{
	public MUHuiGuiLoginNumGift LoginGiftInfo
	{
		get
		{
			return this.m_LoginGiftInfo;
		}
		set
		{
			this.m_LoginGiftInfo = value;
			this.InitInfo(this.m_LoginGiftInfo);
		}
	}

	public HuiGuiQianDaoState State
	{
		get
		{
			return this.m_state;
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblDay.text = Global.GetLang(string.Empty);
		this.btnQianDao.Text = Global.GetLang(string.Empty);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnQianDao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OnSelectItem != null)
			{
				this.OnSelectItem.Invoke(this);
			}
		};
	}

	private void LoadReward(List<string> rewardStr)
	{
		for (int i = 0; i < this.objRewardBg.Count; i++)
		{
			if (i < rewardStr.Count)
			{
				this.objRewardBg[i].SetActive(true);
				GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(rewardStr[i], false, true, true);
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
				ggoodIcon.transform.SetParent(this.itemContainer);
				ggoodIcon.transform.localPosition = new Vector3(75f * (float)i, 0f, 0f);
			}
			else
			{
				this.objRewardBg[i].SetActive(false);
			}
		}
	}

	private void InitInfo(MUHuiGuiLoginNumGift info)
	{
		if (info == null)
		{
			return;
		}
		this.lblDay.text = this.GetDayString(info.TimeOl);
		this.LoadReward(info.GoodsID1);
		this.SetQianDaoState(HuiGuiQianDaoState.CanNotQianDao);
	}

	public void SetQianDaoState(HuiGuiQianDaoState state)
	{
		this.m_state = state;
		if (state == HuiGuiQianDaoState.CanQianDao)
		{
			this.btnQianDao.Text = Global.GetLang("签到");
			this.btnQianDao.isEnabled = true;
		}
		else if (state == HuiGuiQianDaoState.HasQianDao)
		{
			this.btnQianDao.Text = Global.GetLang("已签到");
			this.btnQianDao.isEnabled = false;
		}
		else if (state == HuiGuiQianDaoState.CanNotQianDao)
		{
			this.btnQianDao.Text = Global.GetLang("签到");
			this.btnQianDao.isEnabled = false;
		}
	}

	private string GetDayString(int dayNum)
	{
		string result = string.Empty;
		switch (dayNum)
		{
		case 1:
			result = Global.GetLang("第一天");
			break;
		case 2:
			result = Global.GetLang("第二天");
			break;
		case 3:
			result = Global.GetLang("第三天");
			break;
		case 4:
			result = Global.GetLang("第四天");
			break;
		case 5:
			result = Global.GetLang("第五天");
			break;
		case 6:
			result = Global.GetLang("第六天");
			break;
		case 7:
			result = Global.GetLang("第七天");
			break;
		}
		return result;
	}

	private const float CellWidth = 75f;

	public UILabel lblDay;

	public GButton btnQianDao;

	public List<GameObject> objRewardBg;

	public Transform itemContainer;

	private MUHuiGuiLoginNumGift m_LoginGiftInfo;

	private HuiGuiQianDaoState m_state;

	public Action<HuiGuiQianDaoPartItem> OnSelectItem;
}
