using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameDBServer.Core.GameEvent
{
	public abstract class EventSource
	{
		public void registerListener(int eventType, IEventListener listener)
		{
			lock (this.listeners)
			{
				List<IEventListener> list = null;
				if (!this.listeners.TryGetValue(eventType, out list))
				{
					list = new List<IEventListener>();
					this.listeners.Add(eventType, list);
				}
				list.Add(listener);
			}
		}

		public void removeListener(int eventType, IEventListener listener)
		{
			lock (this.listeners)
			{
				List<IEventListener> list = null;
				if (this.listeners.TryGetValue(eventType, out list))
				{
					list.Remove(listener);
				}
			}
		}

		public void fireEvent(EventObject eventObj)
		{
			if (eventObj != null && eventObj.getEventType() != -1)
			{
				List<IEventListener> listenerList = null;
				if (this.listeners.TryGetValue(eventObj.getEventType(), out listenerList))
				{
					this.dispatchEvent(eventObj, listenerList);
				}
			}
		}

		private void dispatchEvent(EventObject eventObj, List<IEventListener> listenerList)
		{
			foreach (IEventListener eventListener in listenerList)
			{
				try
				{
					eventListener.processEvent(eventObj);
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("事件处理错误: {0},{1}", (EventTypes)eventObj.getEventType(), arg), null, true);
				}
			}
		}

		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();
	}
}
