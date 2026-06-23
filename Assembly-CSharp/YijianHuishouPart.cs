using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class YijianHuishouPart : UserControl
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.goodList.Width = 320.0;
		this.goodList.Height = 200.0;
		this.goodList.Background = new SolidColorBrush(16777215U);
		this.goodList.BorderThickness = 0;
		this.goodList.ItemMargin = new Thickness(0.0, 0.0, 8.0, 8.0);
		Canvas.SetLeft(this.goodList, 17);
		Canvas.SetTop(this.goodList, 17);
		this.Container.Children.Add(this.goodList);
		this.ItemCollection = this.goodList.ItemsSource;
		GIcon icon = U3DUtils.NEW<GIcon>();
		icon.Width = 81.0;
		icon.Height = 21.0;
		icon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		icon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		icon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		icon.Text = Global.GetLang("回收");
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ItemCollection.Count <= 0)
			{
				return;
			}
			string text = string.Empty;
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				GoodsData goodsData = U3DUtils.AS<GIcon>(this.ItemCollection[i]).ItemObject as GoodsData;
				if (text.Length > 0)
				{
					text += ",";
				}
				text += goodsData.Id;
			}
			GameInstance.Game.SpriteOneKeyQuickSaleOut(2, text);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		Canvas.SetLeft(icon, 135);
		Canvas.SetTop(icon, 309);
		this.Container.Children.Add(icon);
		icon = U3DUtils.NEW<GIcon>();
		icon.Width = 81.0;
		icon.Height = 21.0;
		icon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		icon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		icon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		icon.Text = Global.GetLang("取消");
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		Canvas.SetLeft(icon, 232);
		Canvas.SetTop(icon, 309);
		this.Container.Children.Add(icon);
		icon = U3DUtils.NEW<GIcon>();
		icon.Name = "FangruIcon";
		icon.Width = 81.0;
		icon.Height = 21.0;
		icon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		icon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		icon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		icon.Text = Global.GetLang("放入");
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (ObjectClickGetingMgr.IsType(18))
			{
				ObjectClickGetingMgr.CancelClickGetThing(18);
				icon.Text = Global.GetLang("放入");
			}
			else
			{
				(s as GIcon).Cursor = Cursors.Auto;
				icon.Text = Global.GetLang("取消放入");
				ObjectClickGetingMgr.StartClickGetThing(18, e);
			}
		};
		Canvas.SetLeft(icon, 38);
		Canvas.SetTop(icon, 309);
		this.Container.Children.Add(icon);
		this.JingyuanNumText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.JingyuanNumText.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.JingyuanNumText, 90);
		Canvas.SetTop(this.JingyuanNumText, 226);
		this.Container.Children.Add(this.JingyuanNumText);
		this.JingyanNumText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.JingyanNumText.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.JingyanNumText, 255);
		Canvas.SetTop(this.JingyanNumText, 226);
		this.Container.Children.Add(this.JingyanNumText);
	}

	public void InitPartData()
	{
		this.LoadAllWhiteEquip();
		this.SetPrice();
	}

	public void CleanUpChildWindows()
	{
		ObjectClickGetingMgr.CancelClickGetThing(18);
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName("FangruIcon"));
		if (null != gicon)
		{
			gicon.Text = Global.GetLang("放入");
		}
	}

	public void GetNewData()
	{
	}

	private void SetPrice()
	{
		this.JingyuanNum = 0;
		this.JingyanNum = 0;
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			int num = 0;
			int num2 = 0;
			if (Global.GetGoodsSaleBackJingyuaAndExp(U3DUtils.AS<GIcon>(this.ItemCollection[i]).ItemObject as GoodsData, out num, out num2))
			{
				this.JingyuanNum += num;
				this.JingyanNum += num2;
			}
		}
		this.JingyuanNumText.Text = this.JingyuanNum.ToString();
		this.JingyanNumText.Text = this.JingyanNum.ToString();
	}

	private void LoadAllWhiteEquip()
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
			if (goodsData.GCount > 0)
			{
				if (goodsData.Using <= 0)
				{
					if (goodsData.Forge_level <= 0)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
						if (goodsXmlNodeByID != null)
						{
							if (goodsXmlNodeByID.ToLevel < 50)
							{
								if (!(goodsXmlNodeByID.ToType != "-1"))
								{
									int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
									if (categoriyByGoodsID == 7)
									{
										this.AddIcon(goodsData);
									}
									if (goodsXmlNodeByID.ChangeJinYuan > 0)
									{
										if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
										{
											this.AddIcon(goodsData);
										}
										if (this.ItemCollection.Count >= 40)
										{
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddIcon(GoodsData goodsData)
	{
		GGoodIcon ggoodIcon = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			int categoriy = goodsXmlNodeByID.Categoriy;
			try
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 64.0;
				ggoodIcon.Height = 64.0;
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsData.GoodsID,
					1,
					goodsData.Id,
					0
				});
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemCode = goodsData.GoodsID;
				ggoodIcon.ItemObject = goodsData;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.BoxTypes = 1;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			ggoodIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EquipIconMouseLeftButtonUp);
			this.ItemCollection.AddNoUpdate(ggoodIcon);
		}
	}

	private void EquipIconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		GIcon gicon = sender as GIcon;
		if (null == gicon)
		{
			return;
		}
		this.ItemCollection.Remove(gicon);
		this.SetPrice();
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 18)
		{
			return;
		}
		if (clickGetThingEventArgs.Cancel)
		{
			return;
		}
		object sender = clickGetThingEventArgs.sender;
		GIcon gicon = sender as GIcon;
		if (null == gicon)
		{
			return;
		}
		GoodsData goodsData = gicon.ItemObject as GoodsData;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			if (goodsXmlNodeByID.ChangeJinYuan > 0)
			{
				if (!this.FindGoodDataByID(goodsData.Id))
				{
					if (goodsXmlNodeByID.ToLevel > 40)
					{
						if (this.ItemCollection.Count < 40)
						{
							this.AddIcon(goodsData);
							this.ItemCollection.DelayUpdate();
							this.SetPrice();
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("一键回收的空间已经满，请先回收后，再进行添加操作"), new object[0]), 0, -1, -1, 0);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("此物品不可回收"), new object[0]), 0, -1, -1, 0);
					}
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("物品已存在"), new object[0]), 0, -1, -1, 0);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("此物品不能回收"), new object[0]), 0, -1, -1, 0);
			}
		}
		clickGetThingEventArgs.NextClick = true;
	}

	private bool FindGoodDataByID(int id)
	{
		if (this.ItemCollection.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			if ((U3DUtils.AS<GIcon>(this.ItemCollection[i]).ItemObject as GoodsData).Id == id)
			{
				return true;
			}
		}
		return false;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int JingyuanNum;

	private int JingyanNum;

	private GTextBlockOutLine JingyuanNumText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine JingyanNumText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox goodList = new ListBox();

	private ObservableCollection _ItemCollection;
}
