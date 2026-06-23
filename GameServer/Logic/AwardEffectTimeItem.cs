using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class AwardEffectTimeItem
	{
		public void Init(string goodsList, string timeList, string note)
		{
			if (!string.IsNullOrEmpty(goodsList) && !string.IsNullOrEmpty(timeList))
			{
				string[] array = goodsList.Split(new char[]
				{
					'|'
				});
				string[] array2 = timeList.Split(new char[]
				{
					'|'
				});
				if (array.Length == array2.Length)
				{
					this.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, note);
					this.GoodsTimeList = HuodongCachingMgr.ParseGoodsTimeList(array2, note);
				}
			}
		}

		public int GoodsCnt()
		{
			return (this.GoodsDataList != null) ? this.GoodsDataList.Count : 0;
		}

		public AwardItem ToAwardItem()
		{
			AwardItem awardItem = new AwardItem();
			if (this.GoodsDataList != null)
			{
				for (int i = 0; i < this.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = this.GoodsDataList[i];
					bool flag = false;
					if (this.GoodsTimeList != null && this.GoodsTimeList.Count > i)
					{
						AwardEffectTimeItem.TimeDetail timeDetail = this.GoodsTimeList[i];
						if (timeDetail.Type == AwardEffectTimeItem.EffectTimeType.ETT_LastMinutesFromNow)
						{
							DateTime dateTime = TimeUtil.NowDateTime();
							goodsData.Starttime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
							goodsData.Endtime = dateTime.AddMinutes((double)timeDetail.LastMinutes).ToString("yyyy-MM-dd HH:mm:ss");
							flag = true;
						}
						else if (timeDetail.Type == AwardEffectTimeItem.EffectTimeType.ETT_AbsoluteLastTime)
						{
							goodsData.Starttime = timeDetail.AbsoluteStartTime;
							goodsData.Endtime = timeDetail.AbsoluteEndTime;
							flag = true;
						}
					}
					if (!flag)
					{
						goodsData.Starttime = "1900-01-01 12:00:00";
						goodsData.Endtime = "1900-01-01 12:00:00";
					}
					awardItem.GoodsDataList.Add(goodsData);
				}
			}
			return awardItem;
		}

		private List<GoodsData> GoodsDataList = null;

		private List<AwardEffectTimeItem.TimeDetail> GoodsTimeList = null;

		public enum EffectTimeType
		{
			ETT_Unknown,
			ETT_LastMinutesFromNow,
			ETT_AbsoluteLastTime
		}

		public class TimeDetail
		{
			public AwardEffectTimeItem.EffectTimeType Type = AwardEffectTimeItem.EffectTimeType.ETT_Unknown;

			public int LastMinutes = 0;

			public string AbsoluteStartTime = "1900-01-01 12:00:00";

			public string AbsoluteEndTime = "1900-01-01 12:00:00";
		}
	}
}
