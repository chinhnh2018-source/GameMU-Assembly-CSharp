using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Core.GameEvent
{
	public abstract class EventSource
	{
		public void registerListener(int eventType, IEventListener listener)
		{
			lock (this.Mutex)
			{
				if (eventType >= 66)
				{
					List<IEventListener> list;
					if (!this.listeners.TryGetValue(eventType, out list))
					{
						list = new List<IEventListener>();
						this.listeners.Add(eventType, list);
					}
					list.Add(listener);
				}
				else
				{
					List<IEventListener> list = this.listenerArray[eventType];
					if (null == list)
					{
						list = new List<IEventListener>();
					}
					else
					{
						list = new List<IEventListener>(list);
					}
					this.listenerArray[eventType] = list;
					list.Add(listener);
				}
			}
		}

		public void removeListener(int eventType, IEventListener listener)
		{
			lock (this.Mutex)
			{
				if (eventType >= 66)
				{
					List<IEventListener> list = null;
					if (this.listeners.TryGetValue(eventType, out list))
					{
						list.Remove(listener);
					}
				}
				else
				{
					List<IEventListener> list = this.listenerArray[eventType];
					if (null == list)
					{
						list = new List<IEventListener>();
					}
					else
					{
						list = new List<IEventListener>(list);
					}
					this.listenerArray[eventType] = list;
					list.Remove(listener);
				}
			}
		}

		public void fireEvent(EventObject eventObj)
		{
			if (null != eventObj)
			{
				if (eventObj.AsyncEvent)
				{
					lock (this.AsyncEventQueue)
					{
						this.AsyncEventQueue.Enqueue(eventObj);
						return;
					}
				}
				this.fireEventInternal(eventObj);
			}
		}

		public void fireEventInternal(EventObject eventObj)
		{
			int eventType = eventObj.getEventType();
			if (eventType >= 66)
			{
				List<IEventListener> listenerList = null;
				List<IEventListener> list = null;
				lock (this.Mutex)
				{
					if (!this.listeners.TryGetValue(eventObj.getEventType(), out list))
					{
						return;
					}
					listenerList = list.GetRange(0, list.Count);
				}
				this.dispatchEvent(eventObj, listenerList);
			}
			else
			{
				List<IEventListener> list;
				lock (this.Mutex)
				{
					list = this.listenerArray[eventType];
				}
				if (null != list)
				{
					this.dispatchEvent(eventObj, list);
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
					LogManager.WriteLog(2, string.Format("事件处理错误: {0},{1}", (EventTypes)eventObj.getEventType(), arg), null, true);
				}
			}
		}

		public void DispatchEventAsync()
		{
			for (;;)
			{
				EventObject eventObject = null;
				lock (this.AsyncEventQueue)
				{
					if (this.AsyncEventQueue.Count <= 0)
					{
						break;
					}
					eventObject = this.AsyncEventQueue.Dequeue();
				}
				if (null != eventObject)
				{
					this.fireEventInternal(eventObject);
				}
			}
		}

		protected object Mutex = new object();

		protected List<IEventListener>[] listenerArray = new List<IEventListener>[66];

		protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();

		protected Queue<EventObject> AsyncEventQueue = new Queue<EventObject>();
	}
}
