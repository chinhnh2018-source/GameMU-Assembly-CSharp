using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ShenQiPropertyManager
{
	private static void ParseXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Toughness.xml");
		if (gameResXml == null)
		{
			MUDebug.Log<string>(new string[]
			{
				"缺少Config/Toughness.xml"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Toughness");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenQiPropertyData shenQiPropertyData = default(ShenQiPropertyData);
			shenQiPropertyData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			shenQiPropertyData.Toughness = Global.GetXElementAttributeInt(xelementList[i], "Toughness");
			shenQiPropertyData.DeLucky = Global.GetXElementAttributeFloat(xelementList[i], "DeLucky");
			shenQiPropertyData.DeFatalAttack = Global.GetXElementAttributeFloat(xelementList[i], "DeFatalAttack");
			shenQiPropertyData.DeDoubleAttack = Global.GetXElementAttributeFloat(xelementList[i], "DeDoubleAttack");
			shenQiPropertyData.DeSavagePercent = Global.GetXElementAttributeFloat(xelementList[i], "DeSavagePercent");
			shenQiPropertyData.DeColdPercent = Global.GetXElementAttributeFloat(xelementList[i], "DeColdPercent");
			shenQiPropertyData.DeRuthlessPercent = Global.GetXElementAttributeFloat(xelementList[i], "DeRuthlessPercent");
			shenQiPropertyData.DeFrozenPercent = Global.GetXElementAttributeFloat(xelementList[i], "DeFrozenPercent");
			shenQiPropertyData.DePalsyPercent = Global.GetXElementAttributeFloat(xelementList[i], "DePalsyPercent");
			shenQiPropertyData.DeSpeedDownPercent = Global.GetXElementAttributeFloat(xelementList[i], "DeSpeedDownPercent");
			shenQiPropertyData.DeBlowPercent = Global.GetXElementAttributeFloat(xelementList[i], "DeBlowPercent");
			if (!ShenQiPropertyManager.shenQiPropertyList.Contains(shenQiPropertyData))
			{
				ShenQiPropertyManager.shenQiPropertyList.Add(shenQiPropertyData);
			}
			i++;
		}
	}

	public static List<ShenQiPropertyData> GetShenQiPropertyDataList()
	{
		if (ShenQiPropertyManager.shenQiPropertyList.Count > 0)
		{
			return ShenQiPropertyManager.shenQiPropertyList;
		}
		ShenQiPropertyManager.ParseXML();
		return ShenQiPropertyManager.shenQiPropertyList;
	}

	public static ShenQiPropertyData GetShenQiPropertyDataByID(int ID)
	{
		if (ShenQiPropertyManager.shenQiPropertyList.Count <= 0)
		{
			ShenQiPropertyManager.ParseXML();
		}
		ShenQiPropertyData result = default(ShenQiPropertyData);
		for (int i = 0; i < ShenQiPropertyManager.shenQiPropertyList.Count; i++)
		{
			if (ShenQiPropertyManager.shenQiPropertyList[i].ID == ID)
			{
				result = ShenQiPropertyManager.shenQiPropertyList[i];
				break;
			}
		}
		return result;
	}

	public static void Clear()
	{
		ShenQiPropertyManager.shenQiPropertyList.Clear();
	}

	private static List<ShenQiPropertyData> shenQiPropertyList = new List<ShenQiPropertyData>();
}
