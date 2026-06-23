using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class MapEventMgr
	{
		public void AddGuangMuEvent(int guangMuID, int show)
		{
			MapAIEvent mapAIEvent = new MapAIEvent
			{
				GuangMuID = guangMuID,
				Show = show
			};
			lock (this.EventQueue)
			{
				this.EventQueue.Add(mapAIEvent);
			}
		}

		public void PlayMapEvents(GameClient client)
		{
			lock (this.EventQueue)
			{
				foreach (object obj in this.EventQueue)
				{
					if (obj is MapAIEvent)
					{
						MapAIEvent mapAIEvent = (MapAIEvent)obj;
						int guangMuID = mapAIEvent.GuangMuID;
						int show = mapAIEvent.Show;
						client.sendCmd(667, string.Format("{0}:{1}", guangMuID, show), false);
					}
				}
			}
		}

		public void ClearAllMapEvents()
		{
			lock (this.EventQueue)
			{
				this.EventQueue.Clear();
			}
		}

		private List<object> EventQueue = new List<object>();
	}
}
