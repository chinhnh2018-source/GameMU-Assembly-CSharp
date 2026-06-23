using System;

namespace GameServer.Logic.Goods
{
	internal class CondJudger_NeedOpen : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = true;
			GongNengIDs id = Convert.ToInt32(arg);
			if (!GlobalNew.IsGongNengOpened(client, id, false))
			{
				flag = false;
			}
			if (!flag)
			{
				failedMsg = string.Format("物品对应的功能没有开启", new object[0]);
			}
			return flag;
		}
	}
}
