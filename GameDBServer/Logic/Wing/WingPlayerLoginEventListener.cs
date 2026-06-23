using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.Wing
{
	public class WingPlayerLoginEventListener : IEventListener
	{
		private WingPlayerLoginEventListener()
		{
		}

		public static WingPlayerLoginEventListener getInstnace()
		{
			return WingPlayerLoginEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject playerLoginEventObject = (PlayerLoginEventObject)eventObject;
				WingPaiHangManager.getInstance().onPlayerLogin(playerLoginEventObject.RoleInfo.RoleID, playerLoginEventObject.RoleInfo.RoleName);
			}
		}

		private static WingPlayerLoginEventListener instance = new WingPlayerLoginEventListener();
	}
}
