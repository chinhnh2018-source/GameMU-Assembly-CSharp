using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using HSGameEngine.GameEngine.Network.Protocol;

namespace ProtoBuf.Serializers
{
	internal class x268d0b7d0d40d0be
	{
		public static void x8e5503aa9f6a6959(byte[] x0bf8a397ae48c3dd, int x0c1c72e01d5b1534, byte[] x2cf8a93deca9cfc0, int x9c456e4fca445855, int x10f4d88af727adbc)
		{
			Array.Copy(x2cf8a93deca9cfc0, x9c456e4fca445855, x0bf8a397ae48c3dd, x0c1c72e01d5b1534, x10f4d88af727adbc);
		}

		public static void x5185110fe25b3f80(byte[] xe8e3a0880e7c96ea, int x0c1c72e01d5b1534, int x10f4d88af727adbc)
		{
			byte b = 0;
			byte[] bytes;
			int num;
			if (((uint)x0c1c72e01d5b1534 | 2147483647U) != 0U)
			{
				bytes = BitConverter.GetBytes(1695843216);
				num = 0;
				goto IL_2A;
			}
			goto IL_30;
			IL_26:
			num++;
			IL_2A:
			if (num < bytes.Length)
			{
				b += bytes[num];
				goto IL_26;
			}
			IL_30:
			for (int i = x0c1c72e01d5b1534; i < x0c1c72e01d5b1534 + x10f4d88af727adbc; i++)
			{
				int num2 = i;
				xe8e3a0880e7c96ea[num2] ^= b;
			}
			bool flag = ((uint)b | 15U) == 0U;
			if (flag)
			{
				goto IL_26;
			}
		}

		public static bool xafee21697ef649d9(byte[] xa447fc54e41dfe06, byte[] xfc2074a859a5db8c)
		{
			if (xa447fc54e41dfe06.Length != xfc2074a859a5db8c.Length)
			{
				return false;
			}
			bool result = true;
			int i = 0;
			while (i < xa447fc54e41dfe06.Length)
			{
				if (xa447fc54e41dfe06[i] != xfc2074a859a5db8c[i])
				{
					result = false;
					return result;
				}
				do
				{
					i++;
				}
				while (false);
			}
			return result;
		}

		public static void xae1c29ff1c7bf52a(byte[] x5cafa8d49ea71ea1, int x374ea4fe62468d0f, int x10f4d88af727adbc)
		{
			DateTime now = DateTime.Now;
			bool flag;
			do
			{
				long ticks = now.Ticks;
				Random random = new Random((int)(ticks & (long)((ulong)-1)) | (int)(ticks >> 32));
				for (int i = 0; i < x10f4d88af727adbc; i++)
				{
					x5cafa8d49ea71ea1[x374ea4fe62468d0f + i] = (byte)random.Next(0, 255);
				}
				flag = ((uint)ticks + (uint)ticks > uint.MaxValue);
			}
			while (flag);
		}

		public static string x979ed402aff3b49f(byte[] xe7ebe10fa44d8d49)
		{
			string text;
			for (;;)
			{
				IL_50:
				text = "";
				for (int i = 0; i < xe7ebe10fa44d8d49.Length; i++)
				{
					text += ((int)(xe7ebe10fa44d8d49[i] & byte.MaxValue)).ToString("X2").ToUpper();
					if (((uint)i | 2147483647U) == 0U)
					{
						goto IL_50;
					}
				}
				break;
			}
			return text;
		}

		public static byte[] xb5c2672463bc6ff8(string xe4115acdf4fbfccc)
		{
			if (xe4115acdf4fbfccc.Length % 2 != 0)
			{
				return null;
			}
			byte[] array;
			for (;;)
			{
				int num = 0;
				for (;;)
				{
					array = new byte[xe4115acdf4fbfccc.Length / 2];
					int num2 = 0;
					bool flag = (uint)num > uint.MaxValue;
					if (flag)
					{
						goto IL_8F;
					}
					IL_27:
					if (num2 >= xe4115acdf4fbfccc.Length / 2)
					{
						if (!false)
						{
							return array;
						}
						continue;
					}
					else
					{
						string s = xe4115acdf4fbfccc.Substring(num2 * 2, 2);
						num = (int.Parse(s, NumberStyles.HexNumber) & 255);
						array[num2] = (byte)num;
						num2++;
						if (false)
						{
							break;
						}
					}
					IL_8F:
					flag = ((uint)num2 + (uint)num2 < 0U);
					if (flag)
					{
						return array;
					}
					if ((uint)num2 - (uint)num2 >= 0U)
					{
						goto IL_27;
					}
					break;
				}
			}
			return array;
		}

