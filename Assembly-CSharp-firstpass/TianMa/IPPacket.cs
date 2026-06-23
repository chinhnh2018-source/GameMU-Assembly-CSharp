using System;
using System.Net;

namespace TianMa
{
	public class IPPacket
	{
		public IPPacket()
		{
		}

		public IPPacket(ref byte[] Packet)
		{
			try
			{
				this.Version = (byte)(Packet[0] >> 4);
				this.HeaderLength = (Packet[0] & 15) * 4;
				this.TypeOfService = Packet[1];
				this.TotalLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 2));
				this.Identification = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 4));
				this.Flags = (byte)((Packet[6] & 224) >> 5);
				this.FragmentOffset = (ushort)(IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 6)) & 8191);
				this.TimeToLive = Packet[8];
				this.Protocol = Packet[9];
				this.HeaderChecksum = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 10));
				this.SourceAddress = new IPAddress((long)BitConverter.ToInt32(Packet, 12) & (long)((ulong)-1));
				this.DestinationAddress = new IPAddress((long)BitConverter.ToInt32(Packet, 16) & (long)((ulong)-1));
				this.PacketData = new byte[(int)(this.TotalLength - (ushort)this.HeaderLength)];
				Buffer.BlockCopy(Packet, (int)this.HeaderLength, this.PacketData, 0, this.PacketData.Length);
			}
			catch
			{
			}
			byte protocol = this.Protocol;
			if (protocol == 1)
			{
				this.ICMP = new ICMPPacket(ref this.PacketData);
			}
		}

		public byte[] GetBytes()
		{
			if (this.ICMP != null)
			{
				this.Protocol = 1;
				this.PacketData = this.ICMP.GetBytes();
			}
			if (this.PacketData == null)
			{
				this.PacketData = new byte[0];
			}
			if (this.Version == 0)
			{
				this.Version = 4;
			}
			if (this.HeaderLength == 0)
			{
				this.HeaderLength = 20;
			}
			this.TotalLength = (ushort)((int)this.HeaderLength + this.PacketData.Length);
			byte[] array = new byte[(int)this.TotalLength];
			if (this.TimeToLive == 0)
			{
				this.TimeToLive = 128;
			}
			array[0] = (byte)((int)(this.Version & 15) << 4 | (int)(this.HeaderLength / 4 & 15));
			array[1] = this.TypeOfService;
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.TotalLength)), 0, array, 2, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.Identification)), 0, array, 4, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)((int)(this.FragmentOffset & 31) | (int)(this.Flags & 3) << 13))), 0, array, 6, 2);
			array[8] = this.TimeToLive;
			array[9] = this.Protocol;
			Buffer.BlockCopy(BitConverter.GetBytes(0), 0, array, 10, 2);
			Buffer.BlockCopy(this.SourceAddress.GetAddressBytes(), 0, array, 12, 4);
			Buffer.BlockCopy(this.DestinationAddress.GetAddressBytes(), 0, array, 16, 4);
			Buffer.BlockCopy(this.PacketData, 0, array, (int)this.HeaderLength, this.PacketData.Length);
			this.HeaderChecksum = IPPacket.GetChecksum(ref array, 0, (int)(this.HeaderLength - 1));
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.HeaderChecksum)), 0, array, 10, 2);
			return array;
		}

		public static ushort GetChecksum(ref byte[] Packet, int start, int end)
		{
			uint num = 0U;
			int i;
			for (i = start; i < end; i += 2)
			{
				num += (uint)((ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, i)));
			}
			if (i == end)
			{
				num += (uint)((ushort)IPAddress.NetworkToHostOrder((int)Packet[end]));
			}
			while (num >> 16 != 0U)
			{
				num = (num & 65535U) + (num >> 16);
			}
			return (ushort)(~(ushort)num);
		}

		public byte Version;

		public byte HeaderLength;

		public byte TypeOfService;

		public ushort TotalLength;

		public ushort Identification;

		public byte Flags;

		public ushort FragmentOffset;

		public byte TimeToLive;

		public byte Protocol;

		public ushort HeaderChecksum;

		public IPAddress SourceAddress;

		public IPAddress DestinationAddress;

		public byte[] PacketData;

		public ICMPPacket ICMP;
	}
}
