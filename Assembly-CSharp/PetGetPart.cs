using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class PetGetPart : UserControl
{
	public PetGetPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 130.0;
		this.listBox.Height = 90.0;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		Canvas.SetLeft(this.listBox, 21);
		Canvas.SetTop(this.listBox, 35);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.ScrollImg);
		this.ScrollImg.Width = 32.0;
		this.ScrollImg.Height = 32.0;
		Canvas.SetLeft(this.ScrollImg, 204);
		Canvas.SetTop(this.ScrollImg, 44);
	}

	public int CurrentPetCount
	{
		set
		{
			this._CurrentPetCount = value;
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
		this.PetGetIcon = U3DUtils.NEW<GIcon>();
		this.PetGetIcon.Width = 81.0;
		this.PetGetIcon.Height = 21.0;
		this.PetGetIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.PetGetIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.PetGetIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.PetGetIcon.Text = Global.GetLang("获取");
		this.PetGetIcon.TextColor = new SolidColorBrush(10551295U);
		this.PetGetIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.PetGetIcon.EnableIcon)
			{
				return;
			}
			this.PetGetOk();
		};
		Canvas.SetLeft(this.PetGetIcon, 179);
		Canvas.SetTop(this.PetGetIcon, 89);
		this.Container.Children.Add(this.PetGetIcon);
	}

	public void InitPartData()
	{
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("PetCardGoodsIDs", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			this.petIDS = Global.String2IntArray(systemParamByName, ',');
		}
		this.RefreshData();
		if (this._CurrentPetCount <= 0)
		{
			base.InitHintDecoration(50006, new Point((int)(this.PetGetIcon.X + this.PetGetIcon.ActualWidth), (int)(this.PetGetIcon.Y + this.PetGetIcon.ActualHeight / 2.0)), null);
		}
	}

	public void RefreshData()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.ShowPetsList();
	}

	private bool PetGetOk()
	{
		if (null == this.SelectedPetsListItem)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("开启【无量密藏】获取宠物卡后才能得到宠物!"), new object[0]), 0, -1, -1, 0);
			return false;
		}
		if (Global.Data.PetsDataList != null && Global.Data.PetsDataList.Count >= 3)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您最多可以携带3个宠物!"), new object[0]), 0, -1, -1, 0);
			this.PetGetIcon.EnableIcon = false;
			return false;
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
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】在冷却中, 无法使用"), new object[]
				{
					goodsNameByID
				}), 0, -1, -1, 0);
			}
		}
		this.ShowGoodsIcon(this.SelectedPetsListItem.HorseID);
		this.PetGetIcon.EnableIcon = true;
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
		return false;
	}

	private void ShowPetsList()
	{
		int oldSelectedIndex = 0;
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = this.listBox.SelectedIndex;
		}
		this.ItemCollection.Clear();
		if (this.petIDS != null)
		{
			for (int i = 0; i < this.petIDS.Length; i++)
			{
				if (Global.GetTotalGoodsCountByID(this.petIDS[i]) > 0)
				{
					MountsListItem mountsListItem = U3DUtils.NEW<MountsListItem>();
					mountsListItem.HorseID = this.petIDS[i];
					mountsListItem.BodyWidth = 120.0;
					mountsListItem.BodyHeight = 20.0;
					mountsListItem.TextColor = new SolidColorBrush(16777215U);
					mountsListItem.MountName.Text = Global.GetGoodsNameByID(this.petIDS[i], false);
					this.ItemCollection.AddNoUpdate(mountsListItem);
				}
			}
			this.ItemCollection.DelayUpdate();
		}
		this.SelectListBox(oldSelectedIndex);
		if (null == this.SelectedPetsListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.ShowGoodsIcon(this.SelectedPetsListItem.HorseID);
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
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
		this.ShowGoodsIcon(this.SelectedPetsListItem.HorseID);
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
			gicon.TextHorizontalAlignment = global::Layout.Center;
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
			this.ScrollImg.Children.Add(gicon);
		}
	}

	public override void Destroy()
	{
		this.ClearHintDecoration();
	}

	private ListBox listBox = new ListBox();

	private Canvas ScrollImg = new Canvas();

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private GIcon PetGetIcon;

	private int[] petIDS;

	private MountsListItem SelectedPetsListItem;

	private ImageBrush SelectedPetsListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 120.0, 20.0, 5.0, 5.0));

	private int _CurrentPetCount;

	private ObservableCollection _ItemCollection;
}
