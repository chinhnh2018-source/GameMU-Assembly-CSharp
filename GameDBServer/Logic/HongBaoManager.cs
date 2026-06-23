using System;
using System.Collections.Generic;
using System.Threading;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameDBServer.Logic
{
	public class HongBaoManager : SingletonTemplate<HongBaoManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1430, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1431, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1432, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1433, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1434, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1435, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1436, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1437, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1428, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1438, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1439, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1440, SingletonTemplate<HongBaoManager>.Instance());
			if (null == this.WorkThread)
			{
				this.WorkThread = new Thread(new ThreadStart(this.ThreadStart));
				this.WorkThread.IsBackground = true;
				this.WorkThread.Start();
			}
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

		private void ThreadStart()
		{
			for (;;)
			{
				Thread.Sleep(2000);
			}
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 1428:
				this.GetJieRiHongBaoBangAwards(client, nID, cmdParams, count);
				break;
			case 1430:
				this.GetZhanMengHongBaoRankList(client, nID, cmdParams, count);
				break;
			case 1431:
				this.GetJieRiHongBaoRankList(client, nID, cmdParams, count);
				break;
			case 1432:
				this.UpdateZhanMengHongBao(client, nID, cmdParams, count);
				break;
			case 1433:
				this.RecvZhanMengHongBao(client, nID, cmdParams, count);
				break;
			case 1434:
				this.GetZhanMengHongBaoList(client, nID, cmdParams, count);
				break;
			case 1435:
				this.UpdateJieRiHongBao(client, nID, cmdParams, count);
				break;
			case 1436:
				this.RecvJieRiHongBao(client, nID, cmdParams, count);
				break;
			case 1437:
				this.GetJieRiHongBaoList(client, nID, cmdParams, count);
				break;
			case 1438:
				this.GetZhanMengHongBaoLogList(client, nID, cmdParams, count);
				break;
			case 1439:
				this.GetZhanMengHongBaoRecvList(client, nID, cmdParams, count);
				break;
			case 1440:
				this.GetJieRiHongBaoCount(client, nID, cmdParams, count);
				break;
			}
		}

		private void GetZhanMengHongBaoRankList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<HongBaoRankItemData> list = new List<HongBaoRankItemData>();
			try
			{
				ZhanMengHongBaoRankListQueryData zhanMengHongBaoRankListQueryData = DataHelper.BytesToObject<ZhanMengHongBaoRankListQueryData>(cmdParams, 0, count);
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql;
					if (zhanMengHongBaoRankListQueryData.Type == 1)
					{
						sql = string.Format("SELECT `senderid`,`sendername`,MAX(`sendtime`),SUM(`zuanshi`) FROM `t_hongbao_send` WHERE `bhid`={0} and sendtime>='{1}' GROUP BY `senderid` ORDER BY SUM(`zuanshi`) DESC,MAX(`sendtime`) ASC limit {2};", zhanMengHongBaoRankListQueryData.Bhid, zhanMengHongBaoRankListQueryData.StartTime.ToString("yyyy-MM-dd"), zhanMengHongBaoRankListQueryData.Count);
					}
					else
					{
						sql = string.Format("SELECT `rid`,`rname`,MAX(`recvtime`),SUM(`zuanshi`) FROM `t_hongbao_recv` WHERE `bhid`={0} AND recvtime>='{1}' GROUP BY `rid` ORDER BY SUM(`zuanshi`) DESC,MAX(`recvtime`) ASC limit {2};", zhanMengHongBaoRankListQueryData.Bhid, zhanMengHongBaoRankListQueryData.StartTime.ToString("yyyy-MM-dd"), zhanMengHongBaoRankListQueryData.Count);
					}
					int num = 1;
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						HongBaoRankItemData hongBaoRankItemData = new HongBaoRankItemData();
						hongBaoRankItemData.roleId = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10);
						if (hongBaoRankItemData.roleId > 0)
						{
							hongBaoRankItemData.roleName = mySQLDataReader[1].ToString();
							hongBaoRankItemData.daimondNum = Global.SafeConvertToInt32(mySQLDataReader[3].ToString(), 10);
							hongBaoRankItemData.rankID = num++;
							list.Add(hongBaoRankItemData);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<List<HongBaoRankItemData>>(nID, list);
		}

		private void GetZhanMengHongBaoLogList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<HongBaoItemData> list = new List<HongBaoItemData>();
			try
			{
				List<string> list2 = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string text;
					if (list2[0] == "0")
					{
						text = string.Format("SELECT hongbaoid FROM t_hongbao_recv WHERE bhid={0} AND rid={1} order by hongbaoid desc limit {2}", list2[1], list2[2], list2[3]);
						List<int> list3 = new List<int>();
						MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(text, new MySQLParameter[0]);
						while (mySQLDataReader.Read())
						{
							HongBaoItemData hongBaoItemData = new HongBaoItemData();
							list3.Add(Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10));
						}
						mySQLDataReader.Close();
						if (list3.Count > 0)
						{
							text = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE id IN ({0}) order by id desc;", string.Join<int>(",", list3));
						}
						else
						{
							text = "";
						}
					}
					else if (list2[0] == "1")
					{
						text = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE bhid={0} AND senderid={1} order by id desc limit {2};", list2[1], list2[2], list2[3]);
					}
					else
					{
						text = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE bhid={0} order by id desc limit {2};", list2[1], list2[2], list2[3]);
					}
					if (!string.IsNullOrEmpty(text))
					{
						MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(text, new MySQLParameter[0]);
						while (mySQLDataReader.Read())
						{
							list.Add(new HongBaoItemData
							{
								hongBaoID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10),
								sender = mySQLDataReader[3].ToString(),
								beginTime = Global.SafeConvertToDateTime(mySQLDataReader[4].ToString(), DateTime.MinValue),
								endTime = Global.SafeConvertToDateTime(mySQLDataReader[5].ToString(), DateTime.MinValue),
								diamondSumCount = Global.SafeConvertToInt32(mySQLDataReader[7].ToString(), 10),
								type = Global.SafeConvertToInt32(mySQLDataReader[9].ToString(), 10),
								hongBaoStatus = Global.SafeConvertToInt32(mySQLDataReader[12].ToString(), 10)
							});
						}
						mySQLDataReader.Close();
						foreach (HongBaoItemData hongBaoItemData in list)
						{
							text = string.Format("SELECT `zuanshi` FROM t_hongbao_recv WHERE hongbaoid={0} and rid={1};", hongBaoItemData.hongBaoID, list2[2]);
							MySQLDataReader mySQLDataReader2;
							mySQLDataReader = (mySQLDataReader2 = myDbConnection.ExecuteReader(text, new MySQLParameter[0]));
							try
							{
								if (mySQLDataReader.Read())
								{
									hongBaoItemData.diamondCount = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10);
								}
							}
							finally
							{
								if (mySQLDataReader2 != null)
								{
									mySQLDataReader2.Dispose();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<List<HongBaoItemData>>(nID, list);
		}

		private void GetZhanMengHongBaoRecvList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			HongBaoSendData hongBaoSendData = new HongBaoSendData();
			try
			{
				int num = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE id={0};", num);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						hongBaoSendData.hongBaoID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10);
						hongBaoSendData.bhid = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10);
						hongBaoSendData.senderID = Global.SafeConvertToInt32(mySQLDataReader[2].ToString(), 10);
						hongBaoSendData.sender = mySQLDataReader[3].ToString();
						hongBaoSendData.sendTime = Global.SafeConvertToDateTime(mySQLDataReader[4].ToString(), DateTime.MinValue);
						hongBaoSendData.endTime = Global.SafeConvertToDateTime(mySQLDataReader[5].ToString(), DateTime.MinValue);
						hongBaoSendData.message = mySQLDataReader[6].ToString();
						hongBaoSendData.sumDiamondNum = Global.SafeConvertToInt32(mySQLDataReader[7].ToString(), 10);
						hongBaoSendData.sumCount = Global.SafeConvertToInt32(mySQLDataReader[8].ToString(), 10);
						hongBaoSendData.type = Global.SafeConvertToInt32(mySQLDataReader[9].ToString(), 10);
						hongBaoSendData.leftZuanShi = Global.SafeConvertToInt32(mySQLDataReader[10].ToString(), 10);
						hongBaoSendData.leftCount = Global.SafeConvertToInt32(mySQLDataReader[11].ToString(), 10);
						hongBaoSendData.hongBaoStatus = Global.SafeConvertToInt32(mySQLDataReader[12].ToString(), 10);
					}
					mySQLDataReader.Close();
					hongBaoSendData.RecvList = new List<HongBaoRecvData>();
					sql = string.Format("SELECT `bhid`,`rid`,`rname`,`recvtime`,`zuanshi` FROM t_hongbao_recv WHERE hongbaoid={0};", num);
					MySQLDataReader mySQLDataReader2;
					mySQLDataReader = (mySQLDataReader2 = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]));
					try
					{
						while (mySQLDataReader.Read())
						{
							HongBaoRecvData hongBaoRecvData = new HongBaoRecvData();
							hongBaoRecvData.HongBaoID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10);
							hongBaoRecvData.RoleId = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10);
							hongBaoRecvData.RoleName = mySQLDataReader[2].ToString();
							hongBaoRecvData.RecvTime = Global.SafeConvertToDateTime(mySQLDataReader[3].ToString(), DateTime.MinValue);
							hongBaoRecvData.ZuanShi = Global.SafeConvertToInt32(mySQLDataReader[4].ToString(), 10);
							hongBaoSendData.RecvList.Add(hongBaoRecvData);
						}
					}
					finally
					{
						if (mySQLDataReader2 != null)
						{
							mySQLDataReader2.Dispose();
						}
					}
					mySQLDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<HongBaoSendData>(nID, hongBaoSendData);
		}

		private void GetJieRiHongBaoRankList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<JieriHongBaoKingItemData> list = new List<JieriHongBaoKingItemData>();
			try
			{
				List<string> list2 = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT `rid`,`count`,`getawardtimes`,`lasttime`,`rname` FROM `t_hongbao_jieri_recv` WHERE `keystr`='{0}' ORDER BY `count` DESC,`lasttime` ASC,rid ASC limit {1};", list2[0], list2[1]);
					using (MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]))
					{
						int num = 1;
						while (mySQLDataReader.Read())
						{
							list.Add(new JieriHongBaoKingItemData
							{
								RoleID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10),
								TotalRecv = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10),
								GetAwardTimes = Global.SafeConvertToInt32(mySQLDataReader[2].ToString(), 10),
								Rolename = mySQLDataReader[4].ToString(),
								Rank = num++
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<List<JieriHongBaoKingItemData>>(nID, list);
		}

		private void UpdateJieRiHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = 0;
			HongBaoSendData hongBaoSendData = DataHelper.BytesToObject<HongBaoSendData>(cmdParams, 0, count);
			if (hongBaoSendData != null)
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql;
					if (hongBaoSendData.hongBaoID == 0)
					{
						sql = string.Format("insert into `t_hongbao_jieri_send` (`keystr`, `senderid`, `sendtime`, `endtime`, `msg`, `zuanshi`, `type`, `leftzuanshi`, `state`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", new object[]
						{
							hongBaoSendData.sender,
							hongBaoSendData.senderID,
							hongBaoSendData.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.message,
							hongBaoSendData.sumDiamondNum,
							hongBaoSendData.type,
							hongBaoSendData.leftZuanShi,
							0
						});
					}
					else
					{
						sql = string.Format("replace into `t_hongbao_jieri_send` (`id`,`keystr`, `senderid`, `sendtime`, `endtime`, `msg`, `zuanshi`, `type`, `leftzuanshi`, `state`) values({9},'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", new object[]
						{
							hongBaoSendData.sender,
							hongBaoSendData.senderID,
							hongBaoSendData.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.message,
							hongBaoSendData.sumDiamondNum,
							hongBaoSendData.type,
							hongBaoSendData.leftZuanShi,
							hongBaoSendData.hongBaoStatus,
							hongBaoSendData.hongBaoID
						});
					}
					num = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					if (num >= 0 && hongBaoSendData.hongBaoID == 0)
					{
						sql = "SELECT LAST_INSERT_ID();";
						num = myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]);
					}
				}
			}
			client.sendCmd<int>(nID, num);
		}

		private void UpdateZhanMengHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = 0;
			HongBaoSendData hongBaoSendData = DataHelper.BytesToObject<HongBaoSendData>(cmdParams, 0, count);
			if (hongBaoSendData != null)
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql;
					if (hongBaoSendData.hongBaoID == 0)
					{
						sql = string.Format("insert into `t_hongbao_send` (`bhid`, `senderid`, `sendername`, `sendtime`, `endtime`, `msg`, `zuanshi`, `count`, `type`, `leftzuanshi`, `leftcount`, `state`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','0');", new object[]
						{
							hongBaoSendData.bhid,
							hongBaoSendData.senderID,
							hongBaoSendData.sender,
							hongBaoSendData.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.message,
							hongBaoSendData.sumDiamondNum,
							hongBaoSendData.sumCount,
							hongBaoSendData.type,
							hongBaoSendData.leftZuanShi,
							hongBaoSendData.leftCount
						});
					}
					else
					{
						sql = string.Format("replace into `t_hongbao_send` (`bhid`, `senderid`, `sendername`, `sendtime`, `endtime`, `msg`, `zuanshi`, `count`, `type`, `leftzuanshi`, `leftcount`, `state`,`id`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12});", new object[]
						{
							hongBaoSendData.bhid,
							hongBaoSendData.senderID,
							hongBaoSendData.sender,
							hongBaoSendData.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							hongBaoSendData.message,
							hongBaoSendData.sumDiamondNum,
							hongBaoSendData.sumCount,
							hongBaoSendData.type,
							hongBaoSendData.leftZuanShi,
							hongBaoSendData.leftCount,
							hongBaoSendData.hongBaoStatus,
							hongBaoSendData.hongBaoID
						});
					}
					num = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					if (num >= 0 && hongBaoSendData.hongBaoID == 0)
					{
						sql = "SELECT LAST_INSERT_ID();";
						num = myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]);
					}
				}
			}
			client.sendCmd<int>(nID, num);
		}

		private void RecvJieRiHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			JieriHongBaoKingItemData cmdData = null;
			try
			{
				List<string> list = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				if (list != null)
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("INSERT INTO `t_hongbao_jieri_recv` (`keystr`, `rid`, `count`, `getawardtimes`, `lasttime`, `rname`, `zuanshi`) VALUES('{0}',{1},{2},{3},'{4}','{5}','{6}') ON DUPLICATE KEY UPDATE `count`=`count`+{2},rname='{5}',`zuanshi`=`zuanshi`+{6},`lasttime`='{4}';", new object[]
						{
							list[0],
							list[1],
							list[2],
							0,
							list[3],
							list[4],
							list[5]
						});
						if (string.IsNullOrEmpty(list[3]) || myDbConnection.ExecuteSql(sql, new MySQLParameter[0]) >= 0)
						{
							sql = string.Format("SELECT `rid`,`count`,`getawardtimes`,`lasttime`,`rname` FROM `t_hongbao_jieri_recv` WHERE `keystr`='{0}' AND `rid`={1};", list[0], list[1]);
							using (MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]))
							{
								int num = 1;
								if (mySQLDataReader.Read())
								{
									cmdData = new JieriHongBaoKingItemData
									{
										RoleID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10),
										TotalRecv = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10),
										GetAwardTimes = Global.SafeConvertToInt32(mySQLDataReader[2].ToString(), 10),
										Rolename = mySQLDataReader[4].ToString(),
										Rank = num++
									};
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<JieriHongBaoKingItemData>(nID, cmdData);
		}

		private void GetJieRiHongBaoCount(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int cmdData = 0;
			try
			{
				List<string> list = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				if (list != null)
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("SELECT `count` FROM `t_hongbao_jieri_recv` WHERE `keystr`='{0}' AND `rid`={1};", list[0], list[1]);
						cmdData = myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, cmdData);
		}

		private void GetJieRiHongBaoBangAwards(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int cmdData = 0;
			try
			{
				List<string> list = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				if (list != null)
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("update t_hongbao_jieri_recv set getawardtimes={2} where `keystr`='{0}' and `rid`={1} and getawardtimes='0'", list[0], list[1], list[2]);
						cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				cmdData = -1;
			}
			client.sendCmd<int>(nID, cmdData);
		}

		private void RecvZhanMengHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int cmdData = 0;
			try
			{
				HongBaoRecvData hongBaoRecvData = DataHelper.BytesToObject<HongBaoRecvData>(cmdParams, 0, count);
				if (hongBaoRecvData != null)
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("replace into `t_hongbao_recv` (`hongbaoid`, `rid`, `bhid`, `zuanshi`, `recvtime`, `rname`) values('{0}','{1}','{2}','{3}','{4}','{5}');", new object[]
						{
							hongBaoRecvData.HongBaoID,
							hongBaoRecvData.RoleId,
							hongBaoRecvData.BhId,
							hongBaoRecvData.ZuanShi,
							hongBaoRecvData.RecvTime,
							hongBaoRecvData.RoleName
						});
						cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, cmdData);
		}

		private void GetZhanMengHongBaoList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			HongBaoListQueryData hongBaoListQueryData = null;
			try
			{
				List<HongBaoSendData> list = new List<HongBaoSendData>();
				hongBaoListQueryData = DataHelper.BytesToObject<HongBaoListQueryData>(cmdParams, 0, count);
				if (null == hongBaoListQueryData)
				{
					hongBaoListQueryData = new HongBaoListQueryData();
				}
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql;
					if (hongBaoListQueryData.BhId != 0)
					{
						sql = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE `state`=0 AND `bhid`={0};", hongBaoListQueryData.BhId);
					}
					else
					{
						sql = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE `state`=0;", new object[0]);
					}
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						list.Add(new HongBaoSendData
						{
							hongBaoID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10),
							bhid = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10),
							senderID = Global.SafeConvertToInt32(mySQLDataReader[2].ToString(), 10),
							sender = mySQLDataReader[3].ToString(),
							sendTime = Global.SafeConvertToDateTime(mySQLDataReader[4].ToString(), DateTime.MinValue),
							endTime = Global.SafeConvertToDateTime(mySQLDataReader[5].ToString(), DateTime.MinValue),
							message = mySQLDataReader[6].ToString(),
							sumDiamondNum = Global.SafeConvertToInt32(mySQLDataReader[7].ToString(), 10),
							sumCount = Global.SafeConvertToInt32(mySQLDataReader[8].ToString(), 10),
							type = Global.SafeConvertToInt32(mySQLDataReader[9].ToString(), 10),
							leftZuanShi = Global.SafeConvertToInt32(mySQLDataReader[10].ToString(), 10),
							leftCount = Global.SafeConvertToInt32(mySQLDataReader[11].ToString(), 10),
							hongBaoStatus = Global.SafeConvertToInt32(mySQLDataReader[12].ToString(), 10)
						});
					}
					mySQLDataReader.Close();
					foreach (HongBaoSendData hongBaoSendData in list)
					{
						hongBaoSendData.RecvList = new List<HongBaoRecvData>();
						sql = string.Format("SELECT `bhid`,`rid`,`rname`,`recvtime`,`zuanshi` FROM t_hongbao_recv WHERE hongbaoid={0};", hongBaoSendData.hongBaoID);
						MySQLDataReader mySQLDataReader2;
						mySQLDataReader = (mySQLDataReader2 = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]));
						try
						{
							while (mySQLDataReader.Read())
							{
								HongBaoRecvData hongBaoRecvData = new HongBaoRecvData();
								hongBaoRecvData.HongBaoID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10);
								hongBaoRecvData.RoleId = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10);
								hongBaoRecvData.RoleName = mySQLDataReader[2].ToString();
								hongBaoRecvData.RecvTime = Global.SafeConvertToDateTime(mySQLDataReader[3].ToString(), DateTime.MinValue);
								hongBaoRecvData.ZuanShi = Global.SafeConvertToInt32(mySQLDataReader[4].ToString(), 10);
								hongBaoSendData.RecvList.Add(hongBaoRecvData);
							}
						}
						finally
						{
							if (mySQLDataReader2 != null)
							{
								mySQLDataReader2.Dispose();
							}
						}
					}
				}
				hongBaoListQueryData.List = list;
				hongBaoListQueryData.Success = 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<HongBaoListQueryData>(nID, hongBaoListQueryData);
		}

		private void GetJieRiHongBaoList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			HongBaoListQueryData hongBaoListQueryData = null;
			try
			{
				List<HongBaoSendData> list = new List<HongBaoSendData>();
				hongBaoListQueryData = DataHelper.BytesToObject<HongBaoListQueryData>(cmdParams, 0, count);
				if (null != hongBaoListQueryData)
				{
					hongBaoListQueryData.List = list;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("SELECT `id`,`senderid`,`keystr`,`sendtime`,`endtime`,`msg`,`zuanshi`,`type`,`leftzuanshi`,`state` FROM `t_hongbao_jieri_send`  WHERE keystr='{0}';", hongBaoListQueryData.KeyStr);
						MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
						while (mySQLDataReader.Read())
						{
							list.Add(new HongBaoSendData
							{
								hongBaoID = Global.SafeConvertToInt32(mySQLDataReader[0].ToString(), 10),
								senderID = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10),
								sender = mySQLDataReader[2].ToString(),
								sendTime = Global.SafeConvertToDateTime(mySQLDataReader[3].ToString(), DateTime.MinValue),
								endTime = Global.SafeConvertToDateTime(mySQLDataReader[4].ToString(), DateTime.MinValue),
								message = mySQLDataReader[5].ToString(),
								sumDiamondNum = Global.SafeConvertToInt32(mySQLDataReader[6].ToString(), 10),
								type = Global.SafeConvertToInt32(mySQLDataReader[7].ToString(), 10),
								leftZuanShi = Global.SafeConvertToInt32(mySQLDataReader[8].ToString(), 10),
								hongBaoStatus = Global.SafeConvertToInt32(mySQLDataReader[9].ToString(), 10)
							});
						}
					}
					hongBaoListQueryData.Success = 1;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<HongBaoListQueryData>(nID, hongBaoListQueryData);
		}

		private const int ALLY_LOG_COUNT_MAX = 20;

		private const string timefm = "yyyy-MM-dd HH:mm:ss";

		private object Mutex = new object();

		private Thread WorkThread;
	}
}
