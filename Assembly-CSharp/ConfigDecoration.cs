using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigDecoration
{
	public static DecorationVO GetDecorationVOByCode(int code)
	{
		ConfigDecoration.InitOrCheckDecorationVODict();
		DecorationVO result = null;
		if (ConfigDecoration.DecorationVODict.TryGetValue(code, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"越南产品没有在Decorations中配置改特效，Code 为" + code
		});
		return null;
	}

	public static Dictionary<int, DecorationVO> GetDecorationVODict()
	{
		ConfigDecoration.InitOrCheckDecorationVODict();
		return ConfigDecoration.DecorationVODict;
	}

	private static void InitOrCheckDecorationVODict()
	{
		if (ConfigDecoration.DecorationVODict.Count <= 0)
		{
			ConfigDecoration.PreCacheDecorationXmlNodesOld();
		}
	}

	public static void PreCacheDecorationXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheDecorationXmlNodesOld start..."
		});
		string text = "Config/Decorations.Xml";
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
					ConfigDecoration.ParseXMLToVO(text3, text, text2);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "Deco");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, DecorationVO> decorationVODict = ConfigDecoration.DecorationVODict;
		lock (decorationVODict)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				DecorationVO decorationVO = new DecorationVO();
				decorationVO.CopyFrom(xelementList[i]);
				ConfigDecoration.DecorationVODict[decorationVO.Code] = decorationVO;
			}
		}
	}

	public static void ClearData()
	{
		ConfigDecoration.DecorationVODict.Clear();
	}

	public const string GAME_CONFIG_DECOS_FILE = "Config/Decorations.Xml";

	private static Dictionary<int, DecorationVO> DecorationVODict = new Dictionary<int, DecorationVO>();
}
