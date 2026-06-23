using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class HeFuLuoLanActivity : Activity
	{
		public HeFuLuoLanAward GetHeFuLuoLanAward(int _param)
		{
			HeFuLuoLanAward result;
			if (this.HeFuLuoLanAwardDict.ContainsKey(_param))
			{
				result = this.HeFuLuoLanAwardDict[_param];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public new AwardItem GetAward(int _param)
		{
			AwardItem result;
			if (this.HeFuLuoLanAwardDict.ContainsKey(_param))
			{
				result = this.HeFuLuoLanAwardDict[_param].awardData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _param)
		{
			AwardItem award = this.GetAward(_param);
			return base.GiveAward(client, award);
		}

		public new bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _param)
		{
			AwardItem award = this.GetAward(_param);
			return award == null || award.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, award.GoodsDataList);
		}

		public Dictionary<int, HeFuLuoLanAward> HeFuLuoLanAwardDict = new Dictionary<int, HeFuLuoLanAward>();
	}
}
