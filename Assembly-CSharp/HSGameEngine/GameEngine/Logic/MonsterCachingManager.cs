using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public static class MonsterCachingManager
	{
		public static void AddCachingItem(int extensionID, GameObject go, bool isCloneRole)
		{
			if (isCloneRole)
			{
				return;
			}
			MonsterCachingItem monsterCachingItem = null;
			if (MonsterCachingManager.MonsterCachingItemDict.TryGetValue(extensionID, ref monsterCachingItem))
			{
				monsterCachingItem.RefCount++;
				return;
			}
			monsterCachingItem = new MonsterCachingItem
			{
				RefCount = 1,
				Monster = go
			};
			MonsterCachingManager.MonsterCachingItemDict[extensionID] = monsterCachingItem;
		}

		public static GameObject GetCachingItem(int extensionID)
		{
			MonsterCachingItem monsterCachingItem = null;
			if (MonsterCachingManager.MonsterCachingItemDict.TryGetValue(extensionID, ref monsterCachingItem))
			{
				return monsterCachingItem.Monster;
			}
			return null;
		}

		public static void ReleaseCachingItem(int extensionID)
		{
			MonsterCachingItem monsterCachingItem = null;
			if (MonsterCachingManager.MonsterCachingItemDict.TryGetValue(extensionID, ref monsterCachingItem))
			{
				monsterCachingItem.RefCount--;
				if (monsterCachingItem.RefCount <= 0)
				{
					MonsterCachingManager.MonsterCachingItemDict.Remove(extensionID);
					Object.Destroy(monsterCachingItem.Monster);
					monsterCachingItem.Monster = null;
					monsterCachingItem = null;
				}
			}
		}

		public static void ClearCachingItems()
		{
			int[] array = new int[MonsterCachingManager.MonsterCachingItemDict.Count];
			MonsterCachingManager.MonsterCachingItemDict.Keys.CopyTo(array, 0);
			foreach (int num in array)
			{
				MonsterCachingItem monsterCachingItem = MonsterCachingManager.MonsterCachingItemDict[num];
				MonsterCachingManager.MonsterCachingItemDict.Remove(num);
				Object.Destroy(monsterCachingItem.Monster);
				monsterCachingItem.Monster = null;
			}
		}

		public static void AddDeadItem(int extensionID, GameObject go)
		{
			go.SetActive(false);
			MonsterCachingItem2 monsterCachingItem = new MonsterCachingItem2
			{
				LastTicks = DateTime.Now.Ticks,
				Monster = go
			};
			Queue<MonsterCachingItem2> queue = null;
			if (!MonsterCachingManager.MonsterCachingItem2Dict.TryGetValue(extensionID, ref queue))
			{
				queue = new Queue<MonsterCachingItem2>();
				MonsterCachingManager.MonsterCachingItem2Dict[extensionID] = queue;
			}
			queue.Enqueue(monsterCachingItem);
		}

		public static GameObject GetDeadItem(int extensionID)
		{
			Queue<MonsterCachingItem2> queue = null;
			if (MonsterCachingManager.MonsterCachingItem2Dict.TryGetValue(extensionID, ref queue) && queue.Count > 0)
			{
				MonsterCachingItem2 monsterCachingItem = queue.Dequeue();
				GameObject monster = monsterCachingItem.Monster;
				monster.SetActive(true);
				return monster;
			}
			return null;
		}

		public static void ClearCachingDeadItems()
		{
			try
			{
				List<Queue<MonsterCachingItem2>> list = Enumerable.ToList<Queue<MonsterCachingItem2>>(MonsterCachingManager.MonsterCachingItem2Dict.Values);
				if (list != null && list.Count > 0)
				{
					long ticks = DateTime.Now.Ticks;
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						Queue<MonsterCachingItem2> queue = list[i];
						while (queue.Count > 0)
						{
							MonsterCachingItem2 monsterCachingItem = Enumerable.First<MonsterCachingItem2>(queue);
							if (ticks - monsterCachingItem.LastTicks < 100000000L)
							{
								break;
							}
							queue.Dequeue();
							Object.Destroy(monsterCachingItem.Monster);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		private static Dictionary<int, MonsterCachingItem> MonsterCachingItemDict = new Dictionary<int, MonsterCachingItem>();

		private static Dictionary<int, Queue<MonsterCachingItem2>> MonsterCachingItem2Dict = new Dictionary<int, Queue<MonsterCachingItem2>>();
	}
}
