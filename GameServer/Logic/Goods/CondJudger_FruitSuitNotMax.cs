using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_FruitSuitNotMax : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless");
			int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless");
			int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless");
			int roleParamsInt32FromDB4 = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless");
			if (roleParamsInt32FromDB < UseFruitVerify.GetFruitAddPropLimit(client, "Strength") || roleParamsInt32FromDB2 < UseFruitVerify.GetFruitAddPropLimit(client, "Intelligence") || roleParamsInt32FromDB3 < UseFruitVerify.GetFruitAddPropLimit(client, "Dexterity") || roleParamsInt32FromDB4 < UseFruitVerify.GetFruitAddPropLimit(client, "Constitution"))
			{
				flag = true;
			}
			if (!flag)
			{
				failedMsg = GLang.GetLang(8015, new object[0]);
			}
			return flag;
		}
	}
}
