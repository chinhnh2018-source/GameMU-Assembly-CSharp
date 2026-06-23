using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	public class AuctionConfig
	{
		public int GetTimeByAuction(int Auction)
		{
			for (int i = 0; i < this.OrderList.Count; i++)
			{
				if (Auction == this.OrderList[i])
				{
					return this.TimeList[i];
				}
			}
			return -1;
		}

		public int GetNextAuction(int Auction)
		{
			bool flag = false;
			foreach (int num in this.OrderList)
			{
				if (Auction == num)
				{
					flag = true;
				}
				else if (GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)num))
				{
					if (flag)
					{
						return num;
					}
				}
			}
			return -1;
		}

		public int ID;

		public string Name;

		public List<int> OrderList = new List<int>();

		public List<int> TimeList = new List<int>();

		public int OriginPrice;

		public int UnitPrice;

		public int MaxPrice;

		public string SuccessTitle;

		public string SuccessIntro;

		public string FailTitle;

		public string FailIntro;
	}
}
