using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	internal class RunNPCScripts
	{
		public static int ProcessNPCScript(GameClient client, int scriptID, int npcID)
		{
			int num = 0;
			int result;
			if (Global.FilterNPCScriptByID(client, scriptID, out num))
			{
				result = num;
			}
			else
			{
				List<MagicActionItem> list = null;
				if (!GameManager.SystemMagicActionMgr.NPCScriptActionsDict.TryGetValue(scriptID, out list) || null == list)
				{
					result = -3;
				}
				else if (list.Count <= 0)
				{
					result = -1;
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						MagicAction.ProcessAction(client, client, list[i].MagicActionID, list[i].MagicActionParams, -1, -1, 0, 1, -1, npcID, 0, -1, 0, false, false, 1.0, 1, 0.0);
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
