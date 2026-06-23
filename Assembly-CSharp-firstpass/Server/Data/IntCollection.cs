using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class IntCollection
	{
		public int this[int index]
		{
			get
			{
				if (this.MoneyDict == null)
				{
					this.MoneyDict = new Dictionary<int, int>();
				}
				int result;
				if (this.MoneyDict.TryGetValue(index, ref result))
				{
					return result;
				}
				return 0;
			}
			set
			{
				if (this.MoneyDict == null)
				{
					this.MoneyDict = new Dictionary<int, int>();
				}
				this.MoneyDict[index] = value;
			}
		}

		[ProtoMember(1)]
		protected Dictionary<int, int> MoneyDict = new Dictionary<int, int>();
	}
}
