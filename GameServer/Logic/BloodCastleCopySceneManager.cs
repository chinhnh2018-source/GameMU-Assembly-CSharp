using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class BloodCastleCopySceneManager
	{
		public void InitBloodCastleCopyScene()
		{
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.BloodCastle);
		}

		public void LoadBloodCastleListScenes()
		{
		}

		public void SetBloodCastleCopySceneTotalPoint(string sName, int nPoint)
		{
			this.m_sTotalPointName = sName;
			this.m_nTotalPointValue = nPoint;
		}

		public bool CanEnterExistCopyScene(GameClient client)
		{
			CopyMap copyMap = null;
			int fuBenSeqID = client.ClientData.FuBenSeqID;
			lock (this.m_BloodCastleCopyScenesList)
			{
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(fuBenSeqID, out copyMap))
				{
					return false;
				}
			}
			bool result;
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dictionary = null;
				BloodCastleScene bloodCastleScene = null;
				int fubenMapID = copyMap.FubenMapID;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(fubenMapID, out dictionary))
				{
					result = false;
				}
				else if (!dictionary.TryGetValue(fuBenSeqID, out bloodCastleScene))
				{
					result = false;
				}
				else if (bloodCastleScene.m_eStatus != BloodCastleStatus.FIGHT_STATUS_BEGIN)
				{
					result = false;
				}
				else
				{
					result = bloodCastleScene.CantiansRole(client);
				}
			}
			return result;
		}

		public void AddBloodCastleCopyScenes(int nSequenceID, int nFubenID, int nMapCodeID, CopyMap mapInfo)
		{
			lock (this.m_BloodCastleCopyScenesList)
			{
				CopyMap copyMap = null;
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(nSequenceID, out copyMap) || copyMap == null)
				{
					this.m_BloodCastleCopyScenesList.Add(nSequenceID, mapInfo);
				}
			}
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dictionary = null;
				BloodCastleScene bloodCastleScene = null;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dictionary))
				{
					dictionary = new Dictionary<int, BloodCastleScene>();
					this.m_BloodCastleCopyScenesInfo.Add(nFubenID, dictionary);
				}
				if (!dictionary.TryGetValue(nSequenceID, out bloodCastleScene))
				{
					bloodCastleScene = new BloodCastleScene();
					dictionary.Add(nSequenceID, bloodCastleScene);
					bloodCastleScene.CleanAllInfo();
				}
				bloodCastleScene.m_nMapCode = nMapCodeID;
				bloodCastleScene.m_CopyMap = mapInfo;
			}
		}

		public void RemoveBloodCastleListCopyScenes(CopyMap cmInfo, int nSqeID, int nCopyID)
		{
			lock (this.m_BloodCastleCopyScenesList)
			{
				CopyMap copyMap = null;
				if (this.m_BloodCastleCopyScenesList.TryGetValue(nSqeID, out copyMap) && copyMap != null)
				{
					this.m_BloodCastleCopyScenesList.Remove(nSqeID);
				}
			}
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dictionary = null;
				if (this.m_BloodCastleCopyScenesInfo.TryGetValue(nCopyID, out dictionary) && dictionary != null)
				{
					BloodCastleScene bloodCastleScene = null;
					if (dictionary.TryGetValue(nSqeID, out bloodCastleScene) && bloodCastleScene != null)
					{
						dictionary.Remove(nSqeID);
					}
					if (dictionary.Count <= 0)
					{
						this.m_BloodCastleCopyScenesInfo.Remove(nCopyID);
					}
				}
			}
		}

		public int CheckBloodCastleListScenes(int nFuBenMapID)
		{
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dictionary = null;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFuBenMapID, out dictionary))
				{
					return -1;
				}
				if (dictionary == null)
				{
					return -1;
				}
				BloodCastleDataInfo bloodCastleDataInfo = null;
				if (!Data.BloodCastleDataInfoList.TryGetValue(nFuBenMapID, out bloodCastleDataInfo))
				{
					return -1;
				}
				if (bloodCastleDataInfo == null)
				{
					return -1;
				}
				foreach (KeyValuePair<int, BloodCastleScene> keyValuePair in dictionary)
				{
					int key = keyValuePair.Key;
					BloodCastleScene value = keyValuePair.Value;
					if (key >= 0 && value != null)
					{
						if (key == nFuBenMapID && value.m_nPlarerCount < bloodCastleDataInfo.MaxEnterNum && value.m_eStatus < BloodCastleStatus.FIGHT_STATUS_BEGIN)
						{
							return key;
						}
					}
				}
			}
			return -1;
		}

		public bool IsBloodCastleCopyScene(int nFuBenMapID)
		{
			return Data.BloodCastleDataInfoList.ContainsKey(nFuBenMapID);
		}

		public bool IsBloodCastleCopyScene2(int nMpaCodeID)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(nMpaCodeID);
			return mapSceneType == 5;
		}

		public CopyMap GetBloodCastleCopySceneInfo(int nSequenceID)
		{
			CopyMap result;
			if (nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				CopyMap copyMap = null;
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(nSequenceID, out copyMap))
				{
					result = null;
				}
				else
				{
					result = copyMap;
				}
			}
			return result;
		}

		public BloodCastleScene GetBloodCastleCopySceneDataInfo(CopyMap cmInfo, int nSequenceID, int nFuBenID)
		{
			BloodCastleScene result;
			if (cmInfo == null || nSequenceID < 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, BloodCastleScene> dictionary = null;
				if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFuBenID, out dictionary) || dictionary == null)
				{
					result = null;
				}
				else
				{
					BloodCastleScene bloodCastleScene = null;
					if (!dictionary.TryGetValue(nSequenceID, out bloodCastleScene) || bloodCastleScene == null)
					{
						result = null;
					}
					else
					{
						result = bloodCastleScene;
					}
				}
			}
			return result;
		}

		public int EnterBloodCastSceneCopySceneCount(GameClient client, int nFubenID, out int nBloodNum)
		{
			nBloodNum = -1;
			BloodCastleDataInfo bloodCastleDataInfo = null;
			int result;
			if (!Data.BloodCastleDataInfoList.TryGetValue(nFubenID, out bloodCastleDataInfo))
			{
				result = -1;
			}
			else
			{
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				int nType = 1;
				int num = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, dayOfYear, nType);
				nBloodNum = num;
				if (num >= bloodCastleDataInfo.MaxEnterNum)
				{
					bool flag = true;
					int dayOfYear2 = TimeUtil.NowDateTime().DayOfYear;
					int vipLevel = client.ClientData.VipLevel;
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterBloodCastleCountAddValue", ',');
					if (vipLevel > 0 && paramValueIntArrayByName != null && paramValueIntArrayByName[vipLevel] > 0)
					{
						int num2 = paramValueIntArrayByName[vipLevel];
						if (num < bloodCastleDataInfo.MaxEnterNum + num2)
						{
							Global.UpdateVipDailyData(client, dayOfYear2, 1000001);
							flag = false;
						}
					}
					if (flag)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(15, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						return -1;
					}
				}
				result = 1;
			}
			return result;
		}

		public void SendMegToClient(GameClient client, int nFubenID, int nSquID, int nCmdID)
		{
			CopyMap copyMap = null;
			lock (this.m_BloodCastleCopyScenesList)
			{
				if (!this.m_BloodCastleCopyScenesList.TryGetValue(nSquID, out copyMap) || copyMap == null)
				{
					return;
				}
			}
			long num = TimeUtil.NOW();
			lock (this.m_BloodCastleCopyScenesInfo)
			{
				Dictionary<int, BloodCastleScene> dictionary = null;
				if (this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dictionary) && dictionary != null)
				{
					BloodCastleScene bloodCastleScene = null;
					if (dictionary.TryGetValue(nSquID, out bloodCastleScene) && bloodCastleScene != null)
					{
						BloodCastleDataInfo bloodCastleDataInfo = null;
						if (Data.BloodCastleDataInfoList.TryGetValue(nFubenID, out bloodCastleDataInfo) && bloodCastleDataInfo != null)
						{
							if (nCmdID == 545)
							{
								if (bloodCastleScene.m_eStatus <= BloodCastleStatus.FIGHT_STATUS_PREPARE)
								{
									GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, nCmdID, 0, 0, 0, bloodCastleScene.m_nPlarerCount, null);
								}
							}
							else if (nCmdID == 531)
							{
								if (bloodCastleScene.m_lPrepareTime > 0L)
								{
									int num2 = (int)(((long)(bloodCastleDataInfo.PrepareTime * 1000) - (num - bloodCastleScene.m_lPrepareTime)) / 1000L);
									string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
									{
										nFubenID,
										num2,
										bloodCastleDataInfo.NeedKillMonster1Num,
										1,
										bloodCastleDataInfo.NeedKillMonster2Num,
										1,
										1,
										1
									});
									GameManager.ClientMgr.SendToClient(client, strCmd, 531);
									GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 545, 0, 0, 0, bloodCastleScene.m_nPlarerCount, client);
									if (bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
									{
										num2 = (int)(((long)(bloodCastleDataInfo.DurationTime * 1000) - (num - bloodCastleScene.m_lBeginTime)) / 1000L);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 517, num2, 0, 0, bloodCastleScene.m_nPlarerCount, client);
									}
								}
							}
						}
					}
				}
			}
		}

		public int EnterBloodCastSceneCopyScene(GameClient client, int nFubenID, int nBloodNum, out int nSeqID, int mapCode)
		{
			nSeqID = -1;
			if (client.ClientData.BloodCastleAwardPoint > 0)
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BloodCastleFuBenSeqID");
				int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
				int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneFinishFlag");
				int num = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, roleParamsInt32FromDB, roleParamsInt32FromDB2);
				if (num == 0)
				{
					BloodCastleDataInfo bloodCastleDataInfo = null;
					if (Data.BloodCastleDataInfoList.TryGetValue(roleParamsInt32FromDB2, out bloodCastleDataInfo))
					{
						if (bloodCastleDataInfo == null)
						{
							client.ClientData.BloodCastleAwardPoint = 0;
							return 1;
						}
						string text = null;
						string text2 = null;
						for (int i = 0; i < bloodCastleDataInfo.AwardItem2.Length; i++)
						{
							text2 += bloodCastleDataInfo.AwardItem2[i];
							if (i != bloodCastleDataInfo.AwardItem2.Length - 1)
							{
								text2 += "|";
							}
						}
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							-1,
							roleParamsInt32FromDB3,
							client.ClientData.BloodCastleAwardPoint,
							Global.CalcExpForRoleScore(client.ClientData.BloodCastleAwardPoint, bloodCastleDataInfo.ExpModulus),
							client.ClientData.BloodCastleAwardPoint * bloodCastleDataInfo.MoneyModulus,
							text,
							text2
						});
						GameManager.ClientMgr.SendToClient(client, strCmd, 519);
						return -1;
					}
				}
				else
				{
					client.ClientData.BloodCastleAwardPoint = 0;
					Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
				}
			}
			int bloodCastleCopySceneIDForRole = Global.GetBloodCastleCopySceneIDForRole(client);
			int result;
			if (bloodCastleCopySceneIDForRole <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(16, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
				LogManager.WriteLog(2, string.Format("enter bloodcastle scene fail!! get scene info fail!!!!", new object[0]), null, true);
				result = -1;
			}
			else
			{
				BloodCastleDataInfo bloodCastleDataInfo2 = null;
				if (!Data.BloodCastleDataInfoList.TryGetValue(bloodCastleCopySceneIDForRole, out bloodCastleDataInfo2) || bloodCastleDataInfo2 == null)
				{
					result = -1;
				}
				else if (!Global.CanEnterBloodCastleOnTime(bloodCastleDataInfo2.BeginTime, bloodCastleDataInfo2.PrepareTime))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(17, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = -1;
				}
				else
				{
					GoodsData goodsByID = Global.GetGoodsByID(client, bloodCastleDataInfo2.NeedGoodsID);
					if (goodsByID == null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(18, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else if (goodsByID.GCount < bloodCastleDataInfo2.NeedGoodsNum)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(19, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = -1;
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bloodCastleDataInfo2.NeedGoodsID, 1, false, out flag, out flag2, false))
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(20, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							result = -1;
						}
						else
						{
							Dictionary<int, BloodCastleScene> dictionary = null;
							lock (this.m_BloodCastleCopyScenesInfo)
							{
								if (this.m_BloodCastleCopyScenesInfo.TryGetValue(bloodCastleCopySceneIDForRole, out dictionary) && dictionary != null)
								{
									foreach (KeyValuePair<int, BloodCastleScene> keyValuePair in dictionary)
									{
										if (keyValuePair.Value.m_eStatus < BloodCastleStatus.FIGHT_STATUS_BEGIN)
										{
											if (keyValuePair.Value.m_nPlarerCount < bloodCastleDataInfo2.MaxPlayerNum)
											{
												keyValuePair.Value.m_nPlarerCount++;
												nSeqID = keyValuePair.Key;
											}
										}
									}
								}
								if (nSeqID < 0)
								{
									string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
									if (array != null && array.Length >= 2)
									{
										nSeqID = Global.SafeConvertToInt32(array[1]);
										if (nSeqID > 0)
										{
											BloodCastleScene bloodCastleScene = null;
											if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(nFubenID, out dictionary) || dictionary == null)
											{
												dictionary = new Dictionary<int, BloodCastleScene>();
												this.m_BloodCastleCopyScenesInfo.Add(nFubenID, dictionary);
											}
											if (!dictionary.TryGetValue(nSeqID, out bloodCastleScene) || bloodCastleScene == null)
											{
												bloodCastleScene = new BloodCastleScene();
												bloodCastleScene.CleanAllInfo();
												bloodCastleScene.m_nMapCode = mapCode;
												bloodCastleScene.m_nPlarerCount = 1;
												dictionary[nSeqID] = bloodCastleScene;
											}
										}
									}
								}
							}
							Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleFuBenSeqID", nSeqID, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneid", bloodCastleCopySceneIDForRole, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneFinishFlag", 0, true);
							result = 0;
						}
					}
				}
			}
			return result;
		}

		public void HeartBeatBloodCastScene()
		{
			long num = TimeUtil.NOW();
			if (Math.Abs(num - BloodCastleCopySceneManager.LastHeartBeatTicks) >= 1000L)
			{
				BloodCastleCopySceneManager.LastHeartBeatTicks = num;
				HashSet<int> hashSet = new HashSet<int>();
				lock (this.m_BloodCastleCopyScenesList)
				{
					CopyMap copyMap = null;
					foreach (KeyValuePair<int, CopyMap> keyValuePair in this.m_BloodCastleCopyScenesList)
					{
						int fuBenSeqID = keyValuePair.Value.FuBenSeqID;
						int fubenMapID = keyValuePair.Value.FubenMapID;
						int num2 = -1;
						num2 = keyValuePair.Value.MapCode;
						if (fuBenSeqID >= 0 && fubenMapID >= 0 && num2 >= 0)
						{
							copyMap = keyValuePair.Value;
							lock (this.m_BloodCastleCopyScenesInfo)
							{
								BloodCastleDataInfo bloodCastleDataInfo = null;
								if (Data.BloodCastleDataInfoList.TryGetValue(fubenMapID, out bloodCastleDataInfo) && bloodCastleDataInfo != null)
								{
									Dictionary<int, BloodCastleScene> dictionary = null;
									if (this.m_BloodCastleCopyScenesInfo.TryGetValue(fubenMapID, out dictionary) && dictionary != null)
									{
										BloodCastleScene bloodCastleScene = null;
										if (dictionary.TryGetValue(fuBenSeqID, out bloodCastleScene) && bloodCastleScene != null)
										{
											long num3 = TimeUtil.NOW();
											if (bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_NULL)
											{
												int num4 = 0;
												string s = null;
												if (Global.CanEnterBloodCastleCopySceneOnTime(bloodCastleDataInfo.BeginTime, bloodCastleDataInfo.PrepareTime + bloodCastleDataInfo.DurationTime, out num4, out s))
												{
													bloodCastleScene.m_eStatus = BloodCastleStatus.FIGHT_STATUS_PREPARE;
													DateTime dateTime = DateTime.Parse(s);
													bloodCastleScene.m_lPrepareTime = dateTime.Ticks / 10000L;
													List<GameClient> clientsList = keyValuePair.Value.GetClientsList();
													if (null == clientsList)
													{
														return;
													}
													for (int i = 0; i < clientsList.Count; i++)
													{
														this.SendMegToClient(clientsList[i], fubenMapID, fuBenSeqID, 531);
													}
												}
											}
											else if (bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_PREPARE)
											{
												if (num3 >= bloodCastleScene.m_lPrepareTime + (long)(bloodCastleDataInfo.PrepareTime * 1000))
												{
													bloodCastleScene.m_Step++;
													GameManager.CopyMapMgr.AddGuangMuEvent(bloodCastleScene.m_CopyMap, 1, 0);
													bloodCastleScene.m_eStatus = BloodCastleStatus.FIGHT_STATUS_BEGIN;
													bloodCastleScene.m_lBeginTime = TimeUtil.NOW();
													int nTimer = (int)(((long)(bloodCastleDataInfo.DurationTime * 1000) - (num3 - bloodCastleScene.m_lBeginTime)) / 1000L);
													List<GameClient> clientsList2 = bloodCastleScene.m_CopyMap.GetClientsList();
													if (null != clientsList2)
													{
														foreach (GameClient gameClient in clientsList2)
														{
															if (bloodCastleScene.AddRole(gameClient))
															{
																Global.UpdateRoleEnterActivityCount(gameClient, SpecialActivityTypes.BloodCastle);
															}
														}
													}
													GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, keyValuePair.Value, 517, nTimer, 0, 0, 0, null);
													int gateID = bloodCastleDataInfo.GateID;
													string[] array = bloodCastleDataInfo.GatePos.Split(new char[]
													{
														','
													});
													int value = Global.SafeConvertToInt32(array[0]);
													int value2 = Global.SafeConvertToInt32(array[1]);
													GameMap gameMap = null;
													if (!GameManager.MapMgr.DictMaps.TryGetValue(bloodCastleScene.m_nMapCode, out gameMap))
													{
														LogManager.WriteLog(2, string.Format("血色城堡报错 地图配置 ID = {0}", bloodCastleDataInfo.MapCode), null, true);
														return;
													}
													int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
													int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
													GameManager.MonsterZoneMgr.AddDynamicMonsters(num2, gateID, keyValuePair.Value.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
													GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, keyValuePair.Value, 533, 0, 0, 1, 0, null);
												}
											}
											else if (bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
											{
												if (num3 < bloodCastleScene.m_lBeginTime + 20000L)
												{
													List<GameClient> clientsList2 = bloodCastleScene.m_CopyMap.GetClientsList();
													if (null != clientsList2)
													{
														foreach (GameClient gameClient in clientsList2)
														{
															if (bloodCastleScene.AddRole(gameClient))
															{
																LogManager.WriteLog(2, string.Format("EnterCount#BloodCastle#rid={0}", gameClient.ClientData.RoleID), null, true);
																Global.UpdateRoleEnterActivityCount(gameClient, SpecialActivityTypes.BloodCastle);
															}
														}
													}
												}
												if (num3 >= bloodCastleScene.m_lBeginTime + (long)(bloodCastleDataInfo.DurationTime * 1000))
												{
													bloodCastleScene.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
													bloodCastleScene.m_lEndTime = TimeUtil.NOW();
													try
													{
														string text = string.Format("血色城堡已结束,是否完成{0},结束时间{1},m_bIsFinishTask:{2},m_nKillMonsterACount:{3},m_bKillMonsterAStatus:{4},m_nRoleID:{5}", new object[]
														{
															bloodCastleScene.m_bIsFinishTask,
															new DateTime(bloodCastleScene.m_lEndTime * 10000L),
															bloodCastleScene.m_bIsFinishTask,
															bloodCastleScene.m_nKillMonsterACount,
															bloodCastleScene.m_bKillMonsterAStatus,
															bloodCastleScene.m_nRoleID
														});
														LogManager.WriteLog(2, text, null, true);
													}
													catch
													{
													}
												}
												hashSet.Add(copyMap.MapCode);
											}
											else if (bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_END)
											{
												int nTimer = (int)(((long)(bloodCastleDataInfo.LeaveTime * 1000) - (num3 - bloodCastleScene.m_lEndTime)) / 1000L);
												if (!bloodCastleScene.m_bEndFlag)
												{
													long num5 = bloodCastleScene.m_lEndTime - bloodCastleScene.m_lBeginTime;
													long num6 = ((long)(bloodCastleDataInfo.DurationTime * 1000) - num5) / 1000L;
													if (num6 >= (long)bloodCastleDataInfo.DurationTime)
													{
														num6 = (long)(bloodCastleDataInfo.DurationTime / 2);
													}
													int num7 = (int)((long)bloodCastleDataInfo.TimeModulus * num6);
													if (num7 < 0)
													{
														num7 = 0;
													}
													GameManager.ClientMgr.NotifyBloodCastleCopySceneMsgEndFight(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, keyValuePair.Value, bloodCastleScene, 519, nTimer, num7);
												}
											}
										}
									}
								}
							}
						}
					}
					foreach (int mapCode in hashSet)
					{
						GameManager.MonsterZoneMgr.ReloadCopyMapMonsters(mapCode, -1);
					}
				}
			}
		}

		public void CreateMonsterBBloodCastScene(int mapid, BloodCastleDataInfo bcDataTmp, BloodCastleScene bcTmp, int nCopyMapID)
		{
			int needKillMonster2ID = bcDataTmp.NeedKillMonster2ID;
			string[] array = bcDataTmp.NeedCreateMonster2Pos.Split(new char[]
			{
				','
			});
			int value = Global.SafeConvertToInt32(array[0]);
			int value2 = Global.SafeConvertToInt32(array[1]);
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(bcTmp.m_nMapCode, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode), null, true);
			}
			else
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
				int radius = gameMap.CorrectPointToGrid(bcDataTmp.NeedCreateMonster2Radius);
				for (int i = 0; i < bcDataTmp.NeedCreateMonster2Num; i++)
				{
					GameManager.MonsterZoneMgr.AddDynamicMonsters(mapid, needKillMonster2ID, nCopyMapID, 1, gridX, gridY, radius, bcDataTmp.NeedCreateMonster2PursuitRadius, 0, null, null);
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsBloodCastleCopyScene(client.ClientData.FuBenID))
			{
				BloodCastleDataInfo bloodCastleDataInfo = null;
				if (Data.BloodCastleDataInfoList.TryGetValue(client.ClientData.FuBenID, out bloodCastleDataInfo) && bloodCastleDataInfo != null)
				{
					Dictionary<int, BloodCastleScene> dictionary = null;
					if (this.m_BloodCastleCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dictionary) && dictionary != null)
					{
						BloodCastleScene bloodCastleScene = null;
						if (dictionary.TryGetValue(client.ClientData.FuBenSeqID, out bloodCastleScene) && bloodCastleScene != null)
						{
							CopyMap copyMap = null;
							if (this.m_BloodCastleCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) && copyMap != null)
							{
								if (!bloodCastleScene.m_bEndFlag)
								{
									this.SendMegToClient(client, client.ClientData.FuBenID, client.ClientData.FuBenSeqID, 531);
									string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BloodCastleAwardPoint);
									GameManager.ClientMgr.SendToClient(client, strCmd, 532);
									if (bloodCastleScene.m_Step != 0)
									{
										if (bloodCastleScene.m_Step == 1)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, bloodCastleScene.m_nKillMonsterACount, 1, 0, null);
										}
										else if (bloodCastleScene.m_Step == 2)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 2, 0, null);
										}
										else if (bloodCastleScene.m_Step == 3)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, bloodCastleScene.m_nKillMonsterBCount, 3, 0, null);
										}
										else if (bloodCastleScene.m_Step == 4)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 4, 0, null);
										}
										else if (!bloodCastleScene.m_bIsFinishTask)
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 5, 0, null);
										}
										else
										{
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 1, 5, 0, null);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void KillMonsterABloodCastCopyScene(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsBloodCastleCopyScene(client.ClientData.FuBenID))
			{
				BloodCastleDataInfo bloodCastleDataInfo = null;
				if (Data.BloodCastleDataInfoList.TryGetValue(client.ClientData.FuBenID, out bloodCastleDataInfo) && bloodCastleDataInfo != null)
				{
					Dictionary<int, BloodCastleScene> dictionary = null;
					if (this.m_BloodCastleCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dictionary) && dictionary != null)
					{
						BloodCastleScene bloodCastleScene = null;
						if (dictionary.TryGetValue(client.ClientData.FuBenSeqID, out bloodCastleScene) && bloodCastleScene != null)
						{
							CopyMap copyMap = null;
							if (this.m_BloodCastleCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) && copyMap != null)
							{
								if (!bloodCastleScene.m_bEndFlag && bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
								{
									if (bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
									{
										client.ClientData.BloodCastleAwardPoint += monster.MonsterInfo.BloodCastJiFen;
									}
									client.ClientData.BloodCastleAwardPointTmp = client.ClientData.BloodCastleAwardPoint;
									Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, false);
									string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BloodCastleAwardPoint);
									GameManager.ClientMgr.SendToClient(client, strCmd, 532);
									if (monster.MonsterInfo.VLevel >= bloodCastleDataInfo.NeedKillMonster1Level && bloodCastleScene.m_bKillMonsterAStatus == 0)
									{
										int num = Interlocked.Increment(ref bloodCastleScene.m_nKillMonsterACount);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, bloodCastleScene.m_nKillMonsterACount, 1, 0, null);
										if (num == bloodCastleDataInfo.NeedKillMonster1Num)
										{
											bloodCastleScene.m_Step++;
											GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, 2, 0);
											GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, 22, 2);
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 518, 0, 0, 0, 0, null);
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 2, 0, null);
											bloodCastleScene.m_bKillMonsterAStatus = 1;
										}
									}
									if (monster.MonsterInfo.ExtensionID == bloodCastleDataInfo.GateID)
									{
										bloodCastleScene.m_Step++;
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, 3, 0);
										this.CreateMonsterBBloodCastScene(bloodCastleScene.m_nMapCode, bloodCastleDataInfo, bloodCastleScene, client.ClientData.CopyMapID);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 1, 2, 0, null);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 3, 0, null);
									}
									if (monster.MonsterInfo.ExtensionID == bloodCastleDataInfo.NeedKillMonster2ID && bloodCastleScene.m_bKillMonsterBStatus == 0)
									{
										int num = Interlocked.Increment(ref bloodCastleScene.m_nKillMonsterBCount);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, bloodCastleScene.m_nKillMonsterBCount, 3, 0, null);
										if (num == bloodCastleDataInfo.NeedKillMonster2Num)
										{
											bloodCastleScene.m_Step++;
											int monsterID = bloodCastleDataInfo.CrystalID;
											string[] array = bloodCastleDataInfo.CrystalPos.Split(new char[]
											{
												','
											});
											int value = Global.SafeConvertToInt32(array[0]);
											int value2 = Global.SafeConvertToInt32(array[1]);
											GameMap gameMap = null;
											if (!GameManager.MapMgr.DictMaps.TryGetValue(bloodCastleScene.m_nMapCode, out gameMap))
											{
												LogManager.WriteLog(2, string.Format("血色城堡报错 地图配置 ID = {0}", bloodCastleDataInfo.MapCode), null, true);
												return;
											}
											int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
											int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
											GameManager.MonsterZoneMgr.AddDynamicMonsters(bloodCastleScene.m_nMapCode, monsterID, copyMap.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
											GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 4, 0, null);
											bloodCastleScene.m_bKillMonsterBStatus = 1;
										}
									}
									if (monster.MonsterInfo.ExtensionID == bloodCastleDataInfo.CrystalID)
									{
										bloodCastleScene.m_Step++;
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 1, 4, 0, null);
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 0, 5, 0, null);
									}
									if (monster.MonsterInfo.ExtensionID == bloodCastleDataInfo.DiaoXiangID)
									{
										bloodCastleScene.m_Step++;
										GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 533, 0, 1, 5, 0, null);
										this.CompleteBloodcastleAndGiveAwards(client, bloodCastleScene, bloodCastleDataInfo);
									}
									if (monster.MonsterInfo.ExtensionID == bloodCastleDataInfo.CrystalID)
									{
										int monsterID = bloodCastleDataInfo.DiaoXiangID;
										string[] array = bloodCastleDataInfo.DiaoXiangPos.Split(new char[]
										{
											','
										});
										int value = Global.SafeConvertToInt32(array[0]);
										int value2 = Global.SafeConvertToInt32(array[1]);
										GameMap gameMap = null;
										if (GameManager.MapMgr.DictMaps.TryGetValue(bloodCastleScene.m_nMapCode, out gameMap))
										{
											int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
											int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
											GameManager.MonsterZoneMgr.AddDynamicMonsters(bloodCastleScene.m_nMapCode, monsterID, copyMap.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void GiveAwardBloodCastCopyScene(GameClient client, int nMultiple)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BloodCastleFuBenSeqID");
			int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
			int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneFinishFlag");
			int num = GameManager.CopyMapMgr.FindAwardState(client.ClientData.RoleID, roleParamsInt32FromDB, roleParamsInt32FromDB2);
			if (num > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(21, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
			else
			{
				BloodCastleDataInfo bloodCastleDataInfo = null;
				if (Data.BloodCastleDataInfoList.TryGetValue(roleParamsInt32FromDB2, out bloodCastleDataInfo))
				{
					if (roleParamsInt32FromDB3 == 1)
					{
						string[] awardItem = bloodCastleDataInfo.AwardItem2;
						if (awardItem != null && awardItem.Length > 0)
						{
							for (int i = 0; i < awardItem.Length; i++)
							{
								if (!string.IsNullOrEmpty(awardItem[i].Trim()))
								{
									string[] array = awardItem[i].Split(new char[]
									{
										','
									});
									if (!string.IsNullOrEmpty(array[i].Trim()))
									{
										int goodsID = Convert.ToInt32(array[0].Trim());
										int num2 = Convert.ToInt32(array[1].Trim());
										int binding = Convert.ToInt32(array[2].Trim());
										GoodsData goodsData = new GoodsData
										{
											Id = -1,
											GoodsID = goodsID,
											Using = 0,
											Forge_level = 0,
											Starttime = "1900-01-01 12:00:00",
											Endtime = "1900-01-01 12:00:00",
											Site = 0,
											Quality = 0,
											Props = "",
											GCount = num2,
											Binding = binding,
											Jewellist = "",
											BagIndex = 0,
											AddPropIndex = 0,
											BornIndex = 0,
											Lucky = 0,
											Strong = 0,
											ExcellenceInfo = 0,
											AppendPropLev = 0,
											ChangeLifeLevForEquip = 0
										};
										string goodsFromWhere = "血色堡垒奖励--统一奖励";
										if (!Global.CanAddGoodsNum(client, num2))
										{
											Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(1, new object[0]), GLang.GetLang(2602, new object[0]), 1.0);
										}
										else
										{
											Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, num2, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, goodsFromWhere, goodsData.Endtime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
										}
									}
								}
							}
						}
					}
					if (client.ClientData.BloodCastleAwardPoint > 0)
					{
						long num3 = (long)nMultiple * Global.CalcExpForRoleScore(client.ClientData.BloodCastleAwardPoint, bloodCastleDataInfo.ExpModulus);
						int num4 = client.ClientData.BloodCastleAwardPoint * bloodCastleDataInfo.MoneyModulus;
						if (num3 > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, num3, false, true, false, "none");
							GameManager.ClientMgr.NotifyAddExpMsg(client, num3);
						}
						if (num4 > 0)
						{
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num4, "血色城堡副本", false);
							GameManager.ClientMgr.NotifyAddJinBiMsg(client, num4);
						}
						if (client.ClientData.BloodCastleAwardPoint > client.ClientData.BloodCastleAwardTotalPoint)
						{
							client.ClientData.BloodCastleAwardTotalPoint = client.ClientData.BloodCastleAwardPoint;
						}
						if (client.ClientData.BloodCastleAwardPoint > this.m_nTotalPointValue)
						{
							this.SetBloodCastleCopySceneTotalPoint(client.ClientData.RoleName, client.ClientData.BloodCastleAwardPoint);
						}
						client.ClientData.BloodCastleAwardPoint = 0;
						Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
					}
					GameManager.CopyMapMgr.AddAwardState(client.ClientData.RoleID, roleParamsInt32FromDB, roleParamsInt32FromDB2, 1);
					ProcessTask.ProcessAddTaskVal(client, TaskTypes.BloodCastle, -1, 1, new object[0]);
				}
			}
		}

		public void LeaveBloodCastCopyScene(GameClient client, bool clearScore = false)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneid");
			if (client.ClientData.CopyMapID >= 0 && client.ClientData.FuBenSeqID >= 0 && this.IsBloodCastleCopyScene(roleParamsInt32FromDB))
			{
				CopyMap copyMap = null;
				lock (this.m_BloodCastleCopyScenesList)
				{
					if (!this.m_BloodCastleCopyScenesList.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) || copyMap == null)
					{
						return;
					}
				}
				Dictionary<int, BloodCastleScene> dictionary = null;
				lock (this.m_BloodCastleCopyScenesInfo)
				{
					if (!this.m_BloodCastleCopyScenesInfo.TryGetValue(client.ClientData.FuBenID, out dictionary) || dictionary == null)
					{
						return;
					}
					BloodCastleScene bloodCastleScene = null;
					if (!dictionary.TryGetValue(client.ClientData.FuBenSeqID, out bloodCastleScene) || bloodCastleScene == null)
					{
						return;
					}
					Interlocked.Decrement(ref bloodCastleScene.m_nPlarerCount);
					GameManager.ClientMgr.NotifyBloodCastleCopySceneMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, copyMap, 545, 0, 0, 0, bloodCastleScene.m_nPlarerCount, null);
					if (clearScore && bloodCastleScene.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
					{
						client.ClientData.BloodCastleAwardPoint = 0;
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
			}
		}

		public void LogOutWhenInBloodCastleCopyScene(GameClient client)
		{
			this.LeaveBloodCastCopyScene(client, false);
		}

		public void CompleteBloodCastScene(GameClient client, BloodCastleScene bsInfo, BloodCastleDataInfo bsData)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BloodCastleSceneFinishFlag");
			if (roleParamsInt32FromDB != 1)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneFinishFlag", 1, true);
			}
		}

		public void CompleteBloodcastleAndGiveAwards(GameClient client, BloodCastleScene bcTmp, BloodCastleDataInfo bcDataTmp)
		{
			CopyMap bloodCastleCopySceneInfo = GameManager.BloodCastleCopySceneMgr.GetBloodCastleCopySceneInfo(client.ClientData.FuBenSeqID);
			if (bloodCastleCopySceneInfo != null)
			{
				if (bcTmp.m_eStatus != BloodCastleStatus.FIGHT_STATUS_END)
				{
					bcTmp.m_nRoleID = client.ClientData.RoleID;
					string[] awardItem = bcDataTmp.AwardItem1;
					if (awardItem != null && awardItem.Length > 0)
					{
						for (int i = 0; i < awardItem.Length; i++)
						{
							if (!string.IsNullOrEmpty(awardItem[i].Trim()))
							{
								string[] array = awardItem[i].Split(new char[]
								{
									','
								});
								if (!string.IsNullOrEmpty(array[i].Trim()))
								{
									int goodsID = Convert.ToInt32(array[0].Trim());
									int num = Convert.ToInt32(array[1].Trim());
									int binding = Convert.ToInt32(array[2].Trim());
									GoodsData goodsData = new GoodsData
									{
										Id = -1,
										GoodsID = goodsID,
										Using = 0,
										Forge_level = 0,
										Starttime = "1900-01-01 12:00:00",
										Endtime = "1900-01-01 12:00:00",
										Site = 0,
										Quality = 0,
										Props = "",
										GCount = num,
										Binding = binding,
										Jewellist = "",
										BagIndex = 0,
										AddPropIndex = 0,
										BornIndex = 0,
										Lucky = 0,
										Strong = 0,
										ExcellenceInfo = 0,
										AppendPropLev = 0,
										ChangeLifeLevForEquip = 0
									};
									string lang = GLang.GetLang(23, new object[0]);
									if (!Global.CanAddGoodsNum(client, num))
									{
										Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(1, new object[0]), lang, 1.0);
									}
									else
									{
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, num, 0, "", 0, goodsData.Binding, 0, "", true, 1, lang, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
									}
								}
							}
						}
					}
					bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
					bcTmp.m_lEndTime = TimeUtil.NOW();
					bcTmp.m_bIsFinishTask = true;
					this.CompleteBloodCastScene(client, bcTmp, bcDataTmp);
				}
			}
		}

		public void CleanBloodCastScene(int mapid)
		{
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(this.m_sTotalPointName) && this.m_sTotalPointName == oldName)
				{
					this.m_sTotalPointName = newName;
				}
			}
		}

		public Dictionary<int, CopyMap> m_BloodCastleCopyScenesList = new Dictionary<int, CopyMap>();

		public Dictionary<int, Dictionary<int, BloodCastleScene>> m_BloodCastleCopyScenesInfo = new Dictionary<int, Dictionary<int, BloodCastleScene>>();

		public static object m_Mutex = new object();

		public int m_nTotalPointValue = -1;

		public string m_sTotalPointName = "";

		private static long LastHeartBeatTicks = 0L;
	}
}
