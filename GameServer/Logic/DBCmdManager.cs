using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Server;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class DBCmdManager
	{
		public void AddDBCmd(int cmdID, string cmdText, DBCommandEventHandler dbCommandEvent, int serverId)
		{
			Global.ExecuteDBCmd(cmdID, cmdText, serverId);
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
			bytesData = Global.SendAndRecvData<string>(dbCmd.DBCommandID, dbCmd.DBCommandText, dbCmd.ServerId, 0);
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
			string[] fields = null;
			byte[] array = null;
			for (int i = 0; i < list.Count; i++)
			{
				TCPProcessCmdResults tcpprocessCmdResults = this.DoDBCmd(tcpClientPool, pool, list[i], out array);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(2, string.Format("向DBServer请求执行命令失败, CMD={0}", (TCPGameServerCmds)list[i].DBCommandID), null, true);
				}
				else
				{
					int num = BitConverter.ToInt32(array, 0);
					string @string = new UTF8Encoding().GetString(array, 6, num - 2);
					fields = @string.Split(new char[]
					{
						':'
					});
				}
				list[i].DoDBCommandEvent(new DBCommandEventArgs
				{
					Result = tcpprocessCmdResults,
					fields = fields
				});
				this._DBCmdPool.Push(list[i]);
			}
		}

		private DBCmdPool _DBCmdPool = new DBCmdPool(1000);

		private Queue<DBCommand> _DBCmdQueue = new Queue<DBCommand>(1000);
	}
}
