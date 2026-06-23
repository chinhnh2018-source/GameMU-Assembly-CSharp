using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.Common
{
	public class LocalStorage
	{
		public static void SetString(string key, string strValue)
		{
			try
			{
				PlayerPrefs.SetString(key, strValue);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static string GetString(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		public static bool ExistKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}
	}
}
