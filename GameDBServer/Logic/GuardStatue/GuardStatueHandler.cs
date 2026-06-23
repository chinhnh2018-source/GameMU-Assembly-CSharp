using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.GuardStatue
{
	public class GuardStatueHandler : SingletonTemplate<GuardStatueHandler>
	{
		private GuardStatueHandler()
		{
		}

		public TCPProcessCmdResults ProcUpdateRoleGuardStatue(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				int num = Convert.ToInt32(array[1]);
				int level = Convert.ToInt32(array[2]);
				int suit = Convert.ToInt32(array[3]);
				int num2 = Convert.ToInt32(array[4]);
				int lastdayRecoverPoint = Convert.ToInt32(array[5]);
				int lastdayRecoverOffset = Convert.ToInt32(array[6]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3;
				if (this._UpdateRoleGuardStatue(dbMgr, roleID, num, level, suit, num2, lastdayRecoverPoint, lastdayRecoverOffset))
				{
					lock (dbroleInfo)
					{
						if (dbroleInfo.MyGuardStatueDetail == null)
						{
							dbroleInfo.MyGuardStatueDetail = new GuardStatueDetail();
							dbroleInfo.MyGuardStatueDetail.IsActived = true;
						}
						dbroleInfo.MyGuardStatueDetail.GuardStatue.Level = level;
						dbroleInfo.MyGuardStatueDetail.GuardStatue.Suit = suit;
						dbroleInfo.MyGuardStatueDetail.GuardStatue.HasGuardPoint = num2;
						dbroleInfo.MyGuardStatueDetail.LastdayRecoverPoint = lastdayRecoverPoint;
						dbroleInfo.MyGuardStatueDetail.LastdayRecoverOffset = lastdayRecoverOffset;
						dbroleInfo.MyGuardStatueDetail.ActiveSoulSlot = num;
					}
					num3 = 1;
				}
				else
				{
					num3 = -1;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", num3), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private bool _UpdateRoleGuardStatue(DBManager dbMgr, int roleID, int slotCnt, int level, int suit, int totalGuardPoint, int lastdayRecoverPoint, int lastdayRecoverOffset)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_guard_statue(roleid,slot_cnt,level,suit,total_guard_point,lastday_recover_point,lastday_recover_offset) VALUES({0},{1},{2},{3},{4},{5},{6})", new object[]
				{
					roleID,
					slotCnt,
					level,
					suit,
					totalGuardPoint,
					lastdayRecoverPoint,
					lastdayRecoverOffset
				});
				if (myDbConnection.ExecuteNonQuery(sql, 0) > 0)
				{
					result = true;
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcUpdateRoleGuardSoul(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleID = Convert.ToInt32(array[0]);
				int soulType = Convert.ToInt32(array[1]);
				int equipSlot = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num;
				if (this._UpdateRoleGuardSoul(dbMgr, roleID, soulType, equipSlot))
				{
					lock (dbroleInfo)
					{
						if (dbroleInfo.MyGuardStatueDetail == null)
						{
							dbroleInfo.MyGuardStatueDetail = new GuardStatueDetail();
							dbroleInfo.MyGuardStatueDetail.IsActived = true;
						}
						dbroleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.RemoveAll((GuardSoulData soul) => soul.Type == soulType);
						dbroleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(new GuardSoulData
						{
							Type = soulType,
							EquipSlot = equipSlot
						});
					}
					num = 1;
				}
				else
				{
					num = 1;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", num), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private bool _UpdateRoleGuardSoul(DBManager dbMgr, int roleID, int soulType, int equipSlot)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_guard_soul(roleid,soul_type,equip_slot) VALUES({0},{1},{2});", roleID, soulType, equipSlot);
				if (myDbConnection.ExecuteNonQuery(sql, 0) > 0)
				{
					result = true;
				}
			}
			return result;
		}

		public const string TableGuardStatue = "t_guard_statue";

		public const string TableGuardSoul = "t_guard_soul";
	}
}
