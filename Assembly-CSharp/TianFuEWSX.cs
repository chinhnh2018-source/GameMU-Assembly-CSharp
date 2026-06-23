using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Tmsk.Xml;

public class TianFuEWSX : UserControl
{
	protected override void InitializeComponent()
	{
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.gameObject.SetActive(false);
		};
		this.InitStr();
	}

	private void InitStr()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("额外加成")
		});
		this.Title_Pro.text = Global.GetLang("激活属性");
	}

	private new void Start()
	{
	}

	public void SetProPoint(int ym, int jr, int jm)
	{
		this.YM_Point.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			ym.ToString()
		});
		this.JM_Point.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			jm.ToString()
		});
		this.JR_Point.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			jr.ToString()
		});
	}

	public void SetEWSX_Pro()
	{
		Dictionary<int, double> specialProp = this.GetSpecialProp(Global.Data.roleData.MyTalentData);
		if (specialProp == null || specialProp.Count == 0)
		{
			return;
		}
		double num = specialProp[61] * 100.0;
		double num2 = specialProp[62] * 100.0;
		double num3 = specialProp[63] * 100.0;
		double num4 = specialProp[64] * 100.0;
		double num5 = specialProp[65] * 100.0;
		double num6 = specialProp[66] * 100.0;
		Item item = new Item();
		item.ShuXing_1 = (float)num2 / 35f;
		item.ShuXing_2 = (float)num3 / 35f;
		item.ShuXing_3 = (float)num4 / 35f;
		item.ShuXing_4 = (float)num5 / 35f;
		item.ShuXing_5 = (float)num6 / 35f;
		item.ShuXing_6 = (float)num / 35f;
		this.LiuJiaTu.setLiuJiaoXing(item);
		this.EWSX_Pro.text = string.Empty;
		if (num > 0.0)
		{
			UILabel ewsx_Pro = this.EWSX_Pro;
			ewsx_Pro.text = ewsx_Pro.text + Global.GetColorStringForNGUIText(new object[]
			{
				"26ebc9",
				Global.GetLang("野蛮一击率") + string.Format("{0}{1}{2}{3}", new object[]
				{
					":",
					(num < 0.0) ? string.Empty : "+",
					num.ToString(),
					"%"
				})
			}) + "\n";
		}
		if (num2 > 0.0)
		{
			UILabel ewsx_Pro2 = this.EWSX_Pro;
			ewsx_Pro2.text = ewsx_Pro2.text + Global.GetColorStringForNGUIText(new object[]
			{
				"26ebc9",
				Global.GetLang("冷血一击率") + string.Format("{0}{1}{2}{3}", new object[]
				{
					":",
					(num2 < 0.0) ? string.Empty : "+",
					num2.ToString(),
					"%"
				})
			}) + "\n";
		}
		if (num3 > 0.0)
		{
			UILabel ewsx_Pro3 = this.EWSX_Pro;
			ewsx_Pro3.text = ewsx_Pro3.text + Global.GetColorStringForNGUIText(new object[]
			{
				"26ebc9",
				Global.GetLang("无情一击率") + string.Format("{0}{1}{2}{3}", new object[]
				{
					":",
					(num3 < 0.0) ? string.Empty : "+",
					num3.ToString(),
					"%"
				})
			}) + "\n";
		}
		if (num4 > 0.0)
		{
			UILabel ewsx_Pro4 = this.EWSX_Pro;
			ewsx_Pro4.text = ewsx_Pro4.text + Global.GetColorStringForNGUIText(new object[]
			{
				"26ebc9",
				Global.GetLang("抵抗野蛮一击率") + string.Format("{0}{1}{2}{3}", new object[]
				{
					":",
					(num4 < 0.0) ? string.Empty : "+",
					num4.ToString(),
					"%"
				})
			}) + "\n";
		}
		if (num5 > 0.0)
		{
			UILabel ewsx_Pro5 = this.EWSX_Pro;
			ewsx_Pro5.text = ewsx_Pro5.text + Global.GetColorStringForNGUIText(new object[]
			{
				"26ebc9",
				Global.GetLang("抵抗冷血一击率") + string.Format("{0}{1}{2}{3}", new object[]
				{
					":",
					(num5 < 0.0) ? string.Empty : "+",
					num5.ToString(),
					"%"
				})
			}) + "\n";
		}
		if (num6 > 0.0)
		{
			UILabel ewsx_Pro6 = this.EWSX_Pro;
			ewsx_Pro6.text += Global.GetColorStringForNGUIText(new object[]
			{
				"26ebc9",
				Global.GetLang("抵抗无情一击率") + string.Format("{0}{1}{2}{3}", new object[]
				{
					":",
					(num6 < 0.0) ? string.Empty : "+",
					num6.ToString(),
					"%"
				})
			});
		}
	}

	private Dictionary<int, double> GetSpecialProp(TalentData talentData)
	{
		XElement gameResXml = Global.GetGameResXml("Config/TianFuGroupProperty.Xml");
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		dictionary.Add(61, 0.0);
		dictionary.Add(62, 0.0);
		dictionary.Add(63, 0.0);
		dictionary.Add(64, 0.0);
		dictionary.Add(65, 0.0);
		dictionary.Add(66, 0.0);
		if (talentData.CountList == null || talentData.CountList.Count <= 0)
		{
			return dictionary;
		}
		foreach (KeyValuePair<int, int> keyValuePair in talentData.CountList)
		{
			int key = keyValuePair.Key;
			int num = keyValuePair.Value;
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "TianFuSpecialAttribute", "TianFuType", key.ToString());
			XElement xelement = xelementList[0];
			int num2 = int.Parse(xelement.GetXElementAttrStr("NeedExp"));
			num /= num2;
			string xelementAttrStr = xelement.GetXElementAttrStr("TripleAttack");
			string xelementAttrStr2 = xelement.GetXElementAttrStr("SlowAttack");
			string xelementAttrStr3 = xelement.GetXElementAttrStr("VampiricAttack");
			string xelementAttrStr4 = xelement.GetXElementAttrStr("TripleDefense");
			string xelementAttrStr5 = xelement.GetXElementAttrStr("SlowDefensee");
			string xelementAttrStr6 = xelement.GetXElementAttrStr("VampiricDefense");
			Dictionary<int, double> dictionary3;
			Dictionary<int, double> dictionary2 = dictionary3 = dictionary;
			int num4;
			int num3 = num4 = 61;
			double num5 = dictionary3[num4];
			dictionary2[num3] = num5 + double.Parse(xelementAttrStr) * (double)num;
			Dictionary<int, double> dictionary5;
			Dictionary<int, double> dictionary4 = dictionary5 = dictionary;
			int num6 = num4 = 62;
			num5 = dictionary5[num4];
			dictionary4[num6] = num5 + double.Parse(xelementAttrStr2) * (double)num;
			Dictionary<int, double> dictionary7;
			Dictionary<int, double> dictionary6 = dictionary7 = dictionary;
			int num7 = num4 = 63;
			num5 = dictionary7[num4];
			dictionary6[num7] = num5 + double.Parse(xelementAttrStr3) * (double)num;
			Dictionary<int, double> dictionary9;
			Dictionary<int, double> dictionary8 = dictionary9 = dictionary;
			int num8 = num4 = 64;
			num5 = dictionary9[num4];
			dictionary8[num8] = num5 + double.Parse(xelementAttrStr4) * (double)num;
			Dictionary<int, double> dictionary11;
			Dictionary<int, double> dictionary10 = dictionary11 = dictionary;
			int num9 = num4 = 65;
			num5 = dictionary11[num4];
			dictionary10[num9] = num5 + double.Parse(xelementAttrStr5) * (double)num;
			Dictionary<int, double> dictionary13;
			Dictionary<int, double> dictionary12 = dictionary13 = dictionary;
			int num10 = num4 = 66;
			num5 = dictionary13[num4];
			dictionary12[num10] = num5 + double.Parse(xelementAttrStr6) * (double)num;
		}
		return dictionary;
	}

	public UILabel YM_Point;

	public UILabel JM_Point;

	public UILabel JR_Point;

	public UILabel EWSX_Pro;

	public UILabel Title;

	public GButton Close;

	public UILabel Title_Pro;

	public TianFuLiuJiaTu LiuJiaTu;
}
