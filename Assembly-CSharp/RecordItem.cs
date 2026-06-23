using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class RecordItem : UserControl
{
	public RecordItem()
	{
		this.Container.Children.Add(this.textEx);
		this.textEx.TextWrapping = TextWrapping.Wrap;
	}

	public string Text
	{
		set
		{
			Super.FormatTextBlockEx2(this.textEx, value);
		}
	}

	public double BodyHeight
	{
		set
		{
			this.textEx.TextHeight = value;
		}
	}

	public double BodyWidth
	{
		set
		{
			this.textEx.TextWidth = value;
			this.textEx.BodyWidth = value;
		}
	}

	private GTextBlockEx textEx = new GTextBlockEx(string.Empty, 16777215, -1, -1, -1, 0);
}
