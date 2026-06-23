using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class DailyTasksPart : UserControl
{
	public DailyTasksPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
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

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.InitActivityList();
		this.listBox.SelectedIndex = 0;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 398.0;
		this.listBox.Height = 243.0;
		Canvas.SetLeft(this.listBox, 10);
		Canvas.SetTop(this.listBox, 40);
		this.listBox.ItemMargin = new Thickness(0.0, 3.0, 0.0, 1.0);
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.descriptionText);
		this.descriptionText.mouseEnabled = false;
		this.descriptionText.TextColor = new SolidColorBrush(11394222U);
		this.descriptionText.Width = 130.0;
		this.descriptionText.BodyWidth = 130.0;
		this.descriptionText.TextFontWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.descriptionText, 424);
		Canvas.SetTop(this.descriptionText, 36);
		this.Container.Children.Add(this.rewardText);
		this.rewardText.mouseEnabled = false;
		this.rewardText.TextColor = new SolidColorBrush(11394222U);
		this.rewardText.Width = 130.0;
		this.rewardText.BodyWidth = 130.0;
		this.rewardText.TextFontWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.rewardText, 424);
		Canvas.SetTop(this.rewardText, 200);
		this.PrevPageIcon = U3DUtils.NEW<GIcon>();
		this.PrevPageIcon.Width = 16.0;
		this.PrevPageIcon.Height = 21.0;
		this.PrevPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_normal.png"));
		this.PrevPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_hover.png"));
		this.PrevPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_nouse.png"));
		this.PrevPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.PrevPageIcon.EnableIcon)
			{
				this.PrevPage();
				return;
			}
		};
		Canvas.SetLeft(this.PrevPageIcon, 170);
		Canvas.SetTop(this.PrevPageIcon, 296);
		this.Container.Children.Add(this.PrevPageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 16.0;
		this.NextPageIcon.Height = 21.0;
		this.NextPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_normal.png"));
		this.NextPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_hover.png"));
		this.NextPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_nouse.png"));
		this.NextPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.NextPageIcon.EnableIcon)
			{
				this.NextPage();
				return;
			}
		};
		Canvas.SetLeft(this.NextPageIcon, 220);
		Canvas.SetTop(this.NextPageIcon, 296);
		this.Container.Children.Add(this.NextPageIcon);
		this.FirstPage = U3DUtils.NEW<GIcon>();
		this.FirstPage.Width = 66.0;
		this.FirstPage.Height = 25.0;
		this.FirstPage.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPage.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPage.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPage.Text = Global.GetLang("首页");
		this.FirstPage.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.FirstPage.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.FirstPage.EnableIcon)
			{
				this.ShowPage(0);
				return;
			}
		};
		Canvas.SetLeft(this.FirstPage, 98);
		Canvas.SetTop(this.FirstPage, 296);
		this.Container.Children.Add(this.FirstPage);
		this.EndPage = U3DUtils.NEW<GIcon>();
		this.EndPage.Width = 66.0;
		this.EndPage.Height = 21.0;
		this.EndPage.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPage.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPage.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPage.Text = Global.GetLang("尾页");
		this.EndPage.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.EndPage.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.EndPage.EnableIcon)
			{
				this.ShowPage(this.MaxPageCount - 1);
				return;
			}
		};
		Canvas.SetLeft(this.EndPage, 244);
		Canvas.SetTop(this.EndPage, 296);
		this.Container.Children.Add(this.EndPage);
		this.Container.Children.Add(this.PageHint);
		this.PageHint.selectable = false;
		this.PageHint.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.PageHint, 194);
		Canvas.SetTop(this.PageHint, 296);
		this.PageHint.Text = "1/1";
	}

	public void RefreshData()
	{
		int num = 0;
		int num2 = 0;
		if (this.ItemsList == null)
		{
			return;
		}
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			if (Global.CanTaskPaoHuanTask(this.ItemsList[i].ItemTag, out num, out num2, true))
			{
				if (num2 != 0)
				{
					this.ItemsList[i].Jindu = StringUtil.substitute(Global.GetLang("完成进度：{0}/{1}"), new object[]
					{
						Math.Max(0, num - 1),
						num2
					});
				}
			}
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (this.listBox.SelectedItem == null)
		{
			return;
		}
		this.SelectedListItem = this.listBox.SelectedItem.SafeGetComponent<DailyTasksItem>();
		if (this.ListItem == this.SelectedListItem)
		{
			return;
		}
		if (this.ListItem != null)
		{
			this.ListItem.BodyBackground = null;
		}
		this.ListItem = this.SelectedListItem;
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.descriptionText.Text = Global.GetXElementAttributeStr(Global.GetXElement(this.xml, "Copy", "ID", this.SelectedListItem.ItemTag.ToString()), "Description1");
		this.rewardText.Text = Global.GetXElementAttributeStr(Global.GetXElement(this.xml, "Copy", "ID", this.SelectedListItem.ItemTag.ToString()), "Description2");
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

	private void InitActivityList()
	{
		if (this.xml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(this.xml, "Copy");
		if (xelementList == null)
		{
			return;
		}
		List<DailyTasksItem> list = new List<DailyTasksItem>();
		bool flag = this.TabItemsDict.ContainsKey(0);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (Global.GetXElementAttributeInt(xelement, "Type") == 1)
			{
				GTextBlockEx gtextBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
				gtextBlockEx.TextWidth = 100.0;
				gtextBlockEx.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "PublishID");
				string tag = string.Empty;
				if (xelementAttributeInt > 0)
				{
					int num = 3;
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
					int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
					string lang = Global.GetLang("立即前住");
					if (!string.IsNullOrEmpty(lang))
					{
						gtextBlockEx.Text = lang;
						tag = StringUtil.substitute("{0},{1},{2},{3},-1,-1", new object[]
						{
							num,
							-1,
							xelementAttributeInt,
							npcorMonsterMapCodeByID
						});
						gtextBlockEx.SetSpecialText(gtextBlockEx.Text, new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0)), true, tag, true);
					}
				}
				gtextBlockEx.TextClick = new UIEventEventHandler(this.TextClick);
				gtextBlockEx.TextMouseEnter = new UIEventEventHandler(this.TextMouseEnter);
				gtextBlockEx.TextMouseLeave = new UIEventEventHandler(this.TextMouseLeave);
				int num2 = 0;
				GIcon gicon = U3DUtils.NEW<GIcon>();
				gicon.Width = 15.0;
				gicon.Height = 20.0;
				gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/taskchuansong.png"));
				gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/taskchuansong.png"));
				gicon.TipType = 0;
				gicon.Tip = Global.GetLang("每日免费30次\nVIP可无限免费传送");
				gicon.Tag = tag;
				gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					string[] array = ((s as GIcon).Tag as string).Split(new char[]
					{
						','
					});
					if (array.Length == 6)
					{
						this.TansportPoint(Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]), Global.SafeConvertToInt32(array[2]), Global.SafeConvertToInt32(array[3]), -1, -1, true);
					}
				};
				DailyTasksItem dailyTasksItem = U3DUtils.NEW<DailyTasksItem>();
				dailyTasksItem.BodyWidth = 378.0;
				dailyTasksItem.BodyHeight = 20.0;
				dailyTasksItem.Width = 378.0;
				dailyTasksItem.Height = 20.0;
				dailyTasksItem.ActivityName = Global.GetXElementAttributeStr(xelement, "Name");
				dailyTasksItem.Jindu = StringUtil.substitute("完成进度：{0}/{1}", new object[]
				{
					num2,
					Global.GetXElementAttributeStr(xelement, "Information")
				});
				dailyTasksItem.ObjectPublishName = gtextBlockEx;
				dailyTasksItem.ItemTag = Global.GetXElementAttributeInt(xelement, "ID");
				dailyTasksItem.ActivityJoinLevel = Global.GetXElementAttributeStr(xelement, "Level");
				dailyTasksItem.BtnChuansong = gicon;
				list.Add(dailyTasksItem);
			}
		}
		this.TabItemsDict[0] = list;
		list = this.TabItemsDict.GetValue(0);
		this.ItemsList = list;
		if (this.ItemsList != null)
		{
			int currentSelectedPage;
			this.GetHintGoodsIDInfo(this.ItemsList, out currentSelectedPage);
			this.MaxPageCount = (this.ItemsList.Count - 1) / 10 + 1;
			this.CurrentSelectedPage = currentSelectedPage;
			this.ShowPage(this.CurrentSelectedPage);
		}
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

	private void GetHintGoodsIDInfo(List<DailyTasksItem> itemsList, out int pageIndex)
	{
		pageIndex = 0;
		if (itemsList == null)
		{
			return;
		}
		int num = 0;
		if (num < itemsList.Count)
		{
			pageIndex = num / 10;
		}
	}

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	public void ShowPage(int pageIndex)
	{
		this.PageHint.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			pageIndex + 1,
			this.MaxPageCount
		});
		this.ItemCollection.Clear();
		int num = pageIndex * 10;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + 10)
		{
			this.ItemCollection.AddNoUpdate(this.ItemsList[num2]);
			num2++;
		}
		this.ItemCollection.DelayUpdate();
		if (pageIndex <= 0)
		{
			this.PrevPageIcon.EnableIcon = false;
			this.FirstPage.EnableIcon = false;
			this.CurrentSelectedPage = 0;
		}
		else
		{
			this.PrevPageIcon.EnableIcon = true;
			this.FirstPage.EnableIcon = true;
		}
		if (pageIndex >= this.MaxPageCount - 1)
		{
			this.NextPageIcon.EnableIcon = false;
			this.EndPage.EnableIcon = false;
			this.CurrentSelectedPage = this.MaxPageCount - 1;
		}
		else
		{
			this.NextPageIcon.EnableIcon = true;
			this.EndPage.EnableIcon = true;
		}
	}

	private ListBox listBox = new ListBox();

	public ObservableCollection ItemCollection;

	public string CurrentItemTag;

	private DailyTasksItem SelectedListItem;

	private DailyTasksItem ListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 393.0, 20.0, 5.0, 5.0));

	private GTextBlockOutLine descriptionText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine rewardText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Dictionary<int, List<DailyTasksItem>> TabItemsDict = new Dictionary<int, List<DailyTasksItem>>();

	private TextBlock PageHint = new TextBlock();

	private GIcon NextPageIcon;

	private GIcon PrevPageIcon;

	private GIcon FirstPage;

	private GIcon EndPage;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private List<DailyTasksItem> ItemsList;

	private XElement xml = Global.GetGameResXml("Config/Activity/Copy.Xml");

	private GTabControl _tc;

	private bool FirstInitPartData = true;
}
