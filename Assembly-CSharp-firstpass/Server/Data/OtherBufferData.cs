using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OtherBufferData
	{
		public BufferData ToBufferData()
		{
			return new BufferData
			{
				BufferID = this.BufferID,
				BufferSecs = this.BufferSecs,
				BufferType = this.BufferType,
				BufferVal = this.BufferVal,
				StartTime = this.StartTime
			};
		}

		[ProtoMember(1)]
		public int BufferID;

		[ProtoMember(2)]
		public long StartTime;

		[ProtoMember(3)]
		public int BufferSecs;

		[ProtoMember(4)]
		public long BufferVal;

		[ProtoMember(5)]
		public int BufferType;

		[ProtoMember(6)]
		public int RoleID;
	}
}
