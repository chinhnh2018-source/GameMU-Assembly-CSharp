using System;
using HSGameEngine.GameEngine.SilverLight;

public class GScrollView : UserControl, ISilverLight
{
	public GScrollView(int x = 0, int y = 0, int z = 0)
	{
	}

	public int VerticalScrollBarVisibility { get; set; }

	public int HorizontalScrollBarVisibility { get; set; }

	public object Viewer { get; set; }

	public void ResetScrollView()
	{
	}
}
