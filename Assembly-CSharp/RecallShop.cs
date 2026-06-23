using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RecallShop : RecallGoodsEx
{
	public int QianggouId
	{
		get
		{
			return 100000;
		}
	}

	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.activityInfo)
		{
			this.activityInfo.text = Global.GetLang(Global.GetLang("成为回归用户即可领取(每个帐号只能领取一次)"));
		}
		this.LoadMallItem(this.iLoadStartIdx);
		this.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		};
	}

	public override bool InitRewards()
	{
		if (!base.InitRewards())
		{
			return false;
		}
		this.ShowChongZhiListNew();
		return true;
	}

	private void ShowChongZhiListNew()
	{
		if (base.xml == null)
		{
			return;
		}
		if (base.xmlList == null)
		{
			return;
		}
		this._itemCollection.Clear();
	}

	private void LoadMallItem(int StartIdx)
	{
		for (int i = 0; i < base.xmlList.Count; i++)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(base.xmlList[i], "ID");
			double num = (double)Global.GetXElementAttributeInt(base.xmlList[i], "OrigPrice");
			double num2 = (double)Global.GetXElementAttributeInt(base.xmlList[i], "Price");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(base.xmlList[i], "SinglePurchase");
			string xelementAttributeStr = Global.GetXElementAttributeStr(base.xmlList[i], "GoodsID");
			string[] array = Global.GetXElementAttributeStr(base.xmlList[i], "GoodsID").Split(new char[]
			{
				','
			});
			int num3 = Convert.ToInt32(array[0]);
			string shengYuShu = "0";
			string xianGouShu = xelementAttributeInt2.ToString();
			GoodsData goodsData = null;
			this.LoadGoodsData(array, out goodsData);
			string bodyBackground = "shangchengXiangouItem_bak";
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num3);
			MallItem mallItem = U3DUtils.NEW<MallItem>();
			mallItem.ItemID = xelementAttributeInt;
			mallItem.GoodsDataInfo = goodsData;
			mallItem.MallGoodsID = num3;
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(num3);
			if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
			{
				mallItem.GoodsName = string.Concat(new string[]
				{
					"{",
					Global.GetColorByGoodsData(goodsData).ToString("X"),
					"}",
					goodsXmlNodeByID.Title,
					"{-}"
				});
			}
			else
			{
				mallItem.GoodsName = Global.GetColorStringForNGUIText(new object[]
				{
					goodsXmlNodeByID.GoodsColor,
					goodsXmlNodeByID.Title
				});
			}
			mallItem.goodsOwnerTypes = GoodsOwnerTypes.LaoWanJiaShangCheng;
			mallItem.BodyBackground = bodyBackground;
			mallItem.TabPageID = 100000;
			mallItem.ItemID = xelementAttributeInt;
			mallItem.MallGoodsID = num3;
			mallItem.GoodsName = Global.GetColorStringForNGUIText(new object[]
			{
				goodsXmlNodeByID.GoodsColor,
				goodsXmlNodeByID.Title
			});
			mallItem.GoodsPrice = num2.ToString();
			mallItem.GoodsOrgPrice = num.ToString();
			mallItem.ShengYuShu = shengYuShu;
			mallItem.XianGouShu = xianGouShu;
			mallItem.ItemType = 1;
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(num3);
			dummyGoodsData.Binding = 1;
			mallItem.GoodsDataInfo = dummyGoodsData;
			mallItem.InitGoodsIcon();
			mallItem.btnBuyNow.transform.localPosition = new Vector3(40f, -47f, -0.1f);
			mallItem.ItemLeftCount.transform.localPosition = new Vector3(-70f, -51f, -0.2f);
			mallItem.ItemIcon.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e1)
			{
				MallItem mallItem2 = U3DUtils.AS<MallItem>(this.goodsList.SelectedItem);
				if (e1.IDType == 8)
				{
					if (mallItem2.TabPageID == 10000)
					{
						if (Global.Data.roleData.Gold < int.Parse(mallItem2.GoodsPrice) * e1.ID)
						{
							Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
							return;
						}
					}
					else if (Global.Data.roleData.UserMoney < int.Parse(mallItem2.GoodsPrice) * e1.ID)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
						return;
					}
					this.StartGouMai(mallItem2.MallGoodsID, mallItem2.ItemID, e1.ID, mallItem2.ItemType != 0);
				}
			};
			mallItem.OnStartGouHander = new MallItem.StartGouMaiDelegate(this.StartGouMai);
			this.mallItemList.Add(mallItem);
			base.ItemCollection.AddNoUpdate(mallItem);
			UIPanel component = mallItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			if (i >= base.xmlList.Count - 1)
			{
				this.UpdateStroeList();
			}
		}
	}

	private void LoadGoodsData(string[] goods, out GoodsData goodsData)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			goodsData = new GoodsData
			{
				GoodsID = num,
				GCount = Convert.ToInt32(goods[1]),
				Binding = Convert.ToInt32(goods[2]),
				Forge_level = Convert.ToInt32(goods[3]),
				AppendPropLev = Convert.ToInt32(goods[4]),
				Lucky = Convert.ToInt32(goods[5]),
				ExcellenceInfo = Convert.ToInt32(goods[6])
			};
		}
		else
		{
			goodsData = null;
		}
	}

	private void StartGouMai(int buyGoodsID, int itemID, int num, bool isXianGou = false)
	{
		if (Global.IsBagFull())
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买！"), new object[0]), 10, 3);
			return;
		}
		ServerBufferZhaoHui.Instance.SendOpenAward(4, itemID, 1);
	}

	public void UpdateStroeList()
	{
		this.storeLabelMoney.text = Global.Data.roleData.UserMoney.ToString();
		MallItem mallItem = null;
		for (int i = 0; i < this.goodsList.Count(); i++)
		{
			mallItem = this.goodsList.getChildAt(i).GetComponent<MallItem>();
			if (null != mallItem)
			{
				int num = 0;
				foreach (int num2 in this.mTab.RewardIDStateDic.Keys)
				{
					if (mallItem.ItemID == num2)
					{
						num = this.mTab.RewardIDStateDic[num2].state;
						break;
					}
				}
				mallItem.ShengYuShu = (mallItem.XianGouShu.SafeToInt32(0) - num).ToString();
			}
		}
	}

	private const int iQiangGouid = 100000;

	private const int TAG_XIANGOU = 100000;

	public UILabel m_LabTime;

	public UILabel m_LabZuanShi;

	public UILabel m_LabTimeTitle;

	public GButton m_Btn;

	public UIScrollBar m_Scroll;

	public List<MallItem> mallItemList = new List<MallItem>();

	private int iLoadStartIdx;
}
