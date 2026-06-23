using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class HSTextField : TextBlock
{
	public HSTextField(string _text = "", int fontcolor = -1, int bgcolor = -1, int border = -1, int size = -1, int shadow = -1)
	{
	}

	public void addEventListener(string eventName, TextEventHandler handler)
	{
	}

	public void removeEventListener(string eventName, TextEventHandler handler)
	{
	}

	public double width { get; set; }

	public double height { get; set; }

	public double textWidth { get; set; }

	public double textHeight { get; set; }

	public uint borderColor { get; set; }

	public static string fontName = "SimSun";

	public static int defaultFontSize = 12;

	public static uint LEFT;

	public static uint CENTER = 1U;

	public static uint RIGHT = 2U;

	public StyleSheet styleSheet;
}
