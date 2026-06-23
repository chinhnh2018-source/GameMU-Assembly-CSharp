using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class JieriDaLiBaoActivity : Activity
	{
		public override bool GiveAward(GameClient client, int _params)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				bool flag = true;
				if (null != this.MyAwardItem)
				{
					flag = base.GiveAward(client, this.MyAwardItem);
				}
				if (flag)
				{
					int occupation = client.ClientData.Occupation;
					AwardItem occAward = this.GetOccAward(occupation);
					if (null != occAward)
					{
						flag = base.GiveAward(client, occAward);
					}
				}
				result = flag;
			}
			return result;
		}

		public AwardItem GetOccAward(int _params)
		{
			AwardItem result = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				result = this.OccAwardItemDict[_params];
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				int occupation = client.ClientData.Occupation;
				AwardItem occAward = this.GetOccAward(occupation);
				result = ((this.MyAwardItem.GoodsDataList.Count <= 0 && (occAward == null || occAward.GoodsDataList.Count <= 0)) || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList));
			}
			return result;
		}

		public AwardItem MyAwardItem = new AwardItem();

		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
