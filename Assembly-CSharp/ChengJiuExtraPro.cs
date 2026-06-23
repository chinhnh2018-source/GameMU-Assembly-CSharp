using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class ChengJiuExtraPro : MonoBehaviour
{
	private void Awake()
	{
		this.Title.text = Global.GetLang("符文额外加成");
		this.curLabel[0].text = Global.GetLang("当前效果");
		this.lowerLabel[0].text = Global.GetLang("下级效果");
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.gameObject.SetActive(false);
		};
	}

	private void Start()
	{
	}

	public void setFuWenExtarProUI()
	{
		if (Global.Data.ChengjiuFuWen == null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ChengJiuSpecialAttribute.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		if (Global.Data.ChengjiuFuWen.UpResultType == 3)
		{
			XElement xmle = this.getXmle(Global.Data.ChengjiuFuWen.RuneID - 1);
			float cur_ = float.Parse(Global.GetXElementAttributeStr(xmle, "DiKangZhuoYueYiJi"));
			float cur_2 = float.Parse(Global.GetXElementAttributeStr(xmle, "ZhuoYueYiJi"));
			this.Pro(cur_, cur_2, string.Empty);
			this.LowerTitle.gameObject.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			this.upDateUI(xelementList);
		}
	}

	private XElement getXmle(int id)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ChengJiuSpecialAttribute.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		foreach (XElement xelement in xelementList)
		{
			int num = int.Parse(Global.GetXElementAttributeStr(xelement, "ID"));
			if (id == num)
			{
				return xelement;
			}
		}
		return null;
	}

	private void upDateUI(List<XElement> ChengJiuData)
	{
		int id = Global.Data.ChengjiuFuWen.RuneID - 1;
		XElement xmle = this.getXmle(id);
		XElement xmle2 = this.getXmle(Global.Data.ChengjiuFuWen.RuneID);
		bool flag;
		if (xmle != null)
		{
			this.curLabel[0].text = Global.GetLang("当前效果");
			flag = this.Pro(float.Parse(Global.GetXElementAttributeStr(xmle, "DiKangZhuoYueYiJi")), float.Parse(Global.GetXElementAttributeStr(xmle, "ZhuoYueYiJi")), string.Empty);
		}
		else
		{
			flag = this.Pro(0f, 0f, string.Empty);
		}
		if (xmle2 != null)
		{
			if (!flag)
			{
				this.curLabel[0].text = Global.GetLang("下级效果");
				this.Pro(float.Parse(Global.GetXElementAttributeStr(xmle2, "DiKangZhuoYueYiJi")), float.Parse(Global.GetXElementAttributeStr(xmle2, "ZhuoYueYiJi")), Global.GetLang("需要当前符文提升满"));
				this.lowerLabel[0].gameObject.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				this.lowerLabel[0].gameObject.transform.parent.gameObject.SetActive(true);
				this.LastPro(Global.GetLang("需要当前符文提升满"), float.Parse(Global.GetXElementAttributeStr(xmle2, "DiKangZhuoYueYiJi")), float.Parse(Global.GetXElementAttributeStr(xmle2, "ZhuoYueYiJi")));
			}
		}
	}

	public bool Pro(float cur_1, float cur_2, string Condition = "")
	{
		if (cur_1 == 0f && cur_2 == 0f)
		{
			return false;
		}
		float num = cur_1 * 100f;
		float num2 = cur_2 * 100f;
		if (Condition == string.Empty)
		{
			if (cur_1 == 0f)
			{
				this.curLabel[1].text = ((cur_2 != 0f) ? Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(this.strMax_2, num2.ToString())
				}) : string.Empty);
				this.curLabel[2].text = string.Empty;
				this.curLabel[3].text = string.Empty;
			}
			else
			{
				this.curLabel[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(this.strMax_1, num.ToString())
				});
				this.curLabel[1].text = ((cur_2 != 0f) ? Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(this.strMax_2, num2.ToString())
				}) : string.Empty);
			}
		}
		else
		{
			this.curLabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				this.fontColor_1,
				string.Format(this.strMax, Condition.ToString())
			});
			if (cur_1 == 0f)
			{
				this.curLabel[2].text = ((cur_2 != 0f) ? Global.GetColorStringForNGUIText(new object[]
				{
					this.fontColor_1,
					string.Format(this.strMax_2, num2.ToString())
				}) : string.Empty);
			}
			else
			{
				this.curLabel[3].text = Global.GetColorStringForNGUIText(new object[]
				{
					this.fontColor_1,
					string.Format(this.strMax_1, num.ToString())
				});
				this.curLabel[2].text = ((cur_2 != 0f) ? Global.GetColorStringForNGUIText(new object[]
				{
					this.fontColor_1,
					string.Format(this.strMax_2, num2.ToString())
				}) : string.Empty);
			}
		}
		return true;
	}

	public void LastPro(string Condition, float lower_1, float lower_2)
	{
		this.lowerLabel[1].text = ((!(Condition == string.Empty)) ? Global.GetColorStringForNGUIText(new object[]
		{
			this.fontColor_1,
			string.Format(this.strMax, Condition)
		}) : string.Empty);
		float num = lower_1 * 100f;
		float num2 = lower_2 * 100f;
		if (lower_1 == 0f)
		{
			this.lowerLabel[2].text = ((lower_2 != 0f) ? Global.GetColorStringForNGUIText(new object[]
			{
				this.fontColor_1,
				string.Format(this.strMax_2, num2.ToString())
			}) : string.Empty);
		}
		else
		{
			this.lowerLabel[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				this.fontColor_1,
				string.Format(this.strMax_1, num.ToString())
			});
			this.lowerLabel[2].text = ((lower_2 != 0f) ? Global.GetColorStringForNGUIText(new object[]
			{
				this.fontColor_1,
				string.Format(this.strMax_2, num2.ToString())
			}) : string.Empty);
		}
	}

	public UILabel CurPro_1;

	public UILabel CurPro_2;

	public UILabel LowerProCondition;

	public UILabel LowerPro_1;

	public UILabel LowerPro_2;

	public UILabel CurTitle;

	public UILabel LowerTitle;

	public List<UILabel> curLabel = new List<UILabel>();

	public List<UILabel> lowerLabel = new List<UILabel>();

	public GButton closeBtn;

	private string strMax = Global.GetLang("{0}【未激活】");

	private string strMax_1 = Global.GetLang("抵抗卓越一击率：+{0}%");

	private string strMax_2 = Global.GetLang("卓越一击几率：+{0}%");

	private string fontColor_1 = "808081";

	public UILabel Title;
}
