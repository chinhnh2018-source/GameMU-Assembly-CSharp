using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class GiftBagPart : UserControl
{
	public GiftBagPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.info1.Text = Global.GetLang("怒斩升级大礼包，保送您到80级！");
		this.info2.Text = Global.GetLang("每升5级均可在此领取奖励。");
		this.info3.Text = Global.GetLang("各级礼包的详细奖励列表：");
	}

	protected override void InitializeComponent()
	{
		this.Container2.Background = new SolidColorBrush(16777215U);
		this.Container2.HorizontalAlignment = global::Layout.Left;
		this.Container2.VerticalAlignment = global::Layout.Top;
		this.Container2.Orientation = global::Layout.Vertical;
		this.Container2.Children.Add(this.info1);
		this.info1.Text = Global.GetLang("怒斩升级大礼包，保送您到80级！");
		Canvas.SetLeft(this.info1, 11);
		this.info1.Width = 480.0;
		Canvas.SetTop(this.info1, 8);
		this.info1.Height = 23.0;
		this.info1.FontSize = HSTextField.defaultFontSize;
		this.info1.TextColor = new SolidColorBrush(10626862U);
		this.info1.TextLineHeight = 3.0;
		this.info2.TextLineHeight = 3.0;
		this.info3.TextLineHeight = 3.0;
		this.Container2.Children.Add(this.info2);
		this.info2.Text = Global.GetLang("每升5级均可在此领取奖励。");
		Canvas.SetLeft(this.info2, 11);
		this.info2.Width = 480.0;
		Canvas.SetTop(this.info2, 28);
		this.info2.Height = 23.0;
		this.info2.FontSize = HSTextField.defaultFontSize;
		this.info2.TextColor = new SolidColorBrush(1999025U);
		this.Container2.Children.Add(this.info3);
		this.info3.Text = Global.GetLang("各级礼包的详细奖励列表：");
		Canvas.SetLeft(this.info3, 11);
		this.info3.Width = 480.0;
		Canvas.SetTop(this.info3, 56);
		this.info3.Height = 23.0;
		this.info3.FontSize = HSTextField.defaultFontSize;
		this.info3.TextColor = new SolidColorBrush(13277735U);
		this.Container2.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 5);
		Canvas.SetTop(this.listBox, 84);
		this.listBox.Width = 476.0;
		this.listBox.Height = 288.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
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

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		this.scrollViewer1 = new GScrollView(514, 325, 0);
		Canvas.SetLeft(this.scrollViewer1, 25);
		Canvas.SetTop(this.scrollViewer1, 12);
		this.Container2.Width = this.scrollViewer1.Width;
		this.scrollViewer1.Viewer = this.Container2;
		this.scrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Visible;
		this.Container.Children.Add(this.scrollViewer1);
		this.scrollViewer1.ResetScrollView();
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		string text = string.Empty;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/UpLevelGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			text = this.GetToOccupaGoods(Global.GetXElementAttributeStr(xelement, "GoodsIDs"));
			if (!string.IsNullOrEmpty(text))
			{
				this.BuildDataList(text);
			}
		}
		this.scrollViewer1.ResetScrollView();
	}

	private void BuildDataList(string sBaoGuoid)
	{
		UpgradeBagItem upgradeBagItem = U3DUtils.NEW<UpgradeBagItem>();
		upgradeBagItem.BagLevel = this.add * 5 + Global.GetLang(" 级礼包");
		upgradeBagItem.IconListBoxBackground = this.LibaoBackImage;
		this.add++;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 48.0;
		gicon.Height = 48.0;
		gicon.BodySource = this.LibaoImage;
		gicon.DisableHandCursor = true;
		upgradeBagItem.BagBoxIcons = gicon;
		if (this.xmlGoodsPack == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(this.xmlGoodsPack, "Goods", "ID", sBaoGuoid);
		if (xelement == null)
		{
			return;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Item");
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			return;
		}
		string[] array = xelementAttributeStr.Split(new char[]
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
				upgradeBagItem.GoodsBoxBackground = gicon;
				upgradeBagItem.ListBoxIcons = this.Geticon(array3[0], array3[1], array3[2], array3[3], array3[4], array3[5]);
			}
		}
		this.ItemCollection.Add(upgradeBagItem);
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

	private string GetToOccupaGoods(string awardsStr)
	{
		string[] array = awardsStr.Split(new char[]
		{
			','
		});
		if (array.Length == 0)
		{
			return string.Empty;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(array[i]));
			if (goodsXmlNodeByID != null && (1 << Global.Data.roleData.Occupation & goodsXmlNodeByID.ToOccupation) != 0)
			{
				return goodsXmlNodeByID.BaoguoID.ToString();
			}
		}
		return string.Empty;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container2);
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
		this.scrollViewer1.ResetScrollView();
	}

	private StackPanel Container2 = new StackPanel();

	private GTextBlockOutLine info1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info3 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private GScrollView scrollViewer1;

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private ImageBrush LibaoBackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/xtsl_rec2.png"));

	private ImageBrush LibaoImage = new ImageBrush(Global.GetGameResImage("Images/Plate/libao.png"));

	private ImageBrush GoodsBackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/xtsl_rec1.png"));

	private XElement xmlGoodsPack = Global.GetGameResXml("Config/GoodsPack.Xml");

	private int add = 1;

	private bool FirstGetNewData = true;

	private ObservableCollection ItemCollection;

	private GTabControl _tc;
}
