using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ZhuTiFuDuiHuanItem : UserControl
{
	public int ID
	{
		get
		{
			return this.m_ID;
		}
		set
		{
			this.m_ID = value;
		}
	}

	public int Number
	{
		set
		{
			this.m_LabNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("今日可兑换：") + value
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("兑换");
	}

	public ListBox m_ListGoods;

	public GameObject m_GameGoods;

	public GButton m_Btn;

	public UILabel m_LabNumber;

	private int m_ID = -1;
}
