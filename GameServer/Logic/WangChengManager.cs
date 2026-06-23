using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class WangChengManager
	{
		public static void UpdateWangZuBHNameFromDBServer(int bhid)
		{
			WangChengManager.WangZuBHid = bhid;
			if (bhid > 0)
			{
				BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, 0);
				if (null != bangHuiMiniData)
				{
					WangChengManager.WangZuBHName = bangHuiMiniData.BHName;
				}
			}
		}

		public static int GetWangZuBHid()
		{
			return WangChengManager.WangZuBHid;
		}

		public static string GetWangZuBHName()
		{
			return WangChengManager.WangZuBHName;
		}

		public static void ParseWeekDaysTimes()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("WangChengZhanWeekDays");
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
				if (array2.Length > 0 && array2[0] >= 0)
				{
					WangChengManager.WangChengZhanWeekDaysByConfig = true;
					WangChengManager.WangChengZhanWeekDays = array2;
				}
			}
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("WangChengZhanFightingDayTimes");
			WangChengManager.WangChengZhanFightingDayTimes = Global.ParseDateTimeRangeStr(paramValueByName2);
			WangChengManager.MaxTakingHuangGongSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxTakingHuangGongSecs", -1);
			WangChengManager.MaxTakingHuangGongSecs *= 1000;
			Global.UpdateWangChengZhanWeekDays(true);
			WangChengManager.NotifyAllWangChengMapInfoData();
		}

		private static bool IsDayOfWeek(int weekDayID)
		{
			bool result;
			if (null == WangChengManager.WangChengZhanWeekDays)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < WangChengManager.WangChengZhanWeekDays.Length; i++)
				{
					if (WangChengManager.WangChengZhanWeekDays[i] == weekDayID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public static bool IsInWangChengFightingTime()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int dayOfWeek = (int)dateTime.DayOfWeek;
			bool result;
			if (!WangChengManager.IsDayOfWeek(dayOfWeek))
			{
				result = false;
			}
			else
			{
				int num = 0;
				result = Global.JugeDateTimeInTimeRange(dateTime, WangChengManager.WangChengZhanFightingDayTimes, out num, false);
			}
			return result;
		}

		public static bool IsWangChengZhanOver()
		{
			return !WangChengManager.WaitingHuangChengResult;
		}

		public static bool IsInCityWarBattling(GameClient client)
		{
			if (client.ClientData.MapCode == Global.GetHuangGongMapCode())
			{
				if (WangChengZhanStates.None != WangChengManager.WangChengZhanState)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsInBattling()
		{
			return WangChengZhanStates.None != WangChengManager.WangChengZhanState;
		}

		public static void ProcessWangChengZhanResult()
		{
			Global.UpdateWangChengZhanWeekDays(false);
			if (WangChengZhanStates.None == WangChengManager.WangChengZhanState)
			{
				if (WangChengManager.IsInWangChengFightingTime())
				{
					WangChengManager.WangChengZhanState = WangChengZhanStates.Fighting;
					WangChengManager.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
					WangChengManager.WaitingHuangChengResult = true;
					WangChengManager.NotifyAllWangChengMapInfoData();
					Global.BroadcastHuangChengBattleStart();
				}
			}
			else if (WangChengManager.IsInWangChengFightingTime())
			{
				bool flag = WangChengManager.TryGenerateNewHuangChengBangHui();
				if (flag)
				{
					WangChengManager.HandleHuangChengResultEx(false);
					WangChengManager.NotifyAllWangChengMapInfoData();
				}
				else
				{
					WangChengManager.ProcessTimeAddRoleExp();
				}
			}
			else
			{
				WangChengManager.WangChengZhanState = WangChengZhanStates.None;
				WangChengManager.WaitingHuangChengResult = false;
				WangChengManager.TryGenerateNewHuangChengBangHui();
				WangChengManager.HandleHuangChengResultEx(true);
				WangChengManager.NotifyAllWangChengMapInfoData();
			}
		}

		public static bool TryGenerateNewHuangChengBangHui()
		{
			int theOnlyOneBangHui = WangChengManager.GetTheOnlyOneBangHui();
			bool result;
			if (theOnlyOneBangHui <= 0 || WangChengManager.WangZuBHid == theOnlyOneBangHui)
			{
				WangChengManager.LastTheOnlyOneBangHui = -1;
				result = false;
			}
			else if (WangChengManager.LastTheOnlyOneBangHui != theOnlyOneBangHui)
			{
				WangChengManager.LastTheOnlyOneBangHui = theOnlyOneBangHui;
				WangChengManager.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
				result = false;
			}
			else
			{
				if (WangChengManager.LastTheOnlyOneBangHui > 0)
				{
					long num = TimeUtil.NOW();
					if (num - WangChengManager.BangHuiTakeHuangGongTicks > (long)WangChengManager.MaxTakingHuangGongSecs)
					{
						WangChengManager.WangZuBHid = WangChengManager.LastTheOnlyOneBangHui;
						WangChengManager.UpdateWangZuBHNameFromDBServer(theOnlyOneBangHui);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public static int GetTheOnlyOneBangHui()
		{
			List<GameClient> mapAliveClientsEx = GameManager.ClientMgr.GetMapAliveClientsEx(Global.GetHuangGongMapCode(), true);
			int result = -1;
			int num = -1;
			for (int i = 0; i < mapAliveClientsEx.Count; i++)
			{
				GameClient gameClient = mapAliveClientsEx[i];
				if (num != -1)
				{
					if (gameClient.ClientData.Faction > 0 && gameClient.ClientData.Faction != num)
					{
						result = -1;
						break;
					}
				}
				else if (gameClient.ClientData.Faction > 0)
				{
					num = gameClient.ClientData.Faction;
					result = num;
				}
			}
			return result;
		}

		public static void NotifyAllWangChengMapInfoData()
		{
			WangChengMapInfoData wangChengMapInfoData = WangChengManager.FormatWangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllWangChengMapInfoData(wangChengMapInfoData);
		}

		private static void HandleWangChengFailed()
		{
			JunQiManager.HandleLingDiZhanResultByMapCode(6, Global.GetHuangGongMapCode(), 0, true, false);
			Global.BroadcastWangChengFailedHint();
			JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
		}

		private static void HandleHuangChengResultEx(bool isBattleOver = false)
		{
			if (WangChengManager.WangZuBHid <= 0)
			{
				if (isBattleOver)
				{
					WangChengManager.HandleWangChengFailed();
				}
			}
			else
			{
				JunQiManager.HandleLingDiZhanResultByMapCode(6, Global.GetHuangGongMapCode(), WangChengManager.WangZuBHid, true, false);
				Global.BroadcastHuangChengOkHintEx(WangChengManager.WangZuBHName, isBattleOver);
				JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
				if (isBattleOver)
				{
					HuodongCachingMgr.UpdateHeFuWCKingBHID(WangChengManager.WangZuBHid);
				}
			}
		}

		public static void NotifyClientWangChengMapInfoData(GameClient client)
		{
			WangChengMapInfoData wangChengMapInfoData = WangChengManager.GetWangChengMapInfoData(client);
			GameManager.ClientMgr.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
		}

		public static WangChengMapInfoData GetWangChengMapInfoData(GameClient client)
		{
			return WangChengManager.FormatWangChengMapInfoData();
		}

		public static WangChengMapInfoData FormatWangChengMapInfoData()
		{
			string nextBattleTime = GLang.GetLang(43, new object[0]);
			long fightingEndTime = 0L;
			if (WangChengZhanStates.None == WangChengManager.WangChengZhanState)
			{
				nextBattleTime = WangChengManager.GetNextCityBattleTime();
			}
			else
			{
				fightingEndTime = WangChengManager.GetBattleEndMs();
			}
			return new WangChengMapInfoData
			{
				FightingEndTime = fightingEndTime,
				FightingState = (WangChengManager.WaitingHuangChengResult ? 1 : 0),
				NextBattleTime = nextBattleTime,
				WangZuBHName = WangChengManager.WangZuBHName,
				WangZuBHid = WangChengManager.WangZuBHid
			};
		}

		public static Dictionary<int, int> GetWarRequstMap(string warReqString)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			string[] array = warReqString.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'_'
				});
				if (array2.Length == 2)
				{
					int key = int.Parse(array2[0]);
					int num = int.Parse(array2[1]);
					if (!dictionary.ContainsKey(key))
					{
						if (num >= TimeUtil.NowDateTime().DayOfYear || num + 120 <= TimeUtil.NowDateTime().DayOfYear)
						{
							dictionary.Add(key, num);
						}
					}
				}
			}
			return dictionary;
		}

		public static string GeWarRequstString(Dictionary<int, int> warRequstMap)
		{
			string text = "";
			for (int i = 0; i < warRequstMap.Count; i++)
			{
				if (text.Length > 0)
				{
					text += ",";
				}
				text += string.Format("{0}_{1}", warRequstMap.ElementAt(i).Key, warRequstMap.ElementAt(i).Value);
			}
			return text;
		}

		public static int SetCityWarRequestToDBServer(int lingDiID, string nowWarRequest)
		{
			int num = -200;
			string strcmd = string.Format("{0}:{1}", lingDiID, nowWarRequest);
			string[] array = Global.ExecuteDBCmd(10098, strcmd, 0);
			int result;
			if (array == null || array.Length != 5)
			{
				result = num;
			}
			else
			{
				num = Global.SafeConvertToInt32(array[0]);
				JunQiManager.NotifySyncBangHuiLingDiItemsDict();
				result = num;
			}
			return result;
		}

		public static bool IsExistCityWarToday()
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(6);
			bool result;
			if (null == itemByLingDiID)
			{
				result = false;
			}
			else
			{
				Dictionary<int, int> warRequstMap = WangChengManager.GetWarRequstMap(itemByLingDiID.WarRequest);
				result = warRequstMap.ContainsValue(dayOfYear);
			}
			return result;
		}

		public static void UpdateWangChengZhanWeekDays(int[] weekDays)
		{
			if (!WangChengManager.WangChengZhanWeekDaysByConfig)
			{
				WangChengManager.WangChengZhanWeekDays = weekDays;
			}
		}

		protected static void RemoveTodayInWarRequest()
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(6);
			if (null != itemByLingDiID)
			{
				Dictionary<int, int> warRequstMap = WangChengManager.GetWarRequstMap(itemByLingDiID.WarRequest);
				if (warRequstMap.ContainsValue(dayOfYear))
				{
					for (int i = 0; i < warRequstMap.Count; i++)
					{
						if (warRequstMap.Values.ElementAt(i) == dayOfYear)
						{
							warRequstMap.Remove(warRequstMap.Keys.ElementAt(i));
							break;
						}
					}
					string nowWarRequest = WangChengManager.GeWarRequstString(warRequstMap);
					WangChengManager.SetCityWarRequestToDBServer(6, nowWarRequest);
				}
			}
		}

		public static long GetBattleEndMs()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int hour = dateTime.Hour;
			int minute = dateTime.Minute;
			int num = hour * 60 + minute;
			int num2 = 0;
			Global.JugeDateTimeInTimeRange(TimeUtil.NowDateTime(), WangChengManager.WangChengZhanFightingDayTimes, out num2, true);
			return dateTime.AddMinutes((double)Math.Max(0, num2 - num)).Ticks / 10000L;
		}

		public static string GetNextCityBattleTime()
		{
			string lang = GLang.GetLang(43, new object[0]);
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(6);
			string result;
			if (null == itemByLingDiID)
			{
				result = lang;
			}
			else
			{
				Dictionary<int, int> warRequstMap = WangChengManager.GetWarRequstMap(itemByLingDiID.WarRequest);
				List<DateTime> list = new List<DateTime>();
				for (int i = 0; i < warRequstMap.Count; i++)
				{
					DateTime item = TimeUtil.NowDateTime();
					int num = warRequstMap.Values.ElementAt(i) - dayOfYear;
					if (num >= 0)
					{
						item = item.AddDays((double)num);
					}
					else
					{
						int num2 = item.Year + 1;
						item = DateTime.Parse(string.Format("{0}-01-01", num2)).AddDays((double)(warRequstMap.Values.ElementAt(i) - 1));
					}
					list.Add(item);
				}
				list.Sort(delegate(DateTime l, DateTime r)
				{
					int result2;
					if (l.Ticks < r.Ticks)
					{
						result2 = -1;
					}
					else if (l.Ticks > r.Ticks)
					{
						result2 = 1;
					}
					else
					{
						result2 = 0;
					}
					return result2;
				});
				if (list.Count > 0)
				{
					DateTime dateTime = list[0];
					if (WangChengManager.WangChengZhanFightingDayTimes != null && WangChengManager.WangChengZhanFightingDayTimes.Length > 0)
					{
						return list[0].ToString("yyyy-MM-dd " + string.Format("{0:00}:{1:00}", WangChengManager.WangChengZhanFightingDayTimes[0].FromHour, WangChengManager.WangChengZhanFightingDayTimes[0].FromMinute));
					}
				}
				result = lang;
			}
			return result;
		}

		public static bool GetNextCityBattleTimeAndBangHui(out int dayID, out int bangHuiID)
		{
			dayID = -1;
			bangHuiID = -1;
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(6);
			bool result;
			if (null == itemByLingDiID)
			{
				result = false;
			}
			else
			{
				Dictionary<int, int> warRequstMap = WangChengManager.GetWarRequstMap(itemByLingDiID.WarRequest);
				List<DateTime> list = new List<DateTime>();
				for (int i = 0; i < warRequstMap.Count; i++)
				{
					DateTime item = TimeUtil.NowDateTime();
					int num = warRequstMap.Values.ElementAt(i) - dayOfYear;
					if (num >= 0)
					{
						item = item.AddDays((double)num);
					}
					else
					{
						int num2 = item.Year + 1;
						item = DateTime.Parse(string.Format("{0}-01-01", num2)).AddDays((double)(warRequstMap.Values.ElementAt(i) - 1));
					}
					list.Add(item);
				}
				list.Sort(delegate(DateTime l, DateTime r)
				{
					int result2;
					if (l.Ticks < r.Ticks)
					{
						result2 = -1;
					}
					else if (l.Ticks > r.Ticks)
					{
						result2 = 1;
					}
					else
					{
						result2 = 0;
					}
					return result2;
				});
				if (list.Count > 0)
				{
					DateTime dateTime = list[0];
					if (WangChengManager.WangChengZhanFightingDayTimes != null && WangChengManager.WangChengZhanFightingDayTimes.Length > 0)
					{
						dayID = dateTime.DayOfYear;
						for (int i = 0; i < warRequstMap.Count; i++)
						{
							if (dayID == warRequstMap.Values.ElementAt(i))
							{
								bangHuiID = warRequstMap.Keys.ElementAt(i);
								return true;
							}
						}
						return false;
					}
				}
				result = false;
			}
			return result;
		}

		public static bool GetNextCityBattleTimeAndBangHui(out string dayTime, out string bangHuiName)
		{
			dayTime = GLang.GetLang(43, new object[0]);
			bangHuiName = GLang.GetLang(568, new object[0]);
			int warDay;
			int bangHuiID;
			return WangChengManager.GetNextCityBattleTimeAndBangHui(out warDay, out bangHuiID) && WangChengManager.GetWarTimeStringAndBHName(warDay, bangHuiID, out dayTime, out bangHuiName);
		}

		public static string GetCityBattleTimeAndBangHuiListString()
		{
			string result;
			if (WangChengManager.WangChengZhanFightingDayTimes == null || WangChengManager.WangChengZhanFightingDayTimes.Length <= 0)
			{
				result = "";
			}
			else
			{
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(6);
				if (null == itemByLingDiID)
				{
					result = "";
				}
				else
				{
					Dictionary<int, int> warRequstMap = WangChengManager.GetWarRequstMap(itemByLingDiID.WarRequest);
					List<DateTime> list = new List<DateTime>();
					for (int i = 0; i < warRequstMap.Count; i++)
					{
						DateTime item = TimeUtil.NowDateTime();
						int num = warRequstMap.Values.ElementAt(i) - dayOfYear;
						if (num >= 0)
						{
							item = item.AddDays((double)num);
						}
						else
						{
							int num2 = item.Year + 1;
							item = DateTime.Parse(string.Format("{0}-01-01", num2)).AddDays((double)(warRequstMap.Values.ElementAt(i) - 1));
						}
						list.Add(item);
					}
					list.Sort(delegate(DateTime l, DateTime r)
					{
						int result2;
						if (l.Ticks < r.Ticks)
						{
							result2 = -1;
						}
						else if (l.Ticks > r.Ticks)
						{
							result2 = 1;
						}
						else
						{
							result2 = 0;
						}
						return result2;
					});
					string text = "";
					int num3 = 0;
					while (num3 < list.Count && num3 < 10)
					{
						int dayOfYear2 = list[num3].DayOfYear;
						for (int i = 0; i < warRequstMap.Count; i++)
						{
							if (dayOfYear2 == warRequstMap.Values.ElementAt(i))
							{
								int bangHuiID = warRequstMap.Keys.ElementAt(i);
								string arg;
								string arg2;
								WangChengManager.GetWarTimeStringAndBHName(dayOfYear2, bangHuiID, out arg, out arg2);
								if (text.Length > 0)
								{
									text += ",";
								}
								text += string.Format("{0},{1}", arg, arg2);
								break;
							}
						}
						num3++;
					}
					result = text;
				}
			}
			return result;
		}

		private static bool GetWarTimeStringAndBHName(int warDay, int bangHuiID, out string dayTime, out string bangHuiName)
		{
			dayTime = GLang.GetLang(43, new object[0]);
			bangHuiName = GLang.GetLang(568, new object[0]);
			BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bangHuiID, 0);
			if (null != bangHuiMiniData)
			{
				bangHuiName = bangHuiMiniData.BHName;
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			DateTime dateTime = TimeUtil.NowDateTime();
			int num = warDay - dayOfYear;
			if (num >= 0)
			{
				dateTime = dateTime.AddDays((double)num);
			}
			else
			{
				int num2 = dateTime.Year + 1;
				dateTime = DateTime.Parse(string.Format("{0}-01-01", num2)).AddDays((double)(warDay - 1));
			}
			if (WangChengManager.WangChengZhanFightingDayTimes != null && WangChengManager.WangChengZhanFightingDayTimes.Length > 0)
			{
				string arg = dateTime.ToString("yyyy-MM-dd " + string.Format("{0:00}:{1:00}", WangChengManager.WangChengZhanFightingDayTimes[0].FromHour, WangChengManager.WangChengZhanFightingDayTimes[0].FromMinute));
				string arg2 = string.Format("{0:00}:{1:00}", WangChengManager.WangChengZhanFightingDayTimes[0].EndHour, WangChengManager.WangChengZhanFightingDayTimes[0].EndMinute);
				dayTime = string.Format(GLang.GetLang(569, new object[0]), arg, arg2);
			}
			else
			{
				dayTime = dateTime.ToString("yyyy-MM-dd");
			}
			return true;
		}

		private static void ProcessTimeAddRoleExp()
		{
			long num = TimeUtil.NOW();
			if (num - WangChengManager.LastAddBangZhanAwardsTicks >= 10000L)
			{
				WangChengManager.LastAddBangZhanAwardsTicks = num;
				List<object> mapClients = GameManager.ClientMgr.GetMapClients(Global.GetHuangGongMapCode());
				if (null != mapClients)
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						GameClient gameClient = mapClients[i] as GameClient;
						if (gameClient != null)
						{
							BangZhanAwardsMgr.ProcessBangZhanAwards(gameClient);
						}
					}
				}
			}
		}

		private static bool WaitingHuangChengResult = false;

		private static long BangHuiTakeHuangGongTicks = TimeUtil.NOW();

		private static string WangZuBHName = "";

		private static int WangZuBHid = -1;

		public static object ApplyWangChengWarMutex = new object();

		private static int MaxTakingHuangGongSecs = 1200;

		private static bool WangChengZhanWeekDaysByConfig = false;

		private static int[] WangChengZhanWeekDays = null;

		private static DateTimeRange[] WangChengZhanFightingDayTimes = null;

		public static WangChengZhanStates WangChengZhanState = WangChengZhanStates.None;

		private static int LastTheOnlyOneBangHui = -1;

		private static long LastAddBangZhanAwardsTicks = 0L;
	}
}
