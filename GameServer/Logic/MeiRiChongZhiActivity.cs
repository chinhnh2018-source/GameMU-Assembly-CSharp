using System;
using System.Collections.Generic;
using GameServer.Logic.ActivityNew;

namespace GameServer.Logic
{
	public class MeiRiChongZhiActivity : Activity
	{
		public override AwardItem GetAward(GameClient client, int _params)
		{
			AwardItem result;
			if (this.AwardDict.ContainsKey(_params))
			{
				result = this.AwardDict[_params];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override List<int> GetAwardMinConditionlist()
		{
			List<int> list = new List<int>();
			int count = this.AwardDict.Count;
			for (int i = 1; i <= count; i++)
			{
				if (this.AwardDict.ContainsKey(i))
				{
					list.Add(this.AwardDict[i].MinAwardCondionValue);
				}
			}
			return list;
		}

		public int GetIDByYuanBao(int NeedYuanbao)
		{
			foreach (KeyValuePair<int, AwardItem> keyValuePair in this.AwardDict)
			{
				if (keyValuePair.Value.MinAwardCondionValue == NeedYuanbao)
				{
					return keyValuePair.Key;
				}
			}
			return -1;
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem awardItem = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				awardItem = this.AwardDict[_params];
			}
			bool result;
			if (null == awardItem)
			{
				result = false;
			}
			else
			{
				client.ClientData.ClearAwardRecord((RoleAwardMsg)this.ActivityType);
				WeedEndInputActivity weekEndInputActivity = HuodongCachingMgr.GetWeekEndInputActivity();
				if (null != weekEndInputActivity)
				{
					weekEndInputActivity.GiveAward(client, awardItem.MinAwardCondionValue);
				}
				bool flag = base.GiveAward(client, awardItem);
				GameManager.ClientMgr.NotifyGetAwardMsg(client, (RoleAwardMsg)this.ActivityType, "");
				result = flag;
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int nBtnIndex)
		{
			int num = 0;
			WeedEndInputActivity weekEndInputActivity = HuodongCachingMgr.GetWeekEndInputActivity();
			if (null != weekEndInputActivity)
			{
				num = weekEndInputActivity.GetNeedGoodsSpace(client, this.AwardDict[nBtnIndex].MinAwardCondionValue);
			}
			num += this.AwardDict[nBtnIndex].GoodsDataList.Count;
			return Global.CanAddGoodsNum(client, num);
		}

		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();
	}
}
