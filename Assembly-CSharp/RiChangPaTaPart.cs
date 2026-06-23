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

public class RiChangPaTaPart : UserControl
{
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

	public ObservableCollection ItemCollectionAward
	{
		get
		{
			return this._ItemCollectionAward;
		}
		set
		{
			this._ItemCollectionAward = value;
		}
	}

	public ObservableCollection ItemCollectionAwardFirst
	{
		get
		{
			return this._ItemCollectionAwardFirst;
		}
		set
		{
			this._ItemCollectionAwardFirst = value;
		}
	}

	public ObservableCollection ItemCollectionSaodang
	{
		get
		{
			return this._ItemCollectionSaodang;
		}
		set
		{
			this._ItemCollectionSaodang = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnLingquJiangli.Text = Global.GetLang("领取奖励");
		this.BtnShaodang.Text = Global.GetLang("扫荡");
		this.BtnTiaozhan.Text = Global.GetLang("挑战");
		this.BtnTop.Text = Global.GetLang("排行榜");
		if (this.ConstTexts != null && this.ConstTexts.Length == 3)
		{
			this.ConstTexts[0].Text = Global.GetLang("扫荡中…");
			this.ConstTexts[1].Text = Global.GetLang("扫荡次数:");
			this.ConstTexts[2].Text = Global.GetLang("扫荡层数:");
		}
		this.MyText.X = -100.0;
		this.MaxText.X = -100.0;
		this.ConstTexts[1].X = -240.0;
		this.NumText.X = -165.0;
		this.ConstTexts[2].X = -65.0;
		this.TimeText.X = -313.0;
		this.ChenshuText.X = -100.0;
		this.ZhanliText.X = 80.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.ItemList.ItemsSource;
		this.ItemList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ItemMouseLeftButtonUp);
		this.ItemCollectionAward = this.GoodsList1.ItemsSource;
		this.ItemCollectionAwardFirst = this.GoodsList2.ItemsSource;
		this.ItemCollectionSaodang = this.SaodangList.ItemsSource;
		this.BtnTiaozhan.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnterFuBen);
		this.BtnShaodang.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSaoDangFuBen);
		this.BtnLingquJiangli.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnLingquJiangli);
		this.BtnTop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			(Super.GData.PlayZoneRoot as PlayZone).ShowTopListPartWindow(4);
		};
		this.initDataDict(600);
	}

	public void InitPartData(PataSweepTypes mode = PataSweepTypes.None)
	{
		this.initAllText();
		this.GetTiaozhanData();
	}

	private void initAllText()
	{
		this.MyText.Text = string.Empty;
		this.MaxText.Text = string.Empty;
		this.NumText.Text = string.Empty;
		this.ChenshuText.Text = string.Empty;
		this.ZhanliText.Text = string.Empty;
		this.TimeText.Text = string.Empty;
		this.SaodangStateText.Text = string.Empty;
	}

	private void setUIVisible(PataSweepTypes mode)
	{
		if (mode == PataSweepTypes.None)
		{
			this.Bottom0.Visibility = true;
			this.Bottom1.Visibility = false;
			this.Right0.Visibility = true;
			this.Right1.Visibility = false;
			this.NumText.Text = string.Format("{0}/{1}", this.usedSaodangNum, Global.GetMaxShaodangNum());
		}
		else if (mode > PataSweepTypes.None)
		{
			this.Bottom0.Visibility = false;
			this.Bottom1.Visibility = true;
			this.Right0.Visibility = false;
			this.Right1.Visibility = true;
			if (mode == PataSweepTypes.Doing)
			{
				this.saodangTimeSec = this.getMaxSaodangIndex() * 2 - (this.currentSaodangIndex - 1) * 2;
				this.TimeText.Text = this.refreshSaodangTime(this.saodangTimeSec);
			}
		}
	}

	private void setUIEndable(int index)
	{
		int index2 = this.getIndex(index);
		if (this.SelectedItem.Index == index2)
		{
			this.BtnTiaozhan.isEnabled = true;
		}
		else
		{
			this.BtnTiaozhan.isEnabled = false;
		}
		int maxSaodangIndex = this.getMaxSaodangIndex();
		if (maxSaodangIndex > 0)
		{
			this.BtnShaodang.isEnabled = true;
		}
		else
		{
			this.BtnShaodang.isEnabled = false;
		}
	}

	public static void ClearXMLData()
	{
		if (RiChangPaTaPart.DataDict != null)
		{
			RiChangPaTaPart.DataDict.Clear();
			RiChangPaTaPart.DataDict = null;
		}
	}

	private void initDataDict(int tabID = 600)
	{
		if (RiChangPaTaPart.DataDict != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		int[] pataIndexRange = Global.GetPataIndexRange();
		if (pataIndexRange == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy", "ID", pataIndexRange[0], pataIndexRange[1]);
		if (RiChangPaTaPart.DataDict == null)
		{
			RiChangPaTaPart.DataDict = new Dictionary<int, RiChangHuoDongData>();
		}
		for (int i = xelementList.Count - 1; i >= 0; i--)
		{
			XElement xelement = xelementList[i];
			if (Global.GetXElementAttributeInt(xelement, "TabID") == tabID)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Display");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "Display");
				RiChangHuoDongData riChangHuoDongData = new RiChangHuoDongData();
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
				if (!RiChangPaTaPart.DataDict.ContainsKey(riChangHuoDongData.CopyID))
				{
					RiChangPaTaPart.DataDict.Add(riChangHuoDongData.CopyID, riChangHuoDongData);
				}
			}
		}
	}

	private void loadList(int index, int tabID = 600)
	{
		if (RiChangPaTaPart.DataDict == null)
		{
			return;
		}
		this.currentIndex = index;
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		string[] pataIndexRange = Global.GetPataIndexRange((int)Math.Ceiling((double)index / 10.0));
		if (pataIndexRange == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy", "ID", Convert.ToInt32(pataIndexRange[1]), Convert.ToInt32(pataIndexRange[2]));
		int num = pataIndexRange[1].SafeToInt32(0);
		int num2 = pataIndexRange[2].SafeToInt32(0);
		this.ItemCollection.Clear();
		foreach (RiChangHuoDongData riChangHuoDongData in RiChangPaTaPart.DataDict.Values)
		{
			if (riChangHuoDongData.CopyID >= num && riChangHuoDongData.CopyID <= num2)
			{
				RiChangPaTaItem riChangPaTaItem = U3DUtils.NEW<RiChangPaTaItem>();
				riChangHuoDongData.LevelAllow = (0 == UIHelper.AvalidLevel(riChangHuoDongData.MinLevel, riChangHuoDongData.MaxLevel, riChangHuoDongData.MinZhuanSheng, riChangHuoDongData.MaxZhuanSheng));
				riChangHuoDongData.ZhanLiAllow = (riChangHuoDongData.ZhanLi <= Global.Data.roleData.CombatForce);
				int num3 = riChangHuoDongData.CopyID - num;
				riChangPaTaItem.TxtName.Text = riChangHuoDongData.ItemName;
				riChangPaTaItem.Data = riChangHuoDongData;
				riChangPaTaItem.Index = num3 + 1;
				int index2 = this.getIndex(index);
				if (num3 > index2 - 1)
				{
					riChangPaTaItem.TxtName.textColor = 5921370U;
				}
				else if (num3 == index2 - 1)
				{
					riChangPaTaItem.TxtName.textColor = 16711680U;
				}
				else
				{
					riChangPaTaItem.TxtName.textColor = 16429056U;
				}
				this.ItemCollection.AddNoUpdate(riChangPaTaItem);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void locationPos(int index)
	{
		int num = 41;
		int num2 = 7;
		if (index >= this.max - 3)
		{
			index = this.max;
		}
		else if (index <= num2)
		{
			index = num2;
		}
		Vector4 clipRange = this.ItemPanel.clipRange;
		clipRange.y = (float)(-(float)(this.max - index) * num);
		this.ItemPanel.clipRange = clipRange;
		Vector3 localPosition = this.ItemPanel.transform.localPosition;
		this.ItemPanel.transform.localPosition = new Vector3(localPosition.x, Mathf.Abs(clipRange.y + 5.5f), 0f);
	}

	private void selectItem(int index)
	{
		index = this.getIndex(index);
		this.ItemList.SelectedIndex = this.max - index;
		RiChangPaTaItem riChangPaTaItem = U3DUtils.AS<RiChangPaTaItem>(this.ItemList.SelectedItem);
		if (null == riChangPaTaItem)
		{
			return;
		}
		riChangPaTaItem.SelectStat = true;
		this.SelectedItem = riChangPaTaItem;
		this.locationPos(index);
		this.refreshData(riChangPaTaItem);
	}

	private void refreshData(RiChangPaTaItem item)
	{
		this.setUIEndable(this.currentIndex);
		this.ChenshuText.Text = item.TxtName.Text;
		this.ZhanliText.Text = item.Data.ZhanLi.ToString();
		XElement fuBenMapElement = Global.GetFuBenMapElement(item.Data.MapCode, -1);
		if (fuBenMapElement == null)
		{
			return;
		}
		this.ItemCollectionAward.Clear();
		this.ItemCollectionAwardFirst.Clear();
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(8014);
		dummyGoodsData.GCount = Global.GetXElementAttributeInt(fuBenMapElement, "Moneyaward");
		if (dummyGoodsData.GCount > 0)
		{
			UIHelper.AddGoodsIcon(this.ItemCollectionAward, dummyGoodsData, null, false, "bagGrid4_bak");
		}
		dummyGoodsData = Global.GetDummyGoodsData(8002);
		dummyGoodsData.GCount = Global.GetXElementAttributeInt(fuBenMapElement, "Experienceaward");
		if (dummyGoodsData.GCount > 0)
		{
			UIHelper.AddGoodsIcon(this.ItemCollectionAward, dummyGoodsData, null, false, "bagGrid4_bak");
		}
		dummyGoodsData = Global.GetDummyGoodsData(8017);
		dummyGoodsData.GCount = Global.GetXElementAttributeInt(fuBenMapElement, "XingHunaward");
		if (dummyGoodsData.GCount > 0)
		{
			UIHelper.AddGoodsIcon(this.ItemCollectionAward, dummyGoodsData, null, false, "bagGrid4_bak");
		}
		UIHelper.AddAwardGoods(this.ItemCollectionAward, Global.GetXElementAttributeStr(fuBenMapElement, "GoodsIDs"));
		UIHelper.AddAwardGoods(this.ItemCollectionAwardFirst, Global.GetXElementAttributeStr(fuBenMapElement, "FirstGoodsID"));
		dummyGoodsData = Global.GetDummyGoodsData(8002);
		dummyGoodsData.GCount = Global.GetXElementAttributeInt(fuBenMapElement, "FirstExp");
		if (dummyGoodsData.GCount > 0)
		{
			UIHelper.AddGoodsIcon(this.ItemCollectionAwardFirst, dummyGoodsData, null, false, "bagGrid4_bak");
		}
		dummyGoodsData = Global.GetDummyGoodsData(8014);
		dummyGoodsData.GCount = Global.GetXElementAttributeInt(fuBenMapElement, "FirstGold");
		if (dummyGoodsData.GCount > 0)
		{
			UIHelper.AddGoodsIcon(this.ItemCollectionAwardFirst, dummyGoodsData, null, false, "bagGrid4_bak");
		}
		dummyGoodsData = Global.GetDummyGoodsData(8017);
		dummyGoodsData.GCount = Global.GetXElementAttributeInt(fuBenMapElement, "FirstXingHun");
		if (dummyGoodsData.GCount > 0)
		{
			UIHelper.AddGoodsIcon(this.ItemCollectionAwardFirst, dummyGoodsData, null, false, "bagGrid4_bak");
		}
	}

	private void refreshSaodangPersent()
	{
		int maxSaodangIndex = this.getMaxSaodangIndex();
		if (maxSaodangIndex == 0)
		{
			return;
		}
		this.ShaodangProgressBar.Percent = (double)this.currentSaodangIndex / (double)maxSaodangIndex;
		this.ShaodangProgressBar.uiLabel.text = string.Format("{0}/{1}", this.currentSaodangIndex, maxSaodangIndex);
	}

	private string refreshSaodangTime(int sec)
	{
		if (sec == 0)
		{
			return string.Empty;
		}
		return Global.GetColorStringForNGUIText(new object[]
		{
			"c8b188",
			Global.GetLang("剩余时间:"),
			"39f348",
			Global.GetTimeStrBySecEx((double)sec, true, -1)
		});
	}

	private void refreshSaodangText(PataSweepTypes mode, bool isReboot = false)
	{
		this.BtnLingquJiangli.BtnTag = mode.ToString();
		if (isReboot)
		{
			this.BtnLingquJiangli.BtnTag = this.currentSaodangIndex.ToString();
			this.SaodangStateText.Text = Global.GetLang(string.Empty);
			this.BtnLingquJiangli.Text = Global.GetLang("继续扫荡");
			this.BtnLingquJiangli.isEnabled = true;
		}
		else if (mode == PataSweepTypes.Done)
		{
			this.SaodangStateText.Text = Global.GetLang("扫荡完成");
			this.BtnLingquJiangli.Text = Global.GetLang("领取奖励");
			this.BtnLingquJiangli.isEnabled = true;
		}
		else if (mode == PataSweepTypes.Doing)
		{
			this.SaodangStateText.Text = Global.GetLang("扫荡中...");
			this.BtnLingquJiangli.Text = Global.GetLang("领取奖励");
			this.BtnLingquJiangli.isEnabled = false;
		}
	}

	private int getIndex(int index)
	{
		if (index <= 0)
		{
			return 1;
		}
		int num = index % 10;
		return (num != 0) ? num : this.max;
	}

	private int getMaxSaodangIndex()
	{
		if (this.currentIndex >= this.maxIndex)
		{
			return this.currentIndex / 1 * 1;
		}
		return (this.currentIndex - 1) / 1 * 1;
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime > this.startTicks)
		{
			int num = (int)((correctLocalTime - this.startTicks) / 1000L);
			if (num >= this.saodangTimeSec)
			{
				this.TimeText.Text = this.refreshSaodangTime(0);
				base.CancelInvoke("TickProc");
			}
			else
			{
				this.TimeText.Text = this.refreshSaodangTime(this.saodangTimeSec - num);
			}
		}
	}

	private void ItemMouseLeftButtonUp(object sender, MouseEvent e)
	{
		RiChangPaTaItem riChangPaTaItem = U3DUtils.AS<RiChangPaTaItem>(this.ItemList.SelectedItem);
		if (null == riChangPaTaItem)
		{
			return;
		}
		if (this.SelectedItem != null && this.SelectedItem != riChangPaTaItem)
		{
			this.SelectedItem.SelectStat = false;
		}
		if (this.SelectedItem == riChangPaTaItem)
		{
			return;
		}
		this.SelectedItem = riChangPaTaItem;
		this.SelectedItem.SelectStat = true;
		this.refreshData(this.SelectedItem);
	}

	private void OnEnterFuBen(object sender, MouseEvent e)
	{
		SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenPaTaFuBenPartEnter, HelpStateEvents.Clicked, -1);
		if (!this.SelectedItem.Data.LevelAllow)
		{
			if (this.currentIndex > 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要达到{0}转{1}级后，才能继续挑战"), new object[]
				{
					this.SelectedItem.Data.MinZhuanSheng,
					this.SelectedItem.Data.MinLevel
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要达到{0}转{1}级后，才能挑战"), new object[]
				{
					this.SelectedItem.Data.MinZhuanSheng,
					this.SelectedItem.Data.MinLevel
				}), 0, -1, -1, 0);
			}
			return;
		}
		int zhanLi = 0;
		if (!Global.CanEnterFuBenByZhanLi(this.SelectedItem.Data.CopyID, out zhanLi))
		{
			PlayZone.GlobalPlayZone.OpenFuBenTiShiPartWindow(1, zhanLi);
			PlayZone.GlobalPlayZone.m_FuBenTiShiPart.dpsHandler = delegate(object s, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 1)
				{
					PlayZone.GlobalPlayZone.CloseFuBenTiShiPartWindow();
					GameInstance.Game.SpriteEnterFuBen(this.SelectedItem.Data.CopyID);
				}
			};
			return;
		}
		Super.ShowNetWaiting(string.Empty);
		Global.SendEvent("3000", Global.GetLang("万魔塔挑战次数"));
		GameInstance.Game.SpriteEnterFuBen(this.SelectedItem.Data.CopyID);
	}

	private void OnSaoDangFuBen(object sender, MouseEvent e)
	{
		if (this.usedSaodangNum >= Global.GetMaxShaodangNum())
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("今日的扫荡次数已用完，无法扫荡"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), string.Format(Global.GetLang("现在扫荡最大层数{0}，扫荡可直接获取通关与掉落奖励，确定现在扫荡？"), this.getMaxSaodangIndex()), delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					Super.ShowNetWaiting(string.Empty);
					Global.SendEvent("3001", Global.GetLang("万魔塔扫荡次数"));
					GameInstance.Game.SpriteSweepWanmota(0);
				}
			}, buttons);
		}
	}

	private void OnLingquJiangli(object sender, MouseEvent e)
	{
		int num = this.BtnLingquJiangli.BtnTag.SafeToInt32(0);
		Super.ShowNetWaiting(string.Empty);
		if (num == 0)
		{
			GameInstance.Game.SpriteGetSweepReward();
		}
		else if (num > 0)
		{
			this.currentSaodangIndex = num;
			GameInstance.Game.SpriteSweepWanmota(num);
		}
	}

	private void OnSpringSaodangPanel(int stepY, bool isRevert = false)
	{
		if (isRevert)
		{
			int num = 16;
			this.SpringSaodangPanel.target.y = (float)(-(float)num);
			Transform transform = this.SaodangPanel.transform;
			transform.localPosition = new Vector3(transform.localPosition.x, (float)(-(float)num), 0f);
			Vector4 clipRange = this.SaodangPanel.clipRange;
			clipRange.y = 0f;
			this.SaodangPanel.clipRange = clipRange;
		}
		else
		{
			SpringPanel springSaodangPanel = this.SpringSaodangPanel;
			springSaodangPanel.target.y = springSaodangPanel.target.y + (float)stepY;
			this.SpringSaodangPanel.enabled = true;
		}
	}

	public void GetTiaozhanData()
	{
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.SpriteQureyWanmotaInfo();
	}

	public void NotifyTiaozhanData(int result, int currentLayer, int maxLayer, int saodangNum, int saodangIndex, int isReboot)
	{
		Super.HideNetWaiting();
		if (result < 0)
		{
			if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("获取信息失败"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("获取信息失败，错误码：{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		if (result == 0)
		{
			this.maxIndex = Global.GetPataMaxIndex();
			this.usedSaodangNum = saodangNum;
			this.MyText.Text = string.Format(Global.GetLang("{0}层"), currentLayer - 1);
			this.MaxText.Text = string.Format(Global.GetLang("{0}层"), maxLayer);
			currentLayer = Math.Min(currentLayer, this.maxIndex);
			this.loadList(currentLayer, 600);
			this.selectItem(currentLayer);
			if (saodangIndex == -1)
			{
				this.setUIVisible(PataSweepTypes.None);
			}
			else
			{
				this.refreshSaodangPersent();
				if (saodangIndex == 0)
				{
					this.setUIVisible(PataSweepTypes.Done);
					this.refreshSaodangText(PataSweepTypes.Done, false);
				}
				else if (saodangIndex > 0)
				{
					this.currentSaodangIndex = saodangIndex;
					this.setUIVisible(PataSweepTypes.Doing);
					if (isReboot == 1)
					{
						this.refreshSaodangText(PataSweepTypes.Done, true);
					}
					else if (isReboot == 0)
					{
						this.refreshSaodangText(PataSweepTypes.Doing, false);
					}
				}
			}
		}
		SystemHelpMgr.OnAction(UIObjIDs.RiChangFuBenPaTaFuBenPart, HelpStateEvents.Actived, -1);
	}

	public void NotifyEnterCopyMapCmdResult(int roleID, int retCode)
	{
		Super.HideNetWaiting();
		string text = string.Empty;
		if (null != this.SelectedItem)
		{
			text = this.SelectedItem.Data.ItemName;
			GameInstance.Game.SpriteQureyFuBenInfo(this.SelectedItem.Data.MapCode, this.SelectedItem.Data.CopyID);
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
			else if (retCode == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经挑战到了最高层"), new object[0]), 0, -1, -1, 0);
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

	public void NotifySaodangFuben(int result)
	{
		Super.HideNetWaiting();
		if (result < 0)
		{
			if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("今日的扫荡次数已用完，无法扫荡"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("最低挑战过10层才能扫荡"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("开始层号与服务器不一致"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("扫荡已完成，需要领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("扫荡时出错，错误码：{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		if (result == 0)
		{
			if (this.currentSaodangIndex == 0)
			{
				this.usedSaodangNum++;
			}
			this.OnSpringSaodangPanel(0, true);
			this.setUIVisible(PataSweepTypes.Doing);
			this.refreshSaodangText(PataSweepTypes.Doing, false);
			this.refreshSaodangPersent();
			this.startTicks = Global.GetCorrectLocalTime();
			base.InvokeRepeating("TickProc", 1f, 1f);
			this.ItemCollectionSaodang.Clear();
		}
	}

	public void NotifyUpdateSaodangState(LayerSweepData layerSweepData)
	{
		this.currentSaodangIndex = layerSweepData.nLayerOrder;
		this.refreshSaodangPersent();
		if (this.ItemCollectionSaodang.Count >= this.saodangListPageCount)
		{
			this.ItemCollectionSaodang.Clear();
			this.OnSpringSaodangPanel(0, true);
		}
		else if (this.ItemCollectionSaodang.Count >= 3)
		{
			this.OnSpringSaodangPanel(110, false);
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		RiChangPaTaSaodangItem riChangPaTaSaodangItem = U3DUtils.NEW<RiChangPaTaSaodangItem>();
		if (this.currentSaodangIndex == 0)
		{
			riChangPaTaSaodangItem.TxtTitle.Text = Global.GetLang("本次扫荡获得");
			this.refreshSaodangText(PataSweepTypes.Done, false);
			text2 = "\n";
			text3 = UIHelper.FormatGoodsListName(layerSweepData.GoodsList, true);
		}
		else
		{
			riChangPaTaSaodangItem.TxtTitle.Text = string.Format(Global.GetLang("第{0}层"), this.currentSaodangIndex);
			text2 = "  ";
			text3 = UIHelper.FormatGoodsListName(layerSweepData.GoodsList, false);
		}
		text += string.Format(Global.GetLang("星魂 +{0}{1}"), layerSweepData.nXinHun, text2);
		text += string.Format(Global.GetLang("绑金 +{0}\n"), layerSweepData.nMoney);
		riChangPaTaSaodangItem.TxtAwards.Text = text + text3;
		this.ItemCollectionSaodang.Add(riChangPaTaSaodangItem);
	}

	public void NotifyLingquJiangli(int result)
	{
		Super.HideNetWaiting();
		if (result == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == 0)
		{
			this.setUIVisible(PataSweepTypes.None);
		}
	}

	public SpriteSL Bottom0;

	public GButton BtnShaodang;

	public GButton BtnTiaozhan;

	public TextBlock NumText;

	public SpriteSL Right0;

	public ListBox GoodsList1;

	public ListBox GoodsList2;

	public TextBlock ChenshuText;

	public TextBlock ZhanliText;

	public UIPanel ItemPanel;

	public ListBox ItemList;

	public TextBlock MyText;

	public TextBlock MaxText;

	public GButton BtnTop;

	private int currentIndex;

	private int maxIndex;

	private int max = 10;

	private RiChangPaTaItem SelectedItem;

	public SpriteSL Bottom1;

	public GButton BtnLingquJiangli;

	public TextBlock TimeText;

	public TextBlock SaodangStateText;

	public GImgProgressBar ShaodangProgressBar;

	public SpriteSL Right1;

	public SpringPanel SpringSaodangPanel;

	public UIPanel SaodangPanel;

	public ListBox SaodangList;

	public TextBlock[] ConstTexts;

	private int saodangListPageCount = 10;

	private int currentSaodangIndex;

	private int usedSaodangNum;

	private int saodangTimeSec;

	private long startTicks;

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollectionAward;

	private ObservableCollection _ItemCollectionAwardFirst;

	private ObservableCollection _ItemCollectionSaodang;

	public static Dictionary<int, RiChangHuoDongData> DataDict;
}
