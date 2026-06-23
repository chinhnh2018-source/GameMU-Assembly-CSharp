using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ShenHunPartMain : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnFuTi.Text = Global.GetLang("神魂附体");
		this.btnShiZhuang.Text = Global.GetLang("神魂时装");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.futiWindow.skillDetailWindow = this.skillDetailWindow;
		this.shizhuangWindow.skillDetailWindow = this.skillDetailWindow;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnFuTi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetState(ShenHunType.FuTi);
		};
		this.btnShiZhuang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (ShenHunData.GetSelfBianShenFashions().Count <= 0)
			{
				Super.HintMainText(Global.GetLang("您还未获得变身时装"), 10, 3);
			}
			else
			{
				this.SetState(ShenHunType.ShiZhuang);
			}
		};
	}

	public void SetState(ShenHunType type)
	{
		if (this.m_type != type)
		{
			this.m_type = type;
			this.SetButtonSelected(this.btnFuTi, this.m_type == ShenHunType.FuTi);
			this.SetButtonSelected(this.btnShiZhuang, this.m_type == ShenHunType.ShiZhuang);
			this.futiWindow.gameObject.SetActive(this.m_type == ShenHunType.FuTi);
			this.shizhuangWindow.gameObject.SetActive(this.m_type == ShenHunType.ShiZhuang);
			if (this.m_type == ShenHunType.ShiZhuang)
			{
				this.shizhuangWindow.RefershInofs();
			}
		}
	}

	private void SetButtonSelected(GButton btn, bool beSelected)
	{
		string spriteName = (!beSelected) ? "btn2" : "btn1";
		Global.SetButtonSprite(btn, spriteName);
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton btnClose;

	public GButton btnFuTi;

	public GButton btnShiZhuang;

	public ShenHunPartFuTi futiWindow;

	public ShenHunPartShiZhuang shizhuangWindow;

	public ShenHunPartSkillDetail skillDetailWindow;

	private ShenHunType m_type;
}
