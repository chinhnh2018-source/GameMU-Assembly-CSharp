using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class HuiGuiStorePartItem : UserControl
{
	public MUHuiGuiStore StoreInfo
	{
		get
		{
			return this.m_StoreInfo;
		}
		set
		{
			this.m_StoreInfo = value;
			this.InitInfo(this.m_StoreInfo);
		}
	}

	public GoodVO GoodVo
	{
		get
		{
			return this.m_goodVo;
		}
	}

	public HuiGuiStoreState State
	{
		get
		{
			return this.m_state;
		}
	}

	public int LeftBuyNum
	{
		get
		{
			return this.m_leftBuyNum;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnBuy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_state == HuiGuiStoreState.LimitBuy)
			{
				Super.HintMainText(Global.GetLang("已经购买完毕"), 10, 3);
				return;
			}
			if (this.OnBuyItem != null)
			{
				this.OnBuyItem.Invoke(this);
			}
		};
	}

	private void LoadStoreItem(string rewardStr)
	{
		GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(rewardStr, false, true, true);
		int itemCode = ggoodIcon.ItemCode;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(itemCode);
		this.m_goodVo = goodsXmlNodeByID;
		this.lblName.text = goodsXmlNodeByID.Title;
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		ggoodIcon.transform.SetParent(this.itemContainer);
		ggoodIcon.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private void InitInfo(MUHuiGuiStore info)
	{
		if (info == null)
		{
			return;
		}
		this.m_StoreInfo = info;
		this.lblPrice.text = info.Price.ToString();
		this.lblMax.text = Global.GetLang("限购：") + info.SinglePurchase.ToString();
		this.LoadStoreItem(info.GoodsID);
		this.SetRewardState(HuiGuiStoreState.CanBuy);
		this.m_leftBuyNum = info.SinglePurchase;
	}

	public void AddBuyNum(int num)
	{
		this.m_leftBuyNum -= num;
		if (this.m_leftBuyNum < 1)
		{
			this.m_leftBuyNum = 0;
			this.SetRewardState(HuiGuiStoreState.LimitBuy);
		}
		this.lblMax.text = Global.GetLang("限购：") + this.m_leftBuyNum.ToString();
	}

	private void SetRewardState(HuiGuiStoreState state)
	{
		this.m_state = state;
	}

	public int GetBuyMaxNum()
	{
		return this.m_leftBuyNum;
	}

	private const float CellWidth = 75f;

	public Action<HuiGuiStorePartItem> OnBuyItem;

	public UILabel lblName;

	public UILabel lblMax;

	public UILabel lblPrice;

	public GButton btnBuy;

	public List<GameObject> objRewardBg;

	public Transform itemContainer;

	private MUHuiGuiStore m_StoreInfo;

	private GoodVO m_goodVo;

	private HuiGuiStoreState m_state;

	private int m_leftBuyNum;
}
