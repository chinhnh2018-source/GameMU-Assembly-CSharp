using System;

namespace TianMa
{
	public class ICMPTimestampReply : ICMPTimestamp
	{
		public ICMPTimestampReply()
		{
		}

		public ICMPTimestampReply(ref byte[] Packet) : base(ref Packet)
		{
		}
	}
}
