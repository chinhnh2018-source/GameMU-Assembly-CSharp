using System;
using HSGameEngine.GameEngine.Logic;

public class JiYuanPaiHangItem : UserControl
{
	public int SetMingCi
	{
		set
		{
			if (value <= 3)
			{
				this.m_SpMingCi.gameObject.SetActive(true);
				this.m_SpMingCi.spriteName = value.ToString();
				this.m_MingCi.gameObject.SetActive(false);
			}
			else
			{
				this.m_SpMingCi.gameObject.SetActive(false);
				this.m_MingCi.gameObject.SetActive(true);
				this.m_MingCi.text = value.ToString();
			}
		}
	}

	public string SetName
	{
		set
		{
			this.m_MingZi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(value)
			});
		}
	}

	public string SetJinDu
	{
		set
		{
			this.m_JinDu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(value)
			});
		}
	}

	public UISprite m_SpMingCi;

	public UILabel m_MingCi;

	public UILabel m_MingZi;

	public UILabel m_JinDu;
}
