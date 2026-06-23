using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ChongZhiDaliPart : UserControl
{
	public ChongZhiDaliPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.TitleText);
		Canvas.SetLeft(this.TitleText, 30);
		Canvas.SetTop(this.TitleText, 17);
		this.TitleText.TextColor = new SolidColorBrush(4294967040U);
		this.TitleText.fontBold = true;
		this.Container.Children.Add(this.ActivitiesTimeText);
		Canvas.SetLeft(this.ActivitiesTimeText, 280);
		Canvas.SetTop(this.ActivitiesTimeText, 17);
		this.Container.Children.Add(this.otherCampaignText);
		Canvas.SetLeft(this.otherCampaignText, 40);
		Canvas.SetTop(this.otherCampaignText, 412);
		this.otherCampaignText.TextColor = new SolidColorBrush(16764416U);
		this.otherCampaignText.BodyWidth = 470.0;
		this.otherCampaignText.TextWidth = 470.0;
		this.Container.Children.Add(this.otherCampaignText2);
		Canvas.SetLeft(this.otherCampaignText2, 40);
		Canvas.SetTop(this.otherCampaignText2, 432);
		this.otherCampaignText2.TextColor = new SolidColorBrush(16764416U);
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 14);
		Canvas.SetTop(this.listBox, 50);
		this.listBox.Width = 570.0;
		this.listBox.Height = 288.0;
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		this.Container.Children.Add(this.gHudong);
		Canvas.SetLeft(this.gHudong, 28);
		Canvas.SetTop(this.gHudong, 359);
		this.gHudong.TextColor = new SolidColorBrush(4294967040U);
		this.gHudong.fontBold = true;
		this.gHudong.Text = Global.GetLang("活动介绍：");
		this.Container.Children.Add(this.JiFen);
		Canvas.SetLeft(this.JiFen, 315);
		Canvas.SetTop(this.JiFen, 359);
		this.JiFen.TextColor = new SolidColorBrush(uint.MaxValue);
		this.JiFen.FontSize = HSTextField.defaultFontSize;
		this.JiFen.Text = Global.GetLang("您当前拥有积分：");
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		string text = string.Empty;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/BigGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "GiftTime");
		if (xelementList == null)
		{
			return;
		}
		XElement xelement = xelementList[0];
		if (xelement != null)
		{
			this.ActivitiesTimeText.htmlText = "<b>" + Global.GetColorStringForHtmlText(new object[]
			{
				"#ffffff37",
				Global.GetLang("活动时间："),
				"#ff37ff37",
				Global.GetLang("【永久】")
			}) + "</b>";
			this.TitleText.Text = Global.GetXElementAttributeStr(xelement, "Title");
		}
		List<XElement> xelementList2 = Global.GetXElementList(Global.GetXElement(isolateResXml, "GiftList"), "Item");
		if (xelementList2 == null)
		{
			return;
		}
		for (int i = 0; i < xelementList2.Count; i++)
		{
			xelement = xelementList2[i];
			text = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Description");
			if (!string.IsNullOrEmpty(text))
			{
				this.BuildDataList(xelementAttributeStr, text, i);
			}
		}
		this.ItemCollection.DelayUpdate();
		xelementList2 = Global.GetXElementList(Global.GetXElement(isolateResXml, "OtherGifts"), "Item");
		xelement = xelementList2[0];
		this.otherCampaignText.Text = Global.GetXElementAttributeStr(xelement, "Description");
		xelement = xelementList2[1];
		this.otherCampaignText2.Text = Global.GetXElementAttributeStr(xelement, "Description");
		this.GetNewData();
	}

	private void BuildDataList(string sTitle, string sBaoGuoid, int index)
	{
		ChongZhiDaliItem chongZhiDaliItem = U3DUtils.NEW<ChongZhiDaliItem>();
		chongZhiDaliItem.GtextTitles = sTitle;
		chongZhiDaliItem.BodyWidth = 697.0;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		string[] array = sBaoGuoid.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].ToString().Split(new char[]
			{
				','
			});
			if (array2.Length == 6)
			{
				int[] array3 = Global.StringArray2IntArray(array2);
				gicon = U3DUtils.NEW<GIcon>();
				gicon.Width = 40.0;
				gicon.Height = 40.0;
				gicon.BodySource = this.GoodsBackImage;
				chongZhiDaliItem.GoodsBoxBackground = gicon;
				chongZhiDaliItem.ListBoxIcons = this.Geticon(array3[0], array3[1], array3[2], array3[3], array3[4], array3[5]);
			}
		}
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Width = 81.0;
		gicon2.Height = 21.0;
		gicon2.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon2.Text = Global.GetLang("领取奖励");
		gicon2.ItemCode = index + 1;
		gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward((s as GIcon).ItemCode);
		};
		chongZhiDaliItem.LingQuBtn = gicon2;
		this.ItemCollection.AddNoUpdate(chongZhiDaliItem);
	}

	private GGoodIcon Geticon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int bornInex)
	{
		this.goodVO = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (this.goodVO == null)
		{
			return null;
		}
		GoodsData goodsData = Global.AddGiftGoodsData(goodsID, forgeLevel, quality, binding, gcount, bornInex);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(this.goodVO.IconCode, string.Empty), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			12
		});
		ggoodIcon.ItemCategory = this.goodVO.Categoriy;
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.BoxTypes = -1;
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	private void GetAward(int whichOne)
	{
		GameInstance.Game.SpriteGetBigGift(whichOne);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
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
		GameInstance.Game.SpriteGetChongZhiJiFen();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
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

	public void GetAwardResult(int result, int roleID)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result < 0)
		{
			if (result == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取【充值有礼】礼物的时间已经结束了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取【充值有礼】礼物的时间已经结束了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的积分不足，无法领取【充值有礼】礼物"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -300)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包空格不足，请清理出空格后，再领取【充值有礼】礼物"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取【充值有礼】礼物时出错: {0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		this.NotifyChongZhiJiFenResult(roleID, result);
	}

	public void NotifyChongZhiJiFenResult(int roleID, int jiFen)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.JiFen.Text = Global.GetLang("您当前拥有积分：") + jiFen.ToString();
	}

	private ListBox listBox = new ListBox();

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private GoodVO goodVO;

	private ObservableCollection ItemCollection;

	private GTextBlockOutLine ActivitiesTimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine gHudong = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine TitleText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine otherCampaignText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine otherCampaignText2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine JiFen = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ImageBrush GoodsBackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));

	private LoadingWindow LoadingWin;
}
