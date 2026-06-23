using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigSystemOpen
{
	public static int GetVersionSystemOpenIDBySystemOpenID(int ID)
	{
		return ID + 100000;
	}

	public static SystemOpenVO GetSystemOpenVOByID(int ID)
	{
		ConfigSystemOpen.InitOrCheckSystemOpenVODict();
		SystemOpenVO result = null;
		if (ConfigSystemOpen.SystemOpenVODict.TryGetValue(ID, ref result))
		{
			return result;
		}
		return null;
	}

	public static Dictionary<int, SystemOpenVO> GetSystemOpenVODict()
	{
		ConfigSystemOpen.InitOrCheckSystemOpenVODict();
		return ConfigSystemOpen.SystemOpenVODict;
	}

	public static void ResetSystemOpenVOs()
	{
		if (ConfigSystemOpen.SystemOpenVODict.Count > 0)
		{
			foreach (KeyValuePair<int, SystemOpenVO> keyValuePair in ConfigSystemOpen.SystemOpenVODict)
			{
				keyValuePair.Value.IsOpened = false;
			}
		}
		if (ConfigSystemOpen.SystemOpenVOList.Count > 0)
		{
			for (int i = 0; i < ConfigSystemOpen.SystemOpenVOList.Count; i++)
			{
				ConfigSystemOpen.SystemOpenVOList[i].IsOpened = false;
			}
		}
	}

	public static List<SystemOpenVO> GetSystemOpenVOList()
	{
		ConfigSystemOpen.InitOrCheckSystemOpenVODict();
		return ConfigSystemOpen.SystemOpenVOList;
	}

	private static void InitOrCheckSystemOpenVODict()
	{
		if (ConfigSystemOpen.SystemOpenVODict.Count <= 0)
		{
			ConfigSystemOpen.PreCacheSystemOpenXmlNodesOld();
		}
	}

	public static void PreCacheSystemOpenXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheSystemOpenXmlNodesOld start..."
		});
		string text = "Config/SystemOpen.xml";
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
					ConfigSystemOpen.ParseXMLToVO(text3, text, text2);
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

	public static void ParseXMLToVO(string textContent, string xmlName, string resName)
	{
		XElement xelement = XElement.Parse(textContent);
		if (xelement == null)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "System");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, SystemOpenVO> systemOpenVODict = ConfigSystemOpen.SystemOpenVODict;
		lock (systemOpenVODict)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				SystemOpenVO systemOpenVO = new SystemOpenVO();
				systemOpenVO.CopyFrom(xelementList[i]);
				ConfigSystemOpen.SystemOpenVODict[systemOpenVO.ID] = systemOpenVO;
				ConfigSystemOpen.SystemOpenVOList.Add(systemOpenVO);
			}
			ConfigSystemOpen.SystemOpenVOList.Sort(new SystemOpenVO());
		}
		ConfigSystemOpen.CatulateSystemOpenByRoleData();
	}

	public static void CatulateSystemOpenByRoleData()
	{
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return;
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime serverStartTime = Global.GetServerStartTime();
		DateTime dateTime;
		dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, 1, 1, 1);
		DateTime dateTime2;
		dateTime2..ctor(serverStartTime.Year, serverStartTime.Month, serverStartTime.Day, 1, 1, 1);
		int days = (dateTime - dateTime2).Days;
		int num = (int)Global.Data.roleData.MoneyData[137];
		if (ConfigSystemOpen.SystemOpenVODict.Count > 0)
		{
			foreach (SystemOpenVO systemOpenVO in ConfigSystemOpen.SystemOpenVODict.Values)
			{
				if (systemOpenVO.TriggerCondition == 17)
				{
					systemOpenVO.IsOpened = (days >= systemOpenVO.TimeParameters);
				}
				else if (systemOpenVO.TriggerCondition == 18)
				{
					systemOpenVO.IsOpened = (num >= systemOpenVO.TimeParameters);
				}
			}
		}
		if (ConfigSystemOpen.SystemOpenVOList.Count > 0)
		{
			for (int i = 0; i < ConfigSystemOpen.SystemOpenVOList.Count; i++)
			{
				if (ConfigSystemOpen.SystemOpenVOList[i].TriggerCondition == 17)
				{
					ConfigSystemOpen.SystemOpenVOList[i].IsOpened = (days >= ConfigSystemOpen.SystemOpenVOList[i].TimeParameters);
				}
				else if (ConfigSystemOpen.SystemOpenVOList[i].TriggerCondition == 18)
				{
					ConfigSystemOpen.SystemOpenVOList[i].IsOpened = (num >= ConfigSystemOpen.SystemOpenVOList[i].TimeParameters);
				}
			}
		}
	}

	public static void ClearData()
	{
		ConfigSystemOpen.SystemOpenVOList.Clear();
		ConfigSystemOpen.SystemOpenVODict.Clear();
	}

	public const string GAME_CONFIG_SYSTEMOPEN_FILE = "Config/SystemOpen.xml";

	private static Dictionary<int, SystemOpenVO> SystemOpenVODict = new Dictionary<int, SystemOpenVO>();

	private static List<SystemOpenVO> SystemOpenVOList = new List<SystemOpenVO>();
}
