using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class MagicsManyTimeDmageQueue
	{
		public bool AddManyTimeDmageQueueItemEx(int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			Lazy<long> lazy = new Lazy<long>(() => TimeUtil.NOW());
			Lazy<List<ManyTimeDmageItem>> lazy2 = new Lazy<List<ManyTimeDmageItem>>(() => MagicsManyTimeDmageCachingMgr.GetManyTimeDmageItems(magicCode));
			bool result;
			lock (this.mutex)
			{
				ManyTimeDmageMagicItem manyTimeDmageMagicItem;
				if (this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out manyTimeDmageMagicItem))
				{
					if (manyTimeDmageMagicItem.itemList == null)
					{
						result = false;
					}
					else
					{
						if (manyTimeDmageMagicItem.Start(lazy.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
						{
							this.execItemDict.Add(manyTimeDmageMagicItem);
						}
						result = true;
					}
				}
				else
				{
					ManyTimeDmageMagicItem manyTimeDmageMagicItem2;
					if (!MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict.TryGetValue(magicCode, out manyTimeDmageMagicItem2))
					{
						manyTimeDmageMagicItem2 = new ManyTimeDmageMagicItem();
						manyTimeDmageMagicItem2.itemList = lazy2.Value;
						MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict[magicCode] = manyTimeDmageMagicItem2;
					}
					if (!this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out manyTimeDmageMagicItem))
					{
						manyTimeDmageMagicItem = new ManyTimeDmageMagicItem();
						manyTimeDmageMagicItem.itemList = manyTimeDmageMagicItem2.itemList;
						this.manyTimeDmageQueueItemDict[magicCode] = manyTimeDmageMagicItem;
					}
					if (manyTimeDmageMagicItem.itemList == null)
					{
						result = false;
					}
					else
					{
						if (manyTimeDmageMagicItem.Start(lazy.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
						{
							this.execItemDict.Add(manyTimeDmageMagicItem);
						}
						result = true;
					}
				}
			}
			return result;
		}

		public bool AddDelayMagicItemEx(int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			Lazy<long> lazy = new Lazy<long>(() => TimeUtil.NOW());
			bool result;
			lock (this.mutex)
			{
				ManyTimeDmageMagicItem manyTimeDmageMagicItem;
				if (this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out manyTimeDmageMagicItem))
				{
					if (manyTimeDmageMagicItem.Start(lazy.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
					{
						this.execItemDict.Add(manyTimeDmageMagicItem);
					}
					result = true;
				}
				else
				{
					ManyTimeDmageMagicItem manyTimeDmageMagicItem2;
					if (!MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict.TryGetValue(magicCode, out manyTimeDmageMagicItem2))
					{
						manyTimeDmageMagicItem2 = new ManyTimeDmageMagicItem();
						MagicsManyTimeDmageQueue.manyTimeDmageQueueItemStaticDict[magicCode] = manyTimeDmageMagicItem2;
					}
					if (!this.manyTimeDmageQueueItemDict.TryGetValue(magicCode, out manyTimeDmageMagicItem))
					{
						manyTimeDmageMagicItem = new ManyTimeDmageMagicItem();
						manyTimeDmageMagicItem.itemList = manyTimeDmageMagicItem2.itemList;
						this.manyTimeDmageQueueItemDict[magicCode] = manyTimeDmageMagicItem;
					}
					if (manyTimeDmageMagicItem.Start(lazy.Value, magicCode, enemy, enemyX, enemyY, realEnemyX, realEnemyY))
					{
						this.execItemDict.Add(manyTimeDmageMagicItem);
					}
					result = true;
				}
			}
			return result;
		}

		public int GetManyTimeDmageQueueItemNumEx()
		{
			int count;
			lock (this.mutex)
			{
				count = this.execItemDict.Count;
			}
			return count;
		}

		public ManyTimeDmageMagicItem GetCanExecItemsEx(out ManyTimeDmageItem subItem)
		{
			ManyTimeDmageMagicItem manyTimeDmageMagicItem = null;
			subItem = null;
			long num = TimeUtil.NowEx();
			lock (this.mutex)
			{
				List<ManyTimeDmageMagicItem> list = null;
				foreach (ManyTimeDmageMagicItem manyTimeDmageMagicItem2 in this.execItemDict)
				{
					if (num > manyTimeDmageMagicItem2.execTicks)
					{
						manyTimeDmageMagicItem = manyTimeDmageMagicItem2;
						subItem = manyTimeDmageMagicItem.Get();
						if (!manyTimeDmageMagicItem.Next())
						{
							if (null == list)
							{
								list = new List<ManyTimeDmageMagicItem>();
							}
							list.Add(manyTimeDmageMagicItem2);
						}
						break;
					}
				}
				if (null != list)
				{
					foreach (ManyTimeDmageMagicItem manyTimeDmageMagicItem2 in list)
					{
						this.execItemDict.Remove(manyTimeDmageMagicItem2);
					}
				}
			}
			return manyTimeDmageMagicItem;
		}

		public void AddManyTimeDmageQueueItem(ManyTimeDmageQueueItem manyTimeDmageQueueItem)
		{
			lock (this.ManyTimeDmageQueueItemList)
			{
				this.ManyTimeDmageQueueItemList.Add(manyTimeDmageQueueItem);
			}
		}

		public int GetManyTimeDmageQueueItemNum()
		{
			int count;
			lock (this.ManyTimeDmageQueueItemList)
			{
				count = this.ManyTimeDmageQueueItemList.Count;
			}
			return count;
		}

		public List<ManyTimeDmageQueueItem> GetCanExecItems()
		{
			long num = TimeUtil.NOW();
			List<ManyTimeDmageQueueItem> list = new List<ManyTimeDmageQueueItem>();
			lock (this.ManyTimeDmageQueueItemList)
			{
				for (int i = 0; i < this.ManyTimeDmageQueueItemList.Count; i++)
				{
					if (num >= this.ManyTimeDmageQueueItemList[i].ToExecTicks)
					{
						list.Add(this.ManyTimeDmageQueueItemList[i]);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					this.ManyTimeDmageQueueItemList.Remove(list[i]);
				}
			}
			return list;
		}

		public void Clear()
		{
			lock (this.ManyTimeDmageQueueItemList)
			{
				this.ManyTimeDmageQueueItemList.Clear();
			}
			lock (this.mutex)
			{
				this.manyTimeDmageQueueItemDict.Clear();
				this.execItemDict.Clear();
			}
		}

		private object mutex = new object();

		private static Dictionary<int, ManyTimeDmageMagicItem> manyTimeDmageQueueItemStaticDict = new Dictionary<int, ManyTimeDmageMagicItem>();

		private Dictionary<int, ManyTimeDmageMagicItem> manyTimeDmageQueueItemDict = new Dictionary<int, ManyTimeDmageMagicItem>();

		private HashSet<ManyTimeDmageMagicItem> execItemDict = new HashSet<ManyTimeDmageMagicItem>();

		private List<ManyTimeDmageQueueItem> ManyTimeDmageQueueItemList = new List<ManyTimeDmageQueueItem>();
	}
}
