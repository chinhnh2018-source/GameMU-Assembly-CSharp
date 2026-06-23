using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using HSGameEngine.GameEngine.Network.Protocol;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Tools
{
	public class DataHelper
	{
		static DataHelper()
		{
			byte[] bytes;
			for (;;)
			{
				DataHelper.xd7ead33470b23e79 = 0;
				DataHelper.x6d991d7c04ed674e = DataHelper.ConvertToTicks("1970-01-01 08:00");
				if (false)
				{
					break;
				}
				DataHelper.MinZipBytesSize = 256;
				bytes = BitConverter.GetBytes(1695843216);
				if (!false)
				{
					goto Block_2;
				}
			}
			IL_08:
			int num;
			DataHelper.x57cb46cc62e44481 += bytes[num];
			num++;
			IL_1B:
			if (num >= bytes.Length)
			{
				return;
			}
			goto IL_08;
			Block_2:
			num = 0;
			goto IL_1B;
			goto IL_08;
		}

		public static void CopyBytes(byte[] copyTo, int offsetTo, byte[] copyFrom, int offsetFrom, int count)
		{
			Array.Copy(copyFrom, offsetFrom, copyTo, offsetTo, count);
		}

		public static void SortBytes(byte[] bytesData, int offsetTo, int count)
		{
			byte b = DataHelper.xaa45e3a6f3f182d5 ? DataHelper.xd7ead33470b23e79 : DataHelper.x57cb46cc62e44481;
			int i = offsetTo;
			while (i < offsetTo + count)
			{
				do
				{
					int num = i;
					bytesData[num] ^= b;
					i++;
				}
				while (((uint)b | 255U) == 0U);
			}
		}

		public static byte GenerateEncpyptKey(ushort number)
		{
			byte b = 0;
			uint num = (uint)number;
			uint num2;
			int num3;
			do
			{
				num2 = 362146069U;
				num3 = (int)(number % 13 + 7);
				for (int i = 0; i <= num3; i++)
				{
					num2 = 23765U * (num2 & 64535U) + (num >> 16);
					num = 18434U * (num & 63545U) + (num2 >> 16);
				}
			}
			while (((uint)b | 15U) == 0U);
			uint num4 = (num2 << 16) + num;
			while ((b = (byte)(num4 & 255U)) == 0)
			{
				num4 >>= 3;
				if ((uint)number + (uint)num3 <= 4294967295U)
				{
				}
			}
			return b;
		}

		public static void SetUseKey(byte key)
		{
			DataHelper.xd7ead33470b23e79 = key;
			DataHelper.xaa45e3a6f3f182d5 = true;
		}

		public static ushort GenerateOffsetKey(ushort randKey, ushort baseVal)
		{
			uint num = (uint)((int)randKey << 16 | (int)baseVal);
			uint num2 = 35154409U;
			int num3 = (int)(randKey % 13 + 7);
			uint num4;
			if (num4 - (uint)baseVal >= 0U)
			{
			}
			int i = 0;
			uint num5;
			while (i <= num3)
			{
				num2 = 27265U * (num2 & 24735U) + (num >> 14);
				if (!false)
				{
				}
				num = 14634U * (num & 43505U) + (num2 >> 17);
				i++;
				bool flag = (uint)i - num2 < 0U;
				if (flag)
				{
					IL_04:
					num5 >>= 3;
					IL_0A:
					if (num5 == 0U || (num4 = num5 % 1003U) != 0U)
					{
						return baseVal + (ushort)num4;
					}
					goto IL_04;
				}
			}
			num5 = (num2 << 16) + num;
			num4 = 0U;
			goto IL_0A;
		}

		public static void ClearKey()
		{
			DataHelper.xaa45e3a6f3f182d5 = false;
			DataHelper.xd7ead33470b23e79 = 0;
			SessionData.CmdOffset = 0;
		}

		public static bool CompBytes(byte[] left, byte[] right)
		{
			if (left.Length == right.Length)
			{
				bool flag;
				do
				{
					flag = true;
				}
				while (255 == 0);
				int i = 0;
				bool flag2 = (flag ? 1U : 0U) + (uint)i > uint.MaxValue;
				if (!flag2)
				{
					while (i < left.Length)
					{
						if (left[i] != right[i])
						{
							return false;
						}
						i++;
					}
					flag2 = ((flag ? 1U : 0U) > uint.MaxValue);
					if (flag2)
					{
						return false;
					}
				}
				return flag;
			}
			return false;
		}

		public static void RandBytes(byte[] buffer, int offset, int count)
		{
			long ticks = DateTime.Now.Ticks;
			Random random = new Random((int)(ticks & (long)((ulong)-1)) | (int)(ticks >> 32));
			int i = 0;
			while (i < count)
			{
				buffer[offset + i] = (byte)random.Next(0, 255);
				i++;
				if ((uint)offset > 4294967295U)
				{
					return;
				}
			}
		}

		public static string Bytes2HexString(byte[] b)
		{
			int num = 0;
			int i;
			bool flag = (uint)num - (uint)i > uint.MaxValue;
			if (!flag)
			{
				goto IL_5F;
			}
			IL_1A:
			string text;
			while (i < b.Length)
			{
				text += ((int)(b[i] & byte.MaxValue)).ToString("X2").ToUpper();
				i++;
				if (false)
				{
					goto IL_65;
				}
			}
			flag = ((uint)i > uint.MaxValue);
			if (!flag)
			{
				return text;
			}
			IL_5F:
			text = "";
			IL_65:
			i = 0;
			goto IL_1A;
		}

		public static byte[] HexString2Bytes(string s)
		{
			if (s.Length % 2 != 0)
			{
				return null;
			}
			byte[] array;
			for (;;)
			{
				IL_78:
				for (;;)
				{
					IL_7A:
					array = new byte[s.Length / 2];
					for (int i = 0; i < s.Length / 2; i++)
					{
						string s2 = s.Substring(i * 2, 2);
						int num = int.Parse(s2, NumberStyles.HexNumber) & 255;
						if ((uint)i + (uint)i < 0U)
						{
							goto IL_7A;
						}
						if (((uint)num | 4294967294U) == 0U)
						{
							goto IL_78;
						}
						array[i] = (byte)num;
					}
					return array;
				}
			}
			return array;
		}

		public static void WriteFormatExceptionLog(Exception e, string extMsg, bool showMsgBox, bool finalReport = false)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (;;)
				{
					stringBuilder.AppendFormat("应用程序出现了异常[{0}]:\r\n{1}\r\n", finalReport ? 1 : 0, e.Message);
					stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
					bool flag;
					if (e != null)
					{
						flag = ((finalReport ? 1U : 0U) < 0U);
						if (flag)
						{
							goto IL_E7;
						}
						goto IL_69;
					}
					IL_1D:
					LogManager.WriteException(stringBuilder.ToString());
					if (!showMsgBox)
					{
						break;
					}
					if ((showMsgBox ? 1U : 0U) - (finalReport ? 1U : 0U) > 4294967295U)
					{
						continue;
					}
					Console.WriteLine(stringBuilder.ToString());
					flag = (((showMsgBox ? 1U : 0U) | 8U) == 0U);
					if (flag)
					{
						goto IL_69;
					}
					break;
					IL_0B:
					stringBuilder.AppendFormat("\r\n {0}", e.StackTrace);
					goto IL_1D;
					IL_69:
					if (e.InnerException == null)
					{
						goto IL_0B;
					}
					IL_E7:
					stringBuilder.AppendFormat("\r\n {0}", e.InnerException.Message);
					if (false)
					{
						break;
					}
					if (false)
					{
						goto IL_69;
					}
					if ((showMsgBox ? 1U : 0U) - (finalReport ? 1U : 0U) < 0U)
					{
						goto IL_69;
					}
					if (8 == 0)
					{
						break;
					}
					goto IL_0B;
				}
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static void WriteFormatStackLog(StackTrace stackTrace, string extMsg)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("应   用程序出现了对象锁定超时错误:\r\n", new object[0]);
				stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
				stringBuilder.AppendFormat("\r\n {0}", stackTrace.ToString());
				LogManager.WriteException(stringBuilder.ToString());
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static int ConvertToInt32(string str, int defVal)
		{
			try
			{
				if ("*" != str)
				{
					return Convert.ToInt32(str);
				}
				return defVal;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return defVal;
		}

		public static string ConvertToStr(string str, string defVal)
		{
			if ("*" != str)
			{
				return str;
			}
			return defVal;
		}

		public static long ConvertToTicks(string str, long defVal)
		{
			if ("*" == str)
			{
				return defVal;
			}
			str = str.Replace('$', ':');
			try
			{
				DateTime dateTime;
				if (!DateTime.TryParse(str, out dateTime))
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

		public static long ConvertToTicks(string str)
		{
			try
			{
				DateTime dateTime;
				if (!DateTime.TryParse(str, out dateTime))
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

		public static long UnixSecondsToTicks(int secs)
		{
			return DataHelper.x6d991d7c04ed674e + (long)secs * 1000L;
		}

		public static long UnixSecondsToTicks(string secs)
		{
			int secs2 = Convert.ToInt32(secs);
			return DataHelper.UnixSecondsToTicks(secs2);
		}

		public static int UnixSecondsNow()
		{
			long ticks = DateTime.Now.Ticks / 10000L;
			return DataHelper.SysTicksToUnixSeconds(ticks);
		}

		public static int SysTicksToUnixSeconds(long ticks)
		{
			long num = (ticks - DataHelper.x6d991d7c04ed674e) / 1000L;
			return (int)num;
		}

		public static TCPOutPacket ObjectToTCPOutPacket<T>(T instance, TCPOutPacketPool pool, int cmdID)
		{
			byte[] array = DataHelper.ObjectToBytes<T>(instance);
			return TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, cmdID);
		}

		public static byte[] ObjectToBytes<T>(T instance)
		{
			try
			{
				byte[] array = null;
				if (false)
				{
					goto IL_4B;
				}
				MemoryStream memoryStream;
				if (instance != null)
				{
					while (!(instance is IProtoBuffData))
					{
						if (!false)
						{
							goto IL_AA;
						}
						IL_CA:
						if (false)
						{
							continue;
						}
						if (2 != 0)
						{
							goto IL_6E;
						}
						IL_AA:
						if (3 == 0)
						{
							goto IL_CA;
						}
						if (false)
						{
							goto IL_108;
						}
						IL_6E:
						memoryStream = new MemoryStream();
						do
						{
							Serializer.Serialize<T>(memoryStream, instance);
						}
						while (false);
						array = new byte[memoryStream.Length];
						if (!false)
						{
							memoryStream.Position = 0L;
							goto IL_108;
						}
						goto IL_6E;
						IL_108:
						goto IL_4B;
					}
					return (instance as IProtoBuffData).toBytes();
				}
				array = new byte[0];
				IL_2D:
				if (array.Length > DataHelper.MinZipBytesSize)
				{
					byte[] array2 = DataHelper.Compress(array);
					if (false)
					{
						if (false)
						{
							goto IL_4B;
						}
					}
					else if (array2 == null)
					{
						if (8 == 0)
						{
							goto IL_4B;
						}
						if (15 == 0)
						{
							goto IL_69;
						}
						if (!false)
						{
							goto IL_17;
						}
						goto IL_2B;
					}
					if (array2.Length < array.Length)
					{
						array = array2;
					}
					IL_2B:;
				}
				IL_17:
				byte[] result = array;
				IL_69:
				return result;
				IL_4B:
				memoryStream.Read(array, 0, array.Length);
				memoryStream.Dispose();
				memoryStream = null;
				goto IL_2D;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return new byte[0];
		}

		public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
		{
			if (bytesData.Length == 0)
			{
				return default(T);
			}
			try
			{
				T result;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					if (bytesData.Length - offset < 2)
					{
						goto IL_81;
					}
					IL_6B:
					if (120 != bytesData[offset])
					{
						goto IL_81;
					}
					IL_72:
					if (156 == bytesData[offset + 1] || (!false && 218 == bytesData[offset + 1]))
					{
						using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream))
						{
							zoutputStream.Write(bytesData, offset, length);
							zoutputStream.Flush();
							memoryStream.Position = 0L;
							result = Serializer.Deserialize<T>(memoryStream);
							goto IL_AF;
						}
						goto IL_6B;
					}
					IL_81:
					memoryStream.Write(bytesData, offset, length);
					memoryStream.Position = 0L;
					if ((uint)offset + (uint)offset < 0U)
					{
						goto IL_72;
					}
					result = Serializer.Deserialize<T>(memoryStream);
					IL_AF:;
				}
				return result;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return default(T);
		}

		public static T BytesToObject2<T>(byte[] bytesData, int offset, int length) where T : class, IProtoBuffData, new()
		{
			T result = Activator.CreateInstance<T>();
			try
			{
				result.fromBytes(bytesData, offset, length);
				return result;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return default(T);
		}

		public static byte[] Compress(byte[] bytes)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, -1))
				{
					zoutputStream.Write(bytes, 0, bytes.Length);
					zoutputStream.Flush();
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public static byte[] Uncompress(byte[] bytes)
		{
			if (bytes.Length < 2)
			{
				return bytes;
			}
			while (120 == bytes[0])
			{
				if (156 == bytes[1] || 218 == bytes[1])
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream))
						{
							zoutputStream.Write(bytes, 0, bytes.Length);
							zoutputStream.Flush();
						}
						return memoryStream.ToArray();
					}
					return bytes;
				}
				if (2 != 0)
				{
					return bytes;
				}
			}
			return bytes;
		}

		public static byte[] Utf8_2_Unicode(byte[] input)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			for (;;)
			{
				for (;;)
				{
					if (num >= input.Length)
					{
						goto IL_86;
					}
					if (input[num] >= 240)
					{
						goto Block_8;
					}
					if (input[num] >= 224)
					{
						list.Add((byte)((int)(input[num + 2] & 63) | (int)(input[num + 1] & 3) << 6));
						goto IL_104;
					}
					if (false)
					{
						break;
					}
					bool flag;
					if (2 != 0 && input[num] < 192)
					{
						list.Add(input[num]);
						flag = ((uint)num + (uint)num < 0U);
						if (flag)
						{
							goto IL_86;
						}
						list.Add(0);
					}
					else
					{
						list.Add((byte)((int)(input[num + 1] & 63) | (int)(input[num] & 3) << 6));
						list.Add((byte)((input[num] & 28) >> 2));
						num++;
					}
					IL_12:
					num++;
					break;
					IL_A7:
					if ((uint)num - (uint)num <= 4294967295U)
					{
						if ((uint)num - (uint)num <= 4294967295U)
						{
							goto IL_12;
						}
						break;
					}
					IL_104:
					list.Add((byte)((int)input[num] << 4 | (input[num + 1] & 60) >> 2));
					num += 2;
					goto IL_A7;
					IL_86:
					flag = (((uint)num | 1U) == 0U);
					if (!flag)
					{
						goto IL_13C;
					}
					if (false)
					{
						goto IL_A7;
					}
				}
			}
			Block_8:
			return null;
			IL_13C:
			return list.ToArray();
		}

		public static string ZipStringToBase64(string text)
		{
			try
			{
				if (!string.IsNullOrEmpty(text))
				{
					byte[] array = new UTF8Encoding().GetBytes(text);
					string result;
					if (-2 != 0)
					{
						do
						{
							if (array.Length <= 128)
							{
								if (!false)
								{
									break;
								}
							}
							else
							{
								array = DataHelper.Compress(array);
							}
						}
						while (false);
						result = Convert.ToBase64String(array);
						if (false)
						{
							goto IL_33;
						}
					}
					return result;
				}
				IL_33:
				return "";
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return "";
		}

		public static string UnZipStringToBase64(string base64)
		{
			try
			{
				if (string.IsNullOrEmpty(base64))
				{
					return "";
				}
				byte[] array = Convert.FromBase64String(base64);
				string @string;
				do
				{
					array = DataHelper.Uncompress(array);
					@string = new UTF8Encoding().GetString(array, 0, array.Length);
				}
				while (4 == 0);
				return @string;
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
			return "";
		}

		private static byte x57cb46cc62e44481 = 0;

		private static bool xaa45e3a6f3f182d5 = false;

		private static byte xd7ead33470b23e79;

		private static long x6d991d7c04ed674e;

		public static int MinZipBytesSize;
	}
}
