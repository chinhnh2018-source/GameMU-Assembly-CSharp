using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ZhuangBeiChuanChengPart : UserControl
{
	public List<ObservableCollection> equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public bool ChuanChengZhong
	{
		get
		{
			return this._ChuanChengZhong;
		}
		set
		{
			this._ChuanChengZhong = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.YuanZhuangBei);
		this.YuanZhuangBei.Width = 32.0;
		this.YuanZhuangBei.Height = 32.0;
		Canvas.SetLeft(this.YuanZhuangBei, 53);
		Canvas.SetTop(this.YuanZhuangBei, 48);
		this.YuanZhuangBei.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.MuBiaoZhuangBei);
		this.MuBiaoZhuangBei.Width = 32.0;
		this.MuBiaoZhuangBei.Height = 32.0;
		Canvas.SetLeft(this.MuBiaoZhuangBei, 255);
		Canvas.SetTop(this.MuBiaoZhuangBei, 48);
		this.MuBiaoZhuangBei.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Rock);
		this.Rock.Width = 32.0;
		this.Rock.Height = 32.0;
		Canvas.SetLeft(this.Rock, 154);
		Canvas.SetTop(this.Rock, 126);
		this.Rock.Background = new SolidColorBrush(16777215U);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入装备");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Tip = "PutEquipBtn";
		gicon.TipType = 4;
		gicon.ItemCode = 0;
		Canvas.SetLeft(gicon, 23);
		Canvas.SetTop(gicon, 16);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入装备");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Tip = "PutEquipBtn";
		gicon.TipType = 4;
		gicon.ItemCode = 1;
		Canvas.SetLeft(gicon, 236);
		Canvas.SetTop(gicon, 16);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		this.YinLiangText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.YinLiangText.TextFontColor = new SolidColorBrush(uint.MaxValue);
		this.YinLiangText.Text = "0";
		this.YinLiangText.Height = 14.0;
		this.YinLiangText.TextSize = 12.0;
		this.YinLiangText.Width = 30.0;
		Canvas.SetLeft(this.YinLiangText, 145);
		Canvas.SetTop(this.YinLiangText, 173);
		this.Container.Children.Add(this.YinLiangText);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("传承");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.LeftGoodsData != null && this.RightGoodsData != null)
			{
				GameInstance.Game.SpriteDoEquipInherit(this.LeftGoodsData.Id, this.RightGoodsData.Id, this.NeedRockGoodsID);
			}
		};
		Canvas.SetLeft(gicon, 131);
		Canvas.SetTop(gicon, 197);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		this.equipIcon = new List<ObservableCollection>();
		this.equipIcon[0] = this.YuanZhuangBei.ItemsSource;
		this.equipIcon[1] = this.MuBiaoZhuangBei.ItemsSource;
		this.equipIcon[2] = this.Rock.ItemsSource;
		this.InitChuanChengChaiLiao();
	}

	private void InitChuanChengChaiLiao()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EquipInherit", '|');
		if (systemParamIntArrayByName.Length != 3)
		{
			return;
		}
		this.NeedRockGoodsID = systemParamIntArrayByName[0];
		this.NeedRockGoodsCount = systemParamIntArrayByName[1];
		this.NeedYinLiang = systemParamIntArrayByName[2];
		this.AddGoodsIcon(this.NeedRockGoodsID, 2, 1, true);
		this.YinLiangText.Text = string.Empty + this.NeedYinLiang;
		if (Global.Data.roleData.YinLiang < Convert.ToInt32(this.YinLiangText))
		{
			this.YinLiangText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
		}
		else
		{
			this.YinLiangText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
		}
	}

	private void InputEquipMouseLeftButtonUp(object sender, MouseEvent e)
	{
		(sender as GIcon).Cursor = Cursors.Auto;
		this.index = (sender as GIcon).ItemCode;
		ObjectClickGetingMgr.StartClickGetThing(12, e);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 12)
		{
			return;
		}
		if (this.ChuanChengZhong)
		{
			return;
		}
		if (this.index == -1)
		{
			return;
		}
		int clickGetThingDbID = clickGetThingEventArgs.ClickGetThingDbID;
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(clickGetThingDbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					goodsDataByDbID.Id,
					0
				});
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemCode = goodsDataByDbID.GoodsID;
				ggoodIcon.ItemObject = goodsDataByDbID;
				ggoodIcon.BoxTypes = 5;
				ggoodIcon.Text = ((goodsDataByDbID.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
				{
					goodsDataByDbID.Forge_level.ToString()
				}));
				ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.TextVerticalAlignment = global::Layout.Top;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = 4294901760U;
				Super.InitEquipGIcon(ggoodIcon, goodsDataByDbID, false, IconTextTypes.Qianghua);
				this.equipIcon[this.index].Clear();
				this.equipIcon[this.index].Add(ggoodIcon);
				if (this.index == 0)
				{
					this.LeftGoodsData = goodsDataByDbID;
				}
				else if (this.index == 1)
				{
					this.RightGoodsData = goodsDataByDbID;
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有装备才能传承"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	private void AddGoodsIconForEquip(GoodsData goodsData, int index)
	{
		int id = goodsData.Id;
		if (goodsData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					goodsData.Id,
					0
				});
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemCode = goodsData.GoodsID;
				ggoodIcon.ItemObject = goodsData;
				ggoodIcon.BoxTypes = 5;
				ggoodIcon.Text = ((goodsData.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
				{
					goodsData.Forge_level.ToString()
				}));
				ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.TextVerticalAlignment = global::Layout.Top;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = 4294901760U;
				Super.InitEquipGIcon(ggoodIcon, goodsData, false, IconTextTypes.Qianghua);
				this.equipIcon[index].Clear();
				this.equipIcon[index].Add(ggoodIcon);
				if (index == 0)
				{
					this.LeftGoodsData = goodsData;
				}
				else if (index == 1)
				{
					this.RightGoodsData = goodsData;
				}
			}
		}
	}

	private void AddGoodsIcon(int goodsID, int index, int iNeedNub, bool showNum = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		string text = iNeedNub.ToString();
		if (!showNum)
		{
			text = string.Empty;
		}
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
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
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = text;
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableHandCursor = true;
			if (Global.IsForgeRockGoodsID(goodsID))
			{
				gicon.STextVisibility = true;
				gicon.SText = StringUtil.substitute("{0}", new object[]
				{
					Global.GetForgeRockLevelNames(goodsID)
				});
				gicon.STextHorizontalAlignment = global::Layout.Left;
				gicon.STextVerticalAlignment = global::Layout.Top;
				gicon.STextColor = new SolidColorBrush(uint.MaxValue);
				gicon.STextShadowColor = 24831U;
			}
			if (Global.GetTotalGoodsCountByID(goodsID) > 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(gicon);
		}
	}

	public override void Destroy()
	{
		ObjectClickGetingMgr.CancelClickGetThing(12);
	}

	public void OnEquipInheritCompleted(int result, GoodsData leftGoodsData, GoodsData rightGoodsData)
	{
		string goodsNameByID = Global.GetGoodsNameByID(leftGoodsData.GoodsID, false);
		string goodsNameByID2 = Global.GetGoodsNameByID(this.RightGoodsData.GoodsID, false);
		string goodsNameByID3 = Global.GetGoodsNameByID(this.NeedRockGoodsID, false);
		if (result < 1)
		{
			if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败，你没有提供传承属性的【{0}】"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败，你没有接受传承属性的【{0}】"), new object[]
				{
					goodsNameByID2
				}), 0, -1, -1, 0);
			}
			else if (result == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败，传承物品【{0}】或【{1}】不在背包中"), new object[]
				{
					goodsNameByID,
					goodsNameByID2
				}), 0, -1, -1, 0);
			}
			else if (result == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败，传承物品【{0}】或【{1}】处于装备中"), new object[]
				{
					goodsNameByID,
					goodsNameByID2
				}), 0, -1, -1, 0);
			}
			else if (result == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}数量不足，需要{1}个"), new object[]
				{
					goodsNameByID3,
					this.NeedRockGoodsCount
				}), 19, -1, -1, this.NeedRockGoodsID);
			}
			else if (result == -8)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			}
			else if (result == -9)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】的各项可传承属性都比【{1}】高，不需要传承"), new object[]
				{
					goodsNameByID2,
					goodsNameByID
				}), 0, -1, -1, 0);
			}
			else if (result == -11)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("洗练时扣除{0}两银子失败"), new object[]
				{
					this.NeedYinLiang
				}), 0, -1, -1, 0);
			}
			else if (result == -201)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败，传承物品【{0}】与【{1}】不是同类型装备,它们必须是同职业同类型装备"), new object[]
				{
					goodsNameByID,
					goodsNameByID2
				}), 0, -1, -1, 0);
			}
			else if (result == -202)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败，传承物品【{0}】与【{1}】不是职业装备,它们必须是同职业同类型装备"), new object[]
				{
					goodsNameByID,
					goodsNameByID2
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("洗练【{0}】时发生错误:{1}"), new object[]
				{
					base.name,
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		this.AddGoodsIconForEquip(this.LeftGoodsData, 0);
		this.AddGoodsIconForEquip(this.RightGoodsData, 1);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功地将【{0}】的属性传承到了【{1}】"), new object[]
		{
			goodsNameByID,
			goodsNameByID2
		}), 0, -1, -1, 0);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine YinLiangText;

	private ListBox YuanZhuangBei = new ListBox();

	private ListBox MuBiaoZhuangBei = new ListBox();

	private ListBox Rock = new ListBox();

	private GoodsData LeftGoodsData;

	private GoodsData RightGoodsData;

	private int NeedRockGoodsID = -1;

	private int NeedRockGoodsCount = 1;

	private int NeedYinLiang;

	private List<ObservableCollection> _equipIcon;

	private bool _ChuanChengZhong;

	private int index = -1;
}
