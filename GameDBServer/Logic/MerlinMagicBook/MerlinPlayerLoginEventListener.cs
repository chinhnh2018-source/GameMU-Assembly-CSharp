using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.MerlinMagicBook
{
	public class MerlinPlayerLoginEventListener : IEventListener
	{
		private MerlinPlayerLoginEventListener()
		{
		}

		public static MerlinPlayerLoginEventListener getInstnace()
		{
			return MerlinPlayerLoginEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject playerLoginEventObject = (PlayerLoginEventObject)eventObject;
				MerlinRankManager.getInstance().onPlayerLogin(playerLoginEventObject.RoleInfo.RoleID, playerLoginEventObject.RoleInfo.RoleName);
			}
		}

		private static MerlinPlayerLoginEventListener instance = new MerlinPlayerLoginEventListener();
	}
}
