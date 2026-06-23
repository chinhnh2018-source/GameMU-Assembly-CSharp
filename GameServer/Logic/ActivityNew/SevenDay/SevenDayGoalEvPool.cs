using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	public static class SevenDayGoalEvPool
	{
		public static SevenDayGoalEventObject Alloc(GameClient client, ESevenDayGoalFuncType funcType)
		{
			SevenDayGoalEventObject sevenDayGoalEventObject = null;
			lock (SevenDayGoalEvPool.Mutex)
			{
				if (SevenDayGoalEvPool.freeEvList.Count > 0)
				{
					sevenDayGoalEventObject = SevenDayGoalEvPool.freeEvList.Dequeue();
				}
			}
			if (sevenDayGoalEventObject == null)
			{
				sevenDayGoalEventObject = new SevenDayGoalEventObject();
			}
			sevenDayGoalEventObject.Reset();
			sevenDayGoalEventObject.Client = client;
			sevenDayGoalEventObject.FuncType = funcType;
			return sevenDayGoalEventObject;
		}

		public static void Free(SevenDayGoalEventObject evObj)
		{
			if (evObj != null)
			{
				evObj.Reset();
				lock (SevenDayGoalEvPool.Mutex)
				{
					SevenDayGoalEvPool.freeEvList.Enqueue(evObj);
				}
			}
		}

		private static object Mutex = new object();

		private static Queue<SevenDayGoalEventObject> freeEvList = new Queue<SevenDayGoalEventObject>();
	}
}
