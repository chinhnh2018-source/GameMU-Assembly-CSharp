using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuPart : UserControl
{
	public int Page
	{
		get
		{
			return this.m_Page;
		}
		set
		{
			this.m_Page = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitText();
		this.InitOnClick();
		this.InitValue();
		GameInstance.Game.SenZhuTiFuXmlData();
	}

	private void InitText()
	{
	}

	private void InitValue()
	{
		this.m_BtnhuodongListOBC = this.m_BtnhuodongList.ItemsSource;
	}

	private void InitOnClick()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_HandlerClose(this, new DPSelectedItemEventArgs());
		};
	}

	protected override void OnDestroy()
	{
		if (this.m_BtnhuodongListOBC != null)
		{
			for (int i = 0; i < this.m_BtnhuodongListOBC.Count; i++)
			{
				ZhuTiFuItem component = this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>();
				Object.Destroy(component.gameObject);
			}
			this.m_BtnhuodongListOBC = null;
		}
		ActivityTipManager.RegActivityTipItem(11501, null);
		ActivityTipManager.RegActivityTipItem(11502, null);
		base.OnDestroy();
	}

	private void EventHandler(int type, ActivityTipItem args)
	{
		if (this.m_BtnhuodongListOBC != null)
		{
			for (int i = 0; i < this.m_BtnhuodongListOBC.Count; i++)
			{
				if (type == 11501)
				{
					ZhuTiFuItem component = this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>();
					if (component != null && component.Type == 150)
					{
						component.TipActive(args.IsActive);
					}
				}
				else if (type == 11502)
				{
					ZhuTiFuItem component2 = this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>();
					if (component2 != null && component2.Type == 151)
					{
						component2.TipActive(args.IsActive);
					}
				}
			}
		}
	}

	private bool OnZoreItem(string listXml)
	{
		List<bool> list = new List<bool>();
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime serverStartTime = Global.GetServerStartTime();
		DateTime dateTime;
		dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, 1, 1, 1);
		DateTime dateTime2;
		dateTime2..ctor(serverStartTime.Year, serverStartTime.Month, serverStartTime.Day, 1, 1, 1);
		int num = (dateTime - dateTime2).Days + 1;
		XElement xelement = XElement.Parse(listXml);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityZhiGou");
		for (int i = 0; i < xelementList.Count; i++)
		{
			string text = string.Empty;
			text = Global.GetXElementAttributeStr(xelementList[i], "Day");
			if (num >= text.Split(new char[]
			{
				','
			})[0].SafeToInt32(0) && num <= text.Split(new char[]
			{
				','
			})[1].SafeToInt32(0))
			{
				return true;
			}
		}
		return false;
	}

	private void AddBtns(List<ZhuTiFuType> listType)
	{
		int type = 0;
		string xmlList = string.Empty;
		bool flag = true;
		for (int i = 0; i < listType.Count; i++)
		{
			if (listType[i].Type != 150 || this.OnZoreItem(Global.Data.ZhuTiFu.XmlList[i + 1]))
			{
				ZhuTiFuItem item = U3DUtils.NEW<ZhuTiFuItem>();
				this.m_BtnhuodongListOBC.AddNoUpdate(item);
				item.ID = listType[i].ID;
				item.Title = listType[i].Name;
				item.Type = listType[i].Type;
				item.name = listType[i].Name;
				if (i < Global.Data.ZhuTiFu.XmlList.Count)
				{
					item.XmlList = Global.Data.ZhuTiFu.XmlList[i + 1];
				}
				if (item.GetComponent<BoxCollider>() != null)
				{
					Object.Destroy(item.GetComponent<BoxCollider>());
				}
				if (item.m_Button.GetComponent<UIDragPanelContents>() != null && this.m_BtnhuodongList.GetComponent<UIDraggablePanel>() != null)
				{
					item.m_Button.GetComponent<UIDragPanelContents>().draggablePanel = this.m_BtnhuodongList.GetComponent<UIDraggablePanel>();
				}
				item.m_Button.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OnItemClick(item.Type, item.XmlList);
				};
				if (flag)
				{
					type = item.Type;
					xmlList = item.XmlList;
					flag = false;
				}
			}
		}
		this.OnItemClick(type, xmlList);
		ActivityTipManager.RegActivityTipItem(11501, new ActivityTipEventHandler(this.EventHandler));
		ActivityTipManager.RegActivityTipItem(11502, new ActivityTipEventHandler(this.EventHandler));
	}

	private void OnItemClick(int type, string XmlList)
	{
		if (this.m_Type == type)
		{
			return;
		}
		for (int i = 0; i < this.m_BtnhuodongListOBC.Count; i++)
		{
			if (this.m_Type == this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>().Type)
			{
				this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>().IsOnClick(false);
			}
			if (type == this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>().Type)
			{
				this.m_BtnhuodongListOBC.GetAt(i).GetComponent<ZhuTiFuItem>().IsOnClick(true);
			}
		}
		this.m_Type = type;
		this.m_ItemPosition.Clear();
		if (type == 150)
		{
			if (this.m_ZhuTiFuZhiGouPart == null)
			{
				this.m_ZhuTiFuZhiGouPart = U3DUtils.NEW<ZhuTiFuZhiGouPart>();
				this.m_ZhuTiFuZhiGouPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuZhiGouPart.SetData(XmlList);
				GameInstance.Game.SenZhuTiFuZhiGouData();
			}
		}
		else if (type == 151)
		{
			if (this.m_ZhuTiFuLiBaoPart == null)
			{
				this.m_ZhuTiFuLiBaoPart = U3DUtils.NEW<ZhuTiFuLiBaoPart>();
				this.m_ZhuTiFuLiBaoPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuLiBaoPart.GetData(XmlList);
				GameInstance.Game.ZhuTiFuSpriteQueryJieriDaLiBao();
			}
		}
		else if (type == 152)
		{
			if (this.m_ZhuTiFuJingYanPart == null)
			{
				this.m_ZhuTiFuJingYanPart = U3DUtils.NEW<ZhuTiFuJingYanPart>();
				this.m_ZhuTiFuJingYanPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuJingYanPart.AddList(XmlList);
			}
		}
		else if (type == 153)
		{
			if (this.m_ZhuTiFuZhuanXiangPart == null)
			{
				this.m_ZhuTiFuZhuanXiangPart = U3DUtils.NEW<ZhuTiFuZhuanXiangPart>();
				this.m_ZhuTiFuZhuanXiangPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuZhuanXiangPart.AddList(XmlList);
			}
		}
		else if (type == 154)
		{
			if (this.m_ZhuTiFuDuiHuanPart == null)
			{
				this.m_ZhuTiFuDuiHuanPart = U3DUtils.NEW<ZhuTiFuDuiHuanPart>();
				this.m_ZhuTiFuDuiHuanPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuDuiHuanPart.SetXml(XmlList);
				GameInstance.Game.SetZhuTiFudDuiHuanData();
			}
		}
		else if (type == 155)
		{
			if (this.m_ZhuTiFuBossPart == null)
			{
				this.m_ZhuTiFuBossPart = U3DUtils.NEW<ZhuTiFuBossPart>();
				this.m_ZhuTiFuBossPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuBossPart.AddXmlThemeActivityBOSS(XmlList);
				GameInstance.Game.SetZhuTiFudRuQinData();
			}
		}
		else if (type == 156)
		{
			if (this.m_ZhuTiFuMoYuPart == null)
			{
				this.m_ZhuTiFuMoYuPart = U3DUtils.NEW<ZhuTiFuMoYuPart>();
				this.m_ZhuTiFuMoYuPart.transform.SetParent(this.m_ItemPosition.transform, false);
				this.m_ZhuTiFuMoYuPart.SetXml(XmlList);
				this.m_ZhuTiFuMoYuPart.SetData();
			}
		}
		else if (type == 157 && this.m_ZhuTiFuShiLianPart == null)
		{
			this.m_ZhuTiFuShiLianPart = U3DUtils.NEW<ZhuTiFuShiLianPart>();
			this.m_ZhuTiFuShiLianPart.transform.SetParent(this.m_ItemPosition.transform, false);
			this.m_ZhuTiFuShiLianPart.SetData(XmlList);
		}
	}

	public void Return(int cmd, SocketConnectEventArgs e)
	{
		int num = 0;
		if (cmd == 905)
		{
			JieriXmlData jieriXmlData = DataHelper.BytesToObject<JieriXmlData>(e.bytesData, 0, e.bytesData.Length);
			if (Global.Data.ZhuTiFu == null || Global.ZhuTiFuXML_Version != jieriXmlData.Version)
			{
				Global.Data.ZhuTiFu = jieriXmlData;
			}
			Global.ZhuTiFuXML_Version = jieriXmlData.Version;
			List<ZhuTiFuType> list = new List<ZhuTiFuType>();
			XElement xelement = XElement.Parse(Global.Data.ZhuTiFu.XmlList[0]);
			List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityType");
			for (int i = 0; i < xelementList.Count; i++)
			{
				list.Add(new ZhuTiFuType
				{
					ID = Global.GetXElementAttributeInt(xelementList[i], "ID"),
					Type = Global.GetXElementAttributeInt(xelementList[i], "Type"),
					Name = Global.GetXElementAttributeStr(xelementList[i], "Name"),
					Tiptype = Global.GetXElementAttributeInt(xelementList[i], "Tiptype"),
					PeiZhi = Global.GetXElementAttributeStr(xelementList[i], "PeiZhi")
				});
			}
			this.AddBtns(list);
		}
		else if (cmd == 906)
		{
			Dictionary<int, int> dic = DataHelper.BytesToObject<Dictionary<int, int>>(e.bytesData, 0, e.bytesData.Length);
			if (this.m_ZhuTiFuZhiGouPart != null)
			{
				this.m_ZhuTiFuZhiGouPart.AddList(dic);
			}
		}
		else if (cmd == 908)
		{
			num = Convert.ToInt32(e.fields[0]);
			int num2 = Convert.ToInt32(e.fields[1]);
			int num3 = Convert.ToInt32(e.fields[2]);
			if (num >= 0 && num2 == Global.Data.RoleID && this.m_ZhuTiFuLiBaoPart != null)
			{
				if (num3 > 0)
				{
					this.m_ZhuTiFuLiBaoPart.RefreshData(false);
				}
				else
				{
					this.m_ZhuTiFuLiBaoPart.RefreshData(true);
				}
			}
		}
		else if (cmd == 388)
		{
			int num4 = -1;
			if (e.fields.Length == 3)
			{
				num = Convert.ToInt32(e.fields[0]);
				int num5 = Convert.ToInt32(e.fields[1]);
				num4 = Convert.ToInt32(e.fields[2]);
			}
			else if (e.fields.Length == 4)
			{
				num = Convert.ToInt32(e.fields[0]);
				int num5 = Convert.ToInt32(e.fields[1]);
				num4 = Convert.ToInt32(e.fields[2]);
				int num6 = Convert.ToInt32(e.fields[3]);
			}
			else if (e.fields.Length == 5)
			{
				num = Convert.ToInt32(e.fields[0]);
				int num5 = Convert.ToInt32(e.fields[1]);
				num4 = Convert.ToInt32(e.fields[2]);
				int num6 = Convert.ToInt32(e.fields[3]);
				int num7 = Convert.ToInt32(e.fields[4]);
			}
			if (num >= 0)
			{
				if (num4 == 151)
				{
					if (this.m_ZhuTiFuLiBaoPart != null && num >= 0)
					{
						this.m_ZhuTiFuLiBaoPart.RefreshData(false);
					}
				}
				else if (num4 == 154 && this.m_ZhuTiFuDuiHuanPart != null && num >= 0)
				{
					this.m_ZhuTiFuDuiHuanPart.RefreshData(e.fields[4].SafeToInt32(0), e.fields[3].SafeToInt32(0));
				}
			}
		}
		else if (cmd == 907)
		{
			num = Convert.ToInt32(e.fields[0]);
			int num8 = Convert.ToInt32(e.fields[1]);
			this.m_ZhuTiFuDuiHuanPart.AddList(e.fields[2]);
		}
		else if (cmd == 910 && e.fields.Length == 2)
		{
			int actId = Convert.ToInt32(e.fields[0]);
			int bossState = Convert.ToInt32(e.fields[1]);
			if (this.m_ZhuTiFuBossPart != null)
			{
				this.m_ZhuTiFuBossPart.SetData(actId, bossState);
			}
		}
		if (num < 0)
		{
			if (num == -10005)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你已经领取过了"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -10006)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("活动期间充值额度为0，不能领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -10007)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足领取条件"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -10077)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前的隔天登陆次数尚未达到, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -10088)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前充值额度不足, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -10099)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前还不是VIP, 无法领取VIP奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -10888)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前积分不足, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -20000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成兑换道具今日的次数已经为0，请明日再进行合成操作"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -20003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成兑换道具时需要的材料不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -20004)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMojing, null, string.Empty, string.Empty);
			}
			else if (num == -20005)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedQifujifen, null, string.Empty, string.Empty);
			}
			else if (num == -1003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前不在排行榜内，无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("现在不是领取时间"), new object[0]), 0, -1, -1, 0);
			}
			else if (num == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包空格不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(num, false, false)), 10, 3);
			}
		}
	}

	private void SetMainData()
	{
		GameInstance.Game.SenZhuTiFuXmlData();
	}

	public ListBox m_BtnhuodongList;

	public GButton m_BtnClose;

	public SpriteSL m_ItemPosition;

	private ObservableCollection m_BtnhuodongListOBC;

	private ZhuTiFuZhiGouPart m_ZhuTiFuZhiGouPart;

	private ZhuTiFuLiBaoPart m_ZhuTiFuLiBaoPart;

	private ZhuTiFuDuiHuanPart m_ZhuTiFuDuiHuanPart;

	private ZhuTiFuZhuanXiangPart m_ZhuTiFuZhuanXiangPart;

	private ZhuTiFuJingYanPart m_ZhuTiFuJingYanPart;

	private ZhuTiFuBossPart m_ZhuTiFuBossPart;

	private ZhuTiFuMoYuPart m_ZhuTiFuMoYuPart;

	private ZhuTiFuShiLianPart m_ZhuTiFuShiLianPart;

	public DPSelectedItemEventHandler m_HandlerClose;

	private int m_Type;

	private int m_Page = 1;
}
