using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class PetsPart : UserControl
{
	public PetsPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 120.0;
		this.listBox.Height = 80.0;
		Canvas.SetLeft(this.listBox, 35);
		Canvas.SetTop(this.listBox, 56);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.Container.Children.Add(this.MountImg);
		Canvas.SetLeft(this.MountImg, 195);
		Canvas.SetTop(this.MountImg, 25);
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

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.GetIcon = U3DUtils.NEW<GIcon>();
		this.GetIcon.Width = 81.0;
		this.GetIcon.Height = 21.0;
		this.GetIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.GetIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.GetIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.GetIcon.Text = Global.GetLang("获取");
		this.GetIcon.TextColor = new SolidColorBrush(10551295U);
		this.GetIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.RemoveSystemNaviBox(this.Container, Global.GetLang("宠物窗口UI"), null);
			if (this.GetIcon.EnableIcon)
			{
				this.PetGet();
				return;
			}
		};
		Canvas.SetLeft(this.GetIcon, 322);
		Canvas.SetTop(this.GetIcon, 13);
		this.Container.Children.Add(this.GetIcon);
		this.SetOutPetIcon = U3DUtils.NEW<GIcon>();
		this.SetOutPetIcon.Width = 81.0;
		this.SetOutPetIcon.Height = 21.0;
		this.SetOutPetIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.SetOutPetIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.SetOutPetIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.SetOutPetIcon.Text = Global.GetLang("放出");
		this.SetOutPetIcon.TextColor = new SolidColorBrush(10551295U);
		this.SetOutPetIcon.TextHorizontalAlignment = global::Layout.Center;
		this.SetOutPetIcon.TextVerticalAlignment = global::Layout.Center;
		this.SetOutPetIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PetSetOut();
		};
		Canvas.SetLeft(this.SetOutPetIcon, 322);
		Canvas.SetTop(this.SetOutPetIcon, 48);
		this.Container.Children.Add(this.SetOutPetIcon);
		this.ReNameIcon = U3DUtils.NEW<GIcon>();
		this.ReNameIcon.Width = 81.0;
		this.ReNameIcon.Height = 21.0;
		this.ReNameIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.ReNameIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.ReNameIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.ReNameIcon.Text = Global.GetLang("改名");
		this.ReNameIcon.TextColor = new SolidColorBrush(10551295U);
		this.ReNameIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PetReName();
		};
		Canvas.SetLeft(this.ReNameIcon, 322);
		Canvas.SetTop(this.ReNameIcon, 83);
		this.Container.Children.Add(this.ReNameIcon);
		this.ThrowIcon = U3DUtils.NEW<GIcon>();
		this.ThrowIcon.Width = 81.0;
		this.ThrowIcon.Height = 21.0;
		this.ThrowIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		this.ThrowIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		this.ThrowIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		this.ThrowIcon.Text = Global.GetLang("遗弃");
		this.ThrowIcon.TextColor = new SolidColorBrush(10551295U);
		this.ThrowIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PetThrow();
		};
		Canvas.SetLeft(this.ThrowIcon, 322);
		Canvas.SetTop(this.ThrowIcon, 116);
		this.Container.Children.Add(this.ThrowIcon);
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
		if (this.ItemCollection.Count <= 0)
		{
			Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("宠物窗口UI"), 700000, 0, 1);
		}
	}

	public void AddPetData()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (Global.Data.PetsDataList != null && Global.Data.PetsDataList.Count > 0)
		{
			int num = Global.Data.PetsDataList.Count - 1;
			MountsListItem mountsListItem = U3DUtils.NEW<MountsListItem>();
			mountsListItem.DbID = Global.Data.PetsDataList[num].DbID;
			mountsListItem.HorseID = Global.Data.PetsDataList[num].PetID;
			mountsListItem.BodyWidth = 120.0;
			mountsListItem.BodyHeight = 20.0;
			mountsListItem.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 227, 147, 12));
			mountsListItem.MountName.Text = Global.Data.PetsDataList[num].PetName;
			this.ItemCollection.AddNoUpdate(mountsListItem);
		}
		this.ItemCollection.DelayUpdate();
		if (this.ItemCollection.Count > 0)
		{
			this.SelectListBox(this.ItemCollection.Count - 1);
			this.PetSetOut();
		}
	}

	public void ModPetData(bool removed, PetData petData)
	{
		if (removed)
		{
			this.RefreshData();
		}
		else
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				if (U3DUtils.AS<MountsListItem>(this.ItemCollection[i]).DbID == petData.DbID)
				{
					U3DUtils.AS<MountsListItem>(this.ItemCollection[i]).MountName.Text = petData.PetName;
					break;
				}
			}
			this.ShowPetDataInfo(petData, true);
		}
	}

	public void InitPartData()
	{
		this.UnSelectItem();
		if (this.ItemCollection.Count <= 0)
		{
			this.ShowPetDataInfo(null, false);
		}
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		GameInstance.Game.SpriteGetPetsList();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void CleanUpChildWindows()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("宠物窗口UI"), null);
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void OutInOver()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.SetOutPetIcon.Text = this.GetSetOutPetName();
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
		Super.CloseChildWindow(this.Container, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	public void PetGet()
	{
		if (null != this.PetGetWindow)
		{
			this.CloseChildWindow(this.PetGetWindow);
			this.PetGetWindow = null;
			this.petGetPart.Destroy();
			this.petGetPart = null;
			return;
		}
		this.ShowModalDialog();
		this.PetGetWindow = U3DUtils.NEW<GChildWindow>();
		this.PetGetWindow.Left = (double)Super.GetChildLeft((int)this.Container.Width, 308);
		this.PetGetWindow.Top = (double)Super.GetChildTop((int)this.Container.Height, 365);
		this.PetGetWindow.HeadLeft = 0.0;
		this.PetGetWindow.HeadTop = 0.0;
		this.PetGetWindow.HeadWidth = 308.0;
		this.PetGetWindow.HeadHeight = 46.0;
		this.PetGetWindow.BodyLeft = 0.0;
		this.PetGetWindow.BodyTop = 46.0;
		this.PetGetWindow.BodyWidth = 308.0;
		this.PetGetWindow.BodyHeight = 219.0;
		this.InitChildWindow1(this.PetGetWindow, Global.GetLang("获取宠物"));
		this.PetGetWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.PetGetWindow = null;
			this.petGetPart.Destroy();
			this.petGetPart = null;
			return true;
		};
		Canvas.SetZIndex(this.PetGetWindow, 9001.0);
		this.Container.Children.Add(this.PetGetWindow);
		this.petGetPart = U3DUtils.NEW<PetGetPart>();
		this.petGetPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/cwhq_bak.png"), false, 0);
		this.petGetPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0)
			{
				this.PetGetWindow.NotifyClose(0);
			}
		};
		this.petGetPart.CurrentPetCount = this.ItemCollection.Count;
		this.petGetPart.InitPartSize((int)this.PetGetWindow.BodyWidth - 18, (int)this.PetGetWindow.BodyHeight - 9);
		this.petGetPart.InitPartData();
		this.PetGetWindow.SetContent(this.PetGetWindow.BodyPresenter, this.petGetPart, 9.0, 0.0, true);
	}

	private void PetSetOut()
	{
		if (null == this.SelectedPetsListItem)
		{
			return;
		}
		if (Global.GetPetDataByDbID(this.SelectedPetsListItem.DbID) == null)
		{
			return;
		}
		if (Global.Data.roleData.PetDbID <= 0)
		{
			GameInstance.Game.SpritePet(1, this.SelectedPetsListItem.DbID);
		}
		else if (Global.Data.roleData.PetDbID == this.SelectedPetsListItem.DbID)
		{
			GameInstance.Game.SpritePet(2, 0);
		}
		else
		{
			GameInstance.Game.SpritePet(1, this.SelectedPetsListItem.DbID);
		}
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void PetReName()
	{
		if (null == this.SelectedPetsListItem)
		{
			return;
		}
		if (Global.GetPetDataByDbID(this.SelectedPetsListItem.DbID) == null)
		{
			return;
		}
		if (null != this.PetReNameWindow)
		{
			this.CloseChildWindow(this.PetReNameWindow);
			this.PetReNameWindow = null;
			return;
		}
		this.ShowModalDialog();
		this.PetReNameWindow = U3DUtils.NEW<GChildWindow>();
		this.PetReNameWindow.Left = (double)Super.GetChildLeft((int)this.Container.Width, 308);
		this.PetReNameWindow.Top = (double)Super.GetChildTop((int)this.Container.Height, 290);
		this.PetReNameWindow.HeadLeft = 0.0;
		this.PetReNameWindow.HeadTop = 0.0;
		this.PetReNameWindow.HeadWidth = 308.0;
		this.PetReNameWindow.HeadHeight = 46.0;
		this.PetReNameWindow.BodyLeft = 0.0;
		this.PetReNameWindow.BodyTop = 46.0;
		this.PetReNameWindow.BodyWidth = 308.0;
		this.PetReNameWindow.BodyHeight = 144.0;
		this.InitChildWindow1(this.PetReNameWindow, Global.GetLang("宠物改名"));
		this.PetReNameWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.PetReNameWindow = null;
			return true;
		};
		Canvas.SetZIndex(this.PetReNameWindow, 9001.0);
		this.Container.Children.Add(this.PetReNameWindow);
		PetReNamePart petReNamePart = U3DUtils.NEW<PetReNamePart>();
		petReNamePart.PetDbID = this.SelectedPetsListItem.DbID;
		petReNamePart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/cwgm_bak.png"), false, 0);
		petReNamePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0)
			{
				this.PetReNameWindow.NotifyClose(0);
			}
		};
		petReNamePart.InitPartSize((int)this.PetReNameWindow.BodyWidth - 18, (int)this.PetReNameWindow.BodyHeight - 9);
		petReNamePart.InitPartData();
		this.PetReNameWindow.SetContent(this.PetReNameWindow.BodyPresenter, petReNamePart, 9.0, 0.0, true);
	}

	private void PetThrow()
	{
		if (null == this.SelectedPetsListItem)
		{
			return;
		}
		if (Global.Data.roleData.PetDbID == this.SelectedPetsListItem.DbID)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("放出的宠物无法遗弃, 请收回后后再试"), 0, -1, -1, 0);
			return;
		}
		string text = this.SelectedPetsListItem.MountName.Text;
		object petDataByDbID = Global.GetPetDataByDbID(this.SelectedPetsListItem.DbID);
		string message = StringUtil.substitute(Global.GetLang("确认要遗弃宠物：【{0}】？"), new object[]
		{
			text
		});
		GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), message, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(this.Container, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SpriteModPet(this.SelectedPetsListItem.DbID, 1, string.Empty);
			}
			return true;
		};
	}

	private GGoodIcon GetGoodsIcon(GoodsData goodsData, bool isDead)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), isDead, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				goodsData.Id,
				6
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.Text = ((goodsData.GCount <= 1) ? string.Empty : goodsData.GCount.ToString());
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			ggoodIcon.BoxTypes = 11;
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			return ggoodIcon;
		}
		return null;
	}

	private void ShowPetsList()
	{
		int oldSelectedIndex = 0;
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = this.listBox.SelectedIndex;
		}
		this.ItemCollection.Clear();
		if (Global.Data.PetsDataList != null && Global.Data.PetsDataList.Count > 0)
		{
			for (int i = 0; i < Global.Data.PetsDataList.Count; i++)
			{
				MountsListItem mountsListItem = U3DUtils.NEW<MountsListItem>();
				mountsListItem.DbID = Global.Data.PetsDataList[i].DbID;
				mountsListItem.HorseID = Global.Data.PetsDataList[i].PetID;
				mountsListItem.BodyWidth = 120.0;
				mountsListItem.BodyHeight = 20.0;
				mountsListItem.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 227, 147, 12));
				mountsListItem.MountName.Text = Global.Data.PetsDataList[i].PetName;
				this.ItemCollection.AddNoUpdate(mountsListItem);
			}
			this.ItemCollection.DelayUpdate();
		}
		else
		{
			this.ShowPetDataInfo(null, false);
		}
		this.SelectListBox(oldSelectedIndex);
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count - 1);
			int num = oldSelectedIndex;
			if (oldSelectedIndex < 0 && Global.Data.roleData.PetDbID != -1)
			{
				num = Global.GetPetDataIndexByDbID(Global.Data.roleData.PetDbID);
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

	private string GetSetOutPetName()
	{
		if (Global.Data.roleData.PetDbID <= 0)
		{
			return Global.GetLang("放出");
		}
		if (null != this.SelectedPetsListItem && Global.Data.roleData.PetDbID == this.SelectedPetsListItem.DbID)
		{
			return Global.GetLang("收回");
		}
		return Global.GetLang("放出");
	}

	private void UnSelectItem()
	{
		this.MountImg.Source = null;
		this.SelectedPetsListItem = null;
		this.SetOutPetIcon.Text = this.GetSetOutPetName();
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
		object petDataByDbID = Global.GetPetDataByDbID(this.SelectedPetsListItem.DbID);
		if (petDataByDbID == null)
		{
			this.UnSelectItem();
			return;
		}
		this.ShowPetDataInfo(petDataByDbID, true);
	}

	private void ShowPetDataInfo(object petData, bool state = true)
	{
		if (state)
		{
			this.ReNameIcon.EnableIcon = true;
			this.ThrowIcon.EnableIcon = true;
			this.SetOutPetIcon.EnableIcon = true;
			this.DownloadNetImage("NetImages/Pets/" + this.SelectedPetsListItem.HorseID.ToString() + ".png");
			this.SelectedPetsListItem.BodyBackground = this.SelectedPetsListItemBakImg;
		}
		else
		{
			this.ReNameIcon.EnableIcon = false;
			this.ThrowIcon.EnableIcon = false;
			this.SetOutPetIcon.EnableIcon = false;
		}
		this.SetOutPetIcon.Text = this.GetSetOutPetName();
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	public void DownLoaderComplete1(object sender, DownloadEventArgs e)
	{
		if (DownloadEventArgs.COMPLETE != e.type)
		{
			GError.AddErrMsg(StringUtil.substitute(Global.GetLang("下载失败, 原因:{0}"), new object[]
			{
				Global.GetErrorMsg(e)
			}));
		}
		else
		{
			Bitmap bitmap = e.target as Bitmap;
			Super.AddNetImageStream((sender as Downloader).Args, e.target as BitmapData);
			this.GetImageFromCaching((sender as Downloader).Args);
		}
		this.downloader.Completed = null;
		this.downloader = null;
	}

	public bool GetImageFromCaching(string key)
	{
		BitmapData netImageStream = Super.GetNetImageStream(key);
		if (netImageStream == null)
		{
			return false;
		}
		this.MountImg.Source = new ImageBrush(netImageStream);
		return true;
	}

	public void DownloadNetImage(string value)
	{
		if (this.GetImageFromCaching(value))
		{
			return;
		}
		this.MountImg.Source = null;
		if (this.downloader != null)
		{
			this.downloader.CancelRequest();
			this.downloader.Completed = null;
			this.downloader = null;
		}
		this.downloader = new Downloader(null);
		this.downloader.Args = value;
		this.downloader.Completed = new DownloaderEventHander(this.DownLoaderComplete1);
		this.downloader.GetResourceByVer(Global.WebPath(value), Global.ResSwfVer, false);
	}

	private ListBox listBox = new ListBox();

	private Image MountImg = new Image();

	private GIcon SetOutPetIcon;

	private GIcon GetIcon;

	private GIcon ReNameIcon;

	private GIcon ThrowIcon;

	public GChildWindow PetGetWindow;

	public GChildWindow PetReNameWindow;

	public GChildWindow PetStudySpeakWindow;

	private LoadingWindow LoadingWin;

	private bool FirstGetNewData = true;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private MountsListItem SelectedPetsListItem;

	private ImageBrush SelectedPetsListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 120.0, 20.0, 5.0, 5.0));

	private Canvas PlaceHolder;

	private Downloader downloader;

	private ObservableCollection _ItemCollection;

	private PetGetPart petGetPart;
}
