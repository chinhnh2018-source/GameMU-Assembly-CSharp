using System;
using Server.Data;

namespace GameServer.Logic
{
	public class DanBiChongZhiAwardDetail
	{
		public int TotalAwardCntWithOcc(GameClient client)
		{
			int num = 0;
			if (this.AwardDict.GoodsDataList != null)
			{
				num += this.AwardDict.GoodsDataList.Count;
			}
			if (this.AwardDict2.GoodsDataList != null)
			{
				foreach (GoodsData goodsData in this.AwardDict2.GoodsDataList)
				{
					if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
					{
						num++;
					}
				}
			}
			if (this.EffectTimeAwardDict != null)
			{
				num += this.EffectTimeAwardDict.GoodsCnt();
			}
			return num;
		}

		public int ID = 0;

		public AwardItem AwardDict = new AwardItem();

		public AwardItem AwardDict2 = new AwardItem();

		public AwardEffectTimeItem EffectTimeAwardDict = new AwardEffectTimeItem();

		public int MinYuanBao = 0;

		public int MaxYuanBao = 0;

		public int SinglePurchase = 0;
	}
}
