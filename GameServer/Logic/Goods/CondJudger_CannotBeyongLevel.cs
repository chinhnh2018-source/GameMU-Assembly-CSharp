using System;

namespace GameServer.Logic.Goods
{
	public class CondJudger_CannotBeyongLevel : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				string[] array = arg.Split(new char[]
				{
					'|'
				});
				if (array.Length == 2)
				{
					int num = -1;
					int num2 = -1;
					if (int.TryParse(array[0], out num) && int.TryParse(array[1], out num2))
					{
						if (client.ClientData.ChangeLifeCount < num || (client.ClientData.ChangeLifeCount == num && client.ClientData.Level <= num2))
						{
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				failedMsg = GLang.GetLang(139, new object[0]);
			}
			return flag;
		}
	}
}
