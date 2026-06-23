using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class FuliLingQuBtn : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lingquBtn.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.lingquBtn)
		{
			this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.lingquID
				});
			};
		}
	}

	public UISprite lingquStatus;

	public GButton lingquBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int lingquID;
}
