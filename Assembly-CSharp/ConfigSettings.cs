using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigSettings
{
	public static void ClearData()
	{
		ConfigSettings.SettingsMapVODict.Clear();
	}

	public static SettingMapVO GetSettingMapVOByCode(int code)
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		if (code <= 0)
		{
			return null;
		}
		SettingMapVO result = null;
		if (ConfigSettings.SettingsMapVODict.TryGetValue(code, ref result))
		{
			return result;
		}
		MUDebug.LogError<string>(new string[]
		{
			"该地图没有在Setting中配置，ID 为" + code
		});
		return null;
	}

	private static void InitOrCheckSettingsMapVODict()
	{
		if (ConfigSettings.SettingsMapVODict.Count <= 0)
		{
			ConfigSettings.PreCacheSettingsXmlNodesOld();
		}
	}

	public static Dictionary<int, SettingMapVO> GetSettingsMapVODict()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.SettingsMapVODict;
	}

	public static SettingsSpeedConfig GetSettingsSpeedConfig()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingSpeedConfig;
	}

	public static SettingsDistanceConfig GetSettingsDistanceConfig()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingsDistanceConfig;
	}

	public static SettingsSpriteConfig GetSettingsSpriteConfig()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingsSpriteConfig;
	}

	public static SettingsSpriteBrushes GetSettingsSpriteBrushes()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingsSpriteBrushes;
	}

	public static SettingsGoodsPack GetSettingsGoodsPack()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingsGoodsPack;
	}

	public static SettingsAlive GetSettingsAlive()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingsAlive;
	}

	public static SettingsTask GetSettingsTask()
	{
		ConfigSettings.InitOrCheckSettingsMapVODict();
		return ConfigSettings.settingsTask;
	}

	public static bool GetSettingHorseCanUseByMapCode(int MapCode)
	{
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(MapCode);
		return settingMapVOByCode != null && 1 == settingMapVOByCode.Horse;
	}

	public static void PreCacheSettingsXmlNodesOld()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		MUDebug.Log<string>(new string[]
		{
			"PreCacheSettingsXmlNodesOld start..."
		});
		string text = "Config/Settings.Xml";
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
					ConfigSettings.ParseXMLToVO(text3, text, text2);
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
		List<XElement> xelementList = Global.GetXElementList(xelement, "Map");
		if (xelementList == null || xelementList.Count <= 0)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败"), resName, xmlName));
			return;
		}
		Dictionary<int, SettingMapVO> settingsMapVODict = ConfigSettings.SettingsMapVODict;
		lock (settingsMapVODict)
		{
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				SettingMapVO settingMapVO = new SettingMapVO();
				settingMapVO.CopyFrom(xelementList[i]);
				ConfigSettings.SettingsMapVODict[settingMapVO.Code] = settingMapVO;
			}
		}
		ConfigSettings.settingSpeedConfig = new SettingsSpeedConfig();
		XElement xelement2 = Global.GetXElement(xelement, "SpeedConfig");
		ConfigSettings.settingSpeedConfig.CopyFrom(xelement2);
		ConfigSettings.settingsDistanceConfig = new SettingsDistanceConfig();
		XElement xelement3 = Global.GetXElement(xelement, "DistanceConfig");
		ConfigSettings.settingsDistanceConfig.CopyFrom(xelement3);
		ConfigSettings.settingsSpriteConfig = new SettingsSpriteConfig();
		XElement xelement4 = Global.GetXElement(xelement, "SpriteConfig");
		ConfigSettings.settingsSpriteConfig.CopyFrom(xelement4);
		ConfigSettings.settingsSpriteBrushes = new SettingsSpriteBrushes();
		XElement xelement5 = Global.GetXElement(xelement, "SpriteBrushes");
		ConfigSettings.settingsSpriteBrushes.CopyFrom(xelement5);
		ConfigSettings.settingsGoodsPack = new SettingsGoodsPack();
		XElement xelement6 = Global.GetXElement(xelement, "GoodsPack");
		ConfigSettings.settingsGoodsPack.CopyFrom(xelement6);
		ConfigSettings.settingsAlive = new SettingsAlive();
		XElement xelement7 = Global.GetXElement(xelement, "Alive");
		ConfigSettings.settingsAlive.CopyFrom(xelement7);
		ConfigSettings.settingsTask = new SettingsTask();
		XElement xelement8 = Global.GetXElement(xelement, "Task");
		ConfigSettings.settingsTask.CopyFrom(xelement8);
		ConfigSettings.ObstructionShow = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "Obstruction"), "Show");
		ConfigSettings.ObsZIndexShow = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "ObsZIndex"), "Show");
		ConfigSettings.ShowCPUUsageShow = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "ShowCPUUsage"), "Show");
		ConfigSettings.ShowFindWayShow = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "ShowFindWay"), "Show");
		ConfigSettings.PunishShow = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "Punish"), "Show");
		ConfigSettings.KeyPointsShow = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "KeyPoints"), "Show");
	}

	public static string GetMapNameByCode(int mapCode, bool showColor = false)
	{
		if (ConfigSettings.mapNameDict.ContainsKey(mapCode))
		{
			return ConfigSettings.mapNameDict[mapCode];
		}
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return string.Empty;
		}
		string text = settingMapVOByCode.Name;
		if (showColor)
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				text
			});
		}
		ConfigSettings.mapNameDict[mapCode] = text;
		return text;
	}

	public static string GeetMapNameByVo(SettingMapVO mapVo, bool showColor = false)
	{
		if (ConfigSettings.mapNameDict.ContainsKey(mapVo.Code))
		{
			return ConfigSettings.mapNameDict[mapVo.Code];
		}
		if (mapVo == null)
		{
			return string.Empty;
		}
		string text = mapVo.Name;
		if (showColor)
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				text
			});
		}
		ConfigSettings.mapNameDict[mapVo.Code] = text;
		return text;
	}

	public static string GetMapNameByCodeEx(int mapCode, bool showColor = false)
	{
		if (ConfigSettings.mapNameDictEx.ContainsKey(mapCode))
		{
			return ConfigSettings.mapNameDictEx[mapCode];
		}
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return string.Empty;
		}
		string text = settingMapVOByCode.Name;
		if (showColor)
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				text
			});
		}
		ConfigSettings.mapNameDictEx[mapCode] = text;
		return text;
	}

	public static int GetMapPicCodeByCode(int mapCode)
	{
		if (ConfigSettings.mapCodePicCodeDict.ContainsKey(mapCode))
		{
			return ConfigSettings.mapCodePicCodeDict[mapCode];
		}
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return mapCode;
		}
		int picCode = settingMapVOByCode.PicCode;
		ConfigSettings.mapCodePicCodeDict[mapCode] = picCode;
		return picCode;
	}

	public static float GetMapBeiShuByCode(int mapCode)
	{
		if (ConfigSettings.mapCodeBeiShuDict.ContainsKey(mapCode))
		{
			return ConfigSettings.mapCodeBeiShuDict[mapCode];
		}
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return (float)mapCode;
		}
		float beiShu = settingMapVOByCode.BeiShu;
		ConfigSettings.mapCodeBeiShuDict[mapCode] = beiShu;
		return beiShu;
	}

	public static string GetLoadingImage(int mapCode)
	{
		string old = "LoadGame/11.jpg";
		if (Global.IsInZhuTiFuActivity())
		{
			old = Global.GetZhuTiFuNetImg("Loading", old);
		}
		string result;
		if (!ConfigSettings.mapCodeLoadingImageDict.ContainsKey(mapCode))
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
			if (settingMapVOByCode == null)
			{
				return ConfigSettings.GetLoadGamePicName();
			}
			int loadingImage = settingMapVOByCode.loadingImage;
			ConfigSettings.mapCodeLoadingImageDict[mapCode] = loadingImage;
			result = ((loadingImage >= 0) ? string.Format("NetImages/Map/{0}.jpg", loadingImage) : ConfigSettings.GetLoadGamePicName());
		}
		else
		{
			result = ((ConfigSettings.mapCodeLoadingImageDict[mapCode] >= 0) ? string.Format("NetImages/Map/{0}.jpg", ConfigSettings.mapCodeLoadingImageDict[mapCode]) : ConfigSettings.GetLoadGamePicName());
		}
		return result;
	}

	private static string GetLoadGamePicName()
	{
		if (Global.IsTuiGuangFenBao)
		{
			return "LoadGame/Loading_tuiguang2.jpg.qj";
		}
		return "LoadGame/11.jpg";
	}

	public static bool GetLoadingLogoImageVisiable(int mapCode)
	{
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		return settingMapVOByCode != null && settingMapVOByCode.Logo == 1;
	}

	public static string GetMapMusicFileByCode(int mapCode, bool autoStartOnly = true)
	{
		string text = string.Empty;
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return text;
		}
		if (autoStartOnly && settingMapVOByCode.AutoStart != 1)
		{
			return string.Empty;
		}
		if (string.IsNullOrEmpty(settingMapVOByCode.Music))
		{
			return text;
		}
		text = string.Format("Audio/Map/{0}", settingMapVOByCode.Music);
		MUDebug.Log<string>(new string[]
		{
			"mapMusicFile=" + text
		});
		return text;
	}

	public static string GetMap3DResNameByCode(int mapCode)
	{
		string empty = string.Empty;
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return empty;
		}
		if (string.IsNullOrEmpty(settingMapVOByCode.ResName))
		{
			return empty;
		}
		return settingMapVOByCode.ResName;
	}

	public static int GetMapSliceTerrainByCode(int mapCode)
	{
		int result = 0;
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return result;
		}
		return settingMapVOByCode.SliceTerrain;
	}

	public static string GetMap3DResNameByCodeWithoutExt(int mapCode)
	{
		string map3DResNameByCode = ConfigSettings.GetMap3DResNameByCode(mapCode);
		if (string.IsNullOrEmpty(map3DResNameByCode))
		{
			return null;
		}
		int num = map3DResNameByCode.LastIndexOf(".");
		if (num == -1)
		{
			return map3DResNameByCode;
		}
		return map3DResNameByCode.Substring(0, num);
	}

	public static float GetMapFileSizeWithCode(int mapCode)
	{
		int num = 0;
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
		if (settingMapVOByCode == null)
		{
			return (float)num;
		}
		return (float)settingMapVOByCode.FileSize;
	}

	public static float GetMapFileSizeWithCode(SettingMapVO mapVo)
	{
		int num = 0;
		if (mapVo == null)
		{
			return (float)num;
		}
		return (float)mapVo.FileSize;
	}

	public const string GAME_CONFIG_SETTINGS_FILE = "Config/Settings.Xml";

	private static Dictionary<int, SettingMapVO> SettingsMapVODict = new Dictionary<int, SettingMapVO>();

	private static SettingsSpeedConfig settingSpeedConfig = null;

	private static SettingsDistanceConfig settingsDistanceConfig = null;

	private static SettingsSpriteConfig settingsSpriteConfig = null;

	private static SettingsSpriteBrushes settingsSpriteBrushes = null;

	private static SettingsGoodsPack settingsGoodsPack = null;

	private static SettingsAlive settingsAlive = null;

	private static SettingsTask settingsTask = null;

	public static int ObstructionShow;

	public static int ObsZIndexShow;

	public static int ShowCPUUsageShow;

	public static int ShowFindWayShow;

	public static int PunishShow;

	public static int KeyPointsShow;

	private static Dictionary<int, string> mapNameDict = new Dictionary<int, string>();

	private static Dictionary<int, string> mapNameDictEx = new Dictionary<int, string>();

	private static Dictionary<int, int> mapCodePicCodeDict = new Dictionary<int, int>();

	private static Dictionary<int, float> mapCodeBeiShuDict = new Dictionary<int, float>();

	private static Dictionary<int, int> mapCodeLoadingImageDict = new Dictionary<int, int>();
}
