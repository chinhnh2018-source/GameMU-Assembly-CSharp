using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	internal class CFirstChargeMgr
	{
		public static SingleChargeData ChargeData
		{
			get
			{
				SingleChargeData chargeData;
				lock (CFirstChargeMgr.SingleChargeDataMutex)
				{
					chargeData = CFirstChargeMgr._ChargeData;
				}
				return chargeData;
			}
			set
			{
				lock (CFirstChargeMgr.SingleChargeDataMutex)
				{
					CFirstChargeMgr._ChargeData = value;
				}
			}
		}

		public static TCPProcessCmdResults FirstChargeConfig(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				SingleChargeData chargeData = DataHelper.BytesToObject<SingleChargeData>(data, 0, count);
				CFirstChargeMgr.ChargeData = chargeData;
				byte[] array = DataHelper.ObjectToBytes<int>(1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static bool HasGetFirstbindMoney(int money, string[] binddatalist)
		{
			bool result;
			if (null == binddatalist)
			{
				result = false;
			}
			else
			{
				int num = binddatalist.Length;
				for (int i = 0; i < num; i++)
				{
					if (binddatalist[i] == money.ToString())
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private static List<int> MuiltchargeGetBindmoney(int addMoney, int nPlatfromID, string[] binddatalist, CFirstChargeMgr.ChargeType eChargeType, SingleChargeData chargeData)
		{
			List<int> list = new List<int>();
			switch (eChargeType)
			{
			case CFirstChargeMgr.ChargeType.YingYongBao:
			{
				List<int> list2 = chargeData.singleData.Keys.ToList<int>();
				list2.Sort();
				int num = 0;
				int i = list2.Count - 1;
				while (i >= 0)
				{
					if (list2[i] <= addMoney)
					{
						if (list2[i] != chargeData.YueKaMoney || chargeData.ChargePlatType == 1)
						{
							if (!CFirstChargeMgr.HasGetFirstbindMoney(list2[i], binddatalist))
							{
								if (num + list2[i] <= addMoney)
								{
									list.Add(list2[i]);
									num += list2[i];
								}
							}
						}
					}
					IL_CB:
					i--;
					continue;
					goto IL_CB;
				}
				break;
			}
			case CFirstChargeMgr.ChargeType.GangAoTai:
			{
				List<int> list2 = chargeData.singleData.Keys.ToList<int>();
				list2.Sort();
				for (int i = list2.Count - 1; i >= 0; i--)
				{
					if (list2[i] <= addMoney)
					{
						if (chargeData.YueKaBangZuan == 1 || list2[i] != chargeData.YueKaMoney)
						{
							if (!CFirstChargeMgr.HasGetFirstbindMoney(list2[i], binddatalist))
							{
								list.Add(list2[i]);
								break;
							}
						}
					}
				}
				break;
			}
			default:
			{
				int num2 = 0;
				chargeData.singleData.TryGetValue(addMoney, out num2);
				if (num2 > 0)
				{
					if (chargeData.YueKaBangZuan == 1 || addMoney != chargeData.YueKaMoney || chargeData.ChargePlatType == 1)
					{
						if (!CFirstChargeMgr.HasGetFirstbindMoney(addMoney, binddatalist))
						{
							list.Add(addMoney);
						}
					}
				}
				break;
			}
			}
			return list;
		}

		public static void SendToRolebindgold(DBManager dbMgr, string uid, int rid, int addMoney, SingleChargeData chargeData)
		{
			if (chargeData == null)
			{
				LogManager.WriteException(string.Concat(new object[]
				{
					"送绑钻失败，配置表信息为空 uid=",
					uid,
					" money=",
					addMoney
				}));
			}
			else
			{
				string text = CFirstChargeMgr.GetFirstChargeInfo(dbMgr, uid);
				string a = (uid.Length >= 4) ? uid.Substring(0, 4) : "";
				int nPlatfromID = 1;
				if (a == "APPS")
				{
					nPlatfromID = 2;
				}
				CFirstChargeMgr.ChargeType eChargeType = CFirstChargeMgr.ChargeType.Normal;
				string a2 = (uid.Length >= 3) ? uid.Substring(0, 3) : "";
				if (a2 == "YYB")
				{
					eChargeType = CFirstChargeMgr.ChargeType.YingYongBao;
				}
				else if (a2 == "GAT" || a2 == "430")
				{
					eChargeType = CFirstChargeMgr.ChargeType.GangAoTai;
				}
				string[] binddatalist = null;
				if (!string.IsNullOrEmpty(text))
				{
					binddatalist = text.Split(new char[]
					{
						','
					});
				}
				List<int> list = CFirstChargeMgr.MuiltchargeGetBindmoney(addMoney, nPlatfromID, binddatalist, eChargeType, chargeData);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!string.IsNullOrEmpty(text))
						{
							text = text + "," + list[i];
						}
						else
						{
							text = string.Concat(list[i]);
						}
					}
					if (!CFirstChargeMgr.UpdateFirstCharge(dbMgr, uid, text, 0))
					{
						LogManager.WriteException(string.Concat(new object[]
						{
							"送绑钻失败，保存数据库失败 uid=",
							uid,
							" money=",
							addMoney
						}));
					}
					else
					{
						for (int i = 0; i < list.Count; i++)
						{
							int num = chargeData.singleData[list[i]];
							string gmCmd = string.Format("-updateBindgold {0} {1} {2} {3}", new object[]
							{
								uid,
								rid,
								num,
								text
							});
							ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
						}
					}
				}
			}
		}

		public static string GetFirstChargeInfo(DBManager dbMgr, string uid)
		{
			string text = "-1";
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text2 = string.Format("SELECT charge_info FROM t_firstcharge WHERE uid = '{0}'", uid);
				MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					if (mySQLDataReader.Read())
					{
						text = mySQLDataReader["charge_info"].ToString();
						if (string.IsNullOrEmpty(text))
						{
							text = "-1";
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException("GetFirstChargeInfo excepton=" + ex.ToString());
					text = "-2";
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return text;
		}

		public static bool UpdateFirstCharge(DBManager dbMgr, string userId, string chargeinfo, int notget = 0)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE  INTO t_firstcharge (uid, charge_info, notget) VALUES('{0}', '{1}', '{2}')", userId, chargeinfo, notget);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessQueryUserFirstCharge(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的账号不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, text2), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo2 = dbMgr.GetDBUserInfo(text2);
				string firstChargeInfo = CFirstChargeMgr.GetFirstChargeInfo(dbMgr, text2);
				if (firstChargeInfo != "-2")
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, firstChargeInfo, nID);
				}
				else
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("ProcessSaveUserFirstCharge:" + ex.ToString());
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static SingleChargeData _ChargeData = null;

		private static object SingleChargeDataMutex = new object();

		private enum ChargeType
		{
			Normal,
			YingYongBao,
			GangAoTai
		}
	}
}
