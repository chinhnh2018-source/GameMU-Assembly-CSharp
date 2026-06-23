using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class MeiriHaoliPart : UserControl
{
	public MeiriHaoliPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 465.0;
		this.listBox.Height = 48.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.listBox, 44);
		Canvas.SetTop(this.listBox, 71);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 28.0, 0.0);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Icon_LiJiLingQu = U3DUtils.NEW<GIcon>();
		this.Icon_LiJiLingQu.Width = 158.0;
		this.Icon_LiJiLingQu.Height = 69.0;
		this.Icon_LiJiLingQu.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiLingQu_normal2.png"));
		this.Icon_LiJiLingQu.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiLingQu_hover2.png"));
		this.Icon_LiJiLingQu.EnableHint = true;
		Canvas.SetLeft(this.Icon_LiJiLingQu, 185);
		Canvas.SetTop(this.Icon_LiJiLingQu, 188);
		this.Container.Children.Add(this.Icon_LiJiLingQu);
		this.Icon_LiJiLingQu.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnMouseButtonUp);
	}

	public void InitPartData()
	{
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("MeiriChongHaoliID");
		if (num <= 0)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
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
								ggoodIcon.Width = 48.0;
								ggoodIcon.Height = 48.0;
								ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCodeEx(Super.GetIconCode(goodsXmlNodeByID2)), false, 2);
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
								goodsPackItem.Width = 48.0;
								goodsPackItem.Height = 48.0;
								this.ItemCollection.AddNoUpdate(goodsPackItem);
							}
						}
						this.ItemCollection.DelayUpdate();
					}
				}
			}
		}
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	private void OnMouseButtonUp(object sender, MouseEvent e)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void RefreshUI(bool state, int result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，成功领取每日豪礼！欢迎您明天再来！"), new object[0]), 0, -1, -1, 0);
		}
	}

	private LoadingWindow LoadingWin;

	private ListBox listBox = new ListBox();

	private GIcon Icon_LiJiLingQu;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;
}
