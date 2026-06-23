using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class DiamondDig : UserControl
{
	private void RefreshZuanShi()
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spDaiBiZuanShi1, "YingGuanShiChouQu", this.ZuanShiNumber1, "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spDaiBiZuanShi2, "YingGuanShiChouQu", this.ZuanShiNumber2, "xingyunzhixing");
	}

	private void InitTextInPrefabs()
	{
		this.digOnceBtn.Text = Global.GetLang("挖一次");
		this.digTenTimesbtn.Text = Global.GetLang("挖十次");
		this.mFiftyBtn.Text = Global.GetLang("挖五十次");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.GetDiamondsDigConfig();
		this.InitDiamondPitList();
		this.RefreshZuanShi();
		this.digOnceBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.digType = DiamondDigType.DiamondDigType_Once;
			if (!this.IsDigAvailable(this.digType))
			{
				return;
			}
			this.DigDiamondRequest(this.pitType, DiamondDigType.DiamondDigType_Once);
		};
		this.digTenTimesbtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.digType = DiamondDigType.DiamondDigType_TenTimes;
			if (!this.IsDigAvailable(this.digType))
			{
				return;
			}
			this.DigDiamondRequest(this.pitType, DiamondDigType.DiamondDigType_TenTimes);
		};
		this.mFiftyBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.digType = DiamondDigType.DiamondDigType_FiftyTimes;
			if (!this.IsDigAvailable(this.digType))
			{
				return;
			}
			this.DigDiamondRequest(this.pitType, DiamondDigType.DiamondDigType_FiftyTimes);
		};
	}

	public void InitDiamondDig()
	{
		this.InitDiamondBag();
	}

	private void InitDiamondBag()
	{
		if (null == this.diamondBag)
		{
			this.diamondBag = U3DUtils.NEW<DiamondBag>();
			this.diamondBag.InitBag();
		}
		U3DUtils.AddChild(this.bag.gameObject, this.diamondBag.gameObject, true);
	}

	protected override void OnDestroy()
	{
		if (null != this.diamondBag)
		{
			Object.Destroy(this.diamondBag.gameObject);
			this.diamondBag = null;
		}
	}

	private void InitDiamondPitList()
	{
		if (this.list_pitItem == null)
		{
			return;
		}
		for (int i = 0; i < this.list_pitItem.Length; i++)
		{
			DiamondPitItem pitItem = this.list_pitItem[i];
			pitItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.pitType = (DiamondPitType)e.ID;
				if (this.lastIndex + 1 != pitItem.index)
				{
					this.list_pitItem[this.lastIndex].highlight = false;
					pitItem.highlight = true;
					this.lastIndex = pitItem.index - 1;
				}
			};
			int digCostsByID = this.GetDigCostsByID(pitItem.index);
			pitItem.SetCosts(digCostsByID);
			if (pitItem.index == 2)
			{
				this.ZuanShiNumber1 = digCostsByID;
			}
			else if (pitItem.index == 3)
			{
				this.ZuanShiNumber2 = digCostsByID;
			}
		}
	}

	private DigItemAttribute GetDigAttributeByID(int id)
	{
		if (this.dic_digAttribute == null)
		{
			return null;
		}
		return this.dic_digAttribute.GetValue(id);
	}

	private int GetDigCostsByID(int id)
	{
		if (this.dic_digAttribute == null)
		{
			return 0;
		}
		DigItemAttribute value = this.dic_digAttribute.GetValue(id);
		if (value == null)
		{
			return 0;
		}
		if (value.needPowderNum == 0)
		{
			return value.needDiamond;
		}
		return value.needPowderNum;
	}

	private int GetDigTypeByID(int id)
	{
		if (this.dic_digAttribute == null)
		{
			return 0;
		}
		DigItemAttribute value = this.dic_digAttribute.GetValue(id);
		if (value != null)
		{
			return value.type;
		}
		return 0;
	}

	private bool IsDigAvailable(DiamondDigType digType)
	{
		int digCostsByID = this.GetDigCostsByID((int)this.pitType);
		int num = digCostsByID;
		if (digType == DiamondDigType.DiamondDigType_TenTimes)
		{
			num *= 10;
		}
		else if (digType == DiamondDigType.DiamondDigType_FiftyTimes)
		{
			num *= 50;
		}
		this.zuanShiNumber = num;
		if (this.pitType == DiamondPitType.DiamondPitType_General)
		{
			return this.IsFluorescentPointAvailable(num);
		}
		return this.IsDiamondAvailable(num);
	}

	private bool IsFluorescentPointAvailable(int digCost)
	{
		int num = 31;
		int num2 = Global.Data.roleData.RoleCommonUseIntPamams[num];
		if (digCost > num2)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedFluorescentPoint, this.callback, string.Empty, string.Empty);
			return false;
		}
		return true;
	}

	private bool IsDiamondAvailable(int digCost)
	{
		if (Global.GetRoleOwnNumByMoneyType(163) < digCost && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("YingGuanShiChouQu", digCost, true))
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = digCost - Global.GetRoleOwnNumByMoneyType(163);
			string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
			GChildWindow messageBoxWindowXingYun = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindowXingYun.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindowXingYun.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindowXingYun);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
				}
				return true;
			};
			return false;
		}
		return true;
	}

	private void GetDiamondsDigConfig()
	{
		if (this.dic_digAttribute != null && this.dic_digAttribute.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/GemDigType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return;
		}
		this.dic_digAttribute = new Dictionary<int, DigItemAttribute>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			DigItemAttribute digItemAttribute = new DigItemAttribute();
			digItemAttribute.id = Global.GetXElementAttributeInt(xelement, "ID");
			digItemAttribute.type = Global.GetXElementAttributeInt(xelement, "Type");
			digItemAttribute.name = Global.GetXElementAttributeStr(xelement, "Name");
			digItemAttribute.needPowderNum = Global.GetXElementAttributeInt(xelement, "CostYingGuangFenMo");
			digItemAttribute.needDiamond = Global.GetXElementAttributeInt(xelement, "CostZuanShi");
			if (!this.dic_digAttribute.ContainsKey(digItemAttribute.id))
			{
				this.dic_digAttribute.Add(digItemAttribute.id, digItemAttribute);
			}
		}
	}

	private void ShowDiamondDigAnimationWindow()
	{
		DPSelectedItemEventHandler digTypeCallBack = delegate(object sender, DPSelectedItemEventArgs args)
		{
			if (args != null)
			{
				this.digType = (DiamondDigType)args.ID;
				this.DigDiamondRequest(this.pitType, this.digType);
			}
		};
		int digCostsByID = this.GetDigCostsByID((int)this.pitType);
		PlayZone.GlobalPlayZone.ShowDiamondDigAnimationWindow(this.pitType != DiamondPitType.DiamondPitType_General, digCostsByID, this.digType == DiamondDigType.DiamondDigType_Once, digTypeCallBack);
	}

	private void RefreshDigAnimation(List<int> list_goods, Dictionary<int, int> FiftyData = null)
	{
		if (this.digType == DiamondDigType.DiamondDigType_FiftyTimes)
		{
			if (FiftyData == null)
			{
				if (list_goods != null && 0 < list_goods.Count)
				{
					for (int i = 0; i < list_goods.Count; i++)
					{
						if (list_goods[i] != 0)
						{
							Super.HintMainText(Global.GetGoodsNameByID(list_goods[i], true) + " * 1", 50, 3);
						}
					}
				}
				GameInstance.Game.SortDiamondBag();
			}
		}
		else
		{
			if (list_goods == null || list_goods.Count <= 0)
			{
				return;
			}
			if (!PlayZone.GlobalPlayZone.IsDiamondDigAnimationWindowActive())
			{
				this.ShowDiamondDigAnimationWindow();
			}
			PlayZone.GlobalPlayZone.RefreshDiggedDiamondGoodsIconsWithType(this.digType, list_goods);
		}
	}

	public void SetDiamondDigResult(int status, List<int> list_goods, Dictionary<int, int> FiftyGoodsData)
	{
		Super.HideNetWaiting();
		this.RefreshZuanShi();
		string textMsg = string.Empty;
		switch (status + 2)
		{
		case 0:
			textMsg = Global.GetLang("功能未开启");
			break;
		case 1:
			textMsg = Global.GetLang("异常");
			break;
		case 2:
			this.RefreshDigAnimation(list_goods, FiftyGoodsData);
			break;
		case 3:
			textMsg = Global.GetLang(string.Empty);
			break;
		case 4:
			textMsg = Global.GetLang("挖掘类型错误");
			break;
		case 5:
			textMsg = Global.GetLang("背包空间不足1格");
			break;
		case 6:
			textMsg = Global.GetLang("矿坑数据异常");
			break;
		case 7:
			textMsg = Global.GetLang("荧光粉末不足");
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedFluorescentPoint, this.callback, string.Empty, string.Empty);
			return;
		case 8:
			textMsg = Global.GetLang("幸运之星不足");
			break;
		case 9:
			textMsg = Global.GetLang("更新荧光粉末失败");
			break;
		case 10:
			textMsg = Global.GetLang("更新钻石失败");
			break;
		case 11:
			textMsg = Global.GetLang("挖掘数据异常");
			break;
		case 12:
			textMsg = Global.GetLang("背包空间不足");
			break;
		case 13:
			textMsg = Global.GetLang("新增物品失败");
			break;
		case 14:
			textMsg = Global.GetLang("不是荧光宝石");
			break;
		}
		if (status != 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	public void OnDiamondBagSorted(Dictionary<int, GoodsData> dic_goods)
	{
		if (null != this.diamondBag)
		{
			this.diamondBag.SetBagSortResult(dic_goods);
		}
	}

	public void OnDiamondBagChanged(GoodsData gd, BagModifyType type)
	{
		if (null == this.diamondBag)
		{
			return;
		}
		switch (type)
		{
		case BagModifyType.BagModifyType_Add:
			this.diamondBag.AddGoods(gd);
			break;
		case BagModifyType.BagModifyType_Replace:
			this.diamondBag.ReplaceGoods(gd);
			break;
		case BagModifyType.BagModifyType_Destroy:
			this.diamondBag.RemoveGoods(gd);
			break;
		}
	}

	private void DigDiamondRequest(DiamondPitType pitType, DiamondDigType digType)
	{
		int _pitType = this.GetDigTypeByID((int)pitType);
		if ((pitType == DiamondPitType.DiamondPitType_Medium || pitType == DiamondPitType.DiamondPitType_Super) && Global.GetZuanShi(ZuanShiPartClass.YingGuangBaoShi))
		{
			string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "YingGuanShiChouQu", this.zuanShiNumber);
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.zuanShiNumber, text)
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
			{
				messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
			}
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					Global.SetZuanShi(ZuanShiPartClass.YingGuangBaoShi, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
				}
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					if (digType == DiamondDigType.DiamondDigType_Once)
					{
						GameInstance.Game.DigDiamond(_pitType, 0);
					}
					else if (digType == DiamondDigType.DiamondDigType_TenTimes)
					{
						GameInstance.Game.DigDiamond(_pitType, 1);
					}
					else if (digType == DiamondDigType.DiamondDigType_FiftyTimes)
					{
						GameInstance.Game.DigDiamond(_pitType, 2);
					}
				}
				return true;
			};
			return;
		}
		if (digType == DiamondDigType.DiamondDigType_Once)
		{
			GameInstance.Game.DigDiamond(_pitType, 0);
		}
		else if (digType == DiamondDigType.DiamondDigType_TenTimes)
		{
			GameInstance.Game.DigDiamond(_pitType, 1);
		}
		else if (digType == DiamondDigType.DiamondDigType_FiftyTimes)
		{
			GameInstance.Game.DigDiamond(_pitType, 2);
		}
	}

	private const int timesUnit = 10;

	private const int timesUnitEXFifty = 50;

	public GButton digOnceBtn;

	public GButton digTenTimesbtn;

	[SerializeField]
	private GButton mFiftyBtn;

	public GameObject bag;

	public UISprite spDaiBiZuanShi1;

	public UISprite spDaiBiZuanShi2;

	public int ZuanShiNumber1;

	public int ZuanShiNumber2;

	public DiamondPitItem[] list_pitItem;

	private int lastIndex;

	public DPSelectedItemEventHandler callback;

	private Dictionary<int, DigItemAttribute> dic_digAttribute;

	private DiamondBag diamondBag;

	private DiamondPitType pitType = DiamondPitType.DiamondPitType_General;

	private DiamondDigType digType = DiamondDigType.DiamondDigType_Once;

	private int zuanShiNumber;
}
