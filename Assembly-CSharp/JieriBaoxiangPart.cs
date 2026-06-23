using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JieriBaoxiangPart : UserControl
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
		this.liheCanvas = new Canvas();
		this.liheCanvas.Width = 48.0;
		this.liheCanvas.Height = 48.0;
		this.Container.Children.Add(this.liheCanvas);
		Canvas.SetLeft(this.liheCanvas, 31);
		Canvas.SetTop(this.liheCanvas, 35);
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 320.0;
		this.listBox.Height = 129.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 7.0, 14.0);
		Canvas.SetLeft(this.listBox, 112);
		Canvas.SetTop(this.listBox, 19);
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.Children.Add(this.txtShengyuNum);
		Canvas.SetLeft(this.txtShengyuNum, 73);
		Canvas.SetTop(this.txtShengyuNum, 91);
		this.txtShengyuNum.TextColor = new SolidColorBrush(16777215U);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("兑换");
		gicon.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(gicon, 15);
		Canvas.SetTop(gicon, 115);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			if (this.IsInLingquTime)
			{
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
				GameInstance.Game.SpriteFetchActivityAward(14, 0);
				return;
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("不在领取时间内，无法兑换"), 0, -1, -1, 0);
		};
		this.BtnArr[0] = gicon;
	}

	public void InitPartData(List<XElement> xmlList)
	{
		this.xmlMall = Global.GetIsolateResXml("Config/Mall.Xml");
		this.MallItems = Global.GetXElementList(Global.GetXElement(this.xmlMall, "Mall"), "*");
		this.RefreshGoodsByType(Global.limitTabID);
		XElement xelement = xmlList[0];
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "DuiHuanGoodsIDs");
		string text = StringUtil.trim(xelementAttributeStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 6)
			{
				this.AddGoodsIcon(this.liheCanvas, int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), -1);
			}
		}
	}

	private void AddGoodsIcon(Canvas wrapper, int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData itemObject = null;
			string goodsImageURLFromIconCodeEx = Super.GetGoodsImageURLFromIconCodeEx(Super.GetIconCode(goodsXmlNodeByID));
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 48.0;
			gicon.Height = 48.0;
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCodeEx, false, 2);
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				default(object),
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = itemObject;
			gicon.ItemCategory = goodsXmlNodeByID.Categoriy;
			gicon.BoxTypes = -1;
			wrapper.Children.Add(gicon);
			Canvas.SetLeft(gicon, 0);
			Canvas.SetTop(gicon, 0);
		}
	}

	private void RefreshGoodsByType(int id)
	{
		this.ItemCollection.Clear();
		if (this.xmlMall == null)
		{
			return;
		}
		List<JieriMallItem> list = null;
		if (!this.TabItemsDict.ContainsKey(id))
		{
			ImageURL imageURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/sc_item.png"), false, 0);
			list = new List<JieriMallItem>();
			foreach (XElement xelement in this.MallItems)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "GoodsID");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "TabID");
				double num = (double)Global.GetXElementAttributeInt(xelement, "OrigPrice");
				double num2 = (double)Global.GetXElementAttributeInt(xelement, "Price");
				if (xelementAttributeInt3 == id)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(xelementAttributeInt2);
					GIcon gicon = U3DUtils.NEW<GIcon>();
					gicon.Width = 48.0;
					gicon.Height = 48.0;
					gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/64_Hover.png"));
					gicon.TipType = 1;
					gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
					{
						xelementAttributeInt2,
						0,
						-1,
						23
					});
					gicon.ItemCategory = 100 + id;
					gicon.ItemCode = xelementAttributeInt2;
					gicon.DisableHandCursor = true;
					Super.GetGoods64x64ImageFromFile(goodsXmlNodeByID.IconCode, gicon);
					GIcon gicon2 = U3DUtils.NEW<GIcon>();
					gicon2.Width = 66.0;
					gicon2.Height = 21.0;
					gicon2.BodySource = this._BodySource;
					gicon2.NewSource = this._NewSource;
					gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
					gicon2.Text = Global.GetLang("购 买");
					gicon2.Tip = Global.GetLang("点击购买");
					gicon2.ItemObject = xelementAttributeInt;
					gicon2.ItemCode = xelementAttributeInt2;
					gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						this.StartGouMai((s as GIcon).ItemCode, (s as GIcon).ItemObject as JieriMallItem, false);
					};
					JieriMallItem jieriMallItem = U3DUtils.NEW<JieriMallItem>();
					jieriMallItem.BodyWidth = 145.0;
					jieriMallItem.BodyHeight = 48.0;
					jieriMallItem.IconSource = gicon;
					jieriMallItem.MallGoodsID = xelementAttributeInt;
					jieriMallItem.GoodsPrice = num2.ToString();
					jieriMallItem.GoodsBuyBtn = gicon2;
					gicon2.ItemObject = jieriMallItem;
					list.Add(jieriMallItem);
				}
			}
			this.TabItemsDict[id] = list;
		}
		this.TabItemsDict.TryGetValue(id, ref list);
		this.ItemsList = list;
		if (this.ItemsList != null)
		{
			this.LoadGoods();
		}
	}

	public void LoadGoods()
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			this.ItemCollection.AddNoUpdate(this.ItemsList[i]);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void StartGouMai(int buyGoodsID, JieriMallItem jieriMallItem, bool isXianGou = false)
	{
		if (null == jieriMallItem)
		{
			return;
		}
		if (!Global.CanAddGoods(buyGoodsID, 1, 0, "1900-01-01 12:00:00", true))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买物品..."), new object[0]), 1, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.UserMoney - int.Parse(jieriMallItem.GoodsPrice) < 0)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		GameInstance.Game.SpriteMallBuy(jieriMallItem.MallGoodsID, 1, false);
	}

	public void GetData(bool isInLingquTime)
	{
		this.IsInLingquTime = isInLingquTime;
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteQueryZiKa(1);
	}

	public void OnGetDataCompleted(int result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result >= 0)
		{
			this.txtShengyuNum.Text = result.ToString();
		}
	}

	public void OnLingquCompleted(int result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功合成了节日礼盒"), 0, -1, -1, 0);
			this.txtShengyuNum.Text = result.ToString();
		}
	}

	private ListBox listBox = new ListBox();

	private Canvas liheCanvas;

	public GTextBlockOutLine txtShengyuNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GIcon[] BtnArr = new GIcon[0];

	private LoadingWindow LoadingWin;

	private bool IsInLingquTime;

	private XElement xmlMall;

	private List<XElement> MallItems;

	private List<JieriMallItem> ItemsList;

	private Dictionary<int, List<JieriMallItem>> TabItemsDict = new Dictionary<int, List<JieriMallItem>>();

	private ImageBrush _BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/scbtn21_normal.png"), 66.0, 21.0, 3.0, 2.0));

	private ImageBrush _NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/scbtn21_hover.png"), 66.0, 21.0, 3.0, 2.0));

	private ObservableCollection _ItemCollection;
}
