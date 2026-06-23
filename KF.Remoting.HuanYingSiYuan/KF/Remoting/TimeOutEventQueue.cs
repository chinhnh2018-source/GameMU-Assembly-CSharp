using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	public class TimeOutEventQueue<T>
	{
		public void EnqueueTimeOutEventItem(T item, DateTime endTime)
		{
			lock (this.Mutex)
			{
				TimeOutEventBlock<T> timeOutEventBlock = this.ShengBeiBufferTimeListQueue.First.Value;
				if (endTime.Ticks - timeOutEventBlock.EndTime.Ticks >= 10000000L)
				{
					timeOutEventBlock = new TimeOutEventBlock<T>();
					timeOutEventBlock.EndTime = endTime;
					this.ShengBeiBufferTimeListQueue.AddFirst(timeOutEventBlock);
				}
				timeOutEventBlock.ChildList.Add(item);
			}
		}

		public bool DequeueTimeOutEventItem(List<T> outputList, DateTime now)
		{
			bool flag = false;
			lock (this.Mutex)
			{
				for (LinkedListNode<TimeOutEventBlock<T>> linkedListNode = this.ShengBeiBufferTimeListQueue.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
				{
					if (linkedListNode.Value.EndTime > now)
					{
						break;
					}
					if (!flag)
					{
						flag = true;
					}
					outputList.AddRange(linkedListNode.Value.ChildList);
					this.ShengBeiBufferTimeListQueue.RemoveLast();
				}
			}
			return flag;
		}

		private object Mutex = new object();

		private LinkedList<TimeOutEventBlock<T>> ShengBeiBufferTimeListQueue = new LinkedList<TimeOutEventBlock<T>>();
	}
}
