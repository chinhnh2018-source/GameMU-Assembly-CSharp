using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigChengJiu
{
	public static ChengJiuVO GetChengJiuVOByChengJiuID(int chengJiuID)
	{
		ConfigChengJiu.InitOrCheckChengJiuVODict();
		return ConfigChengJiu.ChengJiuVOList.Find((ChengJiuVO g) => g.ChengJiuID == chengJiuID);
	}

	public static List<ChengJiuVO> GetChengJiuVOListByID(int ID)
	{
		ConfigChengJiu.InitOrCheckChengJiuVODict();
		return ConfigChengJiu.ChengJiuVOList.FindAll((ChengJiuVO g) => g.ID == ID);
	}

	public static string GetChengJiuNameByChengJiuID(int chengJiuID)
	{
		ChengJiuVO chengJiuVOByChengJiuID = ConfigChengJiu.GetChengJiuVOByChengJiuID(chengJiuID);
		return (chengJiuVOByChengJiuID == null) ? string.Empty : chengJiuVOByChengJiuID.Name;
	}

	public static List<ChengJiuVO> GetChengJiuVOList()
	{
		ConfigChengJiu.InitOrCheckChengJiuVODict();
		return ConfigChengJiu.ChengJiuVOList;
	}

	private static void InitOrCheckChengJiuVODict()
	{
		if (ConfigChengJiu.ChengJiuVOList.Count <= 0)
		{
			ConfigChengJiu.PreCacheChengJiuXmlNodesOld();
		}
	}

	public static void PreCacheChengJiuXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheChengJiuXmlNodesOld start..."
		});
		string text = "Config/ChengJiu.Xml";
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
					ConfigChengJiu.ParseXMLToVO(text3, text, text2);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "Tab");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		List<ChengJiuVO> chengJiuVOList = ConfigChengJiu.ChengJiuVOList;
		lock (chengJiuVOList)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				ChengJiuVO chengJiuVO = new ChengJiuVO();
				chengJiuVO.CopyFrom(xelementList[i]);
				ConfigChengJiu.ChengJiuVOList.Add(chengJiuVO);
			}
		}
	}

	public static void ClearData()
	{
		ConfigChengJiu.ChengJiuVOList.Clear();
	}

	public const string GAME_CONFIG_CHENGJIU_FILE = "Config/ChengJiu.Xml";

	private static List<ChengJiuVO> ChengJiuVOList = new List<ChengJiuVO>();
}
