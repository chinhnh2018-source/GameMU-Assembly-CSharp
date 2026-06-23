using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class KuaFuCopyDbMgr
	{
		private KuaFuCopyDbMgr()
		{
		}

		public static KuaFuCopyDbMgr Instance
		{
			get
			{
				return KuaFuCopyDbMgr.g_Instance;
			}
		}

		public KFTeamCountControl TeamControl
		{
			get
			{
				KFTeamCountControl control;
				lock (this._ControlMutex)
				{
					control = this._Control;
				}
				return control;
			}
			private set
			{
				lock (this._ControlMutex)
				{
					this._Control = value;
				}
			}
		}

		public void InitConfig()
		{
			try
			{
				KFTeamCountControl kfteamCountControl = new KFTeamCountControl();
				XElement xelement = ConfigHelper.Load("config.xml");
				kfteamCountControl.TeamMaxWaitMinutes = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "FuBenTeamMaxWaitMinutes", "value", 10L);
				this.TeamControl = kfteamCountControl;
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(1, "KFCopyTeam 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -1;
			}
			return result;
		}

		public void CheckLogAsyncEvents(AsyncDataItem[] evList)
		{
			if (evList != null)
			{
				foreach (AsyncDataItem asyncDataItem in evList)
				{
					string text = string.Empty;
					try
					{
						if (asyncDataItem.EventType == 10000)
						{
							CopyTeamCreateData copyTeamCreateData = asyncDataItem.Args[1] as CopyTeamCreateData;
							text = string.Format("INSERT INTO t_kuafu_fuben_game_team(teamid,copyid,by_serverid,by_roleid,createtime) VALUES({0},{1},{2},{3},'{4}')", new object[]
							{
								copyTeamCreateData.TeamId,
								copyTeamCreateData.CopyId,
								copyTeamCreateData.Member.ServerId,
								copyTeamCreateData.Member.RoleID,
								TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
							});
						}
						else if (asyncDataItem.EventType == 10005)
						{
							CopyTeamStartData copyTeamStartData = asyncDataItem.Args[1] as CopyTeamStartData;
							text = string.Format("UPDATE t_kuafu_fuben_game_team SET starttime='{0}', kf_serverid={1} WHERE teamid={2}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), copyTeamStartData.ToServerId, copyTeamStartData.TeamId);
						}
						else if (asyncDataItem.EventType == 10006)
						{
							CopyTeamDestroyData copyTeamDestroyData = asyncDataItem.Args[0] as CopyTeamDestroyData;
							text = string.Format("UPDATE t_kuafu_fuben_game_team SET endtime='{0}' WHERE teamid={1}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), copyTeamDestroyData.TeamId);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(2, "KuaFuCopyDbMgr.CheckLogAsyncEvents Failed!!!", ex, true);
					}
					if (!string.IsNullOrEmpty(text))
					{
						this.ExecuteSqlNoQuery(text);
					}
				}
			}
		}

		public void SaveCopyTeamAnalysisData(KFCopyTeamAnalysis data)
		{
			if (data != null)
			{
				string text = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
				KFCopyTeamAnalysis.Item item = new KFCopyTeamAnalysis.Item();
				foreach (KeyValuePair<int, KFCopyTeamAnalysis.Item> keyValuePair in data.AnalysisDict)
				{
					int key = keyValuePair.Key;
					KFCopyTeamAnalysis.Item value = keyValuePair.Value;
					item.TotalCopyCount += value.TotalCopyCount;
					item.StartCopyCount += value.StartCopyCount;
					item.UnStartCopyCount += value.UnStartCopyCount;
					item.TotalRoleCount += value.TotalRoleCount;
					item.StartRoleCount += value.StartRoleCount;
					item.UnStartRoleCount += value.UnStartRoleCount;
					string sqlCmd = string.Format("REPLACE INTO t_kuafu_fuben_game_log(fuben_id,total_fuben_count,start_fuben_count,unstart_fuben_count,total_role_count,start_role_count,unstart_role_count,time)  VALUES({0},{1},{2},{3},{4},{5},{6},'{7}') ", new object[]
					{
						key,
						value.TotalCopyCount,
						value.StartCopyCount,
						value.UnStartCopyCount,
						value.TotalRoleCount,
						value.StartRoleCount,
						value.UnStartRoleCount,
						text
					});
					this.ExecuteSqlNoQuery(sqlCmd);
				}
				string sqlCmd2 = string.Format("REPLACE INTO t_kuafu_fuben_game_log(fuben_id,total_fuben_count,start_fuben_count,unstart_fuben_count,total_role_count,start_role_count,unstart_role_count,time)  VALUES({0},{1},{2},{3},{4},{5},{6},'{7}') ", new object[]
				{
					-1,
					item.TotalCopyCount,
					item.StartCopyCount,
					item.UnStartCopyCount,
					item.TotalRoleCount,
					item.StartRoleCount,
					item.UnStartRoleCount,
					text
				});
				this.ExecuteSqlNoQuery(sqlCmd2);
				string sqlCmd3 = string.Format("DELETE FROM t_kuafu_fuben_game_log WHERE time != '{0}'", text);
				this.ExecuteSqlNoQuery(sqlCmd3);
			}
		}

		public int AddHongDongAwardRecordForUser(string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			string sqlCmd = string.Format("INSERT INTO t_huodongawarduserhist (userid, activitytype, keystr, hasgettimes,lastgettime) VALUES('{0}', {1}, '{2}', {3}, '{4}')", new object[]
			{
				userid,
				activitytype,
				keystr,
				hasgettimes,
				lastgettime
			});
			return this.ExecuteSqlNoQuery(sqlCmd);
		}

		public int UpdateHongDongAwardRecordForUser(string userid, int activitytype, string keystr, long hasgettimes, string lastgettime)
		{
			string sqlCmd = string.Format("update t_huodongawarduserhist set hasgettimes={0}, lastgettime='{1}' where userid='{2}' and activitytype={3} and keystr='{4}' and hasgettimes!={5}", new object[]
			{
				hasgettimes,
				lastgettime,
				userid,
				activitytype,
				keystr,
				hasgettimes
			});
			return this.ExecuteSqlNoQuery(sqlCmd);
		}

		public int GetAwardHistoryForUser(string userid, int activitytype, string keystr, out long hasgettimes, out string lastgettime)
		{
			hasgettimes = 0L;
			lastgettime = "";
			int result = -1;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ", userid, activitytype, keystr);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				if (mySqlDataReader.Read())
				{
					hasgettimes = Convert.ToInt64(mySqlDataReader["hasgettimes"].ToString());
					lastgettime = mySqlDataReader["lastgettime"].ToString();
					result = 0;
				}
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return result;
		}

		private static readonly KuaFuCopyDbMgr g_Instance = new KuaFuCopyDbMgr();

		public object Mutex = new object();

		public bool Initialized = false;

		private KFTeamCountControl _Control = null;

		private object _ControlMutex = new object();
	}
}
