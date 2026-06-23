using System;
using HSGameEngine.GameEngine.Logic;

public class JiYuanJuanXianJiangLiItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("领取");
	}

	public UILabel m_LabTitle;

	public UISprite m_spYiLingQu;

	public GButton m_Btn;

	public ListBox m_ListBOX;

	public int ID;

	public int value;

	public ObservableCollection m_ObservableCollection;
}
