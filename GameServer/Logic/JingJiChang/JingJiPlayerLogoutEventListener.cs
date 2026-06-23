using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	public class JingJiPlayerLogoutEventListener : IEventListener
	{
		private JingJiPlayerLogoutEventListener()
		{
		}

		public static JingJiPlayerLogoutEventListener getInstance()
		{
			return JingJiPlayerLogoutEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
				JingJiChangManager.getInstance().onChallengeEndForPlayerLogout(playerLogoutEventObject.getPlayer());
			}
		}

		private static JingJiPlayerLogoutEventListener instance = new JingJiPlayerLogoutEventListener();
	}
}
