using System;
using System.Collections.Generic;
using GameDBServer.Core;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class UserOnlineManager
	{
		public static int GetUserOnlineState(string userID)
		{
			lock (UserOnlineManager._RegUserIDDict)
			{
				UserOnlineManager.UserOnlineData userOnlineData;
				if (UserOnlineManager._RegUserIDDict.TryGetValue(userID, out userOnlineData) && TimeUtil.NOW() < userOnlineData.MaxActiveTicks)
				{
					return 1;
				}
			}
			return 0;
		}

		public static bool RegisterUserID(string userID, int serverLineID, int state)
		{
			long num = TimeUtil.NOW();
			lock (UserOnlineManager._RegUserIDDict)
			{
				if (state <= 0)
				{
					UserOnlineManager.UserOnlineData userOnlineData;
					if (UserOnlineManager._RegUserIDDict.TryGetValue(userID, out userOnlineData))
					{
						if (userOnlineData.ServerId == serverLineID)
						{
							UserOnlineManager._RegUserIDDict.Remove(userID);
						}
					}
				}
				else
				{
					UserOnlineManager.UserOnlineData userOnlineData;
					if (!UserOnlineManager._RegUserIDDict.TryGetValue(userID, out userOnlineData))
					{
						userOnlineData = new UserOnlineManager.UserOnlineData();
						userOnlineData.ServerId = serverLineID;
						userOnlineData.UserId = userID;
						UserOnlineManager._RegUserIDDict[userID] = userOnlineData;
					}
					if (num < userOnlineData.MaxActiveTicks)
					{
						if (userOnlineData.ServerId != serverLineID)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("账号 {0} 请求注册登录到 {1} 线，但是该账号已经被注册到 {2} 线", userID, serverLineID, userOnlineData.ServerId), null, true);
							return false;
						}
					}
					else if (userOnlineData.ServerId != serverLineID)
					{
						userOnlineData.ServerId = serverLineID;
						LogManager.WriteLog(LogTypes.Error, string.Format("账号 {0} 注册登录到 {1} 线，该账号在{2} 线注册心跳已超时 ", userID, serverLineID, userOnlineData.ServerId), null, true);
					}
					userOnlineData.MaxActiveTicks = num + 600000L;
				}
			}
			return true;
		}

		public static void ClearUserIDsByServerLineID(int serverLineID)
		{
			lock (UserOnlineManager._RegUserIDDict)
			{
				List<string> list = new List<string>();
				foreach (UserOnlineManager.UserOnlineData userOnlineData in UserOnlineManager._RegUserIDDict.Values)
				{
					if (userOnlineData.ServerId == serverLineID)
					{
						list.Add(userOnlineData.UserId);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					UserOnlineManager._RegUserIDDict.Remove(list[i]);
				}
			}
		}

		public const long MaxActiveTicks = 600000L;

		private static Dictionary<string, UserOnlineManager.UserOnlineData> _RegUserIDDict = new Dictionary<string, UserOnlineManager.UserOnlineData>();

		private class UserOnlineData
		{
			public string UserId;

			public int ServerId;

			public long MaxActiveTicks;
		}
	}
}
