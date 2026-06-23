using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ZadanXingyunzhiDuihuanPart : UserControl
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
		this.Container.Children.Add(this.listbox);
		this.listbox.Background = new SolidColorBrush(16777215U);
		this.listbox.Width = 352.0;
		this.listbox.Height = 48.0;
		this.listbox.ItemMargin = new Thickness(0.0, 0.0, 22.0, 0.0);
		Canvas.SetLeft(this.listbox, 55);
		Canvas.SetTop(this.listbox, 35);
		this.ItemCollection = this.listbox.ItemsSource;
	}

	public void InitPartData()
	{
		this.LoadList();
		GameInstance.Game.SpriteGetZaJinDanJiFen();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
		this.PauseAllEffect(true);
	}

	public void PauseAllEffect(bool pause)
	{
		for (int i = 0; i < this.DecoArr.Count; i++)
		{
			if (this.DecoArr[i] != null)
			{
				this.DecoArr[i].Pause = pause;
			}
		}
	}

	public void GetNewData()
	{
	}

	private void LoadList()
	{
		this.ItemCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/LuckyAward2.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		string text = string.Empty;
		for (int i = 0; i < xelementList.Count; i++)
		{
			text = StringUtil.trim(Global.GetXElementAttributeStr(xelementList[i], "GoodsIDs"));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array.Length == 6)
			{
				this.AddGoodsIcon(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), -1);
			}
		}
		this.ItemCollection.DelayUpdate();
		this.LoadBtn(xelementList.Count);
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataEx = Global.GetDummyGoodsDataEx(goodsID, forgeLevel, quality, binding, gcount, born);
			string goodsImageURLFromIconCodeEx = Super.GetGoodsImageURLFromIconCodeEx(Super.GetIconCode(goodsXmlNodeByID));
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 48.0;
			ggoodIcon.Height = 48.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCodeEx, false, 2);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				default(object),
				-1
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = dummyGoodsDataEx;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = gcount.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			if (Global.IsForgeRockGoodsID(goodsID))
			{
				ggoodIcon.STextVisibility = true;
				ggoodIcon.SText = StringUtil.substitute("{0}", new object[]
				{
					Global.GetForgeRockLevelNames(goodsID)
				});
				ggoodIcon.STextHorizontalAlignment = global::Layout.Left;
				ggoodIcon.STextVerticalAlignment = global::Layout.Top;
				ggoodIcon.STextColor = uint.MaxValue;
				ggoodIcon.STextShadowColor = 24831U;
			}
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataEx, true, IconTextTypes.Qianghua);
			this.ItemCollection.AddNoUpdate(ggoodIcon);
		}
	}

	private void LoadBtn(int num)
	{
		ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 60.0, 21.0, 3.0, 2.0));
		ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 60.0, 21.0, 3.0, 2.0));
		ImageBrush disableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 60.0, 21.0, 3.0, 2.0));
		int num2 = 48;
		int value = 114;
		for (int i = 0; i < num; i++)
		{
			GIcon iconBtn = U3DUtils.NEW<GIcon>();
			iconBtn.Width = 60.0;
			iconBtn.Height = 21.0;
			iconBtn.Text = Global.GetLang("领取");
			iconBtn.BodySource = bodySource;
			iconBtn.NewSource = newSource;
			iconBtn.DisableBodySource = disableBodySource;
			iconBtn.TextColor = new SolidColorBrush(10551295U);
			iconBtn.ItemCode = i + 1;
			iconBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.EnableIcon)
				{
					GameInstance.Game.SpriteGetZaJinDanJiFenAwards(iconBtn.ItemCode);
				}
			};
			iconBtn.EnableIcon = false;
			this.BtnArr[i] = iconBtn;
			Canvas.SetLeft(iconBtn, i * 70 + num2);
			Canvas.SetTop(iconBtn, value);
			this.Container.Children.Add(iconBtn);
		}
	}

	public void EnableFetchButton(int awardNo, bool enable = false)
	{
		for (int i = 0; i < this.BtnArr.Length; i++)
		{
			if (awardNo == this.BtnArr[i].ItemCode)
			{
				this.BtnArr[i].EnableIcon = enable;
				break;
			}
		}
	}

	public void OnQueryZadanDailyDataCompleted(int jiFen, int bits)
	{
		for (int i = 0; i < this.BtnArr.Length; i++)
		{
			this.BtnArr[i].EnableIcon = Global.IsZaJinDanAwardCanBeenGot(this.BtnArr[i].ItemCode, jiFen);
			if (Global.IsZaJinDanAwardHasBeenGot(this.BtnArr[i].ItemCode, bits))
			{
				this.BtnArr[i].EnableIcon = false;
				this.BtnArr[i].Text = Global.GetLang("已经领取");
			}
		}
	}

	public void OnFetchZadanJiFenAwardCompleted(int result, int roleID, int awardNo)
	{
		if (result >= 0)
		{
			this.EnableFetchButton(awardNo, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励成功"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10006)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，幸运值不够"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10007)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，幸运值不够"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -10008)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，已经领取过了"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -125)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，你的背包空格不够"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励时错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<GDecoration> DecoArr = new List<GDecoration>();

	private ListBox listbox = new ListBox();

	private GIcon[] BtnArr = new GIcon[0];

	private ObservableCollection _ItemCollection;
}
