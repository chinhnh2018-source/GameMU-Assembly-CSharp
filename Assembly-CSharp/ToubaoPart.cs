using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class ToubaoPart : UserControl
{
	public ToubaoPart()
	{
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtYPType);
		this.txtYPType.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtYPType, 151);
		Canvas.SetTop(this.txtYPType, 69);
		this.Container.Children.Add(this.txtYP);
		this.txtYP.TextColor = new SolidColorBrush(4294954496U);
		Canvas.SetLeft(this.txtYP, 142);
		Canvas.SetTop(this.txtYP, 90);
		this.Container.Children.Add(this.txtYL1);
		this.txtYL1.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtYL1, 142);
		Canvas.SetTop(this.txtYL1, 114);
		this.Container.Children.Add(this.txtYL2);
		this.txtYL2.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtYL2, 142);
		Canvas.SetTop(this.txtYL2, 138);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon BtnIcon = U3DUtils.NEW<GIcon>();
		BtnIcon.Width = 80.0;
		BtnIcon.Height = 21.0;
		BtnIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		BtnIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		BtnIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		BtnIcon.Text = Global.GetLang("立即投保");
		BtnIcon.TextColor = new SolidColorBrush(10551295U);
		BtnIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!BtnIcon.EnableIcon)
			{
				return;
			}
			if (Global.Data.roleData.UserMoney < this.NeedYinLiangNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您目前没有足够的钻石，无法投保"), new object[0]), 0, -1, -1, 0);
				return;
			}
			GameInstance.Game.SpriteYaBiaoTouBao();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(BtnIcon, 60);
		Canvas.SetTop(BtnIcon, 162);
		this.Container.Children.Add(BtnIcon);
		BtnIcon = U3DUtils.NEW<GIcon>();
		BtnIcon.Width = 80.0;
		BtnIcon.Height = 21.0;
		BtnIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		BtnIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		BtnIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 80.0, 21.0, 3.0, 2.0));
		BtnIcon.Text = Global.GetLang("我再想想");
		BtnIcon.TextColor = new SolidColorBrush(10551295U);
		BtnIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!BtnIcon.EnableIcon)
			{
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(BtnIcon, 149);
		Canvas.SetTop(BtnIcon, 162);
		this.Container.Children.Add(BtnIcon);
	}

	public void InitPartData(int yaBiaoID)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		Global.GetYaBiaoReward(yaBiaoID, out num, out num2, out num3);
		this.NeedYinLiangNum = Global.GetTouBaoYinPiaoNum(num);
		this.txtYP.Text = this.NeedYinLiangNum.ToString();
		this.txtYL1.Text = num.ToString();
		this.txtYL2.Text = (num * 2).ToString();
	}

	public override void Destroy()
	{
	}

	private SpriteSL thisCtrl;

	private int NeedYinLiangNum;

	private GTextBlockOutLine txtYPType = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYP = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYL1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYL2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;
}
