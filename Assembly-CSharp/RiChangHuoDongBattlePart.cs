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

public class RiChangHuoDongBattlePart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._TaskList.ItemsSource;
		}
	}

	private void InitPerfabText()
	{
		try
		{
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
			this.Activity_Time.Name = Global.GetLang("活动时间:");
			this.Need_Level.Name = Global.GetLang("进入等级:");
			this._Enter.Text = Global.GetLang("进入活动");
			this.Activity_Time._Text.transform.localPosition = new Vector3(145f, 0f, 0f);
			this.Need_Level._Text.transform.localPosition = new Vector3(62f, 0f, 0f);
			this.Static_JinTuanJiangLi.text = Global.GetLang("金团奖励");
			this.BtnHelp.gameObject.transform.localPosition = new Vector3(-270f, -80f, this.BtnHelp.gameObject.transform.localPosition.z);
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南/东南亚调试用：" + base.GetType().Name + "初始化报空,please check \"InitPerfabText()\"！"
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		GameInstance.Game.GetJieriFanbeiInfo();
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangHuoDongBattlePart = this;
		}
		this._PageList.ItemPerPage = 1;
		this._PageList.ItemCount = 0;
		this._bak.URL = Global.GetGameResImageString("richangHuodongRichangFuben_bak2.jpg");
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnterHuoDong);
		this.InitPerfabText();
		this.BtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow();
		};
	}

	private void EnterCopyMapCmd()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime - this.LastEnterCopyMapCmdTicks < 2000L)
		{
			return;
		}
		this.LastEnterCopyMapCmdTicks = correctLocalTime;
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.Battle)
		{
			GameInstance.Game.SpriteBattle(4, 0);
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.PKKing)
		{
			GameInstance.Game.SpriteRunNPCScript(-1, 400);
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			GameInstance.Game.SpriteRunNPCScript(-1, 500);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未知错误,请将游戏更新到最新版本再试!"), new object[0]), 0, -1, -1, 0);
		}
	}

	private void OnEnterHuoDong(object sender, MouseEvent e)
	{
		if (PlayZone.OnPreChangeMap(-1, 0))
		{
			return;
		}
		if (!this.CanEnter)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前不在【{0}】活动准备时间内,请稍候"), new object[]
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
		else if (!this.IsGoodsEnough)
		{
			string goodsNameByID = Global.GetGoodsNameByID(this.SelectedItem.EnterGoods, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("{0}不足{1}个,不能进入活动"), goodsNameByID, this.SelectedItem.GoodsNumber), 0, -1, -1, 0);
		}
		else if (Global.Data.CurrentCopyTeamData != null)
		{
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
					this.EnterCopyMapCmd();
				}
			}, -1);
		}
		else
		{
			this.EnterCopyMapCmd();
		}
	}

	private void ActivityTipEventHandler(int type, ActivityTipItem args)
	{
		this.ActivityActive = args.IsActive;
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangHuoDongBattlePart = null;
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			ActivityTipManager.UnRegActivityTipItem(1007, new ActivityTipEventHandler(this.ActivityTipEventHandler));
		}
		base.OnDestroy();
	}

	protected virtual void OnEnable()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	protected IEnumerator TickProc()
	{
		this.BeginTime = 0L;
		int nextTime = 0;
		for (;;)
		{
			DateTime now = Global.GetCorrectDateTime();
			int currentTime = now.Hour * 3600 + now.Minute * 60 + now.Second;
			if ((long)currentTime < this.BeginTime || nextTime <= currentTime)
			{
				if (this.BeginTimes == null)
				{
					this.PrepareTime = 0;
				}
				else if (this.BeginTimes.Count == 1)
				{
					this.BeginTime = (long)this.BeginTimes[0];
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
					this.BeginTime = (long)this.BeginTimes[0];
					nextTime = this.BeginTimes[this.BeginTimes.Count - 1];
					if ((long)currentTime >= this.BeginTime && currentTime < nextTime)
					{
						for (int i = 0; i < this.BeginTimes.Count - 2; i++)
						{
							if (currentTime >= this.BeginTimes[i] && currentTime < this.BeginTimes[i + 1])
							{
								this.BeginTime = (long)this.BeginTimes[i];
								nextTime = this.BeginTimes[i + 1];
								break;
							}
						}
					}
					else if ((long)currentTime < this.BeginTime)
					{
						this.BeginTime = (long)(nextTime - 86400);
						nextTime = this.BeginTimes[0];
					}
					else
					{
						this.BeginTime = (long)nextTime;
						nextTime = this.BeginTimes[0] + 86400;
					}
				}
			}
			string text = null;
			if (this.PrepareTime > 0)
			{
				if ((this.ActivityActive || (long)currentTime <= this.BeginTime + 60L) && (long)currentTime >= this.BeginTime && (long)this.PrepareTime + this.BeginTime >= (long)currentTime)
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"c39550",
						this.GetTimeFormatString(0),
						"00ff00",
						Global.GetLang("剩余") + UIHelper.FormatSecs(this.BeginTime + (long)this.PrepareTime - (long)currentTime, "-")
					});
				}
				else if (nextTime >= currentTime)
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"c39550",
						this.GetTimeFormatString(1),
						"fd010c",
						Global.GetLang("剩余") + UIHelper.FormatSecs((long)(nextTime - currentTime), string.Empty)
					});
				}
			}
			this.Status_Time.text = text;
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	private string GetTimeFormatString(int state)
	{
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.Battle)
		{
			if (state == 0)
			{
				return Global.GetLang("战斗结束倒计时:");
			}
			return Global.GetLang("下轮活动倒计时:");
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			if (state == 0)
			{
				return Global.GetLang("活动结束倒计时:");
			}
			return Global.GetLang("下轮活动倒计时:");
		}
		else
		{
			if (state == 0)
			{
				return Global.GetLang("入场倒计时:");
			}
			return Global.GetLang("开启倒计时:");
		}
	}

	private void RefreshDetail(RiChangHuoDongData item = null)
	{
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
		this.SelectedItem = item;
		this.CopyID = item.CopyID;
		this.MapCode = item.MapCode;
		this.PrepareTime = item.PrepareTime;
		if (this.BeginTimes != null && this.BeginTimes.Count > 0)
		{
			this.BeginTime = (long)this.BeginTimes[0];
			this.Activity_Time.Text = UIHelper.FormatSecsHM(this.BeginTime, string.Empty) + " - " + UIHelper.FormatSecsHM(this.BeginTime + (long)item.DuringTime, string.Empty);
			this.Activity_Time.textColor = ((!this.SelectedItem.ZhanLiAllow) ? 4294770956U : 4294967294U);
		}
		this.Need_Level.Text = item.Level;
		this.Need_Level.textColor = ((!item.LevelAllow) ? 4294770956U : 4294967294U);
		if (this.SelectedItem.EnterGoods >= 0 && this.SelectedItem.GoodsNumber > 0)
		{
			this.IsGoodsEnough = (Global.GetTotalGoodsCountByID(this.SelectedItem.EnterGoods) <= this.SelectedItem.GoodsNumber);
		}
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.Battle)
		{
			GameInstance.Game.SpriteQureyBattleInfo();
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.PKKing)
		{
			GameInstance.Game.SpriteGetTheKingOfPKActivityInfoCmd();
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			GameInstance.Game.SpriteGetAngelTempleInfoCmd();
		}
		this._AwardGoods.ItemsSource.Clear();
		List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(item.AwardGoodsList, 0, int.MaxValue);
		UIHelper.AddAwardGoods(this._AwardGoods.ItemsSource, goodsList, null, false, "bagGrid4_bak", false);
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			NGUITools.SetActive(this.TopListContainer, false);
			NGUITools.SetActive(this.JinTuanContainer, true);
			this.AwardGoodsJinTuan.ItemsSource.Clear();
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("AngelTempleAuctionShow", true);
			if (!string.IsNullOrEmpty(systemParamByName))
			{
				List<GoodsData> goodsList2 = UIHelper.ParseRewardGoodsList(systemParamByName, 0, int.MaxValue);
				UIHelper.AddAwardGoods(this.AwardGoodsJinTuan.ItemsSource, goodsList2, null, false, "bagGrid4_bak", false);
			}
		}
		else
		{
			NGUITools.SetActive(this.TopListContainer, true);
			NGUITools.SetActive(this.JinTuanContainer, false);
		}
	}

	public void InitData(RiChangHuoDongTypes type, string name)
	{
		this.RiChangHuoDongType = type;
		this._FuBenName.text = name;
		this._Enter.isEnabled = false;
		this.ActivityActive = true;
		this._TopListItemName[0].text = null;
		this._TopListItemTime[0].text = null;
		this._TopListItemName[1].text = null;
		this._TopListItemTime[1].text = null;
		switch (type)
		{
		case RiChangHuoDongTypes.Battle:
		{
			this.Static_ZuiGaoJiFen.Text = Global.GetLang("最高积分");
			this.Static_WoDeJiFen.Text = Global.GetLang("我的积分");
			string file = "Config/Battle.xml";
			this.InitZhenYingZhan(file, Global.GetLang("阵营战"));
			break;
		}
		case RiChangHuoDongTypes.PKKing:
		{
			this.Static_ZuiGaoJiFen.Text = Global.GetLang("最高积分");
			this.Static_WoDeJiFen.Text = Global.GetLang("我的积分");
			string file = "Config/ArenaBattle.xml";
			this.InitZhenYingZhan(file, Global.GetLang("PK之王"));
			break;
		}
		case RiChangHuoDongTypes.AngelTemple:
		{
			this.Static_ZuiGaoJiFen.Text = Global.GetLang("最高伤害");
			this.Static_WoDeJiFen.Text = Global.GetLang("上轮击杀者");
			string file = "Config/AngelTemple.xml";
			this.InitZhenYingZhan(file, Global.GetLang("天使神殿"));
			ActivityTipManager.RegActivityTipItem(1007, new ActivityTipEventHandler(this.ActivityTipEventHandler));
			break;
		}
		default:
			MUDebug.LogError<string>(new string[]
			{
				"未知的日常活动类型!"
			});
			break;
		}
	}

	public void InitZhenYingZhan(string file, string copyName)
	{
		XElement gameResXml = Global.GetGameResXml(file);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		using (List<XElement>.Enumerator enumerator = xelementList.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				XElement xelement = enumerator.Current;
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
				int maxLevel = -1;
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				int maxZhuanSheng = -1;
				RiChangHuoDongData riChangHuoDongData = new RiChangHuoDongData();
				riChangHuoDongData.MinZhuanSheng = xelementAttributeInt2;
				riChangHuoDongData.MaxZhuanSheng = maxZhuanSheng;
				riChangHuoDongData.MinLevel = xelementAttributeInt;
				riChangHuoDongData.MaxLevel = maxLevel;
				riChangHuoDongData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
				riChangHuoDongData.ItemName = copyName;
				riChangHuoDongData.MaxEnterNum = -1;
				riChangHuoDongData.AwardGoodsList = Global.GetXElementAttributeStr(xelement, "Award");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "WaitingEnterSecs");
				if (xelementAttributeInt3 > 1)
				{
					riChangHuoDongData.PrepareTime += xelementAttributeInt3;
				}
				xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "PrepareSecs");
				if (xelementAttributeInt3 > 1)
				{
					riChangHuoDongData.PrepareTime += xelementAttributeInt3;
				}
				xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "FightingSecs");
				if (xelementAttributeInt3 > 0)
				{
					riChangHuoDongData.DuringTime = riChangHuoDongData.PrepareTime + xelementAttributeInt3;
					if (this.RiChangHuoDongType == RiChangHuoDongTypes.Battle || this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple)
					{
						riChangHuoDongData.PrepareTime = riChangHuoDongData.DuringTime;
					}
				}
				riChangHuoDongData.BeginTimes = Global.GetXElementAttributeStr(xelement, "TimePoints");
				riChangHuoDongData.ImageStrings = Global.GetXElementAttributeStr(xelement, "Image");
				riChangHuoDongData.Level = UIHelper.FormatLevelLimit(xelementAttributeInt, maxLevel, xelementAttributeInt2, maxZhuanSheng);
				riChangHuoDongData.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt, maxLevel, xelementAttributeInt2, maxZhuanSheng));
				riChangHuoDongData.ZhanLiAllow = (riChangHuoDongData.ZhanLi <= Global.Data.roleData.CombatForce);
				string[] array = riChangHuoDongData.ImageStrings.Split(new char[]
				{
					'|',
					','
				});
				if (array != null && array.Length > 0)
				{
					this.ItemCollection.Clear();
					foreach (string text in array)
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
			}
		}
	}

	public void NotifyHuoDongData(int nSelfScore, int nEnterNum, string strName, string nTopScore)
	{
		this.PerfectTime = nSelfScore;
		if (!string.IsNullOrEmpty(strName))
		{
			this._TopListItemName[0].text = strName;
			this._TopListItemTime[0].text = nTopScore;
		}
		if (nSelfScore >= 0)
		{
			this._TopListItemName[1].text = Global.FormatRoleName(Global.Data.roleData);
			this._TopListItemTime[1].text = nSelfScore.ToString();
		}
		else
		{
			this._TopListItemName[1].text = string.Empty;
			this._TopListItemTime[1].text = string.Empty;
		}
	}

	public void NotifyHuoDongAngleTempleData(string lastKiller, string strTopName, string nTopScore)
	{
		if (!string.IsNullOrEmpty(strTopName))
		{
			this._TopListItemName[0].text = strTopName;
			this._TopListItemTime[0].text = nTopScore;
		}
		this._TopListItemName[1].text = lastKiller;
		this._TopListItemTime[1].text = null;
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
		Super.HideNetWaiting();
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
			else if (retCode != -10)
			{
				if (retCode == -100)
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
	}

	public void InitFanbei()
	{
		if ((this.RiChangHuoDongType == RiChangHuoDongTypes.Battle && Global.isFanbei(2)) || (this.RiChangHuoDongType == RiChangHuoDongTypes.PKKing && Global.isFanbei(3)) || (this.RiChangHuoDongType == RiChangHuoDongTypes.AngelTemple && Global.isFanbei(1)))
		{
			FanbeiPrefab fanbeiPrefab = U3DUtils.NEW<FanbeiPrefab>();
			fanbeiPrefab.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/RewartDouble.png";
			this.obj.Add(fanbeiPrefab);
			this.obj.gameObject.SetActive(true);
		}
	}

	public static ChangeableRulePart.RuleXml GetAngelTempleAuctionHelpData()
	{
		if (RiChangHuoDongBattlePart.m_compJinTuanHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AngelTempleAuctionIntro.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AngelTempleAuctionIntro.xml 出现错误"
				});
				return null;
			}
			RiChangHuoDongBattlePart.m_compJinTuanHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return RiChangHuoDongBattlePart.m_compJinTuanHelpData;
	}

	public void OpenHelpWindow()
	{
		ChangeableRulePart.RuleXml angelTempleAuctionHelpData = RiChangHuoDongBattlePart.GetAngelTempleAuctionHelpData();
		if (angelTempleAuctionHelpData == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到相关配置"
			});
			return;
		}
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		this.m_helpPart.SetHelpInfo(angelTempleAuctionHelpData.list);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.m_helpWindow);
			this.m_helpWindow = null;
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

	public CText Activity_Time;

	public TextBlock Status_Time;

	public ListBox AwardGoodsJinTuan;

	public GameObject JinTuanContainer;

	public GameObject TopListContainer;

	public TextBlock Static_JinTuanJiangLi;

	public GButton BtnHelp;

	public SpriteSL obj;

	public List<TextBlock> _TopListItemName = new List<TextBlock>();

	public List<TextBlock> _TopListItemTime = new List<TextBlock>();

	public List<int> BeginTimes;

	public long BeginTime;

	public int PrepareTime;

	private RiChangHuoDongData SelectedItem;

	[NonSerialized]
	public bool ActivityActive;

	public int CopyID = -1;

	public RiChangHuoDongTypes RiChangHuoDongType;

	private int MapCode = -1;

	private int PerfectTime = int.MaxValue;

	private bool CanEnter = true;

	private bool IsGoodsEnough = true;

	private GChildWindow LoadingWin;

	private long LastEnterCopyMapCmdTicks;

	private static ChangeableRulePart.RuleXml m_compJinTuanHelpData;

	protected GChildWindow m_helpWindow;

	protected CommonHelpWindow m_helpPart;
}
