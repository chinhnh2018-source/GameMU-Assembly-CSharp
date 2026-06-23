using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class ActivitiesGift : UserControl
{
	public ActivitiesGift()
	{
		this.ActivitiesInfoText.TextFontWrapping = TextWrapping.Wrap;
		this.ActiviesTargetText.TextFontWrapping = TextWrapping.Wrap;
		this.ActiviesSourceText.TextFontWrapping = TextWrapping.Wrap;
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
		this.TitleText.TextColor = new SolidColorBrush(10626862U);
		this.TitleText.HorizontalAlignment = global::Layout.Center;
		this.TitleText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.ActivitiesTimeText);
		Canvas.SetLeft(this.ActivitiesTimeText, 341);
		Canvas.SetTop(this.ActivitiesTimeText, 20);
		this.ActivitiesTimeText.TextColor = new SolidColorBrush(10626862U);
		this.ActivitiesTimeText.HorizontalAlignment = global::Layout.Center;
		this.ActivitiesTimeText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.ScrollViewer1);
		this.Panel.Background = new SolidColorBrush(16777215U);
		this.Panel.Orientation = global::Layout.Vertical;
		this.Panel.Width = this.ScrollViewer1.Width;
		this.ScrollViewer1.Width = 358.0;
		this.ScrollViewer1.Height = 140.0;
		Canvas.SetLeft(this.ScrollViewer1, 23);
		Canvas.SetTop(this.ScrollViewer1, 47);
		this.ScrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer1.Viewer = this.Panel;
		this.Panel.Children.Add(this.ActivitiesInfoText);
		Canvas.SetLeft(this.Panel, 3);
		this.ActivitiesInfoText.Text = string.Empty;
		this.ActivitiesInfoText.fontBold = true;
		this.ActivitiesInfoText.TextColor = new SolidColorBrush(16764416U);
		this.ActivitiesInfoText.BodyWidth = 330.0;
		this.ActivitiesInfoText.TextWidth = 330.0;
		this.Container.Children.Add(this.ActiviesTargetText);
		Canvas.SetLeft(this.ActiviesTargetText, 40);
		Canvas.SetTop(this.ActiviesTargetText, 250);
		this.ActiviesTargetText.TextColor = new SolidColorBrush(16764416U);
		this.ActiviesTargetText.HorizontalAlignment = global::Layout.Center;
		this.ActiviesTargetText.VerticalAlignment = global::Layout.Center;
		this.ActiviesTargetText.BodyWidth = 470.0;
		this.ActiviesTargetText.TextWidth = 470.0;
		this.Container.Children.Add(this.ActiviesSourceText);
		Canvas.SetLeft(this.ActiviesSourceText, 40);
		Canvas.SetTop(this.ActiviesSourceText, 300);
		this.ActiviesSourceText.TextColor = new SolidColorBrush(10626862U);
		this.ActiviesSourceText.HorizontalAlignment = global::Layout.Center;
		this.ActiviesSourceText.VerticalAlignment = global::Layout.Center;
		this.ActiviesSourceText.BodyWidth = 470.0;
		this.ActiviesSourceText.TextWidth = 470.0;
		this.ActiviesSourceText.TextClick = new UIEventEventHandler(this.TextClick);
		this.ScrollViewer1.ResetScrollView();
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		Image image = new Image();
		image.Width = 48.0;
		image.Height = 48.0;
		image.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/libao.png"));
		Canvas.SetLeft(image, 426);
		Canvas.SetTop(image, 58);
		this.Container.Children.Add(image);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取奖励");
		Canvas.SetLeft(gicon, 413);
		Canvas.SetTop(gicon, 157);
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
		Canvas.SetLeft(this.VerificationCodeGtext, 386);
		Canvas.SetTop(this.VerificationCodeGtext, 125);
		this.Container.Children.Add(this.VerificationCodeGtext);
	}

	private void GetAward(string liPinMa)
	{
		GameInstance.Game.SpriteGetSongLiGift(liPinMa);
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
		string text = "dl_android";
		XElement xelement = new XElement("config");
		XElement xelement2 = Global.GetIsolateResXml("Config/Gifts/MU_Activities.xml");
		foreach (XElement xelement3 in xelement2.Elements())
		{
			if (xelement3.Attribute("TypeID").Value.ToString() == text)
			{
				xelement.Add(xelement3);
				break;
			}
		}
		xelement2 = xelement;
		if (xelement2 == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement2, "Activities");
		if (xelementList == null)
		{
			return;
		}
		XElement xelement4 = xelementList[0];
		if (xelement4 != null)
		{
			this.ActivitiesTimeText.Text = Global.GetXElementAttributeStr(xelement4, "FromDate") + Global.GetLang("至") + Global.GetXElementAttributeStr(xelement4, "ToDate");
			this.TitleText.Text = Global.GetXElementAttributeStr(xelement4, "Title");
			if (Global.GetXElementAttributeStr(xelement4, "IsNeedCode") == "0")
			{
				this.VerificationCodeGtext.Visibility = false;
			}
		}
		xelement4 = Global.GetXElement(xelement2, "GiftList");
		this.ActivitiesInfoText.Text = Global.GetXElementAttributeStr(xelement4, "Description");
		xelementList = Global.GetXElementList(xelement4, "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			xelement4 = xelementList[i];
			ActivitiesGiftItem activitiesGiftItem = U3DUtils.NEW<ActivitiesGiftItem>();
			activitiesGiftItem.ActivitiesInfoText2.Text = Global.GetXElementAttributeStr(xelement4, "Description");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement4, "GoodsIDs");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				string text2 = string.Empty;
				string[] array = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
				for (int j = 0; j < array.Length; j++)
				{
					string[] array2 = array[j].ToString().Split(new char[]
					{
						','
					});
					if (array2.Length == 5)
					{
						int[] array3 = Global.StringArray2IntArray(array2);
						if (Enumerable.Count<char>(text2) > 0)
						{
							text2 += " ";
						}
						text2 += this.GetGoodsinfo(array3[0], array3[1], array3[2], array3[3], array3[4]);
					}
				}
				Super.FormatTextBlockEx2(activitiesGiftItem.GoodsInfoText, text2);
			}
			this.Panel.Children.Add(activitiesGiftItem);
		}
		this.ScrollViewer1.ResetScrollView();
		xelementList = Global.GetXElementList(xelement2, "ActiviTarget");
		xelement4 = xelementList[0];
		this.ActiviesTargetText.Text = Global.GetXElementAttributeStr(xelement4, "Description");
		xelementList = Global.GetXElementList(Global.GetXElement(xelement2, "LinkList"), "Item");
		if (xelementList != null)
		{
			string text3 = string.Empty;
			foreach (XElement xelement5 in xelementList)
			{
				text3 += Global.GetXElementAttributeStr(xelement5, "title");
				text3 += " ";
			}
			this.ActiviesSourceText.Text = text3;
			foreach (XElement xelement6 in xelementList)
			{
				this.ActiviesSourceText.SetSpecialText(Global.GetXElementAttributeStr(xelement6, "title"), new SolidColorBrush(4278222848U), true, Global.GetXElementAttributeStr(xelement6, "url"), true);
			}
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
		return StringUtil.substitute(Global.GetLang("｛color=#{0} uline=false tag= text={1}｝"), new object[]
		{
			Global.GetEnchanceStrColor((GoodsQuality)quality),
			text
		});
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

	private GTextBlock VerificationCodeGtext;

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private GTextBlockOutLine TitleText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine ActivitiesTimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GScrollView ScrollViewer1 = new GScrollView(0, 0, 0);

	private StackPanel Panel = new StackPanel();

	private GTextBlockOutLine ActivitiesInfoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine ActiviesTargetText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockEx ActiviesSourceText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private GTabControl _tc;
}
