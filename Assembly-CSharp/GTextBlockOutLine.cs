using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GTextBlockOutLine : TextBlock
{
	public GTextBlockOutLine(string _text = "", int fontcolor = -1, int bgcolor = -1, int border = -1, int size = -1, int shadow = -1)
	{
	}

	public new uint fontBorder { get; set; }

	public int numLines { get; set; }

	public double TextSize { get; set; }

	public SolidColorBrush TextFontColor { get; set; }

	public double BodyHeight { get; set; }

	public bool TextUnderLine { get; set; }

	public bool underLine { get; set; }

	public SolidColorBrush TextColor { get; set; }

	public double width { get; set; }

	public double BodyWidth { get; set; }

	public double TextWidth { get; set; }

	public double TextHeight { get; set; }

	public double TextLineHeight { get; set; }

	public UIEventEventHandler TextClick { get; set; }

	public bool center { get; set; }

	public UIEventEventHandler TextMouseEnter { get; set; }

	public UIEventEventHandler TextMouseLeave { get; set; }

	public bool fontBold { get; set; }

	public int Cursor { get; set; }

	public bool doubleClickEnabled { get; set; }

	public SizeSL RealSize { get; set; }

	public void SetSpecialText(string text, SolidColorBrush brush, bool special = false, string tag = "", bool b0 = true)
	{
	}

	protected virtual void OnClick(string key)
	{
	}
}
