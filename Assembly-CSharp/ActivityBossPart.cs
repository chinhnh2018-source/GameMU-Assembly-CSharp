using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ActivityBossPart : UserControl
{
	public ActivityBossPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.txtDescription.TextFontWrapping = TextWrapping.Wrap;
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 409.0;
		this.listBox.Height = 313.0;
		Canvas.SetLeft(this.listBox, 145);
		Canvas.SetTop(this.listBox, 43);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Visible;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.Container.Children.Add(this.txtDescription);
		this.txtDescription.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtDescription, 32);
		Canvas.SetTop(this.txtDescription, 165);
		this.txtDescription.TextWidth = 98.0;
		this.Container.Children.Add(this.BossImg);
		Canvas.SetLeft(this.BossImg, 28);
		Canvas.SetTop(this.BossImg, 8);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.ShowBossList();
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		if (this.ItemCollection.Count > 0)
		{
			this.listBox.SelectedIndex = 0;
		}
		GameInstance.Game.SpriteGetBossInfoDictData();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.tc.Container.Children.Add(this.LoadingWin);
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void NotifyResult(Dictionary<int, BossData> bossInfoDict)
	{
		if (bossInfoDict != null)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				BossData bossData = null;
				ActivityBossListItem activityBossListItem = U3DUtils.AS<ActivityBossListItem>(this.ItemCollection[i]);
				if (bossInfoDict.ContainsKey(activityBossListItem.BossID))
				{
					bossInfoDict.TryGetValue(activityBossListItem.BossID, ref bossData);
					if (string.IsNullOrEmpty(bossData.NextTime))
					{
						activityBossListItem.BossState = Global.GetLang("已刷新");
					}
					else
					{
						activityBossListItem.BossState = Global.StringReplaceAll(bossData.NextTime, "$", ":");
					}
					activityBossListItem.BossLastKiller = bossData.KillMonsterName;
					if (!string.IsNullOrEmpty(bossData.KillMonsterName))
					{
						if (0 >= bossData.KillerOnline)
						{
							activityBossListItem.TxtKillerColor = new SolidColorBrush(4286611584U);
						}
						else
						{
							activityBossListItem.TxtKillerColor = new SolidColorBrush(ColorSL.FromArgb(255, 202, 154, 39));
						}
					}
				}
			}
		}
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	private void ShowBossList()
	{
		this.ItemCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/Activity/BossInfo.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Boss");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			GTextBlockEx gtextBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
			gtextBlockEx.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int num = 2;
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt);
			int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
			string mapNameByCode = ConfigSettings.GetMapNameByCode(npcorMonsterMapCodeByID, false);
			string text = monsterXmlNodeByID.SName;
			text = Global.StringReplaceAll(text, Global.GetLang("【BOSS】"), string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				gtextBlockEx.Text = text;
				string tag = StringUtil.substitute("{0},{1},{2},{3},-1,-1", new object[]
				{
					num,
					-1,
					xelementAttributeInt,
					npcorMonsterMapCodeByID
				});
				gtextBlockEx.SetSpecialText(gtextBlockEx.Text, new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0)), true, tag, true);
			}
			else
			{
				gtextBlockEx.Text = string.Empty;
			}
			gtextBlockEx.TextClick = new UIEventEventHandler(this.TextClick);
			gtextBlockEx.TextMouseEnter = new UIEventEventHandler(this.TextMouseEnter);
			gtextBlockEx.TextMouseLeave = new UIEventEventHandler(this.TextMouseLeave);
			ActivityBossListItem activityBossListItem = U3DUtils.NEW<ActivityBossListItem>();
			activityBossListItem.BodyWidth = 384.0;
			activityBossListItem.BodyHeight = 20.0;
			activityBossListItem.Width = 384.0;
			activityBossListItem.Height = 20.0;
			activityBossListItem.BossName = gtextBlockEx;
			activityBossListItem.BossLevel = Global.GetXElementAttributeStr(xelement, "Level");
			activityBossListItem.BossMap = mapNameByCode;
			activityBossListItem.BossDescription = Global.GetXElementAttributeStr(xelement, "Description");
			activityBossListItem.BossID = Global.GetXElementAttributeInt(xelement, "ID");
			this.ItemCollection.AddNoUpdate(activityBossListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.BodyBackground = null;
		}
		this.SelectedListItem = U3DUtils.AS<ActivityBossListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.BodyWidth = 409.0;
		this.SelectedListItem.BodyHeight = 20.0;
		this.txtDescription.Text = this.SelectedListItem.BossDescription;
		this.DownloadNetImage("NetImages/Boss/" + this.SelectedListItem.BossID.ToString() + ".png");
	}

	private void UnSelectItem()
	{
		this.BossImg.Source = null;
		this.SelectedListItem = null;
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (oldSelectedIndex < 0)
			{
			}
			if (num < 0)
			{
				num = 0;
			}
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	public void DownLoaderComplete1(object sender, DownloadEventArgs e)
	{
		if (e.type != DownloadEventArgs.COMPLETE)
		{
			GError.AddErrMsg(StringUtil.substitute(Global.GetLang("下载失败, 原因:{0}"), new object[]
			{
				Global.GetErrorMsg(e)
			}));
		}
		else
		{
			this.GetImageFromCaching((sender as Downloader).Args);
		}
		this.downloader.Completed = null;
		this.downloader = null;
	}

	public bool GetImageFromCaching(string key)
	{
		BitmapData netImageStream = Super.GetNetImageStream(key);
		if (netImageStream == null)
		{
			return false;
		}
		this.BossImg.Source = new ImageBrush(netImageStream);
		return true;
	}

	public void DownloadNetImage(string value)
	{
		if (this.GetImageFromCaching(value))
		{
			return;
		}
		this.BossImg.Source = null;
		if (this.downloader != null)
		{
			this.downloader.CancelRequest();
			this.downloader.Completed = null;
			this.downloader = null;
		}
		this.downloader = new Downloader(null);
		this.downloader.Args = value;
		this.downloader.Completed = new DownloaderEventHander(this.DownLoaderComplete1);
		this.downloader.GetResourceByVer(Global.WebPath(value), Global.ResSwfVer, false);
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
		int num3 = Convert.ToInt32(array[3]);
		if (num != -1)
		{
			GTipService.NotifyTip(sender as GTextBlockEx, new NotifyTipEventArgs
			{
				MouseState = true,
				TipType = TipTypes.NormalText,
				Tip = Global.GetLang("单击自动寻路"),
				MouseEvent = (e.e as MouseEvent)
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
				MouseEvent = (e.e as MouseEvent)
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
		int num2 = Convert.ToInt32(array[1]);
		int targetNpcID = Convert.ToInt32(array[2]);
		int num3 = Convert.ToInt32(array[3]);
		int x = Convert.ToInt32(array[4]);
		int y = Convert.ToInt32(array[5]);
		if (num == -1 || num3 == -1)
		{
			return true;
		}
		Global.Data.TargetNpcID = targetNpcID;
		Point pos;
		if (num == 2)
		{
			pos = Global.GetMonsterPointByID(num3, Global.Data.TargetNpcID);
		}
		else if (num == 3)
		{
			pos = Global.GetNPCPointByID(num3, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(x, y);
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
			return true;
		}
		if (num == 2)
		{
			Global.Data.GameScene.AutoFindRoad(num3, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
		}
		else if (num == 3)
		{
			Global.Data.GameScene.AutoFindRoad(num3, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(num3, pos, 0, ExtActionTypes.EXTACTION_NONE);
		}
		return true;
	}

	private LoadingWindow LoadingWin;

	private bool FirstGetNewData = true;

	private ActivityBossListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 390.0, 20.0, 5.0, 5.0));

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine txtDescription = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Image BossImg = new Image();

	private Downloader downloader;

	public string CurrentItemTag;

	public ObservableCollection ItemCollection;

	private GTabControl _tc;

	private bool FirstInitPartData = true;
}
