using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic
{
	public class JingJiChangPlayerLogoutEventListener : IEventListener
	{
		private JingJiChangPlayerLogoutEventListener()
		{
		}

		public static JingJiChangPlayerLogoutEventListener getInstnace()
		{
			return JingJiChangPlayerLogoutEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 1)
			{
				PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
				JingJiChangManager.getInstance().onPlayerLogin(playerLogoutEventObject.RoleInfo.RoleID);
			}
		}

		private static JingJiChangPlayerLogoutEventListener instance = new JingJiChangPlayerLogoutEventListener();
	}
}
