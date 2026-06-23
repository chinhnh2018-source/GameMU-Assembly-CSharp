using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using XMLCreater;

public class ConfigVersionSystemOpen
{
	public static int GetSystemOpenIDByVersionSystemOpenID(int ID)
	{
		ConfigVersionSystemOpen.InitOrCheckSettingsMapVODict();
		return ID - 100000;
	}

	public static bool VersionSystemOpenHaveTheId(int id, byte CheckID = 0)
	{
		ConfigVersionSystemOpen.InitOrCheckSettingsMapVODict();
		return (CheckID != 1 || 100000 <= id) && null != ConfigVersionSystemOpen.m_allVersionSystemOpen.GetVersionByID(id);
	}

	private static void InitOrCheckSettingsMapVODict()
	{
		if (ConfigVersionSystemOpen.m_allVersionSystemOpen == null && ConfigVersionSystemOpen.m_allVersionSystemOpen == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/VersionSystemOpen.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/VersionSystemOpen.xml 出现错误"
				});
				return;
			}
			ConfigVersionSystemOpen.m_allVersionSystemOpen = new VersionSystemOpenVOAll(gameResXml);
		}
	}

	public static void ClearData()
	{
		ConfigVersionSystemOpen.m_allVersionSystemOpen = null;
	}

	public static VersionSystemOpenVO GetVersionSystemByID(int ID)
	{
		ConfigVersionSystemOpen.InitOrCheckSettingsMapVODict();
		return ConfigVersionSystemOpen.m_allVersionSystemOpen.GetVersionByID(ID);
	}

	public static bool IsVersionSystemOpen(int ID)
	{
		VersionSystemOpenVO versionSystemByID = ConfigVersionSystemOpen.GetVersionSystemByID(ID);
		return versionSystemByID != null && versionSystemByID.IsOpen;
	}

	public static bool IsShenHunHuTiOpen()
	{
		return ConfigVersionSystemOpen.IsVersionSystemOpen(100103);
	}

	public const string GAME_CONFIG_VERSIOMSYSTEMOPEN_FILE = "Config/VersionSystemOpen.Xml";

	public const int VersionSystemOpenAndSystemOpenDValue = 100000;

	private static VersionSystemOpenVOAll m_allVersionSystemOpen;
}
