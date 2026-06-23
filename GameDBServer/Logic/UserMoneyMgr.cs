using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic.Fund;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class UserMoneyMgr
	{
		public static void UpdateUsersMoney(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - UserMoneyMgr.LastUpdateUserMoneyTicks >= 2000L)
			{
				UserMoneyMgr.LastUpdateUserMoneyTicks = num;
				SingleChargeData chargeData = CFirstChargeMgr.ChargeData;
				if (chargeData == null)
				{
					if (!UserMoneyMgr.ChargeDataLogState)
					{
						UserMoneyMgr.ChargeDataLogState = true;
						LogManager.WriteLog(LogTypes.Error, "处理充值时找不到ChargeData, " + DateTime.Now.ToString(), null, true);
					}
				}
				else
				{
					if (UserMoneyMgr.ChargeDataLogState)
					{
						UserMoneyMgr.ChargeDataLogState = false;
						LogManager.WriteLog(LogTypes.Error, "处理充值时已经获取到了ChargeData, " + DateTime.Now.ToString(), null, true);
					}
					List<TempMoneyInfo> list = new List<TempMoneyInfo>();
					DBQuery.QueryTempMoney(dbMgr, list);
					if (list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							string userID = list[i].userID;
							int chargeRoleID = list[i].chargeRoleID;
							int addUserMoney = list[i].addUserMoney;
							int addUserItem = list[i].addUserItem;
							string chargeTm = list[i].chargeTm;
							LogManager.WriteLog(LogTypes.Error, string.Format("正在处理充值 UID={0}，money={1}，itemid={2}", userID, addUserMoney, addUserItem), null, true);
							DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
							if (dbuserInfo == null)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("处理充值时找不到user, UID={0}，money={1}，itemid={2}", userID, addUserMoney, addUserItem), null, true);
							}
							else if (addUserItem != 0 && !dbuserInfo.ListRoleIDs.Contains(chargeRoleID))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("处理充值直购时user内找不到rid, UID={0}，rid={1}，money={2}，itemid={3}", new object[]
								{
									userID,
									chargeRoleID,
									addUserMoney,
									addUserItem
								}), null, true);
							}
							else
							{
								UserMoneyMgr._ProcessCharge(dbMgr, dbuserInfo, chargeRoleID, addUserMoney, addUserItem, chargeTm, chargeData, false);
								if (addUserItem == 0 && addUserMoney == chargeData.YueKaMoney && chargeData.YueKaMoney > 0)
								{
									UserMoneyMgr._ProcessBuyYueKa(dbMgr, dbuserInfo);
								}
							}
						}
					}
				}
			}
		}

		private static void _ProcessBuyItem(DBManager dbMgr, DBUserInfo dbUserInfo, int chargeRoleID, int addUserMoney, int zhigouID, string chargeTm)
		{
			DBWriter.InsertChargeTempItem(dbMgr, dbUserInfo.UserID, chargeRoleID, addUserMoney, zhigouID, chargeTm);
		}

		private static JieriSuperInputData GetJieriSuperInputDataByChargeTm(SingleChargeData chargeData, string chargeTm)
		{
			JieriSuperInputData result = null;
			DateTime t = DateTime.Parse(chargeTm);
			foreach (JieriSuperInputData jieriSuperInputData in chargeData.SuperInputFanLiDict.Values)
			{
				if (t >= jieriSuperInputData.BeginTime && t <= jieriSuperInputData.EndTime)
				{
					result = jieriSuperInputData;
					break;
				}
			}
			return result;
		}

		private static int _ProcessSuperInputFanLi(DBManager dbMgr, DBUserInfo dbUserInfo, SingleChargeData chargeData, int addUserMoney, int ChargeID, string chargeTm)
		{
			try
			{
				string superInputFanLiKey = chargeData.SuperInputFanLiKey;
				if (string.IsNullOrEmpty(superInputFanLiKey))
				{
					return 0;
				}
				string[] array = superInputFanLiKey.Split(new char[]
				{
					'_'
				});
				if (array.Length != 2)
				{
					return 0;
				}
				DateTime t = DateTime.Parse(array[0]);
				DateTime t2 = DateTime.Parse(array[1]);
				if (TimeUtil.NowDateTime() < t || TimeUtil.NowDateTime() > t2)
				{
					return 0;
				}
				JieriSuperInputData jieriSuperInputDataByChargeTm = UserMoneyMgr.GetJieriSuperInputDataByChargeTm(chargeData, chargeTm);
				if (null == jieriSuperInputDataByChargeTm)
				{
					return 0;
				}
				string arg = jieriSuperInputDataByChargeTm.BeginTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string arg2 = jieriSuperInputDataByChargeTm.EndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string keystr = string.Format("res_{0}_{1}_{2}", arg, arg2, jieriSuperInputDataByChargeTm.ID);
				long num = 0L;
				string lastgettime = "";
				int num2 = DBQuery.GetAwardHistoryForUser(dbMgr, dbUserInfo.UserID, 71, keystr, out num, out lastgettime);
				if (num <= 0L)
				{
					return 0;
				}
				num -= 1L;
				lastgettime = chargeTm;
				num2 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbUserInfo.UserID, 71, keystr, num, lastgettime);
				if (num2 < 0)
				{
					num2 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbUserInfo.UserID, 71, keystr, num, lastgettime);
				}
				return jieriSuperInputDataByChargeTm.ID;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("_ProcessSuperInputFanLi:" + ex.ToString());
			}
			return 0;
		}

		private static void _ProcessBuyYueKa(DBManager dbMgr, DBUserInfo dbUserInfo)
		{
			int num = DBQuery.LastLoginRole(dbMgr, dbUserInfo.UserID);
			string gmCmd = string.Format("-buyyueka {0} {1}", dbUserInfo.UserID, num);
			LogManager.WriteLog(LogTypes.Error, string.Format("处理玩家购买月卡，userid={0}, roleid={1}", dbUserInfo.UserID, num), null, true);
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
		}

		private static void _ProcessCharge(DBManager dbMgr, DBUserInfo dbUserInfo, int chargeRoleID, int addUserMoney, int zhigouID, string chargeTm, SingleChargeData chargeData, bool bZhiGouFail = false)
		{
			int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0);
			int gameConfigItemInt2 = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
			int gameConfigItemInt3 = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-jifen", 1);
			int chargeID = 0;
			chargeData.MoneyVsChargeIDDict.TryGetValue(addUserMoney, out chargeID);
			bool flag = zhigouID == 0 && (chargeData.ChargePlatType == 1 || addUserMoney != chargeData.YueKaMoney);
			bool flag2 = chargeRoleID == -1;
			bool flag3 = zhigouID != 0 && chargeRoleID > 0;
			lock (dbUserInfo)
			{
				if (flag)
				{
					dbUserInfo.Money += addUserMoney * gameConfigItemInt2;
					if (dbUserInfo.Money < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("充值后玩家元宝变负数修正为0, UserID={0}, Money={1}, AddMoney={2}", dbUserInfo.UserID, dbUserInfo.Money, addUserMoney), null, true);
						dbUserInfo.Money = 0;
					}
				}
				if (!flag2 && !bZhiGouFail)
				{
					dbUserInfo.RealMoney += addUserMoney;
					if (dbUserInfo.RealMoney < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("充值后玩家realmoney变负数修正为0, UserID={0}, Money={1}, AddMoney={2}", dbUserInfo.UserID, dbUserInfo.RealMoney, addUserMoney), null, true);
						dbUserInfo.RealMoney = 0;
					}
					if (gameConfigItemInt != dbUserInfo.GiftID)
					{
						dbUserInfo.GiftJiFen = 0;
						dbUserInfo.GiftID = gameConfigItemInt;
					}
					if (dbUserInfo.GiftID > 0)
					{
						dbUserInfo.GiftJiFen += addUserMoney * gameConfigItemInt3;
					}
				}
				int money = dbUserInfo.Money;
				int realMoney = dbUserInfo.RealMoney;
				int giftID = dbUserInfo.GiftID;
				int giftJiFen = dbUserInfo.GiftJiFen;
				DBWriter.UpdateUserInfo(dbMgr, dbUserInfo);
			}
			DBRoleInfo dbroleInfo = Global.FindOnlineRoleInfoByUserInfo(dbMgr, dbUserInfo);
			if (dbroleInfo != null && !bZhiGouFail)
			{
				DBWriter.UpdateCityInfoItem(dbMgr, dbroleInfo.LastIP, dbroleInfo.UserID, "inputmoney", addUserMoney * gameConfigItemInt2);
			}
			int num = chargeRoleID;
			if (!flag2 && !flag3)
			{
				num = DBQuery.LastLoginRole(dbMgr, dbUserInfo.UserID);
			}
			int num2 = Global.TransMoneyToYuanBao(addUserMoney);
			if (flag3)
			{
				UserMoneyMgr._ProcessBuyItem(dbMgr, dbUserInfo, chargeRoleID, addUserMoney, zhigouID, chargeTm);
			}
			int num3 = 0;
			if (!flag3 && addUserMoney != chargeData.YueKaMoney && !bZhiGouFail)
			{
				num3 = UserMoneyMgr._ProcessSuperInputFanLi(dbMgr, dbUserInfo, chargeData, addUserMoney, chargeID, chargeTm);
			}
			if (!flag2 && flag)
			{
				CFirstChargeMgr.SendToRolebindgold(dbMgr, dbUserInfo.UserID, num, addUserMoney, chargeData);
			}
			if (!flag2 && !bZhiGouFail)
			{
				SingletonTemplate<FundManager>.Instance().FundAddMoney(dbUserInfo.UserID, num2, num, 0);
				GameDBManager.RankCacheMgr.OnUserDoSomething(num, RankType.Charge, num2);
			}
			string gmCmd = string.Format("-updateyb {0} {1} {2} {3} {4}", new object[]
			{
				dbUserInfo.UserID,
				num,
				addUserMoney,
				num3,
				zhigouID
			});
			ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
			LogManager.WriteLog(LogTypes.Error, string.Format("处理充值成功 UID={0}，money={1}，itemid={2}", dbUserInfo.UserID, addUserMoney, zhigouID), null, true);
		}

		public static void ScanInputLogToDBLog(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - UserMoneyMgr.LastScanInputLogTicks >= 30000L)
			{
				UserMoneyMgr.LastScanInputLogTicks = num;
				if (UserMoneyMgr.LastScanID < 0)
				{
					UserMoneyMgr.LastScanID = DBQuery.QueryLastScanInputLogID(dbMgr);
				}
				int num2 = DBQuery.ScanInputLogFromTable(dbMgr, UserMoneyMgr.LastScanID);
				if (num2 != UserMoneyMgr.LastScanID)
				{
					UserMoneyMgr.LastScanID = num2;
					DBWriter.UpdateLastScanInputLogID(dbMgr, UserMoneyMgr.LastScanID);
				}
			}
		}

		public static void GMAddCharge(string userid, string money, string rid, string itemid, string time)
		{
			if (string.IsNullOrEmpty(time))
			{
				time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			}
			Random random = new Random();
			string text = random.Next().ToString("X8");
			string text2 = Global.GCC(4, new object[]
			{
				text,
				userid,
				money,
				time
			});
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("insert into t_tempmoney values(null,'{0}','{1}','{2}',{3},{4}, '{5}');", new object[]
				{
					text2.Substring(0, 24) + text,
					userid,
					rid,
					money,
					itemid,
					time
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
				sql = string.Format("INSERT INTO `t_inputlog` VALUES (null, '{2}', '{0}', '{1}', '{5}', '{0}{5}', '1470151268', '{6}', '{4}', 'success', '54', {3}, '{4}');", new object[]
				{
					userid,
					rid,
					money,
					itemid,
					time,
					text,
					text2
				});
				myDbConnection.ExecuteNonQuery(sql, 0);
			}
		}

		public static void QueryTotalUserMoney()
		{
			if (GameDBManager.Flag_Query_Total_UserMoney_Minute >= 5)
			{
				DateTime now = DateTime.Now;
				bool flag = false;
				long num = (now.Ticks - UserMoneyMgr.LastLastQueryServerTotalUserMoneyTime.Ticks) / 10000L;
				if (num >= (long)(GameDBManager.Flag_Query_Total_UserMoney_Minute * 60 * 1000))
				{
					flag = true;
				}
				else if (now.Day != UserMoneyMgr.LastLastQueryServerTotalUserMoneyTime.Day)
				{
					flag = true;
				}
				if (flag)
				{
					UserMoneyMgr.LastLastQueryServerTotalUserMoneyTime = now;
					LogManager.WriteLog(LogTypes.TotalUserMoney, string.Format("{0}\t{1}\t{2}", 10000, GameDBManager.ZoneID, DBQuery.QueryServerTotalUserMoney()), null, true);
				}
			}
		}

		public static TCPProcessCmdResults ProcessGetChargeItemData(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				int num = Convert.ToInt32(array[1]);
				byte handleDel = Convert.ToByte(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<TempItemChargeInfo> instance = DBQuery.QueryTempItemChargeInfo(dbMgr, num, 0, handleDel);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<TempItemChargeInfo>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("ProcessGetChargeItemData:" + ex.ToString());
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessDelChargeItemData(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				SingleChargeData chargeData = CFirstChargeMgr.ChargeData;
				if (chargeData == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<TempItemChargeInfo> list = DBQuery.QueryTempItemChargeInfo(dbMgr, 0, num, 0);
				if (list.Count == 0)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = list[0].userID;
				int chargeRoleID = list[0].chargeRoleID;
				int addUserMoney = list[0].addUserMoney;
				int zhigouID = list[0].zhigouID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (dbuserInfo == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				byte isDel = (num2 == 1) ? 2 : 1;
				if (!DBWriter.DeleteChargeItemInfo(dbMgr, num, isDel))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 == 1)
				{
					UserMoneyMgr._ProcessCharge(dbMgr, dbuserInfo, chargeRoleID, addUserMoney, 0, "", chargeData, true);
				}
				else if (num3 > 0)
				{
					UserMoneyMgr._ProcessCharge(dbMgr, dbuserInfo, chargeRoleID, num3, 0, "", chargeData, true);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("ProcessDelChargeItemData:" + ex.ToString());
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static long LastUpdateUserMoneyTicks = 0L;

		private static bool ChargeDataLogState = false;

		private static long LastScanInputLogTicks = DateTime.Now.Ticks / 10000L;

		private static int LastScanID = -1;

		private static DateTime LastLastQueryServerTotalUserMoneyTime = DateTime.MinValue;
	}
}
