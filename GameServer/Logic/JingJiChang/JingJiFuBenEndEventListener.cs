using System;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.JingJiChang
{
	public class JingJiFuBenEndEventListener : IEventListener
	{
		private JingJiFuBenEndEventListener()
		{
		}

		public static JingJiFuBenEndEventListener getInstance()
		{
			return JingJiFuBenEndEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = (PlayerDeadEventObject)eventObject;
				JingJiChangManager.getInstance().onChallengeEndForPlayerDead(playerDeadEventObject.getPlayer(), playerDeadEventObject.getAttacker());
			}
			if (eventObject.getEventType() == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = (MonsterDeadEventObject)eventObject;
				JingJiChangManager.getInstance().onChallengeEndForMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
		}

		private static JingJiFuBenEndEventListener instance = new JingJiFuBenEndEventListener();
	}
}
