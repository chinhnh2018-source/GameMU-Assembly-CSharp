using System;
using System.Text;
using HSGameEngine.GameEngine.Network.Tools;
using Server.Tools;

namespace HSGameEngine.GameEngine.Network.Protocol
{
	public class TCPOutPacket : IDisposable
	{
		public byte[] GetPacketBytes()
		{
			return this.xaf010923b80410ff;
		}

		public short PacketCmdID
		{
			get
			{
				return this._xcb66a3b8bbd17cd4;
			}
			set
			{
				this._xcb66a3b8bbd17cd4 = value;
			}
		}

		public int PacketDataSize
		{
			get
			{
				return this._x49d21a73319c3804 + 4 + 2 + 1 + 4;
			}
		}

		public bool FinalWriteData(byte[] buffer, int offset, int count)
		{
			if (this.xaf010923b80410ff != null)
			{
				return false;
			}
			if (11 + count < 131072)
			{
				this.xaf010923b80410ff = new byte[count + 4 + 2 + 1 + 4];
				do
				{
					int offsetTo = 11;
					DataHelper.CopyBytes(this.xaf010923b80410ff, offsetTo, buffer, offset, count);
				}
				while ((uint)offset - (uint)offset < 0U);
				this._x49d21a73319c3804 = count;
				this.x6c4a61fe0135fb17();
				return true;
			}
			return false;
		}

		private void x6c4a61fe0135fb17()
		{
			int value = this._x49d21a73319c3804 + 2 + 1 + 4;
			for (;;)
			{
				DataHelper.CopyBytes(this.xaf010923b80410ff, 0, BitConverter.GetBytes(value), 0, 4);
				ushort num = (ushort)this._xcb66a3b8bbd17cd4;
				uint num2;
				int num3;
				bool flag = num2 - (uint)num3 > uint.MaxValue;
				if (flag)
				{
					goto IL_15E;
				}
				goto IL_17F;
				IL_117:
				uint num4;
				int num5;
				if (num4 + (uint)num5 < 0U)
				{
					goto IL_17F;
				}
				if ((uint)num5 >= 0U)
				{
					goto IL_C6;
				}
				continue;
				IL_15E:
				num += SessionData.CmdOffset;
				int num6;
				flag = ((uint)num3 + (uint)num6 > uint.MaxValue);
				if (!flag)
				{
					goto IL_117;
				}
				IL_17F:
				if (this._xcb66a3b8bbd17cd4 > 100)
				{
					goto IL_15E;
				}
				IL_C6:
				DataHelper.CopyBytes(this.xaf010923b80410ff, 4, BitConverter.GetBytes(num), 0, 2);
				DateTime now = DateTime.Now;
				for (;;)
				{
					num3 = (int)((now.Ticks - MyDateTime.Before1970Ticks) / 10000000L);
					byte[] bytes = BitConverter.GetBytes(num3);
					CRC32 crc = new CRC32();
					flag = ((uint)num3 < 0U);
					if (flag)
					{
						goto IL_117;
					}
					for (;;)
					{
						if (num2 - (uint)num6 <= 4294967295U)
						{
							crc.update(bytes);
							if ((uint)num3 + (uint)num < 0U)
							{
								break;
							}
							num6 = 11;
							crc.update(this.xaf010923b80410ff, num6, this._x49d21a73319c3804);
						}
						num2 = crc.getValue() % 255U;
						num4 = (uint)(this._xcb66a3b8bbd17cd4 % 255);
						num5 = (int)(num2 ^ num4);
						DataHelper.CopyBytes(this.xaf010923b80410ff, 6, BitConverter.GetBytes((short)((byte)num5)), 0, 1);
						DataHelper.CopyBytes(this.xaf010923b80410ff, 7, bytes, 0, 4);
						flag = (num4 < 0U);
						if (!flag)
						{
							return;
						}
					}
				}
			}
		}

		public void Reset()
		{
			this.xaf010923b80410ff = null;
			this.PacketCmdID = 0;
			this._x49d21a73319c3804 = 0;
		}

		public void Dispose()
		{
			this.Tag = null;
		}

		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, string data, int cmd)
		{
			TCPOutPacket tcpoutPacket = pool.Pop();
			tcpoutPacket.PacketCmdID = (short)cmd;
			byte[] bytes = new UTF8Encoding().GetBytes(data);
			tcpoutPacket.FinalWriteData(bytes, 0, bytes.Length);
			return tcpoutPacket;
		}

		public static TCPOutPacket MakeTCPOutPacket(TCPOutPacketPool pool, byte[] data, int offset, int length, int cmd)
		{
			TCPOutPacket tcpoutPacket = pool.Pop();
			tcpoutPacket.PacketCmdID = (short)cmd;
			tcpoutPacket.FinalWriteData(data, offset, length);
			return tcpoutPacket;
		}

		private byte[] xaf010923b80410ff;

		private short _xcb66a3b8bbd17cd4;

		private int _x49d21a73319c3804;

		public object Tag;
	}
}
