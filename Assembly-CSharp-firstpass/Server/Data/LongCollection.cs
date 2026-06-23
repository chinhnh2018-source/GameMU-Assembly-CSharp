using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LongCollection
	{
		public long this[int index]
		{
			get
			{
				if (this.MoneyDict == null)
				{
					this.MoneyDict = new Dictionary<int, long>();
				}
				long result;
				if (this.MoneyDict.TryGetValue(index, ref result))
				{
					return result;
				}
				return 0L;
			}
			set
			{
				if (this.MoneyDict == null)
				{
					this.MoneyDict = new Dictionary<int, long>();
				}
				this.MoneyDict[index] = value;
			}
		}

		[ProtoMember(1)]
		protected Dictionary<int, long> MoneyDict = new Dictionary<int, long>();
	}
}
