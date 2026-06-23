using System;
using System.Net.Sockets;
using System.Threading;
using Server.Tools;

namespace HSGameEngine.GameEngine.Network.Protocol
{
	public class TCPInPacket : IDisposable
	{
		public TCPInPacket(int recvBufferSize = 131072)
		{
			this.xaf010923b80410ff = new byte[recvBufferSize];
		}

		public byte[] GetPacketBytes()
		{
			return this.xaf010923b80410ff;
		}

		public Socket CurrentSocket
		{
			get
			{
				return this._x70087761226fbdf2;
			}
			set
			{
				this._x70087761226fbdf2 = value;
			}
		}

		public int PacketCmdID
		{
			get
			{
				int result = 0;
				lock (this)
				{
					result = this._xcb66a3b8bbd17cd4;
				}
				return result;
			}
		}

		public int PacketDataSize
		{
			get
			{
				int result = 0;
				lock (this)
				{
					result = this._x49d21a73319c3804;
				}
				return result;
			}
		}

		public void Dispose()
		{
			this.Reset();
		}

		public void Destroy()
		{
			lock (this)
			{
				this.xd0ab16336c9c9beb = null;
			}
		}

		public event TCPCmdPacketEventHandler TCPCmdPacketEvent
		{
			add
			{
				TCPCmdPacketEventHandler tcpcmdPacketEventHandler = this.xd0ab16336c9c9beb;
				TCPCmdPacketEventHandler tcpcmdPacketEventHandler2;
				do
				{
					tcpcmdPacketEventHandler2 = tcpcmdPacketEventHandler;
					TCPCmdPacketEventHandler value2 = (TCPCmdPacketEventHandler)Delegate.Combine(tcpcmdPacketEventHandler2, value);
					tcpcmdPacketEventHandler = Interlocked.CompareExchange<TCPCmdPacketEventHandler>(ref this.xd0ab16336c9c9beb, value2, tcpcmdPacketEventHandler2);
				}
				while (tcpcmdPacketEventHandler != tcpcmdPacketEventHandler2);
			}
			remove
			{
				TCPCmdPacketEventHandler tcpcmdPacketEventHandler = this.xd0ab16336c9c9beb;
				TCPCmdPacketEventHandler tcpcmdPacketEventHandler2;
				do
				{
					tcpcmdPacketEventHandler2 = tcpcmdPacketEventHandler;
					TCPCmdPacketEventHandler value2 = (TCPCmdPacketEventHandler)Delegate.Remove(tcpcmdPacketEventHandler2, value);
					tcpcmdPacketEventHandler = Interlocked.CompareExchange<TCPCmdPacketEventHandler>(ref this.xd0ab16336c9c9beb, value2, tcpcmdPacketEventHandler2);
				}
				while (tcpcmdPacketEventHandler != tcpcmdPacketEventHandler2);
			}
		}

