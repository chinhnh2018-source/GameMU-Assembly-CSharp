using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class XinQuHuoDongJiangLiListItem : UserControl
{
	public XinQuHuoDongJiangLiListItem()
	{
		this.Container.Children.Add(this.txtTiaoJian);
		this.txtTiaoJian.BodyWidth = 120.0;
		this.txtTiaoJian.TextWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.txtTiaoJian, 0);
		this.txtTiaoJian.TextColor = new SolidColorBrush(16697088U);
		this.Container.Children.Add(this.txtJiangLi);
		this.txtJiangLi.BodyWidth = 200.0;
		this.txtJiangLi.TextWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.txtJiangLi, 130);
		this.txtJiangLi.TextColor = new SolidColorBrush(16697088U);
		this.Container.Children.Add(this.listBox);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 210.0;
		this.listBox.Height += 5.0;
		Canvas.SetLeft(this.listBox, 125);
		Canvas.SetTop(this.listBox, 4);
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.Children.Add(this.txtShuoMing);
		this.txtShuoMing.TextWrapping = TextWrapping.Wrap;
		this.txtShuoMing.BodyWidth = 110.0;
		Canvas.SetLeft(this.txtShuoMing, 335);
		this.txtShuoMing.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.btnCanvas);
		this.btnCanvas.Width = 81.0;
		this.btnCanvas.Height = 25.0;
		Canvas.SetLeft(this.btnCanvas, 458);
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

	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			if (value == 0.0)
			{
				value = this.listBox.ActualHeight + 8.0;
			}
			this.Container.Height = value;
			this.SetPos();
		}
	}

	public string TiaoJian
	{
		get
		{
			return this.txtTiaoJian.Text;
		}
		set
		{
			this.txtTiaoJian.Text = value;
		}
	}

	public string ShuoMing
	{
		get
		{
			return this.txtShuoMing.Text;
		}
		set
		{
			this.txtShuoMing.Text = value;
		}
	}

	public string JiangLi
	{
		get
		{
			return this.txtJiangLi.Text;
		}
		set
		{
			this.txtJiangLi.Text = value;
		}
	}

	public GIcon BtnLingQu
	{
		set
		{
			this.btnCanvas.Children.Add(value);
		}
	}

	public bool JiangLiLeiXing
	{
		get
		{
			return this._type;
		}
		set
		{
			this._type = value;
			this.txtJiangLi.Visibility = !this._type;
			this.listBox.Visibility = this._type;
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
		if (this.JiangLiLeiXing)
		{
			if (string.Empty == goodsList)
			{
				this.LoadShouChongDaLiGoodsList();
			}
			else
			{
				this.LoadOtherJiangLiGoodsList();
			}
		}
	}

	private void LoadShouChongDaLiGoodsList()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ShouChongDaLiID", ',');
		if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length < 3)
		{
			return;
		}
		int id = systemParamIntArrayByName[Global.Data.roleData.Occupation];
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 301 && categoriy <= 302)
			{
				int baoguoID = goodsXmlNodeByID.BaoguoID;
				if (baoguoID > 0)
				{
					Global.Data.ViewGoodsPackDataList = Global.UnPackGoodsID(baoguoID);
					if (Global.Data.ViewGoodsPackDataList != null)
					{
						for (int i = 0; i < Global.Data.ViewGoodsPackDataList.Count; i++)
						{
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.ViewGoodsPackDataList[i].GoodsID);
							if (goodsXmlNodeByID2 != null)
							{
								GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
								ggoodIcon.Width = 32.0;
								ggoodIcon.Height = 32.0;
								ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID2), string.Empty), false, 0);
								ggoodIcon.TipType = 1;
								ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
								{
									goodsXmlNodeByID2.ID,
									0,
									Global.Data.ViewGoodsPackDataList[i].Id,
									7
								});
								ggoodIcon.ItemCategory = goodsXmlNodeByID2.Categoriy;
								ggoodIcon.ItemCode = Global.Data.ViewGoodsPackDataList[i].GoodsID;
								ggoodIcon.ItemObject = Global.Data.ViewGoodsPackDataList[i];
								ggoodIcon.BoxTypes = -1;
								ggoodIcon.Text = ((Global.Data.ViewGoodsPackDataList[i].GCount <= 1) ? string.Empty : Global.Data.ViewGoodsPackDataList[i].GCount.ToString());
								ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
								ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
								ggoodIcon.TextShadowColor = 4278190080U;
								ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
								Super.InitGoodsGIcon(ggoodIcon, Global.Data.ViewGoodsPackDataList[i], true, IconTextTypes.Qianghua);
								GoodsPackItem goodsPackItem = U3DUtils.NEW<GoodsPackItem>();
								goodsPackItem.GoodsImgs = ggoodIcon;
								goodsPackItem.GoodsImgBacks = Global.GetGameResImage("Images/Plate/rm_listItem.png");
								this.ItemCollection.AddNoUpdate(goodsPackItem);
							}
						}
						this.ItemCollection.DelayUpdate();
					}
				}
			}
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
			GoodsData goodsData = Global.AddGiftGoodsData(goodsID, forgeLevel, quality, binding, gcount, born);
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
				goodsData.Id,
				12
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = gcount.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			GoodsPackItem goodsPackItem = U3DUtils.NEW<GoodsPackItem>();
			goodsPackItem.GoodsImgs = ggoodIcon;
			goodsPackItem.GoodsImgBacks = Global.GetGameResImage("Images/Plate/rm_listItem.png");
			this.ItemCollection.AddNoUpdate(goodsPackItem);
		}
	}

	private void SetPos()
	{
		Canvas.SetTop(this.txtTiaoJian, (int)((this.BodyHeight - this.txtTiaoJian.ActualHeight) / 2.0));
		Canvas.SetTop(this.txtJiangLi, (int)((this.BodyHeight - this.txtJiangLi.ActualHeight) / 2.0));
		Canvas.SetTop(this.txtShuoMing, (int)((this.BodyHeight - this.txtShuoMing.ActualHeight) / 2.0));
		Canvas.SetTop(this.btnCanvas, (int)((this.BodyHeight - this.btnCanvas.ActualHeight) / 2.0));
	}

	private GTextBlockOutLine txtTiaoJian = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtJiangLi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtShuoMing = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private Canvas btnCanvas = new Canvas();

	private ObservableCollection _ItemCollection;

	private bool _type;

	private string _GoodsList = string.Empty;
}
