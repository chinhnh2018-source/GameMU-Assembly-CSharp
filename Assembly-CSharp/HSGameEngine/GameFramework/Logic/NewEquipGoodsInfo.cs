using System;

namespace HSGameEngine.GameFramework.Logic
{
	public struct NewEquipGoodsInfo
	{
		public NewEquipGoodsInfo(int dbID, int up)
		{
			this.goodsDbID = dbID;
			this.zhanLiUp = up;
		}

		public int goodsDbID;

		public int zhanLiUp;
	}
}
