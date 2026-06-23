using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class CaiJiLogic
	{
		public static bool LoadConfig()
		{
			CaiJiLogic.DailyNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MuKuangNum", -1);
			CaiJiLogic.DeadReliveTime = (int)GameManager.systemParamsList.GetParamValueIntByName("CrystalDeadTime", -1);
			List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("MuKuangDoubleAward", '|');
			bool result;
			if (paramValueStringListByName == null || paramValueStringListByName.Count == 0)
			{
				result = false;
			}
			else
			{
				CaiJiLogic.dateTimeRangeArray = new CaiJiDateTimeRange[paramValueStringListByName.Count];
				for (int i = 0; i < paramValueStringListByName.Count; i++)
				{
					string[] array = paramValueStringListByName[i].Split(new char[]
					{
						','
					});
					if (array.Length != 3)
					{
						return false;
					}
					CaiJiDateTimeRange caiJiDateTimeRange = new CaiJiDateTimeRange();
					string text = array[0];
					string[] array2 = text.Split(new char[]
					{
						':'
					});
					caiJiDateTimeRange.FromHour = int.Parse(array2[0]);
					caiJiDateTimeRange.FromMinute = int.Parse(array2[1]);
					string text2 = array[1];
					array2 = text2.Split(new char[]
					{
						':'
					});
					caiJiDateTimeRange.EndHour = int.Parse(array2[0]);
					caiJiDateTimeRange.EndMinute = int.Parse(array2[1]);
					caiJiDateTimeRange.DoubleAwardRate = float.Parse(array[2]);
					CaiJiLogic.dateTimeRangeArray[i] = caiJiDateTimeRange;
				}
				CaiJiLogic.GatherTimePer = (int)GameManager.systemParamsList.GetParamValueIntByName("GatherTimePer", 90);
				result = true;
			}
			return result;
		}

		public static int JugeDateTimeInTimeRange(DateTime dateTime, DateTimeRange[] dateTimeRangeArray, bool equalEndTime = true)
		{
			int result;
			if (null == dateTimeRangeArray)
			{
				result = -1;
			}
			else
			{
				int hour = dateTime.Hour;
				int minute = dateTime.Minute;
				for (int i = 0; i < dateTimeRangeArray.Length; i++)
				{
					if (null != dateTimeRangeArray[i])
					{
						int num = dateTimeRangeArray[i].FromHour * 60 + dateTimeRangeArray[i].FromMinute;
						int num2 = dateTimeRangeArray[i].EndHour * 60 + dateTimeRangeArray[i].EndMinute;
						int num3 = hour * 60 + minute;
						if (!equalEndTime)
						{
							num2--;
						}
						if (num3 >= num && num3 <= num2)
						{
							return i;
						}
					}
				}
				result = -1;
			}
			return result;
		}

		public static int ReqStartCaiJi(GameClient client, int monsterId, out int GatherTime)
		{
			GatherTime = 0;
			CaiJiLogic.CancelCaiJiState(client);
			int result;
			if (TimeUtil.NOW() < client.ClientData.CurrentMagicActionEndTicks)
			{
				result = -43;
			}
			else if (client.ClientData.CurrentLifeV <= 0)
			{
				CaiJiLogic.CancelCaiJiState(client);
				result = -3;
			}
			else
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, monsterId);
				if (null == monster)
				{
					result = -1;
				}
				else if (monster.MonsterType != 1601)
				{
					result = -4;
				}
				else if (monster.IsCollected)
				{
					result = -4;
				}
				else
				{
					SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					GetCaiJiTimeEventObject getCaiJiTimeEventObject = new GetCaiJiTimeEventObject(client, monster);
					bool flag = GlobalEventSource4Scene.getInstance().fireEvent(getCaiJiTimeEventObject, mapSceneType);
					if (flag)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = getCaiJiTimeEventObject.GatherTime;
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 25)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = HuanYingSiYuanManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 27)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = YongZheZhanChangManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 39)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = KingOfBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 48)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = CompManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 42)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						GatherTime = KarenBattleManager_MapEast.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 43)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							return -301;
						}
						GatherTime = LingDiCaiJiManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else if (mapSceneType == 59)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							return -301;
						}
						GatherTime = EscapeBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (GatherTime < 0)
						{
							return GatherTime;
						}
					}
					else
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							return -301;
						}
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.systemCaiJiMonsterMgr.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out systemXmlItem) || null == systemXmlItem)
						{
							return -4;
						}
						GatherTime = systemXmlItem.GetIntValue("GatherTime", -1);
						if (client.ClientData.DailyCrystalCollectNum >= CaiJiLogic.DailyNum)
						{
							return -5;
						}
					}
					Global.EndMeditate(client);
					CaiJiLogic.SetCaiJiState(client, monsterId, 0L, monster.UniqueID);
					result = 0;
				}
			}
			return result;
		}

		public static int ReqFinishCaiJi(GameClient client, int monsterId)
		{
			int result;
			if (monsterId != client.ClientData.CaijTargetId || client.ClientData.CaiJiStartTick == 0U || client.ClientData.CaijTargetId == 0)
			{
				CaiJiLogic.CancelCaiJiState(client);
				result = -3;
			}
			else if (client.ClientData.CurrentLifeV <= 0)
			{
				CaiJiLogic.CancelCaiJiState(client);
				result = -3;
			}
			else
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, monsterId);
				if (null == monster)
				{
					CaiJiLogic.CancelCaiJiState(client);
					result = -1;
				}
				else if (monster.UniqueID != client.ClientData.CaiJiTargetUniqueID)
				{
					CaiJiLogic.CancelCaiJiState(client);
					result = -1;
				}
				else if (monster.MonsterType != 1601)
				{
					CaiJiLogic.CancelCaiJiState(client);
					result = -4;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					GetCaiJiTimeEventObject getCaiJiTimeEventObject = new GetCaiJiTimeEventObject(client, monster);
					bool flag = GlobalEventSource4Scene.getInstance().fireEvent(getCaiJiTimeEventObject, mapSceneType);
					int num;
					if (flag)
					{
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 600.0)
						{
							return -301;
						}
						num = getCaiJiTimeEventObject.GatherTime;
						if (num < 0)
						{
							return num;
						}
					}
					else if (mapSceneType == 25)
					{
						num = HuanYingSiYuanManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							return -4;
						}
					}
					else if (mapSceneType == 27)
					{
						num = YongZheZhanChangManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							return -4;
						}
					}
					else if (mapSceneType == 39)
					{
						num = KingOfBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							return -4;
						}
					}
					else if (mapSceneType == 48)
					{
						num = CompManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							return -4;
						}
					}
					else if (mapSceneType == 42)
					{
						num = KarenBattleManager_MapEast.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							return -4;
						}
					}
					else if (mapSceneType == 43)
					{
						num = LingDiCaiJiManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							CaiJiLogic.CancelCaiJiState(client);
							return num;
						}
					}
					else if (mapSceneType == 59)
					{
						num = EscapeBattleManager.getInstance().GetCaiJiMonsterTime(client, monster);
						if (num < 0)
						{
							return -4;
						}
					}
					else
					{
						if (mapSceneType == 21)
						{
							if (client.ClientData.DailyCrystalCollectNum >= CaiJiLogic.DailyNum)
							{
								CaiJiLogic.CancelCaiJiState(client);
								return -6;
							}
						}
						if (!GameManager.systemCaiJiMonsterMgr.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out systemXmlItem) || null == systemXmlItem)
						{
							CaiJiLogic.CancelCaiJiState(client);
							return -4;
						}
						num = systemXmlItem.GetIntValue("GatherTime", -1);
					}
					num = num * CaiJiLogic.GatherTimePer / 100;
					uint num2 = TimeUtil.timeGetTime() - client.ClientData.CaiJiStartTick;
					if ((ulong)num2 < (ulong)((long)(num * 1000)))
					{
						CaiJiLogic.CancelCaiJiState(client);
						LogManager.WriteLog(2, string.Format("采集读条时间不足intervalmsec={0}", num2), null, true);
						result = -5;
					}
					else
					{
						CaiJiLogic.CancelCaiJiState(client);
						if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400.0)
						{
							result = -2;
						}
						else
						{
							lock (monster.CaiJiStateLock)
							{
								if (monster.IsCollected)
								{
									return -4;
								}
								monster.IsCollected = true;
							}
							if (!GlobalEventSource4Scene.getInstance().fireEvent(new CaiJiEventObject(client, monster), mapSceneType))
							{
								if (mapSceneType == 25)
								{
									HuanYingSiYuanManager.getInstance().OnCaiJiFinish(client, monster);
								}
								else if (mapSceneType == 43)
								{
									LingDiCaiJiManager.getInstance().OnCaiJiFinish(client, monster);
								}
								else
								{
									CaiJiLogic.UpdateCaiJiData(client);
									CaiJiLogic.NotifyCollectLastNum(client, 0, CaiJiLogic.DailyNum - client.ClientData.DailyCrystalCollectNum);
									float num3 = 1f;
									int num4 = CaiJiLogic.JugeDateTimeInTimeRange(TimeUtil.NowDateTime(), CaiJiLogic.dateTimeRangeArray, true);
									if (num4 >= 0)
									{
										num3 = CaiJiLogic.dateTimeRangeArray[num4].DoubleAwardRate;
									}
									int num5 = (int)(num3 * (float)systemXmlItem.GetIntValue("ExpAward", -1));
									int num6 = (int)(num3 * (float)systemXmlItem.GetIntValue("XingHunAward", -1));
									int num7 = (int)(num3 * (float)systemXmlItem.GetIntValue("BindZuanShiAward", -1));
									int num8 = (int)(num3 * (float)systemXmlItem.GetIntValue("BindJinBiAward", -1));
									int num9 = (int)(num3 * (float)systemXmlItem.GetIntValue("MoJingAward", -1));
									if (num5 > 0)
									{
										GameManager.ClientMgr.ProcessRoleExperience(client, (long)num5, true, true, false, "none");
									}
									if (num6 > 0)
									{
										GameManager.ClientMgr.ModifyStarSoulValue(client, num6, "采集获得星魂", true, true);
									}
									if (num7 > 0)
									{
										GameManager.ClientMgr.AddUserGold(client, num7, "采集获得绑钻");
									}
									if (num8 > 0)
									{
										GameManager.ClientMgr.AddMoney1(client, num8, "采集获得绑金", true);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(28, new object[0]), new object[]
										{
											num8
										}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
									}
									if (num9 > 0)
									{
										GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, num9, "采集获得魔晶", true, true, false);
									}
									ProcessTask.ProcessAddTaskVal(client, TaskTypes.CaiJi_ShuiJingHuanJing, -1, 1, new object[0]);
								}
							}
							GameManager.MonsterMgr.DeadMonsterImmediately(monster);
							ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.CaiJiGoods, null, 0, -1L, null);
							result = 0;
						}
					}
				}
			}
			return result;
		}

		public static int CancelCaiJiState(GameClient client)
		{
			if (null != client)
			{
				client.ClientData.CaiJiStartTick = 0U;
				client.ClientData.CaijTargetId = 0;
				client.ClientData.CaijGoodsDBId = 0L;
				client.ClientData.CaiJiTargetUniqueID = 0L;
				if (client.ClientData.gatherNpcID > 0)
				{
					client.ClientData.gatherNpcID = 0;
				}
			}
			return 0;
		}

		public static int SetCaiJiState(GameClient client, int monsterId, long goodsID = 0L, long uniqueId = 0L)
		{
			if (null != client)
			{
				client.ClientData.CaiJiStartTick = TimeUtil.timeGetTime();
				client.ClientData.CaijTargetId = monsterId;
				client.ClientData.CaijGoodsDBId = goodsID;
				client.ClientData.CaiJiTargetUniqueID = uniqueId;
			}
			return 0;
		}

		public static bool IsCaiJiState(GameClient client)
		{
			return null != client && ((client.ClientData.CaiJiStartTick > 0U && (client.ClientData.CaijTargetId > 0 || client.ClientData.CaijGoodsDBId > 0L)) || client.ClientData.gatherNpcID > 0);
		}

		public static int NotifyCollectLastNum(GameClient client, int HuodongType, int lastnum)
		{
			string cmdData = string.Format("{0}:{1}:{2}", 0, HuodongType, lastnum);
			client.sendCmd(682, cmdData, false);
			return 0;
		}

		public static int ReqCaiJiLastNum(GameClient client, int huodongType, out int lastnum)
		{
			lastnum = 0;
			int result;
			if (0 == huodongType)
			{
				lastnum = CaiJiLogic.DailyNum - client.ClientData.DailyCrystalCollectNum;
				result = 0;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public static void UpdateCaiJiData(GameClient client)
		{
			client.ClientData.DailyCrystalCollectNum++;
			client._IconStateMgr.CheckCaiJiState(client);
			Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalNum", client.ClientData.DailyCrystalCollectNum, true);
			if (0 == client.ClientData.CrystalCollectDayID)
			{
				client.ClientData.CrystalCollectDayID = TimeUtil.NowDateTime().DayOfYear;
				Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalDayID", client.ClientData.CrystalCollectDayID, true);
			}
		}

		public static void InitRoleDailyCaiJiData(GameClient client, bool isLogin, bool isNewday)
		{
			if (GlobalNew.IsGongNengOpened(client, 42, false))
			{
				if (isLogin)
				{
					client.ClientData.DailyCrystalCollectNum = Global.GetRoleParamsInt32FromDB(client, "CaiJiCrystalNum");
					client.ClientData.CrystalCollectDayID = Global.GetRoleParamsInt32FromDB(client, "CaiJiCrystalDayID");
				}
				bool flag = false;
				if (isNewday)
				{
					if (client.ClientData.DailyCrystalCollectNum >= 0 && client.ClientData.CrystalCollectDayID > 0)
					{
						client.ClientData.OldCrystalCollectData = new OldCaiJiData();
						client.ClientData.OldCrystalCollectData.OldDay = client.ClientData.CrystalCollectDayID;
						client.ClientData.OldCrystalCollectData.OldNum = client.ClientData.DailyCrystalCollectNum;
					}
					flag = true;
				}
				else if (0 == client.ClientData.CrystalCollectDayID)
				{
					flag = true;
				}
				if (flag)
				{
					client.ClientData.DailyCrystalCollectNum = 0;
					client.ClientData.CrystalCollectDayID = TimeUtil.NowDateTime().DayOfYear;
					Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalNum", 0, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "CaiJiCrystalDayID", client.ClientData.CrystalCollectDayID, true);
					if (Global.GetMapSceneType(client.ClientData.MapCode) == 21)
					{
						CaiJiLogic.NotifyCollectLastNum(client, 0, CaiJiLogic.DailyNum);
					}
				}
				client._IconStateMgr.CheckCaiJiState(client);
			}
		}

		public static bool HasLeftnum(GameClient client)
		{
			return GlobalNew.IsGongNengOpened(client, 42, false) && client.ClientData.DailyCrystalCollectNum < CaiJiLogic.DailyNum;
		}

		public static CaiJiDateTimeRange[] dateTimeRangeArray = null;

		public static int DailyNum = 0;

		public static int DeadReliveTime = 0;

		public static int GatherTimePer = 100;
	}
}
