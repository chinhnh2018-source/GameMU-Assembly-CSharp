using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class FontSizeMgr
{
	public static string GetXmlFontName(XElement xml, string name, string defVal)
	{
		if (xml == null)
		{
			return defVal;
		}
		XElement xelement = Global.GetXElement(xml, "Font", "Name", name);
		if (xelement == null)
		{
			return defVal;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Value");
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			return defVal;
		}
		return xelementAttributeStr;
	}

	public static int GetXmlFontSize(XElement xml, string name, int defVal)
	{
		if (xml == null)
		{
			return defVal;
		}
		XElement xelement = Global.GetXElement(xml, "Font", "Name", name);
		if (xelement == null)
		{
			return defVal;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Value");
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			return defVal;
		}
		return Convert.ToInt32(xelementAttributeStr);
	}

	public static void LoadFontSize()
	{
		if (Global.GameLang <= 0)
		{
			return;
		}
		XElement langResXml = Global.GetLangResXml("FontSet.Xml");
		if (langResXml == null)
		{
			return;
		}
		FontSizeMgr.HSTextFieldFontName = FontSizeMgr.GetXmlFontName(langResXml, "HSTextFieldFontName", FontSizeMgr.HSTextFieldFontName);
		HSTextField.fontName = FontSizeMgr.HSTextFieldFontName;
		FontSizeMgr.GBulletinFontName = FontSizeMgr.GetXmlFontName(langResXml, "GBulletinFontName", FontSizeMgr.GBulletinFontName);
		FontSizeMgr.GNoticeFontName = FontSizeMgr.GetXmlFontName(langResXml, "GNoticeFontName", FontSizeMgr.GNoticeFontName);
		FontSizeMgr.GToolTipFontName = FontSizeMgr.GetXmlFontName(langResXml, "GToolTipFontName", FontSizeMgr.GToolTipFontName);
		FontSizeMgr.WindowTitleFontName = FontSizeMgr.GetXmlFontName(langResXml, "WindowTitleFontName", FontSizeMgr.WindowTitleFontName);
		FontSizeMgr.HSTextFieldFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "HSTextFieldFontSize", FontSizeMgr.HSTextFieldFontSize);
		HSTextField.defaultFontSize = FontSizeMgr.HSTextFieldFontSize;
		FontSizeMgr.CDCollDownFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "CDCollDownFontSize", FontSizeMgr.CDCollDownFontSize);
		FontSizeMgr.NormalInputFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "NormalInputFontSize", FontSizeMgr.NormalInputFontSize);
		FontSizeMgr.LineItemTextFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "LineItemTextFontSize", FontSizeMgr.LineItemTextFontSize);
		FontSizeMgr.FacePlateValueFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "FacePlateValueFontSize", FontSizeMgr.FacePlateValueFontSize);
		FontSizeMgr.QuickKeyBarNumFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "QuickKeyBarNumFontSize", FontSizeMgr.QuickKeyBarNumFontSize);
		FontSizeMgr.NPCNameFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "NPCNameFontSize", FontSizeMgr.NPCNameFontSize);
		FontSizeMgr.NPCTalkFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "NPCTalkFontSize", FontSizeMgr.NPCTalkFontSize);
		FontSizeMgr.NPCOperationFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "NPCOperationFontSize", FontSizeMgr.NPCOperationFontSize);
		FontSizeMgr.LingDiUsedSecsFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "LingDiUsedSecsFontSize", FontSizeMgr.LingDiUsedSecsFontSize);
		FontSizeMgr.FirstWorldTitleFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "FirstWorldTitleFontSize", FontSizeMgr.FirstWorldTitleFontSize);
		FontSizeMgr.BattleBarNameFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "BattleBarNameFontSize", FontSizeMgr.BattleBarNameFontSize);
		FontSizeMgr.RadarTitleFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "RadarTitleFontSize", FontSizeMgr.RadarTitleFontSize);
		FontSizeMgr.UserLoginWarningFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "UserLoginWarningFontSize", FontSizeMgr.UserLoginWarningFontSize);
		FontSizeMgr.GBulletinFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "GBulletinFontSize", FontSizeMgr.GBulletinFontSize);
		FontSizeMgr.TipServiceTitleFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "TipServiceTitleFontSize", FontSizeMgr.TipServiceTitleFontSize);
		FontSizeMgr.SystemHintTextSmallFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "SystemHintTextSmallFontSize", FontSizeMgr.SystemHintTextSmallFontSize);
		FontSizeMgr.SystemHintTextBigFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "SystemHintTextBigFontSize", FontSizeMgr.SystemHintTextBigFontSize);
		FontSizeMgr.WindowTitleFontSize = FontSizeMgr.GetXmlFontSize(langResXml, "WindowTitleFontSize", FontSizeMgr.WindowTitleFontSize);
	}

	public static string HSTextFieldFontName = "SimSun";

	public static string GBulletinFontName = "SimSun";

	public static string GNoticeFontName = "SimSun";

	public static string GToolTipFontName = "SimSun";

	public static string WindowTitleFontName = "SimSun";

	public static int HSTextFieldFontSize = 12;

	public static int CDCollDownFontSize = 14;

	public static int NormalInputFontSize = 13;

	public static int LineItemTextFontSize = 14;

	public static int FacePlateValueFontSize = 10;

	public static int QuickKeyBarNumFontSize = 10;

	public static int NPCNameFontSize = 12;

	public static int NPCTalkFontSize = 12;

	public static int NPCOperationFontSize = 14;

	public static int LingDiUsedSecsFontSize = 14;

	public static int FirstWorldTitleFontSize = 16;

	public static int BattleBarNameFontSize = 16;

	public static int RadarTitleFontSize = 14;

	public static int UserLoginWarningFontSize = 14;

	public static int GBulletinFontSize = 12;

	public static int TipServiceTitleFontSize = 12;

	public static int SystemHintTextSmallFontSize = 14;

	public static int SystemHintTextBigFontSize = 16;

	public static int MapNameFontSize = 36;

	public static int WindowTitleFontSize = 16;
}
