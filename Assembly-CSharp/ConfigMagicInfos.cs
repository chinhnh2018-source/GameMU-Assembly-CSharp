using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigMagicInfos
{
	public static MagicInfoVO GetMaigcInfoVOByCode(int code)
	{
		ConfigMagicInfos.InitOrCheckMaigcInfoVODict();
		if (code <= 0)
		{
			return null;
		}
		MagicInfoVO result = null;
		if (ConfigMagicInfos.MagicInfoVODict.TryGetValue(code, ref result))
		{
			return result;
		}
		return null;
	}

	private static void InitOrCheckMaigcInfoVODict()
	{
		if (ConfigMagicInfos.MagicInfoVODict.Count <= 0)
		{
			ConfigMagicInfos.PreCacheSettingsXmlNodesOld();
		}
	}

	public static Dictionary<int, MagicInfoVO> GetMaigcInfoVODict()
	{
		ConfigMagicInfos.InitOrCheckMaigcInfoVODict();
		return ConfigMagicInfos.MagicInfoVODict;
	}

	public static bool IsValid()
	{
		ConfigMagicInfos.InitOrCheckMaigcInfoVODict();
		return ConfigMagicInfos.MagicInfoVODict.Count > 0;
	}

	public static void PreCacheSettingsXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheSettingsXmlNodesOld start..."
		});
		string text = "Config/Magics.Xml";
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
					ConfigMagicInfos.ParseXMLToVO(text3, text, text2);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "Magic");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, MagicInfoVO> magicInfoVODict = ConfigMagicInfos.MagicInfoVODict;
		lock (magicInfoVODict)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				MagicInfoVO magicInfoVO = new MagicInfoVO();
				magicInfoVO.CopyFrom(xelementList[i]);
				ConfigMagicInfos.MagicInfoVODict[magicInfoVO.ID] = magicInfoVO;
			}
		}
	}

	public static MagicInfoVO GetSkillXmlNode(int id)
	{
		return ConfigMagicInfos.GetMaigcInfoVOByCode(id);
	}

	public static string GetSkillNameByID(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return string.Empty;
		}
		return maigcInfoVOByCode.Name;
	}

	public static int GetSkillIconIDByID(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return -1;
		}
		return maigcInfoVOByCode.MagicIcon;
	}

	public static float GetSkillPubCDByID(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return 0f;
		}
		return (float)maigcInfoVOByCode.PubCDTime;
	}

	public static bool CanSkillByBangDing(int skillID, bool hintUser = true)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return false;
		}
		int bangding = maigcInfoVOByCode.Bangding;
		if (bangding <= 0)
		{
			if (hintUser)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("【{0}】技能不能被绑定"), maigcInfoVOByCode.Name), 0, -1, -1, 0);
			}
			return false;
		}
		return true;
	}

	public static long GetSkillQueueTicks(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return 0L;
		}
		long num = (long)maigcInfoVOByCode.Queue;
		if (num <= 0L)
		{
			return 0L;
		}
		return num * 1000L;
	}

	public static int GetSkillActionType(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return 0;
		}
		int actionType = maigcInfoVOByCode.ActionType;
		if (actionType < 0)
		{
			return -1;
		}
		return actionType;
	}

	public static string GetSkillActionName(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		if (maigcInfoVOByCode == null)
		{
			return string.Empty;
		}
		return maigcInfoVOByCode.SkillAction;
	}

	public static bool IsHorseSkill(int skillID)
	{
		MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(skillID);
		return maigcInfoVOByCode != null && 1 == maigcInfoVOByCode.HorseSkill;
	}

	public static void ClearData()
	{
		ConfigMagicInfos.MagicInfoVODict.Clear();
	}

	public const string GAME_CONFIG_MAGICS_FILE = "Config/Magics.Xml";

	private static Dictionary<int, MagicInfoVO> MagicInfoVODict = new Dictionary<int, MagicInfoVO>();
}
