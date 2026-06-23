using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class BaseEventDispatcher
	{
		public bool addEventListener(string id, EventHandler eventHandler)
		{
			return true;
		}

		public bool addEventListener(string id, BaseEventDispatcher.GoodsMovingEventHandler eventHandler)
		{
			return true;
		}

		public bool removeEventListener(string id, EventHandler eventHandler)
		{
			return true;
		}

		public bool removeEventListener(string id, BaseEventDispatcher.GoodsMovingEventHandler eventHandler)
		{
			return true;
		}

		public delegate void GoodsMovingEventHandler(GoodsMovingEvent goodsMovingEvent);
	}
}
