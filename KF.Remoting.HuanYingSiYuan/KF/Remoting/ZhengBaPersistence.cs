using System;
using System.Collections.Generic;
using GameServer.Logic;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	public class ZhengBaPersistence
	{
		private ZhengBaPersistence()
		{
		}

		public ZhengBaSyncData LoadZhengBaSyncData(DateTime now, int selectRoleIfNewCreate, long dayBeginTicks)
		{
			ZhengBaSyncData zhengBaSyncData = new ZhengBaSyncData();
			zhengBaSyncData.Month = ZhengBaUtils.MakeMonth(now);
			zhengBaSyncData.IsThisMonthInActivity = this.CheckThisMonthInActivity(now, dayBeginTicks);
			bool flag;
			if (zhengBaSyncData.IsThisMonthInActivity)
			{
				flag = this.CheckBuildZhengBaRank(selectRoleIfNewCreate, zhengBaSyncData.Month);
				zhengBaSyncData.RoleList = this.LoadZhengBaRankData(zhengBaSyncData.Month);
				zhengBaSyncData.SupportList = this.LoadZhengBaSupportData(zhengBaSyncData.Month);
			}
			else
			{
				flag = false;
				zhengBaSyncData.RoleList = new List<ZhengBaRoleInfoData>();
				zhengBaSyncData.SupportList = new List<ZhengBaSupportAnalysisData>();
			}
			zhengBaSyncData.LastKingData = this.LoadZhengBaKingData(ref zhengBaSyncData.LastKingModTime);
			zhengBaSyncData.RoleModTime = now;
			zhengBaSyncData.SupportModTime = now;
			if (flag && this.MonthRankFirstCreate != null)
			{
				this.MonthRankFirstCreate(selectRoleIfNewCreate);
			}
			return zhengBaSyncData;
		}

		public bool SaveSupportLog(ZhengBaSupportLogData data)
		{
			bool result;
			if (data == null)
			{
				result = false;
			}
			else
			{
				try
				{
					string sqlstring = string.Format("INSERT INTO t_zhengba_support_log(month,from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,`time`,rank_of_day,from_serverid) VALUES({0},{1},{2},'{3}',{4},{5},{6},'{7}',{8},{9});", new object[]
					{
						ZhengBaUtils.MakeMonth(data.Time),
						data.FromRoleId,
						data.FromZoneId,
						data.FromRolename,
						data.SupportType,
						data.ToUnionGroup,
						data.ToGroup,
						data.Time.ToString("yyyy-MM-dd HH:mm:ss"),
						data.RankOfDay,
						data.FromServerId
					});
					if (DbHelperMySQL.ExecuteSql(sqlstring) <= 0)
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, "SaveSupportLog failed!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		public bool SavePkLog(ZhengBaPkLogData log)
		{
			bool result;
			if (log == null)
			{
				result = false;
			}
			else
			{
				try
				{
					string sqlstring = string.Format("INSERT INTO t_zhengba_pk_log(month,day,rid1,zoneid1,rname1,ismirror1,rid2,zoneid2,rname2,ismirror2,result,upgrade,starttime,endtime) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9},{10},{11},'{12}','{13}');", new object[]
					{
						log.Month,
						log.Day,
						log.RoleID1,
						log.ZoneID1,
						log.RoleName1,
						log.IsMirror1 ? 1 : 0,
						log.RoleID2,
						log.ZoneID2,
						log.RoleName2,
						log.IsMirror2 ? 1 : 0,
						log.PkResult,
						log.UpGrade ? 1 : 0,
						log.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
						log.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
					});
					if (DbHelperMySQL.ExecuteSql(sqlstring) <= 0)
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, "SavePkLog failed!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		public bool UpdateRole(int month, int rid, int grade, int state, int group)
		{
			try
			{
				string sqlstring = string.Format("UPDATE t_zhengba_roles SET grade={0},`group`={1},state={2} WHERE month={3} AND rid={4}", new object[]
				{
					grade,
					group,
					state,
					month,
					rid
				});
				if (DbHelperMySQL.ExecuteSql(sqlstring) <= 0)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "UpdateRole failed!", ex, true);
				return false;
			}
			return true;
		}

		private bool CheckThisMonthInActivity(DateTime now, long dayBeginTicks)
		{
			bool result;
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(9))
				{
					result = false;
				}
				else
				{
					DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 30, 201111));
					int num = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 30);
					if (num == 201111)
					{
						if (now.Day > ZhengBaConsts.StartMonthDay)
						{
							result = false;
						}
						else if (now.Day < ZhengBaConsts.StartMonthDay)
						{
							result = true;
						}
						else
						{
							result = (now.TimeOfDay.Ticks < dayBeginTicks);
						}
					}
					else
					{
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = false;
			}
			return result;
		}

		private bool CheckBuildZhengBaRank(int selectRoleIfNewCreate, int nowMonth)
		{
			bool result = false;
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 30, 201111));
				int num = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 30);
				object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 4);
				if (num != nowMonth && single != null && ZhengBaUtils.MakeMonth(DataHelper2.GetRealDate((int)single)) == nowMonth)
				{
					string strSQL = string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_month_paihang ORDER BY duanweirank ASC LIMIT {0};", selectRoleIfNewCreate);
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						ZhengBaRoleInfoData zhengBaRoleInfoData = new ZhengBaRoleInfoData();
						zhengBaRoleInfoData.RoleId = Convert.ToInt32(mySqlDataReader["rid"]);
						zhengBaRoleInfoData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
						zhengBaRoleInfoData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
						zhengBaRoleInfoData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]);
						zhengBaRoleInfoData.DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"]);
						zhengBaRoleInfoData.ZhanLi = Convert.ToInt32(mySqlDataReader["zhanli"]);
						zhengBaRoleInfoData.RoleName = mySqlDataReader["rname"].ToString();
						if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
						{
							zhengBaRoleInfoData.TianTiPaiHangRoleData = (byte[])mySqlDataReader["data1"];
						}
						if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data2")))
						{
							zhengBaRoleInfoData.PlayerJingJiMirrorData = (byte[])mySqlDataReader["data2"];
						}
						if (string.IsNullOrEmpty(zhengBaRoleInfoData.RoleName) && zhengBaRoleInfoData.TianTiPaiHangRoleData != null)
						{
							TianTiPaiHangRoleData_OnlyName tianTiPaiHangRoleData_OnlyName = DataHelper2.BytesToObject<TianTiPaiHangRoleData_OnlyName>(zhengBaRoleInfoData.TianTiPaiHangRoleData, 0, zhengBaRoleInfoData.TianTiPaiHangRoleData.Length);
							if (tianTiPaiHangRoleData_OnlyName != null)
							{
								zhengBaRoleInfoData.RoleName = tianTiPaiHangRoleData_OnlyName.RoleName;
							}
						}
						string strSQL2 = string.Format("REPLACE INTO t_zhengba_roles(`month`,rid,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,`grade`,`group`,state,rname,data1,data2) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},'{10}',@content,@mirror)", new object[]
						{
							nowMonth,
							zhengBaRoleInfoData.RoleId,
							zhengBaRoleInfoData.ZoneId,
							zhengBaRoleInfoData.DuanWeiId,
							zhengBaRoleInfoData.DuanWeiJiFen,
							zhengBaRoleInfoData.DuanWeiRank,
							zhengBaRoleInfoData.ZhanLi,
							100,
							0,
							0,
							zhengBaRoleInfoData.RoleName
						});
						DbHelperMySQL.ExecuteSqlInsertImg(strSQL2, new List<Tuple<string, byte[]>>
						{
							new Tuple<string, byte[]>("content", zhengBaRoleInfoData.TianTiPaiHangRoleData),
							new Tuple<string, byte[]>("mirror", zhengBaRoleInfoData.PlayerJingJiMirrorData)
						});
					}
					if (mySqlDataReader != null)
					{
						mySqlDataReader.Close();
					}
					DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 30, nowMonth));
					result = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		private List<ZhengBaRoleInfoData> LoadZhengBaRankData(int nowMonth)
		{
			List<ZhengBaRoleInfoData> list = new List<ZhengBaRoleInfoData>();
			try
			{
				string strSQL = string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,grade,`group`,state,data1,data2 FROM t_zhengba_roles where `month`={0} ORDER BY duanweirank ASC;", nowMonth);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					ZhengBaRoleInfoData zhengBaRoleInfoData = new ZhengBaRoleInfoData();
					zhengBaRoleInfoData.RoleId = Convert.ToInt32(mySqlDataReader["rid"]);
					zhengBaRoleInfoData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
					zhengBaRoleInfoData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
					zhengBaRoleInfoData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]);
					zhengBaRoleInfoData.DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"]);
					zhengBaRoleInfoData.ZhanLi = Convert.ToInt32(mySqlDataReader["zhanli"]);
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
					{
						zhengBaRoleInfoData.TianTiPaiHangRoleData = (byte[])mySqlDataReader["data1"];
					}
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data2")))
					{
						zhengBaRoleInfoData.PlayerJingJiMirrorData = (byte[])mySqlDataReader["data2"];
					}
					zhengBaRoleInfoData.Grade = Convert.ToInt32(mySqlDataReader["grade"]);
					zhengBaRoleInfoData.Group = Convert.ToInt32(mySqlDataReader["group"]);
					zhengBaRoleInfoData.State = Convert.ToInt32(mySqlDataReader["state"]);
					zhengBaRoleInfoData.RoleName = mySqlDataReader["rname"].ToString();
					list.Add(zhengBaRoleInfoData);
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		private ZhengBaRoleInfoData LoadZhengBaKingData(ref int month)
		{
			try
			{
				ZhengBaRoleInfoData result = null;
				string strSQL = string.Format("SELECT * FROM t_zhengba_roles WHERE grade={0} ORDER BY `month` DESC LIMIT 1;", 1);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				if (mySqlDataReader != null && mySqlDataReader.Read())
				{
					ZhengBaRoleInfoData zhengBaRoleInfoData = new ZhengBaRoleInfoData();
					zhengBaRoleInfoData.RoleId = Convert.ToInt32(mySqlDataReader["rid"]);
					zhengBaRoleInfoData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
					zhengBaRoleInfoData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
					zhengBaRoleInfoData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]);
					zhengBaRoleInfoData.DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"]);
					zhengBaRoleInfoData.ZhanLi = Convert.ToInt32(mySqlDataReader["zhanli"]);
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
					{
						zhengBaRoleInfoData.TianTiPaiHangRoleData = (byte[])mySqlDataReader["data1"];
					}
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data2")))
					{
						zhengBaRoleInfoData.PlayerJingJiMirrorData = (byte[])mySqlDataReader["data2"];
					}
					zhengBaRoleInfoData.Grade = Convert.ToInt32(mySqlDataReader["grade"]);
					zhengBaRoleInfoData.Group = Convert.ToInt32(mySqlDataReader["group"]);
					zhengBaRoleInfoData.State = Convert.ToInt32(mySqlDataReader["state"]);
					zhengBaRoleInfoData.RoleName = mySqlDataReader["rname"].ToString();
					month = Convert.ToInt32(mySqlDataReader["month"]);
					result = zhengBaRoleInfoData;
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
				return result;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		private List<ZhengBaSupportAnalysisData> LoadZhengBaSupportData(int nowMonth)
		{
			List<ZhengBaSupportAnalysisData> list = new List<ZhengBaSupportAnalysisData>();
			try
			{
				string strSQL = string.Format("SELECT support_type,rank_of_day,to_union_group,to_group FROM t_zhengba_support_log where `month`={0}", nowMonth);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					int num = Convert.ToInt32(mySqlDataReader["support_type"]);
					int toUnionGroup = Convert.ToInt32(mySqlDataReader["to_union_group"]);
					int toGroup = Convert.ToInt32(mySqlDataReader["to_group"]);
					int rankOfDay = Convert.ToInt32(mySqlDataReader["rank_of_day"]);
					ZhengBaSupportAnalysisData zhengBaSupportAnalysisData;
					if ((zhengBaSupportAnalysisData = list.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == toUnionGroup && _s.Group == toGroup)) == null)
					{
						zhengBaSupportAnalysisData = new ZhengBaSupportAnalysisData
						{
							UnionGroup = toUnionGroup,
							Group = toGroup,
							RankOfDay = rankOfDay
						};
						list.Add(zhengBaSupportAnalysisData);
					}
					if (num == 1)
					{
						zhengBaSupportAnalysisData.TotalSupport++;
					}
					else if (num == 2)
					{
						zhengBaSupportAnalysisData.TotalOppose++;
					}
					else if (num == 3)
					{
						zhengBaSupportAnalysisData.TotalYaZhu++;
					}
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		public static readonly ZhengBaPersistence Instance = new ZhengBaPersistence();

		public ZhengBaPersistence.FirstCreateDbRank MonthRankFirstCreate = null;

		public delegate void FirstCreateDbRank(int selectRoleIfNewCreate);
	}
}
