using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic.WanMoTa
{
	public class WanMoTaPlayerLoginEventListener : IEventListener
	{
		private WanMoTaPlayerLoginEventListener()
		{
		}

		public static WanMoTaPlayerLoginEventListener getInstnace()
		{
			return WanMoTaPlayerLoginEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject playerLoginEventObject = (PlayerLoginEventObject)eventObject;
				WanMoTaManager.getInstance().onPlayerLogin(playerLoginEventObject.RoleInfo.RoleID, playerLoginEventObject.RoleInfo.RoleName);
			}
		}

		private static WanMoTaPlayerLoginEventListener instance = new WanMoTaPlayerLoginEventListener();
	}
}
