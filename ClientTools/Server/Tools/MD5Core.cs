using System;
using System.Text;

namespace Server.Tools
{
	public sealed class MD5Core
	{
		private MD5Core()
		{
		}

		public static byte[] GetHash(string input, Encoding encoding)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			if (encoding != null)
			{
				byte[] bytes = encoding.GetBytes(input);
				return MD5Core.GetHash(bytes);
			}
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding");
		}

		public static byte[] GetHash(string input)
		{
			return MD5Core.GetHash(input, new UTF8Encoding());
		}

		public static string GetHashString(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			string text = BitConverter.ToString(MD5Core.GetHash(input));
			return text.Replace("-", "");
		}

		public static string GetHashString(string input, Encoding encoding)
		{
			if (input != null)
			{
				while (encoding != null)
				{
					if (false)
					{
						if (false)
						{
							continue;
						}
					}
					byte[] bytes = encoding.GetBytes(input);
					if (false)
					{
						goto IL_15;
					}
					return MD5Core.GetHashString(bytes);
				}
				throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHashString(string) overload to use UTF8 Encoding");
			}
			IL_15:
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}

		public static string GetHashString(string input)
		{
			return MD5Core.GetHashString(input, new UTF8Encoding());
		}

		public static byte[] GetHash(byte[] input)
		{
			x1408d8967d2f1af2 xe6a177a77b62dafb;
			int i;
			if (input == null)
			{
				if (2147483647 != 0)
				{
					goto IL_88;
				}
			}
			else
			{
				xe6a177a77b62dafb = default(x1408d8967d2f1af2);
				do
				{
					xe6a177a77b62dafb.xda71bf6f7c07c3bc = 1732584193U;
					xe6a177a77b62dafb.x8e8f6cc6a0756b05 = 4023233417U;
				}
				while (((uint)i | 255U) == 0U);
				xe6a177a77b62dafb.x857912840ffd015f = 2562383102U;
				xe6a177a77b62dafb.x5d593cee9d844848 = 271733878U;
				if (!true)
				{
					goto IL_88;
				}
				if (2 == 0)
				{
					goto IL_A4;
				}
			}
			if (15 != 0)
			{
			}
			for (i = 0; i <= input.Length - 64; i += 64)
			{
				MD5Core.x27a41f9a819cf224(input, ref xe6a177a77b62dafb, i);
			}
			goto IL_A4;
			IL_88:
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			IL_A4:
			return MD5Core.x8259195df2f3ddaa(input, i, input.Length - i, xe6a177a77b62dafb, (long)input.Length * 8L);
		}

