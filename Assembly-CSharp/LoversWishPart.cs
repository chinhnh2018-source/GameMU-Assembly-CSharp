using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class LoversWishPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Btns[0].Label.text = Global.GetLang("祝福榜");
		this.Btns[1].Label.text = Global.GetLang("祝福记录");
		this.BackGround.URL = "NetImages/GameRes/Images/Wish/ZhuFuBangBG.jpg.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitTab(0);
		this.SetBtnState(0);
		this.RefDiamondNum();
		this.Btns[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.currentTab != 0)
			{
				this.InitTab(0);
				this.SetBtnState(0);
			}
		};
		this.Btns[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.isJiehun())
			{
				string lang = Global.GetLang("未结婚状态无法获得祝福");
				Super.HintMainText(lang, 10, 3);
				return;
			}
			if (this.currentTab != 1)
			{
				this.InitTab(1);
				this.SetBtnState(1);
			}
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs());
		};
	}

	public void InitZuanShiNum()
	{
		this.DiamondNum.text = Global.Data.roleData.UserMoney.ToString();
	}

	public void InitZhuFuBangTab(CoupleWishMainData Data)
	{
		if (Data == null)
		{
			return;
		}
		this.CurrentMainData = Data;
		if (this.loversWishPart_Wish != null)
		{
			this.loversWishPart_Wish.InitTab(Data);
		}
		if (LoversWishPart.GetWishAwardDic().ContainsKey(Data.CanGetAwardId))
		{
			this.OpenGetAwardInterFace(Data.CanGetAwardId);
		}
	}

	public void InitZhuFuJiLuTab(List<CoupleWishWishRecordData> Data)
	{
		if (this.loversWishPart_Record != null && Data != null && Data.Count > 0)
		{
			this.loversWishPart_Record.InitList(Data);
		}
		if (this.CurrentMainData != null)
		{
			this.loversWishPart_Record.InitLeftAttr(this.CurrentMainData);
		}
	}

	private bool isJiehun()
	{
		return GameInstance.Game.CurrentSession != null && GameInstance.Game.CurrentSession.MarriageData != null && (int)GameInstance.Game.CurrentSession.MarriageData.byMarrytype != -1;
	}

	public void RefDiamondNum()
	{
		this.DiamondNum.text = Global.Data.roleData.UserMoney.ToString();
	}

	private void InitTab(int index = 0)
	{
		this.DesObjWindow();
		this.currentTab = index;
		if (index != 0)
		{
			if (index == 1)
			{
				this.OpenLoversWishPart_RecordWindow();
			}
		}
		else
		{
			this.OpenLoversWishPart_WishWindow();
		}
	}

	private void OpenLoversWishPart_WishWindow()
	{
		if (this.loversWishPart_Wish == null)
		{
			this.loversWishPart_Wish = U3DUtils.NEW<LoversWishPart_Wish>();
			U3DUtils.AddChild(this.Pan.gameObject, this.loversWishPart_Wish.gameObject, true);
		}
	}

	private void OpenLoversWishPart_RecordWindow()
	{
		if (this.loversWishPart_Record == null)
		{
			this.loversWishPart_Record = U3DUtils.NEW<LoversWishPart_Record>();
			U3DUtils.AddChild(this.Pan.gameObject, this.loversWishPart_Record.gameObject, true);
		}
	}

	private void OpenGetAwardInterFace(int id)
	{
		if (this.GetAwardWindow == null)
		{
			this.GetAwardWindow = U3DUtils.NEW<GChildWindow>();
			this.GetAwardWindow.IsShowModal = true;
			this.GetAwardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.GetAwardWindow, Global.GetLang("获奖励界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.GetAwardWindow);
		}
		if (this.GetAward == null)
		{
			this.GetAward = U3DUtils.NEW<LoversWishPartGetAward>();
			this.GetAward.InitListItem(id);
			this.GetAward.DPSHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseGetAwardInterFace();
			};
		}
		this.GetAwardWindow.SetContent(this.GetAwardWindow.BodyPresenter, this.GetAward, 0.0, 0.0, true);
	}

	private void CloseGetAwardInterFace()
	{
		if (this.GetAward)
		{
			this.GetAward.transform.parent = null;
			Object.Destroy(this.GetAward.gameObject);
			this.GetAward = null;
		}
		if (this.GetAwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.GetAwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.GetAwardWindow, true);
			this.GetAwardWindow = null;
		}
	}

	private void DesObjWindow()
	{
		if (this.Pan.transform.childCount > 0)
		{
			for (int i = 0; i < this.Pan.transform.childCount; i++)
			{
				Object.Destroy(this.Pan.transform.GetChild(i).gameObject);
			}
		}
	}

	private void SetBtnState(int index)
	{
		for (int i = 0; i < this.Btns.Length; i++)
		{
			if (index == i)
			{
				this.Btns[i].Label.color = NGUIMath.HexToColorEx(14922604U);
				this.Btns[i].Pressed = true;
				this.Btns[i].Refresh();
			}
			else
			{
				this.Btns[i].Label.color = NGUIMath.HexToColorEx(10323559U);
				this.Btns[i].Pressed = false;
				this.Btns[i].Refresh();
			}
		}
	}

	public static void ClearXMLDataOnChageScene()
	{
		if (LoversWishPart.WishAwardDic.Count > 0)
		{
			LoversWishPart.WishAwardDic.Clear();
		}
	}

	public static Dictionary<int, WishAward> GetWishAwardDic()
	{
		if (LoversWishPart.WishAwardDic.Count > 0)
		{
			return LoversWishPart.WishAwardDic;
		}
		XElement gameResXml = Global.GetGameResXml("Config/WishAward.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WishAward");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			WishAward wishAward = new WishAward();
			wishAward.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			wishAward.StarNum = Global.GetXElementAttributeInt(xelementList[i], "StarNum");
			wishAward.EndNum = Global.GetXElementAttributeInt(xelementList[i], "EndNum");
			wishAward.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			wishAward.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
			wishAward.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
			wishAward.MinWishNum = Global.GetXElementAttributeInt(xelementList[i], "MinWishNum");
			if (!LoversWishPart.WishAwardDic.ContainsKey(wishAward.ID))
			{
				LoversWishPart.WishAwardDic.Add(wishAward.ID, wishAward);
			}
			i++;
		}
		return LoversWishPart.WishAwardDic;
	}

	public static void ClearXMLDataOnLoadConfig()
	{
		if (0 < LoversWishPart.WishTypeDic.Count)
		{
			LoversWishPart.WishTypeDic.Clear();
		}
	}

	public static Dictionary<int, WishType> GetWishTypeDic()
	{
		if (LoversWishPart.WishTypeDic.Count > 0)
		{
			return LoversWishPart.WishTypeDic;
		}
		XElement gameResXml = Global.GetGameResXml("Config/WishType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "WishType");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			WishType wishType = new WishType();
			wishType.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			wishType.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			wishType.ItemNum = Global.GetXElementAttributeStr(xelementList[i], "ItemNum");
			wishType.ZhuanShi = Global.GetXElementAttributeInt(xelementList[i], "ZhuanShi");
			wishType.WishNum = Global.GetXElementAttributeInt(xelementList[i], "WishNum");
			wishType.WishIntro = Global.GetXElementAttributeInt(xelementList[i], "WishIntro");
			wishType.Effect = Global.GetXElementAttributeInt(xelementList[i], "Effect");
			wishType.EffectCD = Global.GetXElementAttributeInt(xelementList[i], "EffectCD");
			if (!LoversWishPart.WishTypeDic.ContainsKey(wishType.ID))
			{
				LoversWishPart.WishTypeDic.Add(wishType.ID, wishType);
			}
			i++;
		}
		return LoversWishPart.WishTypeDic;
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public GButton[] Btns;

	public GButton BtnClose;

	public ShowNetImage BackGround;

	public GameObject Pan;

	public UILabel DiamondNum;

	private int currentTab;

	private LoversWishPart_Wish loversWishPart_Wish;

	private LoversWishPart_Record loversWishPart_Record;

	private GChildWindow GetAwardWindow;

	private LoversWishPartGetAward GetAward;

	private CoupleWishMainData CurrentMainData;

	private static Dictionary<int, WishAward> WishAwardDic = new Dictionary<int, WishAward>();

	private static Dictionary<int, WishType> WishTypeDic = new Dictionary<int, WishType>();
}
