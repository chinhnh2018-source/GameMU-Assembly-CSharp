using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_DaTianShiSuit : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				int maxValue = int.MaxValue;
				if (int.TryParse(arg, out maxValue) && client.UsingEquipMgr.GetUsingEquipArchangelWeaponSuit() >= maxValue)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(682, new object[0]), arg));
			}
			return flag;
		}
	}
}
