using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class PetAddBagNumsPart : UserControl
{
	public PetAddBagNumsPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 130.0;
		this.listBox.Height = 90.0;
		Canvas.SetLeft(this.listBox, 22);
		Canvas.SetTop(this.listBox, 36);
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.ScrollImg);
		this.ScrollImg.Width = 32.0;
		this.ScrollImg.Height = 32.0;
		Canvas.SetLeft(this.ScrollImg, 200);
		Canvas.SetTop(this.ScrollImg, 51);
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

	public ImageBrush BodyBackground
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
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("扩展");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (null == this.SelectedPetsListItem)
			{
				return;
			}
			if (Global.GetTotalGoodsCountByID(this.SelectedPetsListItem.HorseID) <= 0)
			{
				Super.ShowMessageBox(this.Container, 0, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("您背包里没有扩展随身仓库的道具, 请到商城购买"), new object[0]), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				return;
			}
			int portableBagCapacity = Global.GetPortableBagCapacity();
			if (portableBagCapacity >= Global.MaxPortableGridNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("随身仓库最大格子数为{0}, 你的最大格子数已满"), new object[]
				{
					Global.MaxPortableGridNum
				}), 0, -1, -1, 0);
				return;
			}
			int num = 1;
			if ((int)this.SelectedPetsListItem.Tag == 1)
			{
				num = 5;
			}
			else if ((int)this.SelectedPetsListItem.Tag == 2)
			{
				num = 10;
			}
			if (portableBagCapacity + num > Global.MaxPortableGridNum)
			{
				if (Global.MaxPortableGridNum - portableBagCapacity >= 5)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("随身仓库最大格子数为{0}, 请使用【5格仓库栏】来扩充"), new object[]
					{
						Global.MaxPortableGridNum
					}), 0, -1, -1, 0);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("随身仓库最大格子数为{0}, 请使用【1格仓库栏】来扩充"), new object[]
					{
						Global.MaxPortableGridNum
					}), 0, -1, -1, 0);
				}
				return;
			}
			GoodsData goodsDataByID = Global.GetGoodsDataByID(this.SelectedPetsListItem.HorseID);
			if (goodsDataByID != null)
			{
				if (!Global.GoodsCoolDown(goodsDataByID.GoodsID))
				{
					GameInstance.Game.SpriteUseGoods(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
				}
				else
				{
					string goodsNameByID = Global.GetGoodsNameByID(goodsDataByID.GoodsID, false);
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】 在冷却中, 无法使用"), new object[]
					{
						goodsNameByID
					}), 0, -1, -1, 0);
				}
			}
		};
		Canvas.SetLeft(gicon, 179);
		Canvas.SetTop(gicon, 90);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		this.bagIDS = ConfigSystemParam.GetSystemParamIntArrayByName("PetAddGridGoodsIDs", ',');
		this.RefreshData(0);
	}

	private void ShowGoodsIcon(int goodsID, int subNum)
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
			gicon.Text = (Global.GetTotalGoodsCountByID(goodsID) - subNum).ToString();
			gicon.TextHorizontalAlignment = global::Layout.Center;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			if (Global.GetTotalGoodsCountByID(goodsID) - subNum <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			this.ScrollImg.Children.Add(gicon);
		}
	}

	private void UnSelectItem()
	{
		this.SelectedPetsListItem = null;
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedPetsListItem)
		{
			this.SelectedPetsListItem.BodyBackground = null;
		}
		this.SelectedPetsListItem = U3DUtils.AS<MountsListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedPetsListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedPetsListItem.BodyBackground = this.SelectedPetsListItemBakImg;
		this.ShowGoodsIcon(this.SelectedPetsListItem.HorseID, 0);
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count - 1);
			int num = oldSelectedIndex;
			if (oldSelectedIndex >= 0 || Global.Data.roleData.PetDbID != -1)
			{
			}
			if (num < 0)
			{
				num = 0;
			}
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	private void ShowOpenBagsToolsList(int subNum)
	{
		int oldSelectedIndex = 0;
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = this.listBox.SelectedIndex;
		}
		this.ItemCollection.Clear();
		for (int i = 0; i < this.bagIDS.Length; i++)
		{
			if (Global.GetTotalGoodsCountByID(this.bagIDS[i]) - subNum > 0)
			{
				MountsListItem mountsListItem = U3DUtils.NEW<MountsListItem>();
				mountsListItem.HorseID = this.bagIDS[i];
				mountsListItem.BodyWidth = 120.0;
				mountsListItem.BodyHeight = 20.0;
				mountsListItem.Tag = i;
				mountsListItem.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 227, 147, 12));
				mountsListItem.MountName.Text = Global.GetGoodsNameByID(this.bagIDS[i], false);
				this.ItemCollection.AddNoUpdate(mountsListItem);
			}
			this.ItemCollection.DelayUpdate();
		}
		this.SelectListBox(oldSelectedIndex);
		if (null != this.SelectedPetsListItem)
		{
			this.ShowGoodsIcon(this.SelectedPetsListItem.HorseID, subNum);
		}
	}

	public void RefreshData(int subNum = 0)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.ShowOpenBagsToolsList(subNum);
	}

	private ListBox listBox = new ListBox();

	private Canvas ScrollImg = new Canvas();

	private LoadingWindow LoadingWin;

	private int[] bagIDS;

	private MountsListItem SelectedPetsListItem;

	private ImageBrush SelectedPetsListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 120.0, 20.0, 5.0, 5.0));

	private ObservableCollection _ItemCollection;
}
