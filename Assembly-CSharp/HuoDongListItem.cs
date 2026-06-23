using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class HuoDongListItem : UserControl
{
	public HuoDongListItem()
	{
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4289453484U);
		Canvas.SetLeft(this.txtName, 10);
		Canvas.SetTop(this.txtName, 4);
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(5207664U);
		base.BackgroundAlpha = 0.01;
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
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

	public string HuoDongName
	{
		get
		{
			return this.txtName.Text;
		}
		set
		{
			this.txtName.Text = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.buttonMode = true;
		base.BackgroundAlpha = 0.2;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.buttonMode = false;
		base.BackgroundAlpha = 0.01;
	}

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
