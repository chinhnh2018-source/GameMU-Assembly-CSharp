using System;

namespace TianMa.Tools
{
	public class ICMPTimestamp : ICMPMessage
	{
		public ICMPTimestamp()
		{
		}

		public ICMPTimestamp(ref byte[] Packet)
		{
			try
			{
				this.Identifier = (ushort)BitConverter.ToInt16(Packet, 0);
				this.SequenceNumber = (ushort)BitConverter.ToInt16(Packet, 2);
				this.OriginateTimestamp = (ulong)((long)BitConverter.ToInt32(Packet, 4));
				this.ReceiveTimestamp = (ulong)((long)BitConverter.ToInt32(Packet, 8));
				this.TransmitTimestamp = (ulong)((long)BitConverter.ToInt32(Packet, 12));
			}
			catch
			{
			}
		}

		public override byte[] GetBytes()
		{
			byte[] array = new byte[16];
			if (8 != 0)
			{
				Buffer.BlockCopy(BitConverter.GetBytes((short)this.Identifier), 0, array, 0, 2);
				Buffer.BlockCopy(BitConverter.GetBytes((short)this.SequenceNumber), 0, array, 2, 2);
			}
			Buffer.BlockCopy(BitConverter.GetBytes((long)this.OriginateTimestamp), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((long)this.ReceiveTimestamp), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((long)this.TransmitTimestamp), 0, array, 12, 4);
			return array;
		}

		public ushort Identifier;

		public ushort SequenceNumber;

		public ulong OriginateTimestamp;

		public ulong ReceiveTimestamp;

		public ulong TransmitTimestamp;
	}
}
