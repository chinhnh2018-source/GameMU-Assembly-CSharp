using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class DoubleExperienPart : UserControl
{
	public DoubleExperienPart()
	{
		this.thisCtrl = this;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 37.0;
		this.Equip.Height = 37.0;
		this.Equip.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Equip, 54);
		Canvas.SetTop(this.Equip, 84);
		this.Equip.BorderThickness = 0;
	}

	public ObservableCollection[] equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
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
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("商城购买");
		Canvas.SetLeft(gicon, 25);
		Canvas.SetTop(gicon, 123);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("我再想想");
		Canvas.SetLeft(gicon, 196);
		Canvas.SetTop(gicon, 156);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 112.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取双倍经验");
		Canvas.SetLeft(gicon, 13);
		Canvas.SetTop(gicon, 156);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.DrawDouExs);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Margin = new Thickness(5.0, 5.0, 0.0, 0.0);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 50.0;
		Canvas.SetLeft(gtextBlockOutLine, 201);
		Canvas.SetTop(gtextBlockOutLine, 92);
		this.Container.Children.Add(gtextBlockOutLine);
		this.ExperText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Margin = new Thickness(5.0, 5.0, 0.0, 0.0);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Width = 30.0;
		Canvas.SetLeft(gtextBlockOutLine, 201);
		Canvas.SetTop(gtextBlockOutLine, 117);
		this.Container.Children.Add(gtextBlockOutLine);
		this.ForceText = gtextBlockOutLine;
	}

	public void InitPartData(int doubleExper, int doubleInterPower)
	{
		this.ExperText.Text = doubleExper.ToString();
		this.ForceText.Text = doubleInterPower.ToString();
		int goodsID = (int)ConfigSystemParam.GetSystemParamIntByName("BigGuanLingPaiID");
		this.InitBiGuanPs(goodsID);
	}

	private void AddGoodsIcon(int goodsID, int index)
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
				goodsXmlNodeByID.ID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = -1;
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
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(gicon);
		}
	}

	private void InitBiGuanPs(int goodsID)
	{
		this.equipIcon = new ObservableCollection[1];
		this.equipIcon[0] = this.Equip.ItemsSource;
		this.equipIcon[0].Clear();
		this.AddGoodsIcon(goodsID, 0);
	}

	public void RefreshGoodsCount()
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[0][0]);
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

	private void DrawDouExs(object sender, MouseEvent e)
	{
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("BigGuanLingPaiID");
		if (Global.GetTotalGoodsCountByID(num) <= 0)
		{
			string goodsNameByID = Global.GetGoodsNameByID(num, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有【{0}】, 无法领取双倍经验和灵力"), new object[]
			{
				goodsNameByID
			}), 19, -1, -1, num);
			return;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3,
				IDType = 0
			});
		}
	}

	public override void Destroy()
	{
		this.Root.Children.Clear();
	}

	private UserControl thisCtrl;

	private GTextBlockOutLine ExperText;

	private GTextBlockOutLine ForceText;

	private Canvas Root;

	private ListBox Equip = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection[] _equipIcon;
}