		internal static byte[] x8259195df2f3ddaa(byte[] xcdaeea7afaf570ff, int x424caea8cd0993e9, int x2e94540690ec6f24, x1408d8967d2f1af2 xe6a177a77b62dafb, long xb5964a891b6cf7c3)
		{
			byte[] array = new byte[64];
			if (2147483647 == 0)
			{
				goto IL_A2;
			}
			byte[] bytes = BitConverter.GetBytes(xb5964a891b6cf7c3);
			Array.Copy(xcdaeea7afaf570ff, x424caea8cd0993e9, array, 0, x2e94540690ec6f24);
			array[x2e94540690ec6f24] = 128;
			bool flag = (uint)x424caea8cd0993e9 > uint.MaxValue;
			if (!flag && x2e94540690ec6f24 > 56)
			{
				MD5Core.x27a41f9a819cf224(array, ref xe6a177a77b62dafb, 0);
				array = new byte[64];
			}
			else
			{
				Array.Copy(bytes, 0, array, 56, 8);
				if (!false)
				{
					goto IL_CB;
				}
				goto IL_99;
			}
			IL_8E:
			Array.Copy(bytes, 0, array, 56, 8);
			IL_99:
			MD5Core.x27a41f9a819cf224(array, ref xe6a177a77b62dafb, 0);
			IL_A2:
			byte[] array2 = new byte[16];
			Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.xda71bf6f7c07c3bc), 0, array2, 0, 4);
			Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.x8e8f6cc6a0756b05), 0, array2, 4, 4);
			Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.x857912840ffd015f), 0, array2, 8, 4);
			flag = (((uint)x424caea8cd0993e9 & 0U) == 0U);
			if (flag)
			{
				Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.x5d593cee9d844848), 0, array2, 12, 4);
				flag = ((uint)x2e94540690ec6f24 - (uint)x2e94540690ec6f24 < 0U);
				if (flag)
				{
					goto IL_8E;
				}
				return array2;
			}
			IL_CB:
			MD5Core.x27a41f9a819cf224(array, ref xe6a177a77b62dafb, 0);
			goto IL_A2;
		}

		internal static void x27a41f9a819cf224(byte[] xcdaeea7afaf570ff, ref x1408d8967d2f1af2 xe2fa3b79f67f775c, int x424caea8cd0993e9)
		{
			uint[] array = MD5Core.xeb51de41c86a575e(xcdaeea7afaf570ff, x424caea8cd0993e9);
			for (;;)
			{
				uint num;
				bool flag = (num & 0U) == 0U;
				if (!flag)
				{
					goto IL_3C3;
				}
				uint num2 = xe2fa3b79f67f775c.xda71bf6f7c07c3bc;
				uint num3 = xe2fa3b79f67f775c.x8e8f6cc6a0756b05;
				uint num4;
				if ((uint)x424caea8cd0993e9 >= 0U)
				{
					num = xe2fa3b79f67f775c.x857912840ffd015f;
					flag = (((uint)x424caea8cd0993e9 & 0U) == 0U);
					if (flag)
					{
						num4 = xe2fa3b79f67f775c.x5d593cee9d844848;
						num2 = MD5Core.x704d5d75fd559aed(num2, num3, num, num4, array[0], 7, 3614090360U);
						num4 = MD5Core.x704d5d75fd559aed(num4, num2, num3, num, array[1], 12, 3905402710U);
						goto IL_788;
					}
					goto IL_465;
				}
				IL_2AE:
				num2 = MD5Core.x9b7c44d488dc3d92(num2, num3, num, num4, array[9], 4, 3654602809U);
				num4 = MD5Core.x9b7c44d488dc3d92(num4, num2, num3, num, array[12], 11, 3873151461U);
				num = MD5Core.x9b7c44d488dc3d92(num, num4, num2, num3, array[15], 16, 530742520U);
				num3 = MD5Core.x9b7c44d488dc3d92(num3, num, num4, num2, array[2], 23, 3299628645U);
				num2 = MD5Core.x3dc5c64d299f7d92(num2, num3, num, num4, array[0], 6, 4096336452U);
				flag = (num3 - num4 < 0U);
				if (flag)
				{
					continue;
				}
				flag = (num4 + num4 > uint.MaxValue);
				if (flag)
				{
					break;
				}
				num4 = MD5Core.x3dc5c64d299f7d92(num4, num2, num3, num, array[7], 10, 1126891415U);
				goto IL_171;
				IL_3C3:
				num = MD5Core.x9b7c44d488dc3d92(num, num4, num2, num3, array[11], 16, 1839030562U);
				num3 = MD5Core.x9b7c44d488dc3d92(num3, num, num4, num2, array[14], 23, 4259657740U);
				for (;;)
				{
					num2 = MD5Core.x9b7c44d488dc3d92(num2, num3, num, num4, array[1], 4, 2763975236U);
					if ((uint)x424caea8cd0993e9 - num2 > 4294967295U)
					{
						goto IL_788;
					}
					num4 = MD5Core.x9b7c44d488dc3d92(num4, num2, num3, num, array[4], 11, 1272893353U);
					num = MD5Core.x9b7c44d488dc3d92(num, num4, num2, num3, array[7], 16, 4139469664U);
					num3 = MD5Core.x9b7c44d488dc3d92(num3, num, num4, num2, array[10], 23, 3200236656U);
					num2 = MD5Core.x9b7c44d488dc3d92(num2, num3, num, num4, array[13], 4, 681279174U);
					num4 = MD5Core.x9b7c44d488dc3d92(num4, num2, num3, num, array[0], 11, 3936430074U);
					num = MD5Core.x9b7c44d488dc3d92(num, num4, num2, num3, array[3], 16, 3572445317U);
					if (!false)
					{
						if ((num3 & 0U) != 0U)
						{
							break;
						}
						flag = ((num2 & 0U) == 0U);
						if (!flag)
						{
							goto IL_1B5;
						}
						flag = (num < 0U);
						if (!flag)
						{
							break;
						}
					}
				}
				flag = ((num | 4U) == 0U);
				if (flag)
				{
					goto IL_40E;
				}
				if (8 == 0)
				{
					continue;
				}
				num3 = MD5Core.x9b7c44d488dc3d92(num3, num, num4, num2, array[6], 23, 76029189U);
				goto IL_2AE;
				IL_479:
				num4 = MD5Core.x9b7c44d488dc3d92(num4, num2, num3, num, array[8], 11, 2272392833U);
				goto IL_3C3;
				IL_465:
				num2 = MD5Core.x9b7c44d488dc3d92(num2, num3, num, num4, array[5], 4, 4294588738U);
				goto IL_479;
				IL_40E:
				num4 = MD5Core.x68123086ccad0951(num4, num2, num3, num, array[2], 9, 4243563512U);
				num = MD5Core.x68123086ccad0951(num, num4, num2, num3, array[7], 14, 1735328473U);
				if (num4 + num3 >= 0U)
				{
					num3 = MD5Core.x68123086ccad0951(num3, num, num4, num2, array[12], 20, 2368359562U);
					goto IL_465;
				}
				goto IL_4A3;
				IL_711:
				num2 = MD5Core.x704d5d75fd559aed(num2, num3, num, num4, array[4], 7, 4118548399U);
				num4 = MD5Core.x704d5d75fd559aed(num4, num2, num3, num, array[5], 12, 1200080426U);
				num = MD5Core.x704d5d75fd559aed(num, num4, num2, num3, array[6], 17, 2821735955U);
				num3 = MD5Core.x704d5d75fd559aed(num3, num, num4, num2, array[7], 22, 4249261313U);
				num2 = MD5Core.x704d5d75fd559aed(num2, num3, num, num4, array[8], 7, 1770035416U);
				num4 = MD5Core.x704d5d75fd559aed(num4, num2, num3, num, array[9], 12, 2336552879U);
				if (num3 < 0U)
				{
					goto IL_479;
				}
				num = MD5Core.x704d5d75fd559aed(num, num4, num2, num3, array[10], 17, 4294925233U);
				if ((num3 | 4294967295U) != 0U)
				{
					num3 = MD5Core.x704d5d75fd559aed(num3, num, num4, num2, array[11], 22, 2304563134U);
					num2 = MD5Core.x704d5d75fd559aed(num2, num3, num, num4, array[12], 7, 1804603682U);
					flag = ((num2 | 2U) == 0U);
					if (!flag)
					{
						if (num2 - num < 0U)
						{
							goto IL_4F9;
						}
						num4 = MD5Core.x704d5d75fd559aed(num4, num2, num3, num, array[13], 12, 4254626195U);
						num = MD5Core.x704d5d75fd559aed(num, num4, num2, num3, array[14], 17, 2792965006U);
						num3 = MD5Core.x704d5d75fd559aed(num3, num, num4, num2, array[15], 22, 1236535329U);
					}
					num2 = MD5Core.x68123086ccad0951(num2, num3, num, num4, array[1], 5, 4129170786U);
					goto IL_570;
				}
				goto IL_29;
				IL_788:
				num = MD5Core.x704d5d75fd559aed(num, num4, num2, num3, array[2], 17, 606105819U);
				num3 = MD5Core.x704d5d75fd559aed(num3, num, num4, num2, array[3], 22, 3250441966U);
				goto IL_711;
				IL_171:
				if (num - num3 <= 4294967295U)
				{
					num = MD5Core.x3dc5c64d299f7d92(num, num4, num2, num3, array[14], 15, 2878612391U);
					num3 = MD5Core.x3dc5c64d299f7d92(num3, num, num4, num2, array[5], 21, 4237533241U);
					goto IL_1B5;
				}
				goto IL_711;
				IL_1E0:
				if (num2 + num2 > 4294967295U)
				{
					goto IL_570;
				}
				num = MD5Core.x3dc5c64d299f7d92(num, num4, num2, num3, array[10], 15, 4293915773U);
				num3 = MD5Core.x3dc5c64d299f7d92(num3, num, num4, num2, array[1], 21, 2240044497U);
				num2 = MD5Core.x3dc5c64d299f7d92(num2, num3, num, num4, array[8], 6, 1873313359U);
				num4 = MD5Core.x3dc5c64d299f7d92(num4, num2, num3, num, array[15], 10, 4264355552U);
				num = MD5Core.x3dc5c64d299f7d92(num, num4, num2, num3, array[6], 15, 2734768916U);
				flag = (num3 - num < 0U);
				if (flag)
				{
					goto IL_171;
				}
				num3 = MD5Core.x3dc5c64d299f7d92(num3, num, num4, num2, array[13], 21, 1309151649U);
				goto IL_5C;
				IL_1B5:
				num2 = MD5Core.x3dc5c64d299f7d92(num2, num3, num, num4, array[12], 6, 1700485571U);
				num4 = MD5Core.x3dc5c64d299f7d92(num4, num2, num3, num, array[3], 10, 2399980690U);
				goto IL_1E0;
				IL_5C:
				num2 = MD5Core.x3dc5c64d299f7d92(num2, num3, num, num4, array[4], 6, 4149444226U);
				num4 = MD5Core.x3dc5c64d299f7d92(num4, num2, num3, num, array[11], 10, 3174756917U);
				if (num2 > 4294967295U)
				{
					goto IL_1E0;
				}
				num = MD5Core.x3dc5c64d299f7d92(num, num4, num2, num3, array[2], 15, 718787259U);
				num3 = MD5Core.x3dc5c64d299f7d92(num3, num, num4, num2, array[9], 21, 3951481745U);
				xe2fa3b79f67f775c.xda71bf6f7c07c3bc = num2 + xe2fa3b79f67f775c.xda71bf6f7c07c3bc;
				xe2fa3b79f67f775c.x8e8f6cc6a0756b05 = num3 + xe2fa3b79f67f775c.x8e8f6cc6a0756b05;
				IL_29:
				xe2fa3b79f67f775c.x857912840ffd015f = num + xe2fa3b79f67f775c.x857912840ffd015f;
				xe2fa3b79f67f775c.x5d593cee9d844848 = num4 + xe2fa3b79f67f775c.x5d593cee9d844848;
				flag = (num + num2 < 0U);
				if (flag)
				{
					goto IL_5C;
				}
				break;
				IL_5DE:
				goto IL_40E;
				IL_4F9:
				num2 = MD5Core.x68123086ccad0951(num2, num3, num, num4, array[13], 5, 2850285829U);
				goto IL_5DE;
				IL_4A3:
				num2 = MD5Core.x68123086ccad0951(num2, num3, num, num4, array[9], 5, 568446438U);
				num4 = MD5Core.x68123086ccad0951(num4, num2, num3, num, array[14], 9, 3275163606U);
				num = MD5Core.x68123086ccad0951(num, num4, num2, num3, array[3], 14, 4107603335U);
				num3 = MD5Core.x68123086ccad0951(num3, num, num4, num2, array[8], 20, 1163531501U);
				goto IL_4F9;
				IL_570:
				num4 = MD5Core.x68123086ccad0951(num4, num2, num3, num, array[6], 9, 3225465664U);
				num = MD5Core.x68123086ccad0951(num, num4, num2, num3, array[11], 14, 643717713U);
				num3 = MD5Core.x68123086ccad0951(num3, num, num4, num2, array[0], 20, 3921069994U);
				num2 = MD5Core.x68123086ccad0951(num2, num3, num, num4, array[5], 5, 3593408605U);
				flag = (num - num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_5DE;
				}
				num4 = MD5Core.x68123086ccad0951(num4, num2, num3, num, array[10], 9, 38016083U);
				num = MD5Core.x68123086ccad0951(num, num4, num2, num3, array[15], 14, 3634488961U);
				num3 = MD5Core.x68123086ccad0951(num3, num, num4, num2, array[4], 20, 3889429448U);
				goto IL_4A3;
			}
		}

		private static uint x704d5d75fd559aed(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + MD5Core.xb17b2bbeab91dd67(x19218ffab70283ef + ((xe7ebe10fa44d8d49 & x3c4da2980d043c95) | ((xe7ebe10fa44d8d49 ^ uint.MaxValue) & x73f821c71fe1e676)) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint x68123086ccad0951(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + MD5Core.xb17b2bbeab91dd67(x19218ffab70283ef + ((xe7ebe10fa44d8d49 & x73f821c71fe1e676) | (x3c4da2980d043c95 & (x73f821c71fe1e676 ^ uint.MaxValue))) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint x9b7c44d488dc3d92(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + MD5Core.xb17b2bbeab91dd67(x19218ffab70283ef + (xe7ebe10fa44d8d49 ^ x3c4da2980d043c95 ^ x73f821c71fe1e676) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint x3dc5c64d299f7d92(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + MD5Core.xb17b2bbeab91dd67(x19218ffab70283ef + (x3c4da2980d043c95 ^ (xe7ebe10fa44d8d49 | (x73f821c71fe1e676 ^ uint.MaxValue))) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint xb17b2bbeab91dd67(uint x7b28e8a789372508, int xe4115acdf4fbfccc)
		{
			return x7b28e8a789372508 << xe4115acdf4fbfccc | x7b28e8a789372508 >> 32 - xe4115acdf4fbfccc;
		}

		private static uint[] xeb51de41c86a575e(byte[] xcdaeea7afaf570ff, int x424caea8cd0993e9)
		{
			uint[] array;
			int num;
			if (xcdaeea7afaf570ff == null)
			{
				bool flag = ((uint)x424caea8cd0993e9 | 15U) == 0U;
				if (flag)
				{
					goto IL_66;
				}
				throw new ArgumentNullException("input", "Unable convert null array to array of uInts");
			}
			else
			{
				array = new uint[16];
				num = 0;
			}
			IL_5E:
			if (num >= 16 && !false)
			{
				return array;
			}
			IL_66:
			array[num] = (uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + num * 4];
			array[num] += (uint)((uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + num * 4 + 1] << 8);
			array[num] += (uint)((uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + num * 4 + 2] << 16);
			do
			{
				array[num] += (uint)((uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + num * 4 + 3] << 24);
			}
			while (((uint)x424caea8cd0993e9 | 2147483648U) == 0U);
			num++;
			goto IL_5E;
		}
	}
}
