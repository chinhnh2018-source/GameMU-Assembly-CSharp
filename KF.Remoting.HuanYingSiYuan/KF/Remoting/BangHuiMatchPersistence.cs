using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	public class BangHuiMatchPersistence
	{
		private BangHuiMatchPersistence()
		{
		}

		public bool LoadDatabase(int seasonIDCur_Gold, int seasonIDLast_Gold, int seasonIDCur_Rookie, int seasonIDLast_Rookie)
		{
			return this.LoadBHMatchBHData(1, this.BHMatchBHDataDict_Gold) && this.LoadBHMatchBHData(2, this.BHMatchBHDataDict_Rookie) && this.LoadBHMatchPKInfoList(1, this.BHMatchPKInfoList_Gold) && this.LoadBHMatchBHData_Join(1, seasonIDCur_Gold, this.BHMatchBHDataList_GoldJoin, false) && this.LoadBHMatchBHData_Join(2, seasonIDCur_Rookie, this.BHMatchBHDataList_RookieJoin, false) && this.LoadBHMatchBHData_Join(2, seasonIDLast_Rookie, this.BHMatchBHDataList_RookieJoinLast, true) && this.LoadBHMatchBHData_LastSeason(1, seasonIDLast_Gold) && this.LoadBHMatchBHData_LastSeason(2, seasonIDLast_Rookie) && this.ReloadDatabasePerRound(1, seasonIDCur_Gold, seasonIDLast_Gold, true) && this.ReloadDatabasePerRound(2, seasonIDCur_Rookie, seasonIDLast_Rookie, true);
		}

		public bool ReloadRankInfo(int renkType, int seasonIDCur, int seasonIDLast)
		{
			return this.LoadBHMatchRankInfo(renkType, seasonIDCur, seasonIDLast, this.BHMatchRankInfoDict);
		}

		public bool ReloadDatabasePerRound(int matchType, int seasonIDCur, int seasonIDLast, bool changeSeason = false)
		{
			for (int i = 0; i < 14; i++)
			{
				if (this.CheckCanReloadRankInfo(matchType, i, changeSeason))
				{
					if (!this.LoadBHMatchRankInfo(i, seasonIDCur, seasonIDLast, this.BHMatchRankInfoDict))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool CheckCanReloadRankInfo(int matchType, int rankLoop, bool changeSeason)
		{
			switch (rankLoop)
			{
			case 0:
			case 2:
			case 4:
			case 6:
			case 8:
			case 10:
			case 12:
				if (matchType != 1)
				{
					return false;
				}
				goto IL_5F;
			}
			if (matchType != 2)
			{
				return false;
			}
			IL_5F:
			return changeSeason || (rankLoop != 4 && rankLoop != 0 && rankLoop != 5 && rankLoop != 1);
		}

		public void AddDelayWriteSql(string sql)
		{
			lock (this.Mutex)
			{
				this.DelayWriteSqlQueue.Enqueue(sql);
			}
		}

		private void WriteDataToDb(string sql)
		{
			try
			{
				LogManager.WriteLog(3, sql, null, true);
				DbHelperMySQL.ExecuteSql(sql);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(string.Format("sql: {0}\r\n{1}", sql, ex.ToString()));
			}
		}

		public void DelayWriteDataProc()
		{
			List<string> list = null;
			lock (this.Mutex)
			{
				if (this.DelayWriteSqlQueue.Count == 0)
				{
					return;
				}
				list = this.DelayWriteSqlQueue.ToList<string>();
				this.DelayWriteSqlQueue.Clear();
			}
			foreach (string sql in list)
			{
				this.WriteDataToDb(sql);
			}
		}

		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				LogManager.WriteLog(3, sqlCmd, null, true);
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(sqlCmd + ex.ToString());
				result = -1;
			}
			return result;
		}

		public void SaveBHMatchBHData(BHMatchBHData data, bool chgChampion = true, bool chgKill = true)
		{
			string sqlCmd = string.Format("INSERT INTO t_banghui_match_bh(`type`, `bhid`, `bhname`, `zoneid_bh`, `rid`, `rname`, `zoneid_r`, `play`, `champion`, `kill`, bullshit, bestrecord) VALUES({0},{1},'{2}',{3},{4},'{5}',{6},{7},{8},{9},{10},{11}) ON DUPLICATE KEY UPDATE `bhname`='{2}',`zoneid_bh`={3},`rid`={4},`rname`='{5}',`zoneid_r`={6},`play`={7},`champion`={8},`kill`={9},`bullshit`={10},`bestrecord`={11};", new object[]
			{
				data.type,
				data.bhid,
				data.bhname,
				data.zoneid_bh,
				data.rid,
				data.rname,
				data.zoneid_r,
				data.hist_play,
				data.hist_champion,
				data.hist_kill,
				data.hist_bullshit,
				data.best_record
			});
			this.ExecuteSqlNoQuery(sqlCmd);
			if (chgChampion)
			{
				sqlCmd = string.Format("UPDATE t_banghui_match_bh SET ranktm_champion=NOW() WHERE `type`={0} AND `bhid`={1};", data.type, data.bhid);
				this.ExecuteSqlNoQuery(sqlCmd);
			}
			if (chgKill)
			{
				sqlCmd = string.Format("UPDATE t_banghui_match_bh SET ranktm_kill=NOW() WHERE `type`={0} AND `bhid`={1};", data.type, data.bhid);
				this.ExecuteSqlNoQuery(sqlCmd);
			}
		}

		public void SaveBHMatchBHSeasonData(int season, BHMatchBHData data, bool chgWin = true, bool chgScore = true)
		{
			string sqlCmd = string.Format("INSERT INTO t_banghui_match_bh_season(`type`, season, bhid, win, `group`, score) VALUES({0},{1},{2},{3},{4},{5}) ON DUPLICATE KEY UPDATE win={3},`group`={4},score={5};", new object[]
			{
				data.type,
				season,
				data.bhid,
				data.cur_win,
				data.group,
				data.cur_score
			});
			this.ExecuteSqlNoQuery(sqlCmd);
			if (chgWin)
			{
				sqlCmd = string.Format("UPDATE t_banghui_match_bh_season SET ranktm_win=NOW() WHERE `type`={0} AND `season`={1} AND `bhid`={2};", data.type, season, data.bhid);
				this.ExecuteSqlNoQuery(sqlCmd);
			}
			if (chgScore)
			{
				sqlCmd = string.Format("UPDATE t_banghui_match_bh_season SET ranktm_score=NOW() WHERE `type`={0} AND `season`={1} AND `bhid`={2};", data.type, season, data.bhid);
				this.ExecuteSqlNoQuery(sqlCmd);
			}
		}

		public void SaveBHMatchRolesSeasonData(int season, BHMatchRoleData roleData, bool chgMvp = true, bool chgKill = true)
		{
			string sql = string.Format("INSERT INTO t_banghui_match_roles_season(`type`, season, rid, mvp, `kill`) VALUES({0},{1},{2},{3},{4}) ON DUPLICATE KEY UPDATE mvp=mvp+{3}, `kill`=`kill`+{4};", new object[]
			{
				roleData.type,
				season,
				roleData.rid,
				roleData.mvp,
				roleData.kill
			});
			this.AddDelayWriteSql(sql);
			if (chgMvp)
			{
				sql = string.Format("UPDATE t_banghui_match_roles_season SET ranktm_mvp=NOW() WHERE `type`={0} AND `season`={1} AND `rid`={2};", roleData.type, season, roleData.rid);
				this.AddDelayWriteSql(sql);
			}
			if (chgKill)
			{
				sql = string.Format("UPDATE t_banghui_match_roles_season SET ranktm_kill=NOW() WHERE `type`={0} AND `season`={1} AND `rid`={2};", roleData.type, season, roleData.rid);
				this.AddDelayWriteSql(sql);
			}
		}

		public void SaveBHMatchRolesData(int type, int rid, string rname, int zoneid, int bhid, byte[] roledata)
		{
			if (null != roledata)
			{
				string text = string.Format("INSERT INTO t_banghui_match_roles(`type`, rid, rname, zoneid, bhid, data1) VALUES({0},{1},'{2}',{3},{4}, @content) ON DUPLICATE KEY UPDATE rname='{2}', bhid={4}, data1=@content;", new object[]
				{
					type,
					rid,
					rname,
					zoneid,
					bhid
				});
				DbHelperMySQL.ExecuteSqlInsertImg(text, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", roledata)
				});
			}
			else
			{
				string text = string.Format("INSERT INTO t_banghui_match_roles(`type`, rid, rname, zoneid, bhid) VALUES({0},{1},'{2}',{3},{4}) ON DUPLICATE KEY UPDATE rname='{2}', bhid={4};", new object[]
				{
					type,
					rid,
					rname,
					zoneid,
					bhid
				});
				this.AddDelayWriteSql(text);
			}
		}

		public void SaveBHMatchPKInfo(BangHuiMatchPKInfo pkinfo)
		{
			string sqlCmd = string.Format("INSERT INTO t_banghui_match_pk_log(`type`, season, round, bhid1, zoneid1, bhid2, zoneid2, result, logtime) VALUES({0},{1},{2},{3},{4},{5},{6},{7},'{8}')  ON DUPLICATE KEY UPDATE result={7};", new object[]
			{
				pkinfo.type,
				pkinfo.season,
				pkinfo.round,
				pkinfo.bhid1,
				pkinfo.zoneid1,
				pkinfo.bhid2,
				pkinfo.zoneid2,
				pkinfo.result,
				TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss")
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public int LoadLastSeasonIDGold()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 44, 0));
			object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 44);
			return Convert.ToInt32(single);
		}

		public void SaveLastSeasonIDGold(int seasonID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 44, seasonID));
		}

		public int GetGoldSeasonID()
		{
			object single = DbHelperMySQL.GetSingle("SELECT MAX(season) FROM t_banghui_match_bh_season WHERE `type`=" + 1);
			return Convert.ToInt32(single);
		}

		public byte[] LoadBHMatchRoleData(int type, int rid)
		{
			byte[] result;
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("SELECT data1 FROM t_banghui_match_roles WHERE `type`={0} AND rid={1}", type, rid));
				if (null == single)
				{
					result = null;
				}
				else
				{
					result = (byte[])single;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		private bool LoadBHMatchBHData(BangHuiMatchType type, Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict)
		{
			bool result;
			if (null == BHMatchBHDataDict)
			{
				result = false;
			}
			else
			{
				BHMatchBHDataDict.Clear();
				try
				{
					string strSQL = string.Format("SELECT * FROM t_banghui_match_bh where `type`={0}", type);
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KuaFuData<BHMatchBHData> kuaFuData = new KuaFuData<BHMatchBHData>();
						kuaFuData.V.type = type;
						kuaFuData.V.bhid = Convert.ToInt32(mySqlDataReader["bhid"]);
						kuaFuData.V.bhname = (mySqlDataReader["bhname"] as string);
						kuaFuData.V.zoneid_bh = Convert.ToInt32(mySqlDataReader["zoneid_bh"]);
						kuaFuData.V.rid = Convert.ToInt32(mySqlDataReader["rid"]);
						kuaFuData.V.rname = (mySqlDataReader["rname"] as string);
						kuaFuData.V.zoneid_r = Convert.ToInt32(mySqlDataReader["zoneid_r"]);
						kuaFuData.V.hist_play = Convert.ToInt32(mySqlDataReader["play"]);
						kuaFuData.V.hist_champion = Convert.ToInt32(mySqlDataReader["champion"]);
						kuaFuData.V.hist_bullshit = Convert.ToInt32(mySqlDataReader["bullshit"]);
						kuaFuData.V.best_record = Convert.ToInt32(mySqlDataReader["bestrecord"]);
						string sqlstring = string.Format("select sum(win) totalwin from t_banghui_match_bh_season where `type`={0} and bhid={1}", type, kuaFuData.V.bhid);
						kuaFuData.V.hist_win = Convert.ToInt32(DbHelperMySQL.GetSingle(sqlstring));
						string sqlstring2 = string.Format("select sum(score) totalscore from t_banghui_match_bh_season where `type`={0} and bhid={1}", type, kuaFuData.V.bhid);
						kuaFuData.V.hist_score = Convert.ToInt32(DbHelperMySQL.GetSingle(sqlstring2));
						kuaFuData.V.hist_kill = Convert.ToInt32(mySqlDataReader["kill"]);
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						BHMatchBHDataDict[kuaFuData.V.bhid] = kuaFuData;
					}
					if (mySqlDataReader != null)
					{
						mySqlDataReader.Close();
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
					return false;
				}
				result = true;
			}
			return result;
		}

		private bool LoadBHMatchPKInfoList(BangHuiMatchType type, KuaFuData<List<BangHuiMatchPKInfo>> BHMatchPKInfoList_Gold)
		{
			bool result;
			if (null == BHMatchPKInfoList_Gold)
			{
				result = false;
			}
			else
			{
				BHMatchPKInfoList_Gold.V.Clear();
				try
				{
					KuaFuData<BHMatchBHData> kuaFuData = null;
					string strSQL = string.Format("SELECT * FROM t_banghui_match_pk_log WHERE `type`={0} ORDER BY `season` DESC, `round` DESC LIMIT {1}", type, 80);
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						BangHuiMatchPKInfo bangHuiMatchPKInfo = new BangHuiMatchPKInfo();
						bangHuiMatchPKInfo.type = Convert.ToByte(mySqlDataReader["type"]);
						bangHuiMatchPKInfo.season = Convert.ToInt32(mySqlDataReader["season"]);
						bangHuiMatchPKInfo.round = Convert.ToByte(mySqlDataReader["round"]);
						bangHuiMatchPKInfo.bhid1 = Convert.ToInt32(mySqlDataReader["bhid1"]);
						bangHuiMatchPKInfo.bhid2 = Convert.ToInt32(mySqlDataReader["bhid2"]);
						bangHuiMatchPKInfo.result = Convert.ToByte(mySqlDataReader["result"]);
						bangHuiMatchPKInfo.zoneid1 = Convert.ToInt32(mySqlDataReader["zoneid1"]);
						bangHuiMatchPKInfo.zoneid2 = Convert.ToInt32(mySqlDataReader["zoneid2"]);
						if (this.BHMatchBHDataDict_Gold.TryGetValue(bangHuiMatchPKInfo.bhid1, out kuaFuData))
						{
							bangHuiMatchPKInfo.bhname1 = KuaFuServerManager.FormatName(kuaFuData.V.zoneid_bh, kuaFuData.V.bhname);
						}
						if (this.BHMatchBHDataDict_Gold.TryGetValue(bangHuiMatchPKInfo.bhid2, out kuaFuData))
						{
							bangHuiMatchPKInfo.bhname2 = KuaFuServerManager.FormatName(kuaFuData.V.zoneid_bh, kuaFuData.V.bhname);
						}
						BHMatchPKInfoList_Gold.V.Add(bangHuiMatchPKInfo);
					}
					TimeUtil.AgeByNow(ref BHMatchPKInfoList_Gold.Age);
					if (mySqlDataReader != null)
					{
						mySqlDataReader.Close();
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
					return false;
				}
				result = true;
			}
			return result;
		}

		private bool LoadBHMatchBHData_LastSeason(BangHuiMatchType type, int seasonID)
		{
			try
			{
				KuaFuData<BHMatchBHData> kuaFuData = null;
				string strSQL = string.Format("SELECT * FROM t_banghui_match_bh_season where `type`={0} and `season`={1}", type, seasonID);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					int key = Convert.ToInt32(mySqlDataReader["bhid"]);
					if (type == 1 && this.BHMatchBHDataDict_Gold.TryGetValue(key, out kuaFuData))
					{
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						kuaFuData.V.last_win = Convert.ToInt32(mySqlDataReader["win"]);
						kuaFuData.V.last_score = 0;
					}
					if (type == 2 && this.BHMatchBHDataDict_Rookie.TryGetValue(key, out kuaFuData))
					{
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						kuaFuData.V.last_win = Convert.ToInt32(mySqlDataReader["win"]);
						kuaFuData.V.last_score = Convert.ToInt32(mySqlDataReader["score"]);
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
				return false;
			}
			return true;
		}

		private bool LoadBHMatchBHData_Join(BangHuiMatchType type, int seasonID, List<BHMatchBHData> BHMatchBHDataList_Join, bool lastSeason = false)
		{
			bool result;
			if (null == BHMatchBHDataList_Join)
			{
				result = false;
			}
			else
			{
				BHMatchBHDataList_Join.Clear();
				try
				{
					KuaFuData<BHMatchBHData> kuaFuData = null;
					string text = string.Format("SELECT * FROM t_banghui_match_bh_season WHERE `type`={0} AND `season`={1}", type, seasonID);
					if (type == 1)
					{
						text += string.Format(" ORDER BY `group` ASC", new object[0]);
					}
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(text, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						int key = Convert.ToInt32(mySqlDataReader["bhid"]);
						if (type == 1 && this.BHMatchBHDataDict_Gold.TryGetValue(key, out kuaFuData))
						{
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							if (!lastSeason)
							{
								kuaFuData.V.cur_win = Convert.ToInt32(mySqlDataReader["win"]);
								kuaFuData.V.group = Convert.ToInt32(mySqlDataReader["group"]);
								kuaFuData.V.cur_score = 0;
							}
							BHMatchBHDataList_Join.Add(kuaFuData.V);
						}
						if (type == 2 && this.BHMatchBHDataDict_Rookie.TryGetValue(key, out kuaFuData))
						{
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							if (!lastSeason)
							{
								kuaFuData.V.cur_win = Convert.ToInt32(mySqlDataReader["win"]);
								kuaFuData.V.group = 0;
								kuaFuData.V.cur_score = Convert.ToInt32(mySqlDataReader["score"]);
							}
							BHMatchBHDataList_Join.Add(kuaFuData.V);
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
					return false;
				}
				result = true;
			}
			return result;
		}

		private string FormatLoadBHMatchRankSql(int rankType, int seasonIDCur, int seasonIDLast)
		{
			int num = BHMatchConsts.MatchRankLimit[rankType];
			string result = "";
			switch (rankType)
			{
			case 0:
				result = string.Format("SELECT bhid a, win b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY `group` ASC LIMIT {2};", 1, seasonIDLast, num);
				break;
			case 1:
				result = string.Format("SELECT bhid a, score b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY score DESC, ranktm_score ASC, bhid ASC LIMIT {2};", 2, seasonIDLast, num);
				break;
			case 2:
				result = string.Format("SELECT bhid a, champion b FROM t_banghui_match_bh WHERE `type`={0} AND champion<>0 ORDER BY champion DESC, ranktm_champion ASC, bhid ASC LIMIT {1};", 1, num);
				break;
			case 3:
				result = string.Format("SELECT bhid a, champion b FROM t_banghui_match_bh WHERE `type`={0} AND champion<>0 ORDER BY champion DESC, ranktm_champion ASC, bhid ASC LIMIT {1};", 2, num);
				break;
			case 4:
				result = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 1, seasonIDLast, num);
				break;
			case 5:
				result = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 2, seasonIDLast, num);
				break;
			case 6:
				result = string.Format("SELECT rid a, mvp b FROM (SELECT rid, SUM(mvp) mvp, MAX(ranktm_mvp) ranktm FROM t_banghui_match_roles_season WHERE `type`={0} GROUP BY rid) a1\r\n                                        WHERE mvp<>0 ORDER BY mvp DESC, ranktm ASC, rid ASC LIMIT {1};", 1, num);
				break;
			case 7:
				result = string.Format("SELECT rid a, mvp b FROM (SELECT rid, SUM(mvp) mvp, MAX(ranktm_mvp) ranktm FROM t_banghui_match_roles_season WHERE `type`={0} GROUP BY rid) a1 \r\n                                        WHERE mvp<>0 ORDER BY mvp DESC, ranktm ASC, rid ASC LIMIT {1};", 2, num);
				break;
			case 8:
				result = string.Format("SELECT bhid a, win b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY win DESC, `group` ASC, ranktm_win ASC, bhid ASC LIMIT {2};", 1, seasonIDCur, num);
				break;
			case 9:
				result = string.Format("SELECT bhid a, score b FROM t_banghui_match_bh_season WHERE `type`={0} AND season={1} ORDER BY score DESC, ranktm_score ASC, bhid ASC LIMIT {2};", 2, seasonIDCur, num);
				break;
			case 10:
				result = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 1, seasonIDCur, num);
				break;
			case 11:
				result = string.Format("SELECT rid a, mvp b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND mvp<>0 ORDER BY mvp DESC, ranktm_mvp ASC, rid ASC LIMIT {2};", 2, seasonIDCur, num);
				break;
			case 12:
				result = string.Format("SELECT rid a, `kill` b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND `kill`<>0 ORDER BY `kill` DESC, ranktm_kill ASC, rid ASC LIMIT {2};", 1, seasonIDCur, num);
				break;
			case 13:
				result = string.Format("SELECT rid a, `kill` b FROM t_banghui_match_roles_season WHERE `type`={0} AND season={1} AND `kill`<>0 ORDER BY `kill` DESC, ranktm_kill ASC, rid ASC LIMIT {2};", 2, seasonIDCur, num);
				break;
			default:
				return result;
			}
			return result;
		}

		private bool LoadBHMatchRankInfo(int rankType, int seasonCur, int seasonLast, KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>> BHMatchRankInfoDict)
		{
			bool result;
			if (null == BHMatchRankInfoDict)
			{
				result = false;
			}
			else
			{
				List<BangHuiMatchRankInfo> list = null;
				if (!BHMatchRankInfoDict.V.TryGetValue(rankType, out list))
				{
					list = (BHMatchRankInfoDict.V[rankType] = new List<BangHuiMatchRankInfo>());
				}
				else
				{
					list.Clear();
				}
				try
				{
					string text = this.FormatLoadBHMatchRankSql(rankType, seasonCur, seasonLast);
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(text, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						BangHuiMatchRankInfo bangHuiMatchRankInfo = new BangHuiMatchRankInfo();
						bangHuiMatchRankInfo.Key = Convert.ToInt32(mySqlDataReader["a"]);
						bangHuiMatchRankInfo.Value = Convert.ToInt32(mySqlDataReader["b"]);
						switch (rankType)
						{
						case 4:
						case 6:
						case 10:
						case 12:
						{
							string sqlstring = string.Format("SELECT zoneid,rname FROM t_banghui_match_roles WHERE `type`={0} AND rid={1};", 1, bangHuiMatchRankInfo.Key);
							object[] array;
							if (DbHelperMySQL.GetSingleValues(sqlstring, out array) >= 0)
							{
								bangHuiMatchRankInfo.Param1 = KuaFuServerManager.FormatName(Convert.ToInt32(array[0]), array[1].ToString());
							}
							string sqlstring2 = string.Format("SELECT zoneid_bh,bhname FROM t_banghui_match_bh, \r\n                                                (SELECT bhid FROM t_banghui_match_roles WHERE `type`={0} AND rid={1}) a1 WHERE t_banghui_match_bh.bhid = a1.bhid;", 1, bangHuiMatchRankInfo.Key);
							if (DbHelperMySQL.GetSingleValues(sqlstring2, out array) >= 0)
							{
								bangHuiMatchRankInfo.Param2 = KuaFuServerManager.FormatName(Convert.ToInt32(array[0]), array[1].ToString());
							}
							list.Add(bangHuiMatchRankInfo);
							break;
						}
						case 5:
						case 7:
						case 11:
						case 13:
						{
							string sqlstring = string.Format("SELECT zoneid,rname FROM t_banghui_match_roles WHERE `type`={0} AND rid={1};", 2, bangHuiMatchRankInfo.Key);
							bangHuiMatchRankInfo.Param1 = Convert.ToString(DbHelperMySQL.GetSingle(sqlstring));
							object[] array;
							if (DbHelperMySQL.GetSingleValues(sqlstring, out array) >= 0)
							{
								bangHuiMatchRankInfo.Param1 = KuaFuServerManager.FormatName(Convert.ToInt32(array[0]), array[1].ToString());
							}
							string sqlstring2 = string.Format("SELECT zoneid_bh,bhname FROM t_banghui_match_bh, \r\n                                                (SELECT bhid FROM t_banghui_match_roles WHERE `type`={0} AND rid={1}) a1 WHERE t_banghui_match_bh.bhid = a1.bhid;", 2, bangHuiMatchRankInfo.Key);
							if (DbHelperMySQL.GetSingleValues(sqlstring2, out array) >= 0)
							{
								bangHuiMatchRankInfo.Param2 = KuaFuServerManager.FormatName(Convert.ToInt32(array[0]), array[1].ToString());
							}
							list.Add(bangHuiMatchRankInfo);
							break;
						}
						case 8:
						case 9:
							goto IL_24B;
						default:
							goto IL_24B;
						}
						continue;
						IL_24B:
						KuaFuData<BHMatchBHData> kuaFuData = null;
						if (this.BHMatchBHDataDict_Gold.TryGetValue(bangHuiMatchRankInfo.Key, out kuaFuData))
						{
							bangHuiMatchRankInfo.Param1 = KuaFuServerManager.FormatName(kuaFuData.V.zoneid_bh, kuaFuData.V.bhname);
							bangHuiMatchRankInfo.Param2 = KuaFuServerManager.FormatName(kuaFuData.V.zoneid_r, kuaFuData.V.rname);
							list.Add(bangHuiMatchRankInfo);
						}
						else if (this.BHMatchBHDataDict_Rookie.TryGetValue(bangHuiMatchRankInfo.Key, out kuaFuData))
						{
							bangHuiMatchRankInfo.Param1 = KuaFuServerManager.FormatName(kuaFuData.V.zoneid_bh, kuaFuData.V.bhname);
							bangHuiMatchRankInfo.Param2 = KuaFuServerManager.FormatName(kuaFuData.V.zoneid_r, kuaFuData.V.rname);
							list.Add(bangHuiMatchRankInfo);
						}
					}
					TimeUtil.AgeByNow(ref BHMatchRankInfoDict.Age);
					if (mySqlDataReader != null)
					{
						mySqlDataReader.Close();
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
					return false;
				}
				result = true;
			}
			return result;
		}

		public static readonly BangHuiMatchPersistence Instance = new BangHuiMatchPersistence();

		public object Mutex = new object();

		public KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>> BHMatchRankInfoDict = new KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>>();

		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Gold = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Rookie = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		public KuaFuData<List<BangHuiMatchPKInfo>> BHMatchPKInfoList_Gold = new KuaFuData<List<BangHuiMatchPKInfo>>();

		public List<BHMatchBHData> BHMatchBHDataList_GoldJoin = new List<BHMatchBHData>();

		public List<BHMatchBHData> BHMatchBHDataList_RookieJoin = new List<BHMatchBHData>();

		public List<BHMatchBHData> BHMatchBHDataList_RookieJoinLast = new List<BHMatchBHData>();

		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
