using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigBaoXiangTips
{
	public static void PreCacheBaoXiangTipsNodes()
	{
		string text = "Config/BaoXiangTips.Xml";
		string text2 = "GameRes";
		try
		{
			text = XmlManager.GetOnlyFileName(text);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text2);
			if (null == assetBundle)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format("GetResVOXml异常, 缓存中没找到 {0}", text2)
				});
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(text) as TextAsset;
				if (null == textAsset)
				{
					MUDebug.LogError<string>(new string[]
					{
						string.Format("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败", text2, text)
					});
				}
				else
				{
					string text3 = textAsset.text;
					ConfigBaoXiangTips.ParseXMLToVO(text3, text, text2);
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static void ParseXMLToVO(string textContent, string xmlName, string resName)
	{
		XElement xelement = XElement.Parse(textContent);
		if (xelement == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败", resName, xmlName)
			});
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "BaoXiangTips");
		if (xelementList == null || xelementList.Count <= 0)
		{
			MUDebug.Log<string>(new string[]
			{
				string.Format("越南产品配置的BaoXiangTips.xml有误，GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败。请检查", resName, xmlName)
			});
			return;
		}
		Dictionary<int, BaoXiangTipsVO> vo_Dic = ConfigBaoXiangTips.VO_Dic;
		lock (vo_Dic)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				BaoXiangTipsVO baoXiangTipsVO = new BaoXiangTipsVO();
				baoXiangTipsVO.CopyFrom(xelementList[i]);
				ConfigBaoXiangTips.VO_Dic[baoXiangTipsVO.GoodsID] = baoXiangTipsVO;
			}
		}
	}

	private static void InitVO()
	{
		if (ConfigBaoXiangTips.VO_Dic == null)
		{
			ConfigBaoXiangTips.VO_Dic = new Dictionary<int, BaoXiangTipsVO>();
		}
		if (ConfigBaoXiangTips.VO_Dic.Count == 0)
		{
			ConfigBaoXiangTips.PreCacheBaoXiangTipsNodes();
		}
	}

	public static BaoXiangTipsVO GetBaoXiangTipsVOByID(int ID)
	{
		ConfigBaoXiangTips.InitVO();
		BaoXiangTipsVO result = null;
		if (ConfigBaoXiangTips.VO_Dic != null)
		{
			ConfigBaoXiangTips.VO_Dic.TryGetValue(ID, ref result);
		}
		return result;
	}

	public static Dictionary<int, BaoXiangTipsVO> GetBaoXiangTipsList()
	{
		ConfigBaoXiangTips.InitVO();
		return ConfigBaoXiangTips.VO_Dic;
	}

	public static bool IsBaoXiangID(int ID)
	{
		ConfigBaoXiangTips.InitVO();
		BaoXiangTipsVO result = null;
		if (ConfigBaoXiangTips.VO_Dic != null)
		{
			ConfigBaoXiangTips.VO_Dic.TryGetValue(ID, ref result);
		}
		return result != null;
	}

	public static void ClearData()
	{
		if (ConfigBaoXiangTips.VO_Dic != null)
		{
			ConfigBaoXiangTips.VO_Dic.Clear();
		}
	}

	public const string GAME_CONFIG_CHENGJIU_FILE = "Config/BaoXiangTips.Xml";

	private static Dictionary<int, BaoXiangTipsVO> VO_Dic = new Dictionary<int, BaoXiangTipsVO>();
}
