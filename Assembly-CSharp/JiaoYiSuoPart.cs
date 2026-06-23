using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class JiaoYiSuoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BuyGoodsBtn.Text = Global.GetLang("购买物品");
		this.m_SaleGoodsBtn.Text = Global.GetLang("出售物品");
		this.m_TradeHistoryBtn.Text = Global.GetLang("交易记录");
		this.m_JinTuanBtn.Text = Global.GetLang("金  团");
		if ((double)Global.VersionCode < 8.0)
		{
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_BuyGoodsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.m_SaleGoodsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
		};
		this.m_TradeHistoryBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(3);
		};
		this.m_JinTuanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(4);
		};
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.RefreshMoney();
		this.InitPartData(1);
	}

	private new void OnDestroy()
	{
	}

	private void InitPartData(int type = 1)
	{
		this.m_BuyGoodsBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.m_SaleGoodsBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.m_TradeHistoryBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.SetPart(type);
	}

	public void RefreshMoney()
	{
		this.m_TextGold.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.m_TextDiamond.text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
	}

	private void SetPart(int type)
	{
		switch (type)
		{
		case 1:
			this.SetBtnStat(this.m_BuyGoodsBtn, true);
			this.SetBtnStat(this.m_SaleGoodsBtn, false);
			this.SetBtnStat(this.m_TradeHistoryBtn, false);
			this.SetBtnStat(this.m_JinTuanBtn, false);
			if (this.m_BuyGoodsPart == null)
			{
				this.m_BuyGoodsPart = U3DUtils.NEW<BuyGoodsPart>();
				this.m_BuyGoodsPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_BuyGoodsPart.gameObject, true);
			}
			break;
		case 2:
			this.SetBtnStat(this.m_SaleGoodsBtn, true);
			this.SetBtnStat(this.m_BuyGoodsBtn, false);
			this.SetBtnStat(this.m_TradeHistoryBtn, false);
			this.SetBtnStat(this.m_JinTuanBtn, false);
			if (this.m_StallNewPart == null)
			{
				this.m_StallNewPart = U3DUtils.NEW<StallNewPart>();
				this.m_StallNewPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 0)
					{
					}
					if (e.ID == 1)
					{
					}
					if (e.ID == 2)
					{
					}
					if (e.ID == 3)
					{
					}
					if (e.ID == 502)
					{
						PlayZone.GlobalPlayZone.OpenWuPinShangJiaWindow(null, 2);
					}
				};
				this.m_StallNewPart.transform.localPosition = new Vector3(0f, -47f, 0f);
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_StallNewPart.gameObject, true);
			}
			else if (Super._ParcelPart != null && !Super._ParcelPart.IsItemFinished)
			{
				this.m_StallNewPart.gameObject.SetActive(type == 2);
				Super._ParcelPart.CheckInitedPartData();
			}
			break;
		case 3:
			this.SetBtnStat(this.m_SaleGoodsBtn, false);
			this.SetBtnStat(this.m_BuyGoodsBtn, false);
			this.SetBtnStat(this.m_TradeHistoryBtn, true);
			this.SetBtnStat(this.m_JinTuanBtn, false);
			if (this.m_StallLogNewPart == null)
			{
				this.m_StallLogNewPart = U3DUtils.NEW<StallLogNewPart>();
				this.m_StallLogNewPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_StallLogNewPart.gameObject, true);
			}
			break;
		case 4:
			this.SetBtnStat(this.m_SaleGoodsBtn, false);
			this.SetBtnStat(this.m_BuyGoodsBtn, false);
			this.SetBtnStat(this.m_TradeHistoryBtn, false);
			this.SetBtnStat(this.m_JinTuanBtn, true);
			if (this.m_BuyGoodsJinTuanPart == null)
			{
				this.m_BuyGoodsJinTuanPart = U3DUtils.NEW<BuyGoodsJinTuanPart>();
				this.m_BuyGoodsJinTuanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_BuyGoodsJinTuanPart.gameObject, true);
			}
			break;
		}
		if (this.m_BuyGoodsPart != null)
		{
			this.m_BuyGoodsPart.gameObject.SetActive(type == 1);
		}
		if (this.m_StallNewPart != null)
		{
			this.m_StallNewPart.gameObject.SetActive(type == 2);
		}
		if (this.m_StallLogNewPart != null)
		{
			this.m_StallLogNewPart.gameObject.SetActive(type == 3);
			if (type == 3)
			{
				GameInstance.Game.SpriteGetBaiTanLogCmd(0);
			}
		}
		if (this.m_BuyGoodsJinTuanPart != null)
		{
			this.m_BuyGoodsJinTuanPart.gameObject.SetActive(type == 4);
		}
	}

	private void SetBtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	public GButton m_BuyGoodsBtn;

	public GButton m_SaleGoodsBtn;

	public GButton m_TradeHistoryBtn;

	public GButton m_JinTuanBtn;

	public GButton m_CloseBtn;

	public UILabel m_TextGold;

	public UILabel m_TextDiamond;

	public GameObject m_Pnl;

	public DPSelectedItemEventHandler DPSelectedItem;

	public BuyGoodsPart m_BuyGoodsPart;

	public StallNewPart m_StallNewPart;

	public StallLogNewPart m_StallLogNewPart;

	public BuyGoodsJinTuanPart m_BuyGoodsJinTuanPart;
}
