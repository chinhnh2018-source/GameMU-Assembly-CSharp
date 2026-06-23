using System;
using HSGameEngine.GameEngine.SilverLight;

public class ActivityGrowListItem : UserControl
{
	public string TaskItem
	{
		get
		{
			return this.txtTask.Text;
		}
		set
		{
			this.txtTask.Text = value;
		}
	}

	public SolidColorBrush TxtItemColor
	{
		get
		{
			return this.txtTask.TextColor;
		}
		set
		{
			this.txtTask.TextColor = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtTask);
		Canvas.SetLeft(this.txtTask, 0);
		Canvas.SetTop(this.txtTask, 0);
		this.txtTask.TextColor = new SolidColorBrush(46850U);
		this.txtTask.FontSize = HSTextField.defaultFontSize;
		this.txtTask.Width = 368.0;
	}

	private GTextBlockOutLine txtTask = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
