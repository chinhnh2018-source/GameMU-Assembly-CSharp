using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Core.GameEvent
{
	public class EventSourceEx<T>
	{
		public void registerListener(int eventType, EventSourceEx<T>.HandlerData listener)
		{
			lock (this.Mutex)
			{
				List<EventSourceEx<T>.HandlerData> list;
				if (!this.listeners.TryGetValue(eventType, out list))
				{
					list = new List<EventSourceEx<T>.HandlerData>();
					this.listeners.Add(eventType, list);
				}
				lock (list)
				{
					list.Add(listener);
				}
			}
		}

		public void removeListener(int eventType, EventSourceEx<T>.HandlerData listener)
		{
			lock (this.Mutex)
			{
				List<EventSourceEx<T>.HandlerData> list = null;
				if (this.listeners.TryGetValue(eventType, out list))
				{
					lock (list)
					{
						list.RemoveAll((EventSourceEx<T>.HandlerData x) => x.Handler == listener.Handler);
					}
				}
			}
		}

		public void fireEvent(int eventType, T eventObj)
		{
			if (null != eventObj)
			{
				this.fireEventInternal(eventType, eventObj);
			}
		}

		public void fireEventInternal(int eventType, T eventObj)
		{
			List<EventSourceEx<T>.HandlerData> list = null;
			lock (this.Mutex)
			{
				if (!this.listeners.TryGetValue(eventType, out list))
				{
					return;
				}
			}
			lock (list)
			{
				this.dispatchEvent(eventType, eventObj, list);
			}
		}

		private void dispatchEvent(int eventType, T eventObj, List<EventSourceEx<T>.HandlerData> listenerList)
		{
			foreach (EventSourceEx<T>.HandlerData handlerData in listenerList)
			{
				try
				{
					handlerData.Handler(eventObj);
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(2, string.Format("事件处理错误: type={0},handler={1},ex={2}", eventType, handlerData.Handler.GetType().FullName, arg), null, true);
				}
			}
		}

		protected object Mutex = new object();

		protected Dictionary<int, List<EventSourceEx<T>.HandlerData>> listeners = new Dictionary<int, List<EventSourceEx<T>.HandlerData>>();

		public class HandlerData
		{
			public int ID;

			public int EventType;

			public Func<T, bool> Handler;

			public List<int> BeforeList;

			public List<int> AfterList;
		}
	}
}
