using System;

namespace TianMa.Tools
{
	public class ICMPInformationReply : ICMPInformationRequest
	{
		public ICMPInformationReply()
		{
		}

		public ICMPInformationReply(ref byte[] Packet) : base(ref Packet)
		{
		}
	}
}
