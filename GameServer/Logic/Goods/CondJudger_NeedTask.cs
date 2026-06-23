using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_NeedTask : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			int num = Global.SafeConvertToInt32(arg);
			if (client != null)
			{
				if (client.ClientData.MainTaskID >= num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(685, new object[0]), GlobalNew.GetTaskName(num)));
			}
			return flag;
		}
	}
}
