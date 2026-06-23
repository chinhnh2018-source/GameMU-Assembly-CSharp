using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class MaskPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.txtDes);
		this.txtDes.BodyWidth = 169.0;
		this.txtDes.TextFontWrapping = TextWrapping.Wrap;
		this.txtDes.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(this.txtDes, 70);
		Canvas.SetTop(this.txtDes, 203);
		this.Container.Children.Add(this.Img);
		this.Img.Width = 168.0;
		this.Img.Height = 130.0;
		Canvas.SetLeft(this.Img, 68);
		Canvas.SetTop(this.Img, 66);
		this.IconStart = U3DUtils.NEW<GIcon>();
		this.IconStart.Width = 81.0;
		this.IconStart.Height = 21.0;
		this.IconStart.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.IconStart.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.IconStart.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.IconStart.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.IconStart.Text = Global.GetLang("立即激活");
		this.IconStart.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnMouseButtonUp);
		this.Container.Children.Add(this.IconStart);
		Canvas.SetLeft(this.IconStart, 113);
		Canvas.SetTop(this.IconStart, 257);
	}

	public void InitPartData(SystemOpenVO data)
	{
		this.Img.Source = new ImageBrush(Global.GetGameResImage(StringUtil.substitute("Images/GongnengYugao/img/{0}.png", new object[]
		{
			data.ID
		})));
		this.IconStart.ItemCode = data.ID;
	}

	private void OnMouseButtonUp(object sender, MouseEvent e)
	{
		if (this.IconStart.EnableIcon)
		{
			GameInstance.Game.SpriteSetSystemOpenParams(this.IconStart.ItemCode);
		}
	}

	public void OnCompleted(int result, int activateIndex)
	{
		if (result < 1)
		{
			if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("等级不够，无法激活"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("激活时发生错误{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			IDType = 1,
			ID = this.IconStart.ItemCode
		});
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine txtDes = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Image Img = new Image();

	private GIcon IconStart;
}
