using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class UpDown : UserControl
{
	public UpDown()
	{
		this.Width = 18.0;
		this.Height = 25.0;
		this.upGIcon = U3DUtils.NEW<GIcon>();
		this.upGIcon.Width = 18.0;
		this.upGIcon.Height = 12.0;
		this.upGIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/uparrow_nomal.png"));
		this.upGIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/uparrow_hover.png"));
		this.upGIcon.ItemCode = 1;
		this.Container.Children.Add(this.upGIcon);
		Canvas.SetLeft(this.upGIcon, 0);
		Canvas.SetTop(this.upGIcon, 0);
		this.upGIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
		this.downGIcon = U3DUtils.NEW<GIcon>();
		this.downGIcon.Width = 18.0;
		this.downGIcon.Height = 12.0;
		this.downGIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/downarrow_normal.png"));
		this.downGIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/downarrow_hover.png"));
		this.downGIcon.ItemCode = -1;
		this.Container.Children.Add(this.downGIcon);
		Canvas.SetLeft(this.downGIcon, 0);
		Canvas.SetTop(this.downGIcon, 13);
		this.downGIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		GIcon gicon = sender as GIcon;
		if (null != gicon)
		{
			if (gicon.ItemCode == 1)
			{
				this.ValueChange.Invoke(this, new ChangeEventArgs
				{
					ChangeType = 1
				});
			}
			else if (gicon.ItemCode == -1)
			{
				this.ValueChange.Invoke(this, new ChangeEventArgs
				{
					ChangeType = -1
				});
			}
		}
	}

	public EventHandler ValueChange;

	private GIcon upGIcon;

	private GIcon downGIcon;
}
