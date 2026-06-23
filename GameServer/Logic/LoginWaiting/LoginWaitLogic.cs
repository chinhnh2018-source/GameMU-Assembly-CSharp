using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Logic.Name;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.LoginWaiting
{
	public class LoginWaitLogic
	{
		public void LoadConfig()
		{
			string gameConfigItemStr = GameManager.PlatConfigMgr.GetGameConfigItemStr("userwaitconfig", "700,1000,400,30000,180000,20000");
			this.m_IntConfig[0] = Global.String2IntArray(gameConfigItemStr, ',');
			gameConfigItemStr = GameManager.PlatConfigMgr.GetGameConfigItemStr("vipwaitconfig", "900,1000,400,30000,180000,20000");
			this.m_IntConfig[1] = Global.String2IntArray(gameConfigItemStr, ',');
		}

		public int GetConfig(LoginWaitLogic.UserType userType, LoginWaitLogic.ConfigType type)
		{
			int result;
			if (userType < LoginWaitLogic.UserType.Normal || userType >= LoginWaitLogic.UserType.Max_Type)
			{
				result = 0;
			}
			else if (type < LoginWaitLogic.ConfigType.NeedWaitNum || type >= (LoginWaitLogic.ConfigType)this.m_IntConfig[(int)userType].Length)
			{
				result = 0;
			}
			else
			{
				result = this.m_IntConfig[(int)userType][(int)type];
			}
			return result;
		}

		public LoginWaitLogic()
		{
			for (LoginWaitLogic.UserType userType = LoginWaitLogic.UserType.Normal; userType < LoginWaitLogic.UserType.Max_Type; userType++)
			{
				this.m_UserList[(int)userType] = new List<LoginWaitLogic.UserInfo>();
			}
		}

		public int GetTotalWaitingCount()
		{
			int result;
			lock (this.m_Mutex)
			{
				int num = 0;
				foreach (List<LoginWaitLogic.UserInfo> list in this.m_UserList)
				{
					num += list.Count;
				}
				result = num;
			}
			return result;
		}

		public int GetWaitingCount(LoginWaitLogic.UserType userType)
		{
			int count;
			lock (this.m_Mutex)
			{
				count = this.m_UserList[(int)userType].Count;
			}
			return count;
		}

		public bool IsInWait(string userID)
		{
			bool result;
			try
			{
				if (string.IsNullOrEmpty(userID))
				{
					result = false;
				}
				else
				{
					lock (this.m_Mutex)
					{
						result = this.m_User2SocketDict.ContainsKey(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::IsInWait userID={0}", userID));
				result = false;
			}
			return result;
		}

		public bool AddToWait(string userID, int zoneID, LoginWaitLogic.UserType userType, TMSKSocket socket)
		{
			try
			{
				lock (this.m_Mutex)
				{
					if (this.IsInWait(userID))
					{
						return false;
					}
					if (this.GetWaitingCount(userType) >= this.GetConfig(userType, LoginWaitLogic.ConfigType.MaxQueueNum))
					{
						return false;
					}
					this.m_UserList[(int)userType].Add(new LoginWaitLogic.UserInfo
					{
						userID = userID,
						zoneID = zoneID,
						socket = socket,
						startTick = TimeUtil.NOW(),
						updateTick = 0L
					});
					this.m_User2SocketDict.Add(userID, socket);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::AddToWait userID={0}", userID));
				return false;
			}
			return true;
		}

		public void RemoveWait(string userID)
		{
			try
			{
				lock (this.m_Mutex)
				{
					if (this.IsInWait(userID))
					{
						foreach (List<LoginWaitLogic.UserInfo> list in this.m_UserList)
						{
							list.RemoveAll((LoginWaitLogic.UserInfo x) => x.userID == userID);
						}
						this.m_User2SocketDict.Remove(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::RemoveWait userID={0}", userID));
			}
		}

		public LoginWaitLogic.UserInfo TopWaiting(LoginWaitLogic.UserType userType)
		{
			LoginWaitLogic.UserInfo result = null;
			try
			{
				lock (this.m_Mutex)
				{
					if (this.GetWaitingCount(userType) <= 0)
					{
						return null;
					}
					result = this.m_UserList[(int)userType][0];
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::TopWaiting", new object[0]));
			}
			return result;
		}

		public LoginWaitLogic.UserInfo PopTopWaiting(LoginWaitLogic.UserType userType)
		{
			LoginWaitLogic.UserInfo userInfo = null;
			try
			{
				lock (this.m_Mutex)
				{
					if (this.GetWaitingCount(userType) <= 0)
					{
						return null;
					}
					userInfo = this.m_UserList[(int)userType][0];
					this.m_UserList[(int)userType].RemoveAt(0);
					this.m_User2SocketDict.Remove(userInfo.userID);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::PopTopWaiting", new object[0]));
			}
			return userInfo;
		}

		public void OutWaitInfo(LoginWaitLogic.UserType userType, int index)
		{
			try
			{
				lock (this.m_Mutex)
				{
					if (index < 0 || index >= this.GetWaitingCount(userType))
					{
						LogManager.WriteLog(2, string.Format("OutWaitInfo Index Was Outside ", new object[0]), null, true);
					}
					else
					{
						LoginWaitLogic.UserInfo userInfo = this.m_UserList[(int)userType][index];
						LogManager.WriteLog(2, string.Format("OutWaitInfo:userID={0} zoneID={1} startTick={2} updateTick={3} firstTick={4} overTick={5}", new object[]
						{
							userInfo.userID,
							userInfo.zoneID,
							userInfo.startTick,
							userInfo.updateTick,
							userInfo.firstTick,
							userInfo.overTick
						}), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::PopTopWaiting", new object[0]));
			}
		}

		public int GetAllowCount()
		{
			int count;
			lock (this.m_AllowUserDict)
			{
				count = this.m_AllowUserDict.Count;
			}
			return count;
		}

		public bool AddToAllow(string userID, int mSeconds)
		{
			try
			{
				if (string.IsNullOrEmpty(userID))
				{
					return false;
				}
				lock (this.m_AllowUserDict)
				{
					if (this.IsInAllowDict(userID))
					{
						this.m_AllowUserDict[userID] = TimeUtil.NOW() + (long)mSeconds;
						return true;
					}
					if (this.GetAllowCount() < this.GetConfig(LoginWaitLogic.UserType.Normal, LoginWaitLogic.ConfigType.MaxServerNum))
					{
						this.m_AllowUserDict[userID] = TimeUtil.NOW() + (long)mSeconds;
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::AddToAllow userID={0}", userID));
				return false;
			}
			return false;
		}

		public void RemoveAllow(string userID)
		{
			try
			{
				if (!string.IsNullOrEmpty(userID))
				{
					lock (this.m_AllowUserDict)
					{
						this.m_AllowUserDict.Remove(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::RemoveAllow userID={0}", userID));
			}
		}

		public bool IsInAllowDict(string userID)
		{
			bool result;
			try
			{
				if (string.IsNullOrEmpty(userID))
				{
					LogManager.WriteLog(2, string.Format("LoginWaitLogic::IsInAllowDict userID={0}", (userID == null) ? "null " : userID), null, true);
					result = false;
				}
				else
				{
					lock (this.m_AllowUserDict)
					{
						result = this.m_AllowUserDict.ContainsKey(userID);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::IsInAllowDict userID={0}", userID));
				result = false;
			}
			return result;
		}

		public void NotifyWaitingInfo(LoginWaitLogic.UserInfo userInfo, int count, long seconds)
		{
			try
			{
				if (null != userInfo)
				{
					if (userInfo.socket != null && userInfo.socket.Connected)
					{
						string data = string.Format("{0}:{1}", count, seconds);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 971);
						Global._TCPManager.MySocketListener.SendData(userInfo.socket, tcpOutPacket, true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyWaitingInfo userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
			}
		}

		public bool NotifyUserEnter(LoginWaitLogic.UserInfo userInfo)
		{
			try
			{
				if (null == userInfo)
				{
					return true;
				}
				if (userInfo.socket == null || !userInfo.socket.Connected)
				{
					return true;
				}
				this.AddToAllow(userInfo.userID, this.GetConfig(LoginWaitLogic.UserType.Normal, LoginWaitLogic.ConfigType.AllowMSeconds));
				if (!userInfo.socket.IsKuaFuLogin)
				{
					ChangeNameInfo changeNameInfo = SingletonTemplate<NameManager>.Instance().GetChangeNameInfo(userInfo.userID, userInfo.zoneID, userInfo.socket.ServerId);
					if (changeNameInfo != null)
					{
						Global._TCPManager.MySocketListener.SendData(userInfo.socket, DataHelper.ObjectToTCPOutPacket<ChangeNameInfo>(changeNameInfo, Global._TCPManager.TcpOutPacketPool, 14002), true);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyUserEnter userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
				return false;
			}
			string data = "";
			try
			{
				string data2 = string.Format("{0}:{1}", userInfo.userID, userInfo.zoneID);
				byte[] array = Global.SendAndRecvData<string>(101, data2, userInfo.socket.ServerId, 0);
				int num = BitConverter.ToInt32(array, 0);
				data = new UTF8Encoding().GetString(array, 6, num - 2);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyUserEnter 向db请求角色列表 faild！ userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
				data = "-1:";
			}
			try
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 101);
				Global._TCPManager.MySocketListener.SendData(userInfo.socket, tcpOutPacket, true);
				this.m_LastEnterSecs = (TimeUtil.NOW() - userInfo.startTick) / 1000L;
				this.m_LastEnterFromFirstSecs = (TimeUtil.NOW() - userInfo.firstTick) / 1000L;
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::NotifyUserEnter 发送角色列表Faild userID={0} zoneID={1}", userInfo.userID, userInfo.zoneID));
				return false;
			}
			return true;
		}

		public LoginWaitLogic.UserType GetUserType(string userID)
		{
			LoginWaitLogic.UserType result = LoginWaitLogic.UserType.Normal;
			try
			{
				if (VIPEumValue.VIP_MIN_NEED_REALMONEY <= 0)
				{
					int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
					if (gameConfigItemInt > 0)
					{
						VIPEumValue.VIP_MIN_NEED_REALMONEY = VIPEumValue.VIP_MIN_NEED_EXP / gameConfigItemInt;
					}
				}
				if (GameManager.ClientMgr.QueryTotaoChongZhiMoney(userID, -1, -1) >= VIPEumValue.VIP_MIN_NEED_REALMONEY)
				{
					result = LoginWaitLogic.UserType.Vip;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "LoginWaitLogic::GetUserType Exception!!!");
				return result;
			}
			return result;
		}

		public int GetUserCount()
		{
			return GameManager.ClientMgr.GetClientCount() + this.GetAllowCount();
		}

		public void UpdateWaitingList()
		{
			long num = TimeUtil.NOW();
			try
			{
				lock (this.m_Mutex)
				{
					foreach (List<LoginWaitLogic.UserInfo> list in this.m_UserList)
					{
						list.RemoveAll((LoginWaitLogic.UserInfo x) => x.socket == null || !x.socket.Connected);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::ProcessWaitingList 1", new object[0]));
			}
			try
			{
				lock (this.m_AllowUserDict)
				{
					List<string> list2 = new List<string>();
					foreach (KeyValuePair<string, long> keyValuePair in this.m_AllowUserDict)
					{
						if (num > keyValuePair.Value)
						{
							if (null == list2)
							{
								list2 = new List<string>();
							}
							list2.Add(keyValuePair.Key);
						}
					}
					if (list2 != null && list2.Count > 0)
					{
						foreach (string key in list2)
						{
							this.m_AllowUserDict.Remove(key);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::ProcessWaitingList 2", new object[0]));
			}
		}

		public void Tick()
		{
			long num = TimeUtil.NOW();
			if (num - this.m_UpdateAllowTick >= 1000L)
			{
				this.m_UpdateAllowTick = num;
				this.UpdateWaitingList();
				this.ProcessWaitingList(LoginWaitLogic.UserType.Vip);
				this.ProcessWaitingList(LoginWaitLogic.UserType.Normal);
			}
		}

		public void ProcessWaitingList(LoginWaitLogic.UserType userType)
		{
			try
			{
				long num = TimeUtil.NOW();
				if (this.GetWaitingCount(userType) > 0)
				{
					int userCount = this.GetUserCount();
					lock (this.m_Mutex)
					{
						int i = 0;
						long num2 = 0L;
						long num3 = 0L;
						foreach (LoginWaitLogic.UserInfo userInfo in this.m_UserList[(int)userType])
						{
							i++;
							if (1 == i && 0L == userInfo.firstTick)
							{
								userInfo.firstTick = TimeUtil.NOW();
							}
							long num4;
							if (userCount + i <= this.GetConfig(userType, LoginWaitLogic.ConfigType.MaxServerNum))
							{
								if (0L == userInfo.overTick)
								{
									userInfo.overTick = ((0L == num2) ? num : num2) + (long)this.GetConfig(userType, LoginWaitLogic.ConfigType.WaitUpdateInt);
								}
								num4 = Global.GMax(1L, (userInfo.overTick - num) / 1000L);
								num4 = Global.GMin(num4, (long)(this.GetConfig(userType, LoginWaitLogic.ConfigType.WaitUpdateInt) / 1000 * i));
								num2 = userInfo.overTick;
							}
							else if (1 == i)
							{
								num4 = Global.GMax(this.m_LastEnterFromFirstSecs, (TimeUtil.NOW() - userInfo.firstTick) / 1000L);
								num4 = Global.GMax(1L, num4);
								num3 = num4;
							}
							else
							{
								num4 = (long)i * num3;
							}
							if (num - userInfo.updateTick <= (long)this.m_UserUpdateInt)
							{
								num2 = userInfo.overTick;
							}
							else
							{
								userInfo.updateTick = num;
								this.NotifyWaitingInfo(userInfo, i, num4);
							}
						}
					}
					if (userCount < this.GetConfig(userType, LoginWaitLogic.ConfigType.NeedWaitNum))
					{
						for (int i = 0; i < 5; i++)
						{
							LoginWaitLogic.UserInfo userInfo = this.PopTopWaiting(userType);
							if (null != userInfo)
							{
								this.NotifyUserEnter(userInfo);
							}
						}
					}
					else if (userCount < this.GetConfig(userType, LoginWaitLogic.ConfigType.MaxServerNum))
					{
						LoginWaitLogic.UserInfo userInfo = this.TopWaiting(userType);
						if (null != userInfo)
						{
							if (userInfo.overTick > 0L)
							{
								if (num >= userInfo.overTick)
								{
									userInfo = this.PopTopWaiting(userType);
									this.NotifyUserEnter(userInfo);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("LoginWaitLogic::Tick", new object[0]));
			}
		}

		private int[][] m_IntConfig = new int[2][];

		private int m_UserUpdateInt = 5000;

		private List<LoginWaitLogic.UserInfo>[] m_UserList = new List<LoginWaitLogic.UserInfo>[2];

		private Dictionary<string, TMSKSocket> m_User2SocketDict = new Dictionary<string, TMSKSocket>();

		private object m_Mutex = new object();

		private long m_UpdateTick = 0L;

		private long m_UpdateAllowTick = 0L;

		private long m_LastEnterSecs = 30L;

		private long m_LastEnterFromFirstSecs = 30L;

		private Dictionary<string, long> m_AllowUserDict = new Dictionary<string, long>();

		public enum ConfigType
		{
			NeedWaitNum,
			MaxServerNum,
			MaxQueueNum,
			WaitUpdateInt,
			AllowMSeconds,
			LogouAllowMSeconds
		}

		public enum UserType
		{
			Normal,
			Vip,
			Max_Type
		}

		public class UserInfo
		{
			public TMSKSocket socket = null;

			public string userID = "";

			public int zoneID = 0;

			public long startTick = 0L;

			public long updateTick = 0L;

			public long firstTick = 0L;

			public long overTick = 0L;
		}
	}
}
