using System;
using Server.Data;

namespace GameServer.Logic.Goods
{
	internal class CondJudger_NoBuff : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = true;
			int bufferID = 0;
			long num = 0L;
			string[] array = arg.Split(new char[]
			{
				'|'
			});
			bool result;
			if (array.Length != 3 || !int.TryParse(array[0], out bufferID) || !long.TryParse(array[1], out num) || string.IsNullOrEmpty(array[2]))
			{
				result = true;
			}
			else
			{
				string text = array[2];
				BufferData bufferDataByID = Global.GetBufferDataByID(client, bufferID);
				if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
				{
					if (bufferDataByID.BufferVal == num)
					{
						flag = false;
					}
				}
				if (!flag && !string.IsNullOrEmpty(text))
				{
					failedMsg = string.Format(text, new object[0]);
				}
				result = flag;
			}
			return result;
		}
	}
}
