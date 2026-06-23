using System;
using System.Collections.Generic;
using System.Threading;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigGoods : ConfigBase
{
	public ConfigGoods()
	{
		this.XmlClearType = ClearType.ClearOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
	}

	public void ClearXMLData(byte clearType)
	{
		ConfigGoods.ClearData();
	}

	public static GoodVO GetGoodsXmlNodeByID(int id)
	{
		if (id <= 0)
		{
			return null;
		}
		GoodVO result = null;
		if (ConfigGoods.GoodsXmlNodeDict.TryGetValue(id, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"打死！东南亚产品没有在客户端配置该物品，ID =" + id
		});
		return null;
	}

	public static void PreCacheGoodsXmlNodes()
	{
		if (ConfigGoods.GoodsXmlNodeDict.Count > 0)
		{
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			"PreCacheGoodsXmlNodes start..."
		});
		string text = "Config/GoodsBin.Xml";
		string text2 = "GameRes_VO";
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
					MUDebug.LogError<string>(new string[]
					{
						"bin读取失败，开始尝试从XML读取该文件" + text
					});
					ConfigGoods.PreCacheGoodsXmlNodesOld();
				}
				else
				{
					VOBinOperator.Instance(typeof(ConfigGoods)).SetBuffer(textAsset.bytes);
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					VOBinOperator.Instance(typeof(ConfigGoods)).ParseBinToVOofGoodVO_ByTrdPairs();
					float realtimeSinceStartup2 = Time.realtimeSinceStartup;
					float num = realtimeSinceStartup2 - realtimeSinceStartup;
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

	public static void PreCacheGoodsXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheGoodsXmlNodes start..."
		});
		string xmlName = "Config/Goods.Xml";
		string resName = "GameRes_VO";
		try
		{
			xmlName = XmlManager.GetOnlyFileName(xmlName);
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resName);
			if (null == assetBundle)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 缓存中没找到 {0}"), resName));
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(xmlName) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
				}
				else
				{
					string textContent = textAsset.text;
					if (Global.IsSupportThread())
					{
						Thread thread = new Thread(delegate()
						{
							ConfigGoods.ParseXMLToVO(textContent, xmlName, resName);
						});
						thread.Start();
					}
					else
					{
						ConfigGoods.ParseXMLToVO(textContent, xmlName, resName);
					}
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "Item");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, GoodVO> goodsXmlNodeDict = ConfigGoods.GoodsXmlNodeDict;
		lock (goodsXmlNodeDict)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				GoodVO goodVO = new GoodVO();
				goodVO.CopyFrom(xelementList[i]);
				ConfigGoods.GoodsXmlNodeDict[goodVO.ID] = goodVO;
			}
		}
	}

	public static GoodVO GetGoodsXmlNodeByCatSuitID(int categoriy, int suitID, int sex, int occ)
	{
		if (ConfigGoods.GoodsXmlNodeDict.Count <= 0)
		{
			return null;
		}
		foreach (KeyValuePair<int, GoodVO> keyValuePair in ConfigGoods.GoodsXmlNodeDict)
		{
			if (categoriy == keyValuePair.Value.Categoriy && suitID == keyValuePair.Value.SuitID && sex == keyValuePair.Value.ToSex && occ == keyValuePair.Value.ToOccupation)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	public static GoodVO GetGoodsXmlNodeByQualityUp(int categoriy, int suitID, int qualityID, int occ)
	{
		if (ConfigGoods.GoodsXmlNodeDict.Count <= 0)
		{
			return null;
		}
		foreach (KeyValuePair<int, GoodVO> keyValuePair in ConfigGoods.GoodsXmlNodeDict)
		{
			if (categoriy == keyValuePair.Value.Categoriy && suitID == keyValuePair.Value.ShouShiSuitID && qualityID == keyValuePair.Value.QualityID && occ == keyValuePair.Value.ToOccupation)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	public static string FindGoodsFilterByName(string searchText)
	{
		string text = string.Empty;
		if (ConfigGoods.GoodsXmlNodeDict.Count <= 0)
		{
			return text;
		}
		int num = 0;
		foreach (KeyValuePair<int, GoodVO> keyValuePair in ConfigGoods.GoodsXmlNodeDict)
		{
			string title = keyValuePair.Value.Title;
			if (!string.IsNullOrEmpty(title))
			{
				if (title.IndexOf(searchText) != -1)
				{
					if (text.Length > 0)
					{
						text += ",";
					}
					text += keyValuePair.Value.ID;
					num++;
					if (num >= 5)
					{
						break;
					}
				}
			}
		}
		return text;
	}

	public static void LoadGoodsNamesDict()
	{
		if (ConfigGoods.GoodsNamesDict.Count > 0)
		{
			return;
		}
		foreach (KeyValuePair<int, GoodVO> keyValuePair in ConfigGoods.GoodsXmlNodeDict)
		{
			int id = keyValuePair.Value.ID;
			string title = keyValuePair.Value.Title;
			ConfigGoods.GoodsNamesDict[title] = id;
		}
	}

	public static int FindGoodsIDByName(string title)
	{
		int result = -1;
		if (!ConfigGoods.GoodsNamesDict.TryGetValue(title, ref result))
		{
			return -1;
		}
		return result;
	}

	public static Dictionary<int, double> GetDicEquipPropsByGoodsId(int GoodsId)
	{
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		if (ConfigGoods.GoodsXmlNodeDict.ContainsKey(GoodsId))
		{
			GoodVO goodVO = ConfigGoods.GoodsXmlNodeDict[GoodsId];
			double[] equipProps = goodVO.EquipProps;
			int num = equipProps.Length;
			if (num == 177)
			{
				for (int i = 0; i < num; i++)
				{
					if (0.0 < equipProps[i])
					{
						dictionary.Add(i, equipProps[i]);
					}
				}
			}
		}
		return dictionary;
	}

	public static void ClearData()
	{
		ConfigGoods.GoodsXmlNodeDict.Clear();
		ConfigGoods.GoodsNamesDict.Clear();
		MUDebug.Log<string>(new string[]
		{
			Global.GetLang("道具表清理  ")
		});
	}

	public const string GAME_CONFIG_GOODS_FILE = "Config/Goods.Xml";

	public const string GAME_CONFIG_GOODS_FILE_BIN = "Config/GoodsBin.Xml";

	public static Dictionary<int, GoodVO> GoodsXmlNodeDict = new Dictionary<int, GoodVO>();

	public static Dictionary<string, int> GoodsNamesDict = new Dictionary<string, int>();
}
