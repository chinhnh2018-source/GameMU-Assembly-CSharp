using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Activity.SevenDay
{
	public class SevenDayActivityManager : SingletonTemplate<SevenDayActivityManager>, IManager, ICmdProcessor
	{
		private SevenDayActivityManager()
		{
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13220, SingletonTemplate<SevenDayActivityManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13221, SingletonTemplate<SevenDayActivityManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13222, SingletonTemplate<SevenDayActivityManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 13220)
			{
				this.HandleUpdate(client, nID, cmdParams, count);
			}
			else if (nID == 13221)
			{
				this.HandleClear(client, nID, cmdParams, count);
			}
			else if (nID == 13222)
			{
				this.HandleQueryCharge(client, nID, cmdParams, count);
			}
		}

		private void HandleQueryCharge(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] array = text.Split(new char[]
			{
				':'
			});
			if (array.Length != 3)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("七日充值，找不到玩家 roleid={0}", num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					Dictionary<string, int> cmdData = this._QueryEachDayChargeYB(DBManager.getInstance(), fromDate, toDate, dbroleInfo.UserID, dbroleInfo.ZoneID);
					client.sendCmd<Dictionary<string, int>>(nID, cmdData);
				}
			}
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
					string text = string.Format("SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND zoneid={3} AND result='success' GROUP BY inputdate UNION ALL  SELECT SUM(amount) as total,DATE_FORMAT(inputtime,'%Y-%m-%d') as inputdate FROM t_inputlog2 WHERE u='{0}' AND inputtime >='{1}' AND inputtime <= '{2}' AND zoneid={3} AND result='success' GROUP BY inputdate", new object[]
					{
						userid,
						fromDate,
						toDate,
						zoneid
					});
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					try
					{
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						while (mySQLDataReader.Read())
						{
							string text2 = mySQLDataReader["inputdate"].ToString();
							int num = Convert.ToInt32(mySQLDataReader["total"].ToString());
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

		private void HandleClear(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			try
			{
				int num = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref num);
				if (dbroleInfo == null)
				{
					throw new Exception("SevenDayActivityManager.HandleClear not Find DBRoleInfo, roleid=" + num);
				}
				string text = string.Format("DELETE FROM t_seven_day_act where roleid={0}", num);
				if (!this.ExecNonQuery(text))
				{
					throw new Exception("SevenDayActivityManager.HandleClear ExecSql Failed, sql= " + text);
				}
				lock (dbroleInfo.SevenDayActDict)
				{
					dbroleInfo.SevenDayActDict.Clear();
				}
				cmdData = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.Message, null, true);
				cmdData = false;
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		private void HandleUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			try
			{
				SevenDayUpdateDbData sevenDayUpdateDbData = DataHelper.BytesToObject<SevenDayUpdateDbData>(cmdParams, 0, count);
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref sevenDayUpdateDbData.RoleId);
				if (dbroleInfo == null)
				{
					throw new Exception("SevenDayActivityManager.HandleUpdate not Find DBRoleInfo, roleid=" + sevenDayUpdateDbData.RoleId);
				}
				string text = string.Format("REPLACE INTO t_seven_day_act(roleid,act_type,id,award_flag,param1,param2) VALUES({0},{1},{2},{3},{4},{5})", new object[]
				{
					sevenDayUpdateDbData.RoleId,
					sevenDayUpdateDbData.ActivityType,
					sevenDayUpdateDbData.Id,
					sevenDayUpdateDbData.Data.AwardFlag,
					sevenDayUpdateDbData.Data.Params1,
					sevenDayUpdateDbData.Data.Params2
				});
				if (!this.ExecNonQuery(text))
				{
					throw new Exception("SevenDayActivityManager.HandleUpdate ExecSql Failed, sql= " + text);
				}
				lock (dbroleInfo.SevenDayActDict)
				{
					Dictionary<int, SevenDayItemData> dictionary = null;
					if (!dbroleInfo.SevenDayActDict.TryGetValue(sevenDayUpdateDbData.ActivityType, out dictionary))
					{
						dictionary = new Dictionary<int, SevenDayItemData>();
						dbroleInfo.SevenDayActDict[sevenDayUpdateDbData.ActivityType] = dictionary;
					}
					dictionary[sevenDayUpdateDbData.Id] = sevenDayUpdateDbData.Data;
				}
				cmdData = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.Message, null, true);
				cmdData = false;
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		private bool ExecNonQuery(string sql)
		{
			bool result = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				mySQLCommand.ExecuteNonQuery();
				mySQLCommand.Dispose();
				result = true;
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = false;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}
	}
}
