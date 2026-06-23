using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using AutoCSer;
using AutoCSer.BinarySerialize;
using AutoCSer.IOS;
using AutoCSer.Json;
using AutoCSer.Log;
using AutoCSer.Metadata;
using AutoCSer.Net;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.TcpCall;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.HuanYingSiYuan.TcpStaticServer
{
	public class KfCall : Server
	{
		public static KeyValue<string, int>[] _identityCommandNames_()
		{
			KeyValue<string, int>[] array = new KeyValue<string, int>[23];
			array[0].Set("KF.TcpCall.EscapeBattle_K(Server.Data.EscapeBattleSyncData)SyncZhengBaData", 0);
			array[1].Set("KF.TcpCall.EscapeBattle_K(AutoCSer.Net.TcpInternalServer.ServerSocketSender,int,int,int,out Server.Data.EscapeBattleFuBenData)ZhengBaKuaFuLogin", 1);
			array[2].Set("KF.TcpCall.EscapeBattle_K(int,System.Collections.Generic.List<int>)GameResult", 2);
			array[3].Set("KF.TcpCall.EscapeBattle_K(int,out int,out int,out string[],out int[])ZhengBaRequestEnter", 3);
			array[4].Set("KF.TcpCall.KFBoCaiManager(Tmsk.Contract.KuaFuData.KFBoCaiShopDB,int)BoCaiBuyItem", 4);
			array[5].Set("KF.TcpCall.KFBoCaiManager(Tmsk.Contract.KuaFuData.KFBuyBocaiData)BuyBoCai", 5);
			array[6].Set("KF.TcpCall.KFBoCaiManager(int)GetKFStageData", 6);
			array[7].Set("KF.TcpCall.KFBoCaiManager(int)GetOpenLottery", 7);
			array[8].Set("KF.TcpCall.KFBoCaiManager(int,long,bool)GetOpenLottery", 8);
			array[9].Set("KF.TcpCall.KFBoCaiManager(int)GetWinHistory", 9);
			array[10].Set("KF.TcpCall.KFBoCaiManager(int,string,int,long)IsCanBuy", 10);
			array[11].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,string[])ExecuteCommand", 11);
			array[12].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,KF.Contract.KuaFuClientContext)InitializeClient", 12);
			array[13].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,System.Func<AutoCSer.Net.TcpServer.ReturnValue<KF.Remoting.KFCallMsg>,bool>)KeepGetMessage", 13);
			array[14].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,int,System.Collections.Generic.Dictionary<int,int>)UpdateKuaFuMapClientCount", 14);
			array[15].Set("KF.TcpCall.ZhanDuiZhengBa_K(KF.Contract.Data.ZhanDuiZhengBaSyncData)SyncZhengBaData", 15);
			array[16].Set("KF.TcpCall.ZhanDuiZhengBa_K(AutoCSer.Net.TcpInternalServer.ServerSocketSender,int,int,int,out KF.Contract.Data.ZhanDuiZhengBaFuBenData)ZhengBaKuaFuLogin", 16);
			array[17].Set("KF.TcpCall.ZhanDuiZhengBa_K(int,int)ZhengBaPkResult", 17);
			array[18].Set("KF.TcpCall.ZhanDuiZhengBa_K(int,out int,out int,out string[],out int[])ZhengBaRequestEnter", 18);
			array[19].Set("KF.TcpCall.EscapeBattle_K(int,int,int)ZhanDuiJoin", 19);
			array[20].Set("KF.TcpCall.EscapeBattle_K(int,int)GameState", 20);
			array[21].Set("KF.TcpCall.EscapeBattle_K(int)GetZhanDuiState", 21);
			array[22].Set("KF.TcpCall.TestS2KFCommunication(int,bool)SendData", 22);
			return array;
		}

		public KfCall(ServerAttribute attribute = null, Func<Socket, bool> verify = null, Action<SubArray<byte>> onCustomData = null, ILog log = null)
		{
			ServerAttribute serverAttribute;
			if ((serverAttribute = attribute) == null)
			{
				serverAttribute = (attribute = ServerAttribute.GetConfig("KfCall", typeof(ZhanDuiZhengBa_K), true));
			}
			base..ctor(serverAttribute, verify, onCustomData, log, true);
			base.setCommandData(23);
			base.setCommand(0);
			base.setCommand(1);
			base.setCommand(2);
			base.setCommand(3);
			base.setCommand(4);
			base.setCommand(5);
			base.setCommand(6);
			base.setCommand(7);
			base.setCommand(8);
			base.setCommand(9);
			base.setCommand(10);
			base.setCommand(11);
			base.setVerifyCommand(12);
			base.setCommand(13);
			base.setCommand(14);
			base.setCommand(15);
			base.setCommand(16);
			base.setCommand(17);
			base.setCommand(18);
			base.setCommand(19);
			base.setCommand(20);
			base.setCommand(21);
			base.setCommand(22);
			if (attribute.IsAutoServer)
			{
				this.Start();
			}
		}

		public override void DoCommand(int index, ServerSocketSender sender, ref SubArray<byte> data)
		{
			long num = TimeUtil.NOW();
			switch (index)
			{
			case 128:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p1 p = default(KfCall._p1);
					if (sender.DeSerialize<KfCall._p1>(ref data, ref p, false))
					{
						(ServerCall<KfCall._s0>.Pop() ?? new KfCall._s0()).Set(sender, 5, ref p);
						CmdMonitor.RecordCmdDetail(0, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(0, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 129:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p3 p2 = default(KfCall._p3);
					if (sender.DeSerialize<KfCall._p3>(ref data, ref p2, false))
					{
						(ServerCall<KfCall._s1>.Pop() ?? new KfCall._s1()).Set(sender, 5, ref p2);
						CmdMonitor.RecordCmdDetail(1, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(1, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 130:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p5 p3 = default(KfCall._p5);
					if (sender.DeSerialize<KfCall._p5>(ref data, ref p3, false))
					{
						(ServerCall<KfCall._s2>.Pop() ?? new KfCall._s2()).Set(sender, 5, ref p3);
						CmdMonitor.RecordCmdDetail(2, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(2, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 131:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p7 p4 = default(KfCall._p7);
					if (sender.DeSerialize<KfCall._p7>(ref data, ref p4, false))
					{
						(ServerCall<KfCall._s3>.Pop() ?? new KfCall._s3()).Set(sender, 5, ref p4);
						CmdMonitor.RecordCmdDetail(3, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(3, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 132:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p9 p5 = default(KfCall._p9);
					if (sender.DeSerialize<KfCall._p9>(ref data, ref p5, false))
					{
						(ServerCall<KfCall._s4>.Pop() ?? new KfCall._s4()).Set(sender, 5, ref p5);
						CmdMonitor.RecordCmdDetail(4, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(4, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 133:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p11 p6 = default(KfCall._p11);
					if (sender.DeSerialize<KfCall._p11>(ref data, ref p6, false))
					{
						(ServerCall<KfCall._s5>.Pop() ?? new KfCall._s5()).Set(sender, 5, ref p6);
						CmdMonitor.RecordCmdDetail(5, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(5, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 134:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p12 p7 = default(KfCall._p12);
					if (sender.DeSerialize<KfCall._p12>(ref data, ref p7, true))
					{
						(ServerCall<KfCall._s6>.Pop() ?? new KfCall._s6()).Set(sender, 5, ref p7);
						CmdMonitor.RecordCmdDetail(6, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(6, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 135:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p12 p7 = default(KfCall._p12);
					if (sender.DeSerialize<KfCall._p12>(ref data, ref p7, true))
					{
						(ServerCall<KfCall._s7>.Pop() ?? new KfCall._s7()).Set(sender, 5, ref p7);
						CmdMonitor.RecordCmdDetail(7, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(7, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 136:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p15 p8 = default(KfCall._p15);
					if (sender.DeSerialize<KfCall._p15>(ref data, ref p8, true))
					{
						(ServerCall<KfCall._s8>.Pop() ?? new KfCall._s8()).Set(sender, 5, ref p8);
						CmdMonitor.RecordCmdDetail(8, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(8, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 137:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p12 p7 = default(KfCall._p12);
					if (sender.DeSerialize<KfCall._p12>(ref data, ref p7, true))
					{
						(ServerCall<KfCall._s9>.Pop() ?? new KfCall._s9()).Set(sender, 5, ref p7);
						CmdMonitor.RecordCmdDetail(9, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(9, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 138:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p18 p9 = default(KfCall._p18);
					if (sender.DeSerialize<KfCall._p18>(ref data, ref p9, true))
					{
						(ServerCall<KfCall._s10>.Pop() ?? new KfCall._s10()).Set(sender, 5, ref p9);
						CmdMonitor.RecordCmdDetail(10, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(10, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 139:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p19 p10 = default(KfCall._p19);
					if (sender.DeSerialize<KfCall._p19>(ref data, ref p10, false))
					{
						(ServerCall<KfCall._s11>.Pop() ?? new KfCall._s11()).Set(sender, 5, ref p10);
						CmdMonitor.RecordCmdDetail(11, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(11, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 140:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p20 p11 = default(KfCall._p20);
					if (sender.DeSerialize<KfCall._p20>(ref data, ref p11, false))
					{
						KfCall._p10 p12 = default(KfCall._p10);
						bool flag = KFServiceBase.TcpStaticServer._M13(sender, p11.p0);
						if (flag)
						{
							sender.SetVerifyMethod();
						}
						p12.Return = flag;
						sender.Push<KfCall._p10>(KfCall._c13, ref p12);
						CmdMonitor.RecordCmdDetail(12, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(12, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 141:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p21 p13 = default(KfCall._p21);
					KFServiceBase.TcpStaticServer._M14(sender, sender.GetCallback<KfCall._p21, KFCallMsg>(KfCall._c14, ref p13));
					CmdMonitor.RecordCmdDetail(13, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
					break;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(13, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 142:
				try
				{
					KfCall._p22 p14 = default(KfCall._p22);
					if (sender.DeSerialize<KfCall._p22>(ref data, ref p14, false))
					{
						(ServerCall<KfCall._s14>.Pop() ?? new KfCall._s14()).Set(sender, 5, ref p14);
						CmdMonitor.RecordCmdDetail(14, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
				}
				catch (Exception ex)
				{
					sender.AddLog(ex);
				}
				CmdMonitor.RecordCmdDetail(14, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			case 143:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p23 p15 = default(KfCall._p23);
					if (sender.DeSerialize<KfCall._p23>(ref data, ref p15, false))
					{
						(ServerCall<KfCall._s15>.Pop() ?? new KfCall._s15()).Set(sender, 5, ref p15);
						CmdMonitor.RecordCmdDetail(15, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(15, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 144:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p25 p16 = default(KfCall._p25);
					if (sender.DeSerialize<KfCall._p25>(ref data, ref p16, false))
					{
						(ServerCall<KfCall._s16>.Pop() ?? new KfCall._s16()).Set(sender, 5, ref p16);
						CmdMonitor.RecordCmdDetail(16, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(16, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 145:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p27 p17 = default(KfCall._p27);
					if (sender.DeSerialize<KfCall._p27>(ref data, ref p17, true))
					{
						(ServerCall<KfCall._s17>.Pop() ?? new KfCall._s17()).Set(sender, 5, ref p17);
						CmdMonitor.RecordCmdDetail(17, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(17, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 146:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p7 p4 = default(KfCall._p7);
					if (sender.DeSerialize<KfCall._p7>(ref data, ref p4, false))
					{
						(ServerCall<KfCall._s18>.Pop() ?? new KfCall._s18()).Set(sender, 5, ref p4);
						CmdMonitor.RecordCmdDetail(18, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(18, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 147:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p29 p18 = default(KfCall._p29);
					if (sender.DeSerialize<KfCall._p29>(ref data, ref p18, true))
					{
						(ServerCall<KfCall._s19>.Pop() ?? new KfCall._s19()).Set(sender, 5, ref p18);
						CmdMonitor.RecordCmdDetail(19, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(19, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 148:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p27 p17 = default(KfCall._p27);
					if (sender.DeSerialize<KfCall._p27>(ref data, ref p17, true))
					{
						(ServerCall<KfCall._s20>.Pop() ?? new KfCall._s20()).Set(sender, 5, ref p17);
						CmdMonitor.RecordCmdDetail(20, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(20, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 149:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p12 p7 = default(KfCall._p12);
					if (sender.DeSerialize<KfCall._p12>(ref data, ref p7, true))
					{
						(ServerCall<KfCall._s21>.Pop() ?? new KfCall._s21()).Set(sender, 5, ref p7);
						CmdMonitor.RecordCmdDetail(21, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(21, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			case 150:
			{
				ReturnType returnType = 0;
				try
				{
					KfCall._p30 p19 = default(KfCall._p30);
					if (sender.DeSerialize<KfCall._p30>(ref data, ref p19, true))
					{
						(ServerCall<KfCall._s22>.Pop() ?? new KfCall._s22()).Set(sender, 5, ref p19);
						CmdMonitor.RecordCmdDetail(22, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
						break;
					}
					returnType = 2;
				}
				catch (Exception ex)
				{
					returnType = 3;
					sender.AddLog(ex);
				}
				sender.Push(returnType);
				CmdMonitor.RecordCmdDetail(22, TimeUtil.NOW() - num, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
				break;
			}
			}
		}

		static KfCall()
		{
			Type[] array = new Type[7];
			array[0] = typeof(KfCall._p12);
			array[1] = typeof(KfCall._p15);
			array[2] = typeof(KfCall._p18);
			array[3] = typeof(KfCall._p27);
			array[4] = typeof(KfCall._p29);
			array[5] = typeof(KfCall._p30);
			Type[] array2 = array;
			array = new Type[4];
			array[0] = typeof(KfCall._p6);
			array[1] = typeof(KfCall._p10);
			array[2] = typeof(KfCall._p31);
			Type[] array3 = array;
			array = new Type[12];
			array[0] = typeof(KfCall._p1);
			array[1] = typeof(KfCall._p3);
			array[2] = typeof(KfCall._p5);
			array[3] = typeof(KfCall._p7);
			array[4] = typeof(KfCall._p9);
			array[5] = typeof(KfCall._p11);
			array[6] = typeof(KfCall._p19);
			array[7] = typeof(KfCall._p20);
			array[8] = typeof(KfCall._p22);
			array[9] = typeof(KfCall._p23);
			array[10] = typeof(KfCall._p25);
			Type[] array4 = array;
			array = new Type[12];
			array[0] = typeof(KfCall._p2);
			array[1] = typeof(KfCall._p4);
			array[2] = typeof(KfCall._p8);
			array[3] = typeof(KfCall._p13);
			array[4] = typeof(KfCall._p14);
			array[5] = typeof(KfCall._p16);
			array[6] = typeof(KfCall._p17);
			array[7] = typeof(KfCall._p21);
			array[8] = typeof(KfCall._p24);
			array[9] = typeof(KfCall._p26);
			array[10] = typeof(KfCall._p28);
			Type[] array5 = array;
			array = new Type[1];
			Type[] array6 = array;
			array = new Type[1];
			CommandBase.CompileSerialize(array2, array3, array4, array5, array6, array);
		}

		private static readonly OutputInfo _c1 = new OutputInfo
		{
			OutputParameterIndex = 2,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c2 = new OutputInfo
		{
			OutputParameterIndex = 4,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c3 = new OutputInfo
		{
			OutputParameterIndex = 6,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c4 = new OutputInfo
		{
			OutputParameterIndex = 8,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c5 = new OutputInfo
		{
			OutputParameterIndex = 10,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c6 = new OutputInfo
		{
			OutputParameterIndex = 10,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c7 = new OutputInfo
		{
			OutputParameterIndex = 13,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c8 = new OutputInfo
		{
			OutputParameterIndex = 14,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c9 = new OutputInfo
		{
			OutputParameterIndex = 16,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c10 = new OutputInfo
		{
			OutputParameterIndex = 17,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c11 = new OutputInfo
		{
			OutputParameterIndex = 10,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c12 = new OutputInfo
		{
			OutputParameterIndex = 6,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c13 = new OutputInfo
		{
			OutputParameterIndex = 10,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c14 = new OutputInfo
		{
			OutputParameterIndex = 21,
			IsKeepCallback = 1,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c15 = new OutputInfo
		{
			OutputParameterIndex = 0,
			IsClientSendOnly = 1,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c16 = new OutputInfo
		{
			OutputParameterIndex = 24,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c17 = new OutputInfo
		{
			OutputParameterIndex = 26,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c18 = new OutputInfo
		{
			OutputParameterIndex = 28,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c19 = new OutputInfo
		{
			OutputParameterIndex = 8,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c20 = new OutputInfo
		{
			OutputParameterIndex = 6,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c21 = new OutputInfo
		{
			OutputParameterIndex = 6,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c22 = new OutputInfo
		{
			OutputParameterIndex = 6,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private static readonly OutputInfo _c23 = new OutputInfo
		{
			OutputParameterIndex = 31,
			IsSimpleSerializeOutputParamter = true,
			IsBuildOutputThread = true
		};

		private sealed class _s0 : ServerCall<KfCall._s0, KfCall._p1>
		{
			private void get(ref ReturnValue<KfCall._p2> value)
			{
				try
				{
					EscapeBattleSyncData @return = EscapeBattle_K.TcpStaticServer._M1(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p2> returnValue = default(ReturnValue<KfCall._p2>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p2>(this.CommandIndex, KfCall._c1, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s1 : ServerCall<KfCall._s1, KfCall._p3>
		{
			private void get(ref ReturnValue<KfCall._p4> value)
			{
				try
				{
					int @return = EscapeBattle_K.TcpStaticServer._M2(this.Sender, this.inputParameter.p1, this.inputParameter.p2, this.inputParameter.p3, out value.Value.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p4> returnValue = default(ReturnValue<KfCall._p4>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p4>(this.CommandIndex, KfCall._c2, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s2 : ServerCall<KfCall._s2, KfCall._p5>
		{
			private void get(ref ReturnValue<KfCall._p6> value)
			{
				try
				{
					int @return = EscapeBattle_K.TcpStaticServer._M3(this.inputParameter.p1, this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p6> returnValue = default(ReturnValue<KfCall._p6>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c3, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s3 : ServerCall<KfCall._s3, KfCall._p7>
		{
			private void get(ref ReturnValue<KfCall._p8> value)
			{
				try
				{
					int @return = EscapeBattle_K.TcpStaticServer._M4(this.inputParameter.p0, out value.Value.p0, out value.Value.p1, out value.Value.p3, out value.Value.p2);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p8> returnValue = default(ReturnValue<KfCall._p8>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p8>(this.CommandIndex, KfCall._c4, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s4 : ServerCall<KfCall._s4, KfCall._p9>
		{
			private void get(ref ReturnValue<KfCall._p10> value)
			{
				try
				{
					bool @return = KFBoCaiManager.TcpStaticServer._M5(this.inputParameter.p0, this.inputParameter.p1);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p10> returnValue = default(ReturnValue<KfCall._p10>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p10>(this.CommandIndex, KfCall._c5, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s5 : ServerCall<KfCall._s5, KfCall._p11>
		{
			private void get(ref ReturnValue<KfCall._p10> value)
			{
				try
				{
					bool @return = KFBoCaiManager.TcpStaticServer._M6(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p10> returnValue = default(ReturnValue<KfCall._p10>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p10>(this.CommandIndex, KfCall._c6, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s6 : ServerCall<KfCall._s6, KfCall._p12>
		{
			private void get(ref ReturnValue<KfCall._p13> value)
			{
				try
				{
					KFStageData @return = KFBoCaiManager.TcpStaticServer._M7(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p13> returnValue = default(ReturnValue<KfCall._p13>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p13>(this.CommandIndex, KfCall._c7, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s7 : ServerCall<KfCall._s7, KfCall._p12>
		{
			private void get(ref ReturnValue<KfCall._p14> value)
			{
				try
				{
					OpenLottery @return = KFBoCaiManager.TcpStaticServer._M8(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p14> returnValue = default(ReturnValue<KfCall._p14>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p14>(this.CommandIndex, KfCall._c8, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s8 : ServerCall<KfCall._s8, KfCall._p15>
		{
			private void get(ref ReturnValue<KfCall._p16> value)
			{
				try
				{
					List<OpenLottery> @return = KFBoCaiManager.TcpStaticServer._M9(this.inputParameter.p1, this.inputParameter.p2, this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p16> returnValue = default(ReturnValue<KfCall._p16>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p16>(this.CommandIndex, KfCall._c9, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s9 : ServerCall<KfCall._s9, KfCall._p12>
		{
			private void get(ref ReturnValue<KfCall._p17> value)
			{
				try
				{
					List<KFBoCaoHistoryData> @return = KFBoCaiManager.TcpStaticServer._M10(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p17> returnValue = default(ReturnValue<KfCall._p17>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p17>(this.CommandIndex, KfCall._c10, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s10 : ServerCall<KfCall._s10, KfCall._p18>
		{
			private void get(ref ReturnValue<KfCall._p10> value)
			{
				try
				{
					bool @return = KFBoCaiManager.TcpStaticServer._M11(this.inputParameter.p0, this.inputParameter.p3, this.inputParameter.p1, this.inputParameter.p2);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p10> returnValue = default(ReturnValue<KfCall._p10>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p10>(this.CommandIndex, KfCall._c11, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s11 : ServerCall<KfCall._s11, KfCall._p19>
		{
			private void get(ref ReturnValue<KfCall._p6> value)
			{
				try
				{
					int @return = KFServiceBase.TcpStaticServer._M12(this.Sender, this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p6> returnValue = default(ReturnValue<KfCall._p6>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c12, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s14 : ServerCall<KfCall._s14, KfCall._p22>
		{
			private void get(ref ReturnValue value)
			{
				try
				{
					KFServiceBase.TcpStaticServer._M15(this.Sender, this.inputParameter.p1, this.inputParameter.p0);
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue returnValue = default(ReturnValue);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s15 : ServerCall<KfCall._s15, KfCall._p23>
		{
			private void get(ref ReturnValue<KfCall._p24> value)
			{
				try
				{
					ZhanDuiZhengBaSyncData @return = ZhanDuiZhengBa_K.TcpStaticServer._M16(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p24> returnValue = default(ReturnValue<KfCall._p24>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p24>(this.CommandIndex, KfCall._c16, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s16 : ServerCall<KfCall._s16, KfCall._p25>
		{
			private void get(ref ReturnValue<KfCall._p26> value)
			{
				try
				{
					int @return = ZhanDuiZhengBa_K.TcpStaticServer._M17(this.Sender, this.inputParameter.p1, this.inputParameter.p2, this.inputParameter.p3, out value.Value.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p26> returnValue = default(ReturnValue<KfCall._p26>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p26>(this.CommandIndex, KfCall._c17, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s17 : ServerCall<KfCall._s17, KfCall._p27>
		{
			private void get(ref ReturnValue<KfCall._p28> value)
			{
				try
				{
					List<ZhanDuiZhengBaNtfPkResultData> @return = ZhanDuiZhengBa_K.TcpStaticServer._M18(this.inputParameter.p0, this.inputParameter.p1);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p28> returnValue = default(ReturnValue<KfCall._p28>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p28>(this.CommandIndex, KfCall._c18, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s18 : ServerCall<KfCall._s18, KfCall._p7>
		{
			private void get(ref ReturnValue<KfCall._p8> value)
			{
				try
				{
					int @return = ZhanDuiZhengBa_K.TcpStaticServer._M19(this.inputParameter.p0, out value.Value.p0, out value.Value.p1, out value.Value.p3, out value.Value.p2);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p8> returnValue = default(ReturnValue<KfCall._p8>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p8>(this.CommandIndex, KfCall._c19, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s19 : ServerCall<KfCall._s19, KfCall._p29>
		{
			private void get(ref ReturnValue<KfCall._p6> value)
			{
				try
				{
					int @return = EscapeBattle_K.TcpStaticServer._M20(this.inputParameter.p0, this.inputParameter.p1, this.inputParameter.p2);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p6> returnValue = default(ReturnValue<KfCall._p6>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c20, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s20 : ServerCall<KfCall._s20, KfCall._p27>
		{
			private void get(ref ReturnValue<KfCall._p6> value)
			{
				try
				{
					int @return = EscapeBattle_K.TcpStaticServer._M21(this.inputParameter.p0, this.inputParameter.p1);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p6> returnValue = default(ReturnValue<KfCall._p6>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c21, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s21 : ServerCall<KfCall._s21, KfCall._p12>
		{
			private void get(ref ReturnValue<KfCall._p6> value)
			{
				try
				{
					int @return = EscapeBattle_K.TcpStaticServer._M22(this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p6> returnValue = default(ReturnValue<KfCall._p6>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c22, ref returnValue);
				}
				base.push(this);
			}
		}

		private sealed class _s22 : ServerCall<KfCall._s22, KfCall._p30>
		{
			private void get(ref ReturnValue<KfCall._p31> value)
			{
				try
				{
					string @return = TestS2KFCommunication.TcpStaticServer._M23(this.inputParameter.p1, this.inputParameter.p0);
					value.Value.Return = @return;
					value.Type = 7;
				}
				catch (Exception ex)
				{
					value.Type = 3;
					this.Sender.AddLog(ex);
				}
			}

			public override void Call()
			{
				ReturnValue<KfCall._p31> returnValue = default(ReturnValue<KfCall._p31>);
				if (this.Sender.IsSocket)
				{
					this.get(ref returnValue);
					this.Sender.Push<KfCall._p31>(this.CommandIndex, KfCall._c23, ref returnValue);
				}
				base.push(this);
			}
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p1
		{
			public EscapeBattleSyncData p0;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p2 : IReturnParameter<EscapeBattleSyncData>
		{
			[Preserve(Conditional = true)]
			public EscapeBattleSyncData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public EscapeBattleSyncData Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p3
		{
			public EscapeBattleFuBenData p0;

			public int p1;

			public int p2;

			public int p3;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p4 : IReturnParameter<int>
		{
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			public EscapeBattleFuBenData p0;

			[IgnoreMember]
			public int Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p5
		{
			public List<int> p0;

			public int p1;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p6 : IReturnParameter<int>
		{
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public int Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p7
		{
			public int p0;

			public int p1;

			public int p2;

			public int[] p3;

			public string[] p4;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p8 : IReturnParameter<int>
		{
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			public int p0;

			public int p1;

			public int[] p2;

			public string[] p3;

			[IgnoreMember]
			public int Ret;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p9
		{
			public KFBoCaiShopDB p0;

			public int p1;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p10 : IReturnParameter<bool>
		{
			[Preserve(Conditional = true)]
			public bool Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public bool Ret;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p11
		{
			public KFBuyBocaiData p0;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p12
		{
			public int p0;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p13 : IReturnParameter<KFStageData>
		{
			[Preserve(Conditional = true)]
			public KFStageData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public KFStageData Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p14 : IReturnParameter<OpenLottery>
		{
			[Preserve(Conditional = true)]
			public OpenLottery Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public OpenLottery Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p15
		{
			public bool p0;

			public int p1;

			public long p2;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p16 : IReturnParameter<List<OpenLottery>>
		{
			[Preserve(Conditional = true)]
			public List<OpenLottery> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public List<OpenLottery> Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p17 : IReturnParameter<List<KFBoCaoHistoryData>>
		{
			[Preserve(Conditional = true)]
			public List<KFBoCaoHistoryData> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public List<KFBoCaoHistoryData> Ret;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p18
		{
			public int p0;

			public int p1;

			public long p2;

			public string p3;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p19
		{
			public string[] p0;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p20
		{
			public KuaFuClientContext p0;
		}

		[Serialize(IsMemberMap = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p21 : IReturnParameter<KFCallMsg>
		{
			[Preserve(Conditional = true)]
			public KFCallMsg Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public KFCallMsg Ret;
		}

		[BoxSerialize, Serialize(IsMemberMap = false), StructLayout(LayoutKind.Auto)]
		internal struct _p22
		{
			public Dictionary<int, int> p0;

			public int p1;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p23
		{
			public ZhanDuiZhengBaSyncData p0;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p24 : IReturnParameter<ZhanDuiZhengBaSyncData>
		{
			[Preserve(Conditional = true)]
			public ZhanDuiZhengBaSyncData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public ZhanDuiZhengBaSyncData Ret;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p25
		{
			public ZhanDuiZhengBaFuBenData p0;

			public int p1;

			public int p2;

			public int p3;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p26 : IReturnParameter<int>
		{
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			public ZhanDuiZhengBaFuBenData p0;

			[IgnoreMember]
			public int Ret;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p27
		{
			public int p0;

			public int p1;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p28 : IReturnParameter<List<ZhanDuiZhengBaNtfPkResultData>>
		{
			[Preserve(Conditional = true)]
			public List<ZhanDuiZhengBaNtfPkResultData> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public List<ZhanDuiZhengBaNtfPkResultData> Ret;
		}

		[BoxSerialize, Serialize(IsMemberMap = false, IsReferenceMember = false), StructLayout(LayoutKind.Auto)]
		internal struct _p29
		{
			public int p0;

			public int p1;

			public int p2;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p30
		{
			public bool p0;

			public int p1;
		}

		[Serialize(IsMemberMap = false, IsReferenceMember = false), BoxSerialize, StructLayout(LayoutKind.Auto)]
		internal struct _p31 : IReturnParameter<string>
		{
			[Preserve(Conditional = true)]
			public string Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			[IgnoreMember]
			public string Ret;
		}
	}
}
