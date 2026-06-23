using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using ProtoBuf;
using Server.Protocol;

namespace Server.Tools
{
	public class DataHelper
	{
		static DataHelper()
		{
			byte[] bytes = BitConverter.GetBytes(1695843216);
			for (int i = 0; i < bytes.Length; i++)
			{
				DataHelper.SortKey += bytes[i];
			}
			ulong num = (ulong)DataHelper.SortKey;
			DataHelper.SortKey64 |= num;
			DataHelper.SortKey64 |= num << 8;
			DataHelper.SortKey64 |= num << 16;
			DataHelper.SortKey64 |= num << 24;
			DataHelper.SortKey64 |= num << 32;
			DataHelper.SortKey64 |= num << 40;
			DataHelper.SortKey64 |= num << 48;
			DataHelper.SortKey64 |= num << 56;
		}

		public static void CopyBytes(byte[] copyTo, int offsetTo, byte[] copyFrom, int offsetFrom, int count)
		{
			Array.Copy(copyFrom, offsetFrom, copyTo, offsetTo, count);
		}

		public unsafe static void SortBytes(byte[] bytesData, int offsetTo, int count)
		{
			if (count <= 32)
			{
				int num = offsetTo + count;
				for (int i = offsetTo; i < num; i++)
				{
					int num2 = i;
					bytesData[num2] ^= DataHelper.SortKey;
				}
			}
			else
			{
				int num3 = count / 8;
				fixed (byte* ptr = &bytesData[offsetTo])
				{
					ulong* ptr2 = (ulong*)ptr;
					for (int j = 0; j < num3; j++)
					{
						ptr2[j] ^= DataHelper.SortKey64;
					}
				}
				int num = offsetTo + count;
				for (int i = offsetTo + num3 * 8; i < num; i++)
				{
					int num4 = i;
					bytesData[num4] ^= DataHelper.SortKey;
				}
			}
		}

		public static bool CompBytes(byte[] left, byte[] right)
		{
			bool result;
			if (left.Length != right.Length)
			{
				result = false;
			}
			else
			{
				bool flag = true;
				for (int i = 0; i < left.Length; i++)
				{
					if (left[i] != right[i])
					{
						flag = false;
						break;
					}
				}
				result = flag;
			}
			return result;
		}

		public static void RandBytes(byte[] buffer, int offset, int count)
		{
			long ticks = DateTime.Now.Ticks;
			Random random = new Random((int)(ticks & (long)((ulong)-1)) | (int)(ticks >> 32));
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = (byte)random.Next(0, 255);
			}
		}

		public static string Bytes2HexString(byte[] b)
		{
			string text = "";
			for (int i = 0; i < b.Length; i++)
			{
				text += ((int)(b[i] & byte.MaxValue)).ToString("X2").ToUpper();
			}
			return text;
		}

