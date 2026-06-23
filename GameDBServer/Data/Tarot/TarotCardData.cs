using System;
using ProtoBuf;

namespace GameDBServer.Data.Tarot
{
	[ProtoContract]
	public class TarotCardData
	{
		public TarotCardData()
		{
		}

		public TarotCardData(string data)
		{
			string[] array = data.Split(new char[]
			{
				'_'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length >= 3)
			{
				this.GoodId = Convert.ToInt32(array[0]);
				this.Level = Convert.ToInt32(array[1]);
				this.Postion = Convert.ToByte(array[2]);
				if (array.Length >= 4)
				{
					this.TarotMoney = Convert.ToInt32(array[3]);
				}
			}
		}

		public string GetDataStrInfo()
		{
			return string.Format("{0}_{1}_{2}_{3};", new object[]
			{
				this.GoodId,
				this.Level,
				this.Postion,
				this.TarotMoney
			});
		}

		[ProtoMember(1)]
		public int GoodId = 0;

		[ProtoMember(2)]
		public int Level = 0;

		[ProtoMember(3)]
		public byte Postion = 0;

		[ProtoMember(4)]
		public int TarotMoney = 0;
	}
}
