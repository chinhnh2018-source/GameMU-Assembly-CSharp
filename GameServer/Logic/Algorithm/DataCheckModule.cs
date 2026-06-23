using System;

namespace GameServer.Logic.Algorithm
{
	public class DataCheckModule
	{
		public class CheckLinerValue
		{
			public CheckLinerValue(int _maxSize)
			{
				this.maxSize = _maxSize;
				this.maxPos = this.maxSize - 1;
				this.valueArray = new int[this.maxSize];
			}

			public void Clear()
			{
				Array.Clear(this.valueArray, 0, this.valueArray.Length);
			}

			public bool Push(int v, int num, int limit)
			{
				bool result;
				if (num > this.maxSize)
				{
					result = false;
				}
				else
				{
					lock (this.mutex)
					{
						if (num <= this.dataCount)
						{
							int num2 = 0;
							int num3 = this.dataPos;
							for (int i = 1; i < num; i++)
							{
								num3 = ((num3 == 0) ? this.maxPos : (num3 - 1));
								num2 += this.valueArray[num3];
							}
							num2 += v;
							if (num2 > limit)
							{
								return false;
							}
						}
						this.valueArray[this.dataPos] = v;
						this.dataPos = ((this.dataPos >= this.maxPos) ? 0 : (this.dataPos + 1));
						if (this.dataCount < this.maxSize)
						{
							this.dataCount++;
						}
					}
					result = true;
				}
				return result;
			}

			public int[] valueArray;

			private int maxSize;

			private int maxPos;

			private int dataPos;

			private int dataCount;

			private object mutex = new object();
		}
	}
}
