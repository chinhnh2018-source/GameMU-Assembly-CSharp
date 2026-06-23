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
	public class RechargeRepayActiveMgr
	{
		private static bool GetCmdDataField(int nID, byte[] data, int count, out string[] fields)
		{
			string text = null;
			fields = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return false;
			}
			fields = text.Split(new char[]
			{
				':'
			});
			return true;
		}

		public static TCPProcessCmdResults ProcessQueryActiveInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			string data2;
			try
			{
				if (!RechargeRepayActiveMgr.GetCmdDataField(nID, data, count, out array))
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Global.SafeConvertToInt32(array[1], 10);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long num3 = 0L;
				string text = "";
				string keystr = "not_limit";
				string text2 = "";
				int num4 = num2;
				if (num4 <= 27)
				{
					if (num4 != 23)
					{
						if (num4 == 27)
						{
							DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
							DateTime toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
							keystr = Global.GetHuoDongKeyString(fromDate.ToString("yyyy-MM-dd HH:mm:ss"), toDate.ToString("yyyy-MM-dd HH:mm:ss"));
							RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
							int rankValue = dbroleInfo.RankValue.GetRankValue(key);
							text2 = string.Concat(rankValue);
						}
					}
					else
					{
						if (5 != array.Length)
						{
							return TCPProcessCmdResults.RESULT_DATA;
						}
						int num5 = Global.SafeConvertToInt32(array[2], 10);
						int num6 = Global.SafeConvertToInt32(array[3], 10);
						keystr = Global.GetHuoDongKeyString(num5.ToString(), num6.ToString());
						Dictionary<int, float> dictionary = new Dictionary<int, float>();
						string text3 = array[4];
						string[] array2 = text3.Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < array2.Length; i++)
						{
							string[] array3 = array2[i].Split(new char[]
							{
								','
							});
							if (2 == array3.Length)
							{
								int key2 = Global.SafeConvertToInt32(array3[0], 10);
								float value = (float)Convert.ToDouble(array3[1]);
								dictionary[key2] = value;
							}
						}
						DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 23, keystr, out num3, out text);
						int num7 = 0;
						if (!string.IsNullOrEmpty(text))
						{
							num7 = Global.GetOffsetDay(DateTime.Parse(text));
						}
						int num8;
						if (num7 < num5)
						{
							num8 = num5;
						}
						else if (num3 > 0L)
						{
							num8 = num7;
						}
						else
						{
							num8 = num5;
						}
						int offsetDay = Global.GetOffsetDay(DateTime.Now);
						int num9 = offsetDay - 1;
						if (num9 > num6)
						{
							num9 = num6;
						}
						int num10 = 0;
						if (num8 == offsetDay)
						{
							num10 = 0;
						}
						else
						{
							for (int i = num8; i <= num9; i++)
							{
								DateTime realDate = Global.GetRealDate(i);
								string text4 = new DateTime(realDate.Year, realDate.Month, realDate.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
								string toDate2 = new DateTime(realDate.Year, realDate.Month, realDate.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
								int roleRankByDay = GameDBManager.DayRechargeRankMgr.GetRoleRankByDay(dbMgr, dbroleInfo.UserID, i);
								if (dictionary.ContainsKey(roleRankByDay))
								{
									RankDataKey key = new RankDataKey(RankType.Charge, text4, toDate2, null);
									int rankValue2 = dbroleInfo.RankValue.GetRankValue(key);
									num10 += (int)((float)rankValue2 * dictionary[roleRankByDay]);
								}
							}
						}
						text2 += num10;
						text2 += ":";
						if (offsetDay > num6)
						{
							text2 += "0";
						}
						else
						{
							List<InputKingPaiHangData> rankByDay = GameDBManager.DayRechargeRankMgr.GetRankByDay(dbMgr, offsetDay);
							text2 += rankByDay.Count;
							int num11 = 1;
							foreach (InputKingPaiHangData inputKingPaiHangData in rankByDay)
							{
								text2 += "|";
								text2 += num11;
								text2 += ",";
								text2 += inputKingPaiHangData.MaxLevelRoleZoneID;
								text2 += ",";
								text2 += inputKingPaiHangData.MaxLevelRoleName;
								num11++;
							}
						}
					}
				}
				else
				{
					switch (num4)
					{
					case 38:
					{
						int num12 = 0;
						int num13 = 0;
						DBQuery.QueryUserMoneyByUserID(dbMgr, dbroleInfo.UserID, out num13, out num12);
						num12 = Global.TransMoneyToYuanBao(num12);
						text2 = string.Concat(num12);
						break;
					}
					case 39:
					{
						string text4 = "2011-11-11";
						string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						text2 = DBQuery.GetUserUsedMoney(dbMgr, num, text4, endTime).ToString();
						break;
					}
					default:
						switch (num4)
						{
						case 46:
						{
							if (array.Length != 4)
							{
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string fromDate2 = array[2].Replace('$', ':');
							string toDate3 = array[3].Replace('$', ':');
							keystr = Global.GetHuoDongKeyString(fromDate2, toDate3);
							RankDataKey key = new RankDataKey(RankType.Charge, fromDate2, toDate3, null);
							int rankValue = dbroleInfo.RankValue.GetRankValue(key);
							text2 = string.Concat(rankValue);
							break;
						}
						case 48:
						{
							DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
							DateTime toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
							keystr = Global.GetHuoDongKeyString(fromDate.ToString("yyyy-MM-dd HH:mm:ss"), toDate.ToString("yyyy-MM-dd HH:mm:ss"));
							RankDataKey key3 = new RankDataKey(RankType.Charge, fromDate, toDate, null);
							int rankValue3 = dbroleInfo.RankValue.GetRankValue(key3);
							RankDataKey key4 = new RankDataKey(RankType.Consume, fromDate, toDate, null);
							int rankValue4 = dbroleInfo.RankValue.GetRankValue(key4);
							text2 = string.Format("{0},{1}", rankValue3, rankValue4);
							break;
						}
						}
						break;
					}
				}
				lock (dbroleInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, num2, keystr, out num3, out text);
					string text5 = "";
					if (num2 == 48)
					{
						text5 = num3.ToString();
					}
					else
					{
						string text6 = num3.ToString();
						if (num3 != 0L)
						{
							int i = 0;
							foreach (char c in text6.ToCharArray())
							{
								text5 += c;
								i++;
								if (i < text6.Length)
								{
									text5 += ",";
								}
							}
						}
						if (string.IsNullOrEmpty(text5))
						{
							text5 = "1";
						}
					}
					data2 = string.Format("{0}:{1}:{2}", 1, text5, text2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			data2 = string.Format("{0}:{1}:{2}", 0, "", "");
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessGetActiveAwards(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			TCPProcessCmdResults result;
			if (!RechargeRepayActiveMgr.GetCmdDataField(nID, data, count, out array))
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int num2 = Global.SafeConvertToInt32(array[1], 10);
				int num3 = Global.SafeConvertToInt32(array[2], 10);
				long num4 = Global.SafeConvertToInt64(array[2], 10);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					string keystr = "not_limit";
					int num5 = num2;
					string data2;
					if (num5 <= 27)
					{
						if (num5 != 23)
						{
							if (num5 == 27)
							{
								DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
								DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
								keystr = Global.GetHuoDongKeyString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
							}
						}
						else
						{
							if (5 != array.Length)
							{
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num6 = Global.SafeConvertToInt32(array[2], 10);
							int num7 = Global.SafeConvertToInt32(array[3], 10);
							keystr = Global.GetHuoDongKeyString(num6.ToString(), num7.ToString());
							Dictionary<int, float> dictionary = new Dictionary<int, float>();
							string text = array[4];
							string[] array2 = text.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < array2.Length; i++)
							{
								string[] array3 = array2[i].Split(new char[]
								{
									','
								});
								if (2 == array3.Length)
								{
									int key = Global.SafeConvertToInt32(array3[0], 10);
									float value = (float)Convert.ToDouble(array3[1]);
									dictionary[key] = value;
								}
							}
							int num8 = 0;
							string text2 = "";
							DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 23, keystr, out num8, out text2);
							int num9 = 0;
							if (!string.IsNullOrEmpty(text2))
							{
								num9 = Global.GetOffsetDay(DateTime.Parse(text2));
							}
							int num10;
							if (num9 < num6)
							{
								num10 = num6;
							}
							else if (num8 > 0)
							{
								num10 = num9;
							}
							else
							{
								num10 = num6;
							}
							int offsetDay = Global.GetOffsetDay(DateTime.Now);
							if (num10 == offsetDay)
							{
								data2 = string.Format("{0}:{1}:{2}", 1, num2, 0);
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							int num11 = Global.GetOffsetDay(DateTime.Now) - 1;
							if (num11 > num7)
							{
								num11 = num7;
							}
							int num12 = 0;
							for (int i = num10; i <= num11; i++)
							{
								DateTime realDate = Global.GetRealDate(i);
								string fromDate = new DateTime(realDate.Year, realDate.Month, realDate.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
								string toDate = new DateTime(realDate.Year, realDate.Month, realDate.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
								int key = GameDBManager.DayRechargeRankMgr.GetRoleRankByDay(dbMgr, dbroleInfo.UserID, i);
								if (dictionary.ContainsKey(key))
								{
									RankDataKey key2 = new RankDataKey(RankType.Charge, fromDate, toDate, null);
									int rankValue = dbroleInfo.RankValue.GetRankValue(key2);
									num12 += (int)((float)rankValue * dictionary[key]);
								}
							}
							data2 = string.Format("{0}:{1}:{2}", 1, num2, num12);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						switch (num5)
						{
						case 38:
						case 39:
							lock (dbroleInfo)
							{
								int num13 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num2, keystr, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
								if (num13 < 0)
								{
									num13 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num2, keystr, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
								}
								if (num13 < 0)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
									return TCPProcessCmdResults.RESULT_FAILED;
								}
							}
							data2 = string.Format("{0}:{1}:{2}", 1, num2, num4);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						default:
							switch (num5)
							{
							case 46:
							{
								if (array.Length != 5)
								{
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string fromDate2 = array[3].Replace('$', ':');
								string toDate2 = array[4].Replace('$', ':');
								keystr = Global.GetHuoDongKeyString(fromDate2, toDate2);
								break;
							}
							case 48:
							{
								DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
								DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
								keystr = Global.GetHuoDongKeyString(dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
								break;
							}
							}
							break;
						}
					}
					lock (dbroleInfo)
					{
						int num13 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num2, keystr, (long)num3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num13 < 0)
						{
							num13 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num2, keystr, (long)num3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						}
						if (num13 < 0)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
							return TCPProcessCmdResults.RESULT_FAILED;
						}
					}
					data2 = string.Format("{0}:{1}:{2}", 1, num2, num3);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
			}
			return result;
		}
	}
}
