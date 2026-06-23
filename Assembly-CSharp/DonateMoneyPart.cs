using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class DonateMoneyPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtMoney);
		this.txtMoney.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtMoney, 42);
		Canvas.SetTop(this.txtMoney, 39);
		this.Container.Children.Add(this.txtDonateValue);
		this.txtDonateValue.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtDonateValue, 162);
		Canvas.SetTop(this.txtDonateValue, 111);
		this.thisCtrl = this;
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
		this.txtDonateMoney = U3DUtils.NEW<GTextBlock>();
		this.txtDonateMoney.BodyWidth = 195.0;
		this.txtDonateMoney.BodyHeight = 21.0;
		this.txtDonateMoney.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 195.0, 21.0, 3.0, 2.0));
		this.txtDonateMoney.FontSize = 12;
		this.txtDonateMoney.Onlydouble = true;
		this.txtDonateMoney.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtDonateMoney, 40);
		Canvas.SetTop(this.txtDonateMoney, 82);
		this.Container.Children.Add(this.txtDonateMoney);
		GIcon gicon = U3DUtils.NEW<GIcon>();
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
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("非【{0}】战盟成员, 无法贡献金币!"), new object[]
				{
					Global.FormatBangHuiName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName)
				}), 0, -1, -1, 0);
				return;
			}
			int num = Global.SafeConvertToInt32(this.txtDonateMoney.EditText);
			if (num < Global.MinDonateBangGongTongQian)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("每次贡献的金币最少需要{0}万"), new object[]
				{
					Global.MinDonateBangGongTongQian / 10000
				}), 0, -1, -1, 0);
				return;
			}
			if (num > Global.Data.roleData.YinLiang)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("每次贡献的金币不能超过背包中已有金币{0}"), new object[]
				{
					Global.Data.roleData.YinLiang
				}), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SpriteDonateBGMoney(num);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 35);
		Canvas.SetTop(gicon, 196);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
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
		Canvas.SetLeft(gicon, 144);
		Canvas.SetTop(gicon, 196);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.txtMoney.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.YinLiang
		});
		this.txtDonateMoney.EditText = StringUtil.substitute("{0}", new object[]
		{
			Global.MinDonateTongQianPerBangGong
		});
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Root);
	}

	private GTextBlock txtDonateMoney;

	private BangHuiDetailData MyBangHuiDetailData;

	private Canvas Root;

	private GTextBlockOutLine txtMoney;

	private GTextBlockOutLine txtDonateValue;

	private SpriteSL thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;
}
