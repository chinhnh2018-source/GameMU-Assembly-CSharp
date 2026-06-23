using System;
using System.Collections.Generic;

namespace GameServer.Logic.Goods
{
	internal class CondJudger_WingSuitLess : ICondJudger
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
				if (client != null && client.ClientData.MyWingData != null && !string.IsNullOrEmpty(arg))
				{
					List<int> list = Global.StringToIntList(arg, '|');
					if (list.Count == 2)
					{
						if (client.ClientData.MyWingData.WingID < list[0] || (client.ClientData.MyWingData.WingID == list[0] && client.ClientData.MyWingData.ForgeLevel < list[1]))
						{
							flag = true;
						}
					}
					else if (list.Count == 1)
					{
						if (client.ClientData.MyWingData.WingID < list[0])
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					failedMsg = GLang.GetLang(8019, new object[0]);
				}
				result = flag;
			}
			return result;
		}
	}
}
