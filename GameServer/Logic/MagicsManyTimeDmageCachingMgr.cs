using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	public static class MagicsManyTimeDmageCachingMgr
	{
		public static List<ManyTimeDmageItem> GetManyTimeDmageItems(int magicCode)
		{
			List<ManyTimeDmageItem> list = null;
			List<ManyTimeDmageItem> result;
			if (!MagicsManyTimeDmageCachingMgr.ManyTimeDmageCachingDict.TryGetValue(magicCode, out list))
			{
				result = null;
			}
			else
			{
				result = list;
			}
			return result;
		}

		public static void ParseManyTimeDmageItems(SystemXmlItems systemMagicMgr)
		{
			Dictionary<int, List<ManyTimeDmageItem>> dictionary = new Dictionary<int, List<ManyTimeDmageItem>>();
			foreach (int num in systemMagicMgr.SystemXmlItemDict.Keys)
			{
				string stringValue = systemMagicMgr.SystemXmlItemDict[num].GetStringValue("ManyTimeDmage");
				if (null != stringValue)
				{
					MagicsManyTimeDmageCachingMgr.ParseMagicManyTimeDmage(dictionary, num, stringValue);
				}
			}
			MagicsManyTimeDmageCachingMgr.ManyTimeDmageCachingDict = dictionary;
		}

		private static void ParseMagicManyTimeDmage(Dictionary<int, List<ManyTimeDmageItem>> dict, int id, string manyTimeDmage)
		{
			manyTimeDmage = manyTimeDmage.Trim();
			if (!string.IsNullOrEmpty(manyTimeDmage))
			{
				List<ManyTimeDmageItem> value = MagicsManyTimeDmageCachingMgr.ParseItems(id, manyTimeDmage);
				dict[id] = value;
			}
		}

		private static List<ManyTimeDmageItem> ParseItems(int id, string manyTimeDmage)
		{
			List<ManyTimeDmageItem> list = new List<ManyTimeDmageItem>();
			string[] array = manyTimeDmage.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("解析技能项的多段伤害配置时，个数配置错误, ID={0}", id), null, true);
				}
				else
				{
					ManyTimeDmageItem item = new ManyTimeDmageItem
					{
						InjuredSeconds = (long)Global.SafeConvertToInt32(array2[0]),
						InjuredPercent = Global.SafeConvertToDouble(array2[1]),
						manyRangeIndex = i
					};
					list.Add(item);
				}
			}
			return list;
		}

		public static Dictionary<int, List<ManyTimeDmageItem>> ManyTimeDmageCachingDict = new Dictionary<int, List<ManyTimeDmageItem>>();
	}
}
