using System;
using System.Collections.Generic;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Remoting;
using Server.Tools;

namespace KF.Remoting
{
	public class ZhanDuiZhengBaPersistence
	{
		private ZhanDuiZhengBaPersistence()
		{
		}

		public ZhanDuiZhengBaSyncData LoadZhengBaSyncData(DateTime now, int selectRoleIfNewCreate)
		{
			ZhanDuiZhengBaSyncData zhanDuiZhengBaSyncData = new ZhanDuiZhengBaSyncData();
			zhanDuiZhengBaSyncData.Month = ZhengBaUtils.MakeMonth(now);
			zhanDuiZhengBaSyncData.IsThisMonthInActivity = false;
			List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
			List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = new List<ZhanDuiZhengBaZhanDuiData>();
			List<ZhanDuiZhengBaPkLogData> pklogList = new List<ZhanDuiZhengBaPkLogData>();
			if (KuaFuServerManager.IsGongNengOpened(113))
			{
				TianTi5v5RankData tianTi5v5RankData = TianTi5v5Service.ZhanDuiGetRankingData(DateTime.MinValue);
				bool flag = this.CheckZhengBaRank(selectRoleIfNewCreate, zhanDuiZhengBaSyncData.Month);
				if (flag)
				{
					zhanDuiZhengBaSyncData.IsThisMonthInActivity = true;
				}
				else if (zhanDuiZhengBaSyncData.Month == ZhengBaUtils.MakeMonth(tianTi5v5RankData.ModifyTime) && tianTi5v5RankData.MonthPaiHangList.Count > ZhanDuiZhengBaConsts.MinZhanDuiNum)
				{
					zhanDuiZhengBaSyncData.IsThisMonthInActivity = true;
					int[] array = MathEx.MatchGroupBinary(64);
					int num = 0;
					while (num < 64 && num < tianTi5v5RankData.MonthPaiHangList.Count)
					{
						int group = Array.IndexOf<int>(array, num + 1) + 1;
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = tianTi5v5RankData.MonthPaiHangList[num];
						ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = new ZhanDuiZhengBaZhanDuiData
						{
							ZhanDuiName = tianTi5v5ZhanDuiData.ZhanDuiName,
							ZhanDuiID = tianTi5v5ZhanDuiData.ZhanDuiID,
							ZhanLi = (long)((int)tianTi5v5ZhanDuiData.ZhanDouLi),
							DuanWeiId = tianTi5v5ZhanDuiData.DuanWeiId,
							DuanWeiRank = tianTi5v5ZhanDuiData.DuanWeiRank,
							ZoneId = tianTi5v5ZhanDuiData.ZoneID,
							MemberList = new List<RoleOccuNameZhanLi>(),
							Group = group
						};
						foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in tianTi5v5ZhanDuiData.teamerList)
						{
							if (tianTi5v5ZhanDuiRoleData.RoleID == tianTi5v5ZhanDuiData.LeaderRoleID)
							{
								zhanDuiZhengBaZhanDuiData.MemberList.Insert(0, new RoleOccuNameZhanLi
								{
									RoleName = tianTi5v5ZhanDuiRoleData.RoleName,
									Occupation = tianTi5v5ZhanDuiRoleData.RoleOcc,
									ZhanLi = tianTi5v5ZhanDuiRoleData.ZhanLi
								});
							}
							else
							{
								zhanDuiZhengBaZhanDuiData.MemberList.Add(new RoleOccuNameZhanLi
								{
									RoleName = tianTi5v5ZhanDuiRoleData.RoleName,
									Occupation = tianTi5v5ZhanDuiRoleData.RoleOcc,
									ZhanLi = tianTi5v5ZhanDuiRoleData.ZhanLi
								});
							}
						}
						list.Add(zhanDuiZhengBaZhanDuiData);
						num++;
					}
					if (!this.BuildZhengBaRank(zhanDuiZhengBaSyncData.Month, list))
					{
						LogManager.WriteLog(1000, "生成并写入战队争霸64强数据失败!", null, true);
						zhanDuiZhengBaSyncData.IsThisMonthInActivity = false;
					}
				}
				if (zhanDuiZhengBaSyncData.IsThisMonthInActivity)
				{
					zhanDuiList = this.LoadZhengBaRankData(zhanDuiZhengBaSyncData.Month);
					pklogList = this.LoadPkLogList(zhanDuiZhengBaSyncData.Month);
				}
			}
			zhanDuiZhengBaSyncData.ZhanDuiList = zhanDuiList;
			zhanDuiZhengBaSyncData.PKLogList = pklogList;
			zhanDuiZhengBaSyncData.RoleModTime = now;
			return zhanDuiZhengBaSyncData;
		}

		public int GetLastTopZhanDui(int month)
		{
			try
			{
				string sql = string.Format("SELECT zhanduiid FROM t_zhandui_zhengba where `month`={0} and grade={1};", month, 1);
				return (int)DbHelperMySQL.GetSingleLong(sql);
			}
			catch (Exception ex)
			{
			}
			return 0;
		}

