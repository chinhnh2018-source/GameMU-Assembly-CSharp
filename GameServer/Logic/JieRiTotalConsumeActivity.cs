using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;

namespace GameServer.Logic
{
	public class JieRiTotalConsumeActivity : Activity
	{
		public override AwardItem GetAward(int _params)
		{
			AwardItem result = null;
			if (this.AwardItemDict.ContainsKey(_params))
			{
				result = this.AwardItemDict[_params];
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

		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem award = this.GetAward(_params);
			bool flag = true;
			if (null != award)
			{
				flag = base.GiveAward(client, award);
			}
			if (flag)
			{
				AwardItem occAward = this.GetOccAward(_params);
				if (null != occAward)
				{
					flag = base.GiveAward(client, occAward);
				}
			}
			return flag;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			AwardItem award = this.GetAward(_params);
			AwardItem occAward = this.GetOccAward(_params);
			List<GoodsData> list = new List<GoodsData>();
			if (null != award)
			{
				list.AddRange(award.GoodsDataList);
			}
			if (null != occAward)
			{
				list.AddRange(occAward.GoodsDataList);
			}
			return list.Count <= 0 || Global.CanAddGoodsDataList(client, list);
		}

		public override string GetAwardMinConditionValues()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = this.AwardItemDict.Count;
			for (int i = 1; i <= count; i++)
			{
				if (this.AwardItemDict.ContainsKey(i))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("_");
					}
					stringBuilder.Append(this.AwardItemDict[i].MinAwardCondionValue);
				}
			}
			return stringBuilder.ToString();
		}

		public Dictionary<int, AwardItem> AwardItemDict = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
