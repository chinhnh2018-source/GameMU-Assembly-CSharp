using System;
using System.Text;

namespace TianMa.Tools
{
	public class ICMPEcho : ICMPMessage
	{
		public ICMPEcho()
		{
		}

		public ICMPEcho(ref byte[] Packet)
		{
			try
			{
				this.Identifier = (ushort)BitConverter.ToInt16(Packet, 0);
				this.SequenceNumber = (ushort)BitConverter.ToInt16(Packet, 2);
				this.Data = Encoding.ASCII.GetString(Packet, 4, Packet.Length - 4);
			}
			catch
			{
			}
		}

		public override byte[] GetBytes()
		{
			if (this.Data == null)
			{
				this.Data = "";
			}
			byte[] array = new byte[4 + this.Data.Length];
			if (255 != 0)
			{
				Buffer.BlockCopy(BitConverter.GetBytes((short)this.Identifier), 0, array, 0, 2);
				Buffer.BlockCopy(BitConverter.GetBytes((short)this.SequenceNumber), 0, array, 2, 2);
				Buffer.BlockCopy(Encoding.ASCII.GetBytes(this.Data), 0, array, 4, this.Data.Length);
			}
			return array;
		}

		public ushort Identifier;

		public ushort SequenceNumber;

		public string Data;
	}
}
