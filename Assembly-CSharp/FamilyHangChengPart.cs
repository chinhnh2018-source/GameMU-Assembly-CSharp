using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class FamilyHangChengPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.DiWangBufImg);
		this.DiWangBufImg.Width = 32.0;
		this.DiWangBufImg.Height = 32.0;
		Canvas.SetLeft(this.DiWangBufImg, 41);
		Canvas.SetTop(this.DiWangBufImg, 72);
		this.Container.Children.Add(this.BodyImage);
		Canvas.SetLeft(this.BodyImage, -127);
		Canvas.SetTop(this.BodyImage, -50);
		this.BodyImage.IsHitTestVisible = false;
		this.Container.Children.Add(this.WeaponImage);
		Canvas.SetLeft(this.WeaponImage, -127);
		Canvas.SetTop(this.WeaponImage, -50);
		Canvas.SetZIndex(this.WeaponImage, 1.0);
		this.WeaponImage.IsHitTestVisible = false;
		this.Container.Children.Add(this.txtHDName);
		this.txtHDName.TextColor = new SolidColorBrush(963281U);
		Canvas.SetLeft(this.txtHDName, 75);
		Canvas.SetTop(this.txtHDName, 48);
		this.Container.Children.Add(this.txtHDName2);
		this.txtHDName2.TextColor = new SolidColorBrush(963281U);
		Canvas.SetLeft(this.txtHDName2, 296);
		Canvas.SetTop(this.txtHDName2, 69);
		this.Container.Children.Add(this.txtZaiXingName);
		this.txtZaiXingName.TextColor = new SolidColorBrush(963281U);
		Canvas.SetLeft(this.txtZaiXingName, 490);
		Canvas.SetTop(this.txtZaiXingName, 77);
		this.Container.Children.Add(this.txtTaiShiName);
		this.txtTaiShiName.TextColor = new SolidColorBrush(963281U);
		Canvas.SetLeft(this.txtTaiShiName, 322);
		Canvas.SetTop(this.txtTaiShiName, 120);
		this.Container.Children.Add(this.txtShangShuName);
		this.txtShangShuName.TextColor = new SolidColorBrush(963281U);
		Canvas.SetLeft(this.txtShangShuName, 490);
		Canvas.SetTop(this.txtShangShuName, 120);
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
		gicon.Text = Global.GetLang("申请城战");
		Canvas.SetLeft(gicon, 2);
		Canvas.SetTop(gicon, 365);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IsHuangDi(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("【城主】自己不需要申请城战"), 0, -1, -1, 0);
				return;
			}
			if (Global.Data.roleData.BHZhiWu != 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能申请城战"), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SpriteCityWarRequest();
		};
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Width = 81.0;
		gicon2.Height = 21.0;
		gicon2.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon2.Text = Global.GetLang("扬州收税");
		Canvas.SetLeft(gicon2, 73);
		Canvas.SetTop(gicon2, 365);
		gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.IsHuangDi(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有本服【城主】才能查看扬州城税收"), 0, -1, -1, 0);
				return;
			}
			this.ShowYangZhouShouShuiWindow();
		};
		GIcon gicon3 = U3DUtils.NEW<GIcon>();
		gicon3.Width = 81.0;
		gicon3.Height = 21.0;
		gicon3.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon3.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon3.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon3.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon3.Text = Global.GetLang("领取王城奖励");
		Canvas.SetLeft(gicon3, 160);
		Canvas.SetTop(gicon3, 365);
		gicon3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.IsHuangDi(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有王族战盟首领才能领取"), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SpriteTakeLingDiDailyAward(6);
		};
		GIcon gicon4 = U3DUtils.NEW<GIcon>();
		gicon4.Width = 112.0;
		gicon4.Height = 21.0;
		gicon4.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 112.0, 21.0, 3.0, 2.0));
		gicon4.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 112.0, 21.0, 3.0, 2.0));
		gicon4.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 112.0, 21.0, 3.0, 2.0));
		gicon4.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon4.Text = Global.GetLang("攻城规则查询");
		Canvas.SetLeft(gicon4, 243);
		Canvas.SetTop(gicon4, 365);
		gicon4.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowchengZhanGzWindow();
		};
		GIcon gicon5 = U3DUtils.NEW<GIcon>();
		gicon5.Width = 81.0;
		gicon5.Height = 21.0;
		gicon5.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon5.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon5.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon5.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon5.Text = Global.GetLang("皇妃列表");
		Canvas.SetLeft(gicon5, 507);
		Canvas.SetTop(gicon5, 365);
		gicon5.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowHFlistWindow();
		};
	}

	public void InitPartData()
	{
	}

	public void GetNewData()
	{
		GameInstance.Game.SpriteGetHuangDiBHInfo();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (this.Deco != null)
		{
			Global.RemoveObject(this.Deco, true);
			this.Deco = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	private string GetIconID(int index, string[] IconIDs)
	{
		if (IconIDs == null)
		{
			return "0";
		}
		if (index < 0 || index >= IconIDs.Length)
		{
			return "0";
		}
		return IconIDs[index];
	}

	public void AddHuangDiIcon()
	{
		this.DiWangBufImg.Children.Clear();
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("BufferItemsGoodsIDs", ',');
		int num = Global.SafeConvertToInt32(this.GetIconID(14, systemParamStringArrayByName));
		string lang = Global.GetLang("乱世人人垂涎欲滴的超强能量之物,可为您加成：\n\n【基础属性】\n 物攻+10%\n 魔攻+10%\n 物防+10%\n 魔防+10%\n 生命上限+50%\n\n【特殊属性】\n 回血能力+200%\n 回魔能力+200%");
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 32.0;
		gicon.Height = 32.0;
		gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(num.ToString(), string.Empty), false, 0);
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
		gicon.TipType = 0;
		gicon.Tip = lang;
		gicon.ItemCode = 0;
		gicon.ItemObject = null;
		gicon.BoxTypes = -1;
		this.DiWangBufImg.Children.Add(gicon);
	}

	public void ClearAllDownloader()
	{
		foreach (Downloader downloader in this.downloaderDict.Keys)
		{
			Downloader downloader2 = downloader;
			downloader2.CancelRequest();
			downloader2.Completed = null;
		}
		this.downloaderDict.Clear();
	}

	public void DownLoaderComplete1(object sender, DownloadEventArgs e)
	{
		Downloader downloader = sender as Downloader;
		if (DownloadEventArgs.COMPLETE != e.type)
		{
			GError.AddErrMsg(StringUtil.substitute(Global.GetLang("下载失败, 原因:{0}"), new object[]
			{
				Global.GetErrorMsg(e)
			}));
		}
		else
		{
			Image image = null;
			if (this.downloaderDict.TryGetValue(downloader, ref image))
			{
				this.GetImageFromCaching(downloader.Args, image);
			}
		}
		downloader.Completed = null;
		this.downloaderDict.Remove(downloader);
	}

	public bool GetImageFromCaching(string key, Image image)
	{
		BitmapData netImageStream = Super.GetNetImageStream(key);
		if (netImageStream == null)
		{
			return false;
		}
		image.Source = new ImageBrush(netImageStream);
		return true;
	}

	public void DownloadNetImage(string value, Image image)
	{
		if (this.GetImageFromCaching(value, image))
		{
			return;
		}
		image.Source = null;
		Downloader downloader = new Downloader(null);
		downloader.Args = value;
		this.downloaderDict.Add(downloader, image);
		downloader.Completed = new DownloaderEventHander(this.DownLoaderComplete1);
		downloader.GetResourceByVer(Global.WebPath(value), Global.ResSwfVer, false);
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

	private void ShowYangZhouShouShuiWindow()
	{
		if (null != this.YangZhouShouShuiWindow)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.YangZhouShouShuiWindow);
			this.YangZhouShouShuiWindow = null;
			this.yangZhouShouShuiPart.CleanUpChildWindows();
			this.yangZhouShouShuiPart = null;
			return;
		}
		this.ShowModalDialog();
		this.YangZhouShouShuiWindow = U3DUtils.NEW<GChildWindow>();
		this.YangZhouShouShuiWindow.Left = (double)Super.GetChildLeft(648, 268);
		this.YangZhouShouShuiWindow.Top = (double)(Super.GetChildTop(471, 314) - 46);
		this.YangZhouShouShuiWindow.HeadLeft = 0.0;
		this.YangZhouShouShuiWindow.HeadTop = 0.0;
		this.YangZhouShouShuiWindow.HeadWidth = 268.0;
		this.YangZhouShouShuiWindow.HeadHeight = 46.0;
		this.YangZhouShouShuiWindow.BodyLeft = 0.0;
		this.YangZhouShouShuiWindow.BodyTop = 46.0;
		this.YangZhouShouShuiWindow.BodyWidth = 268.0;
		this.YangZhouShouShuiWindow.BodyHeight = 314.0;
		this.InitChildWindow1(this.YangZhouShouShuiWindow, Global.GetLang("扬州收税领取"));
		this.YangZhouShouShuiWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.YangZhouShouShuiWindow);
			this.YangZhouShouShuiWindow = null;
			this.yangZhouShouShuiPart.CleanUpChildWindows();
			this.yangZhouShouShuiPart = null;
			return true;
		};
		Canvas.SetZIndex(this.YangZhouShouShuiWindow, 9001.0);
		this.Container.Children.Add(this.YangZhouShouShuiWindow);
		this.yangZhouShouShuiPart = U3DUtils.NEW<YangZhouShouShuiPart>();
		this.yangZhouShouShuiPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_yzss_bak.png"), false, 0);
		this.yangZhouShouShuiPart.InitPartSize((int)this.YangZhouShouShuiWindow.BodyWidth - 18, (int)this.YangZhouShouShuiWindow.BodyHeight - 9);
		this.yangZhouShouShuiPart.InitPartData();
		this.YangZhouShouShuiWindow.SetContent(this.YangZhouShouShuiWindow.BodyPresenter, this.yangZhouShouShuiPart, 9.0, 0.0, true);
		this.yangZhouShouShuiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.YangZhouShouShuiWindow);
			this.YangZhouShouShuiWindow = null;
			this.yangZhouShouShuiPart.CleanUpChildWindows();
			this.yangZhouShouShuiPart = null;
		};
	}

	private void ShowchengZhanGzWindow()
	{
		if (null != this.ChengZhanGzWindow)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.ChengZhanGzWindow);
			this.ChengZhanGzWindow = null;
			this.chengZhanGzPart = null;
			return;
		}
		this.ShowModalDialog();
		this.ChengZhanGzWindow = U3DUtils.NEW<GChildWindow>();
		this.ChengZhanGzWindow.Left = (double)Super.GetChildLeft(630, 648);
		this.ChengZhanGzWindow.Top = (double)Super.GetChildTop(398, 529);
		this.ChengZhanGzWindow.HeadLeft = 0.0;
		this.ChengZhanGzWindow.HeadTop = 0.0;
		this.ChengZhanGzWindow.HeadWidth = 648.0;
		this.ChengZhanGzWindow.HeadHeight = 64.0;
		this.ChengZhanGzWindow.BodyLeft = 0.0;
		this.ChengZhanGzWindow.BodyTop = 64.0;
		this.ChengZhanGzWindow.BodyWidth = 648.0;
		this.ChengZhanGzWindow.BodyHeight = 407.0;
		this.InitChildWindow(this.ChengZhanGzWindow, Global.GetLang("攻城规则说明"));
		this.ChengZhanGzWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.ChengZhanGzWindow);
			this.ChengZhanGzWindow = null;
			this.chengZhanGzPart = null;
			return true;
		};
		Canvas.SetZIndex(this.ChengZhanGzWindow, 9001.0);
		this.Container.Children.Add(this.ChengZhanGzWindow);
		this.chengZhanGzPart = U3DUtils.NEW<ChengZhanGzPart>();
		this.chengZhanGzPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_czgz_bak.png"), false, 0);
		this.chengZhanGzPart.InitPartSize((int)this.ChengZhanGzWindow.BodyWidth - 18, (int)this.ChengZhanGzWindow.BodyHeight - 9);
		this.ChengZhanGzWindow.SetContent(this.ChengZhanGzWindow.BodyPresenter, this.chengZhanGzPart, 9.0, 0.0, true);
	}

	private void ShowHFlistWindow()
	{
		if (null != this.HFListWindow)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.HFListWindow);
			this.HFListWindow = null;
			this.hFListWindowPart.CleanUpChildWindows();
			this.hFListWindowPart = null;
			return;
		}
		this.ShowModalDialog();
		this.HFListWindow = U3DUtils.NEW<GChildWindow>();
		this.HFListWindow.Left = (double)Super.GetChildLeft((int)this.Container.Width, 308);
		this.HFListWindow.Top = (double)Super.GetChildTop((int)this.Container.Height, 365);
		this.HFListWindow.HeadLeft = 0.0;
		this.HFListWindow.HeadTop = 0.0;
		this.HFListWindow.HeadWidth = 308.0;
		this.HFListWindow.HeadHeight = 46.0;
		this.HFListWindow.BodyLeft = 0.0;
		this.HFListWindow.BodyTop = 46.0;
		this.HFListWindow.BodyWidth = 308.0;
		this.HFListWindow.BodyHeight = 302.0;
		this.InitChildWindow1(this.HFListWindow, Global.GetLang("皇妃列表"));
		this.HFListWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(this.HFListWindow);
			this.HFListWindow = null;
			this.hFListWindowPart.CleanUpChildWindows();
			this.hFListWindowPart = null;
			return true;
		};
		Canvas.SetZIndex(this.HFListWindow, 9001.0);
		this.Container.Children.Add(this.HFListWindow);
		this.hFListWindowPart = U3DUtils.NEW<HFListWindowPart>();
		this.hFListWindowPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/hflb_bak.png"), false, 0);
		this.hFListWindowPart.InitPartSize((int)this.HFListWindow.BodyWidth - 18, (int)this.HFListWindow.BodyHeight - 9);
		this.hFListWindowPart.InitPartData();
		this.HFListWindow.SetContent(this.HFListWindow.BodyPresenter, this.hFListWindowPart, 9.0, 0.0, true);
	}

	public void NotifyBangHuiDetailData(BangHuiDetailData bangHuiDetailData)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.WeaponImage.Source = null;
		this.BodyImage.Source = null;
		this.txtHDName.Text = string.Empty;
		this.txtHDName2.Text = string.Empty;
		this.txtZaiXingName.Text = string.Empty;
		this.txtTaiShiName.Text = string.Empty;
		this.txtShangShuName.Text = string.Empty;
		this.DiWangBufImg.Children.Clear();
		if (this.MyBangHuiDetailData == null)
		{
			return;
		}
		BangHuiMgrItemData bangHuiMgrItemDataByZhiWu = Global.GetBangHuiMgrItemDataByZhiWu(1, this.MyBangHuiDetailData);
		if (bangHuiMgrItemDataByZhiWu != null)
		{
			this.txtHDName.Text = Global.FormatRoleName(bangHuiMgrItemDataByZhiWu.ZoneID, bangHuiMgrItemDataByZhiWu.RoleName);
			this.txtHDName2.Text = this.txtHDName.Text;
			if (Global.Data.HuangDiRoleData == null || bangHuiMgrItemDataByZhiWu.RoleID != Global.Data.HuangDiRoleData.RoleID)
			{
				GameInstance.Game.SpriteGetHuangDiRoleData(bangHuiMgrItemDataByZhiWu.RoleID);
			}
			else
			{
				this.NotifyHuangDiRoleDataResult();
			}
		}
		bangHuiMgrItemDataByZhiWu = Global.GetBangHuiMgrItemDataByZhiWu(2, this.MyBangHuiDetailData);
		if (bangHuiMgrItemDataByZhiWu != null)
		{
			this.txtZaiXingName.Text = Global.FormatRoleName(bangHuiMgrItemDataByZhiWu.ZoneID, bangHuiMgrItemDataByZhiWu.RoleName);
		}
		bangHuiMgrItemDataByZhiWu = Global.GetBangHuiMgrItemDataByZhiWu(3, this.MyBangHuiDetailData);
		if (bangHuiMgrItemDataByZhiWu != null)
		{
			this.txtTaiShiName.Text = Global.FormatRoleName(bangHuiMgrItemDataByZhiWu.ZoneID, bangHuiMgrItemDataByZhiWu.RoleName);
		}
		bangHuiMgrItemDataByZhiWu = Global.GetBangHuiMgrItemDataByZhiWu(4, this.MyBangHuiDetailData);
		if (bangHuiMgrItemDataByZhiWu != null)
		{
			this.txtShangShuName.Text = Global.FormatRoleName(bangHuiMgrItemDataByZhiWu.ZoneID, bangHuiMgrItemDataByZhiWu.RoleName);
		}
	}

	public void NotifyBangHuiLingDiInfosDict(Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiInfosDict)
	{
		if (this.yangZhouShouShuiPart != null)
		{
			this.yangZhouShouShuiPart.NotifyBangHuiLingDiInfosDict(bangHuiLingDiInfosDict);
		}
	}

	public void NotifySetLingDiTaxResult(int retCode, int roleID, int bhid, int lingDiID, int newLingDiTax)
	{
		if (this.yangZhouShouShuiPart != null)
		{
			this.yangZhouShouShuiPart.NotifySetLingDiTaxResult(retCode, roleID, bhid, lingDiID, newLingDiTax);
		}
	}

	public void NotifyTakeTaxMoneyResult(int retCode, int roleID, int bhid, int lingDiID, int takeTaxMoney)
	{
		if (this.yangZhouShouShuiPart != null)
		{
			this.yangZhouShouShuiPart.NotifyTakeTaxMoneyResult(retCode, roleID, bhid, lingDiID, takeTaxMoney);
		}
	}

	public void NotifyHuangDiRoleDataResult()
	{
		object huangDiRoleData = Global.Data.HuangDiRoleData;
		object data = Global.Data;
		if (Global.Data.HuangDiRoleData.BodyCode >= 0)
		{
			if (this.RoleBodyEffect != null)
			{
				Global.RemoveObject(this.RoleBodyEffect, true);
				this.RoleBodyEffect = null;
			}
			int num = (Global.Data.HuangDiRoleData.BodyCode > 1) ? Global.Data.HuangDiRoleData.BodyCode : (10000000 + Global.Data.HuangDiRoleData.BodyCode);
			Canvas.SetZIndex(this.RoleBodyEffect, 10.0);
		}
		else if (this.RoleBodyEffect != null)
		{
			Global.RemoveObject(this.RoleBodyEffect, true);
			this.RoleBodyEffect = null;
		}
		if (Global.Data.HuangDiRoleData.WeaponCode > 0)
		{
			if (this.RoleWeaponEffect != null)
			{
				Global.RemoveObject(this.RoleWeaponEffect, true);
				this.RoleWeaponEffect = null;
			}
			int weaponCode = Global.Data.HuangDiRoleData.WeaponCode;
			Canvas.SetZIndex(this.RoleWeaponEffect, 10.0);
		}
		else if (this.RoleWeaponEffect != null)
		{
			Global.RemoveObject(this.RoleWeaponEffect, true);
			this.RoleWeaponEffect = null;
		}
	}

	public void NotifyHuangFeiDataList(List<SearchRoleData> huangFeiDataList)
	{
		if (null != this.hFListWindowPart)
		{
			this.hFListWindowPart.NotifyHuangFeiDataList(huangFeiDataList);
		}
	}

	public void NotifyRemoveHuangFeiResult(int retCode, int otherRoleID)
	{
		if (null != this.hFListWindowPart)
		{
			this.hFListWindowPart.NotifyRemoveHuangFeiResult(retCode, otherRoleID);
		}
	}

	private GDecoration Deco;

	private LoadingWindow LoadingWin;

	private Dictionary<Downloader, Image> downloaderDict = new Dictionary<Downloader, Image>();

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private YangZhouShouShuiPart yangZhouShouShuiPart;

	private GChildWindow YangZhouShouShuiWindow;

	private ChengZhanGzPart chengZhanGzPart;

	private GChildWindow ChengZhanGzWindow;

	private GChildWindow HFListWindow;

	private HFListWindowPart hFListWindowPart;

	public BangHuiDetailData MyBangHuiDetailData;

	private Canvas DiWangBufImg = new Canvas();

	private Image BodyImage = new Image();

	private Image WeaponImage = new Image();

	private GTextBlockOutLine txtHDName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtHDName2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtZaiXingName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTaiShiName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtShangShuName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GDecoration RoleBodyEffect;

	private GDecoration RoleWeaponEffect;
}
