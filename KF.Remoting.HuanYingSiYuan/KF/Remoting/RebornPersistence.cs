using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class RebornPersistence
	{
		private RebornPersistence()
		{
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

		public bool LoadDatabase()
		{
			bool result;
			if (!this.LoadRebornRoleData(this.RebornRoleDataDict))
			{
				result = false;
			}
			else
			{
				for (int i = 0; i <= 3; i++)
				{
					if (!this.LoadRebornRankInfo(i, this.RebornRankDict))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		private string FormatLoadRebornRankSql(int rankType)
		{
			int[] array = new int[]
			{
				100,
				10,
				10,
				10
			};
			string result = "";
			switch (rankType)
			{
			case 0:
				result = string.Format("SELECT rid a, lev b, ptid c FROM t_reborn_roles ORDER BY `lev` DESC, `ranktm_lev` ASC, rid DESC LIMIT {0};", array[rankType]);
				break;
			case 1:
				result = string.Format("SELECT rid a, rarity_last b, ptid c FROM t_reborn_roles WHERE `rarity_last`<>0 ORDER BY `rarity_last` DESC, `ranktm_rl` ASC, rid DESC LIMIT {0};", array[rankType]);
				break;
			case 2:
				result = string.Format("SELECT `rid` a, boss_last b, ptid c FROM t_reborn_roles WHERE `boss_last`<>0 ORDER BY `boss_last` DESC, `ranktm_bl` ASC, rid DESC LIMIT {0};", array[rankType]);
				break;
			case 3:
				result = string.Format("SELECT rid a, liansha_last b, ptid c FROM t_reborn_roles WHERE `liansha_last`<>0 ORDER BY `liansha_last` DESC, `ranktm_lsl` ASC, rid DESC LIMIT {0};", array[rankType]);
				break;
			default:
				return result;
			}
			return result;
		}

		public int GetRebornDayID()
		{
			object single = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 49);
			return Convert.ToInt32(single);
		}

		public void SaveRebornDayID(int dayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 49, dayId));
		}

		public byte[] LoadRebornRoleData(int ptid, int rid)
		{
			byte[] result;
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("SELECT data1 FROM t_reborn_roles WHERE `ptid`={0} AND rid={1}", ptid, rid));
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

		public bool LoadRebornRankInfo(int rankType, KuaFuData<Dictionary<int, List<KFRebornRankInfo>>> RebornRankDict)
		{
			bool result;
			if (null == RebornRankDict)
			{
				result = false;
			}
			else
			{
				List<KFRebornRankInfo> list = null;
				if (!RebornRankDict.V.TryGetValue(rankType, out list))
				{
					list = (RebornRankDict.V[rankType] = new List<KFRebornRankInfo>());
				}
				else
				{
					list.Clear();
				}
				try
				{
					string text = this.FormatLoadRebornRankSql(rankType);
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(text, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KFRebornRankInfo kfrebornRankInfo = new KFRebornRankInfo();
						kfrebornRankInfo.Key = Convert.ToInt32(mySqlDataReader["a"]);
						kfrebornRankInfo.Value = Convert.ToInt32(mySqlDataReader["b"]);
						kfrebornRankInfo.PtID = Convert.ToInt32(mySqlDataReader["c"]);
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (this.RebornRoleDataDict.TryGetValue(new KeyValuePair<int, int>(kfrebornRankInfo.PtID, kfrebornRankInfo.Key), out kuaFuData))
						{
							string worldRoleID = ConstData.FormatWorldRoleID(kfrebornRankInfo.Key, kfrebornRankInfo.PtID);
							KuaFuWorldRoleData kuaFuWorldRoleData = TSingleton<KuaFuWorldManager>.getInstance().LoadKuaFuWorldRoleData(kfrebornRankInfo.Key, kfrebornRankInfo.PtID, worldRoleID);
							if (null != kuaFuWorldRoleData)
							{
								int serverID = ConstData.ConvertToKuaFuServerID(kuaFuWorldRoleData.ZoneID, kuaFuWorldRoleData.PTID);
								kfrebornRankInfo.Param1 = KuaFuServerManager.FormatName(kuaFuData.V.RoleName, serverID);
								kfrebornRankInfo.Param2 = kuaFuWorldRoleData.Channel;
								kfrebornRankInfo.UserID = kuaFuWorldRoleData.UserID;
								kfrebornRankInfo.tagInfo = kuaFuData;
							}
						}
						list.Add(kfrebornRankInfo);
					}
					if (null != RebornRankDict)
					{
						TimeUtil.AgeByNow(ref RebornRankDict.Age);
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

		private bool LoadRebornRoleData(Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict)
		{
			bool result;
			if (null == RebornRoleDataDict)
			{
				result = false;
			}
			else
			{
				try
				{
					string strSQL = string.Format("SELECT * FROM `t_reborn_roles`", new object[0]);
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KuaFuData<KFRebornRoleData> kuaFuData = new KuaFuData<KFRebornRoleData>();
						kuaFuData.V.PtID = Convert.ToInt32(mySqlDataReader["ptid"]);
						kuaFuData.V.RoleID = Convert.ToInt32(mySqlDataReader["rid"]);
						kuaFuData.V.Lev = Convert.ToInt32(mySqlDataReader["lev"]);
						kuaFuData.V.Rarity = Convert.ToInt32(mySqlDataReader["rarity"]);
						kuaFuData.V.RarityLast = Convert.ToInt32(mySqlDataReader["rarity_last"]);
						kuaFuData.V.Boss = Convert.ToInt32(mySqlDataReader["boss"]);
						kuaFuData.V.BossLast = Convert.ToInt32(mySqlDataReader["boss_last"]);
						kuaFuData.V.LianSha = Convert.ToInt32(mySqlDataReader["liansha"]);
						kuaFuData.V.LianShaLast = Convert.ToInt32(mySqlDataReader["liansha_last"]);
						kuaFuData.V.ParseBossAwardListString(Convert.ToString(mySqlDataReader["boss_award"]), kuaFuData.V.BossAwardList);
						kuaFuData.V.RoleName = Convert.ToString(mySqlDataReader["rname"]);
						string text = mySqlDataReader["ranktm_lev"].ToString();
						if (!string.IsNullOrEmpty(text))
						{
							DateTime.TryParse(text, out kuaFuData.V.RankTmLev);
						}
						text = mySqlDataReader["ranktm_r"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmR);
						text = mySqlDataReader["ranktm_rl"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmRL);
						text = mySqlDataReader["ranktm_b"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmB);
						text = mySqlDataReader["ranktm_bl"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmBL);
						text = mySqlDataReader["ranktm_ls"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmLS);
						text = mySqlDataReader["ranktm_lsl"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmLSL);
						RebornRoleDataDict[new KeyValuePair<int, int>(kuaFuData.V.PtID, kuaFuData.V.RoleID)] = kuaFuData;
						kuaFuData.V.RoleData4Selector = (mySqlDataReader["data1"] as byte[]);
						TimeUtil.AgeByNow(ref kuaFuData.Age);
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

		public void UpdateRebornRoleDataBossAward(KFRebornRoleData roleData)
		{
			string sql = string.Format("UPDATE t_reborn_roles SET boss_award='{2}' WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID, roleData.FormatBossAwardString(roleData.BossAwardList));
			this.AddDelayWriteSql(sql);
		}

		public void UpdateRebornRoleDataRoleName(KFRebornRoleData roleData)
		{
			string sql = string.Format("UPDATE t_reborn_roles SET rname='{2}' WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID, roleData.RoleName);
			this.AddDelayWriteSql(sql);
		}

		public void UpdateRebornRoleData4Selector(KFRebornRoleData roleData)
		{
			string strSQL = string.Format("UPDATE t_reborn_roles SET data1=@content WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
			{
				new Tuple<string, byte[]>("content", roleData.RoleData4Selector)
			});
		}

		public void UpdateRebornRoleData(KFRebornRoleData roleData, int chgMask, bool delay = true)
		{
			string text = string.Format("UPDATE t_reborn_roles SET lev={2}, rarity={3}, rarity_last={4}, boss={5}, boss_last={6}, liansha={7}, liansha_last={8} WHERE ptid={0} AND rid={1};", new object[]
			{
				roleData.PtID,
				roleData.RoleID,
				roleData.Lev,
				roleData.Rarity,
				roleData.RarityLast,
				roleData.Boss,
				roleData.BossLast,
				roleData.LianSha,
				roleData.LianShaLast
			});
			if ((chgMask & 1) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_lev=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 2) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_r=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 4) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_rl=ranktm_r WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 8) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_b=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 16) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_bl=ranktm_b WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 32) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_ls=NOW() WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if ((chgMask & 64) > 0)
			{
				text += string.Format("UPDATE t_reborn_roles SET ranktm_lsl=ranktm_ls WHERE ptid={0} AND rid={1};", roleData.PtID, roleData.RoleID);
			}
			if (delay)
			{
				this.AddDelayWriteSql(text);
			}
			else
			{
				DbHelperMySQL.ExecuteSql(text);
			}
		}

		public void InsertRebornRoleData(KFRebornRoleData roleData)
		{
			string sql = string.Format("INSERT INTO t_reborn_roles(ptid, rid, rname, lev, rarity, rarity_last, boss, boss_last, liansha, liansha_last, boss_award) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},'{10}');", new object[]
			{
				roleData.PtID,
				roleData.RoleID,
				roleData.RoleName,
				roleData.Lev,
				roleData.Rarity,
				roleData.RarityLast,
				roleData.Boss,
				roleData.BossLast,
				roleData.LianSha,
				roleData.LianShaLast,
				roleData.FormatBossAwardString(roleData.BossAwardList)
			});
			this.AddDelayWriteSql(sql);
		}

		public static readonly RebornPersistence Instance = new RebornPersistence();

		public object Mutex = new object();

		public Queue<string> DelayWriteSqlQueue = new Queue<string>();

		public Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>>();

		public KuaFuData<Dictionary<int, List<KFRebornRankInfo>>> RebornRankDict = new KuaFuData<Dictionary<int, List<KFRebornRankInfo>>>();
	}
}
