using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;

public class HyperLinkText : UserControl
{
	public HyperLinkText(int width, int height)
	{
		this.FakeMouseEvent = true;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.textBlock = new TextBlock();
		Canvas.SetLeft(this.textBlock, 58);
		Canvas.SetTop(this.textBlock, 0);
		this.Container.Children.Add(this.textBlock);
		this.textBlock.addEventListener("mouseUp", new MouseEventHandler(this.mouseEventLeftButtonUp));
	}

	public bool FakeMouseEvent { get; set; }

	public int tabIndex { get; set; }

	public TextFormat TxtFormat
	{
		set
		{
			this.textBlock.setTextFormat(value);
			this.textBlock.defaultTextFormat = value;
		}
	}

	public string HtmlText
	{
		set
		{
			this.textBlock.htmlText = StringUtil.substitute("<a href='evt:typetext'>{0}</a>", new object[]
			{
				value
			});
		}
	}

	private void mouseEventLeftButtonUp(MouseEvent evt)
	{
		if (this.MouseLeftButtonUp != null)
		{
			this.MouseLeftButtonUp.Invoke(this, evt);
		}
	}

	public EventHandler MouseLeftButtonUp;

	private TextBlock textBlock;
}
