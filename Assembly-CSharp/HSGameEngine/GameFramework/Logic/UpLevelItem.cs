using System;
using System.Collections.Generic;
using Server.Data;

namespace HSGameEngine.GameFramework.Logic
{
	public class UpLevelItem
	{
		public int ID;

		public int ToLevel;

		public List<GoodsData> GoodsDataList;

		public int BindYuanBao;

		public int BindMoney;

		public int MoJing;

		public int Occupation = -1;
	}
}
