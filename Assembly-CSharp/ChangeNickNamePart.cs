using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ChangeNickNamePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtName, 80);
		Canvas.SetTop(this.txtName, 25);
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._roleID;
		}
		set
		{
			this._roleID = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this._roleName;
		}
		set
		{
			this._roleName = value;
		}
	}

	public string OldChengHao
	{
		get
		{
			return this._OldChengHao;
		}
		set
		{
			this._OldChengHao = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtNewName = U3DUtils.NEW<GTextBlock>();
		this.txtNewName.BodyWidth = 150.0;
		this.txtNewName.BodyHeight = 21.0;
		this.txtNewName.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 150.0, 21.0, 3.0, 2.0));
		this.txtNewName.FontSize = 12;
		this.txtNewName.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtNewName, 83);
		Canvas.SetTop(this.txtNewName, 47);
		this.Container.Children.Add(this.txtNewName);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("确定");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.Faction != this.MyBangHuiDetailData.BHID)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("非【{0}】战盟成员无法操作"), new object[]
				{
					this.MyBangHuiDetailData.BHName
				}), 0, -1, -1, 0);
				return;
			}
			if (Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData) <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("普通成员无法执行修改称号操作!"), 0, -1, -1, 0);
				return;
			}
			if (this.txtNewName.EditText.Length > 7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您输入的称号超过了7个汉字，请重新输入！"), 0, -1, -1, 0);
				this.txtNewName.Focus();
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.txtNewName.EditText, delegate(object content, ExecWordsFilterEventArgs result)
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
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("称号不能包含国家规定禁止的词汇!"), 0, -1, -1, 0);
					this.txtNewName.Focus();
					return;
				}
				string text = result.msg;
				text = Global.StringReplaceAll(text, "'", string.Empty);
				text = Global.StringReplaceAll(text, "|", string.Empty);
				text = Global.StringReplaceAll(text, "$", string.Empty);
				text = Global.StringReplaceAll(text, ":", string.Empty);
				GameInstance.Game.SpriteUpdateBHMemberChengHao(this.MyBangHuiDetailData.BHID, this.RoleID, text);
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = 0
					});
				}
			});
		};
		Canvas.SetLeft(gicon, 25);
		Canvas.SetTop(gicon, 106);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
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
		Canvas.SetLeft(gicon, 144);
		Canvas.SetTop(gicon, 106);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		if (this._roleID == -1)
		{
			return;
		}
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.txtName.Text = this._roleName;
		this.txtNewName.EditText = this.OldChengHao;
	}

	private GTextBlock txtNewName;

	private int _roleID = -1;

	private string _roleName = string.Empty;

	private BangHuiDetailData MyBangHuiDetailData;

	private Canvas Root;

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private SpriteSL thisCtrl = new SpriteSL();

	private string _OldChengHao;

	public DPSelectedItemEventHandler DPSelectedItem;
}
