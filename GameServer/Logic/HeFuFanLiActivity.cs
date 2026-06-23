using System;
using System.Text;

namespace GameServer.Logic
{
	public class HeFuFanLiActivity : KingActivity
	{
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = _params
			});
		}

		public override string GetAwardMinConditionValues()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = this.AwardDict.Count;
			for (int i = 1; i <= count; i++)
			{
				if (this.AwardDict.ContainsKey(i))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("_");
					}
					stringBuilder.Append(string.Format("{0},{1}", this.AwardDict[i].MinAwardCondionValue, this.AwardDict[i].MinAwardCondionValue2));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
