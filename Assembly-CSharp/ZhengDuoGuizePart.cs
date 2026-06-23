using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ZhengDuoGuizePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.txtLingDiZhanXianLu.TextColor = new SolidColorBrush(16711680U);
		Canvas.SetLeft(this.txtLingDiZhanXianLu, 80);
		Canvas.SetTop(this.txtLingDiZhanXianLu, 318);
		string xapParamByName = Super.GetXapParamByName("country", string.Empty);
		if ("korea" != xapParamByName)
		{
			this.txtLingDiZhanXianLu.text = ConfigSystemParam.GetSystemParamByName("BangHuiFightingLineID", true);
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
		gicon.Text = Global.GetLang("战旗升级");
		Canvas.SetLeft(gicon, 172);
		Canvas.SetTop(gicon, 365);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangQiInfoData == null)
			{
				return;
			}
			if (this.MyBangQiInfoData.BangQiLevel >= Global.MaxBangHuiFlagLevel)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您的战盟战旗等级已经达到最高等级，无法再升级！"), 0, -1, -1, 0);
				return;
			}
			this.ShowFamilyFlayUpLeveWindow(false);
		};
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Width = 81.0;
		gicon2.Height = 21.0;
		gicon2.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon2.Text = Global.GetLang("战旗改名");
		Canvas.SetLeft(gicon2, 283);
		Canvas.SetTop(gicon2, 365);
		this.Container.Children.Add(gicon2);
		gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.IsBangHuiLeader(Global.Data.roleData, this.BHID))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能执行战旗改名操作!"), 0, -1, -1, 0);
				return;
			}
			this.ShowFamilyFlayRNWindow();
		};
		GIcon gicon3 = U3DUtils.NEW<GIcon>();
		gicon3.Width = 81.0;
		gicon3.Height = 21.0;
		gicon3.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon3.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon3.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon3.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon3.Text = Global.GetLang("战旗信息");
		Canvas.SetLeft(gicon3, 394);
		Canvas.SetTop(gicon3, 365);
		this.Container.Children.Add(gicon3);
		gicon3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowFamilyFlagWindow();
		};
	}

	public void GetNewData(int bhid)
	{
		this.BHID = bhid;
		GameInstance.Game.SpriteGetBangQiInfo(this.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void CloseFamilyFlayUpLeveWindow()
	{
		this.CloseModalDialog();
		this.CloseChildWindow(this.FamilyFlagUpLeveWindow);
		this.FamilyFlagUpLeveWindow = null;
		this.familyflagUpLevePart.Destroy();
		this.familyflagUpLevePart = null;
	}

	public void ShowFamilyFlayUpLeveWindow(bool caching = false)
	{
		if (this.MyBangQiInfoData.BangQiLevel >= Global.MaxBangHuiFlagLevel)
		{
			return;
		}
		if (null != this.FamilyFlagUpLeveWindow)
		{
			this.CloseFamilyFlayUpLeveWindow();
			return;
		}
		this.ShowModalDialog();
		this.FamilyFlagUpLeveWindow = U3DUtils.NEW<GChildWindow>();
		this.FamilyFlagUpLeveWindow.Left = (double)Super.GetChildLeft(640, 308);
		this.FamilyFlagUpLeveWindow.Top = (double)(Super.GetChildTop(471, 239) - 64);
		this.FamilyFlagUpLeveWindow.HeadLeft = 0.0;
		this.FamilyFlagUpLeveWindow.HeadTop = 0.0;
		this.FamilyFlagUpLeveWindow.HeadWidth = 308.0;
		this.FamilyFlagUpLeveWindow.HeadHeight = 46.0;
		this.FamilyFlagUpLeveWindow.BodyLeft = 0.0;
		this.FamilyFlagUpLeveWindow.BodyTop = 46.0;
		this.FamilyFlagUpLeveWindow.BodyWidth = 308.0;
		this.FamilyFlagUpLeveWindow.BodyHeight = 239.0;
		this.FamilyFlagUpLeveWindow.ImgTitle = "NetImages/GameRes/Images/WinTitle/BangqiShengji.png";
		this.InitChildWindow1(this.FamilyFlagUpLeveWindow, Global.GetLang("战旗升级"));
		this.FamilyFlagUpLeveWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseFamilyFlayUpLeveWindow();
			return true;
		};
		Canvas.SetZIndex(this.FamilyFlagUpLeveWindow, 9001.0);
		this.Container.Children.Add(this.FamilyFlagUpLeveWindow);
		this.familyflagUpLevePart = U3DUtils.NEW<FamilyFlagUpLevePart>();
		this.familyflagUpLevePart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_bqsj_bak.png"), false, 0);
		this.familyflagUpLevePart.InitPartSize((int)this.FamilyFlagUpLeveWindow.BodyWidth - 18, (int)this.FamilyFlagUpLeveWindow.BodyHeight - 9);
		this.familyflagUpLevePart.InitPartData(this.MyBangQiInfoData, this.BHID);
		this.FamilyFlagUpLeveWindow.SetContent(this.FamilyFlagUpLeveWindow.BodyPresenter, this.familyflagUpLevePart, 9.0, 0.0, true);
		this.familyflagUpLevePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseFamilyFlayUpLeveWindow();
		};
	}

	private void CloseFamilyFlayRNWindow()
	{
		this.CloseModalDialog();
		this.CloseChildWindow(this.FamilyFlagReNameWindow);
		this.FamilyFlagReNameWindow = null;
		this.familyflagReNamePart.CleanUpChildWindows();
		this.familyflagReNamePart = null;
	}

	private void ShowFamilyFlayRNWindow()
	{
		if (null != this.FamilyFlagReNameWindow)
		{
			this.CloseFamilyFlayRNWindow();
			return;
		}
		this.ShowModalDialog();
		this.FamilyFlagReNameWindow = U3DUtils.NEW<GChildWindow>();
		this.FamilyFlagReNameWindow.Left = (double)Super.GetChildLeft(640, 268);
		this.FamilyFlagReNameWindow.Top = (double)(Super.GetChildTop(471, 165) - 64);
		this.FamilyFlagReNameWindow.HeadLeft = 0.0;
		this.FamilyFlagReNameWindow.HeadTop = 0.0;
		this.FamilyFlagReNameWindow.HeadWidth = 268.0;
		this.FamilyFlagReNameWindow.HeadHeight = 46.0;
		this.FamilyFlagReNameWindow.BodyLeft = 0.0;
		this.FamilyFlagReNameWindow.BodyTop = 46.0;
		this.FamilyFlagReNameWindow.BodyWidth = 268.0;
		this.FamilyFlagReNameWindow.BodyHeight = 165.0;
		this.FamilyFlagReNameWindow.ImgTitle = "NetImages/GameRes/Images/WinTitle/BangqiGaiming.png";
		this.InitChildWindow1(this.FamilyFlagReNameWindow, Global.GetLang("战旗改名"));
		this.FamilyFlagReNameWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseFamilyFlayRNWindow();
			return true;
		};
		Canvas.SetZIndex(this.FamilyFlagReNameWindow, 9001.0);
		this.Container.Children.Add(this.FamilyFlagReNameWindow);
		this.familyflagReNamePart = U3DUtils.NEW<FamilyFlagReNamePart>();
		this.familyflagReNamePart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_bqgm_bak.png"), false, 0);
		this.familyflagReNamePart.InitPartSize((int)this.FamilyFlagReNameWindow.BodyWidth - 18, (int)this.FamilyFlagReNameWindow.BodyHeight - 9);
		this.familyflagReNamePart.InitPartData(this.txtFlagName.Text, this.BHID);
		this.FamilyFlagReNameWindow.SetContent(this.FamilyFlagReNameWindow.BodyPresenter, this.familyflagReNamePart, 9.0, 0.0, true);
		this.familyflagReNamePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID != 1)
			{
				if (e.ID == 2)
				{
					this.CloseFamilyFlayRNWindow();
				}
			}
		};
	}

	public FamilyFlagUpLevePart GetFamilyFlagUpLevePart()
	{
		return this.familyflagUpLevePart;
	}

	private void CloseFamilyFlagWindow()
	{
		this.CloseModalDialog();
		this.CloseChildWindow(this.FamilyFlagWindow);
		this.FamilyFlagWindow = null;
		this.familyflagPart = null;
	}

	private void ShowFamilyFlagWindow()
	{
		if (null != this.FamilyFlagWindow)
		{
			this.CloseFamilyFlagWindow();
			return;
		}
		this.ShowModalDialog();
		this.FamilyFlagWindow = U3DUtils.NEW<GChildWindow>();
		this.FamilyFlagWindow.Left = (double)Super.GetChildLeft(630, 648);
		this.FamilyFlagWindow.Top = (double)Super.GetChildTop(398, 529);
		this.FamilyFlagWindow.HeadLeft = 0.0;
		this.FamilyFlagWindow.HeadTop = 0.0;
		this.FamilyFlagWindow.HeadWidth = 648.0;
		this.FamilyFlagWindow.HeadHeight = 64.0;
		this.FamilyFlagWindow.BodyLeft = 0.0;
		this.FamilyFlagWindow.BodyTop = 64.0;
		this.FamilyFlagWindow.BodyWidth = 648.0;
		this.FamilyFlagWindow.BodyHeight = 407.0;
		this.FamilyFlagWindow.ImgTitle = "NetImages/GameRes/Images/WinTitle/Bangqi.png";
		this.InitChildWindow(this.FamilyFlagWindow, Global.GetLang("战旗"));
		this.FamilyFlagWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseFamilyFlagWindow();
			return true;
		};
		Canvas.SetZIndex(this.FamilyFlagWindow, 9001.0);
		this.Container.Children.Add(this.FamilyFlagWindow);
		this.familyflagPart = U3DUtils.NEW<FamilyFlagPart>();
		this.familyflagPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_bq_bak.png"), false, 0);
		this.familyflagPart.InitPartSize((int)this.FamilyFlagWindow.BodyWidth - 18, (int)this.FamilyFlagWindow.BodyHeight - 9);
		this.FamilyFlagWindow.SetContent(this.FamilyFlagWindow.BodyPresenter, this.familyflagPart, 9.0, 0.0, true);
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

	public void NotifyBangQiInfoData(BangQiInfoData bangQiInfoData)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.MyBangQiInfoData = bangQiInfoData;
	}

	public void NotifyRenameBangQiResult(int retCode, int roleID, int bhid, string qiName)
	{
		if (retCode < 0)
		{
			if (retCode == -1010)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("修改战旗名称时战盟库存金币余额不足【{0}万】"), new object[]
				{
					Global.RenameBangQiNameNeedTongQian / 10000
				}), 0, -1, -1, 0);
			}
			else if (retCode == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("修改战旗名称发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			this.familyflagReNamePart.CleanUpChildWindows();
			return;
		}
		this.CloseFamilyFlayRNWindow();
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战旗名称已经修改为【{0}】"), new object[]
		{
			qiName
		}), 0, -1, -1, 0);
		GameInstance.Game.SpriteGetBangQiInfo(this.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void NotifyUpLevelBangQiResult(int retCode, int roleID, int bhid)
	{
		if (retCode < 0)
		{
			if (retCode == -1110)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升级战旗时, 战盟库存金币余额不足"), new object[0]), 22, -1, -1, 0);
			}
			else if (retCode == -1000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode >= -1115 && retCode <= -1111)
			{
				int num = -1;
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("BangHuiGoodsIDs", ',');
				if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 5)
				{
					int num2 = Math.Abs(-1111 - retCode);
					num = systemParamIntArrayByName[num2];
				}
				string text = (num != -1) ? Global.GetGoodsNameByID(num, false) : Global.GetLang("未知");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升级战旗时, 战盟库存【{0}】不足"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("升级战旗发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			this.familyflagUpLevePart.CleanUpChildWindows();
			return;
		}
		this.CloseFamilyFlayUpLeveWindow();
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战旗已经成功升级到了【{0}级】"), new object[]
		{
			this.MyBangQiInfoData.BangQiLevel + 1
		}), 0, -1, -1, 0);
		GameInstance.Game.SpriteGetBangQiInfo(this.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private LoadingWindow LoadingWin;

	private int BHID;

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private FamilyFlagReNamePart familyflagReNamePart;

	private GChildWindow FamilyFlagReNameWindow;

	private FamilyFlagUpLevePart familyflagUpLevePart;

	private GChildWindow FamilyFlagUpLeveWindow;

	private GChildWindow FamilyFlagWindow;

	private FamilyFlagPart familyflagPart;

	private BangQiInfoData MyBangQiInfoData;

	private Image FlagImg = new Image();

	private GTextBlockOutLine txtFlagName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtFlagLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLindDiName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYouZhouSlName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYouZhouSlLeve = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYouZhouSlFlagName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTaiYuanSlName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTaiYuanSlLeve = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTaiYuanSlFlagName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtXingYangSlLeve = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtXingYangSlFlagName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public bool DelayShowFlagUpdateWindow;

	private Canvas Root;

	private GTextBlockOutLine txtLingDiZhanXianLu = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
