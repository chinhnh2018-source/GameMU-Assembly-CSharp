using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Server.Tools
{
	public class LogManager
	{
		public static LogTypes LogTypeToWrite { get; set; }

		public static string LogPath
		{
			get
			{
				lock (LogManager.x2058f3ef9d3fd4fe)
				{
					if (LogManager._x0a747a20f8e974a7 == string.Empty)
					{
						LogManager._x0a747a20f8e974a7 = AppDomain.CurrentDomain.BaseDirectory + "log/";
						if (!Directory.Exists(LogManager._x0a747a20f8e974a7))
						{
							Directory.CreateDirectory(LogManager._x0a747a20f8e974a7);
							if (!false)
							{
							}
						}
					}
				}
				return LogManager._x0a747a20f8e974a7;
			}
			set
			{
				lock (LogManager.x2058f3ef9d3fd4fe)
				{
					LogManager._x0a747a20f8e974a7 = value;
					goto IL_2A;
				}
				IL_1D:
				Directory.CreateDirectory(LogManager._x0a747a20f8e974a7);
				return;
				IL_2A:
				if (!Directory.Exists(LogManager._x0a747a20f8e974a7))
				{
					goto IL_1D;
				}
			}
		}

		public static string ExceptionPath
		{
			get
			{
				lock (LogManager.x2058f3ef9d3fd4fe)
				{
					if (LogManager._x480310ebecfeb36c == string.Empty)
					{
						if (2 != 0)
						{
						}
						LogManager._x480310ebecfeb36c = AppDomain.CurrentDomain.BaseDirectory + "Exception/";
						while (!Directory.Exists(LogManager._x480310ebecfeb36c))
						{
							Directory.CreateDirectory(LogManager._x480310ebecfeb36c);
							if (!false)
							{
								break;
							}
						}
					}
				}
				return LogManager._x480310ebecfeb36c;
			}
			set
			{
				lock (LogManager.x2058f3ef9d3fd4fe)
				{
					LogManager._x480310ebecfeb36c = value;
					goto IL_2A;
				}
				IL_1D:
				Directory.CreateDirectory(LogManager._x480310ebecfeb36c);
				return;
				IL_2A:
				if (!Directory.Exists(LogManager._x480310ebecfeb36c))
				{
					goto IL_1D;
				}
			}
		}

		private static void xb1fec8b750b90eec(string x668db1d4f1f7b6a9, string xc8b9fb117615f48b)
		{
			try
			{
				string[] array = new string[5];
				do
				{
					array[0] = LogManager.LogPath;
					if (!false)
					{
						array[1] = x668db1d4f1f7b6a9;
						array[2] = "_";
						if (15 != 0)
						{
							array[3] = DateTime.Now.ToString("yyyyMMdd");
							array[4] = ".log";
							StreamWriter streamWriter = File.AppendText(string.Concat(array));
							string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + xc8b9fb117615f48b;
							bool enableDbgView = LogManager.EnableDbgView;
							streamWriter.WriteLine(value);
							streamWriter.Close();
						}
					}
				}
				while (2147483647 == 0);
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		private static void _x89a5deb553f41e77(string x513b1e7e37ef3829)
		{
			try
			{
				StreamWriter streamWriter = File.CreateText(LogManager.ExceptionPath + "Exception_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
				streamWriter.WriteLine(x513b1e7e37ef3829);
				streamWriter.Close();
			}
			catch (Exception exception)
			{
				DebugTextLog.LogException(exception);
			}
		}

		public static void WriteLog(LogTypes logType, string logMsg)
		{
			if (logType < LogManager.LogTypeToWrite)
			{
				return;
			}
			lock (LogManager.x2058f3ef9d3fd4fe)
			{
				LogManager.xb1fec8b750b90eec(logType.ToString(), logMsg);
			}
		}

		public static void WriteException(string exceptionMsg)
		{
			lock (LogManager.x2058f3ef9d3fd4fe)
			{
				LogManager._x89a5deb553f41e77(exceptionMsg);
			}
		}

		public static bool EnableDbgView = false;

		private static string _x0a747a20f8e974a7 = string.Empty;

		private static string _x480310ebecfeb36c = string.Empty;

		private static object x2058f3ef9d3fd4fe = new object();

		[CompilerGenerated]
		private static LogTypes x3ba28682d60acab9;
	}
}
