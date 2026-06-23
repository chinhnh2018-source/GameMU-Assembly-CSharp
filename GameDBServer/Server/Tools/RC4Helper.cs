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
			byte[] array2 = new byte[256];
			int i;
			for (i = 0; i < 256; i++)
			{
				array[i] = (byte)i;
				array2[i] = key[i % key.GetLength(0)];
			}
			int num = 0;
			for (i = 0; i < 256; i++)
			{
				num = (num + (int)array[i] + (int)array2[i]) % 256;
				byte b = array[i];
				array[i] = array[num];
				array[num] = b;
			}
			num = (i = 0);
			for (int j = offset; j < offset + count; j++)
			{
				i = (i + 1) % 256;
				num = (num + (int)array[i]) % 256;
				byte b = array[i];
				array[i] = array[num];
				array[num] = b;
				int num2 = (int)(array[i] + array[num]) % 256;
				int num3 = j;
				bytesData[num3] ^= array[num2];
			}
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
