using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class KuaFuWangZheMallPart : UserControl
{
	public static KingOfBattleStoreData ServerStoreData
	{
		get
		{
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.kuaFuWangZheMallPart != null)
			{
				return PlayZone.GlobalPlayZone.kuaFuWangZheMallPart.serverStoreData;
			}
			return null;
		}
	}

	public static KingOfBattleStoreSaleData ServerSaleDataOf(int ID)
	{
		if (KuaFuWangZheMallPart.ServerStoreData == null || KuaFuWangZheMallPart.ServerStoreData.SaleList == null)
		{
			return null;
		}
		foreach (KingOfBattleStoreSaleData kingOfBattleStoreSaleData in KuaFuWangZheMallPart.ServerStoreData.SaleList)
		{
			if (kingOfBattleStoreSaleData.ID == ID)
			{
				return kingOfBattleStoreSaleData;
			}
		}
		return null;
	}

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
			return KuaFuWangZheMallPart.WangZheData.nJiFen;
		}
	}

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

	private void InitTextInPrefabs()
	{
		this.btnRecharge.Text = Global.GetLang("充值");
		this.btnRefresh.Text = Global.GetLang("刷新");
		this.ConstLblQianggou.text = Global.GetLang("商品刷新倒计时");
		this.lblShuaXin.text = this.KingOfBattleStore[2].ToString();
		this.zuanshixiaohao.text = Global.GetLang("消耗");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.tabPagesDict = new Dictionary<int, KuaFuWangZhePagePart>();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearMallData();
			this.StopHeart();
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.CloseKuaFuWangZheMallWindow();
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
			KuaFuWangZheMallPart.Send_CMD_SPR_KINGOFBATTLE_MALL_REFRESH();
		};
		this.StartHeart();
		this.BFGirlBg.SetActive(true);
	}

	private void InitTabArray()
	{
		this.TabNames = new List<string>();
		this.TabIDs = new List<int>();
		this.TabNames.Add("0");
		this.TabIDs.Add(0);
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
		this.UserJiFenBlock.text = KuaFuWangZheMallPart.JiFen.ToString();
	}

	public void InitPartSize(int idx)
	{
		this.StartPageIdx = idx;
		KuaFuWangZheMallPart.Send_CMD_SPR_KINGOFBATTLE_MALL_DATA();
	}

	private void ClearMallData()
	{
		if (this.tabPagesDict != null)
		{
			foreach (KeyValuePair<int, KuaFuWangZhePagePart> keyValuePair in this.tabPagesDict)
			{
				KuaFuWangZhePagePart value = keyValuePair.Value;
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
		KuaFuWangZhePagePart kuaFuWangZhePagePart = null;
		if (this.tabPagesDict.TryGetValue(id, ref kuaFuWangZhePagePart))
		{
			kuaFuWangZhePagePart.RefreshGoodsByType(-1);
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
		this._Timer = new DispatcherTimer("Wangzhe_Timer");
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
			this.lblLimitTime.text = this.GetQiangGouLeftTimeString();
		}
	}

	private string GetQiangGouLeftTimeString()
	{
		string result = Global.GetLang("已结束");
		if (KuaFuWangZheMallPart.ServerStoreData == null || false)
		{
			return result;
		}
		double num = (double)(this.KingOfBattleStore[0] * 3600);
		double num2 = (double)(Global.GetCorrectDateTime().Ticks / 10000L);
		double num3 = (double)(KuaFuWangZheMallPart.ServerStoreData.LastRefTime.Ticks / 10000L);
		double num4 = num - (num2 - num3) / 1000.0;
		if (num4 >= 0.0)
		{
			if (num4 < 3600.0)
			{
				if (num4 >= 0.0)
				{
					result = StringUtil.substitute(Global.GetLang("{0}分钟{1}秒"), new object[]
					{
						(int)(num4 / 60.0),
						(int)(num4 % 60.0)
					});
				}
			}
			else
			{
				double num5 = num4 % 3600.0;
				result = StringUtil.substitute(Global.GetLang("{0}小时{1}分钟{2}秒"), new object[]
				{
					(int)(num4 / 3600.0),
					(int)(num5 / 60.0),
					(int)(num5 % 60.0)
				});
			}
		}
		return result;
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
		KuaFuWangZhePagePart kuaFuWangZhePagePart = null;
		if (this.tabPagesDict.TryGetValue(100000, ref kuaFuWangZhePagePart))
		{
			this.QiangGouEndTicks = kuaFuWangZhePagePart.QiangGouEndTicks;
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
			KuaFuWangZhePagePart kuaFuWangZhePagePart = U3DUtils.NEW<KuaFuWangZhePagePart>();
			if (this.mallItemsDict.TryGetValue(this.TabIDs[i], ref mallItemXmlList))
			{
				kuaFuWangZhePagePart.mallItemXmlList = mallItemXmlList;
			}
			kuaFuWangZhePagePart.tabID = this.TabIDs[i];
			this.gTabControl.SetTabButtonName(this.TabNames[i], i);
			this.gTabControl.AddPageContent(kuaFuWangZhePagePart.gameObject, i);
			this.tabPagesDict.Add(this.TabIDs[i], kuaFuWangZhePagePart);
			if (i == 0)
			{
				kuaFuWangZhePagePart.RefreshGoodsByType(-1);
				if (kuaFuWangZhePagePart.tabID == kuaFuWangZhePagePart.QianggouId)
				{
					kuaFuWangZhePagePart.RefreshXianGouData();
				}
				else
				{
					kuaFuWangZhePagePart.RefreshGoodsByType(-1);
				}
			}
		}
		this.RefreshUserMoney();
	}

	private void _UpdateStroeList()
	{
		if (KuaFuWangZheMallPart.ServerStoreData != null && KuaFuWangZheMallPart.ServerStoreData.SaleList != null)
		{
			KuaFuWangZhePagePart kuaFuWangZhePagePart = null;
			if (this.tabPagesDict.TryGetValue(0, ref kuaFuWangZhePagePart))
			{
				foreach (KingOfBattleStoreSaleData kingOfBattleStoreSaleData in KuaFuWangZheMallPart.ServerStoreData.SaleList)
				{
					kuaFuWangZhePagePart.RefreshOnsaleInfo(kingOfBattleStoreSaleData.ID, kingOfBattleStoreSaleData.Purchase);
				}
			}
		}
		this.RefreshUserMoney();
	}

	public MallItem GetMallItemByGoodID(int goodID)
	{
		if (PlayZone.GlobalPlayZone == null || PlayZone.GlobalPlayZone.kuaFuWangZheMallPart == null)
		{
			return null;
		}
		MallItem result = null;
		if (KuaFuWangZheMallPart.ServerStoreData != null && KuaFuWangZheMallPart.ServerStoreData.SaleList != null)
		{
			KuaFuWangZhePagePart kuaFuWangZhePagePart = null;
			if (this.tabPagesDict.TryGetValue(0, ref kuaFuWangZhePagePart))
			{
				foreach (MallItem mallItem in kuaFuWangZhePagePart.mallItemList)
				{
					if (mallItem.MallGoodsID == goodID)
					{
						result = mallItem;
						break;
					}
				}
			}
		}
		return result;
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_MALL_DATA()
	{
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_MALL_DATA.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_MALL_DATA(MUSocketConnectEventArgs e)
	{
		KingOfBattleStoreData kingOfBattleStoreData = DataHelper.BytesToObject<KingOfBattleStoreData>(e.bytesData, 0, e.bytesData.Length);
		if (kingOfBattleStoreData != null && PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.kuaFuWangZheMallPart != null)
		{
			PlayZone.GlobalPlayZone.kuaFuWangZheMallPart.serverStoreData = kingOfBattleStoreData;
			if (PlayZone.GlobalPlayZone.kuaFuWangZheMallPart.mallItemsDict.Count == 0)
			{
				PlayZone.GlobalPlayZone.kuaFuWangZheMallPart._CreateNewStroeList();
			}
			else
			{
				PlayZone.GlobalPlayZone.kuaFuWangZheMallPart._UpdateStroeList();
			}
		}
	}

	public static void Send_CMD_SPR_KINGOFBATTLE_MALL_BUY(int xmlID, int countNum)
	{
		string strcmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
		{
			Global.Data.roleData.RoleID,
			xmlID,
			countNum
		});
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_MALL_BUY.SendData(strcmd);
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_MALL_BUY(MUSocketConnectEventArgs e)
	{
		int num = KuaFuWangZheMallPart.ServerErrorProcess(e);
		if (num == -1)
		{
			return;
		}
		if (num == 0)
		{
			KuaFuWangZheMallPart.Send_CMD_SPR_KINGOFBATTLE_MALL_DATA();
		}
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_MALL_REFRESH()
	{
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_MALL_REFRESH.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_MALL_REFRESH(MUSocketConnectEventArgs e)
	{
		if (KuaFuWangZheMallPart.ServerErrorProcess(e) == 0 && PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.kuaFuWangZheMallPart != null)
		{
			PlayZone.GlobalPlayZone.kuaFuWangZheMallPart._CreateNewStroeList();
		}
	}

	public static int ServerErrorProcess(MUSocketConnectEventArgs e)
	{
		string[] fields = e.fields;
		if (fields == null || fields.Length < 1)
		{
			return -1;
		}
		KingOfBattleErrorCode result = Global.SafeConvertToInt32(fields[0]);
		MUDebug.Log<string>(new string[]
		{
			"ServerErrorProcess result: "
		});
		string text = string.Empty;
		switch (result)
		{
		case 0:
			text = string.Format(Global.GetLang("成功"), new object[0]);
			break;
		case 1:
			text = string.Format(Global.GetLang("王者点数不够"), new object[0]);
			break;
		case 2:
			text = string.Format(Global.GetLang("背包空间不够"), new object[0]);
			break;
		case 3:
			text = string.Format(Global.GetLang("非售卖商品"), new object[0]);
			break;
		case 4:
			text = string.Format(Global.GetLang("传来的参数错误"), new object[0]);
			break;
		case 5:
			text = string.Format(Global.GetLang("数据库出错"), new object[0]);
			break;
		case 6:
			text = string.Format(Global.GetLang("限购数量达到上限"), new object[0]);
			break;
		case 7:
			text = string.Format(Global.GetLang("钻石不足"), new object[0]);
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, text, 0, -1, -1, 0);
		}
		return result;
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

	public UILabel zuanshixiaohao;

	private Dictionary<int, KuaFuWangZhePagePart> tabPagesDict;

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

	private List<XElement> MallItems;

	public GDecoration loveEffect;

	public GDecoration eyeEffect;

	private int StartPageIdx;

	private KingOfBattleStoreData serverStoreData;

	private ObservableCollection _ItemCollection;

	private int[] _KingOfBattleStore;
}
