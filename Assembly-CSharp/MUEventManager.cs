using System;
using System.Collections.Generic;

public class MUEventManager
{
	public static bool ContainsEvent(string eventType)
	{
		return MUEventManager.m_eventDic.ContainsKey(eventType);
	}

	public static void CleanUp()
	{
		MUEventManager.m_eventDic.Clear();
	}

	private static void CheckAddingNewListener(string eventType, Delegate listener)
	{
		if (!MUEventManager.m_eventDic.ContainsKey(eventType))
		{
			MUEventManager.m_eventDic.Add(eventType, null);
		}
		Delegate @delegate = MUEventManager.m_eventDic[eventType];
		if (@delegate != null && @delegate.GetType() != listener.GetType())
		{
			throw new Exception(string.Format("try to add incorrect eventtype {0}. needed listener type is {1}, given listener type is {2}.", eventType, @delegate.GetType(), listener.GetType()));
		}
	}

	private static bool CheckRemovingListener(string eventType, Delegate listener)
	{
		bool result;
		if (!MUEventManager.m_eventDic.ContainsKey(eventType))
		{
			result = false;
		}
		else
		{
			Delegate @delegate = MUEventManager.m_eventDic[eventType];
			if (@delegate != null && @delegate.GetType() != listener.GetType())
			{
				throw new Exception(string.Format("try to remove incorrect eventtype {0}. needed listener type is {1}, given listener type is {2}.", eventType, @delegate.GetType(), listener.GetType()));
			}
			result = true;
		}
		return result;
	}

	public static void AddEventListener(string eventType, Action listener)
	{
		MUEventManager.CheckAddingNewListener(eventType, listener);
		MUEventManager.m_eventDic[eventType] = (Action)Delegate.Combine((Action)MUEventManager.m_eventDic[eventType], listener);
	}

	public static void AddEventListener<T>(string eventType, Action<T> listener)
	{
		MUEventManager.CheckAddingNewListener(eventType, listener);
		MUEventManager.m_eventDic[eventType] = (Action<T>)Delegate.Combine((Action<T>)MUEventManager.m_eventDic[eventType], listener);
	}

	public static void AddEventListener<T, U>(string eventType, Action<T, U> listener)
	{
		MUEventManager.CheckAddingNewListener(eventType, listener);
		MUEventManager.m_eventDic[eventType] = (Action<T, U>)Delegate.Combine((Action<T, U>)MUEventManager.m_eventDic[eventType], listener);
	}

	public static void RemoveEventListener(string eventType, Action listener)
	{
		if (MUEventManager.CheckRemovingListener(eventType, listener))
		{
			MUEventManager.m_eventDic[eventType] = (Action)Delegate.Remove((Action)MUEventManager.m_eventDic[eventType], listener);
		}
	}

	public static void RemoveEventListener<T>(string eventType, Action<T> listener)
	{
		if (MUEventManager.CheckRemovingListener(eventType, listener))
		{
			Delegate @delegate = (Action<T>)Delegate.Remove((Action<T>)MUEventManager.m_eventDic[eventType], listener);
			MUEventManager.m_eventDic[eventType] = @delegate;
		}
	}

	public static void RemoveEventListener<T, U>(string eventType, Action<T, U> listener)
	{
		if (MUEventManager.CheckRemovingListener(eventType, listener))
		{
			MUEventManager.m_eventDic[eventType] = (Action<T, U>)Delegate.Remove((Action<T, U>)MUEventManager.m_eventDic[eventType], listener);
		}
	}

	public static void SendEvent(string eventType)
	{
		Delegate @delegate = null;
		if (MUEventManager.m_eventDic.TryGetValue(eventType, ref @delegate))
		{
			if (@delegate == null)
			{
				return;
			}
			Delegate[] invocationList = @delegate.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				if (invocationList[i].GetType() != typeof(Action))
				{
					throw new Exception(string.Format("triggerEvnet {0} error : types of parameters are not match.", eventType));
				}
				Action action = (Action)invocationList[i];
				try
				{
					action.Invoke();
				}
				catch (Exception ex)
				{
					string text = ex.ToString();
					MUDebug.LogError<string>(new string[]
					{
						text
					});
				}
			}
		}
	}

	public static void SendEvent<T>(string eventType, T params01)
	{
		Delegate @delegate = null;
		if (MUEventManager.m_eventDic.TryGetValue(eventType, ref @delegate))
		{
			if (@delegate == null)
			{
				return;
			}
			Delegate[] invocationList = @delegate.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				if (invocationList[i].GetType() != typeof(Action<T>))
				{
					throw new Exception(string.Format("triggerEvnet {0} error : types of parameters are not match. Needed type {1}, given p1 type {2}.", eventType, invocationList[i].GetType(), params01.GetType()));
				}
				Action<T> action = (Action<T>)invocationList[i];
				try
				{
					action.Invoke(params01);
				}
				catch (Exception ex)
				{
					string text = ex.ToString();
					MUDebug.LogError<string>(new string[]
					{
						text
					});
				}
			}
		}
	}

	public static void SendEvent<T, U>(string eventType, T params01, U params02)
	{
		Delegate @delegate = null;
		if (MUEventManager.m_eventDic.TryGetValue(eventType, ref @delegate))
		{
			if (@delegate == null)
			{
				return;
			}
			Delegate[] invocationList = @delegate.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				if (invocationList[i].GetType() != typeof(Action<T, U>))
				{
					throw new Exception(string.Format("SendEvent {0} error : types of parameters are not match. Needed type{1}, given p1 p2 type [{2}], [type{3}].", new object[]
					{
						eventType,
						invocationList[i].GetType(),
						params01.GetType(),
						params02.GetType()
					}));
				}
				Action<T, U> action = (Action<T, U>)invocationList[i];
				try
				{
					action.Invoke(params01, params02);
				}
				catch (Exception ex)
				{
					string text = ex.ToString();
					MUDebug.LogError<string>(new string[]
					{
						text
					});
				}
			}
		}
	}

	private static Dictionary<string, Delegate> m_eventDic = new Dictionary<string, Delegate>();
}