		public static void xe98c1a45850a0243(Exception xfbf34718e704c6bc, string x126ee173ebe44562, bool x83c0ed306f052a0e, bool xfc9cdf6e447a7dbf = false)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (;;)
				{
					if ((xfc9cdf6e447a7dbf ? 1U : 0U) - (x83c0ed306f052a0e ? 1U : 0U) >= 0U)
					{
						stringBuilder.AppendFormat("应用程序出现了异常[{0}]:\r\n{1}\r\n", xfc9cdf6e447a7dbf ? 1 : 0, xfbf34718e704c6bc.Message);
					}
					stringBuilder.AppendFormat("\r\n 额外信息: {0}", x126ee173ebe44562);
					if ((xfc9cdf6e447a7dbf ? 1U : 0U) - (x83c0ed306f052a0e ? 1U : 0U) < 0U)
					{
						goto IL_10;
					}
					if (!false)
					{
						if (xfbf34718e704c6bc == null)
						{
							goto IL_79;
						}
					}
					IL_57:
					bool flag;
					while (xfbf34718e704c6bc.InnerException != null)
					{
						stringBuilder.AppendFormat("\r\n {0}", xfbf34718e704c6bc.InnerException.Message);
						flag = (((xfc9cdf6e447a7dbf ? 1U : 0U) | 2147483648U) == 0U);
						if (!flag)
						{
							break;
						}
						if (false)
						{
							goto IL_79;
						}
					}
					stringBuilder.AppendFormat("\r\n {0}", xfbf34718e704c6bc.StackTrace);
					goto IL_79;
					IL_CC:
					if (((x83c0ed306f052a0e ? 1U : 0U) & 0U) != 0U)
					{
						continue;
					}
					break;
					IL_10:
					if ((x83c0ed306f052a0e ? 1U : 0U) + (xfc9cdf6e447a7dbf ? 1U : 0U) >= 0U)
					{
						goto IL_CC;
					}
					goto IL_2F;
					IL_96:
					flag = ((x83c0ed306f052a0e ? 1U : 0U) + (x83c0ed306f052a0e ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_B1;
					}
					goto IL_10;
					IL_2F:
					if (x83c0ed306f052a0e)
					{
						Console.WriteLine(stringBuilder.ToString());
						goto IL_96;
					}
					if (((x83c0ed306f052a0e ? 1U : 0U) & 0U) != 0U)
					{
						goto IL_57;
					}
					IL_B1:
					flag = ((x83c0ed306f052a0e ? 1U : 0U) + (xfc9cdf6e447a7dbf ? 1U : 0U) < 0U);
					if (flag)
					{
						goto IL_CC;
					}
					break;
					IL_79:
					xa9b5a992f5a4ca94.x89a5deb553f41e77(stringBuilder.ToString());
					flag = ((x83c0ed306f052a0e ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_96;
					}
					goto IL_2F;
				}
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static void x0b83dc438054a776(StackTrace x8e571b2001e1678d, string x126ee173ebe44562)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("应   用程序出现了对象锁定超时错误:\r\n", new object[0]);
				stringBuilder.AppendFormat("\r\n 额外信息: {0}", x126ee173ebe44562);
				stringBuilder.AppendFormat("\r\n {0}", x8e571b2001e1678d.ToString());
				xa9b5a992f5a4ca94.x89a5deb553f41e77(stringBuilder.ToString());
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static int x6064d17efdbb7f1d(string xf6987a1745781d6f, int x0cf9930b37188ae3)
		{
			try
			{
				if ("*" != xf6987a1745781d6f)
				{
					return Convert.ToInt32(xf6987a1745781d6f);
				}
				return x0cf9930b37188ae3;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return x0cf9930b37188ae3;
		}

		public static string x44adbff8febcedeb(string xf6987a1745781d6f, string x0cf9930b37188ae3)
		{
			if ("*" != xf6987a1745781d6f)
			{
				return xf6987a1745781d6f;
			}
			return x0cf9930b37188ae3;
		}

		public static long xf6fa0d1bb537b9c7(string xf6987a1745781d6f, long x0cf9930b37188ae3)
		{
			if ("*" == xf6987a1745781d6f)
			{
				return x0cf9930b37188ae3;
			}
			xf6987a1745781d6f = xf6987a1745781d6f.Replace('$', ':');
			try
			{
				DateTime dateTime;
				if (!DateTime.TryParse(xf6987a1745781d6f, out dateTime))
				{
					return 0L;
				}
				return dateTime.Ticks / 10000L;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return 0L;
		}

		public static long xf6fa0d1bb537b9c7(string xf6987a1745781d6f)
		{
			try
			{
				DateTime dateTime;
				if (!DateTime.TryParse(xf6987a1745781d6f, out dateTime))
				{
					return 0L;
				}
				return dateTime.Ticks / 10000L;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return 0L;
		}

		public static long x513b030d388e906c(int x720152aa6cfeb37e)
		{
			return x268d0b7d0d40d0be.x6d991d7c04ed674e + (long)x720152aa6cfeb37e * 1000L;
		}

		public static long x513b030d388e906c(string x720152aa6cfeb37e)
		{
			int x720152aa6cfeb37e2 = Convert.ToInt32(x720152aa6cfeb37e);
			return x268d0b7d0d40d0be.x513b030d388e906c(x720152aa6cfeb37e2);
		}

		public static int xba48544946b5f89f()
		{
			long xf92937f01b714ca = DateTime.Now.Ticks / 10000L;
			return x268d0b7d0d40d0be.x13b977e337744cc4(xf92937f01b714ca);
		}

		public static int x13b977e337744cc4(long xf92937f01b714ca8)
		{
			long num = (xf92937f01b714ca8 - x268d0b7d0d40d0be.x6d991d7c04ed674e) / 1000L;
			return (int)num;
		}

		public static TCPOutPacket xb80ea8cafac5c74f<T>(T x6ed4ed9ed59eb694, TCPOutPacketPool xb3a098bf0147fd2c, int x541a67b95acd3459)
		{
			byte[] array = x268d0b7d0d40d0be.x9d0c6f7667de9ce7<T>(x6ed4ed9ed59eb694);
			return TCPOutPacket.MakeTCPOutPacket(xb3a098bf0147fd2c, array, 0, array.Length, x541a67b95acd3459);
		}

		public static byte[] x9d0c6f7667de9ce7<T>(T x6ed4ed9ed59eb694)
		{
			try
			{
				MemoryStream memoryStream;
				if (x6ed4ed9ed59eb694 != null)
				{
					memoryStream = new MemoryStream();
					Serializer.Serialize<T>(memoryStream, x6ed4ed9ed59eb694);
					goto IL_6F;
				}
				byte[] array = new byte[0];
				goto IL_25;
				IL_07:
				return array;
				IL_19:
				byte[] array2;
				if (array2.Length >= array.Length)
				{
					if (false)
					{
						goto IL_34;
					}
				}
				else
				{
					array = array2;
					if (!false)
					{
						goto IL_07;
					}
				}
				if (!true)
				{
					goto IL_54;
				}
				goto IL_5A;
				IL_25:
				if (array.Length <= x268d0b7d0d40d0be.x8923f6143c095390)
				{
					goto IL_07;
				}
				array2 = x268d0b7d0d40d0be.x2688d9218ffd4d00(array);
				if (3 == 0)
				{
					goto IL_46;
				}
				if (array2 != null)
				{
					goto IL_19;
				}
				if (!false)
				{
					goto IL_07;
				}
				goto IL_54;
				IL_34:
				memoryStream = null;
				goto IL_25;
				IL_46:
				goto IL_19;
				IL_54:
				if (false)
				{
					goto IL_84;
				}
				if (!false)
				{
					goto IL_34;
				}
				IL_5A:
				IL_68:
				if (255 != 0)
				{
					goto IL_93;
				}
				IL_6F:
				array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				IL_84:
				memoryStream.Read(array, 0, array.Length);
				if (!false)
				{
					memoryStream.Dispose();
					if (false)
					{
						goto IL_46;
					}
					if (false)
					{
						goto IL_68;
					}
					goto IL_54;
				}
				IL_93:
				goto IL_07;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return new byte[0];
		}

		public static T x6a22d00fa826ff39<T>(byte[] xe8e3a0880e7c96ea, int x374ea4fe62468d0f, int x961016a387451f05)
		{
			if (xe8e3a0880e7c96ea.Length == 0)
			{
				return default(T);
			}
			try
			{
				MemoryStream memoryStream = x268d0b7d0d40d0be.x926d6fadbeffece7(xe8e3a0880e7c96ea, x374ea4fe62468d0f, x961016a387451f05);
				memoryStream.Position = 0L;
				T t = Serializer.Deserialize<T>(memoryStream);
				memoryStream.Dispose();
				T result = t;
				if ((uint)x961016a387451f05 >= 0U)
				{
				}
				return result;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return default(T);
		}

		public static byte[] x2688d9218ffd4d00(byte[] xf9a0d04800d70471)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, -1))
				{
					zoutputStream.Write(xf9a0d04800d70471, 0, xf9a0d04800d70471.Length);
					zoutputStream.Flush();
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static MemoryStream x926d6fadbeffece7(byte[] xf9a0d04800d70471, int x374ea4fe62468d0f, int x961016a387451f05)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (xf9a0d04800d70471.Length - x374ea4fe62468d0f >= 2)
			{
				while (120 == xf9a0d04800d70471[x374ea4fe62468d0f])
				{
					if (156 != xf9a0d04800d70471[x374ea4fe62468d0f + 1])
					{
						bool flag = (uint)x374ea4fe62468d0f - (uint)x961016a387451f05 < 0U;
						if (!flag)
						{
							goto IL_2F;
						}
						if (3 != 0)
						{
							break;
						}
						continue;
					}
					IL_08:
					using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream))
					{
						zoutputStream.Write(xf9a0d04800d70471, x374ea4fe62468d0f, x961016a387451f05);
						zoutputStream.Flush();
						return memoryStream;
					}
					IL_2F:
					if (218 == xf9a0d04800d70471[x374ea4fe62468d0f + 1])
					{
						goto IL_08;
					}
					break;
				}
			}
			memoryStream.Write(xf9a0d04800d70471, x374ea4fe62468d0f, x961016a387451f05);
			return memoryStream;
		}

		public static byte[] xde567718ad671ffb(byte[] xcdaeea7afaf570ff)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			for (;;)
			{
				if (num < xcdaeea7afaf570ff.Length)
				{
					IL_111:
					while (xcdaeea7afaf570ff[num] < 240)
					{
						while (xcdaeea7afaf570ff[num] >= 224)
						{
							list.Add((byte)((int)(xcdaeea7afaf570ff[num + 2] & 63) | (int)(xcdaeea7afaf570ff[num + 1] & 3) << 6));
							if (true)
							{
								list.Add((byte)((int)xcdaeea7afaf570ff[num] << 4 | (xcdaeea7afaf570ff[num + 1] & 60) >> 2));
								num += 2;
								if ((uint)num < 0U)
								{
									goto IL_131;
								}
								bool flag = (uint)num + (uint)num > uint.MaxValue;
								if (flag)
								{
									flag = ((uint)num + (uint)num < 0U);
									if (flag)
									{
										continue;
									}
								}
								else
								{
									if (false)
									{
										goto IL_131;
									}
									goto IL_12;
								}
							}
							if (false)
							{
								goto IL_10F;
							}
							goto IL_111;
						}
						if (xcdaeea7afaf570ff[num] >= 192)
						{
							goto IL_33;
						}
						list.Add(xcdaeea7afaf570ff[num]);
						list.Add(0);
						goto IL_12;
					}
					goto IL_10F;
				}
				if (((uint)num & 0U) != 0U)
				{
					goto IL_33;
				}
				break;
				IL_12:
				num++;
				continue;
				IL_33:
				list.Add((byte)((int)(xcdaeea7afaf570ff[num + 1] & 63) | (int)(xcdaeea7afaf570ff[num] & 3) << 6));
				list.Add((byte)((xcdaeea7afaf570ff[num] & 28) >> 2));
				num++;
				goto IL_12;
			}
			goto IL_131;
			IL_10F:
			return null;
			IL_131:
			return list.ToArray();
		}

		public static string xac534c46899d975a(string xb41faee6912a2313)
		{
			try
			{
				if (string.IsNullOrEmpty(xb41faee6912a2313))
				{
					return "";
				}
				byte[] array = new UTF8Encoding().GetBytes(xb41faee6912a2313);
				if (2 == 0 || array.Length > 128)
				{
					array = x268d0b7d0d40d0be.x2688d9218ffd4d00(array);
				}
				return Convert.ToBase64String(array);
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return "";
		}

		public static string x83d869afff19eb5f(string x8fb97c5d5a7b8d4c)
		{
			try
			{
				if (string.IsNullOrEmpty(x8fb97c5d5a7b8d4c) && -2147483648 != 0)
				{
					return "";
				}
				byte[] array = Convert.FromBase64String(x8fb97c5d5a7b8d4c);
				MemoryStream stream = x268d0b7d0d40d0be.x926d6fadbeffece7(array, 0, array.Length);
				StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
				string result;
				if (!false)
				{
					result = streamReader.ReadToEnd();
				}
				return result;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return "";
		}

		private static long x6d991d7c04ed674e = x268d0b7d0d40d0be.xf6fa0d1bb537b9c7("1970-01-01 08:00");

		public static int x8923f6143c095390 = 256;
	}
}
