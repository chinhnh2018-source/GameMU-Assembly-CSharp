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

public class HefuPartBOSSzhizhan : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnGo.Text = Global.GetLang("立即传送");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.rewardList.ItemsSource;
		this.InitRewardItem();
		this.btnGo.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.GotoMap);
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

	private void InitTime()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		this.startTime1 = Global.GetServerMergeHuodongTimeDateTime(1, 0, 0, 0);
		this.startTime2 = Global.GetServerMergeHuodongTimeDateTime(2, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		string pubEndTime = this.endTime.ToString("yyyy-MM-dd HH:mm:ss");
		string pubStartTime = correctDateTime.ToString("yyyy-MM-dd HH:mm:ss");
		DateTime dateTime;
		dateTime..ctor(this.startTime1.Year, this.startTime1.Month, this.startTime1.Day, 12, 0, 0);
		DateTime dateTime2;
		dateTime2..ctor(this.startTime2.Year, this.startTime2.Month, this.startTime2.Day, 12, 0, 0);
		this.startTimeStr1 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.startTimeStr2 = dateTime2.ToString("yyyy-MM-dd HH:mm:ss");
		this.time1.text = string.Format("{0}-{1}-{2}  {3}:{4}", new object[]
		{
			dateTime.Year,
			dateTime.Month,
			dateTime.Day,
			dateTime.Hour,
			(dateTime.Minute != 0) ? dateTime.Minute.ToString() : "00"
		});
		this.time2.text = string.Format("| {0}-{1}-{2}  {3}:{4}", new object[]
		{
			dateTime2.Year,
			dateTime2.Month,
			dateTime2.Day,
			dateTime2.Hour,
			(dateTime2.Minute != 0) ? dateTime2.Minute.ToString() : "00"
		});
		if (Global.InLimitTimeRange(pubStartTime, this.startTimeStr1))
		{
			this.time1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"a39a84",
				this.time1.text
			});
			this.time2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"a39a84",
				this.time2.text
			});
		}
		if (Global.InLimitTimeRange(this.startTimeStr1, this.startTimeStr2))
		{
			this.time1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"a39a84",
				this.time1.text
			});
			this.time2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dec69c",
				this.time2.text
			});
		}
		if (Global.InLimitTimeRange(this.startTimeStr2, pubEndTime))
		{
			this.time1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"a39a84",
				this.time1.text
			});
			this.time2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dec69c",
				this.time2.text
			});
		}
	}

	private void InitRewardItem()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HeFuBOSS.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		XElement xelement = xelementList[0];
		this.monsterID = Global.GetXElementAttributeInt(xelement, "MonsterID");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "BossTime");
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.monsterID);
		int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
		string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
		string sname = monsterXmlNodeByID.SName;
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "ZhanLi");
		this.mapLabel.text = mapNameByCode;
		this.atkLabel.text = xelementAttributeStr3;
		if (xelementAttributeStr2.Contains(","))
		{
			string[] array = xelementAttributeStr2.Split(new char[]
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
			string[] array2 = xelementAttributeStr2.Split(new char[]
			{
				'|'
			});
			this.startDay.Add(int.Parse(array2[0]));
			this.lastDays.Add(int.Parse(array2[1]));
			this.refreshTime.Add(array2[2]);
		}
		DateTime serverMergeHuodongTimeDateTime = Global.GetServerMergeHuodongTimeDateTime(this.startDay[0] - 1, 0, 0, 0);
		DateTime dateTime = serverMergeHuodongTimeDateTime.AddDays((double)(this.lastDays[0] - 1));
		this.time1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0}-{1}-{2}", serverMergeHuodongTimeDateTime.Year, serverMergeHuodongTimeDateTime.Month, serverMergeHuodongTimeDateTime.Day),
			"fac60d",
			Global.GetLang("  至  "),
			"dac7ae",
			string.Format("{0}-{1}-{2}", dateTime.Year, dateTime.Month, dateTime.Day),
			"fac60d",
			Global.GetLang("    每天") + this.refreshTime[0]
		});
		this.loadGoodsList(xelementAttributeStr);
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
		PlayZone.GlobalPlayZone.CloseHefuhuodongWindow();
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

	public TextBlock time2;

	public GButton btnGo;

	public ListBox rewardList;

	private DateTime startTime1;

	private DateTime startTime2;

	private DateTime endTime;

	private string startTimeStr1;

	private string startTimeStr2;

	private int monsterID;

	private List<int> startDay = new List<int>();

	private List<int> lastDays = new List<int>();

	private List<string> refreshTime = new List<string>();

	private ObservableCollection _ItemCollection;
}
