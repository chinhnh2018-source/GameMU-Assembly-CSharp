using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoServerJingJiaState
	{
		public KuaFuLueDuoServerJingJiaState Clone()
		{
			return new KuaFuLueDuoServerJingJiaState
			{
				EndTime = this.EndTime,
				JingJiaList = this.JingJiaList,
				Round = this.Round,
				ServerId = this.ServerId,
				State = this.State,
				ZiYuan = this.ZiYuan
			};
		}

		[ProtoMember(1)]
		public int ServerId;

		[ProtoMember(2)]
		public int ZiYuan;

		[ProtoMember(4)]
		public int Round;

		[ProtoMember(5)]
		public int State;

		[ProtoMember(6)]
		public DateTime EndTime;

		[ProtoMember(7)]
		public List<KuaFuLueDuoBangHuiJingJiaData> JingJiaList;
	}
}
