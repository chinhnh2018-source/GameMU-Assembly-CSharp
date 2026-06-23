using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	[ProtoContract]
	public class GameSceneStateTimeData
	{
		[ProtoMember(1)]
		public int GameType;

		[ProtoMember(2)]
		public int State;

		[ProtoMember(3)]
		public long EndTicks;

		[ProtoMember(4)]
		public int Flags;

		[ProtoMember(5)]
		public byte[] ExtraData;
	}
}
