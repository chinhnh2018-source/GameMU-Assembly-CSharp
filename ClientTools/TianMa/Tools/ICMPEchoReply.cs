using System;

namespace TianMa.Tools
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
