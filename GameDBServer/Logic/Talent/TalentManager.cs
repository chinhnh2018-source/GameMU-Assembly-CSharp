using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Talent
{
	public class TalentManager
	{
		public static TCPProcessCmdResults ProcTalentModify(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 6;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				int totalCount = Convert.ToInt32(array[1]);
				long exp = Convert.ToInt64(array[2]);
				long expAdd = Convert.ToInt64(array[3]);
				int isUp = Convert.ToInt32(array[4]);
				int zoneID = Convert.ToInt32(array[5]);
				string data2 = TalentManager.TalentInfoModify(dbMgr, roleID, totalCount, exp, expAdd, isUp, zoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcTalentEffectModify(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 5;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				int talentType = Convert.ToInt32(array[1]);
				int effectID = Convert.ToInt32(array[2]);
				int effectLevel = Convert.ToInt32(array[3]);
				int zoneID = Convert.ToInt32(array[4]);
				string data2 = TalentManager.TalentEffectModify(dbMgr, roleID, talentType, effectID, effectLevel, zoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcTalentEffectClear(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				int zoneID = Convert.ToInt32(array[1]);
				string data2 = TalentManager.TalentEffectClear(dbMgr, roleID, zoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static string TalentInfoModify(DBManager dbMgr, int roleID, int totalCount, long exp, long expAdd, int isUp, int zoneID)
		{
			int num = 1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_talent(roleID, tatalCount, exp, zoneID) VALUES({0}, {1}, {2}, {3})", new object[]
				{
					roleID,
					totalCount,
					exp,
					zoneID
				});
				int num2 = myDbConnection.ExecuteNonQuery(sql, 0);
				if (num2 > 0)
				{
					num = 0;
					TalentManager.TalentLogAdd(dbMgr, zoneID, roleID, 1, expAdd);
					if (isUp > 0)
					{
						TalentManager.TalentLogAdd(dbMgr, zoneID, roleID, 2, 1L);
					}
					TalentManager.DbUpdateTalent(dbMgr, roleID, totalCount, exp);
				}
			}
			return num.ToString();
		}

		public static void TalentLogAdd(DBManager dbMgr, int zoneID, int roleID, int logType, long logValue)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_talent_log (zoneID, roleID, logType, logValue, logTime) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", new object[]
				{
					zoneID,
					roleID,
					logType,
					logValue,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static string TalentEffectModify(DBManager dbMgr, int roleID, int talentType, int effectID, int effectLevel, int zoneID)
		{
			int num = 1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_talent_effect(roleID, talentType, effectID, effectLevel, zoneID) VALUES({0}, {1}, {2}, {3}, {4})", new object[]
				{
					roleID,
					talentType,
					effectID,
					effectLevel,
					zoneID
				});
				int num2 = myDbConnection.ExecuteNonQuery(sql, 0);
				if (num2 > 0)
				{
					num = 0;
					TalentManager.DbUpdateTalentEffect(dbMgr, roleID, talentType, effectID, effectLevel);
				}
			}
			return num.ToString();
		}

		public static string TalentEffectClear(DBManager dbMgr, int roleID, int zoneID)
		{
			int num = 1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_talent_effect where roleID={0}", roleID);
				int num2 = myDbConnection.ExecuteNonQuery(sql, 0);
				if (num2 > 0)
				{
					TalentManager.TalentLogAdd(dbMgr, zoneID, roleID, 3, 1L);
					num = 0;
					TalentManager.DbTalentClear(dbMgr, roleID);
				}
			}
			return num.ToString();
		}

		private static void DbUpdateTalent(DBManager dbMgr, int roleID, int totalCount, long exp)
		{
			DBRoleInfo dbroleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null != dbroleInfo)
			{
				lock (dbroleInfo)
				{
					dbroleInfo.MyTalentData.TotalCount = totalCount;
					dbroleInfo.MyTalentData.Exp = exp;
				}
			}
		}

		private static void DbUpdateTalentEffect(DBManager dbMgr, int roleID, int talentType, int effectID, int effectLevel)
		{
			DBRoleInfo dbroleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null != dbroleInfo)
			{
				lock (dbroleInfo)
				{
					TalentEffectItem talentEffectItem = null;
					foreach (TalentEffectItem talentEffectItem2 in dbroleInfo.MyTalentData.EffectList)
					{
						if (talentEffectItem2.ID == effectID)
						{
							talentEffectItem = talentEffectItem2;
							break;
						}
					}
					int num;
					if (talentEffectItem == null)
					{
						num = effectLevel;
						talentEffectItem = new TalentEffectItem
						{
							ID = effectID,
							Level = effectLevel,
							TalentType = talentType
						};
						dbroleInfo.MyTalentData.EffectList.Add(talentEffectItem);
					}
					else
					{
						num = effectLevel - talentEffectItem.Level;
						talentEffectItem.Level = effectLevel;
					}
					Dictionary<int, int> countList;
					(countList = dbroleInfo.MyTalentData.CountList)[talentType] = countList[talentType] + num;
				}
			}
		}

		private static void DbTalentClear(DBManager dbMgr, int roleID)
		{
			DBRoleInfo dbroleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
			if (null != dbroleInfo)
			{
				lock (dbroleInfo)
				{
					dbroleInfo.MyTalentData.CountList[1] = 0;
					dbroleInfo.MyTalentData.CountList[2] = 0;
					dbroleInfo.MyTalentData.CountList[3] = 0;
					dbroleInfo.MyTalentData.EffectList = new List<TalentEffectItem>();
				}
			}
		}

		private enum TalentLogType
		{
			Exp = 1,
			Talent,
			Wash
		}
	}
}
