using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class JieRiDengLuActivity : Activity
	{
		public override AwardItem GetAward(GameClient client, int day)
		{
			AwardItem result = null;
			if (this.AwardItemDict.ContainsKey(day))
			{
				result = this.AwardItemDict[day];
			}
			return result;
		}

		public AwardItem GetOccAward(GameClient client, int day)
		{
			AwardItem result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				AwardItem awardItem = null;
				int key = day * 100 + client.ClientData.Occupation;
				if (this.OccAwardItemDict.ContainsKey(key))
				{
					awardItem = this.OccAwardItemDict[key];
				}
				result = awardItem;
			}
			return result;
		}

		public AwardItem GetOccAward(int key)
		{
			AwardItem result = null;
			if (this.OccAwardItemDict.ContainsKey(key))
			{
				result = this.OccAwardItemDict[key];
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				AwardItem award = this.GetAward(client, _params);
				bool flag = true;
				if (null != award)
				{
					flag = base.GiveAward(client, award);
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

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				AwardItem award = this.GetAward(client, _params);
				AwardItem occAward = this.GetOccAward(client, _params);
				List<GoodsData> list = new List<GoodsData>();
				if (award != null && null != award.GoodsDataList)
				{
					list.AddRange(award.GoodsDataList);
				}
				if (occAward != null && null != occAward.GoodsDataList)
				{
					list.AddRange(occAward.GoodsDataList);
				}
				result = (list.Count <= 0 || Global.CanAddGoodsDataList(client, list));
			}
			return result;
		}

		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
