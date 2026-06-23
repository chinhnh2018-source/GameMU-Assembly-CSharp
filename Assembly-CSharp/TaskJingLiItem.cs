using System;
using HSGameEngine.GameEngine.SilverLight;

public class TaskJingLiItem : UserControl
{
	public TaskJingLiItem()
	{
		this.Container.Children.Add(this._Text);
		this._Text.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this._Text, 5);
		this.Container.Children.Add(this._img);
	}

	public BitmapData img
	{
		set
		{
			this._img.Source = new ImageBrush(value);
		}
	}

	public string Text
	{
		set
		{
			this._Text.htmlText = value;
		}
	}

	private GTextBlockOutLine _Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Image _img = new Image();
}
