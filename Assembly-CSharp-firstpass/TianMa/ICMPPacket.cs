using System;

namespace TianMa
{
	public class ICMPPacket
	{
		public ICMPPacket()
		{
		}

		public ICMPPacket(ref byte[] Packet)
		{
			try
			{
				this.Type = Packet[0];
				this.Code = Packet[1];
				this.Checksum = (ushort)BitConverter.ToInt16(Packet, 2);
				this.PacketData = new byte[Packet.Length - 4];
				Buffer.BlockCopy(Packet, 4, this.PacketData, 0, Packet.Length - 4);
			}
			catch
			{
			}
			switch (this.Type)
			{
			case 0:
				this.Message = new ICMPEchoReply(ref this.PacketData);
				break;
			case 3:
				this.Message = new ICMPDestinationUnreachable(ref this.PacketData);
				break;
			case 4:
				this.Message = new ICMPSourceQuench(ref this.PacketData);
				break;
			case 5:
				this.Message = new ICMPRedirect(ref this.PacketData);
				break;
			case 8:
				this.Message = new ICMPEcho(ref this.PacketData);
				break;
			case 11:
				this.Message = new ICMPTimeExceeded(ref this.PacketData);
				break;
			case 12:
				this.Message = new ICMPParameterProblem(ref this.PacketData);
				break;
			case 13:
				this.Message = new ICMPTimestamp(ref this.PacketData);
				break;
			case 14:
				this.Message = new ICMPTimestampReply(ref this.PacketData);
				break;
			case 15:
				this.Message = new ICMPInformationRequest(ref this.PacketData);
				break;
			case 16:
				this.Message = new ICMPInformationReply(ref this.PacketData);
				break;
			}
		}

		public byte[] GetBytes()
		{
			if (this.Message != null)
			{
				this.PacketData = this.Message.GetBytes();
			}
			if (this.Message is ICMPEchoReply)
			{
				this.Type = 0;
			}
			else if (this.Message is ICMPDestinationUnreachable)
			{
				this.Type = 3;
			}
			else if (this.Message is ICMPSourceQuench)
			{
				this.Type = 4;
			}
			else if (this.Message is ICMPRedirect)
			{
				this.Type = 5;
			}
			else if (this.Message is ICMPEcho)
			{
				this.Type = 8;
			}
			else if (this.Message is ICMPTimeExceeded)
			{
				this.Type = 11;
			}
			else if (this.Message is ICMPParameterProblem)
			{
				this.Type = 12;
			}
			else if (this.Message is ICMPTimestamp)
			{
				this.Type = 13;
			}
			else if (this.Message is ICMPTimestampReply)
			{
				this.Type = 14;
			}
			else if (this.Message is ICMPInformationRequest)
			{
				this.Type = 15;
			}
			else if (this.Message is ICMPInformationReply)
			{
				this.Type = 16;
			}
			if (this.PacketData == null)
			{
				this.PacketData = new byte[0];
			}
			byte[] array = new byte[4 + this.PacketData.Length];
			array[0] = this.Type;
			array[1] = this.Code;
			Buffer.BlockCopy(BitConverter.GetBytes(0), 0, array, 2, 2);
			Buffer.BlockCopy(this.PacketData, 0, array, 4, this.PacketData.Length);
			this.Checksum = this.GetChecksum(ref array, 0, array.Length - 1);
			Buffer.BlockCopy(BitConverter.GetBytes((short)this.Checksum), 0, array, 2, 2);
			return array;
		}

		public ushort GetChecksum(ref byte[] Packet, int start, int end)
		{
			uint num = 0U;
			int i;
			for (i = start; i < end; i += 2)
			{
				num += (uint)((ushort)BitConverter.ToInt16(Packet, i));
			}
			if (i == end)
			{
				num += (uint)Packet[end];
			}
			while (num >> 16 != 0U)
			{
				num = (num & 65535U) + (num >> 16);
			}
			return (ushort)(~(ushort)num);
		}

		public byte Type;

		public byte Code;

		public ushort Checksum;

		public byte[] PacketData;

		public ICMPMessage Message;
	}
}
