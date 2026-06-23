using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class AdendaLevelupExtraPropertyController : MonoBehaviour
{
	private void Awake()
	{
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.gameObject.SetActive(false);
		};
	}

	public void SetLevelupExtraProperty()
	{
		this.RefreshValues();
	}

	private XElement GetXElementByID(int id)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShengWangSpecialAttribute.xml");
		return Global.GetXElement(gameResXml, "ShengWangSpecialAttribute", "ID", id.ToString());
	}

	private void RefreshValues()
	{
		int medalID = Global.Data.adendaData.MedalID;
		XElement xelementByID = this.GetXElementByID((medalID > 1) ? (medalID - 1) : 1);
		XElement xelementByID2 = this.GetXElementByID((medalID <= 6) ? medalID : (medalID - 1));
		float xelementAttributeFloat = Global.GetXElementAttributeFloat(xelementByID, "ZhiMingYiJi");
		float xelementAttributeFloat2 = Global.GetXElementAttributeFloat(xelementByID, "DiKangZhiMingYiJi");
		float xelementAttributeFloat3 = Global.GetXElementAttributeFloat(xelementByID2, "ZhiMingYiJi");
		float xelementAttributeFloat4 = Global.GetXElementAttributeFloat(xelementByID2, "DiKangZhiMingYiJi");
		this.SetLevelTitle(Mathf.Max(1, medalID - 1), 0);
		this.SetProperties(xelementAttributeFloat, xelementAttributeFloat2, 0, medalID > 1);
		this.SetLevelTitle(Mathf.Min(6, medalID), 1);
		this.SetProperties(xelementAttributeFloat3, xelementAttributeFloat4, 1, true);
		this.SetNextLevelGroupVisible(medalID > 1 && medalID <= 6);
	}

	private void SetLevelTitle(int medal_num, int level = 0)
	{
		string text = string.Format(Global.GetLang("拥有满级勋章：{0}枚"), medal_num);
		if (level == 0 && null != this.curLevelTitle)
		{
			this.curLevelTitle.text = ((!string.IsNullOrEmpty(text)) ? text : string.Empty);
		}
		else if (level == 1 && null != this.nextLevelTitle)
		{
			this.nextLevelTitle.text = ((!string.IsNullOrEmpty(text)) ? text : string.Empty);
		}
	}

	private void SetProperties(float value_field_1, float value_field_2, int level = 0, bool default_font_color = true)
	{
		if (level == 0)
		{
			if (null != this.criticalHitValue_currentLevel)
			{
				this.criticalHitValue_currentLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					(!default_font_color) ? "808081" : "17e43e",
					string.Format(Global.GetLang("致命一击率：+{0}%"), 100f * value_field_1)
				});
			}
			if (null != this.antiCriticalHitValue_currentLevel)
			{
				this.antiCriticalHitValue_currentLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					(!default_font_color) ? "808081" : "17e43e",
					string.Format(Global.GetLang("抵抗致命一击率：+{0}%"), 100f * value_field_2)
				});
				this.antiCriticalHitValue_currentLevel.gameObject.SetActive(value_field_2 > 0f);
			}
		}
		else if (level == 1)
		{
			if (null != this.criticalHitValue_nextLevel)
			{
				this.criticalHitValue_nextLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					string.Format(Global.GetLang("致命一击率：+{0}%"), 100f * value_field_1)
				});
			}
			if (null != this.antiCriticalHitValue_nextLevel)
			{
				this.antiCriticalHitValue_nextLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"808081",
					string.Format(Global.GetLang("抵抗致命一击率：+{0}%"), 100f * value_field_2)
				});
				this.antiCriticalHitValue_nextLevel.gameObject.SetActive(value_field_2 > 0f);
			}
		}
	}

	private void SetNextLevelGroupVisible(bool visible = true)
	{
		if (null != this.nextLevelTitle)
		{
			this.nextLevelTitle.transform.parent.gameObject.SetActive(visible);
		}
	}

	private const string fontColor_currentLevel = "17e43e";

	private const string fontColor_nextLevel = "808081";

	public UILabel curLevelTitle;

	public UILabel nextLevelTitle;

	public UILabel criticalHitValue_currentLevel;

	public UILabel antiCriticalHitValue_currentLevel;

	public UILabel criticalHitValue_nextLevel;

	public UILabel antiCriticalHitValue_nextLevel;

	public GButton closeBtn;
}
