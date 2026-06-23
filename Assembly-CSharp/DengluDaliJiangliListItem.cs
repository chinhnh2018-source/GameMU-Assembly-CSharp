using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class DengluDaliJiangliListItem : UserControl
{
	public DengluDaliJiangliListItem()
	{
		this.Container.Children.Add(this.txtDayNum);
		this.txtDayNum.BodyWidth = 86.0;
		Canvas.SetLeft(this.txtDayNum, 0);
		Canvas.SetTop(this.txtDayNum, 15);
		this.txtDayNum.TextColor = new SolidColorBrush(16777080U);
		this.txtDayNum.fontBold = true;
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 165.0;
		this.listBox.Height = 42.0;
		Canvas.SetLeft(this.listBox, 80);
		Canvas.SetTop(this.listBox, 0);
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.Children.Add(this.btnCanvas);
		this.btnCanvas.Width = 50.0;
		this.btnCanvas.Height = 29.0;
		Canvas.SetLeft(this.btnCanvas, 250);
		Canvas.SetTop(this.btnCanvas, 11);
	}

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

	public string DayNum
	{
		get
		{
			return this.txtDayNum.Text;
		}
		set
		{
			this.txtDayNum.Text = value;
		}
	}

	public GIcon BtnLingQu
	{
		set
		{
			this.btnCanvas.Children.Add(value);
		}
	}

	public string GoodsList
	{
		get
		{
			return this._GoodsList;
		}
		set
		{
			this._GoodsList = value;
			this.LoadGoodsList(this.GoodsList);
		}
	}

	public uint TextColor
	{
		set
		{
			this.txtDayNum.TextColor = new SolidColorBrush(value);
		}
	}

	private void LoadGoodsList(string goodsList)
	{
		this.ItemCollection.Clear();
		if (!(string.Empty == goodsList))
		{
			this.LoadOtherJiangLiGoodsList();
		}
	}

	private void LoadOtherJiangLiGoodsList()
	{
		string text = StringUtil.trim(this._GoodsList);
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
				this.AddGoodsIcon(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[5]), -1);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataEx = Global.GetDummyGoodsDataEx(goodsID, forgeLevel, quality, binding, gcount, born);
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
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
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataEx, true, IconTextTypes.Qianghua);
			GoodsPackItem goodsPackItem = U3DUtils.NEW<GoodsPackItem>();
			goodsPackItem.GoodsImgs = ggoodIcon;
			goodsPackItem.GoodsImgBacks = Global.GetGameResImage("Images/Plate/rm_listItem.png");
			this.ItemCollection.AddNoUpdate(goodsPackItem);
		}
	}

	private GTextBlockOutLine txtDayNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private Canvas btnCanvas = new Canvas();

	private ObservableCollection _ItemCollection;

	private string _GoodsList = string.Empty;
}
