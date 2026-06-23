using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class TaLuoPaiTeQuan2Part : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_BtnLeft.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			string.Format("{0}", Global.GetLang("确定"))
		});
		this.m_BtnRight.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			string.Format("{0}", Global.GetLang("取消"))
		});
		this.m_Lablel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			string.Format("{0}", Global.GetLang("重置后将失去国王特权效果"))
		});
		this.m_BtnRight.Label.transform.localPosition = new Vector3(this.m_BtnRight.Label.transform.localPosition.x, this.m_BtnRight.Label.transform.localPosition.y, -1f);
		this.m_BtnLeft.Label.transform.localPosition = new Vector3(this.m_BtnLeft.Label.transform.localPosition.x, this.m_BtnLeft.Label.transform.localPosition.y, -1f);
	}

	public GButton m_BtnLeft;

	public GButton m_BtnRight;

	public UILabel m_Lablel;
}
