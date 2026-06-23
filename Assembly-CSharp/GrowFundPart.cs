using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GrowFundPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Zhuangsheng.Label.text = Global.GetLang("转生基金");
		this.Shiguang.Label.text = Global.GetLang("时光基金");
		this.Haoqi.Label.text = Global.GetLang("豪气基金");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Diamond.text = Global.Data.roleData.UserMoney.ToString();
		this.BdDiamond.text = Global.Data.roleData.Gold.ToString();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		};
		this.Zhuangsheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Curpage == 1)
			{
				return;
			}
			this.SetPage(1);
		};
		this.Shiguang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Curpage == 2)
			{
				return;
			}
			this.SetPage(2);
		};
		this.Haoqi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Curpage == 3)
			{
				return;
			}
			if (Global.GetVIPLeve() < 7)
			{
				Super.HintMainText(Global.GetLang("玩家VIP等级达到7级才可开启！"), 10, 3);
				return;
			}
			this.SetPage(3);
		};
		ActivityTipManager.RegActivityTipItem(14106, delegate(int s, ActivityTipItem e)
		{
			if (this.ZhuangshengTip != null)
			{
				this.ZhuangshengTip.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14107, delegate(int s, ActivityTipItem e)
		{
			if (this.ShiguangTip != null)
			{
				this.ShiguangTip.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14108, delegate(int s, ActivityTipItem e)
		{
			if (this.HaoqiTip != null)
			{
				this.HaoqiTip.SetActive(e.IsActive);
			}
		});
		GameInstance.Game.GetFundInfo();
		Super.ShowNetWaiting(null);
	}

	public void InitPage()
	{
		this.Curpage = 1;
		this.SetPage(1);
	}

	public void SetPage(FundData fundData)
	{
		this.Diamond.text = Global.Data.roleData.UserMoney.ToString();
		this.BdDiamond.text = Global.Data.roleData.Gold.ToString();
		this.SetPage(fundData.FundType);
		if (this.buyFundsPart != null)
		{
			this.buyFundsPart.SetListBtnState(fundData);
		}
	}

	public void SetPageList(FundData fundData)
	{
		this.Diamond.text = Global.Data.roleData.UserMoney.ToString();
		this.BdDiamond.text = Global.Data.roleData.Gold.ToString();
		if (this.buyFundsPart != null)
		{
			this.buyFundsPart.SetListBtnState(fundData);
		}
	}

	private void SetPage(int page)
	{
		this.Curpage = page;
		if (Global.fundData == null)
		{
			return;
		}
		if (Global.fundData.FundDic.Count <= 0)
		{
			return;
		}
		if (page == 1)
		{
			this.Zhuangsheng.Label.color = NGUIMath.HexToColorEx(14922604U);
			this.Shiguang.Label.color = NGUIMath.HexToColorEx(10323559U);
			this.Haoqi.Label.color = NGUIMath.HexToColorEx(10323559U);
			this.Zhuangsheng.Pressed = true;
			this.Shiguang.Pressed = false;
			this.Haoqi.Pressed = false;
			this.Zhuangsheng.Refresh();
		}
		else if (page == 2)
		{
			this.Zhuangsheng.Label.color = NGUIMath.HexToColorEx(10323559U);
			this.Shiguang.Label.color = NGUIMath.HexToColorEx(14922604U);
			this.Haoqi.Label.color = NGUIMath.HexToColorEx(10323559U);
			this.Zhuangsheng.Pressed = false;
			this.Shiguang.Pressed = true;
			this.Haoqi.Pressed = false;
			this.Shiguang.Refresh();
		}
		else if (page == 3)
		{
			this.Zhuangsheng.Label.color = NGUIMath.HexToColorEx(10323559U);
			this.Shiguang.Label.color = NGUIMath.HexToColorEx(10323559U);
			this.Haoqi.Label.color = NGUIMath.HexToColorEx(14922604U);
			this.Zhuangsheng.Pressed = false;
			this.Shiguang.Pressed = false;
			this.Haoqi.Pressed = true;
			this.Haoqi.Refresh();
		}
		if (this.buyFundsPart == null)
		{
			this.buyFundsPart = U3DUtils.NEW<BuyFundsPart>();
			U3DUtils.AddChild(this.Pnl.gameObject, this.buyFundsPart.gameObject, true);
		}
		int fundID = Global.fundData.FundDic[page].FundID;
		this.buyFundsPart.InitPage(page, fundID);
		this.buyFundsPart.FundType = page;
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(14106, null);
		ActivityTipManager.RegActivityTipItem(14107, null);
		ActivityTipManager.RegActivityTipItem(14108, null);
	}

	public GButton Close;

	public GButton Zhuangsheng;

	public GButton Shiguang;

	public GButton Haoqi;

	public UILabel Diamond;

	public UILabel BdDiamond;

	public GameObject ZhuangshengTip;

	public GameObject ShiguangTip;

	public GameObject HaoqiTip;

	public GameObject Pnl;

	public BuyFundsPart buyFundsPart;

	public DPSelectedItemEventHandler DPSelectItem;

	private int Curpage;
}
