using System;
using System.Collections.Generic;
using Server.Data;

namespace HSGameEngine.GameEngine.Logic
{
	public class FuBenMapItem
	{
		public int FuBenID;

		public int MapCode;

		public int MaxTime = -1;

		public int MinSaoDangTimer = -1;

		public int Money1;

		public int Experience;

		public List<GoodsData> GoodsDataList;
	}
}
