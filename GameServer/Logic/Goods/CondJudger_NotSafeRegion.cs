using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_NotSafeRegion : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null)
			{
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap) && gameMap != null)
				{
					flag = !gameMap.InSafeRegionList(client.CurrentGrid);
				}
			}
			if (!flag)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(687, new object[0]), ""));
			}
			return flag;
		}
	}
}
