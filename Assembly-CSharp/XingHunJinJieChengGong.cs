using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class XingHunJinJieChengGong : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnOk.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (null != this.m_BtnOk)
		{
			this.m_BtnOk.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			};
		}
	}

	public void InitShowText(string str)
	{
		if (null != this.m_LblShowText)
		{
			this.m_LblShowText.text = str;
		}
	}

	public GButton m_BtnOk;

	public UILabel m_LblShowText;

	public DPSelectedItemEventHandler DPSelectedItem;
}
