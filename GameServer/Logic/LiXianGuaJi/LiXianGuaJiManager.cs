using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.LiXianGuaJi
{
	public class LiXianGuaJiManager : ScheduleTask, IManager
	{
		public TaskInternalLock InternalLock
		{
			get
			{
				return this._InternalLock;
			}
		}

		public static LiXianGuaJiManager getInstance()
		{
			return LiXianGuaJiManager._Instance;
		}

		public bool initialize()
		{
			ScheduleExecutor2.Instance.scheduleExecute(this, 0, 30000);
			return true;
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			ScheduleExecutor2.Instance.scheduleCancle(this);
			LiXianGuaJiManager.SaveGuaJiTimeForAll();
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void run()
		{
			this.ProcessQueue();
		}

		public static List<LiXianGuaJiRoleItem> GetLiXianGuaJiRoleItemList()
		{
			List<LiXianGuaJiRoleItem> result;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				result = LiXianGuaJiManager._LiXianRoleInfoDict.Values.ToList<LiXianGuaJiRoleItem>();
			}
			return result;
		}

		public void ProcessQueue()
		{
			long num = TimeUtil.NOW();
			List<LiXianGuaJiRoleItem> liXianGuaJiRoleItemList = LiXianGuaJiManager.GetLiXianGuaJiRoleItemList();
			for (int i = 0; i < liXianGuaJiRoleItemList.Count; i++)
			{
				this.DoSpriteMeditateTime(liXianGuaJiRoleItemList[i]);
				if (liXianGuaJiRoleItemList[i].MeditateTime + liXianGuaJiRoleItemList[i].NotSafeMeditateTime >= 43200000)
				{
					LiXianGuaJiManager.SaveDBLiXianGuaJiTimeForRole(liXianGuaJiRoleItemList[i]);
					LiXianGuaJiManager.RemoveLiXianGuaJiRole(liXianGuaJiRoleItemList[i].RoleID);
					if (liXianGuaJiRoleItemList[i].FakeRoleID > 0)
					{
						FakeRoleManager.ProcessDelFakeRole(liXianGuaJiRoleItemList[i].FakeRoleID, false);
					}
				}
			}
		}

		private void DoSpriteMeditateTime(LiXianGuaJiRoleItem c)
		{
			long num = TimeUtil.NOW();
			long num2 = num - c.MeditateTicks;
			if (num2 >= 60000L)
			{
				c.MeditateTicks = num;
				bool flag = false;
				Point currentGrid = c.CurrentGrid;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(c.MapCode, out gameMap))
				{
					flag = gameMap.InSafeRegionList(currentGrid);
				}
				if (flag)
				{
					int meditateTime = c.MeditateTime;
					int notSafeMeditateTime = c.NotSafeMeditateTime;
					if (meditateTime + notSafeMeditateTime < 43200000)
					{
						long num3 = Math.Max(num - c.BiGuanTime, 0L);
						num3 = Math.Min(num3 + (long)meditateTime, (long)(43200000 - notSafeMeditateTime));
						c.MeditateTime = (int)num3;
					}
				}
				else
				{
					int meditateTime = c.MeditateTime;
					int notSafeMeditateTime = c.NotSafeMeditateTime;
					if (meditateTime + notSafeMeditateTime < 43200000)
					{
						long num3 = Math.Max(num - c.BiGuanTime, 0L);
						num3 = Math.Min(num3 + (long)notSafeMeditateTime, (long)(43200000 - meditateTime));
						c.NotSafeMeditateTime = (int)num3;
					}
				}
				c.BiGuanTime = num;
			}
		}

		public static void AddLiXianGuaJiRole(GameClient client, int fakeRoleID)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiManager._LiXianRoleInfoDict[client.ClientData.RoleID] = new LiXianGuaJiRoleItem
				{
					ZoneID = client.ClientData.ZoneID,
					UserID = userID,
					RoleID = client.ClientData.RoleID,
					RoleName = client.ClientData.RoleName,
					RoleLevel = client.ClientData.Level,
					CurrentGrid = client.CurrentGrid,
					StartTicks = TimeUtil.NOW(),
					FakeRoleID = fakeRoleID,
					MeditateTime = client.ClientData.MeditateTime,
					NotSafeMeditateTime = client.ClientData.NotSafeMeditateTime,
					MapCode = client.ClientData.MapCode
				};
			}
		}

		public static void RemoveLiXianGuaJiRole(GameClient client)
		{
			LiXianGuaJiManager.RemoveLiXianGuaJiRole(client.ClientData.RoleID);
		}

		public static void RemoveLiXianGuaJiRole(int roleID)
		{
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				LiXianGuaJiManager._LiXianRoleInfoDict.Remove(roleID);
			}
		}

		public static void GetBackLiXianGuaJiTime(GameClient client)
		{
			LiXianGuaJiRoleItem liXianGuaJiRoleItem = null;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				if (LiXianGuaJiManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianGuaJiRoleItem))
				{
					client.ClientData.MeditateTime = liXianGuaJiRoleItem.MeditateTime;
					client.ClientData.NotSafeMeditateTime = liXianGuaJiRoleItem.NotSafeMeditateTime;
					Global.SaveRoleParamsInt32ValueToDB(client, "MeditateTime", client.ClientData.MeditateTime, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "NotSafeMeditateTime", client.ClientData.NotSafeMeditateTime, true);
				}
			}
		}

		public static bool DelFakeRoleByClient(GameClient client)
		{
			int num = -1;
			LiXianGuaJiRoleItem liXianGuaJiRoleItem = null;
			lock (LiXianGuaJiManager._LiXianRoleInfoDict)
			{
				if (!LiXianGuaJiManager._LiXianRoleInfoDict.TryGetValue(client.ClientData.RoleID, out liXianGuaJiRoleItem))
				{
					return false;
				}
				num = liXianGuaJiRoleItem.FakeRoleID;
			}
			if (num > 0)
			{
				FakeRoleManager.ProcessDelFakeRole(num, false);
			}
			return true;
		}

		public static void SaveGuaJiTimeForAll()
		{
			long num = TimeUtil.NOW();
			List<LiXianGuaJiRoleItem> liXianGuaJiRoleItemList = LiXianGuaJiManager.GetLiXianGuaJiRoleItemList();
			for (int i = 0; i < liXianGuaJiRoleItemList.Count; i++)
			{
				LiXianGuaJiManager.SaveDBLiXianGuaJiTimeForRole(liXianGuaJiRoleItemList[i]);
			}
		}

		public static void SaveDBLiXianGuaJiTimeForRole(LiXianGuaJiRoleItem liXianGuaJiRoleItem)
		{
			GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", liXianGuaJiRoleItem.RoleID, "MeditateTime", liXianGuaJiRoleItem.MeditateTime), null, 0);
			GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", liXianGuaJiRoleItem.RoleID, "NotSafeMeditateTime", liXianGuaJiRoleItem.NotSafeMeditateTime), null, 0);
		}

		public const int MaxMingXiangTicks = 43200000;

		private TaskInternalLock _InternalLock = new TaskInternalLock();

		private static LiXianGuaJiManager _Instance = new LiXianGuaJiManager();

		private static Dictionary<int, LiXianGuaJiRoleItem> _LiXianRoleInfoDict = new Dictionary<int, LiXianGuaJiRoleItem>();
	}
}
