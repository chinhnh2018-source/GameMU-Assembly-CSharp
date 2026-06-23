using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OlympicsGuessDataResult
	{
		[ProtoMember(1)]
		public int Type;

		[ProtoMember(2)]
		public List<OlympicsGuessData> List = new List<OlympicsGuessData>();

		[ProtoMember(3)]
		public int DayID;
	}
}
