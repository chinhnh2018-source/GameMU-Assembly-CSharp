using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class XingHunTips : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_Bak.localPosition = new Vector3(0f, 0f, 1f);
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.CloseXingHunTips();
			};
		}
	}

	public void InitLabel(string strTopLblText, string strBottomLblText)
	{
		this.m_LblTop.text = strTopLblText;
		this.m_LblBottom.text = strBottomLblText;
	}

	public GButton m_BtnClose;

	public UILabel m_LblTop;

	public UILabel m_LblBottom;

	public Transform m_Bak;
}
