using System;
using System.Text;

namespace ProtoBuf.Serializers
{
	internal sealed class x270e6eb3f68536aa
	{
		private x270e6eb3f68536aa()
		{
		}

		public static byte[] x6f5dd1a50cda6607(string xcdaeea7afaf570ff, Encoding xff3edc9aa5f0523b)
		{
			if (xcdaeea7afaf570ff == null)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			if (xff3edc9aa5f0523b != null)
			{
				byte[] bytes = xff3edc9aa5f0523b.GetBytes(xcdaeea7afaf570ff);
				return x270e6eb3f68536aa.x6f5dd1a50cda6607(bytes);
			}
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding");
		}

		public static byte[] x6f5dd1a50cda6607(string xcdaeea7afaf570ff)
		{
			return x270e6eb3f68536aa.x6f5dd1a50cda6607(xcdaeea7afaf570ff, new UTF8Encoding());
		}

		public static string xa1a39e70f15819dd(byte[] xcdaeea7afaf570ff)
		{
			if (xcdaeea7afaf570ff == null)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			string text = BitConverter.ToString(x270e6eb3f68536aa.x6f5dd1a50cda6607(xcdaeea7afaf570ff));
			return text.Replace("-", "");
		}

		public static string xa1a39e70f15819dd(string xcdaeea7afaf570ff, Encoding xff3edc9aa5f0523b)
		{
			if (xcdaeea7afaf570ff == null)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			if (xff3edc9aa5f0523b != null)
			{
				byte[] bytes = xff3edc9aa5f0523b.GetBytes(xcdaeea7afaf570ff);
				return x270e6eb3f68536aa.xa1a39e70f15819dd(bytes);
			}
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHashString(string) overload to use UTF8 Encoding");
		}

		public static string xa1a39e70f15819dd(string xcdaeea7afaf570ff)
		{
			return x270e6eb3f68536aa.xa1a39e70f15819dd(xcdaeea7afaf570ff, new UTF8Encoding());
		}

		public static byte[] x6f5dd1a50cda6607(byte[] xcdaeea7afaf570ff)
		{
			if (xcdaeea7afaf570ff == null)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			x1408d8967d2f1af2 xe6a177a77b62dafb = default(x1408d8967d2f1af2);
			int num;
			if (true && 4 != 0)
			{
				if (true)
				{
					xe6a177a77b62dafb.xda71bf6f7c07c3bc = 1732584193U;
					xe6a177a77b62dafb.x8e8f6cc6a0756b05 = 4023233417U;
					bool flag = (uint)num + (uint)num > uint.MaxValue;
					if (flag)
					{
						goto IL_92;
					}
					xe6a177a77b62dafb.x857912840ffd015f = 2562383102U;
					xe6a177a77b62dafb.x5d593cee9d844848 = 271733878U;
					flag = ((uint)num > uint.MaxValue);
					if (flag)
					{
						goto IL_52;
					}
					num = 0;
					goto IL_1D;
				}
				IL_0F:
				x270e6eb3f68536aa.x27a41f9a819cf224(xcdaeea7afaf570ff, ref xe6a177a77b62dafb, num);
				num += 64;
				IL_1D:
				if (num <= xcdaeea7afaf570ff.Length - 64)
				{
					goto IL_0F;
				}
				IL_52:
				IL_92:;
			}
			return x270e6eb3f68536aa.x8259195df2f3ddaa(xcdaeea7afaf570ff, num, xcdaeea7afaf570ff.Length - num, xe6a177a77b62dafb, (long)xcdaeea7afaf570ff.Length * 8L);
		}

