using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.MerlinMagicBook
{
	public class MerlinPlayerLogoutEventListener : IEventListener
	{
		private MerlinPlayerLogoutEventListener()
		{
		}

		public static MerlinPlayerLogoutEventListener getInstnace()
		{
			return MerlinPlayerLogoutEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
				MerlinRankManager.getInstance().onPlayerLogout(playerLogoutEventObject.RoleInfo.RoleID);
			}
		}

		private static MerlinPlayerLogoutEventListener instance = new MerlinPlayerLogoutEventListener();
	}
}
