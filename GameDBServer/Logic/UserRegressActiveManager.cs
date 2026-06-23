using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class UserRegressActiveManager
	{
		public static TCPProcessCmdResults ProcessGetRegressActiveMinTime(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!dbroleInfo.UserID.Equals(text2))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("请求的UserID出错，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string arg;
				string text3;
				if (!DBQuery.GetMinRegtime(dbMgr, text2, out arg, out text3))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("获取最早注册时间出错，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				text = string.Format("{0}:{1}", arg, text3.Replace(":", "$"));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessUpdateEverySignCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace("$", ":");
				string toDate = array[3].Replace("$", ":");
				string activedata = array[4];
				string stage = array[5];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = DBWriter.AddRegressHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 111, huoDongKeyString, (long)num2, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), activedata, stage);
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}", num, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessSprQueryDayUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int activitytype = Global.SafeConvertToInt32(array[1], 10);
				string stage = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = "";
				int num2 = 0;
				string text3 = "";
				switch (activitytype)
				{
				case 111:
				{
					DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
					DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
					string huoDongKeyString = Global.GetHuoDongKeyString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
					int num3 = DBQuery.GetRegressAwardDayHistoryForUser(dbMgr, dbroleInfo.UserID, activitytype, huoDongKeyString, stage, out text2, out num2, out text3);
					if (num3 < 0)
					{
						text2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						num2 = 0;
						text3 = "";
						num3 = DBWriter.AddRegressHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, activitytype, huoDongKeyString, (long)num2, text2, text3, stage);
						if (num3 < 0)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					break;
				}
				case 112:
				{
					DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
					DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
					string huoDongKeyString = Global.GetHuoDongKeyString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
					Dictionary<string, string> dictionary;
					if (!DBQuery.GetRegressAwardHistoryForUser(dbMgr, dbroleInfo.UserID, activitytype, stage, out dictionary))
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					lock (dictionary)
					{
						foreach (string str in dictionary.Values)
						{
							text3 += str;
						}
					}
					break;
				}
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					text2.Replace(":", "$"),
					text3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessUpdateSprQueryDayUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int activitytype = Global.SafeConvertToInt32(array[1], 10);
				string activedata = array[2];
				string stage = array[3];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
				DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
				string huoDongKeyString = Global.GetHuoDongKeyString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
				string text2;
				int num3;
				string text3;
				int num2 = DBQuery.GetRegressAwardDayHistoryForUser(dbMgr, dbroleInfo.UserID, activitytype, huoDongKeyString, stage, out text2, out num3, out text3);
				if (num2 < 0)
				{
					text2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					num3 = 0;
					num2 = DBWriter.AddRegressHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, activitytype, huoDongKeyString, (long)num3, text2, activedata, stage);
					if (num2 < 0)
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				num2 = DBWriter.UpdateRegressHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, activitytype, huoDongKeyString, (long)num3, text2, activedata, stage);
				if (num2 < 0)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num3,
					text2.Replace(":", "$"),
					text3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessSprQueryUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int activitytype = Global.SafeConvertToInt32(array[1], 10);
				string stage = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					string data2 = string.Format("{0}:{1}:0:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<string, string> instance;
				if (!DBQuery.GetRegressAwardHistoryForUser(dbMgr, dbroleInfo.UserID, activitytype, stage, out instance))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessDBUpdateUserLimitGoodsUsedNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int goodsID = Convert.ToInt32(array[1]);
				int dayID = Convert.ToInt32(array[2]);
				int usedNum = Convert.ToInt32(array[3]);
				string stage = array[4];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddUserLimitGoodsBuyItem(dbMgr, dbroleInfo.UserID, goodsID, dayID, usedNum, stage);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加限购物品的历史记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessDBQueryLimitGoodsUsedNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int goodsID = Convert.ToInt32(array[1]);
				string stage = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				int num3 = 0;
				int num4 = DBQuery.QueryUserLimitGoodsUsedNumByRoleID(dbMgr, dbroleInfo.UserID, goodsID, stage, out num2, out num3);
				string data2;
				if (num4 < 0)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num4,
						num2,
						num3
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("通过UserID和goodsID查询物品每日的已经购买数量失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						0,
						num2,
						num3
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessDBQueryUserAllLimitGoodsUsedNumInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int dayID = Convert.ToInt32(array[1]);
				string stage = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, int> instance;
				if (!DBQuery.QueryUserLimitGoodsUsedNumInfoByRoleID(dbMgr, dbroleInfo.UserID, dayID, stage, out instance))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("通过UserID和DayID查询当天商城购买信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, int>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessRergressQueryUserInputMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				string startTime = array[1].Replace('$', ':');
				string endTime = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int userInputMoney = DBQuery.GetUserInputMoney(TCPManager.getInstance().DBMgr, dbroleInfo.UserID, 0, startTime, endTime);
				int num2 = Global.TransMoneyToYuanBao(userInputMoney);
				data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
