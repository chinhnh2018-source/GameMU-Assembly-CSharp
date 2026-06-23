using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ProcessTask
	{
		public static void InitBranchTasks(Dictionary<int, SystemXmlItem> taskXmlDic)
		{
			if (taskXmlDic != null)
			{
				Dictionary<int, List<BranchTaskInfo>> dictionary = new Dictionary<int, List<BranchTaskInfo>>();
				Dictionary<long, List<int>> dictionary2 = new Dictionary<long, List<int>>();
				foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in taskXmlDic)
				{
					if (keyValuePair.Value.GetIntValue("TaskClass", -1) == 1)
					{
						string stringValue = keyValuePair.Value.GetStringValue("ZiDongTask");
						if (!string.IsNullOrEmpty(stringValue))
						{
							string[] array = stringValue.Split(new char[]
							{
								'|'
							});
							foreach (string text in array)
							{
								string[] array3 = text.Split(new char[]
								{
									','
								});
								if (array3.Length != 2)
								{
									LogManager.WriteLog(1000, string.Format("systemtasks.xml 中，任务编号为 {0} 的任务配置为支线任务，但是 ZiDongTask 字段配置错误，请检查。", keyValuePair.Key), null, true);
								}
								else
								{
									BranchTaskInfo branchTaskInfo = new BranchTaskInfo();
									branchTaskInfo.TaskID = keyValuePair.Key;
									branchTaskInfo.triggerType = (BranchTaskTriggerType)Convert.ToInt32(array3[0]);
									branchTaskInfo.triggerParam = Convert.ToInt64(array3[1]);
									List<BranchTaskInfo> list = null;
									if (!dictionary.TryGetValue(branchTaskInfo.TaskID, out list))
									{
										list = new List<BranchTaskInfo>();
										dictionary.Add(branchTaskInfo.TaskID, list);
									}
									list.Add(branchTaskInfo);
									if (branchTaskInfo.triggerType == BranchTaskTriggerType.CompleteTask)
									{
										List<int> list2 = null;
										if (!dictionary2.TryGetValue(branchTaskInfo.triggerParam, out list2))
										{
											list2 = new List<int>();
											dictionary2.Add(branchTaskInfo.triggerParam, list2);
										}
										list2.Add(branchTaskInfo.TaskID);
									}
								}
							}
						}
					}
				}
				ProcessTask.BranchTaskInfoDic = dictionary;
				ProcessTask.ActiveBranchTaskInfoDic = dictionary2;
			}
		}

		public bool IsBranchTask(int taskID)
		{
			SystemXmlItem systemXmlItem = null;
			return GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem) && systemXmlItem.GetIntValue("TaskClass", -1) == 1;
		}

		public static List<BranchTaskInfo> GetBranchTaskTriggerType(int taskID)
		{
			List<BranchTaskInfo> result = null;
			ProcessTask.BranchTaskInfoDic.TryGetValue(taskID, out result);
			return result;
		}

		public static List<int> GetActiveBranchTaskInfo(long taskID)
		{
			List<int> result = null;
			ProcessTask.ActiveBranchTaskInfoDic.TryGetValue(taskID, out result);
			return result;
		}

		public static void ProcessTakeBranchTasks(GameClient client, BranchTaskTriggerType type, long param)
		{
			if (type == BranchTaskTriggerType.CompleteTask)
			{
				List<int> activeBranchTaskInfo = ProcessTask.GetActiveBranchTaskInfo(param);
				if (activeBranchTaskInfo != null && activeBranchTaskInfo.Count > 0)
				{
					foreach (int num in activeBranchTaskInfo)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(num, out systemXmlItem))
						{
							List<BranchTaskInfo> branchTaskTriggerType = ProcessTask.GetBranchTaskTriggerType(num);
							if (branchTaskTriggerType == null || branchTaskTriggerType.Count <= 0)
							{
								break;
							}
							bool flag = true;
							foreach (BranchTaskInfo branchTaskInfo in branchTaskTriggerType)
							{
								if (branchTaskInfo.triggerType == BranchTaskTriggerType.CompleteTask)
								{
									SystemXmlItem systemXmlItem2 = null;
									if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue((int)branchTaskInfo.triggerParam, out systemXmlItem2))
									{
										flag = false;
										break;
									}
									int intValue = systemXmlItem2.GetIntValue("TaskClass", -1);
									if (param != branchTaskInfo.triggerParam)
									{
										if (0 == intValue)
										{
											if ((long)client.ClientData.MainTaskID < branchTaskInfo.triggerParam)
											{
												flag = false;
												break;
											}
										}
										else
										{
											if (1 != intValue)
											{
												flag = false;
												break;
											}
											OldTaskData oldTaskData = Global.FindOldTaskByTaskID(client, (int)branchTaskInfo.triggerParam);
											if (oldTaskData == null || oldTaskData.DoCount <= 0)
											{
												flag = false;
												break;
											}
										}
									}
								}
							}
							if (flag)
							{
								TCPOutPacket tcpoutPacket = null;
								TCPProcessCmdResults tcpprocessCmdResults = Global.TakeNewTask(Global._TCPManager, client.ClientSocket, Global._TCPManager.tcpClientPool, Global._TCPManager.tcpRandKey, Global._TCPManager.TcpOutPacketPool, 125, client, client.ClientData.RoleID, systemXmlItem.GetIntValue("ID", -1), systemXmlItem.GetIntValue("SourceNPC", -1), out tcpoutPacket);
								if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && tcpoutPacket != null)
								{
									client.sendCmd(tcpoutPacket, true);
								}
							}
						}
					}
				}
			}
		}

		private static void UpdateTaskDataToDB(GameClient client, TaskData taskData)
		{
			GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				taskData.DoingTaskID,
				taskData.DbID,
				taskData.DoingTaskFocus,
				taskData.DoingTaskVal1,
				taskData.DoingTaskVal2
			}), null, client.ServerId);
		}

		public static void ProcessAddTaskVal(GameClient client, TaskTypes taskType, int targetId, int val, params object[] args)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemXmlItem = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
							{
								bool flag2 = false;
								for (int j = 1; j <= 2; j++)
								{
									int[] intArrayValue = systemXmlItem.GetIntArrayValue("TargetNPC" + j, '|');
									if (intArrayValue != null && intArrayValue.Contains(targetId))
									{
										string name = string.Format("TargetType{0}", j);
										if (systemXmlItem.GetIntValue(name, -1) == (int)taskType)
										{
											int doingTaskVal = client.ClientData.TaskDataList[i].GetDoingTaskVal(j);
											string name2 = string.Format("TargetNum{0}", j);
											if (doingTaskVal < systemXmlItem.GetIntValue(name2, -1))
											{
												flag2 = true;
												client.ClientData.TaskDataList[i].IncDoingTaskVal(j);
												ProcessTask.UpdateTaskDataToDB(client, client.ClientData.TaskDataList[i]);
											}
										}
									}
								}
								if (flag2)
								{
									GameManager.ClientMgr.NotifyUpdateTask(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
									if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
									{
										int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
										if (-1 != intValue)
										{
											int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
											GameManager.ClientMgr.NotifyUpdateNPCTaskSate(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, intValue + 2130706432, state);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static int GetRoleTaskVal(GameClient client, TaskTypes taskType)
		{
			if (taskType <= TaskTypes.FreindNum)
			{
				switch (taskType)
				{
				case TaskTypes.WingIDLevel:
					return client.ClientData.MyWingData.WingID * 10000 + client.ClientData.MyWingData.ForgeLevel;
				case TaskTypes.XingZuoStar:
					if (null == client.ClientData.RoleStarConstellationInfo)
					{
						return 0;
					}
					lock (client.ClientData.RoleStarConstellationInfo)
					{
						return client.ClientData.RoleStarConstellationInfo.Sum((KeyValuePair<int, int> x) => x.Value);
					}
					break;
				default:
					if (taskType != TaskTypes.FreindNum)
					{
						goto IL_190;
					}
					break;
				}
				return Global.GetFriendCountByType(client, 0);
			}
			switch (taskType)
			{
			case TaskTypes.WanMoTa:
				return GameManager.ClientMgr.GetWanMoTaPassLayerValue(client);
			case TaskTypes.QiFu_10:
				break;
			case TaskTypes.InZhanMeng:
				return (client.ClientData.Faction > 0) ? 1 : 0;
			default:
				if (taskType == TaskTypes.HuFuForgeLevel)
				{
					lock (client.ClientData.GoodsDataList)
					{
						return client.ClientData.GoodsDataList.Max((GoodsData x) => (Global.GetGoodsCatetoriy(x.GoodsID) == 22) ? Global.GetEquipGoodsSuitID(x.GoodsID) : 0);
					}
				}
				break;
			}
			IL_190:
			return -1;
		}

		public static void ProcessRoleTaskVal(GameClient client, TaskTypes taskType, int val = -1)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemXmlItem = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
							{
								bool flag2 = false;
								for (int j = 1; j <= 2; j++)
								{
									string name = string.Format("TargetType{0}", j);
									if (systemXmlItem.GetIntValue(name, -1) == (int)taskType)
									{
										string text = string.Format("TargetNum{0}", j);
										int num;
										if (val < 0)
										{
											num = ProcessTask.GetRoleTaskVal(client, taskType);
										}
										else
										{
											num = val;
										}
										if (num > client.ClientData.TaskDataList[i].GetDoingTaskVal(j))
										{
											flag2 = true;
											client.ClientData.TaskDataList[i].SetDoingTaskVal(j, num);
											ProcessTask.UpdateTaskDataToDB(client, client.ClientData.TaskDataList[i]);
										}
									}
								}
								if (flag2)
								{
									GameManager.ClientMgr.NotifyUpdateTask(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
									if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
									{
										int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
										if (-1 != intValue)
										{
											int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
											GameManager.ClientMgr.NotifyUpdateNPCTaskSate(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, intValue + 2130706432, state);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void InitRoleTaskVal(GameClient client, TaskData taskData, SystemXmlItem systemTask)
		{
			for (int i = 1; i <= 2; i++)
			{
				string name = string.Format("TargetType{0}", i);
				int roleTaskVal = ProcessTask.GetRoleTaskVal(client, (TaskTypes)systemTask.GetIntValue(name, -1));
				if (roleTaskVal < 0)
				{
					break;
				}
				if (roleTaskVal > taskData.GetDoingTaskVal(i))
				{
					taskData.SetDoingTaskVal(i, roleTaskVal);
				}
			}
		}

		public static void Process(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType, Monster monster = null, int chengjiuID = 0, long chengjiuValue = -1L, GameClient otherClient = null)
		{
			switch (taskType)
			{
			case TaskTypes.Talk:
			case TaskTypes.GetSomething:
			case TaskTypes.NeedYuanBao:
				ProcessTask.ProcessTalk(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.KillMonster:
			case TaskTypes.MonsterSomething:
			case TaskTypes.CaiJiGoods:
			case TaskTypes.KillMonsterForLevel:
				ProcessTask.ProcessKillMonster(sl, pool, client, npcID, extensionID, goodsID, taskType, monster);
				break;
			case TaskTypes.BuySomething:
				ProcessTask.ProcessBuy(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.UseSomething:
				ProcessTask.ProcessUsingSomething(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.TransferSomething:
				ProcessTask.ProcessTransferSomething(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.ZhiLiao:
			case TaskTypes.FangHuo:
				ProcessTask.ProcessLuaHandle(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			case TaskTypes.GatherMonster:
				ProcessTask.ProcessGatherMonster(sl, pool, client, npcID, extensionID, goodsID, taskType);
				break;
			default:
				switch (taskType)
				{
				case TaskTypes.KillRoleOtherComp:
				case TaskTypes.KillRoleOtherCompTop:
					ProcessTask.ProcessKillRole(sl, pool, client, taskType, otherClient);
					break;
				default:
					if (taskType == TaskTypes.ChengJiuUpdate)
					{
						ProcessTask.ProcessChengJiuUpdate(sl, pool, client, chengjiuID, chengjiuValue);
					}
					break;
				}
				break;
			}
			ProcessTask.CheckAutoCompleteTask(client);
		}

		public static void CheckAutoCompleteTask(GameClient client)
		{
			if (client != null)
			{
				if (client.ClientData.TaskDataList != null)
				{
					List<TaskData> list = null;
					lock (client.ClientData.TaskDataList)
					{
						list = client.ClientData.TaskDataList.FindAll(delegate(TaskData _t)
						{
							bool result;
							if (!Global.JugeTaskComplete(client, _t.DoingTaskID, _t.DoingTaskVal1, _t.DoingTaskVal2))
							{
								result = false;
							}
							else
							{
								SystemXmlItem systemXmlItem2 = null;
								result = (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(_t.DoingTaskID, out systemXmlItem2) && systemXmlItem2.GetIntValue("IsComplete", -1) == 1);
							}
							return result;
						});
					}
					if (list != null && list.Count > 0)
					{
						foreach (TaskData taskData in list)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskData.DoingTaskID, out systemXmlItem))
							{
								ProcessTask._ProcessSpriteCompTaskCmd(client, systemXmlItem.GetIntValue("DestNPC", -1), taskData.DoingTaskID, taskData.DbID, 0);
							}
						}
					}
				}
			}
		}

		public static void _ProcessSpriteCompTaskCmd(GameClient client, int npcID, int taskID, int dbID, int useYuanBao)
		{
			int num = 140;
			SystemXmlItem systemXmlItem = null;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -200), false);
			}
			else
			{
				TaskData taskDataByDbID = Global.GetTaskDataByDbID(client, dbID);
				if (taskDataByDbID == null || taskDataByDbID.DoingTaskID != taskID)
				{
					client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -210), false);
				}
				else if (!Global.JugeTaskComplete(client, taskID, taskDataByDbID.DoingTaskVal1, taskDataByDbID.DoingTaskVal2))
				{
					client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -220), false);
				}
				else if (!Global.CanCompleteTaskByGridNum(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, taskID))
				{
					client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -2), false);
				}
				else
				{
					if (taskID == Data.InsertAwardtPortableBagTaskID)
					{
						if (client.ClientData.PortableGoodsDataList != null && client.ClientData.PortableGoodsDataList.Count >= client.ClientData.MyPortableBagData.ExtGridNum)
						{
							client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -2), false);
							return;
						}
					}
					if (!Global.CanCompleteTaskByBlessPoint(client, systemXmlItem))
					{
						client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -300), false);
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("TaskClass", -1);
						useYuanBao = Math.Max(1, useYuanBao);
						useYuanBao = Math.Min(3, useYuanBao);
						string text = "完成任务" + taskID;
						if (useYuanBao > 1)
						{
							int num2 = 0;
							int num3 = 0;
							if (2 == useYuanBao)
							{
								num3 = (int)GameManager.systemParamsList.GetParamValueIntByName("DoubleExp", -1);
							}
							else if (3 == useYuanBao)
							{
								num2 = (int)GameManager.systemParamsList.GetParamValueIntByName("BindTongQianTask3Awards", -1);
							}
							if (num3 > 0)
							{
								if (!GameManager.ClientMgr.SubUserMoney(client, num3, "任务完成双倍经验", true, true, true, true, DaiBiSySType.None))
								{
									client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1004), false);
									return;
								}
							}
							else if (num2 > 0)
							{
								if (Global.GetTotalBindTongQianAndTongQianVal(client) < num2)
								{
									client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1002), false);
									return;
								}
								if (!Global.SubBindTongQianAndTongQian(client, num2, "任务完成多倍奖励"))
								{
									client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1003), false);
									return;
								}
							}
							else
							{
								useYuanBao = 1;
							}
						}
						int intValue2 = systemXmlItem.GetIntValue("TaskClass", -1);
						int num4 = (intValue2 == 0) ? 1 : 0;
						byte[] bytes = new UTF8Encoding().GetBytes(string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							client.ClientData.RoleID,
							npcID,
							taskID,
							dbID,
							num4
						}));
						byte[] array = null;
						if (TCPProcessCmdResults.RESULT_FAILED == Global.TransferRequestToDBServer2(Global._TCPManager, client.ClientSocket, Global._TCPManager.tcpClientPool, Global._TCPManager.tcpRandKey, Global._TCPManager.TcpOutPacketPool, num, bytes, bytes.Length, out array, client.ServerId))
						{
							LogManager.WriteLog(2, string.Format("与DBServer通讯失败, CMD={0}", (TCPGameServerCmds)num), null, true);
							client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -3), false);
						}
						else
						{
							int num5 = BitConverter.ToInt32(array, 0);
							short num6 = BitConverter.ToInt16(array, 4);
							string @string = new UTF8Encoding().GetString(array, 6, num5 - 2);
							string[] array2 = @string.Split(new char[]
							{
								':'
							});
							if (array2.Length < 3 || array2[2] == "-1")
							{
								client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1), false);
							}
							else
							{
								int completeTaskZhangJie = client.ClientData.CompleteTaskZhangJie;
								int num7 = 0;
								int num8 = Global.CalcTaskZhangJieID(client, taskID, out num7);
								if (completeTaskZhangJie != num8)
								{
									long num9 = TimeUtil.TimeStamp();
								}
								if (ProcessTask.Complete(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, npcID, -1, taskID, dbID, false, (double)useYuanBao, false))
								{
									if (intValue2 == 0 && taskID > client.ClientData.MainTaskID)
									{
										client.ClientData.MainTaskID = taskID;
										Global.AutoLearnSkills(client);
										GlobalNew.RefreshGongNeng(client);
										HuiJiManager.getInstance().InitDataByTask(client);
										ArmorManager.getInstance().InitDataByTask(client);
										GlobalEventSource.getInstance().fireEvent(new MainTaskProgressEvent(client, taskID));
										client.RunAreaLuaFile(GameManager.MapMgr.DictMaps[client.ClientData.MapCode], RunAreaLuaType.SelfPoint, null, "enterArea", taskID);
									}
									if (0 == intValue2)
									{
									}
									if (num4 > 0)
									{
										ChengJiuManager.ProcessCompleteMainTaskForChengJiu(client, taskID);
									}
									if (intValue2 >= 100 && intValue2 <= 150)
									{
										CompManager.getInstance().HandleCompTaskSomething(client, false);
									}
									ProcessTask.ProcessTakeBranchTasks(client, BranchTaskTriggerType.CompleteTask, (long)taskID);
									client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, 0), false);
									if (systemXmlItem.GetIntValue("IsComplete", -1) == 1)
									{
										SystemXmlItem systemXmlItem2 = null;
										if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(systemXmlItem.GetIntValue("NextTask", -1), out systemXmlItem2))
										{
											TCPOutPacket tcpoutPacket = null;
											TCPProcessCmdResults tcpprocessCmdResults = Global.TakeNewTask(Global._TCPManager, client.ClientSocket, Global._TCPManager.tcpClientPool, Global._TCPManager.tcpRandKey, Global._TCPManager.TcpOutPacketPool, 125, client, client.ClientData.RoleID, systemXmlItem2.GetIntValue("ID", -1), systemXmlItem2.GetIntValue("SourceNPC", -1), out tcpoutPacket);
											if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && tcpoutPacket != null)
											{
												client.sendCmd(tcpoutPacket, true);
											}
											if (TCPProcessCmdResults.RESULT_OK == tcpprocessCmdResults)
											{
												if (105 == taskID)
												{
													FreshPlayerCopySceneManager.AddShuiJingGuanCaiMonsters(client);
												}
											}
											client.RunAreaLuaFile(GameManager.MapMgr.DictMaps[client.ClientData.MapCode], RunAreaLuaType.SelfPoint, null, "enterArea", 0);
										}
									}
								}
								else
								{
									client.sendCmd<SCCompTask>(num, new SCCompTask(client.ClientData.RoleID, npcID, taskID, -1), false);
								}
							}
						}
					}
				}
			}
		}

		private static string GetPropNameGoodsName(string propName)
		{
			string result;
			if (string.IsNullOrEmpty(propName))
			{
				result = propName;
			}
			else
			{
				string[] array = propName.Split(new char[]
				{
					'|'
				});
				if (array.Length <= 1)
				{
					result = propName;
				}
				else
				{
					result = array[0];
				}
			}
			return result;
		}

		private static int GetPropNameGoodsLevel(string propName)
		{
			int result;
			if (string.IsNullOrEmpty(propName))
			{
				result = 0;
			}
			else
			{
				string[] array = propName.Split(new char[]
				{
					'|'
				});
				if (array.Length < 3)
				{
					result = 0;
				}
				else
				{
					result = Global.SafeConvertToInt32(array[1]);
				}
			}
			return result;
		}

		private static int GetPropNameGoodsQuality(string propName)
		{
			int result;
			if (string.IsNullOrEmpty(propName))
			{
				result = 0;
			}
			else
			{
				string[] array = propName.Split(new char[]
				{
					'|'
				});
				if (array.Length < 3)
				{
					result = 0;
				}
				else
				{
					result = (int)Global.GetEnchanceQualityByColorName(array[2]);
				}
			}
			return result;
		}

		private static bool IsTaskValid(GameClient client, SystemXmlItem systemTask, TaskData taskData, long nowTicks)
		{
			int intValue = systemTask.GetIntValue("Taketime", -1);
			bool result;
			if (intValue > 0 && nowTicks - taskData.AddDateTime >= (long)(intValue * 1000))
			{
				result = false;
			}
			else
			{
				string stringValue = systemTask.GetStringValue("PubStartTime");
				string stringValue2 = systemTask.GetStringValue("PubEndTime");
				if (!string.IsNullOrEmpty(stringValue) && !string.IsNullOrEmpty(stringValue2))
				{
					long num = Global.SafeConvertToTicks(stringValue);
					long num2 = Global.SafeConvertToTicks(stringValue2);
					if (nowTicks < num || nowTicks > num2)
					{
						return false;
					}
				}
				int intValue2 = systemTask.GetIntValue("LimitZhuanSheng", -1);
				int intValue3 = systemTask.GetIntValue("LimitLevel", -1);
				result = (0 == Global.AvalidLevel(client.ClientData.ChangeLifeCount, client.ClientData.Level, intValue2, intValue3, -1, -1));
			}
			return result;
		}

		private static void ProcessTalk(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemXmlItem = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
							{
								int[] intArrayValue = systemXmlItem.GetIntArrayValue("TargetNPC1", '|');
								if (intArrayValue != null && intArrayValue.Contains(extensionID))
								{
									bool flag2 = false;
									if (systemXmlItem.GetIntValue("TargetType1", -1) == 0)
									{
										bool flag3 = false;
										int intValue = systemXmlItem.GetIntValue("JingMaiID", -1);
										int intValue2 = systemXmlItem.GetIntValue("BuffID", -1);
										int intValue3 = systemXmlItem.GetIntValue("WuXueID", -1);
										if (intValue <= 0)
										{
											flag3 = true;
										}
										if (intValue3 <= 0)
										{
											flag3 = flag3;
										}
										if (intValue2 > 0)
										{
											BufferData bufferDataByID = Global.GetBufferDataByID(client, intValue2);
											if (null != bufferDataByID)
											{
												if (!Global.IsBufferDataOver(bufferDataByID, nowTicks))
												{
													flag3 = flag3;
												}
											}
										}
										else
										{
											flag3 = flag3;
										}
										if (flag3)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									else if (systemXmlItem.GetIntValue("TargetType1", -1) == 6 && "" != systemXmlItem.GetStringValue("PropsName1"))
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
										{
											bool flag4 = true;
											string stringValue = systemXmlItem.GetStringValue("PropsName1");
											string propNameGoodsName = ProcessTask.GetPropNameGoodsName(stringValue);
											int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
											int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
											int goodsByName = Global.GetGoodsByName(propNameGoodsName);
											if (goodsByName >= 0)
											{
												GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
												if (null == notUsingGoodsByID)
												{
													if (Global.CanAddGoods(client, goodsByName, 1, 1, "1900-01-01 12:00:00", true, false))
													{
														Global.AddGoodsDBCommand(pool, client, goodsByName, 1, 0, "", 0, 1, 0, "", true, 1, "获取任务道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
													}
													else
													{
														flag4 = false;
														GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, GLang.GetLang(516, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
													}
												}
											}
											if (flag4)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									else if (systemXmlItem.GetIntValue("TargetType1", -1) == 7)
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
										{
											int num = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
											if (num > 0)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									if (flag2)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue4 = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue4)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue4, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue4 + 2130706432, state);
											}
										}
									}
								}
								if (extensionID == systemXmlItem.GetIntValue("TargetNPC2", -1))
								{
									bool flag2 = false;
									if (systemXmlItem.GetIntValue("TargetType2", -1) == 0)
									{
										bool flag3 = false;
										int intValue = systemXmlItem.GetIntValue("JingMaiID", -1);
										int intValue2 = systemXmlItem.GetIntValue("BuffID", -1);
										int intValue3 = systemXmlItem.GetIntValue("WuXueID", -1);
										if (intValue <= 0)
										{
											flag3 = true;
										}
										if (intValue3 <= 0)
										{
											flag3 = flag3;
										}
										if (intValue2 > 0)
										{
											BufferData bufferDataByID = Global.GetBufferDataByID(client, intValue2);
											if (null != bufferDataByID)
											{
												if (!Global.IsBufferDataOver(bufferDataByID, nowTicks))
												{
													flag3 = flag3;
												}
											}
										}
										else
										{
											flag3 = flag3;
										}
										if (flag3)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									else if (systemXmlItem.GetIntValue("TargetType2", -1) == 6 && "" != systemXmlItem.GetStringValue("PropsName2"))
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
										{
											bool flag4 = true;
											string stringValue = systemXmlItem.GetStringValue("PropsName2");
											string propNameGoodsName = ProcessTask.GetPropNameGoodsName(stringValue);
											int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
											int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
											int goodsByName = Global.GetGoodsByName(propNameGoodsName);
											if (goodsByName >= 0)
											{
												GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
												if (null == notUsingGoodsByID)
												{
													if (Global.CanAddGoods(client, goodsByName, 1, 1, "1900-01-01 12:00:00", true, false))
													{
														Global.AddGoodsDBCommand(pool, client, goodsByName, 1, 0, "", 0, 1, 0, "", true, 1, "获取任务道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
													}
													else
													{
														flag4 = false;
														GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, GLang.GetLang(516, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
													}
												}
											}
											if (flag4)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									else if (systemXmlItem.GetIntValue("TargetType2", -1) == 7)
									{
										if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
										{
											int num = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
											if (num > 0)
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									if (flag2)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue4 = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue4)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue4, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue4 + 2130706432, state);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessTransferSomething(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemXmlItem = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
							{
								int[] intArrayValue = systemXmlItem.GetIntArrayValue("TargetNPC1", '|');
								if (intArrayValue != null && intArrayValue.Contains(extensionID))
								{
									bool flag2 = false;
									if (systemXmlItem.GetIntValue("TargetType1", -1) == 5 && "" != systemXmlItem.GetStringValue("PropsName1"))
									{
										bool flag3 = true;
										string stringValue = systemXmlItem.GetStringValue("PropsName1");
										string propNameGoodsName = ProcessTask.GetPropNameGoodsName(stringValue);
										int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
										int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
										int goodsByName = Global.GetGoodsByName(propNameGoodsName);
										if (goodsByName >= 0)
										{
											GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
											if (null != notUsingGoodsByID)
											{
												int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
												if (goodsCatetoriy >= 49)
												{
													GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, notUsingGoodsByID.Id, false, false);
												}
											}
											else
											{
												flag3 = false;
											}
										}
										if (flag3)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									if (flag2)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
											}
										}
									}
								}
								if (extensionID == systemXmlItem.GetIntValue("TargetNPC2", -1))
								{
									bool flag2 = false;
									if (systemXmlItem.GetIntValue("TargetType2", -1) == 5 && "" != systemXmlItem.GetStringValue("PropsName2"))
									{
										bool flag3 = true;
										string stringValue = systemXmlItem.GetStringValue("PropsName2");
										string propNameGoodsName = ProcessTask.GetPropNameGoodsName(stringValue);
										int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
										int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
										int goodsByName = Global.GetGoodsByName(propNameGoodsName);
										if (goodsByName >= 0)
										{
											GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
											if (null != notUsingGoodsByID)
											{
												int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
												if (goodsCatetoriy >= 49)
												{
													GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, notUsingGoodsByID.Id, false, false);
												}
											}
											else
											{
												flag3 = false;
											}
										}
										if (flag3)
										{
											if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag2 = true;
											}
										}
									}
									if (flag2)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessGatherMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				SystemXmlItem systemXmlItem = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (doingTaskID == goodsID)
						{
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
							{
								if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
								{
									client.ClientData.TaskDataList[i].DoingTaskVal1++;
									GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.TaskDataList[i].DoingTaskID,
										client.ClientData.TaskDataList[i].DbID,
										client.ClientData.TaskDataList[i].DoingTaskFocus,
										client.ClientData.TaskDataList[i].DoingTaskVal1,
										client.ClientData.TaskDataList[i].DoingTaskVal2
									}), null, client.ServerId);
									GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
									if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
									{
										int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
										if (-1 != intValue)
										{
											int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
											GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static bool TaskOKForSystemKillMoster(GameClient client, int extentionID, int taskid)
		{
			long nowTicks = TimeUtil.NOW();
			SystemXmlItem systemXmlItem = null;
			lock (client.ClientData.TaskDataList)
			{
				int i = 0;
				while (i < client.ClientData.TaskDataList.Count)
				{
					int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
					if (doingTaskID == taskid)
					{
						if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							return false;
						}
						if (!ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
						{
							return false;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			bool result;
			if (systemXmlItem == null)
			{
				result = false;
			}
			else
			{
				int[] intArrayValue = systemXmlItem.GetIntArrayValue("TargetNPC1", '|');
				if (intArrayValue != null && intArrayValue.Contains(extentionID))
				{
					if (systemXmlItem.GetIntValue("TargetType1", -1) == 1)
					{
						return true;
					}
				}
				if (extentionID == systemXmlItem.GetIntValue("TargetNPC2", -1))
				{
					if (systemXmlItem.GetIntValue("TargetType2", -1) == 1)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private static void ProcessKillRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, TaskTypes taskType, GameClient otherClient)
		{
			if (null != otherClient)
			{
				if (null != client.ClientData.TaskDataList)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemXmlItem = null;
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
							{
								if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
								{
									for (int j = 1; j <= 2; j++)
									{
										string name = string.Format("TargetType{0}", j);
										string name2 = string.Format("TargetNum{0}", j);
										bool flag2 = false;
										if (systemXmlItem.GetIntValue(name, -1) == (int)taskType)
										{
											if (client.ClientData.CompType > 0 && otherClient.ClientData.CompType > 0)
											{
												SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
												if (mapSceneType == 48)
												{
													if (systemXmlItem.GetIntValue(name, -1) == 101)
													{
														if (client.ClientData.CompType == otherClient.ClientData.CompType)
														{
															goto IL_42D;
														}
													}
													if (systemXmlItem.GetIntValue(name, -1) == 102)
													{
														if (!CompManager.getInstance().IfTopBoomCompType(client, otherClient.ClientData.CompType, false))
														{
															goto IL_42D;
														}
													}
													if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue(name2, -1))
													{
														client.ClientData.TaskDataList[i].DoingTaskVal1++;
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														flag2 = true;
													}
													if (flag2)
													{
														GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
														if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
														{
															int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
															if (-1 != intValue)
															{
																int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
																GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
															}
														}
													}
												}
											}
										}
										IL_42D:;
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessChengJiuUpdate(SocketListener sl, TCPOutPacketPool pool, GameClient client, int chengjiuID, long roleCurrentValue)
		{
			if (chengjiuID > 0)
			{
				if (null != client.ClientData.TaskDataList)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemXmlItem = null;
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
							{
								if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
								{
									if (systemXmlItem.GetIntValue("ChenJiuID", -1) == chengjiuID)
									{
										if (client.ClientData.TaskDataList[i].ChengJiuVal >= 0L)
										{
											if (ChengJiuManager.IsChengJiuCompleted(client, chengjiuID))
											{
												client.ClientData.TaskDataList[i].ChengJiuVal = -1L;
											}
											else
											{
												client.ClientData.TaskDataList[i].ChengJiuVal = roleCurrentValue;
											}
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (client.ClientData.TaskDataList[i].ChengJiuVal < 0L)
											{
												if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
												{
													int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
													if (-1 != intValue)
													{
														int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
														GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessKillMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType, Monster monster = null)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				int num = Global.GetFocusTaskCount(client);
				bool flag = false;
				SystemXmlItem systemXmlItem = null;
				int num2 = -1;
				SystemXmlItem systemXmlItem2 = null;
				if (GameManager.systemMonsterMgr.SystemXmlItemDict.TryGetValue(extensionID, out systemXmlItem2))
				{
					num2 = systemXmlItem2.GetIntValue("Level", -1);
				}
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
							{
								if (11 == systemXmlItem.GetIntValue("TargetType1", -1))
								{
									if (num2 >= systemXmlItem.GetIntValue("TargetNPC1", -1) && client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
									{
										client.ClientData.TaskDataList[i].DoingTaskVal1++;
										if (num < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
										{
											num++;
											client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
										}
										GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.TaskDataList[i].DoingTaskID,
											client.ClientData.TaskDataList[i].DbID,
											client.ClientData.TaskDataList[i].DoingTaskFocus,
											client.ClientData.TaskDataList[i].DoingTaskVal1,
											client.ClientData.TaskDataList[i].DoingTaskVal2
										}), null, client.ServerId);
										flag = true;
									}
									if (flag)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
											}
										}
									}
								}
								else
								{
									for (int j = 1; j <= 2; j++)
									{
										int[] intArrayValue = systemXmlItem.GetIntArrayValue(string.Format("TargetNPC{0}", j), '|');
										if (intArrayValue != null && intArrayValue.Contains(extensionID))
										{
											string name = string.Format("TargetType{0}", j);
											string name2 = string.Format("TargetNum{0}", j);
											flag = false;
											if (systemXmlItem.GetIntValue(name, -1) == 1)
											{
												string name3 = string.Format("FallPercent{0}", j);
												int intValue2 = systemXmlItem.GetIntValue(name3, -1);
												int num3 = -1;
												if (intValue2 > 0)
												{
													num3 = Global.GetRandomNumber(0, 101);
												}
												if (num3 < intValue2)
												{
													if (client.ClientData.TaskDataList[i].GetDoingTaskVal(j) < systemXmlItem.GetIntValue(name2, -1))
													{
														client.ClientData.TaskDataList[i].IncDoingTaskVal(j);
														if (num < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
														{
															num++;
															client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
														}
														if (systemXmlItem.GetIntValue("NeedTargetNum", -1) > 0 && !string.IsNullOrEmpty(systemXmlItem.GetStringValue("FallGoods")))
														{
															if (client.ClientData.TaskDataList[i].GetDoingTaskVal(j) == systemXmlItem.GetIntValue("NeedTargetNum", -1))
															{
																GameManager.GoodsPackMgr.ProcessTaskDropByTargetNum(client, systemXmlItem.GetStringValue("FallGoods"), monster);
															}
														}
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														flag = true;
													}
												}
											}
											else if (systemXmlItem.GetIntValue(name, -1) == 2)
											{
												int num3 = Global.GetRandomNumber(0, 101);
												string name3 = string.Format("FallPercent{0}", j);
												int intValue2 = systemXmlItem.GetIntValue(name3, -1);
												if (num3 < intValue2)
												{
													if (client.ClientData.TaskDataList[i].GetDoingTaskVal(j) < systemXmlItem.GetIntValue(name2, -1))
													{
														bool flag3 = true;
														string stringValue = systemXmlItem.GetStringValue("PropsName1");
														int goodsByName = Global.GetGoodsByName(stringValue);
														if (goodsByName >= 0)
														{
															if (Global.CanAddGoods(client, goodsByName, 1, 1, "1900-01-01 12:00:00", true, false))
															{
																Global.AddGoodsDBCommand(pool, client, goodsByName, 1, 0, "", 0, 1, 0, "", true, 1, "获取杀怪掉落道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
															}
															else
															{
																flag3 = false;
																GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, StringUtil.substitute(GLang.GetLang(517, new object[0]), new object[]
																{
																	stringValue
																}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
															}
														}
														if (flag3)
														{
															client.ClientData.TaskDataList[i].IncDoingTaskVal(j);
															if (num < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
															{
																num++;
																client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
															}
															GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
															{
																client.ClientData.RoleID,
																client.ClientData.TaskDataList[i].DoingTaskID,
																client.ClientData.TaskDataList[i].DbID,
																client.ClientData.TaskDataList[i].DoingTaskFocus,
																client.ClientData.TaskDataList[i].DoingTaskVal1,
																client.ClientData.TaskDataList[i].DoingTaskVal2
															}), null, client.ServerId);
															flag = true;
														}
													}
												}
											}
											else if (systemXmlItem.GetIntValue(name, -1) == 8)
											{
												int num3 = Global.GetRandomNumber(0, 101);
												string name3 = string.Format("FallPercent{0}", j);
												int intValue2 = systemXmlItem.GetIntValue(name3, -1);
												if (num3 < intValue2)
												{
													if (client.ClientData.TaskDataList[i].GetDoingTaskVal(j) < systemXmlItem.GetIntValue(name2, -1))
													{
														bool flag3 = true;
														string stringValue = systemXmlItem.GetStringValue("PropsName1");
														int goodsByName = Global.GetGoodsByName(stringValue);
														if (goodsByName >= 0)
														{
															if (Global.CanAddGoods(client, goodsByName, 1, 1, "1900-01-01 12:00:00", true, false))
															{
																Global.AddGoodsDBCommand(pool, client, goodsByName, 1, 0, "", 0, 1, 0, "", true, 1, "采集获取道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
															}
															else
															{
																flag3 = false;
																GameManager.ClientMgr.NotifyImportantMsg(sl, pool, client, StringUtil.substitute(GLang.GetLang(518, new object[0]), new object[]
																{
																	stringValue
																}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 1);
															}
														}
														if (flag3)
														{
															client.ClientData.TaskDataList[i].IncDoingTaskVal(j);
															if (num < Data.TaskMaxFocusCount && client.ClientData.TaskDataList[i].DoingTaskFocus <= 0)
															{
																num++;
																client.ClientData.TaskDataList[i].DoingTaskFocus = 1;
															}
															GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
															{
																client.ClientData.RoleID,
																client.ClientData.TaskDataList[i].DoingTaskID,
																client.ClientData.TaskDataList[i].DbID,
																client.ClientData.TaskDataList[i].DoingTaskFocus,
																client.ClientData.TaskDataList[i].DoingTaskVal1,
																client.ClientData.TaskDataList[i].DoingTaskVal2
															}), null, client.ServerId);
															flag = true;
														}
													}
												}
											}
											if (flag)
											{
												GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
												if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
												{
													int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
													if (-1 != intValue)
													{
														int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
														GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessBuy(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				if (-1 != goodsID)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemXmlItem = null;
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
							{
								if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
								{
									if (systemXmlItem.GetIntValue("TargetType1", -1) == 3)
									{
										bool flag2 = false;
										string stringValue = systemXmlItem.GetStringValue("PropsName1");
										string propNameGoodsName = ProcessTask.GetPropNameGoodsName(stringValue);
										int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
										int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
										int goodsByName = Global.GetGoodsByName(propNameGoodsName);
										if (goodsID == goodsByName)
										{
											GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
											if (null != notUsingGoodsByID)
											{
												if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
												{
													client.ClientData.TaskDataList[i].DoingTaskVal1++;
													GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
													{
														client.ClientData.RoleID,
														client.ClientData.TaskDataList[i].DoingTaskID,
														client.ClientData.TaskDataList[i].DbID,
														client.ClientData.TaskDataList[i].DoingTaskFocus,
														client.ClientData.TaskDataList[i].DoingTaskVal1,
														client.ClientData.TaskDataList[i].DoingTaskVal2
													}), null, client.ServerId);
													flag2 = true;
												}
											}
										}
										if (flag2)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
												if (-1 != intValue)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
												}
											}
										}
									}
									if (systemXmlItem.GetIntValue("TargetType2", -1) == 3)
									{
										bool flag2 = false;
										string stringValue = systemXmlItem.GetStringValue("PropsName2");
										string propNameGoodsName = ProcessTask.GetPropNameGoodsName(stringValue);
										int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
										int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
										int goodsByName = Global.GetGoodsByName(propNameGoodsName);
										if (goodsID == goodsByName)
										{
											GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
											if (null != notUsingGoodsByID)
											{
												if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
												{
													client.ClientData.TaskDataList[i].DoingTaskVal2++;
													GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
													{
														client.ClientData.RoleID,
														client.ClientData.TaskDataList[i].DoingTaskID,
														client.ClientData.TaskDataList[i].DbID,
														client.ClientData.TaskDataList[i].DoingTaskFocus,
														client.ClientData.TaskDataList[i].DoingTaskVal1,
														client.ClientData.TaskDataList[i].DoingTaskVal2
													}), null, client.ServerId);
													flag2 = true;
												}
											}
										}
										if (flag2)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
												if (-1 != intValue)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessUsingSomething(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				if (-1 != goodsID)
				{
					long nowTicks = TimeUtil.NOW();
					SystemXmlItem systemXmlItem = null;
					GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
					lock (client.ClientData.TaskDataList)
					{
						for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
						{
							int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
							if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
							{
								if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
								{
									if (systemXmlItem.GetIntValue("TargetType1", -1) == 4)
									{
										bool flag2 = false;
										string stringValue = systemXmlItem.GetStringValue("PropsName1");
										int goodsByName = Global.GetGoodsByName(stringValue);
										if (goodsID == goodsByName)
										{
											int intValue = systemXmlItem.GetIntValue("TargetMapCode1", -1);
											Point point = Global.StrToPoint(systemXmlItem.GetStringValue("TargetPos1"));
											if (intValue >= 0 && !double.IsNaN(point.X) && !double.IsNaN(point.Y))
											{
												Point currentGrid = client.CurrentGrid;
												Point point2 = new Point((double)((int)(point.X / (double)gameMap.MapGridWidth)), (double)((int)(point.Y / (double)gameMap.MapGridHeight)));
												bool flag3 = Math.Abs(point2.X - currentGrid.X) < 3.0 && Math.Abs(point2.Y - currentGrid.Y) < 3.0;
												if (intValue == client.ClientData.MapCode && flag3)
												{
													if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
													{
														client.ClientData.TaskDataList[i].DoingTaskVal1++;
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														flag2 = true;
													}
												}
											}
										}
										if (flag2)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int intValue2 = systemXmlItem.GetIntValue("DestNPC", -1);
												if (-1 != intValue2)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue2, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue2 + 2130706432, state);
												}
											}
										}
									}
									if (systemXmlItem.GetIntValue("TargetType2", -1) == 4)
									{
										bool flag2 = false;
										string stringValue = systemXmlItem.GetStringValue("PropsName2");
										int goodsByName = Global.GetGoodsByName(stringValue);
										if (goodsID == goodsByName)
										{
											int intValue3 = systemXmlItem.GetIntValue("TargetMapCode2", -1);
											Point point3 = Global.StrToPoint(systemXmlItem.GetStringValue("TargetPos2"));
											if (intValue3 >= 0 && !double.IsNaN(point3.X) && !double.IsNaN(point3.Y))
											{
												Point currentGrid = client.CurrentGrid;
												Point point2 = new Point((double)((int)(point3.X / (double)gameMap.MapGridWidth)), (double)((int)(point3.Y / (double)gameMap.MapGridHeight)));
												bool flag3 = Math.Abs(point2.X - currentGrid.X) < 3.0 && Math.Abs(point2.Y - currentGrid.Y) < 3.0;
												if (intValue3 == client.ClientData.MapCode && flag3)
												{
													if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
													{
														client.ClientData.TaskDataList[i].DoingTaskVal2++;
														GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
														{
															client.ClientData.RoleID,
															client.ClientData.TaskDataList[i].DoingTaskID,
															client.ClientData.TaskDataList[i].DbID,
															client.ClientData.TaskDataList[i].DoingTaskFocus,
															client.ClientData.TaskDataList[i].DoingTaskVal1,
															client.ClientData.TaskDataList[i].DoingTaskVal2
														}), null, client.ServerId);
														flag2 = true;
													}
												}
											}
										}
										if (flag2)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int intValue2 = systemXmlItem.GetIntValue("DestNPC", -1);
												if (-1 != intValue2)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue2, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue2 + 2130706432, state);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private static void ProcessLuaHandle(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int goodsID, TaskTypes taskType)
		{
			if (null != client.ClientData.TaskDataList)
			{
				long nowTicks = TimeUtil.NOW();
				bool flag = false;
				SystemXmlItem systemXmlItem = null;
				NPC npc = NPCGeneralManager.FindNPC(client.ClientData.MapCode, extensionID);
				if (null != npc)
				{
					Point currentGrid = client.CurrentGrid;
					Point currentGrid2 = npc.CurrentGrid;
					if (Math.Abs(currentGrid2.X - currentGrid.X) <= 9.0 && Math.Abs(currentGrid2.Y - currentGrid.Y) <= 9.0)
					{
						lock (client.ClientData.TaskDataList)
						{
							for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
							{
								int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
								if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
								{
									if (ProcessTask.IsTaskValid(client, systemXmlItem, client.ClientData.TaskDataList[i], nowTicks))
									{
										if (systemXmlItem.GetIntValue("TargetType1", -1) == 9 || systemXmlItem.GetIntValue("TargetType1", -1) == 10)
										{
											flag = false;
											if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal1++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag = true;
											}
											if (flag)
											{
												GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
												if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
												{
													int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
													if (-1 != intValue)
													{
														int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
														GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
													}
												}
											}
										}
										if (systemXmlItem.GetIntValue("TargetType2", -1) == 9 || systemXmlItem.GetIntValue("TargetType2", -1) == 10)
										{
											flag = false;
											if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
											{
												client.ClientData.TaskDataList[i].DoingTaskVal2++;
												GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
												{
													client.ClientData.RoleID,
													client.ClientData.TaskDataList[i].DoingTaskID,
													client.ClientData.TaskDataList[i].DbID,
													client.ClientData.TaskDataList[i].DoingTaskFocus,
													client.ClientData.TaskDataList[i].DoingTaskVal1,
													client.ClientData.TaskDataList[i].DoingTaskVal2
												}), null, client.ServerId);
												flag = true;
											}
										}
										if (flag)
										{
											GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
											if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
											{
												int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
												if (-1 != intValue)
												{
													int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
													GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static bool IfActivateTask(GameClient client, int taskid)
		{
			SystemXmlItem systemTask = null;
			long nowTicks = TimeUtil.NOW();
			lock (client.ClientData.TaskDataList)
			{
				for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
				{
					if (taskid == client.ClientData.TaskDataList[i].DoingTaskID)
					{
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskid, out systemTask))
						{
							if (ProcessTask.IsTaskValid(client, systemTask, client.ClientData.TaskDataList[i], nowTicks))
							{
								return !Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2);
							}
						}
					}
				}
			}
			return false;
		}

		public static void GMSetMainTaskID(GameClient client, int taskID = 2000)
		{
			int roleID = client.ClientData.RoleID;
			client.ClientData.OldTasks = new List<OldTaskData>();
			client.ClientData.TaskDataList = new List<TaskData>();
			int num = int.MaxValue;
			int num2 = 0;
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.SystemTasksMgr.SystemXmlItemDict)
			{
				SystemXmlItem value = keyValuePair.Value;
				if (keyValuePair.Key < num && keyValuePair.Key >= taskID)
				{
					num = keyValuePair.Key;
					num2 = keyValuePair.Value.GetIntValue("DestNPC", -1);
				}
				if (keyValuePair.Key < taskID)
				{
					list.Add(keyValuePair.Key);
				}
			}
			list.Sort();
			list.Insert(0, roleID);
			if (list.Count > 2)
			{
				list.RemoveRange(1, list.Count - 2);
			}
			Global.sendToDB<int, byte[]>(13000, DataHelper.ObjectToBytes<List<int>>(list), client.ServerId);
			client.sendCmd(140, string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				roleID,
				num2,
				list[list.Count - 1],
				1
			}), false);
			TCPOutPacket tcpoutPacket = null;
			TCPProcessCmdResults tcpprocessCmdResults = Global.TakeNewTask(TCPManager.getInstance(), client.ClientSocket, TCPManager.getInstance().tcpClientPool, TCPManager.getInstance().tcpRandKey, TCPManager.getInstance().TcpOutPacketPool, 125, client, roleID, num, num2, out tcpoutPacket);
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && null != tcpoutPacket)
			{
				client.sendCmd(tcpoutPacket, true);
			}
			Global.ForceCloseClient(client, "", true);
		}

		public static void ProcessTaskValue(SocketListener sl, TCPOutPacketPool pool, GameClient client, string taskName, int valType, int taskVal)
		{
			if (null != client.ClientData.TaskDataList)
			{
				SystemXmlItem systemXmlItem = null;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						int doingTaskID = client.ClientData.TaskDataList[i].DoingTaskID;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(doingTaskID, out systemXmlItem))
						{
							if (!(taskName != systemXmlItem.GetStringValue("Title")))
							{
								if (1 == valType)
								{
									bool flag2 = false;
									if (client.ClientData.TaskDataList[i].DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
									{
										client.ClientData.TaskDataList[i].DoingTaskVal1 = Global.GMin(taskVal, systemXmlItem.GetIntValue("TargetNum1", -1));
										GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.TaskDataList[i].DoingTaskID,
											client.ClientData.TaskDataList[i].DbID,
											client.ClientData.TaskDataList[i].DoingTaskFocus,
											client.ClientData.TaskDataList[i].DoingTaskVal1,
											client.ClientData.TaskDataList[i].DoingTaskVal2
										}), null, client.ServerId);
										flag2 = true;
									}
									if (flag2)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
											}
										}
									}
								}
								else if (2 == valType)
								{
									bool flag2 = false;
									if (client.ClientData.TaskDataList[i].DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
									{
										client.ClientData.TaskDataList[i].DoingTaskVal2 = Global.GMin(taskVal, systemXmlItem.GetIntValue("TargetNum2", -1));
										GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.TaskDataList[i].DoingTaskID,
											client.ClientData.TaskDataList[i].DbID,
											client.ClientData.TaskDataList[i].DoingTaskFocus,
											client.ClientData.TaskDataList[i].DoingTaskVal1,
											client.ClientData.TaskDataList[i].DoingTaskVal2
										}), null, client.ServerId);
										flag2 = true;
									}
									if (flag2)
									{
										GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, client.ClientData.TaskDataList[i].DbID, doingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2, client.ClientData.TaskDataList[i].DoingTaskFocus, client.ClientData.TaskDataList[i].ChengJiuVal);
										if (Global.JugeTaskComplete(client, client.ClientData.TaskDataList[i].DoingTaskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
										{
											int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
											if (-1 != intValue)
											{
												int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue, 0);
												GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue + 2130706432, state);
											}
										}
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		public static void ClearTaskGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, int taskID)
		{
			if (null != client.ClientData.TaskDataList)
			{
				TaskData taskData = Global.GetTaskData(client, taskID);
				if (null != taskData)
				{
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskData.DoingTaskID, out systemXmlItem))
					{
						if (systemXmlItem.GetIntValue("TargetType1", -1) == 5 && "" != systemXmlItem.GetStringValue("PropsName1"))
						{
							string stringValue = systemXmlItem.GetStringValue("PropsName1");
							string name = ProcessTask.GetPropNameGoodsName(stringValue);
							int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
							int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
							int goodsByName = Global.GetGoodsByName(name);
							if (goodsByName >= 0)
							{
								GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
								if (null != notUsingGoodsByID)
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
									if (goodsCatetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, notUsingGoodsByID.Id, false, false);
									}
								}
							}
						}
						else if (systemXmlItem.GetIntValue("TargetType1", -1) == 4 && "" != systemXmlItem.GetStringValue("PropsName1"))
						{
							string name = systemXmlItem.GetStringValue("PropsName1");
							int goodsByName = Global.GetGoodsByName(name);
							if (goodsByName >= 0)
							{
								GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, 0, 0);
								if (null != notUsingGoodsByID)
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
									if (goodsCatetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, notUsingGoodsByID.Id, false, false);
									}
								}
							}
						}
						if (systemXmlItem.GetIntValue("TargetType2", -1) == 5 && "" != systemXmlItem.GetStringValue("PropsName2"))
						{
							string stringValue = systemXmlItem.GetStringValue("PropsName2");
							string name = ProcessTask.GetPropNameGoodsName(stringValue);
							int propNameGoodsLevel = ProcessTask.GetPropNameGoodsLevel(stringValue);
							int propNameGoodsQuality = ProcessTask.GetPropNameGoodsQuality(stringValue);
							int goodsByName = Global.GetGoodsByName(name);
							if (goodsByName >= 0)
							{
								GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, propNameGoodsLevel, propNameGoodsQuality);
								if (null != notUsingGoodsByID)
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
									if (goodsCatetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, notUsingGoodsByID.Id, false, false);
									}
								}
							}
						}
						else if (systemXmlItem.GetIntValue("TargetType2", -1) == 4 && "" != systemXmlItem.GetStringValue("PropsName2"))
						{
							string name = systemXmlItem.GetStringValue("PropsName2");
							int goodsByName = Global.GetGoodsByName(name);
							if (goodsByName >= 0)
							{
								GoodsData notUsingGoodsByID = Global.GetNotUsingGoodsByID(client, goodsByName, 0, 0);
								if (null != notUsingGoodsByID)
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
									if (goodsCatetoriy >= 49)
									{
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, notUsingGoodsByID.Id, false, false);
									}
								}
							}
						}
					}
				}
			}
		}

		public static bool Complete(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int extensionID, int taskID, int dbID, bool useYuanBao, double expBeiShu = 1.0, bool bIsOneClickComlete = false)
		{
			bool result;
			if (null == client.ClientData.TaskDataList)
			{
				result = false;
			}
			else
			{
				int num = -1;
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						if (client.ClientData.TaskDataList[i].DbID == dbID)
						{
							if (bIsOneClickComlete)
							{
								num = i;
								break;
							}
							if (Global.JugeTaskComplete(client, taskID, client.ClientData.TaskDataList[i].DoingTaskVal1, client.ClientData.TaskDataList[i].DoingTaskVal2))
							{
								num = i;
								break;
							}
						}
					}
				}
				if (num < 0)
				{
					result = false;
				}
				else
				{
					TaskData taskData = null;
					lock (client.ClientData.TaskDataList)
					{
						if (num < client.ClientData.TaskDataList.Count)
						{
							taskData = client.ClientData.TaskDataList[num];
							client.ClientData.TaskDataList.RemoveAt(num);
						}
					}
					if (null == taskData)
					{
						result = false;
					}
					else
					{
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
						{
							result = false;
						}
						else
						{
							int intValue = systemXmlItem.GetIntValue("TaskClass", -1);
							if (intValue != 0)
							{
								Global.AddOldTask(client, taskID);
							}
							if ((intValue >= 3 && intValue <= 9) || (intValue >= 100 && intValue <= 150))
							{
								int intValue2 = systemXmlItem.GetIntValue("MinLevel", -1);
								if (!Global.UpdateDailyTaskData(client, intValue2 / 10, taskData.AddDateTime, intValue, bIsOneClickComlete))
								{
									return false;
								}
							}
							GameManager.ClientMgr.NotifyUpdateTask(sl, pool, client, -1, taskID, 0, 0, 0, -1L);
							int state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, npcID - 2130706432, taskID);
							GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, npcID, state);
							int intValue3 = systemXmlItem.GetIntValue("SourceNPC", -1);
							if (-1 != intValue3 && npcID - 2130706432 != intValue3)
							{
								state = Global.ComputeNPCTaskState(client, client.ClientData.TaskDataList, intValue3, taskID);
								GameManager.ClientMgr.NotifyUpdateNPCTaskSate(sl, pool, client, intValue3 + 2130706432, state);
							}
							bool flag3 = false;
							bool flag4 = false;
							if ((systemXmlItem.GetIntValue("TargetType1", -1) == 2 || systemXmlItem.GetIntValue("TargetType1", -1) == 8) && "" != systemXmlItem.GetStringValue("PropsName1"))
							{
								int intValue4 = systemXmlItem.GetIntValue("TargetNum1", -1);
								string stringValue = systemXmlItem.GetStringValue("PropsName1");
								int goodsByName = Global.GetGoodsByName(stringValue);
								if (goodsByName >= 0)
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
									if (goodsCatetoriy >= 49)
									{
										flag3 = false;
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsByName, intValue4, false, out flag3, out flag4, false);
									}
								}
							}
							if ((systemXmlItem.GetIntValue("TargetType2", -1) == 2 || systemXmlItem.GetIntValue("TargetType2", -1) == 8) && "" != systemXmlItem.GetStringValue("PropsName2"))
							{
								int intValue5 = systemXmlItem.GetIntValue("TargetNum2", -1);
								string stringValue = systemXmlItem.GetStringValue("PropsName2");
								int goodsByName = Global.GetGoodsByName(stringValue);
								if (goodsByName >= 0)
								{
									int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsByName);
									if (goodsCatetoriy >= 49)
									{
										flag3 = false;
										GameManager.ClientMgr.NotifyUseGoods(sl, Global._TCPManager.tcpClientPool, pool, client, goodsByName, intValue5, false, out flag3, out flag4, false);
									}
								}
							}
							int num2 = GameManager.TaskAwardsMgr.FindNeedYuanBao(client, taskID);
							int num3 = 0;
							int num4 = 0;
							int num5 = 0;
							int num6 = 0;
							int binding = 1;
							int num7 = 0;
							bool flag5 = false;
							TaskStarDataInfo taskStarDataInfo = null;
							int num8 = 0;
							if (intValue == 8)
							{
								DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, intValue);
								if (dailyTaskData != null && taskData.StarLevel > 0 && taskData.StarLevel <= Data.TaskStarInfo.Count)
								{
									if (dailyTaskData.RecNum == Global.GetMaxDailyTaskNum(client, intValue, dailyTaskData))
									{
										int dailyCircleTaskAddAward = Global.GetDailyCircleTaskAddAward(client);
										if (dailyCircleTaskAddAward > 0)
										{
											num3 = Data.DailyCircleTaskAward[dailyCircleTaskAddAward].Experience;
											num4 = Data.DailyCircleTaskAward[dailyCircleTaskAddAward].MoJing;
											num5 = Data.DailyCircleTaskAward[dailyCircleTaskAddAward].GoodsID;
											num6 = Data.DailyCircleTaskAward[dailyCircleTaskAddAward].GoodsNum;
											binding = Data.DailyCircleTaskAward[dailyCircleTaskAddAward].Binding;
											num7 = Data.DailyCircleTaskAward[dailyCircleTaskAddAward].XingHun;
										}
									}
									taskStarDataInfo = Data.TaskStarInfo[taskData.StarLevel - 1];
									if (taskStarDataInfo != null)
									{
										flag5 = true;
									}
								}
								DailyActiveManager.ProcessCompleteDailyTaskForDailyActive(client, dailyTaskData.RecNum);
							}
							else if (intValue == 9)
							{
								DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, intValue);
								if (dailyTaskData != null)
								{
									if (dailyTaskData.RecNum == Global.GetMaxDailyTaskNum(client, intValue, dailyTaskData))
									{
										num8 = Data.TaofaTaskExAward.BangZuan;
									}
								}
							}
							long num9 = GameManager.TaskAwardsMgr.FindExperience(client, taskID);
							if (num9 > 0L)
							{
								if (useYuanBao)
								{
									num9 *= 2L;
								}
								num9 = (long)((double)num9 * expBeiShu);
								if (flag5)
								{
									num9 = (long)((double)num9 * taskStarDataInfo.ExpModulus);
								}
								num9 += (long)num3;
								GameManager.ClientMgr.ProcessRoleExperience(client, num9, true, false, false, "none");
							}
							if (Global.FilterFallGoods(client))
							{
								List<GoodsData> list = Global.GetTaskAwardsGoodsGridCount(client, taskID);
								if (num5 > 0 && num6 > 0)
								{
									GoodsData goodsData = new GoodsData();
									goodsData.GoodsID = num5;
									goodsData.GCount = num6;
									goodsData.Binding = binding;
									goodsData.Endtime = "1900-01-01 12:00:00";
									if (list == null)
									{
										list = new List<GoodsData>();
									}
									list.Add(goodsData);
								}
								if (list != null && list.Count > 0)
								{
									if (!Global.CanAddGoodsDataList(client, list))
									{
										ProcessTask.SendMailWhenPacketFull(client, list, GLang.GetLang(4019, new object[0]), GLang.GetLang(4019, new object[0]));
									}
									else
									{
										for (int i = 0; i < list.Count; i++)
										{
											Global.AddGoodsDBCommand(pool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "任务奖励", list[i].Endtime, 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
										}
									}
								}
							}
							int num10 = GameManager.TaskAwardsMgr.FindMoney(taskID);
							if (0 < num10)
							{
								num10 = Global.FilterValue(client, num10);
								GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num10, "完成任务：" + taskID, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.Money1,
									num10
								}), EventLevels.Record);
							}
							int num11 = GameManager.TaskAwardsMgr.FindYinLiang(taskID);
							if (0 < num11)
							{
								num11 = Global.FilterValue(client, num11);
								GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num11, "完成任务：" + taskID, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.YinLiang,
									num11
								}), EventLevels.Record);
							}
							int num12 = GameManager.TaskAwardsMgr.FindBindYuanBao(taskID);
							num12 += num8;
							if (0 < num12)
							{
								num12 = Global.FilterValue(client, num12);
								GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num12, "完成任务：" + taskID);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取绑定元宝, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.Gold,
									num12
								}), EventLevels.Record);
							}
							if (0 < num4)
							{
								GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, num4, "日常跑环", false, true, false);
							}
							int num13 = GameManager.TaskAwardsMgr.FindLingLi(taskID);
							if (0 < num13)
							{
								GameManager.ClientMgr.AddInterPower(client, num13, true, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取灵力, roleID={0}({1}), LingLi={2}, newLingLi={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.InterPower,
									num13
								}), EventLevels.Record);
							}
							int num14 = GameManager.TaskAwardsMgr.FindBlessPoint(taskID);
							if (num14 > 0)
							{
								num14 = Global.FilterValue(client, num14);
								int num15 = ProcessHorse.ProcessAddHorseAwardLucky(client, num14, false, "坐骑任务");
								if (num15 >= 0)
								{
									GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务祝福点成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										num14,
										num15
									}), EventLevels.Record);
								}
								else
								{
									GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务祝福点失败, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										num14,
										num15
									}), EventLevels.Record);
								}
							}
							int num16 = GameManager.TaskAwardsMgr.FindZhenQi(client, taskID);
							if (num16 > 0)
							{
								num16 = Global.FilterValue(client, num16);
								GameManager.ClientMgr.ModifyZhenQiValue(client, num16, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务阵旗成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									num16,
									0
								}), EventLevels.Record);
							}
							int num17 = GameManager.TaskAwardsMgr.FindLieSha(client, taskID);
							if (num17 > 0)
							{
								num17 = Global.FilterValue(client, num17);
								if (useYuanBao)
								{
									num17 *= 2;
								}
								GameManager.ClientMgr.ModifyLieShaValue(client, num17, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务猎杀值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									num17,
									0
								}), EventLevels.Record);
							}
							int num18 = GameManager.TaskAwardsMgr.FindWuXing(client, taskID);
							if (num18 > 0)
							{
								num18 = Global.FilterValue(client, num18);
								GameManager.ClientMgr.ModifyWuXingValue(client, num18, true, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务悟性值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									num18,
									0
								}), EventLevels.Record);
							}
							int num19 = GameManager.TaskAwardsMgr.FindJunGong(client, taskID);
							if (num19 > 0)
							{
								num19 = Global.FilterValue(client, num19);
								GameManager.ClientMgr.ModifyJunGongValue(client, num19, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务军功值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									num19,
									0
								}), EventLevels.Record);
							}
							int num20 = GameManager.TaskAwardsMgr.FindRongYu(client, taskID);
							if (num20 > 0)
							{
								num20 = Global.FilterValue(client, num20);
								GameManager.ClientMgr.ModifyRongYuValue(client, num20, true, true);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务荣誉值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									num20,
									0
								}), EventLevels.Record);
							}
							int num21 = GameManager.TaskAwardsMgr.FindMoJing(client, taskID);
							if (num21 > 0)
							{
								num21 = Global.FilterValue(client, num21);
								if (flag5)
								{
									num21 = (int)((double)num21 * taskStarDataInfo.BindYuanBaoModulus);
								}
								GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, num21, "过滤奖励", false, true, false);
								GameManager.SystemServerEvents.AddEvent(string.Format("角色获取任务魔晶值成功, roleID={0}({1}), newBlessPoint={2}, RetCode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									num21,
									0
								}), EventLevels.Record);
							}
							int num22 = GameManager.TaskAwardsMgr.FindXingHun(client, taskID);
							if (num22 > 0)
							{
								num22 = Global.FilterValue(client, num22);
								if (flag5)
								{
									num22 = (int)((double)num22 * taskStarDataInfo.StarSoulModulus);
								}
								num22 += num7;
								GameManager.ClientMgr.ModifyStarSoulValue(client, num22, "过滤奖励", true, true);
							}
							int num23 = GameManager.TaskAwardsMgr.FindCompDonate(client, taskID);
							if (num23 > 0)
							{
								num23 = Global.FilterValue(client, num23);
								GameManager.ClientMgr.ModifyCompDonateValue(client, num23, "过滤奖励", true, true, false);
							}
							int num24 = GameManager.TaskAwardsMgr.FindCompJunXian(client, taskID);
							if (num24 > 0 && client.ClientData.CompType > 0)
							{
								num24 = Global.FilterValue(client, num24);
								TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, num24);
								string msgText = string.Format(GLang.GetLang(4017, new object[0]), num24);
								GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							if (taskID == Data.InsertAwardtPortableBagTaskID)
							{
								string[] array = Data.InsertAwardtPortableBagGoodsInfo.Split(new char[]
								{
									','
								});
								if (array != null)
								{
									GoodsData goodsData2 = new GoodsData();
									goodsData2.GoodsID = Global.SafeConvertToInt32(array[0]);
									goodsData2.GCount = Global.SafeConvertToInt32(array[1]);
									goodsData2.Binding = Global.SafeConvertToInt32(array[2]);
									goodsData2.Forge_level = Global.SafeConvertToInt32(array[3]);
									goodsData2.AppendPropLev = Global.SafeConvertToInt32(array[4]);
									goodsData2.Lucky = Global.SafeConvertToInt32(array[5]);
									goodsData2.ExcellenceInfo = Global.SafeConvertToInt32(array[6]);
									goodsData2.Site = -1000;
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, 0, "", goodsData2.Forge_level, goodsData2.Binding, goodsData2.Site, "", true, 1, "引导给物品到仓库", "1900-01-01 12:00:00", 0, 0, goodsData2.Lucky, 0, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, goodsData2.ChangeLifeLevForEquip, null, null, 0, true);
								}
							}
							Global.AddRoleTaskEvent(client, taskID);
							if (intValue == 0)
							{
								Global.UpdateTaskZhangJieProp(client, taskID, false);
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static void SendMailWhenPacketFull(GameClient client, List<GoodsData> awardsItemList, string strSubject, string strContent)
		{
			int num = awardsItemList.Count / 5;
			int num2 = awardsItemList.Count % 5;
			int num3 = 0;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					List<GoodsData> list = new List<GoodsData>();
					for (int j = 0; j < 5; j++)
					{
						list.Add(awardsItemList[num3]);
						num3++;
					}
					Global.UseMailGivePlayerAward2(client, list, strSubject, strContent, 0, 0, 0);
				}
			}
			if (num2 > 0)
			{
				List<GoodsData> list2 = new List<GoodsData>();
				for (int i = 0; i < num2; i++)
				{
					list2.Add(awardsItemList[num3]);
					num3++;
				}
				Global.UseMailGivePlayerAward2(client, list2, strSubject, strContent, 0, 0, 0);
			}
		}

		public static Dictionary<int, List<BranchTaskInfo>> BranchTaskInfoDic = new Dictionary<int, List<BranchTaskInfo>>();

		public static Dictionary<long, List<int>> ActiveBranchTaskInfoDic = new Dictionary<long, List<int>>();
	}
}
