using System;
using System.Configuration;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic;
using KF.Contract;
using KF.Remoting;
using KF.Remoting.HuanYingSiYuan.TcpStaticClient;
using Server.Tools;
using Tmsk.Contract.Tools;

namespace KF.TcpCall
{
	public class KFCallManager
	{
		public static bool Start()
		{
			lock (KFCallManager.Mutex)
			{
				KFCallManager.KFClient current = KFCallManager.Current;
				string text = null;
				int num = 0;
				string text2 = ConfigurationManager.AppSettings.Get("KFService");
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(new char[]
					{
						':'
					});
					if (array.Length == 2 && int.TryParse(array[1], out num) && num > 0 && num < 65535)
					{
						text = array[0];
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					if (current != null)
					{
						if (KFCallManager.Host != text || KFCallManager.Port != num)
						{
							current.OnSetSocket = null;
							KFCallManager.NewKFClient(text, num);
							current.Dispose();
						}
					}
					else
					{
						KFCallManager.NewKFClient(text, num);
					}
				}
				else if (null != current)
				{
					current.OnSetSocket = null;
					current.Dispose();
				}
			}
			return true;
		}

		private static void NewKFClient(string host, int port)
		{
			ServerAttribute serverAttribute = new ServerAttribute();
			serverAttribute.IsAutoClient = true;
			serverAttribute.Host = host;
			serverAttribute.Port = port;
			serverAttribute.ServerSendBufferMaxSize = 33554432;
			serverAttribute.ClientSendBufferMaxSize = 4194304;
			KuaFuClientContext kuaFuClientContext = new KuaFuClientContext();
			kuaFuClientContext.ServerId = GameManager.ServerId;
			kuaFuClientContext.Token = GameCoreInterface.getinstance().GetLocalAddressIPs();
			KFCallManager.Host = host;
			KFCallManager.Port = port;
			KFCallManager.ClientInfo = kuaFuClientContext;
			Client client = KfCall.NewTcpClient(serverAttribute, null, MyLogAdapter.GetILog(), new Func<ClientSocketSender, bool>(KFCallManager.verifyMethod));
			KFCallManager.Current = new KFCallManager.KFClient(client, serverAttribute, kuaFuClientContext, new Action<KFCallManager.KFClient>(KFCallManager.OnSetSocket));
			client.TryCreateSocket();
		}

		private static bool verifyMethod(ClientSocketSender sender)
		{
			ReturnValue<bool> returnValue = TcpCall.KFServiceBase.InitializeClient(sender, KFCallManager.ClientInfo);
			bool result;
			if (returnValue.Type == 7 && returnValue.Value)
			{
				KFCallManager.ClientInfo.ClientId = TimeUtil.AgeByUnixTime(KFCallManager.ClientInfo.ClientId);
				LogManager.WriteLog(17, string.Format("连接中心成功,host={0},port={1},error={2}", KFCallManager.Host, KFCallManager.Port, returnValue.Type.ToString()), null, true);
				result = (KFCallManager.IsLogin = true);
			}
			else
			{
				LogManager.WriteLog(17, string.Format("连接中心失败,host={0},port={1},error={2}", KFCallManager.Host, KFCallManager.Port, returnValue.Type.ToString()), null, true);
				result = (KFCallManager.IsLogin = false);
			}
			return result;
		}

		private static void OnSetSocket(KFCallManager.KFClient client)
		{
			client.KeepGetMessage = TcpCall.KFServiceBase.KeepGetMessage(new Action<ReturnValue<KFCallMsg>>(KFCallManager.KFCallMsgFunc));
		}

		public static void KFCallMsgFunc(ReturnValue<KFCallMsg> msg)
		{
			if (msg.Type == 7)
			{
				KFCallManager.MsgSource.fireEvent(msg.Value.KuaFuEventType, msg.Value);
			}
		}

		private static object Mutex = new object();

		public static EventSourceEx<KFCallMsg> MsgSource = new EventSourceEx<KFCallMsg>();

		public static bool IsLogin;

		public static string Host;

		public static int Port;

		public static KuaFuClientContext ClientInfo;

		private static KFCallManager.KFClient Current;

		private class KFClient : IDisposable
		{
			public KFClient(Client client, ServerAttribute attribute, KuaFuClientContext clientInfo, Action<KFCallManager.KFClient> onSetSocket)
			{
				this._Attribute = attribute;
				this.OnSetSocket = onSetSocket;
				this.tcpClient = client;
				this.tcpClient.OnSetSocket(delegate(ClientSocketBase socket)
				{
					if (socket.IsSocketVersion(ref this._Socket))
					{
						this.OnSetSocket(this);
					}
				});
			}

			~KFClient()
			{
				this.Dispose();
			}

			public void Dispose()
			{
				if (!this.IsDisposed)
				{
					try
					{
						this.IsDisposed = true;
						GC.SuppressFinalize(this);
						if (null != this.tcpClient)
						{
							this.tcpClient.Dispose();
							this.tcpClient = null;
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}

			private bool IsDisposed;

			public ServerAttribute _Attribute;

			private Client tcpClient;

			private ClientSocketBase _Socket;

			public Action<KFCallManager.KFClient> OnSetSocket;

			public KeepCallback KeepGetMessage;
		}
	}
}
