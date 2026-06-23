using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class HongBaoChongZhiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_BtnConfig.Text = Global.GetLang("充值");
		this.m_LabContent.transform.localPosition = new Vector3(-155f, 60f, 0f);
		this.m_LabContent.lineWidth = 455;
		this.m_BtnConfig.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		};
		base.InitializeComponent();
	}

	public void SetXml(string str = "")
	{
		if (string.IsNullOrEmpty(str))
		{
			return;
		}
		XElement xelement = XElement.Parse(str);
		XElement xelement2 = Global.GetXElement(xelement, "Activities");
		this.m_LabTime1.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间：")
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetXElementAttributeStr(xelement2, "FromDate") + Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("至")
			}) + Global.GetXElementAttributeStr(xelement2, "ToDate")
		}));
		this.m_LabContentTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容：")
		}));
		this.m_LabContent.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetXElementAttributeStr(Global.GetXElement(xelement, "GiftList"), "Description")
		}));
	}

	public UILabel m_LabTime1;

	public UILabel m_LabContentTitle;

	public UILabel m_LabContent;

	public GButton m_BtnConfig;
}
