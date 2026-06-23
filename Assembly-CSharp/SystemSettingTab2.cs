using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class SystemSettingTab2 : UserControl
{
	protected override void InitializeComponent()
	{
		Canvas.SetLeft(this.Container, 12);
		Canvas.SetTop(this.Container, 18);
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}
}
