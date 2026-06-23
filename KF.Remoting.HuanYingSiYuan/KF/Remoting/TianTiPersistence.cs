using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class TianTiPersistence
	{
		private TianTiPersistence()
		{
		}

		public void InitConfig()
		{
			try
			{
				XElement xelement = ConfigHelper.Load("config.xml");
				this.SignUpWaitSecs3 = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SignUpWaitSecs3", "value", 10L);
				this.SignUpWaitSecsAll = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "SignUpWaitSecsAll", "value", 15L);
				this.RankData.MaxPaiMingRank = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "MaxPaiMingRank", "value", 50000L);
				this.MaxSendDetailDataCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "MaxSendDetailDataCount", "value", 100L);
				this.MaxRolePairFightCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "MaxRolePairFightCount", "value", 3L);
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(id),0) FROM t_tianti_game_fuben;"));
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public bool AddDelayWriteSql(string sql, List<Tuple<string, byte[]>> imgList = null, Action<object, int> callback = null)
		{
			bool result;
			lock (this.Mutex)
			{
				if (this.ServerStopping)
				{
					result = false;
				}
				else
				{
					this.DelayWriteSqlQueue.Enqueue(new Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>(sql, imgList, callback));
					result = true;
				}
			}
			return result;
		}

		public void DelayWriteDataProc()
		{
			List<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>> list = null;
			lock (this.Mutex)
			{
				if (this.DelayWriteSqlQueue.Count == 0)
				{
					return;
				}
				list = this.DelayWriteSqlQueue.ToList<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>>();
				this.DelayWriteSqlQueue.Clear();
			}
			foreach (Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>> tuple in list)
			{
				try
				{
					LogManager.WriteLog(3, tuple.Item1, null, true);
					int arg;
					if (null == tuple.Item2)
					{
						arg = DbHelperMySQL.ExecuteSql(tuple.Item1);
					}
					else
					{
						arg = DbHelperMySQL.ExecuteSqlInsertImg(tuple.Item1, tuple.Item2);
					}
					if (tuple.Item3 != null)
					{
						tuple.Item3(tuple, arg);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("sql: {0}\r\n{1}", tuple, ex.ToString()));
				}
			}
		}

		public void OnStopServer()
		{
			this.ServerStopping = true;
			this.DelayWriteDataProc();
		}

		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(1, "TianTi 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		public TianTiRankData GetTianTiRankData(DateTime modifyTime)
		{
			TianTiRankData tianTiRankData = new TianTiRankData();
			lock (this.Mutex)
			{
				tianTiRankData.ModifyTime = this.RankData.ModifyTime;
				tianTiRankData.MaxPaiMingRank = this.RankData.MaxPaiMingRank;
				if (modifyTime < this.RankData.ModifyTime && null != this.RankData.TianTiRoleInfoDataList)
				{
					tianTiRankData.TianTiRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiRoleInfoDataList);
				}
				if (modifyTime < this.RankData.ModifyTime && null != this.RankData.TianTiMonthRoleInfoDataList)
				{
					tianTiRankData.TianTiMonthRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiMonthRoleInfoDataList);
				}
			}
			return tianTiRankData;
		}

		private bool ReloadTianTiRankDayList(List<TianTiRoleInfoData> tianTiRoleInfoDataList)
		{
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_roles where duanweijifen>0 ORDER BY duanweijifen DESC,duanweirank DESC LIMIT {0};", this.RankData.MaxPaiMingRank);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
					tianTiRoleInfoData.RoleId = Convert.ToInt32(mySqlDataReader["rid"]);
					if (num <= this.MaxSendDetailDataCount)
					{
						tianTiRoleInfoData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
						tianTiRoleInfoData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
						tianTiRoleInfoData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]);
						tianTiRoleInfoData.ZhanLi = Convert.ToInt32(mySqlDataReader["zhanli"]);
						tianTiRoleInfoData.RoleName = mySqlDataReader["rname"].ToString();
						if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
						{
							tianTiRoleInfoData.TianTiPaiHangRoleData = (byte[])mySqlDataReader["data1"];
						}
					}
					tianTiRoleInfoData.DuanWeiRank = num;
					tianTiRoleInfoDataList.Add(tianTiRoleInfoData);
					num++;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return false;
		}

		private bool LoadTianTiRankDayList(List<TianTiRoleInfoData> tianTiRoleInfoDataList)
		{
			bool result = false;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT r.rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_roles r, t_tianti_day_paihang d WHERE r.rid=d.rid ORDER BY d.`rank` ASC LIMIT {0};", this.RankData.MaxPaiMingRank);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				int num = 1;
				while (mySqlDataReader.Read())
				{
					TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
					tianTiRoleInfoData.RoleId = Convert.ToInt32(mySqlDataReader["rid"]);
					if (num <= this.MaxSendDetailDataCount)
					{
						tianTiRoleInfoData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
						tianTiRoleInfoData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
						tianTiRoleInfoData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]);
						tianTiRoleInfoData.ZhanLi = Convert.ToInt32(mySqlDataReader["zhanli"]);
						tianTiRoleInfoData.RoleName = mySqlDataReader["rname"].ToString();
						if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
						{
							tianTiRoleInfoData.TianTiPaiHangRoleData = (byte[])mySqlDataReader["data1"];
						}
					}
					tianTiRoleInfoData.DuanWeiRank = num;
					tianTiRoleInfoDataList.Add(tianTiRoleInfoData);
					result = true;
					num++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
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

		public void LoadTianTiRankData(DateTime now)
		{
			try
			{
				this.ExecuteSqlNoQuery("INSERT IGNORE INTO t_async(`id`,`value`) VALUES(4,1);");
				object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 4);
				if (null != single)
				{
					int num = (int)single;
					DateTime realDate = DataHelper2.GetRealDate(num);
					List<TianTiRoleInfoData> tianTiRoleInfoDataList = new List<TianTiRoleInfoData>();
					List<TianTiRoleInfoData> list = new List<TianTiRoleInfoData>();
					MySqlDataReader mySqlDataReader = null;
					try
					{
						this.LoadTianTiRankDayList(tianTiRoleInfoDataList);
						mySqlDataReader = DbHelperMySQL.ExecuteReader(string.Format("SELECT rid,rname,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,data1,data2 FROM t_tianti_month_paihang ORDER BY `duanweirank` ASC LIMIT {0};", this.RankData.MaxPaiMingRank), false);
						int num2 = 1;
						while (mySqlDataReader.Read())
						{
							TianTiRoleInfoData tianTiRoleInfoData = new TianTiRoleInfoData();
							tianTiRoleInfoData.RoleId = Convert.ToInt32(mySqlDataReader["rid"]);
							if (num2 <= this.MaxSendDetailDataCount)
							{
								tianTiRoleInfoData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
								tianTiRoleInfoData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]);
								tianTiRoleInfoData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]);
								tianTiRoleInfoData.ZhanLi = Convert.ToInt32(mySqlDataReader["zhanli"]);
								tianTiRoleInfoData.RoleName = mySqlDataReader["rname"].ToString();
								if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("data1")))
								{
									tianTiRoleInfoData.TianTiPaiHangRoleData = (byte[])mySqlDataReader["data1"];
								}
							}
							tianTiRoleInfoData.DuanWeiRank = num2;
							list.Add(tianTiRoleInfoData);
							num2++;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
					finally
					{
						if (null != mySqlDataReader)
						{
							mySqlDataReader.Close();
						}
					}
					lock (this.Mutex)
					{
						this.RankData.ModifyTime = realDate;
						this.RankData.TianTiRoleInfoDataList = tianTiRoleInfoDataList;
						this.RankData.TianTiMonthRoleInfoDataList = list;
					}
					if (DataHelper2.GetOffsetDay(now) != num)
					{
						this.UpdateTianTiRankData(now, realDate.Month != now.Month, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void UpdateTianTiRankData(DateTime now, bool monthRank = false, bool force = false)
		{
			if (Monitor.TryEnter(this.MutexPaiHang))
			{
				try
				{
					if (!force)
					{
						lock (this.Mutex)
						{
							if (this.RankData.ModifyTime.DayOfYear == now.DayOfYear)
							{
								return;
							}
						}
					}
					if (!monthRank)
					{
						if (now.Day == 1)
						{
							monthRank = true;
						}
					}
					List<TianTiRoleInfoData> list = new List<TianTiRoleInfoData>();
					string text = "";
					MySqlDataReader mySqlDataReader = null;
					try
					{
						this.ReloadTianTiRankDayList(list);
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
						return;
					}
					finally
					{
						if (null != mySqlDataReader)
						{
							mySqlDataReader.Close();
						}
					}
					try
					{
						if (list.Count > 0)
						{
							int num = DbHelperMySQL.ExecuteSql(string.Format("UPDATE t_tianti_roles SET `duanweirank`={0}", this.RankData.MaxPaiMingRank + 1));
							if (num >= 0)
							{
								num = DbHelperMySQL.ExecuteSql("DELETE FROM t_tianti_day_paihang;");
							}
							if (num >= 0)
							{
								int count = list.Count;
								int num2 = 50;
								for (int i = 0; i < count; i++)
								{
									if (i % num2 == 0)
									{
										text = "INSERT INTO t_tianti_day_paihang(rid,rank) VALUES";
									}
									text += string.Format("({0},{1})", list[i].RoleId, list[i].DuanWeiRank);
									if (i % num2 == num2 - 1 || i == count - 1)
									{
										DbHelperMySQL.ExecuteSql(text);
									}
									else
									{
										text += ',';
									}
								}
								DbHelperMySQL.ExecuteSql("UPDATE t_tianti_roles r, t_tianti_day_paihang d SET r.`duanweirank` = d.`rank` WHERE r.`rid` = d.`rid`;");
								if (monthRank)
								{
									DbHelperMySQL.ExecuteSql("DELETE FROM t_tianti_month_paihang;");
									text = "INSERT INTO t_tianti_month_paihang SELECT * FROM t_tianti_roles WHERE rid IN (SELECT rid FROM t_tianti_day_paihang) ORDER BY `duanweirank` ASC;";
									DbHelperMySQL.ExecuteSql(text);
									DbHelperMySQL.ExecuteSql("DELETE FROM t_tianti_day_paihang;");
									DbHelperMySQL.ExecuteSql("UPDATE t_tianti_roles SET `duanweirank`=0,`duanweijifen`=0,`duanweiid`=0;");
								}
							}
							if (num >= 0)
							{
								text = string.Format("UPDATE t_async SET `value`={1} WHERE `id`={0};", 4, DataHelper2.GetOffsetDay(now));
								this.ExecuteSqlNoQuery(text);
							}
						}
						lock (this.Mutex)
						{
							this.RankData.ModifyTime = now;
							if (monthRank)
							{
								this.RankData.TianTiRoleInfoDataList = new List<TianTiRoleInfoData>();
								this.RankData.TianTiMonthRoleInfoDataList = list;
							}
							else
							{
								this.RankData.TianTiRoleInfoDataList = list;
							}
						}
						if (monthRank)
						{
							try
							{
								ZhengBaManagerK.Instance().ReloadSyncData(now);
							}
							catch (Exception ex)
							{
								LogManager.WriteLog(2, "UpdateTianTiRankData -> zhengba reload execption", ex, true);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
				finally
				{
					Monitor.Exit(this.MutexPaiHang);
				}
			}
		}

		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			if (this.TianTiRoleInfoDataQueue.Count > 100000)
			{
				TianTiRoleInfoData tianTiRoleInfoData;
				this.TianTiRoleInfoDataQueue.TryDequeue(out tianTiRoleInfoData);
			}
			this.TianTiRoleInfoDataQueue.Enqueue(data);
		}

		public void WriteRoleInfoDataToDb(TianTiRoleInfoData data)
		{
			try
			{
				List<Tuple<string, byte[]>> list = new List<Tuple<string, byte[]>>();
				list.Add(new Tuple<string, byte[]>("content", data.TianTiPaiHangRoleData));
				list.Add(new Tuple<string, byte[]>("mirror", data.PlayerJingJiMirrorData));
				DbHelperMySQL.ExecuteSqlInsertImg(string.Format("INSERT INTO t_tianti_roles(rid,zoneid,duanweiid,duanweijifen,duanweirank,zhanli,rname,data1,data2) VALUES({0},{1},{2},{3},{4},{5},'{6}',@content,@mirror) ON DUPLICATE KEY UPDATE `zoneid`={1},`duanweiid`={2},`duanweijifen`={3},`duanweirank`={4},`zhanli`={5},`rname`='{6}',`data1`=@content,`data2`=@mirror;", new object[]
				{
					data.RoleId,
					data.ZoneId,
					data.DuanWeiId,
					data.DuanWeiJiFen,
					data.DuanWeiRank,
					data.ZhanLi,
					data.RoleName
				}), list);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void WriteRoleInfoDataProc()
		{
			TianTiRoleInfoData data;
			for (int i = 0; i < 1000; i++)
			{
				if (!this.TianTiRoleInfoDataQueue.TryDequeue(out data))
				{
					break;
				}
				this.WriteRoleInfoDataToDb(data);
				lock (this.Mutex)
				{
					if (this.RankData.TianTiRoleInfoDataList.Count < 3)
					{
						if (!this.RankData.TianTiRoleInfoDataList.Exists((TianTiRoleInfoData x) => x.RoleId == data.RoleId))
						{
							this.RankData.ModifyTime = TimeUtil.NowDateTime();
							this.RankData.TianTiRoleInfoDataList.Add(data);
							data.DuanWeiRank = this.RankData.TianTiRoleInfoDataList.Count;
						}
					}
				}
			}
		}

		public int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result;
			try
			{
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -15;
			}
			return result;
		}

		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		public void LogCreateTianTiFuBen(int gameId, int serverId, int fubenSeqId, int roleCount)
		{
			string sqlCmd = string.Format("INSERT INTO t_tianti_game_fuben(`id`,`serverid`,`fubensid`,`createtime`,`rolenum`) VALUES({0},{1},{2},'{3}',{4});", new object[]
			{
				gameId,
				serverId,
				fubenSeqId,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
				roleCount
			});
			this.ExecuteSqlNoQuery(sqlCmd);
		}

		public bool LoadZhanDuiData(Dictionary<int, TianTi5v5ZhanDuiData> dict)
		{
			bool result = false;
			string strSQL = "select `zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`duanweijifen`,`duanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`,`monthduanweirank`,leaderrolename,zoneid,zorkjifen,zorklastfighttime,escapejifen,escapelastfighttime from t_kf_5v5_zhandui";
			using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
			{
				result = true;
				while (mySqlDataReader.Read())
				{
					try
					{
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = new TianTi5v5ZhanDuiData();
						tianTi5v5ZhanDuiData.ZhanDuiID = Convert.ToInt32(mySqlDataReader[0].ToString());
						tianTi5v5ZhanDuiData.LeaderRoleID = Convert.ToInt32(mySqlDataReader["leaderid"].ToString());
						tianTi5v5ZhanDuiData.XuanYan = mySqlDataReader["xuanyan"].ToString();
						tianTi5v5ZhanDuiData.ZhanDuiName = mySqlDataReader["zhanduiname"].ToString();
						tianTi5v5ZhanDuiData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"].ToString());
						tianTi5v5ZhanDuiData.ZhanDouLi = Convert.ToInt64(mySqlDataReader["zhanli"].ToString());
						byte[] array = (mySqlDataReader["data1"] as byte[]) ?? new byte[0];
						tianTi5v5ZhanDuiData.teamerList = DataHelper2.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(array, 0, array.Length);
						tianTi5v5ZhanDuiData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"].ToString());
						tianTi5v5ZhanDuiData.DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"].ToString());
						tianTi5v5ZhanDuiData.LianSheng = Convert.ToInt32(mySqlDataReader["liansheng"].ToString());
						tianTi5v5ZhanDuiData.FightCount = Convert.ToInt32(mySqlDataReader["fightcount"].ToString());
						tianTi5v5ZhanDuiData.SuccessCount = Convert.ToInt32(mySqlDataReader["successcount"].ToString());
						tianTi5v5ZhanDuiData.LastFightTime = Convert.ToDateTime(mySqlDataReader["lastfighttime"].ToString());
						tianTi5v5ZhanDuiData.MonthDuanWeiRank = Convert.ToInt32(mySqlDataReader["monthduanweirank"].ToString());
						tianTi5v5ZhanDuiData.LeaderRoleName = mySqlDataReader["leaderrolename"].ToString();
						tianTi5v5ZhanDuiData.ZoneID = Convert.ToInt32(mySqlDataReader["zoneid"].ToString());
						tianTi5v5ZhanDuiData.ZorkJiFen = Convert.ToInt32(mySqlDataReader["zorkjifen"].ToString());
						tianTi5v5ZhanDuiData.ZorkLastFightTime = Convert.ToDateTime(mySqlDataReader["zorklastfighttime"].ToString());
						tianTi5v5ZhanDuiData.EscapeJiFen = Convert.ToInt32(mySqlDataReader["escapejifen"].ToString());
						tianTi5v5ZhanDuiData.EscapeLastFightTime = Convert.ToDateTime(mySqlDataReader["escapelastfighttime"].ToString());
						dict[tianTi5v5ZhanDuiData.ZhanDuiID] = tianTi5v5ZhanDuiData;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			return result;
		}

		public int LoadZhanDuiRankList(List<TianTi5v5ZhanDuiData> list, int dayID)
		{
			int result = 0;
			string strSQL = string.Format("select `zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`duanweijifen`,`duanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`,`monthduanweirank`,leaderrolename,zoneid from t_kf_5v5_zhandui_paihang where dayid={0} order by duanweirank", dayID);
			using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
			{
				result = 1;
				while (mySqlDataReader.Read())
				{
					try
					{
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = new TianTi5v5ZhanDuiData();
						tianTi5v5ZhanDuiData.ZhanDuiID = Convert.ToInt32(mySqlDataReader[0].ToString());
						tianTi5v5ZhanDuiData.LeaderRoleID = Convert.ToInt32(mySqlDataReader["leaderid"].ToString());
						tianTi5v5ZhanDuiData.XuanYan = mySqlDataReader["xuanyan"].ToString();
						tianTi5v5ZhanDuiData.ZhanDuiName = mySqlDataReader["zhanduiname"].ToString();
						tianTi5v5ZhanDuiData.DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"].ToString());
						tianTi5v5ZhanDuiData.ZhanDouLi = Convert.ToInt64(mySqlDataReader["zhanli"].ToString());
						byte[] array = (mySqlDataReader["data1"] as byte[]) ?? new byte[0];
						tianTi5v5ZhanDuiData.teamerList = DataHelper2.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(array, 0, array.Length);
						tianTi5v5ZhanDuiData.DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"].ToString());
						tianTi5v5ZhanDuiData.DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"].ToString());
						tianTi5v5ZhanDuiData.LianSheng = Convert.ToInt32(mySqlDataReader["liansheng"].ToString());
						tianTi5v5ZhanDuiData.FightCount = Convert.ToInt32(mySqlDataReader["fightcount"].ToString());
						tianTi5v5ZhanDuiData.SuccessCount = Convert.ToInt32(mySqlDataReader["successcount"].ToString());
						tianTi5v5ZhanDuiData.LastFightTime = Convert.ToDateTime(mySqlDataReader["lastfighttime"].ToString());
						tianTi5v5ZhanDuiData.MonthDuanWeiRank = Convert.ToInt32(mySqlDataReader["monthduanweirank"].ToString());
						tianTi5v5ZhanDuiData.LeaderRoleName = mySqlDataReader["leaderrolename"].ToString();
						tianTi5v5ZhanDuiData.ZoneID = Convert.ToInt32(mySqlDataReader["zoneid"].ToString());
						list.Add(tianTi5v5ZhanDuiData);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			return result;
		}

		public void UpdateZhanDuiDayRank(List<TianTi5v5ZhanDuiData> list, int dayId, int maxRankCount, bool updateMonthRank)
		{
			try
			{
				this.AddDelayWriteSql("delete from t_kf_5v5_zhandui_paihang where dayid=" + dayId, null, null);
				int num = 0;
				while (num < maxRankCount && num < list.Count)
				{
					TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = list[num];
					string sql = string.Format("REPLACE INTO t_kf_5v5_zhandui_paihang (`dayid`,`zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`leaderrolename`,`zoneid`,`zhanli`,`data1`,`data2`,`duanweiid`,`duanweijifen`,`duanweirank`,`monthduanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`) VALUES({15},{0},{1},'{2}','{3}','{4}',{5},{6},@data1,@data2,{7},{8},{9},{10},{11},{12},{13},'{14}');", new object[]
					{
						tianTi5v5ZhanDuiData.ZhanDuiID,
						tianTi5v5ZhanDuiData.LeaderRoleID,
						tianTi5v5ZhanDuiData.XuanYan,
						tianTi5v5ZhanDuiData.ZhanDuiName,
						tianTi5v5ZhanDuiData.LeaderRoleName,
						tianTi5v5ZhanDuiData.ZoneID,
						tianTi5v5ZhanDuiData.ZhanDouLi,
						tianTi5v5ZhanDuiData.DuanWeiId,
						tianTi5v5ZhanDuiData.DuanWeiJiFen,
						tianTi5v5ZhanDuiData.DuanWeiRank,
						tianTi5v5ZhanDuiData.MonthDuanWeiRank,
						tianTi5v5ZhanDuiData.LianSheng,
						tianTi5v5ZhanDuiData.FightCount,
						tianTi5v5ZhanDuiData.SuccessCount,
						tianTi5v5ZhanDuiData.LastFightTime,
						dayId
					});
					this.AddDelayWriteSql(sql, new List<Tuple<string, byte[]>>
					{
						new Tuple<string, byte[]>("@data1", DataHelper2.ObjectToBytes<List<TianTi5v5ZhanDuiRoleData>>(tianTi5v5ZhanDuiData.teamerList)),
						new Tuple<string, byte[]>("@data2", null)
					}, null);
					num++;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void UpdateZhanDuiRankData(List<TianTi5v5ZhanDuiData> list, int dayId, int maxRankCount, bool updateMonthRank)
		{
			try
			{
				if (updateMonthRank)
				{
					this.AddDelayWriteSql(string.Format("update t_kf_5v5_zhandui set `duanweirank`={0},`monthduanweirank`={1}", maxRankCount + 1, maxRankCount + 1), null, null);
					int num = 0;
					while (num < maxRankCount && num < list.Count)
					{
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = list[num];
						string sql = string.Format("update t_kf_5v5_zhandui set `duanweirank`={1},`monthduanweirank`={2}", tianTi5v5ZhanDuiData.ZhanDuiID, tianTi5v5ZhanDuiData.DuanWeiRank, tianTi5v5ZhanDuiData.MonthDuanWeiRank);
						this.AddDelayWriteSql(sql, null, null);
						num++;
					}
				}
				else
				{
					this.AddDelayWriteSql(string.Format("update t_kf_5v5_zhandui set `duanweirank`={0}", maxRankCount + 1), null, null);
					int num = 0;
					while (num < maxRankCount && num < list.Count)
					{
						TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = list[num];
						string sql = string.Format("update t_kf_5v5_zhandui set `duanweirank`={1} where zhanduiid={0}", tianTi5v5ZhanDuiData.ZhanDuiID, tianTi5v5ZhanDuiData.DuanWeiRank);
						this.AddDelayWriteSql(sql, null, null);
						num++;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public int InitZhanDui(TianTi5v5ZhanDuiData data)
		{
			string text = string.Format("INSERT INTO t_kf_5v5_zhandui(`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`leaderrolename`,zoneid) VALUES('{1}','{2}','{3}','{4}','{5}',@content,'{6}',{7});", new object[]
			{
				data.ZhanDuiID,
				data.LeaderRoleID,
				data.XuanYan,
				data.ZhanDuiName,
				data.DuanWeiId,
				data.ZhanDouLi,
				data.LeaderRoleName,
				data.ZoneID
			});
			List<Tuple<string, byte[]>> list = new List<Tuple<string, byte[]>>();
			list.Add(new Tuple<string, byte[]>("@content", DataHelper2.ObjectToBytes<List<TianTi5v5ZhanDuiRoleData>>(data.teamerList)));
			LogManager.WriteLog(3, text, null, true);
			return (int)DbHelperMySQL.ExecuteSqlGetIncrement(text, list);
		}

		public int UpdateZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("update t_kf_5v5_zhandui set `leaderid`={1},`xuanyan`='{2}',`zhanduiname`='{3}',`duanweiid`='{4}',`zhanli`='{5}',`data1`=@data1,leaderrolename='{6}',`liansheng`='{7}',`fightcount`='{8}',`successcount`='{9}',lastfighttime='{10}',`duanweijifen`='{11}',`duanweirank`='{12}',`monthduanweirank`='{13}' where `zhanduiid`={0};", new object[]
			{
				data.ZhanDuiID,
				data.LeaderRoleID,
				data.XuanYan,
				data.ZhanDuiName,
				data.DuanWeiId,
				data.ZhanDouLi,
				data.LeaderRoleName,
				data.LianSheng,
				data.FightCount,
				data.SuccessCount,
				data.LastFightTime,
				data.DuanWeiJiFen,
				data.DuanWeiRank,
				data.MonthDuanWeiRank
			});
			int result;
			if (!this.AddDelayWriteSql(sql, new List<Tuple<string, byte[]>>
			{
				new Tuple<string, byte[]>("@data1", DataHelper2.ObjectToBytes<List<TianTi5v5ZhanDuiRoleData>>(data.teamerList))
			}, null))
			{
				result = -11000;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public void UpdateZhanDuiXuanYan(long teamId, string xuanyan)
		{
			string sql = string.Format("UPDATE t_kf_5v5_zhandui SET xuanyan='{1}' WHERE zhanduiid={0};", teamId, xuanyan);
			this.AddDelayWriteSql(sql, null, null);
		}

		public int DeleteZhanDui(long teamid)
		{
			string text = string.Format("DELETE FROM t_kf_5v5_zhandui WHERE zhanduiid={0};", teamid);
			LogManager.WriteLog(3, text, null, true);
			return this.ExecuteSqlNoQuery(text);
		}

		public int ClearZorkBattleRoleData()
		{
			string sqlCmd = string.Format("UPDATE `t_kf_5v5_zhandui_roles` SET `zorkkill`=0;", new object[0]);
			return this.ExecuteSqlNoQuery(sqlCmd);
		}

		public void UpdateZorkBattleRoleData(ZorkBattleRoleInfo roleData, bool chgKill = true)
		{
			string sql = string.Format("INSERT INTO `t_kf_5v5_zhandui_roles`(`rid`, rname, zoneid, reborncount, rebornlev, `zorkkill`) VALUES({0},'{1}',{2},{3},{4},{5}) ON DUPLICATE KEY UPDATE reborncount={3}, rebornlev={4}, `zorkkill`=`zorkkill`+{5};", new object[]
			{
				roleData.RoleID,
				roleData.Name,
				roleData.ZoneID,
				roleData.RebornCount,
				roleData.RebornLevel,
				roleData.KillRoleNum
			});
			this.AddDelayWriteSql(sql, null, null);
			if (chgKill)
			{
				sql = string.Format("UPDATE t_kf_5v5_zhandui_roles SET `ranktm_zorkkill`=NOW() WHERE `rid`={0};", roleData.RoleID);
				this.AddDelayWriteSql(sql, null, null);
			}
		}

		public int UpdateZorkZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("update t_kf_5v5_zhandui set zorkjifen={1}, zorklastfighttime='{2}' where `zhanduiid`={0};", data.ZhanDuiID, data.ZorkJiFen, data.ZorkLastFightTime.ToString("yyyy-MM-dd HH:mm:ss"));
			int result;
			if (!this.AddDelayWriteSql(sql, null, null))
			{
				result = -11000;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int LoadZorkSeasonID()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, 0));
			object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 52);
			return Convert.ToInt32(single);
		}

		public void SaveZorkSeasonID(int seasonID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, seasonID));
		}

		public int LoadZorkTopZhanDui()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, 0));
			object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 53);
			return Convert.ToInt32(single);
		}

		public void SaveZorkTopZhanDui(int zhanduiID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 53, zhanduiID));
		}

		public int LoadZorkTopKiller()
		{
			DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 52, 0));
			object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 54);
			return Convert.ToInt32(single);
		}

		public void SaveZorkTopKiller(int roleID)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 54, roleID));
		}

		private string FormatLoadZorkBattleRankSql(int rankType)
		{
			string result = "";
			if (rankType == 1)
			{
				result = string.Format("SELECT rid a, zorkkill b FROM `t_kf_5v5_zhandui_roles` WHERE zorkkill<>0 ORDER BY `zorkkill` DESC, `ranktm_zorkkill` ASC LIMIT {0};", 30);
			}
			return result;
		}

		public bool LoadZorkBattleRankInfo(int rankType, List<KFZorkRankInfo> rankList)
		{
			try
			{
				string text = this.FormatLoadZorkBattleRankSql(rankType);
				if (string.IsNullOrEmpty(text))
				{
					return false;
				}
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(text, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KFZorkRankInfo kfzorkRankInfo = new KFZorkRankInfo();
					kfzorkRankInfo.Key = Convert.ToInt32(mySqlDataReader["a"]);
					kfzorkRankInfo.Value = Convert.ToInt32(mySqlDataReader["b"]);
					if (rankType == 1)
					{
						string sqlstring = string.Format("SELECT zoneid,rname,rebornlev FROM `t_kf_5v5_zhandui_roles` WHERE rid={0};", kfzorkRankInfo.Key);
						object[] array;
						if (DbHelperMySQL.GetSingleValues(sqlstring, out array) >= 0)
						{
							kfzorkRankInfo.StrParam1 = KuaFuServerManager.FormatName(Convert.ToInt32(array[0]), array[1].ToString());
							kfzorkRankInfo.Param1 = Convert.ToInt32(array[2]);
						}
						rankList.Add(kfzorkRankInfo);
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

		public int UpdateEscapeZhanDui(TianTi5v5ZhanDuiData data)
		{
			string sql = string.Format("update t_kf_5v5_zhandui set escapejifen={1}, escapelastfighttime='{2}' where `zhanduiid`={0};", data.ZhanDuiID, data.EscapeJiFen, data.EscapeLastFightTime.ToString("yyyy-MM-dd HH:mm:ss"));
			int result;
			if (!this.AddDelayWriteSql(sql, null, null))
			{
				result = -11000;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static int CompareKFEscapeRankInfo(KFEscapeRankInfo x, KFEscapeRankInfo y)
		{
			int num;
			if (x != null && x != y)
			{
				num = y.Value - x.Value;
				if (num == 0)
				{
					num = x.Key - y.Key;
				}
			}
			else if (x == null)
			{
				num = 1;
			}
			else
			{
				num = -1;
			}
			return num;
		}

		public List<KFEscapeRankInfo> LoadEscapeRankData(int season)
		{
			List<KFEscapeRankInfo> list = new List<KFEscapeRankInfo>();
			try
			{
				string strSQL = string.Format("SELECT zhanduiid,zhanduiname,zoneid,duanweijifen,zhanli FROM t_zhandui_escape_paihang e where season={0}", season);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						list.Add(new KFEscapeRankInfo
						{
							Key = Convert.ToInt32(mySqlDataReader["zhanduiid"]),
							ZoneID = Convert.ToInt32(mySqlDataReader["zoneid"]),
							StrParam1 = mySqlDataReader["zhanduiname"].ToString(),
							Value = Convert.ToInt32(mySqlDataReader["duanweijifen"]),
							Param1 = (long)Convert.ToInt32(mySqlDataReader["zhanli"])
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			list.Sort(new Comparison<KFEscapeRankInfo>(TianTiPersistence.CompareKFEscapeRankInfo));
			return list;
		}

		public int GetAsyncInt(int id, int value)
		{
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", id, value));
				return (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 14);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return -1;
		}

		public bool SetAsyncInt(int id, int value)
		{
			try
			{
				string sqlstring = string.Format("update t_async set value={1} where id = {0}", id, value);
				return DbHelperMySQL.ExecuteSql(sqlstring) >= 0;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public bool BuildEscapeRankList(int offsetDay, DateTime minFightTime)
		{
			try
			{
				string sqlstring = string.Format("REPLACE INTO t_zhandui_escape_paihang SELECT {0},zhanduiid,zhanduiname,zoneid,escapejifen,zhanli FROM t_kf_5v5_zhandui WHERE escapelastfighttime>'{1}' ORDER BY duanweijifen DESC, zhanduiid;", offsetDay, minFightTime.ToString("yyyy-MM-dd"));
				return DbHelperMySQL.ExecuteSql(sqlstring) >= 0;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		private List<EscapeBattleZhanDuiData> LoadZhanDuiData()
		{
			List<EscapeBattleZhanDuiData> list = new List<EscapeBattleZhanDuiData>();
			try
			{
				string strSQL = string.Format("SELECT season,e.zhanduiid,zhanduiname,zoneid,e.duanweiid,e.duanweijifen,e.duanweirank,zhanli FROM t_zhandui_escape e left join t_kf_5v5_zhandui z on e.zhanduiid=z.zhanduiid order by e.duanweijifen desc,e.zhanduiid", new object[0]);
				using (MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false))
				{
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						list.Add(new EscapeBattleZhanDuiData
						{
							Season = Convert.ToInt32(mySqlDataReader["season"]),
							ZhanDuiID = Convert.ToInt32(mySqlDataReader["zhanduiid"]),
							ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]),
							ZhanDuiName = mySqlDataReader["zhanduiname"].ToString(),
							DuanWeiId = Convert.ToInt32(mySqlDataReader["duanweiid"]),
							DuanWeiRank = Convert.ToInt32(mySqlDataReader["duanweirank"]),
							DuanWeiJiFen = Convert.ToInt32(mySqlDataReader["duanweijifen"]),
							ZhanLi = Convert.ToInt64(mySqlDataReader["zhanli"])
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

		public static readonly TianTiPersistence Instance = new TianTiPersistence();

		public object Mutex = new object();

		public object MutexPaiHang = new object();

		public bool Initialized = false;

		public int SignUpWaitSecs1 = 5;

		public int SignUpWaitSecs3 = 10;

		public int SignUpWaitSecsAll = 15;

		public int WaitForJoinMaxSecs = 60;

		private int MaxSendDetailDataCount = 100;

		public int MaxRolePairFightCount = 3;

		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();

		public TianTiRankData RankData = new TianTiRankData();

		public ConcurrentQueue<TianTiRoleInfoData> TianTiRoleInfoDataQueue = new ConcurrentQueue<TianTiRoleInfoData>();

		public Queue<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>> DelayWriteSqlQueue = new Queue<Tuple<string, List<Tuple<string, byte[]>>, Action<object, int>>>();

		public bool ServerStopping;

		private int CurrGameId = Global.UninitGameId;
	}
}
