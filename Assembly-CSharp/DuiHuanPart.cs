using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class DuiHuanPart : UserControl
{
	public DuiHuanPart()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JingYuanExchange", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 2)
		{
			this.MinEquipLevel = systemParamIntArrayByName[0];
			this.ExperienceMulti = systemParamIntArrayByName[1];
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
		gicon.Text = Global.GetLang("兑 换");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ZhuangBeiDuiHuan();
		};
		Canvas.SetLeft(gicon, 90);
		Canvas.SetTop(gicon, 175);
		this.Container.Children.Add(gicon);
		this.Container.Children.Add(this.jingYuanText);
		this.jingYuanText.mouseEnabled = false;
		this.jingYuanText.Text = string.Empty;
		this.jingYuanText.TextColor = new SolidColorBrush(16777215U);
		this.jingYuanText.Width = 75.0;
		Canvas.SetLeft(this.jingYuanText, 116);
		Canvas.SetTop(this.jingYuanText, 146);
		this.Container.Children.Add(this.experienceText);
		this.experienceText.mouseEnabled = false;
		this.experienceText.Text = string.Empty;
		this.experienceText.TextColor = new SolidColorBrush(16777215U);
		this.experienceText.Width = 75.0;
		Canvas.SetLeft(this.experienceText, 116);
		Canvas.SetTop(this.experienceText, 122);
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
				if (goodsXmlNodeByID.ToLevel < this.MinEquipLevel)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("等级低于{0}的物品不能兑换"), new object[]
					{
						this.MinEquipLevel
					}), 0, -1, -1, 0);
					return;
				}
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy < 0 || categoriy > 25)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("非装备类物品不能兑换"), new object[]
					{
						this.MinEquipLevel
					}), 0, -1, -1, 0);
					return;
				}
				this.AddGoodsIcon(this.gd.GoodsID, this.gd.GCount, this.gd.Quality, this.gd.Forge_level, this.gd.AddPropIndex, this.gd.BornIndex, this.gd.Binding, true, this.gd.Id);
				this.SetCanExchangeJingYuanAndExpValue(this.gd);
			}
		}
	}

	private void ItemDroppedOnTarget(object sender, object e)
	{
	}

	private void ItemDragCompleted(object sender, object e)
	{
	}

	private void SetCanExchangeJingYuanAndExpValue(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		int changeJinYuan = goodsXmlNodeByID.ChangeJinYuan;
		int num = changeJinYuan * this.ExperienceMulti;
		int categoriy = goodsXmlNodeByID.Categoriy;
		this.jingYuanText.Text = string.Empty + changeJinYuan;
		this.experienceText.Text = string.Empty + num;
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int addPropIndex, int bornIndex, int binding, bool clear, int Id)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		this.experienceText.Text = string.Empty;
		this.jingYuanText.Text = string.Empty;
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
			this.experienceText.Text = (goodsXmlNodeByID.ChangeJinYuan * 10000).ToString();
			this.jingYuanText.Text = goodsXmlNodeByID.ChangeJinYuan.ToString();
		}
	}

	private void ZhuangBeiDuiHuan()
	{
		if (this.gd == null || this.ItemCollection.Count <= 0)
		{
			return;
		}
		GameInstance.Game.SpriteJingYuanExchange(this.gd.Id);
	}

	public void OnExchangeEnd(int result, int goodsDbID, int jingYuan, double experience)
	{
		if (result < 1)
		{
			if (result == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该装备不能兑换到任何精元和经验"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备等级不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精元置换时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			this.ItemCollection.Clear();
			this.experienceText.Text = string.Empty;
			this.jingYuanText.Text = string.Empty;
			this.gd = null;
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("精元置换成功"), new object[0]), 0, -1, -1, 0);
		}
	}

	private FixedListBoxDragDropTarget dragDropListTarget = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox list = new ListBox();

	private GTextBlockOutLine jingYuanText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine experienceText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ObservableCollection _ItemCollection;

	private GoodsData gd;

	private int MinEquipLevel = 40;

	private int ExperienceMulti = 10000;
}
