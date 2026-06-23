using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class XinQuHuoDongListItem : UserControl
{
	public XinQuHuoDongListItem()
	{
		this.Container.Children.Add(this.item);
		this.item.Width = 73.0;
		this.item.Height = 93.0;
		Canvas.SetLeft(this.item, 0);
		Canvas.SetTop(this.item, 0);
		this.Container.Children.Add(this.txtName);
		Canvas.SetLeft(this.txtName, 12);
		Canvas.SetTop(this.txtName, 9);
		this.Container.Children.Add(this.itemImg);
		Canvas.SetLeft(this.itemImg, 11);
		Canvas.SetTop(this.itemImg, 35);
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(1U);
		base.BackgroundAlpha = 0.01;
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

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public ImageURL ItemBackground
	{
		set
		{
			this.item.BackgroundURL = value;
		}
	}

	public string Title
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

	public ImageBrush ItemImg
	{
		set
		{
			this.itemImg.Source = value;
		}
	}

	public string ItemImgURL
	{
		set
		{
			if (value != null)
			{
				this.itemImg.URL = value;
			}
			else
			{
				this.itemImg.Source = null;
			}
		}
	}

	public SolidColorBrush TitleColor
	{
		set
		{
			this.txtName.TextColor = value;
		}
	}

	public bool TitleBold
	{
		set
		{
			this.txtName.fontBold = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.buttonMode = true;
		base.BackgroundAlpha = 0.4;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.buttonMode = false;
		base.BackgroundAlpha = 0.01;
	}

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas item = new Canvas();

	private URLImage itemImg = new URLImage();
}
