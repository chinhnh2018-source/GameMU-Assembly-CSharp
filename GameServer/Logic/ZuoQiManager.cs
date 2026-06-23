using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class ZuoQiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static ZuoQiManager getInstance()
		{
			return ZuoQiManager.instance;
		}

		public bool initialize()
		{
			this.ReLoadConfig(true);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1896, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1897, 3, 3, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1899, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1900, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1901, 2, 2, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1898, 2, 2, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1902, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1903, 1, 1, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2094, 2, 2, ZuoQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1896:
					result = this.ProcessZuoQiMainInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 1897:
					result = this.ProcessZuoQiLieQuCmd(client, nID, bytes, cmdParams);
					break;
				case 1898:
					result = this.ProcessZuoQiUpGradeCmd(client, nID, bytes, cmdParams);
					break;
				case 1899:
					result = this.ProcessZuoQiRideCmd(client, nID, bytes, cmdParams);
					break;
				case 1900:
					result = this.ProcessZuoQiCheckCmd(client, nID, bytes, cmdParams);
					break;
				case 1901:
					result = this.ProcessZuoQiSkillModCmd(client, nID, bytes, cmdParams);
					break;
				case 1902:
					result = this.ProcessZuoQiUpLevelCmd(client, nID, bytes, cmdParams);
					break;
				case 1903:
					result = this.ProcessResetMountBagCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = (nID != 2094 || this.ProcessResetMountGradeCmd(client, nID, bytes, cmdParams));
					break;
				}
			}
			return result;
		}

		public bool ProcessZuoQiMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				ZuoQiMainData cmdData = new ZuoQiMainData
				{
					MountList = client.ClientData.MountList,
					NextFreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10205"),
					MountLevel = Global.GetRoleParamsInt32FromDB(client, "10207")
				};
				client.sendCmd<ZuoQiMainData>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 获取坐骑主页面信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessZuoQiLieQuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[1]);
				int num2 = Convert.ToInt32(cmdParams[2]);
				int result = 0;
				DateTime dateTime = Global.GetRoleParamsDateTimeFromDB(client, "10205");
				List<ZuoQiMini> list = new List<ZuoQiMini>();
				List<int> list2 = new List<int>();
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				List<MountRandomItem> mountList;
				List<MountRandomItem> mountList2;
				List<MountRandomItem> mountList3;
				if (specPriorityActivity != null && specPriorityActivity.IsChouJiangOpen(SpecPActivityChouJiangType.TeQuanShouLie))
				{
					mountList = this.ZuoQiRunTimeData.MountFreeRandomListTeQuan;
					mountList2 = this.ZuoQiRunTimeData.MountRandomListTeQuan;
					mountList3 = this.ZuoQiRunTimeData.MountPayRandomListTeQuan;
				}
				else
				{
					mountList = this.ZuoQiRunTimeData.MountFreeRandomList;
					mountList2 = this.ZuoQiRunTimeData.MountRandomList;
					mountList3 = this.ZuoQiRunTimeData.MountPayRandomList;
				}
				if (num < 0 || num > 1)
				{
					result = 1;
				}
				else
				{
					int num3 = (num == 0) ? 1 : 10;
					if (!ZuoQiManager.CanAddGoodsNum(client, num3))
					{
						result = 2;
					}
					else
					{
						DateTime t = TimeUtil.NowDateTime();
						if (num3 == 1)
						{
							if (dateTime < t)
							{
								ZuoQiMini randomMount = this.GetRandomMount(mountList);
								if (null == randomMount)
								{
									result = 3;
									goto IL_4CD;
								}
								randomMount.Binding = 1;
								list.Add(randomMount);
								dateTime = t.AddSeconds((double)this.ZuoQiFreeTime);
								Global.SaveRoleParamsDateTimeToDB(client, "10205", dateTime, true);
							}
							else
							{
								ZuoQiMini randomMount = this.GetRandomMount(mountList2);
								if (null == randomMount)
								{
									result = 3;
									goto IL_4CD;
								}
								if (num2 != this.ZuoQiChouQuCost)
								{
									result = 14;
									goto IL_4CD;
								}
								if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.ZuoQiChouQuCost, "坐骑猎取_钻石(改幸运之星)", false, DaiBiSySType.ZuoQiBuHuo))
								{
									result = 4;
									goto IL_4CD;
								}
								GameManager.ClientMgr.ModifyMountPointValue(client, (int)(this.ConsumeHuntHorseJiFen * (double)this.ZuoQiChouQuCost), "坐骑猎取获得积分", true, true, false);
								list.Add(randomMount);
							}
						}
						else
						{
							if (num2 != this.ZuoQiChouQuCost_10)
							{
								result = 14;
								goto IL_4CD;
							}
							ZuoQiMini randomMount;
							for (int i = 0; i < num3 - 1; i++)
							{
								randomMount = this.GetRandomMount(mountList2);
								if (null == randomMount)
								{
									break;
								}
								list.Add(randomMount);
							}
							randomMount = this.GetRandomMount(mountList3);
							if (null != randomMount)
							{
								list.Add(randomMount);
							}
							if (list.Count < num3)
							{
								result = 3;
								goto IL_4CD;
							}
							ZuoQiMini value = list[num3 - 1];
							int randomNumber = Global.GetRandomNumber(0, num3);
							list[num3 - 1] = list[randomNumber];
							list[randomNumber] = value;
							if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.ZuoQiChouQuCost_10, "坐骑猎取10_钻石(改幸运之星)", false, DaiBiSySType.ZuoQiBuHuo))
							{
								result = -17;
								goto IL_4CD;
							}
							GameManager.ClientMgr.ModifyMountPointValue(client, (int)(this.ConsumeHuntHorseJiFen * (double)this.ZuoQiChouQuCost_10), "坐骑猎取获得积分", true, true, false);
						}
						foreach (ZuoQiMini zuoQiMini in list)
						{
							int num4 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, zuoQiMini.GoodsID, 1, 0, "", 0, zuoQiMini.Binding, 12000, "", true, 1, "坐骑猎取抽取", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, zuoQiMini.WashProps, null, 0, true);
							if (this.ZuoQiRunTimeData.HorseNotice.Contains(zuoQiMini.GoodsID))
							{
								int num5 = 0;
								foreach (GoodsData goodsData in client.ClientData.MountStoreList)
								{
									if (goodsData.Id == num4 && null != goodsData.WashProps)
									{
										num5 = goodsData.WashProps.Count / 2;
										break;
									}
								}
								string msgText = string.Format(GLang.GetLang(5004, new object[0]), Global.FormatRoleName4(client), num5, Global.GetGoodsNameByID(zuoQiMini.GoodsID));
								Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
							}
							list2.Add(num4);
						}
						client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
						{
							default(DelayExecProcIds),
							2
						});
					}
				}
				IL_4CD:
				ZuoQiChouQuResult cmdData = new ZuoQiChouQuResult
				{
					Result = result,
					GoodsList = string.Join<int>(",", list2),
					FreeTime = dateTime
				};
				client.sendCmd<ZuoQiChouQuResult>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 坐骑猎取信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessZuoQiRideCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (ZuoQiManager.CanRide(client) != 1)
				{
					return true;
				}
				if (client.ClientData.DisMountTick + 2000U > TimeUtil.timeGetTime())
				{
					return true;
				}
				client.ClientData.IsRide = 1;
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MountIsRide, 1);
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					PropsSystemTypes.ZuoQi,
					2,
					this.GetSpeedAdd(client)
				});
				client.ClientData.MoveSpeed = RoleAlgorithm.GetMoveSpeed(client);
				GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 14, 0L, 0, client.ClientData.MoveSpeed);
				string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 52, 1);
				client.sendOthersCmd(427, cmdData);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 处理角色上马信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public void RoleRideMount(GameClient client)
		{
			if (ZuoQiManager.CanRide(client) == 1)
			{
				if (client.ClientData.DisMountTick + 2000U <= TimeUtil.timeGetTime())
				{
					client.ClientData.IsRide = 1;
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MountIsRide, 1);
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						PropsSystemTypes.ZuoQi,
						2,
						this.GetSpeedAdd(client)
					});
					client.ClientData.MoveSpeed = RoleAlgorithm.GetMoveSpeed(client);
					GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 14, 0L, 0, client.ClientData.MoveSpeed);
					string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 52, 1);
					client.sendOthersCmd(427, cmdData);
				}
			}
		}

		public bool ProcessZuoQiCheckCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (null == client.ClientData.MountList)
				{
					return true;
				}
				foreach (MountData mountData in client.ClientData.MountList)
				{
					mountData.IsNew = false;
				}
				Global.sendToDB<int, string>(20321, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 处理查看坐骑图鉴信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessZuoQiSkillModCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10206");
				int num = Convert.ToInt32(cmdParams[1]);
				int num2 = 12;
				if (null == client.ClientData.MountEquipList)
				{
					num2 = 12;
					num = roleParamsInt32FromDB;
				}
				else
				{
					foreach (GoodsData goodsData in client.ClientData.MountEquipList)
					{
						AdvancedItem advancedItem = null;
						Dictionary<int, AdvancedItem> dictionary;
						if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out dictionary))
						{
							if (dictionary.TryGetValue(goodsData.Forge_level, out advancedItem))
							{
								if (advancedItem.SkillID == num)
								{
									Global.SaveRoleParamsInt32ValueToDB(client, "10206", num, true);
									num2 = 0;
									break;
								}
							}
						}
					}
				}
				if (num2 != 0)
				{
					num = roleParamsInt32FromDB;
				}
				ExtData clientExtData = ExtDataManager.GetClientExtData(client);
				long num3 = clientExtData.ZuoQiSkillCDTicks - clientExtData.ZuoQiSkillCdTime;
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, num, num3), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 处理更改角色坐骑技能信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessZuoQiUpGradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				GoodsData goodsData = null;
				int id = Convert.ToInt32(cmdParams[1]);
				goodsData = ZuoQiManager.GetMountEquipGoodsDataByDbID(client, id);
				if (null == goodsData)
				{
					num = 9;
				}
				else
				{
					AdvancedItem advancedItem = null;
					Dictionary<int, AdvancedItem> dictionary;
					if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out dictionary))
					{
						num = 11;
					}
					else if (!dictionary.TryGetValue(goodsData.Forge_level + 1, out advancedItem))
					{
						num = 10;
					}
					else
					{
						foreach (List<int> list in advancedItem.NeedGoods)
						{
							if (Global.GetTotalGoodsCountByID(client, list[0]) < list[1])
							{
								num = 7;
								break;
							}
						}
						if (num != 7)
						{
							foreach (List<int> list in advancedItem.NeedGoods)
							{
								int goodsID = list[0];
								int i = list[1];
								while (i > 0)
								{
									GoodsData goodsByID = Global.GetGoodsByID(client, goodsID);
									if (null == goodsByID)
									{
										break;
									}
									int num4 = (i > goodsByID.GCount) ? goodsByID.GCount : i;
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByID, num4, false, false))
									{
										num = 8;
										break;
									}
									i -= num4;
									if (i <= 0 || num != 0)
									{
										break;
									}
								}
								if (i > 0)
								{
									if (num == 0)
									{
										num = 7;
									}
									GameManager.logDBCmdMgr.AddMessageLog(-1, "操作日志", "坐骑升阶", client.ClientData.RoleName, "材料不足升阶失败", "消耗", client.ClientData.RoleID, client.ClientData.ZoneID, client.strUserID, 0, GameManager.ServerId, "");
									break;
								}
							}
							if (num != 7)
							{
								int num5 = 1;
								string[] array = null;
								string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
								{
									client.ClientData.RoleID,
									goodsData.Id,
									"*",
									goodsData.Forge_level + 1,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									num5,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*"
								});
								TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
								if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED || array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
								{
									num = 8;
								}
								else
								{
									int @using = goodsData.Using;
									if (goodsData.Using > 0)
									{
										goodsData.Using = 0;
										Global.RefreshEquipProp(client, goodsData);
									}
									num3 = this.DelMountSkill(client, goodsData, false);
									goodsData.Forge_level++;
									goodsData.Binding = num5;
									num2 = goodsData.Forge_level;
									if (@using != goodsData.Using)
									{
										goodsData.Using = @using;
										if (Global.RefreshEquipProp(client, goodsData))
										{
										}
									}
									client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
									{
										default(DelayExecProcIds),
										2
									});
									Global.ModRoleGoodsEvent(client, goodsData, 0, "强化", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "强化");
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					cmdParams[1],
					num2,
					goodsData.Binding
				}), false);
				this.AddMountSkill(client, goodsData, num3 == Global.GetRoleParamsInt32FromDB(client, "10206"));
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 处理更改坐骑升阶信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessZuoQiUpLevelCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Global.GetRoleParamsInt32FromDB(client, "10207");
				int num2 = 0;
				int num3 = 0;
				int unionLevel = Global.GetUnionLevel(client, false);
				foreach (KeyValuePair<int, int> keyValuePair in this.Level2UpLevel)
				{
					if (unionLevel <= keyValuePair.Key)
					{
						break;
					}
					num3 = keyValuePair.Value;
				}
				LevelUpItem levelUpItem;
				if (num3 <= num + 1)
				{
					num2 = 5;
				}
				else if (!this.ZuoQiRunTimeData.LevelUpDict.TryGetValue(num + 1, out levelUpItem))
				{
					num2 = 5;
				}
				else if ((long)levelUpItem.Exp > client.ClientData.HunJing)
				{
					num2 = 6;
				}
				else if (!GameManager.ClientMgr.ModifyHunJingValue(client, -levelUpItem.Exp, "坐骑栏升级消耗", true, true, false))
				{
					num2 = 6;
				}
				else
				{
					num++;
					Global.SaveRoleParamsInt32ValueToDB(client, "10207", num, true);
					client.ClientData.ZuoQiMainData.MountLevel = num;
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						default(DelayExecProcIds),
						2
					});
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num2, num), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 处理更改坐骑栏升级信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessResetMountBagCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (null != client.ClientData.MountStoreList)
				{
					lock (client.ClientData.MountStoreList)
					{
						Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
						List<GoodsData> list = new List<GoodsData>();
						for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
						{
							client.ClientData.MountStoreList[i].BagIndex = 1;
							int goodsGridNumByID = Global.GetGoodsGridNumByID(client.ClientData.MountStoreList[i].GoodsID);
							if (goodsGridNumByID > 1)
							{
								GoodsData goodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.MountStoreList[i].GoodsID, client.ClientData.MountStoreList[i].Binding, Global.DateTimeTicks(client.ClientData.MountStoreList[i].Endtime));
								if (dictionary.TryGetValue(key, out goodsData))
								{
									int num = Global.GMin(goodsGridNumByID - goodsData.GCount, client.ClientData.MountStoreList[i].GCount);
									goodsData.GCount += num;
									client.ClientData.MountStoreList[i].GCount -= num;
									client.ClientData.MountStoreList[i].BagIndex = 1;
									goodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.MountStoreList[i]))
									{
										break;
									}
									if (goodsData.GCount >= goodsGridNumByID)
									{
										if (client.ClientData.MountStoreList[i].GCount > 0)
										{
											dictionary[key] = client.ClientData.MountStoreList[i];
										}
										else
										{
											dictionary.Remove(key);
											list.Add(client.ClientData.MountStoreList[i]);
										}
									}
									else if (client.ClientData.MountStoreList[i].GCount <= 0)
									{
										list.Add(client.ClientData.MountStoreList[i]);
									}
								}
								else
								{
									dictionary[key] = client.ClientData.MountStoreList[i];
								}
							}
						}
						for (int i = 0; i < list.Count; i++)
						{
							client.ClientData.MountStoreList.Remove(list[i]);
						}
						client.ClientData.MountStoreList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
						int num2 = 0;
						for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
						{
							client.ClientData.MountStoreList[i].BagIndex = num2++;
							if (!Global.ResetBagGoodsData(client, client.ClientData.MountStoreList[i]))
							{
								break;
							}
						}
					}
				}
				client.sendCmd<List<GoodsData>>(nID, client.ClientData.MountStoreList, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 处理整理坐骑仓库信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessResetMountGradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int dbID = Convert.ToInt32(cmdParams[1]);
				int num2 = 0;
				GoodsData goodsData = null;
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null == goodsData)
				{
					num = 9;
				}
				else
				{
					Dictionary<int, int> zuoQiNeedGoodsNumForCurrLevel = this.GetZuoQiNeedGoodsNumForCurrLevel(goodsData.GoodsID, goodsData.Forge_level);
					if (zuoQiNeedGoodsNumForCurrLevel == null)
					{
						num = 3;
					}
					else
					{
						Dictionary<int, int> dictionary = new Dictionary<int, int>();
						double zuoQiResetRate = this.GetZuoQiResetRate();
						foreach (KeyValuePair<int, int> keyValuePair in zuoQiNeedGoodsNumForCurrLevel)
						{
							if (dictionary.ContainsKey(keyValuePair.Key))
							{
								Dictionary<int, int> dictionary2;
								int key;
								(dictionary2 = dictionary)[key = keyValuePair.Key] = dictionary2[key] + (int)Math.Floor(zuoQiResetRate * (double)keyValuePair.Value);
							}
							else
							{
								dictionary.Add(keyValuePair.Key, (int)Math.Floor(zuoQiResetRate * (double)keyValuePair.Value));
							}
						}
						if (!ZuoQiManager.CanAddGoodsNum(client, dictionary.Count))
						{
							num = 2;
						}
						else
						{
							bool flag = false;
							foreach (KeyValuePair<int, int> keyValuePair in dictionary)
							{
								if (!Global.CanAddGoods2(client, keyValuePair.Key, keyValuePair.Value, 1, "1900-01-01 12:00:00", true))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								num = 2;
							}
							else
							{
								int num3 = 1;
								string[] array = null;
								string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
								{
									client.ClientData.RoleID,
									goodsData.Id,
									"*",
									num2,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									num3,
									"*",
									"*",
									"*",
									"*",
									"*",
									"*",
									"*"
								});
								TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
								if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED || array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
								{
									num = 8;
								}
								else
								{
									foreach (KeyValuePair<int, int> keyValuePair in dictionary)
									{
										Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, keyValuePair.Key, keyValuePair.Value, 0, "", 0, num3, 0, "", true, 1, string.Format("坐骑重置", new object[0]), false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
									}
									if (goodsData.Binding != 1)
									{
										goodsData.Binding = num3;
									}
									goodsData.Forge_level = num2;
									Global.ModRoleGoodsEvent(client, goodsData, 0, "坐骑重置", false);
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					cmdParams[1],
					goodsData.Forge_level,
					goodsData.Binding
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 重置坐骑信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public static GoodsData AddZuoQiGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = 1,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			if (null == client.ClientData.MountStoreList)
			{
				client.ClientData.MountStoreList = new List<GoodsData>();
			}
			lock (client.ClientData.MountStoreList)
			{
				client.ClientData.MountStoreList.Add(goodsData);
			}
			if (null == client.ClientData.MountList)
			{
				client.ClientData.MountList = new List<MountData>();
			}
			foreach (MountData mountData in client.ClientData.MountList)
			{
				if (mountData.GoodsID == goodsData.GoodsID)
				{
					return goodsData;
				}
			}
			GoodsData result;
			if (!ZuoQiManager.CheckIsZuoQiByGoodsID(goodsData.GoodsID))
			{
				result = goodsData;
			}
			else
			{
				client.ClientData.MountList.Add(new MountData
				{
					GoodsID = goodsData.GoodsID,
					IsNew = true
				});
				Global.sendToDB<int, string>(20320, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, goodsData.GoodsID, 1), client.ServerId);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
				result = goodsData;
			}
			return result;
		}

		public static void ProcessZuoQiTuJian(GameClient client, int goodsId)
		{
			if (ZuoQiManager.CheckIsZuoQiByGoodsID(goodsId))
			{
				if (null == client.ClientData.MountList)
				{
					client.ClientData.MountList = new List<MountData>();
				}
				List<MountData> mountList = client.ClientData.MountList;
				if (!mountList.Any((MountData x) => x.GoodsID == goodsId))
				{
					mountList.Add(new MountData
					{
						GoodsID = goodsId,
						IsNew = true
					});
				}
			}
		}

		public static GoodsData GetMountFromEquipListByID(GameClient client, int goodsDbID)
		{
			GoodsData result;
			if (null == client.ClientData.MountEquipList)
			{
				result = null;
			}
			else
			{
				foreach (GoodsData goodsData in client.ClientData.MountEquipList)
				{
					if (goodsData.Id == goodsDbID)
					{
						return goodsData;
					}
				}
				result = null;
			}
			return result;
		}

		public static int GetIdleSlotOfZuoQiStoreGoods(GameClient client)
		{
			int num = 0;
			int result;
			if (null == client.ClientData.MountStoreList)
			{
				result = num;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
				{
					if (list.IndexOf(client.ClientData.MountStoreList[i].BagIndex) < 0)
					{
						list.Add(client.ClientData.MountStoreList[i].BagIndex);
					}
				}
				for (int j = 0; j < ZuoQiManager.GetMaxMountCount(); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetIdleSlotOfZuoQiEquipGoods(GameClient client)
		{
			int num = 0;
			int result;
			if (null == client.ClientData.MountEquipList)
			{
				result = num;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.MountEquipList.Count; i++)
				{
					if (list.IndexOf(client.ClientData.MountEquipList[i].BagIndex) < 0)
					{
						list.Add(client.ClientData.MountEquipList[i].BagIndex);
					}
				}
				for (int j = 0; j < ZuoQiManager.GetMaxMountEquipCount(); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public static bool RemoveStoreGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.MountStoreList == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				lock (client.ClientData.MountStoreList)
				{
					flag = client.ClientData.MountStoreList.Remove(gd);
				}
				result = flag;
			}
			return result;
		}

		public static bool RemoveEquipGoodsData(GameClient client, GoodsData gd)
		{
			bool result;
			if (null == gd)
			{
				result = false;
			}
			else if (client.ClientData.MountEquipList == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				lock (client.ClientData.MountEquipList)
				{
					flag = client.ClientData.MountEquipList.Remove(gd);
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
				ZuoQiManager.getInstance().DelMountSkill(client, gd, true);
				result = flag;
			}
			return result;
		}

		public List<int> GetZuoQiSkillList(GameClient client)
		{
			List<int> list = new List<int>();
			lock (this.ZuoQiRunTimeData.Mutex)
			{
				if (null == client.ClientData.MountEquipList)
				{
					return list;
				}
				foreach (GoodsData goodsData in client.ClientData.MountEquipList)
				{
					AdvancedItem advancedItem = null;
					Dictionary<int, AdvancedItem> dictionary;
					if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out dictionary))
					{
						if (dictionary.TryGetValue(goodsData.Forge_level, out advancedItem))
						{
							list.Add(advancedItem.SkillID);
						}
					}
				}
			}
			return list;
		}

		public static void AddMountEquipGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 0 || goodsData.Site == 12000)
			{
				if (null == client.ClientData.MountEquipList)
				{
					client.ClientData.MountEquipList = new List<GoodsData>();
				}
				lock (client.ClientData.MountEquipList)
				{
					client.ClientData.MountEquipList.Add(goodsData);
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
				ZuoQiManager.getInstance().AddMountSkill(client, goodsData, false);
			}
		}

		public void RefreshProps(GameClient client)
		{
			try
			{
				double[] array = new double[177];
				if (null != client.ClientData.MountList)
				{
					foreach (MountData mountData in client.ClientData.MountList)
					{
						PokedexItem pokedexItem;
						if (this.ZuoQiRunTimeData.PokedexDict.TryGetValue(mountData.GoodsID, out pokedexItem))
						{
							for (int i = 0; i < 177; i++)
							{
								array[i] += pokedexItem.PokedexAttribute[i];
							}
						}
					}
					int num = Global.GetRoleParamsInt32FromDB(client, "10207") + 1;
					int num2 = 0;
					int num3 = 0;
					foreach (GoodsData goodsData in client.ClientData.MountEquipList)
					{
						if (null != goodsData.WashProps)
						{
							num2 += goodsData.WashProps.Count / 2;
						}
						num3 += goodsData.Forge_level + 1;
					}
					List<ArrayAdditionItem> list;
					if (this.ZuoQiRunTimeData.ArrayAdditiionDict.TryGetValue(1, out list))
					{
						foreach (ArrayAdditionItem arrayAdditionItem in list)
						{
							if (num >= arrayAdditionItem.NeedLevel)
							{
								foreach (KeyValuePair<int, double> keyValuePair in arrayAdditionItem.AdditionProps)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								break;
							}
						}
					}
					if (this.ZuoQiRunTimeData.ArrayAdditiionDict.TryGetValue(2, out list))
					{
						foreach (ArrayAdditionItem arrayAdditionItem in list)
						{
							if (num2 >= arrayAdditionItem.NeedSuperiorNum)
							{
								foreach (KeyValuePair<int, double> keyValuePair in arrayAdditionItem.AdditionProps)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								break;
							}
						}
					}
					if (this.ZuoQiRunTimeData.ArrayAdditiionDict.TryGetValue(3, out list))
					{
						foreach (ArrayAdditionItem arrayAdditionItem in list)
						{
							if (num3 >= arrayAdditionItem.NeedOrderNum)
							{
								foreach (KeyValuePair<int, double> keyValuePair in arrayAdditionItem.AdditionProps)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								break;
							}
						}
					}
					List<int> collection = client.ClientData.MountEquipList.ConvertAll<int>((GoodsData _g) => _g.GoodsID);
					foreach (SuitItem suitItem in this.ZuoQiRunTimeData.SuitList)
					{
						bool flag = true;
						List<int> list2 = new List<int>();
						list2.AddRange(collection);
						foreach (int item in suitItem.HorseIDList)
						{
							if (!list2.Remove(item))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							foreach (KeyValuePair<int, double> keyValuePair2 in suitItem.HorseSuitProps)
							{
								array[keyValuePair2.Key] += keyValuePair2.Value;
							}
						}
					}
				}
				double[] array2 = new double[177];
				double[] array3 = new double[177];
				double[] array4 = new double[177];
				if (null != client.ClientData.GoodsDataList)
				{
					List<GoodsData> list3 = new List<GoodsData>();
					list3.AddRange(client.ClientData.GoodsDataList);
					lock (this.ZuoQiRunTimeData.Mutex)
					{
						for (int i = 0; i < this.ZuoQiRunTimeData.HorseEquipAdditionItemList.Count; i++)
						{
							HorseEquipAdditionItem horseEquipAdditionItem = this.ZuoQiRunTimeData.HorseEquipAdditionItemList[i];
							int num4 = 0;
							int num5 = 0;
							int num6 = 0;
							foreach (GoodsData goodsData2 in list3)
							{
								if (goodsData2.Using > 0 && GoodsUtil.IsZuoQiEquip(goodsData2.GoodsID))
								{
									num4 += goodsData2.Forge_level;
									num5 += goodsData2.AppendPropLev;
									num6 += Global.GetEquipGoodsSuitID(goodsData2.GoodsID);
								}
							}
							if (horseEquipAdditionItem.Type == 1 && num4 >= horseEquipAdditionItem.NeedStrengthenLevel)
							{
								array2 = horseEquipAdditionItem.ExtProps;
							}
							else if (horseEquipAdditionItem.Type == 2 && num5 >= horseEquipAdditionItem.NeedAdditionLevel)
							{
								array3 = horseEquipAdditionItem.ExtProps;
							}
							else if (horseEquipAdditionItem.Type == 3 && num6 >= horseEquipAdditionItem.NeedOrderNum)
							{
								array4 = horseEquipAdditionItem.ExtProps;
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQiEquip,
					1,
					array2
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQiEquip,
					2,
					array3
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQiEquip,
					3,
					array4
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ZuoQi,
					array
				});
				if (client.ClientData.IsRide > 0)
				{
					this.RoleRideMount(client);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 更新角色坐骑属性加成失败，rid={0}。", client.ClientData.RoleID), ex, true);
			}
		}

		public double GetExtpropsAddPercent(GameClient client, GoodsData goods)
		{
			double num = 0.0;
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10207");
			LevelUpItem levelUpItem;
			if (this.ZuoQiRunTimeData.LevelUpDict.TryGetValue(roleParamsInt32FromDB, out levelUpItem))
			{
				num += levelUpItem.AdvancedEffect;
			}
			Dictionary<int, AdvancedItem> dictionary;
			if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goods.GoodsID, out dictionary))
			{
				AdvancedItem advancedItem;
				if (dictionary.TryGetValue(goods.Forge_level, out advancedItem))
				{
					num += advancedItem.AdvancedEffect;
				}
			}
			return num;
		}

		public void AddMountSkill(GameClient client, GoodsData goodsData, bool isEquip = false)
		{
			if (null != goodsData)
			{
				AdvancedItem advancedItem = null;
				Dictionary<int, AdvancedItem> dictionary;
				if (this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out dictionary))
				{
					if (dictionary.TryGetValue(goodsData.Forge_level, out advancedItem))
					{
						Global.AddSkillData(client, -1, advancedItem.SkillID, 0);
						if (isEquip)
						{
							Global.SaveRoleParamsInt32ValueToDB(client, "10206", advancedItem.SkillID, true);
							ExtData clientExtData = ExtDataManager.GetClientExtData(client);
							long num = clientExtData.ZuoQiSkillCDTicks - clientExtData.ZuoQiSkillCdTime;
							client.sendCmd(1901, string.Format("{0}:{1}:{2}", 0, advancedItem.SkillID, num), false);
						}
					}
				}
			}
		}

		public int DelMountSkill(GameClient client, GoodsData goodsData, bool notifyClient = true)
		{
			int result;
			if (null == goodsData)
			{
				result = 0;
			}
			else
			{
				AdvancedItem advancedItem = null;
				Dictionary<int, AdvancedItem> dictionary;
				if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsData.GoodsID, out dictionary))
				{
					result = 0;
				}
				else if (!dictionary.TryGetValue(goodsData.Forge_level, out advancedItem))
				{
					result = 0;
				}
				else
				{
					int skillID = advancedItem.SkillID;
					Global.DelSkillData(client, skillID);
					if (notifyClient && skillID == Global.GetRoleParamsInt32FromDB(client, "10206"))
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "10206", 0, true);
						ExtData clientExtData = ExtDataManager.GetClientExtData(client);
						long num = clientExtData.ZuoQiSkillCDTicks - clientExtData.ZuoQiSkillCdTime;
						client.sendCmd(1901, string.Format("{0}:{1}:{2}", 0, 0, num), false);
					}
					result = skillID;
				}
			}
			return result;
		}

		public double GetSpeedAdd(GameClient client)
		{
			double result;
			if (null == client.ClientData.MountEquipList)
			{
				result = 0.0;
			}
			else if (!Global.CanMapRideHorse(client.ClientData.MapCode))
			{
				result = 0.0;
			}
			else
			{
				foreach (GoodsData goodsData in client.ClientData.MountEquipList)
				{
					if (goodsData.Using == 1)
					{
						PokedexItem pokedexItem;
						if (!this.ZuoQiRunTimeData.PokedexDict.TryGetValue(goodsData.GoodsID, out pokedexItem))
						{
							return 0.0;
						}
						return pokedexItem.HorseSpeed;
					}
				}
				result = 0.0;
			}
			return result;
		}

		public static int CanRide(GameClient client)
		{
			int result;
			if (null == client.ClientData.MountEquipList)
			{
				result = 0;
			}
			else if (client.buffManager.IsBuffEnabled(121))
			{
				result = 0;
			}
			else if (!Global.CanMapRideHorse(client.ClientData.MapCode))
			{
				result = 0;
			}
			else
			{
				foreach (GoodsData goodsData in client.ClientData.MountEquipList)
				{
					if (goodsData.Using == 1)
					{
						return 1;
					}
				}
				result = 0;
			}
			return result;
		}

		public static bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.MountStoreList.Count <= ZuoQiManager.GetMaxMountCount();
		}

		public static int GetMaxMountCount()
		{
			return 240;
		}

		public static int GetMaxMountEquipCount()
		{
			return 4;
		}

		public static GoodsData GetMountStoreGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.MountStoreList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.MountStoreList)
				{
					for (int i = 0; i < client.ClientData.MountStoreList.Count; i++)
					{
						if (client.ClientData.MountStoreList[i].Id == id)
						{
							return client.ClientData.MountStoreList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static GoodsData GetMountEquipGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.MountEquipList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.MountEquipList)
				{
					for (int i = 0; i < client.ClientData.MountEquipList.Count; i++)
					{
						if (client.ClientData.MountEquipList[i].Id == id)
						{
							return client.ClientData.MountEquipList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static bool CanAddGoodsToMountEquip(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.MountEquipList == null)
			{
				result = true;
			}
			else
			{
				lock (client.ClientData.MountEquipList)
				{
					result = (client.ClientData.MountEquipList.Count < ZuoQiManager.GetMaxMountEquipCount());
				}
			}
			return result;
		}

		public TCPProcessCmdResults SaleMountProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			TCPProcessCmdResults result;
			if (!GlobalNew.IsGongNengOpened(client, 97, false))
			{
				result = TCPProcessCmdResults.RESULT_OK;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				string[] array = strGoodsID.Split(new char[]
				{
					','
				});
				int i = 0;
				while (i < array.Length)
				{
					int dbID = Global.SafeConvertToInt32(array[i]);
					GoodsData goodsByDbID = Global.GetGoodsByDbID(client, dbID);
					if (goodsByDbID != null && goodsByDbID.Site == 0 && goodsByDbID.Using <= 0 && goodsByDbID.Forge_level == 0)
					{
						bool flag = ZuoQiManager.CheckIsZuoQiByGoodsID(goodsByDbID.GoodsID);
						bool flag2 = ZuoQiManager.CheckIsZuoQiEquipByGoodsID(goodsByDbID.GoodsID);
						if (flag || flag2)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem) && null != systemXmlItem)
							{
								int num4 = systemXmlItem.GetIntValue("ChangeHunJing", -1);
								int num5 = 0;
								int num6 = 0;
								if (flag)
								{
									AdvancedItem advancedItem = null;
									Dictionary<int, AdvancedItem> dictionary;
									if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodsByDbID.GoodsID, out dictionary))
									{
										goto IL_354;
									}
									if (!dictionary.TryGetValue(goodsByDbID.Forge_level, out advancedItem))
									{
										goto IL_354;
									}
									if (advancedItem.ChangeHunJing > 0)
									{
										num4 += advancedItem.ChangeHunJing;
									}
								}
								if (flag2)
								{
									double num7 = 1.0;
									if (goodsByDbID.ExcellenceInfo != 0)
									{
										int equipExcellencePropNum = Global.GetEquipExcellencePropNum(goodsByDbID);
										if (equipExcellencePropNum != 0)
										{
											double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueHuiShouXiShu", ',');
											if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length >= equipExcellencePropNum)
											{
												num7 = paramValueDoubleArrayByName[equipExcellencePropNum - 1];
											}
										}
									}
									num5 += (int)((double)systemXmlItem.GetIntValue("ChangeJinYuan", -1) * num7);
									num6 += systemXmlItem.GetIntValue("ChangeZaiZao", -1);
									if (num6 > 0)
									{
										if (!GlobalNew.IsGongNengOpened(client, 54, false))
										{
											goto IL_354;
										}
										int equipExcellencePropNum2 = Global.GetEquipExcellencePropNum(goodsByDbID);
										if (equipExcellencePropNum2 > 0)
										{
											int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhuoYueHuiShouZaiZaoXiShu", ',');
											num6 *= paramValueIntArrayByName[equipExcellencePropNum2 - 1];
										}
									}
								}
								if (num5 > 0 || num6 > 0 || num4 > 0)
								{
									string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										4,
										goodsByDbID.Id,
										goodsByDbID.GoodsID,
										0,
										goodsByDbID.Site,
										goodsByDbID.GCount,
										goodsByDbID.BagIndex,
										""
									});
									int gcount = goodsByDbID.GCount;
									if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null))
									{
										num += num5;
										num2 += num6;
										num3 += num4;
									}
								}
							}
						}
					}
					IL_354:
					i++;
					continue;
					goto IL_354;
				}
				if (num > 0)
				{
					SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RecoverMoJing);
					sevenDayGoalEventObject.Arg1 = num;
					GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, num, "一键出售或者回收", true, true, false);
				}
				if (num2 > 0)
				{
					GameManager.ClientMgr.ModifyZaiZaoValue(client, num2, "一键出售或者回收", true, true, false);
				}
				if (num3 > 0)
				{
					GameManager.ClientMgr.ModifyHunJingValue(client, num3, "一键出售或者回收", true, true, false);
				}
				result = TCPProcessCmdResults.RESULT_OK;
			}
			return result;
		}

		public TCPProcessCmdResults SaleStoreMountProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			TCPProcessCmdResults result;
			if (!GlobalNew.IsGongNengOpened(client, 97, false))
			{
				result = TCPProcessCmdResults.RESULT_OK;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				string[] array = strGoodsID.Split(new char[]
				{
					','
				});
				int i = 0;
				while (i < array.Length)
				{
					int id = Global.SafeConvertToInt32(array[i]);
					GoodsData mountStoreGoodsDataByDbID = ZuoQiManager.GetMountStoreGoodsDataByDbID(client, id);
					if (mountStoreGoodsDataByDbID != null && mountStoreGoodsDataByDbID.Site == 12000 && mountStoreGoodsDataByDbID.Using <= 0)
					{
						bool flag = ZuoQiManager.CheckIsZuoQiByGoodsID(mountStoreGoodsDataByDbID.GoodsID);
						bool flag2 = ZuoQiManager.CheckIsZuoQiEquipByGoodsID(mountStoreGoodsDataByDbID.GoodsID);
						if (flag || flag2)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(mountStoreGoodsDataByDbID.GoodsID, out systemXmlItem) && null != systemXmlItem)
							{
								int num4 = systemXmlItem.GetIntValue("ChangeHunJing", -1);
								int num5 = 0;
								int num6 = 0;
								if (flag)
								{
									AdvancedItem advancedItem = null;
									Dictionary<int, AdvancedItem> dictionary;
									if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(mountStoreGoodsDataByDbID.GoodsID, out dictionary))
									{
										goto IL_34C;
									}
									if (!dictionary.TryGetValue(mountStoreGoodsDataByDbID.Forge_level, out advancedItem))
									{
										goto IL_34C;
									}
									if (advancedItem.ChangeHunJing > 0)
									{
										num4 += advancedItem.ChangeHunJing;
									}
								}
								if (flag2)
								{
									double num7 = 1.0;
									if (mountStoreGoodsDataByDbID.ExcellenceInfo != 0)
									{
										int equipExcellencePropNum = Global.GetEquipExcellencePropNum(mountStoreGoodsDataByDbID);
										if (equipExcellencePropNum != 0)
										{
											double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuoYueHuiShouXiShu", ',');
											if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length >= equipExcellencePropNum)
											{
												num7 = paramValueDoubleArrayByName[equipExcellencePropNum - 1];
											}
										}
									}
									num5 += (int)((double)systemXmlItem.GetIntValue("ChangeJinYuan", -1) * num7);
									if (!GlobalNew.IsGongNengOpened(client, 54, false))
									{
										goto IL_34C;
									}
									num6 += systemXmlItem.GetIntValue("ChangeZaiZao", -1);
									if (num6 > 0)
									{
										int equipExcellencePropNum2 = Global.GetEquipExcellencePropNum(mountStoreGoodsDataByDbID);
										if (equipExcellencePropNum2 > 0)
										{
											int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhuoYueHuiShouZaiZaoXiShu", ',');
											num6 *= paramValueIntArrayByName[equipExcellencePropNum2 - 1];
										}
									}
								}
								if (num5 > 0 || num6 > 0 || num4 > 0)
								{
									string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										4,
										mountStoreGoodsDataByDbID.Id,
										mountStoreGoodsDataByDbID.GoodsID,
										0,
										mountStoreGoodsDataByDbID.Site,
										mountStoreGoodsDataByDbID.GCount,
										mountStoreGoodsDataByDbID.BagIndex,
										""
									});
									int gcount = mountStoreGoodsDataByDbID.GCount;
									if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null))
									{
										num += num5;
										num2 += num6;
										num3 += num4;
									}
								}
							}
						}
					}
					IL_34C:
					i++;
					continue;
					goto IL_34C;
				}
				if (num > 0)
				{
					SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RecoverMoJing);
					sevenDayGoalEventObject.Arg1 = num;
					GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, num, "一键出售或者回收", true, true, false);
				}
				if (num2 > 0)
				{
					GameManager.ClientMgr.ModifyZaiZaoValue(client, num2, "一键出售或者回收", true, true, false);
				}
				if (num3 > 0)
				{
					GameManager.ClientMgr.ModifyHunJingValue(client, num3, "一键出售或者回收", true, true, false);
				}
				result = TCPProcessCmdResults.RESULT_OK;
			}
			return result;
		}

		public List<int> CalZhuoYueByID(int code)
		{
			List<int> list = new List<int>();
			try
			{
				SuperiorDropItem superiorDropItem;
				if (!this.ZuoQiRunTimeData.SuperiorDropDict.TryGetValue(code, out superiorDropItem))
				{
					return list;
				}
				double random = Global.GetRandom();
				int num = 0;
				foreach (double[] array in superiorDropItem.CommonSuperiorRate)
				{
					if (random < array[0])
					{
						num = Convert.ToInt32(array[1]);
						break;
					}
				}
				random = Global.GetRandom();
				int num2 = 0;
				foreach (double[] array2 in superiorDropItem.SeniorSuperiorRate)
				{
					if (random < array2[0])
					{
						num2 = Convert.ToInt32(array2[1]);
						break;
					}
				}
				list.AddRange(this.GetSuperior(superiorDropItem.CommonSuperiorBank, num));
				list.AddRange(this.GetSuperior(superiorDropItem.SeniorSuperiorBank, num2));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZuoQi :: 根据卓越ID计算随机卓越属性，code={0}。", code), ex, true);
			}
			return list;
		}

		public ZuoQiMini GetRandomMount(List<MountRandomItem> mountList)
		{
			ZuoQiMini zuoQiMini = null;
			int randomNumber = Global.GetRandomNumber(1, 100001);
			lock (this.ZuoQiRunTimeData.Mutex)
			{
				foreach (MountRandomItem mountRandomItem in mountList)
				{
					if (randomNumber >= mountRandomItem.BeginNum && randomNumber <= mountRandomItem.EndNum)
					{
						zuoQiMini = new ZuoQiMini
						{
							GoodsID = mountRandomItem.GoodsID,
							Binding = 1,
							WashProps = new List<int>()
						};
						zuoQiMini.WashProps.AddRange(this.CalZhuoYueByID(mountRandomItem.SuperiorAttributeID));
						break;
					}
				}
			}
			return zuoQiMini;
		}

		public List<int> GetSuperior(List<int> bankListSource, int num)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			list2.AddRange(bankListSource);
			List<int> result;
			if (num <= 0 || num > list2.Count)
			{
				result = list;
			}
			else
			{
				lock (this.ZuoQiRunTimeData.Mutex)
				{
					for (int i = 0; i < num; i++)
					{
						if (list2.Count < 1)
						{
							break;
						}
						int randomNumber = Global.GetRandomNumber(0, list2.Count);
						SuperiorTypeItem superiorTypeItem;
						if (!this.ZuoQiRunTimeData.SuperiorTypeDict.TryGetValue(list2[randomNumber], out superiorTypeItem))
						{
							break;
						}
						double random = Global.GetRandom();
						foreach (double[] array in superiorTypeItem.Parameter)
						{
							if (random <= array[0])
							{
								list.Add(superiorTypeItem.Type);
								list.Add(Convert.ToInt32(array[1]));
								break;
							}
						}
						list2.RemoveAt(randomNumber);
					}
				}
				result = list;
			}
			return result;
		}

		public static bool CheckIsZuoQiByGoodsID(int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem) && systemXmlItem.GetIntValue("Categoriy", -1) == 340;
		}

		public static bool CheckIsZuoQiEquipByGoodsID(int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			bool result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
			{
				result = false;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
				result = (intValue >= 40 && intValue <= 45);
			}
			return result;
		}

		public void RoleDisMount(GameClient client, bool needLog = true)
		{
			if (null != client)
			{
				if (client.ClientData.IsRide == 1)
				{
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						PropsSystemTypes.ZuoQi,
						2,
						0
					});
					client.ClientData.MoveSpeed = RoleAlgorithm.GetMoveSpeed(client);
					GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 14, 0L, 0, client.ClientData.MoveSpeed);
					client.ClientData.IsRide = 0;
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MountIsRide, 0);
					string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 52, 0);
					client.sendOthersCmd(427, cmdData);
				}
				if (needLog)
				{
					client.ClientData.DisMountTick = TimeUtil.timeGetTime();
				}
			}
		}

		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 97, hint);
		}

		public double GetZuoQiResetRate()
		{
			return Math.Min(GameManager.systemParamsList.GetParamValueDoubleByName("HorseReturnNum", 0.0), 1.0);
		}

		public Dictionary<int, int> GetZuoQiNeedGoodsNumForCurrLevel(int goodid, int level)
		{
			AdvancedItem advancedItem = null;
			Dictionary<int, AdvancedItem> dictionary;
			Dictionary<int, int> result;
			if (!this.ZuoQiRunTimeData.AdvancedDict.TryGetValue(goodid, out dictionary))
			{
				result = null;
			}
			else if (!dictionary.TryGetValue(level, out advancedItem))
			{
				result = null;
			}
			else if (advancedItem.Level < 1)
			{
				result = null;
			}
			else
			{
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
				foreach (AdvancedItem advancedItem2 in dictionary.Values)
				{
					foreach (List<int> list in advancedItem2.NeedGoods)
					{
						if (list.Count == 2)
						{
							if (dictionary2.ContainsKey(list[0]))
							{
								Dictionary<int, int> dictionary3;
								int key;
								(dictionary3 = dictionary2)[key = list[0]] = dictionary3[key] + list[1];
							}
							else
							{
								dictionary2.Add(list[0], list[1]);
							}
						}
					}
					if (advancedItem2.Level == advancedItem.Level)
					{
						break;
					}
				}
				result = dictionary2;
			}
			return result;
		}

		public void InitRoleZuoQiData(GameClient client)
		{
			if (this.IsGongNengOpen(client, false))
			{
				if (null == client.ClientData.MountStoreList)
				{
					client.ClientData.MountStoreList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 12000), client.ServerId);
					if (null == client.ClientData.MountStoreList)
					{
						client.ClientData.MountStoreList = new List<GoodsData>();
					}
				}
				if (null == client.ClientData.MountEquipList)
				{
					client.ClientData.MountEquipList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 13000), client.ServerId);
					if (null == client.ClientData.MountEquipList)
					{
						client.ClientData.MountEquipList = new List<GoodsData>();
					}
					foreach (GoodsData goodsData in client.ClientData.MountEquipList)
					{
						this.AddMountSkill(client, goodsData, false);
					}
				}
				if (null == client.ClientData.MountList)
				{
					client.ClientData.MountList = Global.sendToDB<List<MountData>, string>(20319, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
					if (null == client.ClientData.MountList)
					{
						client.ClientData.MountList = new List<MountData>();
					}
				}
				client.ClientData.IsRide = ZuoQiManager.CanRide(client);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
				client.ClientData.ZuoQiMainData = new ZuoQiMainData
				{
					MountLevel = Global.GetRoleParamsInt32FromDB(client, "10207")
				};
			}
		}

		public void OnLogin(GameClient client)
		{
			if (this.IsGongNengOpen(client, false))
			{
				ExtData clientExtData = ExtDataManager.GetClientExtData(client);
				long num = clientExtData.ZuoQiSkillCDTicks - clientExtData.ZuoQiSkillCdTime;
				client.sendCmd(1901, string.Format("{0}:{1}:{2}", 0, Global.GetRoleParamsInt32FromDB(client, "10206"), num), false);
				if (client.ClientData.IsRide == 1)
				{
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						PropsSystemTypes.ZuoQi,
						2,
						this.GetSpeedAdd(client)
					});
				}
			}
		}

		public int ReLoadConfig(bool init = false)
		{
			try
			{
				bool flag = true;
				List<MountRandomItem> mountFreeRandomList;
				if (!this.LoadHorseFreeRandom(out mountFreeRandomList))
				{
					flag = false;
				}
				List<MountRandomItem> mountRandomList;
				if (!this.LoadHorseRandom(out mountRandomList))
				{
					flag = false;
				}
				List<MountRandomItem> mountPayRandomList;
				if (!this.LoadHorsePayRandom(out mountPayRandomList))
				{
					flag = false;
				}
				List<MountRandomItem> mountFreeRandomListTeQuan;
				if (!this.LoadMountFreeRandomListTeQuan(out mountFreeRandomListTeQuan))
				{
					flag = false;
				}
				List<MountRandomItem> mountRandomListTeQuan;
				if (!this.LoadTeQuanHorseRandom(out mountRandomListTeQuan))
				{
					flag = false;
				}
				List<MountRandomItem> mountPayRandomListTeQuan;
				if (!this.LoadTeQuanHorsePayRandom(out mountPayRandomListTeQuan))
				{
					flag = false;
				}
				Dictionary<int, SuperiorDropItem> superiorDropDict;
				if (!this.LoadSuperiorDropXml(out superiorDropDict))
				{
					flag = false;
				}
				Dictionary<int, SuperiorTypeItem> superiorTypeDict;
				if (!this.LoadSuperiorTypeXml(out superiorTypeDict))
				{
					flag = false;
				}
				Dictionary<int, PokedexItem> pokedexDict;
				if (!this.LoadPokedexXml(out pokedexDict))
				{
					flag = false;
				}
				Dictionary<int, LevelUpItem> levelUpDict;
				if (!this.LoadLevelUpXml(out levelUpDict))
				{
					flag = false;
				}
				Dictionary<int, Dictionary<int, AdvancedItem>> advancedDict;
				if (!this.LoadAdvancedXml(out advancedDict))
				{
					flag = false;
				}
				Dictionary<int, List<ArrayAdditionItem>> arrayAdditiionDict;
				if (!this.LoadArrayAdditionXml(out arrayAdditiionDict))
				{
					flag = false;
				}
				List<SuitItem> suitList;
				if (!this.LoadSuitXml(out suitList))
				{
					flag = false;
				}
				List<HorseEquipAdditionItem> horseEquipAdditionItemList;
				if (!this.LoadHorseEquipAdditionXml(out horseEquipAdditionItemList))
				{
					flag = false;
				}
				Dictionary<string, object> dictionary;
				List<KeyValuePair<int, int>> level2UpLevel;
				if (!this.LoadDefaultXml(out dictionary, out level2UpLevel))
				{
					flag = false;
				}
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("HorseNotice", ',');
				if (!flag && !init)
				{
					LogManager.WriteLog(2, string.Format("[ljl]{0}", "重载坐骑失败"), null, true);
					return 0;
				}
				lock (this.ZuoQiRunTimeData.Mutex)
				{
					this.ZuoQiRunTimeData.MountFreeRandomList = mountFreeRandomList;
					this.ZuoQiRunTimeData.MountRandomList = mountRandomList;
					this.ZuoQiRunTimeData.MountPayRandomList = mountPayRandomList;
					this.ZuoQiRunTimeData.MountFreeRandomListTeQuan = mountFreeRandomListTeQuan;
					this.ZuoQiRunTimeData.MountRandomListTeQuan = mountRandomListTeQuan;
					this.ZuoQiRunTimeData.MountPayRandomListTeQuan = mountPayRandomListTeQuan;
					this.ZuoQiRunTimeData.HorseNotice = ((paramValueIntArrayByName == null) ? new HashSet<int>() : new HashSet<int>(paramValueIntArrayByName));
					this.ZuoQiRunTimeData.SuperiorDropDict = superiorDropDict;
					this.ZuoQiRunTimeData.SuperiorTypeDict = superiorTypeDict;
					this.ZuoQiRunTimeData.PokedexDict = pokedexDict;
					this.ZuoQiRunTimeData.LevelUpDict = levelUpDict;
					this.ZuoQiRunTimeData.AdvancedDict = advancedDict;
					this.ZuoQiRunTimeData.ArrayAdditiionDict = arrayAdditiionDict;
					this.ZuoQiRunTimeData.SuitList = suitList;
					this.ZuoQiRunTimeData.HorseEquipAdditionItemList = horseEquipAdditionItemList;
					this.Level2UpLevel = level2UpLevel;
					this.ZuoQiFreeTime = (int)dictionary["ZuoQiFreeTime"];
					this.ZuoQiChouQuCost = (int)dictionary["ZuoQiChouQuCost"];
					this.ZuoQiChouQuCost_10 = (int)dictionary["ZuoQiChouQuCost_10"];
					this.ConsumeHuntHorseJiFen = (double)dictionary["ConsumeHuntHorseJiFen"];
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_ZuoQiConfig]{0}", ex.ToString()), null, true);
			}
			return 1;
		}

		private bool LoadHorseFreeRandom(out List<MountRandomItem> mountFreeRandomList)
		{
			mountFreeRandomList = new List<MountRandomItem>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.HorseFreeRandom), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", ZuoQiConsts.HorseFreeRandom), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						mountFreeRandomList.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.HorseFreeRandom, ex.Message), null, true);
			}
			return false;
		}

		private bool LoadHorseRandom(out List<MountRandomItem> mountRandomList)
		{
			mountRandomList = new List<MountRandomItem>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.HorseRandom), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", ZuoQiConsts.HorseRandom), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
						mountRandomList.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.HorseRandom, ex.Message), null, true);
			}
			return false;
		}

		private bool LoadHorsePayRandom(out List<MountRandomItem> mountPayRandomList)
		{
			mountPayRandomList = new List<MountRandomItem>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.HorsePayRandom), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", ZuoQiConsts.HorsePayRandom), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						mountPayRandomList.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.HorsePayRandom, ex.Message), null, true);
			}
			return false;
		}

		private bool LoadMountFreeRandomListTeQuan(out List<MountRandomItem> MountFreeRandomListTeQuan)
		{
			MountFreeRandomListTeQuan = new List<MountRandomItem>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.TeQuanHorseFreeRandom), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", ZuoQiConsts.TeQuanHorseFreeRandom), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						MountFreeRandomListTeQuan.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.TeQuanHorseFreeRandom, ex.Message), null, true);
			}
			return false;
		}

		private bool LoadTeQuanHorseRandom(out List<MountRandomItem> MountRandomListTeQuan)
		{
			MountRandomListTeQuan = new List<MountRandomItem>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.TeQuanHorseRandom), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", ZuoQiConsts.TeQuanHorseRandom), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
						MountRandomListTeQuan.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.TeQuanHorseRandom, ex.Message), null, true);
			}
			return false;
		}

		private bool LoadTeQuanHorsePayRandom(out List<MountRandomItem> MountPayRandomListTeQuan)
		{
			MountPayRandomListTeQuan = new List<MountRandomItem>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath(ZuoQiConsts.TeQuanHorsePayRandom), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", ZuoQiConsts.TeQuanHorsePayRandom), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						MountPayRandomListTeQuan.Add(new MountRandomItem
						{
							ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
							GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0")),
							GoodsNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0")),
							SuperiorAttributeID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SuperiorAttributeID", "0")),
							BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0")),
							EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"))
						});
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", ZuoQiConsts.TeQuanHorsePayRandom, ex.Message), null, true);
			}
			return false;
		}

		private bool LoadSuperiorDropXml(out Dictionary<int, SuperiorDropItem> superiorDropDict)
		{
			string text = "";
			superiorDropDict = new Dictionary<int, SuperiorDropItem>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseSuperiorDrop);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
					string defAttributeStr = Global.GetDefAttributeStr(xml, "CommonSuperiorRate", "");
					List<double[]> list = new List<double[]>();
					double num2 = 0.0;
					foreach (string text2 in defAttributeStr.Split(new char[]
					{
						'|'
					}))
					{
						string[] array2 = text2.Split(new char[]
						{
							','
						});
						if (array2.Length >= 2)
						{
							num2 += Convert.ToDouble(array2[1]);
							list.Add(new double[]
							{
								num2,
								Convert.ToDouble(array2[0])
							});
						}
					}
					string defAttributeStr2 = Global.GetDefAttributeStr(xml, "SeniorSuperiorRate", "");
					List<double[]> list2 = new List<double[]>();
					num2 = 0.0;
					foreach (string text2 in defAttributeStr2.Split(new char[]
					{
						'|'
					}))
					{
						string[] array2 = text2.Split(new char[]
						{
							','
						});
						if (array2.Length >= 2)
						{
							num2 += Convert.ToDouble(array2[1]);
							list2.Add(new double[]
							{
								num2,
								Convert.ToDouble(array2[0])
							});
						}
					}
					Dictionary<int, SuperiorDropItem> dictionary = superiorDropDict;
					int key = num;
					SuperiorDropItem superiorDropItem = new SuperiorDropItem();
					superiorDropItem.CommonSuperiorRate = list;
					superiorDropItem.CommonSuperiorBank = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xml, "CommonSuperiorBank", "").Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
					superiorDropItem.SeniorSuperiorRate = list2;
					superiorDropItem.SeniorSuperiorBank = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xml, "SeniorSuperiorBank", "").Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
					dictionary[key] = superiorDropItem;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadSuperiorTypeXml(out Dictionary<int, SuperiorTypeItem> superiorTypeDict)
		{
			string text = "";
			superiorTypeDict = new Dictionary<int, SuperiorTypeItem>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseSuperiorType);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int key = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
					string defAttributeStr = Global.GetDefAttributeStr(xml, "Parameter", "");
					List<double[]> list = new List<double[]>();
					double num = 0.0;
					foreach (string text2 in defAttributeStr.Split(new char[]
					{
						'|'
					}))
					{
						string[] array2 = text2.Split(new char[]
						{
							','
						});
						if (array2.Length >= 2)
						{
							num += Convert.ToDouble(array2[1]);
							list.Add(new double[]
							{
								num,
								Convert.ToDouble(array2[0])
							});
						}
					}
					superiorTypeDict[key] = new SuperiorTypeItem
					{
						Type = (int)ConfigParser.GetPropIndexByPropName(Global.GetDefAttributeStr(xml, "Type", "")),
						Parameter = list
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadPokedexXml(out Dictionary<int, PokedexItem> pokedexDict)
		{
			string text = "";
			pokedexDict = new Dictionary<int, PokedexItem>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorsePokedex);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "HorseGoods", "0"));
					string defAttributeStr = Global.GetDefAttributeStr(xml, "PokedexAttribute", "");
					string[] array = defAttributeStr.Split(new char[]
					{
						'|'
					});
					double[] array2 = new double[177];
					if (array.Length > 0)
					{
						foreach (string text2 in array)
						{
							string[] array4 = text2.Split(new char[]
							{
								','
							});
							if (array4.Length == 2)
							{
								ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
								if (propIndexByPropName < ExtPropIndexes.Max)
								{
									array2[(int)propIndexByPropName] += Global.SafeConvertToDouble(array4[1]);
								}
							}
						}
					}
					pokedexDict[num] = new PokedexItem
					{
						HorseGoods = num,
						PokedexAttribute = array2,
						HorseSpeed = Convert.ToDouble(Global.GetDefAttributeStr(xml, "HorseSpeed", "0"))
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadHorseEquipAdditionXml(out List<HorseEquipAdditionItem> list)
		{
			string text = "";
			list = new List<HorseEquipAdditionItem>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseEquipAddition);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
					string defAttributeStr = Global.GetDefAttributeStr(xml, "AdditionProps", "");
					string[] array = defAttributeStr.Split(new char[]
					{
						'|'
					});
					double[] array2 = new double[177];
					if (array.Length > 0)
					{
						foreach (string text2 in array)
						{
							string[] array4 = text2.Split(new char[]
							{
								','
							});
							if (array4.Length == 2)
							{
								ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
								if (propIndexByPropName < ExtPropIndexes.Max)
								{
									array2[(int)propIndexByPropName] += Global.SafeConvertToDouble(array4[1]);
								}
							}
						}
					}
					HorseEquipAdditionItem item = new HorseEquipAdditionItem
					{
						Type = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Type", "0")),
						NeedStrengthenLevel = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedStrengthenLevel", "0")),
						NeedAdditionLevel = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedAdditionLevel", "0")),
						NeedOrderNum = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedOrderNum", "0")),
						AdditionProps = defAttributeStr,
						ExtProps = array2
					};
					list.Add(item);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadAdvancedXml(out Dictionary<int, Dictionary<int, AdvancedItem>> advanceDict)
		{
			string text = "";
			advanceDict = new Dictionary<int, Dictionary<int, AdvancedItem>>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseAdvanced);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int key = Convert.ToInt32(Global.GetDefAttributeStr(xml, "HorseID", "0"));
					int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Level", "0")) - 1;
					string defAttributeStr = Global.GetDefAttributeStr(xml, "NeedGoods", "");
					List<List<int>> needGoods = ConfigHelper.ParserIntArrayList(defAttributeStr, true, '|', ',');
					Dictionary<int, AdvancedItem> dictionary;
					if (!advanceDict.TryGetValue(key, out dictionary))
					{
						dictionary = new Dictionary<int, AdvancedItem>();
						advanceDict[key] = dictionary;
					}
					dictionary[num] = new AdvancedItem
					{
						Level = num,
						NeedGoods = needGoods,
						AdvancedEffect = Convert.ToDouble(Global.GetDefAttributeStr(xml, "AdvancedEffect", "0")),
						SkillID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "SkillID", "0")),
						ChangeHunJing = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ChangeHunJing", "0"))
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadLevelUpXml(out Dictionary<int, LevelUpItem> levelUpDict)
		{
			string text = "";
			levelUpDict = new Dictionary<int, LevelUpItem>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseLevelUp);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int key = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Level", "0")) - 1;
					levelUpDict[key] = new LevelUpItem
					{
						Level = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Level", "0")),
						Exp = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Exp", "0")),
						AdvancedEffect = Convert.ToDouble(Global.GetDefAttributeStr(xml, "AdvancedEffect", "0"))
					};
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadArrayAdditionXml(out Dictionary<int, List<ArrayAdditionItem>> arrayAdditionDict)
		{
			string text = "";
			arrayAdditionDict = new Dictionary<int, List<ArrayAdditionItem>>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseArrayAddition);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Type", "0"));
					List<ArrayAdditionItem> list = null;
					if (!arrayAdditionDict.TryGetValue(num, out list))
					{
						list = new List<ArrayAdditionItem>();
						arrayAdditionDict[num] = list;
					}
					string defAttributeStr = Global.GetDefAttributeStr(xml, "AdditionProps", "");
					string[] array = defAttributeStr.Split(new char[]
					{
						'|'
					});
					List<KeyValuePair<int, double>> list2 = new List<KeyValuePair<int, double>>();
					if (array.Length > 0)
					{
						foreach (string text2 in array)
						{
							string[] array3 = text2.Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array3[0]);
								if (propIndexByPropName < ExtPropIndexes.Max)
								{
									list2.Add(new KeyValuePair<int, double>((int)propIndexByPropName, Global.SafeConvertToDouble(array3[1])));
								}
							}
						}
					}
					list.Add(new ArrayAdditionItem
					{
						Type = num,
						NeedLevel = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedLevel", "0")),
						NeedSuperiorNum = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedSuperiorNum", "0")),
						NeedOrderNum = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedOrderNum", "0")),
						AdditionProps = list2
					});
				}
				foreach (KeyValuePair<int, List<ArrayAdditionItem>> keyValuePair in arrayAdditionDict)
				{
					keyValuePair.Value.Sort(delegate(ArrayAdditionItem x, ArrayAdditionItem y)
					{
						int result;
						if (x.NeedLevel > 0 && y.NeedLevel > 0)
						{
							result = y.NeedLevel - x.NeedLevel;
						}
						else if (x.NeedOrderNum > 0 && y.NeedOrderNum > 0)
						{
							result = y.NeedOrderNum - x.NeedOrderNum;
						}
						else
						{
							result = y.NeedSuperiorNum - x.NeedSuperiorNum;
						}
						return result;
					});
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadSuitXml(out List<SuitItem> suitList)
		{
			string text = "";
			suitList = new List<SuitItem>();
			try
			{
				text = Global.GameResPath(ZuoQiConsts.HorseSuit);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					string defAttributeStr = Global.GetDefAttributeStr(xml, "HorseSuitProps", "");
					string[] array = defAttributeStr.Split(new char[]
					{
						'|'
					});
					List<KeyValuePair<int, double>> list = new List<KeyValuePair<int, double>>();
					if (array.Length > 0)
					{
						foreach (string text2 in array)
						{
							string[] array3 = text2.Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array3[0]);
								if (propIndexByPropName < ExtPropIndexes.Max)
								{
									list.Add(new KeyValuePair<int, double>((int)propIndexByPropName, Global.SafeConvertToDouble(array3[1])));
								}
							}
						}
					}
					List<SuitItem> list2 = suitList;
					SuitItem suitItem = new SuitItem();
					suitItem.HorseIDList = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xml, "HorseID", "").Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
					suitItem.HorseSuitProps = list;
					list2.Add(suitItem);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
			return false;
		}

		private bool LoadDefaultXml(out Dictionary<string, object> DefaultDict, out List<KeyValuePair<int, int>> Level2UpLevelXml)
		{
			Level2UpLevelXml = new List<KeyValuePair<int, int>>();
			DefaultDict = new Dictionary<string, object>();
			DefaultDict.Add("ZuoQiFreeTime", 0);
			DefaultDict.Add("ZuoQiChouQuCost", 0);
			DefaultDict.Add("ZuoQiChouQuCost_10", 0);
			DefaultDict.Add("ConsumeHuntHorseJiFen", 0);
			try
			{
				DefaultDict["ZuoQiFreeTime"] = (int)GameManager.systemParamsList.GetParamValueIntByName("HorseFreeRandom", -1);
				string[] array = GameManager.systemParamsList.GetParamValueByName("HorsePay").Split(new char[]
				{
					','
				});
				if (array.Length < 2)
				{
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败--HorsePay。", "SystemParams.xml"), null, true);
				}
				string[] array2 = GameManager.systemParamsList.GetParamValueByName("HorseLevelMax").Split(new char[]
				{
					'|'
				});
				foreach (string text in array2)
				{
					string[] array4 = text.Split(new char[]
					{
						','
					});
					if (array4.Length >= 3)
					{
						int unionLevel = Global.GetUnionLevel(Convert.ToInt32(array4[0]), Convert.ToInt32(array4[1]), false);
						Level2UpLevelXml.Add(new KeyValuePair<int, int>(unionLevel, Convert.ToInt32(array4[2])));
					}
				}
				DefaultDict["ZuoQiChouQuCost"] = Convert.ToInt32(array[0]);
				DefaultDict["ZuoQiChouQuCost_10"] = Convert.ToInt32(array[1]);
				DefaultDict["ConsumeHuntHorseJiFen"] = Convert.ToDouble(GameManager.systemParamsList.GetParamValueByName("ConsumeHuntHorseJiFen"));
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
			return false;
		}

		public ZuoQiRunData ZuoQiRunTimeData = new ZuoQiRunData();

		public int ZuoQiFreeTime = 0;

		public int ZuoQiChouQuCost = 0;

		public int ZuoQiChouQuCost_10 = 0;

		public double ConsumeHuntHorseJiFen = 0.0;

		public List<KeyValuePair<int, int>> Level2UpLevel = new List<KeyValuePair<int, int>>();

		private static ZuoQiManager instance = new ZuoQiManager();
	}
}
