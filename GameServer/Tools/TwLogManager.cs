using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using GameServer.Logic;
using Server.Tools;

namespace GameServer.Tools
{
	public class TwLogManager
	{
		public static void WriteLog(TwLogType logType, string logMsg)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (TwLogManager._FileLock)
			{
				LogFilePoolItem logFilePoolItem;
				if (!TwLogManager.LogFilePoolList.TryGetValue(logType, out logFilePoolItem))
				{
					logFilePoolItem = new LogFilePoolItem();
					TwLogManager.LogFilePoolList.Add(logType, logFilePoolItem);
				}
				if ((long)dateTime.Hour != logFilePoolItem.OpenTimeOnHours || (long)dateTime.DayOfYear != logFilePoolItem.OpenTimeOnDayOfYear || logFilePoolItem._StreamWriter == null)
				{
					if (null != logFilePoolItem._StreamWriter)
					{
						logFilePoolItem._StreamWriter.Close();
						logFilePoolItem._StreamWriter = null;
					}
					logFilePoolItem._StreamWriter = File.AppendText(TwLogManager.GetLogPath() + TwLogManager.GetFileName(logType));
					logFilePoolItem.OpenTimeOnHours = (long)dateTime.Hour;
					logFilePoolItem.OpenTimeOnDayOfYear = (long)dateTime.DayOfYear;
					logFilePoolItem._StreamWriter.AutoFlush = true;
				}
				try
				{
					TwLogManager._StreamWriter = logFilePoolItem._StreamWriter;
					TwLogManager._StreamWriter.WriteLine(logMsg);
				}
				catch
				{
				}
			}
		}

		private static string GetLogPath()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (TwLogManager._PathLock)
			{
				string text = dateTime.ToString("yyyy");
				string text2 = dateTime.ToString("MM");
				string text3 = dateTime.ToString("dd");
				if (TwLogManager._TwLogPath == string.Empty || text != TwLogManager._YearID || text2 != TwLogManager._MonthID || text3 != TwLogManager._DayID)
				{
					TwLogManager._YearID = text;
					TwLogManager._MonthID = text2;
					TwLogManager._DayID = text3;
					try
					{
						TwLogManager.GetRootPath();
						TwLogManager._TwLogPath = string.Format("{0}\\{1}{2}\\", TwLogManager._RootPath, TwLogManager._YearID, TwLogManager._MonthID);
						if (!Directory.Exists(TwLogManager._TwLogPath))
						{
							Directory.CreateDirectory(TwLogManager._TwLogPath);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return TwLogManager._TwLogPath;
		}

		private static string GetFileName(TwLogType type)
		{
			string result = "";
			switch (type)
			{
			case TwLogType.RoleCreate:
				result = string.Format("active_{0}_{1}_{2}{3}{4}.log", new object[]
				{
					TwLogManager.GetProductID(),
					TwLogManager.GetServerID(),
					TwLogManager._YearID,
					TwLogManager._MonthID,
					TwLogManager._DayID
				});
				break;
			case TwLogType.RoleLogin:
				result = string.Format("login_{0}_{1}_{2}{3}{4}.log", new object[]
				{
					TwLogManager.GetProductID(),
					TwLogManager.GetServerID(),
					TwLogManager._YearID,
					TwLogManager._MonthID,
					TwLogManager._DayID
				});
				break;
			case TwLogType.OnlineNum:
				result = string.Format("online_{0}_{1}_{2}{3}{4}.log", new object[]
				{
					TwLogManager.GetProductID(),
					TwLogManager.GetServerID(),
					TwLogManager._YearID,
					TwLogManager._MonthID,
					TwLogManager._DayID
				});
				break;
			}
			return result;
		}

		public static string GetProductID()
		{
			string productID;
			if (TwLogManager._ProductID != "")
			{
				productID = TwLogManager._ProductID;
			}
			else
			{
				TwLogManager._ProductID = GameManager.PlatConfigMgr.GetGameConfigItemStr("tw_log_pid", "0");
				productID = TwLogManager._ProductID;
			}
			return productID;
		}

		public static string GetServerID()
		{
			string serverID;
			if (TwLogManager._ServerID != "")
			{
				serverID = TwLogManager._ServerID;
			}
			else
			{
				TwLogManager._ServerID = string.Format("{0}{1:000}", TwLogManager.GetProductID(), GameManager.ServerId);
				serverID = TwLogManager._ServerID;
			}
			return serverID;
		}

		public static string GetUserID(string id)
		{
			if (TwLogManager._userHeadArr == null || TwLogManager._userHeadArr.Length <= 0)
			{
				string gameConfigItemStr = GameManager.PlatConfigMgr.GetGameConfigItemStr("tw_log_head", "GATGoogle,GATAn,GAT,TG,YN");
				TwLogManager._userHeadArr = gameConfigItemStr.Split(new char[]
				{
					','
				});
			}
			foreach (string text in TwLogManager._userHeadArr)
			{
				if (id.Length > text.Length && id.Substring(0, text.Length) == text)
				{
					return id.Substring(text.Length);
				}
			}
			return id;
		}

		public static string GetRootPath()
		{
			string rootPath;
			if (TwLogManager._RootPath != "")
			{
				rootPath = TwLogManager._RootPath;
			}
			else
			{
				TwLogManager._RootPath = GameManager.PlatConfigMgr.GetGameConfigItemStr("tw_log_path", "d:\\data\\syslog\\platformlog");
				rootPath = TwLogManager._RootPath;
			}
			return rootPath;
		}

		public static void ScanLog()
		{
			string productID = TwLogManager.GetProductID();
			if (!(productID == "") && !(productID == "0"))
			{
				long num = TimeUtil.NOW();
				if (num - TwLogManager.LastScanTicks >= 600000L)
				{
					TwLogManager.LastScanTicks = num;
					string text = TimeUtil.ConvertDateTimeInt(TimeUtil.NowDateTime()).ToString();
					string text2 = TwLogManager.GetOnLineNum().ToString();
					string serverID = TwLogManager.GetServerID();
					string logMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", new object[]
					{
						text,
						text2,
						text2,
						productID,
						serverID
					});
					TwLogManager.WriteLog(TwLogType.OnlineNum, logMsg);
				}
			}
		}

		private static int GetOnLineNum()
		{
			int result = 0;
			string[] array = Global.ExecuteDBCmd(10063, string.Format("{0}", 0), 0);
			if (array != null || array.Length >= 1)
			{
				result = Global.SafeConvertToInt32(array[0]);
			}
			return result;
		}

		private static string _YearID = string.Empty;

		private static string _MonthID = string.Empty;

		private static string _DayID = string.Empty;

		private static int _HourID = -1;

		private static string _ProductID = "";

		private static string _ServerID = "";

		private static string _RootPath = "";

		private static string _TwLogPath = string.Empty;

		private static object _PathLock = new object();

		private static object _FileLock = new object();

		private static StreamWriter _StreamWriter = null;

		public static Dictionary<TwLogType, LogFilePoolItem> LogFilePoolList = new Dictionary<TwLogType, LogFilePoolItem>();

		private static string[] _userHeadArr = null;

		private static long LastScanTicks = TimeUtil.NOW();
	}
}
