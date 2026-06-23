using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.UnionAlly
{
	public class AllyManager : SingletonTemplate<AllyManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13122, SingletonTemplate<AllyManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13123, SingletonTemplate<AllyManager>.Instance());
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
			case 13122:
				this.GetAllyLogData(client, nID, cmdParams, count);
				break;
			case 13123:
				this.AllyLogAdd(client, nID, cmdParams, count);
				break;
			}
		}

		private void GetAllyLogData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			lock (this._lock)
			{
				int num = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				if (this._allyLogDic.ContainsKey(num))
				{
					client.sendCmd<List<AllyLogData>>(nID, this._allyLogDic[num]);
				}
				else
				{
					List<AllyLogData> allyLogData = this.GetAllyLogData(num);
					this._allyLogDic.Add(num, allyLogData);
					client.sendCmd<List<AllyLogData>>(nID, allyLogData);
				}
			}
		}

		private List<AllyLogData> GetAllyLogData(int unionID)
		{
			List<AllyLogData> result;
			lock (this._lock)
			{
				List<AllyLogData> list = new List<AllyLogData>();
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT unionID,unionZoneID,unionName,logTime,logState FROM t_ally_log WHERE myUnionID={0} ORDER BY logTime DESC LIMIT 20", unionID);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						list.Add(new AllyLogData
						{
							MyUnionID = unionID,
							UnionID = int.Parse(mySQLDataReader["unionID"].ToString()),
							UnionZoneID = int.Parse(mySQLDataReader["unionZoneID"].ToString()),
							UnionName = mySQLDataReader["unionName"].ToString(),
							LogTime = DateTime.Parse(mySQLDataReader["logTime"].ToString()),
							LogState = int.Parse(mySQLDataReader["logState"].ToString())
						});
					}
				}
				result = list;
			}
			return result;
		}

		private void AllyLogAdd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool flag = false;
			AllyLogData allyLogData = DataHelper.BytesToObject<AllyLogData>(cmdParams, 0, count);
			if (allyLogData == null)
			{
				client.sendCmd<bool>(nID, flag);
			}
			else
			{
				lock (this._lock)
				{
					if (!this._allyLogDic.ContainsKey(allyLogData.MyUnionID))
					{
						List<AllyLogData> list = this.GetAllyLogData(allyLogData.MyUnionID);
						this._allyLogDic.Add(allyLogData.MyUnionID, list);
					}
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("INSERT INTO t_ally_log(myUnionID,unionID,unionZoneID,unionName,logTime,logState) VALUE('{0}','{1}','{2}','{3}','{4}','{5}')", new object[]
						{
							allyLogData.MyUnionID,
							allyLogData.UnionID,
							allyLogData.UnionZoneID,
							allyLogData.UnionName,
							allyLogData.LogTime,
							allyLogData.LogState
						});
						flag = (myDbConnection.ExecuteNonQuery(sql, 0) > 0);
					}
					if (flag && allyLogData != null && allyLogData.MyUnionID > 0)
					{
						if (this._allyLogDic.ContainsKey(allyLogData.MyUnionID))
						{
							List<AllyLogData> list = this._allyLogDic[allyLogData.MyUnionID];
							list.Insert(0, allyLogData);
							if (list.Count > 20)
							{
								list.RemoveRange(20, list.Count - 20);
							}
						}
						else
						{
							List<AllyLogData> list = this.GetAllyLogData(allyLogData.MyUnionID);
							list.Add(allyLogData);
							this._allyLogDic.Add(allyLogData.MyUnionID, list);
						}
					}
					client.sendCmd<bool>(nID, flag);
				}
			}
		}

		private const int ALLY_LOG_COUNT_MAX = 20;

		private object _lock = new object();

		private Dictionary<int, List<AllyLogData>> _allyLogDic = new Dictionary<int, List<AllyLogData>>();
	}
}
