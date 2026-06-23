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

public class RiChangFuBenDetailPart : UserControl
{
	public string bakURL
	{
		set
		{
			this._bak.URL = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this._HuiShou._Lable.text = Global.GetLang("自动将扫荡获得装备回收为魔晶");
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
		this.InitTextInPrefabs();
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.riChangFuBenDetailPart = this;
		}
		this._TaskList.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this._bak.URL = Global.GetGameResImageString("richangHuodongRichangFuben_bak2.jpg");
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnterFuBen);
		this._SaoDang.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSaoDangFuBen);
		this._SaoDang.isEnabled = false;
		this._SaoDang.Text = Global.GetLang("扫荡");
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
		this._TaskList.Z = 5.0;
		this.Need_Level._Text.transform.localPosition = new Vector3(65f, 0f, 0f);
		this.Need_ZhanLi._Text.transform.localPosition = new Vector3(140f, 0f, 0f);
		this.Need_Time._Text.transform.localPosition = new Vector3(75f, 0f, 0f);
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
		Global.SendEvent("2600", Global.GetLang("日常副本挑战次数"));
		GameInstance.Game.SpriteEnterFuBen(this.CopyID);
	}

	private void OnEnterFuBen(object sender, MouseEvent e)
	{
		SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenDetailpartEnter, HelpStateEvents.Clicked, -1);
		if (!this.SelectedItem.LevelAllow)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未达到副本进入等级条件，无法进入该副本"), new object[]
			{
				this.SelectedItem.ItemName
			}), 0, -1, -1, 0);
			return;
		}
		if ((this.SelectedItem.MaxEnterNum > 0 && this.SelectedItem.EnterNum >= this.SelectedItem.MaxEnterNum) || (this.SelectedItem.MaxFinishNum > 0 && this.SelectedItem.FinishNum >= this.SelectedItem.MaxFinishNum))
		{
			if (this.SelectedItem.NeedYuanBao <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您今天进入【{0}】副本的次数已经达到了最大限制,不允许扫荡!"), new object[]
				{
					this.SelectedItem.ItemName
				}), 0, -1, -1, 0);
				return;
			}
			if (!this.IsGoodsEnough)
			{
				string goodsNameByID = Global.GetGoodsNameByID(this.SelectedItem.EnterGoods, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("{0}不足{1}个,无法进入该副本"), goodsNameByID, this.SelectedItem.GoodsNumber), 0, -1, -1, 0);
				return;
			}
			int cost = int.MaxValue;
			if (this.FuBenType == 1)
			{
				cost = this.SelectedItem.NeedYuanBao;
			}
			else if (this.SelectedItem.MaxEnterNum > 0)
			{
				cost = (this.SelectedItem.EnterNum - this.SelectedItem.MaxEnterNum + 1) * this.SelectedItem.NeedYuanBao;
			}
			else if (this.SelectedItem.MaxFinishNum > 0)
			{
				cost = (this.SelectedItem.FinishNum - this.SelectedItem.MaxFinishNum + 1) * this.SelectedItem.NeedYuanBao;
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
		if ((this.SelectedItem.MaxEnterNum > 0 && this.SelectedItem.EnterNum >= this.SelectedItem.MaxEnterNum) || (this.SelectedItem.MaxFinishNum > 0 && this.SelectedItem.FinishNum >= this.SelectedItem.MaxFinishNum))
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
		Global.SendEvent("2601", Global.GetLang("日常副本扫荡次数"));
		GameInstance.Game.SpriteSaoDangFuBen(this.MapCode, this.CopyID, (!this._HuiShou.isChecked) ? 0 : 1);
	}

	protected virtual void OnEnable()
	{
		foreach (RiChangFuBenItem riChangFuBenItem in this.ItemList)
		{
			GameInstance.Game.SpriteQureyFuBenInfo(riChangFuBenItem.MapCode, riChangFuBenItem.CopyID);
		}
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.riChangFuBenDetailPart = null;
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

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		RiChangFuBenItem riChangFuBenItem = U3DUtils.AS<RiChangFuBenItem>(this._TaskList.SelectedItem);
		if (null == riChangFuBenItem)
		{
			return;
		}
		if (!riChangFuBenItem.IsEnabeld)
		{
			return;
		}
		if (this.SelectedItem != null && this.SelectedItem != riChangFuBenItem)
		{
			this.SelectedItem.SelectedState = false;
		}
		this.RefreshDetail(riChangFuBenItem);
	}

	private string EncodeEnterNumText(RiChangFuBenItem item)
	{
		string text = null;
		if (item.MaxEnterNum > 0)
		{
			if (item.EnterNum >= item.MaxEnterNum)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("今日剩余: "),
					"fd010c",
					string.Format(Global.GetLang("{0}次"), 0)
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("今日剩余: "),
					"fffffe",
					string.Format(Global.GetLang("{0}次"), item.MaxEnterNum - item.EnterNum)
				});
			}
		}
		else if (item.MaxFinishNum > 0)
		{
			if (item.FinishNum >= item.MaxFinishNum)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("完成次数: "),
					"fd010c",
					string.Format(Global.GetLang("{0}/{1}"), item.FinishNum, item.MaxFinishNum)
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("完成次数: "),
					"fffffe",
					string.Format(Global.GetLang("{0}/{1}"), item.FinishNum, item.MaxFinishNum)
				});
			}
		}
		else
		{
			text = string.Empty;
		}
		return text;
	}

	private void RefreshDetail(RiChangFuBenItem item = null)
	{
		if (null == item)
		{
			item = this.SelectedItem;
		}
		else
		{
			this.SelectedItem = item;
			this.SelectedItem.SelectedState = true;
		}
		if (null != this.SelectedItem)
		{
			this.CopyID = item.CopyID;
			this.MapCode = item.MapCode;
			this.SaoDangTime = item.MinSaoDangTime;
			this.Need_Level.Text = ((!this.SelectedItem.LevelAllow) ? ColorCode.EncodingText(item.Level, "fd010c") : item.Level);
			this.Need_ZhanLi.Text = ((!this.SelectedItem.ZhanLiAllow) ? ColorCode.EncodingText(item.ZhanLi, "fd010c") : item.ZhanLi.ToString());
			if (this.FuBenType == 1)
			{
				this.Need_Time.Text = Global.GetGoodsNameByID(item.EnterGoods, false);
			}
			else
			{
				this.Need_Time.Text = ((item.MinSaoDangTime != -1) ? string.Format(Global.GetLang("{0}内通关"), UIHelper.FormatSecs((long)item.MinSaoDangTime, "-")) : "-");
			}
			if (this.SelectedItem.EnterGoods >= 0 && this.SelectedItem.GoodsNumber > 0)
			{
				this.IsGoodsEnough = (Global.GetTotalGoodsCountByID(this.SelectedItem.EnterGoods) >= this.SelectedItem.GoodsNumber);
			}
			if ((this.SelectedItem.MaxEnterNum > 0 && this.SelectedItem.EnterNum < this.SelectedItem.MaxEnterNum) || (this.SelectedItem.MaxFinishNum > 0 && this.SelectedItem.FinishNum < this.SelectedItem.MaxFinishNum) || !this.IsGoodsEnough || this.SelectedItem.NeedYuanBao <= 0)
			{
				this._Enter.Text = Global.GetLang("进入副本");
			}
			else
			{
				this._Enter.Text = Global.GetLang("钻石进入");
			}
			this._AwardGoods.ItemsSource.Clear();
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(item.RewardGoods, 0, int.MaxValue);
			UIHelper.AddAwardGoods(this._AwardGoods.ItemsSource, goodsList, null, false, "bagGrid4_bak", true);
			this.PerfectTime = item.MyTopTime;
			this._TopListItemName[0].text = item.TopName;
			this._TopListItemTime[0].text = UIHelper.FormatSecs((long)item.TopTime, string.Empty);
			this.CanSaoDang = false;
			if (item.MyTopTime > 0)
			{
				this._TopListItemName[1].text = Global.FormatRoleName(Global.Data.roleData);
				this._TopListItemTime[1].text = UIHelper.FormatSecs((long)item.MyTopTime, string.Empty);
				if (item.MyTopTime <= item.MinSaoDangTime && ((item.MaxEnterNum > 0 && item.EnterNum < item.MaxEnterNum) || (item.MaxFinishNum > 0 && item.FinishNum < item.MaxFinishNum)))
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
			this._SaoDang.isEnabled = this.CanSaoDang;
		}
	}

	public void InitData(int tabID, string name)
	{
		bool flag = true;
		this.TabID = tabID;
		this._FuBenName.text = name;
		if (this.FuBenType == 1)
		{
			this._Title.spriteName = "zhuduiFuben";
			this.Need_Time.Name = Global.GetLang("进入消耗:");
			this._SaoDang.gameObject.SetActive(false);
		}
		else
		{
			this._Title.spriteName = "juqinFuben";
			this.Need_Time.Name = Global.GetLang("扫荡要求:");
			this._SaoDang.gameObject.SetActive(true);
		}
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<RiChangFuBenItem> list = new List<RiChangFuBenItem>();
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		this.ItemList.Clear();
		this.ItemCollection.Clear();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (Global.GetXElementAttributeInt(xelement, "TabID") == this.TabID)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Display");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "UpCopyID");
				RiChangFuBenItem riChangFuBenItem = U3DUtils.NEW<RiChangFuBenItem>("RiChangFuBenItem");
				Vector3 localScale = riChangFuBenItem._Preview.transform.localScale;
				localScale.x = 180f;
				localScale.y = 253f;
				riChangFuBenItem._Preview.transform.localScale = localScale;
				riChangFuBenItem.bounds = new Bounds(new Vector3(0f, 18f, 0f), new Vector3(180f, 256f, 0f));
				if (xelementAttributeInt6 > 0)
				{
					riChangFuBenItem.WeiKaiQi.gameObject.SetActive(true);
					XElement xelement2 = Global.GetXElement(gameResXml, "Copy", "ID", xelementAttributeInt6.ToString());
					riChangFuBenItem.NeedText.Text = string.Format(Global.GetLang("通关[{0}]"), Global.GetXElementAttributeStr(xelement2, "CopyName"));
					riChangFuBenItem.RenWuTitle.Text = string.Empty;
					riChangFuBenItem.IsEnabeld = false;
				}
				this.ItemCollection.AddNoUpdate(riChangFuBenItem);
				riChangFuBenItem.NeedYuanBao = Global.GetXElementAttributeInt(xelement, "NeedYuanBao");
				riChangFuBenItem.MinZhuanSheng = xelementAttributeInt4;
				riChangFuBenItem.MaxZhuanSheng = xelementAttributeInt5;
				riChangFuBenItem.MinLevel = xelementAttributeInt2;
				riChangFuBenItem.MaxLevel = xelementAttributeInt3;
				riChangFuBenItem.name = this.ItemCollection.Count.ToString("0000");
				riChangFuBenItem.GrayTexture = true;
				riChangFuBenItem.SelectedState = false;
				riChangFuBenItem.CopyID = Global.GetXElementAttributeInt(xelement, "ID");
				riChangFuBenItem.TabID = Global.GetXElementAttributeInt(xelement, "TabID");
				riChangFuBenItem.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
				riChangFuBenItem.DisplayID = xelementAttributeInt;
				riChangFuBenItem.ItemName = Global.GetXElementAttributeStr(xelement, "CopyName");
				riChangFuBenItem.CopyType = ((Global.GetXElementAttributeInt(xelement, "CopyType") > 0) ? Global.GetLang("组队") : Global.GetLang("个人"));
				riChangFuBenItem.MaxEnterNum = Global.GetXElementAttributeInt(xelement, "EnterNumber");
				riChangFuBenItem.MaxFinishNum = Global.GetXElementAttributeInt(xelement, "FinishNumber");
				riChangFuBenItem.EnterGoods = Global.GetXElementAttributeInt(xelement, "EnterGoods");
				riChangFuBenItem.GoodsNumber = Global.GetXElementAttributeInt(xelement, "GoodsNumber");
				riChangFuBenItem.ZhanLi = Global.GetXElementAttributeInt(xelement, "ZhanLi");
				riChangFuBenItem.RewardGoods = Global.GetXElementAttributeStr(xelement, "RewardGoods");
				riChangFuBenItem.MinSaoDangTime = Global.GetFuBenMapMinSaoDangTime(riChangFuBenItem.MapCode) * 60;
				riChangFuBenItem._Preview.URL = Super.GetFuBenPreviewImageString(Global.GetXElementAttributeStr(xelement, "Preview2"));
				riChangFuBenItem.Level = UIHelper.FormatLevelLimit(xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4, xelementAttributeInt5);
				riChangFuBenItem.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4, xelementAttributeInt5));
				riChangFuBenItem.ShowDetail = false;
				riChangFuBenItem.ZhanLiAllow = (riChangFuBenItem.ZhanLi <= Global.Data.roleData.CombatForce);
				list.Add(riChangFuBenItem);
				if (flag)
				{
					flag = false;
					this.RefreshDetail(riChangFuBenItem);
				}
				this.ItemList.Add(riChangFuBenItem);
				GameInstance.Game.SpriteQureyFuBenInfo(riChangFuBenItem.MapCode, riChangFuBenItem.CopyID);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	public void NotifyFuBenData(int CopyID, int nClientSec, int nEnterNum, string strName, int nBestTimer, int nFinishNum, bool bIsOpen)
	{
		int num = this.ItemList.FindIndex((RiChangFuBenItem x) => x.CopyID == CopyID);
		if (num < 0 || num > 2)
		{
			return;
		}
		RiChangFuBenItem riChangFuBenItem = this.ItemList[num];
		riChangFuBenItem.MyTopTime = nClientSec;
		riChangFuBenItem.TopName = strName;
		riChangFuBenItem.TopTime = nBestTimer;
		riChangFuBenItem.EnterNum = nEnterNum;
		riChangFuBenItem.FinishNum = nFinishNum;
		riChangFuBenItem.bIsOpen = bIsOpen;
		FuBenData fuBenData = Global.GetFuBenData(CopyID);
		if (fuBenData != null)
		{
			riChangFuBenItem.EnterNum = fuBenData.EnterNum;
			riChangFuBenItem.FinishNum = fuBenData.FinishNum;
		}
		if (this.FuBenType == 1)
		{
			this._Count[num].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("进入消耗: "),
				"00ff00",
				Global.GetGoodsNameByID(riChangFuBenItem.EnterGoods, false)
			});
		}
		else
		{
			this._Count[num].Text = this.EncodeEnterNumText(riChangFuBenItem);
		}
		if (riChangFuBenItem == this.SelectedItem)
		{
			this.RefreshDetail(riChangFuBenItem);
		}
		if (riChangFuBenItem.bIsOpen)
		{
			riChangFuBenItem.WeiKaiQi.gameObject.SetActive(false);
			riChangFuBenItem.IsEnabeld = true;
			if (riChangFuBenItem != this.SelectedItem)
			{
			}
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

	public void ResetGetNewData()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
	}

	public void NotifyEnterCopyMapCmdResult(int roleID, int retCode)
	{
		Super.HideNetWaiting();
		string text = string.Empty;
		if (null != this.SelectedItem)
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
			else if (retCode == -10)
			{
				string text2 = string.Empty;
				int num = 0;
				if (null != this.SelectedItem)
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

	public void NotifySaoDangFuBenCmdResult(int roleID, int retCode)
	{
		string text = string.Empty;
		if (null != this.SelectedItem)
		{
			text = this.SelectedItem.ItemName;
			GameInstance.Game.SpriteQureyFuBenInfo(this.SelectedItem.MapCode, this.SelectedItem.CopyID);
		}
		if (retCode < 0)
		{
			if (retCode == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的通关时间没有达到【{0}】副本扫荡要求的最短时间"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您今天进入【{0}】副本的次数已经达到了最大限制,不允许扫荡!"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (retCode == -4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("扫荡需要VIP{0}"), new object[]
				{
					ConfigSystemParam.GetSystemParamByName("VIPSaoDang", true)
				}), 0, -1, -1, 0);
			}
			else if (retCode == -10)
			{
				string text2 = string.Empty;
				int num = 0;
				if (null != this.SelectedItem)
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

	public UISprite _Title;

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

	public UICheckbox _HuiShou;

	public TextBlock[] _Count;

	public ListBox _TaskList;

	public ListBox _AwardGoods;

	public List<TextBlock> _TopListItemName = new List<TextBlock>();

	public List<TextBlock> _TopListItemTime = new List<TextBlock>();

	private int SaoDangTime;

	private RiChangFuBenItem SelectedItem;

	private List<RiChangFuBenItem> ItemList = new List<RiChangFuBenItem>();

	public int CopyID = -1;

	public int TabID = -1;

	public int FuBenType = -1;

	private int MapCode = -1;

	private int PerfectTime = int.MaxValue;

	private bool CanSaoDang;

	private bool IsGoodsEnough = true;

	private GChildWindow LoadingWin;

	private long LastEnterCopyMapCmdTicks;
}
