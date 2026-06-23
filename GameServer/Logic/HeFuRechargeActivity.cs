using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class HeFuRechargeActivity : Activity
	{
		public HeFuRechargeData getDataByDay(int rank)
		{
			HeFuRechargeData result = null;
			if (this.ConfigDict.ContainsKey(rank))
			{
				result = this.ConfigDict[rank];
			}
			return result;
		}

		public Dictionary<int, HeFuRechargeData> ConfigDict = new Dictionary<int, HeFuRechargeData>();

		public string strcoe;
	}
}
