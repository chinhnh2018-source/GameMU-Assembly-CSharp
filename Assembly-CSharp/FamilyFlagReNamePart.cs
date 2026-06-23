using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class FamilyFlagReNamePart : UserControl
{
	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.thisCtrl = this;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.FamilyFlagName = U3DUtils.NEW<GTextBlock>();
		this.FamilyFlagName.BodyWidth = 136.0;
		this.FamilyFlagName.BodyHeight = 21.0;
		this.FamilyFlagName.FontSize = FontSizeMgr.NormalInputFontSize;
		this.FamilyFlagName.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 136.0, 21.0, 3.0, 2.0));
		this.FamilyFlagName.ReadOnly = false;
		this.FamilyFlagName.Onlydouble = false;
		Canvas.SetLeft(this.FamilyFlagName, 19);
		Canvas.SetTop(this.FamilyFlagName, 40);
		this.Container.Children.Add(this.FamilyFlagName);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("确 定");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.IsBangHuiLeader(Global.Data.roleData, this.BHID))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能执行战旗升级操作!"), 0, -1, -1, 0);
				return;
			}
			if (this.FamilyFlagName.Text.Text.Length < 2)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,您输入的战旗名称不能少于2个字，请重新输入!"), -1, -1, -1, -1, false);
				return;
			}
			if (this.FamilyFlagName.Text.Text.Length > 14)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,您输入的战旗名称已超过14个字，请重新输入!"), -1, -1, -1, -1, false);
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.FamilyFlagName.EditText, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 0, -1, -1, 0);
					return;
				}
				if (result.is_dirty > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("战旗名称不能包含国家规定禁止的词汇!"), 0, -1, -1, 0);
					this.FamilyFlagName.Focus();
					return;
				}
				this.FamilyFlagName.EditText = result.msg;
				this.FamilyFlagName.EditText = Global.StringReplaceAll(this.FamilyFlagName.EditText, "'", string.Empty);
				this.FamilyFlagName.EditText = Global.StringReplaceAll(this.FamilyFlagName.EditText, "|", string.Empty);
				this.FamilyFlagName.EditText = Global.StringReplaceAll(this.FamilyFlagName.EditText, "$", string.Empty);
				this.FamilyFlagName.EditText = Global.StringReplaceAll(this.FamilyFlagName.EditText, ":", string.Empty);
				GameInstance.Game.SpriteRenameBangQi(this.FamilyFlagName.EditText);
				this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
				this.Container.Children.Add(this.LoadingWin);
			});
		};
		Canvas.SetLeft(gicon, 29);
		Canvas.SetTop(gicon, 122);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("取 消");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 2
				});
			}
		};
		Canvas.SetLeft(gicon, 148);
		Canvas.SetTop(gicon, 122);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData(string sFlagName, int bhid)
	{
		this.BHID = bhid;
		this.FamilyFlagName.Text.Text = sFlagName;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	private LoadingWindow LoadingWin;

	private GTextBlock FamilyFlagName;

	private int BHID;

	private SpriteSL thisCtrl = new SpriteSL();

	public DPSelectedItemEventHandler DPSelectedItem;
}
