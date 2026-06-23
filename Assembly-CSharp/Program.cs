using System;

public static class Program
{
	public static ulong GetSpeedKey(byte key)
	{
		ulong num = (ulong)key;
		ulong num2 = (ulong)key;
		num2 |= num << 8;
		num2 |= num << 16;
		num2 |= num << 24;
		num2 |= num << 32;
		num2 |= num << 40;
		num2 |= num << 48;
		return num2 | num << 56;
	}

	private static uint GenSceneRandom(uint seed, int loop)
	{
		uint num = seed;
		uint num2 = 362436069U;
		for (int i = 0; i <= loop; i++)
		{
			num2 = 36969U * (num2 & 65535U) + (num2 >> 16);
			num = 18000U * (num & 65535U) + (num >> 16);
		}
		return (num2 << 16) + num;
	}

	public unsafe static byte[] EncryptSceneData(byte[] bytes)
	{
		int num = bytes.Length;
		if (bytes[num - 1] == 84 && bytes[num - 2] == 77 && bytes[num - 3] == 83 && bytes[num - 4] == 75)
		{
			return bytes;
		}
		Random random = new Random((int)DateTime.Now.Ticks);
		int num2 = random.Next(1, 28);
		int num3 = random.Next(28, 58);
		int num4 = random.Next(1, 58);
		int num5 = random.Next(1, 58);
		byte[] array = new byte[num + 8];
		array[num + 7] = 84;
		array[num + 6] = 77;
		array[num + 5] = 83;
		array[num + 4] = 75;
		array[num + 3] = (byte)(64 + num2);
		array[num + 2] = (byte)(64 + num3);
		array[num + 1] = (byte)(64 + num4);
		array[num] = (byte)(64 + num5);
		Array.Copy(bytes, 0, array, 0, num);
		int seed = (num2 & 240) | (num3 & 15);
		int num6 = (num3 & 240) | (num2 & 15);
		if (num6 > 8)
		{
			num6 = 8;
		}
		uint num7 = Program.GenSceneRandom((uint)seed, num6);
		byte b = (byte)(num7 >> 8 & 15U);
		b |= (byte)((num7 & 15U) << 4);
		ulong speedKey = Program.GetSpeedKey(b);
		int num8 = num / 8;
		fixed (byte* ptr = &array[0])
		{
			int num9 = Program.sekeys.Length;
			ulong* ptr2 = (ulong*)ptr;
			int i = 0;
			int num10 = 0;
			while (i < num8)
			{
				ptr2[i] ^= (Program.sekeys[num10++] ^ speedKey);
				if (num10 == num9)
				{
					num10 = 0;
				}
				i++;
			}
		}
		return array;
	}

	public unsafe static byte[] DecryptSceneData(byte[] bytes)
	{
		if (bytes == null)
		{
			return null;
		}
		int num = bytes.Length;
		if (bytes[num - 1] != 84 || bytes[num - 2] != 77 || bytes[num - 3] != 83 || bytes[num - 4] != 75)
		{
			return bytes;
		}
		byte b = bytes[num - 5] + 1 - 65;
		byte b2 = bytes[num - 6] + 1 - 65;
		int seed = (int)((b & 240) | (b2 & 15));
		int num2 = (int)((b2 & 240) | (b & 15));
		if (num2 > 8)
		{
			num2 = 8;
		}
		uint num3 = Program.GenSceneRandom((uint)seed, num2);
		byte b3 = (byte)(num3 >> 8 & 15U);
		b3 |= (byte)((num3 & 15U) << 4);
		ulong speedKey = Program.GetSpeedKey(b3);
		int num4 = (num - 8) / 8;
		fixed (byte* ptr = &bytes[0])
		{
			int num5 = Program.sekeys.Length;
			ulong* ptr2 = (ulong*)ptr;
			int i = 0;
			int num6 = 0;
			while (i < num4)
			{
				ptr2[i] ^= (Program.sekeys[num6++] ^ speedKey);
				if (num6 == num5)
				{
					num6 = 0;
				}
				i++;
			}
		}
		return bytes;
	}

	public static readonly ulong[] sekeys = new ulong[]
	{
		13566066058741374895UL,
		4731971214571306977UL,
		4193321004716886075UL,
		6979599156043674055UL,
		5586558353969401307UL,
		13575920470149936620UL,
		3366020279693272509UL,
		9759941088763425624UL,
		13776925727083311000UL,
		10768919898749677867UL,
		9563887044969615667UL,
		14955543874447037068UL,
		7979464378319612524UL,
		7677622354713004765UL,
		14764177111311637099UL,
		6686345110620824299UL
	};
}
