using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	[ProtoContract]
	public class KuaFuServerLoginData
	{
		public KuaFuServerLoginData Clone()
		{
			return new KuaFuServerLoginData
			{
				RoleId = this.RoleId,
				ServerId = this.ServerId,
				ServerIp = this.ServerIp,
				ServerPort = this.ServerPort,
				WebLoginToken = this.WebLoginToken,
				EndTicks = this.EndTicks,
				FuBenSeqId = this.FuBenSeqId,
				GameId = this.GameId,
				GameType = this.GameType,
				TempRoleID = this.TempRoleID,
				PTID = this.PTID,
				SignToken = this.SignToken,
				TargetServerID = this.TargetServerID,
				SignCode = this.SignCode,
				Private = this.Private,
				Param1 = this.Param1,
				Param2 = this.Param2,
				Line = this.Line
			};
		}

		[ProtoMember(1)]
		public WebLoginToken WebLoginToken;

		[ProtoMember(2)]
		public int RoleId;

		[ProtoMember(3)]
		public long GameId;

		[ProtoMember(4)]
		public int GameType = 1;

		[ProtoMember(5)]
		public int FuBenSeqId;

		[ProtoMember(6)]
		public long EndTicks;

		[ProtoMember(7)]
		public int ServerId;

		[ProtoMember(8)]
		public string ServerIp;

		[ProtoMember(9)]
		public int ServerPort;

		[ProtoMember(10)]
		public int TempRoleID;

		[ProtoMember(11)]
		public int PTID;

		[ProtoMember(12)]
		public string SignToken;

		[ProtoMember(13)]
		public int TargetServerID;

		[ProtoMember(14)]
		public string SignCode;

		[ProtoMember(15)]
		public int Private;

		[ProtoMember(16)]
		public int Line;

		[ProtoMember(17)]
		public int Param1;

		[ProtoMember(18)]
		public int Param2;
	}
}
