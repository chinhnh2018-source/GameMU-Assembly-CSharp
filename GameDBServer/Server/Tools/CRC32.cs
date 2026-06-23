using System;

namespace Server.Tools
{
	public class CRC32
	{
		private static uint[] makeCrcTable()
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				int num2 = 8;
				while (--num2 >= 0)
				{
					if ((num & 1U) != 0U)
					{
						num = (3988292384U ^ num >> 1);
					}
					else
					{
						num >>= 1;
					}
				}
				array[i] = num;
			}
			return array;
		}

		public uint getValue()
		{
			return this.crc & uint.MaxValue;
		}

		public void reset()
		{
			this.crc = 0U;
		}

		public void update(byte[] buf)
		{
			uint num = 0U;
			int num2 = buf.Length;
			uint num3 = ~this.crc;
			while (--num2 >= 0)
			{
				num3 = (CRC32.crcTable[(int)((UIntPtr)((num3 ^ (uint)buf[(int)((UIntPtr)(num++))]) & 255U))] ^ num3 >> 8);
			}
			this.crc = ~num3;
		}

		public void update(byte[] buf, int off, int len)
		{
			uint num = ~this.crc;
			while (--len >= 0)
			{
				num = (CRC32.crcTable[(int)((UIntPtr)((num ^ (uint)buf[off++]) & 255U))] ^ num >> 8);
			}
			this.crc = ~num;
		}

		private uint crc = 0U;

		private static uint[] crcTable = CRC32.makeCrcTable();
	}
}
