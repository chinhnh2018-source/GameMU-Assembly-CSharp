using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class HefuPartWeizhanErsheng : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTime();
		this.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.btnGo.isEnabled)
			{
				return;
			}
		};
	}

	private void InitTime()
	{
		this.timeLabel.Pivot = 3;
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		this.timeLabel.text = string.Format(Global.GetLang("{0}月{1}日-{2}月{3}日"), new object[]
		{
			this.startTime.Month,
			this.startTime.Day,
			this.endTime.Month,
			this.endTime.Day
		});
	}

	public TextBlock timeLabel;

	public GButton btnGo;

	private DateTime startTime;

	private DateTime endTime;
}
