using System;
using System.Collections.Generic;
using System.Threading;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigNPCs
{
	public static void PreCacheNPCVOs()
	{
		if ((Global.IsUsingXMLVO() || Context.IsAPPVerify) && Context.IsHaiwai)
		{
			MUDebug.Log<string>(new string[]
			{
				"IsUsingXMLVO"
			});
			ConfigNPCs.PreCacheNPCVOsOld();
			return;
		}
		if (ConfigNPCs.NPCVODict.Count > 0)
		{
			return;
		}
		string text = "Config/npcsBin.Xml";
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
					ConfigNPCs.PreCacheNPCVOsOld();
				}
				else
				{
					VOBinOperator.Instance(typeof(ConfigNPCs)).SetBuffer(textAsset.bytes);
					VOBinOperator.Instance(typeof(ConfigNPCs)).ParseBinToVOofNPCInfoVO_ByTrdPairs();
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

	public static void PreCacheNPCVOsOld()
	{
		if (ConfigNPCs.NPCVODict.Count > 0)
		{
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			"PreCacheNPCVOs start..."
		});
		string xmlName = "Config/npcs.Xml";
		string resName = "GameRes_VO";
		if (Context.IsAPPVerify && Context.IsHaiwai)
		{
			xmlName = "Config/npcsIosVerify.Xml";
		}
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
							ConfigNPCs.ParseXMLToVO(textContent, xmlName, resName);
						});
						thread.Start();
					}
					else
					{
						ConfigNPCs.ParseXMLToVO(textContent, xmlName, resName);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "NPC");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, NPCInfoVO> npcvodict = ConfigNPCs.NPCVODict;
		lock (npcvodict)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				NPCInfoVO npcinfoVO = new NPCInfoVO();
				npcinfoVO.CopyFrom(xelementList[i]);
				ConfigNPCs.NPCVODict[npcinfoVO.ID] = npcinfoVO;
			}
		}
	}

	public static void LoadNPCNamesDict()
	{
		if (ConfigNPCs.NPCsNamesDict.Count > 0)
		{
			return;
		}
		foreach (KeyValuePair<int, NPCInfoVO> keyValuePair in ConfigNPCs.NPCVODict)
		{
			ConfigNPCs.NPCsNamesDict[keyValuePair.Value.SName] = keyValuePair.Key;
		}
	}

	public static int FindNPCIDByName(string sname)
	{
		int result = -1;
		if (!ConfigNPCs.NPCsNamesDict.TryGetValue(sname, ref result))
		{
			return -1;
		}
		return result;
	}

	public static string GetNPCNameByID(int npcID)
	{
		NPCInfoVO npcinfoVO;
		if (ConfigNPCs.NPCVODict.TryGetValue(npcID, ref npcinfoVO))
		{
			return npcinfoVO.SName;
		}
		return string.Empty;
	}

	public static NPCInfoVO GetNPCVOByID(int npcID)
	{
		NPCInfoVO result = null;
		if (ConfigNPCs.NPCVODict.TryGetValue(npcID, ref result))
		{
			return result;
		}
		return null;
	}

	public static int GetNPCPicCodeByID(int npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID == null)
		{
			return -1;
		}
		return npcvobyID.PicCode;
	}

	public static string GetNPC3DResNameByID(int npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID == null)
		{
			return string.Empty;
		}
		return npcvobyID.ResName;
	}

	public static int GetNPCDirectionByID(int npcID)
	{
		return 4;
	}

	public static double GetNPC3DResScaleByID(int npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID == null)
		{
			return 1.0;
		}
		float num = npcvobyID.Scale;
		if (num < 0f)
		{
			num = 1f;
		}
		return (double)num;
	}

	public static string GetNPCSoundByID(int npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID == null)
		{
			return string.Empty;
		}
		return npcvobyID.PlaySound;
	}

	public static float GetNPCSoundIntervalByID(int npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID == null)
		{
			return 0f;
		}
		return npcvobyID.Interval;
	}

	public static string GetNPCTalkSoundByID(int npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID == null)
		{
			return string.Empty;
		}
		return npcvobyID.TakeSound;
	}

	public static void ClearData()
	{
		ConfigNPCs.NPCVODict.Clear();
		ConfigNPCs.NPCsNamesDict.Clear();
	}

	public const string GAME_CONFIG_NPCS_FILE = "Config/npcs.Xml";

	public const string GAME_CONFIG_NPCS_FILE_BIN = "Config/npcsBin.Xml";

	public const string GAME_CONFIG_NPCS_NAME = "Confignpcs";

	public const string GAME_CONFIG_NPCS_FILE_IOSVERIFY = "Config/npcsIosVerify.Xml";

	public static Dictionary<int, NPCInfoVO> NPCVODict = new Dictionary<int, NPCInfoVO>();

	public static Dictionary<string, int> NPCsNamesDict = new Dictionary<string, int>();
}
