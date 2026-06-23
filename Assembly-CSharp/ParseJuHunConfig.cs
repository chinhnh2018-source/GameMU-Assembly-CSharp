using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ParseJuHunConfig
{
	public static List<string> GetJuHunTypes()
	{
		if (ParseJuHunConfig.names.Count > 0)
		{
			return ParseJuHunConfig.names;
		}
		XElement gameResXml = Global.GetGameResXml("Config/JuHunType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JuHunType");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ParseJuHunConfig.names.Add(xelementList[i].GetXElementAttrStr("Name"));
			i++;
		}
		return ParseJuHunConfig.names;
	}

	public static List<JuHunData> GetJuHunDatas()
	{
		if (ParseJuHunConfig.juHunDatas.Count > 0)
		{
			return ParseJuHunConfig.juHunDatas;
		}
		XElement gameResXml = Global.GetGameResXml("Config/JuHun.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "JuHun");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			JuHunData juHunData = new JuHunData();
			juHunData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			juHunData.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			juHunData.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			juHunData.GrowProportion = Global.GetXElementAttributeFloat(xelementList[i], "GrowProportion");
			string[] array = Global.GetXElementAttributeStr(xelementList[i], "CostGoods").Split(new char[]
			{
				'|'
			});
			for (int j = 0; j < array.Length; j++)
			{
				CaiLiaoData caiLiaoData = new CaiLiaoData();
				string[] array2 = array[j].Split(new char[]
				{
					','
				});
				caiLiaoData.ID = int.Parse(array2[0]);
				caiLiaoData.Count = int.Parse(array2[1]);
				juHunData.caiLiaos.Add(caiLiaoData);
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[i], "ProtectGoods");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				juHunData.shenShi = xelementAttributeStr;
			}
			else
			{
				juHunData.shenShi = null;
			}
			juHunData.CostBandJinBi = Global.GetXElementAttributeInt(xelementList[i], "CostBandJinBi");
			juHunData.SuccessProportion = Global.GetXElementAttributeFloat(xelementList[i], "SuccessProportion");
			ParseJuHunConfig.juHunDatas.Add(juHunData);
			i++;
		}
		return ParseJuHunConfig.juHunDatas;
	}

	public static JuHunData GetJuHunDataByTypeAndLevel(int type, int level)
	{
		if (ParseJuHunConfig.juHunDatas.Count <= 0)
		{
			ParseJuHunConfig.GetJuHunDatas();
		}
		JuHunData result = null;
		for (int i = 0; i < ParseJuHunConfig.juHunDatas.Count; i++)
		{
			if (ParseJuHunConfig.juHunDatas[i].Type == type && ParseJuHunConfig.juHunDatas[i].Level == level)
			{
				result = ParseJuHunConfig.juHunDatas[i];
				break;
			}
		}
		return result;
	}

	public static JuHunData GetJuHunDataById(int id)
	{
		if (ParseJuHunConfig.juHunDatas.Count <= 0)
		{
			ParseJuHunConfig.GetJuHunDatas();
		}
		JuHunData result = null;
		for (int i = 0; i < ParseJuHunConfig.juHunDatas.Count; i++)
		{
			if (ParseJuHunConfig.juHunDatas[i].ID == id)
			{
				result = ParseJuHunConfig.juHunDatas[i];
				break;
			}
		}
		return result;
	}

	public static void ClearXMLData()
	{
		if (0 < ParseJuHunConfig.names.Count)
		{
			ParseJuHunConfig.names.Clear();
		}
		if (0 < ParseJuHunConfig.juHunDatas.Count)
		{
			ParseJuHunConfig.juHunDatas.Clear();
		}
	}

	private static List<string> names = new List<string>();

	private static List<JuHunData> juHunDatas = new List<JuHunData>();
}
