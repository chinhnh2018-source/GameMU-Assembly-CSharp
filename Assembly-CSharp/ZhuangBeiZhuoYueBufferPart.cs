using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ZhuangBeiZhuoYueBufferPart : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this.ConstTexts != null && this.ConstTexts.Length == 2)
		{
			this.ConstTexts[0].Text = Global.GetLang("卓越效果加成");
			this.ConstTexts[1].Text = Global.GetLang("加成效果:");
		}
		this.m_txtFirstBuffValue.X = 10.0;
		this.m_txtSecondBuffValue.X = 10.0;
		this.m_txtThirdBuffValue.X = 10.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
	}

	public void RefreshUI(int currentZhuoYueLevel)
	{
		string text = "757575";
		string text2 = "f9f702";
		double[] zhuoYueBuffs = Global.GetZhuoYueBuffs();
		switch (currentZhuoYueLevel + 1)
		{
		case 0:
			this.m_txtFirstBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件绿色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[0] * 100.0))
			});
			this.m_txtSecondBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件蓝色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[1] * 100.0))
			});
			this.m_txtThirdBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件紫色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[2] * 100.0))
			});
			break;
		case 1:
			this.m_txtFirstBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				string.Format(Global.GetLang("任意8件绿色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[0] * 100.0))
			});
			this.m_txtSecondBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件蓝色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[1] * 100.0))
			});
			this.m_txtThirdBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件紫色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[2] * 100.0))
			});
			break;
		case 2:
			this.m_txtFirstBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件绿色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[0] * 100.0))
			});
			this.m_txtSecondBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				string.Format(Global.GetLang("任意8件蓝色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[1] * 100.0))
			});
			this.m_txtThirdBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件紫色卓越装 伤害减少+{0}%"), (int)(zhuoYueBuffs[2] * 100.0))
			});
			break;
		case 3:
			this.m_txtFirstBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件绿色卓越装 伤害减少+{0}%"), zhuoYueBuffs[0] * 100.0)
			});
			this.m_txtSecondBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("任意8件蓝色卓越装 伤害减少+{0}%"), zhuoYueBuffs[1] * 100.0)
			});
			this.m_txtThirdBuffValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				string.Format(Global.GetLang("任意8件紫色卓越装 伤害减少+{0}%"), zhuoYueBuffs[2] * 100.0)
			});
			break;
		}
	}

	public TextBlock m_txtFirstBuffValue;

	public TextBlock m_txtSecondBuffValue;

	public TextBlock m_txtThirdBuffValue;

	public GButton CloseBtn;

	public TextBlock[] ConstTexts;

	public DPSelectedItemEventHandler DPSelectedItem;
}
