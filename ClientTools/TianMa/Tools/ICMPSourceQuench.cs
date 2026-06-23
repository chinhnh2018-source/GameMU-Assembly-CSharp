using System;

namespace TianMa.Tools
{
	public class ICMPSourceQuench : ICMPIPHeaderReply
	{
		public ICMPSourceQuench()
		{
		}

		public ICMPSourceQuench(ref byte[] Packet) : base(ref Packet)
		{
		}
	}
}
