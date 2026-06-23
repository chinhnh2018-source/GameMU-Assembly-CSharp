using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class ShengJiLiPart : UserControl
{
	public ShengJiLiPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.info1);
		this.info1.Text = Global.GetLang("《怒斩》升级大礼包，保送您顺利升到80级！");
		Canvas.SetLeft(this.info1, 18);
		this.info1.Width = 480.0;
		Canvas.SetTop(this.info1, 18);
		this.info1.Height = 23.0;
		this.info1.TextColor = new SolidColorBrush(4294967040U);
		this.info1.fontBold = true;
		this.Container.Children.Add(this.info2);
		this.info2.Text = Global.GetLang("您每升5级，即可领取一份超级大礼包！");
		Canvas.SetLeft(this.info2, 18);
		Canvas.SetTop(this.info2, 49);
		this.info2.Width = 480.0;
		this.info2.BodyWidth = 480.0;
		this.info2.Height = 40.0;
		this.info2.TextFontWrapping = TextWrapping.Wrap;
		this.info2.FontSize = HSTextField.defaultFontSize;
		this.info2.TextColor = new SolidColorBrush(4894923U);
		this.Container.Children.Add(this.info3);
		this.info3.Text = Global.GetLang("各级礼包详细奖励列表！");
		Canvas.SetLeft(this.info3, 18);
		this.info3.Width = 480.0;
		Canvas.SetTop(this.info3, 127);
		this.info3.Height = 23.0;
		this.info3.TextColor = new SolidColorBrush(4294967040U);
		this.info3.fontBold = true;
		this.Container.Children.Add(this.timeGtext);
		this.timeGtext.Text = Global.GetLang("活动时间：【永久】");
		Canvas.SetLeft(this.timeGtext, 380);
		this.timeGtext.Width = 480.0;
		Canvas.SetTop(this.timeGtext, 127);
		this.timeGtext.Height = 23.0;
		this.timeGtext.TextColor = new SolidColorBrush(4294967040U);
		this.timeGtext.fontBold = true;
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 15);
		Canvas.SetTop(this.listBox, 165);
		this.listBox.Width = 570.0;
		this.listBox.Height = 288.0;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		string text = string.Empty;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/UpLevelGift.xml");
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
		this.ItemCollection.DelayUpdate();
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
				upgradeBagItem.ListBoxIcons = this.Geticon(array3[0], array3[1], array3[3], array3[4], array3[2], array3[5]);
			}
		}
		this.ItemCollection.AddNoUpdate(upgradeBagItem);
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

	private string GetGoodsinfo(int goodsID, int gcount, int quality, int forgeLevel, int binding)
	{
		string text = Global.GetGoodsNameByID(goodsID, false);
		if (forgeLevel > 0)
		{
			text = StringUtil.substitute("{0}(+{1})x{2}", new object[]
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
	}

	public ObservableCollection ItemCollection;

	private LoadingWindow LoadingWin;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private GTextBlockOutLine info1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine info3 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine timeGtext = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private int add = 1;

	private ImageBrush LibaoBackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/xtsl_rec2.png"));

	private ImageBrush LibaoImage = new ImageBrush(Global.GetGameResImage("Images/Plate/libao.png"));

	private ImageBrush GoodsBackImage = new ImageBrush(Global.GetGameResImage("Images/Plate/xtsl_rec1.png"));

	private XElement xmlGoodsPack = Global.GetGameResXml("Config/GoodsPack.Xml");
}
