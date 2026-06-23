using System;

namespace TianMa.Tools
{
	public class ICMPInformationRequest : ICMPMessage
	{
		public ICMPInformationRequest()
		{
		}

		public ICMPInformationRequest(ref byte[] Packet)
		{
			try
			{
				this.Identifier = (ushort)BitConverter.ToInt16(Packet, 0);
				this.SequenceNumber = (ushort)BitConverter.ToInt16(Packet, 2);
			}
			catch
			{
			}
		}

		public override byte[] GetBytes()
		{
			byte[] array = new byte[4];
			Buffer.BlockCopy(BitConverter.GetBytes((short)this.Identifier), 0, array, 0, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)this.SequenceNumber), 0, array, 2, 2);
			return array;
		}

		public ushort Identifier;

		public ushort SequenceNumber;
	}
}
