using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Server.TCP
{
	public class MemoryManager
	{
		public void AddBatchBlock(int blockNum, int blockSize)
		{
			this.AddBatchBlock2(blockNum, blockSize);
		}

		public void AddBatchBlock2(int blockNum, int blockSize)
		{
			blockNum /= 8;
			lock (this.MemoryDict)
			{
				if (this.MemoryDict.ContainsKey(blockSize))
				{
					this.MemoryDict.Remove(blockSize);
				}
				MemoryStackArray memoryStackArray = new MemoryStackArray();
				for (int i = 0; i < 16; i++)
				{
					Stack<MemoryBlock> stack = new Stack<MemoryBlock>();
					for (int j = 0; j < blockNum; j++)
					{
						MemoryBlock memoryBlock = new MemoryBlock(blockSize, true);
						stack.Push(memoryBlock);
						this.BlockDict[memoryBlock] = 1;
					}
					memoryStackArray.StackList[i] = stack;
				}
				this.MemoryDict.Add(blockSize, memoryStackArray);
			}
			lock (this.BlockSizeList)
			{
				this.BlockSizeList.Add(blockSize);
				this.BlockSizeList.Sort();
			}
		}

		public void Push(MemoryBlock item)
		{
			if (null == item)
			{
				throw new ArgumentNullException("添加到MemoryManager 的item不能是空(null)");
			}
			MemoryStackArray memoryStackArray;
			if (!item.isManaged)
			{
				Interlocked.Add(ref MemoryManager.TotalNewAllocMemorySize, (long)(-(long)item.BlockSize));
			}
			else if (this.MemoryDict.TryGetValue(item.BlockSize, out memoryStackArray))
			{
				int num = Interlocked.Increment(ref memoryStackArray.PushIndex) & 15;
				Stack<MemoryBlock> stack = memoryStackArray.StackList[num];
				lock (stack)
				{
					byte b = 0;
					this.BlockDict.TryGetValue(item, out b);
					if (b <= 0)
					{
						stack.Push(item);
						this.BlockDict[item] = 1;
					}
				}
			}
		}

		public MemoryBlock Pop(int needSize)
		{
			MemoryStackArray memoryStackArray;
			if (this.MemoryDict.TryGetValue(this.GetIndex(needSize), out memoryStackArray))
			{
				int num = Interlocked.Increment(ref memoryStackArray.PopIndex) & 15;
				Stack<MemoryBlock> stack = memoryStackArray.StackList[num];
				lock (stack)
				{
					if (stack.Count > 0)
					{
						MemoryBlock memoryBlock = stack.Pop();
						this.BlockDict[memoryBlock] = 0;
						return memoryBlock;
					}
				}
			}
			Interlocked.Add(ref MemoryManager.TotalNewAllocMemorySize, (long)needSize);
			return new MemoryBlock(needSize, false);
		}

		private int GetIndex(int needSize)
		{
			int result = -1;
			foreach (int num in this.BlockSizeList)
			{
				if (needSize <= num)
				{
					result = num;
					break;
				}
			}
			return result;
		}

		public string GetCacheInfoStr()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			foreach (KeyValuePair<int, MemoryStackArray> keyValuePair in this.MemoryDict)
			{
				int key = keyValuePair.Key;
				List<int> list = null;
				foreach (Stack<MemoryBlock> stack in keyValuePair.Value.StackList)
				{
					lock (stack)
					{
						if (dictionary.TryGetValue(key, out list))
						{
							list.Add(stack.Count);
						}
						else
						{
							dictionary[key] = new List<int>
							{
								stack.Count
							};
						}
					}
				}
			}
			foreach (KeyValuePair<int, List<int>> keyValuePair2 in dictionary)
			{
				int totalCount = 0;
				string countListStr = "";
				keyValuePair2.Value.ForEach(delegate(int x)
				{
					totalCount += x;
					countListStr = x.ToString();
				});
				stringBuilder.AppendFormat(string.Format("大小 {0} bytes 缓存中数量 {1} [{2}]\r\n", keyValuePair2.Key, totalCount, countListStr), new object[0]);
			}
			stringBuilder.AppendFormat("非缓存分配，正在使用的内存: {0}", MemoryManager.GetNewAllocMemorySize());
			return stringBuilder.ToString();
		}

		public string GetUsedMemoryAllocStackTrace()
		{
			StringBuilder stringBuilder = new StringBuilder();
			lock (this.MemoryBlockStackTraceDict)
			{
				foreach (KeyValuePair<MemoryBlock, StackTrace> keyValuePair in this.MemoryBlockStackTraceDict)
				{
					if (keyValuePair.Value != null)
					{
						stringBuilder.AppendFormat("BlockSize:{0},StackTrace:{1}\r\n", keyValuePair.Key.BlockSize, keyValuePair.Value.ToString());
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static long GetNewAllocMemorySize()
		{
			long result = 0L;
			lock (MemoryManager.MemoryLock)
			{
				result = MemoryManager.TotalNewAllocMemorySize;
			}
			return result;
		}

		public const int ConstSplitePoolNum = 16;

		public const int ConstSplitePoolMask = 15;

		private static object MemoryLock = new object();

		private static long TotalNewAllocMemorySize = 0L;

		private Dictionary<int, MemoryStackArray> MemoryDict = new Dictionary<int, MemoryStackArray>();

		private Dictionary<MemoryBlock, StackTrace> MemoryBlockStackTraceDict = new Dictionary<MemoryBlock, StackTrace>();

		private List<int> BlockSizeList = new List<int>();

		private Dictionary<MemoryBlock, byte> BlockDict = new Dictionary<MemoryBlock, byte>();
	}
}
