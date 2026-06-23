using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class OpenServerActiveLevelPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.m_ListGiftItem)
		{
			this.m_ListGiftItemObC = this.m_ListGiftItem.ItemsSource;
		}
	}

	public void ResetPanelGiftsPos()
	{
		this.PanelGifts.transform.localPosition = Vector3.zero;
		this.PanelGifts.clipRange = new Vector4(-4f, -80f, 730f, 246f);
	}

	public void LoadConfigFromXML(string xmlPath, OpenServerActiveType activeType)
	{
		this.ActiveType = activeType;
		XElement isolateResXml = Global.GetIsolateResXml(xmlPath);
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Activities");
		if (xelementList == null)
		{
			return;
		}
		XElement xelement = xelementList[0];
		if (xelement != null)
		{
			this.ShowActiveTime();
		}
		if (Global.GetXElement(isolateResXml, "GiftList") == null)
		{
			return;
		}
		xelementList = Global.GetXElementList(isolateResXml, "Award");
		this.LoadJiangLiInfo(xelementList);
		switch (this.ActiveType)
		{
		case OpenServerActiveType.LEVEL:
			this.ItemBak.URL = Global.GetGameResImageString("kaifuItem_level.png");
			break;
		case OpenServerActiveType.BossKing:
			this.ItemBak.URL = Global.GetGameResImageString("kaifuItem_boss.png");
			break;
		case OpenServerActiveType.ChongZhiKing:
			this.ItemBak.URL = Global.GetGameResImageString("kaifuItem_chongzhi.png");
			break;
		case OpenServerActiveType.XiaoFeiKing:
			this.ItemBak.URL = Global.GetGameResImageString("kaifuItem_xiaofei.png");
			break;
		}
	}

	public void InitDataFromServerInfo(NewZoneActiveData activeData)
	{
		this.ActiveData = activeData;
		if (this.ActiveType == OpenServerActiveType.ChongZhiKing)
		{
			this.RefreshGiftItemByServerDataForChongZhi();
		}
		else if (this.ActiveType == OpenServerActiveType.XiaoFeiKing || this.ActiveType == OpenServerActiveType.BossKing)
		{
			this.RefreshGiftItemByServerDataForXiaoFeiOrBoss();
		}
		else
		{
			this.RefreshGiftItemByServerData();
		}
	}

	public void InitDataFromServerInfoForLevel(NewZoneUpLevelData upLevelData)
	{
		this.UpLevelData = upLevelData;
		this.RefreshGiftItemByServerDataForLevel();
	}

	public void RefreshDataFromGainedInfo(int rankindex)
	{
		int count = this.CurrentItems.Count;
		for (int i = 0; i < count; i++)
		{
			OpenServerActiveLevelPartGiftItem openServerActiveLevelPartGiftItem = this.CurrentItems[i];
			if (openServerActiveLevelPartGiftItem.ID == rankindex)
			{
				openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.Gained;
				break;
			}
		}
	}

	private void ShowActiveTime()
	{
		this.m_TxtHuoDongLingQuShiJianStart.text = string.Empty;
		this.m_TxtHuoDongLingQuShiJianEnd.text = string.Empty;
		this.m_TxtHuoDongShiJianEnd.text = string.Empty;
		this.m_TxtHuoDongShiJianStart.text = string.Empty;
		string huodongTimeStr = Global.GetHuodongTimeStr(0, 0, 0, 0);
		string huodongTimeStr2 = Global.GetHuodongTimeStr(OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 3]);
		string text = string.Empty;
		string text2 = string.Empty;
		switch (this.ActiveType)
		{
		case OpenServerActiveType.LEVEL:
			this.m_TxtHuoDongShiJianStart.Text = huodongTimeStr;
			this.m_TxtHuoDongShiJianEnd.Text = huodongTimeStr2;
			text = Global.GetHuodongTimeStr(0, 0, 0, 0);
			text2 = Global.GetHuodongTimeStr(OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 3]);
			this.m_TxtHuoDongLingQuShiJianStart.Text = text;
			this.m_TxtHuoDongLingQuShiJianEnd.Text = text2;
			break;
		case OpenServerActiveType.BossKing:
		case OpenServerActiveType.ChongZhiKing:
		case OpenServerActiveType.XiaoFeiKing:
			this.m_TxtHuoDongShiJianStart.Text = huodongTimeStr;
			this.m_TxtHuoDongShiJianEnd.Text = huodongTimeStr2;
			text = Global.GetHuodongTimeStr(OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 3]);
			text2 = Global.GetHuodongTimeStr(OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 3]);
			this.m_TxtHuoDongLingQuShiJianStart.Text = text;
			this.m_TxtHuoDongLingQuShiJianEnd.Text = text2;
			break;
		}
	}

	public static bool IsBetweenGainTime(OpenServerActiveType ActiveType)
	{
		bool result = false;
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		switch (ActiveType)
		{
		case OpenServerActiveType.LEVEL:
			dateTime = Global.GetHuodongTimeDateTime(0, 0, 0, 0);
			dateTime2 = Global.GetHuodongTimeDateTime(OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[0, 3]);
			break;
		case OpenServerActiveType.BossKing:
		case OpenServerActiveType.ChongZhiKing:
		case OpenServerActiveType.XiaoFeiKing:
			dateTime = Global.GetHuodongTimeDateTime(OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[1, 3]);
			dateTime2 = Global.GetHuodongTimeDateTime(OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 0], OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 1], OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 2], OpenServerActiveLevelPart.XinQuHuoDongItemTime[3, 3]);
			break;
		}
		if (correctDateTime.CompareTo(dateTime) >= 0 && correctDateTime.CompareTo(dateTime2) <= 0)
		{
			result = true;
		}
		return result;
	}

	private void InitDataFromConfig(OpenServerActiveLevelPartGiftItem item, XElement args)
	{
		switch (this.ActiveType)
		{
		case OpenServerActiveType.LEVEL:
			item.LevelNameFrist = StringUtil.substitute(Global.GetLang("达到{0}转{1}级"), new object[]
			{
				Global.GetXElementAttributeInt(args, "MinZhuanSheng"),
				Global.GetXElementAttributeInt(args, "MinLevel")
			});
			item.LevelNameSecond = Global.GetLang("剩余:0");
			item.TiaoJian = string.Empty;
			item.TiaoJianValue = Global.GetXElementAttributeInt(args, "MinZhuanSheng") * 100 + Global.GetXElementAttributeInt(args, "MinLevel");
			break;
		case OpenServerActiveType.BossKing:
			item.NameFrist = Global.GetXElementAttributeStr(args, "ID");
			item.NameSecond = Global.GetLang("无");
			item.TiaoJian = StringUtil.substitute(Global.GetLang("最低击杀:{0}"), new object[]
			{
				Global.GetXElementAttributeStr(args, "MinBoss")
			});
			break;
		case OpenServerActiveType.ChongZhiKing:
			item.NameFrist = Global.GetXElementAttributeStr(args, "ID");
			item.NameSecond = Global.GetLang("无");
			item.TiaoJian = StringUtil.substitute(Global.GetLang("最低充值{0}钻"), new object[]
			{
				Global.GetXElementAttributeStr(args, "MinYuanBao")
			});
			break;
		case OpenServerActiveType.XiaoFeiKing:
			item.NameFrist = Global.GetXElementAttributeStr(args, "ID");
			item.NameSecond = Global.GetLang("无");
			item.TiaoJian = StringUtil.substitute(Global.GetLang("最低消费{0}钻"), new object[]
			{
				Global.GetXElementAttributeStr(args, "MinYuanBao")
			});
			break;
		}
	}

	private void LoadJiangLiInfo(List<XElement> xml)
	{
		this.m_ListGiftItemObC.Clear();
		this.CurrentItems.Clear();
		if (xml == null)
		{
			return;
		}
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			OpenServerActiveLevelPartGiftItem openServerActiveLevelPartGiftItem = U3DUtils.NEW<OpenServerActiveLevelPartGiftItem>();
			openServerActiveLevelPartGiftItem.ID = Global.GetXElementAttributeInt(xelement, "ID");
			openServerActiveLevelPartGiftItem.ActiveType = this.ActiveType;
			this.InitDataFromConfig(openServerActiveLevelPartGiftItem, xelement);
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsOne");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsTwo");
			string goodsList = xelementAttributeStr2 + "|" + xelementAttributeStr;
			openServerActiveLevelPartGiftItem.GoodsList = goodsList;
			this.m_ListGiftItemObC.Add(openServerActiveLevelPartGiftItem);
			this.CurrentItems.Add(openServerActiveLevelPartGiftItem);
		}
	}

	public static bool GetTotalCanGainStateForOther(NewZoneActiveData activeData, OpenServerActiveType activeType)
	{
		bool result = false;
		if (activeData != null)
		{
			List<PaiHangItemData> ranklist = activeData.Ranklist;
			int num = 0;
			if (ranklist != null)
			{
				num = ranklist.Count;
			}
			for (int i = 0; i < num; i++)
			{
				if (ranklist.Count > i)
				{
					if (activeType == OpenServerActiveType.ChongZhiKing)
					{
						if (ranklist[i].uid == Global.Data.UserID && OpenServerActiveLevelPart.IsBetweenGainTime(activeType) && activeData.GetState != 1)
						{
							result = true;
							break;
						}
					}
					else if (ranklist[i].RoleID == Global.Data.roleData.RoleID && OpenServerActiveLevelPart.IsBetweenGainTime(activeType) && activeData.GetState != 1)
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	public static void ClearXMLData()
	{
		OpenServerActiveLevelPart.TiaoJianValues = null;
	}

	public static List<int> GetTiaoJianValues()
	{
		if (OpenServerActiveLevelPart.TiaoJianValues == null)
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/XinFuGifts/MuLevel.xml");
			if (isolateResXml == null)
			{
				return OpenServerActiveLevelPart.TiaoJianValues;
			}
			List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Award");
			if (xelementList.Count > 0)
			{
				OpenServerActiveLevelPart.TiaoJianValues = new List<int>();
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
				OpenServerActiveLevelPart.TiaoJianValues.Add(xelementAttributeInt * 100 + xelementAttributeInt2);
			}
		}
		return OpenServerActiveLevelPart.TiaoJianValues;
	}

	public static bool GetTotalCanGainStateForLevel(NewZoneUpLevelData upLevelData)
	{
		bool result = false;
		if (upLevelData != null)
		{
			if (OpenServerActiveLevelPart.TiaoJianValues == null)
			{
				OpenServerActiveLevelPart.GetTiaoJianValues();
			}
			List<NewZoneUpLevelItemData> items = upLevelData.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				if (!items[i].GetAward)
				{
					if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level >= OpenServerActiveLevelPart.TiaoJianValues[i] && items[i].LeftNum > 0 && OpenServerActiveLevelPart.IsBetweenGainTime(OpenServerActiveType.LEVEL))
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	private void RefreshGiftItemByServerData()
	{
		if (this.ActiveData != null)
		{
			int count = this.CurrentItems.Count;
			List<PaiHangItemData> ranklist = this.ActiveData.Ranklist;
			if (ranklist != null)
			{
				int num = count - ranklist.Count;
				for (int i = 0; i < count; i++)
				{
					OpenServerActiveLevelPartGiftItem openServerActiveLevelPartGiftItem = this.CurrentItems[i];
					int num2 = i - num;
					if (num2 >= 0 && ranklist.Count > num2)
					{
						openServerActiveLevelPartGiftItem.NameSecond = ranklist[num2].RoleName;
						if (ranklist[num2].RoleID == Global.Data.roleData.RoleID)
						{
							if (!OpenServerActiveLevelPart.IsBetweenGainTime(this.ActiveType))
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
							}
							else if (this.ActiveData.GetState == 1)
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.Gained;
							}
							else
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanGain;
							}
						}
						else
						{
							openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
						}
					}
					else
					{
						openServerActiveLevelPartGiftItem.NameSecond = Global.GetLang("无");
					}
				}
			}
		}
	}

	private void RefreshGiftItemByServerDataForChongZhi()
	{
		if (this.ActiveData != null)
		{
			int count = this.CurrentItems.Count;
			List<PaiHangItemData> ranklist = this.ActiveData.Ranklist;
			if (ranklist != null)
			{
				int count2 = ranklist.Count;
				for (int i = 0; i < count2; i++)
				{
					PaiHangItemData paiHangItemData = ranklist[i];
					if (paiHangItemData.Val2 - 1 >= 0 && paiHangItemData.Val2 - 1 < count)
					{
						OpenServerActiveLevelPartGiftItem openServerActiveLevelPartGiftItem = this.CurrentItems[paiHangItemData.Val2 - 1];
						openServerActiveLevelPartGiftItem.NameSecond = paiHangItemData.RoleName;
						if (paiHangItemData.uid == Global.Data.UserID)
						{
							if (!OpenServerActiveLevelPart.IsBetweenGainTime(this.ActiveType))
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
							}
							else if (this.ActiveData.GetState == 1)
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.Gained;
							}
							else
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanGain;
							}
						}
						else
						{
							openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
						}
					}
				}
			}
		}
	}

	private void RefreshGiftItemByServerDataForXiaoFeiOrBoss()
	{
		if (this.ActiveData != null)
		{
			int count = this.CurrentItems.Count;
			List<PaiHangItemData> ranklist = this.ActiveData.Ranklist;
			if (ranklist != null)
			{
				int count2 = ranklist.Count;
				for (int i = 0; i < count2; i++)
				{
					PaiHangItemData paiHangItemData = ranklist[i];
					if (paiHangItemData.Val2 - 1 >= 0 && paiHangItemData.Val2 - 1 < count)
					{
						OpenServerActiveLevelPartGiftItem openServerActiveLevelPartGiftItem = this.CurrentItems[paiHangItemData.Val2 - 1];
						openServerActiveLevelPartGiftItem.NameSecond = paiHangItemData.RoleName;
						if (paiHangItemData.RoleID == Global.Data.roleData.RoleID)
						{
							if (!OpenServerActiveLevelPart.IsBetweenGainTime(this.ActiveType))
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
							}
							else if (this.ActiveData.GetState == 1)
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.Gained;
							}
							else
							{
								openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanGain;
							}
						}
						else
						{
							openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
						}
					}
				}
			}
		}
	}

	private void RefreshGiftItemByServerDataForLevel()
	{
		if (this.UpLevelData != null)
		{
			int count = this.CurrentItems.Count;
			List<NewZoneUpLevelItemData> items = this.UpLevelData.Items;
			for (int i = 0; i < count; i++)
			{
				OpenServerActiveLevelPartGiftItem openServerActiveLevelPartGiftItem = this.CurrentItems[i];
				if (items.Count > i)
				{
					openServerActiveLevelPartGiftItem.LevelNameSecond = string.Format(Global.GetLang("剩余:{0}"), items[i].LeftNum);
					if (items[i].GetAward)
					{
						openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.Gained;
					}
					else if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level >= openServerActiveLevelPartGiftItem.TiaoJianValue && items[i].LeftNum > 0)
					{
						if (!OpenServerActiveLevelPart.IsBetweenGainTime(this.ActiveType))
						{
							openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
						}
						else
						{
							openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanGain;
						}
					}
					else
					{
						openServerActiveLevelPartGiftItem.GiftGainState = OpenServerActiveGiftGainState.CanNotGain;
					}
				}
				else
				{
					openServerActiveLevelPartGiftItem.LevelNameSecond = string.Format(Global.GetLang("剩余:{0}"), 0);
				}
			}
		}
	}

	public GTextBlockOutLine m_TxtHuoDongLingQuShiJianStart;

	public GTextBlockOutLine m_TxtHuoDongLingQuShiJianEnd;

	public GTextBlockOutLine m_TxtHuoDongShiJianStart;

	public GTextBlockOutLine m_TxtHuoDongShiJianEnd;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListGiftItem = new ListBox();

	public ShowNetImage ItemBak;

	public UIPanel PanelGifts;

	private ObservableCollection m_ListGiftItemObC;

	private List<OpenServerActiveLevelPartGiftItem> CurrentItems = new List<OpenServerActiveLevelPartGiftItem>();

	private OpenServerActiveType ActiveType;

	private NewZoneUpLevelData UpLevelData;

	private NewZoneActiveData ActiveData;

	private static int[,] XinQuHuoDongItemTime = new int[,]
	{
		{
			6,
			23,
			59,
			59
		},
		{
			7,
			0,
			0,
			0
		},
		{
			1,
			0,
			0,
			0
		},
		{
			8,
			23,
			59,
			59
		},
		{
			7,
			23,
			59,
			59
		}
	};

	private static List<int> TiaoJianValues = null;
}
