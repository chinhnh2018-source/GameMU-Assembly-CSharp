using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class DonateGoodsPart : UserControl
{
	public DonateGoodsPart()
	{
		this.ItemCollection1 = this.listBox1.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtName, 27);
		Canvas.SetTop(this.txtName, 15);
		this.Container.Children.Add(this.txtDonateValue);
		this.txtDonateValue.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtDonateValue, 158);
		Canvas.SetTop(this.txtDonateValue, 111);
		this.Container.Children.Add(this.listBox1);
		this.listBox1.Width = 240.0;
		this.listBox1.Height = 60.0;
		Canvas.SetLeft(this.listBox1, 12);
		Canvas.SetTop(this.listBox1, 62);
		this.listBox1.ItemMargin = new Thickness(7.0, 7.0, 6.0, 7.0);
		this.listBox1.Background = new SolidColorBrush(16777215U);
	}

	public ObservableCollection ItemCollection1
	{
		get
		{
			return this._ItemCollection1;
		}
		set
		{
			this._ItemCollection1 = value;
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
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("放入道具");
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		Canvas.SetLeft(gicon, 6);
		Canvas.SetTop(gicon, 157);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("确定");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiDetailData == null)
			{
				return;
			}
			if (this.MyBangHuiDetailData.BHID != Global.Data.roleData.Faction)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("非【{0}】战盟成员, 无法贡献道具!"), new object[]
				{
					Global.FormatBangHuiName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName)
				}), 0, -1, -1, 0);
				return;
			}
			if (this.ItemCollection1.Count <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请先放入捐赠道具!"), new object[0]), 0, -1, -1, 0);
				return;
			}
			List<int> list = this.CalcBangGongGoodsNum();
			GameInstance.Game.SpriteDonateBGGoods(list[0], list[1], list[2], list[3], list[4]);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 92);
		Canvas.SetTop(gicon, 157);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("取消");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 178);
		Canvas.SetTop(gicon, 157);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.BangHuiGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("BangHuiGoodsIDs", ',');
	}

	public void CleanUpChildWindows()
	{
		ObjectClickGetingMgr.CancelClickGetThing(8);
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InputEquipMouseLeftButtonUp(object sender, MouseEvent e)
	{
		(sender as GIcon).Cursor = Cursors.Auto;
		ObjectClickGetingMgr.StartClickGetThing(8, e);
	}

	private bool IsBangGongGoods(int goodsID)
	{
		if (this.BangHuiGoodsIDs == null)
		{
			return false;
		}
		for (int i = 0; i < this.BangHuiGoodsIDs.Length; i++)
		{
			if (this.BangHuiGoodsIDs[i] == goodsID)
			{
				return true;
			}
		}
		return false;
	}

	private int CalcBangBongGoodsNumByID(int goodsID)
	{
		int num = 0;
		for (int i = 0; i < this.ItemCollection1.Count; i++)
		{
			GoodsData goodsData = U3DUtils.AS<GIcon>(this.ItemCollection1[i]).ItemObject as GoodsData;
			if (goodsData.GoodsID == goodsID)
			{
				num += goodsData.GCount;
			}
		}
		return num;
	}

	private List<int> CalcBangGongGoodsNum()
	{
		List<int> list = new List<int>();
		if (this.BangHuiGoodsIDs == null)
		{
			return list;
		}
		for (int i = 0; i < this.BangHuiGoodsIDs.Length; i++)
		{
			list[i] = this.CalcBangBongGoodsNumByID(this.BangHuiGoodsIDs[i]);
		}
		return list;
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 8)
		{
			return;
		}
		int clickGetThingDbID = clickGetThingEventArgs.ClickGetThingDbID;
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(clickGetThingDbID, null);
		if (goodsDataByDbID == null)
		{
			return;
		}
		if (this.BangHuiGoodsIDs == null || this.BangHuiGoodsIDs.Length != 5)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("物品列表不存在!"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.ItemCollection1.Count >= 5)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("一次只能放入5个道具!"), new object[0]), 0, -1, -1, 0);
			return;
		}
		for (int i = 0; i < this.ItemCollection1.Count; i++)
		{
			if ((U3DUtils.AS<GIcon>(this.ItemCollection1[i]).ItemObject as GoodsData).Id == goodsDataByDbID.Id)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不能重复放入同一个格子的道具!"), new object[0]), 0, -1, -1, 0);
				return;
			}
		}
		if (!this.IsBangGongGoods(goodsDataByDbID.GoodsID))
		{
			string goodsNameByID = Global.GetGoodsNameByID(goodsDataByDbID.GoodsID, false);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】非捐赠道具, 无法放入!"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.Margin = new Thickness(10.0, 10.0, 0.0, 0.0);
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				goodsDataByDbID.Id,
				0
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsDataByDbID.GoodsID;
			ggoodIcon.ItemObject = goodsDataByDbID;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.TextSize = 12;
			Super.InitGoodsGIcon(ggoodIcon, goodsDataByDbID, true, IconTextTypes.Qianghua);
			this.ItemCollection1.Add(ggoodIcon);
		}
	}

	private BangHuiDetailData MyBangHuiDetailData;

	private int[] BangHuiGoodsIDs;

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtDonateValue = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox1 = new ListBox();

	private SpriteSL thisCtrl = new SpriteSL();

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection _ItemCollection1;
}
