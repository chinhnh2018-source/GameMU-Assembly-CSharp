using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class JieRiMeiRiLeiJiActivity : Activity
	{
		public override AwardItem GetAward(int _params)
		{
			AwardItem awardItem = null;
			int key = _params / 1000;
			int num = _params % 1000;
			AwardItem result;
			if (num < 1)
			{
				result = null;
			}
			else
			{
				if (this.DayAwardItemDict.ContainsKey(key))
				{
					if (this.DayAwardItemDict[key].Count < num)
					{
						return null;
					}
					awardItem = this.DayAwardItemDict[key][num - 1];
				}
				result = awardItem;
			}
			return result;
		}

		public AwardItem GetOccAward(int _params)
		{
			AwardItem result = null;
			int key = _params / 1000;
			int key2 = _params % 1000;
			if (this.DayOccAwardItemDict.ContainsKey(key))
			{
				if (this.DayOccAwardItemDict[key].ContainsKey(key2))
				{
					result = this.DayOccAwardItemDict[key][key2];
				}
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
				if (flag)
				{
					string text = "";
					if (null != award)
					{
						text = EventLogManager.MakeGoodsDataPropString(award.GoodsDataList);
					}
					if (!string.IsNullOrEmpty(text))
					{
						text += "@";
					}
					if (null != occAward)
					{
						text += EventLogManager.MakeGoodsDataPropString(occAward.GoodsDataList);
					}
					EventLogManager.AddJieRiMeiRiLeiJiEvent(client, _params, text);
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

		public Dictionary<int, List<AwardItem>> DayAwardItemDict = new Dictionary<int, List<AwardItem>>();

		public Dictionary<int, Dictionary<int, AwardItem>> DayOccAwardItemDict = new Dictionary<int, Dictionary<int, AwardItem>>();
	}
}
