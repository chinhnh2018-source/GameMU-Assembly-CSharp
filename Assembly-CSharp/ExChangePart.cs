using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class ExChangePart : UserControl
{
	public ExChangePart()
	{
		this.thisCtrl = this;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtdouble);
		this.txtdouble.TextColor = new SolidColorBrush(4294954496U);
		Canvas.SetLeft(this.txtdouble, 129);
		Canvas.SetTop(this.txtdouble, 23);
	}

	public int GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtYPNum = U3DUtils.NEW<GTextBlock>();
		this.txtYPNum.BodyWidth = 75.0;
		this.txtYPNum.BodyHeight = 21.0;
		this.txtYPNum.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 75.0, 21.0, 3.0, 2.0));
		this.txtYPNum.FontSize = 12;
		this.txtYPNum.Onlydouble = true;
		this.txtYPNum.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		this.txtYPNum.EditText = Global.GetTotalGoodsCountByID(this.GoodsID).ToString();
		Canvas.SetLeft(this.txtYPNum, 24);
		Canvas.SetTop(this.txtYPNum, 72);
		this.Container.Children.Add(this.txtYPNum);
		this.txtYPNum.Focus();
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("全部");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.txtYPNum.EditText = this.txtdouble.Text;
		};
		Canvas.SetLeft(gicon, 143);
		Canvas.SetTop(gicon, 70);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("兑换");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int yinPiaoNum = Global.SafeConvertToInt32(this.txtYPNum.EditText);
			if (yinPiaoNum <= 0)
			{
				return;
			}
			if (yinPiaoNum > Global.GetTotalGoodsCountByID(this.GoodsID))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("要兑换的银票张数超过了背包中的最大银票张数!"), 0, -1, -1, 0);
				return;
			}
			string message = StringUtil.substitute(Global.GetLang("确认要将【{0}】张银票兑换成【{1}】金币吗?"), new object[]
			{
				this.txtYPNum.EditText,
				yinPiaoNum * 1000
			});
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), message, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SpriteBatchYinPiao(yinPiaoNum);
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
						{
							ID = 0,
							IDType = 0
						});
					}
				}
				return true;
			};
		};
		Canvas.SetLeft(gicon, 42);
		Canvas.SetTop(gicon, 103);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("取消");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
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
		Canvas.SetLeft(gicon, 145);
		Canvas.SetTop(gicon, 103);
		this.Container.Children.Add(gicon);
		this.txtdouble.Text = Global.GetTotalGoodsCountByID(this.GoodsID).ToString();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private UserControl thisCtrl;

	private GTextBlock txtYPNum;

	private GTextBlockOutLine txtdouble = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _GoodsID;
}
