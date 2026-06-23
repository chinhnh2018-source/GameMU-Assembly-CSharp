using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AutoCSer.Net;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using KF.Contract;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.HuanYingSiYuan.TcpStaticClient;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.TcpCall
{
	public static class TcpCall
	{
		public class EscapeBattle_K
		{
			public static ReturnValue<int> GameResult(int gameId, List<int> zhanDuiScoreList)
			{
				AutoWaitReturnValue<KfCall._p6> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p6>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p5 p = new KfCall._p5
						{
							p1 = gameId,
							p0 = zhanDuiScoreList
						};
						KfCall._p6 p2 = default(KfCall._p6);
						ReturnType type = sender.WaitGet<KfCall._p5, KfCall._p6>(TcpCall.EscapeBattle_K._c3, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p6>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> GameState(int gameId, int state)
			{
				AutoWaitReturnValue<KfCall._p6> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p6>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p27 p = new KfCall._p27
						{
							p0 = gameId,
							p1 = state
						};
						KfCall._p6 p2 = default(KfCall._p6);
						ReturnType type = sender.WaitGet<KfCall._p27, KfCall._p6>(TcpCall.EscapeBattle_K._c21, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p6>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> GetZhanDuiState(int zhanDuiID)
			{
				AutoWaitReturnValue<KfCall._p6> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p6>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p12 p = new KfCall._p12
						{
							p0 = zhanDuiID
						};
						KfCall._p6 p2 = default(KfCall._p6);
						ReturnType type = sender.WaitGet<KfCall._p12, KfCall._p6>(TcpCall.EscapeBattle_K._c22, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p6>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<EscapeBattleSyncData> SyncZhengBaData(EscapeBattleSyncData lastSyncData)
			{
				AutoWaitReturnValue<KfCall._p2> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p2>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p1 p = new KfCall._p1
						{
							p0 = lastSyncData
						};
						KfCall._p2 p2 = default(KfCall._p2);
						ReturnType type = sender.WaitGet<KfCall._p1, KfCall._p2>(TcpCall.EscapeBattle_K._c1, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<EscapeBattleSyncData>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p2>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<EscapeBattleSyncData>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> ZhanDuiJoin(int zhanDuiID, int jiFen, int readyNum)
			{
				AutoWaitReturnValue<KfCall._p6> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p6>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p29 p = new KfCall._p29
						{
							p0 = zhanDuiID,
							p1 = jiFen,
							p2 = readyNum
						};
						KfCall._p6 p2 = default(KfCall._p6);
						ReturnType type = sender.WaitGet<KfCall._p29, KfCall._p6>(TcpCall.EscapeBattle_K._c20, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p6>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> ZhengBaKuaFuLogin(int zhanDuiID, int gameId, int srcServerID, out EscapeBattleFuBenData copyData)
			{
				AutoWaitReturnValue<KfCall._p4> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p4>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p3 p = new KfCall._p3
						{
							p1 = zhanDuiID,
							p2 = gameId,
							p3 = srcServerID
						};
						KfCall._p4 p2 = default(KfCall._p4);
						ReturnType type = sender.WaitGet<KfCall._p3, KfCall._p4>(TcpCall.EscapeBattle_K._c2, ref autoWaitReturnValue, ref p, ref p2);
						copyData = p2.p0;
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p4>.PushNotNull(autoWaitReturnValue);
					}
				}
				copyData = null;
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> ZhengBaRequestEnter(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
			{
				AutoWaitReturnValue<KfCall._p8> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p8>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p7 p = new KfCall._p7
						{
							p0 = zhanDuiID
						};
						KfCall._p8 p2 = default(KfCall._p8);
						ReturnType type = sender.WaitGet<KfCall._p7, KfCall._p8>(TcpCall.EscapeBattle_K._c4, ref autoWaitReturnValue, ref p, ref p2);
						gameId = p2.p0;
						kuaFuServerID = p2.p1;
						ips = p2.p3;
						ports = p2.p2;
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p8>.PushNotNull(autoWaitReturnValue);
					}
				}
				gameId = 0;
				kuaFuServerID = 0;
				ips = null;
				ports = null;
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			private static readonly CommandInfo _c3 = new CommandInfo
			{
				Command = 130,
				InputParameterIndex = 5,
				TaskType = 0,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c21 = new CommandInfo
			{
				Command = 148,
				InputParameterIndex = 27,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c22 = new CommandInfo
			{
				Command = 149,
				InputParameterIndex = 12,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c1 = new CommandInfo
			{
				Command = 128,
				InputParameterIndex = 1,
				TaskType = 0
			};

			private static readonly CommandInfo _c20 = new CommandInfo
			{
				Command = 147,
				InputParameterIndex = 29,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c2 = new CommandInfo
			{
				Command = 129,
				InputParameterIndex = 3,
				TaskType = 0
			};

			private static readonly CommandInfo _c4 = new CommandInfo
			{
				Command = 131,
				InputParameterIndex = 7,
				TaskType = 0
			};
		}

		public class KFBoCaiManager
		{
			public static ReturnValue<bool> BoCaiBuyItem(KFBoCaiShopDB Item, int maxNum)
			{
				AutoWaitReturnValue<KfCall._p10> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p10>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p9 p = new KfCall._p9
						{
							p0 = Item,
							p1 = maxNum
						};
						KfCall._p10 p2 = default(KfCall._p10);
						ReturnType type = sender.WaitGet<KfCall._p9, KfCall._p10>(TcpCall.KFBoCaiManager._c5, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<bool>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p10>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<bool>
				{
					Type = 11
				};
			}

			public static ReturnValue<bool> BuyBoCai(KFBuyBocaiData data)
			{
				AutoWaitReturnValue<KfCall._p10> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p10>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p11 p = new KfCall._p11
						{
							p0 = data
						};
						KfCall._p10 p2 = default(KfCall._p10);
						ReturnType type = sender.WaitGet<KfCall._p11, KfCall._p10>(TcpCall.KFBoCaiManager._c6, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<bool>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p10>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<bool>
				{
					Type = 11
				};
			}

			public static ReturnValue<KFStageData> GetKFStageData(int type)
			{
				AutoWaitReturnValue<KfCall._p13> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p13>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p12 p = new KfCall._p12
						{
							p0 = type
						};
						KfCall._p13 p2 = default(KfCall._p13);
						ReturnType type2 = sender.WaitGet<KfCall._p12, KfCall._p13>(TcpCall.KFBoCaiManager._c7, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<KFStageData>
						{
							Type = type2,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p13>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<KFStageData>
				{
					Type = 11
				};
			}

			public static ReturnValue<OpenLottery> GetOpenLottery(int type)
			{
				AutoWaitReturnValue<KfCall._p14> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p14>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p12 p = new KfCall._p12
						{
							p0 = type
						};
						KfCall._p14 p2 = default(KfCall._p14);
						ReturnType type2 = sender.WaitGet<KfCall._p12, KfCall._p14>(TcpCall.KFBoCaiManager._c8, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<OpenLottery>
						{
							Type = type2,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p14>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<OpenLottery>
				{
					Type = 11
				};
			}

			public static ReturnValue<List<OpenLottery>> GetOpenLottery(int type, long DataPeriods, bool getOne)
			{
				AutoWaitReturnValue<KfCall._p16> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p16>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p15 p = new KfCall._p15
						{
							p1 = type,
							p2 = DataPeriods,
							p0 = getOne
						};
						KfCall._p16 p2 = default(KfCall._p16);
						ReturnType type2 = sender.WaitGet<KfCall._p15, KfCall._p16>(TcpCall.KFBoCaiManager._c9, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<List<OpenLottery>>
						{
							Type = type2,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p16>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<List<OpenLottery>>
				{
					Type = 11
				};
			}

			public static ReturnValue<List<KFBoCaoHistoryData>> GetWinHistory(int type)
			{
				AutoWaitReturnValue<KfCall._p17> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p17>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p12 p = new KfCall._p12
						{
							p0 = type
						};
						KfCall._p17 p2 = default(KfCall._p17);
						ReturnType type2 = sender.WaitGet<KfCall._p12, KfCall._p17>(TcpCall.KFBoCaiManager._c10, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<List<KFBoCaoHistoryData>>
						{
							Type = type2,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p17>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<List<KFBoCaoHistoryData>>
				{
					Type = 11
				};
			}

			public static ReturnValue<bool> IsCanBuy(int type, string buyValue, int buyNum, long DataPeriods)
			{
				AutoWaitReturnValue<KfCall._p10> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p10>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p18 p = new KfCall._p18
						{
							p0 = type,
							p3 = buyValue,
							p1 = buyNum,
							p2 = DataPeriods
						};
						KfCall._p10 p2 = default(KfCall._p10);
						ReturnType type2 = sender.WaitGet<KfCall._p18, KfCall._p10>(TcpCall.KFBoCaiManager._c11, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<bool>
						{
							Type = type2,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p10>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<bool>
				{
					Type = 11
				};
			}

			private static readonly CommandInfo _c5 = new CommandInfo
			{
				Command = 132,
				InputParameterIndex = 9,
				TaskType = 0,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c6 = new CommandInfo
			{
				Command = 133,
				InputParameterIndex = 11,
				TaskType = 0,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c7 = new CommandInfo
			{
				Command = 134,
				InputParameterIndex = 12,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true
			};

			private static readonly CommandInfo _c8 = new CommandInfo
			{
				Command = 135,
				InputParameterIndex = 12,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true
			};

			private static readonly CommandInfo _c9 = new CommandInfo
			{
				Command = 136,
				InputParameterIndex = 15,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true
			};

			private static readonly CommandInfo _c10 = new CommandInfo
			{
				Command = 137,
				InputParameterIndex = 12,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true
			};

			private static readonly CommandInfo _c11 = new CommandInfo
			{
				Command = 138,
				InputParameterIndex = 18,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true,
				IsSimpleSerializeOutputParamter = true
			};
		}

		public class KFServiceBase
		{
			public static ReturnValue<int> ExecuteCommand(string[] args)
			{
				AutoWaitReturnValue<KfCall._p6> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p6>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p19 p = new KfCall._p19
						{
							p0 = args
						};
						KfCall._p6 p2 = default(KfCall._p6);
						ReturnType type = sender.WaitGet<KfCall._p19, KfCall._p6>(TcpCall.KFServiceBase._c12, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p6>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<bool> InitializeClient(ClientSocketSender _sender_, KuaFuClientContext clientInfo)
			{
				AutoWaitReturnValue<KfCall._p10> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p10>.Pop();
				try
				{
					if (_sender_ != null)
					{
						KfCall._p20 p = new KfCall._p20
						{
							p0 = clientInfo
						};
						KfCall._p10 p2 = default(KfCall._p10);
						ReturnType type = _sender_.WaitGet<KfCall._p20, KfCall._p10>(TcpCall.KFServiceBase._c13, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<bool>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p10>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<bool>
				{
					Type = 11
				};
			}

			public static KeepCallback KeepGetMessage(Action<ReturnValue<KFCallMsg>> _onReturn_)
			{
				Callback<ReturnValue<KfCall._p21>> callback = KfCall.TcpClient.GetCallback<KFCallMsg, KfCall._p21>(_onReturn_);
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						return sender.GetKeep<KfCall._p21>(TcpCall.KFServiceBase._ac14, ref callback);
					}
				}
				finally
				{
					if (callback != null)
					{
						ReturnValue<KfCall._p21> returnValue = new ReturnValue<KfCall._p21>
						{
							Type = 11
						};
						callback.Call(ref returnValue);
					}
				}
				return null;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
			{
				KfCall._p22 p = new KfCall._p22
				{
					p1 = serverId,
					p0 = mapClientCountDict
				};
				KfCall.TcpClient.Sender.CallOnly<KfCall._p22>(TcpCall.KFServiceBase._c15, ref p);
			}

			private static readonly CommandInfo _c12 = new CommandInfo
			{
				Command = 139,
				InputParameterIndex = 19,
				TaskType = 0,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _c13 = new CommandInfo
			{
				Command = 140,
				InputParameterIndex = 20,
				TaskType = 0,
				IsVerifyMethod = true,
				IsSimpleSerializeOutputParamter = true
			};

			private static readonly CommandInfo _ac14 = new CommandInfo
			{
				Command = 141,
				InputParameterIndex = 0,
				TaskType = 2,
				IsKeepCallback = 1
			};

			private static readonly CommandInfo _c15 = new CommandInfo
			{
				Command = 142,
				InputParameterIndex = 22,
				IsSendOnly = 1,
				TaskType = 0
			};
		}

		public class TestS2KFCommunication
		{
			public static ReturnValue<string> SendData(int strLen, bool flag)
			{
				AutoWaitReturnValue<KfCall._p31> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p31>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p30 p = new KfCall._p30
						{
							p1 = strLen,
							p0 = flag
						};
						KfCall._p31 p2 = default(KfCall._p31);
						ReturnType type = sender.WaitGet<KfCall._p30, KfCall._p31>(TcpCall.TestS2KFCommunication._c23, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<string>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p31>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<string>
				{
					Type = 11
				};
			}

			private static readonly CommandInfo _c23 = new CommandInfo
			{
				Command = 150,
				InputParameterIndex = 30,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true,
				IsSimpleSerializeOutputParamter = true
			};
		}

		public class ZhanDuiZhengBa_K
		{
			public static ReturnValue<ZhanDuiZhengBaSyncData> SyncZhengBaData(ZhanDuiZhengBaSyncData lastSyncData)
			{
				AutoWaitReturnValue<KfCall._p24> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p24>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p23 p = new KfCall._p23
						{
							p0 = lastSyncData
						};
						KfCall._p24 p2 = default(KfCall._p24);
						ReturnType type = sender.WaitGet<KfCall._p23, KfCall._p24>(TcpCall.ZhanDuiZhengBa_K._c16, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<ZhanDuiZhengBaSyncData>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p24>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<ZhanDuiZhengBaSyncData>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> ZhengBaKuaFuLogin(int zhanDuiID, int gameId, int srcServerID, out ZhanDuiZhengBaFuBenData copyData)
			{
				AutoWaitReturnValue<KfCall._p26> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p26>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p25 p = new KfCall._p25
						{
							p1 = zhanDuiID,
							p2 = gameId,
							p3 = srcServerID
						};
						KfCall._p26 p2 = default(KfCall._p26);
						ReturnType type = sender.WaitGet<KfCall._p25, KfCall._p26>(TcpCall.ZhanDuiZhengBa_K._c17, ref autoWaitReturnValue, ref p, ref p2);
						copyData = p2.p0;
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p26>.PushNotNull(autoWaitReturnValue);
					}
				}
				copyData = null;
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			public static ReturnValue<List<ZhanDuiZhengBaNtfPkResultData>> ZhengBaPkResult(int gameId, int winner1)
			{
				AutoWaitReturnValue<KfCall._p28> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p28>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p27 p = new KfCall._p27
						{
							p0 = gameId,
							p1 = winner1
						};
						KfCall._p28 p2 = default(KfCall._p28);
						ReturnType type = sender.WaitGet<KfCall._p27, KfCall._p28>(TcpCall.ZhanDuiZhengBa_K._c18, ref autoWaitReturnValue, ref p, ref p2);
						return new ReturnValue<List<ZhanDuiZhengBaNtfPkResultData>>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p28>.PushNotNull(autoWaitReturnValue);
					}
				}
				return new ReturnValue<List<ZhanDuiZhengBaNtfPkResultData>>
				{
					Type = 11
				};
			}

			public static ReturnValue<int> ZhengBaRequestEnter(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
			{
				AutoWaitReturnValue<KfCall._p8> autoWaitReturnValue = AutoWaitReturnValue<KfCall._p8>.Pop();
				try
				{
					ClientSocketSender sender = KfCall.TcpClient.Sender;
					if (sender != null)
					{
						KfCall._p7 p = new KfCall._p7
						{
							p0 = zhanDuiID
						};
						KfCall._p8 p2 = default(KfCall._p8);
						ReturnType type = sender.WaitGet<KfCall._p7, KfCall._p8>(TcpCall.ZhanDuiZhengBa_K._c19, ref autoWaitReturnValue, ref p, ref p2);
						gameId = p2.p0;
						kuaFuServerID = p2.p1;
						ips = p2.p3;
						ports = p2.p2;
						return new ReturnValue<int>
						{
							Type = type,
							Value = p2.Return
						};
					}
				}
				finally
				{
					if (autoWaitReturnValue != null)
					{
						AutoWaitReturnValue<KfCall._p8>.PushNotNull(autoWaitReturnValue);
					}
				}
				gameId = 0;
				kuaFuServerID = 0;
				ips = null;
				ports = null;
				return new ReturnValue<int>
				{
					Type = 11
				};
			}

			private static readonly CommandInfo _c16 = new CommandInfo
			{
				Command = 143,
				InputParameterIndex = 23,
				TaskType = 0
			};

			private static readonly CommandInfo _c17 = new CommandInfo
			{
				Command = 144,
				InputParameterIndex = 25,
				TaskType = 0
			};

			private static readonly CommandInfo _c18 = new CommandInfo
			{
				Command = 145,
				InputParameterIndex = 27,
				TaskType = 0,
				IsSimpleSerializeInputParamter = true
			};

			private static readonly CommandInfo _c19 = new CommandInfo
			{
				Command = 146,
				InputParameterIndex = 7,
				TaskType = 0
			};
		}
	}
}
