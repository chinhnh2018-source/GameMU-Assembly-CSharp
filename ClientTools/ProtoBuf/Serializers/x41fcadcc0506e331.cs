using System;

namespace ProtoBuf.Serializers
{
	internal class x41fcadcc0506e331
	{
		private static uint[] x40d0ed9be6b42277()
		{
			uint[] array = new uint[256];
			int num = 0;
			for (;;)
			{
				uint num2;
				if (num < 256)
				{
					num2 = (uint)num;
					goto IL_B6;
				}
				int num3;
				if ((uint)num3 - (uint)num > 4294967295U)
				{
					goto IL_B6;
				}
				bool flag = num2 - (uint)num < 0U;
				if (flag)
				{
					goto IL_42;
				}
				if (2147483647 == 0)
				{
					goto IL_91;
				}
				break;
				IL_5A:
				if (--num3 >= 0)
				{
					goto IL_91;
				}
				array[num] = num2;
				num++;
				flag = ((num2 & 0U) == 0U);
				if (flag)
				{
					continue;
				}
				goto IL_A0;
				IL_42:
				if ((uint)num3 + (uint)num3 > 4294967295U)
				{
					goto IL_96;
				}
				IL_A0:
				goto IL_5A;
				IL_B6:
				num3 = 8;
				goto IL_A0;
				IL_91:
				if ((num2 & 1U) != 0U)
				{
					num2 = (3988292384U ^ num2 >> 1);
					goto IL_5A;
				}
				IL_96:
				num2 >>= 1;
				goto IL_42;
			}
			return array;
		}

		public uint xe60c75844bdac817()
		{
			return this.xa4660e7fe4e71d99 & uint.MaxValue;
		}

		public void x765375830e9fa890()
		{
			this.xa4660e7fe4e71d99 = 0U;
		}

		public void xa12b4f15548ce49a(byte[] x1ef26dbdd5d13d24)
		{
			uint num = 0U;
			bool flag;
			do
			{
				int num2;
				if ((num | 1U) != 0U)
				{
					num2 = x1ef26dbdd5d13d24.Length;
				}
				uint num3 = ~this.xa4660e7fe4e71d99;
				while (--num2 >= 0)
				{
					num3 = (x41fcadcc0506e331.xe6d5d55071bc950f[(int)((UIntPtr)((num3 ^ (uint)x1ef26dbdd5d13d24[(int)((UIntPtr)(num++))]) & 255U))] ^ num3 >> 8);
				}
				this.xa4660e7fe4e71d99 = ~num3;
				flag = (num - num > uint.MaxValue);
			}
			while (flag);
		}

		public void xa12b4f15548ce49a(byte[] x1ef26dbdd5d13d24, int xf2ebf5458da93d3b, int xb5964a891b6cf7c3)
		{
			uint num = ~this.xa4660e7fe4e71d99;
			while (--xb5964a891b6cf7c3 >= 0)
			{
				num = (x41fcadcc0506e331.xe6d5d55071bc950f[(int)((UIntPtr)((num ^ (uint)x1ef26dbdd5d13d24[xf2ebf5458da93d3b++]) & 255U))] ^ num >> 8);
			}
			this.xa4660e7fe4e71d99 = ~num;
		}

		private uint xa4660e7fe4e71d99;

		private static uint[] xe6d5d55071bc950f = x41fcadcc0506e331.x40d0ed9be6b42277();
	}
}
