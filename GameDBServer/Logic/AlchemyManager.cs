using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class AlchemyManager : SingletonTemplate<AlchemyManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13097, SingletonTemplate<AlchemyManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13098, SingletonTemplate<AlchemyManager>.Instance());
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

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13097:
				this.AlchemyModify(client, nID, cmdParams, count);
				break;
			case 13098:
				this.AlchemyRollBack(client, nID, cmdParams, count);
				break;
			}
		}

		public TCPProcessCmdResults ProcessUpdateAlchemyElement(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (dbroleInfo == null || null == dbroleInfo.AlchemyInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.AlchemyInfo.BaseData.Element += num2;
					num3 = dbroleInfo.AlchemyInfo.BaseData.Element;
				}
				string data2;
				if (flag)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					bool flag3 = false;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("UPDATE t_alchemy SET element='{0}' WHERE rid={1};", num3, num);
						flag3 = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
					if (!flag3)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色炼金元素值失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						data2 = string.Format("{0}:{1}", num, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private void AlchemyRollBack(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string[] array = null;
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
				{
					client.sendCmd<bool>(nID, false);
				}
				else
				{
					int num = int.Parse(array[0]);
					string text = array[1];
					DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
					if (null == dbroleInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("精灵神迹，找不到玩家 roleid={0}", num), null, true);
						client.sendCmd(30767, "0");
					}
					else
					{
						bool cmdData = false;
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							string sql = string.Format("UPDATE t_alchemy SET rollback='{0}' WHERE rid={1};", text, num);
							cmdData = myDbConnection.ExecuteNonQueryBool(sql, 0);
						}
						lock (dbroleInfo)
						{
							if (null != dbroleInfo.AlchemyInfo)
							{
								dbroleInfo.AlchemyInfo.rollbackType = text;
							}
						}
						client.sendCmd<bool>(nID, cmdData);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		private void AlchemyModify(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				AlchemyDataDB alchemyDataDB = DataHelper.BytesToObject<AlchemyDataDB>(cmdParams, 0, count);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref alchemyDataDB.RoleID);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("炼金系统，找不到玩家 roleid={0}", alchemyDataDB.RoleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool flag = false;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("REPLACE INTO t_alchemy(rid, element, dayid, value, todaycost, histcost) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
						{
							alchemyDataDB.RoleID,
							alchemyDataDB.BaseData.Element,
							alchemyDataDB.ElementDayID,
							alchemyDataDB.getStringValue(alchemyDataDB.BaseData.AlchemyValue),
							alchemyDataDB.getStringValue(alchemyDataDB.BaseData.ToDayCost),
							alchemyDataDB.getStringValue(alchemyDataDB.HistCost)
						});
						flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
					if (!flag)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色炼金数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, alchemyDataDB.RoleID), null, true);
						client.sendCmd<bool>(nID, flag);
					}
					else
					{
						lock (dbroleInfo)
						{
							dbroleInfo.AlchemyInfo = alchemyDataDB;
						}
						client.sendCmd<bool>(nID, flag);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}
	}
}
