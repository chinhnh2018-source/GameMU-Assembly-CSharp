using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.AoYunDaTi
{
	public class AoYunDaTiManager : SingletonTemplate<AoYunDaTiManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20300, SingletonTemplate<AoYunDaTiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20301, SingletonTemplate<AoYunDaTiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20302, SingletonTemplate<AoYunDaTiManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20303, SingletonTemplate<AoYunDaTiManager>.Instance());
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
			if (nID == 20300)
			{
				this.GetAoyunDaTiRoleList(client, nID, cmdParams, count);
			}
			else if (nID == 20301)
			{
				this.GetAoyunLastRankDic(client, nID, cmdParams, count);
			}
			else if (nID == 20302)
			{
				this.SetAoyunLastRankDic(client, nID, cmdParams, count);
			}
			else if (nID == 20303)
			{
				this.CleanAoyunPoint(client, nID, cmdParams, count);
			}
		}

		private void GetAoyunDaTiRoleList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<AoyunPaiHangRoleData> list = new List<AoyunPaiHangRoleData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
				string text = string.Format("SELECT * from {0} where pname='{1}'", roleParamType.TableName, "20008");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int num = int.Parse(mySQLDataReader["rid"].ToString());
					int num2 = int.Parse(mySQLDataReader["pvalue"].ToString());
					if (num2 > 0)
					{
						text = string.Format("select zoneid,rname from t_roles where rid={0}", num);
						mySQLCommand = new MySQLCommand(text, mySQLConnection);
						MySQLDataReader mySQLDataReader2 = mySQLCommand.ExecuteReaderEx();
						if (mySQLDataReader2.Read())
						{
							int zoneId = Convert.ToInt32(mySQLDataReader2["zoneid"].ToString());
							string roleName = mySQLDataReader2["rname"].ToString();
							AoyunPaiHangRoleData item = new AoyunPaiHangRoleData
							{
								ZoneId = zoneId,
								RoleId = num,
								RoleName = roleName,
								RolePoint = num2,
								RoleLastPoint = 0
							};
							list.Add(item);
						}
					}
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
			client.sendCmd<List<AoyunPaiHangRoleData>>(nID, list);
		}

		private void GetAoyunLastRankDic(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20009", null);
				string text = string.Format("SELECT * from {0} where pname='{1}'", roleParamType.TableName, "20009");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int key = int.Parse(mySQLDataReader["rid"].ToString());
					int num = int.Parse(mySQLDataReader["pvalue"].ToString());
					if (num > 0)
					{
						dictionary[key] = num;
					}
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
			client.sendCmd<Dictionary<int, int>>(nID, dictionary);
		}

		private void SetAoyunLastRankDic(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				Dictionary<int, int> dictionary = DataHelper.BytesToObject<Dictionary<int, int>>(cmdParams, 0, count);
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20009", null);
				string text = string.Format("DELETE from {0} where pname='{1}'", roleParamType.TableName, "20009");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				mySQLCommand.Dispose();
				string text2 = "";
				foreach (KeyValuePair<int, int> keyValuePair in dictionary)
				{
					text2 += string.Format("({0},{1},{2}),", keyValuePair.Key, "20009", keyValuePair.Value);
				}
				if (text2 != "")
				{
					text2 = text2.Substring(0, text2.Length - 1);
					string text3 = string.Format("INSERT INTO {0} VALUES {1}", roleParamType.TableName, text2);
					mySQLCommand = new MySQLCommand(text3, mySQLConnection);
					mySQLCommand.ExecuteReaderEx();
					mySQLCommand.Dispose();
				}
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
			client.sendCmd<int>(nID, 1);
		}

		private void CleanAoyunPoint(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
				string text = string.Format("DELETE from {0} where pname='{1}'", roleParamType.TableName, "20008");
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
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
			client.sendCmd<int>(nID, 1);
		}
	}
}
