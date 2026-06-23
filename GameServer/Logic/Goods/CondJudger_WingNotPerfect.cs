using System;
using GameServer.Logic.MUWings;

namespace GameServer.Logic.Goods
{
	public class CondJudger_WingNotPerfect : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, 19, true))
			{
				result = false;
			}
			else
			{
				if (!MUWingsManager.IfWingPerfect(client))
				{
					flag = true;
				}
				else if (!LingYuManager.IfLingYuPerfect(client))
				{
					flag = true;
				}
				else if (!ZhuLingZhuHunManager.IfZhuLingPerfect(client))
				{
					flag = true;
				}
				else if (!ZhuLingZhuHunManager.IfZhuHunPerfect(client))
				{
					flag = true;
				}
				if (!flag)
				{
					failedMsg = GLang.GetLang(8018, new object[0]);
				}
				result = flag;
			}
			return result;
		}
	}
}
