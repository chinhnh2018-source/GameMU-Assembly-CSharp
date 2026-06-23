using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class PaoHuanTasksMgr
	{
		public static void SetPaoHuanHistTaskID(int roleID, int taskClass, int taskID)
		{
			lock (PaoHuanTasksMgr.PaoHuanHistDict)
			{
				PaoHuanTaskItem value = new PaoHuanTaskItem
				{
					TaskID = taskID,
					AddDateTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd")
				};
				string key = string.Format("{0}_{1}", roleID, taskClass);
				PaoHuanTasksMgr.PaoHuanHistDict[key] = value;
			}
		}

		public static int FindPaoHuanHistTaskID(int roleID, int taskClass)
		{
			string a = TimeUtil.NowDateTime().ToString("yyyy-MM-dd");
			PaoHuanTaskItem paoHuanTaskItem = null;
			int result;
			lock (PaoHuanTasksMgr.PaoHuanHistDict)
			{
				string key = string.Format("{0}_{1}", roleID, taskClass);
				if (!PaoHuanTasksMgr.PaoHuanHistDict.TryGetValue(key, out paoHuanTaskItem))
				{
					result = -1;
				}
				else if (a != paoHuanTaskItem.AddDateTime)
				{
					result = -1;
				}
				else
				{
					result = paoHuanTaskItem.TaskID;
				}
			}
			return result;
		}

		private static Dictionary<string, PaoHuanTaskItem> PaoHuanHistDict = new Dictionary<string, PaoHuanTaskItem>();
	}
}
