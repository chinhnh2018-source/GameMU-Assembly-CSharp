using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.TCP;

namespace GameServer.Logic
{
	public class DelayForceClosingMgr
	{
		public static void AddDelaySocket(TMSKSocket socket)
		{
			long value = TimeUtil.NOW();
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				if (!DelayForceClosingMgr._Socket2UDict.ContainsKey(socket))
				{
					DelayForceClosingMgr._Socket2UDict[socket] = value;
				}
			}
		}

		public static void RemoveDelaySocket(TMSKSocket socket)
		{
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				DelayForceClosingMgr._Socket2UDict.Remove(socket);
			}
		}

		private static List<TMSKSocket> GetDelaySockets()
		{
			List<TMSKSocket> list = new List<TMSKSocket>();
			long num = TimeUtil.NOW();
			lock (DelayForceClosingMgr._Socket2UDict)
			{
				foreach (TMSKSocket tmsksocket in DelayForceClosingMgr._Socket2UDict.Keys)
				{
					long num2 = DelayForceClosingMgr._Socket2UDict[tmsksocket];
					if (num - num2 >= 3000L)
					{
						list.Add(tmsksocket);
					}
				}
			}
			return list;
		}

		public static void ProcessDelaySockets()
		{
			List<TMSKSocket> delaySockets = DelayForceClosingMgr.GetDelaySockets();
			if (delaySockets.Count > 0)
			{
				for (int i = 0; i < delaySockets.Count; i++)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(delaySockets[i]);
					if (null != gameClient)
					{
						Global.ForceCloseClient(gameClient, "", true);
					}
					else
					{
						Global.ForceCloseSocket(delaySockets[i], "", true);
					}
				}
			}
		}

		private static Dictionary<TMSKSocket, long> _Socket2UDict = new Dictionary<TMSKSocket, long>();
	}
}
