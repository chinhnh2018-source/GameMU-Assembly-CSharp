using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ShenShiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.InitAttr();
		GameInstance.Game.GetJieriFanbeiInfo();
		this.BtnFuWenXiangQian.Label.text = Global.GetLang("符文镶嵌");
		this.BtnZhuangBeiShenShi.Label.text = Global.GetLang("佩戴神识");
		this.BtnFuWenChouQu.Label.text = Global.GetLang("符文抽取");
		this.BtnFuWenZongLan.Label.text = Global.GetLang("符文总览");
		this.BtnFuWenXiangQian.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnZhuangBeiShenShi.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnFuWenChouQu.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnFuWenZongLan.Label.color = NGUIMath.HexToColorEx(8350293U);
		Global.SetSprite(base.gameObject);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetBtnActieve(this.BtnFuWenXiangQian);
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnFuWenXiangQian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenFuWenXiangQianWindow(1);
			this.SetBtnActieve(this.BtnFuWenXiangQian);
		};
		this.BtnZhuangBeiShenShi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenFuWenXiangQianWindow(2);
			this.SetBtnActieve(this.BtnZhuangBeiShenShi);
		};
		this.BtnFuWenChouQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenFuWenXiangQianWindow(3);
			this.SetBtnActieve(this.BtnFuWenChouQu);
		};
		this.BtnFuWenZongLan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenFuWenXiangQianWindow(4);
			this.SetBtnActieve(this.BtnFuWenZongLan);
		};
		GameInstance.Game.GetShenShiMainData();
		GameInstance.Game.GetFuWenList();
		GameInstance.Game.GetFuWenTabList();
	}

	public void OpenChouQuFuWenWindow()
	{
		this.OpenFuWenXiangQianWindow(3);
		this.SetBtnActieve(this.BtnFuWenChouQu);
	}

	public void OpenPeiDaiShenShiWindow()
	{
		this.OpenFuWenXiangQianWindow(2);
		this.SetBtnActieve(this.BtnZhuangBeiShenShi);
	}

	public void InitShenShiMainData(int id, DateTime nextfreetime)
	{
		this.FuWenTabId = id;
		this.NextFreeTime = nextfreetime;
		this.OpenFuWenXiangQianWindow(1);
	}

	public void ReInitTabId(int id)
	{
		this.FuWenTabId = id;
		if (this.FuWenXiangQianWindow)
		{
			this.FuWenXiangQianWindow.oldTabID = this.FuWenTabId;
			this.FuWenXiangQianWindow.fuwenyeID = this.FuWenTabId;
			this.FuWenXiangQianWindow.Init();
		}
	}

	public void InitShenShiChouQuData(FuWenChouQuResult data)
	{
		if (data == null)
		{
			return;
		}
		if (data.Result < 0)
		{
			string errMsg = StdErrorCode.GetErrMsg(data.Result, false, false);
			Super.HintMainText(errMsg, 10, 3);
		}
		this.NextFreeTime = data.FreeTime;
		if (this.FuWenChouQuWindow)
		{
			this.FuWenChouQuWindow.OnQiFuCompleted(data.ChouQuCount, data.GoodsList);
			this.FuWenChouQuWindow.nextTime = this.NextFreeTime;
			if (this.NextFreeTime.Ticks - Global.GetCorrectDateTime().Ticks > 0L)
			{
				this.FuWenChouQuWindow.StartUITimer();
			}
		}
	}

	public void InitAttr()
	{
		if ((this.pageNum == 3 && !Context.IsHaiwai) || (Context.IsHaiwai && IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan("FuWenChouQu")))
		{
			this.zuanShiSprite.spriteName = "xingyunzhixing";
			this.ZuanShiNum.text = Global.GetRoleOwnNumByMoneyType(163).ToString();
		}
		else
		{
			this.zuanShiSprite.spriteName = "diamond";
			this.ZuanShiNum.text = Global.Data.roleData.UserMoney.ToString();
		}
		if (Global.Data.roleData.RoleCommonUseIntPamams.Count > 49)
		{
			this.FuWenZhiChenNum.text = Global.Data.roleData.RoleCommonUseIntPamams[49].ToString();
		}
	}

	public void SetBtnActieve(GButton btn)
	{
		if (btn == this.m_BtnLastSelect)
		{
			return;
		}
		if (null != btn)
		{
			btn.Label.color = NGUIMath.HexToColorEx(16434701U);
			btn.normalSprite = "tabhover";
			btn.Refresh();
		}
		if (null != this.m_BtnLastSelect)
		{
			this.m_BtnLastSelect.Label.color = NGUIMath.HexToColorEx(8350293U);
			this.m_BtnLastSelect.normalSprite = "tabnormal";
			this.m_BtnLastSelect.Refresh();
		}
		this.m_BtnLastSelect = btn;
	}

	private void ClearTabChildren()
	{
		if (this.TabTrans != null && this.TabTrans.childCount != 0)
		{
			int i = 0;
			int childCount = this.TabTrans.childCount;
			while (i < childCount)
			{
				Object.Destroy(this.TabTrans.GetChild(i).gameObject);
				i++;
			}
			this.FuWenXiangQianWindow = null;
			this.ShenShiZhuangBeiWindow = null;
			this.FuWenChouQuWindow = null;
			this.FuWenZongLanWindow = null;
		}
	}

	private void OpenFuWenXiangQianWindow(int type)
	{
		this.pageNum = type;
		switch (type)
		{
		case 1:
			if (this.FuWenXiangQianWindow == null)
			{
				this.ClearTabChildren();
				this.FuWenXiangQianWindow = U3DUtils.NEW<ShenShiPartFuWenXiangQian>();
				this.FuWenXiangQianWindow.transform.SetParent(this.TabTrans, false);
				this.FuWenXiangQianWindow.oldTabID = this.FuWenTabId;
				this.FuWenXiangQianWindow.fuwenyeID = this.FuWenTabId;
				this.FuWenXiangQianWindow.Init();
			}
			break;
		case 2:
			if (this.ShenShiZhuangBeiWindow == null)
			{
				this.ClearTabChildren();
				this.ShenShiZhuangBeiWindow = U3DUtils.NEW<ShenShiPartShenShiZhuangBei>();
				this.ShenShiZhuangBeiWindow.FuWenTabID = this.FuWenTabId;
				this.ShenShiZhuangBeiWindow.transform.SetParent(this.TabTrans, false);
			}
			break;
		case 3:
			if (this.FuWenChouQuWindow == null)
			{
				this.ClearTabChildren();
				this.FuWenChouQuWindow = U3DUtils.NEW<ShenShiPartFuWenChouQu>();
				this.FuWenChouQuWindow.nextTime = this.NextFreeTime;
				if (this.NextFreeTime.Ticks - Global.GetCorrectDateTime().Ticks > 0L)
				{
					this.FuWenChouQuWindow.StartUITimer();
				}
				this.FuWenChouQuWindow.transform.SetParent(this.TabTrans, false);
			}
			break;
		case 4:
			if (this.FuWenZongLanWindow == null)
			{
				this.ClearTabChildren();
				this.FuWenZongLanWindow = U3DUtils.NEW<ShenShiPartFuWenZongLan>();
				this.FuWenZongLanWindow.transform.SetParent(this.TabTrans, false);
			}
			break;
		}
		this.InitAttr();
	}

	public static Dictionary<int, FuWen> GetDicFuWen()
	{
		if (ShenShiPart.dicFuWen.Count > 0)
		{
			return ShenShiPart.dicFuWen;
		}
		XElement gameResXml = Global.GetGameResXml("Config/FuWen.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "FuWen");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			FuWen fuWen = new FuWen();
			fuWen.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			fuWen.GoodsID = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
			fuWen.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			fuWen.Type = Global.GetXElementAttributeStr(xelementList[i], "Type");
			fuWen.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			fuWen.Blue = Global.GetXElementAttributeInt(xelementList[i], "Blue");
			fuWen.Red = Global.GetXElementAttributeInt(xelementList[i], "Red");
			fuWen.Green = Global.GetXElementAttributeInt(xelementList[i], "Green");
			fuWen.PayNum = Global.GetXElementAttributeInt(xelementList[i], "PayNum");
			fuWen.SendNum = Global.GetXElementAttributeInt(xelementList[i], "SendNum");
			if (!ShenShiPart.dicFuWen.ContainsKey(fuWen.GoodsID))
			{
				ShenShiPart.dicFuWen.Add(fuWen.GoodsID, fuWen);
			}
			i++;
		}
		return ShenShiPart.dicFuWen;
	}

	public static Dictionary<int, FuWenHole> GetDicFuWenHole()
	{
		if (ShenShiPart.dicFuWenHole.Count > 0)
		{
			return ShenShiPart.dicFuWenHole;
		}
		XElement gameResXml = Global.GetGameResXml("Config/FuWenHole.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "FuWenHole");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			FuWenHole fuWenHole = new FuWenHole();
			fuWenHole.HoleID = Global.GetXElementAttributeInt(xelementList[i], "HoleID");
			fuWenHole.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			fuWenHole.Type = Global.GetXElementAttributeStr(xelementList[i], "Type");
			fuWenHole.OpenLevel = Global.GetXElementAttributeStr(xelementList[i], "OpenLevel");
			if (!ShenShiPart.dicFuWenHole.ContainsKey(fuWenHole.HoleID))
			{
				ShenShiPart.dicFuWenHole.Add(fuWenHole.HoleID, fuWenHole);
			}
			i++;
		}
		return ShenShiPart.dicFuWenHole;
	}

	public static Dictionary<int, Dictionary<int, FuWenGod>> GetDicFuWenGod()
	{
		if (ShenShiPart.dicFuWenGod.Count > 0)
		{
			return ShenShiPart.dicFuWenGod;
		}
		XElement gameResXml = Global.GetGameResXml("Config/FuWenGod.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "FuWenGod");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			FuWenGod fuWenGod = new FuWenGod();
			fuWenGod.GodID = Global.GetXElementAttributeInt(xelementList[i], "GodID");
			fuWenGod.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			fuWenGod.Intro = Global.GetXElementAttributeStr(xelementList[i], "Intro");
			fuWenGod.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			fuWenGod.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			fuWenGod.NeedBlue = Global.GetXElementAttributeInt(xelementList[i], "NeedBlue");
			fuWenGod.NeedRed = Global.GetXElementAttributeInt(xelementList[i], "NeedRed");
			fuWenGod.NeedGreen = Global.GetXElementAttributeInt(xelementList[i], "NeedGreen");
			if (!ShenShiPart.dicFuWenGod.ContainsKey(fuWenGod.Type))
			{
				Dictionary<int, FuWenGod> dictionary = new Dictionary<int, FuWenGod>();
				dictionary.Add(fuWenGod.Level, fuWenGod);
				ShenShiPart.dicFuWenGod.Add(fuWenGod.Type, dictionary);
			}
			else
			{
				ShenShiPart.dicFuWenGod[fuWenGod.Type].Add(fuWenGod.Level, fuWenGod);
			}
			i++;
		}
		return ShenShiPart.dicFuWenGod;
	}

	public static string GetErrMsg(int errCode)
	{
		string chineseText = Global.GetLang("未知错误...错误码[") + errCode + "]";
		if (errCode < 0)
		{
			switch (errCode + 22)
			{
			case 0:
				chineseText = string.Empty;
				break;
			case 1:
				chineseText = Global.GetLang("超过购买上限");
				break;
			case 2:
				chineseText = Global.GetLang("名字不合法");
				break;
			case 3:
				chineseText = Global.GetLang("战斗状态不能操作");
				break;
			case 4:
				chineseText = Global.GetLang("符文之尘不够");
				break;
			case 5:
				chineseText = Global.GetLang("钻石不够");
				break;
			case 6:
				chineseText = Global.GetLang("背包满");
				break;
			case 7:
				chineseText = Global.GetLang("技能未装备");
				break;
			case 8:
				chineseText = Global.GetLang("无法装备技能");
				break;
			case 9:
				chineseText = Global.GetLang("神识还没激活");
				break;
			case 10:
				chineseText = Global.GetLang("神识激活超过上限");
				break;
			case 11:
				chineseText = Global.GetLang("神识已经激活");
				break;
			case 12:
				chineseText = Global.GetLang("神识不可激活");
				break;
			case 13:
				chineseText = Global.GetLang("数据库操作失败");
				break;
			case 14:
				chineseText = Global.GetLang("符文装备超过上限");
				break;
			case 15:
				chineseText = Global.GetLang("不是符文装备");
				break;
			case 16:
				chineseText = Global.GetLang("符文槽类型不一致");
				break;
			case 17:
				chineseText = Global.GetLang("配置出错");
				break;
			case 18:
				chineseText = Global.GetLang("等级没达到要求");
				break;
			case 19:
				chineseText = Global.GetLang("没找到符文装备");
				break;
			case 20:
				chineseText = Global.GetLang("没装备符文");
				break;
			case 21:
				chineseText = Global.GetLang("符文页没开启");
				break;
			default:
				chineseText = Global.GetLang("其他错误！错误码[") + errCode + "]";
				break;
			}
		}
		return Global.GetLang(chineseText);
	}

	public static void ClearXMLData()
	{
		if (0 < ShenShiPart.dicFuWen.Count)
		{
			ShenShiPart.dicFuWen.Clear();
		}
		if (0 < ShenShiPart.dicFuWenHole.Count)
		{
			ShenShiPart.dicFuWenHole.Clear();
		}
		if (0 < ShenShiPart.dicFuWenGod.Count)
		{
			ShenShiPart.dicFuWenGod.Clear();
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public GButton BtnFuWenXiangQian;

	public GButton BtnZhuangBeiShenShi;

	public GButton BtnFuWenChouQu;

	public GButton BtnFuWenZongLan;

	public UISprite zuanShiSprite;

	public UILabel ZuanShiNum;

	public UILabel FuWenZhiChenNum;

	public Transform TabTrans;

	private int FuWenTabId;

	private DateTime NextFreeTime;

	private int pageNum;

	private GButton m_BtnLastSelect;

	public ShenShiPartFuWenXiangQian FuWenXiangQianWindow;

	public ShenShiPartShenShiZhuangBei ShenShiZhuangBeiWindow;

	public ShenShiPartFuWenChouQu FuWenChouQuWindow;

	public ShenShiPartFuWenZongLan FuWenZongLanWindow;

	private static Dictionary<int, FuWen> dicFuWen = new Dictionary<int, FuWen>();

	private static Dictionary<int, FuWenHole> dicFuWenHole = new Dictionary<int, FuWenHole>();

	private static Dictionary<int, Dictionary<int, FuWenGod>> dicFuWenGod = new Dictionary<int, Dictionary<int, FuWenGod>>();
}
