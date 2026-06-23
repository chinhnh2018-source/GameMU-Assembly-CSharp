using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class GoodsPackPart : UserControl
{
	public GoodsPackPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.PackName.Text = Global.GetLang("N级礼包");
		this.PackName.Text = Global.GetLang("可以领取");
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 242.0;
		this.listBox.Height = 152.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.listBox, 35);
		Canvas.SetTop(this.listBox, 17);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 5.0, 5.0);
		this.Container.Children.Add(this.PackName);
		this.PackName.TextColor = new SolidColorBrush(16764480U);
		this.PackName.Text = Global.GetLang("N级礼包");
		Canvas.SetLeft(this.PackName, 75);
		Canvas.SetTop(this.PackName, 168);
		this.Container.Children.Add(this.UseHint);
		this.UseHint.TextColor = new SolidColorBrush(65280U);
		this.UseHint.Text = Global.GetLang("可以领取");
		Canvas.SetLeft(this.UseHint, 171);
		Canvas.SetTop(this.UseHint, 168);
	}

	private string GetColorText(string text, string colorText = "FFFFFFFF", bool underLine = false)
	{
		return StringUtil.substitute(Global.GetLang("｛color=#{0} uline={1} tag= text={2}｝"), new object[]
		{
			colorText,
			(!underLine) ? "false" : "true",
			text
		});
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
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("解开礼包");
		Canvas.SetLeft(gicon, 113);
		Canvas.SetTop(gicon, 190);
		this.Container.Children.Add(gicon);
		this.UnpackIcon = gicon;
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.UnpackGoodsPackClick);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsID);
		this.PackName.Text = string.Empty;
		this.UseHint.Text = string.Empty;
		if (goodsXmlNodeByID != null)
		{
			this.PackName.Text = goodsXmlNodeByID.Title;
			int toLevel = goodsXmlNodeByID.ToLevel;
			if (toLevel > 0 && Global.Data.roleData.Level < toLevel)
			{
				this.UseHint.TextColor = new SolidColorBrush(4294901760U);
				this.UseHint.Text = Global.GetLang("级别不足");
				this.UnpackIcon.EnableIcon = false;
			}
			else
			{
				this.UseHint.Text = Global.GetLang("可以解包");
				string text = goodsXmlNodeByID.ToType.ToString();
				string text2 = goodsXmlNodeByID.ToTypeProperty.ToString();
				if ("-1" != text && text.Length > 0)
				{
					string[] array = text.Split(new char[]
					{
						','
					});
					string[] array2 = text2.Split(new char[]
					{
						','
					});
					if (array.Length == array2.Length)
					{
						for (int i = 0; i < array.Length; i++)
						{
							string text3 = array[i];
							string text4 = array2[i];
							if (StringUtil.isEqualIgnoreCase(text3, "UseYuanBao"))
							{
								string colorText = this.GetColorText(StringUtil.substitute(Global.GetLang("{0}钻石"), new object[]
								{
									text4
								}), "FF00FF00", true);
								string text5 = StringUtil.substitute(Global.GetLang("消耗{0}打开"), new object[]
								{
									colorText
								});
								this.UseHint.TextColor = new SolidColorBrush(uint.MaxValue);
								Super.FormatTextBlockEx2(this.UseHint, text5);
							}
						}
					}
				}
			}
		}
	}

	private void UnpackGoodsPackClick(object sender, MouseEvent e)
	{
		if (!(sender as GIcon).EnableIcon)
		{
			return;
		}
		GoodsData goodsDataByID = Global.GetGoodsDataByID(this.GoodsID);
		if (goodsDataByID != null)
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包中没有【{0}】, 无法使用"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
		}
	}

	public void InitPartData()
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.GoodsID);
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

	public DPSelectedItemEventHandler DPSelectedItem;

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine PackName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockEx UseHint = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private GIcon UnpackIcon;

	public ObservableCollection ItemCollection;

	private int _GoodsID;
}
