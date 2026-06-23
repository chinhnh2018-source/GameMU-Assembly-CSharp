using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class ShouChongDaLiPart : UserControl
{
	public ShouChongDaLiPart()
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
		Canvas.SetTop(this.listBox, 111);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 28.0, 0.0);
		this.Container.Children.Add(this.hintText);
		this.hintText.TextColor = new SolidColorBrush(16764480U);
		Canvas.SetLeft(this.hintText, 75);
		Canvas.SetTop(this.hintText, 168);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Icon_LiJiChongChi = U3DUtils.NEW<GIcon>();
		this.Icon_LiJiChongChi.Width = 158.0;
		this.Icon_LiJiChongChi.Height = 69.0;
		this.Icon_LiJiChongChi.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiChongZhi_normal.png"));
		this.Icon_LiJiChongChi.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiChongZhi_hover.png"));
		this.Icon_LiJiChongChi.EnableHint = true;
		this.Icon_LiJiChongChi.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnMouseButtonUp);
		this.Icon_LiJiChongChi.Visibility = false;
		Canvas.SetLeft(this.Icon_LiJiChongChi, 185);
		Canvas.SetTop(this.Icon_LiJiChongChi, 188);
		this.Container.Children.Add(this.Icon_LiJiChongChi);
		this.Icon_LiJiLingQu = U3DUtils.NEW<GIcon>();
		this.Icon_LiJiLingQu.Width = 158.0;
		this.Icon_LiJiLingQu.Height = 69.0;
		this.Icon_LiJiLingQu.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiLingQu_normal2.png"));
		this.Icon_LiJiLingQu.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiLingQu_hover2.png"));
		this.Icon_LiJiLingQu.EnableHint = true;
		this.Icon_LiJiLingQu.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnMouseButtonUp);
		this.Icon_LiJiLingQu.Visibility = false;
		Canvas.SetLeft(this.Icon_LiJiLingQu, 185);
		Canvas.SetTop(this.Icon_LiJiLingQu, 188);
		this.Container.Children.Add(this.Icon_LiJiLingQu);
	}

	public void RefreshUI(bool isChongZhi, int ret)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (ret < 0)
		{
			return;
		}
		this.IsChongZhi = isChongZhi;
		if (this.IsChongZhi)
		{
			this.Icon_LiJiChongChi.Visibility = false;
			this.Icon_LiJiLingQu.Visibility = true;
		}
		else
		{
			this.Icon_LiJiChongChi.Visibility = true;
			this.Icon_LiJiLingQu.Visibility = false;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		}
	}

	public void InitPartData()
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
								if (i == 0)
								{
									this.Container.Children.Add(ggoodIcon);
									Canvas.SetLeft(ggoodIcon, 228);
									Canvas.SetTop(ggoodIcon, 23);
									this.Deco = Global.GetDecoration(549, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
									this.Deco.Coordinate = new Point(295, 45);
									Canvas.SetZIndex(this.Deco, 1.0);
								}
								else
								{
									GoodsPackItem goodsPackItem = U3DUtils.NEW<GoodsPackItem>();
									goodsPackItem.GoodsImgs = ggoodIcon;
									goodsPackItem.Width = 48.0;
									goodsPackItem.Height = 48.0;
									this.ItemCollection.AddNoUpdate(goodsPackItem);
								}
							}
						}
						this.ItemCollection.DelayUpdate();
					}
				}
			}
		}
		this.GetChongZhiInfo();
	}

	public void GetChongZhiInfo()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteQueryChongZhiMoney(Global.Data.roleData.RoleID);
	}

	private void OnMouseButtonUp(object sender, MouseEvent e)
	{
		if (this.IsChongZhi)
		{
			if (this.LoadingWin != null)
			{
				this.Container.Children.Remove(this.LoadingWin, true);
				this.LoadingWin.Destroy();
				this.LoadingWin = null;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteGetFirstChongZhiDaLi(Global.Data.roleData.RoleID);
		}
		else
		{
			Super.OpenChongZhiHtmlWindow();
		}
	}

	public void GetFirstChongZhiDaLiResult(int result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
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
		if (this.Deco != null)
		{
			this.Deco.Destroy();
		}
	}

	public void PauseAllEffect(bool pause)
	{
		if (this.Deco != null)
		{
			this.Deco.Pause = pause;
		}
	}

	private ListBox listBox = new ListBox();

	private GIcon Icon_LiJiChongChi;

	private GIcon Icon_LiJiLingQu;

	private GTextBlockOutLine hintText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private bool IsChongZhi;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;

	private LoadingWindow LoadingWin;

	private GDecoration Deco;
}
