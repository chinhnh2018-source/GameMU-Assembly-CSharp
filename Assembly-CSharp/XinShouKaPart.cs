using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class XinShouKaPart : UserControl
{
	public XinShouKaPart()
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
		this.Container.Children.Add(this.songliText);
		Canvas.SetLeft(this.songliText, 20);
		Canvas.SetTop(this.songliText, 412);
		this.songliText.TextColor = new SolidColorBrush(3323350U);
		this.songliText.Text = Global.GetLang("送礼对象：");
		this.Container.Children.Add(this.ActiviesTargetText);
		Canvas.SetLeft(this.ActiviesTargetText, 80);
		Canvas.SetTop(this.ActiviesTargetText, 412);
		this.ActiviesTargetText.TextColor = new SolidColorBrush(16764416U);
		this.ActiviesTargetText.HorizontalAlignment = global::Layout.Center;
		this.ActiviesTargetText.VerticalAlignment = global::Layout.Center;
		this.ActiviesTargetText.BodyWidth = 470.0;
		this.ActiviesTargetText.TextWidth = 470.0;
		this.Container.Children.Add(this.lingQuText);
		Canvas.SetLeft(this.lingQuText, 20);
		Canvas.SetTop(this.lingQuText, 436);
		this.lingQuText.TextColor = new SolidColorBrush(3323350U);
		this.lingQuText.Text = Global.GetLang("礼品码领取：");
		this.Container.Children.Add(this.ActiviesSourceText);
		Canvas.SetLeft(this.ActiviesSourceText, 90);
		Canvas.SetTop(this.ActiviesSourceText, 435);
		this.ActiviesSourceText.TextColor = new SolidColorBrush(10626862U);
		this.ActiviesSourceText.HorizontalAlignment = global::Layout.Center;
		this.ActiviesSourceText.VerticalAlignment = global::Layout.Center;
		this.ActiviesSourceText.BodyWidth = 470.0;
		this.ActiviesSourceText.TextWidth = 470.0;
		this.ActiviesSourceText.TextClick = new UIEventEventHandler(this.TextClick);
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 14);
		Canvas.SetTop(this.listBox, 50);
		this.listBox.Width = 570.0;
		this.listBox.Height = 288.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取奖励");
		Canvas.SetLeft(gicon, 340);
		Canvas.SetTop(gicon, 357);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (string.IsNullOrEmpty(this.VerificationCodeGtext.EditText) && this.VerificationCodeGtext.Visibility)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("请输入礼品码"), 0, -1, -1, 0);
				return;
			}
			this.GetAward(this.VerificationCodeGtext.EditText);
		};
		this.VerificationCodeGtext = U3DUtils.NEW<GTextBlock>();
		this.VerificationCodeGtext.BodyWidth = 136.0;
		this.VerificationCodeGtext.BodyHeight = 21.0;
		this.VerificationCodeGtext.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 136.0, 21.0, 3.0, 2.0));
		this.VerificationCodeGtext.Text.Text = string.Empty;
		this.VerificationCodeGtext.Text.FontSize = FontSizeMgr.NormalInputFontSize;
		this.VerificationCodeGtext.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.VerificationCodeGtext, 186);
		Canvas.SetTop(this.VerificationCodeGtext, 357);
		this.Container.Children.Add(this.VerificationCodeGtext);
		this.Container.Children.Add(this.shuruLiPinmainfo);
		Canvas.SetLeft(this.shuruLiPinmainfo, 28);
		Canvas.SetTop(this.shuruLiPinmainfo, 359);
		this.shuruLiPinmainfo.TextColor = new SolidColorBrush(4294967040U);
		this.shuruLiPinmainfo.HorizontalAlignment = global::Layout.Center;
		this.shuruLiPinmainfo.VerticalAlignment = global::Layout.Center;
		this.shuruLiPinmainfo.fontBold = true;
		this.shuruLiPinmainfo.Text = Global.GetLang("请输入礼品码领取礼包：");
	}

	public void InitPartData()
	{
		if (Global.Data.ActivitData == null)
		{
			GameInstance.Game.SpriteFetchActivtData(0);
			return;
		}
		string text = string.Empty;
		XElement xelement = XElement.Parse(Global.Data.ActivitData.ActivitiesXmlString);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		if (xelementList == null)
		{
			return;
		}
		XElement xelement2 = xelementList[0];
		if (xelement2 != null)
		{
			this.ActivitiesTimeText.htmlText = "<b>" + Global.GetColorStringForHtmlText(new object[]
			{
				"#ffffff37",
				Global.GetLang("活动时间："),
				"#ff37ff37",
				StringUtil.substitute(Global.GetLang("{0}-{1}"), new object[]
				{
					Global.GetXElementAttributeStr(xelement2, "FromDate"),
					Global.GetXElementAttributeStr(xelement2, "ToDate")
				})
			}) + "</b>";
			if ("-1" == Global.GetXElementAttributeStr(xelement2, "FromDate") && "-1" == Global.GetXElementAttributeStr(xelement2, "ToDate"))
			{
				this.ActivitiesTimeText.htmlText = "<b>" + Global.GetColorStringForHtmlText(new object[]
				{
					"#ffffff37",
					Global.GetLang("活动时间："),
					"#ff37ff37",
					Global.GetLang("【永久】")
				}) + "</b>";
			}
			this.TitleText.Text = Global.GetXElementAttributeStr(xelement2, "Title");
			if (Global.GetXElementAttributeStr(xelement2, "IsNeedCode") == "0")
			{
				this.VerificationCodeGtext.Visibility = false;
			}
		}
		List<XElement> xelementList2 = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "Item");
		if (xelementList2 == null)
		{
			return;
		}
		for (int i = 0; i < xelementList2.Count; i++)
		{
			xelement2 = xelementList2[i];
			text = Global.GetXElementAttributeStr(xelement2, "GoodsIDs");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Description");
			if (!string.IsNullOrEmpty(text))
			{
				this.BuildDataList(xelementAttributeStr, text);
			}
		}
		this.ItemCollection.DelayUpdate();
		xelementList2 = Global.GetXElementList(xelement, "ActiviTarget");
		xelement2 = xelementList2[0];
		this.ActiviesTargetText.Text = Global.GetXElementAttributeStr(xelement2, "Description");
		xelementList2 = Global.GetXElementList(Global.GetXElement(xelement, "LinkList"), "Item");
		if (xelementList2 != null)
		{
			string text2 = string.Empty;
			foreach (XElement xelement3 in xelementList2)
			{
				text2 += Global.GetXElementAttributeStr(xelement3, "title");
				text2 += " ";
			}
			this.ActiviesSourceText.Text = text2;
			foreach (XElement xelement4 in xelementList2)
			{
				this.ActiviesSourceText.SetSpecialText(Global.GetXElementAttributeStr(xelement4, "title"), new SolidColorBrush(4278222848U), true, Super.GetJSurl(7), true);
			}
		}
	}

	private void BuildDataList(string sTitle, string sBaoGuoid)
	{
		XinShouKaItem xinShouKaItem = U3DUtils.NEW<XinShouKaItem>();
		xinShouKaItem.GtextTitles = sTitle;
		xinShouKaItem.BodyWidth = 697.0;
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
				xinShouKaItem.GoodsBoxBackground = gicon;
				xinShouKaItem.ListBoxIcons = this.Geticon(array3[0], array3[1], array3[2], array3[3], array3[4], array3[5]);
			}
		}
		this.ItemCollection.AddNoUpdate(xinShouKaItem);
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
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.BoxTypes = -1;
		Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	private void GetAward(string liPinMa)
	{
		GameInstance.Game.SpriteGetSongLiGift(liPinMa);
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
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取活动送礼礼物的时间已经结束了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取活动送礼礼物的时间已经结束了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1020)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("礼品码不存在，请输入有效的礼品码!"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1021)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("礼品码错误，请输入当前区的礼品码!"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -1040)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("礼品码已经超过了最大使用次数限制，请输入其他的礼品码!"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -400)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包空格不足，请清理出空格后，再领取活动礼物"), new object[0]), 1, -1, -1, 0);
			}
			else if (result == -10000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("活动期间，相同礼包一个角色只能领取一次"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取活动礼物时出错: {0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			this.VerificationCodeGtext.EditText = string.Empty;
		}
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(e.Tag is SpecialTextItem))
		{
			return true;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		string text2 = (e.Tag as SpecialTextItem).Tag as string;
		if (text2 == null || string.Empty == text2)
		{
			return true;
		}
		Super.ExternalNavigateURL(text2);
		return true;
	}

	private ListBox listBox = new ListBox();

	private bool FirstGetNewData = true;

	private ObservableCollection ItemCollection;

	private GTextBlockOutLine ActivitiesTimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine shuruLiPinmainfo = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine TitleText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine lingQuText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine songliText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine textUrl = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlock VerificationCodeGtext;

	private GTextBlockEx ActiviesSourceText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private GTextBlockOutLine ActivitiesInfoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine ActiviesTargetText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ImageBrush GoodsBackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));

	private LoadingWindow LoadingWin;
}
