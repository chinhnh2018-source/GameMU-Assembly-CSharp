using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ShenXiangManager
{
	private static void ParseXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/God.xml");
		if (gameResXml == null)
		{
			MUDebug.Log<string>(new string[]
			{
				"缺少Config/God.xml"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "God");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ShenXiangData shenXiangData = default(ShenXiangData);
			shenXiangData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			shenXiangData.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			shenXiangData.GodIcon = Global.GetXElementAttributeStr(xelementList[i], "GodIcon");
			shenXiangData.OpenCondition = Global.GetXElementAttributeStr(xelementList[i], "OpenCondition");
			shenXiangData.ActivationProperty = Global.GetXElementAttributeStr(xelementList[i], "ActivationProperty");
			if (!ShenXiangManager.shenXiangList.Contains(shenXiangData))
			{
				ShenXiangManager.shenXiangList.Add(shenXiangData);
			}
			i++;
		}
	}

	public static List<ShenXiangData> GetShenXiangDataList()
	{
		if (ShenXiangManager.shenXiangList.Count > 0)
		{
			return ShenXiangManager.shenXiangList;
		}
		ShenXiangManager.ParseXML();
		return ShenXiangManager.shenXiangList;
	}

	public static ShenXiangData GetShenXiangDataByID(int ID)
	{
		if (ShenXiangManager.shenXiangList.Count <= 0)
		{
			ShenXiangManager.ParseXML();
		}
		ShenXiangData result = default(ShenXiangData);
		for (int i = 0; i < ShenXiangManager.shenXiangList.Count; i++)
		{
			if (ShenXiangManager.shenXiangList[i].ID == ID)
			{
				result = ShenXiangManager.shenXiangList[i];
				break;
			}
		}
		return result;
	}

	public static void Clear()
	{
		ShenXiangManager.shenXiangList.Clear();
	}

	private static List<ShenXiangData> shenXiangList = new List<ShenXiangData>();
}
