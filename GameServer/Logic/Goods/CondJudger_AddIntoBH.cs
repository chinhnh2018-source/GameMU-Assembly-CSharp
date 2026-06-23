using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_AddIntoBH : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			int num = Global.SafeConvertToInt32(arg);
			if (client != null)
			{
				if (num > 0 && client.ClientData.Faction > 0)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				failedMsg = GLang.GetLang(2002, new object[0]);
			}
			return flag;
		}
	}
}
