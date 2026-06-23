using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class JieriKingListItem : UserControl
{
	public JieriKingListItem()
	{
		this.Container.Children.Add(this.txtRoleName);
		Canvas.SetLeft(this.txtRoleName, 71);
		Canvas.SetTop(this.txtRoleName, 10);
		this.txtRoleName.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtCondition);
		Canvas.SetLeft(this.txtCondition, 196);
		Canvas.SetTop(this.txtCondition, 10);
		this.txtCondition.TextColor = new SolidColorBrush(16762880U);
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 85.0;
		this.listBox.Height = 32.0;
		Canvas.SetLeft(this.listBox, 334);
		Canvas.SetTop(this.listBox, 0);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 8.0, 0.0);
		this.ItemCollection = this.listBox.ItemsSource;
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
				this.AddGoodsIcon(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), -1);
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

	public GTextBlockOutLine txtRoleName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtCondition = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private ObservableCollection _ItemCollection;

	private string _GoodsList = string.Empty;
}
