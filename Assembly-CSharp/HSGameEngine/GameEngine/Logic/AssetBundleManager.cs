using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class AssetBundleManager
	{
		public static void ClearAssetBundleM()
		{
			foreach (KeyValuePair<string, AssetBundle> keyValuePair in AssetBundleManager.AssetBundleDict)
			{
				keyValuePair.Value.Unload(true);
			}
			AssetBundleManager.AssetBundleDict.Clear();
		}

		public static void AddAssetBundle(string key, AssetBundle assetBundle)
		{
			AssetBundleManager.AssetBundleDict[key] = assetBundle;
		}

		public static AssetBundle GetAssetBundle(string key)
		{
			AssetBundle result = null;
			if (!AssetBundleManager.AssetBundleDict.TryGetValue(key, ref result))
			{
				return null;
			}
			return result;
		}

		public static void RemoveAssetBundle(string key)
		{
			if (AssetBundleManager.AssetBundleDict.ContainsKey(key))
			{
				AssetBundleManager.AssetBundleDict.Remove(key);
			}
		}

		public static Dictionary<string, AssetBundle> AssetBundleDict = new Dictionary<string, AssetBundle>();

		public static AssetBundle CurrentMapLoader = null;

		public static AssetBundle CurrentMapSettingsLoader = null;

		public static AssetBundle CurrentTerrainLoader = null;
	}
}
