using System;
using HSGameEngine.GameEngine.SilverLight;

public class FamilyFlagPart : UserControl
{
	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}
}
