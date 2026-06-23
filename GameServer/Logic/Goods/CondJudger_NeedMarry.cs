using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_NeedMarry : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				if ("1" == arg && client.ClientData.MyMarriageData != null && client.ClientData.MyMarriageData.byMarrytype != -1)
				{
					flag = true;
				}
				else if ("0" == arg && (client.ClientData.MyMarriageData == null || client.ClientData.MyMarriageData.byMarrytype == -1))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				if ("1" == arg)
				{
					failedMsg = GLang.GetLang(683, new object[0]);
				}
				else
				{
					failedMsg = GLang.GetLang(684, new object[0]);
				}
			}
			return flag;
		}
	}
}
