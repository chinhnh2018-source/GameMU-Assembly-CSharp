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

public class RecallShopPart : RecallGoodsEx
{
	public static KuaFuWangZhePart.WangzheData WangZheData
	{
		get
		{
			return KuaFuWangZhePart.wangzheData;
		}
	}

	public static int JiFen
	{
		get
		{
			return RecallShopPart.WangZheData.nJiFen;
		}
	}

	public int[] KingOfBattleStore
	{
		get
		{
			if (this._KingOfBattleStore == null)
			{
				this._KingOfBattleStore = new int[]
				{
					24,
					6,
					100
				};
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("KingOfBattleStore", ',');
				if (systemParamIntArrayByName.Length == 3)
				{
					this._KingOfBattleStore[0] = systemParamIntArrayByName[0];
					this._KingOfBattleStore[1] = systemParamIntArrayByName[1];
					this._KingOfBattleStore[2] = systemParamIntArrayByName[2];
				}
			}
			return this._KingOfBattleStore;
		}
	}

	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
		this.btnRecharge.Text = Global.GetLang("充值");
		this.btnRefresh.Text = Global.GetLang("刷新");
		this.ConstLblQianggou.text = Global.GetLang("倒计时");
		this.lblShuaXin.text = this.KingOfBattleStore[2].ToString();
		if (!Context.IsAPPVerify)
		{
			base.name = "MallHotRole.png";
		}
		else
		{
			base.name = "MallHotRoleIosVerify.png";
		}
		this.BFGirlBg.GetComponent<ShowNetImage>().ImageURL = string.Format("NetImages/GameRes/Images/Plate/{0}", base.name);
	}

	protected override void InitializeComponent()
	{
		this.tabPagesDict = new Dictionary<int, RecallShopPagePart>();
		base.InitializeComponent();
		this.Root = this.Container;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearMallData();
			this.StopHeart();
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.CloseRecallMallWindow();
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
		this.btnRefresh.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
	}

	public override void OnDragFinished()
	{
	}

	public override bool InitRewards()
	{
		if (!base.InitRewards())
		{
			return false;
		}
		this.ClearMallData();
		List<XElement> xmlList = base.xmlList;
		this.mallItemsDict.Clear();
		this.InitTabArray();
		this.mallItemsDict[0] = base.xmlList;
		this.CurrentSelectedTab = this.TabIDs[0];
		this.gTabControl.ClearPages();
		this.gTabControl.AddTabPage(Enumerable.Count<string>(this.TabNames));
		this.gTabControl.SetTabButtonBackground("chatTab_normal", "chatTab_hover");
		int num = 0;
		RecallShopPagePart recallShopPagePart = U3DUtils.NEW<RecallShopPagePart>();
		recallShopPagePart.mallItemXmlList = xmlList;
		recallShopPagePart.tabID = this.TabIDs[num];
		this.gTabControl.SetTabButtonName(this.TabNames[num], num);
		this.gTabControl.AddPageContent(recallShopPagePart.gameObject, num);
		this.tabPagesDict.Add(this.TabIDs[num], recallShopPagePart);
		if (num == 0)
		{
			recallShopPagePart.RefreshGoodsByType(-1);
			if (recallShopPagePart.tabID == recallShopPagePart.QianggouId)
			{
				recallShopPagePart.RefreshXianGouData();
			}
			else
			{
				recallShopPagePart.RefreshGoodsByType(-1);
			}
		}
		base.UpdateUIOnServerDataChanged();
		this.UpdateStroeList();
		return true;
	}

	private void InitTabArray()
	{
		this.TabNames = new List<string>();
		this.TabIDs = new List<int>();
		this.TabNames.Add("0");
		this.TabIDs.Add(100000);
		this.mallItemsDict.Add(0, new List<XElement>());
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
		this.UserJiFenBlock.text = RecallShopPart.JiFen.ToString();
	}

	public void InitPartSize(int idx)
	{
		this.StartPageIdx = idx;
		RecallShopPart.Send_CMD_SPR_KINGOFBATTLE_MALL_DATA();
	}

	private void ClearMallData()
	{
		if (this.tabPagesDict != null)
		{
			foreach (KeyValuePair<int, RecallShopPagePart> keyValuePair in this.tabPagesDict)
			{
				RecallShopPagePart value = keyValuePair.Value;
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
		RecallShopPagePart recallShopPagePart = null;
		if (this.tabPagesDict == null)
		{
			return;
		}
		if (this.tabPagesDict.TryGetValue(id, ref recallShopPagePart))
		{
			recallShopPagePart.RefreshGoodsByType(-1);
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
		this._Timer = new DispatcherTimer("RecallShop_Timer");
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

	private void ForgeTimer_Tick(object sender, object e)
	{
		if (null != this.lblLimitTime)
		{
			this.lblLimitTime.text = base.GetQiangGouLeftTimeString();
		}
	}

	public void RefreshMallData(object mallData)
	{
		this._CreateNewStroeList();
		this.RefreshXianGouData();
		this.RefreshGoodsByType(this.GetGoodsTabID(this.ToBuyGoodsID), this.ToBuyGoodsID);
		this.ToBuyGoodsID = -1;
	}

	private void RefreshXianGouData()
	{
		if (Global.Data.MallData == null)
		{
			return;
		}
		RecallShopPagePart recallShopPagePart = null;
		if (this.tabPagesDict.TryGetValue(100000, ref recallShopPagePart))
		{
			this.QiangGouEndTicks = recallShopPagePart.QiangGouEndTicks;
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

	public void _CreateNewStroeList()
	{
		this.ClearMallData();
		List<XElement> mallItemXmlList = null;
		this.mallItemsDict.Clear();
		this.InitTabArray();
		XElement gameResXml = Global.GetGameResXml("Config/KingOfBattleStore.xml");
		if (gameResXml == null)
		{
			return;
		}
		this.mallItemsDict[0] = Global.GetXElementList(gameResXml, "Store");
		this.CurrentSelectedTab = this.TabIDs[0];
		this.gTabControl.ClearPages();
		this.gTabControl.AddTabPage(Enumerable.Count<string>(this.TabNames));
		this.gTabControl.SetTabButtonBackground("chatTab_normal", "chatTab_hover");
		for (int i = 0; i < this.TabNames.Count; i++)
		{
			RecallShopPagePart recallShopPagePart = U3DUtils.NEW<RecallShopPagePart>();
			if (this.mallItemsDict.TryGetValue(this.TabIDs[i], ref mallItemXmlList))
			{
				recallShopPagePart.mallItemXmlList = mallItemXmlList;
			}
			recallShopPagePart.tabID = this.TabIDs[i];
			this.gTabControl.SetTabButtonName(this.TabNames[i], i);
			this.gTabControl.AddPageContent(recallShopPagePart.gameObject, i);
			this.tabPagesDict.Add(this.TabIDs[i], recallShopPagePart);
			if (i == 0)
			{
				recallShopPagePart.RefreshGoodsByType(-1);
				if (recallShopPagePart.tabID == recallShopPagePart.QianggouId)
				{
					recallShopPagePart.RefreshXianGouData();
				}
				else
				{
					recallShopPagePart.RefreshGoodsByType(-1);
				}
			}
		}
		this.RefreshUserMoney();
	}

	public void UpdateStroeList()
	{
		RecallShopPagePart recallShopPagePart = null;
		if (this.tabPagesDict.TryGetValue(100000, ref recallShopPagePart))
		{
			MallItem mallItem = null;
			for (int i = 0; i < recallShopPagePart.listBox.Count(); i++)
			{
				mallItem = U3DUtils.AS<MallItem>(recallShopPagePart.listBox.GetItemByIndex(i));
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
		this.RefreshUserMoney();
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_MALL_DATA()
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_MALL_DATA(MUSocketConnectEventArgs e)
	{
	}

	public static void Send_CMD_SPR_KINGOFBATTLE_MALL_BUY(int xmlID, int countNum)
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_MALL_BUY(MUSocketConnectEventArgs e)
	{
	}

	private const int TAG_XIANGOU = 100000;

	public GTabControl gTabControl;

	public GButton btnClose;

	public GButton btnRecharge;

	public GButton btnRefresh;

	public GameObject MonthCardBg;

	public GButton GoToMonthCardBtn;

	public GameObject BFGirlBg;

	public UILabel UserBdYuanBaoText;

	public UILabel UserMoneyTextBlock;

	public UILabel UserJiFenBlock;

	public UILabel lblLimitTime;

	public UILabel ConstLblQianggou;

	public UILabel lblShuaXin;

	private Dictionary<int, RecallShopPagePart> tabPagesDict;

	private Dictionary<int, List<XElement>> mallItemsDict = new Dictionary<int, List<XElement>>();

	private Canvas Root;

	private int ToBuyGoodsID = -1;

	private double QiangGouEndTicks = -1.0;

	private List<string> TabNames;

	private List<int> TabIDs;

	private int CurrentSelectedTab = -1;

	private List<MallItem> ItemsListXianGou = new List<MallItem>();

	private DispatcherTimer _Timer;

	private MallHotSaleHintPart mallHotSaleHintPart;

	private List<XElement> MallItems;

	public GDecoration loveEffect;

	public GDecoration eyeEffect;

	private int StartPageIdx;

	private int[] _KingOfBattleStore;
}
