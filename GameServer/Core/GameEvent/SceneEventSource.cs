using System;
using System.Collections.Generic;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent
{
	public abstract class SceneEventSource : ISceneEventSource
	{
		public void registerListener(int eventType, int sceneType, IEventListenerEx listener)
		{
			lock (this.Event2Scenelisteners)
			{
				Dictionary<int, List<IEventListenerEx>> dictionary;
				if (!this.Event2Scenelisteners.TryGetValue(eventType, out dictionary))
				{
					dictionary = new Dictionary<int, List<IEventListenerEx>>();
					this.Event2Scenelisteners.Add(eventType, dictionary);
				}
				List<IEventListenerEx> list = null;
				if (!dictionary.TryGetValue(sceneType, out list))
				{
					list = new List<IEventListenerEx>();
					dictionary.Add(sceneType, list);
				}
				list.Add(listener);
			}
		}

		public void removeListener(int eventType, int sceneType, IEventListenerEx listener)
		{
			lock (this.Event2Scenelisteners)
			{
				Dictionary<int, List<IEventListenerEx>> dictionary;
				if (!this.Event2Scenelisteners.TryGetValue(eventType, out dictionary))
				{
					dictionary = new Dictionary<int, List<IEventListenerEx>>();
					this.Event2Scenelisteners.Add(eventType, dictionary);
				}
				List<IEventListenerEx> list = null;
				if (dictionary.TryGetValue(sceneType, out list))
				{
					list.Remove(listener);
				}
			}
		}

		public bool fireEvent(EventObjectEx eventObj, int sceneType)
		{
			int eventType;
			bool result;
			if (eventObj == null || (eventType = eventObj.EventType) == -1)
			{
				result = eventObj.Result;
			}
			else
			{
				List<IEventListenerEx> listenerList = null;
				List<IEventListenerEx> list = null;
				lock (this.Event2Scenelisteners)
				{
					Dictionary<int, List<IEventListenerEx>> dictionary;
					if (!this.Event2Scenelisteners.TryGetValue(eventType, out dictionary))
					{
						return eventObj.Result;
					}
					if (!dictionary.TryGetValue(sceneType, out list))
					{
						return eventObj.Result;
					}
					listenerList = list.GetRange(0, list.Count);
				}
				this.dispatchEvent(eventObj, listenerList);
				result = eventObj.Result;
			}
			return result;
		}

		public void dispatchEvent(EventObjectEx eventObj, List<IEventListenerEx> listenerList)
		{
			foreach (IEventListenerEx eventListenerEx in listenerList)
			{
				try
				{
					eventListenerEx.processEvent(eventObj);
					if (eventObj.Handled)
					{
						break;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("事件处理错误: {0},{1}", (EventTypes)eventObj.EventType, ex.ToString()), null, true);
				}
			}
		}

		protected Dictionary<int, Dictionary<int, List<IEventListenerEx>>> Event2Scenelisteners = new Dictionary<int, Dictionary<int, List<IEventListenerEx>>>();
	}
}
