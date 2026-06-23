using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.CoupleArena
{
	internal class CoupleArenaDbManager : SingletonTemplate<CoupleArenaDbManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1382, SingletonTemplate<CoupleArenaDbManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1371, SingletonTemplate<CoupleArenaDbManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1383, SingletonTemplate<CoupleArenaDbManager>.Instance());
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
			if (nID == 1382)
			{
				this.HandleSaveZhanBao(client, nID, cmdParams, count);
			}
			else if (nID == 1371)
			{
				this.HandleGetZhanBao(client, nID, cmdParams, count);
			}
			else if (nID == 1383)
			{
				this.HandleClrZhanBao(client, nID, cmdParams, count);
			}
		}

		private long GetUnionCouple(int a1, int a2)
		{
			int num = Math.Min(a1, a2);
			int num2 = Math.Max(a1, a2);
			long num3 = (long)num;
			num3 <<= 32;
			return num3 | (long)((ulong)num2);
		}

		private Queue<CoupleArenaZhanBaoItemData> GetZhanBao(long unionCouple)
		{
			Queue<CoupleArenaZhanBaoItemData> result;
			lock (this.Mutex)
			{
				Queue<CoupleArenaZhanBaoItemData> queue = null;
				if (!this.CoupleZhanBaoDict.TryGetValue(unionCouple, out queue))
				{
					queue = new Queue<CoupleArenaZhanBaoItemData>();
					MySQLConnection mySQLConnection = null;
					try
					{
						string text = string.Format("SELECT `to_man_zoneid`,`to_man_rname`,`to_wife_zoneid`,`to_wife_rname`,`is_win`,`get_jifen` FROM t_couple_arena_zhan_bao WHERE `union_couple`={0} ORDER BY `time` DESC LIMIT {1};", unionCouple, this.MaxZhanBaoNum);
						GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
						mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
						MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
						MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
						List<CoupleArenaZhanBaoItemData> list = new List<CoupleArenaZhanBaoItemData>();
						while (mySQLDataReader != null && mySQLDataReader.Read())
						{
							list.Add(new CoupleArenaZhanBaoItemData
							{
								TargetManZoneId = Convert.ToInt32(mySQLDataReader["to_man_zoneid"].ToString()),
								TargetManRoleName = mySQLDataReader["to_man_rname"].ToString(),
								TargetWifeZoneId = Convert.ToInt32(mySQLDataReader["to_wife_zoneid"].ToString()),
								TargetWifeRoleName = mySQLDataReader["to_wife_rname"].ToString(),
								IsWin = (Convert.ToInt32(mySQLDataReader["is_win"].ToString()) > 0),
								GetJiFen = Convert.ToInt32(mySQLDataReader["get_jifen"].ToString())
							});
						}
						list.Reverse();
						foreach (CoupleArenaZhanBaoItemData item in list)
						{
							queue.Enqueue(item);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Error, "CoupleArenaDbManager.GetZhanBao " + ex.Message, null, true);
					}
					finally
					{
						if (mySQLConnection != null)
						{
							DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
						}
					}
					this.CoupleZhanBaoDict[unionCouple] = queue;
				}
				result = queue;
			}
			return result;
		}

		private void HandleGetZhanBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, @string), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					int a = Convert.ToInt32(array[0]);
					int a2 = Convert.ToInt32(array[1]);
					long unionCouple = this.GetUnionCouple(a, a2);
					List<CoupleArenaZhanBaoItemData> list = this.GetZhanBao(unionCouple).ToList<CoupleArenaZhanBaoItemData>();
					list.Reverse();
					client.sendCmd<List<CoupleArenaZhanBaoItemData>>(nID, list);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "HandleGetZhanBao failed, " + ex.Message, null, true);
				client.sendCmd(30767, "0");
			}
		}

		private void HandleClrZhanBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, @string), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					int a = Convert.ToInt32(array[0]);
					int a2 = Convert.ToInt32(array[1]);
					long unionCouple = this.GetUnionCouple(a, a2);
					string text = string.Format("DELETE FROM t_couple_arena_zhan_bao WHERE `union_couple`={0};", unionCouple);
					mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.ExecuteNonQuery();
					mySQLCommand.Dispose();
					lock (this.Mutex)
					{
						this.CoupleZhanBaoDict.Remove(unionCouple);
					}
					client.sendCmd<bool>(nID, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "HandleClrZhanBao failed, " + ex.Message, null, true);
				client.sendCmd<bool>(nID, false);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		private void HandleSaveZhanBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				CoupleArenaZhanBaoSaveDbData coupleArenaZhanBaoSaveDbData = DataHelper.BytesToObject<CoupleArenaZhanBaoSaveDbData>(cmdParams, 0, count);
				lock (this.Mutex)
				{
					long unionCouple = this.GetUnionCouple(coupleArenaZhanBaoSaveDbData.FromMan, coupleArenaZhanBaoSaveDbData.FromWife);
					Queue<CoupleArenaZhanBaoItemData> zhanBao = this.GetZhanBao(unionCouple);
					string text = string.Format("INSERT INTO t_couple_arena_zhan_bao(`union_couple`,`man_rid`,`wife_rid`,`to_man_rid`,`to_man_zoneid`,`to_man_rname`,`to_wife_rid`,`to_wife_zoneid`,`to_wife_rname`,`is_win`,`get_jifen`,`week`,`time`) VALUES({0},{1},{2},{3},{4},'{5}',{6},{7},'{8}',{9},{10},{11},'{12}');", new object[]
					{
						unionCouple,
						coupleArenaZhanBaoSaveDbData.FromMan,
						coupleArenaZhanBaoSaveDbData.FromWife,
						coupleArenaZhanBaoSaveDbData.ToMan,
						coupleArenaZhanBaoSaveDbData.ZhanBao.TargetManZoneId,
						coupleArenaZhanBaoSaveDbData.ZhanBao.TargetManRoleName,
						coupleArenaZhanBaoSaveDbData.ToWife,
						coupleArenaZhanBaoSaveDbData.ZhanBao.TargetWifeZoneId,
						coupleArenaZhanBaoSaveDbData.ZhanBao.TargetWifeRoleName,
						coupleArenaZhanBaoSaveDbData.ZhanBao.IsWin ? 1 : 0,
						coupleArenaZhanBaoSaveDbData.ZhanBao.GetJiFen,
						coupleArenaZhanBaoSaveDbData.FirstWeekday,
						TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
					});
					mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.ExecuteNonQuery();
					mySQLCommand.Dispose();
					zhanBao.Enqueue(coupleArenaZhanBaoSaveDbData.ZhanBao);
					while (zhanBao.Count > this.MaxZhanBaoNum)
					{
						zhanBao.Dequeue();
					}
					cmdData = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "CoupleArenaDbManager.HandleSaveZhanBao " + ex.Message, null, true);
				cmdData = false;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		private object Mutex = new object();

		private readonly int MaxZhanBaoNum = 50;

		private Dictionary<long, Queue<CoupleArenaZhanBaoItemData>> CoupleZhanBaoDict = new Dictionary<long, Queue<CoupleArenaZhanBaoItemData>>();
	}
}