		internal static byte[] x8259195df2f3ddaa(byte[] xcdaeea7afaf570ff, int x424caea8cd0993e9, int x2e94540690ec6f24, x1408d8967d2f1af2 xe6a177a77b62dafb, long xb5964a891b6cf7c3)
		{
			byte[] array = new byte[64];
			byte[] bytes;
			do
			{
				bytes = BitConverter.GetBytes(xb5964a891b6cf7c3);
			}
			while ((uint)x424caea8cd0993e9 + (uint)x424caea8cd0993e9 > 4294967295U);
			bool flag = ((uint)xb5964a891b6cf7c3 & 0U) == 0U;
			if (flag)
			{
				Array.Copy(xcdaeea7afaf570ff, x424caea8cd0993e9, array, 0, x2e94540690ec6f24);
				array[x2e94540690ec6f24] = 128;
				flag = ((uint)x2e94540690ec6f24 + (uint)xb5964a891b6cf7c3 < 0U);
				if (flag)
				{
					goto IL_17B;
				}
			}
			byte[] array2;
			for (;;)
			{
				flag = (((uint)x2e94540690ec6f24 | uint.MaxValue) == 0U);
				if (flag)
				{
					goto IL_E2;
				}
				while (x2e94540690ec6f24 > 56)
				{
					x270e6eb3f68536aa.x27a41f9a819cf224(array, ref xe6a177a77b62dafb, 0);
					flag = (((uint)x2e94540690ec6f24 & 0U) == 0U);
					if (flag)
					{
						goto IL_10A;
					}
				}
				goto IL_E2;
				IL_3D:
				array2 = new byte[16];
				Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.xda71bf6f7c07c3bc), 0, array2, 0, 4);
				Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.x8e8f6cc6a0756b05), 0, array2, 4, 4);
				flag = ((uint)x424caea8cd0993e9 > uint.MaxValue);
				if (flag)
				{
					continue;
				}
				break;
				IL_E2:
				Array.Copy(bytes, 0, array, 56, 8);
				x270e6eb3f68536aa.x27a41f9a819cf224(array, ref xe6a177a77b62dafb, 0);
				if (((uint)x2e94540690ec6f24 & 0U) == 0U)
				{
					goto IL_3D;
				}
				IL_10A:
				if (!false)
				{
					array = new byte[64];
					Array.Copy(bytes, 0, array, 56, 8);
					x270e6eb3f68536aa.x27a41f9a819cf224(array, ref xe6a177a77b62dafb, 0);
				}
				goto IL_3D;
			}
			IL_17B:
			Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.x857912840ffd015f), 0, array2, 8, 4);
			Array.Copy(BitConverter.GetBytes(xe6a177a77b62dafb.x5d593cee9d844848), 0, array2, 12, 4);
			return array2;
		}

		internal static void x27a41f9a819cf224(byte[] xcdaeea7afaf570ff, ref x1408d8967d2f1af2 xe2fa3b79f67f775c, int x424caea8cd0993e9)
		{
			uint[] array = x270e6eb3f68536aa.xeb51de41c86a575e(xcdaeea7afaf570ff, x424caea8cd0993e9);
			uint num = xe2fa3b79f67f775c.xda71bf6f7c07c3bc;
			uint num2;
			if ((uint)x424caea8cd0993e9 + num2 < 0U)
			{
				goto IL_85;
			}
			uint num3 = xe2fa3b79f67f775c.x8e8f6cc6a0756b05;
			bool flag = num + num > uint.MaxValue;
			if (flag)
			{
				goto IL_805;
			}
			num2 = xe2fa3b79f67f775c.x857912840ffd015f;
			uint num4 = xe2fa3b79f67f775c.x5d593cee9d844848;
			num = x270e6eb3f68536aa.x704d5d75fd559aed(num, num3, num2, num4, array[0], 7, 3614090360U);
			num4 = x270e6eb3f68536aa.x704d5d75fd559aed(num4, num, num3, num2, array[1], 12, 3905402710U);
			num2 = x270e6eb3f68536aa.x704d5d75fd559aed(num2, num4, num, num3, array[2], 17, 606105819U);
			goto IL_7AD;
			do
			{
				IL_221:
				num2 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num2, num4, num, num3, array[14], 15, 2878612391U);
				num3 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num3, num2, num4, num, array[5], 21, 4237533241U);
				num = x270e6eb3f68536aa.x3dc5c64d299f7d92(num, num3, num2, num4, array[12], 6, 1700485571U);
				num4 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num4, num, num3, num2, array[3], 10, 2399980690U);
				num2 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num2, num4, num, num3, array[10], 15, 4293915773U);
			}
			while (num3 + num < 0U);
			num3 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num3, num2, num4, num, array[1], 21, 2240044497U);
			flag = ((uint)x424caea8cd0993e9 + (uint)x424caea8cd0993e9 > uint.MaxValue);
			if (flag)
			{
				goto IL_2AD;
			}
			num = x270e6eb3f68536aa.x3dc5c64d299f7d92(num, num3, num2, num4, array[8], 6, 1873313359U);
			goto IL_122;
			IL_85:
			num4 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num4, num, num3, num2, array[11], 10, 3174756917U);
			num2 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num2, num4, num, num3, array[2], 15, 718787259U);
			num3 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num3, num2, num4, num, array[9], 21, 3951481745U);
			xe2fa3b79f67f775c.xda71bf6f7c07c3bc = num + xe2fa3b79f67f775c.xda71bf6f7c07c3bc;
			flag = ((num3 & 0U) == 0U);
			if (!flag)
			{
				goto IL_3F0;
			}
			if (num4 + num2 <= 4294967295U)
			{
				xe2fa3b79f67f775c.x8e8f6cc6a0756b05 = num3 + xe2fa3b79f67f775c.x8e8f6cc6a0756b05;
				flag = (num3 - num3 > uint.MaxValue);
				if (!flag)
				{
					xe2fa3b79f67f775c.x857912840ffd015f = num2 + xe2fa3b79f67f775c.x857912840ffd015f;
					if (num3 - num2 > 4294967295U)
					{
						goto IL_221;
					}
					xe2fa3b79f67f775c.x5d593cee9d844848 = num4 + xe2fa3b79f67f775c.x5d593cee9d844848;
				}
				return;
			}
			goto IL_6E2;
			IL_122:
			num4 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num4, num, num3, num2, array[15], 10, 4264355552U);
			num2 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num2, num4, num, num3, array[6], 15, 2734768916U);
			num3 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num3, num2, num4, num, array[13], 21, 1309151649U);
			IL_2AD:
			IL_35D:
			IL_3D2:
			flag = (num - num2 > uint.MaxValue);
			if (flag)
			{
				goto IL_5E9;
			}
			goto IL_66F;
			IL_3F0:
			num2 = x270e6eb3f68536aa.x68123086ccad0951(num2, num4, num, num3, array[7], 14, 1735328473U);
			num3 = x270e6eb3f68536aa.x68123086ccad0951(num3, num2, num4, num, array[12], 20, 2368359562U);
			for (;;)
			{
				num = x270e6eb3f68536aa.x9b7c44d488dc3d92(num, num3, num2, num4, array[5], 4, 4294588738U);
				num4 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num4, num, num3, num2, array[8], 11, 2272392833U);
				num2 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num2, num4, num, num3, array[11], 16, 1839030562U);
				num3 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num3, num2, num4, num, array[14], 23, 4259657740U);
				num = x270e6eb3f68536aa.x9b7c44d488dc3d92(num, num3, num2, num4, array[1], 4, 2763975236U);
				if (false)
				{
					goto Block_9;
				}
				num4 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num4, num, num3, num2, array[4], 11, 1272893353U);
				if (num2 - num2 < 0U)
				{
					goto IL_7AD;
				}
				num2 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num2, num4, num, num3, array[7], 16, 4139469664U);
				num3 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num3, num2, num4, num, array[10], 23, 3200236656U);
				flag = (num3 - num3 < 0U);
				if (flag)
				{
					goto IL_3D2;
				}
				for (;;)
				{
					num = x270e6eb3f68536aa.x9b7c44d488dc3d92(num, num3, num2, num4, array[13], 4, 681279174U);
					if (num2 - num < 0U)
					{
						break;
					}
					num4 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num4, num, num3, num2, array[0], 11, 3936430074U);
					num2 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num2, num4, num, num3, array[3], 16, 3572445317U);
					num3 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num3, num2, num4, num, array[6], 23, 76029189U);
					num = x270e6eb3f68536aa.x9b7c44d488dc3d92(num, num3, num2, num4, array[9], 4, 3654602809U);
					num4 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num4, num, num3, num2, array[12], 11, 3873151461U);
					if (4 == 0)
					{
						goto IL_35D;
					}
					if (num3 - num3 >= 0U)
					{
						goto Block_5;
					}
				}
			}
			Block_5:
			if (!false)
			{
				num2 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num2, num4, num, num3, array[15], 16, 530742520U);
				num3 = x270e6eb3f68536aa.x9b7c44d488dc3d92(num3, num2, num4, num, array[2], 23, 3299628645U);
				num = x270e6eb3f68536aa.x3dc5c64d299f7d92(num, num3, num2, num4, array[0], 6, 4096336452U);
				num4 = x270e6eb3f68536aa.x3dc5c64d299f7d92(num4, num, num3, num2, array[7], 10, 1126891415U);
				goto IL_221;
			}
			goto IL_536;
			Block_9:
			goto IL_561;
			IL_536:
			num3 = x270e6eb3f68536aa.x68123086ccad0951(num3, num2, num4, num, array[8], 20, 1163531501U);
			flag = (num3 < 0U);
			if (!flag)
			{
				num = x270e6eb3f68536aa.x68123086ccad0951(num, num3, num2, num4, array[13], 5, 2850285829U);
				num4 = x270e6eb3f68536aa.x68123086ccad0951(num4, num, num3, num2, array[2], 9, 4243563512U);
				if (num + num3 < 0U)
				{
					goto IL_5B5;
				}
			}
			goto IL_3F0;
			IL_561:
			num4 = x270e6eb3f68536aa.x68123086ccad0951(num4, num, num3, num2, array[10], 9, 38016083U);
			if (!false)
			{
				num2 = x270e6eb3f68536aa.x68123086ccad0951(num2, num4, num, num3, array[15], 14, 3634488961U);
				num3 = x270e6eb3f68536aa.x68123086ccad0951(num3, num2, num4, num, array[4], 20, 3889429448U);
				num = x270e6eb3f68536aa.x68123086ccad0951(num, num3, num2, num4, array[9], 5, 568446438U);
				num4 = x270e6eb3f68536aa.x68123086ccad0951(num4, num, num3, num2, array[14], 9, 3275163606U);
				num2 = x270e6eb3f68536aa.x68123086ccad0951(num2, num4, num, num3, array[3], 14, 4107603335U);
				goto IL_536;
			}
			goto IL_122;
			IL_5B5:
			num = x270e6eb3f68536aa.x68123086ccad0951(num, num3, num2, num4, array[5], 5, 3593408605U);
			goto IL_561;
			IL_5E9:
			num4 = x270e6eb3f68536aa.x704d5d75fd559aed(num4, num, num3, num2, array[13], 12, 4254626195U);
			num2 = x270e6eb3f68536aa.x704d5d75fd559aed(num2, num4, num, num3, array[14], 17, 2792965006U);
			num3 = x270e6eb3f68536aa.x704d5d75fd559aed(num3, num2, num4, num, array[15], 22, 1236535329U);
			num = x270e6eb3f68536aa.x68123086ccad0951(num, num3, num2, num4, array[1], 5, 4129170786U);
			num4 = x270e6eb3f68536aa.x68123086ccad0951(num4, num, num3, num2, array[6], 9, 3225465664U);
			flag = (num3 + num3 > uint.MaxValue);
			if (!flag)
			{
				num2 = x270e6eb3f68536aa.x68123086ccad0951(num2, num4, num, num3, array[11], 14, 643717713U);
				num3 = x270e6eb3f68536aa.x68123086ccad0951(num3, num2, num4, num, array[0], 20, 3921069994U);
				goto IL_5B5;
			}
			IL_66F:
			flag = (num - num2 < 0U);
			if (flag)
			{
				return;
			}
			num = x270e6eb3f68536aa.x3dc5c64d299f7d92(num, num3, num2, num4, array[4], 6, 4149444226U);
			goto IL_85;
			IL_6E2:
			num3 = x270e6eb3f68536aa.x704d5d75fd559aed(num3, num2, num4, num, array[7], 22, 4249261313U);
			num = x270e6eb3f68536aa.x704d5d75fd559aed(num, num3, num2, num4, array[8], 7, 1770035416U);
			num4 = x270e6eb3f68536aa.x704d5d75fd559aed(num4, num, num3, num2, array[9], 12, 2336552879U);
			num2 = x270e6eb3f68536aa.x704d5d75fd559aed(num2, num4, num, num3, array[10], 17, 4294925233U);
			num3 = x270e6eb3f68536aa.x704d5d75fd559aed(num3, num2, num4, num, array[11], 22, 2304563134U);
			num = x270e6eb3f68536aa.x704d5d75fd559aed(num, num3, num2, num4, array[12], 7, 1804603682U);
			goto IL_5E9;
			IL_7AD:
			num3 = x270e6eb3f68536aa.x704d5d75fd559aed(num3, num2, num4, num, array[3], 22, 3250441966U);
			IL_805:
			if (num4 - (uint)x424caea8cd0993e9 >= 0U)
			{
				num = x270e6eb3f68536aa.x704d5d75fd559aed(num, num3, num2, num4, array[4], 7, 4118548399U);
				num4 = x270e6eb3f68536aa.x704d5d75fd559aed(num4, num, num3, num2, array[5], 12, 1200080426U);
				num2 = x270e6eb3f68536aa.x704d5d75fd559aed(num2, num4, num, num3, array[6], 17, 2821735955U);
				goto IL_6E2;
			}
			goto IL_221;
		}

		private static uint x704d5d75fd559aed(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + x270e6eb3f68536aa.xb17b2bbeab91dd67(x19218ffab70283ef + ((xe7ebe10fa44d8d49 & x3c4da2980d043c95) | ((xe7ebe10fa44d8d49 ^ uint.MaxValue) & x73f821c71fe1e676)) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint x68123086ccad0951(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + x270e6eb3f68536aa.xb17b2bbeab91dd67(x19218ffab70283ef + ((xe7ebe10fa44d8d49 & x73f821c71fe1e676) | (x3c4da2980d043c95 & (x73f821c71fe1e676 ^ uint.MaxValue))) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint x9b7c44d488dc3d92(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + x270e6eb3f68536aa.xb17b2bbeab91dd67(x19218ffab70283ef + (xe7ebe10fa44d8d49 ^ x3c4da2980d043c95 ^ x73f821c71fe1e676) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint x3dc5c64d299f7d92(uint x19218ffab70283ef, uint xe7ebe10fa44d8d49, uint x3c4da2980d043c95, uint x73f821c71fe1e676, uint x08db3aeabb253cb1, int xe4115acdf4fbfccc, uint x3201d6d15a947682)
		{
			return xe7ebe10fa44d8d49 + x270e6eb3f68536aa.xb17b2bbeab91dd67(x19218ffab70283ef + (x3c4da2980d043c95 ^ (xe7ebe10fa44d8d49 | (x73f821c71fe1e676 ^ uint.MaxValue))) + x08db3aeabb253cb1 + x3201d6d15a947682, xe4115acdf4fbfccc);
		}

		private static uint xb17b2bbeab91dd67(uint x7b28e8a789372508, int xe4115acdf4fbfccc)
		{
			return x7b28e8a789372508 << xe4115acdf4fbfccc | x7b28e8a789372508 >> 32 - xe4115acdf4fbfccc;
		}

		private static uint[] xeb51de41c86a575e(byte[] xcdaeea7afaf570ff, int x424caea8cd0993e9)
		{
			if (xcdaeea7afaf570ff != null)
			{
				uint[] array = new uint[16];
				int i;
				for (i = 0; i < 16; i++)
				{
					array[i] = (uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + i * 4];
					array[i] += (uint)((uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + i * 4 + 1] << 8);
					array[i] += (uint)((uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + i * 4 + 2] << 16);
					array[i] += (uint)((uint)xcdaeea7afaf570ff[x424caea8cd0993e9 + i * 4 + 3] << 24);
					if ((uint)i - (uint)x424caea8cd0993e9 >= 0U)
					{
					}
				}
				bool flag = (uint)i - (uint)i < 0U;
				if (!flag)
				{
					return array;
				}
			}
			throw new ArgumentNullException("input", "Unable convert null array to array of uInts");
		}

		internal static char[] xdb2b9a5a5a353435 = new char[]
		{
			'4',
			'C',
			'5',
			'D',
			'1',
			'D',
			'0',
			'2',
			'B',
			'C',
			'C',
			'E',
			'4',
			'9',
			'5',
			'9',
			'F',
			'E',
			'B',
			'9',
			'4',
			'0',
			'1',
			'4',
			'2',
			'1',
			'9',
			'7',
			'F',
			'6',
			'C',
			'A'
		};
	}
}
