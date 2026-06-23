using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class MyInputBoxPart : UserControl
{
	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public string Text
	{
		get
		{
			return this.InputTextBlock.EditText;
		}
		set
		{
			this.InputTextBlock.EditText = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GTextBlock gtextBlock = U3DUtils.NEW<GTextBlock>();
		gtextBlock.BodyWidth = 85.0;
		gtextBlock.BodyHeight = 21.0;
		gtextBlock.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 85.0, 21.0, 3.0, 2.0));
		gtextBlock.Text.Text = string.Empty;
		gtextBlock.Text.FontSize = FontSizeMgr.NormalInputFontSize;
		gtextBlock.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(gtextBlock, 21);
		Canvas.SetTop(gtextBlock, 43);
		this.Container.Children.Add(gtextBlock);
		this.InputTextBlock = gtextBlock;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 75.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_hover.png"));
		gicon.Text = Global.GetLang("确定");
		gicon.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(gicon, 76);
		Canvas.SetTop(gicon, 90);
		this.Container.Children.Add(gicon);
		GIcon gicon2 = gicon;
		gicon2.MouseLeftButtonUp = (MouseLeftButtonUpEventHandler)Delegate.Combine(gicon2.MouseLeftButtonUp, new MouseLeftButtonUpEventHandler(this.OkClick));
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 75.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_hover.png"));
		gicon.Text = Global.GetLang("取消");
		gicon.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(gicon, 164);
		Canvas.SetTop(gicon, 90);
		this.Container.Children.Add(gicon);
		GIcon gicon3 = gicon;
		gicon3.MouseLeftButtonUp = (MouseLeftButtonUpEventHandler)Delegate.Combine(gicon3.MouseLeftButtonUp, new MouseLeftButtonUpEventHandler(this.CancelClick));
	}

	private void OkClick(object sender, MouseEvent e)
	{
		this.ButtonState = "OK";
		if (this.ButtonClick != null)
		{
			this.ButtonClick.Invoke(sender, e);
		}
	}

	private void CancelClick(object sender, MouseEvent e)
	{
		this.ButtonState = "Cancel";
		if (this.ButtonClick != null)
		{
			this.ButtonClick.Invoke(sender, e);
		}
	}

	public EventHandler ButtonClick;

	private GTextBlock InputTextBlock;

	public string ButtonState = "Cancel";
}
