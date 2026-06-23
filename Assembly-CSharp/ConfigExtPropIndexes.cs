using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigExtPropIndexes
{
	private static void InitVO()
	{
		if (0 >= ConfigExtPropIndexes.Dic_ExtPropIndexesVO.Count)
		{
			ConfigExtPropIndexes.PreCacheExtPropIndexesXmlNodesOld();
		}
	}

	public static int GetExtPropIndexesIDByWord(string word)
	{
		ConfigExtPropIndexes.InitVO();
		ExtPropIndexesVO extPropIndexesVO = null;
		if (ConfigExtPropIndexes.Dic_ExtPropIndexesVO.TryGetValue(word, ref extPropIndexesVO))
		{
			return extPropIndexesVO.ID;
		}
		return -1;
	}

	public static bool GetPercentByWord(string word)
	{
		ConfigExtPropIndexes.InitVO();
		int num = 0;
		ExtPropIndexesVO extPropIndexesVO = null;
		if (ConfigExtPropIndexes.Dic_ExtPropIndexesVO.TryGetValue(word, ref extPropIndexesVO))
		{
			num = extPropIndexesVO.Percent;
		}
		return 1 == num;
	}

	public static bool GetPercentByID(int id)
	{
		ConfigExtPropIndexes.InitVO();
		int num = 0;
		foreach (KeyValuePair<string, ExtPropIndexesVO> keyValuePair in ConfigExtPropIndexes.Dic_ExtPropIndexesVO)
		{
			ExtPropIndexesVO value = keyValuePair.Value;
			if (id == value.ID)
			{
				num = value.Percent;
				break;
			}
		}
		return 1 == num;
	}

	public static string GetExtPropIndexesDescriptionByWord(string word, bool bAddSpace = false)
	{
		ConfigExtPropIndexes.InitVO();
		string text = string.Empty;
		ExtPropIndexesVO extPropIndexesVO = null;
		if (ConfigExtPropIndexes.Dic_ExtPropIndexesVO.TryGetValue(word, ref extPropIndexesVO))
		{
			text = extPropIndexesVO.Description;
		}
		if (bAddSpace && !string.IsNullOrEmpty(text) && text.Length == 2)
		{
			string text2 = text;
			text = string.Empty;
			text = string.Format("{0}        {1}", text2.get_Chars(0), text2.get_Chars(1));
		}
		return text;
	}

	public static string GetExtPropIndexesDescriptionByID(int id, bool bAddSpace = false)
	{
		ConfigExtPropIndexes.InitVO();
		string text = string.Empty;
		foreach (KeyValuePair<string, ExtPropIndexesVO> keyValuePair in ConfigExtPropIndexes.Dic_ExtPropIndexesVO)
		{
			ExtPropIndexesVO value = keyValuePair.Value;
			if (id == value.ID)
			{
				text = value.Description;
				if (!string.IsNullOrEmpty(text) && text.Length == 2)
				{
					string text2 = text;
					text = string.Empty;
					text = string.Format("{0}        {1}", text2.get_Chars(0), text2.get_Chars(1));
				}
				break;
			}
		}
		return text;
	}

	public static ExtPropIndexesVO GetExtPropIndexesVOByWord(string word)
	{
		ConfigExtPropIndexes.InitVO();
		ExtPropIndexesVO result = null;
		ConfigExtPropIndexes.Dic_ExtPropIndexesVO.TryGetValue(word, ref result);
		return result;
	}

	public static ExtPropIndexesVO GetExtPropIndexesVoByID(int id)
	{
		ConfigExtPropIndexes.InitVO();
		foreach (KeyValuePair<string, ExtPropIndexesVO> keyValuePair in ConfigExtPropIndexes.Dic_ExtPropIndexesVO)
		{
			ExtPropIndexesVO value = keyValuePair.Value;
			if (id == value.ID)
			{
				return value;
			}
		}
		return null;
	}

	public static int GetExtPropIndexesShowListByID(int id)
	{
		ConfigExtPropIndexes.InitVO();
		foreach (KeyValuePair<string, ExtPropIndexesVO> keyValuePair in ConfigExtPropIndexes.Dic_ExtPropIndexesVO)
		{
			ExtPropIndexesVO value = keyValuePair.Value;
			if (id == value.ID)
			{
				return value.ShowList;
			}
		}
		return 0;
	}

	public static bool GetExtPropIndexesIsRebirthPropByID(int ID)
	{
		return 0 < ConfigExtPropIndexes.GetExtPropIndexesShowList2ByID(ID);
	}

	public static int GetExtPropIndexesShowList2ByID(int id)
	{
		ConfigExtPropIndexes.InitVO();
		foreach (KeyValuePair<string, ExtPropIndexesVO> keyValuePair in ConfigExtPropIndexes.Dic_ExtPropIndexesVO)
		{
			ExtPropIndexesVO value = keyValuePair.Value;
			if (id == value.ID)
			{
				return value.ShowList2;
			}
		}
		return 0;
	}

	public static int GetExtPropIndexesShowListByWord(string word)
	{
		ConfigExtPropIndexes.InitVO();
		ExtPropIndexesVO extPropIndexesVO = null;
		if (ConfigExtPropIndexes.Dic_ExtPropIndexesVO.TryGetValue(word, ref extPropIndexesVO))
		{
			return extPropIndexesVO.ShowList;
		}
		return 0;
	}

	public static float GetExtPropIndexesPercentByWord(string word)
	{
		ConfigExtPropIndexes.InitVO();
		ExtPropIndexesVO extPropIndexesVO = null;
		if (ConfigExtPropIndexes.Dic_ExtPropIndexesVO.TryGetValue(word, ref extPropIndexesVO))
		{
			return (float)extPropIndexesVO.Percent;
		}
		return 0f;
	}

	public static float GetExtPropIndexesPercentByID(int id)
	{
		ConfigExtPropIndexes.InitVO();
		foreach (KeyValuePair<string, ExtPropIndexesVO> keyValuePair in ConfigExtPropIndexes.Dic_ExtPropIndexesVO)
		{
			ExtPropIndexesVO value = keyValuePair.Value;
			if (id == value.ID)
			{
				return (float)value.Percent;
			}
		}
		return 0f;
	}

	private static void PreCacheExtPropIndexesXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"ExtPropIndexesOld start..."
		});
		string text = "Config/ExtPropIndexes.xml";
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
					ConfigExtPropIndexes.ParseXMLToVO(text3, text, text2);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "ExtPropIndexes");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<string, ExtPropIndexesVO> dic_ExtPropIndexesVO = ConfigExtPropIndexes.Dic_ExtPropIndexesVO;
		lock (dic_ExtPropIndexesVO)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				ExtPropIndexesVO extPropIndexesVO = new ExtPropIndexesVO();
				extPropIndexesVO.CopyFrom(xelementList[i]);
				ConfigExtPropIndexes.Dic_ExtPropIndexesVO[extPropIndexesVO.Word] = extPropIndexesVO;
			}
		}
	}

	public static void ClearData()
	{
		ConfigExtPropIndexes.Dic_ExtPropIndexesVO.Clear();
	}

	public static Dictionary<string, ExtPropIndexesVO> Dic_ExtPropIndexesVO = new Dictionary<string, ExtPropIndexesVO>();
}
