using System;

namespace TianMa
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
