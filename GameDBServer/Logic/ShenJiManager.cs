using System;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class ShenJiManager : SingletonTemplate<ShenJiManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13095, SingletonTemplate<ShenJiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13096, SingletonTemplate<ShenJiManager>.Instance());
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
			case 13095:
				this.ShenJiModify(client, nID, cmdParams, count);
				break;
			case 13096:
				this.ShenJiClear(client, nID, cmdParams, count);
				break;
			}
		}

		private void ShenJiModify(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] array = null;
			int length = 3;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int num = int.Parse(array[0]);
				int num2 = int.Parse(array[1]);
				int num3 = int.Parse(array[2]);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("精灵神迹，找不到玩家 roleid={0}", num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool flag = false;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("REPLACE INTO t_shenjifuwen(rid, sjID, level) VALUES('{0}', '{1}', '{2}')", num, num2, num3);
						flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
					if (!flag)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新精灵神迹数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						client.sendCmd<bool>(nID, flag);
					}
					else
					{
						lock (dbroleInfo.ShenJiDict)
						{
							ShenJiFuWenData value = new ShenJiFuWenData
							{
								ShenJiID = num2,
								Level = num3
							};
							dbroleInfo.ShenJiDict[num2] = value;
						}
						client.sendCmd<bool>(nID, flag);
					}
				}
			}
		}

		private void ShenJiClear(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] array = null;
			int length = 1;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int num = int.Parse(array[0]);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("精灵神迹，找不到玩家 roleid={0}", num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool flag = false;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("DELETE FROM t_shenjifuwen where rid={0}", num);
						flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
					if (!flag)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新精灵神迹数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						client.sendCmd<bool>(nID, flag);
					}
					else
					{
						lock (dbroleInfo.ShenJiDict)
						{
							dbroleInfo.ShenJiDict.Clear();
						}
						client.sendCmd<bool>(nID, flag);
					}
				}
			}
		}
	}
}
