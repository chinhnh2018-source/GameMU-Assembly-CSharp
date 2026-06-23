using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class BaokuDaJiangPart : UserControl
{
	public BaokuDaJiangPart()
	{
		this.ItemCollection = this.BaoKuJiangLiList.ItemsSource;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.jinRuBaoKuIcon = U3DUtils.NEW<GIcon>();
		this.jinRuBaoKuIcon.Width = 80.0;
		this.jinRuBaoKuIcon.Height = 21.0;
		this.jinRuBaoKuIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.jinRuBaoKuIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.jinRuBaoKuIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.jinRuBaoKuIcon.Text = Global.GetLang("进入宝库");
		this.jinRuBaoKuIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.jinRuBaoKuIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 1
				});
			}
		};
		Canvas.SetLeft(this.jinRuBaoKuIcon, 261);
		Canvas.SetTop(this.jinRuBaoKuIcon, 333);
		this.Container.Children.Add(this.jinRuBaoKuIcon);
		this.Container.Children.Add(this.BaoKuJiangLiList);
		this.BaoKuJiangLiList.Background = new SolidColorBrush(16777215U);
		this.BaoKuJiangLiList.Width = 545.0;
		this.BaoKuJiangLiList.Height = 32.0;
		this.BaoKuJiangLiList.ItemMargin = new Thickness(0.0, 0.0, 45.0, 0.0);
		Canvas.SetLeft(this.BaoKuJiangLiList, 67);
		Canvas.SetTop(this.BaoKuJiangLiList, 42);
	}

	public void InitPartData()
	{
		this.LoadBaoKuDaJiang();
	}

	private void LoadBaoKuDaJiang()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("BaoKuJiangLi", '|');
		if (systemParamStringArrayByName.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length == 6)
			{
				this.AddGoodsIcon(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), -1);
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData goodsData = Global.AddBaoKuGoodsData(goodsID, forgeLevel, quality, binding, gcount, born);
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
				22
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
			this.ItemCollection.AddNoUpdate(ggoodIcon);
		}
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

	private ListBox BaoKuJiangLiList = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private GIcon jinRuBaoKuIcon;

	private ObservableCollection _ItemCollection;
}
