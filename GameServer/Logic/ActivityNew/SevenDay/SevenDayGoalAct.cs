using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	internal class SevenDayGoalAct
	{
		public SevenDayGoalAct()
		{
			this.evHandlerDict = new Dictionary<ESevenDayGoalFuncType, Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>>();
			this.evHandlerDict[ESevenDayGoalFuncType.RoleLevelUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_RoleLevelUp);
			this.evHandlerDict[ESevenDayGoalFuncType.SkillLevelUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_SkillLevelUp);
			this.evHandlerDict[ESevenDayGoalFuncType.MoJingCntInBag] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_MoJingCntInBag);
			this.evHandlerDict[ESevenDayGoalFuncType.RecoverMoJing] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_RecoverMoJing);
			this.evHandlerDict[ESevenDayGoalFuncType.ExchangeJinHuaJingShiByMoJing] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ExchangeJinHuaJingShiByMoJing);
			this.evHandlerDict[ESevenDayGoalFuncType.JoinJingJiChangTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JoinJingJiChangTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.WinJingJiChangTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_WinJingJiChangTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.JingJiChangRank] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JingJiChangRank);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiBlueUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiBlueUp);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiPurpleUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiPurpleUp);
			this.evHandlerDict[ESevenDayGoalFuncType.RecoverEquipBlueUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_RecoverEquipBlueUp);
			this.evHandlerDict[ESevenDayGoalFuncType.MallInSaleCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_MallInSaleCount);
			this.evHandlerDict[ESevenDayGoalFuncType.GetEquipCountByQiFu] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_GetEquipCountByQiFu);
			this.evHandlerDict[ESevenDayGoalFuncType.PickUpEquipCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PickUpEquipCount);
			this.evHandlerDict[ESevenDayGoalFuncType.EquipChuanChengTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_EquipChuanChengTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.EnterFuBenTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_EnterFuBenTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.KillMonsterInMap] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_KillMonsterInMap);
			this.evHandlerDict[ESevenDayGoalFuncType.JoinActivityTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JoinActivityTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.HeChengTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_HeChengTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.UseGoodsCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_UseGoodsCount);
			this.evHandlerDict[ESevenDayGoalFuncType.JinBiZhuanHuanTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JinBiZhuanHuanTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.BangZuanZhuanHuanTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_BangZuanZhuanHuanTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.ZuanShiZhuanHuanTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ZuanShiZhuanHuanTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.ExchangeJinHuaJingShiByQiFuScore] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ExchangeJinHuaJingShiByQiFuScore);
			this.evHandlerDict[ESevenDayGoalFuncType.CombatChange] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_CombatChange);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiForgeEquip] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiForgeEquip);
			this.evHandlerDict[ESevenDayGoalFuncType.ForgeEquipLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ForgeEquipLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.ForgeEquipTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ForgeEquipTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.CompleteChengJiu] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_CompleteChengJiu);
			this.evHandlerDict[ESevenDayGoalFuncType.ChengJiuLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ChengJiuLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.JunXianLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_JunXianLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiAppendEquip] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiAppendEquip);
			this.evHandlerDict[ESevenDayGoalFuncType.AppendEquipLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_AppendEquipLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.AppendEquipTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_AppendEquipTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.ActiveXingZuo] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_ActiveXingZuo);
			this.evHandlerDict[ESevenDayGoalFuncType.GetSpriteCountBuleUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_GetSpriteCountBuleUp);
			this.evHandlerDict[ESevenDayGoalFuncType.GetSpriteCountPurpleUp] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_GetSpriteCountPurpleUp);
			this.evHandlerDict[ESevenDayGoalFuncType.WingLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_WingLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.WingSuitStarTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_WingSuitStarTimes);
			this.evHandlerDict[ESevenDayGoalFuncType.CompleteTuJian] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_CompleteTuJian);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiSuitEquipCount] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiSuitEquipCount);
			this.evHandlerDict[ESevenDayGoalFuncType.PeiDaiSuitEquipLevel] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_PeiDaiSuitEquipLevel);
			this.evHandlerDict[ESevenDayGoalFuncType.EquipSuitUpTimes] = new Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>(this._Handle_EquipSuitUpTimes);
		}

		public void LoadConfig()
		{
			Dictionary<ESevenDayGoalFuncType, List<int>> dictionary = new Dictionary<ESevenDayGoalFuncType, List<int>>();
			Dictionary<int, SevenDayGoalAct._GoalItemConfig> dictionary2 = new Dictionary<int, SevenDayGoalAct._GoalItemConfig>();
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayGoal.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					SevenDayGoalAct._GoalItemConfig goalItemConfig = new SevenDayGoalAct._GoalItemConfig();
					goalItemConfig.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
					goalItemConfig.Day = (int)Global.GetSafeAttributeLong(xml, "Day");
					goalItemConfig.FuncType = (int)Global.GetSafeAttributeLong(xml, "FunctionType");
					string[] array = Global.GetSafeAttributeStr(xml, "TypeGoal").Split(new char[]
					{
						','
					});
					goalItemConfig.ExtCond1 = ((array.Length >= 1) ? Convert.ToInt32(array[0]) : 0);
					goalItemConfig.ExtCond2 = ((array.Length >= 2) ? Convert.ToInt32(array[1]) : 0);
					goalItemConfig.ExtCond3 = ((array.Length >= 3) ? Convert.ToInt32(array[2]) : 0);
					string[] fields = Global.GetSafeAttributeStr(xml, "Award").Split(new char[]
					{
						'|'
					});
					goalItemConfig.GoodsList = HuodongCachingMgr.ParseGoodsDataList(fields, "七日目标");
					if (!dictionary.ContainsKey((ESevenDayGoalFuncType)goalItemConfig.FuncType))
					{
						dictionary[(ESevenDayGoalFuncType)goalItemConfig.FuncType] = new List<int>();
					}
					dictionary[(ESevenDayGoalFuncType)goalItemConfig.FuncType].Add(goalItemConfig.Id);
					dictionary2.Add(goalItemConfig.Id, goalItemConfig);
				}
				lock (this.ConfigMutex)
				{
					this.Func2GoalId = dictionary;
					this.ItemConfigDict = dictionary2;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayGoal.xml"), ex, true);
			}
		}

		private int GetColor__(int ExcellencePropNum)
		{
			int result;
			if (ExcellencePropNum == 0)
			{
				result = 0;
			}
			else if (ExcellencePropNum >= 1 && ExcellencePropNum <= 2)
			{
				result = 1;
			}
			else if (ExcellencePropNum >= 3 && ExcellencePropNum <= 4)
			{
				result = 2;
			}
			else if (ExcellencePropNum >= 5 && ExcellencePropNum <= 6)
			{
				result = 3;
			}
			else
			{
				result = 4;
			}
			return result;
		}

		public bool HasAnyAwardCanGet(GameClient client, out bool[] bGoalDay)
		{
			bGoalDay = new bool[7];
			for (int i = 0; i < bGoalDay.Length; i++)
			{
				bGoalDay[i] = false;
			}
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				int num;
				if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out num))
				{
					result = false;
				}
				else
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Goal);
					if (activityData == null || activityData.Count <= 0)
					{
						result = false;
					}
					else
					{
						Dictionary<ESevenDayGoalFuncType, List<int>> dictionary = null;
						Dictionary<int, SevenDayGoalAct._GoalItemConfig> dictionary2 = null;
						lock (this.ConfigMutex)
						{
							dictionary = this.Func2GoalId;
							dictionary2 = this.ItemConfigDict;
						}
						if (dictionary == null || dictionary2 == null)
						{
							result = false;
						}
						else
						{
							lock (activityData)
							{
								foreach (KeyValuePair<int, SevenDayItemData> keyValuePair in activityData)
								{
									SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
									if (dictionary2.TryGetValue(keyValuePair.Key, out goalItemConfig))
									{
										if (goalItemConfig.Day <= num)
										{
											if (goalItemConfig.Day > 0 || goalItemConfig.Day <= bGoalDay.Length)
											{
												if (!bGoalDay[goalItemConfig.Day - 1])
												{
													if (this.CheckCanGetAward(client, keyValuePair.Value, goalItemConfig))
													{
														bGoalDay[goalItemConfig.Day - 1] = true;
														flag = true;
													}
												}
											}
										}
									}
								}
							}
							result = flag;
						}
					}
				}
			}
			return result;
		}

		public ESevenDayActErrorCode HandleGetAward(GameClient client, int id)
		{
			int num;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out num))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else
			{
				Dictionary<ESevenDayGoalFuncType, List<int>> dictionary = null;
				Dictionary<int, SevenDayGoalAct._GoalItemConfig> dictionary2 = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.Func2GoalId;
					dictionary2 = this.ItemConfigDict;
				}
				if (dictionary == null || dictionary2 == null)
				{
					result = ESevenDayActErrorCode.ServerConfigError;
				}
				else
				{
					SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
					if (!dictionary2.TryGetValue(id, out goalItemConfig) || goalItemConfig.GoodsList == null)
					{
						result = ESevenDayActErrorCode.ServerConfigError;
					}
					else if (goalItemConfig.Day > num)
					{
						result = ESevenDayActErrorCode.NotReachCondition;
					}
					else
					{
						Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Goal);
						if (activityData == null)
						{
							result = ESevenDayActErrorCode.NotReachCondition;
						}
						else
						{
							lock (activityData)
							{
								SevenDayItemData sevenDayItemData = null;
								if (!activityData.TryGetValue(id, out sevenDayItemData))
								{
									return ESevenDayActErrorCode.NotReachCondition;
								}
								if (!this.CheckCanGetAward(client, sevenDayItemData, goalItemConfig))
								{
									return ESevenDayActErrorCode.NotReachCondition;
								}
								if (!Global.CanAddGoodsNum(client, goalItemConfig.GoodsList.Count))
								{
									return ESevenDayActErrorCode.NoBagSpace;
								}
								sevenDayItemData.AwardFlag = 1;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Goal, id, sevenDayItemData, client.ServerId))
								{
									sevenDayItemData.AwardFlag = 0;
									return ESevenDayActErrorCode.DBFailed;
								}
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, new AwardItem
								{
									GoodsDataList = goalItemConfig.GoodsList
								}, ESevenDayActType.Goal))
								{
								}
							}
							result = ESevenDayActErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		private bool CheckCanGetAward(GameClient client, SevenDayItemData data, SevenDayGoalAct._GoalItemConfig itemConfig)
		{
			bool result;
			if (data == null || itemConfig == null || client == null)
			{
				result = false;
			}
			else if (data.AwardFlag != 0)
			{
				result = false;
			}
			else
			{
				switch (itemConfig.FuncType)
				{
				case 1:
				case 39:
					return data.Params1 > itemConfig.ExtCond1 || (data.Params1 == itemConfig.ExtCond1 && data.Params2 >= itemConfig.ExtCond2);
				case 2:
				case 3:
				case 9:
				case 10:
				case 26:
				case 31:
				case 32:
				case 37:
				case 38:
				case 43:
					return data.Params1 >= itemConfig.ExtCond1;
				case 4:
				case 6:
				case 7:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 22:
				case 23:
				case 24:
				case 28:
				case 29:
				case 34:
				case 35:
				case 40:
				case 44:
					return data.Params1 >= itemConfig.ExtCond1;
				case 5:
				case 17:
				case 19:
				case 20:
				case 21:
				case 25:
					return data.Params1 >= itemConfig.ExtCond2;
				case 8:
					return data.Params1 >= 1 && data.Params1 <= itemConfig.ExtCond1;
				case 18:
					return data.Params1 >= itemConfig.ExtCond3;
				case 27:
				case 33:
				case 36:
				case 42:
					return data.Params1 >= itemConfig.ExtCond2;
				case 30:
				case 41:
					return data.Params1 == 1;
				}
				result = false;
			}
			return result;
		}

		public void Update(GameClient client)
		{
			if (SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client))
			{
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RoleLevelUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.SkillLevelUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CombatChange));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteChengJiu));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ChengJiuLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JunXianLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteTuJian));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ActiveXingZuo));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetSpriteCountBuleUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetSpriteCountPurpleUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiAppendEquip));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiForgeEquip));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiBlueUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiPurpleUp));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiSuitEquipLevel));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiSuitEquipCount));
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.MallInSaleCount));
			}
		}

		public void HandleEvent(SevenDayGoalEventObject evObj)
		{
			if (evObj != null)
			{
				Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>> action = null;
				if (this.evHandlerDict.TryGetValue(evObj.FuncType, out action))
				{
					List<int> list = null;
					Dictionary<int, SevenDayGoalAct._GoalItemConfig> dictionary = null;
					lock (this.ConfigMutex)
					{
						if (!this.Func2GoalId.TryGetValue(evObj.FuncType, out list) || list.Count <= 0)
						{
							return;
						}
						if ((dictionary = this.ItemConfigDict) == null || dictionary.Count <= 0)
						{
							return;
						}
					}
					action(evObj, list, dictionary);
				}
			}
		}

		private void _Handle_RoleLevelUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						sevenDayItemData.Params1 = evObj.Client.ClientData.ChangeLifeCount;
						sevenDayItemData.Params2 = evObj.Client.ClientData.Level;
					}
				}
			}
		}

		private void _Handle_SkillLevelUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.SkillDataList != null)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int key in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(key, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.AwardFlag = 0;
								activityData[key] = sevenDayItemData;
							}
							sevenDayItemData.Params1 = 0;
						}
						bool flag2 = false;
						for (int i = 0; i < evObj.Client.ClientData.SkillDataList.Count; i++)
						{
							if (Global.GetPrevSkilID(evObj.Client.ClientData.SkillDataList[i].SkillID) <= 0)
							{
								if (evObj.Client.ClientData.SkillDataList[i].DbID == -1)
								{
									if (flag2)
									{
										goto IL_1F7;
									}
									flag2 = true;
								}
								int skillLevel = evObj.Client.ClientData.SkillDataList[i].SkillLevel;
								foreach (int key in goalIdList)
								{
									SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
									if (goalConfigDict.TryGetValue(key, out goalItemConfig))
									{
										if (skillLevel >= goalItemConfig.ExtCond2)
										{
											SevenDayItemData sevenDayItemData = null;
											if (!activityData.TryGetValue(key, out sevenDayItemData))
											{
												sevenDayItemData = new SevenDayItemData();
												sevenDayItemData.Params1 = 0;
												sevenDayItemData.AwardFlag = 0;
												activityData[key] = sevenDayItemData;
											}
											sevenDayItemData.Params1++;
										}
									}
								}
							}
							IL_1F7:;
						}
					}
				}
			}
		}

		private void _Handle_MoJingCntInBag(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int tianDiJingYuanValue = GameManager.ClientMgr.GetTianDiJingYuanValue(evObj.Client);
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							if (tianDiJingYuanValue > sevenDayItemData.Params1)
							{
								int @params = sevenDayItemData.Params1;
								sevenDayItemData.Params1 = tianDiJingYuanValue;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
								{
									sevenDayItemData.Params1 = @params;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_RecoverMoJing(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1 += evObj.Arg1;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1 -= evObj.Arg1;
							}
						}
					}
				}
			}
		}

		private void _Handle_ExchangeJinHuaJingShiByMoJing(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
							if (goalConfigDict.TryGetValue(num, out goalItemConfig))
							{
								if (goalItemConfig.ExtCond1 == evObj.Arg1)
								{
									sevenDayItemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_JoinJingJiChangTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_WinJingJiChangTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_JingJiChangRank(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg1 >= 1)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int num in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(num, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = -1;
								sevenDayItemData.AwardFlag = 0;
								activityData[num] = sevenDayItemData;
							}
							if (sevenDayItemData.AwardFlag != 1)
							{
								if (sevenDayItemData.Params1 < 1 || sevenDayItemData.Params1 > evObj.Arg1)
								{
									int @params = sevenDayItemData.Params1;
									sevenDayItemData.Params1 = evObj.Arg1;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1 = @params;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_PeiDaiBlueUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipExcellencePropNum = evObj.Client.UsingEquipMgr.GetUsingEquipExcellencePropNum();
				int @params = (usingEquipExcellencePropNum != null) ? usingEquipExcellencePropNum.Count((int _e) => this.GetColor__(_e) >= 2) : 0;
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
						if (goalConfigDict.TryGetValue(key, out goalItemConfig))
						{
							sevenDayItemData.Params1 = @params;
						}
					}
				}
			}
		}

		private void _Handle_PeiDaiPurpleUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipExcellencePropNum = evObj.Client.UsingEquipMgr.GetUsingEquipExcellencePropNum();
				int @params = (usingEquipExcellencePropNum != null) ? usingEquipExcellencePropNum.Count((int _e) => this.GetColor__(_e) >= 3) : 0;
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
						if (goalConfigDict.TryGetValue(key, out goalItemConfig))
						{
							sevenDayItemData.Params1 = @params;
						}
					}
				}
			}
		}

		private void _Handle_RecoverEquipBlueUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(evObj.Arg1);
				bool flag = false;
				if (goodsCatetoriy >= 0 && goodsCatetoriy <= 6)
				{
					flag = true;
				}
				else if (goodsCatetoriy >= 11 && goodsCatetoriy < 49)
				{
					flag = true;
				}
				if (flag)
				{
					if (this.GetColor__(evObj.Arg3) >= 2)
					{
						Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
						lock (activityData)
						{
							foreach (int num in goalIdList)
							{
								SevenDayItemData sevenDayItemData = null;
								if (!activityData.TryGetValue(num, out sevenDayItemData))
								{
									sevenDayItemData = new SevenDayItemData();
									sevenDayItemData.Params1 = 0;
									sevenDayItemData.AwardFlag = 0;
									activityData[num] = sevenDayItemData;
								}
								if (sevenDayItemData.AwardFlag != 1)
								{
									sevenDayItemData.Params1 += evObj.Arg2;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1 -= evObj.Arg2;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_MallInSaleCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.SaleGoodsDataList != null)
				{
					int num = 0;
					lock (evObj.Client.ClientData.SaleGoodsDataList)
					{
						foreach (GoodsData goodsData in evObj.Client.ClientData.SaleGoodsDataList)
						{
							if (goodsData != null)
							{
								int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
								if (goodsCatetoriy >= 0 && goodsCatetoriy <= 6)
								{
									num++;
								}
								else if (goodsCatetoriy >= 11 && goodsCatetoriy < 49)
								{
									num++;
								}
							}
						}
					}
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int key in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(key, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = 0;
								sevenDayItemData.AwardFlag = 0;
								activityData[key] = sevenDayItemData;
							}
							sevenDayItemData.Params1 = num;
						}
					}
				}
			}
		}

		private void _Handle_GetEquipCountByQiFu(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(evObj.Arg1);
				if (goodsCatetoriy < 0 || goodsCatetoriy > 6)
				{
					if (goodsCatetoriy < 11 || goodsCatetoriy >= 49)
					{
						return;
					}
				}
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1 += evObj.Arg2;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1 -= evObj.Arg2;
							}
						}
					}
				}
			}
		}

		private void _Handle_PickUpEquipCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(evObj.Arg1);
				if (goodsCatetoriy < 0 || goodsCatetoriy > 6)
				{
					if (goodsCatetoriy < 11 || goodsCatetoriy >= 49)
					{
						return;
					}
				}
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1 += evObj.Arg2;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1 -= evObj.Arg2;
							}
						}
					}
				}
			}
		}

		private void _Handle_EquipChuanChengTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_EnterFuBenTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg2 > 0)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int num in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(num, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = 0;
								sevenDayItemData.AwardFlag = 0;
								activityData[num] = sevenDayItemData;
							}
							if (sevenDayItemData.AwardFlag != 1)
							{
								SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
								if (goalConfigDict.TryGetValue(num, out goalItemConfig))
								{
									if (goalItemConfig.ExtCond1 == evObj.Arg1)
									{
										sevenDayItemData.Params1 += evObj.Arg2;
										if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
										{
											sevenDayItemData.Params1 -= evObj.Arg2;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_KillMonsterInMap(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
							if (goalConfigDict.TryGetValue(num, out goalItemConfig))
							{
								if (goalItemConfig.ExtCond1 == evObj.Arg1 && goalItemConfig.ExtCond2 == evObj.Arg2)
								{
									sevenDayItemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_JoinActivityTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
							if (goalConfigDict.TryGetValue(num, out goalItemConfig))
							{
								if (goalItemConfig.ExtCond1 == evObj.Arg1)
								{
									sevenDayItemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_HeChengTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
							if (goalConfigDict.TryGetValue(num, out goalItemConfig))
							{
								if (goalItemConfig.ExtCond1 == evObj.Arg1)
								{
									sevenDayItemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_UseGoodsCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg2 > 0)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int num in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(num, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = 0;
								sevenDayItemData.AwardFlag = 0;
								activityData[num] = sevenDayItemData;
							}
							if (sevenDayItemData.AwardFlag != 1)
							{
								SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
								if (goalConfigDict.TryGetValue(num, out goalItemConfig))
								{
									if (goalItemConfig.ExtCond1 == evObj.Arg1)
									{
										sevenDayItemData.Params1 += evObj.Arg2;
										if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
										{
											sevenDayItemData.Params1 -= evObj.Arg2;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_JinBiZhuanHuanTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_BangZuanZhuanHuanTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_ZuanShiZhuanHuanTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_ExchangeJinHuaJingShiByQiFuScore(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
							if (goalConfigDict.TryGetValue(num, out goalItemConfig))
							{
								if (goalItemConfig.ExtCond1 == evObj.Arg1)
								{
									sevenDayItemData.Params1++;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1--;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_CombatChange(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						sevenDayItemData.Params1 = evObj.Client.ClientData.CombatForce;
					}
				}
			}
		}

		private void _Handle_PeiDaiForgeEquip(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipForge = evObj.Client.UsingEquipMgr.GetUsingEquipForge();
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(key, out itemConfig))
						{
							sevenDayItemData.Params1 = ((usingEquipForge != null) ? usingEquipForge.Count((int _forge) => _forge >= itemConfig.ExtCond1) : 0);
						}
					}
				}
			}
		}

		private void _Handle_ForgeEquipLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							if (evObj.Arg1 > sevenDayItemData.Params1)
							{
								int @params = sevenDayItemData.Params1;
								sevenDayItemData.Params1 = evObj.Arg1;
								if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
								{
									sevenDayItemData.Params1 = @params;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_ForgeEquipTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_CompleteChengJiu(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
						if (goalConfigDict.TryGetValue(key, out goalItemConfig))
						{
							if (ChengJiuManager.IsChengJiuCompleted(evObj.Client, goalItemConfig.ExtCond1))
							{
								sevenDayItemData.Params1 = 1;
							}
						}
					}
				}
			}
		}

		private void _Handle_ChengJiuLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						sevenDayItemData.Params1 = ChengJiuManager.GetChengJiuLevel(evObj.Client);
					}
				}
			}
		}

		private void _Handle_JunXianLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						sevenDayItemData.Params1 = GameManager.ClientMgr.GetShengWangLevelValue(evObj.Client);
					}
				}
			}
		}

		private void _Handle_PeiDaiAppendEquip(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipAppend = evObj.Client.UsingEquipMgr.GetUsingEquipAppend();
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(key, out itemConfig))
						{
							sevenDayItemData.Params1 = ((usingEquipAppend != null) ? usingEquipAppend.Count((int _append) => _append >= itemConfig.ExtCond1) : 0);
						}
					}
				}
			}
		}

		private void _Handle_AppendEquipLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Arg1 > 0)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int num in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(num, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = 0;
								sevenDayItemData.AwardFlag = 0;
								activityData[num] = sevenDayItemData;
							}
							if (sevenDayItemData.AwardFlag != 1)
							{
								if (evObj.Arg1 > sevenDayItemData.Params1)
								{
									int @params = sevenDayItemData.Params1;
									sevenDayItemData.Params1 = evObj.Arg1;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
									{
										sevenDayItemData.Params1 = @params;
									}
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_AppendEquipTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_ActiveXingZuo(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.RoleStarConstellationInfo != null)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int key in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(key, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = 0;
								sevenDayItemData.AwardFlag = 0;
								activityData[key] = sevenDayItemData;
							}
							SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
							if (goalConfigDict.TryGetValue(key, out goalItemConfig))
							{
								int @params = 0;
								if (evObj.Client.ClientData.RoleStarConstellationInfo.TryGetValue(goalItemConfig.ExtCond1, out @params))
								{
									sevenDayItemData.Params1 = @params;
								}
							}
						}
					}
				}
			}
		}

		private void _Handle_GetSpriteCountBuleUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int spriteCount = 0;
				if (evObj.Client.ClientData.DamonGoodsDataList != null)
				{
					evObj.Client.ClientData.DamonGoodsDataList.ForEach(delegate(GoodsData _sprite)
					{
						if (_sprite.Site == 5000 && Global.GetGoodsColorEx(_sprite) >= 2)
						{
							spriteCount++;
						}
					});
				}
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1 = spriteCount;
						}
					}
				}
			}
		}

		private void _Handle_GetSpriteCountPurpleUp(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				int spriteCount = 0;
				if (evObj.Client.ClientData.DamonGoodsDataList != null)
				{
					evObj.Client.ClientData.DamonGoodsDataList.ForEach(delegate(GoodsData _sprite)
					{
						if (_sprite.Site == 5000 && Global.GetGoodsColorEx(_sprite) >= 3)
						{
							spriteCount++;
						}
					});
				}
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1 = spriteCount;
						}
					}
				}
			}
		}

		private void _Handle_WingLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.MyWingData != null)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int key in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(key, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.AwardFlag = 0;
								activityData[key] = sevenDayItemData;
							}
							sevenDayItemData.Params1 = evObj.Client.ClientData.MyWingData.WingID;
							sevenDayItemData.Params2 = evObj.Client.ClientData.MyWingData.ForgeLevel;
						}
					}
				}
			}
		}

		private void _Handle_WingSuitStarTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				if (evObj.Client.ClientData.MyWingData != null)
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
					lock (activityData)
					{
						foreach (int num in goalIdList)
						{
							SevenDayItemData sevenDayItemData = null;
							if (!activityData.TryGetValue(num, out sevenDayItemData))
							{
								sevenDayItemData = new SevenDayItemData();
								sevenDayItemData.Params1 = 0;
								sevenDayItemData.AwardFlag = 0;
								activityData[num] = sevenDayItemData;
							}
							else if (sevenDayItemData.AwardFlag == 1)
							{
							}
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private void _Handle_CompleteTuJian(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
						if (goalConfigDict.TryGetValue(key, out goalItemConfig))
						{
							if (evObj.Client.ClientData.ActivedTuJianItem != null && evObj.Client.ClientData.ActivedTuJianItem.Contains(goalItemConfig.ExtCond1))
							{
								sevenDayItemData.Params1 = 1;
							}
							else
							{
								sevenDayItemData.Params1 = 0;
							}
						}
					}
				}
			}
		}

		private void _Handle_PeiDaiSuitEquipCount(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipSuit = evObj.Client.UsingEquipMgr.GetUsingEquipSuit();
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig itemConfig = null;
						if (goalConfigDict.TryGetValue(key, out itemConfig))
						{
							sevenDayItemData.Params1 = ((usingEquipSuit != null) ? usingEquipSuit.Count((int _suit) => _suit >= itemConfig.ExtCond1) : 0);
						}
					}
				}
			}
		}

		private void _Handle_PeiDaiSuitEquipLevel(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipSuit = evObj.Client.UsingEquipMgr.GetUsingEquipSuit();
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int key in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(key, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[key] = sevenDayItemData;
						}
						SevenDayGoalAct._GoalItemConfig goalItemConfig = null;
						if (goalConfigDict.TryGetValue(key, out goalItemConfig))
						{
							sevenDayItemData.Params1 = ((usingEquipSuit != null && usingEquipSuit.Count > 0) ? usingEquipSuit.Max() : 0);
						}
					}
				}
			}
		}

		private void _Handle_EquipSuitUpTimes(SevenDayGoalEventObject evObj, List<int> goalIdList, Dictionary<int, SevenDayGoalAct._GoalItemConfig> goalConfigDict)
		{
			if (evObj != null && evObj.Client != null)
			{
				List<int> usingEquipSuit = evObj.Client.UsingEquipMgr.GetUsingEquipSuit();
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(evObj.Client, ESevenDayActType.Goal);
				lock (activityData)
				{
					foreach (int num in goalIdList)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(num, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							sevenDayItemData.Params1 = 0;
							sevenDayItemData.AwardFlag = 0;
							activityData[num] = sevenDayItemData;
						}
						if (sevenDayItemData.AwardFlag != 1)
						{
							sevenDayItemData.Params1++;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(evObj.Client.ClientData.RoleID, ESevenDayActType.Goal, num, sevenDayItemData, evObj.Client.ServerId))
							{
								sevenDayItemData.Params1--;
							}
						}
					}
				}
			}
		}

		private Dictionary<ESevenDayGoalFuncType, Action<SevenDayGoalEventObject, List<int>, Dictionary<int, SevenDayGoalAct._GoalItemConfig>>> evHandlerDict = null;

		private object ConfigMutex = new object();

		private Dictionary<ESevenDayGoalFuncType, List<int>> Func2GoalId = null;

		private Dictionary<int, SevenDayGoalAct._GoalItemConfig> ItemConfigDict = null;

		private class _GoalItemConfig
		{
			public int Id;

			public int Day;

			public int FuncType;

			public List<GoodsData> GoodsList;

			public int ExtCond1;

			public int ExtCond2;

			public int ExtCond3;
		}
	}
}
