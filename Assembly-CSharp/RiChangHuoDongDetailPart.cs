using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RiChangHuoDongDetailPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._TaskList.ItemsSource;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangHuoDongDetailPart = this;
		}
		this._PageList.ItemPerPage = 1;
		this._PageList.ItemCount = 0;
		this._bak.URL = Global.GetGameResImageString("richangHuodongRichangFuben_bak2.jpg");
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnterHuoDong);
		this._Name.text = Global.GetLang("日常活动");
		this.Static_HuoDongXinXi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("活动信息")
		});
		this.Static_HuoDongJiangLi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("活动奖励")
		});
		this.Static_ZuiGaoJiFen.text = Global.GetLang("最高积分");
		this.Static_WoDeJiFen.text = Global.GetLang("我的积分");
		this.Need_Level.Name = Global.GetLang("进入等级:");
		this.Need_ZhanLi.Name = Global.GetLang("推荐战力:");
		this.Need_Goods.Name = Global.GetLang("需要道具:");
		this._Enter.Text = Global.GetLang("进入活动");
		this.Status_Count.text = string.Empty;
		this.Need_Level._Text.transform.localPosition = new Vector3(65f, 0f, 0f);
		this.Need_ZhanLi._Text.transform.localPosition = new Vector3(125f, 0f, 0f);
		this.Need_Goods._Text.transform.localPosition = new Vector3(85f, 0f, 0f);
	}

	private void CloseWindow()
	{
		if (this._Close.MouseLeftButtonUp != null)
		{
			this._Close.MouseLeftButtonUp(null, null);
		}
	}

	private void EnterCopyMapCmd()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime - this.LastEnterCopyMapCmdTicks < 2000L)
		{
			return;
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.BloodCastle)
		{
			Global.SendEvent("2000", Global.GetLang("血色城堡参与次数"));
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.Demon)
		{
			Global.SendEvent("2100", Global.GetLang("恶魔广场参与次数"));
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.HuangJin)
		{
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.Battle)
		{
			Global.SendEvent("2300", Global.GetLang("阵营战参与次数"));
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.PKKing)
		{
			Global.SendEvent("2400", Global.GetLang("PK之王参与次数"));
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			Global.SendEvent("2200", Global.GetLang("天使神殿参与次数"));
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.GuZhanChang)
		{
			Global.SendEvent("2500", Global.GetLang("古战场参与次数"));
		}
		this.LastEnterCopyMapCmdTicks = correctLocalTime;
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.BloodCastle)
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteEnterFuBen(this.MapCode);
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.Demon)
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteEnterFuBen(this.MapCode);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("未知错误,请将游戏更新到最新版本再试!"), 0, -1, -1, 0);
		}
		this.CloseWindow();
	}

	private void OnEnterHuoDong(object sender, MouseEvent e)
	{
		if (this.LastAwardPoint > 0)
		{
			this.EnterCopyMapCmd();
		}
		else
		{
			if (PlayZone.OnPreChangeMap(-1, 0))
			{
				return;
			}
			if (!this.CanEnter)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前不在【{0}】活动进入时间内"), new object[]
				{
					this.SelectedItem.ItemName
				}), 0, -1, -1, 0);
			}
			else if (!this.SelectedItem.LevelAllow)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的级别不满足【{0}】活动要求的进入级别范围"), new object[]
				{
					this.SelectedItem.ItemName
				}), 0, -1, -1, 0);
			}
			else if (this.SelectedItem.MaxEnterNum >= 0 && this.SelectedItem.EnterNum >= this.SelectedItem.MaxEnterNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您今日参加【{0}】活动的次数已满"), new object[]
				{
					this.SelectedItem.ItemName
				}), 0, -1, -1, 0);
			}
			else if (!this.IsGoodsEnough)
			{
				string goodsNameByID = Global.GetGoodsNameByID(this.SelectedItem.EnterGoods, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("{0}不足{1}个,不能进入活动"), goodsNameByID, this.SelectedItem.GoodsNumber), 0, -1, -1, 0);
			}
			else
			{
				this.EnterCopyMapCmd();
			}
		}
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangHuoDongDetailPart = null;
		}
		base.OnDestroy();
	}

	protected virtual void OnEnable()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	protected IEnumerator TickProc()
	{
		int beginTime = 0;
		int nextTime = 0;
		for (;;)
		{
			DateTime now = Global.GetCorrectDateTime();
			int currentTime = now.Hour * 3600 + now.Minute * 60 + now.Second;
			bool noCanfig = false;
			if (currentTime < beginTime || nextTime <= currentTime)
			{
				noCanfig = false;
				if (this.BeginTimes == null)
				{
					noCanfig = true;
				}
				else if (this.BeginTimes.Count == 1)
				{
					beginTime = this.BeginTimes[0];
					if (currentTime <= nextTime)
					{
						nextTime = this.BeginTimes[0];
					}
					else
					{
						nextTime = this.BeginTimes[0] + 86400;
					}
				}
				else if (this.BeginTimes.Count >= 2)
				{
					beginTime = this.BeginTimes[0];
					nextTime = this.BeginTimes[this.BeginTimes.Count - 1];
					if (currentTime >= beginTime && currentTime < nextTime)
					{
						for (int i = 0; i < this.BeginTimes.Count - 1; i++)
						{
							if (currentTime >= this.BeginTimes[i] && currentTime < this.BeginTimes[i + 1])
							{
								beginTime = this.BeginTimes[i];
								nextTime = this.BeginTimes[i + 1];
								break;
							}
						}
					}
					else if (currentTime < beginTime)
					{
						beginTime = nextTime - 86400;
						nextTime = this.BeginTimes[0];
					}
					else
					{
						beginTime = nextTime;
						nextTime = this.BeginTimes[0] + 86400;
					}
				}
			}
			string text = null;
			if (noCanfig || this.PrepareTime <= 0)
			{
				this.CanEnter = false;
			}
			else if (currentTime >= beginTime && this.PrepareTime + beginTime >= currentTime)
			{
				this.CanEnter = true;
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"c39550",
					Global.GetLang("入场时间:"),
					"00ff00",
					Global.GetLang("剩余") + UIHelper.FormatSecs((long)(beginTime + this.PrepareTime - currentTime), "-")
				});
			}
			else if (nextTime >= currentTime)
			{
				this.CanEnter = false;
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"c39550",
					Global.GetLang("开启时间:"),
					"fd010c",
					Global.GetLang("剩余") + UIHelper.FormatSecs((long)(nextTime - currentTime), string.Empty)
				});
			}
			this.Status_Time.text = text;
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	public void RefreshDetail(RiChangHuoDongData item = null)
	{
		this.LastAwardPoint = 0;
		if (item == null)
		{
			item = this.SelectedItem;
		}
		else
		{
			this.SelectedItem = item;
		}
		if (item == null)
		{
			return;
		}
		this.CopyID = item.CopyID;
		this.MapCode = item.MapCode;
		this.PrepareTime = item.PrepareTime;
		this.Need_Level.textColor = ((!item.LevelAllow) ? 4294770956U : 4294967294U);
		this.Need_Level.Text = item.Level;
		this.Need_ZhanLi.textColor = ((!this.SelectedItem.ZhanLiAllow) ? 4294770956U : 4294967294U);
		this.Need_ZhanLi.Text = ((item.ZhanLi <= 0) ? string.Empty : item.ZhanLi.ToString());
		string text = string.Format(string.Empty, new object[0]);
		if (item.EnterGoods > 0 && item.GoodsNumber > 0)
		{
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(item.EnterGoods);
			this.IsGoodsEnough = (totalGoodsCountByID >= this.SelectedItem.GoodsNumber);
			string text2;
			if (item.GoodsNumber == 1)
			{
				text2 = Global.GetLang(Global.GetGoodsNameByID(item.EnterGoods, false));
			}
			else
			{
				text2 = string.Format(Global.GetLang("{0}个{1}"), item.GoodsNumber, Global.GetLang(Global.GetGoodsNameByID(item.EnterGoods, false)));
			}
			this.Need_Goods.Text = Global.GetColorStringForNGUIText(new object[]
			{
				(!this.IsGoodsEnough) ? "fd010c" : "fffffe",
				text2
			});
		}
		if (item.MaxEnterNum < 0)
		{
			this.Status_Count.text = "-";
		}
		else
		{
			FuBenData fuBenData = Global.GetFuBenData(this.CopyID);
			if (fuBenData != null)
			{
				item.EnterNum = fuBenData.EnterNum;
				item.FinishNum = fuBenData.FinishNum;
			}
			this.Status_Count.text = Global.GetLang("剩余次数:") + ColorCode.EncodingText2A((long)item.EnterNum, (long)item.MaxEnterNum, "fd010c", "fffffe");
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.BloodCastle)
		{
			GameInstance.Game.SpriteBloodCastleInfo();
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.Demon)
		{
			GameInstance.Game.SpriteQureyDaimonSquareInfo();
		}
		this._AwardGoods.ItemsSource.Clear();
		List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(item.AwardGoodsList, 0, int.MaxValue);
		UIHelper.AddAwardGoods(this._AwardGoods.ItemsSource, goodsList, null, false, "bagGrid4_bak", false);
	}

	public void InitData(RiChangHuoDongTypes type, string name)
	{
		this.RiChangHuoDongType = type;
		this._FuBenName.text = name;
		this._Enter.isEnabled = false;
		this._TopListItemName[0].text = null;
		this._TopListItemTime[0].text = null;
		this._TopListItemName[1].text = null;
		this._TopListItemTime[1].text = null;
		if (type != RiChangHuoDongTypes.BloodCastle)
		{
			if (type != RiChangHuoDongTypes.Demon)
			{
				MUDebug.LogError<string>(new string[]
				{
					"未知的日常活动类型!"
				});
				return;
			}
			string file = "Config/Demon.xml";
			this.InitEMoGuangChang(file);
		}
		else
		{
			string file = "Config/BloodCastleInfo.xml";
			this.InitXueSeChengBao(file);
		}
	}

	public void InitEMoGuangChang(string file)
	{
		XElement gameResXml = Global.GetGameResXml(file);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Emo");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinChangeLife");
			int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxChangeLife");
			if (UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4) == 0)
			{
				RiChangHuoDongData riChangHuoDongData = new RiChangHuoDongData();
				riChangHuoDongData.MinZhuanSheng = xelementAttributeInt3;
				riChangHuoDongData.MaxZhuanSheng = xelementAttributeInt4;
				riChangHuoDongData.MinLevel = xelementAttributeInt;
				riChangHuoDongData.MaxLevel = xelementAttributeInt2;
				riChangHuoDongData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
				riChangHuoDongData.ItemName = Global.GetXElementAttributeStr(xelement, "Name");
				riChangHuoDongData.MaxEnterNum = Global.GetXElementAttributeInt(xelement, "MaxEnter");
				riChangHuoDongData.MaxEnterNum += Global.GetSystemParamVipLeveValue("VIPEnterDaimonSquareCountAddValue");
				riChangHuoDongData.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
				riChangHuoDongData.AwardGoodsList = Global.GetXElementAttributeStr(xelement, "Award");
				riChangHuoDongData.PrepareTime = Global.GetXElementAttributeInt(xelement, "PrepareTime");
				riChangHuoDongData.BeginTimes = Global.GetXElementAttributeStr(xelement, "BeginTime");
				riChangHuoDongData.ImageStrings = Global.GetXElementAttributeStr(xelement, "Image");
				riChangHuoDongData.Level = UIHelper.FormatLevelLimit(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4);
				riChangHuoDongData.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4));
				riChangHuoDongData.ZhanLiAllow = (riChangHuoDongData.ZhanLi <= Global.Data.roleData.CombatForce);
				riChangHuoDongData.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
				int[] array = UIHelper.ParserGoodsString2(riChangHuoDongData.GoodsIDs);
				if (array != null)
				{
					riChangHuoDongData.EnterGoods = array[0];
					riChangHuoDongData.GoodsNumber = array[1];
				}
				string[] array2 = riChangHuoDongData.ImageStrings.Split(new char[]
				{
					'|'
				});
				if (array2 != null && array2.Length > 0)
				{
					this.ItemCollection.Clear();
					foreach (string text in array2)
					{
						if (!string.IsNullOrEmpty(text))
						{
							ShowNetImage showNetImage = U3DUtils.NEW<ShowNetImage>();
							this.ItemCollection.AddNoUpdate(showNetImage);
							showNetImage.Width = 610.0;
							showNetImage.Height = 318.0;
							showNetImage.URL = Super.GetTaskImageString2(text);
						}
					}
					this.ItemCollection.DelayUpdate();
					this._PageList.ItemCount = this.ItemCollection.Count;
				}
				this.BeginTimes = UIHelper.ParserTimeArrayString2(riChangHuoDongData.BeginTimes);
				this._Enter.isEnabled = true;
				this.RefreshDetail(riChangHuoDongData);
				break;
			}
		}
	}

	public void InitXueSeChengBao(string file)
	{
		XElement gameResXml = Global.GetGameResXml(file);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "BloodCastleInfo");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinChangeLife");
			int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxChangeLife");
			if (UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4) == 0)
			{
				RiChangHuoDongData riChangHuoDongData = new RiChangHuoDongData();
				riChangHuoDongData.MinZhuanSheng = xelementAttributeInt3;
				riChangHuoDongData.MaxZhuanSheng = xelementAttributeInt4;
				riChangHuoDongData.MinLevel = xelementAttributeInt;
				riChangHuoDongData.MaxLevel = xelementAttributeInt2;
				riChangHuoDongData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
				riChangHuoDongData.ItemName = Global.GetXElementAttributeStr(xelement, "Name");
				riChangHuoDongData.MaxEnterNum = Global.GetXElementAttributeInt(xelement, "MaxEnter");
				riChangHuoDongData.MaxEnterNum += Global.GetSystemParamVipLeveValue("VIPEnterBloodCastleCountAddValue");
				riChangHuoDongData.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
				riChangHuoDongData.AwardGoodsList = Global.GetXElementAttributeStr(xelement, "Award");
				riChangHuoDongData.PrepareTime = Global.GetXElementAttributeInt(xelement, "PrepareTime");
				riChangHuoDongData.BeginTimes = Global.GetXElementAttributeStr(xelement, "BeginTime");
				riChangHuoDongData.ImageStrings = Global.GetXElementAttributeStr(xelement, "Image");
				riChangHuoDongData.Level = UIHelper.FormatLevelLimit(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4);
				riChangHuoDongData.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4));
				riChangHuoDongData.ZhanLiAllow = (riChangHuoDongData.ZhanLi <= Global.Data.roleData.CombatForce);
				riChangHuoDongData.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
				int[] array = UIHelper.ParserGoodsString2(riChangHuoDongData.GoodsIDs);
				if (array != null)
				{
					riChangHuoDongData.EnterGoods = array[0];
					riChangHuoDongData.GoodsNumber = array[1];
				}
				string[] array2 = riChangHuoDongData.ImageStrings.Split(new char[]
				{
					'|'
				});
				if (array2 != null && array2.Length > 0)
				{
					this.ItemCollection.Clear();
					foreach (string text in array2)
					{
						if (!string.IsNullOrEmpty(text))
						{
							ShowNetImage showNetImage = U3DUtils.NEW<ShowNetImage>();
							this.ItemCollection.AddNoUpdate(showNetImage);
							showNetImage.Width = 610.0;
							showNetImage.Height = 318.0;
							showNetImage.URL = Super.GetTaskImageString2(text);
						}
					}
					this.ItemCollection.DelayUpdate();
					this._PageList.ItemCount = this.ItemCollection.Count;
				}
				this.BeginTimes = UIHelper.ParserTimeArrayString2(riChangHuoDongData.BeginTimes);
				this._Enter.isEnabled = true;
				this.RefreshDetail(riChangHuoDongData);
				break;
			}
		}
	}

	public void NotifyHuoDongData(string nSelfScore, int nEnterNum, string strName, string nTopScore)
	{
		this.CanEnter = false;
		if (!string.IsNullOrEmpty(strName))
		{
			this._TopListItemName[0].text = strName;
			this._TopListItemTime[0].text = nTopScore;
		}
		this.CanEnter = false;
		if (!string.IsNullOrEmpty(nSelfScore))
		{
			if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
			{
				this._TopListItemName[1].text = nSelfScore.ToString();
				this._TopListItemTime[1].text = null;
			}
			else
			{
				this._TopListItemName[1].text = Global.FormatRoleName(Global.Data.roleData);
				this._TopListItemTime[1].text = nSelfScore.ToString();
			}
		}
		else
		{
			this._TopListItemName[1].text = string.Empty;
			this._TopListItemTime[1].text = string.Empty;
		}
		if (this.SelectedItem == null)
		{
			return;
		}
		if (nEnterNum >= 0)
		{
			this.SelectedItem.EnterNum = nEnterNum;
			this.Status_Count.text = Global.GetLang("剩余次数:") + ColorCode.EncodingText2A((long)this.SelectedItem.EnterNum, (long)this.SelectedItem.MaxEnterNum, "fd010c", "fffffe");
		}
		else
		{
			this.SelectedItem.EnterNum = -1;
			this.Status_Count.text = null;
		}
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void NotifyEnterCopyMapCmdResult(int roleID, int retCode)
	{
		string text = string.Empty;
		if (this.SelectedItem != null)
		{
			text = this.SelectedItem.ItemName;
		}
		if (retCode < 0)
		{
			if (retCode == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的级别不满足【{0}】活动要求的进入级别范围"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您今天进入【{0}】活动的次数已经达到了最大限制"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -10)
			{
				string text2 = string.Empty;
				int num = 0;
				if (this.SelectedItem != null)
				{
					num = this.SelectedItem.EnterGoods;
					text2 = Global.GetGoodsNameByID(num, false);
				}
				if (num != 31060 && num != 31061)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("使用【{0}】进入活动时，背包中的【{0}】数量不足!"), new object[]
					{
						text2
					}), 0, -1, -1, 0);
				}
				else if (num == 31060)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【龙境凭证】数量不足,可通过击杀[真·战神][圣·法皇][仙·道尊]获取!"), new object[0]), 0, -1, -1, 0);
				}
				else if (num == 31061)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【鬼狱凭证】数量不足,可通过击杀[梦杀神][天蚩大帝][元始]获取!"), new object[0]), 0, -1, -1, 0);
				}
			}
			else if (retCode == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("队伍活动，只有组建队伍后才能申请进入"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -110)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("队伍活动，只有队伍队长才能申请进入"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1111)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前等待进入活动的排队人数太多，请稍后，再尝试进入..."), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进入活动地图时错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
		}
	}

	public ShowNetImage _bak;

	public GButton _Close;

	public GButton _Enter;

	public ListBox _TaskList;

	public ListBox _AwardGoods;

	public GScrollBarPageList _PageList;

	public TextBlock _Name;

	public TextBlock _FuBenName;

	public TextBlock Static_HuoDongXinXi;

	public TextBlock Static_HuoDongJiangLi;

	public TextBlock Static_ZuiGaoJiFen;

	public TextBlock Static_WoDeJiFen;

	public CText Need_Level;

	public CText Need_ZhanLi;

	public CText Need_Goods;

	public TextBlock Status_Count;

	public TextBlock Status_Time;

	public List<TextBlock> _TopListItemName = new List<TextBlock>();

	public List<TextBlock> _TopListItemTime = new List<TextBlock>();

	public List<int> BeginTimes;

	public int PrepareTime;

	private RiChangHuoDongData SelectedItem;

	public int CopyID = -1;

	public RiChangHuoDongTypes RiChangHuoDongType;

	public int LastAwardPoint;

	private int MapCode = -1;

	private bool CanEnter;

	private bool IsGoodsEnough = true;

	private GChildWindow LoadingWin;

	private long LastEnterCopyMapCmdTicks;
}
