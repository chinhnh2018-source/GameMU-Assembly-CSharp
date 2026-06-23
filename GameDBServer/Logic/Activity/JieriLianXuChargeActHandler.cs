using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	internal class JieriLianXuChargeActHandler : SingletonTemplate<JieriLianXuChargeActHandler>
	{
		private JieriLianXuChargeActHandler()
		{
		}

		public TCPProcessCmdResults ProcQueryActInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int zoneid = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace('$', ':');
				string toDate = array[3].Replace('$', ':');
				List<int> list = new List<int>();
				string[] array2 = array[4].Split(new char[]
				{
					'_'
				});
				foreach (string value in array2)
				{
					if (!string.IsNullOrEmpty(value))
					{
						list.Add(Convert.ToInt32(value));
					}
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				StringBuilder stringBuilder = new StringBuilder(256);
				lock (dbuserInfo)
				{
					Dictionary<string, int> dictionary = this._QueryEachDayChargeYB(dbMgr, fromDate, toDate, dbuserInfo.UserID, zoneid);
					if (dictionary != null && dictionary.Count > 0)
					{
						foreach (KeyValuePair<string, int> keyValuePair in dictionary)
						{
							stringBuilder.Append(keyValuePair.Key).Append(',').Append(keyValuePair.Value).Append('$');
						}
					}
					stringBuilder.Append(':');
					Dictionary<int, int> dictionary2 = this._QueryEachAwardIdFlag(dbMgr, fromDate, toDate, dbuserInfo.UserID, dbroleInfo.ZoneID, array[4].Split(new char[]
					{
						'_'
					}));
					if (dictionary2 != null && dictionary2.Count > 0)
					{
						foreach (KeyValuePair<int, int> keyValuePair2 in dictionary2)
						{
							stringBuilder.Append(keyValuePair2.Key).Append(',').Append(keyValuePair2.Value).Append('$');
						}
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, stringBuilder.ToString(), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcUpdateAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int awardId = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace('$', ':');
				string toDate = array[3].Replace('$', ':');
				int awardFlag = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				int num2 = 1;
				lock (dbuserInfo)
				{
					num2 = this._UpdateAwardFlag(dbMgr, dbroleInfo.UserID, fromDate, toDate, dbroleInfo.ZoneID, awardId, awardFlag);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", num2), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private string _GetActAwardKey_NoZoneID(string fromDate, string toDate, int zoneId, int awardId)
		{
			return Global.GetHuoDongKeyString(fromDate, toDate) + "_" + awardId.ToString();
		}

		private string _GetActAwardKey_Ex(string fromDate, string toDate, int zoneId, int awardId)
		{
			return string.Concat(new string[]
			{
				Global.GetHuoDongKeyString(fromDate, toDate),
				"_",
				zoneId.ToString(),
				"_",
				awardId.ToString()
			});
		}

		private bool _GetAwardIdByExtKey(string extKey, out int awardId)
		{
			awardId = 0;
			bool result;
			if (string.IsNullOrEmpty(extKey))
			{
				result = false;
			}
			else
			{
				string[] array = extKey.Split(new char[]
				{
					'_'
				});
				result = (array != null && array.Length >= 3 && int.TryParse(array[array.Length - 1], out awardId));
			}
			return result;
		}

		private Dictionary<int, int> _QueryEachAwardIdFlag(DBManager dbMgr, string fromDate, string toDate, string userid, int zoneId, string[] AwardIdArray)
		{
			Dictionary<int, int> result;
			if (dbMgr == null || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) || string.IsNullOrEmpty(userid) || AwardIdArray == null || AwardIdArray.Length == 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				MySQLConnection mySQLConnection = null;
				try
				{
					string text = string.Format("SELECT keystr,hasgettimes FROM t_huodongawarduserhist WHERE userid='{0}' AND activitytype={1} AND keystr LIKE '{2}%'", userid, 61, Global.GetHuoDongKeyString(fromDate, toDate));
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					try
					{
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						while (mySQLDataReader.Read())
						{
							string extKey = mySQLDataReader["keystr"].ToString();
							int num = Convert.ToInt32(mySQLDataReader["hasgettimes"].ToString());
							int key = 0;
							if (this._GetAwardIdByExtKey(extKey, out key))
							{
								int num2;
								if (!dictionary.TryGetValue(key, out num2))
								{
									dictionary[key] = num;
								}
								else
								{
									dictionary[key] = (num2 | num);
								}
							}
						}
					}
					catch (MySQLException)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("读取数据库失败: {0}", text), null, true);
					}
					mySQLCommand.Dispose();
					mySQLCommand = null;
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = dictionary;
			}
			return result;
		}

		private Dictionary<string, int> _QueryEachDayChargeYB(DBManager dbMgr, string fromDate, string toDate, string userid, int zoneid)
		{
			Dictionary<string, int> result;
			if (dbMgr == null || string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate) || string.IsNullOrEmpty(userid))
			{
				result = null;
			}
			else
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				MySQLConnection mySQLConnection = null;
				try
				{
					string text = string.Format("SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND result='success' GROUP BY inputdate UNION ALL  SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog2 WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND result='success' GROUP BY inputdate", userid, fromDate, toDate);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					try
					{
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						while (mySQLDataReader.Read())
						{
							string text2 = mySQLDataReader["inputdate"].ToString();
							int money = Convert.ToInt32(mySQLDataReader["total"].ToString());
							int num = Global.TransMoneyToYuanBao(money);
							if (!dictionary.ContainsKey(text2))
							{
								dictionary.Add(text2, num);
							}
							else
							{
								Dictionary<string, int> dictionary2;
								string key;
								(dictionary2 = dictionary)[key = text2] = dictionary2[key] + num;
							}
						}
					}
					catch (MySQLException)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("读取数据库失败: {0}", text), null, true);
					}
					mySQLCommand.Dispose();
					mySQLCommand = null;
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = dictionary;
			}
			return result;
		}

		private int _UpdateAwardFlag(DBManager dbMgr, string userid, string fromDate, string toDate, int zoneId, int awardId, int awardFlag)
		{
			string empty = string.Empty;
			int num = 0;
			int result = 1;
			string keystr = this._GetActAwardKey_NoZoneID(fromDate, toDate, zoneId, awardId);
			if (DBQuery.GetAwardHistoryForUser(dbMgr, userid, 61, keystr, out num, out empty) < 0)
			{
				if (DBWriter.AddHongDongAwardRecordForUser(dbMgr, userid, 61, keystr, (long)awardFlag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) < 0)
				{
					result = -1008;
				}
			}
			else if (DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, userid, 61, keystr, (long)awardFlag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) < 0)
			{
			}
			return result;
		}
	}
}
