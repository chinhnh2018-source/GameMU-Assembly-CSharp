using System;

namespace GameServer.Logic.Goods
{
	internal class CondJudger_JunXianLvl : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int num = -1;
				if (int.TryParse(arg, out num) && GameManager.ClientMgr.GetShengWangLevelValue(client) >= num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(677, new object[0]), arg));
			}
			return flag;
		}
	}
}
