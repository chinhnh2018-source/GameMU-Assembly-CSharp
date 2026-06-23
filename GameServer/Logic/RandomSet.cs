using System;

namespace GameServer.Logic
{
	public class RandomSet
	{
		public int[] ResultList { get; private set; }

		public RandomSet(int count)
		{
			this.AllCount = count;
			this.ResultList = new int[count];
			for (int i = 0; i < count; i++)
			{
				this.ResultList[i] = i;
			}
		}

		public int RandomNext()
		{
			int randomNumber = Global.GetRandomNumber(this.RandomCount, this.AllCount);
			int num = this.ResultList[this.RandomCount];
			this.ResultList[this.RandomCount] = this.ResultList[randomNumber];
			this.ResultList[randomNumber] = num;
			return num;
		}

		private int RandomCount = 0;

		private int AllCount = 0;
	}
}
