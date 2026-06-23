using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class EveryDayFanLiItem : UserControl
{
	public int ChongZhi
	{
		get
		{
			return this.m_ChongZhi;
		}
		set
		{
			this.m_ChongZhi = value;
			this.m_LabChongZhi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("账号充值:")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(string.Format(Global.GetLang("{0}钻"), value))
			});
		}
	}

	public int FanZuan
	{
		get
		{
			return this.m_FanZuan;
		}
		set
		{
			this.m_LabJiangLi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang(string.Format(Global.GetLang("奖励"), value))
			});
			this.m_FanZuan = value;
			this.m_LabFanZuan.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				value
			});
		}
	}

	public int XiaoHao
	{
		get
		{
			return this.m_XiaoHao;
		}
		set
		{
			this.m_XiaoHao = value;
			this.m_LabXiaoHao.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("角色消费:")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(string.Format(Global.GetLang("{0}钻"), value))
			});
		}
	}

	public string Img
	{
		set
		{
			this.m_SpImg.spriteName = value;
		}
	}

	public UILabel m_LabChongZhi;

	public UILabel m_LabFanZuan;

	public UILabel m_LabXiaoHao;

	public UILabel m_LabJiangLi;

	[SerializeField]
	private UISprite m_SpImg;

	private int m_ChongZhi;

	private int m_FanZuan;

	private int m_XiaoHao;
}
