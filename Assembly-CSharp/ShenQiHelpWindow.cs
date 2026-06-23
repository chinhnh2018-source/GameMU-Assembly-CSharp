using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ShenQiHelpWindow : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseCallback != null)
			{
				this.CloseCallback(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	private void InitTextInPrefabs()
	{
		this.m_Rule.Text = Global.GetLang("注入规则：");
		this.m_RuleContent.Text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("每次注入有几率产生")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("普通暴击，超级暴击")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("获得更多\n的属性成长。")
		}));
		this.m_XiaoHao.Text = Global.GetLang("额外消耗：");
		this.m_XiaoHaoContent.Text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("注入时勾选消耗钻石，必定产生")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("暴击效果。")
		}));
		this.m_RuleContent.MaxWidth = 410.0;
		this.m_XiaoHaoContent.MaxWidth = 410.0;
	}

	protected override void OnDestroy()
	{
		this.m_CloseBtn = null;
		this.CloseCallback = null;
		this.m_Rule = null;
		this.m_RuleContent = null;
		this.m_XiaoHao = null;
		this.m_XiaoHaoContent = null;
	}

	public GButton m_CloseBtn;

	public DPSelectedItemEventHandler CloseCallback;

	public TextBlock m_Rule;

	public TextBlock m_RuleContent;

	public TextBlock m_XiaoHao;

	public TextBlock m_XiaoHaoContent;
}
