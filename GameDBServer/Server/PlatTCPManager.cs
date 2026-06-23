using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameDBServer.Core;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Share.Server;

namespace GameDBServer.Server
{
	internal class PlatTCPManager
	{
		private PlatTCPManager()
		{
		}

		public static PlatTCPManager getInstance()
		{
			return PlatTCPManager.instance;
		}

		public Dictionary<int, PlatTCPManager.NetCommandFunc> ProcessCmdFuncDict
		{
			get
			{
				return this._ProcessCmdFuncDict;
			}
		}

		public void initialize(int capacity)
		{
			capacity = Math.Max(capacity, 10);
			this.socketListener = new SocketListener(capacity, 32768);
			this.inputServers = new Dictionary<Socket, int>();
			this.dictInPackets = new Dictionary<Socket, TCPInPacket>();
			this.tcpInPacketPool = new TCPInPacketPool(capacity);
			this.tcpOutPacketPool = TCPOutPacketPool.getInstance();
			this.tcpOutPacketPool.initialize(capacity * 5);
			this.socketListener.SocketClosed += this.SocketClosed;
			this.socketListener.SocketConnected += this.SocketConnected;
			this.socketListener.SocketReceived += this.SocketReceived;
			this.socketListener.SocketSended += this.SocketSended;
			this.InitCmds();
		}

		protected bool InitCmds()
		{
			this._ProcessCmdFuncDict = new Dictionary<int, PlatTCPManager.NetCommandFunc>();
			this._ProcessCmdFuncDict.Add(1, new PlatTCPManager.NetCommandFunc(this.ProcessRechargeData));
			this._ProcessCmdFuncDict.Add(2, new PlatTCPManager.NetCommandFunc(this.ProcessPlatGift));
			return true;
		}

		public SocketListener MySocketListener
		{
			get
			{
				return this.socketListener;
			}
		}

		public void Start(string ip, int port)
		{
			this.socketListener.Init();
			this.socketListener.Start(ip, port);
		}

		public void Stop()
		{
			this.socketListener.Stop();
		}

		private void SocketConnected(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			Socket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.inputServers)
			{
				int num = 0;
				if (!this.inputServers.TryGetValue(currentSocket, out num))
				{
					this.inputServers.Add(currentSocket, 1);
				}
			}
		}

