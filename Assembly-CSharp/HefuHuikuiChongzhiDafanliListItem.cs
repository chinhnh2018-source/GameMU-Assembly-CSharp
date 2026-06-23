using System;
using HSGameEngine.GameEngine.SilverLight;

public class HefuHuikuiChongzhiDafanliListItem : UserControl
{
	public HefuHuikuiChongzhiDafanliListItem()
	{
		this.Container.Children.Add(this.txtTopRoleNameYesterday);
		Canvas.SetLeft(this.txtTopRoleNameYesterday, 5);
		Canvas.SetTop(this.txtTopRoleNameYesterday, 3);
		this.txtTopRoleNameYesterday.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtTopRoleNameToday);
		Canvas.SetLeft(this.txtTopRoleNameToday, 128);
		Canvas.SetTop(this.txtTopRoleNameToday, 3);
		this.txtTopRoleNameToday.TextColor = new SolidColorBrush(65280U);
	}

	public GTextBlockOutLine txtTopRoleNameYesterday = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtTopRoleNameToday = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
