using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class SaleGoodsMoneyCompare2 : IComparer<SaleGoodsData>
	{
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int num = y.SalingGoodsData.SaleYuanBao - x.SalingGoodsData.SaleYuanBao;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				result = y.SalingGoodsData.SaleMoney1 - x.SalingGoodsData.SaleMoney1;
			}
			return result;
		}

		public static readonly SaleGoodsMoneyCompare2 Instance = new SaleGoodsMoneyCompare2();
	}
}
