using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	internal class NewZoneActiveMgr
	{
		private static List<PaiHangItemData> GetActiveKingTypeRanklist(DBManager dbMgr, List<int> minGateValueList, int activityType, string midDate, int maxPaiHang = 10)
		{
			List<HuoDongPaiHangData> activityPaiHangListNearMidTime = DBQuery.GetActivityPaiHangListNearMidTime(dbMgr, activityType, midDate, maxPaiHang);
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			int num = 0;
			int num2 = 0;
			bool flag = true;
			for (int i = 0; i < activityPaiHangListNearMidTime.Count; i++)
			{
				HuoDongPaiHangData huoDongPaiHangData = activityPaiHangListNearMidTime[i];
				huoDongPaiHangData.PaiHang = -1;
				for (int j = 0; j < minGateValueList.Count; j++)
				{
					if (huoDongPaiHangData.PaiHangValue >= minGateValueList[j])
					{
						PaiHangItemData paiHangItemData = new PaiHangItemData();
						if (flag)
						{
							huoDongPaiHangData.PaiHang = j + 1;
						}
						else if (j == num2)
						{
							huoDongPaiHangData.PaiHang = num + 1;
						}
						else if (j + 1 > num)
						{
							huoDongPaiHangData.PaiHang = j + 1;
						}
						else
						{
							huoDongPaiHangData.PaiHang = num + 1;
						}
						paiHangItemData.RoleID = huoDongPaiHangData.RoleID;
						paiHangItemData.RoleName = huoDongPaiHangData.RoleName;
						paiHangItemData.Val2 = huoDongPaiHangData.PaiHang;
						paiHangItemData.Val1 = huoDongPaiHangData.PaiHangValue;
						list.Add(paiHangItemData);
						num2 = j;
						num = huoDongPaiHangData.PaiHang;
						flag = false;
						break;
					}
				}
				if (huoDongPaiHangData.PaiHang < 0 || huoDongPaiHangData.PaiHang >= minGateValueList.Count)
				{
					break;
				}
			}
			return list;
		}

		private static List<PaiHangItemData> GetRankListByActiveLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int activeId, int maxPaiHang = 3)
		{
			List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
			List<PaiHangItemData> list2 = new List<PaiHangItemData>();
			switch (activeId)
			{
			case 34:
				list = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, null, maxPaiHang);
				break;
			case 35:
				list = Global.GetUsedMoneyKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, null, maxPaiHang);
				break;
			case 36:
			{
				string midDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (!Global.IsInActivityPeriod(fromDate, toDate))
				{
					midDate = toDate;
				}
				return NewZoneActiveMgr.GetActiveKingTypeRanklist(dbMgr, minGateValueList, activeId, midDate, maxPaiHang);
			}
			case 37:
			{
				DateTime now = DateTime.Now;
				DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(fromDate, out dateTime);
				DateTime addDaysDataTime = Global.GetAddDaysDataTime(now, -1, true);
				string fromDate2 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string toDate2 = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				list = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate2, toDate2, null, maxPaiHang);
				break;
			}
			}
			int num = 0;
			int num2 = 0;
			string text = "";
			bool flag = true;
			for (int i = 0; i < list.Count; i++)
			{
				InputKingPaiHangData inputKingPaiHangData = list[i];
				inputKingPaiHangData.PaiHang = -1;
				if (activeId != 35)
				{
					Global.GetUserMaxLevelRole(dbMgr, inputKingPaiHangData.UserID, out inputKingPaiHangData.MaxLevelRoleName, out inputKingPaiHangData.MaxLevelRoleZoneID);
				}
				else
				{
					Global.GetRoleNameAndUserID(dbMgr, Global.SafeConvertToInt32(inputKingPaiHangData.UserID, 10), out inputKingPaiHangData.MaxLevelRoleName, out text);
				}
				for (int j = 0; j < minGateValueList.Count; j++)
				{
					int paiHangValue = inputKingPaiHangData.PaiHangValue;
					if (activeId == 35)
					{
						paiHangValue = inputKingPaiHangData.PaiHangValue;
					}
					if (paiHangValue >= minGateValueList[j])
					{
						if (flag)
						{
							inputKingPaiHangData.PaiHang = j + 1;
						}
						else if (j == num2)
						{
							inputKingPaiHangData.PaiHang = num + 1;
						}
						else if (j + 1 > num)
						{
							inputKingPaiHangData.PaiHang = j + 1;
						}
						else
						{
							inputKingPaiHangData.PaiHang = num + 1;
						}
						PaiHangItemData paiHangItemData = new PaiHangItemData();
						paiHangItemData.Val1 = paiHangValue;
						if (activeId == 35)
						{
							paiHangItemData.RoleID = Convert.ToInt32(inputKingPaiHangData.UserID);
						}
						paiHangItemData.RoleName = inputKingPaiHangData.MaxLevelRoleName;
						paiHangItemData.Val2 = inputKingPaiHangData.PaiHang;
						paiHangItemData.uid = inputKingPaiHangData.UserID;
						list2.Add(paiHangItemData);
						num2 = j;
						num = inputKingPaiHangData.PaiHang;
						flag = false;
						break;
					}
				}
				if (inputKingPaiHangData.PaiHang < 0 || inputKingPaiHangData.PaiHang >= minGateValueList.Count)
				{
					break;
				}
			}
			if (activeId == 37)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					int index = j + minGateValueList.Count - list2.Count;
					list2[j].Val1 = list2[j].Val1 * minGateValueList[index] / 100;
				}
			}
			return list2;
		}

		public static TCPProcessCmdResults ProcessQueryActiveInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string text2 = array[2].Replace('$', ':');
				int num2 = Global.SafeConvertToInt32(array[4], 10);
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				List<PaiHangItemData> rankListByActiveLimit = NewZoneActiveMgr.GetRankListByActiveLimit(dbMgr, fromDate, text2, list, num2, list.Count);
				int num3 = 0;
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, text2);
				int num4 = 0;
				string text3 = "";
				switch (num2)
				{
				case 34:
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 34, huoDongKeyString, out num4, out text3);
					if (num4 > 0)
					{
						num4 = 1;
					}
					break;
				case 35:
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 35, huoDongKeyString, out num4, out text3);
					if (num4 > 0)
					{
						num4 = 1;
					}
					break;
				case 36:
					DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, num2, huoDongKeyString, out num4, out text3);
					if (num4 > 0)
					{
						num4 = 1;
					}
					break;
				case 37:
					num3 = NewZoneActiveMgr.ComputTotalFanliValue(dbMgr, dbroleInfo, num2, fromDate, text2, list);
					if (num3 == 0)
					{
						num4 = 1;
					}
					break;
				}
				NewZoneActiveData instance = new NewZoneActiveData
				{
					YuanBao = num3,
					ActiveId = num2,
					Ranklist = rankListByActiveLimit,
					GetState = num4
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<NewZoneActiveData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults GetBossKillAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string toDate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string data;
				if (null == dbroleInfo)
				{
					data = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string midDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (!Global.IsInActivityPeriod(fromDate, toDate))
				{
					midDate = toDate;
				}
				List<HuoDongPaiHangData> paiHangItemListByHuoDongLimit = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, minGateValueList, 36, midDate, 10);
				int num = -1;
				for (int i = 0; i < paiHangItemListByHuoDongLimit.Count; i++)
				{
					if (paiHangItemListByHuoDongLimit[i] != null && dbroleInfo.RoleID == paiHangItemListByHuoDongLimit[i].RoleID)
					{
						num = paiHangItemListByHuoDongLimit[i].PaiHang;
						break;
					}
				}
				if (num <= 0)
				{
					data = string.Format("{0}:{1}:0", -10007, activeid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text = "";
				lock (dbroleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 36, huoDongKeyString, out num2, out text);
					if (num2 > 0)
					{
						data = string.Format("{0}:{1}:0", -10005, activeid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num3 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 36, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data = string.Format("{0}:{1}:0", -1008, activeid);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data = string.Format("{0}:{1}:{2}", 1, num, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults GetConsumeKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string toDate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string data;
				if (null == dbroleInfo)
				{
					data = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<PaiHangItemData> rankListByActiveLimit = NewZoneActiveMgr.GetRankListByActiveLimit(dbMgr, fromDate, toDate, minGateValueList, activeid, minGateValueList.Count);
				int num = -1;
				int num2 = 0;
				for (int i = 0; i < rankListByActiveLimit.Count; i++)
				{
					if (dbroleInfo.RoleID == rankListByActiveLimit[i].RoleID)
					{
						num = rankListByActiveLimit[i].Val2;
						num2 = rankListByActiveLimit[i].Val1;
					}
				}
				if (num <= 0)
				{
					data = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 35, huoDongKeyString, out num3, out text);
					if (num3 > 0)
					{
						data = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 35, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data = string.Format("{0}:{1}:{2}", 1, num, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults GetRechargeKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string toDate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string data;
				if (null == dbroleInfo)
				{
					data = string.Format("{0}:{1}:0", -1001, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<PaiHangItemData> rankListByActiveLimit = NewZoneActiveMgr.GetRankListByActiveLimit(dbMgr, fromDate, toDate, minGateValueList, activeid, minGateValueList.Count);
				int num = -1;
				int num2 = 0;
				for (int i = 0; i < rankListByActiveLimit.Count; i++)
				{
					if (dbroleInfo.UserID == rankListByActiveLimit[i].uid)
					{
						num = rankListByActiveLimit[i].Val2;
						num2 = rankListByActiveLimit[i].Val1;
					}
				}
				if (num <= 0)
				{
					data = string.Format("{0}:{1}:0", -1003, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data = string.Format("{0}:{1}:0", -10006, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num <= 0)
				{
					data = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 34, huoDongKeyString, out num3, out text);
					if (num3 > 0)
					{
						data = string.Format("{0}:{1}:0", -10005, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 34, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data = string.Format("{0}:{1}:0", -1008, roleID);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data = string.Format("{0}:{1}:{2}", 1, num, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults GetNewFanliAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, int roleID, int activeid, string fromDate, string todate, List<int> minGateValueList, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string data;
				if (null == dbroleInfo)
				{
					data = string.Format("{0}:{1}:{2}", -1001, roleID, activeid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = NewZoneActiveMgr.ComputTotalFanliValue(dbMgr, dbroleInfo, activeid, fromDate, todate, minGateValueList);
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					if (num > 0)
					{
						DateTime addDaysDataTime = Global.GetAddDaysDataTime(DateTime.Now, -1, true);
						DateTime dateTime = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 0, 0, 0);
						DateTime dateTime2 = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 23, 59, 59);
						string huoDongKeyString = Global.GetHuoDongKeyString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
						int num2 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 37, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num2 < 0)
						{
							data = string.Format("{0}:{1}:{2}", -1008, roleID, activeid);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data = string.Format("{0}:{1}:{2}", 1, num, activeid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static int ComputNewFanLiValue(DBManager dbMgr, DBRoleInfo roleInfo, int activeid, string fromdate, string todate, List<int> minGateValueList)
		{
			int result = 0;
			List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromdate, todate, minGateValueList, minGateValueList.Count);
			RankDataKey key = new RankDataKey(RankType.Charge, fromdate, todate, null);
			int num = roleInfo.RankValue.GetRankValue(key);
			if (num < 0)
			{
				num = 0;
			}
			for (int i = 0; i < inputKingPaiHangListByHuoDongLimit.Count; i++)
			{
				if (inputKingPaiHangListByHuoDongLimit[i].UserID == roleInfo.UserID)
				{
					num = num * minGateValueList[inputKingPaiHangListByHuoDongLimit[i].PaiHang - 1] / 100;
					result = num;
					break;
				}
			}
			return result;
		}

		private static int ComputTotalFanliValue(DBManager dbMgr, DBRoleInfo roleInfo, int activeid, string fromDate, string todate, List<int> minGateValueList)
		{
			DateTime now = DateTime.Now;
			DateTime t = new DateTime(2000, 1, 1, 0, 0, 0);
			DateTime dateTime = default(DateTime);
			DateTime.TryParse(fromDate, out t);
			DateTime.TryParse(todate, out dateTime);
			int num = 0;
			int result;
			if (now.Ticks <= t.Ticks + 864000000000L)
			{
				result = 0;
			}
			else
			{
				for (int i = 1; i <= 7; i++)
				{
					DateTime addDaysDataTime = Global.GetAddDaysDataTime(now, -i, true);
					DateTime t2 = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 0, 0, 0);
					DateTime dateTime2 = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 23, 59, 59);
					string huoDongKeyString = Global.GetHuoDongKeyString(t2.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
					if (t2 < t)
					{
						break;
					}
					int num2 = 0;
					string text = "";
					DBQuery.GetAwardHistoryForUser(dbMgr, roleInfo.UserID, activeid, huoDongKeyString, out num2, out text);
					if (num2 > 0)
					{
						break;
					}
					num += NewZoneActiveMgr.ComputNewFanLiValue(dbMgr, roleInfo, activeid, t2.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"), minGateValueList);
				}
				result = num;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessGetNewzoneActiveAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			TCPProcessCmdResults result = TCPProcessCmdResults.RESULT_FAILED;
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string text2 = array[2].Replace('$', ':');
				int activeid = Global.SafeConvertToInt32(array[4], 10);
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				switch (activeid)
				{
				case 34:
					result = NewZoneActiveMgr.GetRechargeKingAward(dbMgr, pool, nID, roleID, activeid, fromDate, text2, list, out tcpOutPacket);
					break;
				case 35:
					result = NewZoneActiveMgr.GetConsumeKingAward(dbMgr, pool, nID, roleID, activeid, fromDate, text2, list, out tcpOutPacket);
					break;
				case 36:
					result = NewZoneActiveMgr.GetBossKillAward(dbMgr, pool, nID, roleID, activeid, fromDate, text2, list, out tcpOutPacket);
					break;
				case 37:
					result = NewZoneActiveMgr.GetNewFanliAward(dbMgr, pool, nID, roleID, activeid, fromDate, text2, list, out tcpOutPacket);
					break;
				}
			}
			catch (Exception ex)
			{
			}
			return result;
		}

		public static NewZoneActiveData NewZoneFanli = null;
	}
}
