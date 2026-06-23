using System;

namespace TianMa.Tools
{
	public class ICMPTimeExceeded : ICMPIPHeaderReply
	{
		public ICMPTimeExceeded()
		{
		}

		public ICMPTimeExceeded(ref byte[] Packet) : base(ref Packet)
		{
		}

		public enum CodeEnum
		{
			TimeToLiveExceededInTransit,
			FragmentReassemblyTimeExceeded
		}
	}
}
