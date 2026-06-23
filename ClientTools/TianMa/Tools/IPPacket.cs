using System;
using System.Net;

namespace TianMa.Tools
{
	public class IPPacket
	{
		public IPPacket()
		{
		}

		public IPPacket(ref byte[] Packet)
		{
			byte protocol;
			try
			{
				this.Version = (byte)(Packet[0] >> 4);
				this.HeaderLength = (Packet[0] & 15) * 4;
				this.TypeOfService = Packet[1];
				this.TotalLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 2));
				bool flag = (uint)protocol - (uint)protocol < 0U;
				if (flag)
				{
					goto IL_13B;
				}
				this.Identification = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 4));
				if (!false)
				{
					goto IL_C8;
				}
				IL_18:
				this.HeaderChecksum = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 10));
				this.SourceAddress = new IPAddress((long)BitConverter.ToInt32(Packet, 12) & (long)((ulong)-1));
				if (((uint)protocol | 4294967295U) != 0U)
				{
					this.DestinationAddress = new IPAddress((long)BitConverter.ToInt32(Packet, 16) & (long)((ulong)-1));
					this.PacketData = new byte[(int)(this.TotalLength - (ushort)this.HeaderLength)];
					Buffer.BlockCopy(Packet, (int)this.HeaderLength, this.PacketData, 0, this.PacketData.Length);
					goto IL_174;
				}
				IL_C8:
				this.Flags = (byte)((Packet[6] & 224) >> 5);
				this.FragmentOffset = (ushort)(IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, 6)) & 8191);
				this.TimeToLive = Packet[8];
				this.Protocol = Packet[9];
				IL_13B:
				flag = (((uint)protocol | 3U) == 0U);
				if (!flag)
				{
					goto IL_18;
				}
				IL_174:;
			}
			catch
			{
			}
			protocol = this.Protocol;
			if (protocol != 1)
			{
				return;
			}
			this.ICMP = new ICMPPacket(ref this.PacketData);
		}

		public byte[] GetBytes()
		{
			if (this.ICMP != null)
			{
				goto IL_20A;
			}
			goto IL_1CC;
			byte[] array;
			for (;;)
			{
				IL_1D4:
				if (this.Version != 0)
				{
					goto IL_187;
				}
				this.Version = 4;
				IL_1B4:
				if (!true)
				{
					continue;
				}
				IL_187:
				if (this.HeaderLength == 0)
				{
					this.HeaderLength = 20;
				}
				this.TotalLength = (ushort)((int)this.HeaderLength + this.PacketData.Length);
				array = new byte[(int)this.TotalLength];
				if (false)
				{
					goto IL_1B4;
				}
				break;
			}
			if (false)
			{
				goto IL_1CA;
			}
			if (4 != 0)
			{
				if (4 == 0)
				{
					goto IL_20A;
				}
				if (!false)
				{
					if (this.TimeToLive == 0)
					{
						this.TimeToLive = 128;
					}
				}
			}
			for (;;)
			{
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
				for (;;)
				{
					Buffer.BlockCopy(this.PacketData, 0, array, (int)this.HeaderLength, this.PacketData.Length);
					if (-2147483648 == 0)
					{
						break;
					}
					if (!false)
					{
						goto IL_13;
					}
				}
			}
			do
			{
				IL_1E7:
				this.PacketData = new byte[0];
				if (false)
				{
					goto IL_13;
				}
			}
			while (false);
			goto IL_1CA;
			IL_13:
			this.HeaderChecksum = IPPacket.GetChecksum(ref array, 0, (int)(this.HeaderLength - 1));
			Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)this.HeaderChecksum)), 0, array, 10, 2);
			if (-1 == 0)
			{
				goto IL_20A;
			}
			return array;
			IL_1CA:
			goto IL_1D4;
			IL_1CC:
			if (this.PacketData != null)
			{
				goto IL_1D4;
			}
			goto IL_1E7;
			IL_20A:
			this.Protocol = 1;
			this.PacketData = this.ICMP.GetBytes();
			if (2 == 0)
			{
				goto IL_1E7;
			}
			goto IL_1CC;
		}

		public static ushort GetChecksum(ref byte[] Packet, int start, int end)
		{
			uint num = 0U;
			bool flag = (uint)start < 0U;
			int num2;
			if (flag)
			{
				if (255 == 0)
				{
				}
			}
			else
			{
				num2 = start;
			}
			for (;;)
			{
				IL_57:
				if (num2 >= end)
				{
					if (num2 == end)
					{
						num += (uint)((ushort)IPAddress.NetworkToHostOrder((int)Packet[end]));
					}
					while (num >> 16 != 0U || false)
					{
						num = (num & 65535U) + (num >> 16);
					}
					flag = ((uint)end > uint.MaxValue);
					if (!flag)
					{
						break;
					}
				}
				else
				{
					num += (uint)((ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Packet, num2)));
					num2 += 2;
				}
			}
			return (ushort)(~(ushort)num);
			goto IL_57;
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
