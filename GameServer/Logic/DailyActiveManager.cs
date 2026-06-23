using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class DailyActiveManager
	{
		public static void InitDailyActiveFlagIndex()
		{
			DailyActiveManager.m_DailyActiveInfo.Clear();
			int num = 0;
			DailyActiveManager.m_DailyActiveInfo.Add(100, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(200, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(300, num);
			num += 2;
			for (int i = 400; i <= 401; i++)
			{
				DailyActiveManager.m_DailyActiveInfo.Add(i, num);
				num += 2;
			}
			for (int i = 500; i <= 500; i++)
			{
				DailyActiveManager.m_DailyActiveInfo.Add(i, num);
				num += 2;
			}
			for (int i = 600; i <= 600; i++)
			{
				DailyActiveManager.m_DailyActiveInfo.Add(i, num);
				num += 2;
			}
			for (int i = 700; i <= 700; i++)
			{
				DailyActiveManager.m_DailyActiveInfo.Add(i, num);
				num += 2;
			}
			DailyActiveManager.m_DailyActiveInfo.Add(800, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(900, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(1000, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(1100, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(1200, num);
			num += 2;
			for (int i = 1300; i <= 1302; i++)
			{
				DailyActiveManager.m_DailyActiveInfo.Add(i, num);
				num += 2;
			}
			DailyActiveManager.m_DailyActiveInfo.Add(1400, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(1500, num);
			num += 2;
			DailyActiveManager.m_DailyActiveInfo.Add(1600, num);
			num += 2;
		}

		public static void InitRoleDailyActiveData(GameClient client)
		{
			client.ClientData.DailyActiveValues = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
			client.ClientData.DailyActiveDayLginCount = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveDayLoginNum);
			client.ClientData.DailyTotalKillMonsterNum = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveTotalKilledMonsterNum);
			client.ClientData.DailyTotalKillKillBossNum = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveTotalKilledBossNum);
			client.ClientData.DailyCompleteDailyTaskCount = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteDailyTask);
			client.ClientData.DailyActiveDayBuyItemInMall = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveBuyItemInMall);
		}

		public static void SaveRoleDailyActiveData(GameClient client)
		{
			DailyActiveManager.ModifyDailyActiveInfor(client, client.ClientData.DailyTotalKillMonsterNum, DailyActiveDataField1.DailyActiveTotalKilledMonsterNum, true);
		}

		protected static ushort GetDailyActiveIDByIndex(int index)
		{
			for (int i = 0; i < DailyActiveManager.m_DailyActiveInfo.Count; i++)
			{
				if (DailyActiveManager.m_DailyActiveInfo.ElementAt(i).Value == index)
				{
					return (ushort)DailyActiveManager.m_DailyActiveInfo.ElementAt(i).Key;
				}
			}
			return 0;
		}

		protected static int GetCompletedFlagIndex(int DailyActiveID)
		{
			int num = -1;
			int result;
			if (DailyActiveManager.m_DailyActiveInfo.TryGetValue(DailyActiveID, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		protected static int GetAwardFlagIndex(int DailyActiveID)
		{
			int num = -1;
			int result;
			if (DailyActiveManager.m_DailyActiveInfo.TryGetValue(DailyActiveID, out num))
			{
				result = num + 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public static void AddDailyActivePoints(GameClient client, int DailyActiveID, SystemXmlItem itemDailyActive, bool writeToDB = false)
		{
			int num = Math.Max(0, itemDailyActive.GetIntValue("Award", -1));
			int vipLevel = client.ClientData.VipLevel;
			if (vipLevel > 0 && vipLevel <= VIPEumValue.VIPENUMVALUE_MAXLEVEL)
			{
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPHuoYueAdd", ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0 && paramValueIntArrayByName.Length > VIPEumValue.VIPENUMVALUE_MAXLEVEL)
				{
					num += paramValueIntArrayByName[vipLevel];
				}
			}
			if (0 != num)
			{
				client.ClientData.DailyActiveValues += num;
				if (client.ClientData.DailyActiveValues >= 100)
				{
					WebOldPlayerManager.getInstance().ChouJiangAddCheck(client.ClientData.RoleID, 1);
				}
				client.ClientData.OnlineActiveVal += num;
				DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, writeToDB);
				if (writeToDB)
				{
				}
			}
		}

		public static uint GetDailyActiveDataByField(GameClient client, DailyActiveDataField1 field)
		{
			List<uint> roleParamsUIntListFromDB = Global.GetRoleParamsUIntListFromDB(client, "DailyActiveInfo1");
			uint result;
			if (field >= (DailyActiveDataField1)roleParamsUIntListFromDB.Count)
			{
				result = 0U;
			}
			else
			{
				result = roleParamsUIntListFromDB[(int)field];
			}
			return result;
		}

		public static void ModifyDailyActiveInfor(GameClient client, uint value, DailyActiveDataField1 field, bool writeToDB = false)
		{
			List<uint> roleParamsUIntListFromDB = Global.GetRoleParamsUIntListFromDB(client, "DailyActiveInfo1");
			while (roleParamsUIntListFromDB.Count < (int)(field + 1))
			{
				roleParamsUIntListFromDB.Add(0U);
			}
			roleParamsUIntListFromDB[(int)field] = value;
			Global.SaveRoleParamsUintListToDB(client, roleParamsUIntListFromDB, "DailyActiveInfo1", writeToDB);
		}

		public int GetDailyActiveValue(GameClient client)
		{
			client.ClientData.DailyActiveValues = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
			return client.ClientData.DailyActiveValues;
		}

		public static bool IsDailyActiveCompleted(GameClient client, int DailyActiveID)
		{
			return DailyActiveManager.IsFlagIsTrue(client, DailyActiveID, false);
		}

		public static int IsDailyActiveAwardFetched(GameClient client, int nID)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "DailyActiveAwardFlag");
			return roleParamsInt32FromDB & Global.GetBitValue(nID + 1);
		}

		public static void OnDailyActiveCompleted(GameClient client, int DailyActiveID)
		{
			DailyActiveManager.UpdateDailyActiveFlag(client, DailyActiveID);
			DailyActiveManager.NotifyClientDailyActiveData(client, DailyActiveID, false);
			if (client._IconStateMgr.CheckFuLiMeiRiHuoYue(client))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		public static void NotifyClientDailyActiveData(GameClient client, int justCompleteddailyactive = -1, bool bRefresh = false)
		{
			if (client.ClientData.MyRoleDailyData != null && !bRefresh)
			{
				int todayKillBoss = client.ClientData.MyRoleDailyData.TodayKillBoss;
			}
			DailyActiveData instance = new DailyActiveData
			{
				RoleID = client.ClientData.RoleID,
				DailyActiveValues = (long)client.ClientData.DailyActiveValues,
				TotalKilledMonsterCount = (long)((ulong)client.ClientData.DailyTotalKillMonsterNum),
				DailyActiveTotalLoginCount = (long)((ulong)client.ClientData.DailyActiveDayLginCount),
				DailyActiveOnLineTimer = client.ClientData.DayOnlineSecond,
				DailyActiveInforFlags = DailyActiveManager.GetDailyActiveInfoArray(client),
				NowCompletedDailyActiveID = justCompleteddailyactive,
				TotalKilledBossCount = (int)client.ClientData.DailyTotalKillKillBossNum,
				PassNormalCopySceneNum = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteCopyMap1),
				PassHardCopySceneNum = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteCopyMap2),
				PassDifficultCopySceneNum = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteCopyMap3),
				BuyItemInMall = client.ClientData.DailyActiveDayBuyItemInMall,
				CompleteDailyTaskCount = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteDailyTask),
				CompleteBloodCastleCount = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteBloodCastle),
				CompleteDaimonSquareCount = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteDaimonSquare),
				CompleteBattleCount = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteBattle),
				EquipForge = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveEquipForge),
				EquipAppend = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveEquipAppend),
				ChangeLife = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveChangeLife),
				MergeFruit = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveMergeFruit),
				GetAwardFlag = Global.GetRoleParamsInt32FromDB(client, "DailyActiveAwardFlag")
			};
			byte[] buffer = DataHelper.ObjectToBytes<DailyActiveData>(instance);
			GameManager.ClientMgr.SendToClient(client, buffer, 558);
		}

		protected static List<ushort> GetDailyActiveInfoArray(GameClient client)
		{
			List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "DailyActiveFlag");
			int num = 0;
			List<ushort> list = new List<ushort>();
			for (int i = 0; i < roleParamsUlongListFromDB.Count; i++)
			{
				ulong num2 = roleParamsUlongListFromDB[i];
				for (int j = 0; j < 64; j += 2)
				{
					ulong num3 = 3UL << j;
					ushort num4 = (ushort)((num2 & num3) >> j);
					ushort dailyActiveIDByIndex = DailyActiveManager.GetDailyActiveIDByIndex(num);
					ushort num5 = (ushort)(dailyActiveIDByIndex << 2);
					ushort item = num5 | num4;
					list.Add(item);
					num += 2;
				}
			}
			return list;
		}

		public static int GiveDailyActiveAward(GameClient client, int nid)
		{
			int num = 0;
			SystemXmlItem systemXmlItem = null;
			if (GameManager.systemDailyActiveAward.SystemXmlItemDict.TryGetValue(nid, out systemXmlItem))
			{
				num = Math.Max(0, systemXmlItem.GetIntValue("NeedhuoYue", -1));
			}
			int result;
			if (num > client.ClientData.DailyActiveValues)
			{
				result = -3;
			}
			else if (DailyActiveManager.IsDailyActiveAwardFetched(client, nid) > 0)
			{
				result = -2;
			}
			else
			{
				DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
				List<GoodsData> list = new List<GoodsData>();
				string stringValue = systemXmlItem.GetStringValue("GoodsID");
				if (!string.IsNullOrEmpty(stringValue))
				{
					string[] array = stringValue.Split(new char[]
					{
						'|'
					});
					if (null != array)
					{
						for (int i = 0; i < array.Length; i++)
						{
							string text = array[i];
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2 != null && array2.Length == 7)
							{
								GoodsData item = new GoodsData
								{
									Id = -1,
									GoodsID = Convert.ToInt32(array2[0]),
									Using = 0,
									Forge_level = Convert.ToInt32(array2[3]),
									Starttime = "1900-01-01 12:00:00",
									Endtime = "1900-01-01 12:00:00",
									Site = 0,
									Quality = 0,
									Props = "",
									GCount = Convert.ToInt32(array2[1]),
									Binding = Convert.ToInt32(array2[2]),
									Jewellist = "",
									BagIndex = 0,
									AddPropIndex = 0,
									BornIndex = 0,
									Lucky = Convert.ToInt32(array2[5]),
									Strong = 0,
									ExcellenceInfo = Convert.ToInt32(array2[6]),
									AppendPropLev = Convert.ToInt32(array2[4]),
									ChangeLifeLevForEquip = 0
								};
								list.Add(item);
							}
						}
						if (!Global.CanAddGoodsNum(client, list.Count))
						{
							foreach (GoodsData goodsData in list)
							{
								Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(100, new object[0]), GLang.GetLang(100, new object[0]), 1.0);
							}
						}
						else
						{
							foreach (GoodsData goodsData in list)
							{
								GoodsData goodsData2 = new GoodsData
								{
									Id = -1,
									GoodsID = goodsData.GoodsID,
									Using = 0,
									Forge_level = goodsData.Forge_level,
									Starttime = "1900-01-01 12:00:00",
									Endtime = "1900-01-01 12:00:00",
									Site = 0,
									Quality = goodsData.Quality,
									Props = goodsData.Props,
									GCount = goodsData.GCount,
									Binding = goodsData.Binding,
									Jewellist = goodsData.Jewellist,
									BagIndex = 0,
									AddPropIndex = goodsData.AddPropIndex,
									BornIndex = goodsData.BornIndex,
									Lucky = goodsData.Lucky,
									Strong = goodsData.Strong,
									ExcellenceInfo = goodsData.ExcellenceInfo,
									AppendPropLev = goodsData.AppendPropLev,
									ChangeLifeLevForEquip = goodsData.ChangeLifeLevForEquip
								};
								goodsData2.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 0, goodsData2.Jewellist, true, 1, "副本通关获取物品", false, goodsData2.Endtime, goodsData2.AddPropIndex, goodsData2.BornIndex, goodsData2.Lucky, goodsData2.Strong, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, goodsData2.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
							}
							client.ClientData.AddAwardRecord(RoleAwardMsg.DailyActive, list, false);
							GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.DailyActive, "");
						}
					}
				}
				DailyActiveManager.UpdateDailyActiveAwardFlag(client, nid);
				result = 1;
			}
			return result;
		}

		public static bool IsFlagIsTrue(GameClient client, int DailyActiveID, bool forAward = false)
		{
			int num = DailyActiveManager.GetCompletedFlagIndex(DailyActiveID);
			bool result;
			if (num < 0)
			{
				result = false;
			}
			else
			{
				if (forAward)
				{
					num++;
				}
				List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "DailyActiveFlag");
				if (roleParamsUlongListFromDB.Count <= 0)
				{
					result = false;
				}
				else
				{
					int num2 = num / 64;
					if (num2 >= roleParamsUlongListFromDB.Count)
					{
						result = false;
					}
					else
					{
						int num3 = num % 64;
						ulong num4 = roleParamsUlongListFromDB[num2];
						ulong num5 = 1UL << num3;
						bool flag = (num4 & num5) > 0UL;
						result = flag;
					}
				}
			}
			return result;
		}

		public static bool UpdateDailyActiveFlag(GameClient client, int DailyActiveID)
		{
			int completedFlagIndex = DailyActiveManager.GetCompletedFlagIndex(DailyActiveID);
			bool result;
			if (completedFlagIndex < 0)
			{
				result = false;
			}
			else
			{
				List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "DailyActiveFlag");
				int i = completedFlagIndex / 64;
				while (i > roleParamsUlongListFromDB.Count - 1)
				{
					roleParamsUlongListFromDB.Add(0UL);
				}
				int num = completedFlagIndex % 64;
				ulong num2 = roleParamsUlongListFromDB[i];
				ulong num3 = 1UL << num;
				roleParamsUlongListFromDB[i] = (num2 | num3);
				Global.SaveRoleParamsUlongListToDB(client, roleParamsUlongListFromDB, "DailyActiveFlag", true);
				result = true;
			}
			return result;
		}

		public static void UpdateDailyActiveAwardFlag(GameClient client, int nID)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "DailyActiveAwardFlag");
			int nValue = Global.SetIntSomeBit(nID, roleParamsInt32FromDB, true);
			Global.SaveRoleParamsInt32ValueToDB(client, "DailyActiveAwardFlag", nValue, true);
		}

		public static void ProcessDailyActiveKillMonster(GameClient killer, Monster victim)
		{
			if (DailyActiveManager.CheckLevCondition(killer, 1300) || DailyActiveManager.CheckLevCondition(killer, 1301) || DailyActiveManager.CheckLevCondition(killer, 1302))
			{
				killer.ClientData.DailyTotalKillMonsterNum += 1U;
				SafeClientData clientData = killer.ClientData;
				clientData.TimerKilledMonsterNum += 1;
				if (killer.ClientData.TimerKilledMonsterNum > 20)
				{
					killer.ClientData.TimerKilledMonsterNum = 0;
					DailyActiveManager.ModifyDailyActiveInfor(killer, killer.ClientData.DailyTotalKillMonsterNum, DailyActiveDataField1.DailyActiveTotalKilledMonsterNum, false);
				}
				DailyActiveManager.CheckDailyActiveKillMonster(killer);
				if (401 == victim.MonsterType)
				{
					for (int i = 0; i < Data.KillBossCountForChengJiu.Length; i++)
					{
						if (victim.MonsterInfo.ExtensionID == Data.KillBossCountForChengJiu[i])
						{
							DailyActiveManager.CheckDailyActiveKillBoss(killer);
						}
					}
				}
			}
		}

		public static void CheckDailyActiveKillMonster(GameClient client)
		{
			if (client.ClientData.DailyTotalKillMonsterNum >= client.ClientData.DailyNextKillMonsterNum && 2147483647U != client.ClientData.DailyNextKillMonsterNum)
			{
				bool flag = false;
				uint dailyNextKillMonsterNum = DailyActiveManager.CheckSingleConditionForDailyActive(client, 1300, 1302, (long)((ulong)client.ClientData.DailyTotalKillMonsterNum), "KillMonster", out flag);
				client.ClientData.DailyNextKillMonsterNum = dailyNextKillMonsterNum;
				if (DailyActiveManager.IsDailyActiveCompleted(client, 1302))
				{
					client.ClientData.DailyNextKillMonsterNum = 2147483647U;
				}
			}
		}

		public static void CheckDailyActiveKillBoss(GameClient client)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 1400))
			{
				if (DailyActiveManager.CheckLevCondition(client, 1400))
				{
					bool flag = false;
					client.ClientData.DailyTotalKillKillBossNum += 1U;
					DailyActiveManager.ModifyDailyActiveInfor(client, client.ClientData.DailyTotalKillKillBossNum, DailyActiveDataField1.DailyActiveTotalKilledBossNum, true);
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 1400, 1400, (long)client.ClientData.MyRoleDailyData.TodayKillBoss, "KillBoss", out flag);
				}
			}
		}

		protected static uint CheckSingleConditionForDailyActive(GameClient client, int DailyActiveMinID, int DailyActiveMaxID, long roleCurrentValue, string strCheckField, out bool bIsCompleted)
		{
			bIsCompleted = false;
			SystemXmlItem systemXmlItem = null;
			uint num = 0U;
			for (int i = DailyActiveMinID; i <= DailyActiveMaxID; i++)
			{
				if (DailyActiveManager.CheckLevCondition(client, i))
				{
					if (GameManager.systemDailyActiveInfo.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						if (null != systemXmlItem)
						{
							num = (uint)systemXmlItem.GetIntValue(strCheckField, -1);
							if (roleCurrentValue < (long)((ulong)num))
							{
								break;
							}
							if (!DailyActiveManager.IsDailyActiveCompleted(client, i))
							{
								DailyActiveManager.AddDailyActivePoints(client, i, systemXmlItem, true);
								DailyActiveManager.OnDailyActiveCompleted(client, i);
								string text = string.Format("huoyue server={0} account={1} player={2} zoneid={3} task_id={4}", new object[]
								{
									GameManager.ServerId,
									client.strUserID,
									client.ClientData.LocalRoleID,
									client.ClientData.ZoneID,
									i
								});
								LogManager.WriteLog(5, text, null, true);
								bIsCompleted = true;
							}
						}
					}
				}
			}
			return num;
		}

		public static void ProcessOnlineForDailyActive(GameClient client)
		{
			bool flag = false;
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 200))
			{
				if (client.ClientData.DayOnlineSecond - client.ClientData.DailyOnlineTimeTmp > 0)
				{
					client.ClientData.DailyOnlineTimeTmp += 60;
					if (!DailyActiveManager.CheckLevCondition(client, 200))
					{
						flag = false;
					}
					else
					{
						DailyActiveManager.CheckSingleConditionForDailyActive(client, 200, 200, (long)(client.ClientData.DayOnlineSecond / 60), "Online", out flag);
					}
				}
			}
		}

		public static void ProcessLoginForDailyActive(GameClient client, out bool bIsCompleted)
		{
			bIsCompleted = false;
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 100))
			{
				if (DailyActiveManager.CheckLevCondition(client, 100))
				{
					client.ClientData.DailyActiveDayLginCount += 1U;
					uint dailyActiveDataByField = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveDayLoginNum);
					DailyActiveManager.ModifyDailyActiveInfor(client, client.ClientData.DailyActiveDayLginCount, DailyActiveDataField1.DailyActiveDayLoginNum, true);
					dailyActiveDataByField = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveDayLoginNum);
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 100, 100, (long)((ulong)client.ClientData.DailyActiveDayLginCount), "Login", out bIsCompleted);
					client.ClientData.DailyActiveDayLginSetFlag = true;
				}
			}
		}

		public static void ProcessBuyItemInMallForDailyActive(GameClient client, int nValue)
		{
			int num = Global.GetRoleParamsInt32FromDB(client, "10175");
			num += nValue;
			if (num >= 100)
			{
				WebOldPlayerManager.getInstance().ChouJiangAddCheck(client.ClientData.RoleID, 2);
			}
			Global.SaveRoleParamsInt32ValueToDB(client, "10175", num, true);
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 300))
			{
				if (DailyActiveManager.CheckLevCondition(client, 300))
				{
					uint dailyActiveDataByField = DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveBuyItemInMall);
					client.ClientData.DailyActiveDayBuyItemInMall += (int)(dailyActiveDataByField + (uint)nValue);
					DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveDayBuyItemInMall, DailyActiveDataField1.DailyActiveBuyItemInMall, true);
					bool flag = false;
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 300, 300, (long)client.ClientData.DailyActiveDayBuyItemInMall, "Consumption", out flag);
				}
			}
		}

		public static void ProcessCompleteDailyTaskForDailyActive(GameClient client, int nValue)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 400) || !DailyActiveManager.IsDailyActiveCompleted(client, 401))
			{
				if (DailyActiveManager.CheckLevCondition(client, 400) || DailyActiveManager.CheckLevCondition(client, 401))
				{
					client.ClientData.DailyCompleteDailyTaskCount = (uint)nValue;
					DailyActiveManager.ModifyDailyActiveInfor(client, client.ClientData.DailyCompleteDailyTaskCount, DailyActiveDataField1.DailyActiveCompleteDailyTask, true);
					bool flag = false;
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 400, 401, (long)((ulong)client.ClientData.DailyCompleteDailyTaskCount), "RiChang", out flag);
				}
			}
		}

		public static void ProcessCompleteCopyMapForDailyActive(GameClient client, int nCopyMapLev, int count = 1)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 500) || !DailyActiveManager.IsDailyActiveCompleted(client, 600) || !DailyActiveManager.IsDailyActiveCompleted(client, 700))
			{
				if (nCopyMapLev >= 0)
				{
					bool flag = false;
					switch (nCopyMapLev)
					{
					case 1:
						if (DailyActiveManager.CheckLevCondition(client, 500))
						{
							int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteCopyMap1);
							num++;
							num *= count;
							DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveCompleteCopyMap1, true);
							DailyActiveManager.CheckSingleConditionForDailyActive(client, 500, 500, (long)num, "KillRaid", out flag);
						}
						break;
					case 2:
						if (DailyActiveManager.CheckLevCondition(client, 600))
						{
							int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteCopyMap2);
							num++;
							num *= count;
							DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveCompleteCopyMap2, true);
							DailyActiveManager.CheckSingleConditionForDailyActive(client, 600, 600, (long)num, "KillRaid", out flag);
						}
						break;
					case 3:
						if (DailyActiveManager.CheckLevCondition(client, 700))
						{
							int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteCopyMap3);
							num++;
							num *= count;
							DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveCompleteCopyMap3, true);
							DailyActiveManager.CheckSingleConditionForDailyActive(client, 700, 700, (long)num, "KillRaid", out flag);
						}
						break;
					}
				}
			}
		}

		public static void ProcessCompleteDailyActivityForDailyActive(GameClient client, int nType)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 800) || !DailyActiveManager.IsDailyActiveCompleted(client, 900) || !DailyActiveManager.IsDailyActiveCompleted(client, 1000))
			{
				if (nType >= 0)
				{
					bool flag = false;
					switch (nType)
					{
					case 1:
						if (DailyActiveManager.CheckLevCondition(client, 800))
						{
							int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteBloodCastle);
							num++;
							DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveCompleteBloodCastle, true);
							DailyActiveManager.CheckSingleConditionForDailyActive(client, 800, 800, (long)num, "HuoDongLimit", out flag);
						}
						break;
					case 2:
						if (DailyActiveManager.CheckLevCondition(client, 900))
						{
							int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteDaimonSquare);
							num++;
							DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveCompleteDaimonSquare, true);
							DailyActiveManager.CheckSingleConditionForDailyActive(client, 900, 900, (long)num, "HuoDongLimit", out flag);
						}
						break;
					case 3:
						if (DailyActiveManager.CheckLevCondition(client, 1000))
						{
							int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveCompleteBattle);
							num++;
							DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveCompleteBattle, true);
							DailyActiveManager.CheckSingleConditionForDailyActive(client, 1000, 1000, (long)num, "HuoDongLimit", out flag);
						}
						break;
					}
				}
			}
		}

		public static void ProcessDailyActiveEquipForge(GameClient client)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 1100))
			{
				if (DailyActiveManager.CheckLevCondition(client, 1100))
				{
					int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveEquipForge);
					num++;
					DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveEquipForge, true);
					bool flag = false;
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 1100, 1100, (long)num, "QiangHuaLimit", out flag);
				}
			}
		}

		public static void ProcessDailyActiveEquipAppend(GameClient client)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 1200))
			{
				if (DailyActiveManager.CheckLevCondition(client, 1200))
				{
					int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveEquipAppend);
					num++;
					DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveEquipAppend, true);
					bool flag = false;
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 1200, 1200, (long)num, "ZhuiJiaLimit", out flag);
				}
			}
		}

		public static void ProcessDailyActiveChangeLife(GameClient client)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 1500))
			{
				if (DailyActiveManager.CheckLevCondition(client, 1500))
				{
					int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveChangeLife);
					num++;
					DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveChangeLife, true);
					bool flag = false;
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 1500, 1500, (long)num, "ZhuanShengLimit", out flag);
				}
			}
		}

		public static void ProcessDailyActiveMergeFruit(GameClient client)
		{
			if (!DailyActiveManager.IsDailyActiveCompleted(client, 1600))
			{
				if (DailyActiveManager.CheckLevCondition(client, 1600))
				{
					int num = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveMergeFruit);
					num++;
					DailyActiveManager.ModifyDailyActiveInfor(client, (uint)num, DailyActiveDataField1.DailyActiveMergeFruit, true);
					bool flag = false;
					DailyActiveManager.CheckSingleConditionForDailyActive(client, 1600, 1600, (long)num, "HeChengLimit", out flag);
				}
			}
		}

		public static bool CheckLevCondition(GameClient client, int daTpye)
		{
			SystemXmlItem systemXmlItem = null;
			GameManager.systemDailyActiveInfo.SystemXmlItemDict.TryGetValue(daTpye, out systemXmlItem);
			bool result;
			if (null == systemXmlItem)
			{
				result = false;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("MinZhuanshengleve", -1);
				if (client.ClientData.ChangeLifeCount < intValue)
				{
					result = false;
				}
				else
				{
					if (client.ClientData.ChangeLifeCount == systemXmlItem.GetIntValue("MinZhuanshengleve", -1))
					{
						int intValue2 = systemXmlItem.GetIntValue("Minleve", -1);
						if (client.ClientData.Level < intValue2)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static void CleanDailyActiveInfo(GameClient client)
		{
			List<ulong> lsUlong = new List<ulong>();
			Global.SaveRoleParamsUlongListToDB(client, lsUlong, "DailyActiveFlag", true);
			List<uint> lsUint = new List<uint>();
			Global.SaveRoleParamsUintListToDB(client, lsUint, "DailyActiveInfo1", true);
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			Global.SaveRoleParamsInt32ValueToDB(client, "DailyActiveDayID", dayOfYear, true);
			Global.SaveRoleParamsInt32ValueToDB(client, "DailyActiveAwardFlag", 0, true);
			client.ClientData.DailyActiveDayID = dayOfYear;
			client.ClientData.DailyActiveValues = 0;
			client.ClientData.DailyTotalKillMonsterNum = 0U;
			client.ClientData.DailyCompleteDailyTaskCount = 0U;
			client.ClientData.DailyNextKillMonsterNum = 0U;
			client.ClientData.DailyActiveDayBuyItemInMall = 0;
			client.ClientData.DailyActiveDayLginCount = 0U;
		}

		private static Dictionary<int, int> m_DailyActiveInfo = new Dictionary<int, int>();

		public static int m_DailyActiveDayID = 0;
	}
}
