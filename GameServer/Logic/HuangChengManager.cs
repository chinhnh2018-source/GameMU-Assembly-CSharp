using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class HuangChengManager
	{
		public static void LoadHuangDiRoleIDFromDBServer(int bhid)
		{
			if (bhid > 0)
			{
				string[] array = Global.ExecuteDBCmd(10076, string.Format("{0}", bhid), 0);
				if (array != null && array.Length == 3)
				{
					HuangChengManager.HuangDiRoleID = Global.SafeConvertToInt32(array[0]);
					HuangChengManager.HuangDiRoleName = array[1];
					HuangChengManager.HuangDiBHName = array[2];
				}
			}
		}

		public static int GetHuangDiRoleID()
		{
			return HuangChengManager.HuangDiRoleID;
		}

		public static string GetHuangDiRoleName()
		{
			return HuangChengManager.HuangDiRoleName;
		}

		public static string GetHuangDiBHName()
		{
			return HuangChengManager.HuangDiBHName;
		}

		public static int ProcessTakeSheLiZhiYuan(int roleID, string roleName, string bhName, bool sendToOtherLine = true)
		{
			int huangDiRoleID = HuangChengManager.HuangDiRoleID;
			HuangChengManager.HuangDiRoleID = roleID;
			HuangChengManager.HuangDiRoleName = roleName;
			HuangChengManager.HuangDiBHName = bhName;
			HuangChengManager.HuangDiRoleTicks = TimeUtil.NOW();
			if (sendToOtherLine)
			{
				HuangChengManager.NotifySyncHuanDiRoleInfo(huangDiRoleID, roleID, roleName, bhName);
			}
			return huangDiRoleID;
		}

		public static void NotifySyncHuanDiRoleInfo(int oldHuangDiRoleID, int roleID, string roleName, string bhName)
		{
			string text = string.Format("-synchuangdi {0} {1} {2} {3}", new object[]
			{
				oldHuangDiRoleID,
				roleID,
				roleName,
				bhName
			});
			GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				-1,
				"",
				0,
				"",
				0,
				text,
				0,
				0,
				GameManager.ServerLineID
			}), null, 0);
		}

		public static void ParseWeekDaysTimes()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("HuangChengZhanWeekDays");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					','
				});
				int[] array2 = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = Global.SafeConvertToInt32(array[i]);
				}
				HuangChengManager.HuangChengZhanWeekDays = array2;
			}
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("HuangChengZhanFightingDayTimes");
			HuangChengManager.HuangChengZhanFightingDayTimes = Global.ParseDateTimeRangeStr(paramValueByName2);
			HuangChengManager.MaxHavingSheLiZhiYuanSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxHavingSheLiZhiYuanSecs", -1);
			HuangChengManager.MaxHavingSheLiZhiYuanSecs *= 1000;
		}

		private static bool IsDayOfWeek(int weekDayID)
		{
			bool result;
			if (null == HuangChengManager.HuangChengZhanWeekDays)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < HuangChengManager.HuangChengZhanWeekDays.Length; i++)
				{
					if (HuangChengManager.HuangChengZhanWeekDays[i] == weekDayID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public static bool IsInHuangChengFightingTime()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int dayOfWeek = (int)dateTime.DayOfWeek;
			bool result;
			if (!HuangChengManager.IsDayOfWeek(dayOfWeek))
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, HuangChengManager.HuangChengZhanFightingDayTimes, out num, false);
			}
			return result;
		}

		public static bool CanTakeSheLiZhiYuan()
		{
			return HuangChengManager.HuangDiRoleID <= 0;
		}

		public static bool IsHuangChengZhanOver()
		{
			return !HuangChengManager.WaitingHuangChengResult;
		}

		public static void ProcessHuangChengZhanResult()
		{
			if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
			{
				if (HuangChengZhanStates.None == HuangChengManager.HuangChengZhanState)
				{
					if (HuangChengManager.IsInHuangChengFightingTime())
					{
						HuangChengManager.HuangChengZhanState = HuangChengZhanStates.Fighting;
						HuangChengManager.HuangDiRoleTicks = TimeUtil.NOW();
						HuangChengManager.WaitingHuangChengResult = true;
						HuangChengManager.NotifyAllHuangChengMapInfoData();
						HuangChengManager.HandleOutMapHuangDiRoleChanging();
					}
				}
				else if (HuangChengManager.IsInHuangChengFightingTime())
				{
					if (HuangChengManager.WaitingHuangChengResult)
					{
						HuangChengManager.HandleOutMapHuangDiRoleChanging();
						if (HuangChengManager.HuangDiRoleID > 0)
						{
							long num = TimeUtil.NOW();
							if (num - HuangChengManager.HuangDiRoleTicks > (long)HuangChengManager.MaxHavingSheLiZhiYuanSecs)
							{
								HuangChengManager.WaitingHuangChengResult = false;
								HuangChengManager.HandleHuangChengResult();
								HuangChengManager.NotifyAllHuangChengMapInfoData();
							}
						}
					}
				}
				else
				{
					HuangChengManager.HuangChengZhanState = HuangChengZhanStates.None;
					if (HuangChengManager.WaitingHuangChengResult)
					{
						HuangChengManager.WaitingHuangChengResult = false;
						HuangChengManager.HandleHuangChengResult();
						HuangChengManager.NotifyAllHuangChengMapInfoData();
					}
				}
			}
		}

		public static void NotifyAllHuangChengMapInfoData()
		{
			HuangChengMapInfoData huangChengMapInfoData = HuangChengManager.FormatHuangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllHuangChengMapInfoData(Global.GetHuangChengMapCode(), huangChengMapInfoData);
		}

		private static void HandleHuangChengFailed()
		{
			JunQiManager.HandleLingDiZhanResultByMapCode(2, Global.GetHuangChengMapCode(), 0, true, false);
			HuangChengManager.ProcessHuangChengFightingEndAwards(-1);
			Global.BroadcastHuangChengFailedHint();
			JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
		}

		private static void HandleHuangChengResult()
		{
			if (HuangChengManager.HuangDiRoleID <= 0)
			{
				HuangChengManager.HandleHuangChengFailed();
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(HuangChengManager.HuangDiRoleID);
				if (null == gameClient)
				{
					HuangChengManager.HandleHuangChengFailed();
				}
				else if (gameClient.ClientData.Faction <= 0)
				{
					HuangChengManager.HandleHuangChengFailed();
				}
				else if (gameClient.ClientData.BHZhiWu != 1)
				{
					HuangChengManager.HandleHuangChengFailed();
				}
				else
				{
					JunQiManager.HandleLingDiZhanResultByMapCode(2, Global.GetHuangChengMapCode(), gameClient.ClientData.Faction, true, false);
					HuangChengManager.ProcessHuangChengFightingEndAwards(gameClient.ClientData.Faction);
					Global.BroadcastHuangChengOkHint(gameClient);
					JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
				}
			}
		}

		public static void HandleOutMapHuangDiRoleChanging()
		{
			if (HuangChengManager.HuangDiRoleID > 0)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(HuangChengManager.HuangDiRoleID);
				if (gameClient == null || gameClient.ClientData.MapCode != Global.GetHuangChengMapCode())
				{
					HuangChengManager.HandleDeadHuangDiRoleChanging(null);
				}
			}
		}

		public static void HandleLeaveMapHuangDiRoleChanging(GameClient client)
		{
			if (HuangChengManager.HuangDiRoleID > 0)
			{
				if (client.ClientData.RoleID == HuangChengManager.HuangDiRoleID)
				{
					if (HuangChengManager.WaitingHuangChengResult)
					{
						HuangChengManager.HandleDeadHuangDiRoleChanging(null);
					}
				}
			}
		}

		public static void HandleDeadHuangDiRoleChanging(GameClient client)
		{
			if (null != client)
			{
				if (client.ClientData.RoleID != HuangChengManager.HuangDiRoleID)
				{
					return;
				}
				if (2 != JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode))
				{
					return;
				}
				if (!HuangChengManager.IsInHuangChengFightingTime())
				{
					return;
				}
				if (!HuangChengManager.WaitingHuangChengResult)
				{
					return;
				}
			}
			int num = 0;
			lock (HuangChengManager.SheLiZhiYuanMutex)
			{
				num = HuangChengManager.ProcessTakeSheLiZhiYuan(0, "", "", true);
			}
			if (num > 0)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(num);
				if (null != gameClient)
				{
					Global.RemoveBufferData(gameClient, 14);
				}
			}
			GameManager.ClientMgr.NotifyAllChgHuangDiRoleIDMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, num, HuangChengManager.GetHuangDiRoleID());
			HuangChengManager.NotifyAllHuangChengMapInfoData();
		}

		public static HuangChengMapInfoData GetHuangChengMapInfoData(GameClient client)
		{
			int lingDiIDBy2MapCode = JunQiManager.GetLingDiIDBy2MapCode(client.ClientData.MapCode);
			HuangChengMapInfoData result;
			if (lingDiIDBy2MapCode != 2)
			{
				result = null;
			}
			else
			{
				result = HuangChengManager.FormatHuangChengMapInfoData();
			}
			return result;
		}

		public static HuangChengMapInfoData FormatHuangChengMapInfoData()
		{
			return new HuangChengMapInfoData
			{
				FightingEndTime = HuangChengManager.HuangDiRoleTicks,
				HuangDiRoleID = HuangChengManager.HuangDiRoleID,
				HuangDiRoleName = HuangChengManager.HuangDiRoleName,
				HuangDiBHName = HuangChengManager.HuangDiBHName,
				FightingState = (HuangChengManager.WaitingHuangChengResult ? 1 : 0),
				NextBattleTime = "",
				WangZuBHid = -1
			};
		}

		private static int GetExperienceAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 1000000;
			}
			else
			{
				result = 500000;
			}
			return result;
		}

		private static int GetBangGongAwards(GameClient client, bool success)
		{
			int result;
			if (success)
			{
				result = 100;
			}
			else
			{
				result = 50;
			}
			return result;
		}

		private static void ProcessRoleExperienceAwards(GameClient client, bool success)
		{
			int experienceAwards = HuangChengManager.GetExperienceAwards(client, success);
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)experienceAwards, true, false, false, "none");
		}

		private static void ProcessRoleBangGongAwards(GameClient client, bool success)
		{
			int bangGongAwards = HuangChengManager.GetBangGongAwards(client, success);
			if (bangGongAwards > 0)
			{
				if (GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref bangGongAwards, AddBangGongTypes.None, 0))
				{
					if (0 != bangGongAwards)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", "皇城战结束时的奖励", "系统", client.ClientData.RoleName, "增加", bangGongAwards, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
					}
				}
				GameManager.SystemServerEvents.AddEvent(string.Format("角色获取帮贡, roleID={0}({1}), BangGong={2}, newBangGong={3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					client.ClientData.BangGong,
					bangGongAwards
				}), EventLevels.Record);
			}
		}

		private static bool CanGetAWards(GameClient client, long nowTicks)
		{
			bool result;
			if (nowTicks - client.ClientData.EnterMapTicks < (long)HuangChengManager.MaxHavingSheLiZhiYuanSecs)
			{
				result = false;
			}
			else if (client.ClientData.Faction <= 0)
			{
				result = false;
			}
			else
			{
				BangHuiLingDiItemData anyLingDiItemDataByBHID = JunQiManager.GetAnyLingDiItemDataByBHID(client.ClientData.Faction);
				result = (null != anyLingDiItemDataByBHID);
			}
			return result;
		}

		private static void ProcessHuangChengFightingEndAwards(int huangDiBHID)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(Global.GetHuangChengMapCode());
			if (null != mapClients)
			{
				long nowTicks = TimeUtil.NOW();
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.CurrentLifeV > 0)
						{
							if (HuangChengManager.CanGetAWards(gameClient, nowTicks))
							{
								HuangChengManager.ProcessRoleExperienceAwards(gameClient, huangDiBHID == gameClient.ClientData.Faction);
								HuangChengManager.ProcessRoleBangGongAwards(gameClient, huangDiBHID == gameClient.ClientData.Faction);
							}
						}
					}
				}
			}
		}

		public static int NewXuanFeiSafeNum(int roleID)
		{
			int result;
			lock (HuangChengManager.XuanFeiSafeDict)
			{
				int randomNumber = Global.GetRandomNumber(0, 1000001);
				HuangChengManager.XuanFeiSafeDict[roleID] = randomNumber;
				result = randomNumber;
			}
			return result;
		}

		public static int FindXuanFeiSafeNum(int roleID)
		{
			int result;
			lock (HuangChengManager.XuanFeiSafeDict)
			{
				int num = 0;
				if (!HuangChengManager.XuanFeiSafeDict.TryGetValue(roleID, out num))
				{
					result = -1;
				}
				else
				{
					result = num;
				}
			}
			return result;
		}

		public static void RemoveXuanFeiSafeNum(int roleID)
		{
			lock (HuangChengManager.XuanFeiSafeDict)
			{
				HuangChengManager.XuanFeiSafeDict.Remove(roleID);
			}
		}

		public static object SheLiZhiYuanMutex = new object();

		private static bool WaitingHuangChengResult = false;

		private static long HuangDiRoleTicks = TimeUtil.NOW();

		private static int HuangDiRoleID = 0;

		private static string HuangDiRoleName = "";

		private static string HuangDiBHName = "";

		private static int MaxHavingSheLiZhiYuanSecs = 1200;

		private static int[] HuangChengZhanWeekDays = null;

		private static DateTimeRange[] HuangChengZhanFightingDayTimes = null;

		public static HuangChengZhanStates HuangChengZhanState = HuangChengZhanStates.None;

		private static Dictionary<int, int> XuanFeiSafeDict = new Dictionary<int, int>();
	}
}
