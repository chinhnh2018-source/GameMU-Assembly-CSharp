using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class AddTimesPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.ScrollImg);
		this.ScrollImg.Width = 32.0;
		this.ScrollImg.Height = 32.0;
		Canvas.SetLeft(this.ScrollImg, 38);
		Canvas.SetTop(this.ScrollImg, 44);
		this.Container.Children.Add(this.todayCXTimes);
		this.todayCXTimes.TextColor = new SolidColorBrush(16711680U);
		Canvas.SetLeft(this.todayCXTimes, 220);
		Canvas.SetTop(this.todayCXTimes, 42);
		this.Container.Children.Add(this.otherCXTimes);
		this.otherCXTimes.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.otherCXTimes, 220);
		Canvas.SetTop(this.otherCXTimes, 64);
	}

	public int JingMaiBodyLevel
	{
		get
		{
			return this._JingMaiBodyLevel;
		}
		set
		{
			this._JingMaiBodyLevel = value;
		}
	}

	public int JingMaiID
	{
		get
		{
			return this._JingMaiID;
		}
		set
		{
			this._JingMaiID = value;
		}
	}

	public int JingMaiLevel
	{
		get
		{
			return this._JingMaiLevel;
		}
		set
		{
			this._JingMaiLevel = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("增加次数");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.DisableTextColor = new SolidColorBrush(ColorSL.FromArgb(255, 40, 54, 63));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.AddTimesOk();
		};
		Canvas.SetLeft(gicon, 105);
		Canvas.SetTop(gicon, 123);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		this.todayCXTimes.Text = Global.TodayChongXueNum().ToString();
		this.otherCXTimes.Text = (Global.TodayChongXueNum() + 10).ToString();
		this.ShowGoodsIcon((int)ConfigSystemParam.GetSystemParamIntByName("DailyChongXueGoodsID"));
	}

	private void AddTimesOk()
	{
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("DailyChongXueGoodsID");
		GoodsData goodsDataByID = Global.GetGoodsDataByID(num);
		if (goodsDataByID != null)
		{
			if (!Global.GoodsCoolDown(goodsDataByID.GoodsID))
			{
				if (Global.GetCategoriyByGoodsID(goodsDataByID.GoodsID) == 704)
				{
					GameInstance.Game.SendTOUseTaLuopaiSuiPian(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
				}
				else
				{
					GameInstance.Game.SpriteUseGoods(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
				}
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						IDType = 0
					});
				}
			}
			else
			{
				string goodsNameByID = Global.GetGoodsNameByID(goodsDataByID.GoodsID, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】在冷却中, 无法使用"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			string goodsNameByID2 = Global.GetGoodsNameByID(num, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的背包中没有【{0}】, 无法增加每日冲穴次数"), new object[]
			{
				goodsNameByID2
			}), 2, -1, -1, 0);
		}
	}

	private void ShowGoodsIcon(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = Global.GetTotalGoodsCountByID(goodsID).ToString();
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			if (Global.GetTotalGoodsCountByID(goodsID) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			this.ScrollImg.Children.Clear();
			this.ScrollImg.Children.Add(gicon);
		}
	}

	public void RefreshGoodsCount()
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.ScrollImg.Children.getChildAt(1));
		if (null == gicon)
		{
			return;
		}
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(gicon.ItemCode), string.Empty);
		if (Global.GetTotalGoodsCountByID(gicon.ItemCode) <= 0)
		{
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
		}
		else
		{
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		gicon.Text = Global.GetTotalGoodsCountByID(gicon.ItemCode).ToString();
	}

	private Canvas Root;

	private Canvas ScrollImg = new Canvas();

	private GTextBlockOutLine todayCXTimes = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine otherCXTimes = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _JingMaiBodyLevel;

	private int _JingMaiID;

	private int _JingMaiLevel;
}
