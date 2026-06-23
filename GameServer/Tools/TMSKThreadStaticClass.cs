using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace GameServer.Tools
{
	public class TMSKThreadStaticClass
	{
		public static TMSKThreadStaticClass GetInstance()
		{
			if (null == TMSKThreadStaticClass.ThreadStaticClass)
			{
				TMSKThreadStaticClass.ThreadStaticClass = new TMSKThreadStaticClass();
			}
			return TMSKThreadStaticClass.ThreadStaticClass;
		}

		public TMSKThreadStaticClass()
		{
			foreach (KeyValuePair<int, int> keyValuePair in GameManager.MemoryPoolConfigDict)
			{
				int num = TMSKThreadStaticClass.Log2(keyValuePair.Key);
				if (num < this.MemoryBlockNumArray.Length)
				{
					this.MemoryBlockStackArray[num] = new Stack<MemoryBlock>();
					this.MemoryBlockNumArray[num] = Math.Max(10, keyValuePair.Value / 100);
					this.MemoryBlockSizeArray[num] = keyValuePair.Key;
				}
			}
			Stack<MemoryBlock> stack = null;
			int num2 = 10;
			int num3 = 0;
			for (int i = this.MemoryBlockStackArray.Length - 1; i >= 0; i--)
			{
				if (null == this.MemoryBlockStackArray[i])
				{
					if (null == stack)
					{
						this.MemoryBlockStackArray[i] = new Stack<MemoryBlock>();
						this.MemoryBlockNumArray[i] = 10;
						this.MemoryBlockSizeArray[i] = 0;
					}
					else
					{
						this.MemoryBlockStackArray[i] = stack;
						this.MemoryBlockNumArray[i] = num2;
						this.MemoryBlockSizeArray[i] = num3;
					}
				}
				else
				{
					stack = this.MemoryBlockStackArray[i];
					num2 = this.MemoryBlockNumArray[i];
					num3 = this.MemoryBlockSizeArray[i];
				}
			}
		}

		protected override void Finalize()
		{
			try
			{
				for (int i = 0; i < this.QueueMemoryStream.Count; i++)
				{
					MemoryStream memoryStream = this.QueueMemoryStream.Dequeue();
					memoryStream.Dispose();
				}
				foreach (Stack<MemoryBlock> stack in this.MemoryBlockStackArray)
				{
					while (stack.Count > 0)
					{
						Global._MemoryManager.Push(stack.Pop());
					}
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public void PushMemoryStream(MemoryStream ms)
		{
			try
			{
				if (this.QueueMemoryStream.Count <= 30)
				{
					this.QueueMemoryStream.Enqueue(ms);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
		}

		public MemoryStream PopMemoryStream()
		{
			try
			{
				if (this.QueueMemoryStream.Count > 0)
				{
					MemoryStream memoryStream = this.QueueMemoryStream.Dequeue();
					memoryStream.Position = 0L;
					memoryStream.SetLength(0L);
					return memoryStream;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
			return new MemoryStream();
		}

		public static int Log2(int size)
		{
			return (int)Math.Ceiling(Math.Log((double)size, 2.0));
		}

		public void PushMemoryBlock(MemoryBlock item)
		{
			try
			{
				int num = TMSKThreadStaticClass.Log2(item.BlockSize);
				int num2 = this.MemoryBlockSizeArray[num];
				if (num2 > 0)
				{
					if (num2 < item.BlockSize)
					{
						num++;
					}
					if (this.MemoryBlockStackArray[num].Count <= this.MemoryBlockNumArray[num])
					{
						this.MemoryBlockStackArray[num].Push(item);
					}
					else
					{
						Global._MemoryManager.Push(item);
					}
				}
				else if (this.MemoryBlockStackArray[num].Count <= this.MemoryBlockNumArray[num])
				{
					this.MemoryBlockStackArray[num].Push(item);
				}
				else
				{
					Global._MemoryManager.Push(item);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
		}

		public MemoryBlock PopMemoryBlock(int needSize)
		{
			try
			{
				int num = TMSKThreadStaticClass.Log2(needSize);
				int num2 = this.MemoryBlockSizeArray[num];
				if (num2 > 0)
				{
					if (num2 < needSize)
					{
						num++;
					}
					if (this.MemoryBlockStackArray[num].Count > 0)
					{
						return this.MemoryBlockStackArray[num].Pop();
					}
					return Global._MemoryManager.Pop(needSize);
				}
				else
				{
					if (this.MemoryBlockStackArray[num].Count > 0)
					{
						return this.MemoryBlockStackArray[num].Pop();
					}
					return Global._MemoryManager.Pop((int)Math.Pow(2.0, (double)num));
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "");
			}
			return null;
		}

		private const int QueueMemoryStreamMaxSize = 30;

		[ThreadStatic]
		private static TMSKThreadStaticClass ThreadStaticClass = null;

		private Queue<MemoryStream> QueueMemoryStream = new Queue<MemoryStream>();

		private Stack<MemoryBlock>[] MemoryBlockStackArray = new Stack<MemoryBlock>[20];

		private int[] MemoryBlockNumArray = new int[20];

		private int[] MemoryBlockSizeArray = new int[20];
	}
}
