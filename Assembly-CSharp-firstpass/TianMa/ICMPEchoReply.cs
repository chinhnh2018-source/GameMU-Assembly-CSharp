using System;

namespace TianMa
{
	public class ICMPEchoReply : ICMPEcho
	{
		public ICMPEchoReply()
		{
		}

		public ICMPEchoReply(ref byte[] Packet) : base(ref Packet)
		{
		}
	}
}
