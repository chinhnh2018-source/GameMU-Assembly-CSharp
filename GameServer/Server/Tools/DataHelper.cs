using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using GameServer.Core.Executor;
using GameServer.Tools;
using ProtoBuf;
using Server.Protocol;
using Tmsk.Contract;

namespace Server.Tools
{
	public class DataHelper
	{
		static DataHelper()
		{
			DataHelper.CurrentDirectory = Directory.GetCurrentDirectory() + "\\";
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

		public unsafe static void SortBytes(byte[] bytesData, int offsetTo, int count, ulong ulKey)
		{
			byte b = (byte)ulKey;
			if (count <= 32)
			{
				int num = offsetTo + count;
				for (int i = offsetTo; i < num; i++)
				{
					int num2 = i;
					bytesData[num2] ^= b;
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
						ptr2[j] ^= ulKey;
					}
				}
				int num = offsetTo + count;
				for (int i = offsetTo + num3 * 8; i < num; i++)
				{
					int num4 = i;
					bytesData[num4] ^= b;
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

		public static bool CompBytes(byte[] left, byte[] right, int count)
		{
			bool result;
			if (left.Length < count || right.Length < count)
			{
				result = false;
			}
			else
			{
				bool flag = true;
				for (int i = 0; i < count; i++)
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
			long num = TimeUtil.NOW() * 10000L;
			Random random = new Random((int)(num & (long)((ulong)-1)) | (int)(num >> 32));
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
					SysConOut.WriteLine(stringBuilder.ToString());
				}
			}
			catch (Exception)
			{
			}
		}

		public static void WriteExceptionLogEx(Exception ex, string extMsg)
		{
			try
			{
				StackTrace stackTrace = new StackTrace(2, true);
				string text = string.Format("{0}\r\n{1}\r\n{2}", extMsg, ex.ToString(), stackTrace.ToString());
				LogManager.WriteException(text.ToString());
			}
			catch (Exception)
			{
			}
		}

		public static void WriteStackTraceLog(string extMsg)
		{
			try
			{
				StackTrace stackTrace = new StackTrace(1, true);
				string text = string.Format("{0}\r\n{1}", extMsg, stackTrace.ToString());
				LogManager.WriteException(text);
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
			long ticks = TimeUtil.NowRealTime();
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
					if (instance is IProtoBuffData)
					{
						return (instance as IProtoBuffData).toBytes();
					}
					TMSKThreadStaticClass instance2 = TMSKThreadStaticClass.GetInstance();
					MemoryStream memoryStream = instance2.PopMemoryStream();
					Serializer.Serialize<T>(memoryStream, instance);
					array = new byte[memoryStream.Length];
					memoryStream.Position = 0L;
					memoryStream.Read(array, 0, array.Length);
					instance2.PushMemoryStream(memoryStream);
				}
				if (array.Length > DataHelper.MinZipBytesSize && instance is ICompressed)
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
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "将对象转为字节流发生异常:");
			}
			return new byte[0];
		}

		public static T BytesToObject2<T>(byte[] bytesData, int offset, int length, Socket socket) where T : class, IProtoBuffData, new()
		{
			T result = Activator.CreateInstance<T>();
			try
			{
				result.fromBytes(bytesData, offset, length);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(7, string.Format("解析客户端发上来的数据{0}异常,IP:{1},数据内容：{2}", result.ToString(), socket.RemoteEndPoint.ToString(), Convert.ToBase64String(bytesData, offset, length)), null, true);
			}
			return default(T);
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
					TMSKThreadStaticClass instance = TMSKThreadStaticClass.GetInstance();
					MemoryStream memoryStream = instance.PopMemoryStream();
					memoryStream.Write(array, 0, array.Length);
					memoryStream.Position = 0L;
					T result2 = Serializer.Deserialize<T>(memoryStream);
					instance.PushMemoryStream(memoryStream);
					return result2;
				}
				catch (Exception ex)
				{
					DataHelper.WriteExceptionLogEx(ex, "将字节数据转为对象发生异常:");
				}
				result = default(T);
			}
			return result;
		}

		public static byte[] Compress(byte[] bytes)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream, 9))
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

		public static string EncodeBase64(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				str = "null";
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(bytes);
		}

		public static string DecodeBase64(string base64Str)
		{
			try
			{
				if (!string.IsNullOrEmpty(base64Str))
				{
					byte[] bytes = Convert.FromBase64String(base64Str);
					return Encoding.UTF8.GetString(bytes);
				}
			}
			catch
			{
			}
			return null;
		}

		public static double GetOffsetSecond(DateTime date)
		{
			return (date - DataHelper.StartDate).TotalSeconds;
		}

		public static int GetOffsetDay(DateTime now)
		{
			return (int)(now - DataHelper.StartDate).TotalDays;
		}

		public static int GetOffsetDayNow()
		{
			return DataHelper.GetOffsetDay(TimeUtil.NowDateTime());
		}

		public static DateTime GetRealDate(int day)
		{
			return DataHelper.StartDate.AddDays((double)day);
		}

		public static double CaleTwoLongTimeDay(long First, long Second)
		{
			double result;
			if (First > Second)
			{
				result = Math.Round((double)((First - Second) / 86400000L), 4);
			}
			else
			{
				result = Math.Round((double)((Second - First) / 86400000L), 4);
			}
			return result;
		}

		public static byte SortKey = 0;

		public static ulong SortKey64 = 0UL;

		public static int MinZipBytesSize = 1024;

		public static string CurrentDirectory;

		private static long UnixStartTicks = TimeUtil.Before1970Ticks;

		private static DateTime StartDate = new DateTime(2011, 11, 11);
	}
}