		private List<ZhanDuiZhengBaZhanDuiData> LoadZhengBaRankData(int nowMonth)
		{
			List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
			try
			{
				string strSQL = string.Format("SELECT zhanduiid,zhanduiname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,grade,`group`,state,data1,data2 FROM t_zhandui_zhengba where `month`={0} ORDER BY duanweirank ASC;", nowMonth);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = new ZhanDuiZhengBaZhanDuiData();
						zhanDuiZhengBaZhanDuiData.ZhanDuiID = Convert.ToInt32(mySqlDataReader["zhanduiid"]);
						zhanDuiZhengBaZhanDuiData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
						zhanDuiZhengBaZhanDuiData.ZhanDuiName = mySqlDataReader["zhanduiname"].ToString();
						zhanDuiZhengBaZhanDuiData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
						zhanDuiZhengBaZhanDuiData.DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"]);
						zhanDuiZhengBaZhanDuiData.ZhanLi = Convert.ToInt64(mySqlDataReader["zhanli"]);
						if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
						{
							byte[] array = (byte[])mySqlDataReader["data1"];
							zhanDuiZhengBaZhanDuiData.MemberList = DataHelper2.BytesToObject<List<RoleOccuNameZhanLi>>(array, 0, array.Length);
						}
						zhanDuiZhengBaZhanDuiData.Grade = Convert.ToInt32(mySqlDataReader["grade"]);
						zhanDuiZhengBaZhanDuiData.Group = Convert.ToInt32(mySqlDataReader["group"]);
						zhanDuiZhengBaZhanDuiData.State = Convert.ToInt32(mySqlDataReader["state"]);
						list.Add(zhanDuiZhengBaZhanDuiData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return list;
		}

		private List<ZhanDuiZhengBaPkLogData> LoadPkLogList(int nowMonth)
		{
			List<ZhanDuiZhengBaPkLogData> list = new List<ZhanDuiZhengBaPkLogData>();
			try
			{
				string strSQL = string.Format("SELECT month,id,zhanduiid1,zoneid1,zhanduiname1,zhanduiid2,zoneid2,zhanduiname2,result,upgrade,starttime,endtime FROM t_zhandui_zhengba_pk_log where `month`={0} ORDER BY endtime ASC;", nowMonth);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						list.Add(new ZhanDuiZhengBaPkLogData
						{
							Month = Convert.ToInt32(mySqlDataReader["month"]),
							ID = Convert.ToInt32(mySqlDataReader["id"]),
							ZhanDuiID1 = Convert.ToInt32(mySqlDataReader["zhanduiid1"].ToString()),
							ZoneID1 = Convert.ToInt32(mySqlDataReader["zoneid1"]),
							ZhanDuiName1 = mySqlDataReader["zhanduiname1"].ToString(),
							ZhanDuiID2 = Convert.ToInt32(mySqlDataReader["zhanduiid2"].ToString()),
							ZoneID2 = Convert.ToInt32(mySqlDataReader["zoneid2"]),
							ZhanDuiName2 = mySqlDataReader["zhanduiname2"].ToString(),
							PkResult = Convert.ToInt32(mySqlDataReader["result"]),
							UpGrade = (Convert.ToInt32(mySqlDataReader["upgrade"]) == 1),
							StartTime = Convert.ToDateTime(mySqlDataReader["starttime"].ToString()),
							EndTime = Convert.ToDateTime(mySqlDataReader["endtime"].ToString())
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return list;
		}

		public bool SavePkLog(ZhanDuiZhengBaPkLogData log)
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
					string sql = string.Format("INSERT INTO t_zhandui_zhengba_pk_log(month,id,zhanduiid1,zoneid1,zhanduiname1,zhanduiid2,zoneid2,zhanduiname2,result,upgrade,starttime,endtime) VALUES({0},{1},{2},{3},'{4}',{5},{6},'{7}',{8},{9},'{10}','{11}');", new object[]
					{
						log.Month,
						log.ID,
						log.ZhanDuiID1,
						log.ZoneID1,
						log.ZhanDuiName1,
						log.ZhanDuiID2,
						log.ZoneID2,
						log.ZhanDuiName2,
						log.PkResult,
						log.UpGrade ? 1 : 0,
						log.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
						log.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
					});
					TianTiPersistence.Instance.AddDelayWriteSql(sql, null, null);
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

		public bool UpdateRole(int month, int zhanDuiID, int grade, int state)
		{
			try
			{
				string sql = string.Format("UPDATE t_zhandui_zhengba SET grade={0},state={1} WHERE month={2} AND zhanduiid={3}", new object[]
				{
					grade,
					state,
					month,
					zhanDuiID
				});
				TianTiPersistence.Instance.AddDelayWriteSql(sql, null, null);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "UpdateRole failed!", ex, true);
				return false;
			}
			return true;
		}

		private bool CheckZhengBaRank(int selectRoleIfNewCreate, int nowMonth)
		{
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 13, 201111));
				int num = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 13);
				return num == nowMonth;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		private bool BuildZhengBaRank(int nowMonth, List<ZhanDuiZhengBaZhanDuiData> rankList)
		{
			bool result = false;
			try
			{
				foreach (ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData in rankList)
				{
					string strSQL = string.Format("REPLACE INTO t_zhandui_zhengba(`month`,zhanduiid,zoneid,zhanduiname,duanweiid,duanweijifen,duanweirank,zhanli,`grade`,`group`,state,data1,data2) VALUES({0},{1},{2},'{3}',{4},{5},{6},{7},{8},{9},{10},@content,null)", new object[]
					{
						nowMonth,
						zhanDuiZhengBaZhanDuiData.ZhanDuiID,
						zhanDuiZhengBaZhanDuiData.ZoneId,
						zhanDuiZhengBaZhanDuiData.ZhanDuiName,
						zhanDuiZhengBaZhanDuiData.DuanWeiId,
						0,
						zhanDuiZhengBaZhanDuiData.DuanWeiRank,
						zhanDuiZhengBaZhanDuiData.ZhanLi,
						64,
						zhanDuiZhengBaZhanDuiData.Group,
						0
					});
					DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
					{
						new Tuple<string, byte[]>("content", DataHelper2.ObjectToBytes<List<RoleOccuNameZhanLi>>(zhanDuiZhengBaZhanDuiData.MemberList))
					});
				}
				DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 13, nowMonth));
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public static readonly ZhanDuiZhengBaPersistence Instance = new ZhanDuiZhengBaPersistence();
	}
}
