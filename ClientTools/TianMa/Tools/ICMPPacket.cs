using System;

namespace TianMa.Tools
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
				goto IL_191;
			}
			catch
			{
				goto IL_191;
			}
			IL_5F:
			this.Message = new ICMPInformationReply(ref this.PacketData);
			byte type;
			if (2 == 0)
			{
				bool flag = (uint)type - (uint)type > uint.MaxValue;
				if (!flag)
				{
					return;
				}
			}
			else
			{
				bool flag = (uint)type - (uint)type < 0U;
				if (flag)
				{
					goto IL_191;
				}
				return;
			}
			return;
			IL_191:
			type = this.Type;
			if ((uint)type + (uint)type >= 0U && (uint)type - (uint)type <= 4294967295U)
			{
				switch (type)
				{
				case 0:
					this.Message = new ICMPEchoReply(ref this.PacketData);
					return;
				case 1:
				case 2:
				case 6:
				case 7:
				case 9:
				case 10:
					return;
				case 3:
					this.Message = new ICMPDestinationUnreachable(ref this.PacketData);
					return;
				case 4:
					this.Message = new ICMPSourceQuench(ref this.PacketData);
					return;
				case 5:
					goto IL_F6;
				case 8:
					this.Message = new ICMPEcho(ref this.PacketData);
					if (3 != 0)
					{
						return;
					}
					if (255 == 0)
					{
						goto IL_145;
					}
					if (false)
					{
						goto IL_A3;
					}
					break;
				case 11:
					this.Message = new ICMPTimeExceeded(ref this.PacketData);
					return;
				case 12:
					break;
				case 13:
					this.Message = new ICMPTimestamp(ref this.PacketData);
					return;
				case 14:
					goto IL_A3;
				case 15:
					this.Message = new ICMPInformationRequest(ref this.PacketData);
					return;
				case 16:
					goto IL_5F;
				default:
					return;
				}
				this.Message = new ICMPParameterProblem(ref this.PacketData);
				IL_145:
				return;
			}
			goto IL_F6;
			IL_A3:
			this.Message = new ICMPTimestampReply(ref this.PacketData);
			return;
			IL_F6:
			this.Message = new ICMPRedirect(ref this.PacketData);
			if ((uint)type - (uint)type >= 0U)
			{
				return;
			}
			goto IL_A3;
		}

		public byte[] GetBytes()
		{
			if (this.Message != null)
			{
				goto IL_261;
			}
			IL_24F:
			if (!(this.Message is ICMPEchoReply))
			{
				if (false)
				{
					goto IL_272;
				}
				if (!(this.Message is ICMPDestinationUnreachable))
				{
					while (!false)
					{
						if (this.Message is ICMPSourceQuench)
						{
							this.Type = 4;
							if (2 == 0)
							{
							}
						}
						else if (!(this.Message is ICMPRedirect))
						{
							if (!(this.Message is ICMPEcho))
							{
								if (-2 == 0)
								{
									continue;
								}
								if (!false)
								{
									if (!(this.Message is ICMPTimeExceeded))
									{
										if (!false)
										{
											while (!(this.Message is ICMPParameterProblem))
											{
												if (false)
												{
													goto IL_1FC;
												}
												if (8 != 0)
												{
													if (false)
													{
														goto IL_261;
													}
													if (8 != 0)
													{
														goto IL_128;
													}
												}
												else
												{
													if (!false)
													{
														goto IL_128;
													}
													continue;
												}
												IL_152:
												this.Type = 13;
												goto IL_1A2;
												IL_128:
												if (!(this.Message is ICMPTimestamp))
												{
													goto IL_105;
												}
												goto IL_152;
											}
											this.Type = 12;
											goto IL_62;
										}
										IL_1A2:
										if (8 == 0)
										{
											continue;
										}
									}
									else
									{
										this.Type = 11;
										if (255 == 0)
										{
											continue;
										}
									}
								}
							}
							else
							{
								this.Type = 8;
							}
						}
						else
						{
							this.Type = 5;
						}
						IL_1FC:
						goto IL_62;
					}
					IL_105:
					if (2 != 0 && !(this.Message is ICMPTimestampReply))
					{
						if (!(this.Message is ICMPInformationRequest))
						{
							goto IL_C1;
						}
						this.Type = 15;
					}
					else
					{
						this.Type = 14;
					}
				}
				else
				{
					this.Type = 3;
				}
			}
			else
			{
				this.Type = 0;
			}
			IL_62:
			if (this.PacketData == null)
			{
				goto IL_99;
			}
			if (false)
			{
				goto IL_C1;
			}
			IL_6D:
			byte[] array = new byte[4 + this.PacketData.Length];
			array[0] = this.Type;
			array[1] = this.Code;
			Buffer.BlockCopy(BitConverter.GetBytes(0), 0, array, 2, 2);
			Buffer.BlockCopy(this.PacketData, 0, array, 4, this.PacketData.Length);
			this.Checksum = this.GetChecksum(ref array, 0, array.Length - 1);
			Buffer.BlockCopy(BitConverter.GetBytes((short)this.Checksum), 0, array, 2, 2);
			goto IL_11C;
			IL_99:
			this.PacketData = new byte[0];
			if (false)
			{
				goto IL_11C;
			}
			goto IL_6D;
			IL_C1:
			if (this.Message is ICMPInformationReply)
			{
				this.Type = 16;
				if (2147483647 == 0)
				{
					if (-2147483648 != 0)
					{
						goto IL_99;
					}
				}
			}
			goto IL_62;
			IL_11C:
			if (true)
			{
				return array;
			}
			goto IL_62;
			IL_261:
			this.PacketData = this.Message.GetBytes();
			IL_272:
			goto IL_24F;
		}

		public ushort GetChecksum(ref byte[] Packet, int start, int end)
		{
			uint num = 0U;
			do
			{
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
			}
			while (4 == 0);
			return (ushort)(~(ushort)num);
		}

		public byte Type;

		public byte Code;

		public ushort Checksum;

		public byte[] PacketData;

		public ICMPMessage Message;
	}
}
