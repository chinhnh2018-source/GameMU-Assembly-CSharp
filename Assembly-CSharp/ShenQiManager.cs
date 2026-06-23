using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ShenQiManager
{
	private static void LoadXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Artifact.xml");
		if (gameResXml == null)
		{
			MUDebug.Log<string>(new string[]
			{
				"缺少Config/Artifact.xml"
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Artifact");
		int count = xelementList.Count;
		for (int i = 0; i < count; i++)
		{
			ShenQiXMLData shenQiXMLData = default(ShenQiXMLData);
			shenQiXMLData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			shenQiXMLData.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			shenQiXMLData.ModID = Global.GetXElementAttributeInt(xelementList[i], "ModID");
			shenQiXMLData.ArtifactIcon = Global.GetXElementAttributeStr(xelementList[i], "ArtifactIcon");
			shenQiXMLData.LifeV = Global.GetXElementAttributeInt(xelementList[i], "LifeV");
			shenQiXMLData.AddAttack = Global.GetXElementAttributeInt(xelementList[i], "AddAttack");
			shenQiXMLData.AddDefense = Global.GetXElementAttributeInt(xelementList[i], "AddDefense");
			shenQiXMLData.Toughness = Global.GetXElementAttributeInt(xelementList[i], "Toughness");
			shenQiXMLData.QiangHua = Global.GetXElementAttributeStr(xelementList[i], "QiangHua");
			shenQiXMLData.CostShenLiJingHua = Global.GetXElementAttributeInt(xelementList[i], "CostShenLiJingHua");
			shenQiXMLData.CostGoldCoin = Global.GetXElementAttributeInt(xelementList[i], "CostGoldCoin");
			shenQiXMLData.CostDiamond = Global.GetXElementAttributeInt(xelementList[i], "CostDiamond");
			shenQiXMLData.CostGoldGoods = Global.GetXElementAttributeStr(xelementList[i], "CostGoldGoods");
			if (!ShenQiManager.shenQiDict.ContainsKey(shenQiXMLData.ID))
			{
				ShenQiManager.shenQiDict.Add(shenQiXMLData.ID, shenQiXMLData);
			}
		}
	}

	public static Dictionary<int, ShenQiXMLData> GetShenQiXMLDict()
	{
		if (ShenQiManager.shenQiDict.Count > 0)
		{
			return ShenQiManager.shenQiDict;
		}
		ShenQiManager.LoadXML();
		return ShenQiManager.shenQiDict;
	}

	public static ShenQiXMLData GetShenQiDataByID(int id)
	{
		if (ShenQiManager.shenQiDict.Count <= 0)
		{
			ShenQiManager.LoadXML();
		}
		if (ShenQiManager.shenQiDict.ContainsKey(id))
		{
			return ShenQiManager.shenQiDict[id];
		}
		return default(ShenQiXMLData);
	}

	private static void SetCurrentShenQiDataToDict(int id, ShenQiXMLData data)
	{
		if (ShenQiManager.CurrentShenQiDict.ContainsKey(id))
		{
			ShenQiManager.CurrentShenQiDict[id] = data;
		}
		else
		{
			ShenQiManager.CurrentShenQiDict.Add(id, data);
		}
	}

	public static Dictionary<int, ShenQiXMLData> GetCurrentShenQiDataDict()
	{
		if (ShenQiManager.CurrentShenQiDict.Count > 0)
		{
			return ShenQiManager.CurrentShenQiDict;
		}
		return new Dictionary<int, ShenQiXMLData>();
	}

	public static ShenQiXMLData GetCurrentShenQiDataByID(int id)
	{
		if (ShenQiManager.CurrentShenQiDict.Count > 0)
		{
			return ShenQiManager.CurrentShenQiDict[id];
		}
		return default(ShenQiXMLData);
	}

	public static void RefreshCurrentShenQiDict(ShenQiData shenQiData, bool hasNext = false)
	{
		int shenQiID = shenQiData.ShenQiID;
		if (!hasNext)
		{
			if (ShenQiManager.CurrentShenQiDict.ContainsKey(shenQiID))
			{
				ShenQiXMLData shenQiXMLData = default(ShenQiXMLData);
				shenQiXMLData = ShenQiManager.CurrentShenQiDict[shenQiID];
				shenQiXMLData.LifeV = shenQiData.LifeAdd;
				shenQiXMLData.AddAttack = shenQiData.AttackAdd;
				shenQiXMLData.AddDefense = shenQiData.DefenseAdd;
				shenQiXMLData.Toughness = shenQiData.ToughnessAdd;
				if (ShenQiManager.CurrentShenQiDict.ContainsKey(shenQiID))
				{
					shenQiXMLData.CostGoldGoods = ShenQiManager.CurrentShenQiDict[shenQiID].CostGoldGoods;
				}
				ShenQiManager.CurrentShenQiDict[shenQiID] = shenQiXMLData;
			}
		}
		else
		{
			if (ShenQiManager.CurrentShenQiDict.ContainsKey(shenQiID))
			{
				ShenQiXMLData shenQiXMLData2 = default(ShenQiXMLData);
				shenQiXMLData2 = ShenQiManager.CurrentShenQiDict[shenQiID];
				shenQiXMLData2.LifeV = shenQiData.LifeAdd;
				shenQiXMLData2.AddAttack = shenQiData.AttackAdd;
				shenQiXMLData2.AddDefense = shenQiData.DefenseAdd;
				shenQiXMLData2.Toughness = shenQiData.ToughnessAdd;
				ShenQiManager.CurrentShenQiDict[shenQiID] = shenQiXMLData2;
			}
			int num = shenQiID - 1;
			if (ShenQiManager.CurrentShenQiDict.ContainsKey(num))
			{
				ShenQiXMLData shenQiXMLData3 = default(ShenQiXMLData);
				shenQiXMLData3 = ShenQiManager.CurrentShenQiDict[num];
				shenQiXMLData3.ArtifactIcon = ShenQiManager.GetShenQiDataByID(num).ArtifactIcon;
				shenQiXMLData3.LifeV = ShenQiManager.GetShenQiDataByID(num).LifeV;
				shenQiXMLData3.AddAttack = ShenQiManager.GetShenQiDataByID(num).AddAttack;
				shenQiXMLData3.AddDefense = ShenQiManager.GetShenQiDataByID(num).AddDefense;
				shenQiXMLData3.Toughness = ShenQiManager.GetShenQiDataByID(num).Toughness;
				ShenQiManager.CurrentShenQiDict[num] = shenQiXMLData3;
			}
			if (shenQiID + 1 <= ShenQiManager.GetShenQiXMLDict().Count + 1)
			{
				shenQiData.ShenQiID = shenQiID + 1;
				ShenQiXMLData newSingleFakeShenQiData = ShenQiManager.GetNewSingleFakeShenQiData(shenQiData);
				ShenQiManager.SetCurrentShenQiDataToDict(shenQiData.ShenQiID, newSingleFakeShenQiData);
			}
		}
	}

	public static void InitCurrentShenQiDict(ShenQiData shenQiData, bool isManJi = false)
	{
		int shenQiID = shenQiData.ShenQiID;
		if (shenQiID > 1)
		{
			bool flag = shenQiID == ShenQiManager.GetShenQiXMLDict().Count;
			for (int i = 1; i <= shenQiID; i++)
			{
				ShenQiXMLData data = default(ShenQiXMLData);
				if (flag)
				{
					if (i != shenQiID)
					{
						data = ShenQiManager.GetShenQiDataByID(i);
						ShenQiManager.SetCurrentShenQiDataToDict(i, data);
					}
					else if (i == shenQiID && isManJi)
					{
						data = ShenQiManager.GetShenQiDataByID(i);
						ShenQiManager.SetCurrentShenQiDataToDict(i, data);
					}
					else
					{
						data = ShenQiManager.GetNewSingleShenQiData(shenQiData);
						ShenQiManager.SetCurrentShenQiDataToDict(i, data);
					}
				}
				else if (i != shenQiID)
				{
					data = ShenQiManager.GetShenQiDataByID(i);
					ShenQiManager.SetCurrentShenQiDataToDict(i, data);
				}
				else
				{
					data = ShenQiManager.GetNewSingleShenQiData(shenQiData);
					ShenQiManager.SetCurrentShenQiDataToDict(i, data);
				}
			}
		}
		else
		{
			ShenQiXMLData newSingleShenQiData = ShenQiManager.GetNewSingleShenQiData(shenQiData);
			ShenQiManager.SetCurrentShenQiDataToDict(shenQiID, newSingleShenQiData);
		}
		if (shenQiID + 1 <= ShenQiManager.GetShenQiXMLDict().Count + 1)
		{
			shenQiData.ShenQiID = shenQiID + 1;
			ShenQiXMLData newSingleFakeShenQiData = ShenQiManager.GetNewSingleFakeShenQiData(shenQiData);
			ShenQiManager.SetCurrentShenQiDataToDict(shenQiData.ShenQiID, newSingleFakeShenQiData);
		}
	}

	public static ShenQiXMLData GetNewSingleShenQiData(ShenQiData shenQiData)
	{
		return new ShenQiXMLData
		{
			ID = shenQiData.ShenQiID,
			Name = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).Name,
			ModID = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).ModID,
			ArtifactIcon = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).ArtifactIcon,
			LifeV = shenQiData.LifeAdd,
			AddAttack = shenQiData.AttackAdd,
			AddDefense = shenQiData.DefenseAdd,
			Toughness = shenQiData.ToughnessAdd,
			QiangHua = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).QiangHua,
			CostShenLiJingHua = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostShenLiJingHua,
			CostGoldCoin = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostGoldCoin,
			CostDiamond = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostDiamond,
			CostGoldGoods = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostGoldGoods
		};
	}

	public static ShenQiXMLData GetNewSingleFakeShenQiData(ShenQiData shenQiData)
	{
		return new ShenQiXMLData
		{
			ID = shenQiData.ShenQiID,
			Name = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).Name,
			ModID = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).ModID,
			ArtifactIcon = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).ArtifactIcon,
			LifeV = 0,
			AddAttack = 0,
			AddDefense = 0,
			Toughness = 0,
			QiangHua = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).QiangHua,
			CostShenLiJingHua = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostShenLiJingHua,
			CostGoldCoin = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostGoldCoin,
			CostDiamond = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostDiamond,
			CostGoldGoods = ShenQiManager.GetShenQiDataByID(shenQiData.ShenQiID).CostGoldGoods
		};
	}

	public static void Clear()
	{
		ShenQiManager.shenQiDict.Clear();
		ShenQiManager.CurrentShenQiDict.Clear();
	}

	public static Dictionary<int, ShenQiXMLData> shenQiDict = new Dictionary<int, ShenQiXMLData>();

	private static Dictionary<int, ShenQiXMLData> CurrentShenQiDict = new Dictionary<int, ShenQiXMLData>();
}
