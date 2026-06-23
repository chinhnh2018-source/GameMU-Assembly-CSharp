using System;
using GameServer.Core.GameEvent;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	public class ZhanMengShiJianEventListener : IEventListener
	{
		private ZhanMengShiJianEventListener()
		{
		}

		public static ZhanMengShiJianEventListener getInstance()
		{
			return ZhanMengShiJianEventListener.instance;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				ZhanMengShijianEvent zhanMengShijianEvent = (ZhanMengShijianEvent)eventObject;
				ZhanMengShiJianManager.getInstance().addZhanMengShiJian(zhanMengShijianEvent.BhId, zhanMengShijianEvent.RoleName, zhanMengShijianEvent.ShijianType, zhanMengShijianEvent.Param1, zhanMengShijianEvent.Param2, zhanMengShijianEvent.Param3, zhanMengShijianEvent.ServerId);
			}
		}

		private static ZhanMengShiJianEventListener instance = new ZhanMengShiJianEventListener();
	}
}
