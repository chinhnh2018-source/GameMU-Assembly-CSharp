using System;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class ShenShiManager : SingletonTemplate<ShenShiManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20315, SingletonTemplate<ShenShiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20316, SingletonTemplate<ShenShiManager>.Instance());
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
			if (nID == 20315)
			{
				this.AddRoleFuWenTab(client, nID, cmdParams, count);
			}
			else if (nID == 20316)
			{
				this.UpdateRoleFuWenTab(client, nID, cmdParams, count);
			}
		}

		private void AddRoleFuWenTab(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				FuWenTabData fuWenTabData = DataHelper.BytesToObject<FuWenTabData>(cmdParams, 0, count);
				int ownerID = fuWenTabData.OwnerID;
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref ownerID);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, ownerID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool flag = false;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string text = (fuWenTabData.FuWenEquipList == null) ? "" : string.Join<int>(",", fuWenTabData.FuWenEquipList);
						string text2 = (fuWenTabData.ShenShiActiveList == null) ? "" : string.Join<int>(",", fuWenTabData.ShenShiActiveList);
						string sql = string.Format("REPLACE INTO t_fuwen(rid, tabid, name, fuwenequip, shenshiactive, skillequip) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
						{
							ownerID,
							fuWenTabData.TabID,
							fuWenTabData.Name,
							text,
							text2,
							fuWenTabData.SkillEquip
						});
						flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
					if (!flag)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色插入符文页数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, ownerID), null, true);
						client.sendCmd<int>(nID, -8);
					}
					else
					{
						lock (dbroleInfo.FuWenTabList)
						{
							dbroleInfo.FuWenTabList.Add(fuWenTabData);
						}
						client.sendCmd<int>(nID, flag ? 0 : -8);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		private void UpdateRoleFuWenTab(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				FuWenTabData fuWenTabData = DataHelper.BytesToObject<FuWenTabData>(cmdParams, 0, count);
				int ownerID = fuWenTabData.OwnerID;
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref ownerID);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, ownerID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					bool flag = false;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string text = (fuWenTabData.FuWenEquipList == null) ? "" : string.Join<int>(",", fuWenTabData.FuWenEquipList);
						string text2 = (fuWenTabData.ShenShiActiveList == null) ? "" : string.Join<int>(",", fuWenTabData.ShenShiActiveList);
						string sql = string.Format("REPLACE INTO t_fuwen(rid, tabid, name, fuwenequip, shenshiactive, skillequip) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
						{
							ownerID,
							fuWenTabData.TabID,
							fuWenTabData.Name,
							text,
							text2,
							fuWenTabData.SkillEquip
						});
						flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
					if (!flag)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色插入符文页数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, ownerID), null, true);
						client.sendCmd<int>(nID, -8);
					}
					else
					{
						lock (dbroleInfo.FuWenTabList)
						{
							dbroleInfo.FuWenTabList[fuWenTabData.TabID] = fuWenTabData;
						}
						client.sendCmd<int>(nID, flag ? 0 : -8);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}
	}
}
