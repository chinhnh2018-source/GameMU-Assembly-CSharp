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

public class JingYanFuBenDetailPart : UserControl
{
	public string bakURL
	{
		set
		{
			this._bak.URL = value;
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._TaskList.ItemsSource;
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this._Enter, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.jingYanFuBenDetailPart = this;
		}
		this._bak.URL = Global.GetGameResImageString("richangHuodongRichangFuben_bak2.jpg");
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnterFuBen);
		for (int i = 0; i < this._Count.Length; i++)
		{
			this._Count[i].text = string.Empty;
		}
		this._Enter.Text = Global.GetLang("进入副本");
		this._Name.text = Global.GetLang("日常副本");
		this.Static_FuBenXinXi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("副本信息")
		});
		this.Static_FuBenDiaoLuo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("副本掉落")
		});
		this.Static_ZuiGaoJiLu.text = Global.GetLang("最快通关");
		this.Static_WoDeSuDu.text = Global.GetLang("我的速度");
		this.Need_Level.Name = Global.GetLang("开启等级:");
		this.Need_ZhanLi.Name = Global.GetLang("推荐战力:");
		this.Need_Level._Text.transform.localPosition = new Vector3(60f, 0f, 0f);
		this.Need_ZhanLi._Text.transform.localPosition = new Vector3(125f, 0f, 0f);
	}

	private void EnterCopyMapCmd()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime - this.LastEnterCopyMapCmdTicks < 2000L)
		{
			return;
		}
		this.LastEnterCopyMapCmdTicks = correctLocalTime;
		int zhanLi = 0;
		if (!Global.CanEnterFuBenByZhanLi(this.SelectedItem.CopyID, out zhanLi))
		{
			PlayZone.GlobalPlayZone.OpenFuBenTiShiPartWindow(1, zhanLi);
			PlayZone.GlobalPlayZone.m_FuBenTiShiPart.dpsHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 1)
				{
					PlayZone.GlobalPlayZone.CloseFuBenTiShiPartWindow();
					GameInstance.Game.SpriteEnterFuBen(this.CopyID);
				}
			};
			return;
		}
		Super.ShowNetWaiting(string.Empty);
		if (this.CopyID == 5009)
		{
			Global.SendEvent("2800", Global.GetLang("经验副本挑战次数"));
		}
		if (this.CopyID == 5100)
		{
			Global.SendEvent("2700", Global.GetLang("金币副本挑战次数"));
		}
		GameInstance.Game.SpriteEnterFuBen(this.CopyID);
	}

	private void OnEnterFuBen(object sender, MouseEvent e)
	{
		SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenDetailpartEnter, HelpStateEvents.Clicked, -1);
		if (PlayZone.OnPreChangeMap(-1, 0))
		{
			return;
		}
		if (this.SelectedItem == null)
		{
			return;
		}
		if (!this.SelectedItem.LevelAllow)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未达到副本进入等级条件，无法进入该副本"), new object[]
			{
				this.SelectedItem.ItemName
			}), 0, -1, -1, 0);
			return;
		}
		if (this.SelectedItem.MaxEnterNum >= 0 && this.SelectedItem.EnterNum >= this.SelectedItem.MaxEnterNum)
		{
			int systemParamVipLeveValue = Global.GetSystemParamVipLeveValue("VIPJinYanFuBenNum");
			if (systemParamVipLeveValue <= this.SelectedItem.EnterNum - this.SelectedItem.MaxEnterNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("今日次数已满,无法再次进入"), 0, -1, -1, 0);
			}
			else if (!this.IsGoodsEnough)
			{
				string goodsNameByID = Global.GetGoodsNameByID(this.SelectedItem.EnterGoods, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("{0}不足{1}个,无法进入该副本"), goodsNameByID, this.SelectedItem.GoodsNumber), 0, -1, -1, 0);
			}
			else
			{
				if (this.SelectedItem.TabID == 501)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("今日次数已满,无法再次进入"), 0, -1, -1, 0);
					return;
				}
				if (this.SelectedItem.TabID == 500)
				{
					int cost;
					if (this.SelectedItem.FuBenType == 2)
					{
						cost = this.SelectedItem.NeedYuanBao;
					}
					else
					{
						cost = (this.SelectedItem.EnterNum - this.SelectedItem.MaxEnterNum + 1) * this.SelectedItem.NeedYuanBao;
					}
					string message = StringUtil.substitute(Global.GetLang("确定消耗{0}钻石进入【{1}】副本?"), new object[]
					{
						cost,
						this.SelectedItem.ItemName
					});
					NoTitleWindow noTitleWindow = Super.ShowDialogBox(this.Container, 1, message, 0, 0, 30000, string.Empty, "确定", "取消");
					noTitleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						if ((s1 as NoTitleWindow).DialogBoxReturn == 0)
						{
							if (Global.Data.roleData.UserMoney > cost)
							{
								this.EnterCopyMapCmd();
							}
							else
							{
								Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
							}
						}
						Super.CloseNoTitleWindow(Super.MainWindowRoot, s1 as NoTitleWindow);
						return true;
					};
				}
			}
		}
		else
		{
			this.EnterCopyMapCmd();
		}
	}

	private void OnSaoDangFuBen(object sender, MouseEvent e)
	{
		if (!this.SelectedItem.LevelAllow)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未达到扫荡该副本的条件!,无法扫荡副本"), new object[]
			{
				this.SelectedItem.ItemName
			}), 0, -1, -1, 0);
			return;
		}
		if (this.SelectedItem.EnterNum >= this.SelectedItem.MaxEnterNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】副本进入次数达到上限,无法扫荡副本!"), new object[]
			{
				this.SelectedItem.ItemName
			}), 0, -1, -1, 0);
			return;
		}
		if (this.PerfectTime > this.SaoDangTime)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("最短通关时间不满足扫荡条件,无法扫荡副本!"), 0, -1, -1, 0);
			return;
		}
		GameInstance.Game.SpriteSaoDangFuBen(this.MapCode, this.CopyID, (!this._HuiShou.Check) ? 0 : 1);
	}

	protected virtual void OnEnable()
	{
		foreach (RiChangHuoDongData riChangHuoDongData in this.ItemList)
		{
			GameInstance.Game.SpriteQureyFuBenInfo(riChangHuoDongData.MapCode, riChangHuoDongData.CopyID);
		}
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.jingYanFuBenDetailPart = null;
		}
		base.OnDestroy();
	}

	private bool CanEnter(int MinLevel, int MaxLevel, int minZhuanSheng, int maxZhuanSheng)
	{
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return false;
		}
		MinLevel = ((MinLevel != -1) ? MinLevel : 0);
		MaxLevel = ((MaxLevel != -1) ? MaxLevel : 4095);
		minZhuanSheng = ((minZhuanSheng != -1) ? minZhuanSheng : 0);
		maxZhuanSheng = ((maxZhuanSheng != -1) ? maxZhuanSheng : 4095);
		int num = MinLevel + minZhuanSheng * 65536;
		int num2 = MinLevel + maxZhuanSheng * 65536;
		int num3 = Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 65536;
		return num <= num3 && num3 <= num2;
	}

	private string EncodeEnterNumText(RiChangHuoDongData item)
	{
		string text;
		if (item.MaxEnterNum < 0)
		{
			text = string.Empty;
		}
		else
		{
			int num = (this.SelectedItem.EnterNum > this.SelectedItem.MaxEnterNum) ? this.SelectedItem.MaxEnterNum : this.SelectedItem.EnterNum;
			text = Global.GetLang("进入次数: ") + ColorCode.EncodingText2A((long)num, (long)item.MaxEnterNum, "fd010c", "fffffe");
			int systemParamVipLeveValue;
			if (this.CopyID == 5100)
			{
				systemParamVipLeveValue = Global.GetSystemParamVipLeveValue("VIPJinBiFuBenNum");
			}
			else
			{
				systemParamVipLeveValue = Global.GetSystemParamVipLeveValue("VIPJinYanFuBenNum");
			}
			if (systemParamVipLeveValue > 0)
			{
				int num2 = (this.SelectedItem.EnterNum <= this.SelectedItem.MaxEnterNum) ? 0 : (this.SelectedItem.EnterNum - this.SelectedItem.MaxEnterNum);
				text = text + Global.GetLang("\r\n购买进入次数: ") + ColorCode.EncodingText2A((long)num2, (long)systemParamVipLeveValue, "fd010c", "fffffe");
			}
		}
		return text;
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
		if (this.SelectedItem != null)
		{
			this.CopyID = item.CopyID;
			this.MapCode = item.MapCode;
			this.Need_Level.Text = ((!this.SelectedItem.LevelAllow) ? ColorCode.EncodingText(item.Level, "fd010c") : item.Level);
			this.Need_ZhanLi.Text = ((!this.SelectedItem.ZhanLiAllow) ? ColorCode.EncodingText(item.ZhanLi, "fd010c") : item.ZhanLi.ToString());
			if (this.SelectedItem.EnterGoods >= 0 && this.SelectedItem.GoodsNumber > 0)
			{
				this.IsGoodsEnough = (Global.GetTotalGoodsCountByID(this.SelectedItem.EnterGoods) >= this.SelectedItem.GoodsNumber);
			}
			if (this.SelectedItem.EnterNum >= this.SelectedItem.MaxEnterNum || this.IsGoodsEnough)
			{
				this._Enter.Text = Global.GetLang("进入副本");
			}
			else
			{
				this._Enter.Text = Global.GetLang("钻石进入");
			}
			this._AwardGoods.ItemsSource.Clear();
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(item.RewardGoods, 0, int.MaxValue);
			UIHelper.AddAwardGoods(this._AwardGoods.ItemsSource, goodsList, null, false, "bagGrid4_bak", false);
			this.PerfectTime = item.MyTopTime;
			this._TopListItemName[0].text = item.TopName;
			this._TopListItemTime[0].text = UIHelper.FormatSecs((long)item.TopTime, string.Empty);
			this.CanSaoDang = false;
			if (item.MyTopTime > 0)
			{
				this._TopListItemName[1].text = Global.FormatRoleName(Global.Data.roleData);
				this._TopListItemTime[1].text = UIHelper.FormatSecs((long)item.MyTopTime, string.Empty);
				if (item.MyTopTime <= item.MinSaoDangTime && item.EnterNum < item.MaxEnterNum)
				{
					this.CanSaoDang = true;
				}
			}
			else
			{
				this._TopListItemName[1].text = string.Empty;
				this._TopListItemTime[1].text = string.Empty;
			}
			item.SaoDangAllow = this.CanSaoDang;
		}
	}

	public void InitData(int tabID, string name)
	{
		bool flag = true;
		this.TabID = tabID;
		this._FuBenName.text = name;
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<RiChangFuBenItem> list = new List<RiChangFuBenItem>();
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		this.ItemList.Clear();
		this.ItemCollection.Clear();
		foreach (XElement xelement in xelementList)
		{
			if (Global.GetXElementAttributeInt(xelement, "TabID") == this.TabID)
			{
				if (this.ItemList.Count <= 0)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Display");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
					int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "Display");
					if (UIHelper.AvalidLevel(xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4, xelementAttributeInt5) == 0 || xelementAttributeInt6 != 2)
					{
						RiChangHuoDongData riChangHuoDongData = new RiChangHuoDongData();
						riChangHuoDongData.FuBenType = this.FuBenType;
						riChangHuoDongData.ImageStrings = Global.GetXElementAttributeStr(xelement, "Preview2");
						if (!string.IsNullOrEmpty(riChangHuoDongData.ImageStrings))
						{
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
										showNetImage.URL = Super.GetFuBenPreviewImageString2(text);
									}
								}
								this.ItemCollection.DelayUpdate();
								this._PageList.ItemCount = this.ItemCollection.Count;
							}
						}
						riChangHuoDongData.NeedYuanBao = Global.GetXElementAttributeInt(xelement, "NeedYuanBao");
						riChangHuoDongData.MinZhuanSheng = xelementAttributeInt4;
						riChangHuoDongData.MaxZhuanSheng = xelementAttributeInt5;
						riChangHuoDongData.MinLevel = xelementAttributeInt2;
						riChangHuoDongData.MaxLevel = xelementAttributeInt3;
						riChangHuoDongData.CopyID = Global.GetXElementAttributeInt(xelement, "ID");
						riChangHuoDongData.TabID = Global.GetXElementAttributeInt(xelement, "TabID");
						riChangHuoDongData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
						riChangHuoDongData.DisplayID = xelementAttributeInt;
						riChangHuoDongData.ItemName = Global.GetXElementAttributeStr(xelement, "CopyName");
						riChangHuoDongData.CopyType = ((Global.GetXElementAttributeInt(xelement, "CopyType") > 0) ? Global.GetLang("组队") : Global.GetLang("个人"));
						riChangHuoDongData.MaxEnterNum = Global.GetXElementAttributeInt(xelement, "EnterNumber");
						riChangHuoDongData.EnterGoods = Global.GetXElementAttributeInt(xelement, "EnterGoods");
						riChangHuoDongData.GoodsNumber = Global.GetXElementAttributeInt(xelement, "GoodsNumber");
						riChangHuoDongData.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
						riChangHuoDongData.RewardGoods = Global.GetXElementAttributeStr(xelement, "RewardGoods");
						riChangHuoDongData.Level = UIHelper.FormatLevelLimit(xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4, xelementAttributeInt5);
						riChangHuoDongData.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4, xelementAttributeInt5));
						riChangHuoDongData.ZhanLiAllow = (riChangHuoDongData.ZhanLi <= Global.Data.roleData.CombatForce);
						if (flag)
						{
							flag = false;
							this.RefreshDetail(riChangHuoDongData);
						}
						this.ItemList.Add(riChangHuoDongData);
						GameInstance.Game.SpriteQureyFuBenInfo(riChangHuoDongData.MapCode, riChangHuoDongData.CopyID);
					}
				}
			}
		}
		this.ItemCollection.DelayUpdate();
		SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenDetailpart, HelpStateEvents.Actived, -1);
	}

	public void NotifyFuBenData(int CopyID, int nClientSec, int nEnterNum, string strName, int nBestTimer, int nFinishNum)
	{
		int num = this.ItemList.FindIndex((RiChangHuoDongData x) => x.CopyID == CopyID);
		if (num < 0 || num > 2)
		{
			return;
		}
		RiChangHuoDongData riChangHuoDongData = this.ItemList[0];
		riChangHuoDongData.MyTopTime = nClientSec;
		riChangHuoDongData.TopName = strName;
		riChangHuoDongData.TopTime = nBestTimer;
		riChangHuoDongData.EnterNum = nEnterNum;
		riChangHuoDongData.FinishNum = nFinishNum;
		FuBenData fuBenData = Global.GetFuBenData(CopyID);
		if (fuBenData != null)
		{
			riChangHuoDongData.EnterNum = fuBenData.EnterNum;
		}
		this._Count[num].Text = this.EncodeEnterNumText(riChangHuoDongData);
		if (riChangHuoDongData == this.SelectedItem)
		{
			this.RefreshDetail(riChangHuoDongData);
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

	public void NotifyEnterCopyMapCmdResult(int roleID, int retCode)
	{
		Super.HideNetWaiting();
		string text = string.Empty;
		if (this.SelectedItem != null)
		{
			text = this.SelectedItem.ItemName;
			GameInstance.Game.SpriteQureyFuBenInfo(this.SelectedItem.MapCode, this.SelectedItem.CopyID);
		}
		if (retCode < 0)
		{
			if (retCode == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的级别不满足【{0}】副本要求的进入级别范围"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -50)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的转生级别不满足【{0}】副本要求的进入转生级别范围"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您今天进入【{0}】副本的次数已经达到了最大限制"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("今日购买次数上限已满"), 0, -1, -1, 0);
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
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("使用【{0}】进入副本时，背包中的【{0}】数量不足!"), new object[]
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
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("队伍副本，只有组建队伍后才能申请进入"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -110)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("队伍副本，只有队伍队长才能申请进入"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -1111)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前等待进入副本的排队人数太多，请稍后，再尝试进入..."), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进入副本地图时错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
		}
	}

	public ShowNetImage _bak;

	public GButton _Close;

	public GButton _Enter;

	public GButton _SaoDang;

	public TextBlock _Name;

	public TextBlock _FuBenName;

	public TextBlock Static_FuBenXinXi;

	public TextBlock Static_FuBenDiaoLuo;

	public TextBlock Static_ZuiGaoJiLu;

	public TextBlock Static_WoDeSuDu;

	public CText Need_Level;

	public CText Need_ZhanLi;

	public CText Need_Time;

	public GCheckBox _HuiShou;

	public TextBlock[] _Count;

	public ListBox _TaskList;

	public ListBox _AwardGoods;

	public GScrollBarPageList _PageList;

	public List<TextBlock> _TopListItemName = new List<TextBlock>();

	public List<TextBlock> _TopListItemTime = new List<TextBlock>();

	private int SaoDangTime;

	private RiChangHuoDongData SelectedItem;

	private List<RiChangHuoDongData> ItemList = new List<RiChangHuoDongData>();

	public int CopyID = -1;

	public int TabID = -1;

	private int MapCode = -1;

	public int FuBenType = -1;

	private int PerfectTime = int.MaxValue;

	private bool CanSaoDang;

	private bool IsGoodsEnough = true;

	private GChildWindow LoadingWin;

	private long LastEnterCopyMapCmdTicks;
}
