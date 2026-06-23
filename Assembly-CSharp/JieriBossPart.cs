using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class JieriBossPart : UserControl
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
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 126.0;
		this.listBox.Height = 48.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 13.0, 0.0);
		Canvas.SetLeft(this.listBox, 256);
		Canvas.SetTop(this.listBox, 62);
		this.ItemCollection = this.listBox.ItemsSource;
	}

	public void InitPartData(List<XElement> xmlList)
	{
		XElement xelement = xmlList[0];
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
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
				this.AddGoodsIcon(i, int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), -1);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(int i, int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataEx = Global.GetDummyGoodsDataEx(goodsID, forgeLevel, quality, binding, gcount, born);
			string goodsImageURLFromIconCodeEx = Super.GetGoodsImageURLFromIconCodeEx(Super.GetIconCode(goodsXmlNodeByID));
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 48.0;
			ggoodIcon.Height = 48.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCodeEx, false, 2);
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

	public DPSelectedItemEventHandler DPSelectedItem;

	private ListBox listBox = new ListBox();

	private ObservableCollection _ItemCollection;
}
