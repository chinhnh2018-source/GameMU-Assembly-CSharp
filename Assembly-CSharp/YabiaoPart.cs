using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class YabiaoPart : UserControl
{
	public YabiaoPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.txtDescription.TextFontWrapping = TextWrapping.Wrap;
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
		this.thisCtrl = this;
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 270.0;
		this.listBox.Height = 126.0;
		Canvas.SetLeft(this.listBox, 14);
		Canvas.SetTop(this.listBox, 47);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.Container.Children.Add(this.txtNum);
		this.txtNum.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtNum, 295);
		Canvas.SetTop(this.txtNum, 69);
		this.Container.Children.Add(this.ScrollViewer4);
		this.ScrollViewer4.Width = 217.0;
		this.ScrollViewer4.Height = 253.0;
		Canvas.SetLeft(this.ScrollViewer4, 348);
		Canvas.SetTop(this.ScrollViewer4, 14);
		this.ScrollViewer4.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer4.Viewer = this.Wrapper;
		this.Wrapper.Width = 217.0;
		this.Wrapper.Children.Add(this.txtDescription);
		this.txtDescription.TextColor = new SolidColorBrush(4285638580U);
		Canvas.SetLeft(this.txtDescription, 5);
		Canvas.SetTop(this.txtDescription, 10);
		this.txtDescription.Width = 181.0;
		this.txtDescription.Height = 171.0;
		this.txtDescription.TextWidth = 181.0;
		this.txtDescription.TextLineHeight = 3.0;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.FreeJBIcon = U3DUtils.NEW<GIcon>();
		this.FreeJBIcon.Width = 80.0;
		this.FreeJBIcon.Height = 21.0;
		this.FreeJBIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.FreeJBIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.FreeJBIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.FreeJBIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.FreeJBIcon.Text = Global.GetLang("接镖");
		this.FreeJBIcon.EnableIcon = false;
		this.FreeJBIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			if (null == this.SelectedListItem)
			{
				return;
			}
			this.JBIconOK(0);
		};
		Canvas.SetLeft(this.FreeJBIcon, 349);
		Canvas.SetTop(this.FreeJBIcon, 285);
		this.Container.Children.Add(this.FreeJBIcon);
		this.GoodJBIcon = U3DUtils.NEW<GIcon>();
		this.GoodJBIcon.Width = 112.0;
		this.GoodJBIcon.Height = 21.0;
		this.GoodJBIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		this.GoodJBIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		this.GoodJBIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 112.0, 21.0, 3.0, 2.0));
		this.GoodJBIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.GoodJBIcon.Text = Global.GetLang("押镖令接镖");
		this.GoodJBIcon.EnableIcon = false;
		this.GoodJBIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			if (null == this.SelectedListItem)
			{
				return;
			}
			this.JBIconOK(1);
		};
		Canvas.SetLeft(this.GoodJBIcon, 438);
		Canvas.SetTop(this.GoodJBIcon, 285);
		this.Container.Children.Add(this.GoodJBIcon);
	}

	public void InitPartData()
	{
		this.ShowList();
		this.txtNum.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			Global.GetTodayYaBiaoNum(),
			Global.GetMaxDayYaBiaoNum()
		});
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	private void JBIconOK(int JBType)
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		int yaBiaoID = this.SelectedListItem.YaBiaoID;
		if (yaBiaoID <= 0)
		{
			return;
		}
		if (Global.Data.roleData.Level < this.SelectedListItem.MinLevel || Global.Data.roleData.Level > this.SelectedListItem.MaxLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("你的角色级别不在运镖的要求的范围内！"), 0, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.YinLiang < this.SelectedListItem.YaJin)
		{
			int toBuyGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("YinPiaoGoodsID");
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币余额不足，无法缴纳运镖需要的押金，【{0}金币】"), new object[]
			{
				this.SelectedListItem.YaJin
			}), 19, -1, -1, toBuyGoodsID);
			return;
		}
		int needGoodsID = 0;
		if (JBType == 0)
		{
			if (Global.GetTodayYaBiaoNum() >= Global.GetMaxDayYaBiaoNum())
			{
				needGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("YaBiaoLingGoodsID");
				string goodsNameByID = Global.GetGoodsNameByID(needGoodsID, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("今日的免费运镖次数已满，建议你使用【{0}】进行运镖"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
				return;
			}
		}
		else if (JBType == 1)
		{
			needGoodsID = (int)ConfigSystemParam.GetSystemParamIntByName("YaBiaoLingGoodsID");
		}
		string message = StringUtil.substitute(Global.GetLang("押镖需要扣除【{0}】金币作为押金。押镖成功后，全额退还。押镖失败，不再退还。继续押镖吗？"), new object[]
		{
			this.SelectedListItem.YaJin
		});
		GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), message, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(this.Container, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SpriteStartYaBiao(yaBiaoID, needGoodsID);
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
			}
			return true;
		};
	}

	public void NotifyJBResult(int roleID, int yaBiaoID, int retCode)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		int id = (int)ConfigSystemParam.GetSystemParamIntByName("YaBiaoLingGoodsID");
		string goodsNameByID = Global.GetGoodsNameByID(id, false);
		if (retCode < 0)
		{
			if (retCode == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您上次运镖未交，奖励未领取，您必须交完上次的运镖后再来接镖"), 0, -1, -1, 0);
			}
			else if (retCode == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("今日的免费运镖次数已满，建议你使用【{0}】进行运镖"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
			else if (retCode == -10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("接受{0}运镖时，背包中没有【{0}】, 无法接镖"), new object[]
				{
					goodsNameByID
				}), 9, -1, -1, 0);
			}
			else if (retCode == -11)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("接受【{0}】运镖时，从背包中扣除【{0}】时失败, 无法接镖"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("接受运镖时，发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			return;
		}
		this.txtNum.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			Global.GetTodayYaBiaoNum(),
			Global.GetMaxDayYaBiaoNum()
		});
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
	}

	private void ShowList()
	{
		this.ItemCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/Yabiao.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			YabiaoListItem yabiaoListItem = U3DUtils.NEW<YabiaoListItem>();
			yabiaoListItem.BodyWidth = 270.0;
			yabiaoListItem.BodyHeight = 21.0;
			yabiaoListItem.YaBiaoID = Global.GetXElementAttributeInt(xelement, "ID");
			yabiaoListItem.YaBiaoName = Global.GetXElementAttributeStr(xelement, "Name");
			yabiaoListItem.Level = StringUtil.substitute("{0}-{1}", new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinLevel"),
				Global.GetXElementAttributeStr(xelement, "MaxLevel")
			});
			yabiaoListItem.MinLevel = Global.GetXElementAttributeInt(xelement, "MinLevel");
			yabiaoListItem.MaxLevel = ((Global.GetXElementAttributeInt(xelement, "MaxLevel") >= 0) ? Global.GetXElementAttributeInt(xelement, "MaxLevel") : 65536);
			yabiaoListItem.YaJin = Global.GetXElementAttributeInt(xelement, "YaJin");
			yabiaoListItem.RewardYL = Global.GetXElementAttributeStr(xelement, "RewardYL");
			yabiaoListItem.RewardExp = Global.GetXElementAttributeStr(xelement, "RewardExp");
			yabiaoListItem.Description = Global.GetXElementAttributeStr(xelement, "Description");
			this.ItemCollection.AddNoUpdate(yabiaoListItem);
		}
		this.ItemCollection.DelayUpdate();
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
		this.SelectedListItem = U3DUtils.AS<YabiaoListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.FreeJBIcon.EnableIcon = false;
			this.GoodJBIcon.EnableIcon = false;
			this.UnSelectItem();
			return;
		}
		this.FreeJBIcon.EnableIcon = true;
		this.GoodJBIcon.EnableIcon = true;
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.BodyWidth = 233.0;
		this.SelectedListItem.BodyHeight = 21.0;
		this.txtDescription.Text = this.SelectedListItem.Description;
		this.Wrapper.Height = this.txtDescription.RealSize.Height + 30.0;
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

	private UserControl thisCtrl;

	private GIcon FreeJBIcon;

	private GIcon GoodJBIcon;

	private LoadingWindow LoadingWin;

	private YabiaoListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 270.0, 21.0, 5.0, 5.0));

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine txtNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GScrollView ScrollViewer4 = new GScrollView(0, 0, 0);

	private Canvas Wrapper = new Canvas();

	private GTextBlockOutLine txtDescription = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;
}
