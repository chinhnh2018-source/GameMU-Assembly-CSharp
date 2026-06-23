using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class WeiDuanHaoLiPart : UserControl
{
	public WeiDuanHaoLiPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 210.0;
		this.listBox.Height = 50.0;
		this.listBox.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.listBox, 40);
		Canvas.SetTop(this.listBox, 53);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 5.0, 5.0);
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
		this.Icon_LiJiXiaZai = U3DUtils.NEW<GIcon>();
		this.Icon_LiJiXiaZai.Width = 132.0;
		this.Icon_LiJiXiaZai.Height = 42.0;
		this.Icon_LiJiXiaZai.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiXiaZai_normal.png"));
		this.Icon_LiJiXiaZai.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiXiaZai_hover.png"));
		this.Icon_LiJiXiaZai.EnableHint = true;
		this.Icon_LiJiXiaZai.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnMouseButtonUp);
		this.Icon_LiJiXiaZai.Visibility = false;
		Canvas.SetLeft(this.Icon_LiJiXiaZai, 68);
		Canvas.SetTop(this.Icon_LiJiXiaZai, 102);
		this.Container.Children.Add(this.Icon_LiJiXiaZai);
		this.Icon_LiJiLingQu = U3DUtils.NEW<GIcon>();
		this.Icon_LiJiLingQu.Width = 132.0;
		this.Icon_LiJiLingQu.Height = 42.0;
		this.Icon_LiJiLingQu.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiLingQu_normal.png"));
		this.Icon_LiJiLingQu.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/liJiLingQu_hover.png"));
		this.Icon_LiJiLingQu.EnableHint = true;
		this.Icon_LiJiLingQu.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnMouseButtonUp);
		this.Icon_LiJiLingQu.Visibility = false;
		Canvas.SetLeft(this.Icon_LiJiLingQu, 68);
		Canvas.SetTop(this.Icon_LiJiLingQu, 102);
		this.Container.Children.Add(this.Icon_LiJiLingQu);
	}

	public void RefreshUI(bool isWeiDuan)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this._IsWeiDuan = isWeiDuan;
		if (this._IsWeiDuan)
		{
			this.Icon_LiJiXiaZai.Visibility = false;
			this.Icon_LiJiLingQu.Visibility = true;
		}
		else
		{
			this.Icon_LiJiXiaZai.Visibility = true;
			this.Icon_LiJiLingQu.Visibility = false;
		}
	}

	public void InitPartData()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TinyClientDaLiID", ',');
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
								this.ItemCollection.AddNoUpdate(goodsPackItem);
							}
						}
						this.ItemCollection.DelayUpdate();
					}
				}
			}
		}
		this.RefreshUI(this.IsWeiDuan());
	}

	private void OnMouseButtonUp(object sender, MouseEvent e)
	{
		if (this._IsWeiDuan)
		{
			if (this.LoadingWin != null)
			{
				this.Container.Children.Remove(this.LoadingWin, true);
				this.LoadingWin.Destroy();
				this.LoadingWin = null;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteFetchTinyClientAward();
		}
		else
		{
			Super.ExternalNavigateURL(Super.GetJSurl(6));
		}
	}

	public void OnTinyClientAwardGetCompleted(int result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("领取微端下载奖励成功"), 0, -1, -1, 0);
			this.Icon_LiJiXiaZai.Visibility = false;
			this.Icon_LiJiLingQu.Visibility = false;
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 1
				});
			}
		}
		else if (result == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("你已经领取过微端下载奖励了"), 0, -1, -1, 0);
		}
		else if (result == -20)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("你的背包已满，整理后再领取"), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取微端大礼时错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public bool IsWeiDuan()
	{
		string xapParamByName = Super.GetXapParamByName("isweiduan", string.Empty);
		return xapParamByName == "pc";
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

	private ListBox listBox = new ListBox();

	private GIcon Icon_LiJiXiaZai;

	private GIcon Icon_LiJiLingQu;

	private GTextBlockOutLine hintText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private bool _IsWeiDuan;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;

	private LoadingWindow LoadingWin;
}
