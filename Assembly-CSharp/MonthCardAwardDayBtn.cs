using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class MonthCardAwardDayBtn : UserControl
{
	public GoodsData GetGoodsData(MonthCardAwardDayBtn.GoodsType type = MonthCardAwardDayBtn.GoodsType.GoodsOne)
	{
		if (type == MonthCardAwardDayBtn.GoodsType.GoodsOne)
		{
			if (string.IsNullOrEmpty(this.GoodsOne))
			{
				return null;
			}
			return Global.GetDummyGoodsData(this.GoodsOne);
		}
		else
		{
			if (string.IsNullOrEmpty(this.GoodsTwo))
			{
				return null;
			}
			return Global.GetDummyGoodsData(this.GoodsTwo);
		}
	}

	public void SetState(int state)
	{
		if (state == 0)
		{
			this.PassIcon.SetActive(true);
			this.PassMask.SetActive(true);
			this.RightIcon.SetActive(false);
		}
		else if (state == 1)
		{
			this.PassIcon.SetActive(false);
			this.PassMask.SetActive(true);
			this.RightIcon.SetActive(true);
		}
		else
		{
			this.PassIcon.SetActive(false);
			this.PassMask.SetActive(false);
			this.RightIcon.SetActive(false);
		}
	}

	public void SetDayLabel(int day)
	{
		if (day % 2 != 0)
		{
			this.DayBg.SetActive(true);
			this.DayTxt.enabled = true;
			this.DayTxt.text = string.Format(Global.GetLang("第{0}天"), day);
		}
		else
		{
			this.DayBg.SetActive(false);
			this.DayTxt.enabled = false;
		}
	}

	public GGoodIcon AwardItemBtn;

	public GameObject PassMask;

	public GameObject PassIcon;

	public GameObject RightIcon;

	public GameObject DayBg;

	public UILabel DayTxt;

	[HideInInspector]
	public string DiamondNum;

	[HideInInspector]
	public string GoodsOne;

	[HideInInspector]
	public string Day;

	[HideInInspector]
	public string GoodsTwo;

	[HideInInspector]
	public enum GoodsType
	{
		GoodsOne,
		GoodsTwo
	}
}