		private void SocketClosed(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			Socket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (this.dictInPackets.ContainsKey(currentSocket))
				{
					TCPInPacket item = this.dictInPackets[currentSocket];
					this.tcpInPacketPool.Push(item);
					this.dictInPackets.Remove(currentSocket);
				}
			}
			lock (this.inputServers)
			{
				this.inputServers.Remove(currentSocket);
			}
		}

		private bool SocketReceived(object sender, SocketAsyncEventArgs e)
		{
			SocketListener socketListener = sender as SocketListener;
			TCPInPacket tcpinPacket = null;
			Socket currentSocket = (e.UserToken as AsyncUserToken).CurrentSocket;
			lock (this.dictInPackets)
			{
				if (!this.dictInPackets.TryGetValue(currentSocket, out tcpinPacket))
				{
					tcpinPacket = this.tcpInPacketPool.Pop(currentSocket, new TCPCmdPacketEventHandler(this.TCPCmdPacketEvent));
					this.dictInPackets[currentSocket] = tcpinPacket;
				}
			}
			return tcpinPacket.WriteData(e.Buffer, e.Offset, e.BytesTransferred);
		}

		private void SocketSended(object sender, SocketAsyncEventArgs e)
		{
			TCPOutPacket item = (e.UserToken as AsyncUserToken).Tag as TCPOutPacket;
			this.tcpOutPacketPool.Push(item);
		}

		private bool TCPCmdPacketEvent(object sender)
		{
			TCPInPacket tcpinPacket = sender as TCPInPacket;
			TCPOutPacket tcpoutPacket = null;
			int num = 0;
			bool result;
			if (!this.inputServers.TryGetValue(tcpinPacket.CurrentSocket, out num))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未建立会话或会话已关闭: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket)), null, true);
				result = false;
			}
			else
			{
				long num2 = TimeUtil.NowEx();
				TCPProcessCmdResults tcpprocessCmdResults = PlatTCPManager.ProcessCmd(tcpinPacket.CurrentSocket, this.tcpOutPacketPool, (int)tcpinPacket.PacketCmdID, tcpinPacket.GetPacketBytes(), tcpinPacket.PacketDataSize, out tcpoutPacket);
				long num3 = TimeUtil.NowEx() - num2;
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && null != tcpoutPacket)
				{
					this.socketListener.SendData(tcpinPacket.CurrentSocket, tcpoutPacket);
				}
				else if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析并执行命令失败: {0},{1}, 关闭连接", (TCPGameServerCmds)tcpinPacket.PacketCmdID, Global.GetSocketRemoteEndPoint(tcpinPacket.CurrentSocket)), null, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessCmd(Socket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			PlatTCPManager.NetCommandFunc netCommandFunc = null;
			TCPProcessCmdResults result;
			if (!PlatTCPManager.getInstance().ProcessCmdFuncDict.TryGetValue(nID, out netCommandFunc) || null == netCommandFunc)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("未指定处理方式的命令: {0}, 关闭连接", (TCPPlatCmds)nID), null, true);
				result = TCPProcessCmdResults.RESULT_FAILED;
			}
			else
			{
				result = netCommandFunc(pool, nID, data, count, out tcpOutPacket);
			}
			return result;
		}

		private TCPProcessCmdResults ProcessRechargeData(TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			RechargeData rechargeData = null;
			try
			{
				rechargeData = DataHelper.BytesToObject<RechargeData>(data, 0, count);
				if (null == rechargeData)
				{
					throw new Exception();
				}
			}
			catch (Exception e)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string text = string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
				{
					rechargeData.Amount,
					rechargeData.UserID,
					rechargeData.ZoneID,
					rechargeData.order_no,
					rechargeData.cporder_no,
					rechargeData.Time,
					GameDBManager.serverDBInfo.ChargeKey
				});
				string text2 = MD5Helper.get_md5_string(text);
				string data2;
				if (text2 != rechargeData.Sign)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Sign Faild : sign={0} recvsign={1} strMD5Key={2}", text2, rechargeData.Sign, text), null, true);
					data2 = string.Format("{0}:{1}", rechargeData.Id, -4);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBQuery.CheckOrderNo(DBManager.getInstance(), rechargeData.order_no) && (DBQuery.CheckInputLogOrderNo(DBManager.getInstance(), rechargeData.order_no) || DBQuery.CheckInputLog2OrderNo(DBManager.getInstance(), rechargeData.order_no)))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Insert2OrderNo Faild : order_no={0}", rechargeData.order_no), null, true);
					data2 = string.Format("{0}:{1}", rechargeData.Id, -5);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!DBWriter.Insert2OrderNo(DBManager.getInstance(), rechargeData.order_no))
				{
				}
				if (!DBWriter.Insert2InputLog(DBManager.getInstance(), rechargeData))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Insert2InputLog Faild : order_no={0}", rechargeData.order_no), null, true);
					data2 = string.Format("{0}:{1}", rechargeData.Id, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				TempMoneyInfo tempMoneyInfo = new TempMoneyInfo();
				tempMoneyInfo.userID = rechargeData.UserID;
				tempMoneyInfo.chargeRoleID = rechargeData.RoleID;
				tempMoneyInfo.addUserMoney = rechargeData.Amount;
				tempMoneyInfo.addUserItem = rechargeData.ItemId;
				tempMoneyInfo.chargeTm = rechargeData.ChargeTime;
				tempMoneyInfo.cc = rechargeData.cc;
				tempMoneyInfo.budanflag = rechargeData.BudanFlag;
				if (!DBWriter.Insert2TempMoney(DBManager.getInstance(), tempMoneyInfo))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Insert2TempMoney Faild : order_no={0} userid={1} rid={2} Amount={3} ItemID={4}", new object[]
					{
						rechargeData.order_no,
						rechargeData.UserID,
						rechargeData.RoleID,
						rechargeData.Amount,
						rechargeData.ItemId
					}), null, true);
					data2 = string.Format("{0}:{1}", rechargeData.Id, -3);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}", rechargeData.Id, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private TCPProcessCmdResults ProcessPlatGift(TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				PlatGiftData platGiftData = DataHelper.BytesToObject<PlatGiftData>(data, 0, count);
				if (null == platGiftData)
				{
					throw new Exception("数据解析失败,null == cmdData");
				}
				string text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", new object[]
				{
					platGiftData.RoleID,
					platGiftData.UserID,
					platGiftData.Type,
					platGiftData.ID,
					platGiftData.ExtraData,
					platGiftData.Message,
					platGiftData.Time,
					GameDBManager.serverDBInfo.ChargeKey
				});
				string text2 = MD5Helper.get_md5_string(text);
				string data2;
				if (text2 != platGiftData.Sign)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessRechargeData Sign Faild : sign={0} recvsign={1} strMD5Key={2}", text2, platGiftData.Sign, text), null, true);
					data2 = string.Format("{0}:{1}", platGiftData.ID, -4);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}", platGiftData.ID, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令失败, CMD={0}", (TCPGameServerCmds)nID), null, true);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static PlatTCPManager instance = new PlatTCPManager();

		private TCPInPacketPool tcpInPacketPool = null;

		private TCPOutPacketPool tcpOutPacketPool = null;

		private Dictionary<Socket, int> inputServers = null;

		private Dictionary<Socket, TCPInPacket> dictInPackets = null;

		private Dictionary<int, PlatTCPManager.NetCommandFunc> _ProcessCmdFuncDict;

		private SocketListener socketListener = null;

		public delegate TCPProcessCmdResults NetCommandFunc(TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket);

		public class RechargeConst
		{
			public const int Sucess = 1;

			public const int InputError = -2;

			public const int TempMoneyError = -3;

			public const int SignError = -4;

			public const int AllExist = -5;
		}
	}
}
