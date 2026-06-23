using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class ActivityCopyPart : UserControl
{
	public ActivityCopyPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.des1.TextFontWrapping = TextWrapping.Wrap;
		this.des2.TextFontWrapping = TextWrapping.Wrap;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 542.0;
		this.listBox.Height = 185.0;
		Canvas.SetLeft(this.listBox, 13);
		Canvas.SetTop(this.listBox, 43);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Visible;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.Container.Children.Add(this.ScrollViewer4);
		this.ScrollViewer4.Width = 527.0;
		this.ScrollViewer4.Height = 94.0;
		Canvas.SetLeft(this.ScrollViewer4, 26);
		Canvas.SetTop(this.ScrollViewer4, 243);
		this.ScrollViewer4.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer4.Viewer = this.Wrapper;
		this.Wrapper.Width = 514.0;
		this.Wrapper.Children.Add(this.des1);
		this.des1.TextColor = new SolidColorBrush(4294954496U);
		Canvas.SetLeft(this.des1, 10);
		this.des1.BodyWidth = 460.0;
		this.Wrapper.Children.Add(this.des2);
		this.des2.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.des2, 10);
		this.des2.BodyWidth = 460.0;
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
		this.InitActivityList();
	}

	public void GetNewData()
	{
		int level = Global.Data.roleData.Level;
		if (level == this.LastOldLevel)
		{
			return;
		}
		this.LastOldLevel = level;
		this.ShowActivityList();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitActivityList()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Activity/Copy.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			GTextBlockEx gtextBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
			gtextBlockEx.TextWidth = 100.0;
			gtextBlockEx.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "PublishID");
			if (xelementAttributeInt > 0)
			{
				int num = 3;
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
				int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
				string sname = npcvobyID.SName;
				if (!string.IsNullOrEmpty(sname))
				{
					gtextBlockEx.Text = sname;
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
					gtextBlockEx.Text = Global.GetLang("无");
				}
			}
			else
			{
				gtextBlockEx.Text = Global.GetLang("无");
			}
			gtextBlockEx.TextClick = new UIEventEventHandler(this.TextClick);
			gtextBlockEx.TextMouseEnter = new UIEventEventHandler(this.TextMouseEnter);
			gtextBlockEx.TextMouseLeave = new UIEventEventHandler(this.TextMouseLeave);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Level");
			ActivityCopyListItem activityCopyListItem = U3DUtils.NEW<ActivityCopyListItem>();
			activityCopyListItem.BodyWidth = 526.0;
			activityCopyListItem.BodyHeight = 20.0;
			activityCopyListItem.Width = 526.0;
			activityCopyListItem.Height = 20.0;
			activityCopyListItem.ActivityTime = Global.GetXElementAttributeStr(xelement, "Time");
			activityCopyListItem.ActivityName = Global.GetXElementAttributeStr(xelement, "Name");
			activityCopyListItem.ActivityReward = Global.GetXElementAttributeStr(xelement, "Reward");
			activityCopyListItem.ActivityJoinLevel = ((xelementAttributeInt2 != -1) ? xelementAttributeInt2.ToString() : Global.GetLang("不限"));
			activityCopyListItem.ObjectPublishName = gtextBlockEx;
			activityCopyListItem.ActivityJoinNum = Global.GetXElementAttributeStr(xelement, "JoinNum");
			activityCopyListItem.Des01 = Global.GetXElementAttributeStr(xelement, "Description1");
			activityCopyListItem.Des02 = Global.GetXElementAttributeStr(xelement, "Description2");
			activityCopyListItem.ToLevel = xelementAttributeInt2;
			activityCopyListItem.MaxLevel = Global.GetXElementAttributeInt(xelement, "MaxLevel");
			this.ItemsList.Add(activityCopyListItem);
		}
	}

	private void ShowActivityList()
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			ActivityCopyListItem activityCopyListItem = this.ItemsList[i];
			if (activityCopyListItem.MaxLevel <= 0 || (Global.Data.roleData.Level >= activityCopyListItem.ToLevel && Global.Data.roleData.Level <= activityCopyListItem.MaxLevel))
			{
				if (activityCopyListItem.ToLevel > Global.Data.roleData.Level)
				{
					activityCopyListItem.TxtLevelColor = new SolidColorBrush(ColorSL.FromArgb(255, 162, 39, 46));
					activityCopyListItem.TxtJoinNumColor = new SolidColorBrush(ColorSL.FromArgb(255, 162, 39, 46));
				}
				else
				{
					activityCopyListItem.TxtLevelColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
					activityCopyListItem.TxtJoinNumColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
				}
				this.ItemCollection.AddNoUpdate(activityCopyListItem);
			}
		}
		this.ItemCollection.DelayUpdate();
		if (this.ItemCollection.Count > 0)
		{
			this.listBox.SelectedIndex = 0;
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
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
		this.SelectedListItem = U3DUtils.AS<ActivityCopyListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyWidth = 514.0;
		this.SelectedListItem.BodyHeight = 20.0;
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.des1.Text = this.SelectedListItem.Des01;
		this.des2.Text = this.SelectedListItem.Des02;
		this.Wrapper.Height = this.des1.RealSize.Height + this.des2.RealSize.Height + 20.0;
		Canvas.SetTop(this.des2, Canvas.GetTop(this.des1) + this.des1.RealSize.Height + 10.0);
	}

	private void UnSelectItem()
	{
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
		int num2 = Convert.ToInt32(array[3]);
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
			Global.Data.GameScene.AutoFindRoad(num3, pos, 60, ExtActionTypes.EXTACTION_NPCDLG);
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(num3, pos, 0, ExtActionTypes.EXTACTION_NONE);
		}
		return true;
	}

	private List<ActivityCopyListItem> ItemsList = new List<ActivityCopyListItem>();

	private int LastOldLevel = -1;

	private ActivityCopyListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 526.0, 20.0, 5.0, 5.0));

	private ListBox listBox = new ListBox();

	private GScrollView ScrollViewer4 = new GScrollView(514, 222, 0);

	private Canvas Wrapper = new Canvas();

	private GTextBlockOutLine des1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine des2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public string CurrentItemTag;

	public ObservableCollection ItemCollection;

	private bool FirstInitPartData = true;
}
