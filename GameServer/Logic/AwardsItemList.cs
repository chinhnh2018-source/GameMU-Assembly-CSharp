using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;

namespace GameServer.Logic
{
	public class AwardsItemList
	{
		public List<AwardsItemData> Items
		{
			get
			{
				return this.list;
			}
		}

		public AwardsItemData ParseItem(string awardsString)
		{
			if (!string.IsNullOrEmpty(awardsString))
			{
				string[] array = awardsString.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						return new AwardsItemData
						{
							GoodsID = Global.SafeConvertToInt32(array2[0]),
							GoodsNum = Global.SafeConvertToInt32(array2[1]),
							Binding = Global.SafeConvertToInt32(array2[2]),
							Level = Global.SafeConvertToInt32(array2[3]),
							AppendLev = Global.SafeConvertToInt32(array2[4]),
							IsHaveLuckyProp = Global.SafeConvertToInt32(array2[5]),
							ExcellencePorpValue = Global.SafeConvertToInt32(array2[6]),
							EndTime = "1900-01-01 12:00:00"
						};
					}
				}
			}
			return null;
		}

		public bool ItemEqual(AwardsItemData item0, AwardsItemData item1)
		{
			return item0.GoodsID == item1.GoodsID && item0.Binding == item1.Binding && item0.Level == item1.Level && item0.AppendLev == item1.AppendLev && item0.IsHaveLuckyProp == item1.IsHaveLuckyProp && item0.ExcellencePorpValue == item1.ExcellencePorpValue && item0.Occupation == item1.Occupation && item0.EndTime == item1.EndTime;
		}

		public void Add(string awardsString)
		{
			if (!string.IsNullOrEmpty(awardsString))
			{
				string[] array = awardsString.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					AwardsItemData awardsItemData = this.ParseItem(array[i]);
					if (null != awardsItemData)
					{
						this.list.Add(awardsItemData);
					}
				}
			}
		}

		public void AddNoRepeat(string awardsString)
		{
			if (!string.IsNullOrEmpty(awardsString))
			{
				string[] array = awardsString.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					AwardsItemData awardsItemData = this.ParseItem(array[i]);
					if (null != awardsItemData)
					{
						int j;
						for (j = 0; j < this.list.Count; j++)
						{
							AwardsItemData awardsItemData2 = this.list[j];
							if (this.ItemEqual(awardsItemData, awardsItemData2))
							{
								awardsItemData2.GoodsNum += awardsItemData.GoodsNum;
								break;
							}
						}
						if (j == this.list.Count)
						{
							this.list.Add(awardsItemData);
						}
					}
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.list.Count; i++)
			{
				AwardsItemData awardsItemData = this.list[i];
				stringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5},{6}|", new object[]
				{
					awardsItemData.GoodsID,
					awardsItemData.GoodsNum,
					awardsItemData.Binding,
					awardsItemData.Level,
					awardsItemData.AppendLev,
					awardsItemData.IsHaveLuckyProp,
					awardsItemData.ExcellencePorpValue
				});
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'|'
			});
		}

		private List<AwardsItemData> list = new List<AwardsItemData>();
	}
}
