using System;

namespace Server.Tools
{
	public class CRC32
	{
		private static uint[] x40d0ed9be6b42277()
		{
			uint[] array = new uint[256];
			int num;
			bool flag = (uint)num + (uint)num > uint.MaxValue;
			if (!flag)
			{
				goto IL_F8;
			}
			int num2;
			uint num3;
			for (;;)
			{
				IL_D1:
				for (;;)
				{
					if (num2 < 256)
					{
						num3 = (uint)num2;
						goto IL_A4;
					}
					if ((num3 | 4294967295U) != 0U)
					{
						goto Block_4;
					}
					goto IL_76;
					for (;;)
					{
						IL_83:
						if (--num >= 0)
						{
							while ((num3 & 1U) == 0U)
							{
								flag = ((uint)num2 < 0U);
								if (!flag && 4 != 0)
								{
									goto IL_7F;
								}
								if (!false)
								{
									goto IL_76;
								}
							}
							num3 = (3988292384U ^ num3 >> 1);
							goto IL_B2;
						}
						if (8 != 0)
						{
							array[num2] = num3;
							if (3 != 0)
							{
								break;
							}
							goto IL_B2;
						}
						continue;
						IL_B2:
						if ((uint)num - (uint)num > 4294967295U)
						{
							goto IL_A4;
						}
						if (8 == 0)
						{
							goto IL_D1;
						}
						if (-1 == 0)
						{
							goto IL_E1;
						}
					}
					num2++;
					break;
					IL_7F:
					num3 >>= 1;
					goto IL_83;
					IL_76:
					goto IL_7F;
					IL_A4:
					num = 8;
					goto IL_83;
				}
			}
			Block_4:
			IL_E1:
			flag = ((uint)num2 + num3 < 0U);
			if (!flag)
			{
				return array;
			}
			IL_F8:
			num2 = 0;
			flag = ((uint)num - (uint)num < 0U);
			if (!flag)
			{
				goto IL_D1;
			}
			return array;
		}

		public uint getValue()
		{
			return this.xa4660e7fe4e71d99 & uint.MaxValue;
		}

		public void reset()
		{
			this.xa4660e7fe4e71d99 = 0U;
		}

		public void update(byte[] buf)
		{
			uint num = 0U;
			uint num2;
			bool flag = (num2 | 255U) == 0U;
			int num3;
			if (!flag)
			{
				num3 = buf.Length;
				num2 = ~this.xa4660e7fe4e71d99;
			}
			for (;;)
			{
				if (--num3 < 0)
				{
					this.xa4660e7fe4e71d99 = ~num2;
					if ((num | 8U) != 0U)
					{
						break;
					}
				}
				num2 = (CRC32.xe6d5d55071bc950f[(int)((UIntPtr)((num2 ^ (uint)buf[(int)((UIntPtr)(num++))]) & 255U))] ^ num2 >> 8);
			}
		}

		public void update(byte[] buf, int off, int len)
		{
			uint num = ~this.xa4660e7fe4e71d99;
			while (--len >= 0)
			{
				num = (CRC32.xe6d5d55071bc950f[(int)((UIntPtr)((num ^ (uint)buf[off++]) & 255U))] ^ num >> 8);
			}
			this.xa4660e7fe4e71d99 = ~num;
		}

		private uint xa4660e7fe4e71d99;

		private static uint[] xe6d5d55071bc950f = CRC32.x40d0ed9be6b42277();
	}
}
