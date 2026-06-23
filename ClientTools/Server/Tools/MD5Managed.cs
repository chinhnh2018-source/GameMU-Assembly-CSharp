using System;
using System.Security.Cryptography;

namespace Server.Tools
{
	public class MD5Managed : MD5
	{
		public MD5Managed()
		{
			this.HashSizeValue = 128;
			this.Initialize();
		}

		public override void Initialize()
		{
			this._x4a3f0a05c02f235f = new byte[64];
			if (-1 != 0)
			{
				goto IL_67;
			}
			IL_14:
			this._xbcc0924abad481fb = default(x1408d8967d2f1af2);
			this._xbcc0924abad481fb.xda71bf6f7c07c3bc = 1732584193U;
			this._xbcc0924abad481fb.x8e8f6cc6a0756b05 = 4023233417U;
			this._xbcc0924abad481fb.x857912840ffd015f = 2562383102U;
			IL_50:
			this._xbcc0924abad481fb.x5d593cee9d844848 = 271733878U;
			if (-2147483648 != 0)
			{
				return;
			}
			IL_67:
			this._xff884decc91dea16 = 0;
			if (!false)
			{
				this._x5808173d78349062 = 0L;
				goto IL_14;
			}
			goto IL_50;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int num = ibStart;
			int i;
			bool flag;
			for (;;)
			{
				IL_13D:
				i = this._xff884decc91dea16 + cbSize;
				if (i < 64)
				{
					break;
				}
				Array.Copy(array, num, this._x4a3f0a05c02f235f, this._xff884decc91dea16, 64 - this._xff884decc91dea16);
				MD5Core.x27a41f9a819cf224(this._x4a3f0a05c02f235f, ref this._xbcc0924abad481fb, 0);
				for (;;)
				{
					IL_F1:
					num += 64 - this._xff884decc91dea16;
					do
					{
						i -= 64;
						while (i >= 64)
						{
							Array.Copy(array, num, this._x4a3f0a05c02f235f, 0, 64);
							MD5Core.x27a41f9a819cf224(array, ref this._xbcc0924abad481fb, num);
							i -= 64;
							num += 64;
							flag = ((uint)cbSize + (uint)cbSize > uint.MaxValue);
							if (flag)
							{
								goto IL_F1;
							}
							if ((uint)i - (uint)num > 4294967295U)
							{
								goto IL_2C;
							}
						}
						if ((uint)ibStart - (uint)i < 0U)
						{
							goto IL_13D;
						}
						this._xff884decc91dea16 = i;
						if (!false)
						{
							goto IL_104;
						}
						flag = ((uint)i + (uint)cbSize > uint.MaxValue);
					}
					while (flag);
				}
			}
			IL_11:
			Array.Copy(array, num, this._x4a3f0a05c02f235f, this._xff884decc91dea16, cbSize);
			this._xff884decc91dea16 = i;
			IL_2C:
			this._x5808173d78349062 += (long)cbSize;
			return;
			IL_104:
			flag = (((uint)ibStart | 2147483648U) == 0U);
			if (flag)
			{
				goto IL_11;
			}
			Array.Copy(array, num, this._x4a3f0a05c02f235f, 0, i);
			goto IL_2C;
		}

		protected override byte[] HashFinal()
		{
			this.HashValue = MD5Core.x8259195df2f3ddaa(this._x4a3f0a05c02f235f, 0, this._xff884decc91dea16, this._xbcc0924abad481fb, this._x5808173d78349062 * 8L);
			return this.HashValue;
		}

		private byte[] _x4a3f0a05c02f235f;

		private x1408d8967d2f1af2 _xbcc0924abad481fb;

		private long _x5808173d78349062;

		private int _xff884decc91dea16;
	}
}
