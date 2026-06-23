using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class PathUtils
	{
		public static string GetPersistentPath(string path)
		{
			return Application.persistentDataPath + "/" + path;
		}

		public static string GetWWWPath(string path)
		{
			if (path.StartsWith("http://") || path.StartsWith("ftp://") || path.StartsWith("https://") || path.StartsWith("file://") || path.StartsWith("jar:file://"))
			{
				return path;
			}
			if (Application.platform == 11)
			{
				return path.Insert(0, "file://");
			}
			if (Application.platform != 5 && Application.platform != 3)
			{
				return path.Insert(0, "file:///");
			}
			MUDebug.Log<string>(new string[]
			{
				"越南测试用PathUtils.cs_GetWWWPath_path=" + path
			});
			return path;
		}

		public static string ProjectPath()
		{
			string result;
			if (Application.platform == 5 || Application.platform == 3)
			{
				result = Application.dataPath + "/";
			}
			else
			{
				result = "file:///" + Application.dataPath + "/../";
			}
			return result;
		}

		private static string SteamingAssetsPath(string path = "")
		{
			if (Application.isEditor || Application.platform == 2)
			{
				return "file:///" + Application.dataPath + "/StreamingAssets/" + path;
			}
			if (Application.platform == 11)
			{
				return Application.streamingAssetsPath + "/" + path;
			}
			return "file:///" + Application.streamingAssetsPath + "/" + path;
		}

		public static string SteamingAssetsPath_DontUseThis(string path = "")
		{
			return PathUtils.SteamingAssetsPath(path);
		}

		public static string GameResPath(string path)
		{
			return string.Format("GameRes/{0}", path);
		}

		public static string LoginResPath(string path)
		{
			return string.Format("LoginRes/{0}", path);
		}

		public static string IsolateResPath(string path)
		{
			return string.Format("IsolateRes/{0}", path);
		}

		public static string WebPath(string uri)
		{
			byte b = 0;
			if (PathUtils.PersistentPathDict.TryGetValue(uri, ref b))
			{
				string persistentPath = PathUtils.GetPersistentPath(uri);
				return PathUtils.GetWWWPath(persistentPath);
			}
			string persistentPath2 = PathUtils.GetPersistentPath(uri);
			if (File.Exists(persistentPath2))
			{
				PathUtils.PersistentPathDict[persistentPath2] = 1;
				return PathUtils.GetWWWPath(persistentPath2);
			}
			return PathUtils.SteamingAssetsPath(uri);
		}

		public static string GetAssetName(string resName)
		{
			int num = resName.LastIndexOf(".");
			if (num < 0)
			{
				return resName;
			}
			return resName.Substring(0, num);
		}

		private static Dictionary<string, byte> PersistentPathDict = new Dictionary<string, byte>();
	}
}
