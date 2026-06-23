using System;
using System.Collections.Generic;
using System.Threading;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using Tmsk.Xml;
using UnityEngine;

public class ConfigMonsters
{
	public static MonsterVO GetMonsterXmlNodeByID(int monstreID)
	{
		MonsterVO result = null;
		if (ConfigMonsters.MonsterXmlNode.TryGetValue(monstreID, ref result))
		{
			return result;
		}
		return null;
	}

	public static void PreCacheMonsterXmlNodes()
	{
		if (ConfigMonsters.MonsterXmlNode.Count > 0)
		{
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			"PreCacheMonsterXmlNodes start..."
		});
		string text = "Config/MonstersBin.Xml";
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
					ConfigMonsters.PreCacheMonsterXmlNodesOld();
				}
				else
				{
					VOBinOperator.Instance(typeof(ConfigMonsters)).SetBuffer(textAsset.bytes);
					VOBinOperator.Instance(typeof(ConfigMonsters)).ParseBinToVOofMonsterVO_ByTrdPairs();
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

	public static void PreCacheMonsterXmlNodesOld()
	{
		if (ConfigMonsters.MonsterXmlNode.Count > 0)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheMonsterXmlNodes start..."
		});
		string xmlName = "Config/Monsters.Xml";
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
							ConfigMonsters.ParseXMLToVO(textContent, xmlName, resName);
						});
						thread.Start();
					}
					else
					{
						ConfigMonsters.ParseXMLToVO(textContent, xmlName, resName);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "Monster");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, MonsterVO> monsterXmlNode = ConfigMonsters.MonsterXmlNode;
		lock (monsterXmlNode)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				MonsterVO monsterVO = new MonsterVO();
				monsterVO.CopyFrom(xelementList[i]);
				ConfigMonsters.MonsterXmlNode[monsterVO.ID] = monsterVO;
			}
		}
	}

	public static int GetMonsterMapCodeByID(int monsterID)
	{
		int result = -1;
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
		if (monsterXmlNodeByID != null)
		{
			result = monsterXmlNodeByID.MapCode;
		}
		return result;
	}

	public static int GetMonsterPicCodeByID(int monsterID)
	{
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
		if (monsterXmlNodeByID == null)
		{
			return -1;
		}
		return monsterXmlNodeByID.PicCode;
	}

	public static string GetMonster3DResNameByID(int monsterID)
	{
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
		if (monsterXmlNodeByID == null)
		{
			return string.Empty;
		}
		return monsterXmlNodeByID.ResName;
	}

	public static double GetMonster3DResScaleByID(int monsterID)
	{
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
		if (monsterXmlNodeByID == null)
		{
			return 1.0;
		}
		return (double)monsterXmlNodeByID.Scale;
	}

	public static void AddMonsterPlaySound(int monsterID, GSprite gSprite)
	{
		if (ConfigMonsters.MonsterSoundDic.ContainsKey(monsterID))
		{
			Dictionary<int, int> monsterSoundDic;
			Dictionary<int, int> dictionary = monsterSoundDic = ConfigMonsters.MonsterSoundDic;
			int num = monsterSoundDic[monsterID];
			dictionary[monsterID] = num + 1;
		}
		else
		{
			ConfigMonsters.MonsterSoundDic.Add(monsterID, 1);
			gSprite.RandomPlaySpriteSound(true);
		}
	}

	public static void RemoveMonsterPlaySound(int monsterID)
	{
		int num = 0;
		if (ConfigMonsters.MonsterSoundDic.TryGetValue(monsterID, ref num))
		{
			if (num <= 1)
			{
				ConfigMonsters.MonsterSoundDic.Remove(monsterID);
			}
			else
			{
				Dictionary<int, int> monsterSoundDic;
				Dictionary<int, int> dictionary = monsterSoundDic = ConfigMonsters.MonsterSoundDic;
				int num2 = monsterSoundDic[monsterID];
				dictionary[monsterID] = num2 - 1;
			}
		}
	}

	public static void LoadMonsterNamesDict()
	{
		if (ConfigMonsters.MonsterNamesDict.Count > 0)
		{
			return;
		}
		foreach (KeyValuePair<int, MonsterVO> keyValuePair in ConfigMonsters.MonsterXmlNode)
		{
			ConfigMonsters.MonsterNamesDict[keyValuePair.Value.SName] = keyValuePair.Key;
		}
	}

	public static int FindMonsterIDByName(string sname)
	{
		int result = -1;
		if (!ConfigMonsters.MonsterNamesDict.TryGetValue(sname, ref result))
		{
			return -1;
		}
		return result;
	}

	public static void ClearData()
	{
		ConfigMonsters.MonsterXmlNode.Clear();
		ConfigMonsters.MonsterNamesDict.Clear();
		ConfigMonsters.MonsterSoundDic.Clear();
	}

	public const string GAME_CONFIG_MONSTERS_FILE = "Config/Monsters.Xml";

	public const string GAME_CONFIG_MONSTERS_FILE_BIN = "Config/MonstersBin.Xml";

	public const string GAME_CONFIG_MONSTERS_NAME = "ConfigMonsters";

	public static Dictionary<int, MonsterVO> MonsterXmlNode = new Dictionary<int, MonsterVO>();

	private static Dictionary<int, int> MonsterSoundDic = new Dictionary<int, int>();

	public static Dictionary<string, int> MonsterNamesDict = new Dictionary<string, int>();
}
