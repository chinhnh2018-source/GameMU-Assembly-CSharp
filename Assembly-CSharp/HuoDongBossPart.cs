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

public class HuoDongBossPart : UserControl
{
	public void OnEnable()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (Global.Data.BossInfoLastRefreshTime.AddSeconds(10.0) < Global.GetCorrectDateTime())
			{
				Global.Data.BossInfoLastRefreshTime = Global.Data.BossInfoLastRefreshTime.AddSeconds(5.0);
				GameInstance.Game.SpriteGetBossInfoDictData();
			}
			if (this.ItemsList != null && this.ItemsList.Count > 0)
			{
				this.ShowPage(0);
				this.listBox_SelectionChanged(this._BossList, new EventArgs());
			}
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData(int type)
	{
		this.BossType = type;
		this.InitActivityList(this.BossType);
		this._BossList.SelectedIndex = 0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this._BossList.ItemsSource;
		this.ItemCollectionDropItems = this._GoodsList.ItemsSource;
		this._bak.URL = Global.GetGameResImageString("richangHuodongBoss_bak1.jpg");
		this._NextPage.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.NextPage);
		this._PrevPage.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.PrevPage);
		this._DropItems.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ShowDropItems);
		this._GotoMap.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.GotoMap);
		this._PageList.ItemPerPage = this.ItemCountPerPage;
		this._PageList.ItemCount = 0;
		this._DropItems.Text = Global.GetLang("物品掉落");
		this._GotoMap.Text = Global.GetLang("立即前往");
		UIHelper.SetModalPosZ(this._BossModal.transform);
		this._MapName.X = 110.0;
		this._ZhanLi.X = 355.0;
		this._Killer.X = 355.0;
	}

	public void ShowDropItems(object sender, MouseEvent e)
	{
		string goodsItemList = string.Empty;
		if (null != this.SelectedListItem)
		{
			goodsItemList = this.SelectedListItem.FallItemsList;
		}
		ActivityGoodsListPart activityGoodsListPart = U3DUtils.NEW<ActivityGoodsListPart>();
		if (null != activityGoodsListPart)
		{
			this._BossModal.Show = false;
			GChildWindow childWindow = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(childWindow, "FallItemsWindow");
			Super.GData.PlayZoneRoot.Children.Add(childWindow);
			childWindow.SetContent(childWindow.BodyPresenter, activityGoodsListPart, 0.0, 0.0, true);
			childWindow.ModalType = ChildWindowModalType.TransBak;
			activityGoodsListPart.InitPartData(goodsItemList);
			activityGoodsListPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e1)
			{
				this._BossModal.Show = true;
				Super.CloseChildWindow(this.Container, childWindow);
			};
		}
	}

	public void GotoMap(object sender, MouseEvent e)
	{
		if (Global.Data.CurrentCopyTeamData != null)
		{
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
					int npcid2 = this.SelectedListItem.NPCID;
					int targetType2 = this.SelectedListItem.TargetType;
					int targetID2 = this.SelectedListItem.TargetID;
					int mapCode2 = this.SelectedListItem.MapCode;
					string systemParamByName2 = ConfigSystemParam.GetSystemParamByName("VIPBossChuanSong", true);
					int num3 = 0;
					int num4 = 0;
					Global.GetMapMinLevelAndZhuanSheng(mapCode2, out num3, out num4);
					if (Global.Data.roleData.ChangeLifeCount * 400 + Global.Data.roleData.Level < num4 * 400 + num3)
					{
						Super.HintMainText(Global.GetLang("等级不足，无法传送！"), 10, 3);
						return;
					}
					if (Global.IsGoToKuaFuMap(mapCode2))
					{
						PlayZone.GlobalPlayZone.OpenKuafuMapView(targetType2, -1, targetID2, mapCode2, -1, -1, false, targetID2, 0, false, false);
						return;
					}
					bool forceTansport2 = Global.GetVIPLeve() >= Convert.ToInt32(systemParamByName2);
					this.TansportPoint(targetType2, -1, targetID2, mapCode2, -1, -1, forceTansport2);
					Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventWindowOperation", WindowOperationTypes.CloseActivityWindow));
				}
			}, -1);
		}
		else
		{
			int npcid = this.SelectedListItem.NPCID;
			int targetType = this.SelectedListItem.TargetType;
			int targetID = this.SelectedListItem.TargetID;
			int mapCode = this.SelectedListItem.MapCode;
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("VIPBossChuanSong", true);
			int num = 0;
			int num2 = 0;
			Global.GetMapMinLevelAndZhuanSheng(mapCode, out num, out num2);
			if (Global.Data.roleData.ChangeLifeCount * 400 + Global.Data.roleData.Level < num2 * 400 + num)
			{
				Super.HintMainText(Global.GetLang("等级不足，无法传送！"), 10, 3);
				return;
			}
			if (Global.IsGoToKuaFuMap(mapCode))
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(targetType, -1, targetID, mapCode, -1, -1, false, targetID, 0, false, false);
				return;
			}
			bool forceTansport = Global.GetVIPLeve() >= Convert.ToInt32(systemParamByName);
			this.TansportPoint(targetType, -1, targetID, mapCode, -1, -1, forceTansport);
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventWindowOperation", WindowOperationTypes.CloseActivityWindow));
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (this._BossList.SelectedItem == null)
		{
			return;
		}
		this.SelectedListItem = this._BossList.SelectedItem.SafeGetComponent<ActivityBossItem>();
		if (null != this.SelectedListItem)
		{
			this.RefreshBossInfo(this.SelectedListItem);
			this.RefreshDropItemsList(this.SelectedListItem.FallItemsList);
		}
	}

	private void InitActivityList(int type)
	{
		if (type == 0)
		{
			this.xml = Global.GetGameResXml("Config/Activity/BossInfo.Xml");
		}
		else
		{
			this.xml = Global.GetGameResXml("Config/HuangJin.xml");
		}
		if (this.xml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(this.xml, "Boss");
		if (xelementList == null)
		{
			return;
		}
		List<ActivityBossItem> list = new List<ActivityBossItem>();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (type != 0 || type == Global.GetXElementAttributeInt(xelement, "Type"))
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "NpcID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ZhanLi");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsList");
				int bossUnionLevel = Global.GetBossUnionLevel(xelementAttributeInt, 0);
				float scale = (float)Global.GetXElementAttributeDouble(xelement, "Scale");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Show");
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt);
				int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
				string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
				string text = monsterXmlNodeByID.SName;
				XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(npcorMonsterMapCodeByID, StringUtil.substitute("Monsters.Xml", new object[0]));
				XElement xelement2 = Global.GetXElement(gameMapSettingsXml, "Monster", "Code", xelementAttributeInt.ToString());
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "BirthType");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "TimePoints");
				text = Global.StringReplaceAll(text, Global.GetLang("【BOSS】"), string.Empty);
				if (!string.IsNullOrEmpty(text))
				{
				}
				int targetType;
				int targetID;
				if (xelementAttributeInt2 == -1)
				{
					targetType = 2;
					targetID = xelementAttributeInt;
				}
				else
				{
					targetType = 3;
					targetID = xelementAttributeInt2;
				}
				ActivityBossItem item = U3DUtils.NEW<ActivityBossItem>();
				list.Add(item);
				this.ItemCollection.AddNoUpdate(item);
				item.MonsterID = xelementAttributeInt;
				item.NPCID = xelementAttributeInt2;
				item.TargetType = targetType;
				item.TargetID = targetID;
				item.MapCode = npcorMonsterMapCodeByID;
				item.BirthType = xelementAttributeInt4;
				item.TimeList = xelementAttributeStr3;
				item.MapName = mapNameByCode;
				item.ZhanLi = xelementAttributeStr;
				item.FallItemsList = xelementAttributeStr2;
				item.ItemTag = xelementAttributeInt;
				item.Level = bossUnionLevel;
				item.Scale = scale;
				item.BossName = text;
				item.Show = xelementAttributeInt3;
				item.Init(delegate(object sender, MouseEvent e)
				{
					this.RefreshBossInfo(item);
				});
			}
		}
		if (list.Count == 0)
		{
			this._GotoMap.isEnabled = false;
			this._DropItems.isEnabled = false;
			return;
		}
		this.ItemsList = list;
		this.ItemCollection.DelayUpdate();
		this._PageList.ItemCount = list.Count;
		this.MaxPageCount = this._PageList.PageCount;
		this.CurrentSelectedPage = 0;
		this.ShowPage(this.CurrentSelectedPage);
		this.NotifyResult(Global.Data.BossInfoDict);
	}

	public void RefreshBossInfo(ActivityBossItem shiJieBossItem)
	{
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.SetIconHighlighted(false);
		}
		if (null != shiJieBossItem)
		{
			shiJieBossItem.SetIconHighlighted(true);
		}
		this.SelectedListItem = shiJieBossItem;
		this._Name.text = shiJieBossItem.BossName;
		this._MapName.text = shiJieBossItem.MapName;
		this._ZhanLi.text = shiJieBossItem.ZhanLi;
		this._ZhuangTai.text = shiJieBossItem.BossState;
		this._Killer.text = shiJieBossItem.BossLastKiller;
		this._Time.text = UIHelper.FormatBirthTimes(shiJieBossItem.BirthType, shiJieBossItem.TimeList, 2);
		this._Time.MaxWidth = (double)((shiJieBossItem.BirthType != 7) ? 350 : 0);
		if (!this._BossModal.IsTarget(shiJieBossItem.MonsterID))
		{
			this._BossModal.Clear();
			this._BossModal.MonsterID = shiJieBossItem.MonsterID;
			if (this.monsterNPCResLoader != null)
			{
				this.monsterNPCResLoader.Stop();
			}
			this.monsterNPCResLoader = UIHelper.LoadMonsterRes(this._BossModal, this.SelectedListItem.MonsterID, this.SelectedListItem.Scale);
		}
		this.RefreshDropItemsList(shiJieBossItem.FallItemsList);
	}

	private void RefreshDropItemsList(string goodsStr)
	{
		this.ItemCollectionDropItems.Clear();
		if (string.IsNullOrEmpty(goodsStr))
		{
			return;
		}
		string[] array = goodsStr.Split(new char[]
		{
			'|'
		});
		if (array == null)
		{
			return;
		}
		foreach (string strNum in array)
		{
			int goodsID = strNum.SafeToInt32(0);
			GoodsData fakeEquipGoodsData = Global.GetFakeEquipGoodsData(goodsID, 0, 0);
			UIHelper.AddGoodsIcon3(this.ItemCollectionDropItems, fakeEquipGoodsData, true, true, false, "bagGrid4_bak");
		}
		this.ItemCollectionDropItems.DelayUpdate();
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

	private void GetHintGoodsIDInfo(List<ActivityBossItem> itemsList, out int pageIndex)
	{
		pageIndex = 0;
		if (itemsList == null)
		{
			return;
		}
		int num = 0;
		if (num < itemsList.Count)
		{
			pageIndex = num / this.ItemCountPerPage;
		}
	}

	private void NextPage(object sender, MouseEvent e)
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage(object sender, MouseEvent e)
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	public void ShowPage(int pageIndex)
	{
		this._PageList.GotoPage(pageIndex);
		this.RefreshBossInfo(this.ItemsList[this._PageList.Page * this.ItemCountPerPage]);
		this.CurrentSelectedPage = this._PageList.Page;
	}

	public bool TextMouseEnter(object sender, BaseEventArgs e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(e.Tag is SpecialTextItem))
		{
			return false;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		this.CurrentItemTag = ((e.Tag as SpecialTextItem).Tag as string);
		if (this.CurrentItemTag == null || string.Empty == this.CurrentItemTag)
		{
			return true;
		}
		string[] array = this.CurrentItemTag.Split(new char[]
		{
			','
		});
		if (array.Length != 6)
		{
			return true;
		}
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
		(sender as GTextBlockExItem).Link(new SolidColorBrush(4289014314U));
		int num = Convert.ToInt32(array[0]);
		int num2 = Convert.ToInt32(array[1]);
		int num3 = Convert.ToInt32(array[2]);
		int num4 = Convert.ToInt32(array[3]);
		if (num != -1)
		{
			GTipService.NotifyTip(sender as GTextBlockEx, new NotifyTipEventArgs
			{
				MouseState = true,
				TipType = TipTypes.NormalText,
				Tip = Global.GetLang("单击自动寻路"),
				MouseEvent = (MouseEvent)e.e
			});
		}
		return true;
	}

	public bool TextMouseLeave(object sender, BaseEventArgs e)
	{
		if (e.Tag is SpecialTextItem)
		{
			if (Global.Data.GameCursorImageID < 100)
			{
				base.Cursor = Cursors.Auto;
			}
			(sender as GTextBlockExItem).Unlink();
			GTipService.NotifyTip(this, new NotifyTipEventArgs
			{
				MouseState = false,
				TipType = TipTypes.NormalText,
				Tip = Global.GetLang("单击自动寻路"),
				MouseEvent = (MouseEvent)e.e
			});
			this.CurrentItemTag = null;
			return true;
		}
		return false;
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(e.Tag is SpecialTextItem))
		{
			return true;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		this.CurrentItemTag = ((e.Tag as SpecialTextItem).Tag as string);
		if (this.CurrentItemTag == null || string.Empty == this.CurrentItemTag)
		{
			return true;
		}
		string[] array = this.CurrentItemTag.Split(new char[]
		{
			','
		});
		if (array.Length != 6)
		{
			return true;
		}
		int num = Convert.ToInt32(array[0]);
		int taskID = Convert.ToInt32(array[1]);
		int npcID = Convert.ToInt32(array[2]);
		int num2 = Convert.ToInt32(array[3]);
		int toPosX = Convert.ToInt32(array[4]);
		int toPosY = Convert.ToInt32(array[5]);
		return num == -1 || num2 == -1 || this.TansportPoint(num, taskID, npcID, num2, toPosX, toPosY, false);
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

	public void NotifyResult(Dictionary<int, BossData> bossInfoDict)
	{
		if (bossInfoDict != null)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				BossData bossData = null;
				ActivityBossItem activityBossItem = U3DUtils.AS<ActivityBossItem>(this.ItemCollection[i]);
				if (bossInfoDict.ContainsKey(activityBossItem.MonsterID))
				{
					bossInfoDict.TryGetValue(activityBossItem.MonsterID, ref bossData);
					bool flag = string.IsNullOrEmpty(bossData.NextTime);
					if (activityBossItem.Show == 1)
					{
						activityBossItem.IsExisted = true;
						activityBossItem.BossState = Global.GetLang("未知");
					}
					else if (flag)
					{
						activityBossItem.IsExisted = true;
						activityBossItem.BossState = Global.GetLang("已刷新");
					}
					else
					{
						activityBossItem.IsExisted = false;
						activityBossItem.BossState = Global.GetLang("未刷新");
					}
					activityBossItem.BossLastKiller = bossData.KillMonsterName;
				}
			}
			this.RefreshBossInfo(this.SelectedListItem);
		}
	}

	protected virtual void OnDestory()
	{
		this.ItemsList.Clear();
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone.ShiJieBossPart = null;
		}
		if (this.monsterNPCResLoader != null)
		{
			this.monsterNPCResLoader.Stop();
		}
		base.OnDestroy();
	}

	public GScrollBarPageList _PageList;

	public ListBox _BossList;

	public ListBox _GoodsList;

	public ShowNetImage _BossImg;

	public ShowNetImage _bak;

	public GButton _NextPage;

	public GButton _PrevPage;

	public TextBlock _Name;

	public TextBlock _MapName;

	public TextBlock _ZhanLi;

	public TextBlock _ZhuangTai;

	public TextBlock _Killer;

	public TextBlock _Time;

	public GButton _DropItems;

	public GButton _GotoMap;

	public Modal3DShow _BossModal;

	public int BossType;

	public ObservableCollection ItemCollectionDropItems;

	public ObservableCollection ItemCollection;

	public string CurrentItemTag;

	private int ItemCountPerPage = 8;

	private ActivityBossItem SelectedListItem;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private List<ActivityBossItem> ItemsList;

	private XElement xml;

	private MonsterNPCResLoader monsterNPCResLoader;
}
