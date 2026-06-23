using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigArmyGroupLegions
{
	public static void ClearData()
	{
		ConfigArmyGroupLegions.DicArmyGroupLegions.Clear();
	}

	public static string GetZhiWuNameByID(int id)
	{
		ConfigArmyGroupLegions.InitVO();
		if (id == 0)
		{
			id = 4;
		}
		string result = string.Empty;
		if (ConfigArmyGroupLegions.DicArmyGroupLegions.ContainsKey(id))
		{
			result = ConfigArmyGroupLegions.DicArmyGroupLegions[id].Name;
		}
		return result;
	}

	public static ArmyGroupLegionsVO GetRoleArmyGroupLimitsVO(int ZhiWu)
	{
		ConfigArmyGroupLegions.InitVO();
		if (ConfigArmyGroupLegions.DicArmyGroupLegions.ContainsKey(ZhiWu))
		{
			return ConfigArmyGroupLegions.DicArmyGroupLegions[ZhiWu];
		}
		return null;
	}

	private static void InitVO()
	{
		if (0 >= ConfigArmyGroupLegions.DicArmyGroupLegions.Count)
		{
			ConfigArmyGroupLegions.PreCacheArmyGroupLegionsXmlNodesOld();
		}
	}

	private static void PreCacheArmyGroupLegionsXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"ExtPropIndexesOld start..."
		});
		string text = "Config/LegionsManager.xml";
		string text2 = "GameRes";
		try
		{
			text = XmlManager.GetOnlyFileName(text);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text2);
			if (null == assetBundle)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 缓存中没找到 {0}"), text2));
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(text) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text));
				}
				else
				{
					string text3 = textAsset.text;
					ConfigArmyGroupLegions.ParseXMLToVO(text3, text, text2);
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		finally
		{
		}
	}

	private static void ParseXMLToVO(string textContent, string xmlName, string resName)
	{
		XElement xelement = XElement.Parse(textContent);
		if (xelement == null)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "LegionsManager");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, ArmyGroupLegionsVO> dicArmyGroupLegions = ConfigArmyGroupLegions.DicArmyGroupLegions;
		lock (dicArmyGroupLegions)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				ArmyGroupLegionsVO armyGroupLegionsVO = new ArmyGroupLegionsVO();
				armyGroupLegionsVO.CopyFrom(xelementList[i]);
				ConfigArmyGroupLegions.DicArmyGroupLegions[armyGroupLegionsVO.ID] = armyGroupLegionsVO;
			}
		}
	}

	public static Dictionary<int, ArmyGroupLegionsVO> DicArmyGroupLegions = new Dictionary<int, ArmyGroupLegionsVO>();
}
