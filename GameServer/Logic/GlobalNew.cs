using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.TuJian;
using GameServer.Server;
using HSGameEngine.Tools.AStar;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class GlobalNew
	{
		public static bool IsGongNengOpened(GameClient client, GongNengIDs id, bool hint = false)
		{
			int num = id;
			if (num >= 100000 && num < 120000)
			{
				num -= 100000;
			}
			bool result;
			if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(num))
			{
				result = false;
			}
			else if (null == client)
			{
				result = true;
			}
			else if (client.ClientData.HideGM > 0)
			{
				result = true;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (GameManager.SystemSystemOpen.SystemXmlItemDict.TryGetValue(id, out systemXmlItem))
				{
					int intValue = systemXmlItem.GetIntValue("TriggerCondition", -1);
					if (intValue == 1)
					{
						int[] intArrayValue = systemXmlItem.GetIntArrayValue("TimeParameters", ',');
						if (intArrayValue.Length == 2)
						{
							if (Global.GetUnionLevel(intArrayValue[0], intArrayValue[1], false) > Global.GetUnionLevel(client, false))
							{
								if (hint)
								{
									string msg = string.Format(GLang.GetLang(374, new object[0]), intArrayValue[0], intArrayValue[1]);
									GameManager.ClientMgr.NotifyHintMsg(client, msg);
								}
								return false;
							}
						}
						return true;
					}
					if (intValue == 7)
					{
						int intValue2 = systemXmlItem.GetIntValue("TimeParameters", -1);
						if (client.ClientData.MainTaskID < intValue2)
						{
							if (hint)
							{
								string msg = string.Format(GLang.GetLang(375, new object[0]), GlobalNew.GetTaskName(intValue2));
								GameManager.ClientMgr.NotifyHintMsg(client, msg);
							}
							return false;
						}
						return true;
					}
					else if (intValue == 14)
					{
						string stringValue = systemXmlItem.GetStringValue("TimeParameters");
						if (string.IsNullOrEmpty(stringValue))
						{
							return true;
						}
						string[] array = stringValue.Split(new char[]
						{
							','
						});
						if (array.Length != 2)
						{
							return true;
						}
						int num2 = Convert.ToInt32(array[0]);
						int num3 = Convert.ToInt32(array[1]);
						return client.ClientData.MyWingData.WingID > num2 || (client.ClientData.MyWingData.WingID == num2 && client.ClientData.MyWingData.ForgeLevel >= num3);
					}
					else if (intValue == 15)
					{
						if (client.ClientData.ChengJiuLevel < systemXmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (intValue == 16)
					{
						int shengWangLevelValue = GameManager.ClientMgr.GetShengWangLevelValue(client);
						if (shengWangLevelValue < systemXmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (intValue == 17)
					{
						if (TimeUtil.GetOffsetDays(Global.GetKaiFuTime()) < systemXmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (intValue == 18)
					{
						if ((ulong)client.ClientData.TotalDayLoginNum < (ulong)((long)systemXmlItem.GetIntValue("TimeParameters", -1)))
						{
							return false;
						}
					}
					else if (intValue == 20)
					{
						int bangHuiLevel = Global.GetBangHuiLevel(client);
						if (bangHuiLevel < systemXmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
					else if (intValue == 21)
					{
						if (client.ClientData.RebornLevel < systemXmlItem.GetIntValue("TimeParameters", -1))
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		public static void RefreshGongNeng(GameClient client)
		{
			CaiJiLogic.InitRoleDailyCaiJiData(client, false, false);
			HuanYingSiYuanManager.getInstance().InitRoleDailyHYSYData(client);
			Global.InitRoleDailyTaskData(client, false);
			SingletonTemplate<GuardStatueManager>.Instance().OnTaskComplete(client);
			GameManager.MerlinMagicBookMgr.InitMerlinMagicBook(client);
			SingletonTemplate<SoulStoneManager>.Instance().CheckOpen(client);
			SingletonTemplate<ZhengBaManager>.Instance().CheckGongNengCanOpen(client);
			FundManager.initFundData(client);
			SingletonTemplate<CoupleArenaManager>.Instance().CheckGongNengOpen(client);
			ShenShiManager.getInstance().InitRoleShenShiData(client);
			JueXingManager.getInstance().InitRoleJueXingData(client);
			ZuoQiManager.getInstance().InitRoleZuoQiData(client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		public static int GetFuBenTabNeedTask(int fuBenTabId)
		{
			int result = 0;
			if (!Data.FuBenNeedDict.TryGetValue(fuBenTabId, out result))
			{
				result = 0;
			}
			return result;
		}

		public static bool IsExtraGongNengOpen(GameClient client, ExtraGongNengIds extGongId)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (extGongId == ExtraGongNengIds.DiaoXiangMoBai)
			{
				num = (int)GameManager.systemParamsList.GetParamValueIntByName("MoBaiLevel", -1);
			}
			return (num <= 0 || num <= Global.GetUnionLevel(client, false)) && (num2 <= 0 || num2 <= client.ClientData.MainTaskID) && (num3 <= 0 || num3 <= client.ClientData.VipLevel);
		}

		public static string GetTaskName(int taskId)
		{
			SystemXmlItem systemXmlItem = null;
			string result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskId, out systemXmlItem))
			{
				result = taskId.ToString();
			}
			else
			{
				result = systemXmlItem.GetStringValue("Title");
			}
			return result;
		}

		public static string PrintRoleProps(string otherRoleIdOrName)
		{
			string result = null;
			try
			{
				int num = RoleName2IDs.FindRoleIDByName(otherRoleIdOrName, false);
				if (-1 == num)
				{
					if (!int.TryParse(otherRoleIdOrName, out num))
					{
						return result;
					}
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(num);
				if (null == gameClient)
				{
					return result;
				}
				StringBuilder stringBuilder = new StringBuilder();
				Global.PrintSomeProps(gameClient, ref stringBuilder);
				result = stringBuilder.ToString();
			}
			catch (Exception ex)
			{
			}
			return result;
		}

		public static bool GetNpcTaskData(GameClient client, int extensionID, NPCData npcData)
		{
			List<int> list = null;
			bool result;
			if (!GameManager.NPCTasksMgr.SourceNPCTasksDict.TryGetValue(extensionID, out list))
			{
				result = false;
			}
			else if (0 == list.Count)
			{
				result = false;
			}
			else
			{
				Dictionary<int, GlobalNew.NpcCircleTaskData> dictionary = null;
				for (int i = 0; i < list.Count; i++)
				{
					int num = list[i];
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(num, out systemXmlItem))
					{
						int intValue = systemXmlItem.GetIntValue("TaskClass", -1);
						if ((intValue >= 3 && intValue <= 9) || (intValue >= 100 && intValue <= 150))
						{
							if (Global.CanTaskPaoHuanTask(client, intValue))
							{
								if (Global.CanTakeNewTask(client, num, systemXmlItem))
								{
									GlobalNew.NpcCircleTaskData npcCircleTaskData = null;
									if (dictionary == null || !dictionary.TryGetValue(intValue, out npcCircleTaskData))
									{
										npcCircleTaskData = new GlobalNew.NpcCircleTaskData();
										npcCircleTaskData.taskclass = intValue;
										npcCircleTaskData.oldTaskID = PaoHuanTasksMgr.FindPaoHuanHistTaskID(client.ClientData.RoleID, intValue);
										if (npcCircleTaskData.oldTaskID >= 0)
										{
											if (!Global.CanTakeNewTask(client, npcCircleTaskData.oldTaskID, null))
											{
												npcCircleTaskData.oldTaskID = -1;
											}
										}
										if (null == dictionary)
										{
											dictionary = new Dictionary<int, GlobalNew.NpcCircleTaskData>();
										}
										dictionary[intValue] = npcCircleTaskData;
									}
									if (null != npcCircleTaskData)
									{
										npcCircleTaskData.NpcAttachedTaskID.Add(num);
									}
								}
							}
						}
						else if (Global.CanTakeNewTask(client, num, systemXmlItem))
						{
							if (null == npcData.NewTaskIDs)
							{
								npcData.NewTaskIDs = new List<int>();
							}
							npcData.NewTaskIDs.Add(num);
							if (2 == intValue)
							{
								OldTaskData oldTaskData = Global.FindOldTaskByTaskID(client, list[i]);
								int item = (oldTaskData == null) ? 0 : oldTaskData.DoCount;
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(item);
							}
							else
							{
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(0);
							}
						}
					}
				}
				if (null == dictionary)
				{
					result = true;
				}
				else
				{
					foreach (KeyValuePair<int, GlobalNew.NpcCircleTaskData> keyValuePair in dictionary)
					{
						bool flag = false;
						if (-1 != keyValuePair.Value.oldTaskID)
						{
							if (0 == keyValuePair.Value.NpcAttachedTaskID.Count)
							{
								continue;
							}
							if (-1 != keyValuePair.Value.NpcAttachedTaskID.IndexOf(keyValuePair.Value.oldTaskID))
							{
								if (null == npcData.NewTaskIDs)
								{
									npcData.NewTaskIDs = new List<int>();
								}
								npcData.NewTaskIDs.Add(keyValuePair.Value.oldTaskID);
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(0);
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							flag = true;
						}
						if (flag)
						{
							int num2 = keyValuePair.Value.DoRandomTaskID(client);
							if (-1 != num2)
							{
								if (null == npcData.NewTaskIDs)
								{
									npcData.NewTaskIDs = new List<int>();
								}
								npcData.NewTaskIDs.Add(num2);
								if (null == npcData.NewTaskIDsDoneCount)
								{
									npcData.NewTaskIDsDoneCount = new List<int>();
								}
								npcData.NewTaskIDsDoneCount.Add(0);
								PaoHuanTasksMgr.SetPaoHuanHistTaskID(client.ClientData.RoleID, keyValuePair.Value.taskclass, num2);
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static bool GetNpcFunctionData(GameClient client, int extensionID, NPCData npcData, SystemXmlItem systemNPC)
		{
			bool result;
			if (null == systemNPC)
			{
				result = false;
			}
			else
			{
				string stringValue = systemNPC.GetStringValue("Operations");
				stringValue.Trim();
				if (stringValue != "")
				{
					int[] array = Global.StringArray2IntArray(stringValue.Split(new char[]
					{
						','
					}));
					if (null == npcData.OperationIDs)
					{
						npcData.OperationIDs = new List<int>();
					}
					for (int i = 0; i < array.Length; i++)
					{
						if (!Global.FilterNPCOperationByID(client, array[i], extensionID))
						{
							npcData.OperationIDs.Add(array[i]);
						}
					}
				}
				string text = systemNPC.GetStringValue("Scripts");
				if (null != text)
				{
					text = text.Trim();
				}
				if (!string.IsNullOrEmpty(text))
				{
					int[] array2 = Global.StringArray2IntArray(text.Split(new char[]
					{
						','
					}));
					if (null == npcData.ScriptIDs)
					{
						npcData.ScriptIDs = new List<int>();
					}
					for (int i = 0; i < array2.Length; i++)
					{
						int num = 0;
						if (!Global.FilterNPCScriptByID(client, array2[i], out num))
						{
							npcData.ScriptIDs.Add(array2[i]);
						}
					}
				}
				result = true;
			}
			return result;
		}

		public static TCPClient PopGameDbClient(int serverId, int poolId)
		{
			TCPClient result;
			if (serverId <= 0 || serverId == GameManager.ServerId || serverId == GameManager.KuaFuServerId)
			{
				if (poolId == 0)
				{
					result = Global._TCPManager.tcpClientPool.Pop();
				}
				else
				{
					result = Global._TCPManager.tcpLogClientPool.Pop();
				}
			}
			else
			{
				result = KuaFuManager.getInstance().PopGameDbClient(serverId, poolId);
			}
			return result;
		}

		public static void PushGameDbClient(int serverId, TCPClient tcpClient, int poolId)
		{
			if (serverId <= 0 || serverId == GameManager.ServerId || serverId == GameManager.KuaFuServerId)
			{
				if (poolId == 0)
				{
					Global._TCPManager.tcpClientPool.Push(tcpClient);
				}
				else
				{
					Global._TCPManager.tcpLogClientPool.Push(tcpClient);
				}
			}
			else
			{
				KuaFuManager.getInstance().PushGameDbClient(serverId, tcpClient, poolId);
			}
		}

		public static void UpdateKuaFuRoleDayLogData(int serverId, int roleId, DateTime now, int zoneId, int signUpCount, int startGameCount, int successCount, int faildCount, int gameType)
		{
			Global.SendToDB<RoleKuaFuDayLogData>(20003, new RoleKuaFuDayLogData
			{
				RoleID = roleId,
				Day = now.Date.ToString("yyyy-MM-dd"),
				ZoneId = zoneId,
				SignupCount = signUpCount,
				StartGameCount = startGameCount,
				SuccessCount = successCount,
				FaildCount = faildCount,
				GameType = gameType
			}, serverId);
		}

		public static void RecordSwitchKuaFuServerLog(GameClient client)
		{
			ushort mapCode = 0;
			ushort x = 0;
			ushort y = 0;
			if (0 == Global.GetMapSceneType(client.ClientData.MapCode))
			{
				mapCode = (ushort)client.CurrentMapCode;
				x = (ushort)client.CurrentGrid.X;
				y = (ushort)client.CurrentGrid.Y;
			}
			Global.ModifyMapRecordData(client, mapCode, x, y, 0);
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			LogManager.WriteLog(2, string.Format("GameType={5},RoleId={0},GameId={1},SrcServerId={2},KfIp={3},KfPort={4}", new object[]
			{
				clientKuaFuServerLoginData.RoleId,
				clientKuaFuServerLoginData.GameId,
				clientKuaFuServerLoginData.ServerId,
				clientKuaFuServerLoginData.ServerIp,
				clientKuaFuServerLoginData.ServerPort,
				clientKuaFuServerLoginData.GameType
			}), null, true);
			EventLogManager.AddGameEvent(LogRecordType.KuaFu, new object[]
			{
				clientKuaFuServerLoginData.GameType,
				clientKuaFuServerLoginData.RoleId,
				client.ClientData.Faction,
				client.ClientData.JunTuanId,
				clientKuaFuServerLoginData.GameId
			});
		}

		public static List<int[]> FindPath(Point startPoint, Point endPoint, int mapCode)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			List<int[]> result;
			if (null == gameMap)
			{
				result = null;
			}
			else
			{
				PathFinderFast pathFinderFast;
				if (GlobalNew._pathStack.Count <= 0)
				{
					pathFinderFast = new PathFinderFast(gameMap.MyNodeGrid.GetFixedObstruction())
					{
						Formula = HeuristicFormula.Manhattan,
						Diagonals = true,
						HeuristicEstimate = 2,
						ReopenCloseNodes = true,
						SearchLimit = int.MaxValue,
						Punish = null,
						MaxNum = Global.GMax(gameMap.MapGridWidth, gameMap.MapGridHeight)
					};
				}
				else
				{
					pathFinderFast = GlobalNew._pathStack.Pop();
				}
				startPoint.X = (double)(gameMap.CorrectWidthPointToGridPoint((int)startPoint.X) / gameMap.MapGridWidth);
				startPoint.Y = (double)(gameMap.CorrectHeightPointToGridPoint((int)startPoint.Y) / gameMap.MapGridHeight);
				endPoint.X = (double)(gameMap.CorrectWidthPointToGridPoint((int)endPoint.X) / gameMap.MapGridWidth);
				endPoint.Y = (double)(gameMap.CorrectHeightPointToGridPoint((int)endPoint.Y) / gameMap.MapGridHeight);
				pathFinderFast.EnablePunish = false;
				List<PathFinderNode> list = pathFinderFast.FindPath(startPoint, endPoint);
				if (list == null || list.Count <= 0)
				{
					result = null;
				}
				else
				{
					List<int[]> list2 = new List<int[]>();
					for (int i = 0; i < list.Count; i++)
					{
						list2.Add(new int[]
						{
							list[i].X,
							list[i].Y
						});
					}
					result = list2;
				}
			}
			return result;
		}

		public static bool Copy<T>(T sData, ref T rData)
		{
			try
			{
				foreach (FieldInfo fieldInfo in rData.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					fieldInfo.SetValue(rData, fieldInfo.GetValue(sData));
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, ex.ToString(), null, true);
			}
			return false;
		}

		public static GoodsData ParseGoodsData(string fields)
		{
			try
			{
				string[] array = fields.Split(new char[]
				{
					','
				});
				if (array.Length != 7)
				{
					LogManager.WriteLog(2, string.Format("解析 ParseGoodsData 配置项错误 fields={0}", fields), null, true);
					return null;
				}
				int[] array2 = Global.StringArray2IntArray(array);
				return Global.GetNewGoodsData(array2[0], array2[1], 0, array2[3], array2[2], 0, array2[5], 0, array2[6], array2[4], 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
			return null;
		}

		private static Stack<PathFinderFast> _pathStack = new Stack<PathFinderFast>();

		public class NpcCircleTaskData
		{
			public int DoRandomTaskID(GameClient client)
			{
				int result;
				if (0 == this.NpcAttachedTaskID.Count)
				{
					result = -1;
				}
				else if (this.taskclass == 8)
				{
					result = Global.GetDailyCircleTaskIDBaseChangeLifeLev(client);
				}
				else if (this.taskclass == 9)
				{
					result = Global.GetTaofaTaskIDBaseChangeLifeLev(client);
				}
				else
				{
					int randomNumber = Global.GetRandomNumber(0, this.NpcAttachedTaskID.Count);
					result = this.NpcAttachedTaskID[randomNumber];
				}
				return result;
			}

			public int taskclass = 0;

			public int oldTaskID = 0;

			public List<int> NpcAttachedTaskID = new List<int>();
		}
	}
}
