using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class RebornEquip : SingletonTemplate<RebornEquip>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14123, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14124, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14125, SingletonTemplate<RebornEquip>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14126, SingletonTemplate<RebornEquip>.Instance());
			return true;
		}

		public static TCPProcessCmdResults ProcessUpdateRoleRebornBagNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateRoleRebornBagNum(dbMgr, num, num2);
				string data2;
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num3);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.RebornBagNum = num2;
					}
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

		public static TCPProcessCmdResults ProcessUpdateRebornStorageInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int extGridNum = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateRoleRebornStorageInfo(dbMgr, num, extGridNum);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.RebornGirdData.ExtGridNum = extGridNum;
					}
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

		public static TCPProcessCmdResults ProcessUpdateRoleRebornShowEquipCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateRoleRebornShowEquip(dbMgr, num, num2);
				string data2;
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num3);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色显示装备信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.RebornShowEquip = num2;
					}
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

		public static TCPProcessCmdResults ProcessUpdateRoleRebornShowModelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateRoleRebornShowModel(dbMgr, num, num2);
				string data2;
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num3);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色显示时装信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.RebornShowModel = num2;
					}
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

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 14123:
				this.InsertHoleInfo(client, nID, cmdParams, count);
				break;
			case 14124:
				this.UpdateHoleInfo(client, nID, cmdParams, count);
				break;
			case 14125:
				this.InsertMazingerStoreInfo(client, nID, cmdParams, count);
				break;
			case 14126:
				this.UpdateMazingerStoreInfo(client, nID, cmdParams, count);
				break;
			}
		}

		public static Dictionary<int, RebornEquipData> GetRebornEquipHoleData(DBRoleInfo dbRoleInfo)
		{
			Dictionary<int, RebornEquipData> result = null;
			DBQuery.GetAllRebornEquipHole(DBManager.getInstance(), dbRoleInfo.RoleID, out result);
			return result;
		}

		public void InsertHoleInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				RebornEquipData rebornEquipData = DataHelper.BytesToObject<RebornEquipData>(cmdParams, 0, count);
				if (rebornEquipData != null)
				{
					if (!DBWriter.InsertRebornEquipHoleInfo(rebornEquipData.RoleID, rebornEquipData.HoleID, rebornEquipData.Level, rebornEquipData.Able))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("插入重生装备槽信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, rebornEquipData.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}

		public void UpdateHoleInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				RebornEquipData rebornEquipData = DataHelper.BytesToObject<RebornEquipData>(cmdParams, 0, count);
				if (rebornEquipData != null)
				{
					if (!DBWriter.UpdateRebornEquipHoleInfo(rebornEquipData.RoleID, rebornEquipData.HoleID, rebornEquipData.Level, rebornEquipData.Able))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新重生装备槽信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, rebornEquipData.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}

		public static Dictionary<int, MazingerStoreData> GetMazingerStoreData(DBRoleInfo dbRoleInfo)
		{
			Dictionary<int, MazingerStoreData> result = null;
			DBQuery.GetMazingerStoreInfo(DBManager.getInstance(), dbRoleInfo.RoleID, out result);
			return result;
		}

		public void InsertMazingerStoreInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				MazingerStoreData mazingerStoreData = DataHelper.BytesToObject<MazingerStoreData>(cmdParams, 0, count);
				if (mazingerStoreData != null)
				{
					if (!DBWriter.InsertMazingerStoreInfo(mazingerStoreData.RoleID, mazingerStoreData.Type, mazingerStoreData.Stage, mazingerStoreData.StarLevel, mazingerStoreData.Exp))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("插入魔神秘宝信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, mazingerStoreData.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}

		public void UpdateMazingerStoreInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				MazingerStoreData mazingerStoreData = DataHelper.BytesToObject<MazingerStoreData>(cmdParams, 0, count);
				if (mazingerStoreData != null)
				{
					if (!DBWriter.UpdateMazingerStoreInfo(mazingerStoreData.RoleID, mazingerStoreData.Type, mazingerStoreData.Stage, mazingerStoreData.StarLevel, mazingerStoreData.Exp))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新魔神秘宝信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, mazingerStoreData.RoleID), null, true);
						client.sendCmd<int>(nID, -1);
					}
				}
			}
			catch (Exception ex)
			{
				client.sendCmd<int>(nID, -1);
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, 1);
		}
	}
}
