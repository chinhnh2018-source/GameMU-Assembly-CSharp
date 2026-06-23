using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameDBServer.Data.Tarot
{
	[ProtoContract]
	public class TarotKingData
	{
		public TarotKingData()
		{
			this.AddtionDict = new Dictionary<int, int>();
		}

		public TarotKingData(string data)
		{
			string[] array = data.Split(new char[]
			{
				'_'
			});
			if (array.Length == 3)
			{
				this.StartTime = Convert.ToInt64(array[0]);
				this.BufferSecs = Convert.ToInt64(array[1]);
				string[] array2 = array[2].Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length == 3)
				{
					this.AddtionDict = new Dictionary<int, int>();
					foreach (string text in array2)
					{
						string[] array4 = text.Split(new char[]
						{
							'@'
						}, StringSplitOptions.RemoveEmptyEntries);
						this.AddtionDict.Add(Convert.ToInt32(array4[0]), Convert.ToInt32(array4[1]));
					}
				}
			}
		}

		public string GetDataStrInfo()
		{
			string text = string.Empty;
			if (this.AddtionDict != null && this.AddtionDict.Count == 3)
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.AddtionDict)
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						keyValuePair.Key,
						"@",
						keyValuePair.Value,
						","
					});
				}
			}
			return string.Format("{0}_{1}_{2}", this.StartTime, this.BufferSecs, text);
		}

		[ProtoMember(1)]
		public long StartTime = 0L;

		[ProtoMember(2)]
		public long BufferSecs = 0L;

		[ProtoMember(3)]
		public Dictionary<int, int> AddtionDict;
	}
}
