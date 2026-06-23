using System;
using System.Collections.Generic;

public class ConfigManager
{
	public static void LoadConfig()
	{
		if (0 < ConfigManager.mConfigList.Count)
		{
			for (int i = ConfigManager.mConfigList.Count - 1; i >= 0; i--)
			{
				if (ConfigManager.mConfigList[i] != null)
				{
					if (ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnLondConfig || ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnChangeSceneAndOnLondConfig)
					{
						ConfigManager.mConfigList[i].ClearXMLData(0);
						ConfigManager.mConfigList[i].DisposeInstance();
						ConfigManager.mConfigList.RemoveAt(i);
					}
					else if (ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnChangeSceneAndOnLondConfigNoDispose || ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnLondConfigNoDispose)
					{
						ConfigManager.mConfigList[i].ClearXMLData(0);
					}
				}
				else
				{
					ConfigManager.mConfigList.RemoveAt(i);
				}
			}
		}
	}

	public static void ChangeScene()
	{
		if (0 < ConfigManager.mConfigList.Count)
		{
			for (int i = ConfigManager.mConfigList.Count - 1; i >= 0; i--)
			{
				if (ConfigManager.mConfigList[i] != null)
				{
					if (ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnChangeScene || ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnChangeSceneAndOnLondConfig)
					{
						ConfigManager.mConfigList[i].ClearXMLData(1);
						ConfigManager.mConfigList[i].DisposeInstance();
						ConfigManager.mConfigList.RemoveAt(i);
					}
					else if (ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnChangeSceneNoDispose || ConfigManager.mConfigList[i].XmlClearType == ClearType.ClearOnChangeSceneAndOnLondConfigNoDispose)
					{
						ConfigManager.mConfigList[i].ClearXMLData(1);
					}
				}
				else
				{
					ConfigManager.mConfigList.RemoveAt(i);
				}
			}
		}
	}

	public static void AddConfig(ConfigBase item)
	{
		if (item != null)
		{
			ConfigManager.mConfigList.Add(item);
		}
	}

	private static List<ConfigBase> mConfigList = new List<ConfigBase>();
}
