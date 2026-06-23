using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class EnchaseBijouPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 35.0;
		this.Equip.Height = 35.0;
		Canvas.SetLeft(this.Equip, 53);
		Canvas.SetTop(this.Equip, 64);
		this.Equip.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Jewels);
		this.Jewels.Width = 80.0;
		this.Jewels.Height = 112.0;
		Canvas.SetLeft(this.Jewels, 33);
		Canvas.SetTop(this.Jewels, 168);
		this.Jewels.ItemMargin = new Thickness(0.0, 0.0, 8.0, 8.0);
		this.Jewels.Background = new SolidColorBrush(16777215U);
	}

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

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入装备");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Tip = "PutEquipBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 27);
		Canvas.SetTop(gicon, 20);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			(s as GIcon).Cursor = Cursors.Auto;
			ObjectClickGetingMgr.StartClickGetThing(4, e);
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入宝石");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Tip = "PutJewelBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 27);
		Canvas.SetTop(gicon, 289);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			(s as GIcon).Cursor = Cursors.Auto;
			ObjectClickGetingMgr.StartClickGetThing(5, e);
		};
	}

	public void InitPartData()
	{
		this.equipIcon = new List<ObservableCollection>();
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[1] = this.Jewels.ItemsSource;
	}

	public override void Destroy()
	{
		this.Container.Children.Clear();
		ObjectClickGetingMgr.CancelClickGetThing(4);
		ObjectClickGetingMgr.CancelClickGetThing(5);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ObjectClickEvent objectClickEvent = evt.Tag as ObjectClickEvent;
		if (objectClickEvent.ClickGetThingType != ClickGetThingTypes.EnchaseEquip && objectClickEvent.ClickGetThingType != ClickGetThingTypes.EnchaseJewel)
		{
			return;
		}
		if (objectClickEvent.ClickGetThingType == ClickGetThingTypes.EnchaseEquip)
		{
			this.ProcessAddEquip(objectClickEvent);
		}
		else if (objectClickEvent.ClickGetThingType == ClickGetThingTypes.EnchaseJewel)
		{
			this.ProcessAddJewel(objectClickEvent);
			objectClickEvent.NextClick = true;
		}
	}

	private void ProcessAddEquip(ObjectClickEvent e)
	{
		int clickGetThingDbID = e.ClickGetThingDbID;
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
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					goodsDataByDbID.Id,
					0
				});
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = goodsDataByDbID.GoodsID;
				ggoodIcon.ItemObject = goodsDataByDbID;
				ggoodIcon.BoxTypes = 13;
				ggoodIcon.Text = ((goodsDataByDbID.Forge_level <= 0) ? string.Empty : StringUtil.substitute("{0}", new object[]
				{
					goodsDataByDbID.Forge_level.ToString()
				}));
				ggoodIcon.TextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.TextVerticalAlignment = global::Layout.Top;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = 4294901760U;
				Super.InitEquipGIcon(ggoodIcon, goodsDataByDbID, false, IconTextTypes.Qianghua);
				this.equipIcon[0].Clear();
				this.equipIcon[0].Add(ggoodIcon);
				this.ShowJewelList(goodsDataByDbID.Jewellist);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有装备才能镶嵌宝石"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	private void ShowJewelList(string jewellist)
	{
		this.equipIcon[1].Clear();
		if (string.IsNullOrEmpty(jewellist))
		{
			return;
		}
		string[] array = jewellist.Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(array[i]));
			if (goodsXmlNodeByID != null)
			{
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy == 90 && this.equipIcon[1].Length < 6)
				{
					GIcon gicon = U3DUtils.NEW<GIcon>();
					gicon.Width = 32.0;
					gicon.Height = 32.0;
					gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
					gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
					gicon.TipType = 1;
					gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
					{
						goodsXmlNodeByID.ID,
						0,
						-1,
						-1
					});
					gicon.ItemCategory = goodsXmlNodeByID.Categoriy;
					gicon.ItemCode = Convert.ToInt32(array[i]);
					gicon.ItemObject = null;
					gicon.BoxTypes = 13;
					gicon.TextHorizontalAlignment = global::Layout.Left;
					gicon.TextVerticalAlignment = global::Layout.Top;
					gicon.TextColor = new SolidColorBrush(4294901760U);
					gicon.Text = StringUtil.substitute("LV{0}", new object[]
					{
						Global.GetJewelLevel(goodsXmlNodeByID.ID)
					});
					this.equipIcon[1].Add(gicon);
					gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.JewelMouseLeftButtonUp);
				}
			}
		}
	}

	private void JewelMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.equipIcon[0].Length <= 0)
		{
			return;
		}
		GoodsData goodsData = this.equipIcon[0][0].SafeGetComponent<GIcon>().ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GIcon gicon = sender as GIcon;
		int itemCode = gicon.ItemCode;
		string goodsNameByID = Global.GetGoodsNameByID(itemCode, false);
		GameInstance.Game.SpriteEnchaseJewel(1, goodsData.Id, itemCode);
	}

	private void ProcessAddJewel(ObjectClickEvent e)
	{
		if (this.equipIcon[0].Length <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请先将要镶嵌宝石的装备放到装备位置"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = this.equipIcon[0][0].SafeGetComponent<GIcon>().ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("要镶嵌宝石的装备不在背包中，无法镶嵌宝石"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int clickGetThingDbID = e.ClickGetThingDbID;
		GoodsData gd = Global.GetGoodsDataByDbID(clickGetThingDbID, null);
		if (gd == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy == 90)
			{
				if (Global.CanEnchaseJewel(gd.GoodsID))
				{
					if (Global.CanAddJewelIntoEquip(goodsData.GoodsID, gd.GoodsID))
					{
						if (this.equipIcon[1].Length < 6)
						{
							string goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
							string title = goodsXmlNodeByID.Title;
							string text = string.Empty;
							if (goodsData.Binding != gd.Binding)
							{
								if (goodsData.Binding > 0)
								{
									text = StringUtil.substitute(Global.GetLang("确定要将【{0}】镶嵌到已经绑定的{1}上吗\n镶嵌后【{2}】会被绑定?"), new object[]
									{
										title,
										goodsNameByID,
										title
									});
								}
								else if (gd.Binding > 0)
								{
									text = StringUtil.substitute(Global.GetLang("确定要将已经绑定的【{0}】镶嵌到{1}上吗\n镶嵌后{2}会被绑定?"), new object[]
									{
										title,
										goodsNameByID,
										goodsNameByID
									});
								}
							}
							if (string.Empty != text)
							{
								GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), text, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
								messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
								{
									int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
									Super.CloseMessageBox(this.Container, messageBoxWindow);
									if (messageBoxReturn == 0)
									{
										GameInstance.Game.SpriteEnchaseJewel(0, goodsData.Id, gd.Id);
									}
									return true;
								};
							}
							else
							{
								GameInstance.Game.SpriteEnchaseJewel(0, goodsData.Id, gd.Id);
							}
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("一件装备最多只能镶嵌6块宝石"), new object[0]), 0, -1, -1, 0);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetEquipEchaseJewelHint(goodsData.GoodsID), 0, -1, -1, 0);
					}
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】无法镶嵌到装备上"), new object[]
					{
						goodsXmlNodeByID.Title
					}), 0, -1, -1, 0);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有宝石才能镶嵌到装备上"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	public void NotifyEnchaseResult(GoodsData goodsData, int result, int equipDbID, string jewellist)
	{
		if (this.equipIcon[0].Length <= 0)
		{
			return;
		}
		GoodsData goodsData2 = this.equipIcon[0][0].SafeGetComponent<GIcon>().ItemObject as GoodsData;
		if (goodsData2.Id != equipDbID)
		{
			return;
		}
		if (result < 0)
		{
			if (result == -200)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有空格从装备上摘除宝石"), new object[0]), 1, -1, -1, 0);
			}
			else if (result == -9998)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经不在背包中，无法镶嵌宝石"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -9999)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备佩戴在身上时无法镶嵌宝石"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备镶嵌或摘除时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		this.ShowJewelList(jewellist);
	}

	private ListBox Equip = new ListBox();

	private ListBox Jewels = new ListBox();

	private List<ObservableCollection> _equipIcon;
}
