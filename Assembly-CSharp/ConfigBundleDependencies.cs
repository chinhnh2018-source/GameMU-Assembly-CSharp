using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigBundleDependencies
{
	public static bool TryGetDependencies(string bundle, out string depd)
	{
		if (ConfigBundleDependencies.DependenciesDict == null)
		{
			ConfigBundleDependencies.LoadConfig();
		}
		return ConfigBundleDependencies.DependenciesDict.TryGetValue(bundle, ref depd);
	}

	private static void LoadConfig()
	{
		ConfigBundleDependencies.DependenciesDict = new Dictionary<string, string>();
		try
		{
			string onlyFileName = XmlManager.GetOnlyFileName("Config/BundleDependencies.Xml");
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle("GameRes");
			if (null == assetBundle)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format("GetResVOXml异常, 缓存中没找到 {0}", "GameRes")
				});
			}
			else
			{
				TextAsset textAsset = assetBundle.LoadAsset(onlyFileName) as TextAsset;
				if (null == textAsset)
				{
					MUDebug.LogError<string>(new string[]
					{
						string.Format("GetResVOXml异常, 从缓存获取 {0}后，解析: {1} 失败", "GameRes", onlyFileName)
					});
				}
				else
				{
					string text = textAsset.text;
					XElement xelement = XElement.Parse(text);
					List<XElement> xelementList = Global.GetXElementList(xelement, "Dependency");
					Dictionary<string, string> dependenciesDict = ConfigBundleDependencies.DependenciesDict;
					lock (dependenciesDict)
					{
						for (int i = 0; i < xelementList.Count; i++)
						{
							ConfigBundleDependencies.DependenciesDict[Global.GetXElementAttributeStr(xelementList[i], "Res")] = Global.GetXElementAttributeStr(xelementList[i], "Depd");
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

	public const string GAME_CONFIG_BUNDLE_FILE = "Config/BundleDependencies.Xml";

	private static Dictionary<string, string> DependenciesDict;
}
