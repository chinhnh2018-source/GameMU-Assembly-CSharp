using System;
using Server.Data;

namespace GameServer.Logic.YueKa
{
	public class YueKaDetail
	{
		public YueKaDetail()
		{
			this.HasYueKa = 0;
			this.BegOffsetDay = 0;
			this.EndOffsetDay = 0;
			this.CurOffsetDay = 0;
			this.AwardInfo = "";
		}

		public YueKaData ToYueKaData()
		{
			return new YueKaData
			{
				HasYueKa = (this.HasYueKa == 1),
				CurrDay = this.CurDayOfPerYueKa(),
				AwardInfo = this.AwardInfo,
				RemainDay = this.RemainDayOfYueKa()
			};
		}

		public void ParseFrom(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				string[] array = str.Split(new char[]
				{
					','
				});
				if (array.Length == 5)
				{
					this.HasYueKa = Convert.ToInt32(array[0]);
					this.BegOffsetDay = Convert.ToInt32(array[1]);
					this.EndOffsetDay = Convert.ToInt32(array[2]);
					this.CurOffsetDay = Convert.ToInt32(array[3]);
					this.AwardInfo = array[4];
				}
			}
		}

		public string SerializeToString()
		{
			string result;
			if (this.HasYueKa == 0)
			{
				result = "0,0,0,0,0";
			}
			else
			{
				result = string.Format("{0},{1},{2},{3},{4}", new object[]
				{
					1,
					this.BegOffsetDay,
					this.EndOffsetDay,
					this.CurOffsetDay,
					this.AwardInfo
				});
			}
			return result;
		}

		public int CurDayOfPerYueKa()
		{
			int result;
			if (this.HasYueKa == 0)
			{
				result = 0;
			}
			else
			{
				result = (this.CurOffsetDay - this.BegOffsetDay) % YueKaManager.DAYS_PER_YUE_KA + 1;
			}
			return result;
		}

		public int RemainDayOfYueKa()
		{
			int result;
			if (this.HasYueKa == 0)
			{
				result = 0;
			}
			else
			{
				result = this.EndOffsetDay - this.CurOffsetDay;
			}
			return result;
		}

		public int HasYueKa = 0;

		public int BegOffsetDay = 0;

		public int EndOffsetDay = 0;

		public int CurOffsetDay = 0;

		public string AwardInfo = "";
	}
}
