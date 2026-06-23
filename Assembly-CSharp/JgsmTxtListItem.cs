using System;
using HSGameEngine.GameEngine.SilverLight;

public class JgsmTxtListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
	}

	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public GTextBlockEx TxtGo
	{
		set
		{
			Canvas.SetLeft(value, 12);
			Canvas.SetTop(value, 56);
			this.Container.Children.Add(value);
		}
	}

	private Canvas Root;
}
