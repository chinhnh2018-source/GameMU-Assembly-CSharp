using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class JieRiZiKaLiaBaoActivity : Activity
	{
		public List<int> GetIndexByType(int type)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, JieRiZiKa> keyValuePair in this.JieRiZiKaDict)
			{
				if (type == keyValuePair.Value.type)
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list;
		}

		public new JieRiZiKa GetAward(int id)
		{
			JieRiZiKa result = null;
			if (this.JieRiZiKaDict.ContainsKey(id))
			{
				result = this.JieRiZiKaDict[id];
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			JieRiZiKa award = this.GetAward(_params);
			return null != award && base.GiveAward(client, award.MyAwardItem);
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int _params)
		{
			JieRiZiKa award = this.GetAward(_params);
			return null != award && null != award.MyAwardItem && (award.MyAwardItem.GoodsDataList.Count <= 0 || Global.CanAddGoodsDataList(client, award.MyAwardItem.GoodsDataList));
		}

		public Dictionary<int, JieRiZiKa> JieRiZiKaDict = new Dictionary<int, JieRiZiKa>();
	}
}
