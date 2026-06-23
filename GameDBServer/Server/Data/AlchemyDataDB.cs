using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AlchemyDataDB
	{
		public string getStringValue(Dictionary<int, int> dict)
		{
			string text = "";
			string result;
			if (null == dict)
			{
				result = text;
			}
			else
			{
				foreach (KeyValuePair<int, int> keyValuePair in dict)
				{
					text += string.Format("{0},{1}|", keyValuePair.Key, keyValuePair.Value);
				}
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Substring(0, text.Length - 1);
				}
				result = text;
			}
			return result;
		}

		[ProtoMember(1)]
		public AlchemyData BaseData = new AlchemyData();

		[ProtoMember(2)]
		public int RoleID = 0;

		[ProtoMember(3)]
		public Dictionary<int, int> HistCost = new Dictionary<int, int>();

		[ProtoMember(4)]
		public int ElementDayID = 0;

		[ProtoMember(5)]
		public string rollbackType = "";
	}
}
