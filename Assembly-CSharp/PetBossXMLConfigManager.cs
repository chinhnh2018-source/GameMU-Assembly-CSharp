using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class PetBossXMLConfigManager
{
	private static void LoadXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/PetBoss.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "PetBoss");
		int count = xelementList.Count;
		for (int i = 0; i < count; i++)
		{
			PetBossXMLData petBossXMLData = new PetBossXMLData();
			petBossXMLData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			petBossXMLData.MonsterID = Global.GetXElementAttributeInt(xelementList[i], "MonsterID");
			petBossXMLData.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			petBossXMLData.Star = Global.GetXElementAttributeInt(xelementList[i], "Star");
			petBossXMLData.Quality = Global.GetXElementAttributeInt(xelementList[i], "Quality");
			petBossXMLData.FreeRate = Global.GetXElementAttributeStr(xelementList[i], "FreeRate");
			petBossXMLData.ZuanRate = Global.GetXElementAttributeStr(xelementList[i], "ZuanRate");
			petBossXMLData.Time = Global.GetXElementAttributeInt(xelementList[i], "Time");
			petBossXMLData.PetLevelStep = Global.GetXElementAttributeInt(xelementList[i], "PetLevelStep");
			petBossXMLData.PetLevelStepNum = Global.GetXElementAttributeStr(xelementList[i], "PetLevelStepNum");
			petBossXMLData.ExcellentStep = Global.GetXElementAttributeInt(xelementList[i], "ExcellentStep");
			petBossXMLData.ExcellentStepNum = Global.GetXElementAttributeStr(xelementList[i], "ExcellentStepNum");
			petBossXMLData.PetSuit = Global.GetXElementAttributeStr(xelementList[i], "PetSuit");
			petBossXMLData.PetRate = Global.GetXElementAttributeStr(xelementList[i], "PetRate");
			petBossXMLData.SuitRate = Global.GetXElementAttributeInt(xelementList[i], "SuitRate");
			petBossXMLData.FightAward = Global.GetXElementAttributeStr(xelementList[i], "FightAward");
			petBossXMLData.KillAward = Global.GetXElementAttributeStr(xelementList[i], "KillAward");
			petBossXMLData.KillExtraAward = Global.GetXElementAttributeStr(xelementList[i], "KillExtraAward");
			petBossXMLData.Scaling = Global.GetXElementAttributeFloat(xelementList[i], "Scaling");
			if (!PetBossXMLConfigManager.petBossXMLDict.ContainsKey(petBossXMLData.ID))
			{
				PetBossXMLConfigManager.petBossXMLDict.Add(petBossXMLData.ID, petBossXMLData);
			}
		}
	}

	public static PetBossXMLData GetDataByID(int id)
	{
		if (PetBossXMLConfigManager.petBossXMLDict.Count > 0 && PetBossXMLConfigManager.petBossXMLDict.ContainsKey(id))
		{
			return PetBossXMLConfigManager.petBossXMLDict[id];
		}
		PetBossXMLConfigManager.LoadXML();
		if (PetBossXMLConfigManager.petBossXMLDict.ContainsKey(id))
		{
			return PetBossXMLConfigManager.petBossXMLDict[id];
		}
		Super.HintMainText(Global.GetLang(Global.GetLang("PetBoss.XML 不包含该ID！")), 10, 3);
		return new PetBossXMLData();
	}

	public static void Clear()
	{
		if (PetBossXMLConfigManager.petBossXMLDict.Count > 0)
		{
			PetBossXMLConfigManager.petBossXMLDict.Clear();
		}
	}

	private static Dictionary<int, PetBossXMLData> petBossXMLDict = new Dictionary<int, PetBossXMLData>();
}
