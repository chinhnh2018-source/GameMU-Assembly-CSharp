using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigGoodsObtain
{
	private static void InitVO()
	{
		if (0 < ConfigGoodsObtain.Dic.Count)
		{
			return;
		}
		string text = "Config/GoodsObtain.xml";
		string text2 = "GameRes";
		try
		{
			text = XmlManager.GetOnlyFileName(text);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(text2);
			if (null == assetBundle)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format(Global.GetLang("GetResVOXml异常, 缓存中没找到 {0}"), text2)
				});
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(text) as TextAsset;
				if (null == textAsset)
				{
					MUDebug.LogError<string>(new string[]
					{
						string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), text2, text)
					});
				}
				else if (!string.IsNullOrEmpty(textAsset.text))
				{
					XElement xelement = XElement.Parse(textAsset.text);
					if (xelement != null)
					{
						List<XElement> xelementList = Global.GetXElementList(xelement, "GoodsObtain");
						for (int i = 0; i < xelementList.Count; i++)
						{
							GoodsObtainVO goodsObtainVO = new GoodsObtainVO();
							goodsObtainVO.GoodsID = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
							goodsObtainVO.BaoDianID = Global.GetXElementAttributeStr(xelementList[i], "BaoDianID");
							goodsObtainVO.CoinID = Global.GetXElementAttributeInt(xelementList[i], "CoinID");
							goodsObtainVO.ObtainSwitch = Global.GetXElementAttributeInt(xelementList[i], "ObtainSwitch");
							goodsObtainVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
							ConfigGoodsObtain.Dic.Add(goodsObtainVO.GoodsID, goodsObtainVO);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static bool GoodsHaveObtain(int GoodsID)
	{
		ConfigGoodsObtain.InitVO();
		return ConfigGoodsObtain.Dic.ContainsKey(GoodsID) && ConfigGoodsObtain.Dic[GoodsID].BaoDianIDArray != null && 0 < ConfigGoodsObtain.Dic[GoodsID].BaoDianIDArray.Length;
	}

	public static int[] GetGoodsObtainIdArray(int GoodsID)
	{
		ConfigGoodsObtain.InitVO();
		if (ConfigGoodsObtain.Dic.ContainsKey(GoodsID))
		{
			return ConfigGoodsObtain.Dic[GoodsID].BaoDianIDArray;
		}
		return null;
	}

	public static GoodsObtainVO GetGoodsObtainCoinId(int CoinID)
	{
		ConfigGoodsObtain.InitVO();
		foreach (KeyValuePair<int, GoodsObtainVO> keyValuePair in ConfigGoodsObtain.Dic)
		{
			if (keyValuePair.Value.CoinID == CoinID)
			{
				Dictionary<int, GoodsObtainVO>.Enumerator enumerator;
				KeyValuePair<int, GoodsObtainVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value;
			}
		}
		return null;
	}

	public static string GetGoodsObtainNameByGoodsID(int GoodsID)
	{
		ConfigGoodsObtain.InitVO();
		if (ConfigGoodsObtain.Dic.ContainsKey(GoodsID))
		{
			return ConfigGoodsObtain.Dic[GoodsID].Name;
		}
		return string.Empty;
	}

	public static bool GetGoodsObtainCanShowOnTipsByGoodsID(int GoodsID)
	{
		ConfigGoodsObtain.InitVO();
		return ConfigGoodsObtain.Dic.ContainsKey(GoodsID) && 1 == ConfigGoodsObtain.Dic[GoodsID].ObtainSwitch;
	}

	public static void ClearData()
	{
		ConfigGoodsObtain.Dic.Clear();
	}

	private static Dictionary<int, GoodsObtainVO> Dic = new Dictionary<int, GoodsObtainVO>();
}
