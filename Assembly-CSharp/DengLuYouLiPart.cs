using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class DengLuYouLiPart : UserControl
{
	public DengLuYouLiPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		Canvas.SetZIndex(this.listBox, 100.0);
		this.ItemCollection2 = this.listBox2.ItemsSource;
		Canvas.SetZIndex(this.listBox2, 100.0);
		this.ItemCollection3 = this.listBox3.ItemsSource;
		Canvas.SetZIndex(this.listBox3, 100.0);
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.dayText);
		Canvas.SetLeft(this.dayText, 125);
		Canvas.SetTop(this.dayText, 125);
		this.dayText.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.NeedDayNum1);
		Canvas.SetLeft(this.NeedDayNum1, 52);
		Canvas.SetTop(this.NeedDayNum1, 288);
		this.NeedDayNum1.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.NeedDayNum2);
		Canvas.SetLeft(this.NeedDayNum2, 52);
		Canvas.SetTop(this.NeedDayNum2, 342);
		this.NeedDayNum2.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.NeedDayNum3);
		Canvas.SetLeft(this.NeedDayNum3, 52);
		Canvas.SetTop(this.NeedDayNum3, 392);
		this.NeedDayNum3.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 134);
		Canvas.SetTop(this.listBox, 278);
		this.listBox.Width = 254.0;
		this.listBox.Height = 40.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Opacity = 20.0;
		this.listBox.BorderThickness = 0;
		this.listBox.ItemMargin = new Thickness(18.0, 5.0, 0.0, 0.0);
		this.Container.Children.Add(this.listBox2);
		Canvas.SetLeft(this.listBox2, 134);
		Canvas.SetTop(this.listBox2, 330);
		this.listBox2.Width = 254.0;
		this.listBox2.Height = 40.0;
		this.listBox2.Background = new SolidColorBrush(16777215U);
		this.listBox2.Opacity = 20.0;
		this.listBox2.ItemMargin = new Thickness(18.0, 5.0, 0.0, 0.0);
		this.Container.Children.Add(this.listBox3);
		Canvas.SetLeft(this.listBox3, 134);
		Canvas.SetTop(this.listBox3, 382);
		this.listBox3.Width = 254.0;
		this.listBox3.Height = 40.0;
		this.listBox3.Background = new SolidColorBrush(16777215U);
		this.listBox3.Opacity = 20.0;
		this.listBox3.ItemMargin = new Thickness(18.0, 5.0, 0.0, 0.0);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "LingQu1";
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon, 486);
		Canvas.SetTop(gicon, 286);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward(1);
		};
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Name = "LingQu2";
		gicon2.Width = 66.0;
		gicon2.Height = 21.0;
		gicon2.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon2.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon2.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon2.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon2, 486);
		Canvas.SetTop(gicon2, 338);
		this.Container.Children.Add(gicon2);
		gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward(2);
		};
		GIcon gicon3 = U3DUtils.NEW<GIcon>();
		gicon3.Name = "LingQu3";
		gicon3.Width = 66.0;
		gicon3.Height = 21.0;
		gicon3.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon3.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon3, 486);
		Canvas.SetTop(gicon3, 391);
		this.Container.Children.Add(gicon3);
		gicon3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward(3);
		};
		this.NeedDayNum1.Text = StringUtil.substitute(Global.GetLang("{0}天"), new object[]
		{
			this.GetNeedDayNum(1)
		});
		this.NeedDayNum2.Text = StringUtil.substitute(Global.GetLang("{0}天"), new object[]
		{
			this.GetNeedDayNum(2)
		});
		this.NeedDayNum3.Text = StringUtil.substitute(Global.GetLang("{0}天"), new object[]
		{
			this.GetNeedDayNum(3)
		});
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.loadListDate();
		this.GetNewData();
	}

	private void loadListDate()
	{
		this.ItemCollection.Clear();
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/LoginNumGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Goods");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					for (int j = 0; j < array.Length; j++)
					{
						string[] array2 = array[j].Split(new char[]
						{
							','
						});
						if (array2.Length == 6)
						{
							int[] array3 = Global.StringArray2IntArray(array2);
							GGoodIcon ggoodIcon = this.Geticon(array3[0], array3[1], array3[2], array3[3], array3[4], array3[5]);
							if (!(null == ggoodIcon))
							{
								if (i == 0)
								{
									this.ItemCollection.AddNoUpdate(ggoodIcon);
								}
								else if (i == 1)
								{
									this.ItemCollection2.AddNoUpdate(ggoodIcon);
								}
								else if (i == 2)
								{
									this.ItemCollection3.AddNoUpdate(ggoodIcon);
								}
							}
						}
					}
					this.ItemCollection.DelayUpdate();
					this.ItemCollection2.DelayUpdate();
					this.ItemCollection3.DelayUpdate();
					this.CreateBackground(i, array.Length);
				}
			}
		}
	}

	private void CreateBackground(int iday, int BackgroundCont)
	{
		for (int i = 0; i < BackgroundCont; i++)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 40.0;
			gicon.Height = 40.0;
			gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));
			Canvas.SetLeft(gicon, 149 + i * 40 + i * 10);
			Canvas.SetTop(gicon, 279 + iday * 52);
			this.Container.Children.Add(gicon);
		}
	}

	private GGoodIcon Geticon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int bornInex)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		GoodsData goodsData = Global.AddGiftGoodsData(goodsID, forgeLevel, quality, binding, gcount, bornInex);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			12
		});
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.BoxTypes = -1;
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
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
		GameInstance.Game.SpriteGetHuoDongData();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void GetAward(int whichOne)
	{
		GameInstance.Game.SpriteGetWLoginGift(whichOne);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void SetLingQuIconState(int num, bool havelingQu, bool canLingQu)
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName(StringUtil.substitute("LingQu{0}", new object[]
		{
			num
		})));
		if (havelingQu)
		{
			gicon.Text = Global.GetLang("已领");
			gicon.EnableIcon = false;
		}
		else
		{
			gicon.Text = Global.GetLang("领取");
			gicon.EnableIcon = canLingQu;
		}
	}

	private int GetNeedDayNum(int whichOne)
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/LoginNumGift.Xml");
		if (isolateResXml == null)
		{
			return 10000;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Goods");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			if (whichOne == xelementAttributeInt)
			{
				return Global.GetXElementAttributeInt(xelement, "TimeOl");
			}
		}
		return 10000;
	}

	public void RefreshHuodongData()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.dayText.Text = Global.Data.MyHuoDongData.LoginNum.ToString();
		int loginGiftState = Global.Data.MyHuoDongData.LoginGiftState;
		int needDayNum = this.GetNeedDayNum(1);
		this.SetLingQuIconState(1, (loginGiftState & 1) == 1, Global.Data.MyHuoDongData.LoginNum >= needDayNum);
		needDayNum = this.GetNeedDayNum(2);
		this.SetLingQuIconState(2, (loginGiftState & 2) == 2, Global.Data.MyHuoDongData.LoginNum >= needDayNum);
		needDayNum = this.GetNeedDayNum(3);
		this.SetLingQuIconState(3, (loginGiftState & 4) == 4, Global.Data.MyHuoDongData.LoginNum >= needDayNum);
	}

	public void GetAwardResult(int result, int roleID)
	{
		if (result < 0)
		{
			if (result == -200)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请整理背包，清理出空格后，再领取礼物"), new object[0]), 1, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取礼物时出错: {0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		this.RefreshHuodongData();
	}

	private GTextBlockOutLine dayText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine NeedDayNum1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine NeedDayNum2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine NeedDayNum3 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private ListBox listBox2 = new ListBox();

	private ListBox listBox3 = new ListBox();

	public ObservableCollection ItemCollection;

	public ObservableCollection ItemCollection2;

	public ObservableCollection ItemCollection3;

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;
}
