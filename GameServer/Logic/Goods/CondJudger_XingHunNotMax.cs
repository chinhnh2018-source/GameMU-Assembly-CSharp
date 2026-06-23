using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_XingHunNotMax : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, 32, true))
			{
				result = false;
			}
			else
			{
				if (!GameManager.StarConstellationMgr.IfStarConstellationPerfect(client))
				{
					flag = true;
				}
				if (!flag)
				{
					failedMsg = GLang.GetLang(8014, new object[0]);
				}
				result = flag;
			}
			return result;
		}
	}
}
