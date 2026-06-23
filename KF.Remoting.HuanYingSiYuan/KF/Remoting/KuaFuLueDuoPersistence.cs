using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;

namespace KF.Remoting
{
	public class KuaFuLueDuoPersistence
	{
		private KuaFuLueDuoPersistence()
		{
		}

		public bool LoadDatabase(int seasonID, int seasonIDLast, int minSeasonID)
		{
			try
			{
				if (!this.LoadKuaFuLueDuoServerData(minSeasonID))
				{
					return false;
				}
				if (!this.LoadKuaFuLueDuoBHData(this.KuaFuLueDuoBHDataDict, minSeasonID))
				{
					return false;
				}
				if (!this.LoadKuaFuLueDuoBHData_Join(seasonID, this.JingJiaDict))
				{
					return false;
				}
				if (!this.LoadRankData(minSeasonID, seasonIDLast))
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return false;
		}

		public bool LoadRankData(int minSeasonID, int seasonIDLast)
		{
			for (int i = 0; i < 6; i++)
			{
				if (!this.LoadKuaFuLueDuoRankInfo(i, minSeasonID, seasonIDLast, this.KuaFuLueDuoRankInfoDict))
				{
					return false;
				}
			}
			return true;
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

		public void ClearLastSeasonData()
		{
			this.ExecuteSqlNoQuery("delete from t_kfld_role;");
			this.ExecuteSqlNoQuery("update `t_kfld_server` set `mingxing`='',`zhengfu`=0,`sum_ziyuan`=0,`last_ziyuan`=0;");
			this.ExecuteSqlNoQuery("update `t_kfld_banghui` set `sum_ziyuan`='0',`last`=0;");
		}

		public void SaveKuaFuLueDuoBHData(KuaFuLueDuoBHData data)
		{
			string sqlCmd = string.Format("INSERT INTO t_kfld_banghui(`season`, `bhid`, `bhname`, `zoneid`, `sum_ziyuan`, `last`) VALUES({0},{1},'{2}',{3},{4},{5}) ON DUPLICATE KEY UPDATE `season`='{0}',`bhname`='{2}',`zoneid`={3},`sum_ziyuan`='{4}',`last`={5}", new object[]
			{
				data.season,
				data.bhid,
				data.bhname,
				data.zoneid,
				data.sum_ziyuan,
				data.last_ziyuan
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void SaveKuaFuLueDuoBHSeasonData(int season, KuaFuLueDuoBHData data)
		{
			string sqlCmd = string.Format("INSERT INTO t_kfld_banghui_season(`season`, `bhid`, `bhname`, `zoneid`, `jingjia`, `jingjia_sid`, `last_jingjia`,`group`) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7}) ON DUPLICATE KEY UPDATE `bhname`='{2}',`zoneid`={3},`jingjia`='{4}',`jingjia_sid`={5},`last_jingjia`={6}", new object[]
			{
				data.season,
				data.bhid,
				data.bhname,
				data.zoneid,
				data.jingjia,
				data.jingjia_sid,
				data.last_jingjia,
				data.group
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void SaveKuaFuLueDuoRoleData(int season, int rid, string rname, int zoneid, int kill)
		{
			string sql = string.Format("INSERT INTO t_kfld_role(`rid`, `rname`,`zoneid`, `kill`, `season`,`last`) VALUES({0},'{1}',{2},{3},{4},{3}) ON DUPLICATE KEY UPDATE `rname`='{1}', `kill`=`kill`+{3},`last`={3},`season`={4}", new object[]
			{
				rid,
				rname,
				zoneid,
				kill,
				season
			});
			this.AddDelayWriteSql(sql);
		}

		public void SaveKuaFuLueDuoServerData(int season, KuaFuLueDuoServerInfo data)
		{
			string sqlCmd = string.Format("INSERT INTO t_kfld_server(`serverid`, `sum_ziyuan`, `last_ziyuan`,`mingxing`,`season`) VALUES({0},{1},{2},'{3}',{4}) ON DUPLICATE KEY UPDATE sum_ziyuan='{1}', `last_ziyuan`={2}, `mingxing`='{3}',`season`={4}", new object[]
			{
				data.ServerId,
				data.ZiYuan,
				data.LastZiYuan,
				data.MingXingZhanMengList,
				season
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void SaveKuaFuLueDuoServerMingXing(int serverId, string mingxing)
		{
			string sqlCmd = string.Format("update t_kfld_server set `mingxing`='{1}' where serverid={0};", serverId, mingxing);
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void SaveKuaFuLueDuoServerZhengFuData(int season, KuaFuLueDuoServerInfo data)
		{
			string sql = string.Format("UPDATE t_kfld_server set `season`={1},`zhengfu`={2} where `serverid`={0}", data.ServerId, season, (data.ZhengFuList == null) ? 0 : data.ZhengFuList.Count);
			this.AddDelayWriteSql(sql);
		}

		public void SaveKuaFuLueDuoServerRankData(int season, int group, KuaFuLueDuoServerInfo data, int destServerId, int percent)
		{
			if (destServerId > 0)
			{
				string sql = string.Format("INSERT IGNORE INTO `t_kfld_zhengfu` (`season`, `serverid`, `fall_sid`, `group`, `info`) VALUES ({0}, {1}, {2}, {3}, {4});", new object[]
				{
					season,
					data.ServerId,
					destServerId,
					group,
					percent
				});
				this.AddDelayWriteSql(sql);
			}
		}

		public int LoadSeasonCount()
		{
			object single = DbHelperMySQL.GetSingle("select count(*) from `t_kfld_season`");
			int result;
			if (null == single)
			{
				result = 0;
			}
			else
			{
				result = Convert.ToInt32(single);
			}
			return result;
		}

		public void SaveSeasonID(int seasonID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO `t_kfld_season` VALUES({0});", seasonID));
		}

		public int[] GetHistSeasonIDs(int maxCount)
		{
			int[] array = new int[3];
			try
			{
				string strSQL = string.Format("SELECT `season` FROM `t_kfld_season` order by `season` desc limit {0}", maxCount);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						int num = Convert.ToInt32(mySqlDataReader[0].ToString());
						if (array[0] > 0)
						{
							array[1] = num;
						}
						if (array[0] == 0)
						{
							array[0] = num;
						}
						if (--maxCount == 0)
						{
							array[2] = num;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return array;
		}

		public void SaveSignUpRound(int signUpRound)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 7, signUpRound));
		}

		public int GetSignUpRound()
		{
			object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 7);
			int result;
			if (null == single)
			{
				result = 0;
			}
			else
			{
				result = Convert.ToInt32(single);
			}
			return result;
		}

		private bool LoadKuaFuLueDuoBHData(Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict, int minSeason)
		{
			try
			{
				long age = TimeUtil.AgeByNow();
				string strSQL = string.Format("SELECT * FROM t_kfld_banghui where `season`>={0}", minSeason);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KuaFuData<KuaFuLueDuoBHData> kuaFuData = new KuaFuData<KuaFuLueDuoBHData>();
						kuaFuData.V.season = Convert.ToInt32(mySqlDataReader["season"]);
						kuaFuData.V.bhid = Convert.ToInt32(mySqlDataReader["bhid"]);
						kuaFuData.V.bhname = (mySqlDataReader["bhname"] as string);
						kuaFuData.V.zoneid = Convert.ToInt32(mySqlDataReader["zoneid"]);
						kuaFuData.V.sum_ziyuan = Convert.ToInt32(mySqlDataReader["sum_ziyuan"]);
						kuaFuData.V.last_ziyuan = Convert.ToInt32(mySqlDataReader["last"]);
						kuaFuData.Age = age;
						KuaFuLueDuoBHDataDict[kuaFuData.V.bhid] = kuaFuData;
						int zoneid = kuaFuData.V.zoneid;
						if (kuaFuData.V.sum_ziyuan > 0 && zoneid > 0 && zoneid < this.ZoneID2ServerIDs.Length)
						{
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
							if (this.ServerInfoDict.TryGetValue(this.ZoneID2ServerIDs[zoneid], out kuaFuLueDuoServerInfo))
							{
								string param = KuaFuServerManager.FormatName(kuaFuData.V.bhname, kuaFuData.V.zoneid);
								kuaFuLueDuoServerInfo.MingXingList.Add(new KuaFuLueDuoRankInfo
								{
									Key = kuaFuData.V.bhid,
									Param1 = param,
									Value = kuaFuData.V.sum_ziyuan
								});
							}
						}
					}
				}
				foreach (KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo in this.ServerInfoDict.Values)
				{
					KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
					string text = KuaFuLueDuoUtils.RankList2MingXingStr(kuaFuLueDuoServerInfo.MingXingList, 2);
					if (text != kuaFuLueDuoServerInfo.MingXingZhanMengList)
					{
						kuaFuLueDuoServerInfo.MingXingZhanMengList = text;
						string sqlCmd = string.Format("update t_kfld_server set `mingxing`='{1}' where `serverid`={0}", kuaFuLueDuoServerInfo.ServerId, text);
						this.ExecuteSqlNoQuery(sqlCmd);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return true;
		}

		private bool LoadKuaFuLueDuoServerData(int minSeasonID)
		{
			try
			{
				string strSQL = string.Format("SELECT * FROM t_kfld_server where `season`>={0}", minSeasonID);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						int num = Convert.ToInt32(mySqlDataReader["serverid"]);
						if (num >= 0 && num < this.ZoneID2ServerIDs.Length)
						{
							int num2 = this.ZoneID2ServerIDs[num];
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
							if (!this.ServerInfoDict.TryGetValue(num2, out kuaFuLueDuoServerInfo))
							{
								kuaFuLueDuoServerInfo = new KuaFuLueDuoServerInfo
								{
									ServerId = num2
								};
								kuaFuLueDuoServerInfo.ZhengFuList = new List<int>();
								kuaFuLueDuoServerInfo.ShiChouList = new List<int>();
								this.ServerInfoDict[num2] = kuaFuLueDuoServerInfo;
							}
							int num3 = Convert.ToInt32(mySqlDataReader["sum_ziyuan"]);
							int lastZiYuan = Convert.ToInt32(mySqlDataReader["last_ziyuan"]);
							if (kuaFuLueDuoServerInfo.ZiYuan <= num3)
							{
								kuaFuLueDuoServerInfo.ZiYuan = num3;
								kuaFuLueDuoServerInfo.LastZiYuan = lastZiYuan;
								kuaFuLueDuoServerInfo.SeasonId = Convert.ToInt32(mySqlDataReader["season"]);
								kuaFuLueDuoServerInfo.MingXingZhanMengList = mySqlDataReader["mingxing"].ToString();
							}
							if (num != num2)
							{
								this.ExecuteSqlNoQuery(string.Format("update `t_kfld_server` set `season`=0 where `serverid`={0}", num));
							}
						}
					}
				}
				strSQL = string.Format("SELECT `serverid`,`fall_sid` FROM t_kfld_zhengfu where `season`>={0}", minSeasonID);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						int num = Convert.ToInt32(mySqlDataReader["serverid"]);
						int num4 = Convert.ToInt32(mySqlDataReader["fall_sid"]);
						if (num >= 0 && num < this.ZoneID2ServerIDs.Length && num4 >= 0 && num4 <= this.ZoneID2ServerIDs.Length)
						{
							int num5 = this.ZoneID2ServerIDs[num];
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
							if (this.ServerInfoDict.TryGetValue(num5, out kuaFuLueDuoServerInfo))
							{
								if (!kuaFuLueDuoServerInfo.ZhengFuList.Contains(num4))
								{
									kuaFuLueDuoServerInfo.ZhengFuList.Add(num4);
								}
								int key = this.ZoneID2ServerIDs[num4];
								if (this.ServerInfoDict.TryGetValue(key, out kuaFuLueDuoServerInfo))
								{
									if (!kuaFuLueDuoServerInfo.ShiChouList.Contains(num5))
									{
										kuaFuLueDuoServerInfo.ShiChouList.Add(num5);
									}
								}
							}
						}
					}
				}
				foreach (KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo in this.ServerInfoDict.Values)
				{
					KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
					if (kuaFuLueDuoServerInfo.SeasonId > 0)
					{
						string sqlCmd = string.Format("update t_kfld_server set `sum_ziyuan`={1}, `last_ziyuan`={2},`zhengfu`={3} where `serverid`={0}", new object[]
						{
							kuaFuLueDuoServerInfo.ServerId,
							kuaFuLueDuoServerInfo.ZiYuan,
							kuaFuLueDuoServerInfo.LastZiYuan,
							kuaFuLueDuoServerInfo.ZhengFuList.Count
						});
						this.ExecuteSqlNoQuery(sqlCmd);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return true;
		}

		private bool LoadKuaFuLueDuoBHData_Join(int season, Dictionary<int, KuaFuLueDuoServerJingJiaState> JingJiaDict)
		{
			try
			{
				KuaFuData<KuaFuLueDuoBHData> kuaFuData = null;
				string strSQL = string.Format("SELECT * FROM t_kfld_banghui_season WHERE `season`={0}", season);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					int bhid = Convert.ToInt32(mySqlDataReader["bhid"]);
					if (this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out kuaFuData))
					{
						int num = Convert.ToInt32(mySqlDataReader["jingjia_sid"]);
						int num2 = Convert.ToInt32(mySqlDataReader["jingjia"]);
						int last_jingjia = Convert.ToInt32(mySqlDataReader["last_jingjia"]);
						int group = Convert.ToInt32(mySqlDataReader["group"]);
						string bhName = mySqlDataReader["bhname"] as string;
						int zoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
						kuaFuData.V.jingjia = num2;
						kuaFuData.V.jingjia_sid = num;
						kuaFuData.V.last_jingjia = last_jingjia;
						kuaFuData.V.group = group;
						if (num > 0)
						{
							KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState;
							if (!JingJiaDict.TryGetValue(kuaFuData.V.jingjia_sid, out kuaFuLueDuoServerJingJiaState))
							{
								kuaFuLueDuoServerJingJiaState = new KuaFuLueDuoServerJingJiaState
								{
									ServerId = num,
									JingJiaList = new List<KuaFuLueDuoBangHuiJingJiaData>()
								};
								JingJiaDict[kuaFuData.V.jingjia_sid] = kuaFuLueDuoServerJingJiaState;
							}
							KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData = kuaFuLueDuoServerJingJiaState.JingJiaList.Find((KuaFuLueDuoBangHuiJingJiaData x) => x.BhId == bhid);
							if (kuaFuLueDuoBangHuiJingJiaData == null)
							{
								kuaFuLueDuoBangHuiJingJiaData = new KuaFuLueDuoBangHuiJingJiaData
								{
									BhId = bhid,
									BhName = bhName,
									ZoneId = zoneId,
									ServerId = num,
									ZiJin = num2
								};
								kuaFuLueDuoServerJingJiaState.JingJiaList.Add(kuaFuLueDuoBangHuiJingJiaData);
							}
							else
							{
								kuaFuLueDuoBangHuiJingJiaData.ZiJin = num2;
							}
						}
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

		private string FormatLoadKuaFuLueDuoRankSql(int rankType, int minSeasonID, int seasonIDLast)
		{
			int num = KuaFuLueDuoConsts.MatchRankLimit[rankType];
			string result = "";
			switch (rankType)
			{
			case 0:
				result = string.Format("SELECT `serverid` a,`zhengfu` b,`serverid` p1,`serverid` z FROM t_kfld_server WHERE season>={0} and `zhengfu`>0 ORDER BY `zhengfu` DESC,`serverid`;", minSeasonID);
				break;
			case 2:
				result = string.Format("SELECT `bhid` a,sum_ziyuan b,`bhname` p1, `zoneid` z FROM t_kfld_banghui WHERE season>={0} and `sum_ziyuan`>0 ORDER BY `sum_ziyuan` DESC,`bhid` LIMIT {1};", minSeasonID, num);
				break;
			case 3:
				result = string.Format("SELECT `bhid` a,last b,`bhname` p1, `zoneid` z FROM t_kfld_banghui WHERE season={0} and `last`>0 ORDER BY `last` DESC,`bhid` LIMIT {1};", seasonIDLast, num);
				break;
			case 4:
				result = string.Format("SELECT `rid` a,`kill` b,`rname` p1, `zoneid` z FROM t_kfld_role WHERE season>={0} and `kill`>0 ORDER BY `kill` DESC,rid LIMIT {1};", minSeasonID, num);
				break;
			case 5:
				result = string.Format("SELECT `rid` a,`last` b,`rname` p1, `zoneid` z FROM t_kfld_role WHERE season={0} and `last`>0 ORDER BY `last` DESC,rid LIMIT {1};", seasonIDLast, num);
				break;
			}
			return result;
		}

		public int GetRoleKillNum(int minSeasonID, long rid)
		{
			string sqlstring = string.Format("SELECT `kill` FROM t_kfld_role WHERE rid={0} and season>={1};", rid, minSeasonID);
			object single = DbHelperMySQL.GetSingle(sqlstring);
			int result;
			if (null == single)
			{
				result = 0;
			}
			else
			{
				result = Convert.ToInt32(single);
			}
			return result;
		}

		private bool LoadKuaFuLueDuoRankInfo(int rankType, int minSeasonID, int seasonLast, Dictionary<int, KuaFuLueDuoRankListData> KuaFuLueDuoRankInfoDict)
		{
			try
			{
				long age = TimeUtil.AgeByNow();
				int num = this.ZoneID2GroupIDs.Length;
				KuaFuLueDuoRankListData[] array = new KuaFuLueDuoRankListData[num];
				List<KuaFuLueDuoRankInfo>[] array2 = new List<KuaFuLueDuoRankInfo>[num];
				for (int i = 0; i < this.ZoneID2GroupIDs.Length; i++)
				{
					int key = this.ZoneID2GroupIDs[i];
					KuaFuLueDuoRankListData kuaFuLueDuoRankListData;
					if (!KuaFuLueDuoRankInfoDict.TryGetValue(key, out kuaFuLueDuoRankListData))
					{
						kuaFuLueDuoRankListData = new KuaFuLueDuoRankListData
						{
							Age = age
						};
						kuaFuLueDuoRankListData.LastInfoDict = new Dictionary<int, KuaFuLueDuoRankInfo>();
						kuaFuLueDuoRankListData.SelfInfoDict = new Dictionary<int, KuaFuLueDuoRankInfo>();
						KuaFuLueDuoRankInfoDict[key] = kuaFuLueDuoRankListData;
					}
					List<KuaFuLueDuoRankInfo> list;
					if (!kuaFuLueDuoRankListData.ListDict.TryGetValue(rankType, out list))
					{
						list = new List<KuaFuLueDuoRankInfo>();
						kuaFuLueDuoRankListData.ListDict[rankType] = list;
					}
					array[i] = kuaFuLueDuoRankListData;
					array2[i] = list;
				}
				string text = this.FormatLoadKuaFuLueDuoRankSql(rankType, minSeasonID, seasonLast);
				if (!string.IsNullOrEmpty(text))
				{
					int num2 = 0;
					using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(text, false))
					{
						while (mySqlDataReader != null && mySqlDataReader.Read())
						{
							int num3 = Convert.ToInt32(mySqlDataReader[3]);
							if (num3 < num)
							{
								num2++;
								KuaFuLueDuoRankListData kuaFuLueDuoRankListData = array[num3];
								KuaFuLueDuoRankInfo kuaFuLueDuoRankInfo = new KuaFuLueDuoRankInfo();
								int serverID = Convert.ToInt32(mySqlDataReader[3]);
								kuaFuLueDuoRankInfo.Key = Convert.ToInt32(mySqlDataReader[0]);
								kuaFuLueDuoRankInfo.Value = Convert.ToInt32(mySqlDataReader[1]);
								kuaFuLueDuoRankInfo.Param1 = mySqlDataReader[2].ToString();
								switch (rankType)
								{
								case 0:
									array2[num3].Add(kuaFuLueDuoRankInfo);
									if (num2 == 1)
									{
										kuaFuLueDuoRankListData.LastInfoDict[0] = kuaFuLueDuoRankInfo;
									}
									break;
								case 2:
								case 4:
									kuaFuLueDuoRankInfo.Param1 = KuaFuServerManager.FormatName(kuaFuLueDuoRankInfo.Param1, serverID);
									array2[num3].Add(kuaFuLueDuoRankInfo);
									break;
								case 3:
								case 5:
									kuaFuLueDuoRankInfo.Param1 = KuaFuServerManager.FormatName(kuaFuLueDuoRankInfo.Param1, serverID);
									kuaFuLueDuoRankListData.LastInfoDict[rankType - 1] = kuaFuLueDuoRankInfo;
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				return false;
			}
			return true;
		}

		public static readonly KuaFuLueDuoPersistence Instance = new KuaFuLueDuoPersistence();

		public object Mutex = new object();

		public int SeasonCount;

		public Dictionary<int, KuaFuLueDuoServerInfo> ServerInfoDict = new Dictionary<int, KuaFuLueDuoServerInfo>();

		public int[] ZoneID2GroupIDs = new int[0];

		public int[] ZoneID2ServerIDs = new int[0];

		public Dictionary<int, KuaFuLueDuoRankListData> KuaFuLueDuoRankInfoDict = new Dictionary<int, KuaFuLueDuoRankListData>();

		public Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict = new Dictionary<int, KuaFuData<KuaFuLueDuoBHData>>();

		public Dictionary<int, KuaFuLueDuoServerJingJiaState> JingJiaDict = new Dictionary<int, KuaFuLueDuoServerJingJiaState>();

		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
