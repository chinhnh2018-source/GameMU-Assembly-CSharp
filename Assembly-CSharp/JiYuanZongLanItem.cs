using System;

public class JiYuanZongLanItem : UserControl
{
	public int Progress
	{
		get
		{
			return this.m_EraID;
		}
		set
		{
			this.m_EraID = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public UISprite m_SpSuo;

	public ShowNetImage m_ShowBack;

	public ListBox m_ListBox;

	public UILabel m_LabJiYuan;

	public GButton m_Btn;

	public ShowNetImage m_ShowBtn;

	public UILabel m_LabJinDu;

	public int m_EraID = -1;

	public TweenPosition m_TweenPosition;

	public TweenScale m_TweenScale;

	public int CurrentSelectedPage;

	public float parentPosition;
}
