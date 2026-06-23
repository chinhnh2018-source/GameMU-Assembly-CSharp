using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ChongShengLianluTiShiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_BtnHeiDi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.m_BtnOff.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.m_BtnOn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
			this.handler(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
	}

	public string Content
	{
		set
		{
			this.m_LabContent.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(value)
			});
		}
	}

	public DPSelectedItemEventHandler handler;

	public DPSelectedItemEventHandler handlerClose;

	public GButton m_BtnHeiDi;

	public GButton m_BtnOn;

	public GButton m_BtnOff;

	public UILabel m_LabContent;
}
