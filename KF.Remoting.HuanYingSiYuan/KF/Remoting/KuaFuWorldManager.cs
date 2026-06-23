using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class KuaFuWorldManager : TSingleton<KuaFuWorldManager>, IKuaFuWorld
	{
		public KuaFuWorldManager()
		{
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		public void LoadConfig(bool reload = false)
		{
			try
			{
				this.ConfigLoadFinished = false;
			}
			catch (Exception ex)
			{
			}
			finally
			{
				this.ConfigLoadFinished = true;
			}
		}

		public void ThreadProc(object state)
		{
			int num = 0;
			for (;;)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				try
				{
					if (this.ConfigLoadFinished)
					{
						RebornService.Instance().Update(dateTime);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
				finally
				{
					num = (int)(TimeUtil.NowDateTime() - dateTime).TotalMilliseconds;
				}
				Thread.Sleep(Math.Max(50, 1000 - num));
			}
		}

		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool flag = false;
					int num = ClientAgentManager.Instance().InitializeClient(clientInfo, out flag);
					if (num > 0)
					{
						if (clientInfo.MapClientCountDict != null && clientInfo.MapClientCountDict.Count > 0)
						{
							KuaFuServerManager.UpdateKuaFuLineData(clientInfo.ServerId, clientInfo.MapClientCountDict);
							ClientAgentManager.Instance().SetMainlinePayload(clientInfo.ServerId, clientInfo.MapClientCountDict.Values.ToList<int>().Sum());
						}
					}
					result = num;
				}
				else
				{
					LogManager.WriteLog(2, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
				result = -11003;
			}
			return result;
		}

		public void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			if (mapClientCountDict != null && mapClientCountDict.Count > 0)
			{
				ClientAgent currentClientAgent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
				if (null != currentClientAgent)
				{
					KuaFuServerManager.UpdateKuaFuLineData(currentClientAgent.ClientInfo.ServerId, mapClientCountDict);
					ClientAgentManager.Instance().SetMainlinePayload(currentClientAgent.ClientInfo.ServerId, mapClientCountDict.Values.ToList<int>().Sum());
				}
			}
		}

		public int ExecuteCommand(string cmd)
		{
			int result;
			if (string.IsNullOrEmpty(cmd))
			{
				result = -18;
			}
			else
			{
				string[] args = cmd.Split(KuaFuWorldManager.WriteSpaceChars, StringSplitOptions.RemoveEmptyEntries);
				result = this.ExecCommand(args);
			}
			return result;
		}

		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return new AsyncDataItem(9998, new object[]
			{
				KuaFuServerManager.GetKuaFuLineDataList(mapCode)
			});
		}

		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		public int RegPTKuaFuRoleData(ref KuaFuWorldRoleData data)
		{
			data.WorldRoleID = ConstData.FormatWorldRoleID(data.LocalRoleID, data.PTID);
			KuaFuWorldRoleData kuaFuWorldRoleData = this.LoadKuaFuWorldRoleData(data.LocalRoleID, data.PTID, data.WorldRoleID);
			int result;
			if (null != kuaFuWorldRoleData)
			{
				if (data.RoleData != null)
				{
					kuaFuWorldRoleData.RoleData = data.RoleData;
					int num = YongZheZhanChangPersistence.Instance.WriteKuaFuWorldRoleData(kuaFuWorldRoleData);
					if (num < 0)
					{
						return num;
					}
				}
				data = kuaFuWorldRoleData;
				result = 0;
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					int num2 = 0;
					int kuaFoWorldMaxTempRoleID = YongZheZhanChangPersistence.Instance.GetKuaFoWorldMaxTempRoleID(out num2);
					if (kuaFoWorldMaxTempRoleID >= num2)
					{
						return -22;
					}
					kuaFuWorldRoleData = YongZheZhanChangPersistence.Instance.InsertKuaFuWorldRoleData(data, kuaFoWorldMaxTempRoleID + 1);
					if (null != kuaFuWorldRoleData)
					{
						lock (this.Mutex)
						{
							KuaFuWorldRoleData kuaFuWorldRoleData2;
							if (!this.RoleDataDict.TryGetValue(data.WorldRoleID, out kuaFuWorldRoleData2) || kuaFuWorldRoleData2 == null)
							{
								this.RoleDataDict[data.WorldRoleID] = kuaFuWorldRoleData;
							}
						}
						break;
					}
					Thread.Sleep(500);
				}
				if (null != kuaFuWorldRoleData)
				{
					data = kuaFuWorldRoleData;
					result = kuaFuWorldRoleData.TempRoleID;
				}
				else
				{
					result = -15;
				}
			}
			return result;
		}

		public KuaFuWorldRoleData LoadKuaFuWorldRoleData(int roleId, int ptid, string worldRoleID)
		{
			KuaFuWorldRoleData kuaFuWorldRoleData;
			lock (this.Mutex)
			{
				if (this.RoleDataDict.TryGetValue(worldRoleID, out kuaFuWorldRoleData) && kuaFuWorldRoleData != null)
				{
					return kuaFuWorldRoleData;
				}
			}
			kuaFuWorldRoleData = YongZheZhanChangPersistence.Instance.QueryKuaFuWorldRoleData(roleId, ptid);
			lock (this.Mutex)
			{
				KuaFuWorldRoleData kuaFuWorldRoleData2;
				if (!this.RoleDataDict.TryGetValue(worldRoleID, out kuaFuWorldRoleData2) || kuaFuWorldRoleData2 == null)
				{
					this.RoleDataDict[worldRoleID] = kuaFuWorldRoleData;
				}
			}
			return kuaFuWorldRoleData;
		}

		public int EnterPTKuaFuMap(int serverID, int roleId, int ptid, int mapCode, int kuaFuLine, out string signToken, out string signKey, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			ips = null;
			ports = null;
			signToken = null;
			signKey = null;
			kuaFuServerID = 0;
			string text = ConstData.FormatWorldRoleID(roleId, ptid);
			KuaFuWorldRoleData kuaFuWorldRoleData = this.LoadKuaFuWorldRoleData(roleId, ptid, text);
			int result;
			if (null == kuaFuWorldRoleData)
			{
				result = -4010;
			}
			else
			{
				kuaFuServerID = KuaFuServerManager.EnterKuaFuMapLine(kuaFuLine, mapCode);
				if (kuaFuServerID <= 0)
				{
					result = -100;
				}
				else
				{
					KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerID);
					if (null != kuaFuServerInfo)
					{
						ips = new string[]
						{
							kuaFuServerInfo.Ip
						};
						ports = new int[]
						{
							kuaFuServerInfo.Port
						};
					}
					signToken = Guid.NewGuid().ToString("N");
					signKey = Guid.NewGuid().ToString("N");
					long num = TimeUtil.UTCTicks();
					lock (this.Mutex)
					{
						KuaFuServerLoginData kuaFuServerLoginData;
						if (!this.WorldRoleIDDict.TryGetValue(text, out kuaFuServerLoginData))
						{
							kuaFuServerLoginData = new KuaFuServerLoginData();
							kuaFuServerLoginData.TempRoleID = kuaFuWorldRoleData.TempRoleID;
							this.WorldRoleIDDict[text] = kuaFuServerLoginData;
						}
						kuaFuServerLoginData.SignKey = signKey;
						kuaFuServerLoginData.SignToken = signToken;
						kuaFuServerLoginData.EndTicks = num + 86400000L;
						kuaFuServerLoginData.TargetServerID = kuaFuServerID;
						kuaFuServerLoginData.ServerId = ConstData.ConvertToKuaFuServerID(serverID, ptid);
						kuaFuServerLoginData.RoleId = roleId;
						kuaFuServerLoginData.PTID = ptid;
						kuaFuServerLoginData.GameId = (long)mapCode;
						result = kuaFuServerLoginData.TempRoleID;
					}
				}
			}
			return result;
		}

		public int CheckEnterWorldKuaFuSign(string worldRoleID, string token, out string signKey, out string[] ips, out int[] ports)
		{
			int result = -100;
			signKey = null;
			ips = null;
			ports = null;
			long num = TimeUtil.UTCTicks();
			lock (this.Mutex)
			{
				KuaFuServerLoginData kuaFuServerLoginData;
				if (this.WorldRoleIDDict.TryGetValue(worldRoleID, out kuaFuServerLoginData) && token == kuaFuServerLoginData.SignToken)
				{
					if (num > kuaFuServerLoginData.EndTicks)
					{
						return result;
					}
					result = 0;
					signKey = kuaFuServerLoginData.SignKey;
				}
				KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerLoginData.ServerId);
				if (null != kuaFuServerInfo)
				{
					ips = new string[]
					{
						kuaFuServerInfo.DbIp,
						kuaFuServerInfo.DbIp
					};
					ports = new int[]
					{
						kuaFuServerInfo.DbPort,
						kuaFuServerInfo.LogDbPort
					};
				}
			}
			return result;
		}

		public void Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			RebornService.Instance().SetRoleData4Selector(ptId, roleId, bytes);
		}

		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			return RebornService.Instance().RoleReborn(ptId, roleId, roleName, level);
		}

		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			return RebornService.Instance().Reborn_SyncData(ageRank, ageBoss);
		}

		public void Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			RebornService.Instance().RebornOpt(ptid, rid, optType, param1, param2, param3);
		}

		public KuaFuCmdData Reborn_GetRebornRoleData(int ptId, int roleId, long dataAge)
		{
			return RebornService.Instance().GetRebornRoleData(ptId, roleId, dataAge);
		}

		public void Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			RebornService.Instance().ChangeName(ptId, roleId, roleName);
		}

		public void Reborn_PlatFormChat(int serverId, byte[] bytes)
		{
			RebornService.Instance().PlatFormChat(serverId, bytes);
		}

		private void Broadcast2GsAgent(AsyncDataItem item)
		{
			ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, item, 0);
		}

		public int ExecCommand(string[] args)
		{
			int result = 0;
			try
			{
				if (0 == string.Compare(args[0], "-settime", true))
				{
					if (KuaFuServerManager.EnableGMSetAllServerTime && args.Length >= 3)
					{
						string text = args[2];
						if (args.Length > 3)
						{
							text = text + " " + args[3];
						}
						ThreadPool.QueueUserWorkItem(delegate(object x)
						{
							Thread.Sleep(10000);
							string text2 = x as string;
							if (!string.IsNullOrEmpty(text2))
							{
								TimeUtil.SetTime(text2);
								LogManager.WriteLog(2, string.Format("GM命令修改时间#from={0},to={1}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), text2), null, true);
							}
						}, text);
						this.Broadcast2GsAgent(new AsyncDataItem(9997, args));
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		private const string PTKuaFuManager = "AliasName";

		private object Mutex = new object();

		private bool ConfigLoadFinished;

		public GameTypes GameType = 32;

		private Dictionary<string, KuaFuWorldRoleData> RoleDataDict = new Dictionary<string, KuaFuWorldRoleData>();

		private Dictionary<string, KuaFuServerLoginData> WorldRoleIDDict = new Dictionary<string, KuaFuServerLoginData>();

		private Dictionary<string, KuaFuServerLoginData> RandomTokenDict = new Dictionary<string, KuaFuServerLoginData>();

		private Thread BackgroundThread;

		public static char[] WriteSpaceChars = new char[]
		{
			' '
		};
	}
}
