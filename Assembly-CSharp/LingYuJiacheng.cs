using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class LingYuJiacheng : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public TextBlock labLuck;

	public TextBlock labDeLuck;

	public TextBlock labnextLuck;

	public TextBlock labnextDeLuck;

	public TextBlock labNextNeed;

	public GButton CloseBtn;

	public DPSelectedItemEventHandler DPSelectedItem;
}
