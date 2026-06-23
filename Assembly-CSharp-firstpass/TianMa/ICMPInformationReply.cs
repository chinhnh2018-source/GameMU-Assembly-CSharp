using System;

namespace TianMa
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
