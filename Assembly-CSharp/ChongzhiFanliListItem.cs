using System;
using HSGameEngine.GameEngine.SilverLight;

public class ChongzhiFanliListItem : UserControl
{
	public ChongzhiFanliListItem()
	{
		this.Container.Children.Add(this.txtRoleName);
		Canvas.SetLeft(this.txtRoleName, 163);
		Canvas.SetTop(this.txtRoleName, 2);
		this.txtRoleName.TextColor = new SolidColorBrush(16777215U);
		this.txtRoleName.center = true;
		this.Container.Children.Add(this.txtCondition);
		Canvas.SetLeft(this.txtCondition, 33);
		Canvas.SetTop(this.txtCondition, 2);
		this.txtCondition.TextColor = new SolidColorBrush(16762880U);
		this.txtCondition.center = true;
	}

	public GTextBlockOutLine txtRoleName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtCondition = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
