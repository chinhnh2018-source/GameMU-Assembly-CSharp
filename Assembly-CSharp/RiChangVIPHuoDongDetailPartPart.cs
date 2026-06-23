using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RiChangVIPHuoDongDetailPartPart : UserControl
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
			playZone._RiChangVIPHuoDongDetailPartPart = this;
		}
		this._PageList.ItemPerPage = 1;
		this._PageList.ItemCount = 0;
		this._bak.URL = Global.GetGameResImageString("richangHuodongRichangFuben_bak2.jpg");
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnterHuoDong);
		this._Name.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("日常活动")
		});
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
		this.Need_Level.Name = Global.GetLang("进入等级:");
		this.Need_ZhanLi.Name = Global.GetLang("推荐战力:");
		this.Need_Goods.Name = Global.GetLang("需要钻石:");
		this._Enter.Text = Global.GetLang("立即进入");
		this.Need_Level._Text.transform.localPosition = new Vector3(62f, 0f, 0f);
		this.Need_ZhanLi._Text.transform.localPosition = new Vector3(125f, 0f, 0f);
		this.Need_Goods._Text.transform.localPosition = new Vector3(112f, 0f, 0f);
		this._Name.Y = 235.0;
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
		this.LastEnterCopyMapCmdTicks = correctLocalTime;
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.BossZhiJia)
		{
			GameInstance.Game.SpriteRunNPCScript(-1, 600);
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.HuangJinShengDian)
		{
			GameInstance.Game.SpriteRunNPCScript(-1, 700);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("未知错误,请将游戏更新到最新版本再试!"), 0, -1, -1, 0);
		}
		this.CloseWindow();
	}

	private void OnEnterHuoDong(object sender, MouseEvent e)
	{
		if (PlayZone.OnPreChangeMap(-1, 0))
		{
			return;
		}
		if (!this.SelectedItem.LevelAllow)
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

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangHuoDongDetailPart = null;
		}
		base.OnDestroy();
	}

	public void RefreshDetail(RiChangHuoDongData item = null)
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
		this.CopyID = item.CopyID;
		this.MapCode = item.MapCode;
		this.Need_Level.textColor = ((!item.LevelAllow) ? 4294770956U : 4294967294U);
		this.Need_Level.Text = item.Level;
		this.Need_ZhanLi.textColor = ((!this.SelectedItem.ZhanLiAllow) ? 4294770956U : 4294967294U);
		this.Need_ZhanLi.Text = ((item.ZhanLi <= 0) ? string.Empty : item.ZhanLi.ToString());
		this.Need_Goods.Text = string.Format(Global.GetLang("{0}钻石"), item.EnterNeedZuanShi);
		this.Need_Goods.textColor = ((Global.Data.roleData.UserMoney < item.EnterNeedZuanShi) ? 4294770956U : 4294967294U);
		if (this.RiChangHuoDongType == RiChangHuoDongTypes.BossZhiJia)
		{
			GameInstance.Game.SpriteBloodCastleInfo();
		}
		else if (this.RiChangHuoDongType == RiChangHuoDongTypes.HuangJinShengDian)
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
		if (type != RiChangHuoDongTypes.BossZhiJia)
		{
			if (type != RiChangHuoDongTypes.HuangJinShengDian)
			{
				MUDebug.LogError<string>(new string[]
				{
					"未知的日常活动类型!"
				});
				return;
			}
			string file = "Config/HuangJinShengDian.Xml";
			this.InitXueSeChengBao(file, "HuangJinShengDian");
		}
		else
		{
			string file = "Config/BossZhiJia.Xml";
			this.InitXueSeChengBao(file, "BossZhiJia");
		}
	}

	public void InitXueSeChengBao(string file, string nodeName)
	{
		XElement gameResXml = Global.GetGameResXml(file);
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, nodeName);
		if (xelement == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinChangeLife");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxChangeLife");
		RiChangHuoDongData riChangHuoDongData = new RiChangHuoDongData();
		riChangHuoDongData.MinZhuanSheng = xelementAttributeInt3;
		riChangHuoDongData.MaxZhuanSheng = xelementAttributeInt4;
		riChangHuoDongData.MinLevel = xelementAttributeInt;
		riChangHuoDongData.MaxLevel = xelementAttributeInt2;
		riChangHuoDongData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
		riChangHuoDongData.ItemName = Global.GetXElementAttributeStr(xelement, "Name");
		riChangHuoDongData.MaxEnterNum = -1;
		riChangHuoDongData.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
		riChangHuoDongData.AwardGoodsList = Global.GetXElementAttributeStr(xelement, "Award");
		riChangHuoDongData.EnterNeedZuanShi = Global.GetXElementAttributeInt(xelement, "EnterNeedZuanShi");
		riChangHuoDongData.NeedZuanShiPerMin = Global.GetXElementAttributeInt(xelement, "MapTimeNeedZuanShi");
		riChangHuoDongData.ImageStrings = Global.GetXElementAttributeStr(xelement, "Image");
		riChangHuoDongData.Level = UIHelper.FormatLevelLimit(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4);
		riChangHuoDongData.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4));
		riChangHuoDongData.ZhanLiAllow = (riChangHuoDongData.ZhanLi <= Global.Data.roleData.CombatForce);
		string[] array = riChangHuoDongData.ImageStrings.Split(new char[]
		{
			'|'
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
		this._Enter.isEnabled = true;
		this.RefreshDetail(riChangHuoDongData);
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

	public CText Need_Level;

	public CText Need_ZhanLi;

	public CText Need_Goods;

	private RiChangHuoDongData SelectedItem;

	public int CopyID = -1;

	public RiChangHuoDongTypes RiChangHuoDongType;

	private int MapCode = -1;

	private bool IsGoodsEnough = true;

	private long LastEnterCopyMapCmdTicks;
}
