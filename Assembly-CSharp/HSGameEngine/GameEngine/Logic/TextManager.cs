using System;
using HSGameEngine.GameEngine.Common;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class TextManager
	{
		public static string GetResTxt(string resName, string textName)
		{
			AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resName);
			if (null == assetBundle)
			{
				GError.AddErrMsg(string.Format("GetResTxt异常, 没有缓存AssetBundle {0}", resName));
				return null;
			}
			TextAsset textAsset = assetBundle.LoadAsset(textName) as TextAsset;
			if (null == textAsset)
			{
				GError.AddErrMsg(string.Format("GetResTxt异常, 从缓存获取 {0}后，解析: {1} 失败", resName, textName));
				return null;
			}
			return textAsset.text;
		}

		public static string GetGameResTxt(string textName)
		{
			return TextManager.GetResTxt("GameRes", textName);
		}

		public static string GetLoginResTxt(string textName)
		{
			return TextManager.GetResTxt("LoginRes", textName);
		}

		public static string GetIsolateResTxt(string textName)
		{
			return TextManager.GetResTxt("IsolateRes", textName);
		}
	}
}
