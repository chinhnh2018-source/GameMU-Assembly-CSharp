using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriPartBoss : UserControl
{
	public string ThisXmlName
	{
		get
		{
			return this.thisXmlName;
		}
		set
		{
			this.thisXmlName = value;
			this.InitData(this.ThisXmlName);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.rewardList.ItemsSource;
		this.btnGo.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.GotoMap);
	}

	private void InitTextInPrefabs()
	{
		this.btnGo.Text = Global.GetLang("立即传送");
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

	private void InitData(string strXML)
	{
		XElement xelement = XElement.Parse(strXML);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		XElement xelement2 = xelementList[0];
		if (xelement2 == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ActivityType");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "Award");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		this.monsterID = Global.GetXElementAttributeInt(xelement3, "MonsterID");
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.monsterID);
		int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
		string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
		string sname = monsterXmlNodeByID.SName;
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "ZhanLi");
		this.mapLabel.text = mapNameByCode;
		this.atkLabel.text = xelementAttributeStr5;
		string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement3, "GoodsIDs");
		string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement3, "BossTime");
		if (xelementAttributeStr7.Contains(","))
		{
			string[] array = xelementAttributeStr7.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'|'
				});
				this.startDay.Add(int.Parse(array2[0]));
				this.lastDays.Add(int.Parse(array2[1]));
				this.refreshTime.Add(array2[2]);
			}
		}
		else
		{
			string[] array2 = xelementAttributeStr7.Split(new char[]
			{
				'|'
			});
			this.startDay.Add(int.Parse(array2[0]));
			this.lastDays.Add(int.Parse(array2[1]));
			this.refreshTime.Add(array2[2]);
		}
		DateTime dateTime = DateTime.Parse(xelementAttributeStr).AddDays((double)(this.startDay[0] - 1));
		DateTime dateTime2 = dateTime.AddDays((double)(this.lastDays[0] - 1));
		this.time1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0}-{1}-{2}", dateTime.Year, dateTime.Month, dateTime.Day),
			"fac60d",
			Global.GetLang("    至    "),
			"dac7ae",
			string.Format("{0}-{1}-{2}", dateTime2.Year, dateTime2.Month, dateTime2.Day),
			"fac60d",
			Global.GetLang("每天") + this.refreshTime[0]
		});
		this.loadGoodsList(xelementAttributeStr6);
	}

	private void loadGoodsList(string goodsIDs)
	{
		this.ItemCollection.Clear();
		if (!(string.Empty == goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.loadOtherJiangLiGoodsList(goodsIDs, false);
			}
			else
			{
				this.loadOtherJiangLiGoodsList(array[0], true);
				this.loadOtherJiangLiGoodsList(array[1], false);
			}
		}
	}

	private void loadOtherJiangLiGoodsList(string goodsStr, bool isOcc = false)
	{
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				if (!isOcc || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					this.addGoodsIcon(dummyGoodsDataMu, false);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = -1;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			this.ItemCollection.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			if (ggoodIcon.gameObject.GetComponent<UIPanel>())
			{
				Object.Destroy(ggoodIcon.gameObject.GetComponent<UIPanel>());
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public void GotoMap(object sender, MouseEvent e)
	{
		int type = 2;
		int npcID = this.monsterID;
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.monsterID);
		int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("VIPBossChuanSong", true);
		string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
		string sname = monsterXmlNodeByID.SName;
		int num = 0;
		int num2 = 0;
		Global.GetMapMinLevelAndZhuanSheng(npcorMonsterMapCodeByID, out num, out num2);
		if (Global.Data.roleData.ChangeLifeCount * 400 + Global.Data.roleData.Level < num2 * 400 + num)
		{
			Super.HintMainText(Global.GetLang("等级不足，无法传送！"), 10, 3);
			return;
		}
		bool forceTansport = Global.GetVIPLeve() >= Convert.ToInt32(systemParamByName);
		this.TansportPoint(type, -1, npcID, npcorMonsterMapCodeByID, -1, -1, forceTansport);
		Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventWindowOperation", WindowOperationTypes.CloseActivityWindow));
		PlayZone.GlobalPlayZone.CloseJieRihuodongWindow();
	}

	public bool TansportPoint(int type, int taskID, int npcID, int mapCode, int toPosX, int toPosY, bool forceTansport = false)
	{
		Global.Data.TargetNpcID = npcID;
		Point pos;
		if (type == 2)
		{
			pos = Global.GetMonsterPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else if (type == 3)
		{
			pos = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(toPosX, toPosY);
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
			return true;
		}
		if (type == 2)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
		}
		else if (type == 3)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
		}
		if (forceTansport && Super.CanTransport(mapCode, true, false))
		{
			GameInstance.Game.SpriteTaskTransport(mapCode, pos.X, pos.Y, 0);
		}
		return true;
	}

	public TextBlock mapLabel;

	public TextBlock atkLabel;

	public TextBlock time1;

	public GButton btnGo;

	public ListBox rewardList;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private List<int> startDay = new List<int>();

	private List<int> lastDays = new List<int>();

	private List<string> refreshTime = new List<string>();

	private int monsterID;

	private string thisXmlName = string.Empty;

	private ObservableCollection _ItemCollection;
}
