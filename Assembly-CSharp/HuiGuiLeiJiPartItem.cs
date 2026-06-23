using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;
using XMLCreater;

public class HuiGuiLeiJiPartItem : UserControl
{
	public MUHuiGuiChongZhiGift ChongZhiGiftInfo
	{
		get
		{
			return this.m_ChongZhiGiftInfo;
		}
		set
		{
			this.m_ChongZhiGiftInfo = value;
			this.InitInfo(this.m_ChongZhiGiftInfo);
		}
	}

	public HuiGuiLeiJiRewardState State
	{
		get
		{
			return this.m_state;
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblNum.text = Global.GetLang("累积充值100钻石");
		this.btnCharge.Text = Global.GetLang("充值");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnCharge.MouseLeftButtonUp = delegate(object s, MouseEvent e)
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

	private void InitInfo(MUHuiGuiChongZhiGift info)
	{
		if (info == null)
		{
			return;
		}
		this.lblNum.text = Global.GetLang("累积充值") + info.MinYuanBao + Global.GetLang("钻石");
		this.LoadReward(info.GoodsID1);
		this.SetRewardState(HuiGuiLeiJiRewardState.CanNotGet);
	}

	public void SetRewardState(HuiGuiLeiJiRewardState state)
	{
		this.m_state = state;
		if (state == HuiGuiLeiJiRewardState.CanNotGet)
		{
			this.btnCharge.Text = Global.GetLang("充值");
			this.btnCharge.isEnabled = true;
		}
		else if (state == HuiGuiLeiJiRewardState.CanGet)
		{
			this.btnCharge.Text = Global.GetLang("领取");
			this.btnCharge.isEnabled = true;
		}
		else if (state == HuiGuiLeiJiRewardState.HasGet)
		{
			this.btnCharge.Text = Global.GetLang("已领取");
			this.btnCharge.isEnabled = false;
		}
	}

	private const float CellWidth = 75f;

	public UILabel lblNum;

	public GButton btnCharge;

	public List<GameObject> objRewardBg;

	public Transform itemContainer;

	private MUHuiGuiChongZhiGift m_ChongZhiGiftInfo;

	private HuiGuiLeiJiRewardState m_state;

	public Action<HuiGuiLeiJiPartItem> OnSelectItem;
}