		public bool WriteData(byte[] buffer, int offset, int count)
		{
			bool flag2;
			lock (this)
			{
				if (this.x4874cbe59da9154b)
				{
					goto IL_48C;
				}
				if (count <= 6 - this.xba8d7776976beec6)
				{
					goto IL_1A9;
				}
				IL_121:
				int num = 6 - this.xba8d7776976beec6;
				IL_136:
				int num2 = num;
				uint num3;
				bool flag = (num3 & 0U) == 0U;
				if (!flag)
				{
					goto IL_2B4;
				}
				DataHelper.CopyBytes(this.xcd91477ec461fa31, this.xba8d7776976beec6, buffer, offset, num2);
				this.xba8d7776976beec6 += num2;
				if (this.xba8d7776976beec6 < 6)
				{
					flag = (num3 + (uint)num2 < 0U);
					if (!flag)
					{
						return true;
					}
				}
				else
				{
					this._x49d21a73319c3804 = BitConverter.ToInt32(this.xcd91477ec461fa31, 0);
				}
				this._xcb66a3b8bbd17cd4 = (int)BitConverter.ToUInt16(this.xcd91477ec461fa31, 4);
				byte b;
				flag = (((uint)b | 2147483648U) == 0U);
				if (!flag)
				{
					if (this._x49d21a73319c3804 <= 0)
					{
						goto IL_AC;
					}
				}
				int num4;
				ushort num5;
				if (this._x49d21a73319c3804 < 131072)
				{
					do
					{
						offset += num2;
						if (((uint)num4 | 2147483647U) == 0U)
						{
							goto IL_2E4;
						}
						count -= num2;
						this.x4874cbe59da9154b = true;
						this.x38fe45c74e090096 = 0;
						this._x49d21a73319c3804 -= 2;
						flag2 = this.WriteData(buffer, offset, count);
						flag = ((flag2 ? 1U : 0U) < 0U);
					}
					while (flag);
					flag = ((uint)num5 < 0U);
					if (flag)
					{
						goto IL_121;
					}
					goto IL_220;
				}
				IL_AC:
				flag2 = false;
				IL_19F:
				return flag2;
				IL_1A9:
				num = count;
				goto IL_136;
				IL_1D9:
				flag2 = true;
				if (((uint)count | 2147483648U) == 0U)
				{
					goto IL_486;
				}
				if (((uint)num4 & 0U) != 0U)
				{
					flag = ((uint)count > uint.MaxValue);
					if (!flag)
					{
						goto IL_121;
					}
				}
				IL_220:
				return flag2;
				IL_234:
				if (this.x38fe45c74e090096 < this._x49d21a73319c3804)
				{
					goto IL_1D9;
				}
				bool flag3 = true;
				if (this._xcb66a3b8bbd17cd4 > 100)
				{
					if ((uint)num2 - (uint)count > 4294967295U)
					{
						goto IL_3CE;
					}
					this._xcb66a3b8bbd17cd4 -= (int)SessionData.CmdOffset;
				}
				if (this._xcb66a3b8bbd17cd4 == (int)SessionData.OffsetChgCmdId)
				{
					num3 = BitConverter.ToUInt32(this.xaf010923b80410ff, 0);
					goto IL_3CE;
				}
				if (2147483647 == 0)
				{
					goto IL_3BF;
				}
				goto IL_32F;
				IL_2B4:
				if (false)
				{
					goto IL_19F;
				}
				count -= num4;
				if ((uint)count >= 0U)
				{
					return this.WriteData(buffer, offset, count);
				}
				goto IL_2EB;
				IL_2E4:
				this.xba8d7776976beec6 = 0;
				IL_2EB:
				if (!flag3)
				{
					return false;
				}
				if (count > num4)
				{
					offset += num4;
					if ((uint)count + (uint)num2 >= 0U)
					{
						goto IL_2B4;
					}
				}
				while ((uint)num4 < 0U)
				{
					flag = (((uint)num4 & 0U) == 0U);
					if (!flag)
					{
						goto IL_234;
					}
					flag = ((uint)num5 - (flag3 ? 1U : 0U) > uint.MaxValue);
					if (!flag)
					{
						break;
					}
				}
				goto IL_1D9;
				IL_31F:
				this._xcb66a3b8bbd17cd4 = 0;
				this._x49d21a73319c3804 = 0;
				this.x38fe45c74e090096 = 0;
				this.x4874cbe59da9154b = false;
				goto IL_2E4;
				IL_32F:
				if (this._xcb66a3b8bbd17cd4 != (int)SessionData.GenerateKeyCmdId)
				{
					if ((uint)b + (uint)count > 4294967295U)
					{
						goto IL_1A9;
					}
					if (this.xd0ab16336c9c9beb != null)
					{
						flag3 = this.xd0ab16336c9c9beb(this);
						goto IL_31F;
					}
					goto IL_31F;
				}
				IL_3BF:
				num5 = BitConverter.ToUInt16(this.xaf010923b80410ff, 0);
				b = DataHelper.GenerateEncpyptKey(num5);
				if (((uint)offset & 0U) == 0U)
				{
					if (((uint)b & 0U) != 0U)
					{
						goto IL_48C;
					}
					DataHelper.SetUseKey(b);
					goto IL_31F;
				}
				IL_3CE:
				SessionData.CmdOffset = DataHelper.GenerateOffsetKey((ushort)(num3 >> 16), (ushort)(num3 & 65535U));
				goto IL_31F;
				IL_486:
				if (num4 <= 0)
				{
					goto IL_234;
				}
				DataHelper.CopyBytes(this.xaf010923b80410ff, this.x38fe45c74e090096, buffer, offset, num4);
				this.x38fe45c74e090096 += num4;
				goto IL_234;
				IL_48C:
				num4 = ((count < this._x49d21a73319c3804 - this.x38fe45c74e090096) ? count : (this._x49d21a73319c3804 - this.x38fe45c74e090096));
				if ((uint)b - (uint)num5 >= 0U)
				{
					goto IL_486;
				}
				goto IL_32F;
			}
			return flag2;
		}

		public void Reset()
		{
			lock (this)
			{
				this._x70087761226fbdf2 = null;
				this._xcb66a3b8bbd17cd4 = 0;
				if (3 != 0)
				{
					this._x49d21a73319c3804 = 0;
					this.x38fe45c74e090096 = 0;
					this.x4874cbe59da9154b = false;
					this.xba8d7776976beec6 = 0;
				}
			}
		}

		private byte[] xaf010923b80410ff;

		private Socket _x70087761226fbdf2;

		private int _xcb66a3b8bbd17cd4;

		private int _x49d21a73319c3804;

		private int x38fe45c74e090096;

		private bool x4874cbe59da9154b;

		private TCPCmdPacketEventHandler xd0ab16336c9c9beb;

		private byte[] xcd91477ec461fa31 = new byte[6];

		private int xba8d7776976beec6;
	}
}
