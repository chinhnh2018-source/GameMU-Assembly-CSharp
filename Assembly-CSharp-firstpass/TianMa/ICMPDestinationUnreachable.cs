using System;

namespace TianMa
{
	public class ICMPDestinationUnreachable : ICMPIPHeaderReply
	{
		public ICMPDestinationUnreachable()
		{
		}

		public ICMPDestinationUnreachable(ref byte[] Packet) : base(ref Packet)
		{
		}

		public enum CodeEnum
		{
			NetUnreachable,
			HostUnreachable,
			ProtocolUnreachable,
			PortUnreachable,
			FragmentationNeededAndDFSet,
			SourceRouteFailed
		}
	}
}
