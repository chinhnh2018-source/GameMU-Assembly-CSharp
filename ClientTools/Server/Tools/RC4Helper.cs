using System;
using System.Text;

namespace Server.Tools
{
	public class RC4Helper
	{
		public static void RC4(byte[] bytesData, int offset, int count, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			RC4Helper.RC4(bytesData, offset, count, bytes);
		}

		public static void RC4(byte[] bytesData, int offset, int count, byte[] key)
		{
			byte[] array = new byte[256];
			int num;
			if (((uint)num & 0U) != 0U)
			{
				goto IL_153;
			}
			goto IL_189;
			IL_CF:
			int num2;
			byte b;
			array[num2] = b;
			int num3;
			num3++;
			IL_D8:
			byte[] array2;
			if (num3 < 256)
			{
				num2 = (num2 + (int)array[num3] + (int)array2[num3]) % 256;
				b = array[num3];
				array[num3] = array[num2];
				goto IL_CF;
			}
			num2 = (num3 = 0);
			if ((uint)b + (uint)num < 0U)
			{
				goto IL_BC;
			}
			if (-2 == 0)
			{
				goto IL_104;
			}
			if ((uint)num3 - (uint)num3 < 0U)
			{
				goto IL_189;
			}
			num = offset;
			IL_6D:
			if (num >= offset + count)
			{
				return;
			}
			IL_BC:
			num3 = (num3 + 1) % 256;
			IL_104:
			num2 = (num2 + (int)array[num3]) % 256;
			b = array[num3];
			array[num3] = array[num2];
			bool flag = (uint)b + (uint)num3 < 0U;
			if (!flag)
			{
				array[num2] = b;
				int num4;
				if ((uint)num2 <= 4294967295U)
				{
					num4 = (int)(array[num3] + array[num2]) % 256;
				}
				int num5 = num;
				bytesData[num5] ^= array[num4];
				num++;
				goto IL_6D;
			}
			IL_147:
			if (num3 >= 256)
			{
				num2 = 0;
				num3 = 0;
				goto IL_D8;
			}
			array[num3] = (byte)num3;
			IL_153:
			array2[num3] = key[num3 % key.GetLength(0)];
			if ((uint)offset - (uint)num >= 0U)
			{
				num3++;
				goto IL_147;
			}
			goto IL_CF;
			IL_189:
			array2 = new byte[256];
			num3 = 0;
			goto IL_147;
		}

		public static void RC4(byte[] bytesData, byte[] key)
		{
			RC4Helper.RC4(bytesData, 0, bytesData.Length, key);
		}

		public static void RC4(byte[] bytesData, string key)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(key);
			RC4Helper.RC4(bytesData, bytes);
		}
	}
}
