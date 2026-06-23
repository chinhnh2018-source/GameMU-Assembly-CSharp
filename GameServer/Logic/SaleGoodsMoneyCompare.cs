using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class SaleGoodsMoneyCompare : IComparer<SaleGoodsData>
	{
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int num = x.SalingGoodsData.SaleYuanBao - y.SalingGoodsData.SaleYuanBao;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				result = x.SalingGoodsData.SaleMoney1 - y.SalingGoodsData.SaleMoney1;
			}
			return result;
		}

		public static readonly SaleGoodsMoneyCompare Instance = new SaleGoodsMoneyCompare();
	}
}
