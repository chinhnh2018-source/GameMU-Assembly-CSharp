using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChangeInfoPart : UserControl
{
	public ChangeInfoPart()
	{
		this.txtInfo.Background = new SolidColorBrush(16777215U);
		this.txtInfo.Foreground = new SolidColorBrush(uint.MaxValue);
		this.txtInfo.FontSize = HSTextField.defaultFontSize;
		this.txtInfo.CaretBrush = new SolidColorBrush(uint.MaxValue);
		this.txtInfo.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtInfo);
		Canvas.SetLeft(this.txtInfo, 27);
		Canvas.SetTop(this.txtInfo, 30);
		this.txtInfo.Width = 235.0;
		this.txtInfo.Height = 119.0;
		this.txtInfo.TextFontWrapping = TextWrapping.Wrap;
		this.txtInfo.Background = new SolidColorBrush(16777215U);
		this.txtInfo.HorizontalScrollBarVisibility = global::ScrollBarVisibility.Hidden;
		this.txtInfo.VerticalScrollBarVisibility = global::ScrollBarVisibility.Hidden;
		UIEventListener.Get(base.gameObject).onKey = new UIEventListener.KeyCodeDelegate(this.KeyUp);
	}

	protected void KeyUp(GameObject go, KeyCode e)
	{
		if (e != 271)
		{
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int bhid
	{
		get
		{
			return this._bhid;
		}
		set
		{
			this._bhid = value;
		}
	}

	public string BulletinMsg
	{
		get
		{
			return this.txtInfo.Text;
		}
		set
		{
			this.txtInfo.Text = value;
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
		gicon.Text = Global.GetLang("确定");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData.RoleID, this.MyBangHuiDetailData) <= 0)
			{
				Super.HintMainText(Global.GetLang("普通成员无权修改战盟公告!"), 10, 3);
				return;
			}
			if (this.txtInfo.Text.Length > 100)
			{
				Super.HintMainText(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"), 10, 3);
				this.txtInfo.Focus();
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.txtInfo.Text, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 10, 3);
					return;
				}
				if (result.is_dirty > 0)
				{
					Super.HintMainText(Global.GetLang("战盟公告不能包含国家规定禁止的词汇!"), 10, 3);
					this.txtInfo.Focus();
					return;
				}
				this.txtInfo.Text = result.msg;
				this.txtInfo.Text = Global.StringReplaceAll(this.txtInfo.Text, "'", string.Empty);
				this.txtInfo.Text = Global.StringReplaceAll(this.txtInfo.Text, "|", string.Empty);
				this.txtInfo.Text = Global.StringReplaceAll(this.txtInfo.Text, "$", string.Empty);
				this.txtInfo.Text = Global.StringReplaceAll(this.txtInfo.Text, ":", string.Empty);
				GameInstance.Game.SpriteUpdateBangHuiBulletinMsg(this.bhid, this.txtInfo.Text);
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = 0
					});
				}
			});
		};
		Canvas.SetLeft(gicon, 58);
		Canvas.SetTop(gicon, 179);
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
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 148);
		Canvas.SetTop(gicon, 179);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
	}

	private BangHuiDetailData MyBangHuiDetailData;

	private TextBox txtInfo = new TextBox();

	private int _bhid;

	public DPSelectedItemEventHandler DPSelectedItem;
}
