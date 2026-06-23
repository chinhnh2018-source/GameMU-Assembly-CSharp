using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RecallShopPagePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblNoQianggouData.text = Global.GetLang("暂无抢购数据");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.mallItemList = new List<MallItem>();
	}

	public int QianggouId
	{
		get
		{
			return 100000;
		}
	}

	public int tabID
	{
		get
		{
			return this._tabID;
		}
		set
		{
			this._tabID = value;
			if (value == 100000)
			{
				this.listBox.cellHeight = 193f;
				Vector3 localPosition = this.listBox.transform.localPosition;
				this.listBox.transform.localPosition = new Vector3(localPosition.x, localPosition.y - 48f, localPosition.z);
			}
			else
			{
				this.listBox.cellHeight = 100f;
			}
		}
	}

	public int mallType
	{
		get
		{
			return this._mallType;
		}
		set
		{
			this._mallType = value;
		}
	}

	public List<XElement> mallItemXmlList
	{
		set
		{
			this._mallItemXmlList = value;
		}
	}

	public double QiangGouEndTicks
	{
		get
		{
			return this._QiangGouEndTicks;
		}
		set
		{
			this._QiangGouEndTicks = value;
		}
	}

	private void OnEnable()
	{
		if (Enumerable.Count<MallItem>(this.mallItemList) <= 0)
		{
			if (this.tabID == 100000)
			{
				this.RefreshXianGouData();
			}
			else
			{
				this.RefreshGoodsByType(-1);
			}
		}
	}

	public void RefreshXianGouData()
	{
	}

	public void RefreshGoodsByType(int hintGoodsID = -1)
	{
		this.ClearHintDecoration();
		if (this._mallItemXmlList == null)
		{
			return;
		}
		if (this.RunningCoroutine == null && base.gameObject.activeInHierarchy)
		{
			this.RunningCoroutine = base.StartCoroutine<bool>(this.LoadMallItem(this.iLoadStartIdx));
		}
	}

	public void RefreshOnsaleInfo(int xmlID, int num)
	{
		for (int i = 0; i < this.listBox.Count(); i++)
		{
			MallItem mallItem = U3DUtils.AS<MallItem>(this.listBox.GetItemByIndex(i));
			if (null != mallItem && mallItem.ItemID == xmlID)
			{
				mallItem.ItemLimitCount.text = string.Empty + (int.Parse(mallItem.ItemLimitCount.text) - num);
				mallItem.ItemLeftCount.text = string.Empty + (int.Parse(mallItem.ItemLeftCount.text) - num);
			}
		}
	}

	public MallItem GetMallItem(int xmlID)
	{
		for (int i = 0; i < this.listBox.Count(); i++)
		{
			MallItem mallItem = U3DUtils.AS<MallItem>(this.listBox.GetItemByIndex(i));
			if (null != mallItem && mallItem.ItemID == xmlID)
			{
				return mallItem;
			}
		}
		return null;
	}

	private void showSelectTip(MallItem mi, EventArgs e)
	{
		if (null == mi)
		{
			return;
		}
		if (int.Parse(mi.XianGouShu) <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("物品限购数量为0，无法进行购买!"), 0, -1, -1, 0);
			return;
		}
		if (mi.GoodsDataInfo != null)
		{
			GTipServiceEx.ShowTip(mi.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.WangZheShangCheng, GoodsPriceUnitTypes.KingOfBattlePoint, int.Parse(mi.ItemPrice.text), mi.GoodsDataInfo);
		}
		else
		{
			GTipServiceEx.ShowTip(mi.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.WangZheShangCheng, GoodsPriceUnitTypes.KingOfBattlePoint, int.Parse(mi.ItemPrice.text), mi.MallGoodsID, -1, -1, null);
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
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

	private IEnumerator LoadMallItem(int StartIdx)
	{
		yield return null;
		int limitPerFrame = (StartIdx + this.LoadCountPerFrame > this._mallItemXmlList.Count) ? this._mallItemXmlList.Count : (StartIdx + this.LoadCountPerFrame);
		for (int i = StartIdx; i < limitPerFrame; i++)
		{
			int mallID = Global.GetXElementAttributeInt(this._mallItemXmlList[i], "ID");
			double origPrice = (double)Global.GetXElementAttributeInt(this._mallItemXmlList[i], "OrigPrice");
			double price = (double)Global.GetXElementAttributeInt(this._mallItemXmlList[i], "Price");
			int SinglePurchase = Global.GetXElementAttributeInt(this._mallItemXmlList[i], "SinglePurchase");
			string GoodsID = Global.GetXElementAttributeStr(this._mallItemXmlList[i], "GoodsID");
			string[] goods = Global.GetXElementAttributeStr(this._mallItemXmlList[i], "GoodsID").Split(new char[]
			{
				','
			});
			int goodsID = Convert.ToInt32(goods[0]);
			string fullPurchase = "0";
			string singlePurchase = SinglePurchase.ToString();
			GoodsData goodsData = null;
			this.LoadGoodsData(goods, out goodsData);
			string _ImageURL = "shangchengXiangouItem_bak";
			GoodsData tempGoodsData = null;
			GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			MallItem si = U3DUtils.NEW<MallItem>();
			si.ItemID = mallID;
			si.GoodsDataInfo = goodsData;
			si.MallGoodsID = goodsID;
			int categoriy = Global.GetCategoriyByGoodsID(goodsID);
			if (categoriy >= 0 && categoriy < 25)
			{
				si.GoodsName = string.Concat(new string[]
				{
					"{",
					Global.GetColorByGoodsData(goodsData).ToString("X"),
					"}",
					goodVO.Title,
					"{-}"
				});
			}
			else
			{
				si.GoodsName = Global.GetColorStringForNGUIText(new object[]
				{
					goodVO.GoodsColor,
					goodVO.Title
				});
			}
			si.goodsOwnerTypes = GoodsOwnerTypes.LaoWanJiaShangCheng;
			si.BodyBackground = _ImageURL;
			si.TabPageID = 100000;
			si.ItemID = mallID;
			si.MallGoodsID = goodsID;
			si.GoodsName = Global.GetColorStringForNGUIText(new object[]
			{
				goodVO.GoodsColor,
				goodVO.Title
			});
			si.GoodsPrice = price.ToString();
			si.GoodsOrgPrice = origPrice.ToString();
			si.ShengYuShu = fullPurchase;
			si.XianGouShu = singlePurchase;
			si.ItemType = 1;
			tempGoodsData = Global.GetDummyGoodsData(goodsID);
			tempGoodsData.Binding = 1;
			si.GoodsDataInfo = tempGoodsData;
			si.InitGoodsIcon();
			si.ItemIcon.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e1)
			{
				MallItem mallItem = U3DUtils.AS<MallItem>(this.listBox.SelectedItem);
				if (e1.IDType == 8)
				{
					if (mallItem.TabPageID == 10000)
					{
						if (Global.Data.roleData.Gold < int.Parse(mallItem.GoodsPrice) * e1.ID)
						{
							Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
							return;
						}
					}
					else if (Global.Data.roleData.UserMoney < int.Parse(mallItem.GoodsPrice) * e1.ID)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
						return;
					}
					this.StartGouMai(mallItem.MallGoodsID, mallItem.ItemID, e1.ID, mallItem.ItemType != 0);
				}
			};
			si.OnStartGouHander = new MallItem.StartGouMaiDelegate(this.StartGouMai);
			this.mallItemList.Add(si);
			this.ItemCollection.AddNoUpdate(si);
			UIPanel temppanel = si.transform.GetComponent<UIPanel>();
			if (temppanel != null)
			{
				Object.Destroy(temppanel);
			}
		}
		if (limitPerFrame < this._mallItemXmlList.Count)
		{
			this.iLoadStartIdx = limitPerFrame;
			base.StartCoroutine<bool>(this.LoadMallItem(this.iLoadStartIdx));
		}
		else
		{
			this.iLoadStartIdx = 0;
			this.RunningCoroutine = null;
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.recallShopPart != null)
			{
				PlayZone.GlobalPlayZone.recallShopPart.UpdateStroeList();
			}
		}
		yield break;
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

	private const int iQiangGouid = 100000;

	public UILabel lblNoQianggouData;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox listBox;

	private ObservableCollection ItemCollection;

	private List<MallItem> mallItemList;

	public UIScrollBar scrollBar;

	public UIDraggablePanel dragPanel;

	private int _tabID;

	private int _mallType;

	private List<XElement> _mallItemXmlList;

	private double _QiangGouEndTicks = -1.0;

	private TTMonoBehaviour.Coroutine<bool> RunningCoroutine;

	private int iLoadStartIdx;

	private int LoadCountPerFrame = 8;

	public enum MallType
	{
		TypeNormal,
		TypeOnSale
	}
}
