using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class QiFuShuoMingPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = 0
				});
			};
		}
	}

	public GButton m_BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage NetImage;
}
