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
	public class YaoSaiBossManager : SingletonTemplate<YaoSaiBossManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(20306, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20307, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20308, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20309, SingletonTemplate<YaoSaiBossManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(20310, SingletonTemplate<YaoSaiBossManager>.Instance());
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
			case 20306:
				this.GetYaoSaiBossData(client, nID, cmdParams, count);
				break;
			case 20307:
				this.SetYaoSaiBossData(client, nID, cmdParams, count);
				break;
			case 20308:
				this.GetYaoSaiBossFightLog(client, nID, cmdParams, count);
				break;
			case 20309:
				this.SetYaoSaiBossFightLog(client, nID, cmdParams, count);
				break;
			case 20310:
				this.CleanYaoSaiBossFightData(client, nID, cmdParams, count);
				break;
			}
		}

		public void GetYaoSaiBossData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, YaoSaiBossData> dictionary = new Dictionary<int, YaoSaiBossData>();
				MySQLConnection mySQLConnection = null;
				try
				{
					RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
					string text = "select * from t_yaosaiboss";
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					while (mySQLDataReader.Read())
					{
						int num = int.Parse(mySQLDataReader["rid"].ToString());
						int bossID = int.Parse(mySQLDataReader["bossID"].ToString());
						int num2 = int.Parse(mySQLDataReader["bosslife"].ToString());
						DateTime deadTime = DateTime.Parse(mySQLDataReader["deadtime"].ToString());
						dictionary[num] = new YaoSaiBossData
						{
							OwnerID = num,
							BossID = bossID,
							LifeV = (double)num2,
							DeadTime = deadTime
						};
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
				client.sendCmd<Dictionary<int, YaoSaiBossData>>(nID, dictionary);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public void GetYaoSaiBossFightLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				Dictionary<int, List<YaoSaiBossFightLog>> dictionary = new Dictionary<int, List<YaoSaiBossFightLog>>();
				MySQLConnection mySQLConnection = null;
				try
				{
					RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("20008", null);
					string text = "select * from t_yaosaiboss_fight";
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					while (mySQLDataReader.Read())
					{
						int key = int.Parse(mySQLDataReader["rid"].ToString());
						int otherRid = int.Parse(mySQLDataReader["otherrid"].ToString());
						string otherRname = mySQLDataReader["otherrname"].ToString();
						int inviteType = int.Parse(mySQLDataReader["invitetype"].ToString());
						int fightLife = int.Parse(mySQLDataReader["fightlife"].ToString());
						List<YaoSaiBossFightLog> list = null;
						if (!dictionary.TryGetValue(key, out list))
						{
							list = new List<YaoSaiBossFightLog>();
							dictionary[key] = list;
						}
						list.Add(new YaoSaiBossFightLog
						{
							OtherRid = otherRid,
							OtherRname = otherRname,
							InviteType = inviteType,
							FightLife = fightLife
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
				client.sendCmd<Dictionary<int, List<YaoSaiBossFightLog>>>(nID, dictionary);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public void SetYaoSaiBossData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string text = null;
				try
				{
					text = new UTF8Encoding().GetString(cmdParams, 0, count);
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
					client.sendCmd(30767, "0");
					return;
				}
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					string sql = string.Format("insert into t_yaosaiboss (rid,bossID,bosslife,deadtime) values ({0}, {1}, {2}, '{3}') on duplicate key update bosslife={2}", new object[]
					{
						array[0],
						array[1],
						array[2],
						array[3].Replace('$', ':')
					});
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public void SetYaoSaiBossFightLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string text = null;
				try
				{
					text = new UTF8Encoding().GetString(cmdParams, 0, count);
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
					client.sendCmd(30767, "0");
					return;
				}
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					string sql = string.Format("insert into t_yaosaiboss_fight (rid,otherrid,otherrname,invitetype,fightlife) values ({0}, {1}, '{2}', {3}, {4})", new object[]
					{
						array[0],
						array[1],
						array[2],
						array[3],
						array[4]
					});
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}

		public void CleanYaoSaiBossFightData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string text = null;
				try
				{
					text = new UTF8Encoding().GetString(cmdParams, 0, count);
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
					client.sendCmd(30767, "0");
					return;
				}
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					string sql = string.Format("delete from t_yaosaiboss where rid={0}", array[0]);
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					sql = string.Format("delete from t_yaosaiboss_fight where rid={0}", array[0]);
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						myDbConnection.ExecuteNonQuery(sql, 0);
					}
					client.sendCmd<int>(nID, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 cmd={0}, ex={1}", nID, ex.Message), null, true);
			}
		}
	}
}