		public static byte[] HexString2Bytes(string s)
		{
			byte[] result;
			if (s.Length % 2 != 0)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[s.Length / 2];
				for (int i = 0; i < s.Length / 2; i++)
				{
					string s2 = s.Substring(i * 2, 2);
					int num = int.Parse(s2, NumberStyles.HexNumber) & 255;
					array[i] = (byte)num;
				}
				result = array;
			}
			return result;
		}

		public static void WriteFormatExceptionLog(Exception e, string extMsg, bool showMsgBox, bool finalReport = false)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("应用程序出现了异常[{0}]:\r\n{1}\r\n", finalReport ? 1 : 0, e.Message);
				stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
				if (null != e)
				{
					if (e.InnerException != null)
					{
						stringBuilder.AppendFormat("\r\n {0}", e.InnerException.Message);
					}
					stringBuilder.AppendFormat("\r\n {0}", e.StackTrace);
				}
				LogManager.WriteException(stringBuilder.ToString());
				if (showMsgBox)
				{
					Console.WriteLine(stringBuilder.ToString());
				}
			}
			catch (Exception)
			{
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
			catch (Exception)
			{
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
			catch (Exception)
			{
			}
			return defVal;
		}

		public static long ConvertToInt64(string str, long defVal)
		{
			try
			{
				if ("*" != str)
				{
					return Convert.ToInt64(str);
				}
				return defVal;
			}
			catch (Exception)
			{
			}
			return defVal;
		}

		public static double ConvertToDouble(string str, double defVal)
		{
			try
			{
				if ("*" != str)
				{
					return Convert.ToDouble(str);
				}
				return defVal;
			}
			catch (Exception)
			{
			}
			return defVal;
		}

		public static string ConvertToStr(string str, string defVal)
		{
			string result;
			if ("*" != str)
			{
				result = str;
			}
			else
			{
				result = defVal;
			}
			return result;
		}

		public static long ConvertToTicks(string str, long defVal)
		{
			long result;
			if ("*" == str)
			{
				result = defVal;
			}
			else
			{
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
				catch (Exception)
				{
				}
				result = 0L;
			}
			return result;
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
			catch (Exception)
			{
			}
			return 0L;
		}

		public static long UnixSecondsToTicks(int secs)
		{
			return DataHelper.UnixStartTicks + (long)secs * 1000L;
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
			long num = (ticks - DataHelper.UnixStartTicks) / 1000L;
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
				byte[] array;
				if (null == instance)
				{
					array = new byte[0];
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					Serializer.Serialize<T>(memoryStream, instance);
					array = new byte[memoryStream.Length];
					memoryStream.Position = 0L;
					memoryStream.Read(array, 0, array.Length);
					memoryStream.Dispose();
				}
				if (array.Length > DataHelper.MinZipBytesSize)
				{
					byte[] array2 = DataHelper.Compress(array);
					if (null != array2)
					{
						if (array2.Length < array.Length)
						{
							array = array2;
						}
					}
				}
				return array;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return new byte[0];
		}

		public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
		{
			T result;
			if (bytesData.Length == 0)
			{
				result = default(T);
			}
			else
			{
				try
				{
					byte[] array = new byte[length];
					DataHelper.CopyBytes(array, 0, bytesData, offset, length);
					array = DataHelper.Uncompress(array);
					MemoryStream memoryStream = new MemoryStream();
					memoryStream.Write(array, 0, array.Length);
					memoryStream.Position = 0L;
					T result2 = Serializer.Deserialize<T>(memoryStream);
					memoryStream.Dispose();
					return result2;
				}
				catch (Exception ex)
				{
				}
				result = default(T);
			}
			return result;
		}

		public static string ObjectToHexString<T>(T instance)
		{
			byte[] array = DataHelper.ObjectToBytes<T>(instance);
			string result;
			if (array == null || array.Length == 0)
			{
				result = "null";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(array.Length * 2 + 2);
				stringBuilder.Append("0x");
				foreach (byte b in array)
				{
					stringBuilder.Append(b.ToString("X2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
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
			byte[] result;
			if (bytes.Length < 2)
			{
				result = bytes;
			}
			else if (120 != bytes[0])
			{
				result = bytes;
			}
			else if (156 != bytes[1] && 218 != bytes[1])
			{
				result = bytes;
			}
			else
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream))
					{
						zoutputStream.Write(bytes, 0, bytes.Length);
						zoutputStream.Flush();
					}
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public static byte[] Utf8_2_Unicode(byte[] input)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] >= 240)
				{
					return null;
				}
				if (input[i] >= 224)
				{
					list.Add((byte)((int)(input[i + 2] & 63) | (int)(input[i + 1] & 3) << 6));
					list.Add((byte)((int)input[i] << 4 | (input[i + 1] & 60) >> 2));
					i += 2;
				}
				else if (input[i] >= 192)
				{
					list.Add((byte)((int)(input[i + 1] & 63) | (int)(input[i] & 3) << 6));
					list.Add((byte)((input[i] & 28) >> 2));
					i++;
				}
				else
				{
					list.Add(input[i]);
					list.Add(0);
				}
			}
			return list.ToArray();
		}

		public static string ZipStringToBase64(string text)
		{
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					return "";
				}
				byte[] array = new UTF8Encoding().GetBytes(text);
				if (array.Length > 128)
				{
					array = DataHelper.Compress(array);
				}
				return Convert.ToBase64String(array);
			}
			catch (Exception)
			{
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
				array = DataHelper.Uncompress(array);
				return new UTF8Encoding().GetString(array, 0, array.Length);
			}
			catch (Exception)
			{
			}
			return "";
		}

		private static byte SortKey = 0;

		private static ulong SortKey64 = 0UL;

		private static long UnixStartTicks = DataHelper.ConvertToTicks("1970-01-01 08:00");

		public static int MinZipBytesSize = 65536;
	}
}
