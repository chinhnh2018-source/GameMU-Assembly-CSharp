using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MallHotSalePart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnRecharge.Text = Global.GetLang("充值");
		this.GoToMonthCardBtn.Text = Global.GetLang("查看详情");
		this.ConstLblQianggou.text = Global.GetLang("抢购倒计时");
		string text = string.Empty;
		if (!Context.IsAPPVerify)
		{
			text = "MallHotRole.png";
		}
		else
		{
			text = "MallHotRoleIosVerify.png";
		}
		this.BFGirlBg.GetComponent<ShowNetImage>().ImageURL = string.Format("NetImages/GameRes/Images/Plate/{0}", text);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.tabPagesDict = new Dictionary<int, MallHotSalePagePart>();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearMallData();
			this.StopHeart();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.btnRecharge.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				if (Global.g_bIsYaoQingCeShi)
				{
					Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("该功能暂未开放，敬请期待。"), -1, -1, -1, -1, false);
				}
				else
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1
					});
				}
			}
		};
		this.GoToMonthCardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
		if (Global.IsYueKaOpen() && Global.Data.roleData.RoleCommonUseIntPamams.Count > 28 && Global.Data.roleData.RoleCommonUseIntPamams[28] == 0)
		{
			this.GoToMonthCardBtn.gameObject.SetActive(true);
			this.MonthCardBg.SetActive(true);
			this.BFGirlBg.SetActive(false);
			if (Context.IsHaiwai)
			{
				this.GoToMonthCardBtn.gameObject.SetActive(false);
				this.MonthCardBg.SetActive(false);
				this.BFGirlBg.SetActive(true);
			}
		}
		else
		{
			this.GoToMonthCardBtn.gameObject.SetActive(false);
			this.MonthCardBg.SetActive(false);
			this.BFGirlBg.SetActive(true);
		}
	}

	private void InitTabArray()
	{
		XElement xelement = XElement.Parse(Global.Data.MallData.MallTabXmlString);
		if (xelement == null)
		{
			return;
		}
		this.TabNames = new List<string>();
		this.TabIDs = new List<int>();
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "Mall"), "*");
		foreach (XElement xelement2 in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ID");
			if (xelementAttributeInt != Global.limitTabID)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Name");
				if (xelementAttributeStr != string.Empty)
				{
					this.TabNames.Add(xelementAttributeStr);
					this.TabIDs.Add(xelementAttributeInt);
					this.mallItemsDict.Add(xelementAttributeInt, new List<XElement>());
				}
			}
		}
	}

	private void SepriteMallInfos(List<XElement> mallItems)
	{
		List<XElement> list = null;
		foreach (XElement xelement in mallItems)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			if (this.mallItemsDict.TryGetValue(xelementAttributeInt, ref list))
			{
				list.Add(xelement);
			}
		}
	}

	private int FindTabID(string text)
	{
		for (int i = 0; i < this.TabNames.Count; i++)
		{
			if (this.TabNames[i] == text)
			{
				return this.TabIDs[i];
			}
		}
		return -1;
	}

	private int FindTabIDIndexByID(int tabID)
	{
		for (int i = 0; i < this.TabIDs.Count; i++)
		{
			if (this.TabIDs[i] == tabID)
			{
				return i;
			}
		}
		return 0;
	}

	public void RefreshUserMoney()
	{
		this.UserMoneyTextBlock.text = Global.Data.roleData.UserMoney.ToString();
		if (null != this.mallHotSaleHintPart)
		{
			this.mallHotSaleHintPart.RefreshUserMoney();
		}
	}

	public void RefreshUserYinLiang()
	{
	}

	public void InitPartSize(int idx)
	{
		this.StartPageIdx = idx;
		if (Global.Data.MallData == null)
		{
			GameInstance.Game.SpriteFetchMallData(0);
			return;
		}
		this.InitPartSize_Ex();
	}

	protected void InitPartSize_Ex()
	{
		List<XElement> mallItemXmlList = null;
		this.mallItemsDict.Clear();
		this.xmlMall = XElement.Parse(Global.Data.MallData.MallXmlString);
		this.MallItems = Global.GetXElementList(Global.GetXElement(this.xmlMall, "Mall"), "*");
		this.InitTabArray();
		this.SepriteMallInfos(this.MallItems);
		this.CurrentSelectedTab = this.TabIDs[0];
		this.gTabControl.ClearPages();
		this.gTabControl.AddTabPage(Enumerable.Count<string>(this.TabNames));
		this.gTabControl.SetTabButtonBackground("chatTab_normal", "chatTab_hover");
		for (int i = 0; i < this.TabNames.Count; i++)
		{
			MallHotSalePagePart mallHotSalePagePart = U3DUtils.NEW<MallHotSalePagePart>();
			if (this.mallItemsDict.TryGetValue(this.TabIDs[i], ref mallItemXmlList))
			{
				mallHotSalePagePart.mallItemXmlList = mallItemXmlList;
			}
			mallHotSalePagePart.tabID = this.TabIDs[i];
			this.gTabControl.SetTabButtonName(this.TabNames[i], i);
			this.gTabControl.AddPageContent(mallHotSalePagePart.gameObject, i);
			this.tabPagesDict.Add(this.TabIDs[i], mallHotSalePagePart);
			if (i == 0)
			{
				mallHotSalePagePart.RefreshGoodsByType(-1);
				if (mallHotSalePagePart.tabID == mallHotSalePagePart.QianggouId)
				{
					mallHotSalePagePart.RefreshXianGouData();
				}
				else
				{
					mallHotSalePagePart.RefreshGoodsByType(-1);
				}
			}
		}
		if (string.IsNullOrEmpty(Global.Data.MallData.QiangGouXmlString) && this.StartPageIdx == 0)
		{
			this.gTabControl.SetActivePage(this.StartPageIdx + 1);
		}
		else
		{
			this.gTabControl.SetActivePage(this.StartPageIdx);
		}
		this.RefreshUserMoney();
		this.RefreshUserYinLiang();
	}

	private void ClearMallData()
	{
		if (this.tabPagesDict != null)
		{
			foreach (KeyValuePair<int, MallHotSalePagePart> keyValuePair in this.tabPagesDict)
			{
				MallHotSalePagePart value = keyValuePair.Value;
				value.mallItemXmlList = null;
				value.transform.parent = null;
				Object.Destroy(value.gameObject);
			}
			this.tabPagesDict.Clear();
		}
	}

	private int GetGoodsTabID(int goodsID)
	{
		if (goodsID <= 0)
		{
			return this.TabIDs[0];
		}
		foreach (XElement xelement in this.MallItems)
		{
			if (Global.GetXElementAttributeInt(xelement, "GoodsID") == goodsID)
			{
				return Global.GetXElementAttributeInt(xelement, "TabID");
			}
		}
		return this.TabIDs[0];
	}

	public void InitPartData(int toBuyGoodsID)
	{
		if (this.TabIDs != null)
		{
			this.RefreshGoodsByType(this.GetGoodsTabID(toBuyGoodsID), toBuyGoodsID);
			this.ToBuyGoodsID = -1;
		}
		else
		{
			this.ToBuyGoodsID = toBuyGoodsID;
		}
		this.RefreshUserGold();
	}

	private void RefreshGoodsByType(int id, int hintGoodsID)
	{
		MallHotSalePagePart mallHotSalePagePart = null;
		if (this.tabPagesDict.TryGetValue(id, ref mallHotSalePagePart))
		{
			mallHotSalePagePart.RefreshGoodsByType(-1);
		}
	}

	public void RefreshUserGold()
	{
		this.UserBdYuanBaoText.text = Global.Data.roleData.Gold.ToString();
	}

	private void StartGouMai(int buyGoodsID, MallItem mallItem, bool isXianGou = false)
	{
		if (null == mallItem)
		{
			return;
		}
		if (!Global.CanAddGoods(buyGoodsID, 1, 0, "1900-01-01 12:00:00", true))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买物品..."), new object[0]), 1, -1, -1, 0);
			return;
		}
		if (this.CurrentSelectedTab == 10000 && !isXianGou)
		{
			if (Global.Data.roleData.Gold - mallItem.GoodsPrice.SafeToInt32(0) < 0)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
		}
		else if (Global.Data.roleData.UserMoney - mallItem.GoodsPrice.SafeToInt32(0) < 0)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		if (this.CurrentSelectedTab == 10000)
		{
			GameInstance.Game.SpriteMallZhenQiBuy(mallItem.MallGoodsID, 1);
		}
		else if (isXianGou)
		{
			GameInstance.Game.SpriteMallQiangGouBuy(mallItem.MallGoodsID, 1, false, buyGoodsID);
		}
		else
		{
			GameInstance.Game.SpriteMallBuy(mallItem.MallGoodsID, 1, false);
		}
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("QiangGou_Timer");
		this._Timer.Interval = TimeSpan.FromSeconds(1.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
	}

	public void OnQiangGouBuyFinished(int ret, int qiangGouId, int num)
	{
		if (ret < 0)
		{
			return;
		}
		MallHotSalePagePart mallHotSalePagePart = null;
		if (this.tabPagesDict.TryGetValue(100000, ref mallHotSalePagePart))
		{
			mallHotSalePagePart.RefreshOnsaleInfo(qiangGouId, num);
		}
	}

	public void RefreshMallData(object mallData)
	{
		this.InitPartSize_Ex();
		this.RefreshXianGouData();
		this.RefreshGoodsByType(this.GetGoodsTabID(this.ToBuyGoodsID), this.ToBuyGoodsID);
		this.ToBuyGoodsID = -1;
	}

	private void ForgeTimer_Tick(object sender, object e)
	{
		if (null != this.lblLimitTime)
		{
			this.lblLimitTime.text = this.GetQiangGouLeftTimeString();
		}
	}

	private string GetQiangGouLeftTimeString()
	{
		string result = Global.GetLang("已结束");
		double num = (double)(Global.GetCorrectDateTime().Ticks / 10000L);
		double num2 = (this.QiangGouEndTicks - num) / 1000.0;
		if (num2 < 3600.0)
		{
			if (num2 >= 0.0)
			{
				result = StringUtil.substitute(Global.GetLang("{0}分钟{1}秒"), new object[]
				{
					(int)(num2 / 60.0),
					(int)(num2 % 60.0)
				});
			}
		}
		else
		{
			double num3 = num2 % 3600.0;
			result = StringUtil.substitute(Global.GetLang("{0}小时{1}分钟{2}秒"), new object[]
			{
				(int)(num2 / 3600.0),
				(int)(num3 / 60.0),
				(int)(num3 % 60.0)
			});
		}
		return result;
	}

	private void RefreshXianGouData()
	{
		if (Global.Data.MallData == null)
		{
			return;
		}
		MallHotSalePagePart mallHotSalePagePart = null;
		if (this.tabPagesDict.TryGetValue(100000, ref mallHotSalePagePart))
		{
			this.QiangGouEndTicks = mallHotSalePagePart.QiangGouEndTicks;
		}
	}

	private void SetXianGouState(int qiangGouId, int num)
	{
		for (int i = 0; i < this.ItemsListXianGou.Count; i++)
		{
			MallItem mallItem = this.ItemsListXianGou[i];
			if (mallItem.MallGoodsID == qiangGouId)
			{
				mallItem.XianGouShu = (mallItem.XianGouShu.SafeToInt32(0) - num).ToString();
				mallItem.ShengYuShu = (mallItem.ShengYuShu.SafeToInt32(0) - num).ToString();
			}
		}
	}

	private void AddEffect()
	{
		if (this.eyeEffect == null)
		{
			this.eyeEffect = Global.GetDecoration(535, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
			this.eyeEffect.Coordinate = new Point(174, 94);
		}
		if (this.loveEffect == null)
		{
			this.loveEffect = Global.GetDecoration(536, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
			this.loveEffect.Coordinate = new Point(108, 253);
		}
	}

	public void PauseAllEffect(bool pause)
	{
	}

	public void ShowSubTabpage(int idx)
	{
		this.gTabControl.SetActivePage(idx);
	}

	private const int TAG_XIANGOU = 100000;

	public ShowNetImage MallImage;

	public GTabControl gTabControl;

	public GButton btnClose;

	public GButton btnRecharge;

	public GameObject MonthCardBg;

	public GButton GoToMonthCardBtn;

	public GameObject BFGirlBg;

	public UILabel UserBdYuanBaoText;

	public UILabel UserMoneyTextBlock;

	public UILabel lblLimitTime;

	public UILabel ConstLblQianggou;

	private Dictionary<int, MallHotSalePagePart> tabPagesDict;

	private Dictionary<int, List<XElement>> mallItemsDict = new Dictionary<int, List<XElement>>();

	private Canvas Root;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int ToBuyGoodsID = -1;

	private double QiangGouEndTicks = -1.0;

	private List<string> TabNames;

	private List<int> TabIDs;

	private int CurrentSelectedTab = -1;

	private List<MallItem> ItemsListXianGou = new List<MallItem>();

	private DispatcherTimer _Timer;

	private MallHotSaleHintPart mallHotSaleHintPart;

	private XElement xmlMall;

	private List<XElement> MallItems;

	public GDecoration loveEffect;

	public GDecoration eyeEffect;

	private int StartPageIdx;

	private ObservableCollection _ItemCollection;
}
