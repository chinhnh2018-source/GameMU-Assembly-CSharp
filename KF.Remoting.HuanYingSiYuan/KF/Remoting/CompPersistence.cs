using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class CompPersistence
	{
		private CompPersistence()
		{
		}

		public bool LoadDatabase()
		{
			bool result;
			if (!this.LoadCompData(this.CompDataDict))
			{
				result = false;
			}
			else if (!this.LoadCompRoleData(this.CompRoleDataDict))
			{
				result = false;
			}
			else
			{
				for (int i = 1; i <= 3; i++)
				{
					if (!this.LoadCompRankInfo(1, i, this.CompRankJunXianDict, null))
					{
						return false;
					}
				}
				for (int i = 1; i <= 3; i++)
				{
					if (!this.LoadCompRankInfo(2, i, this.CompRankJunXianLastDict, null))
					{
						return false;
					}
				}
				if (!this.LoadCompRankInfo(3, 0, null, this.CompRankBossDamageList))
				{
					result = false;
				}
				else
				{
					for (int i = 1; i <= 3; i++)
					{
						if (!this.LoadCompRankInfo(4, i, this.CompRankBattleJiFenDict, null))
						{
							return false;
						}
					}
					for (int i = 1; i <= 3; i++)
					{
						if (!this.LoadCompRankInfo(5, i, this.CompRankMineJiFenDict, null))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
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

		private string FormatLoadCompRankSql(int rankType, int compType)
		{
			int[] array = new int[]
			{
				20,
				20,
				3,
				50
			};
			string result = "";
			switch (rankType)
			{
			case 1:
				result = string.Format("SELECT rid a, junxian b FROM t_comp_roles WHERE `type`={0} AND `junxian`<>0 ORDER BY `junxian` DESC, `ranktm_jx` ASC, rid DESC LIMIT {1};", compType, array[rankType - 1]);
				break;
			case 2:
				result = string.Format("SELECT rid a, junxian_last b FROM t_comp_roles WHERE `type_last`={0} AND `junxian_last`<>0 ORDER BY `junxian_last` DESC, `ranktm_jxl` ASC, rid DESC LIMIT {1};", compType, array[rankType - 1]);
				break;
			case 3:
				result = string.Format("SELECT `type` a, bossdamage b FROM t_comp ORDER BY `type` ASC LIMIT {0};", array[rankType - 1]);
				break;
			case 4:
				result = string.Format("SELECT rid a, battlejifen b FROM t_comp_roles WHERE `type_battle`={0} AND `battlejifen`<>0 ORDER BY `battlejifen` DESC, `ranktm_bjf` ASC, rid DESC;", compType);
				break;
			case 5:
				result = string.Format("SELECT rid a, minejifen b FROM t_comp_roles WHERE `type_mine`={0} AND `minejifen`<>0 ORDER BY `minejifen` DESC, `ranktm_mjf` ASC, rid DESC;", compType);
				break;
			default:
				return result;
			}
			return result;
		}

		public int GetCompDayID()
		{
			object single = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 45);
			return Convert.ToInt32(single);
		}

		public int GetCompWeekDayID()
		{
			object single = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 46);
			return Convert.ToInt32(single);
		}

		public int GetCompBattleWeekDayID()
		{
			object single = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 47);
			return Convert.ToInt32(single);
		}

		public int GetCompMineWeekDayID()
		{
			object single = DbHelperMySQL.GetSingle("SELECT value FROM t_async WHERE id = " + 48);
			return Convert.ToInt32(single);
		}

		public void SaveCompDayID(int dayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 45, dayId));
		}

		public void SaveCompWeekDayID(int weekDayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 46, weekDayId));
		}

		public void SaveCompBattleWeekDayID(int weekDayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 47, weekDayId));
		}

		public void SaveCompMineWeekDayID(int weekDayId)
		{
			DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 48, weekDayId));
		}

		public bool LoadCompRankInfo(int rankType, int compType, KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankDict, KuaFuData<List<KFCompRankInfo>> CompRankList)
		{
			bool result;
			if (CompRankDict == null && null == CompRankList)
			{
				result = false;
			}
			else
			{
				List<KFCompRankInfo> list = null;
				if (null != CompRankDict)
				{
					if (!CompRankDict.V.TryGetValue(compType, out list))
					{
						list = (CompRankDict.V[compType] = new List<KFCompRankInfo>());
					}
					else
					{
						list.Clear();
					}
				}
				else
				{
					list = CompRankList.V;
					list.Clear();
				}
				try
				{
					string text = this.FormatLoadCompRankSql(rankType, compType);
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
					if (rankType == 3)
					{
						for (int i = 1; i <= 3; i++)
						{
							list.Add(new KFCompRankInfo());
						}
					}
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(text, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KFCompRankInfo kfcompRankInfo = new KFCompRankInfo();
						kfcompRankInfo.Key = Convert.ToInt32(mySqlDataReader["a"]);
						kfcompRankInfo.Value = Convert.ToInt32(mySqlDataReader["b"]);
						if (rankType == 3)
						{
							list[kfcompRankInfo.Key - 1] = kfcompRankInfo;
						}
						else
						{
							KuaFuData<KFCompRoleData> kuaFuData = null;
							if (this.CompRoleDataDict.TryGetValue(kfcompRankInfo.Key, out kuaFuData))
							{
								kfcompRankInfo.Param1 = KuaFuServerManager.FormatName(kuaFuData.V.RoleName, kuaFuData.V.ZoneID);
								kfcompRankInfo.tagInfo = kuaFuData;
							}
							list.Add(kfcompRankInfo);
						}
					}
					if (null != CompRankDict)
					{
						TimeUtil.AgeByNow(ref CompRankDict.Age);
					}
					if (null != CompRankList)
					{
						TimeUtil.AgeByNow(ref CompRankList.Age);
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

		private bool LoadCompData(KuaFuData<Dictionary<int, KFCompData>> CompDataDict)
		{
			bool result;
			if (null == CompDataDict)
			{
				result = false;
			}
			else
			{
				try
				{
					string strSQL = string.Format("SELECT * FROM `t_comp`", new object[0]);
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KFCompData kfcompData = new KFCompData();
						kfcompData.InitPlunderResList();
						kfcompData.CompType = Convert.ToInt32(mySqlDataReader["type"]);
						kfcompData.BoomValue = Convert.ToInt32(mySqlDataReader["boomval"]);
						kfcompData.YestdBoomValue = Convert.ToInt32(mySqlDataReader["boomval_yestd"]);
						kfcompData.EnemyCompType = Convert.ToInt32(mySqlDataReader["enemytype"]);
						kfcompData.EnemyCompTypeSet = Convert.ToInt32(mySqlDataReader["enemyset"]);
						kfcompData.Bulletin = mySqlDataReader["bulletin"].ToString();
						kfcompData.Crystal = Convert.ToInt32(mySqlDataReader["crystal"]);
						kfcompData.BossDamageTop = Convert.ToInt32(mySqlDataReader["bossdamage"]);
						kfcompData.Boss = Convert.ToInt32(mySqlDataReader["boss"]);
						kfcompData.YestdCrystal = Convert.ToInt32(mySqlDataReader["crystal_yestd"]);
						kfcompData.YestdBoss = Convert.ToInt32(mySqlDataReader["boss_yestd"]);
						kfcompData.ParsePlunderResListString(mySqlDataReader["plunderres"].ToString(), kfcompData.PlunderResList);
						kfcompData.ParsePlunderResListString(mySqlDataReader["plunderres_yestd"].ToString(), kfcompData.YestdPlunderResList);
						kfcompData.ParseStrongholdDictString(mySqlDataReader["stronghold"].ToString(), kfcompData.StrongholdDict);
						kfcompData.BossKillCompType = Convert.ToInt32(mySqlDataReader["bosskilltype"]);
						kfcompData.YestdBossKillCompType = Convert.ToInt32(mySqlDataReader["bosskilltype_yestd"]);
						kfcompData.MineRes = Convert.ToInt32(mySqlDataReader["mine"]);
						kfcompData.MineRank = Convert.ToInt32(mySqlDataReader["mine_rank"]);
						CompDataDict.V[kfcompData.CompType] = kfcompData;
					}
					TimeUtil.AgeByNow(ref CompDataDict.Age);
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

		private bool LoadCompRoleData(Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict)
		{
			bool result;
			if (null == CompRoleDataDict)
			{
				result = false;
			}
			else
			{
				try
				{
					string strSQL = string.Format("SELECT * FROM `t_comp_roles`", new object[0]);
					MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					while (mySqlDataReader != null && mySqlDataReader.Read())
					{
						KuaFuData<KFCompRoleData> kuaFuData = new KuaFuData<KFCompRoleData>();
						kuaFuData.V.RoleID = Convert.ToInt32(mySqlDataReader["rid"]);
						kuaFuData.V.ZoneID = Convert.ToInt32(mySqlDataReader["zoneid"]);
						kuaFuData.V.CompType = Convert.ToInt32(mySqlDataReader["type"]);
						kuaFuData.V.CompTypeLast = Convert.ToInt32(mySqlDataReader["type_last"]);
						kuaFuData.V.JunXian = Convert.ToInt32(mySqlDataReader["junxian"]);
						kuaFuData.V.JunXianLast = Convert.ToInt32(mySqlDataReader["junxian_last"]);
						kuaFuData.V.RoleName = Convert.ToString(mySqlDataReader["rname"]);
						kuaFuData.V.BattleJiFen = Convert.ToInt32(mySqlDataReader["battlejifen"]);
						kuaFuData.V.CompTypeBattle = Convert.ToInt32(mySqlDataReader["type_battle"]);
						kuaFuData.V.CompTypeMine = Convert.ToInt32(mySqlDataReader["type_mine"]);
						kuaFuData.V.MineJiFen = Convert.ToInt32(mySqlDataReader["minejifen"]);
						string text = mySqlDataReader["ranktm_bjf"].ToString();
						if (!string.IsNullOrEmpty(text))
						{
							DateTime.TryParse(text, out kuaFuData.V.RankTmBJF);
						}
						text = mySqlDataReader["ranktm_mjf"].ToString();
						DateTime.TryParse(text, out kuaFuData.V.RankTmMJF);
						CompRoleDataDict[kuaFuData.V.RoleID] = kuaFuData;
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

		public byte[] LoadCompRoleData4Selector(int rid)
		{
			byte[] result;
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("SELECT data1 FROM t_comp_roles WHERE rid={0}", rid));
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

		public void SaveCompData(KFCompData compData, bool delay = true)
		{
			string text = string.Format("INSERT INTO t_comp(`type`, boomval, enemytype, enemyset, `bulletin`, crystal, boss, crystal_yestd, boss_yestd, plunderres, plunderres_yestd, bosskilltype, bosskilltype_yestd, bossdamage, boomval_yestd, stronghold, mine, mine_rank) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},{8},'{9}','{10}',{11},{12},{13},{14},'{15}',{16},{17}) ON DUPLICATE KEY UPDATE boomval={1}, enemytype={2}, enemyset={3}, bulletin='{4}', crystal={5}, boss={6}, crystal_yestd={7}, boss_yestd={8},  plunderres='{9}', plunderres_yestd='{10}', bosskilltype={11}, bosskilltype_yestd={12}, bossdamage={13}, boomval_yestd={14}, stronghold='{15}', mine={16}, mine_rank={17};", new object[]
			{
				compData.CompType,
				compData.BoomValue,
				compData.EnemyCompType,
				compData.EnemyCompTypeSet,
				compData.Bulletin,
				compData.Crystal,
				compData.Boss,
				compData.YestdCrystal,
				compData.YestdBoss,
				compData.FormatPlunderResListString(compData.PlunderResList),
				compData.FormatPlunderResListString(compData.YestdPlunderResList),
				compData.BossKillCompType,
				compData.YestdBossKillCompType,
				compData.BossDamageTop,
				compData.YestdBoomValue,
				compData.FormatStrongholdDictString(compData.StrongholdDict),
				compData.MineRes,
				compData.MineRank
			});
			if (delay)
			{
				this.AddDelayWriteSql(text);
			}
			else
			{
				this.ExecuteSqlNoQuery(text);
			}
		}

		public void SaveCompRoleData(KFCompRoleData roleData, bool chgJX = false, bool chgJXL = false, bool chgJiFen = false, bool chgMine = false)
		{
			if (null != roleData.RoleData4Selector)
			{
				string text = string.Format("INSERT INTO t_comp_roles(rid, `type`, rname, zoneid, junxian, junxian_last, type_last, battlejifen, type_battle, minejifen, type_mine, data1) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10},@content) ON DUPLICATE KEY UPDATE `type`={1}, rname='{2}', junxian={4}, junxian_last={5}, type_last={6}, battlejifen={7}, type_battle={8}, minejifen={9}, type_mine={10}, data1=@content;", new object[]
				{
					roleData.RoleID,
					roleData.CompType,
					roleData.RoleName,
					roleData.ZoneID,
					roleData.JunXian,
					roleData.JunXianLast,
					roleData.CompTypeLast,
					roleData.BattleJiFen,
					roleData.CompTypeBattle,
					roleData.MineJiFen,
					roleData.CompTypeMine
				});
				DbHelperMySQL.ExecuteSqlInsertImg(text, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("content", roleData.RoleData4Selector)
				});
			}
			else
			{
				string text = string.Format("INSERT INTO t_comp_roles(rid, `type`, rname, zoneid, junxian, junxian_last, type_last, battlejifen, type_battle, minejifen, type_mine) VALUES({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10}) ON DUPLICATE KEY UPDATE `type`={1}, rname='{2}', junxian={4}, junxian_last={5}, type_last={6}, battlejifen={7}, type_battle={8}, minejifen={9}, type_mine={10};", new object[]
				{
					roleData.RoleID,
					roleData.CompType,
					roleData.RoleName,
					roleData.ZoneID,
					roleData.JunXian,
					roleData.JunXianLast,
					roleData.CompTypeLast,
					roleData.BattleJiFen,
					roleData.CompTypeBattle,
					roleData.MineJiFen,
					roleData.CompTypeMine
				});
				this.AddDelayWriteSql(text);
			}
			if (null != roleData.RoleData4Selector)
			{
				if (chgJX)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_jx=NOW() WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(text);
				}
				if (chgJXL)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_jxl=ranktm_jx WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(text);
				}
				if (chgJiFen)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_bjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(text);
				}
				if (chgMine)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_mjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.ExecuteSqlNoQuery(text);
				}
			}
			else
			{
				if (chgJX)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_jx=NOW() WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(text);
				}
				if (chgJXL)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_jxl=ranktm_jx WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(text);
				}
				if (chgJiFen)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_bjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(text);
				}
				if (chgMine)
				{
					string text = string.Format("UPDATE t_comp_roles SET ranktm_mjf=NOW() WHERE rid={0};", roleData.RoleID);
					this.AddDelayWriteSql(text);
				}
			}
		}

		public static readonly CompPersistence Instance = new CompPersistence();

		public object Mutex = new object();

		public KuaFuData<Dictionary<int, KFCompData>> CompDataDict = new KuaFuData<Dictionary<int, KFCompData>>();

		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankJunXianLastDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		public KuaFuData<List<KFCompRankInfo>> CompRankBossDamageList = new KuaFuData<List<KFCompRankInfo>>();

		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankBattleJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		public KuaFuData<Dictionary<int, List<KFCompRankInfo>>> CompRankMineJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();

		public Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict = new Dictionary<int, KuaFuData<KFCompRoleData>>();

		public Queue<string> DelayWriteSqlQueue = new Queue<string>();
	}
}
