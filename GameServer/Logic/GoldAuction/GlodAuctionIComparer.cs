using System;
using System.Collections.Generic;

namespace GameServer.Logic.GoldAuction
{
	public class GlodAuctionIComparer : IComparer<AuctionItemS2C>
	{
		public GlodAuctionIComparer(int ordeType, bool isAscend)
		{
			this.IOrdeType = ordeType;
			this.IsAscend = isAscend;
		}

		public int Compare(AuctionItemS2C d1, AuctionItemS2C d2)
		{
			int num = this.IsAscend ? -1 : 1;
			long num2 = d1.LastTime;
			long num3 = d2.LastTime;
			if (1 == this.IOrdeType)
			{
				num2 = d1.Price;
				num3 = d2.Price;
			}
			else if (2 == this.IOrdeType)
			{
				num2 = d1.MaxPrice;
				num3 = d2.MaxPrice;
			}
			int result;
			if (num2 < num3)
			{
				result = num;
			}
			else if (num2 > num3)
			{
				result = -num;
			}
			else if (d1.LastTime < d2.LastTime)
			{
				result = num;
			}
			else if (d1.LastTime > d2.LastTime)
			{
				result = -num;
			}
			else if (d1.Price < d2.Price)
			{
				result = num;
			}
			else if (d1.Price > d2.Price)
			{
				result = -num;
			}
			else if (d1.MaxPrice < d2.MaxPrice)
			{
				result = num;
			}
			else if (d1.MaxPrice > d2.MaxPrice)
			{
				result = -num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private int IOrdeType;

		private bool IsAscend;
	}
}
