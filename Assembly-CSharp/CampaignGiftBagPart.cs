using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class CampaignGiftBagPart : UserControl
{
	public CampaignGiftBagPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.TitleText);
		Canvas.SetLeft(this.TitleText, 30);
		Canvas.SetTop(this.TitleText, 20);
		this.TitleText.TextColor = new SolidColorBrush(16690432U);
		this.TitleText.FontSize = HSTextField.defaultFontSize;
		this.TitleText.HorizontalAlignment = global::Layout.Center;
		this.TitleText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.CampaignStartTimeText);
		Canvas.SetLeft(this.CampaignStartTimeText, 306);
		Canvas.SetTop(this.CampaignStartTimeText, 18);
		this.CampaignStartTimeText.TextColor = new SolidColorBrush(16711680U);
		this.CampaignStartTimeText.FontSize = HSTextField.defaultFontSize;
		this.CampaignStartTimeText.HorizontalAlignment = global::Layout.Center;
		this.CampaignStartTimeText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.CampaignEndTimeText);
		Canvas.SetLeft(this.CampaignEndTimeText, 426);
		Canvas.SetTop(this.CampaignEndTimeText, 18);
		this.CampaignEndTimeText.TextColor = new SolidColorBrush(16711680U);
		this.CampaignEndTimeText.FontSize = HSTextField.defaultFontSize;
		this.CampaignEndTimeText.HorizontalAlignment = global::Layout.Center;
		this.CampaignEndTimeText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 514.0;
		this.listBox.Height = 205.0;
		Canvas.SetLeft(this.listBox, 26);
		Canvas.SetTop(this.listBox, 44);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.Container.Children.Add(this.JiFen);
		Canvas.SetLeft(this.JiFen, 330);
		Canvas.SetTop(this.JiFen, 258);
		this.JiFen.TextColor = new SolidColorBrush(uint.MaxValue);
		this.JiFen.FontSize = HSTextField.defaultFontSize;
		this.Container.Children.Add(this.otherCampaignText);
		Canvas.SetLeft(this.otherCampaignText, 40);
		Canvas.SetTop(this.otherCampaignText, 294);
		this.otherCampaignText.TextColor = new SolidColorBrush(4894923U);
		this.otherCampaignText.FontSize = HSTextField.defaultFontSize;
		this.Container.Children.Add(this.otherCampaignText2);
		Canvas.SetLeft(this.otherCampaignText2, 40);
		Canvas.SetTop(this.otherCampaignText2, 311);
		this.otherCampaignText2.TextColor = new SolidColorBrush(4894923U);
		this.otherCampaignText2.FontSize = HSTextField.defaultFontSize;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
	}

	private void GetAward(int whichOne)
	{
		GameInstance.Game.SpriteGetBigGift(whichOne);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.tc.Container.Children.Add(this.LoadingWin);
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/BigGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "GiftTime");
		XElement xelement = xelementList[0];
		this.TitleText.Text = Global.GetXElementAttributeStr(xelement, "Title");
		this.CampaignStartTimeText.Text = Global.GetXElementAttributeStr(xelement, "FromDate");
		this.CampaignEndTimeText.Text = Global.GetXElementAttributeStr(xelement, "ToDate");
		xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "OtherGifts"), "Item");
		xelement = xelementList[0];
		if (xelement != null)
		{
			this.otherCampaignText.Text = Global.GetXElementAttributeStr(xelement, "Description");
		}
		xelement = xelementList[1];
		if (xelement != null)
		{
			this.otherCampaignText2.Text = Global.GetXElementAttributeStr(xelement, "Description");
		}
		xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "GiftList"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 81.0;
			gicon.Height = 21.0;
			gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
			gicon.Text = Global.GetLang("领取奖励");
			gicon.ItemCode = i + 1;
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!(s as GIcon).EnableIcon)
				{
					return;
				}
				this.GetAward((s as GIcon).ItemCode);
			};
			xelement = xelementList[i];
			CampaignGiftListItem campaignGiftListItem = U3DUtils.NEW<CampaignGiftListItem>();
			campaignGiftListItem.BodyWidth = 514.0;
			campaignGiftListItem.BodyHeight = 101.0;
			campaignGiftListItem.HuoDongJieShao = Global.GetXElementAttributeStr(xelement, "Description");
			campaignGiftListItem.FrameBackground = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));
			campaignGiftListItem.GoodImgURL = "NetImages/GameRes/Images/Goods/411000.png";
			campaignGiftListItem.LingQuBtn = gicon;
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Description_1");
				if (!string.IsNullOrEmpty(xelementAttributeStr))
				{
					string[] array = xelementAttributeStr.Split(new char[]
					{
						'@'
					});
					if (array.Length > 0)
					{
						campaignGiftListItem.GoodName_01 = array[0] + ":";
						campaignGiftListItem.GoodIntro_01 = array[1];
					}
				}
			}
			if (xelement != null)
			{
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Description_2");
				if (!string.IsNullOrEmpty(xelementAttributeStr2))
				{
					string[] array2 = xelementAttributeStr2.Split(new char[]
					{
						'@'
					});
					if (array2.Length > 0)
					{
						campaignGiftListItem.GoodName_02 = array2[0] + ":";
						campaignGiftListItem.GoodIntro_02 = array2[1];
					}
				}
			}
			if (xelement != null)
			{
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
				if (!string.IsNullOrEmpty(xelementAttributeStr3))
				{
					string text = string.Empty;
					string[] array3 = xelementAttributeStr3.Split(new char[]
					{
						'|'
					});
					for (int j = 0; j < array3.Length; j++)
					{
						string[] array4 = array3[j].ToString().Split(new char[]
						{
							','
						});
						if (array4.Length == 5)
						{
							int[] array5 = Global.StringArray2IntArray(array4);
							if (Enumerable.Count<char>(text) > 0)
							{
								text += " ";
							}
							text += this.GetGoodsinfo(array5[0], array5[1], array5[2], array5[3], array5[4]);
						}
					}
					campaignGiftListItem.txtItem_2 = text;
				}
			}
			this.ItemCollection.AddNoUpdate(campaignGiftListItem);
		}
		this.ItemCollection.DelayUpdate();
		this.GetNewData();
	}

	private string GetGoodsinfo(int goodsID, int gcount, int quality, int forgeLevel, int binding)
	{
		string text = Global.GetGoodsNameByID(goodsID, false);
		if (forgeLevel > 0)
		{
			text = StringUtil.substitute("{0}[+{1}]x{2}", new object[]
			{
				text,
				forgeLevel,
				gcount
			});
		}
		else
		{
			text = StringUtil.substitute("{0}x{1}", new object[]
			{
				text,
				gcount
			});
		}
		return text;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
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
		GameInstance.Game.SpriteGetChongZhiJiFen();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.tc.Container.Children.Add(this.LoadingWin);
	}

	public void GetAwardResult(int result, int roleID)
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
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
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.JiFen.Text = jiFen.ToString();
	}

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private GTextBlockOutLine TitleText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine CampaignStartTimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine CampaignEndTimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine JiFen = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine otherCampaignText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine otherCampaignText2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public ObservableCollection ItemCollection;

	private GTabControl _tc;
}
