using System;

namespace TianMa.Tools
{
	public class ICMPIPHeaderReply : ICMPMessage
	{
		public ICMPIPHeaderReply()
		{
		}

		public ICMPIPHeaderReply(ref byte[] Packet)
		{
			try
			{
				this.Data = new byte[Packet.Length - 4];
				Buffer.BlockCopy(Packet, 4, this.Data, 0, this.Data.Length);
				this.IP = new IPPacket(ref this.Data);
			}
			catch
			{
			}
		}

		public override byte[] GetBytes()
		{
			if (this.Data == null)
			{
				this.Data = new byte[0];
			}
			byte[] array = new byte[4 + this.Data.Length];
			Buffer.BlockCopy(this.Data, 0, array, 4, this.Data.Length);
			return array;
		}

		public byte[] Data;

		public IPPacket IP;
	}
}
