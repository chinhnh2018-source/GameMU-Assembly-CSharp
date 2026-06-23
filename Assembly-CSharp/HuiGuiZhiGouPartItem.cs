using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;
using XMLCreater;

public class HuiGuiZhiGouPartItem : UserControl
{
	public MUHuiGuiDayZhiGou ZhiGouInfo
	{
		get
		{
			return this.m_ZhiGouInfo;
		}
		set
		{
			this.m_ZhiGouInfo = value;
			this.InitInfo(this.m_ZhiGouInfo);
		}
	}

	public HuiGuiZhiGouState State
	{
		get
		{
			return this.m_state;
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblPrice.text = Global.GetLang(string.Empty);
		this.btnBuy.Text = Global.GetLang(string.Empty);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnBuy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
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

	private void InitInfo(MUHuiGuiDayZhiGou info)
	{
		if (info == null)
		{
			return;
		}
		this.m_ZhiGouInfo = info;
		this.lblPrice.text = Global.GetLang("价格:") + info.Price;
		this.lblMax.text = Global.GetLang("限购:") + info.Max;
		this.LoadReward(info.GoodsID1);
		this.SetRewardState(HuiGuiZhiGouState.CanBuy);
	}

	public void SetPrice(string price)
	{
		this.lblPrice.text = Global.GetLang("价格:") + price;
	}

	public void SetBuyedNum(int num)
	{
		int num2 = this.m_ZhiGouInfo.Max - num;
		if (num2 <= 0)
		{
			num2 = 0;
			this.SetRewardState(HuiGuiZhiGouState.LimitBuy);
		}
		this.lblMax.text = Global.GetLang("限购:") + num2;
	}

	private void SetRewardState(HuiGuiZhiGouState state)
	{
		this.m_state = state;
		if (state == HuiGuiZhiGouState.CanBuy)
		{
			this.btnBuy.Text = Global.GetLang("购买");
			this.btnBuy.isEnabled = true;
		}
		else if (state == HuiGuiZhiGouState.LimitBuy)
		{
			this.btnBuy.Text = Global.GetLang("已限购");
			this.btnBuy.isEnabled = false;
		}
	}

	private const float CellWidth = 75f;

	public UILabel lblPrice;

	public UILabel lblMax;

	public GButton btnBuy;

	public List<GameObject> objRewardBg;

	public Transform itemContainer;

	private MUHuiGuiDayZhiGou m_ZhiGouInfo;

	private HuiGuiZhiGouState m_state;

	public Action<HuiGuiZhiGouPartItem> OnSelectItem;
}
