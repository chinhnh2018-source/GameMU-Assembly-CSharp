using System;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.Logic
{
	public class JingJiChangPlayerLoginEventListener : IEventListener
	{
		private JingJiChangPlayerLoginEventListener()
		{
		}

		public static JingJiChangPlayerLoginEventListener getInstnace()
		{
			return JingJiChangPlayerLoginEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				PlayerLoginEventObject playerLoginEventObject = (PlayerLoginEventObject)eventObject;
				JingJiChangManager.getInstance().onPlayerLogin(playerLoginEventObject.RoleInfo.RoleID);
			}
		}

		private static JingJiChangPlayerLoginEventListener instance = new JingJiChangPlayerLoginEventListener();
	}
}
