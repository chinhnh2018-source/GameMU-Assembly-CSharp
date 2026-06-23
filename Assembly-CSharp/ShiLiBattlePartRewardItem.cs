using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartRewardItem : UserControl
{
	public MUForceCraftReward CraftReward
	{
		get
		{
			return this.m_craftReward;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetSelected(false);
	}

	public void InitInfo(MUForceCraftReward reward)
	{
		this.m_craftReward = reward;
		if (reward.Rank < 0)
		{
			this.lblPaiMing.text = Global.GetLang("前") + Mathf.RoundToInt(reward.RankRate * 100f) + "%";
		}
		else
		{
			this.lblPaiMing.text = reward.Rank.ToString();
		}
		this.lblJunXian.text = reward.Grade.ToString();
		this.lblGongXian.text = reward.Contribution.ToString();
		this.LoadItems(reward.LstGoodsOne);
		this.LoadItems(reward.LstGoodsTwo);
		this.gridGoods.Reposition();
	}

	private void LoadItems(List<string> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(items[i], false, true, true);
			ggoodIcon.transform.SetParent(this.gridGoods.transform);
			ggoodIcon.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
			ggoodIcon.transform.localPosition = Vector3.zero;
		}
	}

	public void SetSelected(bool beSelected)
	{
		this.goSelected.SetActive(beSelected);
	}

	public UILabel lblPaiMing;

	public UILabel lblJunXian;

	public UILabel lblGongXian;

	public UIGrid gridGoods;

	public GameObject goSelected;

	private MUForceCraftReward m_craftReward;
}
