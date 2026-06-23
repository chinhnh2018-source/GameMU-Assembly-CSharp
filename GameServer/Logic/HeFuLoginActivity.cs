using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class HeFuLoginActivity : Activity
	{
		public override AwardItem GetAward(int _params)
		{
			AwardItem result = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				result = this.AwardDict[_params];
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			AwardItem award = this.GetAward(_params);
			return null == award || award.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, award.GoodsDataList);
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem award = this.GetAward(_params);
			return null != award && base.GiveAward(client, award);
		}

		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
