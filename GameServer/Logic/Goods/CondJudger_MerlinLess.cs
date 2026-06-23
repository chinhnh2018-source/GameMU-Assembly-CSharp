using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	public class CondJudger_MerlinLess : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool result;
			if (!GameManager.MerlinMagicBookMgr.IsOpenMerlin(client))
			{
				result = false;
			}
			else
			{
				int level = client.ClientData.MerlinData._Level;
				int starNum = client.ClientData.MerlinData._StarNum;
				List<int> list = Global.StringToIntList(arg, '|');
				bool flag = level < list[0] || (level == list[0] && starNum < list[1]);
				if (!flag)
				{
					failedMsg = GLang.GetLang(8013, new object[0]);
				}
				result = flag;
			}
			return result;
		}
	}
}
