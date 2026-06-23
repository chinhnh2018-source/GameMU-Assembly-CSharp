using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class LogDBCmdManager
	{
		public void AddDBLogInfo(int nGoodDBID, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid, int nSurplus, int serverId, GoodsData goodsData = null)
		{
			if (!("" == strObjName))
			{
				this.AddGameDBLogInfo(nGoodDBID, strObjName, strFrom, strCurrEnvName, strTarEnvName, strOptType, nAmount, nZoneID, userid, nSurplus, serverId);
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-dblog", 0);
				if (gameConfigItemInt <= 0)
				{
					string text = "";
					if (null != goodsData)
					{
						text = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", new object[]
						{
							goodsData.ExcellenceInfo,
							goodsData.Forge_level,
							goodsData.AppendPropLev,
							goodsData.Binding,
							goodsData.Lucky,
							Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.WashProps)),
							Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.ElementhrtsProps)),
							goodsData.JuHunID
						});
					}
					strFrom = strFrom.Replace(':', '-');
					string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
					{
						nGoodDBID,
						strObjName,
						strFrom,
						strCurrEnvName,
						strTarEnvName,
						strOptType,
						nAmount,
						nZoneID,
						nSurplus,
						text
					});
					this.AddDBCmd(20000, cmdText, null, serverId);
				}
			}
		}

		public void AddMessageLog(int dbid, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid, int nSurplus, int serverId, string extData)
		{
			strFrom = strFrom.Replace(':', '-');
			string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				dbid,
				strObjName,
				strFrom,
				strCurrEnvName,
				strTarEnvName,
				strOptType,
				nAmount,
				nZoneID,
				nSurplus,
				extData
			});
			this.AddDBCmd(20000, cmdText, null, serverId);
		}

		public void AddGameDBLogInfo(int nGoodDBID, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid, int nSurplus, int serverId)
		{
			if (!("钻石" != strObjName))
			{
				strFrom = strFrom.Replace(':', '-');
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
				{
					nGoodDBID,
					strObjName,
					strFrom,
					strCurrEnvName,
					strTarEnvName,
					strOptType,
					nAmount,
					nZoneID,
					userid,
					nSurplus
				});
				Global.ExecuteDBCmd(20000, strcmd, serverId);
			}
		}

		public void AddTradeNumberInfo(int type, int money, int roleid1, int roleid2, int serverId = 0)
		{
			string str = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				type,
				money,
				DataHelper.ConvertToTicks(str),
				GameManager.ServerLineID,
				this.CombClientInfo(roleid1, serverId),
				this.CombClientInfo(roleid2, serverId)
			});
			this.AddDBCmd(20002, cmdText, null, serverId);
		}

		public void AddTradeFreqInfo(int type, int count, int roleid, int serverId = 0)
		{
			string str = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				type,
				count,
				DataHelper.ConvertToTicks(str),
				GameManager.ServerLineID,
				this.CombClientInfo(roleid, serverId)
			});
			this.AddDBCmd(20001, cmdText, null, serverId);
		}

		public string CombClientInfo(int roleid, int serverId)
		{
			string strcmd = string.Format("{0}", roleid);
			string[] array = Global.ExecuteDBCmd(10179, strcmd, serverId);
			string result;
			if (array == null || array.Length != 9)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
				if (null == gameClient)
				{
					result = "-1:-1:-1:-1:-1:-1:-1:-1:-1:-1";
				}
				else
				{
					int num = gameClient.ClientData.TotalOnlineSecs;
					result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
					{
						gameClient.strUserID,
						roleid,
						gameClient.ClientData.RoleName,
						-1,
						-1,
						gameClient.ClientData.UserMoney,
						num,
						gameClient.ClientData.ChangeLifeCount * 100 + gameClient.ClientData.Level,
						gameClient.ClientData.RegTime,
						Global.GetSocketRemoteIP(gameClient, false)
					});
				}
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
				int num = (gameClient != null) ? gameClient.ClientData.TotalOnlineSecs : Convert.ToInt32(array[6]);
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
				{
					array[0],
					array[1],
					array[2],
					array[3],
					array[4],
					array[5],
					num,
					array[7],
					array[8],
					(gameClient != null) ? Global.GetSocketRemoteIP(gameClient, false) : ""
				});
			}
			return result;
		}

		private void AddDBCmd(int cmdID, string cmdText, DBCommandEventHandler dbCommandEvent, int serverId)
		{
			DBCommand dbcommand = this._DBCmdPool.Pop();
			if (null == dbcommand)
			{
				dbcommand = new DBCommand();
			}
			dbcommand.DBCommandID = cmdID;
			dbcommand.DBCommandText = cmdText;
			dbcommand.ServerId = serverId;
			if (null != dbCommandEvent)
			{
				dbcommand.DBCommandEvent += dbCommandEvent;
			}
			lock (this._DBCmdQueue)
			{
				this._DBCmdQueue.Enqueue(dbcommand);
			}
		}

		public int GetDBCmdCount()
		{
			int count;
			lock (this._DBCmdQueue)
			{
				count = this._DBCmdQueue.Count;
			}
			return count;
		}

		private TCPProcessCmdResults DoDBCmd(TCPClientPool tcpClientPool, TCPOutPacketPool pool, DBCommand dbCmd, out byte[] bytesData)
		{
			bytesData = Global.SendAndRecvData<string>(dbCmd.DBCommandID, dbCmd.DBCommandText, dbCmd.ServerId, 1);
			TCPProcessCmdResults result;
			if (bytesData == null || bytesData.Length <= 0)
			{
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				result = TCPProcessCmdResults.RESULT_OK;
			}
			return result;
		}

		public void ExecuteDBCmd(TCPClientPool tcpClientPool, TCPOutPacketPool pool)
		{
			lock (this._DBCmdQueue)
			{
				if (this._DBCmdQueue.Count <= 0)
				{
					return;
				}
			}
			List<DBCommand> list = new List<DBCommand>();
			lock (this._DBCmdQueue)
			{
				while (this._DBCmdQueue.Count > 0)
				{
					list.Add(this._DBCmdQueue.Dequeue());
				}
			}
			byte[] array = null;
			for (int i = 0; i < list.Count; i++)
			{
				TCPProcessCmdResults tcpprocessCmdResults = this.DoDBCmd(tcpClientPool, pool, list[i], out array);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(2, string.Format("向LogDBServer请求执行命令失败, CMD={0}", (TCPGameServerCmds)list[i].DBCommandID), null, true);
				}
				this._DBCmdPool.Push(list[i]);
			}
		}

		private DBCmdPool _DBCmdPool = new DBCmdPool(2000);

		private Queue<DBCommand> _DBCmdQueue = new Queue<DBCommand>(2000);
	}
}
