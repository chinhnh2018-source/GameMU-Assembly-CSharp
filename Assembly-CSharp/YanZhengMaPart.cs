using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class YanZhengMaPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtHint = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this.txtHint.BodyWidth = 220.0;
		this.txtHint.BodyHeight = 20.0;
		this.txtHint.Text = Global.GetLang("请输入下图中的字符，不区分大小写");
		this.txtHint.TextColor = new SolidColorBrush(7448500U);
		Canvas.SetLeft(this.txtHint, 39);
		Canvas.SetTop(this.txtHint, 8);
		this.Container.Children.Add(this.txtHint);
		this.imgYanZhengMa = U3DUtils.NEW<ValidateCode>();
		this.imgYanZhengMa.Width = 104.0;
		this.imgYanZhengMa.Height = 32.0;
		Canvas.SetLeft(this.imgYanZhengMa, 173);
		Canvas.SetTop(this.imgYanZhengMa, 32);
		this.Container.Children.Add(this.imgYanZhengMa);
		this.txtReLoadYanZhengMa = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		this.txtReLoadYanZhengMa.TextWidth = 120.0;
		this.txtReLoadYanZhengMa.FontSize = 12;
		this.txtReLoadYanZhengMa.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
		this.txtReLoadYanZhengMa.Text = Global.GetLang("看不清？换一张");
		this.txtReLoadYanZhengMa.SetSpecialText(this.txtReLoadYanZhengMa.Text, new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0)), true, null, true);
		this.txtReLoadYanZhengMa.TextClick = new UIEventEventHandler(this.TextClick);
		Canvas.SetLeft(this.txtReLoadYanZhengMa, 182);
		Canvas.SetTop(this.txtReLoadYanZhengMa, 72);
		this.Container.Children.Add(this.txtReLoadYanZhengMa);
		this.inputYanZhengMa = U3DUtils.NEW<GTextBlock>();
		this.inputYanZhengMa.BodyWidth = 136.0;
		this.inputYanZhengMa.BodyHeight = 21.0;
		this.inputYanZhengMa.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 136.0, 21.0, 3.0, 2.0));
		this.inputYanZhengMa.FontSize = 12;
		this.inputYanZhengMa.Onlydouble = true;
		this.inputYanZhengMa.Text.border = false;
		this.inputYanZhengMa.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.inputYanZhengMa, 25);
		Canvas.SetTop(this.inputYanZhengMa, 35);
		this.Container.Children.Add(this.inputYanZhengMa);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("确定");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.inputYanZhengMa.EditText == this.imgYanZhengMa.chars)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = Convert.ToInt32(this.inputYanZhengMa.EditText)
					});
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您输入的验证码不正确！"), new object[0]), 0, -1, -1, 0);
			}
		};
		Canvas.SetLeft(gicon, 54);
		Canvas.SetTop(gicon, 103);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("取消");
		gicon.TextColor = new SolidColorBrush(10551295U);
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
		Canvas.SetLeft(gicon, 145);
		Canvas.SetTop(gicon, 103);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		this.GetYanZhengMa();
		this.inputYanZhengMa.Focus();
	}

	private void GetYanZhengMa()
	{
		GameInstance.Game.SpriteGetMailSendCode(Global.Data.roleData.RoleID);
	}

	public void OnGetMailSendCodeCompleted(string code)
	{
		this.imgYanZhengMa.chars = code;
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		this.InitPartData();
		return true;
	}

	public bool TextMouseEnter(object sender, BaseEventArgs e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(e.Tag is SpecialTextItem))
		{
			return false;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		this.CurrentItemTag = ((e.Tag as SpecialTextItem).Tag as string);
		if (this.CurrentItemTag == null || string.Empty == this.CurrentItemTag)
		{
			return true;
		}
		string[] array = this.CurrentItemTag.Split(new char[]
		{
			','
		});
		if (array.Length != 6)
		{
			return true;
		}
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
		(sender as GTextBlockExItem).Link(new SolidColorBrush(4289014314U));
		int num = Convert.ToInt32(array[0]);
		int num2 = Convert.ToInt32(array[1]);
		int num3 = Convert.ToInt32(array[2]);
		int num4 = Convert.ToInt32(array[3]);
		if (num != -1)
		{
			GTipService.NotifyTip(sender as GTextBlockEx, new NotifyTipEventArgs
			{
				MouseState = true,
				TipType = TipTypes.NormalText,
				Tip = Global.GetLang("单击自动寻路"),
				MouseEvent = (e.e as MouseEvent)
			});
		}
		return true;
	}

	public bool TextMouseLeave(object sender, BaseEventArgs e)
	{
		if (e.Tag is SpecialTextItem)
		{
			if (Global.Data.GameCursorImageID < 100)
			{
				base.Cursor = Cursors.Auto;
			}
			(sender as GTextBlockExItem).Unlink();
			GTipService.NotifyTip(this, new NotifyTipEventArgs
			{
				MouseState = false,
				TipType = TipTypes.NormalText,
				Tip = Global.GetLang("看不清？换一张"),
				MouseEvent = (e.e as MouseEvent)
			});
			this.CurrentItemTag = null;
			return true;
		}
		return false;
	}

	public void CleanUpChildWindows()
	{
		this.ClearHintDecoration();
		Super.CleanUpAllChildWindows(this.Container);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine txtHint;

	private GTextBlock inputYanZhengMa;

	private GTextBlockEx txtReLoadYanZhengMa;

	private ValidateCode imgYanZhengMa;

	public string CurrentItemTag;
}
