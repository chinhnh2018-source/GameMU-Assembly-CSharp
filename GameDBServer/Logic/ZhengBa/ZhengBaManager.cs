using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.ZhengBa
{
	public class ZhengBaManager : SingletonTemplate<ZhengBaManager>, IManager, ICmdProcessor
	{
		private ZhengBaManager()
		{
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14014, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14013, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14015, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14012, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14011, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14016, SingletonTemplate<ZhengBaManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14017, SingletonTemplate<ZhengBaManager>.Instance());
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

		private bool ExecNonQuery(string sql)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		private MySQLDataReader ExecSelect(string sql)
		{
			MySQLConnection mySQLConnection = null;
			MySQLDataReader result;
			try
			{
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				mySQLCommand.Dispose();
				result = mySQLDataReader;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID == 14014)
			{
				this.HandleLoadPkLog(client, nID, cmdParams, count);
			}
			else if (nID == 14013)
			{
				this.HandleLoadSupportLog(client, nID, cmdParams, count);
			}
			else if (nID == 14015)
			{
				this.HandleLoadSupportFlag(client, nID, cmdParams, count);
			}
			else if (nID == 14012)
			{
				this.HandleSavePkLog(client, nID, cmdParams, count);
			}
			else if (nID == 14011)
			{
				this.HandleSaveSupportLog(client, nID, cmdParams, count);
			}
			else if (nID == 14016)
			{
				this.HandleLoadWaitAwardYaZhu(client, nID, cmdParams, count);
			}
			else if (nID == 14017)
			{
				this.HandleSetYaZhuAward(client, nID, cmdParams, count);
			}
		}

		private void HandleSetYaZhuAward(GameServerClient client, int nID, byte[] cmdParams, int count)
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
					int num = Convert.ToInt32(array[0]);
					int num2 = Convert.ToInt32(array[1]);
					int num3 = Convert.ToInt32(array[2]);
					int num4 = Convert.ToInt32(array[3]);
					string sql = string.Format("UPDATE t_zhengba_support_flag SET is_award=1 WHERE month={0} AND from_rid={1} AND to_union_group={2} AND to_group={3} AND support_type={4};", new object[]
					{
						num,
						num2,
						num3,
						num4,
						3
					});
					if (!this.ExecNonQuery(sql))
					{
					}
					client.sendCmd<bool>(nID, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<bool>(nID, false);
			}
		}

		private void HandleLoadWaitAwardYaZhu(GameServerClient client, int nID, byte[] cmdParams, int count)
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
					int num = Convert.ToInt32(array[0]);
					string sql = string.Format("SELECT to_union_group,to_group,rank_of_day,is_award,from_rid FROM t_zhengba_support_flag WHERE support_type={0} AND month={1} AND is_award=0", 3, num);
					MySQLDataReader mySQLDataReader = this.ExecSelect(sql);
					List<ZhengBaWaitYaZhuAwardData> list = new List<ZhengBaWaitYaZhuAwardData>();
					while (mySQLDataReader != null && mySQLDataReader.Read())
					{
						list.Add(new ZhengBaWaitYaZhuAwardData
						{
							FromRoleId = Convert.ToInt32(mySQLDataReader["from_rid"].ToString()),
							Month = num,
							RankOfDay = Convert.ToInt32(mySQLDataReader["rank_of_day"].ToString()),
							UnionGroup = Convert.ToInt32(mySQLDataReader["to_union_group"].ToString()),
							Group = Convert.ToInt32(mySQLDataReader["to_group"].ToString())
						});
					}
					client.sendCmd<List<ZhengBaWaitYaZhuAwardData>>(nID, list);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<List<ZhengBaWaitYaZhuAwardData>>(nID, null);
			}
		}

		private void HandleSaveSupportLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				ZhengBaSupportLogData zhengBaSupportLogData = DataHelper.BytesToObject<ZhengBaSupportLogData>(cmdParams, 0, count);
				if (zhengBaSupportLogData.FromServerId == GameDBManager.ZoneID)
				{
					string sql = string.Format("INSERT INTO t_zhengba_support_flag(month,rank_of_day,from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,time,from_serverid) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9})", new object[]
					{
						zhengBaSupportLogData.Month,
						zhengBaSupportLogData.RankOfDay,
						zhengBaSupportLogData.FromRoleId,
						zhengBaSupportLogData.FromZoneId,
						zhengBaSupportLogData.FromRolename,
						zhengBaSupportLogData.SupportType,
						zhengBaSupportLogData.ToUnionGroup,
						zhengBaSupportLogData.ToGroup,
						zhengBaSupportLogData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
						zhengBaSupportLogData.FromServerId
					});
					if (!this.ExecNonQuery(sql))
					{
						client.sendCmd<bool>(nID, false);
						return;
					}
				}
				string sql2 = string.Format("INSERT INTO t_zhengba_support_log(month,rank_of_day,from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,time,from_serverid) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9})", new object[]
				{
					zhengBaSupportLogData.Month,
					zhengBaSupportLogData.RankOfDay,
					zhengBaSupportLogData.FromRoleId,
					zhengBaSupportLogData.FromZoneId,
					zhengBaSupportLogData.FromRolename,
					zhengBaSupportLogData.SupportType,
					zhengBaSupportLogData.ToUnionGroup,
					zhengBaSupportLogData.ToGroup,
					zhengBaSupportLogData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
					zhengBaSupportLogData.FromServerId
				});
				if (!this.ExecNonQuery(sql2))
				{
				}
				client.sendCmd<bool>(nID, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<bool>(nID, false);
			}
		}

		private void HandleSavePkLog(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				ZhengBaPkLogData zhengBaPkLogData = DataHelper.BytesToObject<ZhengBaPkLogData>(cmdParams, 0, count);
				string sql = string.Format("INSERT INTO t_zhengba_pk_log(month,day,rid1,zoneid1,rname1,ismirror1,rid2,zoneid2,rname2,ismirror2,result,upgrade,starttime,endtime) VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},'{8}',{9},{10},{11},'{12}','{13}')", new object[]
				{
					zhengBaPkLogData.Month,
					zhengBaPkLogData.Day,
					zhengBaPkLogData.RoleID1,
					zhengBaPkLogData.ZoneID1,
					zhengBaPkLogData.RoleName1,
					zhengBaPkLogData.IsMirror1 ? 1 : 0,
					zhengBaPkLogData.RoleID2,
					zhengBaPkLogData.ZoneID2,
					zhengBaPkLogData.RoleName2,
					zhengBaPkLogData.IsMirror2 ? 1 : 0,
					zhengBaPkLogData.PkResult,
					zhengBaPkLogData.UpGrade ? 1 : 0,
					zhengBaPkLogData.StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
					zhengBaPkLogData.EndTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				if (!this.ExecNonQuery(sql))
				{
					client.sendCmd<bool>(nID, false);
				}
				else
				{
					client.sendCmd<bool>(nID, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				client.sendCmd<bool>(nID, false);
			}
		}

		private void HandleLoadSupportFlag(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (array.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("HandleLoadSupportFlag，找不到玩家 roleid={0}", num), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					string sql = string.Format("SELECT support_type,to_union_group,to_group,rank_of_day FROM t_zhengba_support_flag WHERE from_rid={0} AND month={1}", num, num2);
					MySQLDataReader mySQLDataReader = this.ExecSelect(sql);
					List<ZhengBaSupportFlagData> list = new List<ZhengBaSupportFlagData>();
					while (mySQLDataReader != null && mySQLDataReader.Read())
					{
						int toUnionGroup = Convert.ToInt32(mySQLDataReader["to_union_group"].ToString());
						int toGroup = Convert.ToInt32(mySQLDataReader["to_group"].ToString());
						int supportDay = Convert.ToInt32(mySQLDataReader["rank_of_day"].ToString());
						int num3 = Convert.ToInt32(mySQLDataReader["support_type"].ToString());
						ZhengBaSupportFlagData zhengBaSupportFlagData = list.Find((ZhengBaSupportFlagData _f) => _f.UnionGroup == toUnionGroup && _f.Group == toGroup);
						if (zhengBaSupportFlagData == null)
						{
							list.Add(zhengBaSupportFlagData = new ZhengBaSupportFlagData
							{
								UnionGroup = toUnionGroup,
								Group = toGroup,
								SupportDay = supportDay
							});
						}
						if (num3 == 2)
						{
							zhengBaSupportFlagData.IsOppose = true;
						}
						else if (num3 == 1)
						{
							zhengBaSupportFlagData.IsSupport = true;
						}
						else if (num3 == 3)
						{
							zhengBaSupportFlagData.IsYaZhu = true;
						}
					}
					client.sendCmd<List<ZhengBaSupportFlagData>>(nID, list);
				}
			}
		}

		private void HandleLoadSupportLog(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (array.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				List<int> list = new List<int>();
				string sql = string.Format("SELECT DISTINCT(to_union_group) FROM t_zhengba_support_log WHERE `month`={0}", num);
				MySQLDataReader mySQLDataReader = this.ExecSelect(sql);
				while (mySQLDataReader != null && mySQLDataReader.Read())
				{
					int num3 = Convert.ToInt32(mySQLDataReader["to_union_group"].ToString());
					list.Add(num3);
				}
				Dictionary<int, List<ZhengBaSupportLogData>> dictionary = new Dictionary<int, List<ZhengBaSupportLogData>>();
				foreach (int num4 in list)
				{
					sql = string.Format("SELECT from_rid,from_zoneid,from_rolename,support_type,to_union_group,to_group,time,month,rank_of_day,from_serverid FROM t_zhengba_support_log WHERE month={0} AND `to_union_group`={1} ORDER BY `time` DESC limit {2};", num, num4, num2);
					mySQLDataReader = this.ExecSelect(sql);
					while (mySQLDataReader != null && mySQLDataReader.Read())
					{
						int num3 = Convert.ToInt32(mySQLDataReader["to_union_group"].ToString());
						List<ZhengBaSupportLogData> list2 = null;
						if (!dictionary.TryGetValue(num3, out list2))
						{
							list2 = (dictionary[num3] = new List<ZhengBaSupportLogData>());
						}
						ZhengBaSupportLogData zhengBaSupportLogData = new ZhengBaSupportLogData();
						zhengBaSupportLogData.FromRoleId = Convert.ToInt32(mySQLDataReader["from_rid"].ToString());
						zhengBaSupportLogData.FromZoneId = Convert.ToInt32(mySQLDataReader["from_zoneid"].ToString());
						zhengBaSupportLogData.FromRolename = mySQLDataReader["from_rolename"].ToString();
						zhengBaSupportLogData.SupportType = Convert.ToInt32(mySQLDataReader["support_type"].ToString());
						zhengBaSupportLogData.ToUnionGroup = num3;
						zhengBaSupportLogData.ToGroup = Convert.ToInt32(mySQLDataReader["to_group"].ToString());
						zhengBaSupportLogData.Month = Convert.ToInt32(mySQLDataReader["month"].ToString());
						zhengBaSupportLogData.RankOfDay = Convert.ToInt32(mySQLDataReader["rank_of_day"].ToString());
						zhengBaSupportLogData.FromServerId = Convert.ToInt32(mySQLDataReader["from_serverid"].ToString());
						zhengBaSupportLogData.Time = DateTime.Parse(mySQLDataReader["time"].ToString());
						list2.Add(zhengBaSupportLogData);
					}
				}
				foreach (List<ZhengBaSupportLogData> list3 in dictionary.Values)
				{
					list3.Reverse();
				}
				client.sendCmd<Dictionary<int, List<ZhengBaSupportLogData>>>(nID, dictionary);
			}
		}

		private void HandleLoadPkLog(GameServerClient client, int nID, byte[] cmdParams, int count)
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
			if (array.Length != 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string sql = string.Format("SELECT month,day,rid1,zoneid1,rname1,ismirror1,rid2,zoneid2,rname2,ismirror2,result,upgrade,starttime,endtime FROM t_zhengba_pk_log WHERE month={0} ORDER BY endtime DESC LIMIT {1};", num, num2);
				MySQLDataReader mySQLDataReader = this.ExecSelect(sql);
				List<ZhengBaPkLogData> list = new List<ZhengBaPkLogData>();
				while (mySQLDataReader != null && mySQLDataReader.Read())
				{
					list.Add(new ZhengBaPkLogData
					{
						Day = Convert.ToInt32(mySQLDataReader["day"].ToString()),
						Month = Convert.ToInt32(mySQLDataReader["month"].ToString()),
						RoleID1 = Convert.ToInt32(mySQLDataReader["rid1"].ToString()),
						ZoneID1 = Convert.ToInt32(mySQLDataReader["zoneid1"].ToString()),
						RoleName1 = mySQLDataReader["rname1"].ToString(),
						IsMirror1 = (Convert.ToInt32(mySQLDataReader["ismirror1"].ToString()) == 1),
						RoleID2 = Convert.ToInt32(mySQLDataReader["rid2"].ToString()),
						ZoneID2 = Convert.ToInt32(mySQLDataReader["zoneid2"].ToString()),
						RoleName2 = mySQLDataReader["rname2"].ToString(),
						IsMirror2 = (Convert.ToInt32(mySQLDataReader["ismirror2"].ToString()) == 1),
						PkResult = Convert.ToInt32(mySQLDataReader["result"].ToString()),
						UpGrade = (Convert.ToInt32(mySQLDataReader["upgrade"].ToString()) == 1),
						StartTime = DateTime.Parse(mySQLDataReader["starttime"].ToString()),
						EndTime = DateTime.Parse(mySQLDataReader["endtime"].ToString())
					});
				}
				list.Reverse();
				client.sendCmd<List<ZhengBaPkLogData>>(nID, list);
			}
		}

		private enum EZhengBaSupport
		{
			Invalid,
			Support,
			Oppose,
			YaZhu
		}
	}
}
