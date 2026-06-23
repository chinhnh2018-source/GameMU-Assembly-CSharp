using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class JgsmPart : UserControl
{
	public JgsmPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.ItemCollection_01 = this.listBox_01.ItemsSource;
		this.ItemCollection_02 = this.listBox_02.ItemsSource;
		this.ItemCollection_03 = this.listBox_03.ItemsSource;
		this.ItemCollection_04 = this.listBox_04.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 79.0;
		this.listBox.Height = 339.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 6.0);
		Canvas.SetLeft(this.listBox, 261);
		Canvas.SetTop(this.listBox, 40);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.listBox_01);
		Canvas.SetLeft(this.listBox_01, 343);
		Canvas.SetTop(this.listBox_01, 45);
		this.listBox_01.Width = 281.0;
		this.listBox_01.Height = 80.0;
		this.listBox_01.ItemMargin = new Thickness(0.0, 0.0, 7.0, 7.0);
		this.listBox_01.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.listBox_02);
		Canvas.SetLeft(this.listBox_02, 343);
		Canvas.SetTop(this.listBox_02, 132);
		this.listBox_02.Width = 281.0;
		this.listBox_02.Height = 80.0;
		this.listBox_02.ItemMargin = new Thickness(0.0, 0.0, 7.0, 7.0);
		this.listBox_02.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.listBox_03);
		Canvas.SetLeft(this.listBox_03, 343);
		Canvas.SetTop(this.listBox_03, 219);
		this.listBox_03.Width = 281.0;
		this.listBox_03.Height = 80.0;
		this.listBox_03.ItemMargin = new Thickness(0.0, 0.0, 7.0, 7.0);
		this.listBox_03.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.listBox_04);
		Canvas.SetLeft(this.listBox_04, 343);
		Canvas.SetTop(this.listBox_04, 306);
		this.listBox_04.Width = 281.0;
		this.listBox_04.Height = 80.0;
		this.listBox_04.ItemMargin = new Thickness(0.0, 0.0, 7.0, 7.0);
		this.listBox_04.Background = new SolidColorBrush(16777215U);
	}

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

	public ObservableCollection ItemCollection_01
	{
		get
		{
			return this._ItemCollection_01;
		}
		set
		{
			this._ItemCollection_01 = value;
		}
	}

	public ObservableCollection ItemCollection_02
	{
		get
		{
			return this._ItemCollection_02;
		}
		set
		{
			this._ItemCollection_02 = value;
		}
	}

	public ObservableCollection ItemCollection_03
	{
		get
		{
			return this._ItemCollection_03;
		}
		set
		{
			this._ItemCollection_03 = value;
		}
	}

	public ObservableCollection ItemCollection_04
	{
		get
		{
			return this._ItemCollection_04;
		}
		set
		{
			this._ItemCollection_04 = value;
		}
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
		this.arrayItemCollection[0] = this.ItemCollection_01;
		this.arrayItemCollection[1] = this.ItemCollection_02;
		this.arrayItemCollection[2] = this.ItemCollection_03;
		this.arrayItemCollection[3] = this.ItemCollection_04;
		this.InitList();
	}

	private void InitList()
	{
		XElement gameResXml = Global.GetGameResXml("Config/JunGong.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			GTextBlockEx gtextBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
			gtextBlockEx.TextWidth = 50.0;
			gtextBlockEx.FontSize = 12;
			gtextBlockEx.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NpcID");
			if (xelementAttributeInt > 0)
			{
				int num = 3;
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
				if (npcvobyID != null)
				{
					int npcorMonsterMapCodeByID = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
					gtextBlockEx.Text = Global.GetLang("立即前往");
					string tag = StringUtil.substitute("{0},{1},{2},{3},-1,-1", new object[]
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
			JgsmTxtListItem jgsmTxtListItem = U3DUtils.NEW<JgsmTxtListItem>();
			jgsmTxtListItem.BodyWidth = 79.0;
			jgsmTxtListItem.BodyHeight = 81.0;
			jgsmTxtListItem.Width = 79.0;
			jgsmTxtListItem.Height = 81.0;
			jgsmTxtListItem.TxtGo = gtextBlockEx;
			this.ItemCollection.AddNoUpdate(jgsmTxtListItem);
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Goods");
			string[] array = xelementAttributeStr.Split(new char[]
			{
				','
			});
			for (int j = 0; j < array.Length; j++)
			{
				int num2 = Global.SafeConvertToInt32(array[j]);
				if (num2 > 0)
				{
					this.AddGoodsIcon(this.arrayItemCollection[i], num2);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(ObservableCollection list, int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = -1;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			list.Add(gicon);
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

	public ObservableCollection[] arrayItemCollection = new ObservableCollection[4];

	public string CurrentItemTag;

	private Canvas Root;

	private ListBox listBox = new ListBox();

	private ListBox listBox_01 = new ListBox();

	private ListBox listBox_02 = new ListBox();

	private ListBox listBox_03 = new ListBox();

	private ListBox listBox_04 = new ListBox();

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection_01;

	private ObservableCollection _ItemCollection_02;

	private ObservableCollection _ItemCollection_03;

	private ObservableCollection _ItemCollection_04;
}
