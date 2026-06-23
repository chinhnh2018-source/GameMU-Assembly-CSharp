using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using KF.Contract;
using KF.Remoting;
using Server.Tools;

namespace KF.TcpCall
{
	[Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = 240, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public static class KFServiceBase
	{
		public static void TimerProc()
		{
			ClientAgentManager.Instance().SendAsyncKuaFuMsg();
		}

		[Method]
		public static bool InitializeClient(ServerSocketSender socket, KuaFuClientContext clientInfo)
		{
			bool result = false;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool flag = false;
					int num = ClientAgentManager.Instance().InitializeClient(clientInfo, out flag);
					if (num > 0)
					{
						result = true;
						socket.ClientObject = ClientAgentManager.Instance().GetCurrentClientAgent(clientInfo.ServerId);
						if (clientInfo.MapClientCountDict != null && clientInfo.MapClientCountDict.Count > 0)
						{
							KuaFuServerManager.UpdateKuaFuLineData(clientInfo.ServerId, clientInfo.MapClientCountDict);
							ClientAgentManager.Instance().SetMainlinePayload(clientInfo.ServerId, clientInfo.MapClientCountDict.Values.ToList<int>().Sum());
						}
					}
				}
				else
				{
					LogManager.WriteLog(2, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
			}
			return result;
		}

		[KeepCallbackMethod(IsClientAsynchronous = false, IsClientAwaiter = false)]
		public static void KeepGetMessage(ServerSocketSender socket, Func<ReturnValue<KFCallMsg>, bool> onMessage)
		{
			ClientAgent clientAgent = socket.ClientObject as ClientAgent;
			if (null != clientAgent)
			{
				clientAgent.KFCallMsg = onMessage;
			}
		}

		[Method]
		public static int ExecuteCommand(ServerSocketSender socket, string[] args)
		{
			return -11004;
		}

		[Method]
		public static void UpdateKuaFuMapClientCount(ServerSocketSender socket, int serverId, Dictionary<int, int> mapClientCountDict)
		{
			ClientAgent clientAgent = socket.ClientObject as ClientAgent;
			if (mapClientCountDict != null && mapClientCountDict.Count > 0)
			{
				KuaFuServerManager.UpdateKuaFuLineData(clientAgent.ClientInfo.ServerId, mapClientCountDict);
				ClientAgentManager.Instance().SetMainlinePayload(clientAgent.ClientInfo.ServerId, mapClientCountDict.Values.ToList<int>().Sum());
			}
		}

		public const string ClientSegmentationCopyPath = "GameServer\\Remoting\\";

		internal static class TcpStaticServer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M12(ServerSocketSender _sender_, string[] args)
			{
				return KFServiceBase.ExecuteCommand(_sender_, args);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M13(ServerSocketSender _sender_, KuaFuClientContext clientInfo)
			{
				return KFServiceBase.InitializeClient(_sender_, clientInfo);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void _M14(ServerSocketSender _sender_, Func<ReturnValue<KFCallMsg>, bool> _onReturn_)
			{
				KFServiceBase.KeepGetMessage(_sender_, _onReturn_);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static void _M15(ServerSocketSender _sender_, int serverId, Dictionary<int, int> mapClientCountDict)
			{
				KFServiceBase.UpdateKuaFuMapClientCount(_sender_, serverId, mapClientCountDict);
			}
		}
	}
}
