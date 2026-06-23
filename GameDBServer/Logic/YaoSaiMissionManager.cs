using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class YaoSaiMissionManager : SingletonTemplate<YaoSaiMissionManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20311, SingletonTemplate<YaoSaiMissionManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20312, SingletonTemplate<YaoSaiMissionManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20313, SingletonTemplate<YaoSaiMissionManager>.Instance());
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
			case 20311:
				this.GetYaoSaiMissionData(client, nID, cmdParams, count);
				break;
			case 20312:
				this.SetYaoSaiMissionData(client, nID, cmdParams, count);
				break;
			case 20313:
				this.DelYaoSaiMissionData(client, nID, cmdParams, count);
				break;
			}
		}

		public List<YaoSaiMissionData> GetYaoSaiMissionDataByRid(int rid)
		{
			try
			{
				List<YaoSaiMissionData> list = new List<YaoSaiMissionData>();
				MySQLConnection mySQLConnection = null;
				try
				{
					string text = string.Format("select * from t_yaosaimission where rid='{0}'", rid);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					while (mySQLDataReader.Read())
					{
						int siteID = int.Parse(mySQLDataReader["siteid"].ToString());
						int missionID = int.Parse(mySQLDataReader["missionid"].ToString());
						int state = int.Parse(mySQLDataReader["state"].ToString());
						string zhiPaiJingLing = mySQLDataReader["zhipaijingling"].ToString();
						DateTime startTime = DateTime.Parse(mySQLDataReader["starttime"].ToString());
						list.Add(new YaoSaiMissionData
						{
							SiteID = siteID,
							MissionID = missionID,
							State = state,
							ZhiPaiJingLing = zhiPaiJingLing,
							StartTime = startTime
						});
					}
					mySQLCommand.Dispose();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.Message);
				}
				finally
				{
					if (null != mySQLConnection)
					{
						DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
					}
				}
				return list;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取角色任务信息错误, ex={1}", ex.Message), null, true);
			}
			return null;
		}

		public void GetYaoSaiMissionData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, @string), null, true);
					client.sendCmd(nID, null);
				}
				client.sendCmd<List<YaoSaiMissionData>>(nID, this.GetYaoSaiMissionDataByRid(Convert.ToInt32(array[0])));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 获取角色任务信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public void SetYaoSaiMissionData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, List<YaoSaiMissionData>> dictionary = DataHelper.BytesToObject<Dictionary<int, List<YaoSaiMissionData>>>(cmdParams, 0, count);
				foreach (KeyValuePair<int, List<YaoSaiMissionData>> keyValuePair in dictionary)
				{
					foreach (YaoSaiMissionData yaoSaiMissionData in keyValuePair.Value)
					{
						string text = string.Format("insert into t_yaosaimission values ({0},{1},{2},{3},'{4}','{5}') on duplicate key update missionid={2},state={3},zhipaijingling='{4}',starttime='{5}';\r\n", new object[]
						{
							keyValuePair.Key,
							yaoSaiMissionData.SiteID,
							yaoSaiMissionData.MissionID,
							yaoSaiMissionData.State,
							yaoSaiMissionData.ZhiPaiJingLing,
							yaoSaiMissionData.StartTime
						});
						GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							myDbConnection.ExecuteNonQuery(text, 0);
						}
					}
				}
				client.sendCmd<int>(nID, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 更新角色任务信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public void DelYaoSaiMissionData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, List<YaoSaiMissionData>> dictionary = DataHelper.BytesToObject<Dictionary<int, List<YaoSaiMissionData>>>(cmdParams, 0, count);
				foreach (KeyValuePair<int, List<YaoSaiMissionData>> keyValuePair in dictionary)
				{
					foreach (YaoSaiMissionData yaoSaiMissionData in keyValuePair.Value)
					{
						string text = string.Format("delete from t_yaosaimission where rid={0} and siteid={1}", keyValuePair.Key, yaoSaiMissionData.SiteID);
						GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							myDbConnection.ExecuteNonQuery(text, 0);
						}
					}
				}
				client.sendCmd<int>(nID, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiMission :: 更新角色任务信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public Dictionary<int, List<YaoSaiMissionData>> RoleMissionDict = new Dictionary<int, List<YaoSaiMissionData>>();
	}
}
