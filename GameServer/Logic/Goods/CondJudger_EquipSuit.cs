using System;

namespace GameServer.Logic.Goods
{
	internal class CondJudger_EquipSuit : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int maxValue = int.MaxValue;
				if (int.TryParse(arg, out maxValue) && client.ClientData._ReplaceExtArg.CurrEquipSuit >= maxValue)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				failedMsg = string.Format("当前装备的品阶不能低于{0}", arg);
			}
			return flag;
		}
	}
}
