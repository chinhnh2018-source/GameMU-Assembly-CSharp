using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class VipNewPart : UserControl
{
	public VipNewPart()
	{
		this.ItemCollection = this.VIPJiangLiList.ItemsSource;
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

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
		this.PauseAllEffect(true);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData(int value)
	{
	}

	protected override void InitializeComponent()
	{
		this.xml = Global.GetGameResXml("Config/VIP.Xml");
		this.Container.Children.Add(this.HeadKuangImg);
		this.HeadKuangImg.Width = 90.0;
		this.HeadKuangImg.Height = 73.0;
		Canvas.SetLeft(this.HeadKuangImg, 10);
		Canvas.SetTop(this.HeadKuangImg, 10);
		this.HeadKuangImg.URL = "NetImages/GameRes/Images/Plate/npcTouxiang_bak.png";
		Canvas.SetZIndex(this.HeadKuangImg, 2.0);
		this.Container.Children.Add(this.MaskLayer);
		this.MaskLayer.graphics.beginFill(0, 1);
		this.MaskLayer.graphics.drawCircle(55, 47, 32);
		this.MaskLayer.graphics.endFill();
		this.Container.Children.Add(this.HeadImg);
		this.HeadImg.Width = 64.0;
		this.HeadImg.Height = 64.0;
		Canvas.SetLeft(this.HeadImg, 20);
		Canvas.SetTop(this.HeadImg, 20);
		string text = StringUtil.substitute("Images/Face/{0}{1}_0", new object[]
		{
			Global.Data.roleData.Occupation,
			Global.Data.roleData.RoleSex
		});
		this.HeadImg.Source = new ImageBrush(Global.GetLoginResImage(StringUtil.substitute("{0}.png", new object[]
		{
			text
		})));
		this.Container.Children.Add(this.RoleNameText);
		Canvas.SetLeft(this.RoleNameText, 114);
		Canvas.SetTop(this.RoleNameText, 15);
		this.RoleNameText.TextColor = new SolidColorBrush(16777080U);
		Super.FormatTextBlockEx2(this.RoleNameText, Global.GetLang("亲爱的：｛color=#0x00ffff uline=false tag= text=") + Global.Data.roleData.RoleName + Global.GetLang("｝"));
		this.Container.Children.Add(this.titleText);
		this.titleText.BodyWidth = 213.0;
		this.titleText.BodyHeight = 44.0;
		this.titleText.TextWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.titleText, 114);
		Canvas.SetTop(this.titleText, 32);
		this.titleText.TextColor = new SolidColorBrush(16777080U);
		this.Container.Children.Add(this.VIPJiangLiList);
		this.VIPJiangLiList.Width = 354.0;
		this.VIPJiangLiList.Height = 227.0;
		this.VIPJiangLiList.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.VIPJiangLiList, 12);
		Canvas.SetTop(this.VIPJiangLiList, 223);
		this.VIPJiangLiList.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.VIPJiangLiList.ItemMargin = new Thickness(2.0, 4.0, 0.0, 0.0);
		GIcon gicon = this.Geticon(30000);
		Canvas.SetLeft(gicon, 20);
		Canvas.SetTop(gicon, 122);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			this.InitVipListData("1");
		};
		GIcon gicon2 = this.Geticon(30001);
		this.Container.Children.Add(gicon2);
		Canvas.SetLeft(gicon2, 138);
		Canvas.SetTop(gicon2, 122);
		this.Container.Children.Add(gicon2);
		gicon2.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			this.InitVipListData("3");
		};
		GIcon gicon3 = this.Geticon(30002);
		this.Container.Children.Add(gicon3);
		Canvas.SetLeft(gicon3, 256);
		Canvas.SetTop(gicon3, 122);
		this.Container.Children.Add(gicon3);
		gicon3.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			this.InitVipListData("6");
		};
		this.Container.Children.Add(this.selectItemImage);
		this.selectItemImage.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/vip_select.png"));
		this.selectItemImage.mouseEnabled = false;
		BitmapData value = Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_normal.png"), 48.0, 24.0, 3.0, 2.0);
		BitmapData value2 = Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/tasktab_hover.png"), 48.0, 24.0, 3.0, 2.0);
		GIcon gicon4 = U3DUtils.NEW<GIcon>();
		gicon4.Name = VIPTypes.Month.ToString();
		gicon4.Width = 48.0;
		gicon4.Height = 24.0;
		gicon4.BodySource = new ImageBrush(value);
		gicon4.NewSource = new ImageBrush(value2);
		gicon4.Text = Global.GetLang("购买");
		gicon4.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon4.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 0
				});
			}
		};
		this.Container.Children.Add(gicon4);
		Canvas.SetLeft(gicon4, 79);
		Canvas.SetTop(gicon4, 144);
		gicon4 = U3DUtils.NEW<GIcon>();
		gicon4.Name = VIPTypes.Season.ToString();
		gicon4.Width = 48.0;
		gicon4.Height = 24.0;
		gicon4.BodySource = new ImageBrush(value);
		gicon4.NewSource = new ImageBrush(value2);
		gicon4.Text = Global.GetLang("购买");
		gicon4.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon4.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 1,
					ID = 1
				});
			}
		};
		this.Container.Children.Add(gicon4);
		Canvas.SetLeft(gicon4, 197);
		Canvas.SetTop(gicon4, 144);
		gicon4 = U3DUtils.NEW<GIcon>();
		gicon4.Name = VIPTypes.HalfYear.ToString();
		gicon4.Width = 48.0;
		gicon4.Height = 24.0;
		gicon4.BodySource = new ImageBrush(value);
		gicon4.NewSource = new ImageBrush(value2);
		gicon4.Text = Global.GetLang("购买");
		gicon4.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon4.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 2,
					ID = 2
				});
			}
		};
		this.Container.Children.Add(gicon4);
		Canvas.SetLeft(gicon4, 315);
		Canvas.SetTop(gicon4, 144);
		this.yueDeco = Global.GetDecoration(545, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.yueDeco.Coordinate = new Point(86, 140);
		Canvas.SetZIndex(this.yueDeco, 1.0);
		this.banNianDeco = Global.GetDecoration(547, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.banNianDeco.Coordinate = new Point(205, 140);
		Canvas.SetZIndex(this.banNianDeco, 1.0);
		this.nianDeco = Global.GetDecoration(546, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.nianDeco.Coordinate = new Point(323, 140);
		Canvas.SetZIndex(this.nianDeco, 1.0);
	}

	private void InitVipListData(string Type)
	{
		if (this.xml == null)
		{
			return;
		}
		this.SetselectItemImage(Type);
		this.ItemCollection.Clear();
		if (Convert.ToInt32(Type) == Global.GetVipType() && Convert.ToInt32(Type) != 0)
		{
			this.LoadVIPJiangLiList(Type);
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(this.xml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (!(Type != Global.GetXElementAttributeStr(xelement, "Type")))
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GetID");
				VIPJiangLiListItem vipjiangLiListItem = U3DUtils.NEW<VIPJiangLiListItem>();
				vipjiangLiListItem.BodyWidth = 298.0;
				vipjiangLiListItem.BodyHeight = 21.0;
				vipjiangLiListItem.JiangLiTextBodyWidth = 298.0;
				vipjiangLiListItem.JiangLiText = Global.GetXElementAttributeStr(xelement, "Description");
				this.ItemCollection.AddNoUpdate(vipjiangLiListItem);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void SetselectItemImage(string Type)
	{
		int num = 0;
		if (Convert.ToInt32(Type) == 3)
		{
			num = 1;
		}
		if (Convert.ToInt32(Type) == 6)
		{
			num = 2;
		}
		Canvas.SetLeft(this.selectItemImage, 15 + 118 * num);
		Canvas.SetTop(this.selectItemImage, 117);
	}

	private void LoadVIPJiangLiList(string Type)
	{
		if (this.xml == null)
		{
			return;
		}
		int num = Convert.ToInt32(Type);
		GIcon iconBtn = null;
		List<XElement> xelementList = Global.GetXElementList(this.xml, "Item");
		ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 39.0, 16.0, 3.0, 2.0));
		ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 39.0, 16.0, 3.0, 2.0));
		ImageBrush disableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 39.0, 16.0, 3.0, 2.0));
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GetID");
			if (num == Global.GetXElementAttributeInt(xelement, "Type") && xelementAttributeInt != 100 && xelementAttributeInt != 101 && xelementAttributeInt != 102)
			{
				if (xelementAttributeInt > 0)
				{
					string lang = Global.GetLang("领取");
					if (xelementAttributeInt == 14)
					{
						lang = Global.GetLang("清洗");
					}
					else if (xelementAttributeInt == 15)
					{
						lang = Global.GetLang("领取");
						if (!Global.HasNotLearnManuSkill())
						{
							lang = Global.GetLang("已学习");
						}
					}
					else if (xelementAttributeInt == 19)
					{
						lang = Global.GetLang("修理");
					}
					iconBtn = U3DUtils.NEW<GIcon>();
					iconBtn.Width = 39.0;
					iconBtn.Height = 16.0;
					iconBtn.Text = lang;
					iconBtn.BodySource = bodySource;
					iconBtn.NewSource = newSource;
					iconBtn.DisableBodySource = disableBodySource;
					iconBtn.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
					iconBtn.Name = "FetchBtn_" + xelementAttributeInt;
					iconBtn.ItemCode = xelementAttributeInt;
					iconBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						if (iconBtn.EnableIcon)
						{
							bool flag = true;
							if (iconBtn.ItemCode == 15 && Global.Data.roleData.Level < 35)
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("35级之后才可以学习本技能"), new object[0]), 0, -1, -1, 0);
								return;
							}
							if ((iconBtn.ItemCode == 10 || iconBtn.ItemCode == 11 || iconBtn.ItemCode == 12) && Global.IsBufferExist(39))
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您已拥有怒斩·PK王BUFFER,不能领取VIP护体状态"), new object[0]), 0, -1, -1, 0);
								return;
							}
							if (iconBtn.ItemCode == 14 && Global.Data.roleData.PKPoint < 200)
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您当前不是红名，不需要清洗"), new object[0]), 0, -1, -1, 0);
								return;
							}
							if (iconBtn.ItemCode == 19 && !Global.IsAnyEquipNeedMend())
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您当前身上佩戴的装备不需要修理"), new object[0]), 0, -1, -1, 0);
								return;
							}
							if ((iconBtn.ItemCode == 10 || iconBtn.ItemCode == 11 || iconBtn.ItemCode == 12) && Global.IsAttackFuZhouBufferExist())
							{
								flag = false;
							}
							if (iconBtn.ItemCode == 13 && Global.IsExpBufferExist())
							{
								flag = false;
							}
							if (iconBtn.ItemCode >= 16 && iconBtn.ItemCode <= 18 && Global.IsBufferExist(21))
							{
								flag = false;
							}
							if (flag)
							{
								GameInstance.Game.SpriteFetchVipDailyAward(iconBtn.ItemCode);
							}
							else
							{
								int nPriority = iconBtn.ItemCode;
								GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("相关buffer已经存在，领取后将覆盖，继续领取吗？"), new object[0]), (int)((this.Width - 253.0) / 2.0), (int)((this.Height - 171.0) / 2.0), (int)this.Width, (int)this.Height, 0.01, default(Vector3), null, null);
								messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
								{
									int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
									Super.CloseMessageBox(this.Container, messageBoxWindow);
									if (messageBoxReturn == 0)
									{
										GameInstance.Game.SpriteFetchVipDailyAward(nPriority);
									}
									return true;
								};
							}
						}
					};
					if (null == this.BtnArr[xelementAttributeInt])
					{
						this.BtnArr[xelementAttributeInt] = iconBtn;
					}
					else
					{
						iconBtn = this.BtnArr[xelementAttributeInt];
					}
				}
				else
				{
					iconBtn = null;
				}
				VIPJiangLiListItem vipjiangLiListItem = U3DUtils.NEW<VIPJiangLiListItem>();
				vipjiangLiListItem.BodyWidth = 298.0;
				vipjiangLiListItem.BodyHeight = 21.0;
				vipjiangLiListItem.JiangLiTextBodyWidth = 298.0;
				vipjiangLiListItem.JiangLiText = Global.GetXElementAttributeStr(xelement, "Description");
				if (null != iconBtn)
				{
					vipjiangLiListItem.BtnLingQu = iconBtn;
				}
				this.ItemCollection.AddNoUpdate(vipjiangLiListItem);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void SetFetchButtonStateForPriority(int priority, bool enable)
	{
		if (priority == 19)
		{
			return;
		}
		if (priority >= 0 && null != this.BtnArr[priority])
		{
			this.BtnArr[priority].EnableIcon = false;
		}
	}

	private GIcon Geticon(int GoodsCod)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsCod);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 48.0;
		gicon.Height = 48.0;
		gicon.TipType = 1;
		gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			GoodsCod,
			0,
			-1,
			-1
		});
		gicon.ItemCode = GoodsCod;
		gicon.DisableHandCursor = false;
		Super.GetGoods64x64ImageFromFile(goodsXmlNodeByID.IconCode, gicon);
		return gicon;
	}

	public void OnUseVipDailyPriorityCompleted(int result, int roleID, int priority)
	{
		if (result >= 0)
		{
			this.SetFetchButtonStateForPriority(priority, false);
		}
		else if (result == -10000)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，需要vip权限"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10001)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，今天你已经领取过了"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10004)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，没有需要领悟的技能了"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10005)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，需要领悟的技能条件不满足"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10006)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，未红名，不需要洗"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -125)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励失败，你的背包空格不够"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取特权奖励时错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void OnUpdateVipDailyData(List<VipDailyData> vipDailyDataList)
	{
		if (vipDailyDataList == null)
		{
			return;
		}
		int dayOfYear = Global.GetCorrectDateTime().DayOfYear;
		for (int i = 0; i < vipDailyDataList.Count; i++)
		{
			VipDailyData vipDailyData = vipDailyDataList[i];
			if (vipDailyData != null && dayOfYear == vipDailyData.DayID && vipDailyData.UsedTimes >= 1)
			{
				this.SetFetchButtonStateForPriority(vipDailyData.PriorityType, false);
			}
		}
	}

	public void OnVipOnceAwardGetCompleted(int result)
	{
		if (result >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("领取奖励成功"), 0, -1, -1, 0);
		}
		else if (result == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("你已经领取过了"), 0, -1, -1, 0);
		}
		else if (result == -2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("领取失败，背包空格不足"), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励时错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void Refresh(int vipLeve)
	{
		this.PauseAllEffect(false);
		if (this.vip == vipLeve)
		{
			return;
		}
		if (vipLeve == 0)
		{
			this.titleText.Text = Global.GetLang("您还不是VIP玩家，成为尊贵的VIP玩家即可获得以下丰厚的道具和特权");
			this.InitVipListData(VIPTypes.HalfYear.ToString());
		}
		else
		{
			GameInstance.Game.SpriteQueryVipDailyDataList();
			switch (vipLeve)
			{
			case 1:
				Super.FormatTextBlockEx2(this.titleText, StringUtil.substitute(Global.GetLang("恭喜，您已经是尊贵的｛color=#0x37ff37 uline=false tag= text={0}｝玩家!"), new object[]
				{
					Global.GetLang("白银VIP")
				}));
				break;
			case 3:
				Super.FormatTextBlockEx2(this.titleText, StringUtil.substitute(Global.GetLang("恭喜，您已经是尊贵的｛color=#0x37ff37 uline=false tag= text={0}｝玩家!"), new object[]
				{
					Global.GetLang("黄金VIP")
				}));
				break;
			case 6:
				Super.FormatTextBlockEx2(this.titleText, StringUtil.substitute(Global.GetLang("恭喜，您已经是尊贵的｛color=#0x37ff37 uline=false tag= text={0}｝玩家!"), new object[]
				{
					Global.GetLang("钻石VIP")
				}));
				break;
			}
			this.InitVipListData(vipLeve.ToString());
		}
		this.vip = vipLeve;
	}

	public void PauseAllEffect(bool pause)
	{
		if (this.yueDeco != null)
		{
			this.yueDeco.Pause = pause;
		}
		if (this.banNianDeco != null)
		{
			this.banNianDeco.Pause = pause;
		}
		if (this.nianDeco != null)
		{
			this.nianDeco.Pause = pause;
		}
	}

	private URLImage HeadKuangImg = new URLImage();

	private URLImage vipImg0 = new URLImage();

	private URLImage vipImg1 = new URLImage();

	private URLImage vipImg2 = new URLImage();

	private Image HeadImg = new Image();

	private Sprite MaskLayer = new Sprite();

	private GTextBlockEx RoleNameText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private GTextBlockEx titleText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private XElement xml;

	private GIcon[] BtnArr = new GIcon[0];

	private GDecoration yueDeco;

	private GDecoration banNianDeco;

	private GDecoration nianDeco;

	private ListBox VIPJiangLiList = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	public Image selectItemImage = new Image();

	private ObservableCollection _ItemCollection;

	private int vip = -1;
}
