using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Building
{
	public class BuildingManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static BuildingManager getInstance()
		{
			return BuildingManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1550, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1551, 3, 3, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1552, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1553, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1554, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1555, 2, 2, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1556, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1557, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1558, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1559, 1, 1, BuildingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!GlobalNew.IsGongNengOpened(client, 70, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1550:
					result = this.ProcessBuildGetListCmd(client, nID, bytes, cmdParams);
					break;
				case 1551:
					result = this.ProcessBuildExcuteCmd(client, nID, bytes, cmdParams);
					break;
				case 1552:
					result = this.ProcessBuildFinishCmd(client, nID, bytes, cmdParams);
					break;
				case 1553:
					result = this.ProcessBuildRefreshCmd(client, nID, bytes, cmdParams);
					break;
				case 1554:
					result = this.ProcessBuildGetAllLevelAwardCmd(client, nID, bytes, cmdParams);
					break;
				case 1555:
					result = this.ProcessBuildGetAwardCmd(client, nID, bytes, cmdParams);
					break;
				case 1556:
					result = this.ProcessBuildOpenQueueCmd(client, nID, bytes, cmdParams);
					break;
				case 1557:
					result = this.ProcessBuildGetQueueCmd(client, nID, bytes, cmdParams);
					break;
				case 1558:
					result = this.ProcessBuildGetStateCmd(client, nID, bytes, cmdParams);
					break;
				case 1559:
					result = this.ProcessBuildGetAllLevelAwardStateCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public BuildingData GetBuildingData(GameClient client, int BuildID)
		{
			BuildingData result = null;
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				if (client.ClientData.BuildingDataList[i].BuildId == BuildID)
				{
					result = client.ClientData.BuildingDataList[i];
					break;
				}
			}
			return result;
		}

		public void OnRoleLogin(GameClient client)
		{
			try
			{
				if (!GameFuncControlManager.IsGameFuncDisabled(7))
				{
					if (null != client.ClientData.BuildingDataList)
					{
						if (client.ClientData.BuildingDataList.Count == 0)
						{
							this.GeneralBuildingData(client);
						}
						else if (client.ClientData.BuildingDataList.Count < this.BuildDict.Count)
						{
							this.GeneralBuildingData(client);
						}
						else if (this.GetOpenPayTeamNum(client) == 0 && client.ClientData.BuildingDataList[0].TaskID_4 == 0)
						{
							this.GeneralBuildingData(client);
						}
						if (GlobalNew.IsGongNengOpened(client, 70, false))
						{
							this.BuildingDataChecking(client);
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		public void RandomBuildTaskData(GameClient client, int BuildID, BuildingData myBuildData, bool ConstRefresh = false)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(7))
			{
				this.BuildTaskData(client, BuildID, myBuildData, ConstRefresh);
			}
		}

		public void BuildTaskData(GameClient client, int BuildID, BuildingData myBuildData, bool ConstRefresh = false)
		{
			BuildingConfigData buildingConfigData = null;
			this.BuildDict.TryGetValue(BuildID, out buildingConfigData);
			if (null != buildingConfigData)
			{
				myBuildData.BuildId = BuildID;
				myBuildData.BuildTime = BuildingManager.ConstBuildTime;
				lock (this.RandomTaskMutex)
				{
					myBuildData.TaskID_1 = this.BuildTask(myBuildData.BuildId, BuildingQuality.White);
					myBuildData.TaskID_2 = this.BuildTask(myBuildData.BuildId, BuildingQuality.Green);
					myBuildData.TaskID_3 = this.BuildTask(myBuildData.BuildId, BuildingQuality.Blue);
					myBuildData.TaskID_4 = this.BuildTask(myBuildData.BuildId, BuildingQuality.Purple);
				}
			}
		}

		public void ResetRandSkipVavle()
		{
			foreach (KeyValuePair<int, BuildingTaskConfigData> keyValuePair in this.BuildTaskDict)
			{
				if (keyValuePair.Value.RandSkip)
				{
					keyValuePair.Value.RandSkip = false;
				}
			}
		}

		public int RandomBuildTask(int BuildID, BuildingQuality quality)
		{
			List<BuildingTaskConfigData> list = new List<BuildingTaskConfigData>();
			foreach (KeyValuePair<int, BuildingTaskConfigData> keyValuePair in this.BuildTaskDict)
			{
				if (!keyValuePair.Value.RandSkip && keyValuePair.Value.BuildID == BuildID && keyValuePair.Value.quality == quality)
				{
					list.Add(keyValuePair.Value);
				}
			}
			int randomNumber = Global.GetRandomNumber(0, list.Count);
			int result;
			if (list.Count != 0)
			{
				list[randomNumber].RandSkip = true;
				result = list[randomNumber].TaskID;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int BuildTask(int BuildID, BuildingQuality quality)
		{
			List<BuildingTaskConfigData> list = new List<BuildingTaskConfigData>();
			foreach (KeyValuePair<int, BuildingTaskConfigData> keyValuePair in this.NewBuildTaskDict)
			{
				if (keyValuePair.Value.BuildID == BuildID && keyValuePair.Value.quality == quality)
				{
					return keyValuePair.Value.TaskID;
				}
			}
			return 0;
		}

		public BuildingQuality RandomQualityByList(List<BuildingRandomData> RandomList)
		{
			double num = 0.0;
			double num2 = (double)Global.GetRandomNumber(1, 101) / 100.0;
			for (int i = 0; i < RandomList.Count; i++)
			{
				num += RandomList[i].rate;
				if (num2 <= num)
				{
					return RandomList[i].quality;
				}
			}
			return BuildingQuality.Null;
		}

		public void UpdateBuildingLogDB(GameClient client, BuildingLogType BuildLogType)
		{
			EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.Building, LogRecordType.IntValue, new object[]
			{
				(int)BuildLogType
			});
		}

		public void UpdateBuildingDataDB(GameClient client, BuildingData myBuildData)
		{
			if (null != myBuildData)
			{
				string text = null;
				if (!string.IsNullOrEmpty(myBuildData.BuildTime))
				{
					text = myBuildData.BuildTime.Replace(':', '$');
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					client.ClientData.RoleID,
					myBuildData.BuildId,
					myBuildData.BuildLev,
					myBuildData.BuildExp,
					text,
					myBuildData.TaskID_1,
					myBuildData.TaskID_2,
					myBuildData.TaskID_3,
					myBuildData.TaskID_4
				});
				Global.ExecuteDBCmd(13300, strcmd, client.ServerId);
			}
		}

		public void BuildingDataChecking(GameClient client)
		{
			try
			{
				List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
				List<BuildTeam> list = new List<BuildTeam>();
				List<BuildTeam> list2 = new List<BuildTeam>();
				client.ClientData.NengLiangSmall = Global.GetRoleParamsInt32FromDB(client, "10168");
				client.ClientData.NengLiangMedium = Global.GetRoleParamsInt32FromDB(client, "10169");
				client.ClientData.NengLiangBig = Global.GetRoleParamsInt32FromDB(client, "10170");
				client.ClientData.NengLiangSuper = Global.GetRoleParamsInt32FromDB(client, "10171");
				int num = 4;
				for (int i = 0; i < num; i++)
				{
					BuildingData BuildData = client.ClientData.BuildingDataList[i];
					if (BuildData.TaskID_1 == 0 || BuildData.TaskID_2 == 0 || BuildData.TaskID_3 == 0 || (BuildData.TaskID_4 == 0 && BuildData.TaskID_3 >= 10000))
					{
						this.RandomBuildTaskData(client, BuildData.BuildId, BuildData, false);
						this.UpdateBuildingDataDB(client, BuildData);
					}
					if (0 == string.Compare(BuildData.BuildTime, BuildingManager.ConstBuildTime))
					{
						BuildTeam buildTeam = buildingQueueData.Find((BuildTeam _da) => _da.BuildID == BuildData.BuildId);
						if (null != buildTeam)
						{
							list.Add(buildTeam);
						}
					}
					else
					{
						BuildTeam buildTeam = buildingQueueData.Find((BuildTeam _da) => _da.BuildID == BuildData.BuildId);
						if (null == buildTeam)
						{
							list2.Add(new BuildTeam
							{
								BuildID = BuildData.BuildId,
								TaskID = BuildData.TaskID_1
							});
						}
					}
				}
				foreach (BuildTeam buildTeam2 in list)
				{
					this.RemoveBuildingQueueData(client, buildTeam2.BuildID, buildTeam2.TaskID);
					LogManager.WriteLog(7, string.Format("领地数据检查RemoveBuildingQueueData, RoleID={0}, RoleName={1}, BuildID={2}, TaskID={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						buildTeam2.BuildID,
						buildTeam2.TaskID
					}), null, true);
				}
				foreach (BuildTeam buildTeam2 in list2)
				{
					if (this.AddBuildingQueueData(client, buildTeam2.BuildID, buildTeam2.TaskID))
					{
						LogManager.WriteLog(7, string.Format("领地数据检查AddBuildingQueueData, RoleID={0}, RoleName={1}, BuildID={2}, TaskID={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							buildTeam2.BuildID,
							buildTeam2.TaskID
						}), null, true);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		public void GeneralBuildingData(GameClient client)
		{
			foreach (KeyValuePair<int, BuildingConfigData> keyValuePair in this.BuildDict)
			{
				if (null == this.GetBuildingData(client, keyValuePair.Value.BuildID))
				{
					BuildingData buildingData = new BuildingData();
					this.RandomBuildTaskData(client, keyValuePair.Value.BuildID, buildingData, false);
					this.UpdateBuildingDataDB(client, buildingData);
					if (null != buildingData)
					{
						client.ClientData.BuildingDataList.Add(buildingData);
					}
				}
			}
		}

		public List<BuildTeam> GetBuildingQueueData(GameClient client)
		{
			List<BuildTeam> list = new List<BuildTeam>();
			string name = "BuildQueueData";
			string roleParamByName = Global.GetRoleParamByName(client, name);
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (1 >= array.Length)
				{
					return list;
				}
				for (int i = 1; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'|'
					});
					if (array2.Length == 3)
					{
						list.Add(new BuildTeam
						{
							_TeamType = (BuildTeamType)Convert.ToInt32(array2[0]),
							BuildID = Convert.ToInt32(array2[1]),
							TaskID = Convert.ToInt32(array2[2])
						});
					}
				}
			}
			return list;
		}

		public BuildingState GetBuildState(GameClient client, int BuildID, int TaskID)
		{
			BuildingTaskConfigData buildingTaskConfigData;
			this.BuildTaskDict.TryGetValue(TaskID, out buildingTaskConfigData);
			if (null == buildingTaskConfigData)
			{
				this.NewBuildTaskDict.TryGetValue(TaskID, out buildingTaskConfigData);
				if (null == buildingTaskConfigData)
				{
					return BuildingState.EBS_Null;
				}
			}
			int i = 0;
			while (i < client.ClientData.BuildingDataList.Count)
			{
				BuildingData buildingData = client.ClientData.BuildingDataList[i];
				if (buildingData.BuildTime != BuildingManager.ConstBuildTime && BuildID == buildingData.BuildId)
				{
					DateTime dateTime;
					DateTime.TryParse(buildingData.BuildTime, out dateTime);
					long num = TimeUtil.NowDateTime().Ticks / 10000L - dateTime.Ticks / 10000L;
					long num2 = num / 1000L;
					long num3 = (long)(buildingTaskConfigData.Time * 60) - num2;
					if (num3 <= 0L)
					{
						return BuildingState.EBS_Finish;
					}
					return BuildingState.EBS_InBuilding;
				}
				else
				{
					i++;
				}
			}
			return BuildingState.EBS_Null;
		}

		public void GetTaskNumInEachTeam(GameClient client, out int free, out int pay)
		{
			free = 0;
			pay = 0;
			List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
			for (int i = 0; i < buildingQueueData.Count; i++)
			{
				if (BuildTeamType.FreeTeam == buildingQueueData[i]._TeamType)
				{
					free++;
				}
				else if (BuildTeamType.PayTeam == buildingQueueData[i]._TeamType)
				{
					pay++;
				}
			}
		}

		public int GetOpenPayTeamNum(GameClient client)
		{
			string name = "BuildQueueData";
			string roleParamByName = Global.GetRoleParamByName(client, name);
			int result;
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (0 == array.Length)
				{
					result = 0;
				}
				else
				{
					result = Convert.ToInt32(array[0]);
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public void SaveBuildingQueueData(GameClient client, List<BuildTeam> BuildQueue)
		{
			string text = "";
			text += this.GetOpenPayTeamNum(client);
			for (int i = 0; i < BuildQueue.Count; i++)
			{
				if (BuildQueue[i].BuildID != 0)
				{
					text += ',';
					text += (int)BuildQueue[i]._TeamType;
					text += '|';
					text += BuildQueue[i].BuildID;
					text += '|';
					text += BuildQueue[i].TaskID;
				}
			}
			string roleParamsKey = "BuildQueueData";
			Global.SaveRoleParamsStringToDB(client, roleParamsKey, text, true);
		}

		public void ModifyOpenPayNum(GameClient client, int chg)
		{
			string text = "BuildQueueData";
			string text2 = Global.GetRoleParamByName(client, text);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "0";
			}
			string[] array = text2.Split(new char[]
			{
				','
			});
			if (0 != array.Length)
			{
				int num = Convert.ToInt32(array[0]) + chg;
				if (num < 0)
				{
					num = 0;
				}
				text2 = Convert.ToString(num);
				for (int i = 1; i < array.Length; i++)
				{
					text2 += ',';
					text2 += array[i];
				}
				Global.SaveRoleParamsStringToDB(client, text, text2, true);
			}
		}

		public bool RemoveBuildingQueueData(GameClient client, int BuildID, int TaskID)
		{
			List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
			int num = -1;
			for (int i = 0; i < buildingQueueData.Count; i++)
			{
				if (BuildID == buildingQueueData[i].BuildID && TaskID == buildingQueueData[i].TaskID)
				{
					num = i;
					buildingQueueData[i].BuildID = 0;
					break;
				}
			}
			bool result;
			if (-1 == num)
			{
				result = false;
			}
			else
			{
				if (BuildTeamType.PayTeam == buildingQueueData[num]._TeamType)
				{
					this.ModifyOpenPayNum(client, -1);
				}
				this.SaveBuildingQueueData(client, buildingQueueData);
				result = true;
			}
			return result;
		}

		public BuildTeamType GetBuildTaskQueueType(GameClient client, int BuildID, int TaskID)
		{
			BuildTeamType result = BuildTeamType.NullTeam;
			List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
			for (int i = 0; i < buildingQueueData.Count; i++)
			{
				if (buildingQueueData[i].BuildID == BuildID && buildingQueueData[i].TaskID == TaskID)
				{
					result = buildingQueueData[i]._TeamType;
					break;
				}
			}
			return result;
		}

		public bool AddBuildingQueueData(GameClient client, int BuildID, int TaskID)
		{
			return this.AddBuildingTaskData(client, BuildID, TaskID);
		}

		public bool AddBuildingTaskData(GameClient client, int BuildID, int TaskID)
		{
			List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
			int num = buildingQueueData.FindIndex((BuildTeam x) => x.BuildID == BuildID);
			bool result;
			if (num >= 0)
			{
				result = false;
			}
			else
			{
				BuildTeam item = new BuildTeam
				{
					BuildID = BuildID,
					TaskID = TaskID
				};
				buildingQueueData.Add(item);
				this.SaveBuildingQueueData(client, buildingQueueData);
				result = true;
			}
			return result;
		}

		public bool CheckAnyTaskFinish(GameClient client)
		{
			bool result = false;
			List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				int buildId = client.ClientData.BuildingDataList[i].BuildId;
				if (!(client.ClientData.BuildingDataList[i].BuildTime == BuildingManager.ConstBuildTime))
				{
					BuildingConfigData buildingConfigData;
					this.BuildDict.TryGetValue(buildId, out buildingConfigData);
					if (null != buildingConfigData)
					{
						int num = 0;
						for (int j = 0; j < buildingQueueData.Count; j++)
						{
							if (buildingQueueData[j].BuildID == buildId)
							{
								num = buildingQueueData[j].TaskID;
								break;
							}
						}
						BuildingTaskConfigData buildingTaskConfigData;
						this.BuildTaskDict.TryGetValue(num, out buildingTaskConfigData);
						if (null != buildingTaskConfigData)
						{
							BuildingState buildState = this.GetBuildState(client, buildId, num);
							if (BuildingState.EBS_Finish == buildState)
							{
								result = true;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		public bool CheckCanGetAnyAllLevelAward(GameClient client)
		{
			bool result = false;
			HashSet<int> hashSet = new HashSet<int>();
			string name = "BuildAllLevAward";
			string roleParamByName = Global.GetRoleParamByName(client, name);
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					hashSet.Add(Global.SafeConvertToInt32(array[i]));
				}
			}
			int num = 0;
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				num += client.ClientData.BuildingDataList[i].BuildLev;
			}
			foreach (KeyValuePair<int, BuildingLevelAwardConfigData> keyValuePair in this.BuildLevelAwardDict)
			{
				if (!hashSet.Contains(keyValuePair.Key))
				{
					if (num >= keyValuePair.Value.AllLevel)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public void BuildTaskFinish(GameClient client, BuildingData BuildData, BuildingConfigData BConfigData, BuildingTaskConfigData TaskConfigData)
		{
			BuildData.BuildTime = BuildingManager.ConstBuildTime;
			BuildingLevelConfigData buildingLevelConfigData;
			this.BuildLevelDict.TryGetValue(new KeyValuePair<int, int>(BuildData.BuildId, BuildData.BuildLev), out buildingLevelConfigData);
			if (null != buildingLevelConfigData)
			{
				int num = (int)(TaskConfigData.ExpNum * buildingLevelConfigData.Exp * (double)TaskConfigData.Time);
				BuildData.BuildExp += num;
				BuildingLevelConfigData buildingLevelConfigData2 = buildingLevelConfigData;
				while (BuildData.BuildLev != BConfigData.MaxLevel)
				{
					if (buildingLevelConfigData2 == null || BuildData.BuildExp < buildingLevelConfigData2.UpNeedExp)
					{
						break;
					}
					BuildData.BuildExp -= buildingLevelConfigData2.UpNeedExp;
					BuildData.BuildLev++;
					this.BuildLevelDict.TryGetValue(new KeyValuePair<int, int>(BuildData.BuildId, BuildData.BuildLev), out buildingLevelConfigData2);
				}
				if (BuildData.BuildLev == BConfigData.MaxLevel)
				{
					BuildData.BuildExp = 0;
				}
				double num2 = (TaskConfigData.SumNum - TaskConfigData.ExpNum) * (double)TaskConfigData.Time;
				this.RandomBuildTaskData(client, BuildData.BuildId, BuildData, false);
				if (buildingLevelConfigData.Money > 0.0)
				{
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, (int)(buildingLevelConfigData.Money * num2), "建造任务完成", true);
				}
				if (buildingLevelConfigData.MoJing > 0.0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)(buildingLevelConfigData.MoJing * num2), "建造任务完成", false, true, false);
				}
				if (buildingLevelConfigData.XingHun > 0.0)
				{
					GameManager.ClientMgr.ModifyStarSoulValue(client, (int)(buildingLevelConfigData.XingHun * num2), "建造任务完成", true, true);
				}
				if (buildingLevelConfigData.ChengJiu > 0.0)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)(buildingLevelConfigData.ChengJiu * num2), "建造任务完成", false, true);
				}
				if (buildingLevelConfigData.ShengWang > 0.0)
				{
					GameManager.ClientMgr.ModifyShengWangValue(client, (int)(buildingLevelConfigData.ShengWang * num2), "建造任务完成", false, true);
				}
				if (buildingLevelConfigData.YuanSu > 0.0)
				{
					GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, (int)(buildingLevelConfigData.YuanSu * num2), "建造任务完成", true, false);
				}
				if (buildingLevelConfigData.YingGuang > 0.0)
				{
					GameManager.FluorescentGemMgr.AddFluorescentPoint(client, (int)(buildingLevelConfigData.YingGuang * num2), "建造任务完成", true);
				}
			}
		}

		public void BuildingLevelUp_GM(GameClient client, int buildID)
		{
			BuildingData buildingData = null;
			for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
			{
				if (client.ClientData.BuildingDataList[i].BuildId == buildID)
				{
					buildingData = client.ClientData.BuildingDataList[i];
					break;
				}
			}
			if (null != buildingData)
			{
				BuildingConfigData buildingConfigData;
				this.BuildDict.TryGetValue(buildingData.BuildId, out buildingConfigData);
				if (null != buildingConfigData)
				{
					if (buildingData.BuildLev != buildingConfigData.MaxLevel)
					{
						buildingData.BuildLev++;
						this.UpdateBuildingDataDB(client, buildingData);
						byte[] buffer = DataHelper.ObjectToBytes<BuildingData>(buildingData);
						GameManager.ClientMgr.SendToClient(client, buffer, 1560);
					}
				}
			}
		}

		public bool ProcessBuildGetListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					BuildingData buildingData = client.ClientData.BuildingDataList[i];
					if (buildingData.BuildTime == BuildingManager.ConstBuildTime)
					{
						this.RandomBuildTaskData(client, buildingData.BuildId, buildingData, true);
						this.UpdateBuildingDataDB(client, buildingData);
					}
				}
				byte[] buffer = DataHelper.ObjectToBytes<List<BuildingData>>(client.ClientData.BuildingDataList);
				GameManager.ClientMgr.SendToClient(client, buffer, nID);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(7))
				{
					return false;
				}
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int num4 = Global.SafeConvertToInt32(cmdParams[2]);
				BuildingData buildingData = null;
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					if (client.ClientData.BuildingDataList[i].BuildId == num3)
					{
						buildingData = client.ClientData.BuildingDataList[i];
						break;
					}
				}
				string cmdData;
				if (null == buildingData)
				{
					num = 12;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						num4,
						-1
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (buildingData.TaskID_1 != num4 && buildingData.TaskID_2 != num4 && buildingData.TaskID_3 != num4 && buildingData.TaskID_4 != num4)
				{
					num = 13;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						num4,
						-1
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (buildingData.BuildTime != BuildingManager.ConstBuildTime)
				{
					num = 6;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						num4,
						-1
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int addType = 0;
				if (buildingData.TaskID_1 == num4)
				{
					if (client.ClientData.NengLiangSmall < 1)
					{
						num = 15;
					}
					addType = 1;
				}
				else if (buildingData.TaskID_2 == num4)
				{
					if (client.ClientData.NengLiangMedium < 1)
					{
						num = 15;
					}
					addType = 2;
				}
				else if (buildingData.TaskID_3 == num4)
				{
					if (client.ClientData.NengLiangBig < 1)
					{
						num = 15;
					}
					addType = 3;
				}
				else if (buildingData.TaskID_4 == num4)
				{
					if (client.ClientData.NengLiangSuper < 1)
					{
						num = 15;
					}
					addType = 4;
				}
				if (num == 15)
				{
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						num4,
						-1
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (!this.AddBuildingQueueData(client, num3, num4))
				{
					num = 5;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						num4,
						-1
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				BuildTeamType buildTeamType = BuildTeamType.FreeTeam;
				this.ModifyNengLiangPointsValue(client, addType, -1, "领地升级启动", true, true);
				buildingData.BuildTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
				this.UpdateBuildingDataDB(client, buildingData);
				this.UpdateBuildingLogDB(client, BuildingLogType.BuildLog_Task);
				this.UpdateBuildingLogDB(client, BuildingLogType.BuildLog_TaskRole);
				if (client._IconStateMgr.CheckBuildingIcon(client, false))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					num2,
					num3,
					num4,
					buildTeamType
				});
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildFinishCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(7))
				{
					return false;
				}
				return false;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildRefreshCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(7))
				{
					return false;
				}
				return false;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildGetAllLevelAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(7))
				{
					return false;
				}
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				BuildingLevelAwardConfigData buildingLevelAwardConfigData = null;
				this.BuildLevelAwardDict.TryGetValue(num3, out buildingLevelAwardConfigData);
				string cmdData;
				if (null == buildingLevelAwardConfigData)
				{
					num = 2;
					cmdData = string.Format("{0}:{1}:{2}", num, num2, num3);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				string text = "BuildAllLevAward";
				string text2 = Global.GetRoleParamByName(client, text);
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array = text2.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						if (num3 == Global.SafeConvertToInt32(array[i]))
						{
							num = 1;
							cmdData = string.Format("{0}:{1}:{2}", num, num2, num3);
							client.sendCmd(nID, cmdData, false);
							return true;
						}
					}
				}
				int num4 = 0;
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					num4 += client.ClientData.BuildingDataList[i].BuildLev;
				}
				if (num4 < buildingLevelAwardConfigData.AllLevel)
				{
					num = 2;
					cmdData = string.Format("{0}:{1}:{2}", num, num2, num3);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				List<GoodsData> list = Global.ConvertToGoodsDataList(buildingLevelAwardConfigData.GoodsList.Items, -1);
				if (!Global.CanAddGoodsDataList(client, list))
				{
					num = 14;
					cmdData = string.Format("{0}:{1}:{2}", num, num2, num3);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				for (int j = 0; j < list.Count; j++)
				{
					GoodsData goodsData = list[j];
					if (null != goodsData)
					{
						goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "获得领地总等级奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
					}
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 += num3;
				}
				else
				{
					text2 += '|';
					text2 += num3;
				}
				Global.SaveRoleParamsStringToDB(client, text, text2, true);
				cmdData = string.Format("{0}:{1}:{2}", num, num2, num3);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildGetAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(7))
				{
					return false;
				}
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int num4 = 0;
				BuildingData buildingData = null;
				for (int i = 0; i < client.ClientData.BuildingDataList.Count; i++)
				{
					if (client.ClientData.BuildingDataList[i].BuildId == num3)
					{
						buildingData = client.ClientData.BuildingDataList[i];
						break;
					}
				}
				string cmdData;
				if (null == buildingData)
				{
					num = 12;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						0,
						0
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				BuildingConfigData buildingConfigData;
				this.BuildDict.TryGetValue(buildingData.BuildId, out buildingConfigData);
				if (null == buildingConfigData)
				{
					num = 12;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						0,
						0
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (buildingData.BuildTime == BuildingManager.ConstBuildTime)
				{
					num = 13;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						0,
						0
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
				for (int i = 0; i < buildingQueueData.Count; i++)
				{
					if (buildingQueueData[i].BuildID == num3)
					{
						num4 = buildingQueueData[i].TaskID;
						break;
					}
				}
				if (0 == num4)
				{
					num = 13;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						0,
						0
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				BuildingTaskConfigData buildingTaskConfigData;
				this.BuildTaskDict.TryGetValue(num4, out buildingTaskConfigData);
				if (null == buildingTaskConfigData)
				{
					this.NewBuildTaskDict.TryGetValue(num4, out buildingTaskConfigData);
					if (null == buildingTaskConfigData)
					{
						num = 13;
						cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							num,
							num2,
							num3,
							0,
							0
						});
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				BuildingState buildState = this.GetBuildState(client, num3, num4);
				if (BuildingState.EBS_Finish != buildState)
				{
					num = 6;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						0,
						0
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (!this.RemoveBuildingQueueData(client, num3, num4))
				{
					num = 13;
					cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						num,
						num2,
						num3,
						buildingData.BuildLev,
						buildingData.BuildExp
					});
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				this.BuildTaskFinish(client, buildingData, buildingConfigData, buildingTaskConfigData);
				this.UpdateBuildingDataDB(client, buildingData);
				byte[] buffer = DataHelper.ObjectToBytes<BuildingData>(buildingData);
				GameManager.ClientMgr.SendToClient(client, buffer, 1560);
				if (client._IconStateMgr.CheckBuildingIcon(client, false))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					num2,
					num3,
					buildingData.BuildLev,
					0
				});
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildOpenQueueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(7))
				{
					return false;
				}
				return false;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildGetQueueCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				string text = string.Format("{0}:{1}", num, num2);
				string name = "BuildQueueData";
				string roleParamByName = Global.GetRoleParamByName(client, name);
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					string[] array = roleParamByName.Split(new char[]
					{
						','
					});
					text += ':';
					text += array[0];
					for (int i = 1; i < array.Length; i++)
					{
						text += ':';
						text += array[i];
					}
				}
				else
				{
					text += ':';
					text += 0;
				}
				client.sendCmd(nID, text, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildGetAllLevelAwardStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				string name = "BuildAllLevAward";
				string text = Global.GetRoleParamByName(client, name);
				if (!string.IsNullOrEmpty(text))
				{
					List<int> list = new List<int>();
					string[] array = text.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(Global.SafeConvertToInt32(array[i]));
					}
					list.Sort(delegate(int left, int right)
					{
						int result;
						if (left < right)
						{
							result = -1;
						}
						else if (left > right)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					text = string.Join<int>("|", list.ToArray());
				}
				string cmdData = string.Format("{0}:{1}:{2}", num, num2, text);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBuildGetStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				string text = string.Format("{0}:{1}", num, num2);
				List<BuildTeam> buildingQueueData = this.GetBuildingQueueData(client);
				for (int i = 0; i < buildingQueueData.Count; i++)
				{
					text += ':';
					text += buildingQueueData[i].BuildID;
					text += '|';
					text += (int)this.GetBuildState(client, buildingQueueData[i].BuildID, buildingQueueData[i].TaskID);
				}
				text += string.Format(":{0}|{1}", 7, YaoSaiJianYuManager.getInstance().GetYaoSaiJianYuState(client.ClientData.RoleID, 0));
				client.sendCmd(nID, text, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ManorQueueNum");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				this.ManorQueueNumMax = Global.SafeConvertToInt32(paramValueByName);
			}
			paramValueByName = GameManager.systemParamsList.GetParamValueByName("ManorFreeQueueNum");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				this.ManorFreeQueueNumMax = Global.SafeConvertToInt32(paramValueByName);
			}
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("ManorQuickFinishNum");
			if (!string.IsNullOrEmpty(paramValueByName2))
			{
				this.ManorQuickFinishNum = Global.SafeConvertToInt32(paramValueByName2);
			}
			string paramValueByName3 = GameManager.systemParamsList.GetParamValueByName("ManorRandomTaskPrice");
			if (!string.IsNullOrEmpty(paramValueByName3))
			{
				this.ManorRandomTaskPrice = Global.SafeConvertToInt32(paramValueByName3);
			}
			string paramValueByName4 = GameManager.systemParamsList.GetParamValueByName("ManorQueuePrice");
			if (!string.IsNullOrEmpty(paramValueByName4))
			{
				this.ManorQueuePrice = Global.SafeConvertToInt32(paramValueByName4);
			}
			return this.LoadBuildFile() && this.LoadBuildTaskFile() && this.LoadBuildLevelFile() && this.LoadBuildLevelAwardFile();
		}

		public bool LoadBuildFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/Build.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						BuildingConfigData buildingConfigData = new BuildingConfigData();
						buildingConfigData.BuildID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						buildingConfigData.MaxLevel = (int)Global.GetSafeAttributeLong(xelement2, "MaxLevel");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "FreeRandomTask");
						if (string.IsNullOrEmpty(safeAttributeStr))
						{
							LogManager.WriteLog(1, string.Format("读取建筑物配置文件中的免费任务配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析建筑物配置文件中的免费任务配置项1失败", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < array.Length; i++)
								{
									string[] array2 = array[i].Split(new char[]
									{
										','
									});
									if (array2.Length >= 2)
									{
										BuildingRandomData buildingRandomData = new BuildingRandomData();
										buildingRandomData.quality = (BuildingQuality)Global.SafeConvertToInt32(array2[0]);
										buildingRandomData.rate = Convert.ToDouble(array2[1]);
										buildingConfigData.FreeRandomList.Add(buildingRandomData);
									}
								}
							}
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "RandomTask");
						if (string.IsNullOrEmpty(safeAttributeStr2))
						{
							LogManager.WriteLog(1, string.Format("读取建筑物配置文件中的任务配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析建筑物配置文件中的任务配置项1失败", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < array.Length; i++)
								{
									string[] array2 = array[i].Split(new char[]
									{
										','
									});
									if (array2.Length >= 2)
									{
										BuildingRandomData buildingRandomData = new BuildingRandomData();
										buildingRandomData.quality = (BuildingQuality)Global.SafeConvertToInt32(array2[0]);
										buildingRandomData.rate = Convert.ToDouble(array2[1]);
										buildingConfigData.RandomList.Add(buildingRandomData);
									}
								}
							}
						}
						this.BuildDict[buildingConfigData.BuildID] = buildingConfigData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Build.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadBuildTaskFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/BuildTask.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				Dictionary<int, List<BuildingTaskConfigData>> dictionary = new Dictionary<int, List<BuildingTaskConfigData>>();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						BuildingTaskConfigData buildingTaskConfigData = new BuildingTaskConfigData();
						buildingTaskConfigData.TaskID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						buildingTaskConfigData.BuildID = (int)Global.GetSafeAttributeLong(xelement2, "BuildID");
						buildingTaskConfigData.quality = (BuildingQuality)Global.GetSafeAttributeLong(xelement2, "Quality");
						buildingTaskConfigData.SumNum = Global.GetSafeAttributeDouble(xelement2, "Sum");
						buildingTaskConfigData.ExpNum = Global.GetSafeAttributeDouble(xelement2, "ExpNum");
						buildingTaskConfigData.Time = (int)Global.GetSafeAttributeLong(xelement2, "Time");
						if (buildingTaskConfigData.TaskID < 1000)
						{
							this.BuildTaskDict[buildingTaskConfigData.TaskID] = buildingTaskConfigData;
						}
						else
						{
							this.NewBuildTaskDict[buildingTaskConfigData.TaskID] = buildingTaskConfigData;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "BuildTask.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadBuildLevelFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/BuildLevel.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						BuildingLevelConfigData buildingLevelConfigData = new BuildingLevelConfigData();
						buildingLevelConfigData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						buildingLevelConfigData.BuildID = (int)Global.GetSafeAttributeLong(xelement2, "BuildID");
						buildingLevelConfigData.Level = (int)Global.GetSafeAttributeLong(xelement2, "Level");
						buildingLevelConfigData.UpNeedExp = (int)Global.GetSafeAttributeLong(xelement2, "UpNeedExp");
						buildingLevelConfigData.Exp = Global.GetSafeAttributeDouble(xelement2, "Exp");
						buildingLevelConfigData.Money = Global.GetSafeAttributeDouble(xelement2, "Money");
						buildingLevelConfigData.MoJing = Global.GetSafeAttributeDouble(xelement2, "MoJing");
						buildingLevelConfigData.XingHun = Global.GetSafeAttributeDouble(xelement2, "XingHun");
						buildingLevelConfigData.ChengJiu = Global.GetSafeAttributeDouble(xelement2, "ChengJiu");
						buildingLevelConfigData.ShengWang = Global.GetSafeAttributeDouble(xelement2, "ShengWang");
						buildingLevelConfigData.YuanSu = Global.GetSafeAttributeDouble(xelement2, "YuanSu");
						buildingLevelConfigData.YingGuang = Global.GetSafeAttributeDouble(xelement2, "YingGuang");
						this.BuildLevelDict.Add(new KeyValuePair<int, int>(buildingLevelConfigData.BuildID, buildingLevelConfigData.Level), buildingLevelConfigData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "BuildTask.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadBuildLevelAwardFile()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Manor/BuildLevelAward.xml"));
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					BuildingLevelAwardConfigData buildingLevelAwardConfigData = new BuildingLevelAwardConfigData();
					buildingLevelAwardConfigData.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					buildingLevelAwardConfigData.AllLevel = (int)Global.GetSafeAttributeLong(xml, "AllLevel");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Award");
					if (string.IsNullOrEmpty(safeAttributeStr))
					{
						LogManager.WriteLog(1, string.Format("读取建筑物总等级奖励配置项1失败", new object[0]), null, true);
					}
					else
					{
						ConfigParser.ParseAwardsItemList(safeAttributeStr, ref buildingLevelAwardConfigData.GoodsList, '|', ',');
					}
					this.BuildLevelAwardDict[buildingLevelAwardConfigData.ID] = buildingLevelAwardConfigData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "BuildLevelAward.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void ModifyNengLiangPointsValue(GameClient client, int addType, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (addValue != 0 && addType >= 1 && addType <= 4)
			{
				switch (addType)
				{
				case 1:
					client.ClientData.NengLiangSmall += addValue;
					client.ClientData.NengLiangSmall = Math.Max(client.ClientData.NengLiangSmall, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_小", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangSmall, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10168", client.ClientData.NengLiangSmall, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangSmall, (long)addValue, (long)client.ClientData.NengLiangSmall, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangSmall, client.ClientData.NengLiangSmall);
					}
					break;
				case 2:
					client.ClientData.NengLiangMedium += addValue;
					client.ClientData.NengLiangMedium = Math.Max(client.ClientData.NengLiangMedium, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_中", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangMedium, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10169", client.ClientData.NengLiangMedium, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangMedium, (long)addValue, (long)client.ClientData.NengLiangMedium, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangMedium, client.ClientData.NengLiangMedium);
					}
					break;
				case 3:
					client.ClientData.NengLiangBig += addValue;
					client.ClientData.NengLiangBig = Math.Max(client.ClientData.NengLiangBig, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_大", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangBig, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10170", client.ClientData.NengLiangBig, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangBig, (long)addValue, (long)client.ClientData.NengLiangBig, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangBig, client.ClientData.NengLiangBig);
					}
					break;
				case 4:
					client.ClientData.NengLiangSuper += addValue;
					client.ClientData.NengLiangSuper = Math.Max(client.ClientData.NengLiangSuper, 0);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "能量核心_超", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.NengLiangSuper, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10171", client.ClientData.NengLiangSuper, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.NengLiangSuper, (long)addValue, (long)client.ClientData.NengLiangSuper, strFrom);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.NengLiangSuper, client.ClientData.NengLiangSuper);
					}
					break;
				}
			}
		}

		private const string Build_fileName = "Config/Manor/Build.xml";

		private const string BuildTask_fileName = "Config/Manor/BuildTask.xml";

		private const string BuildLevel_fileName = "Config/Manor/BuildLevel.xml";

		private const string BuildLevelAward_fileName = "Config/Manor/BuildLevelAward.xml";

		private const int NewBuildTaskIDBind = 1000;

		public static readonly string ConstBuildTime = "0000-00-00 00:00:00";

		public object RandomTaskMutex = new object();

		private static BuildingManager instance = new BuildingManager();

		protected Dictionary<int, BuildingConfigData> BuildDict = new Dictionary<int, BuildingConfigData>();

		protected Dictionary<int, BuildingTaskConfigData> BuildTaskDict = new Dictionary<int, BuildingTaskConfigData>();

		protected Dictionary<KeyValuePair<int, int>, BuildingLevelConfigData> BuildLevelDict = new Dictionary<KeyValuePair<int, int>, BuildingLevelConfigData>();

		protected Dictionary<int, BuildingLevelAwardConfigData> BuildLevelAwardDict = new Dictionary<int, BuildingLevelAwardConfigData>();

		protected Dictionary<int, BuildingTaskConfigData> NewBuildTaskDict = new Dictionary<int, BuildingTaskConfigData>();

		public int ManorFreeQueueNumMax = 0;

		public int ManorQueueNumMax = 0;

		public int ManorQuickFinishNum = 0;

		public int ManorRandomTaskPrice = 0;

		public int ManorQueuePrice = 0;
	}
}
