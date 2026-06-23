using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class HunQiHuiShouPart : UserControl
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
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.dragDropListTarget);
		this.dragDropListTarget.AllowDrop = true;
		Canvas.SetLeft(this.dragDropListTarget, 113);
		Canvas.SetTop(this.dragDropListTarget, 47);
		Canvas.SetZIndex(this.dragDropListTarget, 1.0);
		this.dragDropListTarget.Children.Add(this.list);
		this.dragDropListTarget.Background = new SolidColorBrush(16777215U);
		this.list.Width = 32.0;
		this.list.Height = 32.0;
		this.list.Background = new SolidColorBrush(540994U);
		this.list.BackgroundAlpha = 0.01;
		this.dragDropListTarget.AllowDrop = true;
		this.dragDropListTarget.Drop = new EventHandler(this.DragDrop);
		this.dragDropListTarget.ItemDroppedOnTarget = new EventHandler(this.ItemDroppedOnTarget);
		this.dragDropListTarget.ItemDragCompleted = new EventHandler(this.ItemDragCompleted);
		this.ItemCollection = this.list.ItemsSource;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("回 收");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ZhuangBeiHuiShou();
		};
		Canvas.SetLeft(gicon, 90);
		Canvas.SetTop(gicon, 175);
		this.Container.Children.Add(gicon);
		this.Container.Children.Add(this.lieShaZhiText);
		this.lieShaZhiText.mouseEnabled = false;
		this.lieShaZhiText.Text = "0";
		this.lieShaZhiText.TextColor = new SolidColorBrush(16777215U);
		this.lieShaZhiText.Width = 75.0;
		Canvas.SetLeft(this.lieShaZhiText, 116);
		Canvas.SetTop(this.lieShaZhiText, 148);
		this.Container.Children.Add(this.lieShaZhiTitle);
		this.lieShaZhiTitle.mouseEnabled = false;
		this.lieShaZhiTitle.Text = Global.GetLang("可兑换值:");
		this.lieShaZhiTitle.TextColor = new SolidColorBrush(16777215U);
		this.lieShaZhiTitle.Width = 96.0;
		Canvas.SetLeft(this.lieShaZhiTitle, 50);
		Canvas.SetTop(this.lieShaZhiTitle, 148);
	}

	public void DragDrop(object sender, object e)
	{
		if (this.list != Super.GData.DragingListBox && null != Super.GData.DragingItem && Super.GData.DragingItem.BoxTypes == 1)
		{
			this.gd = (Super.GData.DragingItem.ItemObject as GoodsData);
			if (this.gd.Using <= 0)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.gd.GoodsID);
				if (goodsXmlNodeByID == null)
				{
					return;
				}
				int categoriy = goodsXmlNodeByID.Categoriy;
				int zhanHunPrice = goodsXmlNodeByID.ZhanHunPrice;
				if (zhanHunPrice <= 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该装备无法兑换到战魂值"), new object[0]), 0, -1, -1, 0);
					return;
				}
				this.AddGoodsIcon(this.gd.GoodsID, this.gd.GCount, this.gd.Quality, this.gd.Forge_level, this.gd.AddPropIndex, this.gd.BornIndex, this.gd.Binding, true, this.gd.Id);
				long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("HunqiExchange");
				this.lieShaZhiTitle.Text = Global.GetLang("可兑换值:");
				this.lieShaZhiText.Text = StringUtil.substitute("{0}-{1}", new object[]
				{
					(int)((double)((long)zhanHunPrice * systemParamIntByName) / 100.0),
					zhanHunPrice
				});
			}
		}
	}

	private void ItemDroppedOnTarget(object sender, object e)
	{
	}

	private void ItemDragCompleted(object sender, object e)
	{
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int addPropIndex, int bornIndex, int binding, bool clear = false, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(Id, null);
			goodsDataByDbID.Id = Id;
			this.ItemCollection.Clear();
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
				goodsDataByDbID.Id,
				20
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = goodsDataByDbID;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = gcount.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			Super.InitGoodsGIcon(ggoodIcon, goodsDataByDbID, true, IconTextTypes.Qianghua);
			this.ItemCollection.Add(ggoodIcon);
			this.LieShaPrice = goodsXmlNodeByID.ZhanHunPrice;
		}
	}

	private void ZhuangBeiHuiShou()
	{
		if (this.ItemCollection.Count <= 0 || this.gd == null)
		{
			return;
		}
		GameInstance.Game.SpriteHunqiExchange(this.gd.Id);
	}

	public void OnExchangeEnd(int result, int goodsDbID, int lieShaValue)
	{
		if (result < 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("魂器兑换时发生错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
		else
		{
			this.ItemCollection.Clear();
			this.lieShaZhiTitle.Text = Global.GetLang("已兑换值:");
			this.lieShaZhiText.Text = string.Empty + lieShaValue;
			this.gd = null;
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("魂器置换成功，得到{0}战魂值"), new object[]
			{
				lieShaValue
			}), 0, -1, -1, 0);
		}
	}

	private FixedListBoxDragDropTarget dragDropListTarget = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox list = new ListBox();

	private GTextBlockOutLine lieShaZhiText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine lieShaZhiTitle = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ObservableCollection _ItemCollection;

	private int LieShaPrice;

	private RandomAS rand = new RandomAS(0);

	private GoodsData gd;

	private Canvas aa = new Canvas();
}
