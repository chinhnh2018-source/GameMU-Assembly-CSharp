using System;
using System.Collections.Generic;
using System.Linq;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic;
using KF.Contract.Data;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class TianTiDbCmdProcessor : ICmdProcessor
	{
		private TianTiDbCmdProcessor()
		{
		}

		public static TianTiDbCmdProcessor getInstance()
		{
			return TianTiDbCmdProcessor.instance;
		}

		public void registerProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10200, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(969, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10201, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10202, TianTiDbCmdProcessor.getInstance());
			this.LoadZhanDuiData();
			TCPCmdDispatcher.getInstance().registerProcessor(3670, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3709, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3715, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3688, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3716, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3699, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3717, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3722, TianTiDbCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(3723, TianTiDbCmdProcessor.getInstance());
		}

		public bool LoadZhanDuiData()
		{
			bool result = false;
			lock (this.Mutex)
			{
				string sql = "select`zhanduiid`,`leaderid`,`xuanyan`,`zhanduiname`,`duanweiid`,`zhanli`,`data1`,`duanweijifen`,`duanweirank`,`liansheng`,`fightcount`,`successcount`,`lastfighttime`,`monthduanweirank`,leaderrolename,zoneid,zorkjifen,zorkwin,zorkwinstreak,zorkbossinjure,zorklastfighttime,escapejifen,escapelastfighttime from t_kf_5v5_zhandui";
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					using (MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]))
					{
						result = true;
						this.ZhanDuiDataList.Age = 1L;
						this.ZhanDuiDataList.V = new List<TianTi5v5ZhanDuiData>();
						while (mySQLDataReader.Read())
						{
							TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = new TianTi5v5ZhanDuiData();
							tianTi5v5ZhanDuiData.ZhanDuiID = Convert.ToInt32(mySQLDataReader[0].ToString());
							tianTi5v5ZhanDuiData.LeaderRoleID = Convert.ToInt32(mySQLDataReader["leaderid"].ToString());
							tianTi5v5ZhanDuiData.XuanYan = mySQLDataReader["xuanyan"].ToString();
							tianTi5v5ZhanDuiData.ZhanDuiName = mySQLDataReader["zhanduiname"].ToString();
							tianTi5v5ZhanDuiData.DuanWeiId = Convert.ToInt32(mySQLDataReader["duanweiid"].ToString());
							tianTi5v5ZhanDuiData.ZhanDouLi = Convert.ToInt64(mySQLDataReader["zhanli"].ToString());
							byte[] array = (mySQLDataReader["data1"] as byte[]) ?? new byte[0];
							tianTi5v5ZhanDuiData.teamerList = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(array, 0, array.Length);
							tianTi5v5ZhanDuiData.DuanWeiJiFen = Convert.ToInt32(mySQLDataReader["duanweijifen"].ToString());
							tianTi5v5ZhanDuiData.DuanWeiRank = Convert.ToInt32(mySQLDataReader["duanweirank"].ToString());
							tianTi5v5ZhanDuiData.LianSheng = Convert.ToInt32(mySQLDataReader["liansheng"].ToString());
							tianTi5v5ZhanDuiData.FightCount = Convert.ToInt32(mySQLDataReader["fightcount"].ToString());
							tianTi5v5ZhanDuiData.SuccessCount = Convert.ToInt32(mySQLDataReader["successcount"].ToString());
							tianTi5v5ZhanDuiData.LastFightTime = Convert.ToDateTime(mySQLDataReader["lastfighttime"].ToString());
							tianTi5v5ZhanDuiData.MonthDuanWeiRank = Convert.ToInt32(mySQLDataReader["monthduanweirank"].ToString());
							tianTi5v5ZhanDuiData.LeaderRoleName = mySQLDataReader["leaderrolename"].ToString();
							tianTi5v5ZhanDuiData.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
							tianTi5v5ZhanDuiData.ZorkJiFen = Convert.ToInt32(mySQLDataReader["zorkjifen"].ToString());
							tianTi5v5ZhanDuiData.ZorkWin = Convert.ToInt32(mySQLDataReader["zorkwin"].ToString());
							tianTi5v5ZhanDuiData.ZorkWinStreak = Convert.ToInt32(mySQLDataReader["zorkwinstreak"].ToString());
							tianTi5v5ZhanDuiData.ZorkBossInjure = Convert.ToInt32(mySQLDataReader["zorkbossinjure"].ToString());
							tianTi5v5ZhanDuiData.ZorkLastFightTime = Convert.ToDateTime(mySQLDataReader["zorklastfighttime"].ToString());
							tianTi5v5ZhanDuiData.EscapeJiFen = Convert.ToInt32(mySQLDataReader["escapejifen"].ToString());
							tianTi5v5ZhanDuiData.EscapeLastFightTime = Convert.ToDateTime(mySQLDataReader["escapelastfighttime"].ToString());
							this.ZhanDuiDataDict[tianTi5v5ZhanDuiData.ZhanDuiID] = new AgeDataT<TianTi5v5ZhanDuiData>(1L, tianTi5v5ZhanDuiData);
							this.ZhanDuiDataList.V.Add(tianTi5v5ZhanDuiData);
						}
					}
				}
				this.ZhanDuiDataList.V.Sort(new Comparison<TianTi5v5ZhanDuiData>(this.ZhanDuiDataCompare));
			}
			return result;
		}

		public int ZhanDuiDataCompare(TianTi5v5ZhanDuiData x, TianTi5v5ZhanDuiData y)
		{
			int result;
			if (x.LastFightTime < this.MonthStartDateTime)
			{
				if (y.LastFightTime < this.MonthStartDateTime)
				{
					int num = y.DuanWeiJiFen - x.DuanWeiJiFen;
					result = ((num != 0) ? num : (x.ZhanDuiID - y.ZhanDuiID));
				}
				else
				{
					result = 1;
				}
			}
			else if (y.LastFightTime < this.MonthStartDateTime)
			{
				result = -1;
			}
			else
			{
				int num = y.DuanWeiJiFen - x.DuanWeiJiFen;
				result = ((num != 0) ? num : (x.ZhanDuiID - y.ZhanDuiID));
			}
			return result;
		}

		public bool QueryZhanDuiRoleInfo(TianTi5v5ZhanDuiData data)
		{
			bool result = false;
			if (data != null && null != data.teamerList)
			{
				foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in data.teamerList)
				{
					try
					{
						int roleID = tianTi5v5ZhanDuiRoleData.RoleID;
						DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleID);
						if (null != dbroleInfo)
						{
							int roleParamsInt = Global.GetRoleParamsInt32(dbroleInfo, "10241");
							if (tianTi5v5ZhanDuiRoleData.RoleName != dbroleInfo.RoleName || tianTi5v5ZhanDuiRoleData.RoleOcc != dbroleInfo.Occupation || tianTi5v5ZhanDuiRoleData.ZhanLi != (long)dbroleInfo.CombatForce || tianTi5v5ZhanDuiRoleData.RebornLevel != roleParamsInt)
							{
								result = true;
							}
							tianTi5v5ZhanDuiRoleData.RoleName = dbroleInfo.RoleName;
							tianTi5v5ZhanDuiRoleData.RoleOcc = dbroleInfo.Occupation;
							tianTi5v5ZhanDuiRoleData.ZhanLi = (long)dbroleInfo.CombatForce;
							tianTi5v5ZhanDuiRoleData.ZhuanSheng = dbroleInfo.ChangeLifeCount;
							tianTi5v5ZhanDuiRoleData.Level = dbroleInfo.Level;
							tianTi5v5ZhanDuiRoleData.RebornLevel = roleParamsInt;
							if (tianTi5v5ZhanDuiRoleData.RoleID == data.LeaderRoleID)
							{
								data.LeaderRoleName = dbroleInfo.RoleName;
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
				data.ZhanDouLi = data.teamerList.Sum((TianTi5v5ZhanDuiRoleData x) => x.ZhanLi);
			}
			return result;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			DBManager dbmanager = DBManager.getInstance();
			int cmdData = 0;
			if (nID <= 3688)
			{
				if (nID != 969)
				{
					if (nID != 3670)
					{
						if (nID == 3688)
						{
							AgeDataT<int> ageDataT = DataHelper.BytesToObject<AgeDataT<int>>(cmdParams, 0, count);
							AgeDataT<List<TianTi5v5ZhanDuiMiniData>> ageDataT2 = new AgeDataT<List<TianTi5v5ZhanDuiMiniData>>(ageDataT.Age, null);
							int v = ageDataT.V;
							lock (this.Mutex)
							{
								if (this.ZhanDuiDataListNeedUpdate)
								{
									List<TianTi5v5ZhanDuiData> list = new List<TianTi5v5ZhanDuiData>();
									foreach (AgeDataT<TianTi5v5ZhanDuiData> ageDataT3 in this.ZhanDuiDataDict.Values)
									{
										if (null != ageDataT3.V)
										{
											list.Add(ageDataT3.V);
										}
									}
									DateTime now = DateTime.Now;
									this.MonthStartDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
									list.Sort(new Comparison<TianTi5v5ZhanDuiData>(this.ZhanDuiDataCompare));
									this.ZhanDuiDataList.V = list;
									this.ZhanDuiDataListNeedUpdate = false;
									TimeUtil.AgeByNow(ref this.ZhanDuiDataList.Age);
								}
								if (ageDataT.Age < this.ZhanDuiDataList.Age)
								{
									ageDataT2.Age = this.ZhanDuiDataList.Age;
									ageDataT2.V = new List<TianTi5v5ZhanDuiMiniData>();
									int num = 0;
									while (num < this.ZhanDuiDataList.V.Count && num < v)
									{
										TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = this.ZhanDuiDataList.V[num];
										TianTi5v5ZhanDuiMiniData tianTi5v5ZhanDuiMiniData = new TianTi5v5ZhanDuiMiniData();
										tianTi5v5ZhanDuiMiniData.ZhanDuiID = tianTi5v5ZhanDuiData.ZhanDuiID;
										tianTi5v5ZhanDuiMiniData.DuanWeiID = tianTi5v5ZhanDuiData.DuanWeiId;
										tianTi5v5ZhanDuiMiniData.DuiZhangName = tianTi5v5ZhanDuiData.ZhanDuiName;
										tianTi5v5ZhanDuiMiniData.XuanYan = tianTi5v5ZhanDuiData.XuanYan;
										tianTi5v5ZhanDuiMiniData.ZhanDouLi = tianTi5v5ZhanDuiData.ZhanDouLi;
										tianTi5v5ZhanDuiMiniData.Name = tianTi5v5ZhanDuiData.LeaderRoleName;
										tianTi5v5ZhanDuiMiniData.MemberList = new List<RoleNameLevelData>();
										foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in tianTi5v5ZhanDuiData.teamerList)
										{
											tianTi5v5ZhanDuiMiniData.MemberList.Add(new RoleNameLevelData(tianTi5v5ZhanDuiRoleData.ZhuanSheng, tianTi5v5ZhanDuiRoleData.Level, tianTi5v5ZhanDuiRoleData.RoleName, tianTi5v5ZhanDuiRoleData.RoleID == tianTi5v5ZhanDuiData.LeaderRoleID, tianTi5v5ZhanDuiRoleData.RoleOcc));
										}
										ageDataT2.V.Add(tianTi5v5ZhanDuiMiniData);
										num++;
									}
								}
							}
							client.sendCmd<AgeDataT<List<TianTi5v5ZhanDuiMiniData>>>(nID, ageDataT2);
						}
					}
					else
					{
						TianTiLogItemData tianTiLogItemData = DataHelper.BytesToObject<TianTiLogItemData>(cmdParams, 0, count);
						if (tianTiLogItemData != null && tianTiLogItemData.RoleId > 0)
						{
							lock (this.Mutex)
							{
								KF5V5RoleLogData kf5V5RoleLogData;
								if (this.KF5V5RoleLogDataDict.TryGetValue(tianTiLogItemData.RoleId, out kf5V5RoleLogData))
								{
									kf5V5RoleLogData.LogItemList.Insert(0, tianTiLogItemData);
								}
							}
							DBWriter.InsertKF5v5ItemLog(DBManager.getInstance(), tianTiLogItemData);
						}
						client.sendCmd<int>(nID, cmdData);
					}
				}
				else
				{
					int num2 = DataHelper.BytesToObject<int>(cmdParams, 0, count);
					bool flag3 = false;
					TianTiRoleLogData tianTiRoleLogData;
					lock (this.Mutex)
					{
						if (!this.TianTiRoleLogDataDict.TryGetValue(num2, out tianTiRoleLogData))
						{
							tianTiRoleLogData = new TianTiRoleLogData();
							this.TianTiRoleLogDataDict.Add(num2, tianTiRoleLogData);
							flag3 = true;
						}
					}
					if (flag3)
					{
						tianTiRoleLogData.LogItemList = DBQuery.GetTianTiLogItemDataList(dbmanager, num2, 100);
					}
					lock (this.Mutex)
					{
						if (null != tianTiRoleLogData.LogItemList)
						{
							if (tianTiRoleLogData.LogItemList.Count > 0)
							{
								if (tianTiRoleLogData.LogItemList.Count > 100)
								{
									int num3 = tianTiRoleLogData.LogItemList.Count - 100;
									if (num3 > 0)
									{
										tianTiRoleLogData.LogItemList.RemoveRange(100, num3);
									}
								}
							}
						}
					}
					client.sendCmd<List<TianTiLogItemData>>(nID, tianTiRoleLogData.LogItemList);
				}
			}
			else if (nID <= 3709)
			{
				if (nID != 3699)
				{
					if (nID == 3709)
					{
						int num2 = DataHelper.BytesToObject<int>(cmdParams, 0, count);
						bool flag3 = false;
						KF5V5RoleLogData kf5V5RoleLogData2;
						lock (this.Mutex)
						{
							if (!this.KF5V5RoleLogDataDict.TryGetValue(num2, out kf5V5RoleLogData2))
							{
								kf5V5RoleLogData2 = new KF5V5RoleLogData();
								this.KF5V5RoleLogDataDict.Add(num2, kf5V5RoleLogData2);
								flag3 = true;
							}
						}
						if (flag3)
						{
							kf5V5RoleLogData2.LogItemList = DBQuery.GetT5v5ItemDataList(dbmanager, num2, 100);
						}
						lock (this.Mutex)
						{
							if (null != kf5V5RoleLogData2.LogItemList)
							{
								if (kf5V5RoleLogData2.LogItemList.Count > 0)
								{
									if (kf5V5RoleLogData2.LogItemList.Count > 100)
									{
										int num3 = kf5V5RoleLogData2.LogItemList.Count - 100;
										if (num3 > 0)
										{
											kf5V5RoleLogData2.LogItemList.RemoveRange(100, num3);
										}
									}
								}
							}
						}
						client.sendCmd<List<TianTiLogItemData>>(nID, kf5V5RoleLogData2.LogItemList);
					}
				}
				else
				{
					int num4 = DataHelper.BytesToObject<int>(cmdParams, 0, count);
					lock (this.Mutex)
					{
						AgeDataT<TianTi5v5ZhanDuiData> ageDataT4;
						if (this.ZhanDuiDataDict.TryGetValue(num4, out ageDataT4))
						{
							TimeUtil.AgeByNow(ref ageDataT4.Age);
							ageDataT4.V = null;
							this.ZhanDuiDataListNeedUpdate = true;
							using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
							{
								string sql = string.Format("delete from t_kf_5v5_zhandui where zhanduiid={0}", num4);
								cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
							}
						}
					}
					client.sendCmd<int>(nID, cmdData);
				}
			}
			else
			{
				switch (nID)
				{
				case 3715:
				{
					AgeDataT<int> ageDataT = DataHelper.BytesToObject<AgeDataT<int>>(cmdParams, 0, count);
					int num4 = ageDataT.V;
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT4;
					lock (this.Mutex)
					{
						if (this.ZhanDuiDataDict.TryGetValue(ageDataT.V, out ageDataT4))
						{
							if (this.QueryZhanDuiRoleInfo(ageDataT4.V))
							{
								TimeUtil.AgeByNow(ref ageDataT4.Age);
							}
							if (ageDataT.Age == ageDataT4.Age || ageDataT4.V == null)
							{
								ageDataT4 = new AgeDataT<TianTi5v5ZhanDuiData>(ageDataT4.Age, null);
							}
							goto IL_91C;
						}
					}
					TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData2 = new TianTi5v5ZhanDuiData();
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql2 = string.Format("select zhanduiname,xuanyan,`leaderid`,duanweiid,duanweijifen,duanweirank,liansheng,fightcount,successcount,lastfighttime,monthduanweirank,zhanli,data1,leaderrolename,zoneid,zorkjifen,zorkwin,zorkwinstreak,zorkbossinjure,zorklastfighttime,escapejifen,escapelastfighttime from t_kf_5v5_zhandui where zhanduiid={0}", num4);
						using (MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql2, new MySQLParameter[0]))
						{
							if (mySQLDataReader.Read())
							{
								tianTi5v5ZhanDuiData2.ZhanDuiID = num4;
								tianTi5v5ZhanDuiData2.LeaderRoleID = Convert.ToInt32(mySQLDataReader["leaderid"].ToString());
								tianTi5v5ZhanDuiData2.XuanYan = mySQLDataReader["xuanyan"].ToString();
								tianTi5v5ZhanDuiData2.ZhanDuiName = mySQLDataReader["zhanduiname"].ToString();
								tianTi5v5ZhanDuiData2.DuanWeiId = Convert.ToInt32(mySQLDataReader["duanweiid"].ToString());
								tianTi5v5ZhanDuiData2.ZhanDouLi = Convert.ToInt64(mySQLDataReader["zhanli"].ToString());
								byte[] array = (mySQLDataReader["data1"] as byte[]) ?? new byte[0];
								tianTi5v5ZhanDuiData2.teamerList = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiRoleData>>(array, 0, array.Length);
								tianTi5v5ZhanDuiData2.DuanWeiJiFen = Convert.ToInt32(mySQLDataReader["duanweijifen"].ToString());
								tianTi5v5ZhanDuiData2.DuanWeiRank = Convert.ToInt32(mySQLDataReader["duanweirank"].ToString());
								tianTi5v5ZhanDuiData2.LianSheng = Convert.ToInt32(mySQLDataReader["liansheng"].ToString());
								tianTi5v5ZhanDuiData2.FightCount = Convert.ToInt32(mySQLDataReader["fightcount"].ToString());
								tianTi5v5ZhanDuiData2.SuccessCount = Convert.ToInt32(mySQLDataReader["successcount"].ToString());
								tianTi5v5ZhanDuiData2.LastFightTime = Convert.ToDateTime(mySQLDataReader["lastfighttime"].ToString());
								tianTi5v5ZhanDuiData2.MonthDuanWeiRank = Convert.ToInt32(mySQLDataReader["monthduanweirank"].ToString());
								tianTi5v5ZhanDuiData2.LeaderRoleName = mySQLDataReader["leaderrolename"].ToString();
								tianTi5v5ZhanDuiData2.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
								tianTi5v5ZhanDuiData2.ZorkJiFen = Convert.ToInt32(mySQLDataReader["zorkjifen"].ToString());
								tianTi5v5ZhanDuiData2.ZorkWin = Convert.ToInt32(mySQLDataReader["zorkwin"].ToString());
								tianTi5v5ZhanDuiData2.ZorkWinStreak = Convert.ToInt32(mySQLDataReader["zorkwinstreak"].ToString());
								tianTi5v5ZhanDuiData2.ZorkBossInjure = Convert.ToInt32(mySQLDataReader["zorkbossinjure"].ToString());
								tianTi5v5ZhanDuiData2.ZorkLastFightTime = Convert.ToDateTime(mySQLDataReader["zorklastfighttime"].ToString());
								tianTi5v5ZhanDuiData2.EscapeJiFen = Convert.ToInt32(mySQLDataReader["escapejifen"].ToString());
								tianTi5v5ZhanDuiData2.EscapeLastFightTime = Convert.ToDateTime(mySQLDataReader["escapelastfighttime"].ToString());
							}
						}
					}
					lock (this.Mutex)
					{
						if (tianTi5v5ZhanDuiData2.ZhanDuiID > 0)
						{
							if (!this.ZhanDuiDataDict.TryGetValue(ageDataT.V, out ageDataT4) || ageDataT4.V == null)
							{
								ageDataT4 = new AgeDataT<TianTi5v5ZhanDuiData>(1L, tianTi5v5ZhanDuiData2);
								this.ZhanDuiDataDict[ageDataT.V] = ageDataT4;
							}
						}
						else
						{
							ageDataT4 = new AgeDataT<TianTi5v5ZhanDuiData>(ageDataT.Age + 1L, null);
							this.ZhanDuiDataDict[ageDataT.V] = ageDataT4;
						}
						if (this.QueryZhanDuiRoleInfo(ageDataT4.V))
						{
							TimeUtil.AgeByNow(ref ageDataT4.Age);
						}
					}
					IL_91C:
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, ageDataT4);
					break;
				}
				case 3716:
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT4 = null;
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT5 = DataHelper.BytesToObject<AgeDataT<TianTi5v5ZhanDuiData>>(cmdParams, 0, count);
					if (ageDataT5 != null && ageDataT5.V != null)
					{
						lock (this.Mutex)
						{
							TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData2 = ageDataT5.V;
							if (!this.ZhanDuiDataDict.TryGetValue(tianTi5v5ZhanDuiData2.ZhanDuiID, out ageDataT4))
							{
								ageDataT4 = new AgeDataT<TianTi5v5ZhanDuiData>(0L, tianTi5v5ZhanDuiData2);
								this.ZhanDuiDataDict[tianTi5v5ZhanDuiData2.ZhanDuiID] = ageDataT4;
								this.ZhanDuiDataListNeedUpdate = true;
							}
							else
							{
								if (ageDataT4.V == null || ageDataT4.V.LeaderRoleID != ageDataT5.V.LeaderRoleID || ageDataT4.V.LeaderRoleName != ageDataT5.V.LeaderRoleName || ageDataT4.V.DuanWeiId != ageDataT5.V.DuanWeiId)
								{
									this.ZhanDuiDataListNeedUpdate = true;
								}
								if (ageDataT5.V.ZhanDuiDataModeType == 1)
								{
									ageDataT4.V.ZhanDuiID = tianTi5v5ZhanDuiData2.ZhanDuiID;
									ageDataT4.V.XuanYan = tianTi5v5ZhanDuiData2.XuanYan;
									ageDataT4.V.ZhanDuiName = tianTi5v5ZhanDuiData2.ZhanDuiName;
									ageDataT4.V.LeaderRoleID = tianTi5v5ZhanDuiData2.LeaderRoleID;
									ageDataT4.V.ZhanDouLi = tianTi5v5ZhanDuiData2.ZhanDouLi;
									ageDataT4.V.teamerList = tianTi5v5ZhanDuiData2.teamerList;
									ageDataT4.V.TeamerRidList = tianTi5v5ZhanDuiData2.TeamerRidList;
									ageDataT4.V.LeaderRoleName = tianTi5v5ZhanDuiData2.LeaderRoleName;
									ageDataT4.V.ZoneID = tianTi5v5ZhanDuiData2.ZoneID;
								}
								else if (ageDataT5.V.ZhanDuiDataModeType == 0)
								{
									ageDataT4.V.DuanWeiId = tianTi5v5ZhanDuiData2.DuanWeiId;
									ageDataT4.V.DuanWeiJiFen = tianTi5v5ZhanDuiData2.DuanWeiJiFen;
									ageDataT4.V.DuanWeiRank = tianTi5v5ZhanDuiData2.DuanWeiRank;
									ageDataT4.V.ZhanDouLi = tianTi5v5ZhanDuiData2.ZhanDouLi;
									ageDataT4.V.LianSheng = tianTi5v5ZhanDuiData2.LianSheng;
									ageDataT4.V.SuccessCount = tianTi5v5ZhanDuiData2.SuccessCount;
									ageDataT4.V.FightCount = tianTi5v5ZhanDuiData2.FightCount;
									ageDataT4.V.MonthDuanWeiRank = tianTi5v5ZhanDuiData2.MonthDuanWeiRank;
									ageDataT4.V.LastFightTime = tianTi5v5ZhanDuiData2.LastFightTime;
									using (List<TianTi5v5ZhanDuiRoleData>.Enumerator enumerator2 = ageDataT4.V.teamerList.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											TianTi5v5ZhanDuiRoleData role = enumerator2.Current;
											TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData2 = tianTi5v5ZhanDuiData2.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == role.RoleID);
											if (null != tianTi5v5ZhanDuiRoleData2)
											{
												role.MonthFightCounts = tianTi5v5ZhanDuiRoleData2.MonthFightCounts;
												role.TodayFightCount = tianTi5v5ZhanDuiRoleData2.TodayFightCount;
												role.MonthFigntCount = tianTi5v5ZhanDuiRoleData2.MonthFigntCount;
												role.ZhanLi = tianTi5v5ZhanDuiRoleData2.ZhanLi;
												role.RoleOcc = tianTi5v5ZhanDuiRoleData2.RoleOcc;
												role.ZhuanSheng = tianTi5v5ZhanDuiRoleData2.ZhuanSheng;
												role.Level = tianTi5v5ZhanDuiRoleData2.Level;
												role.RebornLevel = tianTi5v5ZhanDuiRoleData2.RebornLevel;
												role.ModelData = tianTi5v5ZhanDuiRoleData2.ModelData;
											}
										}
									}
								}
							}
							ageDataT5.Age = TimeUtil.AgeByNow(ref ageDataT4.Age);
							using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
							{
								tianTi5v5ZhanDuiData2 = ageDataT4.V;
								if (null != tianTi5v5ZhanDuiData2)
								{
									MySQLParameter mySQLParameter = new MySQLParameter("@p1", tianTi5v5ZhanDuiData2.ZhanDuiName);
									MySQLParameter mySQLParameter2 = new MySQLParameter("@p2", tianTi5v5ZhanDuiData2.XuanYan);
									string text = DataHelper.ObjectToHexString<List<TianTi5v5ZhanDuiRoleData>>(tianTi5v5ZhanDuiData2.teamerList);
									string sql = string.Format("INSERT INTO t_kf_5v5_zhandui (zhanduiid,zhanduiname,xuanyan,`leaderid`,duanweiid,duanweijifen,duanweirank,liansheng,fightcount,successcount,lastfighttime,monthduanweirank,zhanli,data1,leaderrolename,zoneid) VALUES({0},@p1,@p2,{3},{4},{5},{6},{7},{8},{9},'{10}',{11},{12},{14},'{13}',{15}) ON DUPLICATE KEY UPDATE zhanduiname=@p1,xuanyan=@p2,leaderid={3},duanweiid={4},duanweijifen={5},duanweirank={6},liansheng={7},fightcount={8},successcount={9},lastfighttime='{10}',monthduanweirank={11},zhanli={12},data1={14},leaderrolename='{13}',zoneid={15}", new object[]
									{
										tianTi5v5ZhanDuiData2.ZhanDuiID,
										tianTi5v5ZhanDuiData2.ZhanDuiName,
										tianTi5v5ZhanDuiData2.XuanYan,
										tianTi5v5ZhanDuiData2.LeaderRoleID,
										tianTi5v5ZhanDuiData2.DuanWeiId,
										tianTi5v5ZhanDuiData2.DuanWeiJiFen,
										tianTi5v5ZhanDuiData2.DuanWeiRank,
										tianTi5v5ZhanDuiData2.LianSheng,
										tianTi5v5ZhanDuiData2.FightCount,
										tianTi5v5ZhanDuiData2.SuccessCount,
										tianTi5v5ZhanDuiData2.LastFightTime,
										tianTi5v5ZhanDuiData2.MonthDuanWeiRank,
										tianTi5v5ZhanDuiData2.ZhanDouLi,
										tianTi5v5ZhanDuiData2.LeaderRoleName,
										text,
										tianTi5v5ZhanDuiData2.ZoneID
									});
									int num5 = myDbConnection.ExecuteSql(sql, new MySQLParameter[]
									{
										mySQLParameter,
										mySQLParameter2
									});
								}
							}
						}
					}
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, ageDataT4);
					break;
				}
				case 3717:
				{
					int[] array2 = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
					DBRoleInfo dbroleInfo = dbmanager.GetDBRoleInfo(ref array2[0]);
					if (null != dbroleInfo)
					{
						dbroleInfo.ZhanDuiID = array2[1];
						dbroleInfo.ZhanDuiZhiWu = array2[2];
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							string sql = string.Format("update t_roles set zhanduiid={1},zhanduizhiwu={2} where rid={0}", dbroleInfo.RoleID, dbroleInfo.ZhanDuiID, dbroleInfo.ZhanDuiZhiWu);
							cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
						}
					}
					client.sendCmd<int>(nID, cmdData);
					break;
				}
				case 3718:
				case 3719:
				case 3720:
				case 3721:
					break;
				case 3722:
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT4 = null;
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT5 = DataHelper.BytesToObject<AgeDataT<TianTi5v5ZhanDuiData>>(cmdParams, 0, count);
					if (ageDataT5 != null && ageDataT5.V != null)
					{
						lock (this.Mutex)
						{
							TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData2 = ageDataT5.V;
							if (!this.ZhanDuiDataDict.TryGetValue(tianTi5v5ZhanDuiData2.ZhanDuiID, out ageDataT4))
							{
								client.sendCmd(30767, "0");
								break;
							}
							ageDataT4.V.ZorkJiFen = tianTi5v5ZhanDuiData2.ZorkJiFen;
							ageDataT4.V.ZorkWin = tianTi5v5ZhanDuiData2.ZorkWin;
							ageDataT4.V.ZorkWinStreak = tianTi5v5ZhanDuiData2.ZorkWinStreak;
							ageDataT4.V.ZorkBossInjure = tianTi5v5ZhanDuiData2.ZorkBossInjure;
							ageDataT4.V.ZorkLastFightTime = tianTi5v5ZhanDuiData2.ZorkLastFightTime;
							ageDataT5.Age = TimeUtil.AgeByNow(ref ageDataT4.Age);
							using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
							{
								string sql = string.Format("UPDATE t_kf_5v5_zhandui SET zorkjifen={1}, zorkwin={2}, zorkwinstreak={3}, zorkbossinjure={4}, zorklastfighttime='{5}' WHERE zhanduiid={0}; ", new object[]
								{
									tianTi5v5ZhanDuiData2.ZhanDuiID,
									tianTi5v5ZhanDuiData2.ZorkJiFen,
									tianTi5v5ZhanDuiData2.ZorkWin,
									tianTi5v5ZhanDuiData2.ZorkWinStreak,
									tianTi5v5ZhanDuiData2.ZorkBossInjure,
									tianTi5v5ZhanDuiData2.ZorkLastFightTime
								});
								int num5 = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
							}
						}
					}
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, ageDataT4);
					break;
				}
				case 3723:
				{
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT4 = null;
					AgeDataT<TianTi5v5ZhanDuiData> ageDataT5 = DataHelper.BytesToObject<AgeDataT<TianTi5v5ZhanDuiData>>(cmdParams, 0, count);
					if (ageDataT5 != null && ageDataT5.V != null)
					{
						lock (this.Mutex)
						{
							TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData2 = ageDataT5.V;
							if (!this.ZhanDuiDataDict.TryGetValue(tianTi5v5ZhanDuiData2.ZhanDuiID, out ageDataT4))
							{
								client.sendCmd(30767, "0");
								break;
							}
							ageDataT4.V.EscapeJiFen = tianTi5v5ZhanDuiData2.EscapeJiFen;
							ageDataT4.V.EscapeLastFightTime = tianTi5v5ZhanDuiData2.EscapeLastFightTime;
							ageDataT5.Age = TimeUtil.AgeByNow(ref ageDataT4.Age);
							using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
							{
								string sql = string.Format("UPDATE t_kf_5v5_zhandui SET escapejifen={1}, escapelastfighttime='{2}' WHERE zhanduiid={0}; ", tianTi5v5ZhanDuiData2.ZhanDuiID, tianTi5v5ZhanDuiData2.EscapeJiFen, tianTi5v5ZhanDuiData2.EscapeLastFightTime);
								int num5 = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
							}
						}
					}
					client.sendCmd<AgeDataT<TianTi5v5ZhanDuiData>>(nID, ageDataT4);
					break;
				}
				default:
					switch (nID)
					{
					case 10200:
					{
						TianTiLogItemData tianTiLogItemData2 = DataHelper.BytesToObject<TianTiLogItemData>(cmdParams, 0, count);
						if (tianTiLogItemData2 != null && tianTiLogItemData2.RoleId > 0)
						{
							lock (this.Mutex)
							{
								TianTiRoleLogData tianTiRoleLogData;
								if (this.TianTiRoleLogDataDict.TryGetValue(tianTiLogItemData2.RoleId, out tianTiRoleLogData))
								{
									tianTiRoleLogData.LogItemList.Insert(0, tianTiLogItemData2);
								}
							}
							DBWriter.InsertTianTiItemLog(DBManager.getInstance(), tianTiLogItemData2);
						}
						client.sendCmd<int>(nID, cmdData);
						break;
					}
					case 10201:
					{
						RoleTianTiData roleTianTiData = DataHelper.BytesToObject<RoleTianTiData>(cmdParams, 0, count);
						if (null != roleTianTiData)
						{
							DBRoleInfo dbroleInfo2 = dbmanager.GetDBRoleInfo(ref roleTianTiData.RoleId);
							if (null != dbroleInfo2)
							{
								lock (dbroleInfo2)
								{
									dbroleInfo2.TianTiData = roleTianTiData;
									DBWriter.UpdateTianTiRoleData(dbmanager, roleTianTiData);
								}
							}
						}
						client.sendCmd<int>(nID, cmdData);
						break;
					}
					case 10202:
					{
						int[] array3 = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
						if (array3 != null && array3.Length == 2)
						{
							DBRoleInfo dbroleInfo2 = dbmanager.GetDBRoleInfo(ref array3[0]);
							if (null != dbroleInfo2)
							{
								lock (dbroleInfo2)
								{
									dbroleInfo2.TianTiData.RongYao = array3[1];
									if (dbroleInfo2.TianTiData.LastFightDayId > 0)
									{
										cmdData = DBWriter.UpdateTianTiRoleRongYao(dbmanager, array3[0], array3[1]);
									}
									else
									{
										cmdData = DBWriter.UpdateTianTiRoleData(dbmanager, dbroleInfo2.TianTiData);
									}
								}
							}
						}
						client.sendCmd<int>(nID, cmdData);
						break;
					}
					}
					break;
				}
			}
		}

		private const int MaxCacheLogItemCount = 100;

		private static TianTiDbCmdProcessor instance = new TianTiDbCmdProcessor();

		private object Mutex = new object();

		private Dictionary<int, TianTiRoleLogData> TianTiRoleLogDataDict = new Dictionary<int, TianTiRoleLogData>();

		private Dictionary<int, KF5V5RoleLogData> KF5V5RoleLogDataDict = new Dictionary<int, KF5V5RoleLogData>();

		private Queue<TianTiLogItemData> TianTiRoleItemLogCacheQueue = new Queue<TianTiLogItemData>();

		private Queue<TianTiLogItemData> TianTiRoleItemLogWriteQueue = new Queue<TianTiLogItemData>();

		private bool ZhanDuiDataListNeedUpdate;

		private Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>> ZhanDuiDataDict = new Dictionary<int, AgeDataT<TianTi5v5ZhanDuiData>>();

		private AgeDataT<List<TianTi5v5ZhanDuiData>> ZhanDuiDataList = new AgeDataT<List<TianTi5v5ZhanDuiData>>();

		private DateTime MonthStartDateTime;
	}
}
