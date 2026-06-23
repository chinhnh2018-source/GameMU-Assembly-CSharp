using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ChengJiuTiShiPart : UserControl
{
	public ChengJiuTiShiPart()
	{
		this.Container.Children.Add(this.titleText);
		this.titleText.Height = 20.0;
		this.titleText.center = true;
		Canvas.SetLeft(this.titleText, 172);
		Canvas.SetTop(this.titleText, 25);
		this.titleText.TextFontColor = new SolidColorBrush(65535U);
		this.titleText.mouseEnabled = false;
		this.Container.Children.Add(this.infoText);
		this.infoText.Width = 174.0;
		this.infoText.BodyWidth = 174.0;
		this.infoText.TextFontWrapping = TextWrapping.Wrap;
		Canvas.SetLeft(this.infoText, 106);
		Canvas.SetTop(this.infoText, 46);
		this.infoText.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.lingQuTextEx);
		Canvas.SetLeft(this.lingQuTextEx, 232);
		Canvas.SetTop(this.lingQuTextEx, 64);
		this.lingQuTextEx.HorizontalAlignment = global::Layout.Center;
		this.lingQuTextEx.VerticalAlignment = global::Layout.Center;
		this.lingQuTextEx.Text = Global.GetLang("立即领取");
		this.lingQuTextEx.SetSpecialText(Global.GetLang("立即领取"), new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0)), true, null, true);
		this.lingQuTextEx.TextClick = new UIEventEventHandler(this.TextClick);
	}

	public string NameText
	{
		set
		{
			this.titleText.Text = value;
		}
	}

	public string InfoText
	{
		set
		{
			this.infoText.Text = value;
		}
	}

	public int ID
	{
		set
		{
			this._id = value;
		}
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this._id,
				IDType = 1
			});
		}
		this.lingQuTextEx.Visibility = false;
		return true;
	}

	public GTextBlockOutLine titleText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine infoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockEx lingQuTextEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _id;
}
