using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class ZuoQiManager : SingletonTemplate<ZuoQiManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20319, SingletonTemplate<ZuoQiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20320, SingletonTemplate<ZuoQiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20321, SingletonTemplate<ZuoQiManager>.Instance());
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
			if (nID == 20319)
			{
				this.GetRoleZuoQiData(client, nID, cmdParams, count);
			}
			else if (nID == 20320)
			{
				this.SetRoleZuoQiData(client, nID, cmdParams, count);
			}
			else if (nID == 20321)
			{
				this.CheckRoleZuoQiData(client, nID, cmdParams, count);
			}
		}

		private void GetRoleZuoQiData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				int num = Convert.ToInt32(@string);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					client.sendCmd<List<MountData>>(nID, dbroleInfo.MountList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		private void SetRoleZuoQiData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, @string), null, true);
					client.sendCmd<int>(nID, -4);
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("REPLACE INTO t_zuoqi(rid, goodsid, isnew) VALUES('{0}', '{1}', '{2}')", num, goodsID, num2);
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					MountData mountData = dbroleInfo.MountList.Find((MountData _g) => _g.GoodsID == goodsID);
					if (null == mountData)
					{
						mountData = new MountData
						{
							GoodsID = goodsID,
							IsNew = (1 == num2)
						};
						dbroleInfo.MountList.Add(mountData);
					}
					else
					{
						mountData.IsNew = (1 == num2);
					}
					client.sendCmd<int>(nID, 1);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<int>(nID, -8);
			}
		}

		private void CheckRoleZuoQiData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				int num = Convert.ToInt32(@string);
				DBRoleInfo dbroleInfo = DBManager.getInstance().FindDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("update t_zuoqi set isnew=0 where rid={0}", num);
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					foreach (MountData mountData in dbroleInfo.MountList)
					{
						mountData.IsNew = false;
					}
					client.sendCmd<int>(nID, 1);
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
